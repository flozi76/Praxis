var SearchEffects = (function() {

    // Define Urls for HttpPost on Controllers 
    var getEffectNamesUrl = "/SearchEffects/GetEffectNames";
    var searchEssentialOilUrl = "/SearchEffects/SearchEssentialOil";
    var essentialOilDetailsUrl = "/SearchEffects/EssentialOilDetails";
    var searchUrl = "/SearchEffects/Search";

    // Set variables globally
    this.activeFilterValue = null;

    function init() {
        console.log("init search effects");
        initializeElements();
    }

    function initializeElements() {
        // Initialize autocomplete and the required data.
        loadEffectNames();

        // Click on "Search" button.
        $("#search-effects #search-effects-button").on("click", function (e) {
            showEffectsSearchResults(e);
        });

        // Click on "Reset Search" button.
        $("#search-effects #reset-search-effects-button").on("click", function (e) {
            resetSearch();
        });

        // Click on "X" of specific search item.
        $(".clear-search-inputs").click(function (e) {
            clearSearchInput(e);
        });
    }

    function initializeElementsForShowEffectsSearchResults() {
        // Click on "Details" link.
        $("#search-effects .details-buttons").on("click", function (e) {
            showEssentialOilDetails(e);
        });

        // Click on the filter buttons.
        $("#search-effects .filter-list-amount-button").on("click", function (e) {
            filterEffectsSearchResultsAmount(e);
        });

        // Click table row.
        $("#effects-search-results-container table tbody tr").on("click", function (e) {
            highlightSearchedEffectInputs(e);
        });

        // Click on elements OUTSIDE of tbody.
        $('#effects-search-results-container table tbody').click(function (event) {
            // .one(..) is executed at most once per element per event type.
            $('html').one('click', function () {
                // Is executed, if outside of tbody element is clicked.
                console.log("click outside of tbody, remove highlight of search texts & of clicked row");

                // Remove highlight of clicked row and of search effect texts.
                $("#effects-search-results-container table tbody tr").removeClass("highlight-input");
                $("#search-effects .search-effects-texts").removeClass("highlight-input");
            });

            event.stopPropagation();
        });
    }

    function initializeElementsForShowEssentialOilDetails() {
        // Initialize the slider in order for the sliders to show.
        Slider.init();

        // IMPORTANT: stuff that changes here, also has to change in window.onpopstate = function(..)
        // Click on "Zurück" button in EssentialOilDetails.
        $("#essential-oil-details #search-effects-back-button").click(function (e) {
            // e.preventDefault() is necessary in order to prevent the window.onpopstate to be fired after the custom "Zurück" button was clicked.
            e.preventDefault();
            console.log("Click on 'Zurück' button in EssentialOilDetails.");
            showEffectsSearch();
        });

        // Workaround to implement the same functionality when the browser "Zurück" button is clicked, as when the custom "Zurück" button
        // in the Details page is clicked. Workaround is necessary because EssentialOilDetails page & EffectSearchResults are partial views.
        // "Click on 'Zurück' button in browser."
        window.onpopstate = function (e) {
            // Check is necessary in order to prevent the showEffectsSearch to be fired if other navigation stuff is done. (e.g. click on "Details" in result list)
            if ($("#essential-oil-details").length !== 0) {
                console.log("Click on 'Zurück' button in browser.");
                showEffectsSearch();
            }
        };
    }
    
    function loadEffectNames() {
        // Get all effect names from controller.
        var url = Duftfinder.getRootPath() + getEffectNamesUrl;
        $.get(url)
            .then(function(result) {
                console.log("loadEffectNames succeeded");
                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                var effectNames = result;

                // Initialize autocomplete with all the effect names.
                $(".search-effects-texts").autocomplete({
                    source: effectNames
                });
            }).fail(function(result) {
                console.log("loadEffectNames failed");
                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError + " " + Resources.Error_SearchDataCouldNotBeLoaded);
            });
    }

    // Show effects search results. Makes post to controller and gets the 
    // partial view there. Sets html result for the search result partial view.
    function showEffectsSearchResults(e) {
        // Prevents the default submit.
        e.preventDefault();

        showLoader();

        resetSearchEffectItemsWhereNoSearchInputs();

        // Get the data from the form.
        var data = $("#search-effects-form").serialize();

        var url = Duftfinder.getRootPath() + searchEssentialOilUrl;

        $.post(url, data)
            .then(function(result) {
                console.log("showEffectsSearchResults succeeded");
                // Return result is html of contents of PartialView _EssentialOilSearchResults.

                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                // Set the result container with the html result
                $("#effects-search-results-container").html(result);

                initializeElementsForShowEffectsSearchResults();

                // Only scroll to essential oil, when returned from EssentialOilDetails.
                Duftfinder.scrollToValue();

                // Set the filter on the view according to the value that was set before navigation on Details.
                if (SearchEffects.activeFilterValue) {
                    filterEffectsSearchResultsAmount(e);
                }
            }).fail(function(result) {
                console.log("showEffectsSearchResults failed");
                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError);
            }).always(function(result) {
                // Loses focus of input. Is necessary for when enter is pressed.
                $("#search-effects .search-effects-texts").blur();
            });
    }

    // Show essential oil details. Makes post to controller and gets the 
    // partial view there. Sets html result for the details partial view.
    function showEssentialOilDetails(e) {
        // The id of the clicked essential oil.
        Duftfinder.lastEditedValueId = e.currentTarget.id;

        // Set the value of the active filter globally in order to restore later.
        SearchEffects.activeFilterValue = $(".filter-list-amount-button.active").text();

        // Get the data from the form.
        var data = $("#search-effects-form").serialize();

        // Add the essential oil id to the data in order to pass it to the controller.
        data = data + "&EssentialOilId=" + Duftfinder.lastEditedValueId;

        var url = Duftfinder.getRootPath() + essentialOilDetailsUrl;

        $.post(url, data)
            .then(function (result) {
                console.log("showEssentialOilDetails succeeded");

                // Return result is html of contents of PartialView _EssentialOilDetails.
                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                // Set the result container with the html result
                $("#search-effects").html(result);

                initializeElementsForShowEssentialOilDetails();

            }).fail(function (result) {
                console.log("showEssentialOilDetails failed");

                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError);
            });
    }

    // Show effect search. Makes post to controller and gets the 
    // partial view there. Sets html result for the details partial view.
    function showEffectsSearch() {
        // Get the data from the form.
        var data = $("#search-effects-hidden-form").serialize();

        var url = Duftfinder.getRootPath() + searchUrl;

        $.post(url, data)
            .then(function (result) {
                console.log("showEffectsSearch succeeded");

                // Return result is html of contents of PartialView _EssentialOilDetails.
                // Hide alert danger & general error message.
                Duftfinder.hideAlert();

                // Set the result container with the html result
                $("#search-effects").html(result);

                // Init the slider in order for the sliders to show.
                Slider.init();

                // Initialize the elements in EffectSearch.
                initializeElements();

                // Trigger search buttons, in order to show the search results after navigation back.
                $("#search-effects #search-effects-button").trigger("click");
            }).fail(function (result) {
                console.log("showEffectsSearch failed");

                // Show alert danger & general error message.
                Duftfinder.showAlert(Resources.Error_UnexpectedError);
            });
    }

    function filterEffectsSearchResultsAmount(e) {
        console.log("filtering EffectsSearchResultsAmount...");

        // Get the filter amount value from the data-item attribute. 
        // If data-item-attribut has no value, filter amount is restored from the global variable.
        // Otherwise use value from the data-item attribute.
        var filterAmount = setFilterAmount($(e.currentTarget).attr("data-item-filter-list-amount"));
        console.log("filterAmount is " + filterAmount);
        
        // Remove all hidden tr of table.
        $("#effects-search-results-container table tbody tr").removeClass("display-none");

        if (!isDefaultFilterSet(filterAmount)) {
            // Hide all tr elements of table, according to the filterAmount.
            // Add 2 to filterAmount, because of header element of tr and then from 1 onwards.
            var hideChildrenValue = parseInt(filterAmount) + 2;
            $("#effects-search-results-container table tbody tr:nth-child(n+ " + hideChildrenValue + ")").addClass("display-none");
        }

        changeActiveFilterButton(filterAmount);
    }

    function changeActiveFilterButton(filterAmount) {
        console.log("changing ActiveFilterButton...");

        // Change active filter button.
        $(".filter-list-amount-button").removeClass("active");
        if (isDefaultFilterSet(filterAmount)) {
            $(".filter-list-amount-button:first-child").addClass("active");

        } else {
            // Check which filter is activated according to the data-item attribute.
            $(".filter-list-amount-button").each(function () {
                if ($(this).attr("data-item-filter-list-amount") === filterAmount) {
                    $(this).addClass("active");
                }
            });
        }
    }

    function highlightSearchedEffectInputs(e) {
        console.log("highlighting SearchedEffectInputs...");

        // Don't highlight anything, if header of table was clicked.
        if ($(e.currentTarget).children("th").length > 0) {
            console.log("header of table is clicked, do not highlight row");
            return;
        }

        // Remove highlight of clicked row and of search effect texts.
        $("#effects-search-results-container table tbody tr").removeClass("highlight-input");
        $("#search-effects .search-effects-texts").removeClass("highlight-input");

        // Get clicked essential oil id from table tr data item attribute and highlight the clicked row.
        var essentialOilId = $(e.currentTarget).attr("data-item-value-id");
        $(e.currentTarget).addClass("highlight-input");

        // String/array that contains all effects, that the clicked essential oil is effective for. Is retrieved from hidden field in table.
        var stringOfAllSearchedEffectsInEssentialOil = $("#effects-search-results-container #search-effect-text-" + essentialOilId).val();
        var arrayOfAllSearchedEffectsInEssentialOil = stringOfAllSearchedEffectsInEssentialOil.split(';@');

        console.log("clicked essential oil with id " + essentialOilId + " is effective for effects: " + stringOfAllSearchedEffectsInEssentialOil);
        
        // Highlight all search effect textes, that are contained in the clicked essential oil.
        $("#search-effects .search-effects-texts").each(function () {
            var searchEffectText = $.trim($(this).val());
            // Check if search effect exist in array.
            if ($.inArray(searchEffectText, arrayOfAllSearchedEffectsInEssentialOil) > -1 && searchEffectText !== "") {
                console.log("highlight effect " + searchEffectText);
                $(this).addClass("highlight-input");
            }           
        });
    }
   
    function showLoader() {
        // Show loader according to whether result is already displayed or not.
        if ($("#effects-search-results-header").length === 0) {
            $("#effects-search-results-container").append("<div id=\"loader-container\"><div class=\"loader\"></div></div>");
        } else {
            $("#effects-search-results-header").append("<div id=\"loader-container\" class=\"margin-top-small\"><div class=\"loader\"></div></div>");
        }
    }

    function resetSearchEffectItemsWhereNoSearchInputs() {
        // Remove highlight of search text.
        $("#search-effects .search-effects-texts").removeClass("disabled-input");

        // Check every search effect container, if search value was inputted.
        // Important for slider: selector must be ".slider:input" -> otherwise console error.
        $("#search-effects .search-effects-items").each(function (index) {
            // Reset slider if no search text is entered.
            if ($(this).find(".search-effects-texts").val() === "") {
                $(this).find(".slider:input").bootstrapSlider("setValue", 0);
            }

            // Highlight search text if no slider value was enterd, but search text was entered.
            if ($(this).find(".slider:input").bootstrapSlider("getValue") === 0
                && $(this).find(".search-effects-texts").val() !== "") {
                $(this).find(".search-effects-texts").addClass("disabled-input");
            }
        });
    }

    function resetSearch() {
        // Reset all text boxes and sliders.
        // Important for slider: selector must be ".slider:input" -> otherwise console error.
        $("#search-effects .search-effects-texts").val("");
        $("#search-effects .slider:input").bootstrapSlider("setValue", 0);
        $("#search-effects .search-effects-texts").removeClass("disabled-input");

        // Reset the result list.
        $("#effects-search-results-container").html("");
    }

    function clearSearchInput(e) {
        // Reset specific search text box and slider.
        var id = $(e.currentTarget).attr("data-item-id");
        $("#search-effects #search-effect-text-" + id).val("");
        $("#search-effects #slider-" + id).bootstrapSlider("setValue", 0);
        $("#search-effects #search-effect-text-" + id).removeClass("disabled-input");
    }

    function setFilterAmount(filterAmount) {
        // Get filterAmount value from globally set activeFilterValue. Is set when return from Details.
        if (!filterAmount && SearchEffects.activeFilterValue) {
            if (isDefaultFilterSet(SearchEffects.activeFilterValue)) {
                filterAmount = null;
            } else {
                filterAmount = SearchEffects.activeFilterValue;
            }
            SearchEffects.activeFilterValue = null;
        }
        return filterAmount;
    }

    function isDefaultFilterSet(filterAmount) {
        if (filterAmount !== "" && filterAmount !== null && filterAmount !== Resources.SearchEffects_SearchResultFilter_All) {
            return false;
        }
        return true;
    }

    return {
        init: init
    };
})();

$(function() {
    SearchEffects.init();
});