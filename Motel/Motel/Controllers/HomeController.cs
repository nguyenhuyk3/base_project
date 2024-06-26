using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Models;
using System.Diagnostics;
using WebProject.Models;
using Motel.Utility.Database;

namespace Motel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult ShowConnectedUsers()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
