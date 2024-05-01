using Microsoft.AspNetCore.SignalR;

namespace Motel.Utility.Hubs
{
    public class ChatHub : Hub
    {

        public override Task OnConnectedAsync()
        {

            ConnectedUsers.myConnectedUsers.Add(Context.ConnectionId);


            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnectedUsers.myConnectedUsers.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            if (string.IsNullOrEmpty(user))
            {
                /*
                `Clients.All` to send a message to all client connections connecting to this hub. 
                    `SendAsync` is a method provided by the SignalR client proxy to send messages to clients. 

                `"ReceiveMessage"` is the name of the method that the client will call to 
                    process messages sent from the server. When the client receives this message, 
                    it calls the specific method with the name `"ReceiveMessage"` 
                    and passes the `user` and `message` parameters.

                `user` and `message` are the message data that needs to be sent to the client. 
                    They are passed as parameters to the `"ReceiveMessage"` method on the client 
                    when the message is sent from the server.
                */
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            else
            {
                await Clients.Client(user).SendAsync("ReceiveMessage", user, message);
            }
        }


    }
}
