// Toast function
function toastNotification({ title = "", message = "", type = "info", duration = 3000 }) {
    const main = document.getElementById("toast-notification");

    if (main) {
        const toast = document.createElement("div");

        // Auto remove toast
        const autoRemoveId = setTimeout(function () {
            main.removeChild(toast);
        }, duration + 1000);

        // Remove toast when clicked
        toast.onclick = function (e) {
            if (e.target.closest(".toast-notification__close")) {
                main.removeChild(toast);
                clearTimeout(autoRemoveId);
            }
        };

        const icons = {
            success: "fas fa-check-circle",
            info: "fas fa-info-circle",
            warning: "fas fa-exclamation-circle",
            error: "fas fa-exclamation-circle"
        };
        const icon = icons[type];
        const delay = (duration / 1000).toFixed(2);

        toast.classList.add("toast-notification", `toast-notification--${type}`);
        toast.style.animation = `slideInLeft ease .3s, fadeOut linear 1s ${delay}s forwards`;

        toast.innerHTML = `
                    <div class="toast-notification__icon">
                        <i class="${icon}"></i>
                    </div>
                    <div class="toast-notification__body">
                        <h3 class="toast-notification__title">${title}</h3>
                        <p class="toast-notification__msg">${message}</p>
                    </div>
                    <div class="toast-notification__close">
                        <i class="fas fa-times"></i>
                    </div>
                `;

        main.appendChild(toast);
    }
}

window.toastNotification = toastNotification;


