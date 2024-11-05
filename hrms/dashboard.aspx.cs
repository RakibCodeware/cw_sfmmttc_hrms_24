using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrms
{
    public partial class dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["__GetUserId__"] == null)
                {
                    HttpContext.Current.Response.Redirect("/hrms/login");
                }
                AccessControl.getDataAccessPermission(Session["__GetUserId__"].ToString());
            }
        }
    }
}