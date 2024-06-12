"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Connect to group when going to page
connection.start().then(function () {
    var senderId = document.getElementById("senderId")?.value
    var receiverId = document.getElementById("receiverId").value;

    console.log(senderId, receiverId);

    // This is the case where the site owner enters
    if (senderId === undefined) {
        senderId = null;
    }

    console.log(senderId, receiverId);

    connection.invoke("JoinRatingGroup", senderId, receiverId)
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
connection.on("ReceiveRating", function (senderId, senderFullName, response) {
    var senderIdOnsite = document.getElementById("senderId")?.value
    console.log(senderIdOnsite)

    // Check if the client on the page is the sender
    // If not, receive this notification
    if (senderId !== senderIdOnsite) {
        toastNotification({
            title: "Thành công!",
            message: `${senderFullName} vừa mới đánh giá.`,
            type: "success",
            duration: 5000
        });
    }

    const review = {
        senderFullName: senderFullName,
        avatar: response.avatar ? response.avatar : "/images/50x50.png",
        rating: response.rating,
        comment: response.content
    };

    console.log(review)

    // I will create function for this later
    // ========== || ==========
    var noCommnetsP = document.getElementById('noComments');

    if (noCommnetsP) {
        noCommnetsP.parentNode.removeChild(noCommnetsP);
    }

    var cardDiv = document.createElement("div");

    cardDiv.className = "card mb-2";

    var cardBodyDiv = document.createElement("div");

    cardBodyDiv.className = "card-body d-flex align-items-center";

    var avatarDiv = document.createElement("div");

    avatarDiv.className = "avatar rounded-circle overflow-hidden mr-3";

    var img = document.createElement("img");

    img.src = review.avatar;
    img.alt = "Avatar";
    img.className = "img-fluid avatar-of-commenter";

    avatarDiv.appendChild(img);

    var contentDiv = document.createElement("div");

    var cardTitle = document.createElement("h5");

    cardTitle.className = "card-title";
    cardTitle.innerHTML = "<span>" + review.senderFullName + "</span> đã đánh giá " + review.rating + " sao";

    var cardText = document.createElement("p");

    cardText.className = "card-text";
    cardText.textContent = review.comment;

    contentDiv.appendChild(cardTitle);
    contentDiv.appendChild(cardText);

    cardBodyDiv.appendChild(avatarDiv);
    cardBodyDiv.appendChild(contentDiv);

    cardDiv.appendChild(cardBodyDiv);

    var reviewList = document.getElementById("review-list");
    var firstCard = reviewList.firstChild;

    if (firstCard) {
        reviewList.insertBefore(cardDiv, firstCard);
    } else {
        reviewList.appendChild(cardDiv);
    }
    // ========== || ==========
});

// Leave the group when leaving the page 
window.addEventListener("unload", function (event) {
    var senderId = document.getElementById("senderId")?.value
    var receiverId = document.getElementById("receiverId")?.value;

    console.log(sender, receiver)

    // This is the case where the site owner enters
    if (sender === undefined) {
        sender = receiver;
    }

    connection.invoke("LeaveRatingGroup", senderId, receiverId)
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
        var senderId = document.getElementById("senderId").value;
        var receiverId = document.getElementById("receiverId").value;
        var ratingString = document.getElementById("ratingValue").value;
        var ratingNumber = parseInt(ratingString);
        var content = document.getElementById("content").value;

        const response = {
            rating: ratingNumber,
            content: content
        };

        console.log(senderId, receiverId, response);

        if (receiverId.length > 0) {
            connection
                .invoke("SendRatingToGroup", senderId, receiverId, response)
                .then(() => {
                    connection.invoke("SendRatingToReceiver", senderId, receiverId, response)
                        .catch(function (err) {
                            console.error("Error at SendRatingToReceiver:" + err.toString());
                        });
                })
                .catch(function (err) {
                    console.error("Errỏr at SendRatingToGroup:" + err.toString());
                });
        }

        event.preventDefault();
    });
}