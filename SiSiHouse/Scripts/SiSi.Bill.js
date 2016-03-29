
// --- List ---
$(function () {
    var PRODUCT_ID = 0,
        PICTURE = 1,
        PRODUCT_CODE = 2,
        PRODUCT_NAME = 3,
        BRAND_NAME = 4,
        CATEGORY_NAME = 5,
        COLOR_NAME = 6,
        SIZE = 7,
        QUANTITY = 8,
        SALES = 9,
        MODIFIED_DATE = 10,
        MODIFIED_USER = 11,
        RETAIL_CODE = 12,
        PRODUCT_DETAIL_ID = 13,
        STATUS_ID = 14;

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
        "sAjaxSource": '/ManageBill/Search',
        "fnServerParams": serverParams
    };
    var aoColumnDefs = [
        { "sName": "CREATED_DATE", "bSortable": false, "bVisible": false, "aTargets": [PRODUCT_ID], "sWidth": "0%" },
        { "sName": "CREATED_DATE", "bSortable": false, "aTargets": [PICTURE], "sWidth": "8%", "sClass": "center", "mRender": function (data, type, full) { return '<img src="' + data + '" class="tb-display-picture">'; } },
        { "sName": "PRODUCT_CODE", "aTargets": [PRODUCT_CODE], "bVisible": false },
        { "sName": "PRODUCT_NAME", "aTargets": [PRODUCT_NAME], "sWidth": "21%", "sClass": "left", "mRender": function (data, type, full) { return '<div>Mã: ' + full[PRODUCT_CODE] + '</div><div>Tên: ' + data + '</div>'; } },
        { "sName": "BRAND_NAME", "aTargets": [BRAND_NAME], "bVisible": false },
        { "sName": "BRAND_NAME", "aTargets": [CATEGORY_NAME], "sWidth": "16%", "sClass": "left", "mRender": function (data, type, full) { return '<div>Hãng: ' + full[BRAND_NAME] + '</div><div>Loại: ' + data + '</div>'; } },
        { "sName": "SIZE", "aTargets": [COLOR_NAME], "bVisible": false },
        { "sName": "SIZE", "aTargets": [SIZE], "sWidth": "9%", "sClass": "left", "mRender": function (data, type, full) { return '<div>Màu: ' + full[COLOR_NAME] + '</div><div>Size: ' + data + '</div>'; } },
        { "sName": "QUANTITY", "aTargets": [QUANTITY], "sWidth": "6%", "sClass": "center" },
        { "sName": "SALES", "aTargets": [SALES], "sWidth": "8%", "sClass": "right" },
        { "sName": "CREATED_DATE", "aTargets": [MODIFIED_DATE], "sWidth": "9%", "sClass": "left" },
        { "sName": "CREATED_USER", "aTargets": [MODIFIED_USER], "sWidth": "9%", "sClass": "left" },
        {
            "sName": "CREATED_DATE", "bVisible": Constant.ROLE.ADMIN == Constant.CurrentUserRole, "bSortable": false, "aTargets": [RETAIL_CODE], "sWidth": "8%", "sClass": "center"
            , "mRender": function (data, type, full) { return BindAction(data, full[PRODUCT_ID], full[PRODUCT_DETAIL_ID], full[QUANTITY], full[STATUS_ID]); }
        }
    ];

    var dataTable = CreateDataTable('#InfoTable', oSorting, oPaginate, oServerSide, aoColumnDefs);

    function serverParams(aoData) {
        aoData.push(
            { "name": "TARGET_YEAR", "value": 0 },
            { "name": "TARGET_MONTH", "value": 0 },
            { "name": "PRODUCT_CODE", "value": $("#Condition_PRODUCT_CODE").val() },
            { "name": "PRODUCT_NAME", "value": $("#Condition_PRODUCT_NAME").val() },
            { "name": "TARGET_DATE", "value": $("#Condition_TARGET_DATE").val() },
            { "name": "SEX", "value": getCheckedValue('.select-sex') },
            { "name": "BRAND_ID", "value": getCheckedValue('.select-brand') },
            { "name": "CATEGORY_ID", "value": getCheckedValue('.select-category') }
        );
    }

    function BindAction(retailCode, productID, productDetailID, quantity, statusID) {
        var html = '<div><a class="action-undo" data-retail-code="' + retailCode + '" data-product-id="' + productID + '" data-product-detail-id="' + productDetailID + '" data-quantity="' + quantity + '" data-status-id="' + statusID + '"> <i class="fa fa-undo error"></i> Trả lại</a></div>'
            + '<div><a class="action-delete" data-retail-code="' + retailCode + '" data-product-id="' + productID + '" data-product-detail-id="' + productDetailID + '" data-quantity="' + quantity + '" data-status-id="' + statusID + '"> <i class="fa fa-remove error"></i> Xóa</a></div>';

        return html;
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

        searchByCondition();
    });

    $(document).off(".action-delete");
    $(document).on("click", ".action-delete", function () {
        var retailCode = $(this).data('retail-code');

        var param = JSON.stringify({
            RETAIL_CODE: retailCode,
            PRODUCT_ID: $(this).data('product-id'),
            PRODUCT_DETAIL_ID: $(this).data('product-detail-id'),
            QUANTITY: $(this).data('quantity'),
            STATUS_ID: $(this).data('status-id'),
            IS_UNDO: false
        });

        DoActionWithBill(param, Constant.MESSAGE.CONFIRM_DELETE_BILL.replace('{0}', retailCode));
    });

    $(document).off(".action-undo");
    $(document).on("click", ".action-undo", function () {
        var retailCode = $(this).data('retail-code');
        var param = JSON.stringify({
            RETAIL_CODE: retailCode,
            PRODUCT_ID: $(this).data('product-id'),
            PRODUCT_DETAIL_ID: $(this).data('product-detail-id'),
            QUANTITY: $(this).data('quantity'),
            STATUS_ID: $(this).data('status-id'),
            IS_UNDO: true
        });

        DoActionWithBill(param, Constant.MESSAGE.CONFIRM_UNDO_BILL.replace('{0}', retailCode));
    });

    function DoActionWithBill(param, messageConfirm) {
        SiSi.utility.ShowConfirmDialog(messageConfirm, function (action) {
            if (action) {
                $.ajax({
                    url: '/ManageBill/DoAction',
                    type: 'POST',
                    data: param,
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    processData: false,
                    success: function (data) {
                        if (data.statusCode == 201) { // update success
                            SiSi.utility.showInformationDialog(Constant.DIALOG.SUSSCES, data.message);
                            dataTable.fnStandingRedraw();
                        }

                        if (typeof (data.ErrorMessages) !== 'undefined') // invalid data
                            SiSi.utility.showInformationDialog(Constant.DIALOG.WARNING, data.ErrorMessages);
                    },
                    error: function (error) {
                        if (error.status == 419) //419: Authentication Timeout
                            SiSi.utility.showInformationDialog(Constant.DIALOG.WARNING, Constant.MESSAGE.CONNECT_TIMEOUT, Constant.URL_REDIRECT_TIMEOUT);
                    }
                });
            }
        });
    }
});
