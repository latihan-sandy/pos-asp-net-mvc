$(document).ready(function() {

    $("body").on("click", ".btn-report-show", function(e) {
        e.preventDefault();
        let url = $(this).attr("data-href");
        let route = $(this).attr("data-route");
        $("#route").val(route);
        $("#iframe-report").attr("src", url);
        $("#myModal").modal("show");
        return false;
    });

    $('.date-filter').datepicker({
        autoclose: true,
        clearBtn: true,
        format: 'yyyy-mm-dd'
    }).on('changeDate', function(ev) {
        let route = $("#route").val();
        let first_date = $("#first_date").val();
        let last_date = $("#last_date").val();
        let src = "/Report/" + route + "?first=" + first_date + "&last=" + last_date;
        if (first_date && last_date) {
            $("#iframe-report").attr("src", src);
        }
    });

    $("#btn-print").click(function(e) {
        e.preventDefault();
        document.getElementById('iframe-report').contentWindow.print();
        return false;
    });


});