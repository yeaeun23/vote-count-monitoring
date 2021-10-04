(function ($) {
    $.fn.extend({
        jqbar: function (options) {
            var settings = $.extend({ value: 100 }, options);

            return this.each(function () {
                $(this).append("<span class='bar-level'></span>");

                var progressBar = $(this).find(".bar-level").attr("data-value", settings.value);
                progressBar.css({ height: 10, background: "#f60" });
                progressBar.animate({ width: settings.value + "%", duration: 500 });
            });
        }
    });
})(jQuery);
