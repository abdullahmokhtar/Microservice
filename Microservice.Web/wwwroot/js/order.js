let dataTable;

$(document).ready(function () {
    loadTable();
});

function loadTable() {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');

    dataTable = $('#tblData').DataTable({
         "order":[0, 'desc'],
        "ajax": {
            "url": "/Order/GetAll?status=" + status,
        },
         "columns": [
             { data: "orderHeaderId", "width": "5%" },
             { data: "email", "width": "25%" },
             { data: "name", "width": "20%" },
             { data: "phone", "width": "10%" },
             { data: "statusString", "width": "10%" },
             { data: "orderTotal", "width": "10%" },
             {
                data: "orderHeaderId",
                width: "10%",
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/order/details?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                }           
             }
        ]
    })
}