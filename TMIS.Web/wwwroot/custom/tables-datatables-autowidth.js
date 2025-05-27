'use strict';

$(document).ready(function () {
  $('.dt-tbl').DataTable({
    "paging": true,
    "searching": true,
    "ordering": true,
    "info": true,
    "autoWidth": false,
    "columnDefs": [
      {
        // Change '0' to your target column index
        "targets": 0,
        "render": function (data, type, row) {
          if (type === 'display' && data.length > 20) {
            const truncated = data.substr(0, 20) + '...';
            return `<span title="${data.replace(/"/g, '&quot;')}">${truncated}</span>`;
          }
          return `<span title="${data.replace(/"/g, '&quot;')}">${data}</span>`;
        }      
      }]
  });
});
