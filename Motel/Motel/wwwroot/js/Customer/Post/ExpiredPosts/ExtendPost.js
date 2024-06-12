$(document).ready(function () {
    $('#extendPostModal').on('shown.bs.modal', function (e) {
        var postId = $(e.relatedTarget).data('postid');

        $(this).data('postId', postId);
    });

    $('#extendButton').click(function () {
        var postId = $('#extendPostModal').data('postId');
        var vipName = $('input[name=vipOption]:checked').val();

        $.ajax({
            url: '/Customer/Post/ExtendPost',
            type: 'POST',
            data: {
                postId: postId,
                vipName: vipName
            },
            success: function (response) {
                console.log(response)

                // When renewed successfully
                if (response.expired) {
                    toastNotification({
                        title: "Thành công",
                        message: response.message,
                        type: "success",
                        duration: 5000
                    });

                    window.location.reload();
                }
                // Error unable to update
                else if (!response.expired && !response.success) {
                    toastNotification({
                        title: "Thất bại",
                        message: response.message,
                        type: "error",
                        duration: 5000
                    });
                }
                // Insufficient balance
                else if (insufficientBalance) {
                    toastNotification({
                        title: "Thất bại",
                        message: response.message,
                        type: "error",
                        duration: 5000
                    });
                }
                // The article is expired
                else {
                    toastNotification({
                        title: "Thất bại",
                        message: response.message,
                        type: "error",
                        duration: 5000
                    });
                }
            },
            error: function () {
                // Xử lý khi có lỗi xảy ra trong quá trình gửi yêu cầu AJAX
                // Ví dụ: Hiển thị thông báo lỗi, vv.
                alert('Đã xảy ra lỗi khi gia hạn bài viết!');
            }
        });
    });
});