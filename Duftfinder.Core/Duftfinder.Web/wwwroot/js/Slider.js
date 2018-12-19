var Slider = (function() {
    function init() {
        console.log("init sliders");
        initializeElements();
    }

    function initializeElements() {
        // Sliders
        $(".slider").bootstrapSlider();

        // Sets class for read-only slider in #essential-oil-details.
        $("#essential-oil-details .slider").addClass("col-md-12");
        $("#essential-oil-details .slider").addClass("col-sm-12");
        $("#essential-oil-details .slider").addClass("col-12");


        $(".slider").on("change",
            function(e) {
                sliderChanged(e);
            });
    }

    function sliderChanged(e) {
        // Get id of slider from attribute.
        var id = $(e.currentTarget).attr("id").replace("slider-", "");
        var sliderId = "#slider-" + id;

        $(sliderId).bootstrapSlider({
            formatter: function(value) {
                console.log(sliderId + " has value: " + value);
            }
        });
    }

    return {
        init: init
    };
})();

$(function() {
    Slider.init();
});