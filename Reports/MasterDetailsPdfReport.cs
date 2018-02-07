using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
//using PdfRpt.Core.FunctionalTests.CustomDataSources;
using PdfRpt.FluentInterface;
using PdfRpt.Core.Helper;
using Electro.model.datatakemodel;
using Electro.model.Repository;
using System.Linq;
using Electro.model.DataContext;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace electroweb.Reports
{
    public  class MasterDetailsPdfReport
    {
        public static MyAppContext data;
         public static IServiceProvider ServiceProvider { get; set; }
         public static IElementoRepository  _IelementosRepository{ get; set; }
     /*  public MasterDetailsPdfReport(IElementoRepository  IelementosRepository){

            _IelementosRepository=IelementosRepository;
       }*/
        public static byte[] CreateInMemoryPdfReport(string wwwroot)
        {
            return CreateMasterDetailsPdfReport(wwwroot).GenerateAsByteArray(); // creating an in-memory PDF file
        }
       
        /*public static async Task<Stream> getContent()
        {
                using (HttpClient c = new HttpClient())
                            {
                                using (Stream s = await c.GetStreamAsync("http://54.86.105.4/"))
                                {
                                    // do any logic with the image stream, save it, store it...etc.
                                return s;
                                }
                            }
         } */
         public static IPdfReportData CreateStreamingPdfReport(string wwwroot, Stream stream)
        {
         //  wwwroot=getContent().Result.StreamToBytes().ToString;
           
            return CreateMasterDetailsPdfReport(wwwroot).Generate(data => data.AsPdfStream(stream, closeStream: false));
        }
       
        public static PdfReport CreateMasterDetailsPdfReport(String wwwroot)
        {
            
            return new PdfReport().DocumentPreferences(doc =>
                {
                    doc.RunDirection(PdfRunDirection.LeftToRight);
                    doc.Orientation(PageOrientation.Portrait);
                    doc.PageSize(PdfPageSize.A4);
                    doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "PdfRpt", Keywords = "IList Rpt.", Subject = "Test Rpt", Title = "Test" });
                    doc.Compression(new CompressionSettings
                    {
                        EnableCompression = true,
                        EnableFullCompression = true
                    });
                })
                .DefaultFonts(fonts =>
            {
                fonts.Path(TestUtils.GetVerdanaFontPath(),
                            TestUtils.GetTahomaFontPath());
                fonts.Size(9);
                fonts.Color(System.Drawing.Color.Black);
            })
                .PagesFooter(footer =>
                {
                    footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
                })
                .PagesHeader(header =>
                {
                    header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                    
                    header.CustomHeader(new MasterDetailsHeaders { PdfRptFont = header.PdfFont });
                    
                })
                .MainTableTemplate(template =>
                {
                    template.BasicTemplate(BasicTemplate.SilverTemplate);
                })
                .MainTablePreferences(table =>
                {
                   table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = false,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 170
                });
                })
                .MainTableDataSource( dataSource =>
                {
                    /* 
                  var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
              var listelements= elementos.ToList();
              //var listOfRows =listelements;
                    var listOfRows = new List<Foto>();
                    foreach (var item in listelements)
                    
                    {
                       listOfRows= item.Fotos.ToList();
              // listOfRows.Add(new Foto { Id = item.Id, Elemento_Id =item.Id, Ruta = item.Fotos.FirstOrDefault().Ruta });
                    
                        
                    }
                    dataSource.StronglyTypedList(listOfRows);*/
                    var listOfRows = new List<Usuario>();
                    for (int i = 0; i < 40; i++)
                    {
                        listOfRows.Add(new Usuario { Id = i, Apellido = "LastName " + i, Nombre = "Name " + i,  Empresa_Id= 1});
                    }
                    dataSource.StronglyTypedList(listOfRows);
                })
                .MainTableSummarySettings(summarySettings =>
                {
                    summarySettings.OverallSummarySettings("Summary");
                    summarySettings.PreviousPageSummarySettings("Previous Page Summary");
                    summarySettings.PageSummarySettings("Page Summary");
                })
                .MainTableColumns(columns =>
                {
                    columns.AddColumn(column =>
                    {
                        column.PropertyName("rowNo");
                        column.IsRowNumber(true);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(0);
                        column.Width(1);
                        column.HeaderCell("#");
                    });
                
                    columns.AddColumn(column =>
                    {
                        column.PropertyName<Usuario>(x => x.Id);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(1);
                        column.Width(2);
                        column.HeaderCell("Id");
                    });

                    columns.AddColumn(column =>
                    {
                        column.PropertyName<Usuario>(x => x.Nombre);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(2);
                        column.Width(3);
                        column.HeaderCell("Nombre");
                    });
                 
                    columns.AddColumn(column =>
                    {
                        column.PropertyName<Usuario>(x => x.Apellido);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(3);
                        column.Width(3);
                        column.HeaderCell("Last Name");
                        column.Group(
                    (val1, val2) =>
                    {
                        return val1.ToString() == val2.ToString();
                    });
                    });
                    

                    columns.AddColumn(column =>
                    {
                        column.PropertyName<Usuario>(x => x.Empresa_Id);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(4);
                        column.Width(2);
                        column.HeaderCell("Empresa Id");
                        column.ColumnItemsTemplate(template =>
                        {
                            template.TextBlock();
                            template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                ? string.Empty : string.Format("{0:n0}", obj));
                        });
                        column.AggregateFunction(aggregateFunction =>
                        {
                            aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                            aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                ? string.Empty : string.Format("{0:n0}", obj));
                        });
                    });

                    //ESTA ES PARA MOSTRAR  FOTOS EN CELDAS
                    /* 
                     columns.AddColumn(column =>
                {
                    column.PropertyName<Foto>(x=>x.Ruta);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Image");
                   column.ColumnItemsTemplate(t => t.ImageFilePath(defaultImageFilePath:"wwwroot/Images", fitImages: false));
                });*/

                })
                .MainTableEvents(events =>
                {
                    events.DataSourceIsEmpty(message: "There is no data available to display.");
                })
                .Export(export =>
                {
                    export.ToExcel();
                    
                });
                


        }
    }

        public class MasterDetailsHeaders : IPageHeader
        {
            public IPdfFont PdfRptFont { get; set; }
             public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
        {
            var codigoapoyo = newGroupInfo.GetSafeStringValueOf<Usuario>(x => x.Nombre);
            var parentLastName = newGroupInfo.GetSafeStringValueOf<Usuario>(x => x.Empresa_Id);
            var IdElement = newGroupInfo.GetSafeStringValueOf<Usuario>(x => x.Id);

            var table = new PdfGrid(relativeWidths: new[] { 1f, 5f }) { WidthPercentage = 100 };
            table.AddSimpleRow(
                (cellData, cellProperties) =>
                {
                    cellData.Value = "Codigo apoyo:";
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                },
                (cellData, cellProperties) =>
                {
                    cellData.Value = codigoapoyo;
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                });
            table.AddSimpleRow(
                (cellData, cellProperties) =>
                {
                    cellData.Value = "Empresa Id:";
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                },
                (cellData, cellProperties) =>
                {
                    cellData.Value = parentLastName;
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                });
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   cellData.Value = "Id:";
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
               },
               (cellData, cellProperties) =>
               {
                   cellData.Value = IdElement;
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
               });
            return table.AddBorderToTable(borderColor: BaseColor.LightGray, spacingBefore: 5f);
        }
public PdfGrid RenderingReportHeader(Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
        {
            var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   //electroweb/Images/wwwroot/01.png does not exists & defaultImageFilePath was not found.'
                  /*  string url ="http://54.86.105.4/";
                   cellData.CellTemplate = new ImageFilePathField();

                   cellData.Value = TestUtils.GetHtmlPage(url+"Fotos/fb9c27b1-98f7-4388-b13c-664d22cac022.jpg");
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Center;*/
                   cellData.CellTemplate = new ImageFilePathField();
                   cellData.Value = TestUtils.GetImagePath("logo.png");
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
               });
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   cellData.Value = "Reporte inventario";
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
               });
            return table.AddBorderToTable();
         }
        }
}