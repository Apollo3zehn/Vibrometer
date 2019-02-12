window.Vibrometer = {
    InitializeChart: function (id, xmin, xmax, xlabel, ymin, ymax, ylabel)
    {
        let context = document.getElementById(id);

        let config = {
            type: "scatter",
            data: {
                datasets: [
                    {
                        data: [],
                        backgroundColor: "rgba(233, 30, 99, 0.2)",
                        borderColor: "rgba(233, 30, 99)",
                        lineTension: 0,
                        pointRadius: 1,
                        borderWidth: 1,
                        showLine: true,
                        fill: false
                    },
                    {
                        data: [],
                        backgroundColor: "rgba(0, 188, 212, 0.2)",
                        borderColor: "rgba(0, 188, 212)",
                        borderWidth: 1,
                        lineTension: 0,
                        pointRadius: 1,
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
                            labelString: xlabel
                        },
                        ticks: {
                            autoSkip: true,
                            min: xmin,
                            max: xmax,
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
                            labelString: ylabel
                        },
                        ticks: {
                            min: ymin,
                            max: ymax
                        },
                        type: "linear"
                    }]
                },
                title: {
                    display: false
                }
            }
        };

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
    UpdateChart: function (id, data1, data2)
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
    WriteVibFile: function (vibrometerState)
    {
        var json = JSON.stringify(vibrometerState, null, 2);

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