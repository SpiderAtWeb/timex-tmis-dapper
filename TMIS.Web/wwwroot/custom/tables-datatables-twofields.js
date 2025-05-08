/**
 * DataTables Basic
 */

'use strict';

let fv, offCanvasEl, selectUrl, insertUrl, updateUrl, deleteUrl;
let  dt_basic;
document.addEventListener('DOMContentLoaded', function () { 

  selectUrl = document.getElementById('selectUrl').value;
  insertUrl = document.getElementById('insertUrl').value;
  updateUrl = document.getElementById('updateUrl').value;
  deleteUrl = document.getElementById('deleteUrl').value;
  (function () {

    const formAddNewRecord = document.getElementById('form-add-new-record');
    setTimeout(() => {
      const newRecord = document.querySelector('.create-new'),
        offCanvasElement = document.querySelector('#add-new-record');

      // To open offCanvas, to add new record
      if (newRecord) {
        newRecord.addEventListener('click', function () {
          offCanvasEl = new bootstrap.Offcanvas(offCanvasElement);
          // Empty fields on offCanvas open
          $('#recordId').val('');
          (offCanvasElement.querySelector('.dt-prop-name').value = ''),
            (offCanvasElement.querySelector('.dt-prop-desc').value = ''),
            // Open offCanvas with form
            offCanvasEl.show();
        });
      }
    }, 200);

    // Form validation for Add new record
    fv = FormValidation.formValidation(formAddNewRecord, {
      fields: {
        propName: {
          validators: {
            notEmpty: {
              message: 'The Name is required'
            }
          }
        },
        propDesc: {
          validators: {
            notEmpty: {
              message: 'The Description is required'
            },            
          }
        },
      },

      plugins: {
        trigger: new FormValidation.plugins.Trigger(),
        bootstrap5: new FormValidation.plugins.Bootstrap5({
          // Use this for enabling/changing valid/invalid class
          // eleInvalidClass: '',
          eleValidClass: '',
          rowSelector: '.col-sm-12'
        }),
        submitButton: new FormValidation.plugins.SubmitButton(),
        // defaultSubmit: new FormValidation.plugins.DefaultSubmit(),
        autoFocus: new FormValidation.plugins.AutoFocus()
      },
      init: instance => {
        instance.on('plugins.message.placed', function (e) {
          if (e.element.parentElement.classList.contains('input-group')) {
            e.element.parentElement.insertAdjacentElement('afterend', e.messageElement);
          }
        });
      }
    });
  })();

});




// datatable (jquery)
$(function () {
  var dt_basic_table = $('.datatables-basic');

  // DataTable with buttons
  // --------------------------------------------------------------------

  if (dt_basic_table.length) {
    dt_basic = dt_basic_table.DataTable({
      ajax: selectUrl,
      type: 'GET',
      columns: [
        { data: 'id' },
        { data: 'propName' },
        { data: 'propDesc' },
        { data: '' }
      ],
      columnDefs: [
        {
          targets: 0,
          searchable: false,
          visible: false
        },
        {
          targets: 1
        },
        {
          targets: -1, // Target the action column
          orderable: false,
          searchable: false,
          render: function (data, type, row, meta) {
            return `
              <button class="edit-record btn btn-primary btn-sm" data-id="${row.id}"> <i class="ri-pencil-fill"></i> </button>
              <button class="delete-record btn btn-danger btn-sm" data-id="${row.id}"><i class="ri-delete-bin-fill"></i></button>
            `;
          }
        },
      ],
      order: [[1, 'asc']],
      dom: '<"card-header flex-column flex-md-row"<"head-label text-center"><"dt-action-buttons text-end pt-3 pt-md-0"B>><"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
      displayLength: 7,
      lengthMenu: [7, 10, 25, 50, 75, 100],
      language: {
        paginate: {
          next: '<i class="ri-arrow-right-s-line"></i>',
          previous: '<i class="ri-arrow-left-s-line"></i>'
        }
      },
      buttons: [
        {
          text: '<i class="ri-add-line"></i> <span class="d-none d-sm-inline-block">New Record</span>',
          className: 'create-new btn btn-primary waves-effect waves-light'
        }
      ],     
    });   
  }
  fv.on('core.form.valid', function () {
    var recordId = $('#recordId').val(); // Get the record ID
    var $new_name = $('.add-new-record .dt-prop-name').val(), $new_desc = $('.add-new-record .dt-prop-desc').val();

    if (recordId) {
      updateRecord();
    }
    else {
      insertRecord();
    }
      // Hide offcanvas using javascript method
      offCanvasEl.hide();
  });

  // Handle "Edit" button click
  $('.datatables-basic tbody').on('click', '.edit-record', function () {
    var row = dt_basic.row($(this).parents('tr'));
    var rowData = row.data();

    // Show offCanvas form
    offCanvasEl = new bootstrap.Offcanvas(document.querySelector('#add-new-record'));
    offCanvasEl.show();

    // Populate the form with the row data
    $('.dt-prop-name').val(rowData.propName);
    $('.dt-prop-desc').val(rowData.propDesc);

    // Store the record ID in a hidden field (to use during update)
    $('#recordId').val(rowData.id);

  });
    
  // Handle form submission for Insert
  function insertRecord() {   
    var updatedName = $('.dt-prop-name').val();
    var updatedDesc = $('.dt-prop-desc').val();

    $.ajax({
      url: insertUrl,
      type: 'POST',
      data: {      
        propName: updatedName,
        propDesc: updatedDesc
      },
      success: function (response) {
        if (response.success) {
          dt_basic.ajax.reload(null, false);

          // Hide offCanvas
          offCanvasEl.hide();
        } else {
          // Handle error
          alert(response.message);
        }
      },
      error: function (xhr, status, error) {
        alert('An error occurred: ' + error);
      }
    });
  }

  // Handle form submission for update
  function updateRecord() {

    var recordId = $('#recordId').val();
    var updatedName = $('.dt-prop-name').val();
    var updatedDesc = $('.dt-prop-desc').val();

    $.ajax({
      url: updateUrl,
      type: 'POST',
      data: {
        id: recordId,
        propName: updatedName,
        propDesc: updatedDesc
      },
      success: function (response) {
        if (response.success) {
          dt_basic.ajax.reload(null, false);

          // Hide offCanvas
          offCanvasEl.hide();
        } else {
          // Handle error
          alert(response.message);
        }
      },
      error: function (xhr, status, error) {
        alert('An error occurred: ' + error);
      }
    });
  }
  // Handle "Delete" button click
  $('.datatables-basic tbody').on('click', '.delete-record', function () {
    var row = dt_basic.row($(this).parents('tr'));
    var rowData = row.data();
    var recordId = rowData.id

    // Alert With Functional Confirm Button
    Swal.fire({
      title: 'Are you sure to delete ? ' + rowData.propName,
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      customClass: {
        confirmButton: 'btn btn-primary me-3 waves-effect waves-light',
        cancelButton: 'btn btn-outline-secondary waves-effect'
      },
      buttonsStyling: false
    }).then(function (result) {
      if (result.value) {
        // AJAX delete call
        $.ajax({
          url: deleteUrl +"/" + recordId, // Replace with your delete URL
          type: 'GET',        
          success: function (response) {
            dt_basic.ajax.reload(null, false);
            Swal.fire({
              icon: 'success',
              title: 'Deleted!',
              text: 'Your file has been deleted.',
              customClass: {
                confirmButton: 'btn btn-success waves-effect'
              }
            });
          },
          error: function (xhr, status, error) {
            Swal.fire({
              icon: 'error',
              title: 'Oops...',
              text: 'Something went wrong! ' + error,
              customClass: {
                confirmButton: 'btn btn-danger waves-effect'
              }
            });
          }
        });
      }
    });
  });

});



