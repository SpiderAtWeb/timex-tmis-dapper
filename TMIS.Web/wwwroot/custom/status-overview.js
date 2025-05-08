/**
 * Analytics Cards
 */

'use strict';
let selectUrl;
(function () {

  selectUrl = document.getElementById('selectUrl').value;

  var options = {
    series: [
      {
        data: [] // We'll populate this later with AJAX data
      }
    ],
    chart: {
      height: 250,
      type: 'bar'
    },
    plotOptions: {
      bar: {
        borderRadius: 4,
        borderRadiusApplication: 'end',
        horizontal: false,
      }
    },
    dataLabels: {
      enabled: true,
      enabledOnSeries: undefined,
      formatter: function (val, opts) {
        return val
      },
      textAnchor: 'middle',
      distributed: false,
      offsetX: 0,
      offsetY: 0,
      style: {
        fontSize: '20px',
        fontFamily: 'Helvetica, Arial, sans-serif',
        fontWeight: 'bold',
        colors: ['#000']
      },
     
    },   
  };

  // Create the chart with initial empty data
  var chart = new ApexCharts(document.querySelector("#salesOverviewChart"), options);

  // Fetch data via AJAX (using fetch API)
  fetch(selectUrl)  // Replace with your actual API endpoint
    .then(response => response.json())  // Parse the JSON response
    .then(data => {
      // Map the response data to the required format for the chart
      const formattedData = data.map(item => ({
        x: item.status,  // Assuming `status` is the key for your categories like 'New', 'Idle', etc.
        y: item.value    // Assuming `value` is the key for the corresponding numeric values
      }));

      // Update the chart with the new data
      chart.updateSeries([{ data: formattedData }]);
    })
    .catch(error => {
      console.error('Error loading data:', error);
    });

  // Render the chart
  chart.render();




})();
