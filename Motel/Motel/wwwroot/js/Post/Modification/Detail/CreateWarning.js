"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    if (document.getElementById("CreateWarning")?.disabled !== undefined) {
        document.getElementById("CreateWarning").disabled = false;
    }
}).catch(function (err) {
    return console.error(err.toString());
});

document.addEventListener('DOMContentLoaded', function () {
    var createWarningButton = document.getElementById('CreateWarning');

    createWarningButton.addEventListener('click', function (event) {
        event.preventDefault();

        var postId = document.getElementById('PostId').value;
        var xhr = new XMLHttpRequest();

        xhr.open('POST', '/Admin/Post/CreateWarning?postId=' + postId, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);
                    if (response.success) {
                        var senderId = document.getElementById('AdminId').value;
                        var receiverId = document.getElementById('OwnerId').value;
                        var content = "Quản trị viên đã cảnh báo bài viết của bạn!";

                        connection
                            .invoke("SendWarning", senderId, receiverId, content)
                            .catch(function (err) {
                                return console.error(err.toString());
                            });
                    } else {
                        alert('Cập nhật không thành công');
                    }
                } else {
                    alert('Đã có lỗi xảy ra');
                }
            }
        };

        xhr.send();
    });
});