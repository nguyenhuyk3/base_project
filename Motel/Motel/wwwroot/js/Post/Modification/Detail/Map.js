function initMap() {
    var latitude = parseFloat(document.getElementById('latitude').value);
    var longitude = parseFloat(document.getElementById('longitude').value);
    var location = { lat: latitude, lng: longitude };
    console.log(location)
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 20,
        center: location
    });
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });

    map.addListener('click', function () {
        var url = 'https://www.google.com/maps/search/?api=1&query=' + latitude + ',' + longitude;

        window.open(url, '_blank');
    });
}

