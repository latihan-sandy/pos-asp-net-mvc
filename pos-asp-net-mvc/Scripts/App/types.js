$(document).ready(function() {

    let dataTableColumns = [{
        name: 'type.name',
        data: 'type_name'
    }, {
        name: 'type.description',
        data: 'type_description',
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
        "route": "/Type/DataTable",
        "columns": dataTableColumns
    });

});