document.addEventListener('DOMContentLoaded', function () {
    // Event listener for thumbnail clicks
    document.querySelectorAll('#thumbCarousel img').forEach(function (img, index) {
        img.addEventListener('click', function () {
            // Calculate the index of the clicked thumbnail
            var carouselItem = img.closest('.carousel-item');
            var carouselItems = Array.from(carouselItem.parentNode.children);
            var carouselItemIndex = carouselItems.indexOf(carouselItem);
            var imgIndex = Array.from(img.parentNode.parentNode.children).indexOf(img.parentNode);
            var index = carouselItemIndex * 4 + imgIndex;

            // Trigger the main carousel to go to the selected image
            $('#mainCarousel').carousel(index); // Using Bootstrap's jQuery plugin function
        });
    });
});