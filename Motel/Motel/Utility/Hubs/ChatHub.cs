using DnsClient;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MongoDB.Driver;
using Motel.Areas.Customer.Models;
using Motel.Models;
using Motel.Utility.Checking;
using Motel.Utility.Database;
using NuGet.Protocol.Plugins;
using System;
using System.Reflection;
using System.Security.Claims;

namespace Motel.Utility.Hubs
{
    public class ChatHub : Hub
    {
        // This `Dictionary` will be stored as key-value "email: List of id"
        private static Dictionary<string, List<string>> emailToIdsMapping = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();
        private readonly DatabaseConstructor _databaseConstructor;

        public ChatHub(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseConstructor = new DatabaseConstructor(databaseSettings);
        }

        public override Task OnConnectedAsync()
        {
            // This will return the currently logged in email
            var email = Context.User.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Email)?.Value;
            var userAccountId = Context.UserIdentifier;

            if (email == null || userAccountId == null)
            {
                return base.OnConnectedAsync();
            }

            List<string> userAccountIds;

            // Check to see if having an email in `emailToIdsMapping`
            // This below line wil return `userAccountIds` = null
            if (!emailToIdsMapping.TryGetValue(email, out userAccountIds))
            {
                userAccountIds = new List<string>();

                userAccountIds.Add(userAccountId);

                emailToIdsMapping.Add(email, userAccountIds);
            }
            // This below line will return a `List<string>` corresponding to `email`
            else
            {
                userAccountIds.Add(userAccountId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var email = Context.User.Identity.Name;
            var userAccountId = Context.UserIdentifier;

            if (email == null || userAccountId == null)
            {
                return base.OnDisconnectedAsync(exception);
            }

            List<string> userAccountIds;

            if (emailToIdsMapping.TryGetValue(email, out userAccountIds))
            {
                userAccountIds.Remove(userAccountId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        // Single
        public async Task SendMessageToReceiver(string sender, string receiver, Response response)
        {
            // `receiver` is email
            var receiverId = emailToIdsMapping.GetValueOrDefault(receiver);

            if (receiverId.Count > 0)
            {
                await Clients.User(receiverId.FirstOrDefault()).SendAsync("ReceiveMessage", sender, response);
            }
        }

        // Single
        // This function will send rating to receiver and receive notificaiton
        public async Task SendRatingToReceiver(string senderId, string receiverId, Response response)
        {
            var receiverDocument = await _databaseConstructor.UserAccountCollection
                                                                .Find(userAccount => userAccount.Id == receiverId)
                                                                .FirstOrDefaultAsync();
            var senderDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(userAccount => userAccount.Id == senderId)
                                                        .FirstOrDefaultAsync();
            //var receiverIds = groups.GetValueOrDefault(senderDoc.Id);
            var rating = receiverDocument.Rating + response.Rating;

            Notification notification = new Notification()
            {
                SenderId = senderDoc.Id,
                Rating = response.Rating,
                Content = response.Content,
            };

            receiverDocument?.Notifications?.Add(notification);

            var updateDefinition = Builders<UserAccount>.Update
                                            .Combine(
                                                Builders<UserAccount>.Update.Set(f => f.Rating, rating),
                                                Builders<UserAccount>.Update.Push(f => f.Notifications, notification)
                                                );
            //var filter = Builders<UserAccount>.Filter.Eq(u => u.Id, receiverDocument.Id);

            await _databaseConstructor.UserAccountCollection.UpdateOneAsync(f => f.Id == receiverDocument.Id, updateDefinition);


            await Clients.User(receiverId).SendAsync("ReceiveNotification", notification);
            //if (receiverIds.Count > 0)
            //{
            //    await Clients.User(receiverIds.FirstOrDefault()).SendAsync("ReceiveNotification", notification);
            //}
        }

        //public async Task SendMessage(string user, string message)
        //{
        //    if (string.IsNullOrEmpty(user))
        //    {
        //        /*
        //        `Clients.All` to send a message to all client connections connecting to this hub. 
        //            `SendAsync` is a method provided by the SignalR client proxy to send messages to clients. 

        //        `"ReceiveMessage"` is the name of the method that the client will call to 
        //            process messages sent from the server. When the client receives this message, 
        //            it calls the specific method with the name `"ReceiveMessage"` 
        //            and passes the `user` and `message` parameters.

        //        `user` and `message` are the message data that needs to be sent to the client. 
        //            They are passed as parameters to the `"ReceiveMessage"` method on the client 
        //            when the message is sent from the server.
        //        */
        //        await Clients.All.SendAsync("ReceiveMessage", user, message);
        //    }
        //    else
        //    {
        //        await Clients.Client(user).SendAsync("ReceiveMessage", user, message);
        //    }
        //}

        // Group
        public async Task JoinRatingGroup(string senderId, string receiverId)
        {
            List<string> members;

            if (!groups.TryGetValue(receiverId, out members))
            {
                members = new List<string>();

                members.Add(senderId);

                groups.Add(receiverId, members);
            }
            // This below line will return a `List<string>` corresponding to `email`
            else
            {
                members.Add(senderId);
            }

            Motel.ConnectedUsers.mappings = groups.ToDictionary(pair => pair.Key, pair => pair.Value);

            var senderIds = groups.GetValueOrDefault(receiverId);

            if (senderIds.Count > 0)
            {
                // The logged in clients will use their id for joining group
                // Group name will be email's owner of page 
                await Groups.AddToGroupAsync(Context.ConnectionId, receiverId);
            }
        }

        public async Task LeaveRatingGroup(string senderId, string receiverId)
        {
            List<string> members;

            if (emailToIdsMapping.TryGetValue(receiverId, out members))
            {
                members.Remove(senderId);
            }

            Motel.ConnectedUsers.mappings = groups.ToDictionary(pair => pair.Key, pair => pair.Value);

            var senderIds = emailToIdsMapping.GetValueOrDefault(senderId);

            if (senderIds.Count > 0)
            {
                await Groups.RemoveFromGroupAsync(senderIds.FirstOrDefault(), receiverId);
            }
        }

        public async Task SendRatingToGroup(string senderId, string receiverId, Response response)
        {
            var senderDoc = await _databaseConstructor.UserAccountCollection
                                                        .Find(f => f.Id == senderId)
                                                        .FirstOrDefaultAsync();
            var senderFullName = senderDoc.Info.FullName;

            await Clients.Group(receiverId).SendAsync("ReceiveRating", senderId, senderFullName, response);
        }

        public async Task SendReviewPermission(string senderId, string receiverEmail, Response response)
        {
            var senderDoc = await _databaseConstructor.UserAccountCollection
                                                            .Find(f => f.Id == senderId)
                                                            .FirstOrDefaultAsync();
            var receiverDoc = await _databaseConstructor.UserAccountCollection
                                                           .Find(f => f.Email == receiverEmail)
                                                           .FirstOrDefaultAsync();
            var notification = new Notification()
            {
                SenderId = senderDoc.Id,
                FullName = senderDoc.Info.FullName,
                Content = response.Content,
            };

            senderDoc.ReviewPersons?.Add(receiverDoc.Id);
            receiverDoc.Notifications?.Add(notification);

            //var updateDefinition = Builders<UserAccount>.Update
            //                                .Combine(
            //                                    Builders<UserAccount>.Update.Push(f => f.Notifications, notification)
            //                                );

            //await _databaseConstructor.UserAccountCollection.UpdateOneAsync(f => f.Id == receiverDoc.Id, updateDefinition);

            var senderIds = emailToIdsMapping.GetValueOrDefault(senderDoc.Email);

            if (senderIds.Count > 0)
            {
                Clients.User(senderIds.FirstOrDefault()).SendAsync("ReceiveReviewPermission", notification);
            }
        }
    }
}
