$(function () {
    $('.carousel').carousel({
        interval: 5000
    });


    var tb = $('.tooltip-btn');

    tb.on('mouseenter tap', function () {
        if ($('#tooltip-text').length == 0) {
            var tooltipText = $(this).data("tiptext");
            $(this).append('<div id="tooltip-text" class="tooltip-text sd-shadow">' + tooltipText + '</div>').css("z-index", "9999");
        }
    });

    tb.on('mouseleave tap', function () {
        if ($('#tooltip-text').length > 0) {
            $('#tooltip-text').remove();
            $(this).css("z-index", "initial");
        }
    });
});