using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Duftfinder.Web.App_Start
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/tether.min.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-slider.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-slider.min.css",
                "~/Content/themes/base/jquery-ui.min.css",
                "~/Content/font-awesome.min.css",
                "~/Content/Site.css"));

            Bundle searchEssentialOil = new ScriptBundle("~/bundles/searchEssentialOil").Include(
                "~/Scripts/app/SearchEssentialOil.js");
            bundles.Add(searchEssentialOil);

            Bundle searchEffects = new ScriptBundle("~/bundles/searchEffects").Include(
                "~/Scripts/app/SearchEffects.js");
            bundles.Add(searchEffects);
            
            Bundle essentialOil = new ScriptBundle("~/bundles/essentialOil").Include(
                "~/Scripts/app/EssentialOil.js");
            bundles.Add(essentialOil);

            Bundle effect = new ScriptBundle("~/bundles/effect").Include(
                "~/Scripts/app/Effect.js");
            bundles.Add(effect);

            Bundle molecule = new ScriptBundle("~/bundles/molecule").Include(
                "~/Scripts/app/Molecule.js");
            bundles.Add(molecule);

            Bundle userAdmin = new ScriptBundle("~/bundles/userAdmin").Include(
                "~/Scripts/app/UserAdmin.js");
            bundles.Add(userAdmin);
            
            Bundle dialogs = new ScriptBundle("~/bundles/dialogs").Include(
                "~/Scripts/app/Dialog.js");
            bundles.Add(dialogs);      
                  
            Bundle sliders = new ScriptBundle("~/bundles/sliders").Include(
                "~/Scripts/app/Slider.js");
            bundles.Add(sliders);

            Bundle autocomplete = new ScriptBundle("~/bundles/autocomplete").Include(
                "~/Scripts/app/Autocomplete.js");
            bundles.Add(autocomplete);

            Bundle scripts = new ScriptBundle("~/bundles/scripts").Include(
                "~/Scripts/app/Script.js");
            bundles.Add(scripts);
        }
    }
}