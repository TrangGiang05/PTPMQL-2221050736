// ===============================
// Open Create Modal
// ===============================
$(document).on('click', '#btnAddBook', function () {

    $.ajax({

        url: '/Book/Create',

        type: 'GET',

        success: function (response) {

            $('#modalContainer').html(response);

            // Bootstrap 5
            const modal = new bootstrap.Modal(
                document.getElementById('createBookModal')
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
$(document).on('submit', '#createBookForm', function (e) {

    e.preventDefault();

    let form = $(this);

    $.ajax({

        url: '/Book/Create',

        type: 'POST',

        data: form.serialize(),

        success: function (response) {

            // Create success
            if (response.success) {

                // Close modal
                const modalElement =
                    document.getElementById('createBookModal');

                const modal =
                    bootstrap.Modal.getInstance(modalElement);

                modal.hide();

                // Reload table
                loadBooks(currentPage);

            }
            else {

                // Nếu validate lỗi
                $('#modalContainer').html(response);

                const modal = new bootstrap.Modal(
                    document.getElementById('createBookModal')
                );

                modal.show();

            }

        },

        error: function () {

            alert('Create failed');

        }

    });

});