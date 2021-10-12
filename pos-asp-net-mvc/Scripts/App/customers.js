$(document).ready(function() {

    let dataTableColumns = [{
            name: 'customer.name',
            data: 'customer_name'
        }, {
            name: 'customer.phone',
            data: 'customer_phone'
        }, {
            name: 'customer.email',
            data: 'customer_email'
        },
        {
            name: 'customer.website',
            data: 'customer_website'
        }, {
            name: 'key_id',
            data: 'action',
            "orderable": false
        },
    ];


    dataTableInit({
        "container": "#table-data",
        "route": "/Customer/DataTable",
        "columns": dataTableColumns
    });

});