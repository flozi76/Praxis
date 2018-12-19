var Dialog = (function() {
    // Set variables globally
    this.dialogItemId = null;

    function init() {
        console.log("init dialogs");
        initializeElements();
    }

    function initializeElements() {
        // Buttons
        $(".submit-buttons").on("click",
            function(e) {
                submitConfirmation(e);
            });
    }

    // Show confirmation or notification dialog. Makes post to controller and gets the 
    // partial view there. Sets html result for dialog.
    function showDialog(e, showConfirmUrl) {
        // Get info from data attributes.
        Dialog.dialogItemId = $(e.currentTarget).attr("data-item-id");
        var name = $(e.currentTarget).attr("data-item-name");

        var data = { id: Dialog.dialogItemId, name: name };
        var url = Duftfinder.getRootPath() + showConfirmUrl;

        $.post(url, data)
            .then(function(result) {
                console.log("showDialog succeeded");
                // Return result is html of dialog contents.
                // Set the dialog with the html result
                $("#dialog-modal-" + Dialog.dialogItemId).html(result);

                // Register the modal.
                $("#dialog-modal-" + Dialog.dialogItemId).modal("show");

                // Init the dialog in order for the buttons to work.
                init();

            }).fail(function(result) {
                console.log("showDialog failed");
                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError);
            });
    }

    // Click on OK of confirmation dialog.
    function submitConfirmation(e) {
        // Get id of element & action from submit buttons attribute.
        var id = $(e.currentTarget).attr("data-confirmation-id");
        var confirmationUrl = $(e.currentTarget).attr("data-confirmation-action");

        // Get data.
        var data = { id: id };
        var url = Duftfinder.getRootPath() + confirmationUrl;

        $.post(url, data)
            .done(function() {
                console.log("submitConfirmation succeeded");
                // Hide alert & reload page.
                Duftfinder.hideAlert();
                location.reload();
            })
            .fail(function(data) {
                console.log("submitConfirmation failed");
                // Show alert danger & appropriate error message.
                Duftfinder.showAlert(data.responseJSON.errorMessage);
            })
            .always(function() {
                // Hide the dialog.
                $("#dialog-modal-" + id).modal("hide");
            });
    }

    return {
        init: init,
        showDialog: showDialog
    };
})();

$(function() {
    Dialog.init();
});