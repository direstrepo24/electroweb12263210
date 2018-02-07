using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using electroweb.DTO;
using electroweb.Reports;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace electroweb.Reports
{
    public class ElementosMasterDetail
    {
         public static byte[] CreateInMemoryPdfReport(string wwwroot, List<ElementoViewModel> ListElementos)
        {
            return CreateMasterDetailsPdfReport(wwwroot, ListElementos).GenerateAsByteArray(); // creating an in-memory PDF file
        }
       
        
         public static IPdfReportData CreateStreamingPdfReport(string wwwroot, Stream stream, List<ElementoViewModel> listfotos)
        {
            return CreateMasterDetailsPdfReport(wwwroot, listfotos).Generate(data => data.AsPdfStream(stream, closeStream: false));
        }
       
        public static PdfReport CreateMasterDetailsPdfReport(String wwwroot, List<ElementoViewModel> listfotos)
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
                    //footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
                    footer.CustomFooter(new CustomFooter(footer.PdfFont, PdfRunDirection.LeftToRight));
                })
                .PagesHeader(header =>
                {
                    header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                    
                    header.CustomHeader(new MasterDetailsHeadersElemento { PdfRptFont = header.PdfFont });
                    
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
                    
                 // var elementos= await _IelementosRepository.AllIncludingAsync(a=>a.Proyecto, b=>b.Material, c=>c.LocalizacionElementos, d=>d.Estado, e=>e.NivelTensionElemento, f=>f.LongitudElemento, g=>g.Fotos);
                var listelements= listfotos; 
                dataSource.StronglyTypedList(listelements);
                   
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
                        column.PropertyName<ElementoViewModel>(x => x.Id);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(1);
                        column.Width(2);
                        column.HeaderCell("Id");
                    });

                    columns.AddColumn(column =>
                    {
                        column.PropertyName<ElementoViewModel>(x => x.CodigoApoyo);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(2);
                        column.Width(3);
                        column.HeaderCell("CodigoApoyo");
                    });
                    
                    columns.AddColumn(column =>
                    {
                        column.PropertyName<ElementoViewModel>(x => x.HoraInicio);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(3);
                        column.Width(3);
                        column.HeaderCell("HoraInicio");
                        column.Group(
                    (val1, val2) =>
                    {
                        return val1.ToString() == val2.ToString();
                    });
                    });
                    

                    columns.AddColumn(column =>
                    {
                        column.PropertyName<ElementoViewModel>(x => x.Id);
                        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                        column.IsVisible(true);
                        column.Order(4);
                        column.Width(2);
                        column.HeaderCell("Id");
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
}


public class CustomFooter : IPageFooter
    {
        PdfContentByte _pdfContentByte;
        readonly IPdfFont _pdfRptFont;
        readonly iTextSharp.text.Font _font;
        readonly PdfRunDirection _direction;
        PdfTemplate _template;

        public CustomFooter(IPdfFont pdfRptFont, PdfRunDirection direction)
        {
            _direction = direction;
            _pdfRptFont = pdfRptFont;
            _font = _pdfRptFont.Fonts[0];
        }

        public void ClosingDocument(PdfWriter writer, Document document, IList<SummaryCellData> columnCellsSummaryData)
        {
            _template.BeginText();
            _template.SetFontAndSize(_pdfRptFont.Fonts[0].BaseFont, 8);
            _template.SetTextMatrix(0, 0);
          
            _template.ShowText((writer.PageNumber - 1).ToString());
            _template.EndText();
           

        }

        public void PageFinished(PdfWriter writer, Document document, IList<SummaryCellData> columnCellsSummaryData)
        {
            var pageSize = document.PageSize;
            var text = "Page " + writer.PageNumber + " / ";
            var textLen = _font.BaseFont.GetWidthPoint(text, _font.Size);
            var center = (pageSize.Left + pageSize.Right) / 2;
            var align = _direction == PdfRunDirection.RightToLeft ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;

            ColumnText.ShowTextAligned(
                        canvas: _pdfContentByte,
                        alignment: align,
                        phrase: new Phrase(text, _font),
                        x: center,
                        y: pageSize.GetBottom(25),
                        rotation: 0,
                        runDirection: (int)_direction,
                        arabicOptions: 0);


          var images=TestUtils.GetImagePath("01.png");
        // System.Drawing.Bitmap bm = new Bitmap(images);


            var x = _direction == PdfRunDirection.RightToLeft ? center - textLen : center + textLen;
            _pdfContentByte.AddTemplate(_template, x, pageSize.GetBottom(25));
        }

        public void DocumentOpened(PdfWriter writer, IList<SummaryCellData> columnCellsSummaryData)
        {
            _pdfContentByte = writer.DirectContent;
            _template = _pdfContentByte.CreateTemplate(50, 50);
        }
    }
        public class MasterDetailsHeadersElemento : IPageHeader
        {
        public IPdfFont PdfRptFont { get; set; }
        public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
        {
            var fechacreacion ="sdfdsf";
            var codigoapoyo ="sdfdsf";
            var altura = 4;

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
                    cellData.Value = "Fecha Creacion";
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                },
                (cellData, cellProperties) =>
                {
                    cellData.Value = fechacreacion;
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                });
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   cellData.Value = "Altura Disp:";
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
               },
               (cellData, cellProperties) =>
               {
                   cellData.Value = altura;
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

                    string url = "http://54.86.105.4/Fotos/d978a65c-5071-4d21-9aac-8a3b81032ba7.jpg";
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(new Uri(url));
                    

                    byte[] imageArray = System.IO.File.ReadAllBytes(@url);
                   string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                    
                    /*using (System.Drawing.Image image = jpg)
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();
                            cellProperties.HorizontalAlignment = HorizontalAlignment.Justified;
                           cellData.Value = TestUtils.GetHtmlPage("http://54.86.105.4/Fotos/d978a65c-5071-4d21-9aac-8a3b81032ba7.jpg");

                            // Convert byte[] to Base64 String
                          //  string base64String = Convert.ToBase64String(imageBytes);
                          
                        }
                    }*/



                   /// iTextSharp.text.Image jpg =  iTextSharp.text.Image.GetInstance(new Uri(url));

                   //cellData.Value = jpg.Url.LocalPath;
                
                        //doc.Add(new Paragraph("JPG"));

                        ///string url = "http://54.86.105.4/Fotos/d978a65c-5071-4d21-9aac-8a3b81032ba7.jpg";

                       /// iTextSharp.text.Image jpg =  iTextSharp.text.Image.GetInstance(new Uri(url));



                        ///cellData.Value=jpg.Width;

                   




               });
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   cellData.Value = "Reporte inventario";
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
               });
               /* table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   //electroweb/Images/wwwroot/01.png does not exists & defaultImageFilePath was not found.'
             
                   cellData.CellTemplate = new ImageFilePathField();

                   cellData.CellTemplate = new ImageFilePathField();
                   cellData.Value = TestUtils.GetImagePath("reporte_superior.png");
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Justified;
               });*/
            return table.AddBorderToTable();
         }
        
    }