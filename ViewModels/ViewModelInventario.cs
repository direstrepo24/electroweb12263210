using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotVVM.BusinessPack.Controls;
using DotVVM.BusinessPack.Export.Csv;
using DotVVM.Framework.Controls;
using Electro.model.DataContext;
using Electro.model.datatakemodel;
using Electro.model.Repository;
using electroweb.DTO;
//using PdfSharpCore.Drawing;
//using PdfSharpCore.Fonts;
//sing PdfSharpCore.Pdf;
using OfficeOpenXml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;

using Microsoft.AspNetCore.Hosting;
using electroweb.Reports;
using OfficeOpenXml.Style;
using System.Drawing;


using iTextSharp.text;
using iTextSharp.text.pdf;


namespace electroweb.ViewModels
{
    public class ViewModelInventario:BaseViewModel
    {
         private readonly IHostingEnvironment _hostingEnvironment;
        // var context = serviceProvider.GetRequiredService<MyAppContext>();
          // private readonly MyAppContext context;
           private Document document { get; set; }
             private readonly IElementoRepository  _IelementosRepository;
            public bool IsEditing { get; set; }
            public GridViewUserSettings UserSettings { get; set; }
            public BpGridViewDataSet<ElementoViewModel> Elementos { get; set; }
            //public BpGridViewDataSet<Empresa> Empresaexport { get; set; }
            public bool IsInserting { get; set; }
            private readonly IMapper _mapper;
            public List<Foto> listFotos{get;set;}
         public ViewModelInventario(IMapper mapper, IElementoRepository IelementosRepository, IHostingEnvironment hostingEnvironment){
                //Mapper
                 _mapper = mapper;
                _IelementosRepository=IelementosRepository;
                _hostingEnvironment = hostingEnvironment;
               // context=ctx;
        }
        public override Task Init()
        {
            Elementos = new BpGridViewDataSet<ElementoViewModel> {
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
                        ColumnName = "CodigoApoyo",
                        DisplayOrder = 1,
                        ColumnWidth = 400
                    },
                    new GridViewColumnSetting {
                        ColumnName = "NumeroApoyo",
                        DisplayOrder = 2
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
                        ColumnName = "Proyecto",
                        
                        DisplayOrder = 9
                    },
                    new GridViewColumnSetting {
                        ColumnName = "Material",
                        
                        DisplayOrder = 10
                    },
                     new GridViewColumnSetting {
                        ColumnName = "LongitudElemento",
                        
                        DisplayOrder = 11
                    },
                     new GridViewColumnSetting {
                        ColumnName = "Estado",
                        
                        DisplayOrder = 12
                    },
                     new GridViewColumnSetting {
                        ColumnName = "NivelTensionElemento",
                        
                        DisplayOrder = 13
                    },
                  
                    
                }
            };
            return base.Init();
        }

        public void EditEmpresa(EmpresaDto empresa)
        {
            Elementos.RowEditOptions.EditRowId = empresa.Id;
            IsEditing = true;
            
        }
       /* public async Task<EmpresaDto> getEmpresa(EmpresaDto empresa){
         
            
            var modeedit=await _IEmpresaRepository.GetSingleAsync(m=>m.Id==empresa.Id);
            var model = _mapper.Map<Empresa, EmpresaDto>(modeedit);
            return model;
        }*/

        /* 
        public async Task UpdateEmpresa(ElementoViewModel elemento)
        {
           // var modeledit =await getEmpresa(empresa);
           //context.Empresa.Update(empresaedit);
           // var model = _mapper.Map<EmpresaDto, Empresa>(modeledit);
           // model=modeledit;
            var modeedit=await _IelementosRepository.GetSingleAsync(m=>m.Id==empresa.Id);
           //var model = _mapper.Map<Empresa, EmpresaDto>(modeedit);
           if(modeedit!=null){
             
                modeedit.Nit=empresa.Nit;
                modeedit.Nombre=empresa.Nombre;
                modeedit.Telefono=empresa.Telefono;
                modeedit.Direccion=empresa.Direccion;
               // lecturaedit.Id=id;
               
               await  _IelementosRepository.EditAsync(modeedit);
               
                }
            CancelEdit();
        }
        */
        private void CancelEdit()
        {
            Elementos.RowEditOptions.EditRowId = -1;
            IsEditing = false;
        }

        public void CancelEditEmpresa()
        {
            CancelEdit();
            Elementos.RequestRefresh(true);
        }

        public GridViewDataSetLoadedData<ElementoViewModel> GetData(IGridViewDataSetLoadOptions gridViewDataSetOptions)
        {
            var queryable = GetQueryable(50).Result;
            return queryable.GetDataFromQueryable(gridViewDataSetOptions);
        }
       

        private async Task<IQueryable<ElementoViewModel>> GetQueryable(int size)
        {
          //  var numbers = new List<Empresa>();
            // for (var i = 0; i < size; i++)
            // {
            //     numbers.Add(new Customer { Id = i + 1, Name = $"Customer {i + 1}", BirthDate = DateTime.Now.AddYears(-i), Orders = i });
            // }
            var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
            
            
             var model = _mapper.Map<IEnumerable<Elemento>, IEnumerable<ElementoViewModel>>(elementos);
        
            //empresa.as
            return  model.AsQueryable();
        }
        
        //exportar datos a excel
         public void Export()
        {
             
                var exporter = new GridViewExportCsv<ElementoViewModel>(new GridViewExportCsvSettings<ElementoViewModel> { Separator = ";" });
                var gridView = Context.View.FindControlByClientId<DotVVM.BusinessPack.Controls.GridView>("data", true);
                using (var file = exporter.Export(gridView,  this.Elementos))
                {
                    Context.ReturnFile(file, "Report.csv", "text/csv");
               // Context.ReturnFile(file, "export.pdf", "application/pdf");
                }
        }
        public void ExportDetail()
        {

             var outputStream = new MemoryStream();
            MasterDetailsPdfReport.CreateStreamingPdfReport(_hostingEnvironment.WebRootPath, outputStream);
           // return new FileStreamResult(outputStream, "application/pdf")
            //{
              //  FileDownloadName = "report.pdf"
            //};
             Context.ReturnFile(outputStream, "report.pdf", "application/pdf");
          

        }
         public void ExportFotos()
        {
            var elementos= GetQueryable(100).Result;//await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
           var elem=elementos.ToList();
            listFotos=new List<Foto>();
             foreach(var item in elem){

                // listFotos=item.Fotos.ToList();
             } 
             var modelfotos = _mapper.Map<IEnumerable<Foto>, IEnumerable<FotoViewModel>>(listFotos);
            var listafotos=modelfotos.ToList();
            
             var outputStream = new MemoryStream();
            FotosMasterDetail.CreateStreamingPdfReport(_hostingEnvironment.WebRootPath, outputStream, listafotos);
           // return new FileStreamResult(outputStream, "application/pdf")
            //{
              //  FileDownloadName = "report.pdf"
            //};
             Context.ReturnFile(outputStream, "report.pdf", "application/pdf");
          

        }
        
        public void InsertNewCustomer()
        {
            Elementos.RowInsertOptions.InsertedItem = new ElementoViewModel {
                Id = Elementos.Items.Max(c => c.Id) + 1,
                NumeroApoyo=0
            };
            IsInserting = true;
            
        }

        public void CancelInsertNewCustomer()
        {
            Elementos.RowInsertOptions.InsertedItem = null;
            IsInserting = false;
        }

        public void SaveNewCustomer()
        {
            // Save inserted item to database
            Elementos.Items.Add(Elementos.RowInsertOptions.InsertedItem);
            CancelInsertNewCustomer();
        }

        //exportar datos a excel y pdf

       

        



       

    }
}