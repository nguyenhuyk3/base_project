function addNotificationToList(notification) {
    var listItem = document.createElement('li');

    listItem.classList.add('list-group-item');

    var headerDiv = document.createElement('div');

    headerDiv.classList.add('d-flex', 'w-100', 'justify-content-between');

    var headerTitle = document.createElement('h5');

    headerTitle.classList.add('mb-1');
    headerTitle.textContent = notification.email + ' đã đánh giá cho bạn.';

    headerDiv.appendChild(headerTitle);
    listItem.appendChild(headerDiv);

    var list = document.getElementById("notificationList");

    if (list.firstChild) {
        list.insertBefore(listItem, list.firstChild);
    } else {
        list.appendChild(listItem);
    }
}

window.addNotificationToList = addNotificationToList;