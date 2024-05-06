using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.Post.Models;
using Motel.Models;
using Motel.Utility.Database;
using System.Security.Cryptography;


namespace Motel.Areas.Post.Controllers
{
    [Area("Post")]
    public class Modification : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Modification(IOptions<DatabaseSettings> databaseSettings, IWebHostEnvironment hostingEnvironment)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
            _hostingEnvironment = hostingEnvironment;
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

            _databaseConstructor.PostCollection.InsertOne(post);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }
    }
}
