$(document).ready(function () {
  // Initialize the DataTable
  var table = $('.dt-tbl').DataTable({
    "paging": true,
    "searching": true,
    "ordering": true,
    "info": true,
    "autoWidth": true,
    "scrollX": true,
    "columnDefs": [
      {
        "targets": "_all",
        "className": "nowrap"
      }
    ]
  });

  // Add double-click event to table rows
  $('.dt-tbl tbody').on('dblclick', 'tr', function () {
    var rowData = table.row(this).data(); // Get the data for the clicked row
    if (rowData) {

      $("#rowModalLabel").text(rowData[1]);


      // Update modal content dynamically
      $('#home').html(`
                           <div class="row justify-content-center">
                              <div class="col-4">
                                 <div class="header">Customer</div>
                                 <div class="desc">${rowData[2]}</div>
                              </div>
                              <div class="col-4">
                                 <div class="header">Inquiry Type</div>
                                 <div class="desc">${rowData[3]}</div>
                              </div>
                               <div class="col-4">
                                 <div class="header">Style No</div>
                                 <div class="desc">${rowData[4]}</div>
                              </div>
                           </div>
                           <div class="row justify-content-center">
                              <div class="col-4">
                                 <div class="header">Style Description</div>
                                 <div class="desc">${rowData[5]}</div>
                              </div>
                              <div class="col-4">
                                 <div class="header">Color</div>
                                 <div class="desc">${rowData[6]}</div>
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
                                 <div class="desc">${rowData[10]}</div>
                              </div>
                               <div class="col-4">
                                 <div class="header">Sub-Extend</div>
                                 <div class="desc">${rowData[9]}</div>
                              </div>
                           </div>
                           <div class="row justify-content-center">
                                     <div class="col">
                                        <div class="header">Inquiry Comment</div>
                                        <div class="desc">${rowData[11]}</div>
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

        var rowHtml = `<div class="table-responsive  freeze-column"> <table class="table table-striped">
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
                                                                      <th>Select</th>
                                                                 </tr>
                                                              </thead>
                                                           <tbody>`;

        $.each(data.activityList, function (index, activity) {
          // Create HTML for the main activity
          rowHtml += ` <tr data-id="${activity.taskId}">
                     <td>
                      <i class="fa ${(activity.activityIsCompleted && activity.dueDates > 0) ? 'fa-check-circle text-warning' :
              (activity.activityIsCompleted) ? 'fa-check-circle text-success' :
              (!activity.activityIsCompleted && activity.dueDates < 0) ? 'fa-times-circle text-danger' : ''}"
                                    style="font-size: 18px;">
                     </td>
                     <td>${index + 1}</td>
                     <td class="freeze">${activity.activityText}</td>
                     <td>${activity.activityRequiredDate}</td>
                     <td>${activity.activityActualCmpltdDate}</td>
                     <td>${activity.dueDates > 0 ? activity.dueDates + ' Days' : ''}</td>
                     <td>${activity.activityDoneBy}</td>
                     <td>${activity.activityComment}</td>
                     <td>
                       ${activity.activityIsCompleted ? activity.activityDoneComment :
                      `<input type="text" class="form-control activity-done-comment" placeholder="Enter comment" />`}
                     </td>
                     <td>
                      ${activity.activityIsCompleted ? '' :
                      `<input type="checkbox" class="activity-checkbox" />`}</td>
                     </tr>`;

          // Iterate over sub-activities and create HTML for each
          $.each(activity.subActivityList, function (subIndex, subActivity) {
            rowHtml += `<tr data-id="${subActivity.taskId}" class="sub-task-row">
                      <td>
                         <i class="fa ${(subActivity.activityIsCompleted && subActivity.dueDates > 0) ? 'fa-check-circle text-warning' :
                                        (subActivity.activityIsCompleted) ? 'fa-check-circle text-success' :
                                        (!subActivity.activityIsCompleted && subActivity.dueDates < 0) ? 'fa-times-circle text-danger' : ''}" 
                                        style="font-size: 18px;">
                         </i>
                      </td>
                      <td>${index + 1} . ${subIndex + 1}</td>
                      <td class="sub-task freeze">${subActivity.subActivityText}</td>
                      <td>${subActivity.activityRequiredDate}</td>
                      <td>${subActivity.activityActualCmpltdDate}</td>
                      <td>${subActivity.dueDates > 0 ? subActivity.dueDates + ' Days' : ''}</td>
                      <td>${subActivity.activityDoneBy}</td>
                      <td>${subActivity.activityComment}</td>
                      <td>
                         ${subActivity.activityIsCompleted ? subActivity.activityDoneComment :
                        `<input type="text" class="form-control activity-done-comment" placeholder="Enter Comment" />`}
                      </td>
                      <td>
                         ${subActivity.activityIsCompleted ? '' :
                        `<input type="checkbox" class="activity-checkbox" />`}
                      </td>
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

function saveCheckedActivities() {
  // Arrays to store checked parent and child activities
  let mainTaskList = [];
  let subTaskList = [];

  // Iterate over all rows with checkboxes
  $(".activity-checkbox:checked").each(function () {
    // Get the parent row
    let $row = $(this).closest("tr");

    // Extract taskId and comment
    let taskId = $row.data("id");
    let comment = $row.find(".activity-done-comment").val();

    // Check if the row is a sub-task based on a class or structure
    if ($row.hasClass("sub-task-row")) {
      // Add to child activities array
      subTaskList.push({
        taskId: taskId,
        comment: comment
      });
    } else {
      // Add to parent activities array
      mainTaskList.push({
        taskId: taskId,
        comment: comment
      });
    }
  });

  // Example AJAX call
  $.ajax({
    url: '/PLMS/TaskCompletion/SaveCompletedTasks', // Replace with your actual endpoint URL
    type: 'POST',
    contentType: 'application/json',
    data: JSON.stringify({ mainTasks: mainTaskList, subTasks: subTaskList }),
    success: function () {
      $('#rowModal').modal('hide');
      toastr.success('Data saved successfully.');
    },
    error: function (xhr, status, error) {
      $('#rowModal').modal('hide');
      toastr.error("Error! Data not saved.");
      console.error("Error:", xhr, status, error);
    },
  });
}
