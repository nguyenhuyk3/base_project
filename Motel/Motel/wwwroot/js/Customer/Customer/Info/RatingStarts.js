// Add an event to listen when a star is selected
var stars = document.querySelectorAll('input[name="rating"]');

stars.forEach(function (star) {
    star.addEventListener('change', function () {
        // Get the value of the selected star and assign it to the hidden field
        document.getElementById('ratingValue').value = this.value;
    });
});

// This function will save review into databse and shoot out a notification
function saveReview() {
    var senderId = document.getElementById("senderId").value;
    var receiverId = document.getElementById("receiverId").value;
    var ratingString = document.getElementById("ratingValue").value;
    var ratingNumber = parseInt(ratingString);
    var comment = document.getElementById("comment").value;

    const content = {
        rating: ratingNumber,
        comment: comment
    };

    var xhr = new XMLHttpRequest();
    var formData = new FormData();

    formData.append('senderId', senderId);
    formData.append('receiverId', receiverId);
    formData.append('content', JSON.stringify(content));

    console.log(senderId, receiverId, content)
    xhr.open('POST', '/Review/SaveReview')
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                // Xử lý kết quả thành công
                var response = JSON.parse(xhr.responseText);

                if (response.success) {
                    toastNotification({
                        title: "Thành công!",
                        message: response.message,
                        type: "success",
                        duration: 5000
                    });
                } else {
                    toastNotification({
                        title: "Thất bại",
                        message: response.message,
                        type: "warning",
                        duration: 5000
                    });
                }
            } else {
                toastNotification({
                    title: "Thất bại",
                    message: "Đã xảy ra lỗi khi gửi đánh giá.",
                    type: "error",
                    duration: 5000
                });
            }
        }
    };

    // Chuyển đổi dữ liệu thành chuỗi JSON và gửi request đi
    xhr.send(formData);
}

document.addEventListener("DOMContentLoaded", function () {
    // Chọn nút "Lưu đánh giá" bằng id
    var saveButton = document.getElementById("sendButton");

    if (saveButton) {
        // Gắn sự kiện click cho nút "Lưu đánh giá"
        saveButton.addEventListener("click", saveReview);
    }
});