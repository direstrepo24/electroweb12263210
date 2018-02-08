using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotVVM.Framework.Controls;
using Electro.model.datatakemodel;
using Electro.model.Models.datatakemodel;
using Electro.model.Repository;
using electroweb.Model_Response;
using Microsoft.AspNetCore.Hosting;

using DotVVM.BusinessPack.Controls;
using DotVVM.BusinessPack.Export.Csv;
using Electro.model.DataContext;
using System.Globalization;

namespace electroweb.ViewModels
{
    public class ViewModelReporteGeneral:BaseViewModel
    {

        #region Atributes
      
        private readonly IDepartamentoRepository  _departamentoRepository;
        private readonly ICiudadRepository _ciudadRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly ICiudad_EmpresaRepository _ciudad_EmpresaRepository;
        private readonly ILongitudElementoRepository _longitudElementoRepository;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;



        #endregion

        #region Events
        #endregion

        #region Properties
       

        public List<Departamento>  Departamentos {get; set;}

        
        public Departamento SelectedDeparment { get; set; }

        public List<Ciudad>  Ciudades {get; set;}=new List<Ciudad>();

        public Ciudad SelectedCiudad { get; set; }


        public List<ResponseCiudadEmpresa>  Empresas {get; set;}= new List<ResponseCiudadEmpresa>();

        public ResponseCiudadEmpresa SelectedEmpresa { get; set; }



        public ReportGeneralViewModel SelectedReportGeneralViewModel {get; set;}
      


        public bool Limpiar { get; set; }=true;

    
        public long Empresa_Id { get; set; }
        public long Ciudad_Id { get; set; }
        public long Departamento_Id { get; set; }


        public long TotalInventarioElemento { get; set; }
        public long TotalInventarioOcupacines { get; set; }

        public string ValorAnualRecaudoTotal {get; set;}

           public string ValorMensualRecaudoTotal {get; set;}

         public bool IsModalDisplayed { get; set; }

         
     



        #endregion

        #region Contructor
        public ViewModelReporteGeneral(
        IMapper mapper,
        IHostingEnvironment hostingEnvironment,
        IDepartamentoRepository departamentoRepository,
        ICiudadRepository ciudadRepository,
        ICiudad_EmpresaRepository ciudad_EmpresaRepository,
        IEmpresaRepository empresaRepository,
        ILongitudElementoRepository longitudElementoRepository
        ){
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;

            _departamentoRepository= departamentoRepository;
            _ciudadRepository= ciudadRepository;
            _empresaRepository= empresaRepository;
            _ciudad_EmpresaRepository= ciudad_EmpresaRepository;
            _longitudElementoRepository= longitudElementoRepository;




            LoadInit();
        }

        

        #endregion

        #region Events

        

        public async  void Consultar()
        {
            try{
         
            await InitLoadReport();
          

            }catch(Exception ex){
                  throw new Exception("Exception Occured While Printing", ex);
            }
        }



        public void DepartmentChange()
        {
            SelectedCiudad=null;
            SelectedEmpresa=null;
            Ciudades.Clear();
            Empresas.Clear();
            ValorAnualRecaudoTotal=string.Empty;
            ValorMensualRecaudoTotal=string.Empty;

            Ciudad_Id=-1;
            Empresa_Id=-1;
            if(SelectedDeparment!=null) {
                Departamento_Id= SelectedDeparment.Id;
                Ciudades = GetQueryableCiudades(Departamento_Id).Result.ToList();
                Ciudades.Add(new Ciudad{
                    Id=0,
                    Nombre="Todos los municipios",
                });
                
            }else{
                Ciudad_Id=-1;
                Empresa_Id=-1;
                Departamento_Id=-1;
            }  
            Limpiar=true;

           /// InitElementos();
        }


        public void CiudadChange()
        {
          //  IsVisible=true;
            SelectedEmpresa=null;
            Empresas.Clear();
            ValorAnualRecaudoTotal=string.Empty;
              ValorMensualRecaudoTotal=string.Empty;
            Empresa_Id=-1;
            if(SelectedCiudad!=null) {
              Ciudad_Id= SelectedCiudad.Id;
              Empresas = GetQueryableEmpresasByCiudad(Ciudad_Id).Result.ToList();
              Empresas.Add(new ResponseCiudadEmpresa{
                    Id=0,
                    Empresa_Id=0,
                    Empresa= new ResponseEmpresa{Id=0,Nombre="Todas las empresas"},
               });
            }else{
                Ciudad_Id=-1;
                Empresa_Id=-1;
                Departamento_Id=-1;
            }

             Limpiar=true;
            //InitElementos();
        }

        public void EmpresaChange()
        {
         // IsVisible=true;
           ValorAnualRecaudoTotal=string.Empty;
             ValorMensualRecaudoTotal=string.Empty;
            if(SelectedEmpresa!=null) {
               Empresa_Id= SelectedEmpresa.Empresa.Id;
              if(Empresa_Id==0){
                   //VisibleRadioButtonDetallado=true;
                    
              }else{
                  
                  // VisibleRadioButtonDetallado=false;
              }
              
            } else{
               Empresa_Id= -1;
            }
            Limpiar=true;
        }


        #endregion

        #region Methods

        private  void LoadInit()
        {
            var listDepartamnets= GetQueryableDepartament().Result;
            //var listEstados = GetQueryableElementos().Result;
            Departamentos= listDepartamnets.ToList();
            Departamento_Id=-1;
        }
        private async Task<IQueryable<ResponseCiudadEmpresa>> GetQueryableEmpresasByCiudad(long ciudad_id)
        {
            var listEmpresas= await _ciudad_EmpresaRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==ciudad_id, b=>b.Empresa,c=>c.Ciudad);
            var viewModel = _mapper.Map<IEnumerable<Ciudad_Empresa>, IEnumerable<ResponseCiudadEmpresa>>(listEmpresas);
            return  viewModel.AsQueryable();
        }
        private async Task<IQueryable<Ciudad>> GetQueryableCiudades(long departamento_id)
        {
            var ciudades= await _ciudadRepository.AllIncludingAsyncWhere(a=>a.departmentoId==departamento_id);
            return  ciudades.AsQueryable();
        }
        private async Task<IQueryable<Departamento>> GetQueryableDepartament()
        {
            var departamentos= await _departamentoRepository.AllIncludingAsync();
            return  departamentos.AsQueryable();
        }

        #endregion






        public BpGridViewDataSet<ReportGeneralViewModel> Report { get; set; }

        public async Task InitLoadReport()
        {
            Report = new BpGridViewDataSet<ReportGeneralViewModel> {
                OnLoadingData = GetData
            };
            Report.SetSortExpression(nameof(ReportGeneralViewModel.Operador));  
            //return base.Init();
        }

        public GridViewDataSetLoadedData<ReportGeneralViewModel> GetData(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
          //  var queryable = GetQueryable(15);
            var queryable = GetQueryableApoyosAllApoyos(Ciudad_Id, Empresa_Id).Result;
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }

        private async Task<IQueryable<ReportGeneralViewModel>> GetQueryableApoyosAllApoyos(long Ciudad_Id, long Empresa_Id)
        {
            //IEnumerable<ReportGeneralViewModel> allCables=Enumerable.Empty<ReportGeneralViewModel>();

            var allCables=new List<View_Cable_Report>();

            var listEmpresasReport=new List<ReportGeneralViewModel>();

            var dataContext = MyAppContext.GetInstance();

            var longitudElementos=await _longitudElementoRepository.GetAllAsync();

            Decimal TotaRecaudo=0; 


            if(Empresa_Id==0){
                    allCables= dataContext.View_Cables.Where(a=>a.Ciudad_Id== Ciudad_Id).ToList();
            }else{
                    allCables= dataContext.View_Cables.Where(a=>a.Ciudad_Id== Ciudad_Id && a.Empresa_Id==Empresa_Id).ToList();
            }




            var allApoyos= allCables.GroupBy(a=>a.Elemento_Id).ToList();

            
            TotalInventarioElemento=allApoyos.Count;
            TotalInventarioOcupacines =allCables.AsEnumerable().Sum(a=>a.Cantidad_Cable);


            CantidadLongitudPostes= new List<CantidadLongitudPostesViewModel>();
             foreach(var itemlongitud in longitudElementos){
                 CantidadLongitudPostes.Add(new CantidadLongitudPostesViewModel{
                     Longitud_Elemento= itemlongitud.Valor,
                     Total_Postes= allApoyos.Where(a=>a.FirstOrDefault().Longitud_Elemento_Id==itemlongitud.Id).Count()

                 });
             }

            OcupacionesLongitudList= new List<ValorLongitudViewModel>();
            foreach(var item in longitudElementos){
                     var cantidadOcupacionesLongitud= allCables.Where(a=>a.Longitud_Elemento_Id==item.Id).AsEnumerable().Sum(a=>a.Cantidad_Cable);
                      OcupacionesLongitudList.Add(new ValorLongitudViewModel{
                     Longitud_Elemento= item.Valor,
                     Total_Ocupaciones=cantidadOcupacionesLongitud
                 });
              }


            var groupEmpresas= allCables.GroupBy(a=>a.Empresa_Id).ToList();

            foreach(var item in groupEmpresas){

                var Apoyos_Empresa= allApoyos.Count;

                long OcupacionesOperador= 0;


                decimal  percentaje = 0m;

               
               



                if(Empresa_Id==0){
                    Apoyos_Empresa= allCables.Where(a=>a.Empresa_Id==item.FirstOrDefault().Empresa_Id).GroupBy(a=>a.Elemento_Id).Count();
                    OcupacionesOperador=allCables.Where(a=>a.Empresa_Id==item.FirstOrDefault().Empresa_Id).AsEnumerable().Sum(a=>a.Cantidad_Cable);
                    
                }else{
                    OcupacionesOperador=TotalInventarioOcupacines;
                }


             

                
                 // percentaje= (Apoyos_Empresa*100)/allApoyos.Count;
                 // valor_item = double.Parse(porcentaje, System.Globalization.CultureInfo.InvariantCulture);

                 Decimal SumaTotalRecaudo=0;
                 var ValorAnualNorma=string.Empty;
                 Decimal Recaudo_Longitud=0;

                 long Longitud_Elemento=0;

                 var listValorLongitudes= new List<ValorLongitudViewModel>();


                 foreach(var itemlongitud in longitudElementos){
                     var cantidadOcupacionesLongitud= allCables.Where(a=>a.Empresa_Id==item.FirstOrDefault().Empresa_Id && a.Longitud_Elemento_Id==itemlongitud.Id).AsEnumerable().Sum(a=>a.Cantidad_Cable);
                      if(itemlongitud.Valor==6){
                           SumaTotalRecaudo+=cantidadOcupacionesLongitud*37806;
                           ValorAnualNorma=string.Format("$ {0}",37.806);
                           Recaudo_Longitud=cantidadOcupacionesLongitud*37806;
                           Longitud_Elemento=6;
                      }else if(itemlongitud.Valor==8){
                           SumaTotalRecaudo+=cantidadOcupacionesLongitud*37806;
                             ValorAnualNorma=string.Format("$ {0}",37.806);
                              Recaudo_Longitud=cantidadOcupacionesLongitud*37806;
                                 Longitud_Elemento=8;
                      }
                      else if(itemlongitud.Valor==10){
                           SumaTotalRecaudo+=cantidadOcupacionesLongitud*42531;
                             ValorAnualNorma=string.Format("$ {0}",42.531);
                              Recaudo_Longitud=cantidadOcupacionesLongitud*42531;
                               Longitud_Elemento=10;
                      }else if(itemlongitud.Valor==12){
                           SumaTotalRecaudo+=cantidadOcupacionesLongitud*47256;
                             ValorAnualNorma=string.Format("$ {0}",47.256);
                              Recaudo_Longitud=cantidadOcupacionesLongitud*47256;
                                 Longitud_Elemento=12;
                      }else if(itemlongitud.Valor==14 ){
                           SumaTotalRecaudo+=cantidadOcupacionesLongitud*73596;
                            ValorAnualNorma=string.Format("$ {0}",73.596);
                               Recaudo_Longitud=cantidadOcupacionesLongitud*73596;
                                  Longitud_Elemento=14;                      }
                      else if(itemlongitud.Valor==16 ){
                            SumaTotalRecaudo+=cantidadOcupacionesLongitud*73596;
                            ValorAnualNorma=string.Format("$ {0}",73.596);
                            Recaudo_Longitud=cantidadOcupacionesLongitud*73596;
                            Longitud_Elemento=16;
                      }


                      listValorLongitudes.Add(new ValorLongitudViewModel{
                          ValorAnualNorma=ValorAnualNorma,
                          Recaudo_Longitud=string.Format("$ {0:N2}", Recaudo_Longitud),
                          Longitud_Elemento=Longitud_Elemento,
                          Total_Ocupaciones=cantidadOcupacionesLongitud

                      });
                 }


                TotaRecaudo+=SumaTotalRecaudo;
             

                percentaje=Decimal.Divide((OcupacionesOperador*100),TotalInventarioOcupacines);

                string valor_percentaje = string.Format("{0:N2} %", percentaje);

                string valor_recaudo_anual = string.Format("$ {0:N2}", SumaTotalRecaudo);

                 string valor_recaudo_menusal = string.Format("$ {0:N2}", SumaTotalRecaudo/12);
            

                listEmpresasReport.Add(new ReportGeneralViewModel{
                    Operador=item.FirstOrDefault().Nombre_Empresa,
                    Apoyos= Apoyos_Empresa,
                    Ocupaciones= OcupacionesOperador,
                    Porcentaje=valor_percentaje,
                    RecaudoAnual=valor_recaudo_anual,
                    RecaudoMensual=valor_recaudo_menusal,
                    ValorLongitudes=listValorLongitudes
                });
            }

            ValorAnualRecaudoTotal=string.Format("$ {0:N2}", TotaRecaudo);
            ValorMensualRecaudoTotal=string.Format("$ {0:N2}", TotaRecaudo/12);

           /* var numbers = new List<ReportGeneralViewModel>();
            for (var i = 0; i < size; i++)
            {
                 numbers.Add(new ReportGeneralViewModel { Operador = i + "Claro", Apoyos = i + 1, Ocupaciones = i ,Porcentaje=20});
            }*/
              //  numbers.Add(new ReportGeneralViewModel { Operador = i + "Claro", Apoyos = $"ReportGeneralViewModel {i + 1}", Ocupaciones = i });
            await InitLoadPostesLongitud();
            await InitLoadOcupacionesLongitud();
            return listEmpresasReport.AsQueryable();
        }


         public async void ViewModalDisplay(ReportGeneralViewModel reportGeneralViewModel)
        {
           
            SelectedReportGeneralViewModel=reportGeneralViewModel;
             IsModalDisplayed=true;
             await InitLoadByLongitud();
           

        }




        ///Ocupaciones por longitud

        
        public BpGridViewDataSet<ValorLongitudViewModel> ListReportLongitud { get; set; }

        public async Task InitLoadByLongitud()
        {
            ListReportLongitud = new BpGridViewDataSet<ValorLongitudViewModel> {
                OnLoadingData = GetDataByLongitud
            };
            ListReportLongitud.SetSortExpression(nameof(ValorLongitudViewModel.Longitud_Elemento));  
            //return base.Init();
        }

        public GridViewDataSetLoadedData<ValorLongitudViewModel> GetDataByLongitud(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
          //  var queryable = GetQueryable(15);
            var queryable = GetQueryableDetailListLongitudes();
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }

        private  IQueryable<ValorLongitudViewModel> GetQueryableDetailListLongitudes()
        {
             return SelectedReportGeneralViewModel.ValorLongitudes.AsQueryable();
        }



        //POSTES LONGITUD
        public List<CantidadLongitudPostesViewModel> CantidadLongitudPostes {get; set;}
        public BpGridViewDataSet<CantidadLongitudPostesViewModel> PostesLongitud { get; set; }

        public  async Task InitLoadPostesLongitud()
        {
            PostesLongitud = new BpGridViewDataSet<CantidadLongitudPostesViewModel> {
                OnLoadingData = GetDataLongitud
            };
            PostesLongitud.SetSortExpression(nameof(CantidadLongitudPostesViewModel.Longitud_Elemento));

           
        }

        public GridViewDataSetLoadedData<CantidadLongitudPostesViewModel> GetDataLongitud(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
            var queryable = GetQueryablePostesLongitud();
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }

        private IQueryable<CantidadLongitudPostesViewModel> GetQueryablePostesLongitud()
        {
           return CantidadLongitudPostes.AsQueryable();
        }

        //Oupaciones Longitud Poste
        public List<ValorLongitudViewModel> OcupacionesLongitudList {get; set;}
        public BpGridViewDataSet<ValorLongitudViewModel> OcupacionesLongitudGrid { get; set; }
        public  async Task InitLoadOcupacionesLongitud()
        {
            OcupacionesLongitudGrid = new BpGridViewDataSet<ValorLongitudViewModel> {
                OnLoadingData = GetDataOcupacionesLongitud
            };
            OcupacionesLongitudGrid.SetSortExpression(nameof(ValorLongitudViewModel.Longitud_Elemento));
        }

        public GridViewDataSetLoadedData<ValorLongitudViewModel> GetDataOcupacionesLongitud(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
            var queryable = GetQueryableOcupacionesLongitudGeneral();
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }

        private IQueryable<ValorLongitudViewModel> GetQueryableOcupacionesLongitudGeneral()
        {
           return OcupacionesLongitudList.AsQueryable();
        }

    }






    public class ReportGeneralViewModel{
        public string Operador {get; set;}
        public int Apoyos {get; set;}
        public long Ocupaciones {get; set;}
        public string Porcentaje {get; set; }

        public string RecaudoAnual {get; set; }

        public string RecaudoMensual {get; set; }

     


        public List<ValorLongitudViewModel> ValorLongitudes {get; set;}
     

    }


     public class ValorLongitudViewModel{
        public string ValorAnualNorma {get; set;}
        public double Longitud_Elemento {get; set;}
        public long Total_Ocupaciones {get; set;}
        public string Recaudo_Longitud {get; set; }
    }


     public class CantidadLongitudPostesViewModel{
        public double Longitud_Elemento {get; set;}
        public long Total_Postes {get; set;}
      
    }

}