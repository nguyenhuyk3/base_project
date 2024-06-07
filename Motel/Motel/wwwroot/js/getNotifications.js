"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().catch(function (err) {
    console.error(err.toString());
});

// When load page this function will get unreadnotifications of owner
document.addEventListener('DOMContentLoaded', function () {
    var receiverEmail = document.getElementById("receiverEmail").value;

    if (receiverEmail.length !== 0) {
        var xhr = new XMLHttpRequest();

        xhr.open('GET', '/Review/GetUnreadedNotifications?receiverEmail=' + receiverEmail, true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    console.log(response)

                    // This code will update notifications counter
                    var notificationsCounter = document.getElementById('notificationsCounter');

                    notificationsCounter.textContent = response.count.toString();
                    // ========== || ==========
                    response.unreadedNotifications.forEach(function (notification) {
                        // I will create function for this later
                        // ========== || ==========

                        // <div class="notification-item">
                        var notificationItem = document.createElement("div");

                        notificationItem.classList.add("notification-item");

                        // <div class="d-flex align-items-center">
                        var itemContent = document.createElement("div");

                        itemContent.classList.add("d-flex", "align-items-center");

                        // <img src="@src" alt="Ảnh người dùng">
                        var img = document.createElement("img");

                        img.src = notification.sender_img;
                        img.alt = "Ảnh người dùng";

                        // <div>
                        var textContainer = document.createElement("div");

                        // <h6>
                        var h6 = document.createElement("h6");
                        h6.textContent = notification.sender_full_name + " đã đánh giá cho bạn!";

                        // <p>
                        var p = document.createElement("p");
                        var createdAt = new Date(notification.created_at);
                        var formattedDate = createdAt.toLocaleString('vi-VN', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                        });

                        p.textContent = formattedDate;

                        textContainer.appendChild(h6);
                        textContainer.appendChild(p);

                        itemContent.appendChild(img);
                        itemContent.appendChild(textContainer);

                        notificationItem.appendChild(itemContent);

                        var notificationsMenu = document.querySelector('.dropdown-menu-notifications');

                        notificationsMenu.insertBefore(notificationItem, notificationsMenu.firstChild)
                        // ========== || ==========
                    });
                } else {
                    console.error("Đã xảy ra lỗi khi lấy dữ liệu từ server");
                }
            }
        };

        xhr.send();
    }
});


// This function receive notification when having notification
connection.on("ReceiveNotification", function (notification) {
    // Set notification counter
    // ========== || ==========
    var notificationsCounter = document.getElementById("notificationsCounter")
    var numberOfNotifications = parseInt(notificationsCounter.textContent);

    numberOfNotifications++;
    notificationsCounter.textContent = numberOfNotifications.toString();
    // ========== || ==========

    // I will create function for this later
    // ========== || ==========

    // <div class="notification-item">
    var notificationItem = document.createElement("div");

    notificationItem.classList.add("notification-item");

    // <div class="d-flex align-items-center">
    var itemContent = document.createElement("div");

    itemContent.classList.add("d-flex", "align-items-center");

    // <img src="@src" alt="Ảnh người dùng">
    var img = document.createElement("img");

    img.src = notification.sender_img;
    img.alt = "Ảnh người dùng";

    // <div>
    var textContainer = document.createElement("div");

    // <h6>
    var h6 = document.createElement("h6");
    h6.textContent = notification.sender_full_name + " đã đánh giá cho bạn!";

    // <p>
    var p = document.createElement("p");
    var createdAt = new Date(notification.created_at);
    var formattedDate = createdAt.toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    });

    p.textContent = formattedDate;

    textContainer.appendChild(h6);
    textContainer.appendChild(p);

    itemContent.appendChild(img);
    itemContent.appendChild(textContainer);

    notificationItem.appendChild(itemContent);

    var notificationsMenu = document.querySelector('.dropdown-menu-notifications');

    notificationsMenu.insertBefore(notificationItem, notificationsMenu.firstChild)
    // ========== || ==========
});


// When clicking into notification icon then this function will set counter to 0 
// and transform all posts have IsRead = 'false' to 'true'
if (document.getElementById("notificationsCounter")) {
    document.getElementById("notificationsCounter").addEventListener("click", () => {
        var ownerId = document.getElementById('ownerId').value;
        var xhr = new XMLHttpRequest();

        xhr.open('POST', '/Customer/Notification/CreateReponse?ownerId=' + ownerId, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    console.log(response)

                    if (response.success) {
                        var notificationsCounter = document.getElementById('notificationsCounter');

                        notificationsCounter.textContent = response.count;
                    }
                }
            }
        }

        xhr.send();
    });
}