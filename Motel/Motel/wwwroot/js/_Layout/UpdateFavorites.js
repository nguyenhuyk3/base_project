// Update favorites in heart shape
function updateFavorites() {
    var favoriteItems = document.querySelectorAll('.dropdown-menu-favorites .favorite-item');

    if (favoriteItems.length === 0) {
        document.querySelector('.dropdown-menu-favorites .no-favorites').classList.remove('d-none');
        document.querySelector('.dropdown-menu-favorites .dropdown-divider').style.display = 'none';
        document.querySelector('.dropdown-menu-favorites .dropdown-item.text-center').style.display = 'none';
    } else {
        document.querySelector('.dropdown-menu-favorites .no-favorites').classList.add('d-none');
        document.querySelector('.dropdown-menu-favorites .dropdown-divider').style.display = 'block';
        document.querySelector('.dropdown-menu-favorites .dropdown-item.text-center').style.display = 'block';
    }
}

document.addEventListener("DOMContentLoaded", function () {
    updateFavorites();
});

