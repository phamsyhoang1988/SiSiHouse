/*!
 * File: SiSi.utility.js
 * Company: ***
 * Copyright © 2014 i-Enter Asia. All rights reserved.
 * Project: SiSiHouse
 * Author: HoangPS
 * Created date: 2015/04/15
 */

var SiSi = SiSi || {};

SiSi.utility = (function () {
    function replaceAllMoney() {
        $('.money').each(function () {
            var data = this.value;
            if (data.length > 0) {
                this.value = data.replace(/,/g, '');
            }
        });
    }

    // Format money in textbox to string with symbol ','
    function formatMoney(obj) {
        obj = obj != null ? obj : '.money';

        $(obj).each(function () {
            var value = this.value;

            if (value.length > 0) {
                var money = convertIntToMoney(this.value);

                this.value = money;
            }
        });
    }

    // Format money in label to string with symbol ','
    function formatMoneyLabel() {
        $('label.lbl-money, label.money').each(function () {
            var data = convertMoneyToInt($(this).text(), true);
            var money = convertIntToMoney(data);

            $(this).text(money);
        });
    }

    // Format integer to money string with symbol ','
    function convertIntToMoney(input) {
        var valueArr = input.toString().split('.');
        var beforeDot = SiSi.utility.convertMoneyToInt(valueArr[0]);
        var result = beforeDot.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");

        if (valueArr.length > 1) {
            result += SiSi.utility.validPositiveNumeric(valueArr[1]) ? '.' + valueArr[1] : '';
        }

        return result;
    }

    // Format money string to integer
    function convertMoneyToInt(input, canNegative) {
        if (input.length == 0)
            return 0;

        var strValue = input.replace(/,/g, '');
        var intValue = validPositiveNumeric(strValue) ? parseInt(strValue) : 0;

        if (typeof (canNegative) !== "undefined" && canNegative == true)
            intValue = validNegativeNumeric(strValue) ? parseInt(strValue) : 0;

        return intValue;
    }

    // Check positive number only
    function validPositiveNumeric(value) {
        var data = value.trim().length == 0 ? '0' : value;

        if (value.indexOf('-') !== -1 || !$.isNumeric(value)) {
            return false;
        }
        return true;
    }

    // Check isvalid date
    // Return true if valid, false if invalid
    function isValidDate(date) {
        var bits = date.split('/');
        var d = new Date(bits[0], bits[1] - 1, bits[2]);
        return d && (d.getMonth() + 1) == bits[1] && d.getDate() == Number(bits[2]);
    }

    // Check validation of date field
    // date is input data
    // control is field name
    // return invalid message if invalid, null if valid
    function validDate(date, format, control) {
        if (date.length > 0) {
            if (format == 'yyyy/mm') {
                date += '/01';
            }

            if (!/^[0-9]{4}\/[0-9]{1,2}\/[0-9]{1,2}/.test(date) || !isValidDate(date)) {
                return (control + Constant.ERR_FORMAT);
            }

            var year = parseInt(date.split('/')[0]);

            if (Constant.MIN_YEAR > year || year > Constant.MAX_YEAR) {
                if (format == 'yyyy/mm') {
                    return control + ' giới hạn trong khoảng từ 1753/01 tới 9999/12';
                }
                return control + ' giới hạn trong khoảng từ 1753/01 tới 9999/12';
            }
        }

        return null;
    }

    // Compare startDate with endDate
    // Return true if valid, false if invalid
    function compareDate(startDate, endDate, format) {
        var start = $.datepicker.formatDate('yy/mm/dd', new Date(startDate));
        var end = $.datepicker.formatDate('yy/mm/dd', new Date(endDate));

        if (format == 'yyyy/mm') {
            start = $.datepicker.formatDate('yy/mm/dd', new Date(startDate + '/01'));
            end = $.datepicker.formatDate('yy/mm/dd', new Date(endDate + '/01'));
        }

        var valid = true;
        if (start > end) {
            valid = false;
        }

        return valid;
    }

    // Compare startDate with endDate
    // Return true if valid, false if invalid
    function compareDateRange(startDate, endDate, rangeMonth) {
        var start = new Date(startDate + '/01');
        var end = new Date(endDate + '/01');

        var valid = true;

        if ((end.getMonth() + end.getFullYear() * 12 - start.getMonth() - start.getFullYear() * 12) > rangeMonth) {
            valid = false;
        }

        return valid;
    }

    // Check range of a duration time
    function validateRangeYear(startYear, endYear, rangeYear) {
        var arrStart = startYear.split('/');
        var arrEnd = endYear.split('/');
        var sYear = parseInt(arrStart[0]);
        var sMonth = parseInt(arrStart[1]);
        var eYear = parseInt(arrEnd[0]);
        var eMonth = parseInt(arrEnd[1]);

        if ((eYear - sYear) * 12 + (eMonth - sMonth) > rangeYear) {
            return false;
        }
        return true;
    }

    // Show client error message on the top of page
    function showClientError(errMessage, position) {
        position = typeof (position) === 'undefined' ? '#title' : position;

        $('.validation-summary-errors').remove();
        var html = '<div class="validation-summary-errors"><ul>';

        for (var i = 0; i < errMessage.length; i++) {
            html += '<li class="first last">' + errMessage[i] + '</li>';
        }
        html += '</ul></div>';
        $(position).after(html);
        $(document).scrollTop(0);

        return;
    }

    // Remove all error validation on page
    function removeValidationError() {
        $('.validation-summary-errors').remove();
        $("label.label-validation-error").removeClass("label-validation-error");
    }

    // Encode html to string
    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }

    // Decode string to html
    function htmlDecode(value) {
        return $('<div/>').html(value).text();
    }

    // get data from server by Ajax GET. Return result
    function getDataByAjax(async, url, param, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            dataType: 'json',
            cache: false,
            async: async,
            success: function (data) {
                callback(data);
            },
            error: function (err) {
                callback(false);
            }
        });
    }

    // get data from server by Ajax GET with param is serialize to check valid data. Return result
    function checkDataExistByAjax(url, param) {
        var result;

        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            dataType: 'json',
            traditional: true,
            async: false,
            cache: false,
            success: function (data) {
                result = data;
            },
            error: function (err) {
                if (err.status == 419) //419: Authentication Timeout
                    showInformationDialog(Constant.DIALOG.WARNING, Constant.ERR_CONNECT_TIMEOUT, Constant.URL_REDIRECT_TIMEOUT);
                return;
            }
        });
        return result;
    }

    // get image data by ajax
    function getImageByAjax(url, param, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            contentType: 'application/json',
            dataType: 'json',
            cache: false,
            success: function (data) {
                callback(data);
            },
            error: function (err) {
                if (err.status == 419) //419: Authentication Timeout
                    showInformationDialog(Constant.DIALOG.WARNING, Constant.ERR_CONNECT_TIMEOUT, Constant.URL_REDIRECT_TIMEOUT);
                callback(null);
            }
        });
    }

    function ShowConfirmDialog(message, callback) {
        BootstrapDialog.show({
            title: '<i class="fa fa-warning"></i> Cảnh báo',
            message: message,
            closable: false,
            buttons: [{
                label: 'Đồng ý',
                cssClass: 'btn dark',
                hotkey: 13,
                action: function (dialog) {
                    dialog.close();
                    callback(true);
                }
            },
            {
                label: 'Hủy',
                cssClass: 'btn light',
                action: function (dialog) {
                    dialog.close();
                    callback(false);
                }
            }]
        });
    }

    function showInformationDialog(dialogType, message, urlRedirect, callback) {
        BootstrapDialog.show({
            title: dialogType,
            message: message,
            closable: false,
            buttons: [{
                label: 'OK',
                hotkey: 13,
                cssClass: 'btn dark',
                action: function (dialog) {
                    if (typeof (urlRedirect) !== 'undefined' && urlRedirect !== null) {
                        window.location.href = urlRedirect;
                    }

                    if (typeof (callback) !== 'undefined') {
                        callback(null);
                    }
                    dialog.close();
                }
            }]
        });
    }

    function nvl(targetString, defaultString) {
        var value = targetString;

        if (value == null) {
            value = defaultString;
        }

        return value;
    }

    return {
        replaceAllMoney: replaceAllMoney,
        formatMoney: formatMoney,
        formatMoneyLabel: formatMoneyLabel,
        convertMoneyToInt: convertMoneyToInt,
        convertIntToMoney: convertIntToMoney,
        validDate: validDate,
        validateRangeYear: validateRangeYear,
        compareDate: compareDate,
        compareDateRange: compareDateRange,
        showClientError: showClientError,
        htmlEncode: htmlEncode,
        htmlDecode: htmlDecode,
        getDataByAjax: getDataByAjax,
        checkDataExistByAjax: checkDataExistByAjax,
        getImageByAjax: getImageByAjax,
        removeValidationError: removeValidationError,
        ShowConfirmDialog: ShowConfirmDialog,
        showInformationDialog: showInformationDialog,
        nvl: nvl,
        validPositiveNumeric: validPositiveNumeric
    };
}());
