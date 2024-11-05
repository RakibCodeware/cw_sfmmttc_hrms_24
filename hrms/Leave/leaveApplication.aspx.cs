using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrms.Leave
{
    public partial class leaveApplication : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            classes.commonTask.LoadBranch(ddlCompany);
            ddlCompany.SelectedIndex = 1;

        }
    }
}