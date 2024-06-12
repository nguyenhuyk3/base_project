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
using WebProject.Models.Address;


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
        [Authorize(Policy = "RequireCustomer")]
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
            ViewBag.Cities = new SelectList(cities, "ProvinceId", "Name");

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

        public async Task<List<Image>> ReturnListmagesFromView(IEnumerable<IFormFile> fileInput)
        {
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

            return images;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ModificationLayoutViewModel model, IEnumerable<IFormFile> fileInput)
        {
            var vipDoc = await _databaseConstructor.VipCollection
                                                    .Find(f => f.Name == model.PostAdd.VipName)
                                                    .FirstOrDefaultAsync();
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();
            var categoryName = await _databaseConstructor.CategoryCollection
                                                            .Find(category => category.Id == model.PostAdd.CategoryId)
                                                            .Project(category => category.Name)
                                                            .FirstOrDefaultAsync();
            var cityName = await _databaseConstructor.CityCollection
                                                        .Find(city => city.ProvinceId == model.PostAdd.ProvinceId)
                                                        .Project(city => city.Name)
                                                        .FirstOrDefaultAsync();
            var images = await ReturnListmagesFromView(fileInput);
            var coordinates = await _getCoordinates.EncodeCoordinates(model.PostAdd.Street,
                                                            model.PostAdd.Ward,
                                                            model.PostAdd.District,
                                                            cityName);

            // Missing funitire attribute
            Motel.Models.Post post = new Motel.Models.Post()
            {
                OwnerId = _getter.GetLoginId(),
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
                        Furniture = model.PostAdd.Furniture,
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
                ExpiredAt = DateTime.UtcNow,
            };

            await _databaseConstructor.PostCollection.InsertOneAsync(post);

            ownerDoc.Posts ??= new List<string>();

            //var postId = new ObjectId(post.Id);
            ownerDoc.Posts.Add(post.Id);

            var ownerUpdate = Builders<Motel.Models.UserAccount>.Update.Set("posts", ownerDoc.Posts)
                                                                       .Inc("balance", -vipDoc.Cost);

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(f => f.Id == ownerId, ownerUpdate);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Detail(string postId)
        {
            var postDoc = await _databaseConstructor.PostCollection
                                                        .Find(f => f.Id == postId)
                                                        .FirstOrDefaultAsync();
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == postDoc.OwnerId)
                                                        .FirstOrDefaultAsync();
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ownerDoc.Id == senderId)
            {
                ViewData["isOwner"] = true;
            }
            else
            {
                ViewData["isOwner"] = false;
            }

            ViewData["PostId"] = postId;

            // Check if this user has received advice for this post
            if (!string.IsNullOrEmpty(senderId))
            {
                ownerDoc.Bookings ??= new List<Motel.Models.Booking>();

                if (ownerDoc.Bookings != null)
                {
                    bool have = false;

                    foreach (var booking in ownerDoc.Bookings)
                    {
                        if (booking.ContactInfo.OwnerId == senderId && booking.PostId == postId)
                        {
                            ViewData["Booked"] = true;

                            have = true;

                            break;
                        }
                    }

                    if (!have)
                    {
                        ViewData["Booked"] = false;
                    }
                }
            }

            var model = new Models.PostDetail
            {
                Post = postDoc,
                OwnerId = ownerDoc.Id,
                Avatar = ownerDoc.Info.Avatar,
                Name = ownerDoc.Info.FullName,
                Phone = ownerDoc.Info.Phone,
                Email = ownerDoc.Info.Email,
            };

            // Sends coordinates 
            ViewBag.Latitude = postDoc.PostDetail.AddressDetail.Latitude;
            ViewBag.Longitude = postDoc.PostDetail.AddressDetail.Longitude;

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<ActionResult> Edit(string postId)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                      .Find(f => f.Id == ownerId)
                                                      .FirstOrDefaultAsync();

            ownerDoc.Posts ??= new List<string>();

            if (!ownerDoc.Posts.Contains(postId))
            {
                return RedirectToAction("Index", "Home", new { area = "Post" });
            }

            var postDoc = await _databaseConstructor.PostCollection
                                                        .Find(f => f.Id == postId)
                                                        .FirstOrDefaultAsync();

            if (ownerDoc.Id != postDoc.OwnerId)
            {
                return RedirectToAction("Index", "Home", new { area = "Post" });
            }

            var categories = await _databaseConstructor.CategoryCollection
                                                        .Find(_ => true)
                                                        .ToListAsync();
            var cities = await _databaseConstructor.CityCollection
                                                    .Find(_ => true)
                                                    .ToListAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Cities = new SelectList(cities, "ProvinceId", "Name");

            // Missing funiture attribute
            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                PostAdd = new PostAdd()
                {
                    PostId = postId,
                    CategoryId = categories.Find(f => f.Name == postDoc.CategoryName).Id,
                    ProvinceId = cities.Find(f => f.Name == postDoc.PostDetail.AddressDetail.City).ProvinceId,
                    District = postDoc.PostDetail.AddressDetail.District,
                    Ward = postDoc.PostDetail.AddressDetail.Ward,
                    Street = postDoc.PostDetail.AddressDetail.Street,
                    Address = postDoc.PostDetail.AddressDetail.Address,
                    SubjectOnSite = postDoc.SubjectOnSite,
                    Description = postDoc.PostDetail.Description,
                    SquareMeter = postDoc.PostDetail.HomeInformation.SquareMeter,
                    Price = postDoc.PostDetail.Price,
                    Bedroom = postDoc.PostDetail.HomeInformation.Bedroom,
                    Toilet = postDoc.PostDetail.HomeInformation.Toilet,
                    Floor = postDoc.PostDetail.HomeInformation.Floor,
                    Furniture = "Không nội thất",
                    Name = ownerDoc.Info.FullName,
                    Phone = ownerDoc.Info.Phone,
                    Email = ownerDoc.Info.Email,
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ModificationLayoutViewModel model, IEnumerable<IFormFile> fileInput)
        {
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                      .Find(f => f.Id == _getter.GetLoginId())
                                                      .FirstOrDefaultAsync();
            var categoryName = await _databaseConstructor.CategoryCollection
                                                          .Find(category => category.Id == model.PostAdd.CategoryId)
                                                          .Project(category => category.Name)
                                                          .FirstOrDefaultAsync();
            var cityName = await _databaseConstructor.CityCollection
                                                        .Find(city => city.ProvinceId == model.PostAdd.ProvinceId)
                                                        .Project(city => city.Name)
                                                        .FirstOrDefaultAsync();
            var postDoc = await _databaseConstructor.PostCollection
                                                        .Find(f => f.Id == model.PostAdd.PostId)
                                                        .FirstOrDefaultAsync();
            var images = new List<Image>();

            if (fileInput.Count() > 0)
            {
                images = await ReturnListmagesFromView(fileInput);
            }
            else
            {
                images = postDoc.PostDetail.Images;
            }

            var coordinates = await _getCoordinates.EncodeCoordinates(model.PostAdd.Street,
                                                           model.PostAdd.Ward,
                                                           model.PostAdd.District,
                                                           cityName);
            var postDetail = new Motel.Models.PostDetail
            {
                Description = model.PostAdd.Description,
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
            };
            var contactInfo = new ContactInfo()
            {
                Name = model.PostAdd.Name,
                Email = model.PostAdd.Email,
                Phone = model.PostAdd.Phone,
            };
            var updatePostDefinition = Builders<Motel.Models.Post>.Update
                                            .Set(f => f.CategoryName, categoryName)
                                            .Set(f => f.SubjectOnSite, model.PostAdd.SubjectOnSite)
                                            .Set(f => f.PostDetail, postDetail)
                                            .Set(f => f.ContactInfo, contactInfo);
            var postFilter = Builders<Motel.Models.Post>.Filter.Eq(p => p.Id, model.PostAdd.PostId);

            await _databaseConstructor.PostCollection.UpdateOneAsync(postFilter, updatePostDefinition);

            return RedirectToAction("Index", "Home", new { area = "Post" });
        }

        //[HttpPost]
        //public async Task<JsonResult> DeletePost(string postId)
        //{
        //    var postDoc = await _databaseConstructor.PostCollection
        //                                                .Find(f => f.Id == postId)
        //                                                .FirstOrDefaultAsync();

        //    postDoc.State.IsDeleted = true;

        //    var updatePostDoc = Builders<Motel.Models.Post>.Update.Set(f => f.State, postDoc.State);
        //    var filterPostDoc = Builders<Motel.Models.Post>.Filter.Eq(f => f.Id, postDoc.Id);

        //    await _databaseConstructor.PostCollection.UpdateOneAsync(filterPostDoc, updatePostDoc);

        //    return Json(new { success = true });
        //}

        //[HttpPost]
        //public async Task<JsonResult> Extend(string postId, string vipName)
        //{
        //    var postDoc = await _databaseConstructor.PostCollection
        //                                               .Find(f => f.Id == postId)
        //                                               .FirstOrDefaultAsync();
        //    var vipDoc = await _databaseConstructor.VipCollection
        //                                               .Find(f => f.Name == vipName)
        //                                               .FirstOrDefaultAsync();
        //    var extension = postDoc.ExpiredAt.AddDays(vipDoc.Days);
        //    var updatePostDoc = Builders<Motel.Models.Post>.Update.Set(f => f.ExpiredAt, extension);
        //    var filterPostDoc = Builders<Motel.Models.Post>.Filter.Eq(f => f.Id, postDoc.Id);

        //    await _databaseConstructor.PostCollection.UpdateOneAsync(filterPostDoc, updatePostDoc);

        //    return Json(new { success = true, message = "Gia hạn thành công" });
        //}
    }
}

