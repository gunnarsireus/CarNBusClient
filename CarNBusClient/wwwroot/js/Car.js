$(document).ready(function () {
    console.log('Car.js loaded ');
});

const oneSecond = 1000;
(function () {
    window.setTimeout(getCars, oneSecond);
})();

function convertSpeed(s) {
    return Math.round(s / 10) + "," + Math.round(s % 10);
}

function createTable(cars) {
    var table = document.getElementById("myTable");
    var rowCount = table.rows.length;
    for (var x = rowCount - 1; x > 0; x--) {
        table.deleteRow(x);
    }

    for (var i = 0; i < cars.length; i++) {
        let car = {
            regNr: cars[i].regNr,
            online: cars[i].online,
            speed: cars[i].speed
        };

        var row = table.insertRow(i + 1);
        var cell0 = row.insertCell(0);
        var cell1 = row.insertCell(1);
        var cell2 = row.insertCell(2);
        var cell3 = row.insertCell(3);
        var cell4 = row.insertCell(4);
        cell0.innerHTML = car.regNr;
        cell1.innerHTML = car.online;
        cell2.innerHTML = convertSpeed(car.speed);
        cell3.innerHTML = 0;
        cell4.innerHTML = 0;
    }
}


function getCars() {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState === XMLHttpRequest.DONE) {   // XMLHttpRequest.DONE == 4
            if (xmlhttp.status === 200) {
                let cars = JSON.parse(xmlhttp.responseText);
                createTable(cars);
                window.setTimeout(getCars, oneSecond);
            }
            else if (xmlhttp.status === 400) {
                alert('There was an error 400');
            }
            else {
                //alert('something else other than 200 was returned');
            }
        }
    };

    if (localStorage.getItem("apiAddress") !== null) {
        xmlhttp.open("GET", localStorage.getItem("apiAddress")  + "/api/read/car", true);
        xmlhttp.send();
    }
}
