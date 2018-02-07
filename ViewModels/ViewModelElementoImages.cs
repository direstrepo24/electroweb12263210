using System.IO;
using AutoMapper;
using electroweb.Reports;
using electroweb.Reports.MasterReports;
using Microsoft.AspNetCore.Hosting;

namespace electroweb.ViewModels
{
    public class ViewModelElementoImages:BaseViewModel
    {

        private readonly IMapper _mapper;
        private readonly IHostingEnvironment _hostingEnvironment;
        
        public ViewModelElementoImages(
       
        IMapper mapper,
        IHostingEnvironment hostingEnvironment
     
        ){

           
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;

       
        }

         public void ExportDetalleFotos(){
             var outputStream = new MemoryStream();
            ImageFilePathPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream);
           // return new FileStreamResult(outputStream, "application/pdf")
            //{
              //  FileDownloadName = "report.pdf"
            //};
            Context.ReturnFile(outputStream, "report_image.pdf", "application/pdf");
        }


         public void ExportDetalleHtml(){
            var outputStream = new MemoryStream();
            HtmlHeaderPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream);
           // return new FileStreamResult(outputStream, "application/pdf")
            //{
              //  FileDownloadName = "report.pdf"
            //};
            Context.ReturnFile(outputStream, "report.pdf", "application/pdf");
        }


          public void ExportGroupingDocsPdfReport(){
             
            var outputStream = new MemoryStream();
            GroupingDocsPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream);
           // return new FileStreamResult(outputStream, "application/pdf")
            //{
              //  FileDownloadName = "report.pdf"
            //};
            Context.ReturnFile(outputStream, "report_group.pdf", "application/pdf");
        }


         public void ExportCustomSummaryPerPagePdfReport(){
             
            var outputStream = new MemoryStream();
            CustomSummaryPerPagePdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream);
           // return new FileStreamResult(outputStream, "application/pdf")
            //{
              //  FileDownloadName = "report.pdf"
            //};
            Context.ReturnFile(outputStream, "report_customsumary.pdf", "application/pdf");
        }

        public void ExportEventsPdfReport(){
             
            var outputStream = new MemoryStream();
            EventsPdfReport.CreateHtmlHeaderPdfReportStream(_hostingEnvironment.WebRootPath, outputStream);
           // return new FileStreamResult(outputStream, "application/pdf")
            //{
              //  FileDownloadName = "report.pdf"
            //};
            Context.ReturnFile(outputStream, "report_events.pdf", "application/pdf");
        }


    }
}