using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.personnel
{
    public partial class EmpInfo_Index : System.Web.UI.Page
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
                        divSeparation.Visible = false;
                        divSeparationReport.Visible = false;
                        divSeparationComp.Visible = true;
                        divSeparationReportComp.Visible = true;

                        divEmployeeEntry.Visible = false;
                        divEmployeeList.Visible = false;
                        divEmployeeListReport.Visible = false;
                        divEmployeeProfileReport.Visible = false;
                        divManPowerStatusReport.Visible = false;
                        divMonthlyManPowerReport.Visible = false;
                        divContactListReport.Visible = false;
                        divIDCardReport.Visible = false;
                        divBloodGroup.Visible = false;
                    }
                    else
                    {
                        divSeparationComp.Visible = false;
                        divSeparationReportComp.Visible = false;
                        divSeparation.Visible = true; 
                        divSeparationReport.Visible = true;
                        
                    }
                }
            }
            catch (Exception ex) { Response.Redirect("~/ControlPanel/Login.aspx"); }

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