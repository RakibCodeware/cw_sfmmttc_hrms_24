using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.leave
{
    public partial class shortleave_report : System.Web.UI.Page
    {
        DataTable dt;
        string CompanyId = "";
        string SqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
           
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                txtFromDate.Text = "01-" + DateTime.Now.ToString("MM-yyyy");
                txtToDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                setPrivilege();               
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
        }
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                string[] AccessPermission = new string[0];                
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "yearly_leaveStatus_report.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
                ViewState["__ReadAction__"] = AccessPermission[0];
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
            }
            catch { }

        }
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompanyId = (ddlCompanyName .SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;
            lstSelected.Items.Clear();
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            ShorLeaveReport();
        }
        private void ShorLeaveReport()
        {
            if (txtFromDate.Text.Trim() == "")
            {
                lblMessage.InnerText = "warning->Select from date.";
                txtFromDate.Focus();
                return;
            }
            if (txtToDate.Text.Trim() == "")
            {
                lblMessage.InnerText = "warning->Select to date.";
                txtToDate.Focus();
                return;
            }            
            CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;
            if (txtCardNo.Text.Trim().Length == 0)
            {
                if (lstSelected.Items.Count == 0)
                {
                    lblMessage.InnerText = "warning->Select department.";
                    lstAll.Focus();
                    return;
                }
                string DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                SqlCmd = "SELECT EmpName, substring(EmpCardNo,8,6) as EmpCardNo, EmpId,convert(varchar(10),LvDate,105) as LvDate, FromTime, ToTime, LvTime, Remarks, CompanyName, Address, DsgName, DptId, DptName FROM v_Leave_ShortLeave where IsActive=1 and LvStatus=1 and CompanyId='" + CompanyId + "' and LvDate>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "' and LvDate<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and DptID " + DepartmentList + " order by LvDate ";
            }
            else
            {
                if (txtCardNo.Text.Trim().Length < 6)
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + 6 + " Digits)";
                    txtCardNo.Focus();
                    return;
                }
                SqlCmd = "SELECT EmpName, substring(EmpCardNo,8,6) as EmpCardNo, EmpId,convert(varchar(10),LvDate,105) as LvDate, FromTime, ToTime, LvTime, Remarks, CompanyName, Address, DsgName, DptId, DptName FROM v_Leave_ShortLeave where IsActive=1 and LvStatus=1 and CompanyId='" + CompanyId + "' and LvDate>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "' and LvDate<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' order by LvDate ";
            }
            sqlDB.fillDataTable(SqlCmd, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Data Not Found.";
                return;
            }
            Session["__ShorLeaveReport__"] = dt;
            string DateRange = txtFromDate.Text.Trim() + " to " + txtToDate.Text.Trim();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=ShorLeaveReport-" + DateRange.Replace('-', '/') + "');", true);  //Open New Tab for Sever side code


        }
    }
}