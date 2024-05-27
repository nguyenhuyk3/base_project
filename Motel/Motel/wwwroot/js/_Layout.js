var username = document.getElementById('username');
var userMenu = document.getElementById('user-menu');
var bin = document.getElementById('bin');

// Biến flag để kiểm tra tương tác với user-menu
var isUserMenuHovered = false;

// Khi di chuột vào username hoặc user-menu
username.addEventListener('mouseenter', function (event) {
    userMenu.style.visibility = 'visible';
    isUserMenuHovered = true;
});

userMenu.addEventListener('mouseenter', function (event) {
    userMenu.style.visibility = 'visible';
    isUserMenuHovered = true;
});

// Khi di chuột rời khỏi username và user-menu
username.addEventListener('mouseleave', function (event) {
    if (!isUserMenuHovered) {
        userMenu.style.visibility = 'hidden';
    }
    isUserMenuHovered = false;
});

userMenu.addEventListener('mouseleave', function (event) {
    if (!isUserMenuHovered) {
        userMenu.style.visibility = 'hidden';
    }
    isUserMenuHovered = false;
});

// Khi di chuột vào div cha
bin.addEventListener('mouseenter', function () {
    if (!isUserMenuHovered) {
        userMenu.style.visibility = 'hidden';
    }
    isUserMenuHovered = false;
});