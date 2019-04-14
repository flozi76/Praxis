var Duftfinder = (function() {
    // Set variables globally
    this.lastEditedValueId = null;

    // Define rootPath according to whether version is running on deployed server or locally.
    function getRootPath() {
        // window.ROOT for running on localhost 
        // "" for running on AppHarbor
        console.log("root path is " + window.location.host);
        return window.location.host === "localhost" ? window.ROOT : "";
    }

    // Define pathName according to whether version is running on deployed server or locally.
    function splitPathNameFromRoot() {
        console.log("location path name  is " + location.pathname);
        var originPath = location.origin;

        if (window.location.host === "localhost") {
            // For localhost.
            originPath = originPath + "/Duftfinder.Web";
            return location.href.replace(originPath, "");
        } else {
            // For running on AppHarbor
            return location.href.replace(originPath, "");
        }
    }

    function init() {
        console.log("init script");
        initializeElements();
    }

    function initializeElements() {
        // Highlight the active navigation item.
        setActiveNavigationItem();

        // Tooltips
        $('[data-toggle="tooltip"]').tooltip();

        // Click on X of alert
        $("#alert-danger .close").on("click",
            function(e) {
                hideAlert();
            });

        $(".picture").mouseover(function () {
            var picnumber = $(this).attr("picnumber");
            console.log("Mouse moving over picture " + picnumber);
            $("#pop-up_" + picnumber).show();
        });

        $(".picture").mouseout(function () {
            var picnumber = $(this).attr("picnumber");
            console.log("Mouse leaved picture");
            $("#pop-up_" + picnumber).hide();
        });
    }

    function hideAlert() {
        // Hide alert danger & general error message.
        $("#alert-danger #alert-text").text("");
        $("#alert-danger").addClass("display-none");
    }

    function showAlert(errorMessage) {
        // Show alert danger & general error message.
        $("#alert-danger #alert-text").text(errorMessage);
        $("#alert-danger").removeClass("display-none");
    }

    // Workaround to set the color of the acive navigation item.
    function setActiveNavigationItem() {
        // Remove http://localhost/Duftfinder.Web/ or http://duftfinder.apphb.com/ before url.

        var controllerAndActionUrl = splitPathNameFromRoot().toLowerCase();

        // Check if page url contains string and set active class appropriately.
        if ((controllerAndActionUrl === "/" || controllerAndActionUrl === "")) {
            $("#search-essential-oil-navigation-item").addClass("active");
        } else if (controllerAndActionUrl.indexOf("/searcheffects") >= 0) {
            $("#search-effects-navigation-item").addClass("active");
        } else if (controllerAndActionUrl.indexOf("/essentialoil") >= 0) {
            $("#essential-oil-navigation-item").addClass("active");
        } else if (controllerAndActionUrl.indexOf("/effect") >= 0) {
            $("#effect-navigation-item").addClass("active");
        } else if (controllerAndActionUrl.indexOf("/molecule") >= 0) {
            $("#molecule-navigation-item").addClass("active");
        } else if (controllerAndActionUrl.indexOf("/useradmin") >= 0) {
            $("#user-admin-navigation-item").addClass("active");
        } else if (controllerAndActionUrl.indexOf("/configuration") >= 0) {
            $("#configuration-icon").addClass("active");
        }
    }

    // Workaround to set the color of the acive subnavigation item.
    // Has to be like this, because of partial view in CreateOrEdit, AssignMolecule, AssignEffect.
    // Otherwise the class is lost, after load of page.
    function setActiveSubnavigationItem() {
        // Remove http://localhost/Duftfinder.Web/ or http://duftfinder.apphb.com/ before url.
        var urlControllerAndAction = splitPathNameFromRoot().toLowerCase();

        // Remove active class.
        $(".subnavigation-item").removeClass("active");

        // Check if page url contains string and set active class appropriately.
        if (urlControllerAndAction.indexOf("createoredit") >= 0) {
            $("#create-or-edit-subnavigation-item").addClass("active");
        } else if (urlControllerAndAction.indexOf("assignmolecule") >= 0) {
            $("#assign-molecule-subnavigation-item").addClass("active");
        } else if (urlControllerAndAction.indexOf("assigneffect") >= 0) {
            $("#assign-effect-subnavigation-item").addClass("active");
        } else if (urlControllerAndAction.indexOf("assignessentialoil") >= 0) {
            $("#assign-essential-oil-subnavigation-item").addClass("active");
        }
    }

    function scrollToValue() {
        // Scrolls to value (e.g. essential oil, effect or molecule), when returned from CreateOrEdit, 
        // AssignMolecule, AssignEffect EssentialOilDetails etc.
        if (Duftfinder.lastEditedValueId) {
            console.log("Scroll to essential oil with id: " + Duftfinder.lastEditedValueId);
            $("html, body").animate({
                    scrollTop: $("tr[data-item-value-id=" + Duftfinder.lastEditedValueId + "]").offset().top - 200
                },
                1000);

            // Reset the value
            Duftfinder.lastEditedValueId = null;
        }
    }

    // Allow users to enter numbers only.
    $(".numeric-only").bind("keypress",
        function(e) {
            if (e.keyCode === "9" || e.keyCode === "16") {
                return;
            }
            var code;
            if (e.keyCode) code = e.keyCode;
            else if (e.which) code = e.which;
            // Allow . (is code 46)
            //if (e.which === 46) 
            //    return false;
            if (code === 8 || code === 46)
                return true;
            if (code < 48 || code > 57)
                return false;
        });

    // Disable paste.
    $(".numeric-only").bind("paste",
        function(e) {
            e.preventDefault();
        });

    return {
        init: init,
        getRootPath: getRootPath,
        showAlert: showAlert,
        hideAlert: hideAlert,
        setActiveSubnavigationItem: setActiveSubnavigationItem,
        scrollToValue: scrollToValue,
    };
})();

$(function() {
    Duftfinder.init();
});