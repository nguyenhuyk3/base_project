"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection
    .start()
    .catch(function (err) {
        return console.error(err.toString());
    });

$(document).ready(function () {
    $('#infoModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Nút đã kích hoạt modal
        var postId = button.data('id');
        var senderId = button.data('senderid');
        var phone = button.data('phone');
        var email = button.data('email');
        var name = button.data('name');

        // Cập nhật nội dung modal
        $('#modalPhone').text(phone);
        $('#modalEmail').text(email);
        $('#modalName').text(name);

        $.ajax({
            url: '/Customer/Customer/ReadBooking',
            type: 'POST',
            data: {
                senderId: senderId,
                postId: postId
            },
            success: function (response) {
                var reviewStatus = button.closest('tr').find('.review-status');
                console.log(response);

                if (!response.isFirst) {
                    reviewStatus.html('<i class="bi bi-check-circle-fill icon-green"></i>');

                    var receiverId = button.data('senderid');
                    var senderId = document.getElementById('OwnerId').value;

                    console.log(receiverId + " " + senderId);

                    connection
                        .invoke("SendReviewPermission", senderId, receiverId)
                        .catch(function (err) {
                            return console.error(err.toString());
                        });
                }
            },
            error: function () {
                toastNotification({
                    title: "Thất bại",
                    message: "Có lỗi xảy ra, vui lòng thử lại sau.",
                    type: "error",
                    duration: 5000
                });
            }
        });
    });
});
