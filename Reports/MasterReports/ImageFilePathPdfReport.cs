using System;
using System.Collections.Generic;
using System.IO;
using Electro.model.DataContext;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace electroweb.Reports.MasterReports
{
    public class ImageFilePathPdfReport
    {

        public static MyAppContext data;
         public static IServiceProvider ServiceProvider { get; set; }
        public static byte[] CreateInMemoryPdfReport(string wwwroot)
        {
            return CreateImageFilePathPdfReport(wwwroot).GenerateAsByteArray(); // creating an in-memory PDF file
        }
       
        
         public static IPdfReportData CreateHtmlHeaderPdfReportStream(string wwwroot, Stream stream)
        {
            return CreateImageFilePathPdfReport(wwwroot).Generate(data => data.AsPdfStream(stream, closeStream: false));
        }
        public static PdfReport CreateImageFilePathPdfReport(String wwwroot)
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "PdfRpt", Keywords = "Test", Subject = "Test Rpt", Title = "Test" });
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
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.ImagePath(TestUtils.GetImagePath("01.png"));
                    defaultHeader.Message("Our new rpt.");
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.SnowyPineTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .MainTableDataSource(dataSource =>
            {
                var listOfRows = new List<ImageRecord>
                                             {
                                                 new ImageRecord
                                                     {
                                                         Id=1,
                                                         ImagePath =  TestUtils.GetImagePath("01.png"),
                                                         Name = "Rnd"
                                                     },
                                                 new ImageRecord
                                                     {
                                                         Id=2,
                                                         ImagePath =  TestUtils.GetImagePath("02.png"),
                                                         Name = "Bug"
                                                     },
                                                 new ImageRecord
                                                     {
                                                         Id=3,
                                                         ImagePath =  TestUtils.GetImagePath("03.png"),
                                                         Name = "Stuff"
                                                     },
                                                 new ImageRecord
                                                     {
                                                         Id=4,
                                                         ImagePath =  TestUtils.GetImagePath("04.png"),
                                                         Name = "Sun"
                                                     }
                                             };
                dataSource.StronglyTypedList(listOfRows);
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
                    column.PropertyName<ImageRecord>(x => x.Id);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("Id");
                    column.ColumnItemsTemplate(t => t.Barcode(new Barcode39()));
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<ImageRecord>(x => x.ImagePath);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Image");
                    column.ColumnItemsTemplate(t => t.ImageFilePath(defaultImageFilePath: string.Empty, fitImages: false));
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<ImageRecord>(x => x.Name);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Name");
                });
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Export(export =>
            {
                export.ToExcel();
                export.ToXml();
            });
        }
    }

    public class ImageRecord
    {
        public int Id { set; get; }
        public string ImagePath { set; get; }
        public string Name { set; get; }
    }
}