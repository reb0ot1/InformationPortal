(function () {
    window.lineColunmChart = {
        showChart: function (data, containerName) {
            Highcharts.chart(containerName, {
                chart: {
                    zoomType: 'xy'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: [{
                    categories: data.categories,
                    crosshair: true
                }],
                yAxis: [{ // Primary yAxis
                    labels: {
                        format: data.yAxisFirstValueFormat,
                        style: {
                            color: Highcharts.getOptions().colors[1]
                        }
                    },
                    title: {
                        text: data.yAxisFirstText,
                    }
                }, { // Secondary yAxis
                    title: {
                        text: data.yAxisSecondText,
                    },
                    labels: {
                        format: data.yAxisFirstValueFormat,
                        style: {
                            color: Highcharts.getOptions().colors[0]
                        }
                    },
                    opposite: true
                }],
                tooltip: {
                    shared: true
                },
                //legend: {
                //    layout: 'vertical',
                //    align: 'left',
                //    x: 120,
                //    verticalAlign: 'top',
                //    y: 100,
                //    floating: true,
                //    backgroundColor:
                //        Highcharts.defaultOptions.legend.backgroundColor || // theme
                //        'rgba(255,255,255,0.25)'
                //},
                series: data.series
            });
        }
    }
})();