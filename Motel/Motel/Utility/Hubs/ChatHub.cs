using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Motel.Areas.Customer.Models;
using Motel.Utility.Checking;
using Motel.Utility.Database;
using NuGet.Protocol.Plugins;
using System;

namespace Motel.Utility.Hubs
{
    public class ChatHub : Hub
    {
        // This `Dictionary` will be stored as key-value "email: List of id"
        private static Dictionary<string, List<string>> emailToIdsMapping = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>();

        public override Task OnConnectedAsync()
        {
            // This will return the currently logged in email
            var email = Context.User.Identity.Name;
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
        public async Task SendMessageToReceiver(string sender, string receiver, Content content)
        {
            // `receiver` is email
            var receiverId = emailToIdsMapping.GetValueOrDefault(receiver);

            if (receiverId.Count > 0)
            {
                await Clients.User(receiverId.FirstOrDefault()).SendAsync("ReceiveMessage", sender, content);
            }
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
        public async Task JoinRatingGroup(string sender, string receiver)
        {
            List<string> members;

            if (!groups.TryGetValue(receiver, out members))
            {
                members = new List<string>();

                members.Add(sender);

                groups.Add(receiver, members);
            }
            // This below line will return a `List<string>` corresponding to `email`
            else
            {
                members.Add(sender);
            }

            Motel.ConnectedUsers.mappings = groups.ToDictionary(pair => pair.Key, pair => pair.Value);

            var senderId = emailToIdsMapping.GetValueOrDefault(sender);

            if (senderId.Count > 0)
            {
                // The logged in clients will use their id for joining group
                // Group name will be email's owner of page 
                await Groups.AddToGroupAsync(Context.ConnectionId, "1");
            }
        }

        public async Task LeaveRatingGroup(string sender, string receiver)
        {
            List<string> members;

            if (emailToIdsMapping.TryGetValue(receiver, out members))
            {
                members.Remove(sender);
            }

            Motel.ConnectedUsers.mappings = groups.ToDictionary(pair => pair.Key, pair => pair.Value);

            var senderId = emailToIdsMapping.GetValueOrDefault(sender);

            if (senderId.Count > 0)
            {
                await Groups.RemoveFromGroupAsync(senderId.FirstOrDefault(), receiver);
            }
        }

        public async Task SendRatingToGroup(string sender, string receiver, Content content)
        {
            await Clients.Group("1").SendAsync("ReceiveRating", sender, content);
        }
    }
}
