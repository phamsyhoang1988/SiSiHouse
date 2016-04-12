var Constant = {
    MESSAGE: {
        CONFIRM: 'Vui lòng xác nhận lại thao tác thay đổi dữ liệu.',
        CONFIRM_CREATE_RETAIL: 'Bạn có chắc chắn muốn tạo hóa đơn bán lẻ không ?',
        CONFIRM_DELETE_PRODUCT: 'Bạn có chắc chắn muốn xóa "{0}" không ?',
        CONFIRM_DELETE_BILL: 'Bạn có chắc chắn muốn xóa hóa đơn "{0}" không ?',
        CONFIRM_UNDO_BILL: 'Bạn có chắc chắn muốn trả lại hóa đơn "{0}" không ?',
        CREATE_SUSSCES: 'Đã tạo hóa đơn bán lẻ thành công.',
        CONNECT_TIMEOUT: 'Phiên làm việc đã kết thúc. Vui lòng đăng nhập để tiếp tục.',
        ERROR: {
            UPDATE_DATA: 'Có lỗi trong quá trình xử lý dữ liệu.',
            REQUIRED: 'Vui lòng nhập ',
            REQUIRED_SELECT: 'Vui lòng chọn ',
            EXIST_COLOR_SIZE: 'Bạn đã chọn trùng màu và size. Vui lòng chọn màu và size khác.',
            QUANTITY: 'Vui lòng không nhập số lượng bán vượt quá số lượng còn trong kho.',
            EXIST_COLOR: 'Màu sắc đã được chọn rồi. Vui lòng chọn màu khác.',
        }
    },
    DIALOG: {
        SUSSCES: "<i class='fa fa-check-circle'></i>",
        WARNING: "<i class='fa fa-warning'></i>"
    },
    MIN_YEAR: 1753,
    MAX_YEAR: 9999,
    ERR_FORMAT: ' sai định dạng.',
    DATE_FORMAT: 'mm/dd/yyyy',
    ERR_CONNECT_TIMEOUT: 'Phiên làm việc đã kết thúc. Vui lòng đăng nhập để tiếp tục',
    URL_REDIRECT_TIMEOUT: '/SiSi/GetIn',
    ROLE: {
        USER: 0,
        ADMIN: 1
    },
    STATUS: {
        WAITING: '0',
        SELLING: '1',
        SALE_OFF: '2',
        OUT_OF_STOCK: '3',
    },
    REGX: /[^a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\_\^\@@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]/g
}

$(document).ready(function () {
    $(window).scroll(function (e) {
        // show hide header
        if ($(this).scrollTop() > 155) {
            $('.shop-header').addClass('unfolded-header');
            $('#iPageContent').addClass('scroll-over-top');
        } else {
            $('.shop-header').removeClass('unfolded-header');
            $('#iPageContent').removeClass('scroll-over-top');
        }

        // show hide button scroll to top
        if ($(this).scrollTop() > 200) {
            $('#scrollTop').fadeIn();
        } else {
            $('#scrollTop').fadeOut();
        }
    });

    $('#scrollTop').click(function () {
        $("html, body").animate({ scrollTop: 0 }, 300);
        return false;
    });

    $(".datepicker-years").datepicker({
        format: "yyyy",
        viewMode: "years",
        minViewMode: "years",
        language: 'vi',
        autoclose: true
    });

    $(".datepicker-months").datepicker({
        format: "yyyy/mm",
        viewMode: "months",
        minViewMode: "months",
        language: 'vi',
        autoclose: true
    });

    $(".datepicker-full, .input-daterange").datepicker({
        format: "yyyy/mm/dd",
        language: 'vi',
        autoclose: true
    });

    // Event show hide search form
    $(document).on('click', '.show-hide-search', function (e) {
        $('.search-form').stop(true).slideToggle('fast');

        if ($('.show-hide-search i').hasClass('fa-chevron-circle-up')) {
            $('.show-hide-search i').removeClass('fa-chevron-circle-up').addClass('fa-chevron-circle-down');
        } else {
            $('.show-hide-search i').removeClass('fa-chevron-circle-down').addClass('fa-chevron-circle-up');
        }
    });

    // Event show hide search form
    $('#searchAll').on('click', function (e) {
        var searchValue = $(this).siblings('#txtSearchAll').val();

        if (searchValue.trim().length > 0) {
            window.location.href = '/Show/Search/' + searchValue.trim();
        }
    });

    $('#txtSearchAll').on('focus', function (e) {
        $(document).bind('keypress', function (e) {
            var code = e.keyCode || e.which;
            if (code == 13) { //Enter keycode
                $('#searchAll').click();
            }
        });
    });

    $('#txtSearchAll').on('blur', function (e) {
        $(document).unbind('keypress');
    });

    $('#contactUs').on('click', function (e) {
        $('#mdContactUs').modal({ backdrop: true });
    });

    $('#closeContact').on('click', function (e) {
        $('#mdContactUs').modal('toggle');
    });
});

// Set window name
function SetWindowName(name) {
    sessionStorage.setItem('WindowName', name);
    window.name = name;
}

// Event auto start on load
$(function () {
    // set cookie when change screen
    $(window).on("beforeunload", function (e) {
        var name = sessionStorage.getItem('WindowName');
        if (name != null) {
            document.cookie = 'WindowName=' + name + '; path=/';
        }
    });
});

// control input on date time picker
$(document).off('.datepicker-full, .input-daterange');
$(document).on('keydown', '.datepicker-full, .input-daterange', function (e) {
    var charCode = event.keyCode;
    var handled = true;

    if (charCode > 47 && 58 > charCode && charCode && !event.shiftKey) { // only numeric
        handled = false;
    } else if (charCode == 186 && charCode && event.shiftKey) { // char ':'
        handled = false;
    } else {
        switch (charCode) {
            case 8: // backspace
                handled = false;
                break;
            case 9: // tab
                handled = false;
                break;
            case 13: // enter
                handled = false;
                break;
            case 37: // left
                handled = false;
                break;
            case 39: // right
                handled = false;
                break;
            case 46: // delete
                handled = false;
                break;
            case 191: // char '/'
                handled = false;
                break;
            case 116: // f5
                handled = false;
                break;
            case 65: // ctrl + a
                if (event.ctrlKey || event.metaKey) {
                    handled = false;
                    break;
                }
            case 67: // ctrl + c
                if (event.ctrlKey || event.metaKey) {
                    handled = false;
                    break;
                }
            case 86: // ctrl + v
                if (event.ctrlKey || event.metaKey) {
                    handled = false;
                    break;
                }
        }
    }

    if (handled) {
        event.preventDefault();
        event.stopPropagation();
    }
});
