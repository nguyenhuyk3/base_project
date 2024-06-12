using Amazon.Runtime.Internal.Transform;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.Customer.Models;
using Motel.Areas.Post.Models;
using Motel.Models;
using Motel.Utility.Checking;
using Motel.Utility.Database;
using Motel.Utility.Momo;
using Motel.ViewModels;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Claims;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Motel.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class Customer : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Customer(IOptions<DatabaseSettings> databaseSettings, IWebHostEnvironment hostingEnvironment)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Info(string userAccountId, int? page)
        {
            if (page == null)
            {
                page = 1;
            }

            int pageSize = 5;
            int pageNum = page ?? 1;

            var ownerDoc = _databaseConstructor.UserAccountCollection
                                                .Find(userAccount => userAccount.Id == userAccountId)
                                                .FirstOrDefault();

            ownerDoc.Posts ??= new List<string>();

            var posts = await GetPostsByIds(ownerDoc.Posts);
            var reversedPassiveReviews = ownerDoc.PassiveReviews?.ToList();
            var isReviewed = false;
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(senderId))
            {
                var senderDoc = await _databaseConstructor.UserAccountCollection
                                                       .Find(f => f.Id == senderId)
                                                       .FirstOrDefaultAsync();

                senderDoc.ActiveReviewPersons ??= new List<string>();

                foreach (var personIsAllowed in senderDoc.ActiveReviewPersons)
                {
                    if (personIsAllowed == ownerDoc.Id)
                    {
                        isReviewed = true;

                        break;
                    }
                }
            }

            reversedPassiveReviews?.Reverse();

            ViewData["isReviewed"] = isReviewed;

            var pagedReversedPassiveReviews = reversedPassiveReviews?.ToPagedList(pageNum, pageSize);

            InfoViewModel model = new InfoViewModel()
            {
                Owner = ownerDoc,
                ReviewsOnSite = pagedReversedPassiveReviews,
                Posts = posts,
            };

            return View(model);
        }

        public async Task<List<Motel.Models.Post>> GetPostsByIds(List<string> postIds)
        {
            var filter = Builders<Motel.Models.Post>.Filter.In(post => post.Id, postIds);
            var posts = await _databaseConstructor.PostCollection.Find(filter).ToListAsync();

            return posts;
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> Payment()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();
            var model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
            };

            return View(model);
        }

        // In the post detail page, when clicking the "Nhận tư vấn" button,
        // it will activate this function
        [HttpPost]
        public async Task<JsonResult> CreateBooking(string senderId, string receiverId, string postId)
        {
            // Check if the person accessing the page is the owner or not
            if (senderId == receiverId)
            {
                return Json(new { logginedIn = false });
            }

            if (string.IsNullOrEmpty(senderId))
            {
                return Json(new { message = "Bạn phải đăng nhập để có thể nhận tư vấn!", success = false });
            }

            var senderDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == senderId)
                                                        .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(senderDoc.Info.Phone))
            {
                return Json(new { message = "Bạn chưa cập nhập số điện thoại!", success = false });
            }

            Booking booking = new Booking()
            {
                OwnerId = receiverId,
                ContactInfo = new ContactInfo()
                {
                    OwnerId = senderDoc.Id,
                    Name = senderDoc.Info.FullName,
                    Email = senderDoc.Info.Email,
                    Phone = senderDoc.Info.Phone,
                },
                PostId = postId
            };

            // I will reverse List<Booking> later

            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == receiverId)
                                                        .FirstOrDefaultAsync();

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(
                    f => f.Id == receiverId,
                    Builders<Motel.Models.UserAccount>.Update.Push(f => f.Bookings, booking)
                  );

            return Json(new { message = "Bạn sẽ nhận được phản hồi từ chủ bài viết!", sucess = true });
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> Bookings()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();
            var peopleHaveNoAppointmenetYet = new Dictionary<string, bool>();
            // userAccountId, booked, postId
            var peopleBooked = new Dictionary<string, bool>();

            ownerDoc.Bookings ??= new List<Booking>();

            foreach (var booking in ownerDoc.Bookings)
            {
                var key = booking.OwnerId + "_" + booking.PostId;

                if (!peopleBooked.ContainsKey(key))
                {
                    peopleBooked.Add(key, booking.IsReaded);
                }
            }

            //ownerDoc.PassiveReviewPersons ??= new List<string>();

            //foreach (var booking in ownerDoc.Bookings)
            //{
            //    if (!booking.IsReaded)
            //    {
            //        foreach (var passiveReviewPerson in ownerDoc.PassiveReviewPersons)
            //        {
            //            if (booking.ContactInfo.Owner == passiveReviewPerson)
            //            {
            //                continue;
            //            }
            //            else
            //            {
            //                if (!peopleDoNotRateYet.ContainsKey(booking.ContactInfo.Owner))
            //                {
            //                    peopleDoNotRateYet.Add(booking.ContactInfo.Owner, true);
            //                }
            //            }
            //        }
            //    }
            //}

            var model = new ModificationLayoutViewModel
            {
                Owner = ownerDoc,
                PeopleBooked = peopleBooked
            };

            return View(model);
        }

        // When owner click to the detail button for seeing detail of customer 
        // then owner will allow customer review myself
        [HttpPost]
        public async Task<JsonResult> ReadBooking(string senderId, string postId)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();
            var senderDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == senderId)
                                                        .FirstOrDefaultAsync();
            var found = false;

            for (int i = 0; i < ownerDoc.Bookings.Count; i++)
            {
                var booking = ownerDoc.Bookings[i];

                if (!booking.IsReaded)
                {
                    if (booking.ContactInfo.OwnerId == senderId && booking.PostId == postId)
                    {
                        booking.IsReaded = true;
                        found = true;

                        break;
                    }

                    if (i == ownerDoc.Bookings.Count - 1)
                    {
                        booking.IsReaded = true;
                    }
                }
            }

            senderDoc.ActiveReviewPersons ??= new List<string>();

            if (!senderDoc.ActiveReviewPersons.Contains(ownerDoc.Id))
            {
                senderDoc.ActiveReviewPersons.Add(ownerDoc.Id);
            }

            ownerDoc.PassiveReviewPersons ??= new List<string>();

            if (!ownerDoc.PassiveReviewPersons.Contains(senderDoc.Id))
            {
                ownerDoc.PassiveReviewPersons.Add(senderDoc.Id);
            }

            var senderFilter = Builders<Motel.Models.UserAccount>.Filter.Eq(f => f.Id, senderId);
            var ownerFilter = Builders<Motel.Models.UserAccount>.Filter.Eq(f => f.Id, ownerId);
            var senderUpdate = Builders<Motel.Models.UserAccount>.Update.Set(f => f.ActiveReviewPersons, senderDoc.ActiveReviewPersons);
            var ownerUpdate = Builders<Motel.Models.UserAccount>.Update.Combine(
                                        Builders<Motel.Models.UserAccount>.Update.Set(f => f.Bookings, ownerDoc.Bookings),
                                        Builders<Motel.Models.UserAccount>.Update.Set(f => f.PassiveReviewPersons, ownerDoc.PassiveReviewPersons)
                                        );

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(senderFilter, senderUpdate);
            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(ownerFilter, ownerUpdate);

            if (found)
            {
                return Json(new { isFirst = false });
            }
            else
            {
                return Json(new { message = "Bạn có thể đánh giá chủ bài viết!", isFirst = true });
            }
        }

        //public async Task<ActionResult> ViolatedPost(string postId)
        //{
        //    var postDoc = await _databaseConstructor.PostCollection
        //                                              .Find(f => f.Id == postId)
        //                                              .FirstOrDefaultAsync();

        //    if (!postDoc.State.IsViolated)
        //    {
        //        return RedirectToAction("Index", "Home", new { area = "Post" });
        //    }

        //    var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var ownerDoc = await _databaseConstructor.UserAccountCollection
        //                                               .Find(f => f.Id == ownerId)
        //                                               .FirstOrDefaultAsync();

        //    ViewData["postId"] = postId;

        //    ModificationLayoutViewModel model = new ModificationLayoutViewModel()
        //    {
        //        Owner = ownerDoc,
        //    };

        //    return View(model);
        //}

        [HttpPost]
        public async Task<JsonResult> CreateReponse(string postId)
        {
            //var postDoc = await _databaseConstructor.PostCollection
            //                                           .Find(f => f.Id == postId)
            //                                           .FirstOrDefaultAsync();

            //postDoc.State.IsEdited = true;

            //var updatePostDoc = Builders<Motel.Models.Post>.Update.Set(f => f.State, postDoc.State);
            //var filterPostDoc = Builders<Motel.Models.Post>.Filter.Eq(f => f.Id, postDoc.Id);

            //await _databaseConstructor.PostCollection.UpdateOneAsync(filterPostDoc, updatePostDoc);

            return Json(new { success = true, message = "Thành công" });
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> ViolatedPost(string postId)
        {
            var postDoc = await _databaseConstructor.PostCollection
                                                .Find(f => f.Id == postId)
                                                .FirstOrDefaultAsync();

            if (!postDoc.State.IsViolated)
            {
                return RedirectToAction("Index", "Home", new { area = "Post" });
            }

            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                     .Find(f => f.Id == userAccountId)
                                                     .FirstOrDefaultAsync();

            ownerDoc.Posts ??= new List<string>();

            if (!ownerDoc.Posts.Contains(postId))
            {
                return RedirectToAction("Index", "Home", new { area = "Post" });
            }

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                PostAdd = new PostAdd()
                {
                    PostId = postId,
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
        public async Task<IActionResult> ViolatedPost(ModificationLayoutViewModel model, IEnumerable<IFormFile> fileInput)
        {
            var userAccountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var postDoc = await _databaseConstructor.PostCollection
                                                        .Find(f => f.Id == model.PostAdd.PostId)
                                                        .FirstOrDefaultAsync();
            var images = new List<Motel.Models.Image>();

            if (fileInput.Count() != 0)
            {
                images = await ReturnListmagesFromView(fileInput);
            }
            else
            {
                images = postDoc.PostDetail.Images;
            }

            var postDetail = new Motel.Models.PostDetail
            {
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
            };
            var contactInfo = new ContactInfo()
            {
                Name = model.PostAdd.Name,
                Email = model.PostAdd.Email,
                Phone = model.PostAdd.Phone,
            };
            var updatePostDefinition = Builders<Motel.Models.Post>.Update
                                          .Set(f => f.SubjectOnSite, model.PostAdd.SubjectOnSite)
                                          .Set(f => f.PostDetail, postDetail)
                                          .Set(f => f.ContactInfo, contactInfo)
                                          .Set(f => f.State.IsEdited, true);
            var postFilter = Builders<Motel.Models.Post>.Filter.Eq(p => p.Id, model.PostAdd.PostId);

            await _databaseConstructor.PostCollection.UpdateOneAsync(postFilter, updatePostDefinition);

            return RedirectToAction("PostingList", "Post", new { area = "Customer" });
        }

        public async Task<ActionResult> InfoOfOwner()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();
            var info = new Motel.Areas.Customer.Models.Info()
            {
                OwnerId = ownerId,
                Avatar = !string.IsNullOrEmpty(ownerDoc.Info.Avatar) ?
                                                ownerDoc.Info.Avatar : "/images/150x150.png",
                FullName = ownerDoc.Info.FullName,
                Sex = ownerDoc.Info.Sex,
                Email = ownerDoc.Info.Email,
                Phone = !string.IsNullOrEmpty(ownerDoc.Info.Phone) ?
                                                ownerDoc.Info.Phone : ""
            };

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Info = info
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateInfo(ModificationLayoutViewModel model, IFormFile avatarFile)
        {
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == model.Info.OwnerId)
                                                        .FirstOrDefaultAsync();

            if (avatarFile != null)
            {
                var fileName = Path.GetFileName(avatarFile.FileName);
                var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                var filePath = Path.Combine(uploadPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(fileStream);
                }

                filePath = "\\images\\" + fileName;

                ownerDoc.Info.Avatar = filePath;
            }


            ownerDoc.Info.FullName = model.Info.FullName;
            ownerDoc.Info.Email = model.Info.Email;
            ownerDoc.Info.Phone = model.Info.Phone;
            ownerDoc.Info.Sex = model.Info.Sex;

            var filter = Builders<Motel.Models.UserAccount>.Filter.Eq(u => u.Id, model.Info.OwnerId);
            var update = Builders<Motel.Models.UserAccount>.Update
                                                            .Set(u => u.Info, ownerDoc.Info);

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(filter, update);

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, ownerDoc.Id.ToString()),
                new Claim(ClaimTypes.Name, ownerDoc.Info.FullName),
                new Claim(ClaimTypes.Email, ownerDoc.Email),
                new Claim(ClaimTypes.Role, ownerDoc.RoleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
            {
                IsPersistent = true
            });

            return RedirectToAction("InfoOfOwner", "Customer", new { area = "Customer" });
        }

        public IActionResult Checkout(string price)
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ownerId == null)
            {
                return RedirectToAction("Index", "Post");
            }

            // Request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string serectKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string orderInfo = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string returnUrl = $"https://localhost:7244/Customer/Customer/ReturnUrl?ownerId={ownerId}&price={price}";
            string notifyurl = "https://momo.vn/notify";
            string amount = price; // Tính toán tổng giá trị
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";
            string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

            MomoSecurity crypto = new MomoSecurity();

            string signature = crypto.SignSHA256(rawHash, serectKey);

            //build body json request
            JObject message = new JObject
                {
                    { "partnerCode", partnerCode },
                    { "accessKey", accessKey},
                    { "requestId", requestId },
                    { "amount", amount },
                    { "orderId", orderid },
                    { "orderInfo", orderInfo },
                    { "returnUrl", returnUrl },
                    { "notifyUrl", notifyurl },
                    { "extraData", extraData },
                    { "requestType", "captureMoMoWallet" },
                    { "signature", signature }
                };

            string responseFromMomo = PaymentRequest.SendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        public async Task<IActionResult> ReturnUrl(string ownerId, string price)
        {
            // Lấy query string và loại bỏ phần "signature"
            string param = Request.QueryString.ToString();

            int signatureIndex = param.IndexOf("signature");

            if (signatureIndex > 0)
            {
                param = param.Substring(0, signatureIndex - 1);
            }

            // Mã hóa URL bằng WebUtility.UrlEncode
            param = WebUtility.UrlEncode(param);

            MomoSecurity crypto = new MomoSecurity();

            string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string signature = crypto.SignSHA256(param, secretKey);

            // Check signature
            //if (signature != Request.Query["signature"].ToString())
            //{
            //    ViewData["Signature"] = signature;

            //    ViewData["Param"] = Request.Query["signature"].ToString();

            //    return View();
            //}

            // Kiểm tra mã lỗi
            if (!Request.Query["errorCode"].Equals("0"))
            {
                ViewData["IsSuccessful"] = false;
            }
            else
            {
                // Update bill
                var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync(); ;

                Bill bill = new Bill()
                {
                    OwnerId = ownerId,
                    Cost = int.Parse(price),
                    CostString = price + " VND"
                };

                await _databaseConstructor.BillCollection
                                        .InsertOneAsync(bill);

                ownerDoc.Bills ??= new List<Bill>();
                ownerDoc.Bills.Add(bill);

                var filterUserAccount = Builders<Motel.Models.UserAccount>.Filter.Eq(f => f.Id, ownerId);
                var updateUserAccount = Builders<Motel.Models.UserAccount>.Update
                                                .Inc(f => f.Balance, bill.Cost)
                                                .Set(f => f.Bills, ownerDoc.Bills);

                await _databaseConstructor.UserAccountCollection.UpdateOneAsync(filterUserAccount, updateUserAccount);

                ViewData["IsSuccessful"] = true;
            }

            return View();
        }

        public async Task<IActionResult> Bills()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(ownerId))
            {
                return RedirectToAction("Index", "Home", new { area = "Post" });
            }

            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();
            var bills = new List<Bill>();

            if (ownerDoc.RoleName == "Customer")
            {
                bills = ownerDoc.Bills;
            }
            else
            {
                var billDocs = await _databaseConstructor.BillCollection
                                                     .Find(_ => true)
                                                     .ToListAsync();

                billDocs.Reverse();

                bills = billDocs;
            }

            var model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
                Bills = bills
            };

            return View(model);
        }
    }
}
