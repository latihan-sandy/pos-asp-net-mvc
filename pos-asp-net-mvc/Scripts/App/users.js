$(document).ready(function() {

    let dataTableColumns = [{
        name: 'user.username',
        data: 'user_username'
    }, {
        name: 'user.email',
        data: 'user_email'
    }, {
        name: 'user.phone',
        data: 'user_phone'
    }, {
        name: 'key_id',
        data: 'action',
        "orderable": false
    }, ];


    dataTableInit({
        "container": "#table-data",
        "route": "/User/DataTable",
        "columns": dataTableColumns
    });

    if ($("#role_ids_").length) {
        $("#role_ids_").select2({
            theme: "bootstrap",
        });
    }

});