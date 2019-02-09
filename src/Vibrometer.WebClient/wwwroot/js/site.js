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
    },
    ReadVibFile: function (fileInputId)
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
                    resolve(JSON.parse(fileReader.result));
                } catch (e)
                {
                    reject(new DOMException("Could not parse JSON data."));
                }
            };

            fileReader.readAsText(fileInput.files[0]);
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