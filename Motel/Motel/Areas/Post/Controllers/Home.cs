using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Utility.Database;

namespace Motel.Areas.Post.Controllers
{
    [Area("Post")]
    public class Home : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Home(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var pots = await _databaseConstructor.PostCollection
                                        .Find(_ => true)
                                        .ToListAsync();

            return View(pots);

        }
    }
}
