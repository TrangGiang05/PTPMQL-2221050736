$(document).on('click', '.btn-edit-book', function () {
    let id = $(this).data('id');
    $.ajax({
        url: '/Book/Edit/' + id,
        type: 'GET',
        success: function (response) {
            $('#modalContainer').html(response);
            const modal = new bootstrap.Modal(
                document.getElementById('editBookModal')
            );
            modal.show();
        },
        error: function () {
            alert('Cannot load edit form');
        }
    });
});
$(document).on('submit', '#editBookForm', function (e) {
    e.preventDefault();
    let form = $(this);
    $.ajax({
        url: '/Book/Edit',
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                // Close modal
                const modalElement = document.getElementById('editBookModal');
                const modal = bootstrap.Modal.getInstance(modalElement);
                modal.hide();
                // Reload table
                loadBooks(currentPage);
            }
            else {
                $('#modalContainer').html(response);
                const modal = new bootstrap.Modal(document.getElementById('editBookModal')
                );
                modal.show();
            }
        },
        error: function () {
            alert('Update failed');
        }
    });
});