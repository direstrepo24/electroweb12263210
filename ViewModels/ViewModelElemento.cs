using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
using System.Globalization;


using OfficeOpenXml.Style;
using System.Drawing;


using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Reflection;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.Util;
using Electro.model.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Data;
using NPOI.HSSF.UserModel;

namespace electroweb.ViewModels
{
    public class ViewModelElemento:BaseViewModel
    {

        #region Atributes
        private readonly IElementoRepository  _IelementosRepository;
        private readonly IDepartamentoRepository  _departamentoRepository;
        private readonly ICiudadRepository _ciudadRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly ICiudad_EmpresaRepository _ciudad_EmpresaRepository;
        private readonly IElementoCableRepository _elementoCableRepository;
        private readonly IEquipoElementoRepository _equipoElementoRepository;

        private readonly ILocalizacionElementoRepository _localizacionElementoRepository;

        private readonly INovedadRepository _novedadRepository;
        private readonly IFotoRepository _fotoRepository;

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


        public List<ResponseCiudadEmpresa>  Empresas {get; set;}= new List<ResponseCiudadEmpresa>();

        public ResponseCiudadEmpresa SelectedEmpresa { get; set; }

        public BpGridViewDataSet<ElementoReportViewModel> Elementos { get; set; }

        public GridViewUserSettings UserSettings { get; set; }

        public DateTime SelectedDateStart { get; set; }=DateTime.Now;

        public DateTime SelectedDateEnd { get; set; }=DateTime.Now;

         public bool Limpiar { get; set; }=true;

    

        public long Empresa_Id { get; set; }
        public long Ciudad_Id { get; set; }
        public long Departamento_Id { get; set; }

      

        public string ReportType { get; set; }="General";
        
        public bool VisibleRadioButtonDetallado { get; set; }=true;
        public bool IsVisibleExportPdf { get; set; }=false;
        public bool IsVisibleExportExcell { get; set; }=false;

        

        #endregion

        #region Contructor
        public ViewModelElemento(
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
        ILocalizacionElementoRepository localizacionElementoRepository
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
            _localizacionElementoRepository= localizacionElementoRepository;
         
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

        

        public  void Consultar()
        {
            try{
         
            Limpiar=true;
            InitElementos();

            if(ReportType=="General"){
                var list = ReportGeneralView().Result;
                ReportElementos=list.ToList();
                Limpiar=false;
                InitElementos();
                
            }else{
                var list = ReportDetalladoView().Result;
                ReportElementos=list.ToList();
                Limpiar=false;
                InitElementos();
                
            }

            }catch(Exception ex){
                  throw new Exception("Exception Occured While Printing", ex);
            }
            
           
        }

        
        public  void ReportTypeChange()
        {

            if(ReportType=="General"){
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
                IsVisibleExportPdf=false;
                IsVisibleExportExcell=false;
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
             IsVisibleExportExcell=false;
            ReportType="Detallado";
            if(ReportElementos!=null){
                    ReportElementos.Clear();
            }
            

            Limpiar=true;
            InitElementos();
            }
        }


        public void DateChange()
        {
             
              if(Empresa_Id==0){
                   VisibleRadioButtonDetallado=true;
              }else{
                  // VisibleRadioButtonDetallado=false;
              }
         




               IsVisibleExportPdf=false;
               IsVisibleExportExcell=false;
             //InitElementos();
        }

        public void DepartmentChange()
        {
         
       
            SelectedCiudad=null;
            SelectedEmpresa=null;
            Ciudades.Clear();
            Empresas.Clear();

            Ciudad_Id=-1;
            Empresa_Id=-1;
          //  VisibleRadioButtonDetallado=false;
            ReportType="General";
             IsVisibleExportPdf=false;
              IsVisibleExportExcell=false;
         

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
                //VisibleRadioButtonDetallado=false;
                ReportType="General";
            }  


            Limpiar=true;
            InitElementos();

           /// InitElementos();
        }


        public void CiudadChange()
        {
          //  IsVisible=true;
            SelectedEmpresa=null;
            Empresas.Clear();
            Empresa_Id=-1;
          //  VisibleRadioButtonDetallado=false;
            ReportType="General";
             IsVisibleExportPdf=false;
              IsVisibleExportExcell=false;
             

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
              //  VisibleRadioButtonDetallado=false;
                ReportType="General";
            }

             Limpiar=true;
            InitElementos();
            //InitElementos();
        }

        public void EmpresaChange()
        {
         // IsVisible=true;
            if(SelectedEmpresa!=null) {
               Empresa_Id= SelectedEmpresa.Empresa.Id;
              if(Empresa_Id==0){
                   //VisibleRadioButtonDetallado=true;
                    
              }else{
                   ReportType="General";
                  
                  // VisibleRadioButtonDetallado=false;
              }
              
            }  else{
               Empresa_Id= -1;
                ReportType="General";
               // VisibleRadioButtonDetallado=false;
            }

             IsVisibleExportPdf=false;
             IsVisibleExportExcell=false;
            

        

             Limpiar=true;
            InitElementos();
           // InitElementos();
           // Empresas = GetQueryableEmpresasByCiudad(ciudad_id).Result.ToList();

        
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

        #region UI Elements
        public   void  InitElementos()
        {
            Elementos = new BpGridViewDataSet<ElementoReportViewModel> {
                OnLoadingData = GetData,
                RowEditOptions = new RowEditOptions {
                    PrimaryKeyPropertyName = nameof(ElementoReportViewModel.Id),
                    EditRowId = -1
                }
            };

           Elementos.SetSortExpression(nameof(ElementoReportViewModel.Id));
           UserSettings = new GridViewUserSettings {
                EnableUserSettings = true,
                
                ColumnsSettings = new List<GridViewColumnSetting> {

                    new GridViewColumnSetting {
                        ColumnName = "Elemento_Id",
                        DisplayOrder = 0,
                        ColumnWidth = 50
                    },
                    new GridViewColumnSetting {
                        ColumnName = "CodigoApoyo",
                        DisplayOrder = 1,
                        ColumnWidth = 400
                    },
                    
                    new GridViewColumnSetting {
                        ColumnName = "FechaLevantamiento",
                        DisplayOrder = 2
                    },
                    new GridViewColumnSetting {
                        ColumnName = "HoraInicio",
                        DisplayOrder = 3
                    },
                    new GridViewColumnSetting {
                        ColumnName = "HoraFin",
                        DisplayOrder = 4
                    },
                     new GridViewColumnSetting {
                        ColumnName = "ResistenciaMecanica",
                        
                        DisplayOrder = 5
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Retenidas",
                        
                        DisplayOrder = 6
                    },
                    new GridViewColumnSetting {
                        ColumnName = "AlturaDisponible",
                        
                        DisplayOrder = 7
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Nombre_Proyecto",
                        
                        DisplayOrder = 8
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Nombre_Material",
                        
                        DisplayOrder = 9
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Longitud",
                        
                        DisplayOrder = 10
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Nombre_Estado",
                        
                        DisplayOrder = 11
                    },

                    new GridViewColumnSetting {
                        ColumnName = "Nombre_Nivel_Tension",
                        
                        DisplayOrder = 12
                    },
                     new GridViewColumnSetting {
                        ColumnName = "SobreRbt",
                        
                        DisplayOrder = 13
                    },

                    new GridViewColumnSetting {
                        ColumnName = "Tiene_Marquilla",
                        
                        DisplayOrder = 14
                    },
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
            var lisEmpty= new List<ElementoReportViewModel>();
            return  lisEmpty.AsQueryable();
        }

       ///Report General
       /* 
        private async Task<IQueryable<ElementoViewModel>> ReportGeneral(){
            IEnumerable<ElementoCable> cables=Enumerable.Empty<ElementoCable>();
            if(Ciudad_Id==0 && Empresa_Id==0){
                cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Empresa.Ciudad.departmento.Id==Departamento_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, f=>f.Ciudad_Empresa.Ciudad.departmento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
            //Todas las empresas de una ciudad
            else if(Ciudad_Id!=0 && Empresa_Id==0){
                cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad, l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
            else{
                cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id && a.Empresa_Id==Empresa_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
           var filterListCablesDates=cables.ToList().Where(a=>a.Elemento.FechaLevantamiento.Day>=SelectedDateStart.Day && 
                                                a.Elemento.FechaLevantamiento.Month>=SelectedDateStart.Month &&
                                                a.Elemento.FechaLevantamiento.Year>=SelectedDateStart.Year &&
                                                a.Elemento.FechaLevantamiento.Day<=SelectedDateEnd.Day && 
                                                a.Elemento.FechaLevantamiento.Month<=SelectedDateEnd.Month &&
                                                a.Elemento.FechaLevantamiento.Year<=SelectedDateEnd.Year ).ToList();
            var list= new List<ElementoViewModel>();
            foreach(var item in filterListCablesDates){
                var viewModelMap = _mapper.Map<Elemento, ElementoViewModel>(item.Elemento);
                viewModelMap.EmpresaOperadora_Id=item.Empresa_Id;
                viewModelMap.NombreEmpresaOperadora=item.Ciudad_Empresa.Empresa.Nombre;
                viewModelMap.NombreCiudad=item.Ciudad_Empresa.Ciudad.Nombre;
                viewModelMap.Direccion= item.Elemento.LocalizacionElementos.FirstOrDefault().Direccion;
                viewModelMap.Detalle_Cable= item.DetalleTipoCable.Cable.Nombre;
                viewModelMap.Cantidad_Cable= item.Cantidad.ToString();
               
                var coords= string.Empty;
                if(item.Elemento.LocalizacionElementos.FirstOrDefault().Latitud>0 && item.Elemento.LocalizacionElementos.FirstOrDefault().Longitud>0){
                    var latitud=  string.Format("{0:N6}", item.Elemento.LocalizacionElementos.FirstOrDefault().Latitud);
                    var longitud=  string.Format("{0:N6}", item.Elemento.LocalizacionElementos.FirstOrDefault().Longitud);
                    coords= string.Format("{0},{1}",latitud,longitud);
                }else{
                     coords= string.Format("{0},{1}",item.Elemento.LocalizacionElementos.FirstOrDefault().Latitud,item.Elemento.LocalizacionElementos.FirstOrDefault().Longitud);
                }
                viewModelMap.Coordenadas= coords;

                var Tiene_Amplificador="NO";
                var Tiene_Fuente ="NO";
                var Tiene_DistribuidorFibra ="NO";
                var ConectadoRbt="";
                var MedidorBt="";
                var Otro_Equipo="";

                if(viewModelMap.CodigoApoyo==null || viewModelMap.CodigoApoyo==""){
                    //Verificar la novedad del codigo de apoyo vacio
                    var novedad= await _novedadRepository.GetSingleAsync(a=>a.Elemento_Id==item.Elemento.Id && a.DetalleTipoNovedad.Tipo_Novedad_id==1, b=>b.DetalleTipoNovedad);
                    if(novedad!=null){
                        if(novedad.Detalle_Tipo_Novedad_Id!=3){
                            viewModelMap.CodigoApoyo=novedad.DetalleTipoNovedad.Nombre;
                        }
                    }
                }


                //Si es una consulta por operador se habilita
                //Equipos
                if(Ciudad_Id>0 && Empresa_Id>0){
                    foreach(var equipo in item.Elemento.Equipos){
                    var queryequipo= await _equipoElementoRepository.GetSingleAsync(a=>a.Id==equipo.Id && a.Ciudad_Empresa.Empresa_Id==Empresa_Id, b=>b.TipoEquipo,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa);
                        if(queryequipo!=null){
                            var mapEquipo= _mapper.Map<EquipoElemento, EquipoViewModel>(queryequipo);
                            if(mapEquipo.TipoEquipo.Nombre.ToUpper().Contains("Fuente".ToUpper())){
                                Tiene_Fuente="SI";
                                if(queryequipo.ConectadoRbt){
                                ConectadoRbt="SI";
                                }
                                if(queryequipo.MedidorBt){
                                    MedidorBt="SI";
                                }
                            }else if(mapEquipo.TipoEquipo.Nombre.ToUpper().Contains("Amplificador".ToUpper())){
                                Tiene_Amplificador="SI";
                                if(queryequipo.ConectadoRbt){
                                ConectadoRbt="SI";
                                }
                                if(queryequipo.MedidorBt){
                                    MedidorBt="SI";
                                }
                            }else if(mapEquipo.TipoEquipo.Nombre.ToUpper().Contains("Nodo".ToUpper())){
                                Tiene_DistribuidorFibra="SI";
                                if(queryequipo.ConectadoRbt){
                                ConectadoRbt="SI";
                                }
                                if(queryequipo.MedidorBt){
                                    MedidorBt="SI";
                                }
                            }else if(mapEquipo.TipoEquipo.Nombre.ToUpper().Contains("Caja de Empalme".ToUpper())){
                                Tiene_DistribuidorFibra="SI";
                                if(queryequipo.ConectadoRbt){
                                ConectadoRbt="SI";
                                }
                                if(queryequipo.MedidorBt){
                                    MedidorBt="SI";
                                }
                            }else if(mapEquipo.TipoEquipo.Nombre.ToUpper().Contains("Otros".ToUpper())){
                                Otro_Equipo="SI";
                                if(queryequipo.ConectadoRbt){
                                ConectadoRbt="SI";
                                }
                                if(queryequipo.MedidorBt){
                                    MedidorBt="SI";
                                }
                            }
                        }
                    }
                }
                viewModelMap.Tiene_Amplificador=Tiene_Amplificador;
                viewModelMap.Tiene_Fuente=Tiene_Fuente;
                viewModelMap.Tiene_DistribuidorFibra=Tiene_DistribuidorFibra;
                viewModelMap.ConectadoRbt=ConectadoRbt;
                viewModelMap.MedidorBt=MedidorBt;
                viewModelMap.Otro_Equipo=Otro_Equipo;

                list.Add(viewModelMap);
            }
            return  list.AsQueryable();
        }
        */

        
        private async Task<IQueryable<ElementoReportViewModel>> ReportGeneralView(){
            //IEnumerable<ElementoCable> cables=Enumerable.Empty<ElementoCable>();
            var dataContext = MyAppContext.GetInstance();
            IEnumerable<View_Cable_Report> cables=Enumerable.Empty<View_Cable_Report>();
            if(Ciudad_Id==0 && Empresa_Id==0){
                cables = dataContext.View_Cables.Where(a=>a.Departamento_Id==Departamento_Id).ToList();
                //cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Empresa.Ciudad.departmento.Id==Departamento_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, f=>f.Ciudad_Empresa.Ciudad.departmento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
            //Todas las empresas de una ciudad
            else if(Ciudad_Id!=0 && Empresa_Id==0){
                cables = dataContext.View_Cables.Where(a=>a.Ciudad_Id==Ciudad_Id).ToList();
            }
            else{
                cables = dataContext.View_Cables.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Empresa_Id==Empresa_Id).ToList();
               // cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id && a.Empresa_Id==Empresa_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }

           var filterListCablesDates=cables.ToList().Where(a=>a.FechaLevantamiento.Day>=SelectedDateStart.Day && 
                                                a.FechaLevantamiento.Month>=SelectedDateStart.Month &&
                                                a.FechaLevantamiento.Year>=SelectedDateStart.Year &&
                                                a.FechaLevantamiento.Day<=SelectedDateEnd.Day && 
                                                a.FechaLevantamiento.Month<=SelectedDateEnd.Month &&
                                                a.FechaLevantamiento.Year<=SelectedDateEnd.Year ).ToList();

            var list= new List<ElementoReportViewModel>();
            foreach(var item in filterListCablesDates){
                var viewModelMap = _mapper.Map<View_Cable_Report, ElementoReportViewModel>(item);
                /*var coords= string.Empty;
                if(item.Latitud_Elemento>0 &&item.Longitud_Elemento>0){
                    var latitud=  string.Format("{0:N6}", item.Latitud_Elemento);
                    var longitud=  string.Format("{0:N6}", item.Longitud_Elemento);
                    coords= string.Format("{0},{1}",latitud,longitud);
                }else{
                     coords= string.Format("{0},{1}",item.Latitud_Elemento,item.Longitud_Elemento);
                }

                viewModelMap.Coordenadas_Elemento= coords;*/

                var Tiene_Amplificador="NO";
                var Tiene_Fuente ="NO";
                var Tiene_DistribuidorFibra ="NO";
                var ConectadoRbt="";
                var MedidorBt="";
                var Otro_Equipo="";
                var Empresa_Equipo="";

                if(viewModelMap.CodigoApoyo==null || viewModelMap.CodigoApoyo==""){
                    //Verificar la novedad del codigo de apoyo vacio
                    var novedad= await _novedadRepository.GetSingleAsync(a=>a.Elemento_Id==item.Elemento_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==1, b=>b.DetalleTipoNovedad);
                    if(novedad!=null){
                        if(novedad.Detalle_Tipo_Novedad_Id!=3){
                            viewModelMap.CodigoApoyo=novedad.DetalleTipoNovedad.Nombre;
                        }
                    }
                }
                /*
                var group = cables.ToList().GroupBy(a=>a.Elemento_Id);
                var list= new List<Elemento>();
                foreach(var item in group.ToList()){
                    list.Add(item.FirstOrDefault().Elemento);
                } */
                //Si es una consulta por operador se habilita
                //Equipos     a=>a.Elemento_Id==item.Elemento_Id, b=>b.TipoEquipo,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa
               // if(Ciudad_Id>0 && Empresa_Id>0){
                if(Ciudad_Id>0){
                    var equiposElementos= await _equipoElementoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Elemento_Id, b=>b.TipoEquipo,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa);
                    foreach(var queryequipo in equiposElementos.ToList()){
                            var mapEquipo= _mapper.Map<EquipoElemento, EquipoViewModel>(queryequipo);
                            ConectadoRbt=mapEquipo.ConectadoRbt;
                            MedidorBt=mapEquipo.MedidorBt;
                            if(mapEquipo.TipoEquipo_Id==1){//Id equivale a Fuente 
                                Tiene_Fuente="SI";
                                Empresa_Equipo=queryequipo.Ciudad_Empresa.Empresa.Nombre;
                            }else if(mapEquipo.TipoEquipo_Id==2){//Id equivale a Amplificador 
                                Tiene_Amplificador="SI";
                                Empresa_Equipo=queryequipo.Ciudad_Empresa.Empresa.Nombre;
                            }else if(mapEquipo.TipoEquipo_Id==3){//Id equivale a Nodo 
                                Tiene_DistribuidorFibra="SI";
                                Empresa_Equipo=queryequipo.Ciudad_Empresa.Empresa.Nombre;
                            }else if(mapEquipo.TipoEquipo_Id==4){//Id equivale a Empalme
                                Tiene_DistribuidorFibra="SI";
                                Empresa_Equipo=queryequipo.Ciudad_Empresa.Empresa.Nombre;
                            }else if(mapEquipo.TipoEquipo_Id==5){//Id equivale a Otros
                                Otro_Equipo="SI";
                            }
                    }
                }

                viewModelMap.Tiene_Amplificador=Tiene_Amplificador;
                viewModelMap.Tiene_Fuente=Tiene_Fuente;
                viewModelMap.Tiene_DistribuidorFibra=Tiene_DistribuidorFibra;
                viewModelMap.ConectadoRbt=ConectadoRbt;
                viewModelMap.MedidorBt=MedidorBt;
                viewModelMap.Otro_Equipo=Otro_Equipo;
                viewModelMap.NumeroApoyo= item.Elemento_Id;
                viewModelMap.Empresa_Equipo= Empresa_Equipo;
                list.Add(viewModelMap);
            }

            return  list.AsQueryable();
        }





        private  IQueryable<ElementoReportViewModel> GetQueryableElementosGeneral()
        {
            if(ReportElementos.Count>0){
                IsVisibleExportPdf=true;
              
                if(Empresa_Id>0){
                    IsVisibleExportExcell=true;
                }
                    
            }else{
             
                 IsVisibleExportPdf=false;
                 IsVisibleExportExcell=false;
            }
            //empresa.as
            return  ReportElementos.AsQueryable();
        }

         ///Report Detallado

         /* 
         private async Task<IQueryable<ElementoViewModel>> ReportDetallado(){
             // var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
            IEnumerable<ElementoCable> cables=Enumerable.Empty<ElementoCable>();

            //Todas lkas ciudades y todas las empresas
            if(Ciudad_Id==0 && Empresa_Id==0){
                cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Empresa.Ciudad.departmentoId==Departamento_Id ,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, f=>f.Ciudad_Empresa.Ciudad.departmento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,m=>m.DetalleTipoCable.TipoCable,n=>n.Elemento.Equipos, o=>o.Elemento.Fotos, q=>q.Elemento.Novedades );
            }
            //Todas las empresas de una ciudad
            else if(Ciudad_Id!=0 && Empresa_Id==0){
                cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad, l=>l.DetalleTipoCable.Cable,m=>m.DetalleTipoCable.TipoCable,n=>n.Elemento.Equipos, o=>o.Elemento.Fotos,q=>q.Elemento.Novedades);
            }
            else{
                cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id && a.Empresa_Id==Empresa_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,m=>m.DetalleTipoCable.TipoCable,n=>n.Elemento.Equipos, o=>o.Elemento.Fotos,q=>q.Elemento.Novedades);
            }
           var filterListCablesDates=cables.ToList().Where(a=>a.Elemento.FechaLevantamiento.Day>=SelectedDateStart.Day && 
                                                a.Elemento.FechaLevantamiento.Month>=SelectedDateStart.Month &&
                                                a.Elemento.FechaLevantamiento.Year>=SelectedDateStart.Year &&
                                                a.Elemento.FechaLevantamiento.Day<=SelectedDateEnd.Day && 
                                                a.Elemento.FechaLevantamiento.Month<=SelectedDateEnd.Month &&
                                                a.Elemento.FechaLevantamiento.Year<=SelectedDateEnd.Year ).ToList();
           
          
            var list= new List<ElementoViewModel>();

           /// var MaximumPrice=0;
            foreach(var item in filterListCablesDates){

                //var sumdata= filterListCablesDates.Count/100;
                ///MaximumPrice+=sumdata;

                var viewModelMap = _mapper.Map<Elemento, ElementoViewModel>(item.Elemento);
                viewModelMap.EmpresaOperadora_Id=item.Empresa_Id;
                viewModelMap.NombreEmpresaOperadora=item.Ciudad_Empresa.Empresa.Nombre;
                viewModelMap.NombreCiudad=item.Ciudad_Empresa.Ciudad.Nombre;
                viewModelMap.Direccion= item.Elemento.LocalizacionElementos.FirstOrDefault().Direccion;
                viewModelMap.Detalle_Cable= item.DetalleTipoCable.Cable.Nombre;
                viewModelMap.Tipo_Cable= item.DetalleTipoCable.TipoCable.Nombre;
                viewModelMap.Cantidad_Cable= item.Cantidad.ToString();

                viewModelMap.Equipos.Clear();
                viewModelMap.Fotos.Clear();

                if(viewModelMap.CodigoApoyo==null || viewModelMap.CodigoApoyo==""){
                    //Verificar la novedad del codigo de apoyo vacio
                    var novedad= await _novedadRepository.GetSingleAsync(a=>a.Elemento_Id==item.Elemento.Id && a.DetalleTipoNovedad.Tipo_Novedad_id==1, b=>b.DetalleTipoNovedad);
                    if(novedad!=null){
                        if(novedad.Detalle_Tipo_Novedad_Id!=3){
                            viewModelMap.CodigoApoyo=novedad.DetalleTipoNovedad.Nombre;
                        }
                    }
                }
                //list Fotos
                var listFotos= new List<FotoViewModel>();

                var listFotosPoste= _fotoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Elemento_Id);
                listFotos= _mapper.Map<IEnumerable<Foto>, IEnumerable<FotoViewModel>>(listFotosPoste.Result).ToList();
                viewModelMap.Fotos =listFotos;
                
                foreach(var equipo in item.Elemento.Equipos){

                    var queryequipo= await _equipoElementoRepository.GetSingleAsync(a=>a.Id==equipo.Id, b=>b.TipoEquipo,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa);
                    var mapEquipo= _mapper.Map<EquipoElemento, EquipoViewModel>(queryequipo);
                  
                    mapEquipo.NombreEmpresaEquipo=mapEquipo.Ciudad_Empresa.Empresa.Nombre;
                        var ConectadoRbt="NO";
                        var MedidorBt="NO";

                        if( equipo.ConectadoRbt){
                            ConectadoRbt="SI";
                        }

                        if(equipo.MedidorBt){
                            MedidorBt="SI";
                        }

                        mapEquipo.ConectadoRbt= ConectadoRbt;
                        mapEquipo.MedidorBt= MedidorBt;
                        mapEquipo.TipoEquipo=equipo.TipoEquipo;

                        viewModelMap.Equipos.Add(mapEquipo);
                }

                var sobrerbt="NO";
                var tienemarquilla="NO";

                if( item.SobreRbt){
                    sobrerbt="SI";
                }

                if(item.Tiene_Marquilla){
                    tienemarquilla="SI";
                }

                viewModelMap.SobreRbt= sobrerbt;
                viewModelMap.Tiene_Marquilla= tienemarquilla;


                var coords= string.Empty;
                if(item.Elemento.LocalizacionElementos.FirstOrDefault().Latitud>0 && item.Elemento.LocalizacionElementos.FirstOrDefault().Longitud>0){
                    var latitud=  string.Format("{0:N6}", item.Elemento.LocalizacionElementos.FirstOrDefault().Latitud);
                    var longitud=  string.Format("{0:N6}", item.Elemento.LocalizacionElementos.FirstOrDefault().Longitud);
                    coords= string.Format("{0},{1}",latitud,longitud);
                }else{
                     coords= string.Format("{0},{1}",item.Elemento.LocalizacionElementos.FirstOrDefault().Latitud,item.Elemento.LocalizacionElementos.FirstOrDefault().Longitud);
                }
                viewModelMap.Coordenadas= coords;

                list.Add(viewModelMap);
            }

            return  list.AsQueryable();

        }
        */



        private async Task<IQueryable<ElementoReportViewModel>> ReportDetalladoView(){
            //IEnumerable<ElementoCable> cables=Enumerable.Empty<ElementoCable>();
            var dataContext = MyAppContext.GetInstance();
            IEnumerable<View_Cable_Report> cables=Enumerable.Empty<View_Cable_Report>();

            if(Ciudad_Id==0 && Empresa_Id==0){
                cables = dataContext.View_Cables.Where(a=>a.Departamento_Id==Departamento_Id).ToList();
                //cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Empresa.Ciudad.departmento.Id==Departamento_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, f=>f.Ciudad_Empresa.Ciudad.departmento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
            //Todas las empresas de una ciudad
            else if(Ciudad_Id!=0 && Empresa_Id==0){
                cables = dataContext.View_Cables.Where(a=>a.Ciudad_Id==Ciudad_Id).ToList();
            }
            else{
                cables = dataContext.View_Cables.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Empresa_Id==Empresa_Id).ToList();
               // cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id && a.Empresa_Id==Empresa_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
          
           var filterListCablesDates=cables.ToList().Where(a=>a.FechaLevantamiento.Day>=SelectedDateStart.Day && 
                                                a.FechaLevantamiento.Month>=SelectedDateStart.Month &&
                                                a.FechaLevantamiento.Year>=SelectedDateStart.Year &&
                                                a.FechaLevantamiento.Day<=SelectedDateEnd.Day && 
                                                a.FechaLevantamiento.Month<=SelectedDateEnd.Month &&
                                                a.FechaLevantamiento.Year<=SelectedDateEnd.Year ).ToList();

            var list= new List<ElementoReportViewModel>();
            foreach(var item in filterListCablesDates){
                var viewModelMap = _mapper.Map<View_Cable_Report, ElementoReportViewModel>(item);
                  viewModelMap.NumeroApoyo= item.Elemento_Id;
               /* var coords= string.Empty;
                if(item.Latitud_Elemento>0 &&item.Longitud_Elemento>0){
                    var latitud=  string.Format("{0:N6}", item.Latitud_Elemento);
                    var longitud=  string.Format("{0:N6}", item.Longitud_Elemento);
                    coords= string.Format("{0},{1}",latitud,longitud);
                }else{
                     coords= string.Format("{0},{1}",item.Latitud_Elemento,item.Longitud_Elemento);
                }
                viewModelMap.Coordenadas_Elemento= coords;*/

                if(viewModelMap.CodigoApoyo==null || viewModelMap.CodigoApoyo==""){
                    //Verificar la novedad del codigo de apoyo vacio
                    var novedad= await _novedadRepository.GetSingleAsync(a=>a.Elemento_Id==item.Elemento_Id && a.DetalleTipoNovedad.Tipo_Novedad_id==1, b=>b.DetalleTipoNovedad);
                    if(novedad!=null){
                        if(novedad.Detalle_Tipo_Novedad_Id!=3){
                            viewModelMap.CodigoApoyo=novedad.DetalleTipoNovedad.Nombre;
                        }
                    }
                }
        
                //list Fotos
                var listFotos= new List<FotoViewModel>();
                var listFotosPoste= _fotoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Elemento_Id);
                listFotos= _mapper.Map<IEnumerable<Foto>, IEnumerable<FotoViewModel>>(listFotosPoste.Result).ToList();
                viewModelMap.Fotos =listFotos;

                var equiposElementos= await _equipoElementoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Elemento_Id, b=>b.TipoEquipo,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa);
                viewModelMap.Equipos= new List<EquipoViewModel>();
                foreach(var queryequipo in equiposElementos){
                    var mapEquipo= _mapper.Map<EquipoElemento, EquipoViewModel>(queryequipo);
                    mapEquipo.NombreEmpresaEquipo=mapEquipo.Ciudad_Empresa.Empresa.Nombre;    
                    viewModelMap.Equipos.Add(mapEquipo);
                }

                



                list.Add(viewModelMap);
            }
            return  list.AsQueryable();
        }

        private  IQueryable<ElementoReportViewModel> GetQueryableElementosDetallado()
        {
            if(ReportElementos.Count>0){
                IsVisibleExportPdf=true;
              
                if(Empresa_Id>0){
                    IsVisibleExportExcell=true;
                }
            }else{
                 IsVisibleExportPdf=false;
                 IsVisibleExportExcell=false;
               
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
            string date_start_report = String.Format("{0:dd/MM/yyyy}", SelectedDateStart);
            string date_end_report = String.Format("{0:dd/MM/yyyy}", SelectedDateEnd);
            ElementosDetallePdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream, ReportElementos,date_start_report,date_end_report);
        
            var empresa= RemoveDiacritics(SelectedCiudad.Nombre);

            //Revisar nombres con tildes
            Context.ReturnFile(outputStream, string.Format("report_detalle_{0}.pdf",empresa), "application/pdf");
        }

        static string RemoveDiacritics(string text) 
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public void ExporGeneralPdf(){
            var outputStream = new MemoryStream();

            string date_start_report = String.Format("{0:dd/MM/yyyy}", SelectedDateStart);
            string date_end_report = String.Format("{0:dd/MM/yyyy}", SelectedDateEnd);

             var empresa= RemoveDiacritics(SelectedCiudad.Nombre);


         ElementosPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream, ReportElementos,date_start_report,date_end_report);
          Context.ReturnFile(outputStream, string.Format("report_detalle_{0}.pdf",empresa), "application/pdf");
        }
        
        public void ExportExcel(){
            var outputStream = new MemoryStream();
            ElementosPdfReport.CreateInMemoryPdfReport(_hostingEnvironment.WebRootPath, ReportElementos);
            Context.ReturnFile(outputStream,  string.Format("report_{0}.xlsx",SelectedCiudad.Nombre), "application/ms-excel");
        }

        private async Task<IQueryable<ElementoViewModel>> GetQueryable(int size)
        {
            var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
            var model = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(elementos);
            return  model.AsQueryable();
        }

    public void LLenarExcel()
    {

        string empresa= string.Empty;
        string sWebRootFolder = _hostingEnvironment.WebRootPath;
        string sFileName = @"INVENTARIO_CABLEOPERADORES.xlsx";
        FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        var memory = new MemoryStream();
        if (!file.Exists)
        {
            /* 
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Demo");
                IRow row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("ID");
                row.CreateCell(1).SetCellValue("Name");
                row.CreateCell(2).SetCellValue("Age");
                row = excelSheet.CreateRow(1);
                row.CreateCell(0).SetCellValue(1);
                row.CreateCell(1).SetCellValue("Kane Williamson");
                row.CreateCell(2).SetCellValue(29);
                row = excelSheet.CreateRow(2);
                row.CreateCell(0).SetCellValue(2);
                row.CreateCell(1).SetCellValue("Martin Guptil");
                row.CreateCell(2).SetCellValue(33);
                row = excelSheet.CreateRow(3);
                row.CreateCell(0).SetCellValue(3);
                row.CreateCell(1).SetCellValue("Colin Munro");
                row.CreateCell(2).SetCellValue(23);
                workbook.Write(fs);
                fs.Close();
            }*/
        }else{
            //Limpiar
            using (
                FileStream rstrEmpty = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbookEmpty;
                   // rstr.CopyTo(memory);
                  
                   // memory.Position = 0;
                   workbookEmpty = new XSSFWorkbook(rstrEmpty);
                   ISheet excelSheetEmpty = workbookEmpty.GetSheet("Inventario");

                    var style1 = workbookEmpty.CreateCellStyle();
                    style1.BorderBottom = BorderStyle.Thin;
                    style1.BorderLeft =BorderStyle.Thin;
                    style1.BorderRight =BorderStyle.Thin;
                    style1.BorderTop =BorderStyle.Thin;

                   using (FileStream wstrEmpty = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                   {
                    //Realizar consulta y recorrer por cada elemento
                    //llenar el archivo de excel
                    
                    var l=0;
                    for(int i=0; i<=14000;i++){
                        //IRow row = excelSheetEmpty.GetRow(11+l);
                        ///row.Height = 20;
                        var cell = excelSheetEmpty.CreateRow(11+l);
                        cell.Height=30 * 13;
                        cell.CreateCell(0).SetCellValue("");
                        cell.GetCell(0).CellStyle=style1;

                        cell.CreateCell(1).SetCellValue("");
                        cell.GetCell(1).CellStyle=style1;

                        cell.CreateCell(2).SetCellValue("");
                        cell.GetCell(2).CellStyle=style1;

                        cell.CreateCell(3).SetCellValue("");
                        cell.GetCell(3).CellStyle=style1;

                        cell.CreateCell(4).SetCellValue("");
                        cell.GetCell(4).CellStyle=style1;

                        cell.CreateCell(5).SetCellValue("");
                        cell.GetCell(5).CellStyle=style1;

                        cell.CreateCell(6).SetCellValue("");
                        cell.GetCell(6).CellStyle=style1;

                        cell.CreateCell(7).SetCellValue("");
                        cell.GetCell(7).CellStyle=style1;

                        cell.CreateCell(8).SetCellValue("");
                        cell.GetCell(8).CellStyle=style1;

                        cell.CreateCell(9).SetCellValue("");
                        cell.GetCell(9).CellStyle=style1;

                        cell.CreateCell(10).SetCellValue("");
                        cell.GetCell(10).CellStyle=style1;
                        
                        cell.CreateCell(11).SetCellValue("");
                        cell.GetCell(11).CellStyle=style1;

                        cell.CreateCell(12).SetCellValue("");
                        cell.GetCell(12).CellStyle=style1;

                        cell.CreateCell(13).SetCellValue("");
                        cell.GetCell(13).CellStyle=style1;

                        cell.CreateCell(14).SetCellValue("");
                        cell.GetCell(14).CellStyle=style1;

                        cell.CreateCell(15).SetCellValue("");
                        cell.GetCell(15).CellStyle=style1;

                        cell.CreateCell(16).SetCellValue("");
                        cell.GetCell(16).CellStyle=style1;

                        cell.CreateCell(17).SetCellValue("");
                        cell.GetCell(17).CellStyle=style1;

                        cell.CreateCell(18).SetCellValue("");
                        cell.GetCell(18).CellStyle=style1;

                        cell.CreateCell(19).SetCellValue("");
                        cell.GetCell(19).CellStyle=style1;

                         cell.CreateCell(20).SetCellValue("");
                        cell.GetCell(20).CellStyle=style1;

                        l++;
                    }
                    workbookEmpty.Write(wstrEmpty);
                    wstrEmpty.Close();
                    rstrEmpty.Close();
                   }
                }
          
                using (
                FileStream rstr = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook;
                   // rstr.CopyTo(memory);
                   // memory.Position = 0;
                    workbook = new XSSFWorkbook(rstr);
                    
                   ISheet excelSheet = workbook.GetSheet("Inventario");
                   using (FileStream wstr = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
                   {
                    //Realizar consulta y recorrer por cada elemento
                    //llenar el archivo de excel

                    var j = 0;


                    var dataList= new List<ElementoReportViewModel>();
                    dataList=  ReportElementos.ToList();

                    if(dataList.Count>0){
                    
                        IRow FechaLevantamiento = excelSheet.GetRow(3);
                        FechaLevantamiento.GetCell(2).SetCellValue(string.Format("{0:dd/MM/yyyy}", dataList.FirstOrDefault().FechaLevantamiento));

                        IRow Ciudad = excelSheet.GetRow(4);
                        Ciudad.GetCell(2).SetCellValue(string.Format("{0}", dataList.FirstOrDefault().Ciudad));

                        IRow Empresa = excelSheet.GetRow(5);
                        Empresa.GetCell(2).SetCellValue(string.Format("{0}", dataList.FirstOrDefault().Nombre_Empresa));

                        empresa= dataList.FirstOrDefault().Nombre_Empresa;
                        
                    }else{
                        IRow FechaLevantamiento = excelSheet.GetRow(3);
                        FechaLevantamiento.GetCell(2).SetCellValue("");

                        IRow Ciudad = excelSheet.GetRow(4);
                        Ciudad.GetCell(2).SetCellValue("");

                        IRow Empresa = excelSheet.GetRow(5);
                        Empresa.GetCell(2).SetCellValue("");

                         empresa= "";
                    }

                 

                    /* 
                    var style2 = workbook.CreateCellStyle();
                    style2.FillForegroundColor = HSSFColor.Yellow.Index2;
                    style2.FillPattern = FillPattern.SolidForeground;*/

                    //var numero_apoyo=0;
                    foreach (var item in dataList)
                    {       
                       // numero_apoyo= numero_apoyo+1;
                            //for(int i=0; i<=dataList.Count;i++){
                            IRow row = excelSheet.GetRow(11+j);
                            row.GetCell(0).SetCellValue(item.Elemento_Id);
                            row.GetCell(1).SetCellValue(item.CodigoApoyo);
                            row.GetCell(2).SetCellValue(item.Sigla_Material);
                            row.GetCell(3).SetCellValue(item.Longitud);
                            row.GetCell(4).SetCellValue(item.ResistenciaMecanica);
                            row.GetCell(5).SetCellValue(item.Sigla_Estado);
                            row.GetCell(6).SetCellValue(item.Valor_Nivel_Tension);
                            row.GetCell(7).SetCellValue(item.AlturaDisponible);
                            row.GetCell(8).SetCellValue("");
                            row.GetCell(9).SetCellValue(item.SobreRbt);
                            row.GetCell(10).SetCellValue(item.Cantidad_Cable);
                            row.GetCell(11).SetCellValue("");
                            row.GetCell(12).SetCellValue(item.Nombre_Cable);
                            row.GetCell(13).SetCellValue(item.Tiene_Amplificador);
                            row.GetCell(14).SetCellValue(item.Tiene_Fuente);
                            row.GetCell(15).SetCellValue(item.Tiene_DistribuidorFibra);
                            row.GetCell(16).SetCellValue(item.ConectadoRbt);
                            row.GetCell(17).SetCellValue(item.Otro_Equipo);
                            row.GetCell(18).SetCellValue("");
                            row.GetCell(19).SetCellValue(item.Direccion_Elemento);
                            j++;
                       // }
                    }
                    workbook.Write(wstr);
                    wstr.Close();
                    rstr.Close();
                   }
                }
            }

        var archivo=file.OpenRead();

        var empresaNameFormated= RemoveDiacritics(empresa);

        Context.ReturnFile(archivo, string.Format("Inventario_{0}.xlsx",empresaNameFormated), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
     }





     public void LLenarExcelPlano()
    {

        if(ReportElementos!=null){

string empresa= string.Empty;
         
        string sWebRootFolder = _hostingEnvironment.WebRootPath;
        string sFileName = @"Inventario_Plano_.xlsx";
        FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        var memory = new MemoryStream();
        /*
           var outputStream = new MemoryStream();
            ElementosPdfReport.CreateInMemoryPdfReport(_hostingEnvironment.WebRootPath, ReportElementos);
            Context.ReturnFile(outputStream,  string.Format("report_{0}.xlsx",SelectedCiudad.Nombre), "application/ms-excel");*/

        if (file.Exists)
        {
            file.Delete();
            file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
        }

       
            using (var fs = new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook;
                workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Inventario");
                IRow row = excelSheet.CreateRow(0);
                  row.Height=30 * 13;

                var style1 = workbook.CreateCellStyle();
                  style1.FillForegroundColor = HSSFColor.Orange.Index;
                  style1.FillPattern = FillPattern.SolidForeground;
                
                    /* 
                    foreach (DataColumn column in ReportElementos.ToList())
                    {
                        int rowIndex = 0;
                        foreach (DataRow row2 in ReportElementos.ToList())
                        {
                            HSSFRow dataRow = sheet1.CreateRow(rowIndex);
                            dataRow.CreateCell(column.Ordinal).SetCellValue(row2[column].ToString());
                            rowIndex++;
                        }
                        sheet1.AutoSizeColumn(column.Ordinal);
                    }
                    */

                row.CreateCell(0).SetCellValue("Numero Apoyo");
                row.GetCell(0).CellStyle=style1;
             
               

                row.CreateCell(1).SetCellValue("Codigo Apoyo");
                row.GetCell(1).CellStyle=style1;
 

                row.CreateCell(2).SetCellValue("Long. Poste");
                row.GetCell(2).CellStyle=style1;

                row.CreateCell(3).SetCellValue("Estado");
                row.GetCell(3).CellStyle=style1;

                row.CreateCell(4).SetCellValue("Nivel Tension");
                row.GetCell(4).CellStyle=style1;

                row.CreateCell(5).SetCellValue("Altura Disponible");
                row.GetCell(5).CellStyle=style1;

                row.CreateCell(6).SetCellValue("Resistencia Mecanica");
                row.GetCell(6).CellStyle=style1;

                row.CreateCell(7).SetCellValue("Material");
                row.GetCell(7).CellStyle=style1;

                row.CreateCell(8).SetCellValue("Retenidas");
                row.GetCell(8).CellStyle=style1;

                row.CreateCell(9).SetCellValue("Direccion");
                row.GetCell(9).CellStyle=style1;
                
                row.CreateCell(10).SetCellValue("Coordenadas");
                row.GetCell(10).CellStyle=style1;

                row.CreateCell(11).SetCellValue("Cable");
                row.GetCell(11).CellStyle=style1;

                row.CreateCell(12).SetCellValue("Tipo Cable");
                row.GetCell(12).CellStyle=style1;

                row.CreateCell(13).SetCellValue("Operador");
                row.GetCell(13).CellStyle=style1;

                row.CreateCell(14).SetCellValue("Nivel Ocupaion");
                row.GetCell(14).CellStyle=style1;

                row.CreateCell(15).SetCellValue("Esta el cable sobre RBT ?");
                row.GetCell(15).CellStyle=style1;

                row.CreateCell(16).SetCellValue("El cable cuenta con marquilla ?");
                row.GetCell(16).CellStyle=style1;
            

                row.CreateCell(17).SetCellValue("Amplificador");
                row.GetCell(17).CellStyle=style1;

                row.CreateCell(18).SetCellValue("Fuente");
                row.GetCell(18).CellStyle=style1;

                row.CreateCell(19).SetCellValue("Distribuidor de fibra");
                row.GetCell(19).CellStyle=style1;

                row.CreateCell(20).SetCellValue("Conectado a red electrica");
                row.GetCell(20).CellStyle=style1;

                row.CreateCell(21).SetCellValue("Operador del Equipo");
                row.GetCell(21).CellStyle=style1;

                var dataList= new List<ElementoReportViewModel>();
               
                dataList=  ReportElementos.ToList();
                 empresa= dataList.FirstOrDefault().Nombre_Empresa;
                var j = 1;
                foreach (var item in dataList)
                {       
                       // numero_apoyo= numero_apoyo+1;
                            //for(int i=0; i<=dataList.Count;i++){

                            IRow rowData =excelSheet.CreateRow(j);

                            //row = excelSheet.CreateRow(j);
                            rowData.CreateCell(0).SetCellValue(item.Elemento_Id);
                            rowData.CreateCell(1).SetCellValue(item.CodigoApoyo);
                            rowData.CreateCell(2).SetCellValue(item.Longitud);
                            rowData.CreateCell(3).SetCellValue(item.Nombre_Estado);
                            rowData.CreateCell(4).SetCellValue(item.Sigla_Nivel_Tension);
                            rowData.CreateCell(5).SetCellValue(item.AlturaDisponible);
                            rowData.CreateCell(6).SetCellValue(item.ResistenciaMecanica);
                            rowData.CreateCell(7).SetCellValue(item.Nombre_Material);
                            rowData.CreateCell(8).SetCellValue(item.Retenidas);
                            rowData.CreateCell(9).SetCellValue(item.Direccion_Elemento);
                            rowData.CreateCell(10).SetCellValue(item.Coordenadas_Elemento);
                            rowData.CreateCell(11).SetCellValue(item.Nombre_Cable);
                            rowData.CreateCell(12).SetCellValue(item.Nombre_Tipo_Cable);
                            rowData.CreateCell(13).SetCellValue(item.Nombre_Empresa);
                            rowData.CreateCell(14).SetCellValue(item.Cantidad_Cable);
                            rowData.CreateCell(15).SetCellValue(item.SobreRbt);
                            rowData.CreateCell(16).SetCellValue(item.Tiene_Marquilla);
                            rowData.CreateCell(17).SetCellValue(item.Tiene_Amplificador);
                            rowData.CreateCell(18).SetCellValue(item.Tiene_Fuente);
                            rowData.CreateCell(19).SetCellValue(item.Tiene_DistribuidorFibra);
                            rowData.CreateCell(20).SetCellValue(item.ConectadoRbt);
                            rowData.CreateCell(21).SetCellValue(item.Empresa_Equipo);

                            j++;
                       // }
                }


                int numberOfColumns = excelSheet.GetRow(1).PhysicalNumberOfCells;
                for (int i = 1; i <= numberOfColumns; i++)
                {
                    excelSheet.AutoSizeColumn(i);
                    GC.Collect(); // Add this line
                }
                workbook.Write(fs);
                fs.Close();
            }
        

        
        var archivo=file.OpenRead();

        string date_start_report = String.Format("{0:dd/MM/yyyy}", SelectedDateStart);
        string date_end_report = String.Format("{0:dd/MM/yyyy}", SelectedDateEnd);
        
        string replaceDateStart=date_start_report.Replace("/","_");
        string replaceDateEnd=date_end_report.Replace("/","_");

        if(Empresa_Id>0){
            var empresaNameFormated= RemoveDiacritics(empresa);
            Context.ReturnFile(archivo, string.Format("Inventario_Plano_{0}_DE_{1}_A_{2}.xlsx",empresaNameFormated,replaceDateStart,replaceDateEnd), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }else{
        
            Context.ReturnFile(archivo, string.Format("Inventario_Plano_(TODAS_LAS_EMPRESAS)_DE_{0}_A_{1}.xlsx",replaceDateStart,replaceDateEnd), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    
        }
        }
     }

        #endregion

    }
}