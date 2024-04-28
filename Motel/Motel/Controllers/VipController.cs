using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebProject.Models;
using WebProject.Utility.Database;

namespace WebProject.Controllers
{
    public class VipController : Controller
    {
        private readonly IMongoCollection<Vip> _vipsCollection;
        public VipController(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _vipsCollection = mongoDatabase.GetCollection<Vip>(
                databaseSettings.Value.VipsCollectionName);
        }

        public async Task<IActionResult> ListVip()
        {
            var listVip = await _vipsCollection.AsQueryable().ToListAsync();
            return View(listVip);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vip vip)
        {
            if (ModelState.IsValid)
            {
                await _vipsCollection.InsertOneAsync(vip);
                return RedirectToAction(nameof(ListVip));
            }
            return View(vip);
        }
    }
}
