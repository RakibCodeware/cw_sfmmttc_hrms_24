using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class salary_index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (!IsPostBack)
                {
                    HttpCookie getCookies = Request.Cookies["userInfo"];
                    if (getCookies == null || getCookies.Value == "")
                    {
                        Response.Redirect("~/ControlPanel/Login.aspx");

                    }
                    if (getCookies["__IsCompliance__"].ToString().Equals("True"))
                    {
                        
                        divEarnLeavePaymentGeneration.Visible = false;
                        divEarnLeavePaymentSheet.Visible = false;
                        divEarnLeavePaymentGenerationComp.Visible = true;
                        divEarnLeavePaymentSheetComp.Visible = true;
                        
                        divPromotionEntryComp.Visible = true;
                        divPromotionReportComp.Visible = true;
                        divIncrementComp.Visible = true;
                        divIncrementReportComp.Visible = true;
                        divPromotionEntry.Visible = false;
                        divPromotionReport.Visible = false;
                        divIncrement.Visible = false;
                        divIncrementReport.Visible = false;

                        divSalaryEntry.Visible = false;
                        divSalaryEntryc.Visible = true;
                        divAllowanceCalculation.Visible = false;                      
                        divAutoIncrementComp.Visible = true;

                     
                    }
                    else
                    {
                        divEarnLeavePaymentGeneration.Visible = true;
                        divEarnLeavePaymentSheet.Visible = true;
                        divEarnLeavePaymentGenerationComp.Visible = false;
                        divEarnLeavePaymentSheetComp.Visible = false;

                        divPromotionEntryComp.Visible = false;
                        divPromotionReportComp.Visible = false;
                        divIncrementComp.Visible = false;
                        divIncrementReportComp.Visible = false;
                        divPromotionEntry.Visible = true;
                        divPromotionReport.Visible = true;
                        divIncrement.Visible = true;
                        divIncrementReport.Visible = true;
                        divAutoIncrementComp.Visible = false;
                        divSalaryEntry.Visible = true;
                        divSalaryEntryc.Visible = false;
                    }
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