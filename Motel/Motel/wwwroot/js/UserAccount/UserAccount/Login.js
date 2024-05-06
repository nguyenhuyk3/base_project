document.addEventListener("DOMContentLoaded", function () {
    var checkbox = document.getElementById("RememberMe");
    var rememberMeInput = document.getElementById("RememberMeInput");

    checkbox.addEventListener("change", function () {
        rememberMeInput.value = checkbox.checked;
    });
});