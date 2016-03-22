
$(document).ready(function () {
    SiSi.utility.formatMoney();

    //if ($('#ProductInfo_PRODUCT_ID').val() !== '0') {
    //    SetMoneySign();
    //    SetRealPrice();
    //    ResetProductColor();
    //}
});

$('.search-customer').click(function () {
    var url = '/SiSi/Select?callback=SetCustomer';
    url += '&TB_iframe=true&modal=true&height=600&width=1200';
    tb_show(null, url, false);
});

function SetCustomer(res) {
    if (typeof (res) !== 'undefined' && res !== null) {
        $('#CustomerInfo_USER_ID').val(res[0].CUSTOMER_ID);
        $('#CustomerInfo_FULL_NAME').val(res[0].FULL_NAME);
        $('#CustomerInfo_MOBILE').val(res[0].MOBILE);
        $('#CustomerInfo_EMAIL').val(res[0].EMAIL);
        $('#CustomerInfo_PRIVATE_PAGE').val(res[0].PRIVATE_PAGE);
        $('#CustomerInfo_ADDRESS').val(res[0].ADDRESS);
    }
}