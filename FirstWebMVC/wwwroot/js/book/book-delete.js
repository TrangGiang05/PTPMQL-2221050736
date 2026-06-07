$(document).on('click', '.btn-delete-book', function () {
    let id = $(this).data('id');
    $.ajax({
        url: '/Book/Delete/' + id,
        type: 'GET',
        success: function (response) {
            $('#modalContainer').html(response);
            const modal = new bootstrap.Modal(document.getElementById('deleteBookModal')
            );
            modal.show();
        },
        error: function () {
            alert('Cannot load delete form');
        }
    });
});
$(document).on('submit', '#deleteBookForm', function (e) {
    e.preventDefault();
    let form = $(this);
    $.ajax({
        url: '/Book/Delete',
        type: 'POST',
        data: form.serialize(),
        success: function (response) {
            if (response.success) {
                // Close modal
                const modalElement = document.getElementById('deleteBookModal');
                const modal = bootstrap.Modal.getInstance(modalElement);
                modal.hide();
                // Reload table
                loadBooks(currentPage);
            }
            else {
                alert('Delete failed');
            }
        },
        error: function () {
            alert('Delete failed');
        }
    });
});