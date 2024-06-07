using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

        [HttpPost]
        public async Task<JsonResult> ReadNotifications(string ownerId)
        {
            var ownerDoc = await _databaseConstructor.UserAccountCollection
                                                .Find(f => f.Id == ownerId)
                                                .FirstOrDefaultAsync();
            var unreadNotifications = ownerDoc.Notifications ??= new List<Motel.Models.Notification>();

            unreadNotifications.Reverse();

            foreach (var notification in unreadNotifications)
            {
                if (!notification.IsReaded)
                {
                    notification.IsReaded = true;
                }
                else
                {
                    break;
                }
            }

            var filterNotification = Builders<Motel.Models.UserAccount>.Filter.Eq(f => f.Id, ownerId);
            var updateNotification = Builders<Motel.Models.UserAccount>.Update.Set(u => u.Notifications, unreadNotifications);

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(filterNotification, updateNotification);

            return Json(new { success = true, count = 0 }); ;
        }
    }
}
