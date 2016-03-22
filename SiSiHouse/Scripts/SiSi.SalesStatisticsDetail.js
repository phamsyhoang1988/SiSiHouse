
var productID = 0;

// --- List ---
$(function () {
    var oSorting = {
        "bSort": true
    };
    var oPaginate = {
        "bPaginate": true,
        "iDisplayLength": 20,
        "bStateSave": false
    };
    var oServerSide = {
        "bServerSide": true,
        "sAjaxSource": '/ManageStatistics/SearchDetail',
        "fnServerParams": serverParams,
        "fnInitComplete": displayData,
        "fnDrawCallback": reDrawTable
    };
    var aoColumnDefs = [
        { "sName": "MODIFIED_DATE", "bSortable": false, "bVisible": false, "aTargets": [0], "sWidth": "0%" },
        { "sName": "MODIFIED_DATE", "bSortable": false, "aTargets": [1], "sWidth": "8%", "sClass": "center", "mRender": function (data, type, full) { return '<img src="' + data + '" class="tb-display-artwork">'; } },
        { "sName": "PRODUCT_CODE", "aTargets": [2], "bVisible": false },
        { "sName": "PRODUCT_NAME", "aTargets": [3], "sWidth": "21%", "sClass": "left", "mRender": function (data, type, full) { return '<div>Mã: ' + full[2] + '</div><div>Tên: ' + data + '</div>'; } },
        { "sName": "BRAND_NAME", "aTargets": [4], "bVisible": false },
        { "sName": "BRAND_NAME", "aTargets": [5], "sWidth": "16%", "sClass": "left", "mRender": function (data, type, full) { return '<div>Hãng: ' + full[4] + '</div><div>Loại: ' + data + '</div>'; } },
        { "sName": "BRAND_NAME", "aTargets": [6], "bVisible": false },
        { "sName": "SIZE", "aTargets": [7], "sWidth": "7%", "sClass": "left", "mRender": function (data, type, full) { return '<div>Màu: ' + full[6] + '</div><div>Size: ' + data + '</div>'; } },
        { "sName": "QUANTITY", "aTargets": [8], "sWidth": "7%", "sClass": "center" },
        { "sName": "SALES", "bSortable": false, "aTargets": [9], "sWidth": "9%", "sClass": "right" },
        { "sName": "SALES", "aTargets": [10], "sWidth": "9%", "sClass": "right" },
        { "sName": "SALES", "bSortable": false, "aTargets": [11], "sWidth": "9%", "sClass": "right" },
        { "sName": "SALES", "bSortable": false, "aTargets": [12], "sWidth": "8%", "sClass": "right" },
        { "sName": "SALES", "aTargets": [13], "sWidth": "7%", "sClass": "center" }
    ];

    var dataTable = CreateDataTable('#InfoTable', oSorting, oPaginate, oServerSide, aoColumnDefs);

    function serverParams(aoData) {
        aoData.push(
            { "name": "TARGET_YEAR", "value": $("#Condition_TARGET_YEAR").val() },
            { "name": "TARGET_MONTH", "value": $("#Condition_TARGET_MONTH").val() },
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

    var dataReturn = null;

    function displayData(data) {
        dataReturn = data;
    }

    function reDrawTable() {
        var html = '<tr class="total">'
            + '<td colspan="4" class="center">Tổng</td>'
            + '<td class="center">' + dataReturn.totalQuantity + '</td>'
            + '<td class="right">' + dataReturn.totalCost + '</td>'
            + '<td class="right">' + dataReturn.totalSales + '</td>'
            + '<td class="right">' + dataReturn.totalProfit + '</td>'
            + '<td class="right">' + dataReturn.totalProfitRate + '</td>'
            + '<td></td></tr>';

        $('#InfoTable tfoot').empty().append(html);
    }

    function searchByCondition() {
        dataTable.fnPageChange("first");
    }

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

        searchByCondition();
    });
});
