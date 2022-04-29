using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CloudFileServer.Models;
using System.Globalization;
using System.Threading;
using System.IO.Compression;
using CloudFileServer.Functions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http.Extensions;
using SixLabors.ImageSharp.Drawing.Processing;
using System.IO;
using System.Security.Cryptography;
namespace CloudFileServer.Controllers
{
    //[Route("File/{action=Index}/{id?}")]
    public class DownloadController : Controller
    {
        static Object _lock = new Object();
        static int id = 0;
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
            if (!Directory.Exists(rootPath))
                throw new TypeInitializationException("Configured path doesn't exist. Change it in appsettings.json.", new Exception());
        }

        public IActionResult Index(string path, string? sort, bool? reverse, bool? mkdir, bool? thumbnail)
        {
            if (HttpContext.Session.GetInt32("loggedIn") == null)
                return Redirect("/Home/LogIn");
            if (path == null)
                path = "/";
            if (path.Contains(".."))
                path = "/";
            path.Replace("%2F", "/");
            if (!Directory.Exists(rootPath + path))
                return Content("Path doesn't exist");
            DownloadViewModel temp = new DownloadViewModel(path, rootPath);
            if (sort != null)
                temp.sortInfo.SortBy = sort;
            if (reverse == true)
                temp.sortInfo.Reverse = true;
            temp.Sort();
            temp.mkdir = (mkdir != null && mkdir == true);
            temp.thumbnail = (thumbnail != null && thumbnail == true);
            return View(temp);
        }
        [HttpGet]
        public IActionResult Download(string root, string[] folders, string[] files)
        {
            if (HttpContext.Session.GetInt32("loggedIn") == null)
                return Redirect("/Home/LogIn");
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

        public async Task<IActionResult> SaveFileToPhysicalFolder(string path, string? sort, bool? reverse, bool? thumbnail)
        {
            if (HttpContext.Session.GetInt32("loggedIn") == null)
                return Redirect("/Home/LogIn");
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
            //string? sort, bool? reverse, bool? thumbnail
            return Redirect("/Download?path=" + path + "&sort=" + sort + "&reverse=" + reverse.ToString() + "&thumbnail=" + thumbnail);
        }
        public IActionResult Mkdir(string path, string folderName)
        {
            if (HttpContext.Session.GetInt32("loggedIn") == null)
                return Redirect("/Home/LogIn");
            path.Replace("%2F", "/");
            if (!Directory.Exists(Path.Join(rootPath, path)))
                return Content("Directory does not exist");
            Directory.CreateDirectory(Path.Join(rootPath, path, folderName));
            return Redirect("/Download?path=" + path);
        }
        public IActionResult Image(string path, string name)
        {
            string hash = HelperFunctions.GetStringHashString(Path.Join(rootPath, path, name));
            //Console.WriteLine(Path.Join(rootPath, path, name));
            if(!HelperFunctions.Exists("thumbnails/"+hash+".jpg")){
                HelperFunctions.MakeThumbnail(Path.Join(rootPath, path, name));
            }
            Byte[] b = System.IO.File.ReadAllBytes("thumbnails/"+hash+".jpg");   // You can use your own method over here.         
            return File(b, "image/jpeg");
            //lock (_lock)
            //{
            int n = id;
            //Byte[] b = System.IO.File.ReadAllBytes(@"/home/joo/Projects/CloudFileServer/test/2d_car.jpg");   // You can use your own method over here.         
            //return File(b, "image/jpeg");

            int thumbnailHeight = 50;
            /*using (var image = SixLabors.ImageSharp.Image.Load(Path.Join(rootPath, path, name)))
            {
                // Resize the image in place and return it for chaining.
                // 'x' signifies the current image processing context.
                using (var ms = new MemoryStream())
                {
                    float aspect = (float)image.Width / image.Height;
                    image.Mutate(x => x.Resize((int)(aspect * thumbnailHeight), thumbnailHeight));
                    //if (image.Width > 160)
                    //image.Mutate(x => x.Resize(200, (int)(200 / aspect)));

                    image.SaveAsJpeg(ms);
                    return File(ms.ToArray(), "image/webp");
                }
            }*/
            return new EmptyResult();

            //}

        }
    }
}