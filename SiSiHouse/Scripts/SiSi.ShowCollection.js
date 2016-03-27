

$(function () {
    var type = $('#hdnCollectionType').val();
    var totalProduct = $('#hdnTotalProduct').val();
    var htmlTempProduct = '<li class="product visible-c">'
        + '<a class="product-image" href="{link}">'
        + '<img class="lazy img-responsive main" src="{picture_1}">'
        + '<img class="lazy img-responsive auxiliar" src="{picture_2}">'
        + '</a>'
        + '</li>';
    var readyScroll = true;

    function DisplayCollection(dataList) {
        var html = '';

        for (var i = 0; i < dataList.length; i++) {
            html += htmlTempProduct.replace('{link}', '/Show/Item/' + dataList[i][0]).replace('{picture_1}', dataList[i][1]).replace('{picture_2}', dataList[i][2]);
        }

        if ($('ul#product-list li.product').length > 0) {
            // add more product
            $('ul#product-list li.product:last').after(html);
        } else {
            //add first list of product
            $('ul#product-list').append(html);
        }

        readyScroll = true;
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
            BindData(countProduct);
        }
    });

    BindData(0);
});

