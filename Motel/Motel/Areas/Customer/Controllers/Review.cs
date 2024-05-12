using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.Customer.Models;
using Motel.Utility.Database;
using Newtonsoft.Json;
using ZstdSharp.Unsafe;

namespace Motel.Areas.Customer.Controllers
{
    public class Review : Controller
    {
        private readonly DatabaseConstructor _databaseConstructor;

        public Review(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        public async Task<JsonResult> SaveReview(string senderId, string receiverId, string content)
        {
            var contentObject = JsonConvert.DeserializeObject<Content>(content);
            var sender = _databaseConstructor.UserAccountCollection
                                .Find(sender => sender.Id == senderId)
                                .FirstOrDefault();
            var receiver = _databaseConstructor.UserAccountCollection
                               .Find(receiver => receiver.Id == receiverId)
                               .FirstOrDefault();
            var review = new Motel.Models.Review
            {
                Sender = senderId,
                SenderEmail = sender.Email,
                Comment = contentObject.Comment,
                Rating = contentObject.Rating
            };

            await _databaseConstructor.ReviewCollection.InsertOneAsync(review);

            sender?.ActiveReviews?.Add(review);
            receiver?.PassiveReviews?.Add(review);

            var updateTasks = new List<Task<UpdateResult>>();

            if (sender != null)
            {
                var senderUpdateTask = _databaseConstructor.UserAccountCollection.UpdateOneAsync(
                    u => u.Id == senderId,
                    Builders<Motel.Models.UserAccount>.Update.Push(u => u.ActiveReviews, review));

                updateTasks.Add(senderUpdateTask);
            }

            if (receiver != null)
            {
                var receiverUpdateTask = _databaseConstructor.UserAccountCollection.UpdateOneAsync(
                    u => u.Id == receiverId,
                    Builders<Motel.Models.UserAccount>.Update.Push(u => u.PassiveReviews, review));

                updateTasks.Add(receiverUpdateTask);
            }

            await Task.WhenAll(updateTasks);

            var success = updateTasks.All(task => task.Result.ModifiedCount == 1);
            var message = success ? "Bạn đã gửi đánh giá thành công." : "Bạn đã đánh giá thất bại.";

            return new JsonResult(new { success, message });
        }
    }
}

