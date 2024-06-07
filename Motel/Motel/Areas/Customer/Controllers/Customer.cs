using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Hosting;
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
        private readonly Getter _getter;

        public Customer(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
            _getter = new Getter(new HttpContextAccessor());
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
                ReviewsOnSite = pagedReversedPassiveReviews
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "RequireCustomer")]
        public async Task<IActionResult> PostingList(string ownerId)
        {
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                      .Find(f => f.Id == ownerId)
                                                      .FirstOrDefaultAsync();

            ModificationLayoutViewModel model = new ModificationLayoutViewModel()
            {
                Owner = ownerDoc,
            };

            return View(model);
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
        public async Task<JsonResult> CreateBooking(string ownerId, string postId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the person accessing the page is the owner or not
            if (senderId == ownerId)
            {
                return Json(new { logginedIn = false });
            }

            if (string.IsNullOrEmpty(senderId))
            {
                return Json(new { message = "Bạn phải đăng nhập để có thể nhận tư vấn!", sucess = false });
            }

            var senderDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == senderId)
                                                        .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(senderDoc.Info.Phone))
            {
                return Json(new { message = "Bạn chưa cập nhập số điện thoại!", sucess = false });
            }

            Booking booking = new Booking()
            {
                Owner = ownerId,
                ContactInfo = new ContactInfo()
                {
                    Owner = senderDoc.Id,
                    Name = senderDoc.Info.FullName,
                    Email = senderDoc.Info.Email,
                    Phone = senderDoc.Info.Phone,
                },
                PostId = postId
            };

            // I will reverse List<Booking> later

            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(
                    f => f.Id == ownerId,
                    Builders<Motel.Models.UserAccount>.Update.Push(f => f.Bookings, booking)
                  );

            return Json(new { message = "Bạn sẽ nhận được phản hồi từ chủ bài viết!", sucess = true });
        }

        [HttpGet]
        public async Task<IActionResult> Bookings()
        {
            var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == ownerId)
                                                        .FirstOrDefaultAsync();
            var peopleDoNotRateYet = new Dictionary<string, bool>();

            ownerDoc.PassiveReviewPersons ??= new List<string>();

            foreach (var booking in ownerDoc.Bookings)
            {
                if (!booking.IsReaded)
                {
                    foreach (var passiveReviewPerson in ownerDoc.PassiveReviewPersons)
                    {
                        if (booking.ContactInfo.Owner == passiveReviewPerson)
                        {
                            continue;
                        }
                        else
                        {
                            if (!peopleDoNotRateYet.ContainsKey(booking.ContactInfo.Owner))
                            {
                                peopleDoNotRateYet.Add(booking.ContactInfo.Owner, true);
                            }
                        }
                    }
                }
            }

            var model = new ModificationLayoutViewModel
            {
                Owner = ownerDoc,
                PeopleDoNotRateYet = peopleDoNotRateYet
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

                if (booking.ContactInfo.Owner == senderId && booking.PostId == postId)
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

            senderDoc.ActiveReviewPersons ??= new List<string>();
            senderDoc.ActiveReviewPersons.Add(ownerDoc.Id);

            var senderFilter = Builders<Motel.Models.UserAccount>.Filter.Eq(f => f.Id, senderId);
            var ownerFilter = Builders<Motel.Models.UserAccount>.Filter.Eq(f => f.Id, ownerId);
            var senderUpdate = Builders<Motel.Models.UserAccount>.Update.Set(f => f.ActiveReviewPersons, senderDoc.ActiveReviewPersons);
            var ownerUpdate = Builders<Motel.Models.UserAccount>.Update.Set(f => f.Bookings, ownerDoc.Bookings);

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
                    PostId = postId,
                    CategoryId = categories.Find(f => f.Name == postDoc.CategoryName).Id,
                    ApiId = cities.Find(f => f.Name == postDoc.PostDetail.AddressDetail.City).ApiId.ToString(),
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
                    Owner = ownerId,
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
    }
}
