var dataTable;

$(document).ready(function () {

    var url = window.location.href;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        }
        else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }

    }


});

    function loadDataTable(status) {
        dataTable = $('#tblData').DataTable({
            "ajax": { url: '/admin/order/getall?status=' + status },
            "columns": [
                { data: 'id', "width": "25%" },
                { data: 'name', "width": "10%" },
                { data: 'applicationUser.email', "width": "20%" },
                { data: 'phoneNumber', "width": "10%" },
                { data: 'orderStatus', "width": "10%" },
                { data: 'orderTotal', "width": "10%" },
                {
                    data: 'id',
                    "render": function (data) {
                        return `<div class="w-75 btn-group" role="group">
                                            <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"><i class="bi-pencil-square"></i>Edit</a>
                                   </div>`
                    },
                    "width": "25%"
                }

            ]
        });
    }
