using System.Web;
using System.Web.Optimization;

namespace OnlineShop.App
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region js
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/bootstrap.min.js",
                    "~/Scripts/respond.js",
                    "~/Scripts/metisMenu.min.js"
                    ));
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                       "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/raphael").Include(
                        "~/Scripts/raphael-min.js"));
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/datatables/jquery.dataTables.js",
                "~/Scripts/datatables/dataTables.bootstrap.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                   "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/datepicker").Include(
                   "~/Scripts/datepickers/bootstrap-datepicker.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-modal-carousel").Include(
                   "~/Scripts/bootstrap-modal-carousel/bootstrap-modal-carousel.min.js"));

            #endregion
            #region css
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/font-awesome.min.css"));
            bundles.Add(new StyleBundle("~/Content/adminstyle").Include(  "~/Content/metisMenu.min.css","~/Content/timeline.css","~/Content/sb-admin-2.css", "~/Content/morris.css"));

            bundles.Add(new StyleBundle("~/Content/datatables").Include(
              "~/Scripts/datatables/dataTables.bootstrap.css",
               "~/Scripts/datatables/dataTables.responsive.css"
               ));
 bundles.Add(new StyleBundle("~/Content/datepicker").Include(
              "~/Content/datepicker/bootstrap-datepicker3.min.css",
              "~/Content/datepicker/bootstrap-datepicker.min.css",
               "~/Content/datepicker/bootstrap-datepicker3.standalone.min.css"
               ));
 bundles.Add(new StyleBundle("~/Content/bootstrap-modal-carousel").Include(
              "~/Content/bootstrap-modal-carousel/bootstrap-modal-carousel.min.css"
               ));

            #endregion
        }
    }
}
