var mymap = L.map("mapid").setView([51.505, -0.09], 13);
var geojson;
//max Indicator Value
var maxValue = 0;
var selectCountry = document.querySelector("#country");
var selectIndicator = document.querySelector("#indicators");
var indicator = $("#indicator option:selected").text();
console.log(bounds.features[0]);
// Получаем значения
selectCountry.addEventListener("change", function () {
    console.log($("#country").val());

    getIndicatorsArray();
});
selectIndicator.addEventListener("change", function () {
    mymap.invalidateSize(true);
    getIndicatorsArray();
    geojson = new L.geoJson(bounds, {
        style: style,
        onEachFeature: onEachFeature,
    }).addTo(mymap);
});
L.tileLayer(
    "https://api.mapbox.com/styles/v1/{id}/tiles/{z}/{x}/{y}?access_token={accessToken}",
    {
        attribution:
            'Map data &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, Imagery © <a href="https://www.mapbox.com/">Mapbox</a>',
        maxZoom: 18,
        id: "mapbox/streets-v11",
        tileSize: 512,
        zoomOffset: -1,
        accessToken:
            "pk.eyJ1IjoicnVzbGFua2FyIiwiYSI6ImNra2djcTE2MTE0OGsycHF1ZHpicnN3ZTgifQ.Z_cXtfVb5IRYhzj8ZPqiJg",
    }
).addTo(mymap);
function getColor(d) {
    return d === maxValue
        ? "#800026"
        : d > maxValue * 0.9
            ? "#BD0026"
            : d == null
                ? "#343434"
                : d > maxValue * 0.7
                    ? "#E31A1C"
                    : d > maxValue * 0.6
                        ? "#FC4E2A"
                        : d > maxValue * 0.5
                            ? "#FD8D3C"
                            : d > maxValue * 0.4
                                ? "#FEB24C"
                                : d > maxValue * 0.1
                                    ? "#FED976"
                                    : "#FFEDA0";
}
function style(feature) {
    return {
        fillColor: getColor(feature.properties["high_tech_exports"]),
        weight: 2,
        opacity: 1,
        color: "white",
        dashArray: "3",
        fillOpacity: 0.7,
    };
}

function highlightFeature(e) {
    var layer = e.target;

    layer.setStyle({
        weight: 5,
        color: "#666",
        dashArray: "",
        fillOpacity: 0.7,
    });

    if (!L.Browser.ie && !L.Browser.opera && !L.Browser.edge) {
        layer.bringToFront();
    }
    info.update(layer.feature.properties);
}

function resetHighlight(e) {
    geojson.resetStyle(e.target);
    info.update();
}

function zoomToFeature(e) {
    mymap.fitBounds(e.target.getBounds());
}

function onEachFeature(feature, layer) {
    layer.on({
        mouseover: highlightFeature,
        mouseout: resetHighlight,
        click: zoomToFeature,
    });
}


var info = L.control();

info.onAdd = function (mymap) {
    this._div = L.DomUtil.create("div", "info"); // create a div with a class "info"
    this.update();
    return this._div;
};

// method that we will use to update the control based on feature properties passed
info.update = function (props) {
    this._div.innerHTML =
        "<h4>US Population Density</h4>" +
        (props
            ? "<b>" +
            props.name +
            "</b><br />" +
            props.density +
            " people / mi<sup>2</sup>"
            : "Hover over a state");
};

info.addTo(mymap);

function arrayMin(arr) {
    var len = arr.length,
        min = Infinity;
    while (len--) {
        if (arr[len] != null) {
            if (arr[len] < min) {
                min = arr[len];
            }
        }
    }
    return min;
}

function arrayMax(arr) {
    var len = arr.length,
        max = -Infinity;
    while (len--) {
        if (arr[len] > max) {
            max = arr[len];
        }
    }
    return max;
}

geojson = new L.geoJson(bounds, {
    style: style,
    onEachFeature: onEachFeature,
}).addTo(mymap);

function getIndicatorsArray() {
    var array = [];
    var minValue = 0;

    const data = bounds.features.map((feature) => {
        var value = feature.properties["high_tech_exports"];
        array.push(value);
        return value;
    });
    // Максимальное и минимальное значения для нахождения пропорции по цветам
    maxValue = arrayMax(array);

    minValue = arrayMin(array);

}

function getHighCountries() { }
getIndicatorsArray();