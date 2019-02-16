window.Vibrometer = {
    InitializeChart: function (id, xMin, xMax, xLabel, settings)
    {
        let context = document.getElementById(id);

        let config = {
            type: "scatter",
            data: {
                datasets: [
                    {
                        data: [],
                        backgroundColor: "rgba(234, 44, 109, 0.2)",
                        borderColor: "rgba(234, 44, 109)",
                        borderWidth: 1,
                        lineTension: 0,
                        pointRadius: 2,
                        showLine: true,
                        fill: false
                    },
                    {
                        data: [],
                        backgroundColor: "rgba(0, 188, 212, 0.2)",
                        borderColor: "rgba(0, 188, 212)",
                        borderWidth: 1,
                        lineTension: 0,
                        pointRadius: 2,
                        showLine: true,
                        fill: false
                    }
                ]
            },
            options: {
                animation: {
                    duration: 0,
                    hover: {
                        animationDuration: 0
                    },
                    responsiveAnimationDuration: 0
                },
                elements: {
                    line: {
                        tension: 0
                    }
                },
                legend: {
                    display: false
                },
                scales: {
                    xAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: xLabel
                        },
                        ticks: {
                            autoSkip: true,
                            min: xMin,
                            max: xMax,
                            maxRotation: 0,
                            callback: function (value, index, values)
                            {
                                return value.toFixed(2);
                            }
                        }
                    }],
                    yAxes: [{
                        display: true,
                        position: "left",
                        scaleLabel: {
                            display: true,
                            labelString: settings.yLabel
                        },
                        type: "linear"
                    }]
                },
                title: {
                    display: false
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data)
                        {
                            return tooltipItem.xLabel.toFixed(2) + ", " + tooltipItem.yLabel.toFixed(2);
                        }
                    }
                }
            }
        };

        window.Vibrometer.UpdateChartLimits(id, settings, config.options);

        if (window.Vibrometer.Chart && window.Vibrometer.Chart.canvas === context)
        {
            window.Vibrometer.Chart.options = config.options;
            window.Vibrometer.Chart.data = config.data;
            window.Vibrometer.Chart.update({
                duration: 0
            });
        }
        else
        {
            window.Vibrometer.Chart = new Chart(context, config);
        }
    },
    UpdateChartLimits: function (id, settings, chartOptions)
    {
        let update = true;

        if (chartOptions === undefined)
        {
            chartOptions = window.Vibrometer.Chart.options;
            update = false;
        }

        switch (settings.limitMode)
        {
            case 1:
                chartOptions.scales.yAxes[0].ticks = { };
                break;
            case 2:
                chartOptions.scales.yAxes[0].ticks = {
                    beginAtZero: true,
                    min: settings.yMin
                };
                break;
            case 3:
                chartOptions.scales.yAxes[0].ticks = {
                    beginAtZero: true,
                    max: settings.yMax
                };
                break;
            case 4:
                chartOptions.scales.yAxes[0].ticks = {
                    min: settings.yMin,
                    max: settings.yMax
                };
                break;
            default:
                throw new Error();
        }

        if (update && window.Vibrometer.Chart)
        {
            window.Vibrometer.Chart.update({
                duration: 0
            });
        }
    },
    UpdateChartData: function (id, data1, data2)
    {
        window.Vibrometer.Chart.config.data.datasets[0].data = data1;
        window.Vibrometer.Chart.config.data.datasets[1].data = data2;

        window.Vibrometer.Chart.update({
            duration: 0
        });
    },
    ReadFile: function (fileInputId, readAsBinaryString)
    {
        return new Promise((resolve, reject) =>
        {
            let fileInput = document.getElementById(fileInputId);
            let fileReader = new FileReader();

            fileReader.onerror = () =>
            {
                fileReader.abort();
                reject(new DOMException("File could not be read."));
            };

            fileReader.onload = () =>
            {
                try
                {
                    if (readAsBinaryString === true)
                    {
                        resolve(fileReader.result.split("base64,")[1]);
                    }
                    else
                    {
                        resolve(JSON.parse(fileReader.result));
                    }
                } catch (e)
                {
                    reject(new DOMException("Could not parse JSON data."));
                }
            };

            if (readAsBinaryString === true)
            {
                fileReader.readAsDataURL(fileInput.files[0]);
            }
            else
            {
                fileReader.readAsText(fileInput.files[0]);
            }
        });
    },
    WriteVibFile: function (fpgaSettings)
    {
        var json = JSON.stringify(fpgaSettings, null, 2);

        var blob = new Blob([json], {
            type: "text/plain;charset=utf-8"
        });

        var url = URL.createObjectURL(blob);

        var link = document.createElement('a');
        link.download = "config.vib.json";
        link.href = url;

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
};