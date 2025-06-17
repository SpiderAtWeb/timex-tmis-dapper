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
})();
