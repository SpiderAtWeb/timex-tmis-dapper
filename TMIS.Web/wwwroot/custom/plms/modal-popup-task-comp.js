$(document).ready(function () {
  // Initialize the DataTable
  var table = $('.dt-tbl').DataTable({
    "paging": true,
    "searching": true,
    "ordering": true,
    "info": true,
    "autoWidth": false,
    pageLength: 100,
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
      }
    ]
  });

  // Add double-click event to table rows
  $('#mainTable').on('dblclick', 'tr', function () {
    var rowData = table.row(this).data(); // Get the data for the clicked row
    if (rowData) {

      $("#rowModalLabel").text(rowData[1]);

      // Update modal content dynamically
      $('#home').html(`<div class="row justify-content-center">
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
                                        <div class="header">Sample Type</div>
                                        <div class="desc">${rowData[7]}</div>
                                     </div>
                                  </div>                                
                                    <div class="row justify-content-center">
                                      <div class="col-4">
                                        <div class="header">Season</div>
                                        <div class="desc">${rowData[8]}</div>
                                     </div>                                    
                                      <div class="col-8">
                                        <div class="header">Remarks</div>
                                        <div class="desc">${rowData[9]}</div>
                                     </div>
                                     </div>                                                
                                  </div>
                                   <div id="activityContainer">
                                        <div id="loadingMessage" style="display:none;">Loading...</div>
                                   </div>`);

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
      url: '/PLMS/TaskCompletion/LoadModal?Id=' + id,
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
                                                                     <th>Upload Zip</th>
                                                                 </tr>
                                                              </thead>
                                                           <tbody>`;

        $.each(data.activityList, function (index, activity) {
          // Create HTML for the main activity
          rowHtml += ` <tr data-id="${activity.id}">
                     <td>
                      <i class="fa ${(activity.isCompleted && activity.dueDates > 0) ? 'fa-check-circle text-warning' :
              (activity.isCompleted) ? 'fa-check-circle text-success' :
              (!activity.isCompleted && activity.dueDates < 0) ? 'fa-times-circle text-danger' : ''}"
                                    style="font-size: 18px;">
                     </td>
                     <td>${index + 1}</td>
                     <td class="freeze">${activity.activityName}</td>
                     <td>${activity.requiredDate}</td>
                     <td>${activity.actualCompletedDate}</td>
                     <td>${activity.dueDates > 0 ? activity.dueDates + ' Days' : ''}</td>
                     <td>${activity.doneBy}</td>
                     <td>${activity.planRemakrs}</td>
                     <td>
                       ${activity.isCompleted ? activity.remarks :
                      `<input type="text" class="form-control activity-done-comment" placeholder="Enter comment" />`}
                     </td>
                     <td>
                      ${activity.isCompleted ? '' :`<input type="checkbox" class="activity-checkbox" />`}</td>
                      <td>
                       ${activity.isCompleted ? (
                          activity.zipFilePath
                            ? `<a href="/${activity.zipFilePath}" target="_blank" title="Download ZIP"><i class="fa fa-paperclip text-primary" style="font-size: 18px;"></i></a>`
                            : ''
                        ) : `
                 <div class="d-flex flex-column align-items-start">
                                  <div class="custom-zip-upload">
                                    <label class="upload-icon" title="Upload ZIP">
                                      <span class="upload-icon-wrapper">
                                          <i class="fa fa-upload"></i>
                                        </span>
                                      <input type="file" class="zip-upload" accept=".zip" />
                                    </label>
                                  </div>
                                </div>
              `}
                      </td>
                     </tr>`;

          // Iterate over sub-activities and create HTML for each
          $.each(activity.subActivityList, function (subIndex, subActivity) {
            rowHtml += `<tr data-id="${subActivity.id}" class="sub-task-row">
                      <td>
                         <i class="fa ${(subActivity.isCompleted && subActivity.dueDates > 0) ? 'fa-check-circle text-warning' :
                                        (subActivity.isCompleted) ? 'fa-check-circle text-success' :
                                        (!subActivity.isCompleted && subActivity.dueDates < 0) ? 'fa-times-circle text-danger' : ''}" 
                                        style="font-size: 18px;">
                         </i>
                      </td>
                      <td>${index + 1} . ${subIndex + 1}</td>
                      <td class="sub-task freeze">${subActivity.subactivityName}</td>
                      <td>${subActivity.requiredDate}</td>
                      <td>${subActivity.actualCompletedDate}</td>
                      <td>${subActivity.dueDates > 0 ? subActivity.dueDates + ' Days' : ''}</td>
                      <td>${subActivity.doneBy}</td>
                      <td>${subActivity.planRemakrs}</td>
                      <td>
                         ${subActivity.isCompleted ? subActivity.remarks :
                        `<input type="text" class="form-control activity-done-comment" placeholder="Enter Comment" />`}
                      </td>
                      <td>
                         ${subActivity.isCompleted ? '' :
                        `<input type="checkbox" class="activity-checkbox" />`}
                      </td>
                        <td>
                          ${subActivity.isCompleted ? (
                            subActivity.zipFilePath
                              ? `<a href="/${subActivity.zipFilePath}" target="_blank" title="Download ZIP"><i class="fa fa-paperclip text-primary" style="font-size: 18px;"></i></a>`
                                              : ''
                                          ) : `
                                <div class="d-flex flex-column align-items-start">  
                                  <div class="custom-zip-upload">
                                    <label class="upload-icon" title="Upload ZIP">
                                      <span class="upload-icon-wrapper">
                                          <i class="fa fa-upload"></i>
                                        </span>
                                      <input type="file" class="zip-upload" accept=".zip" />
                                    </label>
                                  </div>
                                </div>
                              `}
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

$(document).on('change', '.zip-upload', function () {
  const file = this.files[0];
  const maxSize = 5 * 1024 * 1024; // 5MB in bytes

  if (file && file.size > maxSize) {
    alert("File size exceeds the 5MB limit.");
    this.value = ''; // Clear the file input
    return;
  }

  const iconSpan = $(this).closest('label').find('.upload-icon-wrapper');
  iconSpan.html('<i class="fa fa-paperclip text-success"></i>');
});


function saveCheckedActivities() {
  // Arrays to store checked parent and child activities
  //let mainTaskList = [];
  //let subTaskList = [];
  let formData = new FormData();

  let mainIndex = 0;
  let subIndex = 0;

  // Iterate over all rows with checkboxes
  $(".activity-checkbox:checked").each(function () {
    // Get the parent row
    let $row = $(this).closest("tr");

    // Extract id and comment
    let taskId = $row.data("id");
    let comment = $row.find(".activity-done-comment").val();
    let fileInput = $row.find(".zip-upload")[0];
    let zipFile = fileInput?.files[0];


    formData.append(`mainTasks[${mainIndex}].taskId`, taskId);
    formData.append(`mainTasks[${mainIndex}].comment`, comment);
    formData.append(`mainTasks[${mainIndex}].zipFile`, zipFile);

    if ($row.hasClass("sub-task-row")) {
      formData.append(`subTasks[${subIndex}].taskId`, taskId);
      formData.append(`subTasks[${subIndex}].comment`, comment);
      if (zipFile) {
        formData.append(`subTasks[${subIndex}].zipFile`, zipFile);
      }
      subIndex++;
    } else {
      formData.append(`mainTasks[${mainIndex}].taskId`, taskId);
      formData.append(`mainTasks[${mainIndex}].comment`, comment);
      if (zipFile) {
        formData.append(`mainTasks[${mainIndex}].zipFile`, zipFile);
      }
      mainIndex++;
    }

  });

  if (mainIndex === 0 && subIndex === 0) {
    toastr.warning("No activities selected.");
    return;
  }

  // Example AJAX call
  $.ajax({
    url: '/PLMS/TaskCompletion/SaveCompletedTasks', // Replace with your actual endpoint URL
    type: 'POST',
    data: formData,
    processData: false, // Required for FormData
    contentType: false, // Required for FormData   
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
