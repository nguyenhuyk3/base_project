// Update notification in  shape
function updateNotifications() {
    var notificationItems = document.querySelectorAll('.dropdown-menu-notifications .notification-item');

    if (notificationItems.length === 0) {
        document.querySelector('.dropdown-menu-notifications .no-notifications').classList.remove('d-none');
        document.querySelector('.dropdown-menu-notifications .dropdown-divider').style.display = 'none';
        document.querySelector('.dropdown-menu-notifications .dropdown-item.text-center').style.display = 'none';
    } else {
        document.querySelector('.dropdown-menu-notifications .no-notifications').classList.add('d-none');
        document.querySelector('.dropdown-menu-notifications .dropdown-divider').style.display = 'block';
        document.querySelector('.dropdown-menu-notifications .dropdown-item.text-center').style.display = 'block';
    }
}

document.addEventListener("DOMContentLoaded", function () {
    updateNotifications();
});