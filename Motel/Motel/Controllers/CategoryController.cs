using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebProject.Models;
using WebProject.Utility.Database;

namespace WebProject.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IMongoCollection<Catergory> _categoryCollection;
        public CategoryController(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _categoryCollection = mongoDatabase.GetCollection<Catergory>(
                databaseSettings.Value.CategoriesCollectionName);
        }

        public async Task<IActionResult> ListCategory()
        {
            var listCategory = await _categoryCollection.AsQueryable().ToListAsync();
            return View(listCategory);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Catergory catergory)
        {
            if (ModelState.IsValid)
            {
                await _categoryCollection.InsertOneAsync(catergory);
                return RedirectToAction(nameof(ListCategory));
            }
            return View(catergory);
        }
    }
}
