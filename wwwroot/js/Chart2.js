var ctx = document.getElementById('doughnut').getContext('2d');
var myChart = new Chart(ctx, {
    type: 'doughnut',
    data: {
        labels: ['TikTok', 'Facbook', 'Instegram', 'Twitter', 'Google'],
        datasets: [{
            label: 'views and exposure',
            data: [8567, 1325, 8659, 1640, 2554],
            backgroundColor: [
                'rgba(41,155,99,0.8)',
                'rgba(54,162,235,0.8)',
                'rgba(255,206,86,0.8)',
                'rgba(120,46,139,0.8)'
            ],
            borderColor: [
                'rgba(41,155,99,1)',
                'rgba(54,162,235,1)',
                'rgba(130,206,86,1)',
                'rgba(110,24,191,1)'
            ],
            borderWidth: 1
        }],
    },
    options: {
        responsive: true
    }
});