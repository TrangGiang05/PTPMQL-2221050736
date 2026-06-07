$(document).on('click', '.btn-delete-student', function () {
    let id = $(this).data('id');
    $.ajax({
        url: '/Student/Delete/' + id,
        type: 'GET',
        success: function (response) {
            $('#modalContainer').html(response);
            const modal = new bootstrap.Modal(document.getElementById('deleteStudentModal')
            );
            modal.show();
        },
        error: function () {
            alert('Cannot load delete form');
        }
    });
});
$(document).on('submit', '#deleteStudentForm', function (e) {
    e.preventDefault();
    let form = $(this);
    $.ajax({
        url: '/Student/Delete',
        type: 'POST',
        dataType: 'json',
        data: form.serialize(),
        success: function (response) {
            console.log('Delete response:', response);
            if (response.success) {
                // Close modal
                const modalElement = document.getElementById('deleteStudentModal');
                const modal = bootstrap.Modal.getInstance(modalElement);
                modal.hide();

                // Remove backdrop
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');

                // Reload table
                loadStudents(currentPage);
            }
            else {
                alert('Lỗi: ' + (response.message || 'Xóa sinh viên thất bại'));
            }
        },
        error: function (xhr, status, error) {
            console.error('Delete error:', xhr.responseText);
            alert('Delete failed: ' + error);
        }
    });
});