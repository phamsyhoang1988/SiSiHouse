
$(function () {
    var type = $('#hdnCollectionType').val();
    var totalProduct = $('#hdnTotalProduct').val();
    var htmlTempProduct = '<li class="product visible-c">'
        + '<a class="product-image" href="{link}" title="{title}">'
        + '<img class="lazy img-responsive main" src="{picture_1}">'
        + '<img class="lazy img-responsive auxiliar" src="{picture_2}">'
        + '</a>'
        + '</li>';
    var readyScroll = true;

    function SetViewType() {
        var viewType = localStorage.getItem('viewtype');

        if (typeof(viewType) != 'undefined') {
            $('a.view-type').removeClass('selected');

            if (viewType == '2') {
                $('#two-columns a').addClass('selected');
                $('#products-content').removeClass('four');
            } else {
                $('#four-columns a').addClass('selected');
                $('#products-content').addClass('four');
            }
        }
    }

    function DisplayCollection(dataList) {
        var html = '';

        for (var i = 0; i < dataList.length; i++) {
            html += htmlTempProduct.replace('{link}', '/Show/Item/' + dataList[i][0]).replace('{title}', dataList[i][1]).replace('{picture_1}', dataList[i][2]).replace('{picture_2}', dataList[i][3]);
        }

        if ($('ul#product-list li.product').length > 0) {
            // add more product
            $('ul#product-list li.product:last').after(html);
        } else {
            //add first list of product
            $('ul#product-list').append(html);
        }

        readyScroll = true;
        $('#loadingCollection').removeClass('show');
    }

    function BindData(countItem) {
        SiSi.utility.getDataByAjax('/Show/List/', { type: type, countItem: countItem }, function (result) {
            if (result && result.data.length > 0) {
                DisplayCollection(result.data);
            }
        });
    }

    function element_in_scroll(elem) {
        var docViewTop = $(window).scrollTop();
        var docViewBottom = docViewTop + $(window).height();

        var elemTop = $(elem).offset().top;
        var elemBottom = elemTop + $(elem).height();

        return ((elemBottom <= docViewBottom) && (elemTop >= docViewTop));
    }

    $(window).scroll(function (e) {
        if (!readyScroll) {
            return false;
        }

        var wintop = $(window).scrollTop(), docheight = $(document).height(), winheight = $(window).height();
        var scrolltrigger = 0.95;
        var countProduct = $('ul#product-list li.product').length;

        if (totalProduct > countProduct &&  (wintop / (docheight - winheight)) > scrolltrigger && readyScroll) {
            readyScroll = false;
            $('#loadingCollection').addClass('show');

            BindData(countProduct);
        }
    });

    $('a.view-type').click(function () {
        if (!$(this).hasClass('selected')) {
            $('a.view-type').removeClass('selected');
            $(this).addClass('selected');

            var columns = $(this).data('columns');

            if (columns == '2') {
                $('#products-content').removeClass('four');
                localStorage.setItem('viewtype', '2');
            } else {
                $('#products-content').addClass('four');
                localStorage.setItem('viewtype', '4');


                var countProduct = $('ul#product-list li.product').length;

                if (6 > countProduct) {
                    BindData(countProduct);
                }
            }
        }
    });

    SetViewType();
    BindData(0);
});
