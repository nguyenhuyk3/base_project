var boxCss = "none";

document.querySelector(".ikrNotifications").addEventListener("click", () => {
    if (boxCss == "none") {
        boxCss = "block";
    } else {
        boxCss = "none";
    }

    notificationsElement.style.setProperty('display', boxCss, '!important');

})