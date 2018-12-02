var EssentialOil = (function () {

    // Define Urls for HttpPost on Controllers
    var displayUploadedPictureUrl = "/EssentialOil/DisplayUploadedPicture"; // IMPORTANT: This url MUST have a / as a prefix. Otherwise Controller won't be called.
    var confirmDeleteUrl = "/EssentialOil/ShowConfirmDelete";
    
    function init() {
        console.log("init essential oils");
        initializeElements();

        // Set last created or edited essential oil id.
        Duftfinder.lastEditedValueId = $("#lastEditedEssentialOilId").val();

        // Scroll to essential oil, when returned from create or edit.
        Duftfinder.scrollToValue();
    }

    function initializeElements() {
        // Highlight the active subnavigation item.
        Duftfinder.setActiveSubnavigationItem();

        // Buttons
        $("#upload-file-input").on("change", displayUploadedPicture);

        // Click on "Löschen" link.
        $("#essential-oils .delete-buttons").on("click", function (e) {
            Dialog.showDialog(e, confirmDeleteUrl);
        });

        // Click on "Bild enfernen" button.
        $("#essential-oil-form #remove-picture").on("click", function () {
            removePicture();
        });
    }

    // Display uploaded picture, after picture is selected from file system.
    function displayUploadedPicture() {
        // Show loader.
        $("#essential-oil-form #loader-container").removeClass("display-none");

        // Get form data
        var formData = new FormData($("#essential-oil-form")[0]);
        var url = Duftfinder.getRootPath() + displayUploadedPictureUrl;
        $.ajax({
            url: url,
            type: 'POST',
            beforeSend: function() { },
            success: function(result) {
                console.log("displayUploadedPicture succeeded");
                // Show uploaded picture in container, hide error message and loader.
                $("#picture-container").attr("src", result.ImageDisplayString);
                $("#fake-file-input").val(result.FileName);

                // Show "Bild entfernen" button.
                $("#essential-oil-form #remove-picture-container").removeClass("display-none");

                Duftfinder.hideAlert();

                $("#essential-oil-form #loader-container").addClass("display-none");
            },
            error: function(data) {
                console.log("displayUploadedPicture failed");

                // Show custom or default error message.
                var errorMessage = Resources.Error_FileCannotBeUploaded;
                if (data.responseJSON !== null && data.responseJSON.errorMessage !== null) {
                    errorMessage = data.responseJSON.errorMessage;
                }

                console.log(errorMessage);

                // Show error message and remove picture & file name.
                Duftfinder.showAlert(errorMessage);
                $("#picture-container").attr("src", "");
                $("#fake-file-input").val("");
            },
            data: formData,
            cache: false,
            contentType: false,
            processData: false
        });
    }

    // Remove picture from preview & from model.
    function removePicture() {
        // Clear picture preview.
        $("#picture-container").attr("src", "");
        $("#fake-file-input").val("");

        // Remove picture from model.
        $("#essential-oil-form #picture-data-as-string").val(null);
        $("#essential-oil-form #picture-file-name").val(null);

        // Hide "Bild entfernen" button.
        $("#essential-oil-form #remove-picture-container").addClass("display-none");
    }

    return {
        init: init
    };
})();

$(function() {
    EssentialOil.init();
});