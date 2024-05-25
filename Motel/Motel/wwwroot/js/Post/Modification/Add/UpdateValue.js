// Automatically fill in information from DropList into the address box
function updateAddress() {
    var city = document.getElementById("ApiId").options[document.getElementById("ApiId").selectedIndex].text;

    var districtElement = document.getElementById("District");
    let district = "";

    if (districtElement.options[districtElement.selectedIndex]?.text !== undefined) {
        district = districtElement.options[districtElement.selectedIndex].text;
    }

    var wardElement = document.getElementById("Ward");
    let ward = "";

    if (wardElement.options[wardElement.selectedIndex]?.text !== undefined) {
        ward = wardElement.options[wardElement.selectedIndex].text;
    }

    var street = document.getElementById("Street").value;
    var address = city + ", " + district + ", " + ward + ", " + street;

    document.getElementById("Address").value = address;
}

document.getElementById("ApiId").addEventListener("change", updateAddress);
document.getElementById("District").addEventListener("change", updateAddress);
document.getElementById("Ward").addEventListener("change", updateAddress);
document.getElementById("Street").addEventListener("change", updateAddress);

// Increase or decrease values for cells
function decreaseValue(fieldId) {
    var field = document.getElementById(fieldId);
    var quantity = parseInt(field.value);

    if (quantity > 0) {
        field.value = quantity - 1;
    }
}

function increaseValue(fieldId) {
    var field = document.getElementById(fieldId);
    var quantity = parseInt(field.value);

    field.value = quantity + 1;
}

document.querySelector('.btn-decrease-bedroom').addEventListener('click', function () {
    decreaseValue('Bedroom');
});

document.querySelector('.btn-increase-bedroom').addEventListener('click', function () {
    increaseValue('Bedroom');
});

document.querySelector('.btn-decrease-toilet').addEventListener('click', function () {
    decreaseValue('Toilet');
});

document.querySelector('.btn-increase-toilet').addEventListener('click', function () {
    increaseValue('Toilet');
});

document.querySelector('.btn-decrease-floor').addEventListener('click', function () {
    decreaseValue('Floor');
});

document.querySelector('.btn-increase-floor').addEventListener('click', function () {
    increaseValue('Floor');
});

document.getElementById('ChooseImageButton').addEventListener('click', function () {
    document.getElementById('FileInput').click();
});

// Choose furniture 
document.addEventListener("DOMContentLoaded", function () {
    var btnCustom = document.querySelectorAll(".btn-custom");

    function handleClick() {
        var furniture = this.textContent;

        var cleanedFurnitureValue = furniture.replace(/[\r\n]+/g, "").trim();

        document.getElementById("Furniture").value = cleanedFurnitureValue;

        btnCustom.forEach(function (btn) {
            btn.classList.remove("btn-primary");
            btn.classList.add("btn-secondary");
        });

        this.classList.remove("btn-secondary");
        this.classList.add("btn-primary");
    }

    btnCustom.forEach(function (btn) {
        btn.addEventListener("click", handleClick);
    });
});

// Choose images 
let selectedImages = [];

document.getElementById('FileInput').addEventListener('change', function () {
    var imagePreview = document.getElementById('ImagePreview');

    // Get the list of selected files
    var files = this.files;

    // Check the number of new files with the number of selected files
    if (selectedImages.length + files.length > 10) {
        alert('Bạn chỉ được chọn tối đa 10 ảnh!');
        return;
    }

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var reader = new FileReader();

        // Check if the file is selected
        if (!selectedImages.includes(file)) {
            // If not, add to the list of selected images
            selectedImages.push(file);
        }

        // Displays image   
        reader.onload = function (event) {
            var img = document.createElement('img');

            img.src = event.target.result;
            img.classList.add('preview-image');

            imagePreview.appendChild(img);
        };

        reader.readAsDataURL(file); // Đọc dữ liệu của file dưới dạng URL
    }
})


