var productID = 0;

// --- List ---
$(function () {
    var PRODUCT_ID = 0;
    var ROOT_LINK = 1;
    var PICTURE = 2;
    var PRODUCT_INFO = 3;
    var PRODUCT_DETAIL = 4;
    var PRODUCT_IN_STOCK = 5;
    var PRODUCT_STATUS = 6;
    var SALES_PRICE = 7;
    var REAL_PRICE = 8;
    var MODIFIED_DATE = 9;
    var DELETE_FLAG = 10;
    var STATUS_ID = 11;
    var PRODUCT_NAME = 12;

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
        "sAjaxSource": '/ManageProduct/Search',
        "fnServerParams": serverParams,
        "fnInitComplete": displayData,
        "fnDrawCallback": displayComplete
    };
    var aoColumnDefs = [
        { "sName": "MODIFIED_DATE", "bSortable": false, "bVisible": false, "aTargets": [PRODUCT_ID], "sWidth": "0%" },
        {
            "sName": "MODIFIED_DATE", "bSortable": false, "bVisible": Constant.ROLE.ADMIN == Constant.CurrentUserRole, "aTargets": [ROOT_LINK], "sWidth": "6%", "sClass": "center"
            , "mRender": function (data, type, full) { return SiSi.utility.nvl(data) ? '<a href="' + data + '" target="_blank"><i class="fa fa-eye"></i> Xem</a>' : ''; }
        },
        {
            "sName": "MODIFIED_DATE", "bSortable": false, "aTargets": [PICTURE], "sWidth": "8%", "sClass": "center"
            , "mRender": function (data, type, full) { return BuildPicture(full[PRODUCT_ID], data); }
        },
        {
            "sName": "PRODUCT_NAME", "aTargets": [PRODUCT_INFO], "sWidth": "21%", "sClass": "left productName"
            , "mRender": function (data, type, full) { return BuildLinkToUpdate(full[PRODUCT_ID], data, full[DELETE_FLAG]); }
        },
        { "sName": "CATEGORY_NAME", "aTargets": [PRODUCT_DETAIL], "sWidth": "14%", "sClass": "left" },
        { "sName": "MODIFIED_DATE", "bSortable": false, "aTargets": [PRODUCT_IN_STOCK], "sWidth": "6%", "sClass": "center" },
        { "sName": "STATUS_ID", "aTargets": [PRODUCT_STATUS], "sWidth": "7%", "sClass": "left" },
        { "sName": "SALE_PRICE", "aTargets": [SALES_PRICE], "sWidth": "7%", "sClass": "right" },
        { "sName": "REAL_PRICE", "bVisible": Constant.ROLE.ADMIN == Constant.CurrentUserRole, "aTargets": [REAL_PRICE], "sWidth": "7%", "sClass": "right" },
        { "sName": "MODIFIED_DATE", "bVisible": Constant.ROLE.ADMIN == Constant.CurrentUserRole, "aTargets": [MODIFIED_DATE], "sWidth": "8%", "sClass": "center" },
        {
            "sName": "PRODUCT_NAME", "bSortable": false, "aTargets": [DELETE_FLAG], "sWidth": "6%", "sClass": "center"
            , "mRender": function (data, type, full) { return BuildAction(full[PRODUCT_ID], data, full[STATUS_ID], full[PRODUCT_NAME]); }
        }
    ];

    var dataTable = CreateDataTable('#InfoTable', oSorting, oPaginate, oServerSide, aoColumnDefs);

    function serverParams(aoData) {
        aoData.push(
            { "name": "PRODUCT_CODE", "value": $("#Condition_PRODUCT_CODE").val() },
            { "name": "PRODUCT_NAME", "value": $("#Condition_PRODUCT_NAME").val() },
            { "name": "FROM", "value": $("#Condition_FROM").val() },
            { "name": "TO", "value": $("#Condition_TO").val() },
            { "name": "SEX", "value": getCheckedValue('.select-sex') },
            { "name": "BRAND_ID", "value": getCheckedValue('.select-brand') },
            { "name": "CATEGORY_ID", "value": getCheckedValue('.select-category') },
            { "name": "STATUS_ID", "value": getCheckedValue('.select-status') },
            { "name": "DELETE_FLAG", "value": $("#Condition_DELETE_FLAG").prop('checked') }
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

    function BuildLinkToUpdate(id, display, deleted) {
        var link = Constant.ROLE.ADMIN == Constant.CurrentUserRole ? '<a href="/ManageProduct/Update/' + id + '" class="' + deleted + '">' + display + '</a>' : display;

        return link;
    }

    function BuildPicture(id, file) {
        var html = '<img src="' + file + '" class="tb-display-picture">';

        return BuildLinkToUpdate(id, html, '');
    }

    function BuildAction(productID, deleted, status, productName) {
        var html = '';

        if (Constant.STATUS.SELLING == status || Constant.STATUS.SALE_OFF == status) {
            html += '<div><a data-toggle="modal" data-target="#retailContent" data-whatever="' + productID + '"> <i class="fa fa-cart-plus"></i> Bán lẻ</a></div>'
        }

        if (Constant.ROLE.ADMIN == Constant.CurrentUserRole && deleted == '0') {
            html += '<div><a class="action-delete" data-product-id="' + productID + '" data-product-name="' + productName + '"> <i class="fa fa-remove error"></i> Xóa</a></div>'
        }

        return html;
    }

    function displayData(data) {
        if (data.aaData.length == 0)
            $(".dataTables_paginate").hide();
        else
            $(".dataTables_paginate").show();
    }

    function displayComplete(data) {
        $('#InfoTable a.1').each(function () {
            $(this).parents('tr').removeClass().addClass('deleted-row');
        });
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
        $(".search-condition input:text").val('');

        $(".select-sex, .select-brand, .select-category, .select-status, #Condition_DELETE_FLAG").prop('checked', false);

        searchByCondition();
    });

    $("#btnCreate").click(function () {
        window.location.href = '/ManageProduct/Update'
    });

    $(document).off(".action-delete");
    $(document).on("click", ".action-delete", function () {
        var id = $(this).data('product-id');
        var productName = $(this).data('product-name');

        SiSi.utility.ShowConfirmDialog(Constant.MESSAGE.CONFIRM_DELETE_PRODUCT.replace('{0}', productName), function (action) {
            if (action) {
                $.ajax({
                    url: '/ManageProduct/Delete/' + id,
                    type: 'POST',
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
    });
});


// --- Retail ---
function BindDetailInStock(colorID, size, $parent) {
    var param = {
        productID: productID,
        colorID: colorID,
        size: size
    };
    var detail = SiSi.utility.checkDataExistByAjax('/ManageProduct/JsonGetDetailInStock', param);

    $parent.find('.quantity-in-stock').text('Còn ' + detail.data.QUANTITY);
    $parent.find('.retail-product-detail-id').val(detail.data.PRODUCT_DETAIL_ID);
}

function ResetArrRetail() {
    $('#tbRetail tbody tr').each(function (index, value) {
        var productDetail = 'RetailList[' + index + '].PRODUCT_DETAIL_ID';
        var colorID = 'RetailList[' + index + '].COLOR_ID';
        var size = 'RetailList[' + index + '].SIZE';
        var quantity = 'RetailList[' + index + '].QUANTITY';
        var totalPrice = 'RetailList[' + index + '].TOTAL_PRICE';

        $(this).find('.retail-product-detail-id').attr('name', productDetail);
        $(this).find('.ddlSelectColor').attr('name', colorID);
        $(this).find('.ddlSize').attr('name', size);
        $(this).find('.retail-quantity').attr('name', quantity);
        $(this).find('.total-price').attr('name', totalPrice);
    });
}

function CalOrdersTotalPrice() {
    var totalPrice = 0;

    $('.total-price').each(function () {
        totalPrice += SiSi.utility.convertMoneyToInt(this.value);
    });

    $('#ordersTotalPrice').text(SiSi.utility.convertIntToMoney(totalPrice));
}

function ValidFormData() {
    var invalidData = [];

    $('.ddlSelectColor').each(function () {
        var $target = $(this);

        if (this.value.length == 0) {
            invalidData.push(Constant.MESSAGE.ERROR.REQUIRED_SELECT + 'màu sản phẩm.');
            return false;
        } else {
            var existColorSize = 0;
            $('.ddlSelectColor').each(function () {
                if ($target.val() == this.value
                    && $target.parents('tr').find('.ddlSize').val() == $(this).parents('tr').find('.ddlSize').val()) {
                    existColorSize++;
                }
            });

            if (existColorSize > 1) {
                invalidData.push(Constant.MESSAGE.ERROR.EXIST_COLOR_SIZE);
                return false;
            }
        }
    });

    $('.retail-quantity').each(function () {
        if (this.value.length == 0) {
            invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'số lượng bán.');
            return false;
        } else {
            var quantityInStock = parseInt($(this).parents('tr').find('.quantity-in-stock').text().replace('Còn ', ''));
            if (parseInt(this.value) > quantityInStock) {
                invalidData.push(Constant.MESSAGE.ERROR.QUANTITY);
                return false;
            }
        }
    });

    $('.total-price').each(function () {
        if (this.value.length == 0) {
            invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'đơn giá.');
            return false;
        }
    });

    return invalidData;
}

$('#retailContent').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    productID = parseInt(button.data('whatever'));

    $(this).find('.modal-body').empty().load('/ManageProduct/Retail/' + productID);
});

$(document).off(".btnAddProduct");
$(document).on("click", ".btnAddProduct", function () {
    var html = $('#tbRetail tbody tr:first').prop('outerHTML');

    $('#tbRetail tbody tr:last').after(html);

    var $target = $('#tbRetail tbody tr:last');

    $target.find('input:text').attr('readonly', 'readonly');
    $target.find('input, select').val('');
    $target.find('.quantity-in-stock').text('');
    $target.find('.ddlSize').empty();

    ResetArrRetail();
});

$(document).off(".remove-product");
$(document).on("click", ".remove-product", function () {
    var $parent = $(this).parents('tr');

    if ($('#tbRetail tbody tr').length > 1) {
        $parent.remove();
        ResetArrRetail();

    } else {
        $parent.find('input, select').val('');
        $parent.find('.quantity-in-stock').text('');
        $parent.find('.ddlSize').empty();
    }
});

$(document).off(".ddlSelectColor");
$(document).on("change", ".ddlSelectColor", function () {
    var $parent = $(this).parents('tr');

    $parent.find('.ddlSize').empty();

    if (this.value.length > 0) {
        var param = {
            productID: productID,
            colorID: this.value
        };
        var sizes = SiSi.utility.checkDataExistByAjax('/ManageProduct/JsonGetSizeByColor', param);

        if (sizes.data.length > 0) {
            var html = '';
            for (var i = 0; i < sizes.data.length; i++) {
                var size = sizes.data[i].SIZE;

                html += '<option value="' + size + '">' + size + '</option>';
            }

            $parent.find('.ddlSize').append(html);
            $parent.find('input:text').removeAttr('readonly');

            BindDetailInStock(this.value, $parent.find('.ddlSize').val(), $parent);
        }
    } else {
        $parent.find('.quantity-in-stock').text('');
        $parent.find('input').val('');
        $parent.find('input:text').attr('readonly', 'readonly');
    }
});

$(document).off(".ddlSize");
$(document).on("change", ".ddlSize", function () {
    var $parent = $(this).parents('tr');

    if (this.value.length > 0) {
        BindDetailInStock($parent.find('.ddlSelectColor').val(), this.value, $parent);
    }
});

$(document).off(".retail-quantity");
$(document).on("change", ".retail-quantity", function () {
    var realPrice = $('#ProductInfo_SALE_PRICE').val();
    var $saleOffPrice = $('#ProductInfo_SALE_OFF_PRICE');
    var $parent = $(this).parents('tr');
    var salePrice = parseInt($saleOffPrice.length == 0 ? realPrice : $saleOffPrice.val());
    var quantity = this.value.length == 0 ? 0 : parseInt(this.value);
    var totalPrice = quantity * salePrice;

    $parent.find('.total-price').val(SiSi.utility.convertIntToMoney(totalPrice));

    CalOrdersTotalPrice();
});

$(document).off(".total-price");
$(document).on("change", ".total-price", function () {
    CalOrdersTotalPrice();
});

$(document).off(".btnCreateRetail");
$(document).on("click", ".btnCreateRetail", function () {
    SiSi.utility.removeValidationError();

    var invalidData = ValidFormData();

    if (invalidData.length > 0) {
        SiSi.utility.showClientError(invalidData, '.modal #title');
        return false;
    }

    var $parent = $(this).parents('.modal-content');

    SiSi.utility.ShowConfirmDialog(Constant.MESSAGE.CONFIRM_CREATE_RETAIL, function (action) {
        if (action) {
            SiSi.utility.replaceAllMoney();
            $parent.find('.retail-regist').submit();
        }
    });
});