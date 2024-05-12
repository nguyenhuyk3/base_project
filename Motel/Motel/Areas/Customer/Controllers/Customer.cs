using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.Customer.Models;
using Motel.Models;
using Motel.Utility.Checking;
using Motel.Utility.Comparer;
using Motel.Utility.Database;
using System.Diagnostics.Metrics;
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

            var reversedPassiveReviews = owner?.PassiveReviews.ToList();

            reversedPassiveReviews?.Reverse();

            var pagedReversedPassiveReviews = reversedPassiveReviews.ToPagedList(pageNum, pageSize);

            ViewData["isClient"] = isClient;

            InfoViewModel model = new InfoViewModel()
            {
                Owner = owner,
                SenderId = _getter.GetLoginId(),
                ReviewsOnSite = pagedReversedPassiveReviews
            };

            return View(model);
        }
    }
}
