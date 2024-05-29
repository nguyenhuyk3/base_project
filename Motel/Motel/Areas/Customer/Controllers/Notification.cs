using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Motel.Areas.Customer.Models;
using Motel.Utility.Database;
using Newtonsoft.Json;

namespace Motel.Areas.Customer.Controllers
{
    public class Notification : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Notification(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }
        
        //public async Task<JsonResult> CreateReviewNotification(string sender, string fullName, string content)
        //{
        //    var contentObject = JsonConvert.DeserializeObject<Content>(content);
        //}
    }
}
