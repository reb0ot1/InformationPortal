(function () {
    window.lineChart = {
        showChart: function (data, containerName) {
            console.log(data);
            Highcharts.chart(containerName, {
                chart: {
                    type: 'line'
                },
                title: {
                    text: ''
                },
                subtitle: {
                    text: ''
                },
                xAxis: {
                    categories: data.categories
                },
                yAxis: {
                    title: {
                        text: ''
                    }
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.series.name+'</b>:'+this.y;
                    }
                    //pointFormat: '<b>Positive found</b>:{point.y:,.2f}'
                },
                //plotOptions: {
                //    line: {
                //        dataLabels: {
                //            enabled: true
                //        },
                //        enableMouseTracking: true
                //    }
                //},
                series: data.series
                //series: [{
                //    name: 'Tokyo',
                //    data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
                //}, {
                //    name: 'London',
                //    data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
                //}]
            });
        }
    }
})();