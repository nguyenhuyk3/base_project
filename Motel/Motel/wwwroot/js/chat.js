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
if (document.getElementById("sendButton")?.disabled !== undefined) {
    document.getElementById("sendButton").disabled = true;
}

// When receive message it will show new message
connection.on("ReceiveMessage", function (sender, content) {
    console.log(content)
    toastNotification({
        title: "Thành công!",
        message: `${sender} vừa mới đánh giá cho bạn.`,
        type: "success",
        duration: 5000
    });

    var card = document.createElement("div");

    card.className = "card mb-2";

    var cardBody = document.createElement("div");

    cardBody.className = "card-body";

    var cardTitle = document.createElement("h5");

    cardTitle.className = "card-title";

    var span = document.createElement("span");

    span.textContent = sender;

    var ratingText = document.createTextNode(` đã đánh giá ${content.rating} sao`);

    cardTitle.appendChild(span);
    cardTitle.appendChild(ratingText);

    var cardText = document.createElement("p");

    cardText.className = "card-text";
    cardText.textContent = content.comment;

    cardBody.appendChild(cardTitle);
    cardBody.appendChild(cardText);
    card.appendChild(cardBody);

    var reviewList = document.getElementById("review-list");

    // Lấy phần tử đầu tiên trong danh sách
    var firstCard = reviewList.firstChild;

    // Chèn thẻ mới vào trước phần tử đầu tiên
    if (firstCard) {
        reviewList.insertBefore(card, firstCard);
    } else {
        reviewList.appendChild(card); // Nếu danh sách rỗng, thêm vào cuối danh sách
    }
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
});

// `connection.start()`: This method is used to start the connection process to the SignalR server.
connection.start().then(function () {
    if (document.getElementById("sendButton")?.disabled !== undefined) {
        document.getElementById("sendButton").disabled = false;
    }
}).catch(function (err) {
    return console.error(err.toString());
});

if (document.getElementById("sendButton")) {
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var sender = document.getElementById("sender").value;
        var receiver = document.getElementById("receiver").value;
        var ratingString = document.getElementById("ratingValue").value;
        var ratingNumber = parseInt(ratingString);
        var comment = document.getElementById("comment").value;

        const content = {
            rating: ratingNumber,
            comment: comment
        }

        if (receiver.length > 0) {
            connection
                .invoke("SendMessageToReceiver", sender, receiver, content)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        }

        event.preventDefault();
    });
}




