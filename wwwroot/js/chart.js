var ctx = document.getElementById('lineChart').getContext('2d');
var myChart = new Chart(ctx,{
    type: 'line',
    data: {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun','Jul','Aug','Sep','Oct','Nov','Dec'],
        datasets: [{
            label: 'Earning in ₪',
            data: [7000, 11453, 78453, 85949, 11939, 5939,547562,856575,13255,8678,64517,35547],
            backgrounfColor: [
                'rgba(85,85,85,1)'
                

            ],
            borderColor: [
                'rgba(41,155,99)'
               
            ],
            borderWidth: 1
        }],
    },
    options: {
        responsive: true
    }
});