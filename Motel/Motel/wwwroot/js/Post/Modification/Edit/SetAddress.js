document.addEventListener("DOMContentLoaded", function () {
    var city = document.getElementById("ApiId").options[document.getElementById("ApiId").selectedIndex].text;
    var addressElement = document.getElementById("Address");

    addressElement.value = city + ",";
});
