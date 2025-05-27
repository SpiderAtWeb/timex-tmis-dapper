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
          url: "/PLMS/Overview/GetAllDataList",
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
      title: "Qr Code",
      width: "150px",
      hidden: true
    },
    {
      field: "InquiryRef",
      title: "Inquiry Ref",
      width: "150px"
    },
    {
      field: "Customer",
      title: "Customer",
      width: "150px"
    },
    {
      field: "StyleNo",
      title: "StyleNo",
      width: "150px"
    },
    {
      field: "StyleDesc",
      title: "Style Desc",
      width: "150px"
    },
    {
      field: "ColorCode",
      title: "Color",
      width: "150px"
    },
    {
      field: "InquiryType",
      title: "Inquiry Type",
      width: "250px"
    },
    {
      field: "ResponseType",
      title: "Response Type",
      width: "250px"
    },
    {
      field: "Seasons",
      title: "Seasons",
      width: "250px"
    },
    {
      field: "SampleStage",
      title: "Extend",
      width: "150px"
    },
    {
      field: "SampleType",
      title: "Sub-Extend",
      width: "150px"
    },
    {
      field: "InquiryRecDate",
      title: "Inq.Rec Date",
      format: "{0:dd/MM/yy}",
      width: "150px"
    },
    {
      field: "DateResponseReq",
      title: "Response Exp Date",
      format: "{0:dd/MM/yy}",
      width: "150px"
    },
    {
      field: "InquiryComment",
      title: "Inquiry Comment",
      width: "250px"
    },
    {
      field: "IsFOBOrCM",
      title: "FOB/CM",
      width: "100px"
    },
    {
      field: "Price",
      title: "Price",
      width: "100px"
    },
    {
      field: "SMV",
      title: "SMV",
      width: "100px"
    },
    {
      field: "BuyerComments",
      title: "Buyer Comments",
      width: "150px"
    },
    {
      field: "DateActualRespRec",
      title: "Response Rece Date",
      format: "{0:dd/MM/yy}",
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
      // Update modal content dynamically
      $('#home').html(`
                                  <div class="row justify-content-center">
                                     <div class="col-4">
                                        <div class="header">Customer</div>
                                        <div class="desc">${rowData[2]}</div>
                                     </div>
                                     <div class="col-4">
                                        <div class="header">Inquiry Type</div>
                                        <div class="desc">${rowData[6]}</div>
                                     </div>
                                      <div class="col-4">
                                        <div class="header">Style No</div>
                                        <div class="desc">${rowData[3]}</div>
                                     </div>
                                  </div>
                                  <div class="row justify-content-center">
                                     <div class="col-4">
                                        <div class="header">Style Description</div>
                                        <div class="desc">${rowData[4]}</div>
                                     </div>
                                     <div class="col-4">
                                        <div class="header">Color</div>
                                        <div class="desc">${rowData[5]}</div>
                                     </div>
                                      <div class="col-4">
                                        <div class="header">Response Type</div>
                                        <div class="desc">${rowData[7]}</div>
                                     </div>
                                  </div>
                                  <div class="row justify-content-center">
                                     <div class="col-4">
                                        <div class="header">Season</div>
                                        <div class="desc">${rowData[8]}</div>
                                     </div>                                 
                                     <div class="col-4">
                                        <div class="header">Extend</div>
                                        <div class="desc">${rowData[9]}</div>
                                     </div>
                                         <div class="col-4">
                                        <div class="header">Sub-Extend</div>
                                        <div class="desc">${rowData[10]}</div>
                                     </div>
                                  </div>
                                    <div class="row justify-content-center">
                                     <div class="col">
                                        <div class="header">Inquiry Comment</div>
                                        <div class="desc">${rowData[13]}</div>
                                     </div>                                                
                                  </div>
                                    <div class="row justify-content-center">
                                     <div class="col-4">
                                        <div class="header">${rowData[14]} Price</div>
                                        <div class="desc">${rowData[15]}</div>
                                     </div>
                                      <div class="col-4">
                                        <div class="header">SMV Value</div>
                                        <div class="desc">${rowData[16]}</div>
                                     </div>
                                      <div class="col-4">
                                     </div> 
                                  </div>
                                   <div id="activityContainer">
                                        <div id="loadingMessage" style="display:none;">Loading...</div>
                                   </div>
                                `);

      handleChange(rowData[0]);
      // Show the modal
      $('#rowModal').modal('show');
    }
  });

  // Ensure the close button works
  $('#rowModal').on('hidden.bs.modal', function () {
    // Reset content or any necessary cleanup after closing
    $('#home').html('');
  });

  // Manually close the modal if the close button is clicked
  $('#rowModal .close').click(function () {
    $('#rowModal').modal('hide');
  });

  function handleChange(id) {
    $('#loadingMessage').show();

    $.ajax({
      url: '/PLMS/Common/LoadModal?Id=' + id,
      type: 'POST',
      contentType: 'application/json',
      success: function (data) {

        $('#artwork').attr('src', '');

        if (data.artWork) {
          // Set the src attribute of the image with the base64 data
          $('#artwork').attr('src', 'data:image/jpeg;base64,' + data.artWork);
        }

        // Clear previous logs before appending new ones
        $('#contact').html('');

        if (data.logStrings && data.logStrings.length > 0) {
          data.logStrings.forEach(function (log) {
            $('#contact').append('<p>' + log + '</p>');
          });
        } else {
          $('#contact').append('<p>No logs found.</p>');
        }

        var container = $('#activityContainer');
        container.empty();

        var rowHtml = `<div class="table-responsive freeze-column"> <table class="table table-striped">
                                                                    <thead>
                                                                        <tr>
                                                                            <th></th>
                                                                            <th>#</th>
                                                                            <th class="freeze">Activity</th>
                                                                            <th>Plan Date</th>
                                                                            <th>Comp. Date</th>
                                                                            <th>Due Dates</th>
                                                                            <th>Done By</th>
                                                                            <th>Plan Comments</th>
                                                                            <th>Completed Comments</th>
                                                                        </tr>
                                                                     </thead>
                                                                  <tbody>`;

        $.each(data.activityList, function (index, activity) {
          // Create HTML for the main activity
          rowHtml += `<tr>
                       <td>
                        <i class="fa ${(activity.activityIsCompleted && activity.dueDates > 0) ? 'fa-check-circle text-warning' :
              (activity.activityIsCompleted) ? 'fa-check-circle text-success' :
                (!activity.activityIsCompleted && activity.dueDates < 0) ? 'fa-times-circle text-danger' : ''}" 
                                        style="font-size: 18px;"></i>
                       </td>
                       <td>${index + 1}</td>
                       <td class="freeze">${activity.activityText}</td>
                       <td>${activity.activityRequiredDate}</td>
                       <td>${activity.activityActualCmpltdDate}</td>
                       <td>${activity.dueDates > 0 ? activity.dueDates + ' Days' : ''}</td>
                       <td>${activity.activityDoneBy}</td>
                       <td>${activity.activityComment}</td>
                       <td>${activity.activityDoneComment}</td>
                       </tr>`;

          // Iterate over sub-activities and create HTML for each
          $.each(activity.subActivityList, function (subIndex, subActivity) {
            rowHtml += ` <tr>
                            <td>
                            <i class="fa ${(subActivity.activityIsCompleted && subActivity.dueDates > 0) ? 'fa-check-circle text-warning' :
                (subActivity.activityIsCompleted) ? 'fa-check-circle text-success' :
                  (!subActivity.activityIsCompleted && subActivity.dueDates < 0) ? 'fa-times-circle text-danger' : ''}" 
                                            style="font-size: 18px;"></i>
                            </td>
                            <td> ${index + 1} . ${subIndex + 1}</td>
                            <td class="sub-task freeze">${subActivity.subActivityText}</td>
                            <td>${subActivity.activityRequiredDate}</td>
                            <td>${subActivity.activityActualCmpltdDate}</td>
                            <td>${subActivity.dueDates > 0 ? subActivity.dueDates + ' Days' : ''}</td>
                            <td>${subActivity.activityDoneBy}</td>
                            <td>${subActivity.activityComment}</td>
                            <td>${subActivity.activityDoneComment}</td>
                            </tr>`;
          });
        });

        rowHtml += `</tbody> </table> </div>`;
        container.append(rowHtml);
      },
      error: function (xhr, status, error) {
        $('#loadingMessage').hide();
        // Hide the loading text after success
        console.error("Error:", xhr);
        console.error("Error:", status);
        console.error("Error:", error);
      }
    });
  }
});
