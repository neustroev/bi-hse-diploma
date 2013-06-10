(function () {
    DataMartEnum = {};
    DataMartEnum.Action = {
        Transactions: "GetTransactions",
        Amounts: "GetAmounts",
        Aggregations: "GetAggregations",
        Ratings: "GetRatings"
    }
    DataMartEnum.TableSettings = {
        Class: "well table table-hover table-condensed"
    }
})();

DataMart = function (settings) {
    this.navigationPanel = settings.navigationPanel;
    this.viewContainers = settings.viewContainers;
    this.viewControls = settings.viewControls;
    this.updateDataControl = settings.updateControl;
    this.loader = settings.loader;

    this._init();
};

var dmProto = DataMart.prototype;

dmProto._init = function () {
    this.applyEvents();
    this.showViewContent(this.viewContainers.first());
};

dmProto.applyEvents = function () {
    var self = this;
    self.navigationPanel.find("li").on("click", function (ev) {
        ev.preventDefault();
        var item = this;
        self._onNavigationPanelItemClick(item);
    });
    self.navigationPanel.affix();

    self.viewContainers.on("show", function (ev) {
        var item = this;
        self._onviewContainershow(item);
    });
    self.viewControls.on("click", function (ev) {
        var item = this;
        self._onViewControlClick(item);
    });
    self.updateDataControl.on("click", function (ev) {
        ev.preventDefault();
        self.loader.show();
        $.post("/DataMart/UpdateData", self._onDataUpdated.bind(self));
    });

};

dmProto.showViewContent = function (element) {
    element.addClass("active");   
    if (!element.attr("dataLoaded")) {
        this.loader.show();
        element.find(".chartView").empty();
        element.find(".tableView").empty();
        var action = element.data("action");
        $.getJSON("/DataMart/" + action, null, function (data) {
            this._onDataLoaded(element, action, data);
            element.attr("dataLoaded", true);
            this.loader.hide();
        }.bind(this));
    };
};

dmProto._onNavigationPanelItemClick = function (item) {
    var jqItem = $(item);
    this.navigationPanel.find("li").removeClass("active");
    jqItem.addClass("active");

    var viewContainerselector = jqItem.children("a").attr("href");
    this.viewContainers.removeClass("active");

    this.viewContainers.filter(viewContainerselector).trigger("show");
};

dmProto._onviewContainershow = function (item) {
    var jqItem = $(item);
    this.showViewContent(jqItem);
};

dmProto._onViewControlClick = function (item) {
    var jqItem = $(item);
    jqItem.parent().find("button").removeClass("active");
    jqItem.addClass("active");
    this.viewContainers.filter(".active").find(".contentView > div").removeClass("active")
    var target = jqItem.data("target");
    this.viewContainers.filter(".active").find(target).addClass("active");
};

dmProto._onDataLoaded = function (container, action, data) {
    switch (action) {
        case DataMartEnum.Action.Transactions:
            this.createTransactionsData(container, data);
            break;
        case DataMartEnum.Action.Amounts:
            this.createAmountsData(container, data);
            break;
        case DataMartEnum.Action.Aggregations:
            this.createAggregationsData(container, data);
            break;
        case DataMartEnum.Action.Ratings:
            this.createRatingsData(container, data);
        default:
            break;
    };
};

dmProto._onDataUpdated = function (data) {
    this.loader.hide();
    if (data.Success) {
        var currentElement = $("[dataLoaded].active");
        $("[dataLoaded]").removeAttr("dataloaded");
        this.showViewContent(currentElement);
    }
    else {
        alert("Ошибка обновления данных");
    }
};

dmProto.createTransactionsData = function (container, data) {
    var categories = [],
        series = [{
            name: "Прибыль",
            data: []
        }, {
            name: "Покупки",
            data: []
        }];
    var table = $("<table />", {
        "class": DataMartEnum.TableSettings.Class
    });    
    table.append("<thead><tr><th>Дата</th><th>Прибыль</th><th>Покупки</th></tr></thead>")
    $.map(data, function (item) {
        var date = new Date(parseInt(item.Date.replace(/\D/g, ''), 10)).toLocaleDateString(),
            purchase = item.PurchaseCount,
            profit = parseFloat(item.Profit.toFixed(2));
        categories.push(date);
        series[0].data.push(profit);
        series[1].data.push(purchase);
        table.append("<tr><td>" + date + "</td><td>" + profit + "</td><td>" + purchase + "</td></tr>");
    });
    container.find(".tableView").append("<h2>Ежедневные транзакции</h2>").append(table);
    container.find(".chartView").highcharts({
        chart: {
            type: 'line',
            zoomType: 'x',
        },
        title: {
            text: 'Показатели транзакций',
        },
        xAxis: {
            categories: categories,
            labels: {
                enabled: false
            },
            type: "datetime",
        },
        yAxis: {
            title: {
                text: "Количество"
            }
        },
        series: series,
        tooltip: {
            shared: true,
            crosshairs: true
        },
        plotOptions: {
            series: {
                cursor: 'pointer',
                point: {
                    events: {
                        click: function () {
                            hs.htmlExpand(null, {
                                pageOrigin: {
                                    x: this.pageX,
                                    y: this.pageY
                                },
                                headingText: this.category,
                                maincontentText: this.series.name + ': ' + this.y,
                                width: 200
                            });
                        }
                    }
                },
                marker: {
                    lineWidth: 1
                }
            }
        },
    });
};

dmProto.createAmountsData = function (container, data) {
    var columnSeries = [],
        pieSeries = [],
        genSeries = [],
        catSeries = [],
        amountsTable = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        }),
        genresTable = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        }),
        categoriesTable = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        }),
        usersTable = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        });
    amountsTable.append("<thead><tr><th>Показатель</th><th>Значение</th></tr></thead>");
    genresTable.append("<thead><tr><th>Жанр</th><th>Количество публикаций</th></tr></thead>");
    categoriesTable.append("<thead><tr><th>Категория</th><th>Количество публикаций</th></tr></thead>");
    usersTable.append("<thead><tr><th>Пользователи</th><th>Количество</th></tr></thead>");
    $.map(data, function (item) {
        var name = item.Name,
            amount = item.Count;
        switch (item.Search) {
            case "users":
                usersTable.append("<tr><td>" + name + "</td><td>" + amount + "</td></tr>");
                pieSeries.push([name, amount]);
                break;
            case "amount":
                amountsTable.append("<tr><td>" + name + "</td><td>" + amount + "</td></tr>");
                columnSeries.push({
                    name: name,
                    data: [amount],
                    dataLabels: {
                        enabled: true,
                        align: 'center'
                    }
                });
                break;
            case "genre":
                genresTable.append("<tr><td>" + name + "</td><td>" + amount + "</td></tr>");
                genSeries.push({
                    name: name,
                    data: [amount],
                    dataLabels: {
                        enabled: true,
                        align: 'left'
                    }
                });
                break;
            case "category":
                categoriesTable.append("<tr><td>" + name + "</td><td>" + amount + "</td></tr>");
                catSeries.push({
                    name: name,
                    data: [amount],
                    dataLabels: {
                        enabled: true,
                        align: 'left'
                    }
                });
                break;
        }
    });
    container.find(".tableView").append("<h2>Количественные показатели</h2>").append(amountsTable)
        .append("<h2>Соотношение пользователей</h2>").append(usersTable)
        .append("<h2>Публикации по жанрам</h2>").append(categoriesTable)
        .append("<h2>Публикации по категориям</h2>").append(genresTable);
    (function _renderColumnChart(series, chartCont) {
        container.find(".chartView").append(chartCont);
        chartCont.highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: 'Количественные показатели'
            },
            xAxis: {
                labels: {
                    enabled: false
                }
            },
            yAxis: {
                title: "Количество"
            },
            tooltip: {
                headerFormat: " "
            },
            series: series

        });
    })(columnSeries, $("<div />"));
    (function _renderPieChart(series, chartCont) {
        container.find(".chartView").append(chartCont);
        chartCont.highcharts({
            chart: {
                plotBackgroundColor: null,
                plotBorderWidth: null,
                plotShadow: false
            },
            title: {
                text: 'Соотношение пользователей'
            },
            tooltip: {
                pointFormat: '<b>{point.y}</b> ({point.percentage}%)',
                percentageDecimals: 0
            },
            plotOptions: {
                pie: {
                    allowPointSelect: true,
                    cursor: 'pointer',
                    dataLabels: {
                        enabled: false
                    },
                    showInLegend: true
                }
            },
            series: [{
                type: 'pie',
                data: series
            }]
        });
    })(pieSeries, $("<div />"));
    function _renderBarChart(series, chartCont, title) {
        container.find(".chartView").append(chartCont);
        chartCont.highcharts({
            chart: {
                type: 'bar'
            },
            title: {
                text: title
            },
            xAxis: {
                labels: {
                    enabled: false
                }
            },
            yAxis: {
                title: "Количество"
            },
            tooltip: {
                headerFormat: " "
            },
            series: series
        });
    };
    _renderBarChart(genSeries, $("<div />"), "Количество публикаций по жанрам");
    _renderBarChart(catSeries, $("<div />"), "Количество публикаций по категориям");
};

dmProto.createAggregationsData = function (container, data) {
    function _renderChart(series, chartCont, stacking, title) {
        container.find(".chartView").append(chartCont);
        chartCont.highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: title
            },
            xAxis: {
                labels: {
                    enabled: false,
                },
                title: {
                    text: "Пользователи"
                }                
            },
            yAxis: {
                title: "Количество",
                stackLabels: {
                    enabled: true,
                }
            },
            tooltip: {
                headerFormat: "Пользователь {point.key}<br />"
            },
            plotOptions: {
                column: {
                    stacking: stacking,
                }
            },
            series: series
        });
    };

    var entityCache = {},
        soldSeries = [{
            name: "Непроданные публикации",
            data: []
        }, {
            name: "Проданные публикации",
            data: []
        }],
        priceSeries = [{
            name: "Средняя стоимость публикации",
            data: []
        }, {
            name: "Максимальная стоимость публикации",
            data: []
        }],
        categories = [],
        table = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        });
    table.append("<thead><tr><th>Пользователь</th><th>Публикации</th><th>Продано публикаций</th><th>Средняя стоимость</th><th>Максимальная стоимость</th></tr></thead>")
    window.a = data;
    $.map(data, function (item) {
        var entity = item.Entity;
        if (!entityCache[entity]) {
            entityCache[entity] = {}
        }
        if (!entityCache[entity][item.Indicator]) {
            entityCache[entity][item.Indicator] = {};
        }
        entityCache[entity][item.Indicator][item.Aggregation.Name] = item.Aggregation.Value;
    });
    for (var key in entityCache) {
        var values = entityCache[key],
            allPub = parseFloat(values.all.sum ? values.all.sum.toFixed(2) : 0),
            soldPub = parseFloat(values.sold.sum ? values.sold.sum.toFixed(2) : 0),
            avgPrice = parseFloat(values.price.avg ? values.price.avg.toFixed(2) : 0),
            maxPrice = parseFloat(values.price.max ? values.price.max.toFixed(2) : 0);
        table.append("<tr><td>" + key + "</td><td>" + allPub + "</td><td>" + soldPub + "</td><td>" + avgPrice + "</td><td>" + maxPrice + "</td></tr>")
        soldSeries[0].data.push({
            y: allPub - soldPub,
            name: key
        });
        soldSeries[1].data.push({
            y: soldPub,
            name: key
        });
        priceSeries[0].data.push({
            y: avgPrice,
            name: key
        });
        priceSeries[1].data.push({
            y: maxPrice,
            name: key
        });
    }
    container.find(".tableView").append("<h2>Статистика по публикациям пользователей</h2>").append(table);
    _renderChart(soldSeries, $("<div />"), "normal", "Соотношение публикаций");
    _renderChart(priceSeries, $("<div />"), null, "Цены на публикации");
};

dmProto.createRatingsData = function (container, data) {
    var writerSeries = [{
        name: "Проданные публикации",
        data: []
    }],
        catSeries = [],
        genSeries = [],
        writerTable = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        }),
        genTable = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        }),
        catTable = $("<table />", {
            "class": DataMartEnum.TableSettings.Class
        });
    writerTable.append("<thead><tr><th>Пользователь</th><th>Куплено публикаций</th></tr></thead>");
    genTable.append("<thead><tr><th>Жанр</th><th>Куплено публикаций</th></tr></thead>");
    catTable.append("<thead><tr><th>Категория</th><th>Куплено публикаций</th></tr></thead>");

    function _renderWriterRatingChart(series, chartCont) {
        container.find(".chartView").append(chartCont);
        chartCont.highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: "Рейтинг авторов"
            },
            xAxis: {
                labels: {
                    enabled: false,
                },
            },
            yAxis: {
                title: "Количество"
            },
            tooltip: {
                headerFormat: "{point.key}<br />"
            },
            series: series
        });
    };
    function _renderColumnRatingChart(series, chartCont, title) {
        container.find(".chartView").append(chartCont);
        chartCont.highcharts({
            chart: {
                type: 'column'
            },
            title: {
                text: title
            },
            xAxis: {
                labels: {
                    enabled: false,
                },
            },
            yAxis: {
                title: "Количество"
            },
            tooltip: {
                headerFormat: "{point.series.name}<br/>",
                pointFormat: "Продано публикаций: {point.y}"
            },
            series: series
        });
    };
    $.map(data, function (item) {
        var name = item.Name,
            count = item.Count;
        switch (item.Search) {
            case "writer":
                if (count > 0) {
                    writerSeries[0].data.push({
                        name: name,
                        y: count
                    });
                    writerTable.append("<tr><td>" + name + "</td><td>" + count + "</td></tr>");
                }
                break;
            case "genre":
                genSeries.push({
                    name: name,
                    data: [count],
                    dataLabels: {
                        enabled: true,
                        align: 'center'
                    }
                });
                genTable.append("<tr><td>" + name + "</td><td>" + count + "</td></tr>");
                break;
            case "category":
                catSeries.push({
                    name: name,
                    data: [count],
                    dataLabels: {
                        enabled: true,
                        align: 'center'
                    }
                });
                catTable.append("<tr><td>" + name + "</td><td>" + count + "</td></tr>");
                break;
        }

    });
    container.find(".tableView").append("<h2>Рейтинг авторов</h2>").append(writerTable)
        .append("<h2>Рейтинг жанров</h2>").append(genTable)
        .append("<h2>Рейтинг категорий</h2>").append(catTable);

    _renderWriterRatingChart(writerSeries, $("<div />"));
    _renderColumnRatingChart(genSeries, $("<div />"), "Рейтинг жанров");
    _renderColumnRatingChart(catSeries, $("<div />"), "Рейтинг категорий");

};

dmProto = null;
