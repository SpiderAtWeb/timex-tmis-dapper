$(document).ready(function () {
  $("#configurator").kendoTabStrip();

  $("#grid").kendoGrid({
    toolbar: ["excel"],
    excel: {
      allPages: true
    },
    columns: [{
      draggable: true
    },
    {
      field: "name"
    }
    ],
    dataSource: {
      transport: {
        read: {
          url: "/TGPS/Overview/GetAllDataList",
          dataType: "jsonp"
        }
      },
      pageSize: 15,
    },
    selectable: "multiple row",
    sortable: true,
    reorderable: true,
    pageable: true,
    filterable: {
      mode: "row"
    },
    pageable: {
      buttonCount: 5
    },
    // dataBound: function () {
    //     for (var i = 0; i < this.columns.length; i++) {
    //         this.autoFitColumn(i);
    //     }
    // },
    scrollable: true,
    groupable: true,
    columns: [{
      field: "Id",
      title: "Id",
      width: "150px",
      hidden: true
    },
    {
      field: "GatePassNo",
      title: "Gatepass #",
      width: "150px"
    },
    {
      field: "GpSubject",
      title: "Gatepass Subject",
      width: "150px"
    },
    {
      field: "GenDateTime",
      title: "Gen Date/Time",
      width: "150px"
    },
    {
      field: "GenGPassTo",
      title: "To",
      width: "150px"
    },
    {
      field: "PassStatus",
      title: "Status",
      width: "150px"
    },  
    ],
  });

  // Handle double-click on table row
  $("#grid").on("dblclick", "table tr", function (e) {
    e.stopPropagation();

    var rowData = $(this).children("td").map(function () {
      return $(this).text();
    }).get();

    if (rowData) {
      handleChange(rowData[0]);
    }
   });

  function handleChange(id) {
    $('#loadingMessage').show();

    $.ajax({
      url: '/TGPS/GenGoodsPass/GetGatePassDetails?Id=' + id,
      type: 'GET',
      contentType: 'application/json',
      success: function (data) {
        if (data != null || data != undefined || data != "") {
          $('#loadingMessage').hide();
          $("#gatePassModalContent").html(data);
          $("#viewGatePassModal").modal("show");
          return;
        }
      },
      error: function (xhr, status, error) {
        $('#loadingMessage').hide();
        $("#gatePassModalContent").html("<p class='text-danger'>Failed to load details.</p>");
        $("#viewGatePassModal").modal("show");
        // Hide the loading text after success
        console.error("Error:", xhr);
        console.error("Error:", status);
        console.error("Error:", error);
      }
    });
  }
});
