var boxCss = "none";

function toggleNotifications() {
    if (boxCss === "none") {
        boxCss = "block";
    } else {
        boxCss = "none";
    }

    var notificationsElement = document.querySelector(".ikrNotifications");

    notificationsElement.style.display = boxCss;
}

if (document.querySelector('.ikrNoti_Button') != null) {
    document.querySelector('.ikrNoti_Button').addEventListener("click", toggleNotifications);
}

if (document.querySelector('.ikrNotifications') != null) {
    document.querySelector('.ikrNotifications').addEventListener("click", toggleNotifications);
}