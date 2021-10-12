$(document).ready(function() {

    let dataTableColumns = [{
            name: 'product.sku',
            data: 'product_sku'
        }, {
            name: 'product.name',
            data: 'product_name'
        },
        {
            name: 'product.stock',
            data: 'product_stock'
        }, {
            name: 'key_id',
            data: 'action',
            "orderable": false
        },
    ];


    dataTableInit({
        "container": "#table-data",
        "route": "/Product/DataTable",
        "columns": dataTableColumns
    });

    var calcPriceSale = function() {
        let purchase = parseFloat($("#price_purchase").val());
        let profit = parseFloat($("#price_profit").val());
        let prc = parseFloat(profit / 100);
        let cost = purchase * prc;
        let priceSale = purchase + cost;
        $("#price_sales").val(priceSale || purchase);
    }

    $('#price_purchase').keyup(function() {
        calcPriceSale();
    });

    $('#price_profit').keyup(function() {
        calcPriceSale();
    });

    $("#form-submit select").addClass("form-control");
    $("#form-submit select").select2({
        theme: "bootstrap",
    });

});