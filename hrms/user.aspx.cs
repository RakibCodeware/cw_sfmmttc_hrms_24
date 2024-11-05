using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrms
{
    public partial class user : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["__ReadAction__"] = "0";
                Session["__WriteAction__"] = "0";
                Session["__UpdateAction__"] = "0";
                Session["__DeletAction__"] = "0";
                int[] pagePermission = { 443, 444, 447, 448 };

                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);
                setprivilige(userPagePermition);
                string token = Session["__UserToken__"]?.ToString();
                Console.WriteLine(token);

                classes.commonTask.LoadBranch(ddlCompany);
                ddlCompany.SelectedIndex = 1;
                //ViewState["__IsCompliance__"] = "False";
            }
        }

        private void setprivilige(int[] permission)
        {
            if(permission.Contains(443))
                Session["__ReadAction__"] = "1";
            if(permission.Contains(444))
                Session["__WriteAction__"] = "1";
            if (permission.Contains(447))
                Session["__UpdateAction__"] = "1";
            if(permission.Contains(448))
                Session["__DeletAction__"] = "1";


        }
    }
}