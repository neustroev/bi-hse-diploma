(function () {
    DataMartEnum = {};
    DataMartEnum.Action = {
        Transactions: "GetTransactions",
        Amounts: "GetAmounts",
        Aggregations: "GetAggregations"
    }
})();

DataMart = function (settings) {
    this.navigationPanel = settings.navigationPanel;
    this.viewContainers = settings.viewContainers;
    this.viewControls = settings.viewControls;

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
};

dmProto.showViewContent = function (element) {
    element.addClass("active");
    if (!element.data("dataLoaded")) {
        var action = element.data("action");
        $.getJSON("/DataMart/" + action, null, function (data) {
            this._onDataLoaded(element, action, data);
            element.data("dataLoaded", true);
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
        default:
            break;
    };
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
        "class": "table table-hover"
    });
    table.append("<thead><tr><th>Дата</th><th>Прибыль</th><th>Покупки</th></tr></thead>")
    $.map(data, function (item) {
        var date = new Date(parseInt(item.Date.replace(/\D/g, ''), 10)).toDateString(),
            purchase = item.PurchaseCount,
            profit = parseFloat(item.Profit.toFixed(2));
        categories.push(date);
        series[0].data.push(profit);
        series[1].data.push(purchase);
        table.append("<tr><td>" + date + "</td><td>" + profit + "</td><td>" + purchase + "</td></tr>");
    });
    container.find(".tableView").append(table);
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
            "class": "table table-hover"
        }),
        genresTable = $("<table />", {
            "class": "table table-hover"
        }),
        categoriesTable = $("<table />", {
            "class": "table table-hover"
        }),
        usersTable = $("<table />", {
            "class": "table table-hover"
        });
    amountsTable.append("<thead><tr><th>Показатель</th><th>Количество</th></tr></thead>");
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
    container.find(".tableView").append(amountsTable).append(usersTable).append(categoriesTable).append(genresTable);
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
    function _renderChart(series, categories, chartCont, title) {
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
                    enabled: true,
                    rotation: -90
                },
                categories: categories
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

    var entityCache = {},
        soldSeries = [{
            name: "Публикации пользователя",
            data: []
        }, {
            name: "Проданные публикации пользователя",
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
            "class": "table table-hover"
        });
    table.append("<thead><tr><th>Пользователь</th><th>Публикации</th><th>Продано публикаций</th><th>Средняя стоимость</th><th>Максимальная стоимость</th></tr></thead>")
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
        categories.push(key);
        soldSeries[0].data.push(allPub);
        soldSeries[1].data.push(soldPub);
        priceSeries[0].data.push(avgPrice);
        priceSeries[1].data.push(maxPrice);
    }
    container.find(".tableView").append(table);
    _renderChart(soldSeries, categories, $("<div />"));
    _renderChart(priceSeries, categories, $("<div />"));
};

dmProto = null;
