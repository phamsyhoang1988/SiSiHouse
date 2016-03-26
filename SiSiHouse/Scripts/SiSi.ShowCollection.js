
$(document).ready(function () {

    // First load data
    $(function () {
        
    });
});

$(function () {
    var type = $('#hdnCollectionType').val();
    var html = '';

    function DisplayCollection(dataList) {

        for (var i = 0; i < dataList.length; i++) {

        }
    }

    function BindData(countItem) {
        SiSi.utility.getDataByAjax('/Show/List/', { type: type, countItem: countItem }, function (result) {
            if (result) {
                console.log(result);
            }
        });
    }

    $(window).scroll(function (e) {
        if ($('#products-content').length > 0 && ($(this).innerHeight() + $(this).scrollTop()) >= $('body').height()) {
            //BindData(0);
        }
    });

    BindData(0);
});

