using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class bonus_index : System.Web.UI.Page
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
                        aBonusSetup.Visible = false;
                        aBonusGenerate.Visible = false;
                        aBonusMonthSetup.Visible = false;
                        aBonusGenerate.Visible = false;
                        aBonusRiseFall.Visible = false;
                        aBonusSheet.Visible = false;
                        aBonusMissSheet.Visible = false;
                        aBonusSummary.Visible = false;

                        aBonusSetupc.Visible = true;
                        aBonusGeneratec.Visible = true;
                        aBonusMonthSetupc.Visible = true;
                        aBonusGeneratec.Visible = true;
                        aBonusSheetc.Visible = true;
                    }
                    else
                    {
                        aBonusSetup.Visible = true;
                        aBonusGenerate.Visible = true;
                        aBonusMonthSetup.Visible = true;
                        aBonusGenerate.Visible = true;
                        aBonusRiseFall.Visible = true;
                        aBonusSheet.Visible = true;
                        aBonusMissSheet.Visible = true;
                        aBonusSummary.Visible = true;

                        aBonusSetupc.Visible = false;
                        aBonusGeneratec.Visible = false;
                        aBonusMonthSetupc.Visible = false;
                        aBonusGeneratec.Visible = false;
                        aBonusSheetc.Visible = false;
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