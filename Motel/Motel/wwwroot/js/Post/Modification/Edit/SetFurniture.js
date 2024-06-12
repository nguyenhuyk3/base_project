document.addEventListener("DOMContentLoaded", function () {
    var buttons = document.querySelectorAll(".btn-custom");
    var funiture = document.getElementById("Furniture");

    function updateButtonStyles() {
        buttons.forEach(function (button) {
            if (button.textContent.trim() === funiture.value) {
                button.classList.remove("btn-secondary");
                button.classList.add("btn-primary");
            } else {
                button.classList.remove("btn-primary");
                button.classList.add("btn-secondary");
            }
        });
    }

    buttons.forEach(function (button) {
        button.addEventListener("click", function () {
            hiddenInput.value = button.getAttribute("data-value");

            updateButtonStyles();
        });
    });

    updateButtonStyles();
});