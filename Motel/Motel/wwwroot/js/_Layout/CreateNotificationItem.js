function createNotificationItem(notification) {
    // <div class="notification-item">
    var notificationItem = document.createElement("div");

    notificationItem.classList.add("notification-item");

    // <div class="d-flex align-items-center">
    var itemContent = document.createElement("div");

    itemContent.classList.add("d-flex", "align-items-center");

    // <img src="https://via.placeholder.com/50" alt="User 3">
    var img = document.createElement("img");
    img.src = notification.senderImg;
    img.alt = "Ảnh người dùng";

    // <div>
    var textContainer = document.createElement("div");

    // <h6>
    var h6 = document.createElement("h6");
    h6.textContent = "xyz";

    var p = document.createElement("p");
    p.textContent = "abc";

    textContainer.appendChild(h6);
    textContainer.appendChild(p);

    itemContent.appendChild(img);
    itemContent.appendChild(textContainer);

    notificationItem.appendChild(itemContent);

    return notificationItem;
}
