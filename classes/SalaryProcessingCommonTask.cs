using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SigmaERP.classes
{
    public static class SalaryProcessingCommonTask
    {
        static DataTable dt;
        public static void loadSeparationMonth(DropDownList ddlMonth,string CompnayId)
        {
            try
            {
                dt = new DataTable();
                dt=CRUD.ExecuteReturnDataTable("Select distinct MonthName,Format(FromDate,'MMM-yyyy') as YearMonth,MonthId From tblMonthSetup  where CompanyId='" + CompnayId + "' order by MonthId desc");
                ddlMonth.DataTextField = "YearMonth";
                ddlMonth.DataValueField = "MonthName";
                ddlMonth.DataSource = dt;
                ddlMonth.DataBind();
                ddlMonth.Items.Insert(0, new ListItem(" ", " "));
            }
            catch { }
        }

    }
}