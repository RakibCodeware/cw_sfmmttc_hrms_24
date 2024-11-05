using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP
{
    public partial class attendance_default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (!IsPostBack)
                {
                    HttpCookie getCookies = Request.Cookies["userInfo"];
                    //ViewState["__CShortName__"] = getCookies["__CShortName__"].ToString();
                    if (getCookies == null || getCookies.Value == "")
                    {
                        Response.Redirect("~/ControlPanel/Login.aspx");

                    }
                    if (getCookies["__IsCompliance__"].ToString().Equals("True"))
                    {
                        divMonthSetup.Visible = false;
                        divMonthSetupComp.Visible = true;

                        divAttProcessing.Visible = false;
                        divAttProcessingNew.Visible = false;
                        divManuallyCount.Visible = false;
                        divAttendanceList.Visible = false;
                        divAttSummary.Visible = false;
                        divInOutReport.Visible = false;
                        divManualReport.Visible = false;
                        divManpowerStatement.Visible = false;
                        divManpowerWiseAttendance.Visible = false;
                        divOvertimeReport.Visible = false;
                        divAbsentNotification.Visible = false;
                        divOutduty.Visible = false;
                        divOutdutyApproval.Visible = false;
                        divOutdutyReport.Visible = false;
                        divWeekendSetEmpWise.Visible = false;
                        divGeneralDay.Visible = false;
                        divWeekendInfoReport.Visible = false;
                        
                    }
                    else
                    {
                        divMonthSetup.Visible = true;
                        divMonthSetupComp.Visible = false;
                    }
                    divAttProcessing.Visible = false;
                    //if (ViewState["__CShortName__"].ToString().Equals("MRC"))// Marico
                    //if (ViewState["__CShortName__"].ToString().Equals("MRC"))// Marico
                    //{
                    //    divAttReportDaterange.Visible = true;
                    //    divManpowerWiseAttendance.Visible = false;
                    //}
                    //else
                    //{
                    //    divAttReportDaterange.Visible = false;
                    //    divManpowerWiseAttendance.Visible = true;

                    //}
                    //if (Session["__UserNameText__"].ToString() == "common" && !ViewState["__CShortName__"].ToString().Equals("MRC"))
                    //{
                    //    try
                    //    {
                    //        divMonthSetup.Visible = false;
                    //        divMonthSetupComp.Visible = false;
                    //        divAttProcessing.Visible = false;
                    //        divAttProcessingNew.Visible = false;
                    //        divManuallyCount.Visible = false;
                    //        divAttendanceList.Visible = false;
                    //        divAttSummary.Visible = false;
                    //        divInOutReport.Visible = false;
                    //        divManualReport.Visible = false;
                    //        divManpowerStatement.Visible = false;
                    //        divManpowerWiseAttendance.Visible = false;
                    //        divOvertimeReport.Visible = false;
                    //        divAbsentNotification.Visible = false;                           
                    //        divOutdutyApproval.Visible = false;
                    //        divOutdutyReport.Visible = false;
                    //        divWeekendSetEmpWise.Visible = false;
                    //        divGeneralDay.Visible = false;
                    //        divWeekendInfoReport.Visible = false;
                    //        divMonthlyStatus.Visible = false;


                    //    }
                    //    catch { Response.Redirect("~/default.aspx"); }
                    //}
                }




            }
            catch (Exception ex)
            {
                Response.Redirect("~/ControlPanel/Login.aspx");
            }
        }

        public bool IsRouteExists(string url)
        {
            foreach (Route route in RouteTable.Routes)
            {
                var routeUrl = route.Url?.ToLower();
                if (!string.IsNullOrEmpty(routeUrl) && routeUrl.Contains(url.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}