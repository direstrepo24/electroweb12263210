using System.IO;
using electroweb.Reports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace electroweb.Controllers
{

    public class ReportController: Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReportController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            var reportBytes = InMemoryPdfReport.CreateInMemoryPdfReport(_hostingEnvironment.WebRootPath);
            return File(reportBytes, "application/pdf", "report.pdf");
        }

        /// <summary>
        /// GET /Report/streaming
        /// </summary>
        public IActionResult Streaming()
        {
            var outputStream = new MemoryStream();
            MasterDetailsPdfReport.CreateStreamingPdfReport(_hostingEnvironment.WebRootPath, outputStream);
            return new FileStreamResult(outputStream, "application/pdf")
            {
                FileDownloadName = "report.pdf"
            };
        }
        
    }
}