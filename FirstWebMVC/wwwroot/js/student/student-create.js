// ===============================
// Open Create Modal
// ===============================
$(document).on('click', '#btnAddStudent', function () {

    $.ajax({

        url: '/Student/Create',

        type: 'GET',

        success: function (response) {

            $('#modalContainer').html(response);

            // Bootstrap 5
            const modal = new bootstrap.Modal(
                document.getElementById('createStudentModal')
            );

            modal.show();

        },

        error: function () {

            alert('Cannot load create form');

        }

    });

});


// ===============================
// Submit Create Form
// ===============================
$(document).on('submit', '#createStudentForm', function (e) {

    e.preventDefault();

    let form = $(this);

    $.ajax({

        url: '/Student/Create',

        type: 'POST',

        dataType: 'json',

        data: form.serialize(),

        success: function (response) {

            console.log('Create response:', response);

            // Create success
            if (response.success) {

                // Close modal
                const modalElement =
                    document.getElementById('createStudentModal');

                const modal =
                    bootstrap.Modal.getInstance(modalElement);

                modal.hide();

                // Remove backdrop
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');

                // Reload table
                loadStudents(currentPage);

            }
            else {
                // Validation error
                alert('Lỗi: ' + (response.message || 'Thêm sinh viên thất bại'));
            }

        },

        error: function (xhr, status, error) {

            console.error('Create error:', xhr.responseText);

            alert('Create failed: ' + error);

        }

    });

});