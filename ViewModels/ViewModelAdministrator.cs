using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotVVM.BusinessPack.Controls;
using DotVVM.Framework.Controls;
using Electro.model.DataContext;
using Electro.model.datatakemodel;
using Electro.model.Models.datatakemodel;
using Electro.model.Repository;
using electroweb.DTO;
using Microsoft.AspNetCore.Hosting;

namespace electroweb.ViewModels
{
    public class ViewModelAdministrator:BaseViewModel
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

          public List<ViewModelFilterType>  FilterTypes {get; set;}=new List< ViewModelFilterType>();
           public ViewModelFilterType SelectedFilterType { get; set; }


        

        public Ciudad SelectedCiudad { get; set; }


        


        public ReportGeneralViewModel SelectedReportGeneralViewModel {get; set;}
      


        public bool Limpiar { get; set; }=true;

    
        public long Empresa_Id { get; set; }
        public long Ciudad_Id { get; set; }
        public long Departamento_Id { get; set; }


        public bool Is_Visible_By_Ciudad {get; set; }=false;
        public bool Is_Visible_By_Numero_Apoyo {get; set; }=false;

         public int PageSize { get; set; } = 5;


         
     



        #endregion

        #region Contructor
        public ViewModelAdministrator(
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

        

        public   void Consultar()
        {
            try{

               
         
           // await InitLoadReport();
          

            }catch(Exception ex){
                  throw new Exception("Exception Occured While Printing", ex);
            }
        }


        public   void FiltroChange()
        {
            try{
                 if(SelectedFilterType.Id==3){
                    Is_Visible_By_Ciudad=true;
                    Is_Visible_By_Numero_Apoyo=false;

                }else{
                    Is_Visible_By_Ciudad=false;
                    Is_Visible_By_Numero_Apoyo=true;
                }

                
         
           // await InitLoadReport();
          

            }catch(Exception ex){
                  throw new Exception("Exception Occured While Printing", ex);
            }
        }



        public void DepartmentChange()
        {
            SelectedCiudad=null;
          //  SelectedEmpresa=null;
            Ciudades.Clear();
        //    Empresas.Clear();
            

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
          //  SelectedEmpresa=null;
           // Empresas.Clear();
             
            Empresa_Id=-1;
            if(SelectedCiudad!=null) {
              Ciudad_Id= SelectedCiudad.Id;
           
            }else{
                Ciudad_Id=-1;
                Empresa_Id=-1;
                Departamento_Id=-1;
            }

             Limpiar=true;
            //InitElementos();
        }

     

        #endregion

        #region Methods

        private async  void LoadInit()
        {

            var listDepartamnets= GetQueryableDepartament().Result;
            //var listEstados = GetQueryableElementos().Result;
            Departamentos= listDepartamnets.ToList();
            Departamento_Id=-1;



            //Tipo Filtros
            FilterTypes.Add(new ViewModelFilterType{
                Id=1,
                Nombre="Buscar por numero apoyo"
            });

            FilterTypes.Add(new ViewModelFilterType{
                Id=2,
                Nombre="Buscar por codigo apoyo"
            });

            FilterTypes.Add(new ViewModelFilterType{
                Id=3,
                Nombre="Buscar por ciudad"
            });

            await InitLoadElementos();
         


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

        private async Task<IQueryable<ElementoReportViewModel>> GetQueryableElementos(){
            //IEnumerable<ElementoCable> cables=Enumerable.Empty<ElementoCable>();
            var dataContext = MyAppContext.GetInstance();
            IEnumerable<View_Elemento_Report> elementos=Enumerable.Empty<View_Elemento_Report>();

            elementos = dataContext.View_Elementos.ToList();
             var viewModelMap = _mapper.Map<IEnumerable<View_Elemento_Report>,IEnumerable<ElementoReportViewModel>>(elementos);
                //cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Empresa.Ciudad.departmento.Id==Departamento_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, f=>f.Ciudad_Empresa.Ciudad.departmento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
           return viewModelMap.AsQueryable();

        }

      


        #endregion



        #region UI ELEMENTS
         ///CONFIG GRID VIEW

        public BpGridViewDataSet<ElementoReportViewModel> ListElementosGridView { get; set; }

        public async Task InitLoadElementos()
        {
            ListElementosGridView = new BpGridViewDataSet<ElementoReportViewModel> {
                OnLoadingData = GetDataElementos
            };
            ListElementosGridView.SetSortExpression(nameof(ElementoReportViewModel.Elemento_Id));  
             ListElementosGridView.PagingOptions.PageSize = PageSize;
            //return base.Init();
        }

        public GridViewDataSetLoadedData<ElementoReportViewModel> GetDataElementos(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
          //  var queryable = GetQueryable(15);
            var queryable = GetQueryableElementos().Result;
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }







        #endregion

       
    }





    public class ViewModelFilterType
    {
        public string Nombre {get; set;}
        public long Id {get; set;}


    }

   
}