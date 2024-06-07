"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
   
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveWarning", function (notification) {
    console.log(notification);

    // ==========||==========
    var notificationcounter = document.getElementById('notificationsCounter');
    var numberOfNotificationcounter = parseInt(notificationcounter.textContent);

    numberOfNotificationcounter++;

    notificationcounter.textContent = numberOfNotificationcounter.toString();
    // ==========||==========

    // I will create function for this later
    // ==========||==========
    // <div class="notification-item">
    var notificationItem = document.createElement('div');
   
    notificationItem.classList.add('notification-item');

    //  <div class="d-flex align-items-center">
    var contentContainer = document.createElement('div');
   
    contentContainer.classList.add('d-flex', 'align-items-center');

    // <img src="notification.sender_img" alt="Ảnh admin">
    var imgElement = document.createElement('img');
   
    imgElement.src = notification.sender_img;
    imgElement.alt = 'Ảnh admin';

    // <div>
    var textContainer = document.createElement('div');
   
    // <h6>
    var h6Element = document.createElement('h6');
   
    h6Element.textContent = notification.content;

    // <p>
    var pElement = document.createElement('p');
    var createdAt = new Date(notification.created_at);
    var formattedDate = createdAt.toLocaleString('vi-VN', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    });
   
    pElement.textContent = formattedDate;

    textContainer.appendChild(h6Element);
    textContainer.appendChild(pElement);
    contentContainer.appendChild(imgElement);
    contentContainer.appendChild(textContainer);
    notificationItem.appendChild(contentContainer);

    var notificationsDropdownMenu = document.querySelector('.dropdown-menu-notifications');
    
    notificationsDropdownMenu.insertBefore(notificationItem, notificationsDropdownMenu.firstChild);
});

connection.on("ReceiveReponse", function (notification) {
    console.log(notification);

    var notificationsCounter = document.getElementById('notificationsCounter');
    var numberOfNotification = parseInt(notificationsCounter.textContent);

    notificationsCounter.textContent = numberOfNotification.toString();
});