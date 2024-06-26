﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Motel.Areas.Customer.Models;
using Motel.Models;
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

        public async Task<JsonResult> SaveReview(string senderId, string receiverId, string response)
        {
            var contentObject = JsonConvert.DeserializeObject<Response>(response);
            var senderDoc = _databaseConstructor.UserAccountCollection
                                                    .Find(sender => sender.Id == senderId)
                                                    .FirstOrDefault();
            var receiverDoc = _databaseConstructor.UserAccountCollection
                                                    .Find(receiver => receiver.Id == receiverId)
                                                    .FirstOrDefault();
            var review = new Motel.Models.Review
            {
                SenderId = senderId,
                SenderFullName = senderDoc.Info.FullName,
                SenderAvatar = senderDoc.Info.Avatar,
                Comment = contentObject.Content,
                Rating = (int)contentObject.Rating
            };

            var totalRating = receiverDoc.TotalRating + (int)contentObject.Rating;
            var numberOfRatings = receiverDoc.PassiveReviews.Count + 1;
            var newRating = (float)totalRating / numberOfRatings;

            await _databaseConstructor.ReviewCollection.InsertOneAsync(review);

            var updateTasks = new List<Task<UpdateResult>>();

            if (senderDoc != null)
            {
                var senderUpdateTask = _databaseConstructor.UserAccountCollection.UpdateOneAsync(
                    u => u.Id == senderId,
                    Builders<Motel.Models.UserAccount>.Update.Push(u => u.ActiveReviews, review));

                updateTasks.Add(senderUpdateTask);
            }

            if (receiverDoc != null)
            {
                var receiverUpdateTask = _databaseConstructor.UserAccountCollection.UpdateOneAsync(
                    u => u.Id == receiverId,
                    Builders<Motel.Models.UserAccount>.Update
                                                            .Push(u => u.PassiveReviews, review)
                                                            .Set(u => u.TotalRating, totalRating)
                                                            .Set(u => u.Rating, newRating));

                updateTasks.Add(receiverUpdateTask);
            }

            await Task.WhenAll(updateTasks);

            var success = updateTasks.All(task => task.Result.ModifiedCount == 1);
            var message = success ? "Bạn đã gửi đánh giá thành công." : "Bạn đã đánh giá thất bại.";

            return new JsonResult(new { success, message });
        }

        // This function will take unread notifications
        // and add them to the list of read notifications
        [HttpGet]
        public async Task<JsonResult> GetUnreadedNotifications(string receiverEmail)
        {
            var receiverDocument = _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Email == receiverEmail)
                                                        .FirstOrDefault();
            var unreadNotifications = receiverDocument.Notifications ??= new List<Motel.Models.Notification>();
            var unreadedNotificationCount = 0;

            foreach (var notification in unreadNotifications)
            {
                if (!notification.IsReaded)
                {
                    notification.IsReaded = true;
                    unreadedNotificationCount++;
                }
            }

            var filterNotification = Builders<Motel.Models.UserAccount>.Filter.Eq(f => f.Id, receiverDocument.Id);
            var updateNotification = Builders<Motel.Models.UserAccount>.Update.Set(u => u.Notifications, unreadNotifications);

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(filterNotification, updateNotification);

            var NotiData = new
            {
                Count = unreadedNotificationCount,
                UnreadedNotifications = unreadNotifications
            };

            return new JsonResult(NotiData);
        }
    }
}

