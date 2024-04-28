"use strict";

/* 
1. Create a HubConnectionBuilder object:
    `new signalR.HubConnectionBuilder()`: This is how to create an object.
    `HubConnectionBuilder`, part of the SignalR JavaScript client. This object is used to 
    establish and configure the SignalR connection.
2. Configure the hub URL:
    `withUrl("/chatHub")`: The withUrl() method is used to specify 
    the URL of the hub to which the client wants to connect. This path must match 
    the path to which you mapped the hub in your server code.
3. Build and create connections:
    `build()`: The build() method is called to create a HubConnection object from 
    the previously configured HubConnectionBuilder object. This object represents 
    the SignalR connection between the client and the server, 
    and it is used to send and receive realtime messages.
*/
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

// When receive message it will show new message
connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");

    document.getElementById("messagesList").appendChild(li);

    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

// `connection.start()`: This method is used to start the connection process to the SignalR server.
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    // var userAccountId = document.getElementById("userAccountId").value
    var message = document.getElementById("messageInput").value;

    // The `invoke()` method is used to send a request to 
    // the server to call a specific method on the hub.
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});




