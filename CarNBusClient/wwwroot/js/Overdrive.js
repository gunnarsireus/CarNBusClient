// Write your JavaScript code.
$(document).ready(function () {
    updateOnlineTimer();
    updateSpeedTimer();
    console.log('documentReady Overdrive');
});


let overdriveInterval = 10;
let overdriveInterval2 = 5;
let numberOfCars = 7;
const oneSecond = 1000;

function convertSpeed(s) {
    return Math.round(s / 10) + "," + Math.round(s % 10);
}


function updateOnlineTimer() {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState === XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4
            if (xmlhttp.status === 200) {
                let cars = JSON.parse(xmlhttp.responseText);
                updateOnlineOverdrive(cars);
            }
            else if (xmlhttp.status === 400) {
                alert('There was an error 400');
            }
            else {
                //alert('something else other than 200 was returned: ' + xmlhttp.status);
            }
        }
    };

    xmlhttp.open("GET", "http://localhost:63484/api/read/car", true);
    xmlhttp.send();
}

function updateSpeedTimer() {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState === XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4
            if (xmlhttp.status === 200) {
                let cars = JSON.parse(xmlhttp.responseText);
                updateSpeedOverdrive(cars);
            }
            else if (xmlhttp.status === 400) {
                alert('There was an error 400');
            }
            else {
                //alert('something else other than 200 was returned: ' + xmlhttp.status);
            }
        }
    };

    xmlhttp.open("GET", "http://localhost:63484/api/read/car", true);
    xmlhttp.send();
}

function updateOnlineOverdrive(cars) {
    if (cars.length === 0) {
        setTimeout(updateOnlineTimer, oneSecond);
        console.log("No vehicles found!");
        return;
    }
    numberOfCars = cars.length;
    const selectedItem = Math.floor(Math.random() * cars.length);
    let selectedCar = cars[selectedItem];
    if (selectedCar.locked === true) {
        console.log(selectedCar.regNr + " is Locked for uppdating of Online/Offline!");
        overdriveInterval = Math.round(overdriveInterval / numberOfCars);
        setTimeout(updateOnlineTimer, overdriveInterval);
        return;
    }
    selectedCar.online = !selectedCar.online;
    $.ajax({
        url: 'http://localhost:63484/api/write/car/online/' + selectedCar.carId,
        contentType: "application/json",
        type: "PUT",
        data: JSON.stringify(selectedCar),
        dataType: "json"
    });

    let interval = Math.round(overdriveInterval / numberOfCars);
    setTimeout(updateOnlineTimer, interval);
}

function updateSpeedOverdrive(cars) {
    if (cars.length === 0) {
        setTimeout(updateSpeedTimer, oneSecond);
        console.log("No vehicles found!");
        return;
    }
    numberOfCars = cars.length;
    const selectedItem = Math.floor(Math.random() * cars.length);
    let selectedCar = cars[selectedItem];
    if (selectedCar.locked === true) {
        console.log(selectedCar.regNr + " is Locked for uppdating of Online/Offline!");
        let interval = Math.round(overdriveInterval2 / numberOfCars);
        setTimeout(updateSpeedTimer, interval);
        return;
    }
    const delta = selectedCar.speed / 10;
    selectedCar.speed = selectedCar.speed + Math.round(delta / 2 - Math.floor(Math.random() * delta));
    $.ajax({
        url: 'http://localhost:63484/api/write/car/speed/' + selectedCar.carId,
        contentType: "application/json",
        type: "PUT",
        data: JSON.stringify(selectedCar),
        dataType: "json"
    });

    overdriveInterval2 = Math.round(overdriveInterval2 / numberOfCars);
    setTimeout(updateSpeedTimer, overdriveInterval2);
}

