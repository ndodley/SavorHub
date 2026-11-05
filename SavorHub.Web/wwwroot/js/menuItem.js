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
                clearCsvPreview();
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

// Parses CSV text into rows of fields, handling quoted values that contain
// commas, quotes ("") or line breaks - matches CsvUtility.Parse on the server.
function parseCsv(text) {
    var rows = [], row = [], field = '', inQuotes = false;
    text = text.replace(/^\uFEFF/, '');

    for (var i = 0; i < text.length; i++) {
        var c = text[i];
        if (inQuotes) {
            if (c === '"') {
                if (text[i + 1] === '"') { field += '"'; i++; }
                else { inQuotes = false; }
            } else {
                field += c;
            }
        } else if (c === '"') {
            inQuotes = true;
        } else if (c === ',') {
            row.push(field); field = '';
        } else if (c === '\r' || c === '\n') {
            if (c === '\r' && text[i + 1] === '\n') i++;
            row.push(field); field = '';
            if (!(row.length === 1 && row[0] === '')) rows.push(row);
            row = [];
        } else {
            field += c;
        }
    }
    row.push(field);
    if (!(row.length === 1 && row[0] === '')) rows.push(row);
    return rows;
}

// Hides the preview table and resets the file picker after a successful upload,
// so the page only shows the saved menu items.
function clearCsvPreview() {
    if ($.fn.DataTable.isDataTable('#csvTable')) {
        $('#csvTable').DataTable().clear().destroy();
        $('#csvTable').empty();
    }
    $('#csvTableContainer').hide();
    $('#csvFileInput').val('');
}

function displayCSVData(csvData) {
    var rows = parseCsv(csvData);
    if (rows.length === 0) return;
    var headers = rows[0].map(function (h) { return h.trim(); });

    var data = [];
    for (var i = 1; i < rows.length; i++) {
        var obj = {};
        for (var j = 0; j < headers.length; j++) {
            obj[headers[j]] = rows[i][j] || '';
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
                        if (!data) return '';
                        var path = data.replace(/\\/g, '/');
                        // Web paths from an export (e.g. /images/menuItems/x.png) can be previewed;
                        // full paths on disk (from the sample template) can't, so show the text.
                        if (/^\/?images\//i.test(path)) {
                            var src = path.charAt(0) === '/' ? path : '/' + path;
                            return `<img src="${src}" alt="Image" style="width: 50px; height: 50px;" />`;
                        }
                        return $('<span>').text(data).prop('outerHTML');
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
