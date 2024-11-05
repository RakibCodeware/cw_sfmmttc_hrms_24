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
    public partial class advance_index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
           
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