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

                    // Set notification counter
                    // ========== || ==========
                    var notificationsCounter = document.getElementById('notificationsCounter');

                    notificationsCounter.textContent = response.count.toString();
                    // ========== || ==========

                    response.unreadedNotifications.forEach(function (notification) {
                        // I will create function for this later
                        // ========== || ==========

                        // <div class="notification-item mt-2">
                        const notificationItem = document.createElement('div');

                        notificationItem.classList.add('notification-item', 'mt-2');

                        // <div class="d-flex align-items-center" style="margin-bottom: 0;">
                        const flexDiv = document.createElement('div');

                        flexDiv.classList.add('d-flex', 'align-items-center');
                        flexDiv.style.marginBottom = '0';

                        // <img src="notification.sender_img" alt="Ảnh admin">
                        const img = document.createElement('img');

                        img.src = notification.sender_img;
                        img.alt = "Ảnh người dùng";

                        // <div class="d-block">
                        const textDiv = document.createElement('div');

                        textDiv.classList.add('d-block', 'content-block');

                        // <h6>
                        const title = document.createElement('h6');

                        title.textContent = notification.content;

                        // <p class="mt-3">
                        const description = document.createElement('p');
                        const createdAt = new Date(notification.created_at);
                        const formattedDate = createdAt.toLocaleString('vi-VN', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour: '2-digit',
                            minute: '2-digit',
                        });

                        description.classList.add('mt-3');
                        description.textContent = formattedDate;

                        textDiv.appendChild(title);
                        textDiv.appendChild(description);

                        flexDiv.appendChild(img);
                        flexDiv.appendChild(textDiv);

                        notificationItem.appendChild(flexDiv);

                        var notificationsDropdownMenu = document.querySelector('.dropdown-menu-notifications');

                        notificationsDropdownMenu.insertBefore(notificationItem, notificationsDropdownMenu.firstChild);
                        // ========== || ==========
                    });

                    updateNotifications();
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
    console.log(notification)

    // Set notification counter
    // ========== || ==========
    var notificationsCounter = document.getElementById("notificationsCounter")
    var numberOfNotifications = parseInt(notificationsCounter.textContent);

    numberOfNotifications++;
    notificationsCounter.textContent = numberOfNotifications.toString();
    // ========== || ==========

    // I will create function for this later
    // ==========||==========

    //  <div class="notification-item mt-2">
    const notificationItem = document.createElement('div');

    notificationItem.classList.add('notification-item', 'mt-2');

    //  <div class="d-flex align-items-center" style="margin-bottom: 0;">
    const flexDiv = document.createElement('div');

    flexDiv.classList.add('d-flex', 'align-items-center');
    flexDiv.style.marginBottom = '0';

    // <img src="notification.sender_img" alt="Ảnh admin">
    const img = document.createElement('img');

    img.src = notification.sender_img;
    img.alt = 'Ảnh người dùng';

    // <div class="d-block">
    const textDiv = document.createElement('div');

    textDiv.classList.add('d-block', 'content-block');

    // <h6>
    const title = document.createElement('h6');

    title.textContent = notification.content;

    // <p class="mt-3">
    const description = document.createElement('p');
    const createdAt = new Date(notification.created_at);
    const formattedDate = createdAt.toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    });

    description.classList.add('mt-3');
    description.textContent = formattedDate;

    textDiv.appendChild(title);
    textDiv.appendChild(description);

    flexDiv.appendChild(img);
    flexDiv.appendChild(textDiv);

    notificationItem.appendChild(flexDiv);

    var notificationsDropdownMenu = document.querySelector('.dropdown-menu-notifications');

    notificationsDropdownMenu.insertBefore(notificationItem, notificationsDropdownMenu.firstChild);
    // ========== || ==========
});

// This function receive notification when having notification
connection.on("ReceiveReviewPermission", function (notification) {
    console.log(notification);

    // Set notification counter
    // ========== || ==========
    var notificationsCounter = document.getElementById("notificationsCounter")
    var numberOfNotifications = parseInt(notificationsCounter.textContent);

    numberOfNotifications++;
    notificationsCounter.textContent = numberOfNotifications.toString();
    // ========== || ==========

    // I will create function for this later
    // ==========||==========

    //  <div class="notification-item mt-2">
    const notificationItem = document.createElement('div');

    notificationItem.classList.add('notification-item', 'mt-2');

    //  <div class="d-flex align-items-center" style="margin-bottom: 0;">
    const flexDiv = document.createElement('div');

    flexDiv.classList.add('d-flex', 'align-items-center');
    flexDiv.style.marginBottom = '0';

    // <img src="notification.sender_img" alt="Ảnh admin">
    const img = document.createElement('img');

    img.src = notification.sender_img;
    img.alt = 'Ảnh người dùng';

    // <div class="d-block">
    const textDiv = document.createElement('div');

    textDiv.classList.add('d-block', 'content-block');

    // <h6>
    const title = document.createElement('h6');

    title.textContent = notification.content;

    // <p class="mt-3">
    const description = document.createElement('p');
    const createdAt = new Date(notification.created_at);
    const formattedDate = createdAt.toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    });

    description.classList.add('mt-3');
    description.textContent = formattedDate;

    textDiv.appendChild(title);
    textDiv.appendChild(description);

    flexDiv.appendChild(img);
    flexDiv.appendChild(textDiv);

    notificationItem.appendChild(flexDiv);

    var notificationsDropdownMenu = document.querySelector('.dropdown-menu-notifications');

    notificationsDropdownMenu.insertBefore(notificationItem, notificationsDropdownMenu.firstChild);
    // ========== || ==========
});


// When clicking into notification icon then this function will set counter to 0 
// and transform all posts have IsRead = 'false' to 'true'
if (document.getElementById("notificationsDropdown")) {
    document.getElementById("notificationsDropdown").addEventListener("click", () => {
        var ownerId = document.getElementById('OwnerId').value;

        console.log(ownerId)

        var xhr = new XMLHttpRequest();

        xhr.open('POST', '/Customer/Notification/ReadNotifications?ownerId=' + ownerId, true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    console.log(response)

                    if (response.success) {
                        // Set counter
                        // ========== || ==========
                        var notificationsCounter = document.getElementById('notificationsCounter');

                        notificationsCounter.textContent = response.count;
                        // ========== || ==========
                    }
                }
            }
        }

        xhr.send();
    });
}