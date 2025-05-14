using System.Web.Optimization;

namespace Yoonite.UI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/plugin-scripts")
                   .Include("~/Content/plugins/jquery/jquery.js",
                            "~/Content/plugins/tether/js/tether.min.js",
                            "~/Content/plugins/popper/popper.min.js",
                            "~/Content/plugins/bootstrap/js/bootstrap.min.js",
                            "~/Content/plugins/knockout/knockout.3.4.2.min.js",
                            "~/Content/plugins/jquery-affix/jquery.affix.js",
                            "~/Content/plugins/jquery-inlinesvg/jquery.inlinesvg.min.js",
                            "~/Content/plugins/jquery-scrollTo/jquery.scrollTo.min.js",
                            "~/Content/plugins/jquery-trackpad-scroll-emulator/js/jquery.trackpad-scroll-emulator.min.js",
                            "~/Content/plugins/jquery-validate/jquery.validate.min.js",
                            "~/Content/plugins/slick/slick.min.js",
                            "~/Content/plugins/toastr/toastr.min.js",
                            "~/Content/plugins/wNumb/wNumb.js",
                            "~/Content/plugins/dropzone/dropzone.js",
                            "~/Content/plugins/azure-api/azure-storage.common.min.js",
                            "~/Content/plugins/azure-api/azure-storage.blob.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/theme-scripts")
                   .Include("~/Content/theme/js/explorer.js"));

            bundles.Add(new ScriptBundle("~/bundles/view_models")
                   .Include("~/ViewModels/*.js"));



            bundles.Add(new StyleBundle("~/bundles/plugin-styles")
                   .Include("~/Content/plugins/jquery-trackpad-scroll-emulator/css/trackpad-scroll-emulator.css",
                            "~/Content/plugins/slick/slick.css",
                            "~/Content/plugins/slick/slick-theme.css",
                            "~/Content/plugins/toastr/toastr.css",
                            "~/Content/plugins/dropzone/dropzone.css"));

            bundles.Add(new StyleBundle("~/bundles/theme-styles")
                   .Include("~/Content/theme/css/explorer-custom.css"));

            bundles.Add(new StyleBundle("~/bundles/fonts")
                   .Include("~/Content/fonts/font-awesome/css/font-awesome.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}