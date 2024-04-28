using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Mono.TextTemplating;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebProject.Models;
using WebProject.Utility.Database;
using WebProject.ViewModels;

namespace WebProject.Controllers
{
    public class PostModelController : Controller
    {
        private readonly IMongoCollection<Post> _postCollection;

        public PostModelController(IOptions<DatabaseSettings> databaseSettings)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            _postCollection = mongoDatabase.GetCollection<Post>(
                databaseSettings.Value.PostsCollectionName);
        }

        public IActionResult Index()
        {
            List<Post> posts = new List<Post>();
            posts = _postCollection.AsQueryable().ToList();
            return View(posts);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var addressDetail = new AddressDetail()
                {
                    AddressString = postViewModel.AddressString,
                    City = postViewModel.City,
                    District = postViewModel.District,
                    Award = postViewModel.Award,
                    Street = postViewModel.Street,
                };

                var homeInfo = new HomeInformation()
                {
                    Width = postViewModel.Width,
                    Length = postViewModel.Length,
                    SquareMeter = postViewModel.SquareMeter,
                    BedRoom = postViewModel.BedRoom,
                    Floor = postViewModel.Floor,
                    Toilet = postViewModel.Toilet,

                };

                PostDetail postDetail = new PostDetail()
                {
                    AddressDetail = addressDetail,
                    HomeInformation = homeInfo,
                    NumberOfImage = postViewModel.NumberOfImage,
                    Price = postViewModel.Price,
                    PriceString = postViewModel.PriceString
                };


                Post post = new Post()
                {
                    Category = postViewModel.Category,
                    Subject = postViewModel.Subject,
                    ExpiredAt = DateTime.Now.AddDays(2),
                    PostDetail = postDetail,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                // Thêm đối tượng mới vào cơ sở dữ liệu MongoDB
                _postCollection.InsertOneAsync(post);
                return RedirectToAction("Index");
            }
            // Nếu ModelState không hợp lệ, quay lại view Create với dữ liệu nhập sai
            return View(postViewModel);
        }

        public IActionResult Details(string id)
        {
            var post = _postCollection.Find(x => x.Id == id).FirstOrDefault();
            return View(post);
        }

        public IActionResult Edit(string id)
        {
            var post = _postCollection.Find(c => c.Id == id).FirstOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public IActionResult Edit(Post post)
        {
            if (!ModelState.IsValid)
            {
                // If ModelState is not valid, return the view with the current post model
                return View(post);
            }

            try
            {
                var filter = Builders<Post>.Filter.Eq(x => x.Id, post.Id);
                var update = Builders<Post>.Update
                    .Set(x => x.Subject, post.Subject)
                    .Set(x => x.Category, post.Category)
                    .Set(x => x.PostDetail.AddressDetail.City, post.PostDetail.AddressDetail.City)
                    .Set(x => x.PostDetail.AddressDetail.Street, post.PostDetail.AddressDetail.Street)
                    .Set(x => x.PostDetail.AddressDetail.Award, post.PostDetail.AddressDetail.Award)
                    .Set(x => x.PostDetail.AddressDetail.District, post.PostDetail.AddressDetail.District)
                    .Set(x => x.PostDetail.HomeInformation.Length, post.PostDetail.HomeInformation.Length)
                    .Set(x => x.PostDetail.HomeInformation.Width, post.PostDetail.HomeInformation.Width)
                    .Set(x => x.PostDetail.HomeInformation.SquareMeter, post.PostDetail.HomeInformation.SquareMeter)
                    .Set(x => x.PostDetail.HomeInformation.Floor, post.PostDetail.HomeInformation.Floor)
                    .Set(x => x.PostDetail.HomeInformation.BedRoom, post.PostDetail.HomeInformation.BedRoom)
                    .Set(x => x.PostDetail.HomeInformation.Toilet, post.PostDetail.HomeInformation.Toilet);

                var result = _postCollection.UpdateOne(filter, update);

                if (result.ModifiedCount == 0)
                {
                    return NotFound();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error");
                return View(post); // Return the view with the current post model to display the data again
            }
        }

        public IActionResult Delete(string id)
        {
            var post = _postCollection.Find(c => c.Id == id).FirstOrDefault();
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public IActionResult Delete(Post post)
        {
            _postCollection.DeleteOne(c => c.Id == post.Id);
            return RedirectToAction("Index");
        }
    }
}