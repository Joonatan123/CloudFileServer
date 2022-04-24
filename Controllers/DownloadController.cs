using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CloudFileServer.Models;
using System.Globalization;
using System.Threading;
using System.IO.Compression;
using CloudFileServer.Functions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace CloudFileServer.Controllers
{
    //[Route("File/{action=Index}/{id?}")]
    public class DownloadController : Controller
    {
        string rootPath = "undefined";
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;

        public DownloadController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IConfiguration config)
        {
            _config = config;
            _logger = logger;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("de-DE");
            _webHostEnvironment = webHostEnvironment;
            rootPath = _config.GetValue<string>("RootPath", "undefined");
            if(!Directory.Exists(rootPath))
                throw new TypeInitializationException("Configured path doesn't exist. Change it in appsettings.json.", new Exception());
        }

        public IActionResult Index(string path, string? sort, bool? reverse)
        {
            if (path == null)
                path = "/";
            if (path.Contains(".."))
                path = "/";
            if (!Directory.Exists(rootPath + path))
                return Content("Path doesn't exist");
            path.Replace("%2F", "/");
            DownloadViewModel temp = new DownloadViewModel(path, rootPath);
            if (sort != null)
                temp.sortInfo.SortBy = sort;
            if (reverse == true)
                temp.sortInfo.Reverse = true;
            temp.Sort();
            return View(temp);
        }
        [HttpGet]
        public IActionResult Download(string root, string[] folders, string[] files)
        {
            string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string tempDirectory = Path.Join(programDirectory + "Download/");
            string sourceDirectory = Path.Join(rootPath, root);
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
            Directory.CreateDirectory(tempDirectory);


            foreach (string folder in folders)
            {
                HelperFunctions.CloneDirectory(Path.Join(sourceDirectory, folder), Path.Join(tempDirectory, folder));
            }

            foreach (string file in files)
            {
                System.IO.File.Copy(Path.Join(sourceDirectory, file), Path.Join(tempDirectory, file));
            }
            if (System.IO.File.Exists(Path.Join(programDirectory, "/Download.zip")))
                System.IO.File.Delete(Path.Join(programDirectory, "/Download.zip"));
            ZipFile.CreateFromDirectory(tempDirectory, Path.Join(programDirectory, "/Download.zip"));

            Directory.Delete(tempDirectory, true);
            byte[] bytes = System.IO.File.ReadAllBytes(Path.Join(programDirectory, "/Download.zip"));
            return File(bytes, "application/octet-stream", "Download.zip");
        }
        [RequestSizeLimit(100_000_000)]
        [Route("/Download/Upload")]
        [HttpPost]

        public async Task<IActionResult> SaveFileToPhysicalFolder(string path)
        {
            //return Content(path);
            var boundary = HeaderUtilities.RemoveQuotes(
             MediaTypeHeaderValue.Parse(Request.ContentType).Boundary
            ).Value;

            var reader = new MultipartReader(boundary, Request.Body);

            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDisposition = ContentDispositionHeaderValue.TryParse(
                 section.ContentDisposition, out var contentDisposition
                );

                if (hasContentDisposition)
                {
                    if (contentDisposition.DispositionType.Equals("form-data") &&
                    (!string.IsNullOrEmpty(contentDisposition.FileName.Value) ||
                    !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value)))
                    {
                        string fileStoragePath = Path.Join(rootPath, path);
                        string fileName = contentDisposition.FileName.Value;//Path.GetRandomFileName() + ".jpg";
                        // uploaded files form fileds
                        byte[] fileByteArray;
                        using (var memoryStream = new MemoryStream())
                        {
                            await section.Body.CopyToAsync(memoryStream);
                            fileByteArray = memoryStream.ToArray();
                        }
                        using (var fileStream = System.IO.File.Create(Path.Combine(fileStoragePath, fileName)))
                        {
                            await fileStream.WriteAsync(fileByteArray);
                        }
                    }
                    else
                    {
                        // no file uplading fields
                    }
                }
                section = await reader.ReadNextSectionAsync();
            }

            return Redirect("/Download?path=" + path);
        }


    }
}