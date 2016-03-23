
$(document).ready(function () {
    SiSi.utility.formatMoney();

    if ($('#ProductInfo_PRODUCT_ID').val() !== '0') {
        SetMoneySign();
        SetSaleOffContent($("#ProductInfo_STATUS_ID").val());
    }
});


// START - Submit data
function ValidFormData() {
    var invalidData = [];
    var status = $("#ProductInfo_STATUS_ID").val();

    if ($("#ProductInfo_PRODUCT_CODE").val().length === 0) {
        invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'mã sản phẩm');
    }

    if ($("#ProductInfo_PRODUCT_NAME").val().length === 0) {
        invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'tên sản phẩm');
    }

    if (status.length === 0) {
        invalidData.push(Constant.MESSAGE.ERROR.REQUIRED_SELECT + 'trạng thái');
    } else if (status !== '0') {
        if ($('#ProductInfo_WEIGHT').val().length === 0) {
            invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'trọng lượng sản phẩm');
        }

        if ($('#ProductInfo_IMPORT_DATE').val().length === 0) {
            invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'ngày nhập kho');
        }

        if ($('#ProductInfo_SALE_PRICE').val().length === 0) {
            invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'giá bán sản phẩm');
        }

        if (status == '2' && $('#ProductInfo_SALE_OFF_PRICE').val().length == 0) {
            invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'giá khuyến mãi');
        }
    }

    if ($("#ddlSelectMoney").val().length === 0) {
        invalidData.push(Constant.MESSAGE.ERROR.REQUIRED_SELECT + 'nguồn hàng');
    }

    if ($('.product-detail:not(.deleted)').length == 0) {
        invalidData.push(Constant.MESSAGE.ERROR.REQUIRED_SELECT + 'màu của sản phẩm');
    } else {
        var existInvalid = false;
        $('.product-detail:not(.deleted)').each(function () {
            if ($(this).find('.ddlSelectColor').val().length === 0) {
                invalidData.push(Constant.MESSAGE.ERROR.REQUIRED_SELECT + 'màu của sản phẩm');
                return false;
            }

            var arrSize = [];
            $(this).find('.product-size').each(function () {
                arrSize.push(this.value.toUpperCase());
                if (this.value.length === 0) {
                    invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'cỡ sản phẩm');
                    existInvalid = true;
                    return false;
                }
            });

            if (existInvalid) {
                return false;
            }

            $(this).find('.product-quantity').each(function () {

                if (this.value.length === 0) {
                    invalidData.push(Constant.MESSAGE.ERROR.REQUIRED + 'số lượng sản phẩm');
                    existInvalid = true;
                    return false;
                }
            });

            if (existInvalid) {
                return false;
            }

            if (arrSize.length > 1) { // Check duplicate
                var sortedArr = arrSize.sort();

                for (var i = 0; i < arrSize.length - 1; i++) {
                    if (sortedArr[i + 1] == sortedArr[i]) {
                        invalidData.push('Vui lòng không nhập trùng size cho mỗi màu sản phẩm');
                        existInvalid = true;
                        break;
                    }
                }
            }

            if (existInvalid) {
                return false;
            }
        });
    }

    return invalidData;
}

$('#btnSubmit').click(function (e) {
    SiSi.utility.removeValidationError();

    var invalidData = ValidFormData();

    if (invalidData.length > 0) {
        SiSi.utility.showClientError(invalidData);
        return false;
    }

    SiSi.utility.ShowConfirmDialog(Constant.MESSAGE.CONFIRM, function (action) {
        if (action) {
            SiSi.utility.replaceAllMoney();
            $('#frmUpdate').submit();
        }
    });
});

$("#frmUpdate").submit(function (e) {
    var formData = new FormData(this);

    $.ajax({
        url: $(this).attr("action"),
        type: 'POST',
        data: formData,
        mimeType: "multipart/form-data",
        contentType: false,
        cache: false,
        processData: false,
        success: function (response) {
            var data = JSON.parse(response);

            if (data.statusCode == 201) { // update success
                SiSi.utility.showInformationDialog(Constant.DIALOG.SUSSCES, data.message, '/ManageProduct');
            }

            if (typeof (data.ErrorMessages) !== 'undefined') // invalid data
                SiSi.utility.showClientError(data.ErrorMessages);
        },
        error: function (error) {
            if (error.status == 419) //419: Authentication Timeout
                SiSi.utility.showInformationDialog(Constant.DIALOG.WARNING, Constant.MESSAGE.CONNECT_TIMEOUT, Constant.URL_REDIRECT_TIMEOUT);
        }
    });

    e.preventDefault(); // prevent Default action
});
// END - Submit data


// START - Module money
function SetMoneySign() {
    $('.import-price-sign').text($('#ddlSelectMoney option:selected').attr('sign'));
}

function SetRealPrice() {
    var exchangeRate = parseFloat($('#ProductInfo_EXCHANGE_RATE').val());
    var wage = parseFloat($('#ProductInfo_WAGE').val()) / 100 + 1;
    var weightPostage = parseFloat($('#ProductInfo_WEIGHT_POSTAGE').val());
    var importPrice = parseFloat($('#ProductInfo_IMPORT_PRICE').val());
    var weight = $('#ProductInfo_WEIGHT').val().length > 0 ? parseFloat($('#ProductInfo_WEIGHT').val()) : 0;
    var realPrice = (importPrice * wage * exchangeRate) + (weight * weightPostage);

    $('#ProductInfo_REAL_PRICE').val(realPrice.toFixed(0));
    $('#ProductInfo_REAL_PRICE').val(SiSi.utility.convertIntToMoney(realPrice.toFixed(0)));
}

function SetSaleOffContent(status) {
    if (status == '2')
        $('.sale-off-content').show();
    else {
        $('.sale-off-content').hide();
    }
}

$('#ProductInfo_IMPORT_PRICE').on('change', function (e) {
    if (this.value.length === 0)
        this.value = '0';

    SetRealPrice();
});

$('#ProductInfo_WEIGHT').on('change', function (e) {
    SetRealPrice();
});

$('#ddlSelectMoney').on('change', function (e) {
    if (this.value.length == 0) {
        $('#ProductInfo_MONEY_TYPE_ID, #ProductInfo_EXCHANGE_RATE, #ProductInfo_WEIGHT_POSTAGE, #ProductInfo_WAGE').val(0);
    } else {
        var $selectedMoney = $('#ddlSelectMoney option:selected');

        if ($('#ProductInfo_PRODUCT_ID').val() !== '0' && $('#ProductInfo_MONEY_TYPE_ID').attr('oldvalue') === this.value) {
            var compare = false;
            var message = '<div> Có sự khác nhau về giá trị ngoại tệ ở thời điểm đã lưu so với thời điểm hiện tại: ';

            if ($selectedMoney.attr('exchange-rate') !== $('#ProductInfo_EXCHANGE_RATE').attr('oldvalue')) {
                message += ' <br/> Tỷ giá chuyển đổi: <b>'
                    + SiSi.utility.convertIntToMoney($('#ProductInfo_EXCHANGE_RATE').attr('oldvalue'))
                    + 'VNĐ</b> so với <b>'
                    + SiSi.utility.convertIntToMoney($selectedMoney.attr('exchange-rate'))
                    + 'VNĐ</b>';
                compare = true;
            }

            if ($selectedMoney.attr('weight-postage') !== $('#ProductInfo_WEIGHT_POSTAGE').attr('oldvalue')) {
                message += ' <br/> Phí trọng lượng: <b>'
                    + SiSi.utility.convertIntToMoney($('#ProductInfo_WEIGHT_POSTAGE').attr('oldvalue'))
                    + 'VNĐ</b> so với <b>'
                    + SiSi.utility.convertIntToMoney($selectedMoney.attr('weight-postage'))
                    + 'VNĐ</b>';
                compare = true;
            }

            if ($selectedMoney.attr('wage') !== $('#ProductInfo_WAGE').attr('oldvalue')) {
                message += ' <br/> Phí nhập hàng: <b>'
                    + SiSi.utility.convertIntToMoney($('#ProductInfo_WAGE').attr('oldvalue'))
                    + '%</b> so với <b>'
                    + SiSi.utility.convertIntToMoney($selectedMoney.attr('wage'))
                    + '%</b>';
                compare = true;
            }

            message += ' <br/> Bạn có muốn thay đổi sang giá trị ngoại tệ ở thời điểm hiện tại không ? </div> '

            if (compare) {
                SiSi.utility.ShowConfirmDialog(message, function (action) {
                    if (action) {
                        $('#ProductInfo_EXCHANGE_RATE').val($selectedMoney.attr('exchange-rate'));
                        $('#ProductInfo_WEIGHT_POSTAGE').val($selectedMoney.attr('weight-postage'));
                        $('#ProductInfo_WAGE').val($selectedMoney.attr('wage'));
                    } else {
                        $('#ProductInfo_EXCHANGE_RATE').val($('#ProductInfo_EXCHANGE_RATE').attr('oldvalue'));
                        $('#ProductInfo_WEIGHT_POSTAGE').val($('#ProductInfo_WEIGHT_POSTAGE').attr('oldvalue'));
                        $('#ProductInfo_WAGE').val($('#ProductInfo_WAGE').attr('oldvalue'));
                    }
                });
            }
        } else {
            $('#ProductInfo_MONEY_TYPE_ID').val(this.value);
            $('#ProductInfo_EXCHANGE_RATE').val($selectedMoney.attr('exchange-rate'));
            $('#ProductInfo_WEIGHT_POSTAGE').val($selectedMoney.attr('weight-postage'));
            $('#ProductInfo_WAGE').val($selectedMoney.attr('wage'));
        }
    }

    SetMoneySign();
    SetRealPrice();
});

$('#ProductInfo_STATUS_ID').on('change', function () {
    SetSaleOffContent(this.value);
});
// END - Module money


// START - Module product detail
function ResetProductDetail() {
    $('.product-quantity-detail').each(function (index, value) {
        var detailID = 'ProductQuantityList[' + index + '].PRODUCT_DETAIL_ID';
        var colorID = 'ProductQuantityList[' + index + '].COLOR_ID';
        var deleted = 'ProductQuantityList[' + index + '].DELETED';
        var size = 'ProductQuantityList[' + index + '].SIZE';
        var quantity = 'ProductQuantityList[' + index + '].QUANTITY';

        $(this).find('.product-detail-id').attr('name', detailID);
        $(this).find('.product-color-id').attr('name', colorID);
        $(this).find('.product-deleted').attr('name', deleted);
        $(this).find('.product-size').attr('name', size);
        $(this).find('.product-quantity').attr('name', quantity);
    });
}

$('.btnAddColor').click(function (e) {
    var html = '<div class="form-group product-detail">'
        + ' <div class="col-lg-6">'
        + ' <label class="col-lg-4 control-label">Màu sắc</label>'
        + ' <div class="col-lg-4">'
        + $('.ddlSelectColor').first().prop('outerHTML')
        + ' </div>'
        + ' <div class="col-lg-4">'
        + ' <button class="btn dark btnAddSize" type="button"><i class="fa fa-plus"></i> Size</button>'
        + ' </div>'
        + ' </div>'
        + ' <div class="col-lg-6 product-quantity-list">'
        + ' <div class="form-group product-quantity-detail">'
        + ' <input class="product-detail-id" type="hidden">'
        + ' <input class="product-color-id" type="hidden">'
        + ' <div class="col-lg-3">'
        + ' <input class="form-control product-size" maxlength="10" placeholder="Size ..." type="text">'
        + ' </div>'
        + ' <div class="col-lg-3">'
        + ' <input class="form-control numeric product-quantity" maxlength="3" placeholder="Số lượng ..." type="text">'
        + ' </div>'
        + ' <div class="col-lg-3">'
        + ' <button class="btn dark btnDeleteSize" type="button"><i class="fa fa-remove"></i></button>'
        + ' </div>'
        + ' </div>'
        + ' </div>'
        + ' </div>';

    $('.product-detail:last').after(html);

    var $targetContent = $('.product-detail:last');
    var productDetailID = $targetContent.index() * -1;

    $targetContent.find('.product-detail-id').val(productDetailID);
    $targetContent.find('.ddlSelectColor').val('').removeClass('deleted seleted');

    ResetProductDetail();
});

$(document).off('.btnAddSize');
$(document).on('click', '.btnAddSize', function () {
    var $targetContent = $(this).parents('.product-detail');
    var html = $targetContent.find('.product-quantity-detail').first().prop('outerHTML');

    $targetContent.find('.product-quantity-detail').last().after(html);

    var $targetElement = $targetContent.find('.product-quantity-detail').last();

    $targetElement.find('.product-detail-id').val('-1');
    $targetElement.find('input:text').val('').removeAttr('value');
    $targetElement.find('.product-deleted').remove();

    ResetProductDetail();
});

$(document).off('.btnDeleteSize');
$(document).on('click', '.btnDeleteSize', function () {
    var $parentContent = $(this).parents('.product-detail');

    if ($parentContent.find('.product-quantity-detail:not(.deleted)').length > 1) {
        var $targetContent = $(this).parents('.product-quantity-detail');

        if ($targetContent.hasClass('old-value')) {
            $targetContent.find('.product-deleted').val(true);
            $targetContent.addClass('deleted').hide();
        } else {
            $targetContent.remove();
        }
    } else {
        if ($parentContent.hasClass('old-value')) {
            $parentContent.addClass('deleted').hide();
            $parentContent.find('.product-deleted').val(true);
            $parentContent.find('.ddlSelectColor').addClass('deleted');
        } else {
            if ($('.product-detail:not(.deleted)').length > 1)
                $parentContent.remove();
            else {
                $parentContent.find('.ddlSelectColor, .product-size, .product-quantity, .product-color-id').val('');
            }
        }
    }

    ResetProductDetail();
});

$(document).off('.ddlSelectColor');
$(document).on('change', '.ddlSelectColor', function () {
    var $target = $(this);
    var selectID = $target.val();
    var exist = false;

    $(this).addClass('selected');
    $('.ddlSelectColor:not(.selected, .deleted) option:selected').each(function () {
        if (selectID === this.value) {
            exist = true;
            return false;
        }
    });

    if (exist) {
        SiSi.utility.showInformationDialog(Constant.DIALOG.WARNING, Constant.MESSAGE.ERROR.EXIST_COLOR);
        $target.val('');
        $target.parents('.product-detail').find('.product-color-id').val('');
    }

    $target.removeClass('selected');
    $target.parents('.product-detail').find('.product-color-id').val(selectID);
});
// END - Module product detail


// START - Module picture
function ResetPictureDetail() {
    $('.picture-detail').each(function (index, value) {
        var pictureID = 'PictureList[' + index + '].PICTURE_ID';
        var mainPicture = 'PictureList[' + index + '].IS_MAIN';

        $(this).find('.picture-id').attr('name', pictureID);
        $(this).find('.cbxMainPic').parent().find('input').attr('name', mainPicture);

        if ($(this).hasClass('old-value')) {
            var deleted = 'PictureList[' + index + '].DELETED';

            $(this).find('.picture-deleted').attr('name', deleted);
        }
    });
}

function SetPicture($imgElement, file) {
    var reader = new FileReader();

    reader.onload = function (e) {
        $imgElement.attr('src', e.target.result).attr('title', file.name).addClass('set');
    };

    reader.readAsDataURL(file);
}

$('.btnAddPicture').click(function (e) {
    $('#productPicture').click();
});

$(document).off('.btnDeletePicture');
$(document).on('click', '.btnDeletePicture', function () {
    var $targetContent = $(this).parents('.picture-detail');

    if ($targetContent.hasClass('old-value')) {
        $targetContent.hide();
        $targetContent.find('.picture-deleted').val(true);
    } else {
        $targetContent.remove();
    }

    ResetPictureDetail();
});

$(document).off('#productPicture');
$(document).on('change', '#productPicture', function () {
    var fileList = $(this).prop('files');

    if (fileList.length > 0) {
        var html = '<div class="form-group col-lg-3 picture-detail">'
        + ' <input class="picture-id" type="hidden">'
        + '<div class="col-lg-12 text-right"> <i class="fa fa-remove error btnDeletePicture"></i></div>'
        + ' <div class="col-lg-12">'
        + ' <img src="/Images/no_image.png" title="Chưa có ảnh" class="display-picture" />'
        + ' </div>'
        + ' <div class="col-lg-12">'
        + ' <label class="picture-name short-text text-overflow">&nbsp;</label>'
        + ' <input class="cbxMainPic" type="checkbox" value="true"><input type="hidden" value="false">'
        + ' </div>'
        + ' </div>';

        for (var i = 0; i < fileList.length; i++) {
            if ($('.picture-detail').length > 0)
                $('.picture-detail:last').after(html);
            else
                $('.picture-list').append(html);

            var file = fileList[i];
            var $targetContent = $('.picture-detail:last');
            var pictureID = $targetContent.index() * -1;

            $targetContent.find('.picture-id').val(pictureID);
            $targetContent.find('.picture-name').text(file.name);

            var $imgElement = $targetContent.find('.display-picture');

            SetPicture($imgElement, file);
        }

        ResetPictureDetail();
    }
});

// END - Module picture
