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

        // Thực hiện yêu cầu AJAX
        $.ajax({
            url: '/Customer/Customer/ReadBooking',
            type: 'POST',
            data: {
                senderId: senderId,
                postId: postId
            },
            success: function (response) {
                // Cập nhật trạng thái của tất cả các hàng có cùng senderId
                $('tr[data-senderid="' + senderId + '"]').each(function () {
                    var reviewStatus = $(this).find('.review-status');
                    if (!response.isFirst) {
                        reviewStatus.html('<p>Bạn đã tư vấn</p>');
                    }
                });
            },
            error: function () {
                toastNotification({
                    title: "Thất bại",
                    message: "Có lỗi xảy ra, vui lòng thử lại sau.",
                    type: "error",
                    duration: 5000
                })
            }
        });
    });
});
