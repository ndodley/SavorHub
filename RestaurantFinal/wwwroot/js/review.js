$(document).ready(function () {
    loadDataTable();

    // Edit button click handler (delegated)
    $('#tblData').on('click', '.btn-edit-review', function () {
        var id = $(this).data('id');
        $.get(`/api/review/${id}`, function (data) {
            $('#editReviewId').val(data.id);
            $('#editReviewContent').val(data.content);
            $('#editReviewRating').val(data.rating);
            var modal = new bootstrap.Modal(document.getElementById('editReviewModal'));
            modal.show();
        });
    });

    // Save changes button handler
    $('#saveEditReviewBtn').on('click', function () {
        var id = $('#editReviewId').val();
        var review = {
            Id: id,
            Content: $('#editReviewContent').val(),
            Rating: $('#editReviewRating').val()
        };
        $.ajax({
            url: `/api/review/${id}`,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(review),
            success: function (data) {
                if (data.success) {
                    $('#tblData').DataTable().ajax.reload();
                    toastr.success(data.message);
                    var modal = bootstrap.Modal.getInstance(document.getElementById('editReviewModal'));
                    modal.hide();
                } else {
                    toastr.error(data.message || 'Error updating review');
                }
            },
            error: function (xhr) {
                toastr.error('Error updating review');
            }
        });
    });
});

function loadDataTable() {
    $('#tblData').DataTable({
        "ajax": {
            "url": "/api/review",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "content", "width": "50%" },
            { "data": "rating", "width": "10%" },
            { "data": "date", "width": "20%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <button type="button" class="btn btn-success text-white btn-edit-review" data-id="${data}" style="cursor:pointer; width:100px;">
                                    Edit
                                </button>
                                &nbsp;
                                <a onClick=Delete('/api/review/'+${data}) class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                    Delete
                                </a>
                            </div>`;
                }, "width": "20%"
            }
        ],
        "width": "100%"
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        $('#tblData').DataTable().ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}