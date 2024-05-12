using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.Areas.Post.Models;
using Motel.Areas.UserAccount.Controllers;
using Motel.Models;
using Motel.Utility.Checking;
using Motel.Utility.Database;
using System.Security.Claims;
using System.Security.Cryptography;


namespace Motel.Areas.Post.Controllers
{
    [Area("Post")]
    public class Modification : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Getter _getter;

        public Modification(IOptions<DatabaseSettings> databaseSettings, IWebHostEnvironment hostingEnvironment)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
            _hostingEnvironment = hostingEnvironment;
            _getter = new Getter(new HttpContextAccessor());
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
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

            PostAdd model = new PostAdd();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PostAdd model, IEnumerable<IFormFile> fileInput)
        {
            var categoryName = _databaseConstructor.CategoryCollection
                                    .Find(category => category.Id == model.CategoryId)
                                    .Project(category => category.Name)
                                    .FirstOrDefault();
            var cityName = _databaseConstructor.CityCollection
                                .Find(city => city.ApiId == int.Parse(model.ApiId))
                                .Project(city => city.Name)
                                .FirstOrDefault();

            List<Image> images = new List<Image>();

            foreach (var file in fileInput)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    filePath = "\\images\\" + fileName;

                    Image image = new Image()
                    {
                        Url = filePath,
                    };

                    images.Add(image);
                }
            }

            Motel.Models.Post post = new Motel.Models.Post()
            {
                Owner = _getter.GetLoginId(),
                CategoryName = categoryName,
                SubjectOnSite = model.SubjectOnSite,
                State = new State(),
                PostDetail = new PostDetail
                {
                    AddressDetail = new AddressDetail
                    {
                        Address = model.Address,
                        City = cityName,
                        District = model.District,
                        Ward = model.Ward,
                        Street = model.Street,
                    },
                    Description = model.Description,
                    HomeInformation = new HomeInformation
                    {
                        SquareMeter = model.SquareMeter,
                        Bedroom = model.Bedroom,
                        Toilet = model.Toilet,
                        Floor = model.Floor
                    },
                    Images = images,
                    NumberOfImage = images.Count,
                    Price = model.Price,
                    PriceString = model.Price + " VND",
                },
                ContactInfo = new ContactInfo()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                },
                ExpiredAt = DateTime.Now,
            };

            await _databaseConstructor.PostCollection.InsertOneAsync(post);

            var result = await _databaseConstructor.PostCollection
                                .Find(f => f.Id == post.Id).FirstOrDefaultAsync();
            var userAccount = _databaseConstructor.UserAccountCollection
                              .Find(userAccount => userAccount.Id == _getter.GetLoginId())
                              .FirstOrDefault();
            var posts = userAccount.Posts;

            if (posts == null)
            {
                posts = new List<Motel.Models.Post>();

                posts.Add(result);
            }
            else
            {
                posts.Add(result);
            }

            var objectId = new ObjectId(_getter.GetLoginId());
            var userAccountFilter = Builders<Motel.Models.UserAccount>.Filter.Eq("_id", objectId);
            var userAccountUpdate = Builders<Motel.Models.UserAccount>.Update.Set("posts", posts);

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(userAccountFilter, userAccountUpdate);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
