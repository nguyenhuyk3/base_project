"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Connect to group when going to page
connection.start().then(function () {
    var sender = document.getElementById("sender")?.value
    var receiver = document.getElementById("receiver").value;

    console.log(sender, receiver);

    // This is the case where the site owner enters
    if (sender === undefined) {
        sender = receiver;
    }

    console.log(sender, receiver);

    connection.invoke("JoinRatingGroup", sender, receiver)
        .then(() => {
            if (document.getElementById("sendButton")?.disabled !== undefined) {
                document.getElementById("sendButton").disabled = false;
            }

            return toastNotification({
                title: "Thành công!",
                message: `Tham gia nhóm thành công.`,
                type: "success",
                duration: 5000
            });
        })
        .catch(function (err) {
            toastNotification({
                title: "Thất bại!",
                message: `Tham gia nhóm thất bại.`,
                type: "error",
                duration: 5000
            });
        });
}).catch(function (err) {
    return console.error(err.toString());
});

// Disable the send button until connection is established.
if (document.getElementById("sendButton")?.disabled !== undefined) {
    document.getElementById("sendButton").disabled = true;
}

// Listen for events when the `SendRatingToGroup` method is called
connection.on("ReceiveRating", function (sender, content) {
    var senderOnsite = document.getElementById("sender")?.value

    // Check if the client on the page is the sender
    // If not, receive this notification
    if (sender !== senderOnsite) {
        toastNotification({
            title: "Thành công!",
            message: `${sender} vừa mới đánh giá.`,
            type: "success",
            duration: 5000
        });
    }

    const review = {
        sender: sender,
        avatar: "~/images/images.jpg",
        rating: content.rating,
        comment: content.comment
    };

    // I will create function for this later
    // ========== || ==========
    // Create new div tag
    var cardDiv = document.createElement("div");

    cardDiv.className = "card mb-2";

    // Create a div tag for the card's content
    var cardBodyDiv = document.createElement("div");

    cardBodyDiv.className = "card-body d-flex align-items-center";

    // Create div tag for avatar
    var avatarDiv = document.createElement("div");

    avatarDiv.className = "avatar rounded-circle overflow-hidden mr-3";

    // Create image tag
    var img = document.createElement("img");
    img.src = review.avatar; // Đặt src tùy thuộc vào đường dẫn hình ảnh thực tế
    img.alt = "Avatar";
    img.className = "img-fluid avatar-of-commenter";

    // Add image to avatar card
    avatarDiv.appendChild(img);

    // Create div tag for review content
    var contentDiv = document.createElement("div");

    // Create card title
    var cardTitle = document.createElement("h5");
    cardTitle.className = "card-title";
    cardTitle.innerHTML = "<span>" + review.sender + "</span> đã đánh giá " + review.rating + " sao";

    // Create card title
    var cardText = document.createElement("p");
    cardText.className = "card-text";
    cardText.textContent = review.comment;

    // Add title and content to the content tag
    contentDiv.appendChild(cardTitle);
    contentDiv.appendChild(cardText);

    // Add child tags to the cardBody tag
    cardBodyDiv.appendChild(avatarDiv);
    cardBodyDiv.appendChild(contentDiv);

    // Add the cardBody tag to the card tag
    cardDiv.appendChild(cardBodyDiv);

    var reviewList = document.getElementById("review-list");
    var firstCard = reviewList.firstChild;

    if (firstCard) {
        reviewList.insertBefore(cardDiv, firstCard);
    } else {
        reviewList.appendChild(cardDiv); // If the list is empty, add it to the end of the list
    }
    // ========== || ==========
});

// Leave the group when leaving the page 
window.addEventListener("unload", function (event) {
    var sender = document.getElementById("sender")?.value
    var receiver = document.getElementById("receiver")?.value;

    console.log(sender, receiver)

    // This is the case where the site owner enters
    if (sender === undefined) {
        sender = receiver;
    }

    connection.invoke("LeaveRatingGroup", sender, receiver)
        .catch(function (err) {
            toastNotification({
                title: "Thất bại!",
                message: `Rời nhóm thất bại.`,
                type: "error",
                duration: 5000
            });
        });
});

// When clicking send rating, `SendRatingToGroup` will be called on the server
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
        };

        console.log(content);

        if (receiver.length > 0) {
            connection
                .invoke("SendRatingToGroup", sender, receiver, content)
                .catch(function (err) {
                    console.error(err.toString());
                });
        }

        event.preventDefault();
    });
}