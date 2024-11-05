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
    public partial class outduty_report : System.Web.UI.Page
    {

        //permission=321;
        DataTable dt;
        DataTable dtSetPrivilege;
        string CompanyId = "";
        string SqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 321 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                txtFromDate.Text = "01-" + DateTime.Now.ToString("MM-yyyy");
                txtToDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompany.Enabled = false;
                ddlCompany.SelectedValue = ViewState["__CompanyId__"].ToString();
                Session["__MinDigits__"] = "6";

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
                classes.commonTask.LoadBranch(ddlCompany, ViewState["__CompanyId__"].ToString());
                //------------load privilege setting inof from db------
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "daily_movement.aspx", ddlCompany, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];                
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
                //-----------------------------------------------------
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

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            OutDutyReport();
        }
        private void OutDutyReport()
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
            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString():ddlCompany.SelectedValue;
            if (txtCardNo.Text.Trim().Length == 0)
            {
                if(lstSelected.Items.Count==0)
                {
                    lblMessage.InnerText = "warning->Select department.";
                    lstAll.Focus();
                    return;
                }
                string DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                SqlCmd = "select EmpId,substring(EmpCardNo,8,6) as EmpCardNo,EmpName,DsgName,DptId,DptName,CompanyId,CompanyName,Address,InTime,OutTime,Remark,AssignedBy,Place,convert(varchar(10),Date,105) as Date from v_tblOutDuty where Status=1 and CompanyId='" + CompanyId+"' and Date>='"+commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim())+ "' and Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and DptID "+DepartmentList +" "+ EmpTypeID + " order by Date ";
            }
              
            else
            {
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();                    
                    return;
                }
                bool condition = AccessControl.hasEmpcardPermission(txtCardNo.Text.Trim(), CompanyId);
                if (!condition)
                {
                    return;
                }
                SqlCmd = "select EmpId,substring(EmpCardNo,8,6) as EmpCardNo,EmpName,DsgName,DptId,DptName,CompanyId,CompanyName,Address,InTime,OutTime,Remark,AssignedBy,Place,convert(varchar(10),Date,105) as Date from v_tblOutDuty where Status=1 and CompanyId='" + CompanyId + "' and Date>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "' and Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' order by Date ";                
            }
            sqlDB.fillDataTable(SqlCmd, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Data Not Found.";               
                return;
            }
            Session["__OutDutyReport__"] = dt;
            string DateRange = txtFromDate.Text.Trim() + " to " + txtToDate.Text.Trim();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=OutDutyReport-" + DateRange.Replace('-','/') +"');", true);  //Open New Tab for Sever side code

        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            lstSelected.Items.Clear();
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
        }
    }
}