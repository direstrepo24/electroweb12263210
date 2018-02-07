using System.Collections.Generic;

using iTextSharp.text;
using PdfRpt.Core.Contracts;

namespace electroweb.Templates
{
    public class GrayTemplate: ITableTemplate
    {
        public HorizontalAlignment HeaderHorizontalAlignment
        {
            get { return HorizontalAlignment.Center; }
        }

        public BaseColor AlternatingRowBackgroundColor
        {
            get { return new BaseColor(System.Drawing.Color.WhiteSmoke.ToArgb()); }
        }

        public BaseColor CellBorderColor
        {
            get { return new BaseColor(System.Drawing.Color.LightGray.ToArgb()); }
        }

        public IList<BaseColor> HeaderBackgroundColor
        {
            get { return new List<BaseColor> { new BaseColor(System.DrawingCore.ColorTranslator.FromHtml("#990000").ToArgb()), new BaseColor(System.DrawingCore.ColorTranslator.FromHtml("#e80000").ToArgb()) }; }
        }

        public BaseColor RowBackgroundColor
        {
            get { return null; }
        }

        public IList<BaseColor> PreviousPageSummaryRowBackgroundColor
        {
            get { return new List<BaseColor> { new BaseColor(System.Drawing.Color.LightSkyBlue.ToArgb()) }; }
        }

        public IList<BaseColor> SummaryRowBackgroundColor
        {
            get { return new List<BaseColor> { new BaseColor(System.Drawing.Color.LightSteelBlue.ToArgb()) }; }
        }

        public IList<BaseColor> PageSummaryRowBackgroundColor
        {
            get { return new List<BaseColor> { new BaseColor(System.Drawing.Color.Yellow.ToArgb()) }; }
        }

        public BaseColor AlternatingRowFontColor
        {
            get { return new BaseColor(System.DrawingCore.ColorTranslator.FromHtml("#333333").ToArgb()); }
        }

        public BaseColor HeaderFontColor
        {
            get { return new BaseColor(System.Drawing.Color.White.ToArgb()); }
        }

        public BaseColor RowFontColor
        {
            get { return new BaseColor(System.DrawingCore.ColorTranslator.FromHtml("#333333").ToArgb()); }
        }

        public BaseColor PreviousPageSummaryRowFontColor
        {
            get { return new BaseColor(System.Drawing.Color.Black.ToArgb()); }
        }

        public BaseColor SummaryRowFontColor
        {
            get { return new BaseColor(System.Drawing.Color.Black.ToArgb()); }
        }

        public BaseColor PageSummaryRowFontColor
        {
            get { return new BaseColor(System.Drawing.Color.Black.ToArgb()); }
        }

        public bool ShowGridLines
        {
            get { return true; }
        }
    }
}
