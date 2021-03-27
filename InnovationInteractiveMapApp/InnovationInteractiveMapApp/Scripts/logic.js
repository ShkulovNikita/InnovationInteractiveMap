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

function translateToRussian(phrase) {
    if (phrase == "total_patent_applications")
        return "Количество заявлений на патенты";
    if (phrase == "total_trademark_applications")
        return "Количество заявлений на торговые марки";
    if (phrase == "high_tech_exports")
        return "Экспорт высоких технологий (в % от производимого экспорта)";
    if (phrase == "high_tech_exports_usd")
        return "Экспорт высоких технологий (в долларах США)";
    if (phrase == "res_and_dev_expenditure")
        return "Затраты на НИОКР (в % от ВВП)";
    if (phrase == "payment_for_intel_property")
        return "Оплата интеллектуальной собственности (в долларах США)";
}

//возвращает значение, если оно есть, или N/A, если null
function CheckInfo(str) {
    if (str != null)
        return str;
    else return "N/A";
}

//обновление поля в зависимости от текущей страны
info.update = function (props) {
    if ((currentIndicator != "default") && (currentIndicator != "Выберите показатель")) {
        this._div.innerHTML =
            "<h4>Выбранная страна:</h4>" +
            (props
                ? "<b>" +
                props.ADMIN +
                "</b><br />" +
                translateToRussian(currentIndicator) + ": " + CheckInfo(props[currentIndicator])
                : "Наведите курсор на страну");
    } else {
        this._div.innerHTML =
            "<h4>Выбранная страна:</h4>" +
            (props
                ? "<b>" +
                props.ADMIN +
                "</b><br />" 
                : "Наведите курсор на страну");
    }
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
//минимальное значение показателя
var minValue = 0;
//текущий показатель
var currentIndicator = selectIndicator.value;
//легенда
var legend = L.control({ position: 'bottomright' });

//получение легенды
legend.onAdd = function (map) {
    var div = L.DomUtil.create("div", "info legend"),
        grades = [minValue, maxValue * 0.1, maxValue * 0.2, maxValue * 0.3, maxValue * 0.4, maxValue * 0.5, maxValue * 0.7, maxValue * 0.9, maxValue],
        labels = [];
    console.log("grades = " + grades);
    // loop through our density intervals and generate a label with a colored square for each interval
    for (var i = 0; i < grades.length; i++) {
        div.innerHTML +=
            '<i style="background:' +
            getColor(grades[i] + 1) +
            '"></i> ' +
            grades[i] +
            (grades[i + 1]
                ? "&ndash;" + grades[i + 1] + "<br>"
                : "+");
    }

    return div;
};

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

    legend.addTo(mymap);
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
    if ((currentIndicator != "default") && (currentIndicator != "Выберите показатель")) {
        var val;

        bounds.features.forEach(function (item, i, arr) {
            if (item.properties.ADMIN == selectCountry.value) {
                for ([key, value] of Object.entries(item.properties)) {
                    if (key == currentIndicator) {
                        val = value;
                    } else {
                    }
                }
            }
        });

        $('#chosen-country').html(selectCountry.value + ": ");
        $('#chosen-value').html(val);
    }




    getIndicatorsArray();
});

getIndicatorsArray();

function getHighCountries() {
    array1 = [];

    var t = -1;
    var idplace = 100;
    var str = "Russia";
    bounds.features.forEach(function (item, i, arr) {
        t = t + 1;

        var name = item.properties.ADMIN;
        var indicator = "";

        for ([key, value] of Object.entries(item.properties)) {
            if (key == currentIndicator) {
                indicator = value;
                array1.push({ name: name, value: indicator });
            } else {
            }
        }
    });

    array1.sort(function (a, b) {
        return a.value - b.value;
    });
    var str = "Russia";

    //найти Россию
    for (i = 0; i < array1.length; i++) {
        var countryName = array1[i].name;

        if (countryName == str) {
            idplace = i;
            break;
        }
    }

    var array3 = array1.slice(array1.length - 3, array1.length);

    //удалить старые строки
    $('.top-row').remove();

    if ((translateIndicator(currentIndicator) == "default") || (currentIndicator == "default")) {
        $("#topCountries").append(
            `<tr class='top-row'><td>n</td><td>-----</td><td>-----</td><td>-----</td></tr>`
        );
        for (i = 0; i < 3; i++) {
            $("#topCountries").append(
                `<tr class='top-row'><td>${i+1}</td><td>-----</td><td>-----</td><td>-----</td></tr>`
            );
        }
    } else {
        $("#topCountries").append(
            `<tr class='top-row'><td><b>${array1.length - idplace}</b></td><td><b>${array1[idplace].name}</b></td><td><b>${array1[idplace].value}</b></td><td><b>0</b></td></tr>`
        );
        for (i = 2; i > -1; i--) {
            $("#topCountries").append(
                `<tr class='top-row'><td>${3 - i}</td><td>${array3[i].name}</td><td>${array3[i].value}</td><td>${getArray(array1[idplace].value, array3[i].value).toFixed(3)}</td></tr>`
            );
        }
    }

    getCloseCountries(idplace);
}

function getArray(value, valueT) {
    return (valueT - value) / value * 100;
}

function getCloseCountries(id) {
    //удалить старые строки
    $('.close-row').remove();

    if ((translateIndicator(currentIndicator) == "default") || (currentIndicator == "default")) {
        for (i = 0; i < 3; i++) {
            $("#closeCountries1").append(
                `<tr class='close-row'><td>-----</td><td>-----</td><td>-----</td></tr>`
            );
        }
    } else {
        theNearCountries = [];

        //валидация
        if (id != (array1.length - 1)) {
            theNearCountries.push({
                name: array1[id - 1].name,
                value: array1[id - 1].value,
                perc: getArray(array1[id].value, array1[id - 1].value),
            });
            theNearCountries.push({
                name: array1[id].name,
                value: array1[id].value,
                perc: getArray(array1[id].value, array1[id].value),
            });
            theNearCountries.push({
                name: array1[id + 1].name,
                value: array1[id + 1].value,
                perc: getArray(array1[id].value, array1[id + 1].value),
            });
        } else {
            theNearCountries.push({
                name: array1[id - 2].name,
                value: array1[id - 2].value,
                perc: getArray(array1[id].value, array1[id - 2].value),
            });
            theNearCountries.push({
                name: array1[id - 1].name,
                value: array1[id - 1].value,
                perc: getArray(array1[id].value, array1[id - 1].value),
            });
            theNearCountries.push({
                name: array1[id].name,
                value: array1[id].value,
                perc: getArray(array1[id].value, array1[id].value),
            });
        }

        theNearCountries.forEach(function (item, i, arr) {
            if (item.name == "Russia") {
                $("#closeCountries1").append(
                    `<tr class='close-row'><td><b>${item.name}</b></td><td><b>${item.value}</b></td><td><b>${item.perc}</b></td></tr>`
                );
            } else {
                $("#closeCountries1").append(
                    `<tr class='close-row'><td>${item.name}</td><td>${item.value}</td><td>${item.perc.toFixed(3)}</td></tr>`
                );
            }
        });
    }
}