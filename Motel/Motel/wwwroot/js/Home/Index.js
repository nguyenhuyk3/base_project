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

document.querySelector('.ikrNoti_Button').addEventListener("click", toggleNotifications);
document.querySelector('.ikrNotifications').addEventListener("click", toggleNotifications);