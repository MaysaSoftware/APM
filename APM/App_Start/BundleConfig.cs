using System.Web;
using System.Web.Optimization;

namespace APM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/ContentBPMNCSS").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/LayoutCSS")
                .Include("~/Theme/CSS/Layout.css", new CssRewriteUrlTransform())
            );


            bundles.Add(new StyleBundle("~/SigninCSS")
                .Include("~/Theme/CSS/Signin.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/DesktopCSS")
                .Include("~/Theme/CSS/Desktop.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/EditorCSS")
                .Include("~/Theme/CSS/Editor.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/HomeCSS")
                .Include("~/Theme/CSS/Home.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/KendoCSS").Include(
                     "~/Content/kendo/2022.1.119/kendo.bootstrap-main.min.css",
                     "~/Content/bootstrap.min.css",
                     "~/Content/kendo/2022.1.119/kendo.rtl.min.css"
                     ));


            bundles.Add(new ScriptBundle("~/KendoJS").Include(
                      "~/Scripts/kendo/2022.1.119/jquery.min.js",
                      "~/Scripts/kendo/2022.1.119/jszip.min.js",
                      "~/Scripts/kendo/2022.1.119/kendo.all.min.js",
                      "~/Scripts/kendo/2022.1.119/kendo.aspnetmvc.min.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/kendo.modernizr.custom.js"
                      ));


            bundles.Add(new ScriptBundle("~/Login/IndexJS").Include(
                "~/Theme/Script/Login/Index.js"
                ));

            bundles.Add(new ScriptBundle("~/Login/SignupJS").Include(
                "~/Theme/Script/Login/Signup.js"
                ));

            bundles.Add(new ScriptBundle("~/Login/ForgotPasswordJS").Include(
                "~/Theme/Script/Login/ForgotPassword.js"
                ));

            bundles.Add(new ScriptBundle("~/Login/VerifyCodeJS").Include(
                "~/Theme/Script/Login/VerifyCode.js"
                ));
             

            bundles.Add(new ScriptBundle("~/Home/IndexJS").Include(
                "~/Theme/Script/Home/Index.js",
                "~/Theme/Script/Home/RightMenu.js",
                "~/Theme/Script/Email.js",
                "~/Theme/Script/Comment.js"
                ));
            bundles.Add(new ScriptBundle("~/LodarJS").Include(
                "~/Theme/Script/Lodar.js"
                ));

            bundles.Add(new StyleBundle("~/FontCSS")
                //.Include("~/Theme/Fonts/fontawesome/css/all.css", new CssRewriteUrlTransform())
                //.Include("~/Theme/Fonts/IranSans/css/style.css", new CssRewriteUrlTransform())
                .Include("~/Fonts/Base.css", new CssRewriteUrlTransform())
                .Include("~/Fonts/Awesome.css", new CssRewriteUrlTransform())
                .Include("~/Fonts/BFonts.css", new CssRewriteUrlTransform())
            );

            bundles.Add(new StyleBundle("~/BPMNCSS") 
                .Include("~/Theme/CSS/BPMN/bpmn-js.css", new CssRewriteUrlTransform())
                .Include("~/Theme/CSS/BPMN/bpmn.css", new CssRewriteUrlTransform())
                .Include("~/Theme/CSS/BPMN/diagram-js.css", new CssRewriteUrlTransform())
                .Include("~/Theme/CSS/BPMN/font/bpmn.eot", new CssRewriteUrlTransform())
                .Include("~/Theme/CSS/BPMN/font/bpmn.ttf", new CssRewriteUrlTransform())
                .Include("~/Theme/CSS/BPMN/font/bpmn.woff", new CssRewriteUrlTransform())
                .Include("~/Theme/CSS/BPMN/font/bpmn.woff2", new CssRewriteUrlTransform())
            );
            
            bundles.Add(new StyleBundle("~/DiagramCSS") 
                .Include("~/Theme/CSS/Diagram.css", new CssRewriteUrlTransform())
            );


            bundles.Add(new ScriptBundle("~/BPMNModelerJS").Include(
                 "~/Theme/Script/BPMN/bpmn-modeler.development.js",
                 "~/Theme/Script/BPMN/jquery.js",
                 "~/Theme/Script/BPMN/BPMN.js"
                 ));
            bundles.Add(new ScriptBundle("~/BPMNViewerJS").Include(
                 "~/Theme/Script/BPMN/bpmn-viewer.development.js",
                 "~/Theme/Script/BPMN/BPMN.js"
                 )); 

            bundles.Add(new ScriptBundle("~/FocusJS").Include(
                 "~/Theme/Script/Focus.js"
                 ));

            bundles.Add(new ScriptBundle("~/PopupWindowJS").Include(
                 "~/Theme/Script/PopupWindow.js"
                 ));


            bundles.Add(new ScriptBundle("~/DesktopJS").Include(
                 "~/Theme/Script/Desktop.js",
                 "~/Theme/Script/EventTable.js",
                 "~/Theme/Script/Grid/ImportData.js",
                 "~/Theme/Script/Grid/GridGroupableField.js"
                 ));

            bundles.Add(new ScriptBundle("~/RoleTypePermissionJS").Include(
                 "~/Theme/Script/RoleTypePermission.js"
                 ));

            bundles.Add(new ScriptBundle("~/AttachmentJS").Include(
                 "~/Theme/Script/Attachment.js"
                 ));

            bundles.Add(new ScriptBundle("~/SysSettingJS").Include(
                 "~/Theme/Script/SysSetting.js"
                 ));

            bundles.Add(new StyleBundle("~/SysSettingCSS")
                 .Include("~/Theme/CSS/Setting.css", new CssRewriteUrlTransform())
             );

            bundles.Add(new ScriptBundle("~/pdfJS").Include(
                 "~/Theme/Script/pdf.js", 
                 "~/Theme/Script/pdf.worker.js",
                 "~/Theme/Script/PDF/html2canvas.js",
                 "~/Theme/Script/PDF/jspdf.min.js"
                 ));
             
            bundles.Add(new StyleBundle("~/PersionDatePickerCSS")
                 .Include("~/Theme/Script/PersionDateTimePicker/jalalidatepicker.css", new CssRewriteUrlTransform()) 
             );

            bundles.Add(new ScriptBundle("~/PersionDatePickerJS").Include(
                "~/Theme/Script/PersionDateTimePicker/JalaliDate.js",
                "~/Theme/Script/PersionDateTimePicker/jalalidatepicker.js"
            ));

            bundles.Add(new StyleBundle("~/MiladyDatePickerCSS")
                    .Include("~/Theme/Script/MiladyDateTimePicker/Miladydatepicker.css", new CssRewriteUrlTransform())
                );
            
            bundles.Add(new StyleBundle("~/ReportBuilderCSS")
                    .Include("~/Theme/CSS/ReportBuilder.css", new CssRewriteUrlTransform())
                );
            
            bundles.Add(new StyleBundle("~/DashboardCSS")
                    .Include("~/Theme/CSS/Dashboard.css", new CssRewriteUrlTransform())
                );

            bundles.Add(new StyleBundle("~/PrintCSS")
                    .Include("~/Theme/CSS/Print/Print.css", new CssRewriteUrlTransform())
                    .Include("~/Theme/CSS/Print/Print_A4_Portrait.css", new CssRewriteUrlTransform())
                );

            bundles.Add(new ScriptBundle("~/PrintJS").Include(
                "~/Theme/Script/Print.js"
            ));

            bundles.Add(new ScriptBundle("~/MiladyDatePickerJS").Include(
                "~/Theme/Script/MiladyDateTimePicker/Miladydatepicker.js"
            ));

            bundles.Add(new ScriptBundle("~/InputValidtionJS").Include(
                "~/Theme/Script/InputValidtion.js"
            ));

            bundles.Add(new ScriptBundle("~/ProcessReferralJS").Include(
                "~/Theme/Script/ProcessReferral.js"
            ));

            bundles.Add(new ScriptBundle("~/ReportJS").Include(
                "~/Theme/Script/Report.js"
            ));

            bundles.Add(new ScriptBundle("~/SignalRJS").Include( 
                "~/Theme/Script/SignalR.js"
            ));

            bundles.Add(new ScriptBundle("~/KendoSettingJS").Include(
                "~/Theme/Script/Kendo/kendo.messages.fa-IR.js",
                "~/Theme/Script/Kendo/kendo.validator.js"
            ));


            bundles.Add(new ScriptBundle("~/CalendarGridJS").Include(
                "~/Theme/Script/CalendarGrid.js"
            ));

            bundles.Add(new ScriptBundle("~/SessionTimerJS").Include(
                "~/Theme/Script/SessionTimer.js"
            ));

            bundles.Add(new ScriptBundle("~/OrgChartJS").Include(
                "~/Theme/Script/OrgChart.js"
            ));

            bundles.Add(new ScriptBundle("~/DashboardJS").Include(
                "~/Theme/Script/Dashboard.js"
            ));

            bundles.Add(new ScriptBundle("~/WebcamJS").Include(
                "~/Theme/Script/Webcam/webcam.min.js"
            ));


            bundles.Add(new ScriptBundle("~/AghajariOilAndGas_Reservation_Food_JS").Include(
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/Food.js",
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/Shift.js",
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/Delayed2.js",
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/Delayed.js",
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/Letter.js",
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/Letter2.js",
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/Office.js",
                "~/Theme/Script/SpecialScript/AghajariOilAndGas/Reservation/ChangeFood.js"
            ));

            //bundles.UseCdn = true;
            //BundleTable.EnableOptimizations = true;
        }

    }
}
