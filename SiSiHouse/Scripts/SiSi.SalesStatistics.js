
// --- List ---
$(function () {
    var oSorting = {
        "bSort": true
    };
    var oPaginate = {
        "bPaginate": false,
        "iDisplayLength": 20,
        "bStateSave": false
    };
    var oServerSide = {
        "bServerSide": true,
        "sAjaxSource": '/ManageStatistics/Search',
        "fnServerParams": serverParams,
        "fnInitComplete": displayData,
        "fnDrawCallback": reDrawTable
    };
    var aoColumnDefs = [
        { "sName": "TARGET_YEAR, TARGET_MONTH", "bSortable": false, "bVisible": false, "aTargets": [0], "sWidth": "0%" },
        { "sName": "TARGET_YEAR, TARGET_MONTH", "bSortable": false, "aTargets": [1], "sWidth": "10%", "sClass": "center", "mRender": function (data, type, full) { return BuildTime(full[0], data); } },
        { "sName": "TOTAL_QUANTITY", "bSortable": false, "aTargets": [2], "sWidth": "10%", "sClass": "center" },
        { "sName": "TOTAL_COST", "bSortable": false, "aTargets": [3], "sWidth": "20%", "sClass": "right" },
        { "sName": "TOTAL_SALES", "bSortable": false, "aTargets": [4], "sWidth": "20%", "sClass": "right" },
        { "sName": "TOTAL_PROFIT", "bSortable": false, "bSortable": false, "aTargets": [5], "sWidth": "20%", "sClass": "right" },
        { "sName": "TOTAL_PROFIT", "bSortable": false, "bSortable": false, "aTargets": [6], "sWidth": "20%", "sClass": "right" }
    ];

    var dataTable = CreateDataTable('#InfoTable', oSorting, oPaginate, oServerSide, aoColumnDefs, false);

    function serverParams(aoData) {
        aoData.push(
            { "name": "TARGET_YEAR", "value": $("#Condition_TARGET_YEAR").val() },
            { "name": "SEX", "value": getCheckedValue('.select-sex') },
            { "name": "BRAND_ID", "value": getCheckedValue('.select-brand') },
            { "name": "CATEGORY_ID", "value": getCheckedValue('.select-category') }
        );
    }

    function getCheckedValue(element) {
        var value = "";

        $(element).each(function () {
            if ($(this).prop('checked')) {
                value += (value.length > 0 ? "," + $(this).attr('alt') : $(this).attr('alt'));
            }
        });

        return value;
    }

    function BuildTime(year, month) {
        var html = '<a class="detailSalesStatistics" data-year="' + year + '" data-month="' + month + '">' + year + '/' + month + '</a>';

        return html;
    }

    var dataReturn = null;

    function displayData(data) {
        dataReturn = data;
    }

    function reDrawTable() {
        if (dataReturn != null && dataReturn.sEcho == 1) {
            var html = '<tr class="total">'
                + '<td class="center">Tổng</td>'
                + '<td class="center">' + dataReturn.totalQuantity + '</td>'
                + '<td class="right">' + dataReturn.totalCost + '</td>'
                + '<td class="right">' + dataReturn.totalSales + '</td>'
                + '<td class="right">' + dataReturn.totalProfit + '</td>'
                + '<td class="right">' + dataReturn.totalProfitRate + '</td>'
                + '</tr>';

            $('#InfoTable tfoot').append(html);
        }
    }

    function searchByCondition() {
        dataTable.fnPageChange("first");
    }

    $(".dropdown-menu input:checkbox").change(function () {
        searchByCondition();
    });

    $(".clear-condition i").click(function () {
        $(this).parents('.dropdown-menu').find('input:checkbox').prop('checked', false);
        searchByCondition();
    });

    $(document).bind('keypress', function (e) {
        var code = e.keyCode || e.which;
        if (code == 13) { //Enter keycode
            searchByCondition();
        }
    });

    $("#btnSearch").click(function () {
        searchByCondition();
    });

    $("#btnClear").click(function () {
        $('.search-condition input:text').val('');
        $('.select-sex, .select-brand, .select-category').prop('checked', false);

        var currentDate = new Date();
        $('#Condition_TARGET_YEAR').val(currentDate.getFullYear());
        $('.datepicker-years').datepicker('update', currentDate);

        searchByCondition();
    });

    $(document).off(".detailSalesStatistics");
    $(document).on("click", ".detailSalesStatistics", function () {
        var year = $(this).attr("data-year");
        var month = $(this).attr("data-month");
        var $form = $('#frmDetailSales');

        $form.find('#hdnYear').val(year);
        $form.find('#hdnMonth').val(month);
        $form.submit();
    });
});
