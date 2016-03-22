
$(function () {
    $(document).off('.numeric');
    $(document).on('focus', '.numeric', function () {
        $(".numeric").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
        // Integer
        $(".integer").css({ "ime-mode": "disabled" }).numeric({ decimal: false });
        // A positive number
        $(".positive").css({ "ime-mode": "disabled" }).numeric({ negative: false });
        // Positive integer
        $(".positive-integer").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    // Event focus in money field
    $(document).off('.money');
    $(document).on('focus', '.money', function () {
        $(this).css({ "ime-mode": "disabled" }).numeric({ decimal: '.', negative: false, decimalPlaces: 2 }).val(this.value.replace(/,/g, ''));
    });

    $(document).off('.money');
    $(document).on('focusout', '.money', function () {
        $(this).val(SiSi.utility.convertIntToMoney(this.value));
    });

    $(document).off('.decimal');
    $(document).on('focus', '.decimal', function () {
        $(this).css({ "ime-mode": "disabled" }).numeric({ decimal: '.', negative: false, decimalPlaces: 2 });
    });
});
