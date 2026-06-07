// ===============================
// Open Create Modal
// ===============================
$(document).on('click', '#btnAddStudent', function () {

    $.ajax({

        url: '/std/Create',

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

        url: '/std/Create',

        type: 'POST',

        data: form.serialize(),

        success: function (response) {

            // Create success
            if (response.success) {

                // Close modal
                const modalElement =
                    document.getElementById('createStudentModal');

                const modal =
                    bootstrap.Modal.getInstance(modalElement);

                modal.hide();

                // Reload table
                loadStudents(currentPage);

            }
            else {

                // Nếu validate lỗi
                $('#modalContainer').html(response);

                const modal = new bootstrap.Modal(
                    document.getElementById('createStudentModal')
                );

                modal.show();

            }

        },

        error: function () {

            alert('Create failed');

        }

    });

});