var carousel = document.getElementById('carouselExampleControls');

var carouselInterval = setInterval(function () {
    var currentIndex = $('.carousel-item.active').index();
    var totalItems = $('.carousel-item').length;
    var nextIndex = (currentIndex + 1) % totalItems;
    $('#carouselExampleControls').carousel(nextIndex);
}, 5000);

carousel.addEventListener('mouseover', function () {
    clearInterval(carouselInterval);
});

carousel.addEventListener('mouseout', function () {
    carouselInterval = setInterval(function () {
        var currentIndex = $('.carousel-item.active').index();
        var totalItems = $('.carousel-item').length;
        var nextIndex = (currentIndex + 1) % totalItems;
        $('#carouselExampleControls').carousel(nextIndex);
    }, 5000);
});