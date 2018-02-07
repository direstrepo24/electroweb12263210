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
    public class ViewModelReportUsuario:BaseViewModel
    {
         #region Atributes
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
        public List<Usuario> Usuarios {get; set;}= new List<Usuario>();
         public Usuario SelectedUsuario { get; set; }


        public GridViewUserSettings UserSettings { get; set; }

        public DateTime SelectedDateStart { get; set; }=DateTime.Now;

        public DateTime SelectedDateEnd { get; set; }=DateTime.Now;

         public bool Limpiar { get; set; }=true;

    

        public long Empresa_Id { get; set; }
        public long Ciudad_Id { get; set; }
        public long Departamento_Id { get; set; }

        public long Usuario_Id { get; set; }

         
        public string ReportType { get; set; }="General";

        

          public bool VisibleRadioButtonDetallado { get; set; }=true;
           public bool IsVisibleExportPdf { get; set; }=false;

          

        #endregion

        #region Contructor
        public ViewModelReportUsuario(
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
        IUsuarioRepository usuarioRepository
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

         public void UserChange()
        {
            if(SelectedUsuario!=null) {
                Usuario_Id= SelectedUsuario.Id;
             
            }else{
                Usuario_Id=-1;
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


            var listusuarios= GetQueryableUsuario().Result;
            Usuarios= listusuarios.ToList();
            Usuarios.Add(new Usuario{
                    Id=0,
                    Empresa_Id=0,
                    Nombre="Todos los usuarios",
                    Apellido="",
                    Cedula="",
                    CorreoElectronico="",
                    Direccion="",
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

          private async Task<IQueryable<Usuario>> GetQueryableUsuario()
        {
            var usuario= await _usuarioRepository.AllIncludingAsync();
            return  usuario.AsQueryable();

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
                        ColumnName = "Id",
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
            IEnumerable<Elemento> elementos=Enumerable.Empty<Elemento>();
             //Todas las ciudades y todos los usuarios
            if(Ciudad_Id==0 && Usuario_Id==0 ){
                elementos =await _IelementosRepository.AllIncludingAsyncWhere(a=>a.Ciudad.departmentoId==Departamento_Id,c=>c.Material,d=>d.Proyecto, f=>f.LocalizacionElementos, g=>g.Estado, h=>h.NivelTensionElemento, i=>i.LongitudElemento,k=>k.Ciudad, l=>l.Ciudad.departmento,k=>k.Cables);
            }
            //Todas los usuarios de una ciudad
            else if(Ciudad_Id!=0 && Usuario_Id==0){
                elementos =await _IelementosRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id,c=>c.Material,d=>d.Proyecto, f=>f.LocalizacionElementos, g=>g.Estado, h=>h.NivelTensionElemento, i=>i.LongitudElemento,k=>k.Ciudad, l=>l.Ciudad.departmento,k=>k.Cables);
            }
            else{
                elementos =await _IelementosRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id && a.Usuario_Id==Usuario_Id,c=>c.Material,d=>d.Proyecto,  f=>f.LocalizacionElementos, g=>g.Estado, h=>h.NivelTensionElemento, i=>i.LongitudElemento,k=>k.Ciudad, l=>l.Ciudad.departmento,k=>k.Cables);
            }

           var filterListElementosDates=elementos.ToList().Where(a=>a.FechaLevantamiento.Day>=SelectedDateStart.Day && 
                                                a.FechaLevantamiento.Month>=SelectedDateStart.Month &&
                                                a.FechaLevantamiento.Year>=SelectedDateStart.Year &&
                                                a.FechaLevantamiento.Day<=SelectedDateEnd.Day && 
                                                a.FechaLevantamiento.Month<=SelectedDateEnd.Month &&
                                                a.FechaLevantamiento.Year<=SelectedDateEnd.Year ).ToList();
                                                
           

           // var viewModelMap = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(filterListCablesDates);
              var list= new List<ElementoViewModel>();

            foreach(var item in filterListElementosDates){
                var viewModelMap = _mapper.Map<Elemento, ElementoViewModel>(item);
                viewModelMap.NombreCiudad=item.Ciudad.Nombre;
                viewModelMap.Direccion= item.LocalizacionElementos.FirstOrDefault().Direccion;
                string hourStartFormat = DateTime.ParseExact(item.HoraInicio,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                string hourEndFormat = DateTime.ParseExact(item.HoraFin,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");


                var usuario= await _usuarioRepository.GetSingleAsync(a=>a.Id==item.Usuario_Id);
                viewModelMap.HoraInicio=hourStartFormat;
                viewModelMap.HoraFin=hourEndFormat;
                viewModelMap.Usuario = string.Format("{0} {1}",usuario.Nombre, usuario.Apellido);

                var coords= string.Empty;
                if(item.LocalizacionElementos.FirstOrDefault().Latitud>0 && item.LocalizacionElementos.FirstOrDefault().Longitud>0){
                    var latitud=  string.Format("{0:N6}", item.LocalizacionElementos.FirstOrDefault().Latitud);
                    var longitud=  string.Format("{0:N6}", item.LocalizacionElementos.FirstOrDefault().Longitud);
                    coords= string.Format("{0},{1}",latitud,longitud);
                }else{
                     coords= string.Format("{0},{1}",item.LocalizacionElementos.FirstOrDefault().Latitud,item.LocalizacionElementos.FirstOrDefault().Longitud);
                }
                
                 viewModelMap.Coordenadas= item.LocalizacionElementos.Where(a=>a.Element_Id==item.Id).FirstOrDefault().Coordenadas;
                list.Add(viewModelMap);
            }


           
             return  list.AsQueryable();
        }*/

        private async Task<IQueryable<ElementoReportViewModel>> ReportGeneralView(){
            //IEnumerable<ElementoCable> cables=Enumerable.Empty<ElementoCable>();
            var dataContext = MyAppContext.GetInstance();
            IEnumerable<View_Elemento_Report> cables=Enumerable.Empty<View_Elemento_Report>();
            if(Ciudad_Id==0 && Usuario_Id==0){
                cables = dataContext.View_Elementos.Where(a=>a.Departamento_Id==Departamento_Id).ToList();
                //cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Empresa.Ciudad.departmento.Id==Departamento_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, f=>f.Ciudad_Empresa.Ciudad.departmento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
            //Todas las empresas de una ciudad
            else if(Ciudad_Id!=0 && Usuario_Id==0){
                cables = dataContext.View_Elementos.Where(a=>a.Ciudad_Id==Ciudad_Id).ToList();
            }
             else if(Ciudad_Id==0 && Usuario_Id!=0){
                cables = dataContext.View_Elementos.Where(a=>a.Departamento_Id==Departamento_Id && a.Usuario_Id==Usuario_Id).ToList();
            }

            else{
                cables = dataContext.View_Elementos.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Usuario_Id==Usuario_Id).ToList();
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
                var viewModelMap = _mapper.Map<View_Elemento_Report, ElementoReportViewModel>(item);
                string hourStartFormat = DateTime.ParseExact(item.HoraInicio,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                string hourEndFormat = DateTime.ParseExact(item.HoraFin,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                viewModelMap.HoraInicio=hourStartFormat;
                viewModelMap.HoraFin=hourEndFormat;
                viewModelMap.Usuario = string.Format("{0} {1}",item.Nombre_Usuario, item.Apellido_Usuario);
                 viewModelMap.Elemento_Id=item.Id;

                 viewModelMap.FechaLevantamientoFormat = item.FechaLevantamiento.ToString("dd/MM/yyyy");
            
                  var cablesElementos= await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Id);
                  var cables_map= _mapper.Map<IEnumerable<ElementoCable>, IEnumerable<ElementoCableViewModel>>(cablesElementos.ToList());
                  viewModelMap.Cables= new List<ElementoCableViewModel>();
                  viewModelMap.Cables= cables_map.ToList();

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
         /*private async Task<IQueryable<ElementoViewModel>> ReportDetallado(){
             // var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
            IEnumerable<Elemento> elementos=Enumerable.Empty<Elemento>();
             //Todas las ciudades y todos los usuarios
            if(Ciudad_Id==0 && Usuario_Id==0 ){
                elementos =await _IelementosRepository.AllIncludingAsyncWhere(a=>a.Ciudad.departmentoId==Departamento_Id,c=>c.Material,d=>d.Proyecto, f=>f.LocalizacionElementos, g=>g.Estado, h=>h.NivelTensionElemento, i=>i.LongitudElemento,k=>k.Ciudad, l=>l.Ciudad.departmento,k=>k.Cables,n=>n.Equipos, o=>o.Fotos, q=>q.Novedades);
            }
            //Todas los usuarios de una ciudad
            else if(Ciudad_Id!=0 && Usuario_Id==0){
                elementos =await _IelementosRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id,c=>c.Material,d=>d.Proyecto, f=>f.LocalizacionElementos, g=>g.Estado, h=>h.NivelTensionElemento, i=>i.LongitudElemento,k=>k.Ciudad, l=>l.Ciudad.departmento,k=>k.Cables,n=>n.Equipos, o=>o.Fotos, q=>q.Novedades);
            }
            else{
                elementos =await _IelementosRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Id==Ciudad_Id && a.Usuario_Id==Usuario_Id,c=>c.Material,d=>d.Proyecto,  f=>f.LocalizacionElementos, g=>g.Estado, h=>h.NivelTensionElemento, i=>i.LongitudElemento,k=>k.Ciudad, l=>l.Ciudad.departmento,k=>k.Cables,n=>n.Equipos, o=>o.Fotos, q=>q.Novedades);
            }

           var filterListElementosDates=elementos.ToList().Where(a=>a.FechaLevantamiento.Day>=SelectedDateStart.Day && 
                                                a.FechaLevantamiento.Month>=SelectedDateStart.Month &&
                                                a.FechaLevantamiento.Year>=SelectedDateStart.Year &&
                                                a.FechaLevantamiento.Day<=SelectedDateEnd.Day && 
                                                a.FechaLevantamiento.Month<=SelectedDateEnd.Month &&
                                                a.FechaLevantamiento.Year<=SelectedDateEnd.Year ).ToList();
           
            var list= new List<ElementoViewModel>();

           /// var MaximumPrice=0;
            foreach(var item in filterListElementosDates){

                //var sumdata= filterListCablesDates.Count/100;
                ///MaximumPrice+=sumdata;

               var viewModelMap = _mapper.Map<Elemento, ElementoViewModel>(item);
                viewModelMap.NombreCiudad=item.Ciudad.Nombre;
                viewModelMap.Direccion= item.LocalizacionElementos.FirstOrDefault().Direccion;
                string hourStartFormat = DateTime.ParseExact(item.HoraInicio,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");
                string hourEndFormat = DateTime.ParseExact(item.HoraFin,"HH:mm",CultureInfo.CurrentCulture).ToString("hh:mm tt");

                var usuario= await _usuarioRepository.GetSingleAsync(a=>a.Id==item.Usuario_Id);
                viewModelMap.HoraInicio=hourStartFormat;
                viewModelMap.HoraFin=hourEndFormat;
                viewModelMap.Usuario = string.Format("{0} {1}",usuario.Nombre, usuario.Apellido);
                viewModelMap.Cables.Clear();
                viewModelMap.Equipos.Clear();
                viewModelMap.Fotos.Clear();

                //list Fotos
                var listFotos= new List<FotoViewModel>();
                var listFotosPoste= _fotoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Id);
                listFotos= _mapper.Map<IEnumerable<Foto>, IEnumerable<FotoViewModel>>(listFotosPoste.Result).ToList();
                viewModelMap.Fotos =listFotos;
            
                foreach(var equipo in item.Equipos){
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
                foreach(var cable in item.Cables){
                    var querycable= await _elementoCableRepository.GetSingleAsync(a=>a.Id==cable.Id, b=>b.DetalleTipoCable,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa,e=>e.DetalleTipoCable.Cable,f=>f.DetalleTipoCable.TipoCable);
                    var mapCable= _mapper.Map<ElementoCable, ElementoCableViewModel>(querycable);
                    var sobrerbt="NO";
                    var tienemarquilla="NO";

                    if( cable.SobreRbt){
                        sobrerbt="SI";
                    }
                    if(cable.Tiene_Marquilla){
                        tienemarquilla="SI";
                    }
                    mapCable.SobreRbt= sobrerbt;
                    mapCable.Tiene_Marquilla= tienemarquilla;
                  
                    mapCable.NombreEmpresaCable=mapCable.Ciudad_Empresa.Empresa.Nombre;
                    mapCable.TipoCable= mapCable.DetalleTipoCable.TipoCable.Nombre;
                    mapCable.DetalleCable= mapCable.DetalleTipoCable.Cable.Nombre;
                    viewModelMap.Cables.Add(mapCable);
                }
                viewModelMap.Coordenadas= item.LocalizacionElementos.Where(a=>a.Element_Id==item.Id).FirstOrDefault().Coordenadas;
                list.Add(viewModelMap);
            }
            
        
            /// var model = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(list);
            // ReportElementos=model.ToList();
         
            ReportElementos=list;
            //empresa.as
            return  list.AsQueryable();

         }*/
         private async Task<IQueryable<ElementoReportViewModel>> ReportDetalladoView(){
            //IEnumerable<ElementoCable> cables=Enumerable.Empty<ElementoCable>();
            var dataContext = MyAppContext.GetInstance();
            IEnumerable<View_Elemento_Report> cables=Enumerable.Empty<View_Elemento_Report>();

            if(Ciudad_Id==0 && Usuario_Id==0){
                cables = dataContext.View_Elementos.Where(a=>a.Departamento_Id==Departamento_Id).ToList();
                //cables =await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Ciudad_Empresa.Ciudad.departmento.Id==Departamento_Id,b=>b.Elemento,c=>c.Elemento.Material,d=>d.Elemento.Proyecto, e=>e.Elemento.Material, f=>f.Elemento.LocalizacionElementos, g=>g.Elemento.Estado, h=>h.Elemento.NivelTensionElemento, i=>i.Elemento.LongitudElemento, f=>f.Ciudad_Empresa.Ciudad.departmento,j=>j.Ciudad_Empresa.Empresa,k=>k.Ciudad_Empresa.Ciudad,l=>l.DetalleTipoCable.Cable,n=>n.Elemento.Equipos);
            }
            //Todas las empresas de una ciudad
            else if(Ciudad_Id!=0 && Usuario_Id==0){
                cables = dataContext.View_Elementos.Where(a=>a.Ciudad_Id==Ciudad_Id).ToList();
            }
            else if(Ciudad_Id==0 && Usuario_Id!=0){
                cables = dataContext.View_Elementos.Where(a=>a.Departamento_Id==Departamento_Id && a.Usuario_Id==Usuario_Id).ToList();
            }
            else{
                cables = dataContext.View_Elementos.Where(a=>a.Ciudad_Id==Ciudad_Id && a.Usuario_Id==Usuario_Id).ToList();
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
                var viewModelMap = _mapper.Map<View_Elemento_Report, ElementoReportViewModel>(item);
                viewModelMap.Elemento_Id=item.Id;
                viewModelMap.Usuario = string.Format("{0} {1}",item.Nombre_Usuario, item.Apellido_Usuario);
                if(viewModelMap.CodigoApoyo==null || viewModelMap.CodigoApoyo==""){
                    //Verificar la novedad del codigo de apoyo vacio
                    var novedad= await _novedadRepository.GetSingleAsync(a=>a.Elemento_Id==item.Id && a.DetalleTipoNovedad.Tipo_Novedad_id==1, b=>b.DetalleTipoNovedad);
                    if(novedad!=null){
                        if(novedad.Detalle_Tipo_Novedad_Id!=3){
                            viewModelMap.CodigoApoyo=novedad.DetalleTipoNovedad.Nombre;
                        }
                    }
                }

                viewModelMap.FechaLevantamientoFormat = item.FechaLevantamiento.ToString("dd/MM/yyyy");
        
                //list Fotos
                var listFotos= new List<FotoViewModel>();
                var listFotosPoste= _fotoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Id);
                listFotos= _mapper.Map<IEnumerable<Foto>, IEnumerable<FotoViewModel>>(listFotosPoste.Result).ToList();
                viewModelMap.Fotos =listFotos;

                var equiposElementos= await _equipoElementoRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Id, b=>b.TipoEquipo,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa);
                viewModelMap.Equipos= new List<EquipoViewModel>();
                foreach(var queryequipo in equiposElementos){
                    var mapEquipo= _mapper.Map<EquipoElemento, EquipoViewModel>(queryequipo);
                    mapEquipo.NombreEmpresaEquipo=mapEquipo.Ciudad_Empresa.Empresa.Nombre;    
                    viewModelMap.Equipos.Add(mapEquipo);
                }

                var cablesElementos= await _elementoCableRepository.AllIncludingAsyncWhere(a=>a.Elemento_Id==item.Id,b=>b.DetalleTipoCable,c=>c.Ciudad_Empresa, d=>d.Ciudad_Empresa.Empresa,e=>e.DetalleTipoCable.Cable,f=>f.DetalleTipoCable.TipoCable);
                  viewModelMap.Cables= new List<ElementoCableViewModel>();
                foreach(var cable in cablesElementos.ToList()){
                    var mapCable= _mapper.Map<ElementoCable, ElementoCableViewModel>(cable);
                    mapCable.NombreEmpresaCable=mapCable.Ciudad_Empresa.Empresa.Nombre;
                    mapCable.TipoCable= mapCable.DetalleTipoCable.TipoCable.Nombre;
                    mapCable.DetalleCable= mapCable.DetalleTipoCable.Cable.Nombre;
                    viewModelMap.Cables.Add(mapCable);
                }
                list.Add(viewModelMap);
            }
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
            ///DateTime date_start = DateTime.ParseExact(SelectedDateStart.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
            ///DateTime date_end = DateTime.ParseExact(SelectedDateEnd.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            string date_start_report = String.Format("{0:dd/MM/yyyy}", SelectedDateStart);
            string date_end_report = String.Format("{0:dd/MM/yyyy}", SelectedDateEnd);
           ElementosDetalleByUsuarioPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream, ReportElementos,date_start_report,date_end_report);
            Context.ReturnFile(outputStream, "report_detalle.pdf", "application/pdf");
        }

        public void ExportPdfUserPostes(){
            var outputStream = new MemoryStream();
            string date_start_report = String.Format("{0:dd/MM/yyyy}", SelectedDateStart);
            string date_end_report = String.Format("{0:dd/MM/yyyy}", SelectedDateEnd);
            ElementosGroupByDatesPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream, ReportElementos,date_start_report,date_end_report);
            Context.ReturnFile(outputStream, "reporte_general_contabilidad.pdf", "application/pdf");
        }

        

        public void ExporGeneralPdf(){
            var outputStream = new MemoryStream();
            string date_start_report = String.Format("{0:dd/MM/yyyy}", SelectedDateStart);
            string date_end_report = String.Format("{0:dd/MM/yyyy}", SelectedDateEnd);
            ElementosByUsuarioPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream, ReportElementos,date_start_report,date_end_report);
            Context.ReturnFile(outputStream, "reporte_general.pdf", "application/pdf");
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

        private async Task<IQueryable<ElementoViewModel>> GetQueryable(int size)
        {
            var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
            var model = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(elementos);
            return  model.AsQueryable();
        }

        #endregion
    }
}