$(document).ready(function () {
    $('.thumbnail-gallery').slick({
        slidesToShow: 6,
        slidesToScroll: 6,
        arrows: true,
        infinite: false,
        focusOnSelect: true
    });

    $('.thumbnail-gallery img').on('click', function () {
        var slideIndex = $(this).attr('data-slide-to');
        $('#mainCarousel').carousel(parseInt(slideIndex));
    });
});