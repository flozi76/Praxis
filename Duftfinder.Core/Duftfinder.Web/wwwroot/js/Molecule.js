var Molecule = (function () {

    // Define Urls for HttpPost on Controllers 
    var confirmDeleteUrl = "/Molecule/ShowConfirmDelete";

    function init() {
        console.log("init molecules");
        initializeElements();

        // Set last created or edited molecule id.
        Duftfinder.lastEditedValueId = $("#lastEditedMoleculeId").val();

        // Scroll to effect, when returned from create or edit.
        Duftfinder.scrollToValue();
    }

    function initializeElements() {
        // Click on "Löschen" link.
        $("#molecules .delete-buttons").on("click", function (e) {
            Dialog.showDialog(e, confirmDeleteUrl);
        });
    }

    return {
        init: init
    };
})();

$(function () {
    Molecule.init();
});