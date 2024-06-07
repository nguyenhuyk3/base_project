document.addEventListener("DOMContentLoaded", function () {
    var apiIdDropdown = document.getElementById('ApiId');
    var apiId = apiIdDropdown.value;
    var xhr = new XMLHttpRequest();

    xhr.open('GET', '/Address/GetDistricts?apiId=' + apiId);
    xhr.onload = function () {
        if (xhr.status === 200) {
            var data = JSON.parse(xhr.responseText);
            var districtDropdown = document.getElementById('District');

            districtDropdown.innerHTML = '';

            data.forEach(function (district) {
                var option = document.createElement('option');

                option.text = district;
                option.value = district;

                districtDropdown.appendChild(option);
            });
        } else {
            alert('Đã xảy ra lỗi khi lấy danh sách quận.');
        }
    };

    xhr.send();
});
