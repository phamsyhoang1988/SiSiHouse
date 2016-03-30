
$(function () {
    $('#menu_button').click(function () {
        var $target = $(this);

        if ($target.hasClass('open')) {
            $('#menu_layer').hide();
            $target.removeClass('open');
            $('#menu_header').animate({ left: "-230px" });
            $('body').removeClass('menu-header-open');
        } else {
            $('#menu_layer').show();
            $target.addClass('open');
            $('#menu_header').animate({ left: "0" });
            $('body').addClass('menu-header-open');
        }
    });

    $('.has-sub-menu').on('click', function (e) {
        var $target = $(this).next('.sub-menu-content');

        $('.sub-menu-content.open').not($target).removeClass('open').stop(true).slideToggle('fast');

        if ($target.hasClass('open')) {
            $target.removeClass('open').stop(true).slideToggle('fast');
        } else {
            $target.addClass('open').stop(true).slideToggle('fast');
        }
    });

    $('#showSearchForm').on('click', function (e) {
        var $target = $('#search_form');

        if ($target.hasClass('active')) {
            $target.removeClass('active');
            $('#iPageContent, #ItxHomeBlocks').animate({ marginTop: 0 });


        } else {
            $target.addClass('active');
            $('#iPageContent, #ItxHomeBlocks').animate({ marginTop: "25px" });
        }

        $('#menu_button.open').click();
    });

    $('#menu_layer').on('click', function (e) {
        $('#menu_button.open').click();
    });
});
