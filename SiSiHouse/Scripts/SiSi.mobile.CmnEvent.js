
$(function () {
    $('#menu_button').click(function () {
        var $target = $(this);

        if ($target.hasClass('open')) {
            $('#menu_layer').hide();
            $target.removeClass('open');
            $('#menu_header').animate({ left: "-230px" });
        } else {
            $('#menu_layer').show();
            $target.addClass('open');
            $('#menu_header').animate({ left: "0" });
        }
    });

    $('.has-sub-menu').on('click', function (e) {
        $(this).next().stop(true).slideToggle('fast');
    });

    $('#showSearchForm').on('click', function (e) {
        var $target = $('#search_form');

        if ($target.hasClass('active')) {
            $target.removeClass('active');
        } else {
            $target.addClass('active');
        }
    });
});

//$(document).ready(function () {
    
//});
