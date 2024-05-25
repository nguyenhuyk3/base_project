using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Motel.Areas.Post.Models;
using Motel.Areas.UserAccount.Controllers;
using Motel.Models;
using Motel.Utility.Address;
using Motel.Utility.Checking;
using Motel.Utility.Database;
using Motel.ViewModels;
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
        private readonly Motel.Models.UserAccount _owner;
        private readonly GetCoordinates _getCoordinates;

        public Modification(IOptions<DatabaseSettings> databaseSettings,
                            IWebHostEnvironment hostingEnvironment)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
            _hostingEnvironment = hostingEnvironment;
            _getter = new Getter(new HttpContextAccessor());
            _getCoordinates = new GetCoordinates("AIzaSyBaExchm2U82YZk2kI1xztt_cXV1dCoVwM");
        }

        [HttpGet]
        [Authorize(Policy = " ")]
        public async Task<IActionResult> Add()
        {
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == _getter.GetLoginId())
                                                        .FirstOrDefaultAsync();
            var categories = await _databaseConstructor.CategoryCollection
                                                        .Find(_ => true)
                                                        .ToListAsync();
            var cities = await _databaseConstructor.CityCollection
                                                    .Find(_ => true)
                                                    .ToListAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Cities = new SelectList(cities, "ApiId", "Name");

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                PostAdd = new PostAdd()
                {
                    Name = ownerDoc.Info.FullName,
                    Phone = ownerDoc.Info.Phone,
                    Email = ownerDoc.Info.Email,
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ModificationLayoutViewModel model, IEnumerable<IFormFile> fileInput)
        {
            var vipDoc = await _databaseConstructor.VipCollection
                                                    .Find(f => f.Name == model.PostAdd.VipName)
                                                    .FirstOrDefaultAsync();
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == _getter.GetLoginId())
                                                        .FirstOrDefaultAsync();
            var categoryName = await _databaseConstructor.CategoryCollection
                                                            .Find(category => category.Id == model.PostAdd.CategoryId)
                                                            .Project(category => category.Name)
                                                            .FirstOrDefaultAsync();
            var cityName = await _databaseConstructor.CityCollection
                                                        .Find(city => city.ApiId == int.Parse(model.PostAdd.ApiId))
                                                        .Project(city => city.Name)
                                                        .FirstOrDefaultAsync();

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

            var coordinates = await _getCoordinates.EncodeCoordinates(model.PostAdd.Street,
                                                                        model.PostAdd.Ward,
                                                                        model.PostAdd.District,
                                                                        cityName);

            Motel.Models.Post post = new Motel.Models.Post()
            {
                Owner = _getter.GetLoginId(),
                VipName = vipDoc.Name,
                CategoryName = categoryName,
                SubjectOnSite = model.PostAdd.SubjectOnSite,
                State = new State(),
                PostDetail = new Motel.Models.PostDetail
                {
                    AddressDetail = new AddressDetail
                    {
                        Address = model.PostAdd.Address,
                        City = cityName,
                        District = model.PostAdd.District,
                        Ward = model.PostAdd.Ward,
                        Street = model.PostAdd.Street,
                        Latitude = coordinates.Latitude,
                        Longitude = coordinates.Longitude,
                    },
                    Description = model.PostAdd.Description,
                    HomeInformation = new HomeInformation
                    {
                        SquareMeter = model.PostAdd.SquareMeter,
                        Bedroom = model.PostAdd.Bedroom,
                        Toilet = model.PostAdd.Toilet,
                        Floor = model.PostAdd.Floor
                    },
                    Images = images,
                    NumberOfImage = images.Count,
                    Price = model.PostAdd.Price,
                    PriceString = model.PostAdd.Price + " VND",
                },
                ContactInfo = new ContactInfo()
                {
                    Name = model.PostAdd.Name,
                    Email = model.PostAdd.Email,
                    Phone = model.PostAdd.Phone,
                },
                ExpiredAt = DateTime.Now,
            };

            await _databaseConstructor.PostCollection.InsertOneAsync(post);

            ownerDoc.Posts ??= new List<Motel.Models.Post>();
            ownerDoc.Posts.Add(post);

            var ownerId = new ObjectId(_getter.GetLoginId());
            var ownerFilter = Builders<Motel.Models.UserAccount>.Filter.Eq("_id", ownerId);
            var ownerUpdate = Builders<Motel.Models.UserAccount>.Update.Set("posts", ownerDoc.Posts)
                                                                       .Inc("balance", -vipDoc.Cost);

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(ownerFilter, ownerUpdate);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string postId)
        {
            var postDoc = await _databaseConstructor.PostCollection
                                            .Find(f => f.Id == postId)
                                            .FirstOrDefaultAsync();
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == postDoc.Owner)
                                                        .FirstOrDefaultAsync();
            var model = new Models.PostDetail
            {
                Post = postDoc,
                Avatar = ownerDoc.Info.Avatar,
                Name = ownerDoc.Info.FullName,
                Phone = ownerDoc.Info.Phone,
                Email = ownerDoc.Info.Email,
            };

            ViewBag.Latitude = postDoc.PostDetail.AddressDetail.Latitude;
            ViewBag.Longitude = postDoc.PostDetail.AddressDetail.Longitude;

            return View(model);
        }
    }
}

