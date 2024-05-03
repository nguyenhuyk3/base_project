using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;

namespace Motel.Areas.Post.Controllers
{
    [Area("Post")]
    public class Modification : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Modification(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var categories = _databaseConstructor.CategoryCollection
                                .Find(_ => true)
                                .ToList();
            var cities = _databaseConstructor.CityCollection
                                .Find(_ => true)
                                .ToList();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Cities = new SelectList(cities, "ApiId", "Name");

            return View();
        }
    }
}
