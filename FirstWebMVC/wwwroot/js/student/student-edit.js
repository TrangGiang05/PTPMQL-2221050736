$(document).on('click', '.btn-edit-student', function () {
    let id = $(this).data('id');
    $.ajax({
        url: '/Student/Edit/' + id,
        type: 'GET',
        success: function (response) {
            $('#modalContainer').html(response);
            const modal = new bootstrap.Modal(
                document.getElementById('editStudentModal')
            );
            modal.show();
        },
        error: function () {
            alert('Cannot load edit form');
        }
    });
});
$(document).on('submit', '#editStudentForm', function (e) {
    e.preventDefault();
    let form = $(this);
    $.ajax({
        url: '/Student/Edit',
        type: 'POST',
        dataType: 'json',
        data: form.serialize(),
        success: function (response) {
            console.log('Edit response:', response);
            if (response.success) {
                // Close modal
                const modalElement = document.getElementById('editStudentModal');
                const modal = bootstrap.Modal.getInstance(modalElement);
                modal.hide();

                // Remove backdrop
                $('.modal-backdrop').remove();
                $('body').removeClass('modal-open');

                // Reload table
                loadStudents(currentPage);
            }
            else {
                alert('Lỗi: ' + (response.message || 'Cập nhật sinh viên thất bại'));
            }
        },
        error: function (xhr, status, error) {
            console.error('Edit error:', xhr.responseText);
            alert('Update failed: ' + error);
        }
    });
});