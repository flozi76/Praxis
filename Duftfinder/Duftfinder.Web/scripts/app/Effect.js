var Effect = (function () {

    // Define Urls for HttpPost on Controllers 
    var confirmDeleteUrl = "/Effect/ShowConfirmDelete";

    function init() {
        console.log("init effects");
        initializeElements();

        // Set last created or edited effect id.
        Duftfinder.lastEditedValueId = $("#lastEditedEffectId").val();

        // Scroll to effect, when returned from create or edit.
        Duftfinder.scrollToValue();
    }

    function initializeElements() {
        // Highlight the active subnavigation item.
        Duftfinder.setActiveSubnavigationItem();

        // Click on "Löschen" link.
        $("#effects .delete-buttons").on("click", function (e) {
            Dialog.showDialog(e, confirmDeleteUrl);
        });
    }

    return {
        init: init
    };
})();

$(function () {
    Effect.init();
});