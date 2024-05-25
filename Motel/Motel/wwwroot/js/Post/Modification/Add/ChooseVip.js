// Choose VIP
document.addEventListener('DOMContentLoaded', function () {
    var radios = document.querySelectorAll('input[type="radio"][name="options"]');
    var hiddenInput = document.getElementById('VipName');

    radios.forEach(function (radio) {
        radio.addEventListener('change', function () {
            // Cập nhật giá trị của thẻ input ẩn
            hiddenInput.value = this.value.split('-')[0]

            console.log(hiddenInput.value)

            // Xóa lớp 'active' khỏi tất cả các label
            radios.forEach(function (r) {
                r.parentNode.classList.remove('active');
            });

            // Thêm lớp 'active' cho label của radio đã chọn
            this.parentNode.classList.add('active');
        });
    });

    // Kích hoạt radio đầu tiên ban đầu
    var activeRadio = document.querySelector('input[type="radio"][name="options"]:checked');
    if (activeRadio) {
        activeRadio.parentNode.classList.add('active');
    }
});