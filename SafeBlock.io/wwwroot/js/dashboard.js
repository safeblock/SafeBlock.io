$(document).ready(function () {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    setTimeout(function () {
        $("#loader").fadeOut(300);
    }, 1500);

    $(".avatar").hover(function (e) {
        $(this).find(".overlay").removeClass("hide");
    }, function () {
        $(this).find(".overlay").addClass("hide");
        });

    window.chartColors = {
        red: 'rgb(255, 99, 132)',
        orange: 'rgb(255, 159, 64)',
        yellow: 'rgb(255, 205, 86)',
        green: 'rgb(75, 192, 192)',
        blued: 'rgba(54, 162, 235, 1)',
        blue: 'rgba(54, 162, 235, 0.1)',
        purple: 'rgb(153, 102, 255)',
        grey: 'rgb(201, 203, 207)'
    };

    var ctx = document.getElementById("myChart").getContext('2d');
    var myChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
            datasets: [{
                label: 'My First dataset',
                backgroundColor: window.chartColors.blue,
                borderColor: window.chartColors.blued,
                data: [10, 30, 39, 20, 25, 34, 0],
                fill: "start",
            }/*, {
                label: 'My Second dataset',
                fill: "start",
                backgroundColor: window.chartColors.blue,
                borderColor: window.chartColors.blue,
                data: [18, 33, 22, 19, 11, 39, 30],
            }*/]
        },
        options: {
            spanGaps: false,
            elements: {
                line: {
                    tension: 0.000001
                }
            },
            plugins: {
                filler: {
                    propagate: false
                }
            },
            responsive: true,
            title: {
                display: false,
                text: "s"
            },
            scales: {
                xAxes: [{
                    gridLines: {
                        display: false,
                        drawBorder: false
                    }
                }],
                yAxes: [{
                    gridLines: {
                        display: false,
                        drawBorder: false
                    }
                }]
            }
        }
    });
});