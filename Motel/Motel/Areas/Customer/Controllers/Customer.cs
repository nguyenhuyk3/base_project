using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Info(string userAccountId, int? page)
        {
            if (page == null)
            {
                page = 1;
            }

            int pageSize = 5;
            int pageNum = page ?? 1;

            var owner = _databaseConstructor.UserAccountCollection
                              .Find(userAccount => userAccount.Id == userAccountId)
                              .FirstOrDefault();
            var clientId = _getter.GetLoginId();

            bool isClient = owner.Id != clientId;

            var reversedPassiveReviews = owner.PassiveReviews?.ToList();

            reversedPassiveReviews?.Reverse();

            var pagedReversedPassiveReviews = reversedPassiveReviews?.ToPagedList(pageNum, pageSize);

            ViewData["isClient"] = isClient;

            InfoViewModel model = new InfoViewModel()
            {
                Owner = owner,
                SenderId = _getter.GetLoginId(),
                ReviewsOnSite = pagedReversedPassiveReviews
            };

            return View(model);
        }

        [HttpGet]
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

        public IActionResult Payment()
        {
            return View();
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
