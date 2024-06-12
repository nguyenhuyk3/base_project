"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
   
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveWarning", function (notification) {
    console.log(notification);

    // Set notification counter 
    // ==========||==========
    var notificationcounter = document.getElementById('notificationsCounter');
    var numberOfNotificationcounter = parseInt(notificationcounter.textContent);

    numberOfNotificationcounter++;

    notificationcounter.textContent = numberOfNotificationcounter.toString();
    // ==========||==========

    // I will create function for this later
    // ==========||==========

    //  <div class="notification-item mt-2">
    const notificationItem = document.createElement('div');

    notificationItem.classList.add('notification-item', 'mt-2');

    const flexDiv = document.createElement('div');

    //  <div class="d-flex align-items-center" style="margin-bottom: 0;">
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

    updateNotifications();
});

connection.on("ReceiveResponse", function (notification) {
    console.log(notification);

    // Set notification counter
    // ==========||==========
    var notificationsCounter = document.getElementById('notificationsCounter');
    var numberOfNotification = parseInt(notificationsCounter.textContent);

    notificationsCounter.textContent = numberOfNotification.toString();
    // ==========||==========

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

    updateNotifications();
});