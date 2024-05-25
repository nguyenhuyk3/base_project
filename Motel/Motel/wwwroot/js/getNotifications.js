"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().catch(function (err) {
    console.error(err.toString());
});
document.addEventListener('DOMContentLoaded', function () {
    var receiverEmail = document.getElementById("receiverEmail").value;

    if (receiverEmail.length !== 0) {
        console.log(receiverEmail)

        var xhr = new XMLHttpRequest();

        xhr.open('GET', '/Review/GetUnreadedNotifications?receiverEmail=' + receiverEmail, true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    console.log(response)

                    // This code will update notifications counter
                    var notiCounterOnHead = document.getElementById("notiCounterOnHead")
                    var ikrNoti_ounter = document.getElementById("ikrNoti_Counter")

                    notiCounterOnHead.textContent = response.count.toString();
                    ikrNoti_ounter.textContent = response.count.toString()
                    // ==========||==========

                    response.unreadedNotifications.forEach(function (notification) {
                        // I will create function for this later
                        // ========== || ==========
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


connection.on("ReceiveNotification", function (notification) {
    console.log(notification)

    var ikrNotiCounter = document.getElementById("ikrNoti_Counter")
    var count = parseInt(ikrNotiCounter.textContent);

    console.log(ikrNotiCounter.textContent)

    count += 1;

    ikrNotiCounter.textContent = count.toString();

    // I will create function for this later
    // ========== || ==========
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
    // ========== || ==========
});

if (document.getElementById("ikrNoti_Button")) {
    document.getElementById("ikrNoti_Button").addEventListener("click", () => {
        var notiCounterOnHead = document.getElementById("notiCounterOnHead")
        var ikrNoti_ounter = document.getElementById("ikrNoti_Counter")

        console.log(notiCounterOnHead.textContent + " " + ikrNoti_ounter.textContent);

        notiCounterOnHead.textContent = 0;
        ikrNoti_ounter.textContent = 0;
    })
}