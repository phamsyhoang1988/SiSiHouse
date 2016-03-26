
$(function () {
    SiSi.utility.getDataByAjax('/Show/List/' + $('#hdnCollectionType').val(), null, function (result) {
        if (result) {
            console.log('vao');
        }
    });
});
