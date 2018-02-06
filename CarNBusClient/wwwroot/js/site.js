// Write your JavaScript code.
$(document).ready(function () {
    timerJob();
    timerJob2();
    console.log('documentReady');
});

$(".uppercase").keyup(function () {
    var text = $(this).val();
    $(this).val(text.toUpperCase());
});0

function clearErrors() {
    $(".validation-summary-errors").empty();
};

let longInterval = 10000;
let longInterval2 = 10;
let numberOfCars = 7;
const oneSecond = 1000;

function convertSpeed(s) {
    return Math.round(s / 10) + "," + Math.round(s % 10);
}


function timerJob() {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState === XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4
            if (xmlhttp.status === 200) {
                let cars = JSON.parse(xmlhttp.responseText);
                updateOnline(cars);
            }
            else if (xmlhttp.status === 400) {
                alert('There was an error 400');
            }
            else {
                //alert('something else other than 200 was returned: ' + xmlhttp.status);
            }
        }
    };

    xmlhttp.open("GET", "http://localhost:63484/api/car", true);
    xmlhttp.send();
}

function timerJob2() {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState === XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4
            if (xmlhttp.status === 200) {
                let cars = JSON.parse(xmlhttp.responseText);
                updateSpeed(cars);
            }
            else if (xmlhttp.status === 400) {
                alert('There was an error 400');
            }
            else {
                //alert('something else other than 200 was returned: ' + xmlhttp.status);
            }
        }
    };

    xmlhttp.open("GET", "http://localhost:63484/api/car", true);
    xmlhttp.send();
}

function updateOnline(cars) {
    if (cars.length === 0) {
        setTimeout(timerJob, oneSecond);
        console.log("No vehicles found!");
        return;
    }
    numberOfCars = cars.length;
    const selectedItem = Math.floor(Math.random() * cars.length);
    let selectedCar = cars[selectedItem];
    if (selectedCar.locked === true) {
        console.log(selectedCar.regNr + " is Locked for uppdating of Online/Offline!");
        longInterval = Math.round(longInterval / numberOfCars);
        setTimeout(timerJob, longInterval);
        return;
    }
    selectedCar.online = !selectedCar.online;
    $.ajax({
        url: 'http://localhost:63484/api/car/' + selectedCar.carId,
        contentType: "application/json",
        type: "PUT",
        data: JSON.stringify(selectedCar),
        dataType: "json"
    });

    const onlineSelector = `#${selectedCar.carId} td:eq(3)`;
    const onlineSelector2 = `#${selectedCar.carId + "_2"} td:eq(4)`;
    const onlineSelector3 = `#${selectedCar.carId + "_3"}`;
    if (selectedCar.online === true) {
        $(onlineSelector).text("Online");
        $(onlineSelector).removeClass("alert-danger");
        $(onlineSelector2).text("Online");
        $(onlineSelector2).removeClass("alert-danger");
        $(onlineSelector3).text("Online");
        $(onlineSelector3).removeClass("alert-danger");
        console.log(selectedCar.regNr + " is Online!");
    }
    else {
        $(onlineSelector).text("Offline");
        $(onlineSelector).addClass("alert-danger");
        $(onlineSelector2).text("Offline");
        $(onlineSelector2).addClass("alert-danger");
        $(onlineSelector3).text("Offline");
        $(onlineSelector3).addClass("alert-danger");
        console.log(selectedCar.regNr + " is Offline!");
    }

    if (document.getElementById("All") !== null) {
        doFiltering();
    }
    let interval = Math.round(longInterval / numberOfCars);
    setTimeout(timerJob, interval);
}

function updateSpeed(cars) {
    if (cars.length === 0) {
        setTimeout(timerJob2, oneSecond);
        console.log("No vehicles found!");
        return;
    }
    numberOfCars = cars.length;
    const selectedItem = Math.floor(Math.random() * cars.length);
    let selectedCar = cars[selectedItem];
    if (selectedCar.locked === true) {
        console.log(selectedCar.regNr + " is Locked for uppdating of Online/Offline!");
        let interval = Math.round(longInterval2 / numberOfCars);
        setTimeout(timerJob2, interval);
        return;
    }
    const delta = Math.round(selectedCar.speed / 10);
    selectedCar.speed = selectedCar.speed + delta / 2 - Math.floor(Math.random() * delta);
    $.ajax({
        url: 'http://localhost:63484/api/car/' + selectedCar.carId,
        contentType: "application/json",
        type: "PUT",
        data: JSON.stringify(selectedCar),
        dataType: "json"
    });

    const speedSelector = `#${selectedCar.carId} td:eq(2)`;
    const speedSelector2 = `#${selectedCar.carId + "_2"} td:eq(2)`;
    const speedSelector3 = `#${selectedCar.carId + "_4"}`;
    if (selectedCar.online === true) {
        $(speedSelector).text(convertSpeed(selectedCar.speed));
        $(speedSelector2).text(convertSpeed(selectedCar.speed));
        $(speedSelector3).text(convertSpeed(selectedCar.speed));
    }
    else {
        $(speedSelector).text(convertSpeed(selectedCar.speed));
        $(speedSelector2).text(convertSpeed(selectedCar.speed));
        $(speedSelector3).text(convertSpeed(selectedCar.speed));
    }

    longInterval2 = Math.round(longInterval2 / numberOfCars);
    setTimeout(timerJob2, longInterval2);
}

function doFiltering() {
    let selection = 0;
    let radiobtn = document.getElementById("All");
    if (radiobtn.checked === false) {
        radiobtn = document.getElementById("Online");
        if (radiobtn.checked === true) {
            selection = 1;
        }
        else {
            selection = 2;
        }
    }

    var table = $('#cars > tbody');
    $('tr', table).each(function () {
        $(this).removeClass("hidden");
        let td = $('td:eq(3)', $(this)).html();
        if (td !== undefined) {
            td = td.trim();
        }
        if (td === "Offline" && selection === 1) {
            $(this).addClass("hidden");  //Show only Online
        }
        if (td === "Online" && selection === 2) {
            $(this).addClass("hidden"); //Show only Offline
        }
    });
};

function showModals() {
    window.open("./html/Car.html", "_blank", "toolbar=yes,scrollbars=yes,resizable=yes,top=50,left=50,width=500,height=400");
}
