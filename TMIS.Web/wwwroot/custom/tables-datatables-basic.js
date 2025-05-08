'use strict';

$(document).ready(function () {
  $('.dt-tbl').DataTable({
    "paging": true,
    "searching": true,
    "ordering": true,
    "info": true,
    "autoWidth": true,
    "scrollX": true, // Enables horizontal scrolling
    "columnDefs": [
      { "targets": "_all", "className": "nowrap" } // Add 'nowrap' class to all columns
    ]
  });

});


