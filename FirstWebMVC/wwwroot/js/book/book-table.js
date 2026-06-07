// ===============================
// Load books bằng AJAX
// ===============================
function loadBooks(page = 1) {
    currentPage = page;
    $.ajax({
        url: '/Book/GetBooks',
        type: 'GET',
        data: {
            page: currentPage,
            pageSize: currentPageSize
        },
        beforeSend: function () {
            $('#bookTableContainer').html(`
                <div class="text-center p-5">
                    <div class="spinner-border text-primary"
                         role="status">
                    </div>
                    <div class="mt-2">
                        Loading books...
                    </div>
                </div>
            `);
        },
        success: function (response) {
            $('#bookTableContainer').html(response);
        },
        error: function () {
            $('#bookTableContainer').html(`
                <div class="alert alert-danger">
                    Error loading books.
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
    loadBooks(page);
});
// ===============================
// Change page size
// ===============================
$(document).on('change', '#pageSizeSelect', function () {
    currentPageSize = $(this).val();
    // Reset về trang đầu
    currentPage = 1;
    loadBooks(currentPage);
});