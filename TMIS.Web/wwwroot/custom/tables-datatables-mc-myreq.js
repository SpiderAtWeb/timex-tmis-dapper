/**
 * DataTables Advanced (jquery)
 */

'use strict';

$(document).ready(function () {
  let selectUrl = document.getElementById('selectUrl').value;

  $('#myRequests').DataTable({
    ajax: function (data, callback, settings) {
      var selectedDate = $('#dateToday').val();
      
      $.ajax({
        url: selectUrl,
        type: 'GET',
        data: {
          dateTr: selectedDate
        },
        success: function (response) {
          // Call the callback with the response data to update the table
          callback(response);
        },
        error: function (xhr, error, code) {
          // Handle error
          console.error('Error loading data: ', error);
        }
      });
    },
    columns: [
      { data: 'id' },
      { data: 'qrCode' },
      { data: 'serialNo' },
      { data: 'machineType' },
      { data: 'currentUnit' },
      { data: 'trStatusId' },
    ],
    "paging": false,
    "searching": false,
    "ordering": true,
    "info": true,
    "autoWidth": false,
    columnDefs: [{
      // For Responsive
      targets: 0,
      orderable: false,
      searchable: false,
      visible: false
    },
      {
        // Label
        targets: 5,
        render: function (data, type, full, meta) {
          var $status_number = data;
          var $status = {
            3: { title: 'Requested', class: ' bg-label-warning' },
            4: { title: 'Approved', class: ' bg-label-success' },
            5: { title: 'Rejected', class: ' bg-label-danger' },
          };

          if (typeof $status[$status_number] === 'undefined') {
            return data;
          }
          return (
            '<span class="badge rounded-pill' +
            $status[$status_number].class +
            '">' +
            $status[$status_number].title +
            '</span>'
          );
        }
      },
    ]
  });
});

function reloadDataTable() {
  var selectedDate = $('#dateToday').val();
  $('#myRequests').DataTable().ajax.reload(null, false);
}
