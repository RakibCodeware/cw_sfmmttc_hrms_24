using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.attendance
{
    public partial class attendance_analysis_report : System.Web.UI.Page
    {
        DataTable dt;
        //permission=327
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            int[] pagePermission = { 327 };

            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                txtFromDate.Text = "01-" + DateTime.Now.ToString("MM-yyyy");
                txtToDate.Text =  DateTime.Now.ToString("dd-MM-yyyy");
                setPrivilege();
               
              
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
        }
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForList(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "attendance.aspx", ddlCompanyList, gvAttendanceList, btnSearch);

                //ViewState["__ReadAction__"] = AccessPermission[0];
                //ViewState["__WriteAction__"] = AccessPermission[1];
                //ViewState["__UpdateAction__"] = AccessPermission[2];
                //ViewState["__DeletAction__"] = AccessPermission[3];
            }
            catch { }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtFromDate.Text.Trim().Length != 10)
            {
                lblMessage.InnerText = "warning-> Select From Date";
                txtFromDate.Focus();
                return;
            }
            if (txtToDate.Text.Trim().Length != 10)
            {
                lblMessage.InnerText = "warning-> Select To Date";
                txtFromDate.Focus();
                return;
            }
            SearchAttendanceList();
        }
        private void SearchAttendanceList()
        {
            try
            {
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                lblMessage.InnerText = "";
                int Days = (DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim())) - DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()))).Days+1;
                string sql = @"select EmpId,EmpCardNo,EmpProximityNo as RegId,EmpName,count(EmpId) as AttDays from v_tblAttendanceRecord 
where IsActive=1 and EmpJoiningDate<'"+commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim())+ @"' and ATTDate>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + @"' and ATTDate<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + @"'
group by EmpId,EmpCardNo,EmpProximityNo,EmpName having count(EmpId)<"+ Days.ToString();
                //------------------------------------------------------------------------------------------------
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(sql);
                if (dt==null || dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Data not found.";
                    gvAttendanceList.DataSource = null;
                    gvAttendanceList.DataBind();
                    return;
                }
                gvAttendanceList.DataSource = dt;
                gvAttendanceList.DataBind();

            }
            catch { }
        }



    }
}