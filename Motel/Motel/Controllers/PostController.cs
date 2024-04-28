using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebProject.Models;
using WebProject.Utility.Database;

namespace WebProject.Controllers
{
    public class PostController : Controller
    {
        //private readonly IMongoCollection<Post> _postsCollection;

        //public PostController(IOptions<DatabaseSettings> databaseSettings)
        //{
        //    var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
        //    var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

        //    _postsCollection = mongoDatabase.GetCollection<Post>
        //        (databaseSettings.Value.PostsCollectionName);
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Add()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Add(Post post)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        await _postsCollection.InsertOneAsync(post);

        //        return RedirectToAction("Index", "Home"); 
        //    }

        //    return View(post);
        //}
    }
}
