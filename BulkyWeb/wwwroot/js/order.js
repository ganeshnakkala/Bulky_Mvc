//const { Toast } = require("../lib/bootstrap/dist/js/bootstrap.bundle");

var dataTable;

    $(document).ready(function () {
        loadDataTable();
        });

    function loadDataTable() {
        dataTable = $('#tblData').DataTable({
            "ajax": { url: '/admin/order/getall' },
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
