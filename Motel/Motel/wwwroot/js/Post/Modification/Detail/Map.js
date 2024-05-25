function initMap() {
    //var latitude = @ViewBag.Latitude;
    //var longitude = @ViewBag.Longitude;
    //console.log(latitude, longitude)
    var location = { lat: 10.7765436, lng: 106.7050533 };
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

