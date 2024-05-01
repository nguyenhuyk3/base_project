using Microsoft.AspNetCore.Mvc;

namespace Motel.Areas.Post.Controllers
{
    [Area("Post")]
    public class Home : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
