using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using AutoMapper;
using Electro.model.datatakemodel;
using Electro.model.Models.datatakemodel;
using Electro.model.Repository;
using electroweb.DTO;
using electroweb.Model_Response;
using electroweb.Reports;
using Microsoft.AspNetCore.Hosting;

using DotVVM.BusinessPack.Controls;
using DotVVM.BusinessPack.Export.Csv;
using DotVVM.Framework.Controls;
using electroweb.Reports.MasterReports;
using Electro.model.DataContext;

namespace electroweb.ViewModels
{
    public class ViewModelReportNovedades:BaseViewModel
    {
         #region Atributes
        private ITipoNovedadRepository _tipoNovedadRepository;
        private IDetalleTipoNovedadRepository _detalleTipoNovedadRepository;
       
        private readonly IElementoRepository  _IelementosRepository;
        private readonly IDepartamentoRepository  _departamentoRepository;
        private readonly ICiudadRepository _ciudadRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly ICiudad_EmpresaRepository _ciudad_EmpresaRepository;
        private readonly IElementoCableRepository _elementoCableRepository;
        private readonly IEquipoElementoRepository _equipoElementoRepository;

        private readonly INovedadRepository _novedadRepository;
        private readonly IFotoRepository _fotoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;

        #endregion

        #region Events
        #endregion

        #region Properties
        public List<ElementoViewModel> ListElementos{get;set;}

        public static  List<ElementoReportViewModel> ReportElementos{get;set;}

        public List<Departamento>  Departamentos {get; set;}

      
        public Departamento SelectedDeparment { get; set; }

        public List<Ciudad>  Ciudades {get; set;}=new List<Ciudad>();

        public Ciudad SelectedCiudad { get; set; }

        public BpGridViewDataSet<ElementoReportViewModel> Elementos { get; set; }
        public List<TipoNovedad> ListTipoNovedad {get; set;}= new List<TipoNovedad>();
        public TipoNovedad SelectTipoNovedad { get; set; }
        public DetalleTipoNovedad SelectDetalleTipoNovedad { get; set; }

        public List<DetalleTipoNovedad> ListDetalleTipoNovedad {get; set;}= new List<DetalleTipoNovedad>();

        public Usuario SelectedUsuario { get; set; }
        public GridViewUserSettings UserSettings { get; set; }

        public bool Limpiar { get; set; }=true;

        public long Empresa_Id { get; set; }
        public long Ciudad_Id { get; set; }
        public long Departamento_Id { get; set; }

        public long Tipo_Novedad_Id {get; set;}
        public long Detalle_Tipo_Novedad_Id {get; set;}
        public string ReportType { get; set; }="General";
        public bool VisibleRadioButtonDetallado { get; set; }=true;
        public bool IsVisibleExportPdf { get; set; }=false;
        #endregion

        #region Contructor
        public ViewModelReportNovedades(
        IElementoRepository elementoRepository,
        IMapper mapper,
        IHostingEnvironment hostingEnvironment,
        IDepartamentoRepository departamentoRepository,
        ICiudadRepository ciudadRepository,
        ICiudad_EmpresaRepository ciudad_EmpresaRepository,
        IEmpresaRepository empresaRepository,
        IElementoCableRepository elementoCableRepository,
        IEquipoElementoRepository equipoElementoRepository,
        INovedadRepository novedadRepository,
        IFotoRepository fotoRepository,
        IUsuarioRepository usuarioRepository,
        ITipoNovedadRepository tipoNovedadRepository,
        IDetalleTipoNovedadRepository detalleTipoNovedadRepository
        ){

            _IelementosRepository= elementoRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;

            _departamentoRepository= departamentoRepository;
            _ciudadRepository= ciudadRepository;
            _empresaRepository= empresaRepository;
            _ciudad_EmpresaRepository= ciudad_EmpresaRepository;
            _elementoCableRepository= elementoCableRepository;
            _equipoElementoRepository=equipoElementoRepository;
            _fotoRepository= fotoRepository;
            _novedadRepository=novedadRepository;
            _usuarioRepository= usuarioRepository;
            _tipoNovedadRepository= tipoNovedadRepository;
            _detalleTipoNovedadRepository= detalleTipoNovedadRepository;

            //Services
           // Departamentos= new List<Departamento>();
            //Ciudades= new List<Ciudad>();
            //Empresas= new List<ResponseCiudadEmpresa>();
            //ReportElementos= new List<ElementoViewModel>();
            //SelectedDateStart= DateTime.Now;
            //SelectedDateEnd= DateTime.Now;
            LoadInit();
        }

        

        #endregion

        #region Events
         public  void ReportTypeChange()
        {

            if(ReportType=="General"){
              
                IsVisibleExportPdf=false;
                ReportType="General";
                Limpiar=true;
                if(ReportElementos!=null){
                    ReportElementos.Clear();
                }
               
                InitElementos();
            }else{
                /* 
            Empresa_Id=0;

            if(SelectedEmpresa!=null){
                SelectedEmpresa.Empresa.Nombre="Todas las empresas";
                SelectedEmpresa.Empresa.Id=0;
                SelectedEmpresa.Id=0;
                SelectedEmpresa.Empresa_Id=0;
            }else{
                SelectedEmpresa= new ResponseCiudadEmpresa{
                    Id=0,
                    Empresa_Id=0,
                    Empresa= new ResponseEmpresa{Id=0,Nombre="Todas las empresas"},

               };
            }
            */

            IsVisibleExportPdf=false;
            ReportType="Detallado";
            if(ReportElementos!=null){
                    ReportElementos.Clear();
            }
            

            Limpiar=true;
            InitElementos();
            }
        }

        
        public void TipoNovedadChange()
        {
            if(SelectTipoNovedad!=null) {
                Tipo_Novedad_Id= SelectTipoNovedad.Id;
                var list= GetQueryableDetalleNovedad(Tipo_Novedad_Id).Result;
                ListDetalleTipoNovedad=list.ToList();
                ListDetalleTipoNovedad.Add(new DetalleTipoNovedad{
                  Id=0,
                  Nombre="Todas las novedades"
              });
             
            }else{
                Tipo_Novedad_Id=-1;
            }  
            Limpiar=true;
            IsVisibleExportPdf=false;
            InitElementos();
           /// InitElementos();
        }

        public void DetalleTipoNovedadChange()
        {
            if(SelectDetalleTipoNovedad!=null) {
                Detalle_Tipo_Novedad_Id= SelectDetalleTipoNovedad.Id;
             
            }else{
                Detalle_Tipo_Novedad_Id=-1;
            }  
            Limpiar=true;
            IsVisibleExportPdf=false;
            InitElementos();
           /// InitElementos();
        }



        public  void Consultar()
        {
            Limpiar=true;
            InitElementos();
            if(ReportType=="General"){
                var list = ViewReportGeneral().Result;
                ReportElementos=list.ToList();
                Limpiar=false;
                InitElementos();
            }else{
                var list = ViewReportDetallado().Result;
                ReportElementos=list.ToList();
                Limpiar=false;
                InitElementos();
            }
        }



        public void DateChange()
        {
            
              if(Empresa_Id==0){
                  // VisibleRadioButtonDetallado=true;
              }else{
                 //  VisibleRadioButtonDetallado=false;
              }

            IsVisibleExportPdf=false;
             //InitElementos();
        }

        public void DepartmentChange()
        {
         
         
            SelectedCiudad=null;
           
            Ciudades.Clear();
          
            Ciudad_Id=-1;
            Empresa_Id=-1;
           // VisibleRadioButtonDetallado=false;
            ReportType="General";
            IsVisibleExportPdf=false;
         

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
               /// VisibleRadioButtonDetallado=false;
                ReportType="General";
            }  


            Limpiar=true;
            InitElementos();
           /// InitElementos();
        }


        public void CiudadChange()
        {
          //  IsVisible=true;
         
            Empresa_Id=-1;
         //   VisibleRadioButtonDetallado=false;
            ReportType="General";
             IsVisibleExportPdf=false;
             

            if(SelectedCiudad!=null) {
              Ciudad_Id= SelectedCiudad.Id;
            }else{
                Ciudad_Id=-1;
                Empresa_Id=-1;
                Departamento_Id=-1;
            ///    VisibleRadioButtonDetallado=false;
                ReportType="General";
            }

             Limpiar=true;
            InitElementos();
            //InitElementos();
        }

       


        #endregion

        #region Methods

        private  void LoadInit()
        {
            var listDepartamnets= GetQueryableDepartament().Result;
            Departamentos= listDepartamnets.ToList();
            Departamento_Id=-1;

            var listTipoNovedad= GetQueryableTipooNovedad().Result;
            ListTipoNovedad= listTipoNovedad.ToList();

              ListTipoNovedad.Add(new TipoNovedad{
                  Id=0,
                  Nombre="Todas los tipos novedades"
              });
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

        private async Task<IQueryable<TipoNovedad>> GetQueryableTipooNovedad()
        {
            var tipoNovedad= await _tipoNovedadRepository.AllIncludingAsync();
            return  tipoNovedad.AsQueryable();
        }

        private async Task<IQueryable<DetalleTipoNovedad>> GetQueryableDetalleNovedad(long tipo_novedad_id)
        {
            var detalle_tipo_novedad= await _detalleTipoNovedadRepository.AllIncludingAsyncWhere(a=>a.Tipo_Novedad_id==tipo_novedad_id);
            return  detalle_tipo_novedad.AsQueryable();
        }

        #endregion

        #region UI Elements
        public   void  InitElementos()
        {
            Elementos = new BpGridViewDataSet<ElementoReportViewModel> {
                OnLoadingData = GetData,
                RowEditOptions = new RowEditOptions {
                    PrimaryKeyPropertyName = nameof(Empresa.Id),
                    EditRowId = -1
                }
            };
            Elementos.SetSortExpression(nameof(Empresa.Id));
           UserSettings = new GridViewUserSettings {
                EnableUserSettings = true,
                
                ColumnsSettings = new List<GridViewColumnSetting> {
                     new GridViewColumnSetting {
                        ColumnName = "Elemento_Id",
                        DisplayOrder = 0,
                        ColumnWidth = 50
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Usuario",
                        DisplayOrder = 1,
                    },
                    new GridViewColumnSetting {
                        ColumnName = "CodigoApoyo",
                        DisplayOrder = 2,
                        ColumnWidth = 400
                    },
                   
                    new GridViewColumnSetting {
                        ColumnName = "FechaLevantamiento",
                        DisplayOrder = 3
                    },
                    new GridViewColumnSetting {
                        ColumnName = "HoraInicio",
                        DisplayOrder = 4
                    },
                    new GridViewColumnSetting {
                        ColumnName = "HoraFin",
                        DisplayOrder = 5
                    },
                     new GridViewColumnSetting {
                        ColumnName = "ResistenciaMecanica",
                        
                        DisplayOrder = 6
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Retenidas",
                        
                        DisplayOrder = 7
                    },
                    new GridViewColumnSetting {
                        ColumnName = "AlturaDisponible",
                        
                        DisplayOrder = 8
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Nombre_Proyecto",
                        
                        DisplayOrder = 9
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Nombre_Material",
                        
                        DisplayOrder = 10
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Longitud",
                        
                        DisplayOrder = 11
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Nombre_Estado",
                        
                        DisplayOrder = 12
                    },

                    new GridViewColumnSetting {
                        ColumnName = "Nombre_Nivel_Tension",
                        
                        DisplayOrder = 13
                    },

                     new GridViewColumnSetting {
                        ColumnName = "Coordenadas_Elemento",
                        
                        DisplayOrder = 14
                    },

                      new GridViewColumnSetting {
                        ColumnName = "Nombre_Tipo_Novedad",
                        
                        DisplayOrder = 15
                    },

                      new GridViewColumnSetting {
                        ColumnName = "Nombre_Detalle_Tipo_Novedad",
                        
                        DisplayOrder = 16
                    }
                }
            };
           // return base.Init();
        }
        public GridViewDataSetLoadedData<ElementoReportViewModel> GetData(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {

            if(Limpiar){
               var queryable = GetQueryableElementosEmpty();
               return queryable.GetDataFromQueryable(gridViewDataSetOptions);
            }

            if(ReportType=="General"){
               var queryable = GetQueryableElementosGeneral();
               return queryable.GetDataFromQueryable(gridViewDataSetOptions);
            }else{
                var queryable = GetQueryableElementosDetallado();
                return queryable.GetDataFromQueryable(gridViewDataSetOptions);
            }
           
        }

          private  IQueryable<ElementoReportViewModel> GetQueryableElementosEmpty()
        {
            //empresa.as
            var lisEmpty= new List<ElementoReportViewModel>();
            return  lisEmpty.AsQueryable();
        }

       ///Report General
       /* 
        private async Task<IQueryable<ElementoViewModel>> ReportGeneral(){

            IEnumerable<Novedad> elementosNovedad=Enumerable.Empty<Novedad>();
             //Todas las ciudades y todos los usuarios
            if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id==0 ){
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            //Todas los usuarios de una ciudad
            else if(Ciudad_Id!=0 && Detalle_Tipo_Novedad_Id==0){
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id!=0){
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else{
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto,  f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }

      
            //var viewModelMap = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(filterListCablesDates);
            var list= new List<ElementoViewModel>();

            foreach(var item in elementosNovedad){
                var viewModelMap = _mapper.Map<Elemento, ElementoViewModel>(item.Elemento);
                viewModelMap.NombreCiudad=item.Elemento.Ciudad.Nombre;
                viewModelMap.Direccion= item.Elemento.LocalizacionElementos.FirstOrDefault().Direccion;
                string hourStartFormat = DateTime.ParseExact(item.Elemento.HoraInicio,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                string hourEndFormat = DateTime.ParseExact(item.Elemento.HoraFin,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");

                viewModelMap.CodigoApoyo=item.DetalleTipoNovedad.Nombre;
                  viewModelMap.NumeroApoyo= item.Elemento_Id;

                var usuario= await _usuarioRepository.GetSingleAsync(a=>a.Id==item.Elemento.Usuario_Id);
                viewModelMap.HoraInicio=hourStartFormat;
                viewModelMap.HoraFin=hourEndFormat;
                viewModelMap.Usuario = string.Format("{0} {1}",usuario.Nombre, usuario.Apellido);

                viewModelMap.Detalle_Tipo_Novedad_Id=item.Detalle_Tipo_Novedad_Id;
                viewModelMap.Nombre_Detalle_Tipo_Novedad=item.DetalleTipoNovedad.Nombre;
                viewModelMap.Descripcion_Novedad=item.Descripcion;
                viewModelMap.Tipo_Novedad_id=item.DetalleTipoNovedad.Tipo_Novedad_id;
                viewModelMap.Nombre_Tipo_Novedad=item.DetalleTipoNovedad.TipoNovedad.Nombre;
                 viewModelMap.Coordenadas= item.Elemento.LocalizacionElementos.Where(a=>a.Element_Id==item.Elemento_Id).FirstOrDefault().Coordenadas;
                list.Add(viewModelMap);
            }
            
            return  list.AsQueryable();
        }
        */



         private async Task<IQueryable<ElementoReportViewModel>> ViewReportGeneral(){
            IEnumerable<View_Novedad_Elemento_Report> elementosNovedad=Enumerable.Empty<View_Novedad_Elemento_Report>();
               var dataContext = MyAppContext.GetInstance();
             //Todas las ciudades y todos los usuarios

             if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id==0){
                elementosNovedad=dataContext.View_Novedades.Where(a=>a.Departamento_Id==Departamento_Id);
             }
             else if(Ciudad_Id!=0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id==0){
                   elementosNovedad=dataContext.View_Novedades.Where(a=>a.Ciudad_Id==Ciudad_Id);
             }
            else if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id!=0){
                elementosNovedad=dataContext.View_Novedades.Where(a=>a.Departamento_Id==Departamento_Id && a.Tipo_Novedad_id==Tipo_Novedad_Id);

               // elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            //Todas los usuarios de una ciudad
            else if(Ciudad_Id!=0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id!=0){

                 elementosNovedad=dataContext.View_Novedades.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Tipo_Novedad_id==Tipo_Novedad_Id);
               // elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id!=0 && Tipo_Novedad_Id!=0){
                elementosNovedad=dataContext.View_Novedades.Where(a=>a.Departamento_Id==Departamento_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id && a.Tipo_Novedad_id==Tipo_Novedad_Id);
               // elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else{
                 elementosNovedad=dataContext.View_Novedades.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id);
              ///  elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto,  f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }

            //var viewModelMap = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(filterListCablesDates);
            var list= new List<ElementoReportViewModel>();
            foreach(var item in elementosNovedad){
                var viewModelMap = _mapper.Map<View_Novedad_Elemento_Report, ElementoReportViewModel>(item);
                string hourStartFormat = DateTime.ParseExact(item.HoraInicio,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                string hourEndFormat = DateTime.ParseExact(item.HoraFin,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
              //  viewModelMap.CodigoApoyo=item.Nombre_Detalle_Tipo_Novedad;
                viewModelMap.NumeroApoyo= item.Elemento_Id;
                viewModelMap.HoraInicio=hourStartFormat;
                viewModelMap.HoraFin=hourEndFormat;
                
                viewModelMap.Usuario = string.Format("{0} {1}",item.Nombre_Usuario, item.Apellido_Usuario);
                list.Add(viewModelMap);
            }
            return  list.AsQueryable();
        }

        private  IQueryable<ElementoReportViewModel> GetQueryableElementosGeneral()
        {
            if(ReportElementos.Count>0){
                IsVisibleExportPdf=true;
            }else{
                 IsVisibleExportPdf=false;
            }
            //empresa.as
            return  ReportElementos.AsQueryable();
        }

         ///Report Detallado
         /* 
         private async Task<IQueryable<ElementoViewModel>> ReportDetallado(){
             // var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
            IEnumerable<Novedad> elementosNovedad=Enumerable.Empty<Novedad>();
             //Todas las ciudades y todos los usuarios
            if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id==0 ){
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            //Todas los usuarios de una ciudad
            else if(Ciudad_Id!=0 && Detalle_Tipo_Novedad_Id==0){
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id!=0){
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else{
                elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto,  f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }

            var list= new List<ElementoViewModel>();

           /// var MaximumPrice=0;
            foreach(var item in elementosNovedad){
                //var sumdata= filterListCablesDates.Count/100;
                ///MaximumPrice+=sumdata;

               var viewModelMap = _mapper.Map<Elemento, ElementoViewModel>(item.Elemento);
                viewModelMap.NombreCiudad=item.Elemento.Ciudad.Nombre;
                viewModelMap.NumeroApoyo= item.Elemento_Id;
                viewModelMap.Direccion= item.Elemento.LocalizacionElementos.FirstOrDefault().Direccion;
                string hourStartFormat = DateTime.ParseExact(item.Elemento.HoraInicio,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                string hourEndFormat = DateTime.ParseExact(item.Elemento.HoraFin,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");

                var usuario= await _usuarioRepository.GetSingleAsync(a=>a.Id==item.Elemento.Usuario_Id);
                viewModelMap.HoraInicio=hourStartFormat;
                viewModelMap.HoraFin=hourEndFormat;
                viewModelMap.Usuario = string.Format("{0} {1}",usuario.Nombre, usuario.Apellido);
                
                viewModelMap.Detalle_Tipo_Novedad_Id=item.Detalle_Tipo_Novedad_Id;
                viewModelMap.Nombre_Detalle_Tipo_Novedad=item.DetalleTipoNovedad.Nombre;
                viewModelMap.Descripcion_Novedad=item.Descripcion;
                viewModelMap.Tipo_Novedad_id=item.DetalleTipoNovedad.Tipo_Novedad_id;
                viewModelMap.Nombre_Tipo_Novedad=item.DetalleTipoNovedad.TipoNovedad.Nombre;
            
                viewModelMap.Fotos.Clear();
                 viewModelMap.Equipos.Clear();

                  viewModelMap.Cables.Clear();
                //list Fotos
                var listFotos= new List<FotoViewModel>();

                var listFotosPoste= _fotoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Id);
                listFotos= _mapper.Map<IEnumerable<Foto>, IEnumerable<FotoViewModel>>(listFotosPoste.Result).ToList();
                viewModelMap.Fotos =listFotos;


                viewModelMap.Coordenadas= item.Elemento.LocalizacionElementos.Where(a=>a.Element_Id==item.Elemento_Id).FirstOrDefault().Coordenadas;
                list.Add(viewModelMap);
            }
            //empresa.as
            return  list.AsQueryable();
         }*/

         private async Task<IQueryable<ElementoReportViewModel>> ViewReportDetallado(){
             // var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
             IEnumerable<View_Novedad_Elemento_Report> elementosNovedad=Enumerable.Empty<View_Novedad_Elemento_Report>();
               var dataContext = MyAppContext.GetInstance();
             //Todas las ciudades y todos los usuarios
            if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id==0){
                elementosNovedad=dataContext.View_Novedades.Where(a=>a.Departamento_Id==Departamento_Id);
             }
             else if(Ciudad_Id!=0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id==0){
                   elementosNovedad=dataContext.View_Novedades.Where(a=>a.Ciudad_Id==Ciudad_Id);
             }
            else if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id!=0){
                elementosNovedad=dataContext.View_Novedades.Where(a=>a.Departamento_Id==Departamento_Id && a.Tipo_Novedad_id==Tipo_Novedad_Id);

               // elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            //Todas los usuarios de una ciudad
            else if(Ciudad_Id!=0 && Detalle_Tipo_Novedad_Id==0 && Tipo_Novedad_Id!=0){

                 elementosNovedad=dataContext.View_Novedades.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Tipo_Novedad_id==Tipo_Novedad_Id);
               // elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else if(Ciudad_Id==0 && Detalle_Tipo_Novedad_Id!=0 && Tipo_Novedad_Id!=0){
                elementosNovedad=dataContext.View_Novedades.Where(a=>a.Departamento_Id==Departamento_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id && a.Tipo_Novedad_id==Tipo_Novedad_Id);
               // elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad.departmentoId==Departamento_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }
            else{
                 elementosNovedad=dataContext.View_Novedades.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id);
              ///  elementosNovedad =await _novedadRepository.AllIncludingAsyncWhere(a=>a.Elemento.Ciudad_Id==Ciudad_Id && a.Detalle_Tipo_Novedad_Id==Detalle_Tipo_Novedad_Id,c=>c.Elemento.Material,d=>d.Elemento.Proyecto,  f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,k=>k.Elemento.Ciudad, m=>m.DetalleTipoNovedad,n=>n.DetalleTipoNovedad.TipoNovedad);
            }

            //var viewModelMap = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(filterListCablesDates);
            var list= new List<ElementoReportViewModel>();

           /// var MaximumPrice=0;
            foreach(var item in elementosNovedad){

               var viewModelMap = _mapper.Map<View_Novedad_Elemento_Report, ElementoReportViewModel>(item);
                string hourStartFormat = DateTime.ParseExact(item.HoraInicio,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                string hourEndFormat = DateTime.ParseExact(item.HoraFin,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                
                viewModelMap.Usuario = string.Format("{0} {1}",item.Nombre_Usuario, item.Apellido_Usuario);
                viewModelMap.Fotos= new List<FotoViewModel>();
               
                //list Fotos
                var listFotos= new List<Foto>();

                if(Detalle_Tipo_Novedad_Id==0){

                }


                var listFotosPoste= await  _fotoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Elemento_Id );
                foreach(var itemfoto in listFotosPoste.ToList().Where(a=>a.Novedad_Id==item.Novedad_Id || a.Novedad_Id==0)){
                    listFotos.Add(itemfoto);
                }
                viewModelMap.Fotos = _mapper.Map<IEnumerable<Foto>, IEnumerable<FotoViewModel>>(listFotos).ToList();
                list.Add(viewModelMap);
            }
            //empresa.as
            return  list.AsQueryable();
        }

        private  IQueryable<ElementoReportViewModel> GetQueryableElementosDetallado()
        {
            if(ReportElementos.Count>0){
                IsVisibleExportPdf=true;
            }else{
                 IsVisibleExportPdf=false;
            }
            //empresa.as
            return  ReportElementos.AsQueryable();
        }
    
       
       
        #endregion

        #region Report PDF
      
        public void ExportPdf(){

             if(ReportType=="General"){
                 ExporGeneralPdf();
             }else{
                 ExportDetalladoPdf();
             }
        }

        public void ExportDetalladoPdf(){
            var outputStream = new MemoryStream();
            ElementosDetalleNovedadPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream, ReportElementos,"","",Detalle_Tipo_Novedad_Id);
            Context.ReturnFile(outputStream, "report_detalle_novedades.pdf", "application/pdf");
        }

        public void ExporGeneralPdf(){
            var outputStream = new MemoryStream();
            ElementosByNovedadesPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream, ReportElementos);
            Context.ReturnFile(outputStream, "reporte_general_novedades.pdf", "application/pdf");
        }
        

        public void ExportExcel()
        {
                var exporter = new GridViewExportCsv<ElementoReportViewModel>(new GridViewExportCsvSettings<ElementoReportViewModel> { Separator = ";" });
                var gridView = Context.View.FindControlByClientId<DotVVM.BusinessPack.Controls.GridView>("data", true);
                using (var file = exporter.Export(gridView,  this.Elementos))
                {
                    Context.ReturnFile(file, "ReportePlano.csv", "text/csv");
               // Context.ReturnFile(file, "export.pdf", "application/pdf");
                }
        }

        #endregion
    }
}