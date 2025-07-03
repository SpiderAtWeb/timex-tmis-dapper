/**
 * Analytics Cards
 */

'use strict';
(function () {
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
      enabledOnSeries: undefined, // Optional: Can specify [0] or [0,1] to enable only on certain series
      formatter: function (val, opts) {
        return val; // You can format value (e.g., add %, suffixes, etc.)
      },
      textAnchor: 'middle', // Aligns the text (start | middle | end)
      distributed: false, // For bar charts, whether to distribute labels along bars
      offsetX: 0,
      offsetY: 0,
      style: {
        fontSize: '20px',
        fontFamily: 'Helvetica, Arial, sans-serif',
        fontWeight: 'bold',
        colors: ['#000'] // You can use an array or a single color string
      }
    },
  };
})();
