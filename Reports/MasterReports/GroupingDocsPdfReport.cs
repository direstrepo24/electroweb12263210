using System;
using System.Collections.Generic;
using System.IO;
using electroweb.Templates;
using iTextSharp.text.pdf;
using PdfRpt;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace electroweb.Reports.MasterReports
{
    public class GroupingDocsPdfReport
    {

        public static byte[] CreateInMemoryPdfReport(string wwwroot)
        {
            return CreateGroupingDocsPdfReport(wwwroot).GenerateAsByteArray(); // creating an in-memory PDF file
        }
       
        
         public static IPdfReportData CreateHtmlHeaderPdfReportStream(string wwwroot, Stream stream)
        {
            return CreateGroupingDocsPdfReport(wwwroot).Generate(data => data.AsPdfStream(stream, closeStream: false));
        }
         private static IPdfFont getWatermarkFont()
        {
            var watermarkFont = new GenericFontProvider(
                                        TestUtils.GetVerdanaFontPath(),
                                        TestUtils.GetTahomaFontPath());
            watermarkFont.Color = new GrayColor(0.75f);
            watermarkFont.Size = 50;
            return watermarkFont;
        }

        public static PdfReport CreateGroupingDocsPdfReport(String wwwroot)
        {
           return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.RightToLeft);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "نرم افزار ", Keywords = "حساب تفصیلی ", Subject = "حساب تفصیلی " , Title = "حساب تفصیلی "  });
                doc.DiagonalWatermark(new DiagonalWatermark
                {
                    Text = "Diagonal Watermark\nLine 2\nLine 3",
                    RunDirection = PdfRunDirection.LeftToRight,
                    Font = getWatermarkFont(),
                    FillOpacity = 0.6f,
                    StrokeOpacity = 1
                });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
            .DefaultFonts(fonts =>
            {
                fonts.Path(System.IO.Path.Combine(TestUtils.GetRouteFonts(), "fonts", "tahoma.ttf"),
                           TestUtils.GetVerdanaFontPath());
                fonts.Size(8);
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(string.Concat("کاربر : ", "وحيد",
                                               " | ", "تاریخ تهیه گزارش : ", PersianDate.ToPersianDateTime(DateTime.Now, "/", true).FixWeakCharacters()));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.Message("Titulo");
                    defaultHeader.ImagePath(TestUtils.GetImagePath("01.png"));
                });
            })
            .MainTableTemplate(template =>
            {
                template.CustomTemplate(new GrayTemplate());
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = true,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 5f
                });
            })
            .MainTableDataSource(dataSource =>
            {
                var rows = new List<VoucherRowPrintViewModel>();
                var rnd = new Random();
                for (int i = 0; i < 10; i++)
                {
                    rows.Add(new VoucherRowPrintViewModel
                    {
                        Title ="Title "+ i,
                        VoucherNumber =i,
                        VoucherDate = DateTime.Now.AddDays(-i),
                        Description = "Descripcion  "+i,
                        Debtor = i%2==0? 0: rnd.Next(1,100),
                        Creditor= i%2!=0? 0: rnd.Next(1,100)
                    });
                }
                dataSource.StronglyTypedList(rows);
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Title);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.IsRowNumber(true);
                    column.Order(0);
                    column.Width(0.7f);
                    column.Group(true,
                       (val1, val2) =>
                       {
                           return val1.ToString() == val2.ToString();
                       });
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName("rowNumber");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.IsRowNumber(true);
                    column.Order(0);
                    column.Width(0.7f);
                    column.HeaderCell("#");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.VoucherNumber);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("Numero");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.VoucherDate);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1.5f);
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj =>
                        {
                            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                                return string.Empty;
                            return PersianDate.ToPersianDateTime((DateTime) obj);
                        });
                    });
                    column.HeaderCell("Fecha");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Description);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(4);
                    column.HeaderCell("Descripcion");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Debtor);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1.5f);
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
                    column.HeaderCell("Debtor");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Creditor);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1.5f);
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
                    column.HeaderCell("Credito");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.CaclulatedDetection);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("Sum");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.CaclulatedRemains);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(1.5f);
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                    });
                    column.HeaderCell("Calculo");
                });

            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings(" OverallSummarySettings");
                summarySettings.PreviousPageSummarySettings(" PreviousPageSummarySettings");
                //summarySettings.AllGroupsSummarySettings("جمع نهايي");
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "داده ای جهت نمایش وجود ندارد.");
                events.CellCreated(args =>
                {
                    args.Cell.BasicProperties.CellPadding = 4f;
                });
                events.MainTableAdded(args =>
                {
                    var taxTable = new PdfGrid(3);  // Create a clone of the MainTable's structure
                    taxTable.RunDirection = 3;
                    taxTable.SetWidths(new float[] { 3, 3, 3 });
                    taxTable.WidthPercentage = 100f;
                    taxTable.SpacingBefore = 10f;

                    taxTable.AddSimpleRow(
                        (data, cellProperties) =>
                        {
                            data.Value = " Enuar";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        },
                        (data, cellProperties) =>
                        {
                            data.Value = " Muñoz";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        },
                        (data, cellProperties) =>
                        {
                            data.Value = " Castillo";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        });
                    args.PdfDoc.Add(taxTable);
                });

                
            })
            .Export(export =>
            {
                export.ToExcel("خروجی اکسل");
                export.ToCsv("خروجی CSV");
                export.ToXml("خروجی XML");
            });
        }
    }

    public class VoucherRowPrintViewModel
    {
        public string Title { set; get; }
        public int VoucherNumber { set; get; }
        public DateTime VoucherDate { set; get; }
        public string Description { set; get; }
        public int Debtor { set; get; }
        public int Creditor { set; get; }

        public string CaclulatedDetection
        {
            get { return Debtor > 0 ? "بد" : "بس"; }
        }

        public int CaclulatedRemains
        {
            get { return Debtor - Creditor; }
        }
    
    }
}