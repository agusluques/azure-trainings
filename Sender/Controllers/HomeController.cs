using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sender.Models;

namespace Sender.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static ITopicClient topicClient;
        public HomeController(ILogger<HomeController> logger, IConfiguration Configuration)
        {
            _logger = logger;
            topicClient = new TopicClient(Configuration.GetConnectionString("PrimaryConnectionString"), Configuration.GetConnectionString("TopicName"));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> Send(string msg)
        {
            _logger.LogInformation("El mensaje es: {0}", msg);

            await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(msg)));

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
