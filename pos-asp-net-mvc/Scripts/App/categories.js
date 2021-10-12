$(document).ready(function() {

    let dataTableColumns = [{
        name: 'categories.name',
        data: 'category_name'
    }, {
        name: 'categories.description',
        data: 'category_description',
        render: function(data, type, row, meta) {
            var notif = data.split(".");
            return notif[0];
        }
    }, {
        name: 'key_id',
        data: 'action',
        "orderable": false
    }, ];


    dataTableInit({
        "container": "#table-data",
        "route": "/Category/DataTable",
        "columns": dataTableColumns
    });

});