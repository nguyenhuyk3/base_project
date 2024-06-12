document.addEventListener('DOMContentLoaded', function () {
    var getBookingButton = document.getElementById('GetBooking');

    getBookingButton.addEventListener('click', function () {
        var senderId = document.getElementById('OwnerId').value;
        var receiverId = document.getElementById('OwnerIdOnSite').value;
        var postId = document.getElementById('PostId').value;

        console.log(senderId)

        var xhr = new XMLHttpRequest();

        xhr.open('POST', '/Customer/Customer/CreateBooking?senderId=' + senderId + "&receiverId=" + receiverId + "&postId=" + postId);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    console.log(response);
                    if (response.sucess) {
                        toastNotification({
                            title: "Thành công!",
                            message: response.message,
                            type: "success",
                            duration: 5000
                        })

                        getBookingButton.classList.remove('btn-outline-info');
                        getBookingButton.classList.add('btn-success');
                        getBookingButton.textContent = 'Đã gửi tư vấn';
                        getBookingButton.disabled = true;
                    } else {
                        toastNotification({
                            title: "Thất bại!",
                            message: "Gửi thất bại",
                            type: "error",
                            duration: 5000
                        })
                    }
                } else {
                    toastNotification({
                        title: "Lỗi!",
                        message: "Có lỗi xảy ra khi gửi yêu cầu.",
                        type: "error",
                        duration: 5000
                    });
                }
            }
        };

        xhr.send();

    });
});
