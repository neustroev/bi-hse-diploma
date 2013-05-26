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
    //element.find(".chartView").addClass("active");
    //element.find("button.chartViewControl").addClass("active");
    if (!element.data("dataLoaded")) {
        //TO DO: ajax calls
        element.find(".chartView").highcharts({
            chart: {
                type: 'line',
                marginRight: 130,
                marginBottom: 25
            },
            title: {
                text: 'Monthly Average Temperature',
                x: -20 //center
            },
            subtitle: {
                text: 'Source: WorldClimate.com',
                x: -20
            },
            xAxis: {
                categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
            },
            yAxis: {
                title: {
                    text: 'Temperature (°C)'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                valueSuffix: '°C'
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                x: -10,
                y: 100,
                borderWidth: 0
            },
            series: [{
                name: 'Tokyo',
                data: [7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
            }, {
                name: 'New York',
                data: [-0.2, 0.8, 5.7, 11.3, 17.0, 22.0, 24.8, 24.1, 20.1, 14.1, 8.6, 2.5]
            }, {
                name: 'Berlin',
                data: [-0.9, 0.6, 3.5, 8.4, 13.5, 17.0, 18.6, 17.9, 14.3, 9.0, 3.9, 1.0]
            }, {
                name: 'London',
                data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
            }]
        });
        element.data("dataLoaded", true);
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

dmProto = null;
