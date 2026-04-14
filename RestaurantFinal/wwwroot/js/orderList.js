var dataTable;
var currencyFormatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
});
var pickupTimeFormatter = new Intl.DateTimeFormat('en-US', {
    month: 'short',
    day: '2-digit',
    year: 'numeric',
    hour: 'numeric',
    minute: '2-digit'
});

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("cancelled")) {
        loadList("cancelled");
    }
    else {
        if (url.includes("completed")) {
            loadList("completed");
        }
        else {
            if (url.includes("ready")) {
                loadList("ready");
            }
            else if (url.includes("all")) {
                loadList("all");
            }
            else {
                loadList("inprocess");
            }
        }
    }
});


function loadList(param) {
    var columns = [
        { "data": "id", "width": "15%" },
        { "data": "pickupName", "width": "15%" },
        { "data": "applicationUser.email", "width": "15%" },
        {
            "data": "orderTotal",
            "width": "15%",
            "render": function (data, type) {
                var numericValue = Number(data);

                if (type !== 'display' && type !== 'filter') {
                    return numericValue;
                }

                return Number.isFinite(numericValue)
                    ? currencyFormatter.format(numericValue)
                    : '0.00';
            }
        },
        {
            "data": "pickUpTime",
            "width": "25%",
            "render": function (data, type) {
                if (type !== 'display' && type !== 'filter') {
                    return data;
                }

                var dateValue = new Date(data);
                return Number.isNaN(dateValue.getTime())
                    ? data
                    : pickupTimeFormatter.format(dateValue);
            }
        },
        {
            "data": "id",
            "render": function (data) {
                return `<div class="w-75 btn-group" >
                        <a href="/Admin/Order/OrderDetails?id=${data}"  class="btn btn-success text-white mx-2">
                        <i class="bi bi-pencil-square"></i>  </a>
                        </div>`
            },
            "width": "15%"
        }
    ];

    if (param === "all") {
        columns.splice(5, 0, { "data": "status", "width": "10%" });
    }

    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/order?status=" + param,
            "type": "GET",
            "datatype": "json"
        },
        "columns": columns,
        "width": "100%"
    });
}



/*
var dataTable;
$(document).ready(function () {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/order?",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "15%" },
            { "data": "pickupName", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },
            { "data": "pickUpTime", "width": "25%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" >
                            <a href="/Admin/Order/OrderDetails?id=${data}"  class="btn btn-success text-white mx-2">
                            <i class="bi bi-pencil-square"></i>  </a>
                            </div>`
                },

                "width": "15%"
            }
        ],
        "width": "100%"
    });
});
*/