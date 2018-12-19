var SearchEssentialOil = (function() {

    // Define Urls for HttpPost on Controllers 
    var getEssentialOilNamesUrl = "/SearchEssentialOil/GetEssentialOilNames";
    var searchEssentialOilUrl = "/SearchEssentialOil/SearchEssentialOil";
    var essentialOilDetailsUrl = "/SearchEssentialOil/EssentialOilDetails";
    var searchUrl = "/SearchEssentialOil/Search";

    function init() {
        console.log("init search essential oil");
        initializeElements();
    }

    function initializeElements() {
        // Initialize autocomplete and the required data.
        loadEssentialOilNames();

        // Click on "Search" button.
        $("#search-essential-oil #search-essential-oil-button").on("click",
            function(e) {
                showEssentialOilSearchResults(e);
            });

        // Click on "Reset Search" button.
        $("#search-essential-oil #reset-search-essential-oil-button").on("click",
            function(e) {
                resetSearch();
            });

        // Click on "X" button.
        $("#clear-search").click(function(e) {
            clearSearchInput();
        });
    }

    function initializeElementsForshowEssentialOilSearchResults() {
        // Click on "Details" link.
        $("#search-essential-oil .details-buttons").on("click",
            function(e) {
                showEssentialOilDetails(e);
            });
    }

    function initializeElementsForShowEssentialOilDetails() {
        // Initialize the slider in order for the sliders to show.
        Slider.init();

        // IMPORTANT: stuff that changes here, also has to change in window.onpopstate = function(..)
        // Click on "Zurück" button in EssentialOilDetails.
        $("#essential-oil-details #search-essential-oil-back-button").click(function(e) {
            // e.preventDefault() is necessary in order to prevent the window.onpopstate to be fired after the custom "Zurück" button was clicked.
            e.preventDefault();
            console.log("Click on 'Zurück' button in EssentialOilDetails.");
            showEssentialOilSearch();
        });


        // Workaround to implement the same functionality when the browser "Zurück" button is clicked, as when the custom "Zurück" button
        // in the Details page is clicked. Workaround is necessary because EssentialOilDetails page & EssentialOilSearchResults are partial views.
        // "Click on 'Zurück' button in browser."
        window.onpopstate = function() {
            // Check is necessary in order to prevent the showEssentialOilSearch to be fired if other navigation stuff is done. (e.g. click on "Details" in result list)
            if ($("#essential-oil-details").length !== 0) {
                console.log("Click on 'Zurück' button in browser.");
                showEssentialOilSearch();
            }
        };
    }

    function loadEssentialOilNames() {
        // Get all essential oil names from controller.
        var url = Duftfinder.getRootPath() + getEssentialOilNamesUrl;
        $.get(url)
            .then(function(result) {
                console.log("loadEssentialOilNames succeeded");
                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                var essentialOilNames = result;

                // Initialize autocomplete with all the essential oil names.
                $("#search-essential-oil-text").autocomplete({
                    source: essentialOilNames
                });
            }).fail(function(result) {
                console.log("loadEssentialOilNames failed");
                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError +
                    " " +
                    Resources.Error_SearchDataCouldNotBeLoaded);
            });
    }

    // Show essential oil search results. Makes post to controller and gets the 
    // partial view there. Sets html result for the search result partial view.
    function showEssentialOilSearchResults(e) {
        // Prevents the default submit.
        e.preventDefault();

        showLoader();

        // Get the data from the form.
        var data = $("#search-essential-oil-form").serialize();
        var url = Duftfinder.getRootPath() + searchEssentialOilUrl;

        $.post(url, data)
            .then(function(result) {
                console.log("showEssentialOilSearchResults succeeded");
                // Return result is html of contents of PartialView _EssentialOilSearchResults.

                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                // Set the result container with the html result
                $("#essential-oil-search-results-container").html(result);

                initializeElementsForshowEssentialOilSearchResults();

                // Only scroll to essential oil, when returned from EssentialOilDetails.
                Duftfinder.scrollToValue();

            }).fail(function(result) {
                console.log("showEssentialOilSearchResults failed");
                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError);
            }).always(function(result) {
                // Loses focus of input. Is necessary for when enter is pressed.
                $("#search-essential-oil #search-essential-oil-text").blur();
            });
    }

    // Show essential oil details. Makes post to controller and gets the 
    // partial view there. Sets html result for the details partial view.
    function showEssentialOilDetails(e) {
        // The id of the clicked essential oil.
        Duftfinder.lastEditedValueId = e.currentTarget.id;

        // Get the data from the form.
        var data = $("#search-essential-oil-form").serialize();

        // Add the essential oil id to the data in order to pass it to the controller.
        data = data + "&essentialOilId=" + Duftfinder.lastEditedValueId;

        var url = Duftfinder.getRootPath() + essentialOilDetailsUrl;

        $.post(url, data)
            .then(function(result) {
                console.log("showEssentialOilDetails succeeded");

                // Return result is html of contents of PartialView _EssentialOilDetails.
                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                // Set the result container with the html result
                $("#search-essential-oil").html(result);

                initializeElementsForShowEssentialOilDetails();

            }).fail(function(result) {
                console.log("showEssentialOilDetails failed");

                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError);
            });
    }

    // Show essential oil search. Makes post to controller and gets the 
    // partial view there. Sets html result for the details partial view.
    function showEssentialOilSearch() {
        // Get the data from the form.
        var data = $("#search-essential-oil-hidden-form").serialize();

        var url = Duftfinder.getRootPath() + searchUrl;

        $.post(url, data)
            .then(function(result) {
                console.log("showEssentialOilSearch succeeded");

                // Return result is html of contents of PartialView _EssentialOilDetails.
                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                // Set the result container with the html result
                $("#search-essential-oil").html(result);

                // Initialize the elements in EssentialOilSearch.
                initializeElements();

                // Trigger search button, in order to show the search results after navigation back.
                $("#search-essential-oil #search-essential-oil-button").trigger("click");
            }).fail(function(result) {
                console.log("showEssentialOilSearch failed");

                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError);
            });
    }


    function showLoader() {
        // Show loader according to whether result is already displayed or not.
        if ($("#essential-oil-search-results-header").length === 0) {
            $("#essential-oil-search-results-container")
                .append("<div id=\"loader-container\"><div class=\"loader\"></div></div>");
        } else {
            $("#essential-oil-search-results-header")
                .append("<div id=\"loader-container\" class=\"margin-top-small\"><div class=\"loader\"></div></div>");
        }
    }

    function resetSearch() {
        // Reset text box.
        $("#search-essential-oil #search-essential-oil-text").val("");

        // Reset the result list.
        $("#essential-oil-search-results-container").html("");
    }

    function clearSearchInput() {
        // Reset search text box.
        $("#search-essential-oil #search-essential-oil-text").val("");
    }

    return {
        init: init
    };
})();

$(function() {
    SearchEssentialOil.init();
});