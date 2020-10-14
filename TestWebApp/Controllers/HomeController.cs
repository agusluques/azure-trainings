using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestWebApp.Helpers;

namespace TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StorageHelper _storageHelper;
        private readonly IConfiguration _configuration;


        public HomeController(ILogger<HomeController> logger, IConfiguration Configuration, StorageHelper storageHelper)
        {
            _logger = logger;
            _storageHelper = storageHelper;
            _configuration = Configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(IFormFile file)
        {
            
            Console.WriteLine(file);


            using (var fileStream = file.OpenReadStream())
            {
                await _storageHelper.UploadImage(fileStream, file.FileName, _configuration);           
            }

            return Redirect(Request.Headers["Referer"].ToString());

        }
    } 
}
