/**
 * DataTables Basic
 */

'use strict';

$(document).ready(function () {
  $('#dashboard').DataTable({
    "paging": true,
    "searching": true,
    "ordering": true,
    "info": true,
    "autoWidth": false,
    columnDefs: [
      {
        // Label
        targets: 3,
        render: function (data, type, full, meta) {
          var $status_number = data;
          var $status = {
            1: { title: 'New', class: ' bg-label-primary' },
            2: { title: 'Idle', class: ' bg-label-danger' },
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



