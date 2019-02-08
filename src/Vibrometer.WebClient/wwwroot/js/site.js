window.Vibrometer = {
    InitializeChart: function (id, title, xmin, xmax, xlabel, ymin, ymax, ylabel)
    {
        context = document.getElementById(id);

        window.Vibrometer.Chart = new Chart(context,
            {
                type: "scatter",
                data: {
                    datasets: [{
                        data: [],
                        backgroundColor: "rgba(233, 30, 99, 0.2)",
                        borderColor: "rgba(233, 30, 99)",
                        borderWidth: 1,
                        lineTension: 0.25,
                        pointRadius: 1,
                        showLine: true
                    }]
                },
                options: {
                    legend: {
                        display: false
                    },
                    scales: {
                        xAxes: [{
                            display: true,
                            scaleLabel: {
                                display: true,
                                labelString: xlabel
                            },
                            ticks: {
                                autoSkip: true,
                                min: xmin,
                                max: xmax
                            }
                        }],
                        yAxes: [{
                            display: true,
                            position: "left",
                            scaleLabel: {
                                display: true,
                                labelString: ylabel
                            },
                            //ticks: {
                            //    beginAtZero: true,
                            //    min: ymin,
                            //    max: ymax
                            //},
                            type: "linear"
                        }]
                    },
                    title: {
                        display: true,
                        fontColor: "#555",
                        fontSize: 17,
                        fontStyle: "",
                        padding: 25,
                        text: title
                    },
                    tooltips: {
                        enabled: false
                    }
                }
            });

        return true;
    },
    UpdateChart: function (id, data)
    {
        window.Vibrometer.Chart.config.data.datasets[0].data = data;
        window.Vibrometer.Chart.update(0);

        return true;
    }
};