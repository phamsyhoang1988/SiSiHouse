using System.Web;
using System.Web.Optimization;

namespace SiSiHouse
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            /* --- JS --- */
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jQuery-2.1.3.min.js"
                , "~/Scripts/bootstrap.min.js"
                , "~/Scripts/app.js"
                , "~/Scripts/kickstart.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery/plugins").Include(
                "~/Scripts/plugins/thickbox.js"
                , "~/Scripts/plugins/pace.js"
                , "~/Scripts/plugins/bootstrap-datepicker.js"
                , "~/Scripts/plugins/bootstrap-datepicker.vi.js"
                , "~/Scripts/plugins/bootstrap-dialog.min.js"
                , "~/Scripts/plugins/jquery.numeric.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/input").Include(
                "~/Scripts/SiSi.CmnEventUtil.js"
                , "~/Scripts/SiSi.numeric.js"
                , "~/Scripts/SiSi.Utility.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins/datatables/dataTable").Include(
                "~/Scripts/plugins/datatables/jquery.dataTables.js"
                , "~/Scripts/plugins/datatables/dataTables.bootstrap.js"
                , "~/Scripts/plugins/datatables/SiSi.dataTables.js"
                , "~/Scripts/plugins/datatables/fnStandingRedraw.js"));

            //bundles.Add(new ScriptBundle("~/bundles/dragOn").Include("~/Scripts/plugins/drag-on.js"));

            bundles.Add(new ScriptBundle("~/bundles/UpdateProduct").Include("~/Scripts/SiSi.UpdateProduct.js"));

            bundles.Add(new ScriptBundle("~/bundles/ListProduct").Include("~/Scripts/SiSi.ListProduct.js"));

            bundles.Add(new ScriptBundle("~/bundles/UpdateOrders").Include("~/Scripts/SiSi.UpdateOrders.js"));

            bundles.Add(new ScriptBundle("~/bundles/SalesStatistics").Include("~/Scripts/SiSi.SalesStatistics.js"));

            bundles.Add(new ScriptBundle("~/bundles/SalesStatisticsDetail").Include("~/Scripts/SiSi.SalesStatisticsDetail.js"));

            bundles.Add(new ScriptBundle("~/bundles/Bill").Include("~/Scripts/SiSi.Bill.js"));

            bundles.Add(new ScriptBundle("~/bundles/ShowCollection").Include("~/Scripts/SiSi.ShowCollection.js"));

            bundles.Add(new ScriptBundle("~/bundles/ShowItem").Include("~/Scripts/SiSi.ShowItem.js"));

            bundles.Add(new ScriptBundle("~/bundles/MobileCmEvent").Include("~/Scripts/SiSi.mobile.CmnEvent.js"));

            bundles.Add(new ScriptBundle("~/bundles/plugins/mobileSlider/slider").Include("~/Scripts/plugins/mobileSlider/swiper.js"));


            /* --- CSS --- */
            bundles.Add(new StyleBundle("~/Content/bootstrap/css/bootstrap").Include("~/Content/bootstrap/css/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/adminLTE/altCss").Include("~/Content/adminLTE/AdminLTE.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome/css/icon").Include(
                "~/Content/fontawesome/css/font-awesome.css"
                , "~/Content/fontawesome/css/font-awesome-animation.css"
            ));

            bundles.Add(new StyleBundle("~/Content/common").Include(
                "~/Content/datepicker.css"
                , "~/Content/cm-layout.css"
                , "~/Content/cm-form.css"
                , "~/Content/thickbox.css"
            ));

            bundles.Add(new StyleBundle("~/Content/table").Include("~/Content/dataTables.bootstrap.css"));

            bundles.Add(new StyleBundle("~/Content/CustomControllerCss/ManageProduct").Include("~/Content/CustomControllerCss/ManageProduct.css"));

            bundles.Add(new StyleBundle("~/Content/CustomControllerCss/ManageOrders").Include("~/Content/CustomControllerCss/ManageOrders.css"));

            bundles.Add(new StyleBundle("~/Content/CustomControllerCss/ManageStatistics").Include("~/Content/CustomControllerCss/ManageStatistics.css"));

            bundles.Add(new StyleBundle("~/Content/Shop").Include("~/Content/shop.css"));

            bundles.Add(new StyleBundle("~/Content/Mobile").Include("~/Content/mobile.css"));
        }
    }
}