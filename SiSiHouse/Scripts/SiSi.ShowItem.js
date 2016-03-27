

$(function () {
    $('#facebook').click(function () {
        window.open('http://www.facebook.com/sharer.php?u=' + window.location.href, 'SiSi House', 'width=700, height=700');
        return false;
    });

    $('#product-image-thumbs img').click(function () {
        $('#product-image-thumbs li').removeClass('active')
        $(this).parent().addClass('active');

        var index = $(this).data('index');

        $('html, body').animate({
            scrollTop: $('img#' + index).offset().top
        }, 1000);
    });
});
