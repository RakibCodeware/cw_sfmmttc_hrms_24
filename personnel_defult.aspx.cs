using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP
{
    public partial class personnel_defult : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    HttpCookie getCookies = Request.Cookies["userInfo"];
                    if (getCookies == null || getCookies.Value == "")
                    {
                        Response.Redirect("~/ControlPanel/Login.aspx");
                    }
                    else
                    {

                        if (Session["__IsCompliance__"].ToString().Equals("True"))
                        {
                            try
                            {
                                divRoster.Visible = false;
                            }
                            catch { Response.Redirect("~/default.aspx"); }
                        }
                    }
                }
                catch { }
            }
        }
    }
}