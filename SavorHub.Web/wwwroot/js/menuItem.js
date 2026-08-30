var dataTable;
$(document).ready(function () {
    loadList();

    $('#csvFileInput').change(function () {
        var fileInput = $('#csvFileInput')[0];
        if (fileInput.files.length === 0) {
            alert("Please select a file");
            return;
        }

        var file = fileInput.files[0];
        var reader = new FileReader();
        reader.onload = function (e) {
            var csvData = e.target.result;
            displayCSVData(csvData);
        };
        reader.readAsText(file);
    });

    $('#uploadCSVButton').click(function () {
        var fileInput = $('#csvFileInput')[0];
        if (fileInput.files.length === 0) {
            alert("Please select a file");
            return;
        }

        var formData = new FormData();
        formData.append("file", fileInput.files[0]);

        $.ajax({
            url: '/api/MenuItem/UploadCSV',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                alert(response);
                dataTable.ajax.reload();
            },
            error: function (error) {
                alert("Error uploading file");
            }
        });
    });
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/MenuItem",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "price", "width": "10%" },
            { "data": "category.name", "width": "15%" },
            { "data": "foodType.name", "width": "15%" },
            {
                "data": "image",
                "render": function (data) {
                    return `<img src="${data}" alt="Image" style="width: 50px; height: 50px;" />`;
                },
                "width": "20%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" >
                            <a href="/Admin/MenuItems/upsert?id=${data}"  class="btn btn-success text-white mx-2">
                            <i class="bi bi-pencil-square"></i>Edit  </a>
                            <a onClick=Delete('/api/MenuItem/'+${data})  class="btn btn-danger text-white mx-2">
                             <i class="bi bi-trash-fill"></i>Delete  </a>
                            </div>`
                },
                "width": "20%"
            }
        ],
        "width": "100%"
    });
}

function displayCSVData(csvData) {
    var lines = csvData.split('\n');
    var headers = lines[0].split(',');

    var data = [];
    for (var i = 1; i < lines.length; i++) {
        if (!lines[i]) continue;
        var obj = {};
        var currentline = lines[i].split(',');
        for (var j = 0; j < headers.length; j++) {
            obj[headers[j]] = currentline[j];
        }
        data.push(obj);
    }

    $('#csvTable').DataTable({
        data: data,
        columns: headers.map(function (header) {
            if (header === "Image") {
                return {
                    title: header,
                    data: header,
                    render: function (data) {
                        return `<img src="/${data}" alt="Image" style="width: 50px; height: 50px;" />`;
                    }
                };
            } else {
                return { title: header, data: header };
            }
        }),
        destroy: true
    });

    $('#csvTableContainer').show();
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
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}
