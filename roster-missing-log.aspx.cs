using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP
{
    public partial class roster_missing_log : System.Web.UI.Page
    {
        string qurey = "";
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadRosterMissingLog();
            }
        }
        private void loadRosterMissingLog()
        {
            try
            {
                string STID = Request.QueryString["STID"].ToString();
                qurey = "select stml.EmpID,stml.STID,s.SftName,e.EmpName,e.DsgName,SUBSTRING(e.EmpCardNo,8,6) as EmpCardNo,format(stml.Date,'dd-MM-yyyy') as Date, format(stml.InsertTime,'dd-MM-yyyy HH:mm:ss') as InsertTime from ShiftTransferDetailsMissingLog stml inner join ShiftTransferInfo sti on stml.STID=sti.STId left join  HRD_Shift s on sti.SftId=s.SftId left join v_EmployeeDetails e on stml.EmpID=e.EmpId and e.IsActive=1 where stml.STID="+ STID;
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(qurey);
                gvEmpList.DataSource = dt;
                gvEmpList.DataBind();
            }
            catch(Exception ex) { }
        }
    }
}