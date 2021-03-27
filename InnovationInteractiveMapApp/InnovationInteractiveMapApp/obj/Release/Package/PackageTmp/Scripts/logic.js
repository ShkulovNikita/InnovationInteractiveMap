//отображение карты
var mymap = L.map("mapid").setView([51.505, -0.09], 5);

var geojson;

//добавление границ
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

//первичное создание карты
geojson = new L.geoJson(bounds, {
    style: initialStyle,
    onEachFeature: onEachFeature,
}).addTo(mymap);

//оформление при первичном отображении карты
function initialStyle(feature) {
    return {
        fillColor: 'gray',
        weight: 2,
        opacity: 1,
        color: "white",
        dashArray: "3",
        fillOpacity: 0.7,
    };
}

//функция подбора цвета в зависимости от относительного значения показателя
function getColor(d) {
    return d === maxValue ?     "#800026" :
           d > maxValue * 0.9 ? "#BD0026" :
           d == null ?          "#343434" :
           d > maxValue * 0.7 ? "#E31A1C" :
           d > maxValue * 0.6 ? "#FC4E2A" :
           d > maxValue * 0.5 ? "#FD8D3C" :
           d > maxValue * 0.4 ? "#FEB24C" :
           d > maxValue * 0.1 ? "#FED976" :
                                "#FFEDA0";
}

//смена цвета при наведении
function highlightFeature(e) {
    //получить наведенную область
    var layer = e.target;

    //установить соответствующий стиль
    layer.setStyle({
        weight: 5,
        color: "#666",
        dashArray: "",
        fillOpacity: 0.7,
    });

    //поддержка браузеров
    if (!L.Browser.ie && !L.Browser.opera && !L.Browser.edge) {
        layer.bringToFront();
    }

    info.update(layer.feature.properties);
}

//убрать эффект наведения
function resetHighlight(e) {

    //geojson.resetStyle(e.target);
    if ((translateIndicator(currentIndicator) == "default") || (currentIndicator == "default")) {
        geojson.setStyle(initialStyle);
    }
    else {
        //получить массив значений показателя
        getIndicatorsArray(currentIndicator);
        geojson.setStyle(style)
    }

    info.update();
}

//приближение при клике
function zoomToFeature(e) {
    mymap.fitBounds(e.target.getBounds());
}

//добавить на все страны возможность наведения и клика
function onEachFeature(feature, layer) {
    layer.on({
        mouseover: highlightFeature,
        mouseout: resetHighlight,
        click: zoomToFeature,
    });
}

//переменная для поля с информацией о стране
var info = L.control();

//создание поля
info.onAdd = function (mymap) {
    this._div = L.DomUtil.create("div", "info");
    this.update();
    return this._div;
};

//обновление поля в зависимости от текущей страны
info.update = function (props) {
    this._div.innerHTML =
        "<h4>Выбранная страна:</h4>" +
        (props
            ? "<b>" +
            props.ADMIN +
            "</b><br />"
            : "Наведите курсор на страну");
};

//добавить поле к карте
info.addTo(mymap);

//найти минимальное значение в массиве
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

//найти максимальное значение в массиве
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

//список стран
var selectCountry = document.querySelector("#country");
//список показателей
var selectIndicator = document.querySelector("#indicators");
//максимальное значение показателя
var maxValue = 0;
//текущий показатель
var currentIndicator = selectIndicator.value;

//изменение показателя
selectIndicator.addEventListener("change", (event) => {
    mymap.invalidateSize(true);

    //получить текущий индикатор
    currentIndicator = selectIndicator.value;

    //перевести значение на английский
    currentIndicator = translateIndicator(currentIndicator);

    if (currentIndicator == "default") {
        geojson.setStyle(initialStyle);
    }
    else {
        //получить массив значений показателя
        getIndicatorsArray(currentIndicator);
        geojson.setStyle(style)
    }

    getHighCountries();
});

//получение показателя по его русскому названию
function translateIndicator(indicator) {
    if (indicator == "Выберите показатель")
        return "default";
    if (indicator == "Количество заявлений на патенты")
        return "total_patent_applications";
    if (indicator == "Количество заявлений на торговые марки")
        return "total_trademark_applications";
    if (indicator == "Экспорт высоких технологий (в % от производимого экспорта)")
        return "high_tech_exports";
    if (indicator == "Экспорт высоких технологий (в долларах США)")
        return "high_tech_exports_usd";
    if (indicator == "Затраты на НИОКР (в % от ВВП)")
        return "res_and_dev_expenditure";
    if (indicator == "Оплата интеллектуальной собственности (в долларах США)")
        return "payment_for_intel_property";
}

//обновление минимального и максимального значений по выбранному показателю
function getIndicatorsArray(indicator) {
    var array = [];

    const data = bounds.features.map((feature) => {
        var value = feature.properties[indicator];
        array.push(value);
        return value;
    });
    // Максимальное и минимальное значения для нахождения пропорции по цветам
    maxValue = arrayMax(array);

    minValue = arrayMin(array);
}

//получение цвета
function style(feature) {
    return {
        fillColor: getColor(feature.properties[currentIndicator]),
        weight: 2,
        opacity: 1,
        color: "white",
        dashArray: "3",
        fillOpacity: 0.7,
    };
}

//выбрана страна
selectCountry.addEventListener("change", function () {
    console.log($("#country").val());

    getIndicatorsArray();
});

getIndicatorsArray();

function getHighCountries() {
    array1 = [];

    var t = -1;
    var idplace = 0;
    var str = "Russia";
    bounds.features.forEach(function (item, i, arr) {
        // var nearIndicators = [];

        t = t + 1;

        var name = item.properties.ADMIN;
        var indicator = "";

        for ([key, value] of Object.entries(item.properties)) {
            // t = t + 1; ptnkl tp tkn
            if (value == "total_trademark_applications") {
                indicator = value;
                //countries.push(value);
                array1.push({ name: name, value: indicator });
            } else {
            }
            // if (name == "Russia") {
            //   idplace = t;
            // }
        }
        console.log(name);
        if (name == "Russia") {
            idplace = t;
        }
    });

    array1.sort(function (a, b) {
        return a.value - b.value;
    });
    console.log(array1);
    var str = "Russia";

    var array3 = array1.slice(array1.length - 3, array1.length);
    array3.forEach(function (item, i, arr) {
        $("#topCountries").append(
            `<tr><td>${item.name}</td><td>${item.value}</td></tr>`
        );
    });
    idplace = 100;
    console.log(Number(idplace));

    console.log(array3);
}
