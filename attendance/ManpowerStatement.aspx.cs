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
    public partial class ManpowerStatement : System.Web.UI.Page
    {
        DataTable dt;        
        string CompanyId = "";
        string sqlCmd = "";
        //permisssion=323;
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 323 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

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

                //------------load privilege setting inof from db------
                //------------load privilege setting inof from db------
                classes.commonTask.LoadBranch(ddlCompany, ViewState["__CompanyId__"].ToString());
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "daily_movement.aspx", ddlCompany, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];
                classes.commonTask.LoadShiftNameByCompany(ViewState["__CompanyId__"].ToString(), ddlShift);
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
                //-----------------------------------------------------
            }
            catch { }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            GenerateReportInEnglish();         

        }


        private void GenerateReportInEnglish()
        {
            if (lstSelected.Items.Count == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
         
          

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";            
            string DepartmentList = "";
            CompanyList = "in ('" + CompanyId + "')";
            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            sqlCmd = "select CompanyId, CompanyName,Address,DptId, DptName,DsgId,DsgName, sum( case when AttStatus in('P','L') then 1 else 0 end) as P, sum(case when AttStatus in('Lv') then 1 else 0 end) as Lv, sum(case when AttStatus in('A') then 1 else 0 end) as A, sum( case when AttStatus in('W','H') then 1 else 0 end) as WH from v_tblAttendanceRecord1 where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and CompanyId " + CompanyList + "   AND DptId " + DepartmentList + " " + EmpTypeID + "  " + ShiftName + "   group by CompanyId, CompanyName,Address,DptId, DptName,DsgId,DsgName";
            sqlDB.fillDataTable(sqlCmd,dt=new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__AttManpowerStatement__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=AttManpowerStatement-" + txtDate.Text.Replace('-','/') + "');", true);  //Open New Tab for Sever side code

        }
        
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }

        private void addAllTextInShift()
        {
            //    if (ddlShiftList.Items.Count > 2)
            //        ddlShiftList.Items.Insert(1, new ListItem("All", "00"));
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            classes.commonTask.LoadShiftNameByCompany(CompanyId, ddlShift);
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
            lstSelected.Items.Clear();           
        }
 }
}