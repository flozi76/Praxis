var UserAdmin = (function () {

    // Define Urls for HttpPost on Controllers
    var confirmDeleteUrl = "/UserAdmin/ShowConfirmDelete";
    var notifyAccountNotVerifiedUrl = "/UserAdmin/ShowNotifyAccountNotVerified"; // IMPORTANT: This url MUST have a / as a prefix. Otherwise Controller won't be called.
    var notifyEmailWillBeSentToUserUrl = "/UserAdmin/ShowNotifyEmailWillBeSentToUser"; // IMPORTANT: This url MUST have a / as a prefix. Otherwise Controller won't be called.

    function init() {
        console.log("init user admin");
        initializeElements();
    }

    function initializeElements() {
        // Click on "Löschen" link.
        $("#user-admin .delete-buttons").on("click", function (e) {
            Dialog.showDialog(e, confirmDeleteUrl);
        });

        // Click on "vom Admin bestätigt" checkbox.
        $("#user-admin-form #cb-is-confirmed").on("click", function (e) {
            showNotifiyUserNotVerifiedDialog(e);
        });
    }

    // Show dialog, that notifies the admin, that the user has not verified his account, when the admin clickes the checkbox.
    function showNotifiyUserNotVerifiedDialog(e) {
        var isAccountVerified = $('#user-admin-form #cb-is-account-verified').is(":checked");
        var isConfirmedChecked = $('#user-admin-form #cb-is-confirmed').is(":checked");

        // Set attr values for checkbox, as they can't be set directly on the checkbox element.
        var id = $("#hidden-checkbox-values").attr("data-item-id");
        var name = $("#hidden-checkbox-values").attr("data-item-name");
        $(e.currentTarget).attr("data-item-id", id);
        $(e.currentTarget).attr("data-item-name", name);

        // Show dialog.
        if (!isAccountVerified && isConfirmedChecked) {
            // Show notification if account is not verified & admin checked the isConfirmed checkbox.
            Dialog.showDialog(e, notifyAccountNotVerifiedUrl);
        } else if (isConfirmedChecked) {
            // Show notification if admin checked the isConfirmed checkbox to indicate that email will be sent to user.
            Dialog.showDialog(e, notifyEmailWillBeSentToUserUrl);
        }
    }

    return {
        init: init
    };
})();

$(function () {
    UserAdmin.init();
});