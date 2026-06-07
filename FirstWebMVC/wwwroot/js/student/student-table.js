// ===============================
// Load students bằng AJAX
// ===============================
function loadStudents(page = 1) {
    currentPage = page;
    $.ajax({
        url: '/Student/GetStudents',
        type: 'GET',
        data: {
            page: currentPage,
            pageSize: currentPageSize
        },
        beforeSend: function () {
            $('#studentTableContainer').html(`
                <div class="text-center p-5">
                    <div class="spinner-border text-primary"
                         role="status">
                    </div>
                    <div class="mt-2">
                        Loading students...
                    </div>
                </div>
            `);
        },
        success: function (response) {
            $('#studentTableContainer').html(response);
        },
        error: function () {
            $('#studentTableContainer').html(`
                <div class="alert alert-danger">
                    Error loading students.
                </div>
            `);
        }
    });
}
// ===============================
// Click pagination
// ===============================
$(document).on('click', '.pagination-link', function (e) {
    e.preventDefault();
    let page = $(this).data('page');
    // Không load nếu disabled
    if ($(this).parent().hasClass('disabled')) {
        return;
    }
    loadStudents(page);
});
// ===============================
// Change page size
// ===============================
$(document).on('change', '#pageSizeSelect', function () {
    currentPageSize = $(this).val();
    // Reset về trang đầu
    currentPage = 1;
    loadStudents(currentPage);
});