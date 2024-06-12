document.addEventListener('DOMContentLoaded', function () {
    // Select the dropdown for cities
    var provinceIdDropdown = document.getElementById('ProvinceId');

    // Add event listener for change event
    // Get districts
    provinceIdDropdown.addEventListener('change', function () {
        var provinceId = this.value;
        // Send GET request to server to get districts
        var xhr = new XMLHttpRequest();

        xhr.open('GET', '/Address/GetDistricts?provinceId=' + provinceId);

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
});

document.addEventListener("DOMContentLoaded", function () {
    // When you click to select a district,
    // it will retrieve the awards in the city
    document.getElementById('District').addEventListener('change', function () {
        var district = this.value;
        var provinceId = document.getElementById('ProvinceId').value;
        var xhr = new XMLHttpRequest();

        xhr.open('GET', '/Address/GetWards?provinceId=' + provinceId + '&districtName=' + district, true);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE && xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                var wardDropdown = document.getElementById('Ward');

                wardDropdown.innerHTML = ''; // Clear previous options

                data.forEach(function (ward) {
                    var option = document.createElement('option');

                    option.textContent = ward;
                    option.value = ward;

                    wardDropdown.appendChild(option);
                });
            } else if (xhr.readyState === XMLHttpRequest.DONE && xhr.status !== 200) {
                alert('Đã xảy ra lỗi khi lấy danh sách phường.');
            }
        };

        xhr.send();
    });
});

document.addEventListener("DOMContentLoaded", function () {
    // Gets streets
    document.getElementById('Ward').addEventListener('change', function () {
        var provinceId = document.getElementById('ProvinceId').value;
        var district = document.getElementById('District').value;
        var ward = this.value;
        var xhr = new XMLHttpRequest();

        xhr.open('GET', '/Address/GetStreets?provinceId=' + provinceId + '&district=' + district + '&ward=' + ward, true);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE && xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                var streetDropdown = document.getElementById('Street');

                streetDropdown.innerHTML = ''; // Clear previous options

                data.forEach(function (street) {
                    var option = document.createElement('option');

                    option.textContent = street;
                    option.value = street;

                    streetDropdown.appendChild(option);
                });
            } else if (xhr.readyState === XMLHttpRequest.DONE && xhr.status !== 200) {
                alert('Đã xảy ra lỗi khi lấy danh sách đường.');
            }
        };

        xhr.send();
    });
});



