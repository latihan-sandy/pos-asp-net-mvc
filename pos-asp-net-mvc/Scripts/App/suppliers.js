$(document).ready(function() {

    let dataTableColumns = [{
        name: 'supplier.name',
        data: 'supplier_name'
    }, {
        name: 'supplier.phone',
        data: 'supplier_phone'
    }, {
        name: 'supplier.email',
        data: 'supplier_email'
    },
        {
            name: 'supplier.website',
            data: 'supplier_website'
        }, {
            name: 'key_id',
            data: 'action',
            "orderable": false
        },
    ];


    dataTableInit({
        "container": "#table-data",
        "route": "/Supplier/DataTable",
        "columns": dataTableColumns
    });

});