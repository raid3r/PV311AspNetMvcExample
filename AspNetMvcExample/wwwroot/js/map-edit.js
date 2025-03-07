let marker
let map

function onMapClick(e) {
    //alert("You clicked the map at " + e.latlng);
    document.querySelector("[name=Lat]").value = e.latlng.lat
    document.querySelector("[name=Lng]").value = e.latlng.lng
    if (marker) {
        map.removeLayer(marker)
    }
    marker = L.marker(e.latlng).addTo(map)
}

function initMap() {

    const lat = document.querySelector("[name=Lat]").value
    const lng = document.querySelector("[name=Lng]").value

    map = L.map('map')

    if (lat && lng) {
        map.setView([lat, lng], 12)
        marker = L.marker([lat, lng]).addTo(map)
    } else {
        map.setView([50.4501, 30.5234], 12)
    }
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
    }).addTo(map)
    map.on('click', onMapClick);

}

initMap()
