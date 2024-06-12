document.getElementById('ChooseImageButton').addEventListener('click', function () {
    document.getElementById('AvatarFile').click();
});

document.getElementById('AvatarFile').addEventListener('change', function (event) {
    var input = event.target;

    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('avatarPreview').src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);

        // Update the hidden input with the selected file's name
        document.getElementById('Info.Avatar').value = input.files[0].name;
    }
});