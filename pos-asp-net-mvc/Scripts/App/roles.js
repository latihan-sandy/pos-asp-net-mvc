$(document).ready(function() {

    let dataTableColumns = [{
        name: 'role.name',
        data: 'role_name'
    },  {
        name: 'key_id',
        data: 'action',
        orderable: false,
        render: function(data, type, row, meta) {
            if (row.role_name !== 'Administrator') {
                return data;
            } else {
                var action = data.split("&nbsp;");
                var btn = "";
                action.forEach(function(row) {
                    let temp = $(row);
                    if (temp.hasClass("btn-show")) {
                        btn += row;
                    }
                });
                return btn;
            }
        }
    }, ];


    dataTableInit({
        "container": "#table-data",
        "route": "/Role/DataTable",
        "columns": dataTableColumns
    });

});