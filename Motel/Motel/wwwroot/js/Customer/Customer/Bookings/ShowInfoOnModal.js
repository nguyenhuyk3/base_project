$('#infoModal').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget); // Nút đã kích hoạt modal
    var phone = button.data('phone'); // Truy xuất giá trị từ thuộc tính data-phone
    var email = button.data('email'); // Truy xuất giá trị từ thuộc tính data-email
    var name = button.data('name'); // Truy xuất giá trị từ thuộc tính data-name

    // Cập nhật nội dung của modal
    var modal = $(this);
    modal.find('#modalPhone').text(phone);
    modal.find('#modalEmail').text(email);
    modal.find('#modalName').text(name);
});