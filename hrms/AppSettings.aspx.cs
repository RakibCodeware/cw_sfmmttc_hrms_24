using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrms
{
    public partial class AppSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DevloperContent.Visible = false;
            if (IsPostBack && Request["__EVENTTARGET"] == "passwordSubmit")
            {
                string password = devhiddenPassword.Value;
               
                    if (Session["__DevloperPassword__"] != null)
                    {
                        if (password == Session["__DevloperPassword__"].ToString())
                        {
                            DevloperContent.Visible = true;
                            Session["__DevloperLogin__"] = true;
                        }
                        else
                        {
                            DevloperContent.Visible = false;
                        }
                    }
                    else
                    {
                        DevloperContent.Visible = false;
                    }
                

            }
        }
        //git test
        protected void logout_Click(object sender, EventArgs e)
        {
            //Session["__DevloperLogin__"]=false;
            Response.Redirect("dashboard");
        }
    }
}