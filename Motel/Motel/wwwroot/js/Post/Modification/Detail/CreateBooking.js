document.addEventListener('DOMContentLoaded', function () {
    var getBookingButton = document.getElementById('GetBooking');

    getBookingButton.addEventListener('click', function () {
        var ownerId = document.getElementById('OwnerId').value;
        var postId = document.getElementById('PostId').value;

        var xhr = new XMLHttpRequest();

        xhr.open('POST', '/Customer/Customer/CreateBooking?ownerId=' + ownerId + "&postId=" + postId);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);

                    if (response.sucess) {
                        toastNotification({
                            title: "Thành công!",
                            message: response.message,
                            type: "success",
                            duration: 5000
                        })

                        getBookingButton.classList.remove('btn-outline-info');
                        getBookingButton.classList.add('btn-success');
                        getBookingButton.textContent = 'Đã nhận tư vấn';
                        getBookingButton.disabled = true;
                    } else {
                        toastNotification({
                            title: "Thất bại!",
                            message: response.message,
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
