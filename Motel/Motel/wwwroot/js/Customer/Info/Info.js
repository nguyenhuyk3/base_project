const tinBan = document.querySelector('.sale');
const tinChoThue = document.querySelector('.rental');
const noiDung = document.querySelector('.content');
const sale1 = noiDung.querySelector('.sale-post');
const rental1 = noiDung.querySelector('.rental-post');

tinBan.addEventListener('click', function () {
    sale1.style.display = 'block';
    rental1.style.display = 'none';
});

tinChoThue.addEventListener('click', function () {
    sale1.style.display = 'none';
    rental1.style.display = 'block';
});

