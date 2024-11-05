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

namespace SigmaERP.payroll
{
    public partial class bonus_summary_report : System.Web.UI.Page
    {
        DataTable dt;
        //permission=393
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 391 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.loadEmpTye(rblEmployeeType);
                rblEmployeeType.SelectedValue = "1";
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                ViewState["__IsGerments__"] = classes.commonTask.IsGarments();
                //if (!bool.Parse(ViewState["__IsGerments__"].ToString()))
                //    trHideForIndividual.Visible = false;
            }
        }

        DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];

                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();

                if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Master Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Viewer"))
                {

                    classes.commonTask.LoadBranch(ddlCompanyName);
                    classes.commonTask.LoadDepartmentByCompanyInListBox(ViewState["__CompanyId__"].ToString(), lstAll);
                   // classes.commonTask.LoadShift(ddlShiftName, ViewState["__CompanyId__"].ToString());

                }
                else
                {
                    chkForAllCompany.Visible = false;
                    dtSetPrivilege = new DataTable();
                    chkForAllCompany.Enabled = true;
                    classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                    classes.commonTask.LoadDepartmentByCompanyInListBox(ViewState["__CompanyId__"].ToString(), lstAll);
                   // classes.commonTask.LoadShift(ddlShiftName, ViewState["__CompanyId__"].ToString());

                    //if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Admin"))
                    //{
                    //    btnPreview.CssClass = ""; btnPreview.Enabled = false;
                    //}

                    //sqlDB.fillDataTable("select * from UserPrivilege where PageName='bonus_summary_report.aspx' and UserId=" + getCookies["__getUserId__"].ToString() + "", dtSetPrivilege);
                    //if (dtSetPrivilege.Rows.Count > 0)
                    //{
                    //    if (bool.Parse(dtSetPrivilege.Rows[0]["ReadAction"].ToString()).Equals(true))
                    //    {
                    //        btnPreview.CssClass = "Pbutton"; btnPreview.Enabled = true;
                    //    }
                    //    else
                    //    {
                    //        tblGenerateType.Visible = false;
                    //        WarningMessage.Visible = true;
                    //        btnPreview.CssClass = ""; btnPreview.Enabled = false;
                    //    }

                    //}
                    //else
                    //{
                    //    tblGenerateType.Visible = false;
                    //    WarningMessage.Visible = true;
                    //    btnPreview.CssClass = ""; btnPreview.Enabled = false;
                    //}

                }

                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                classes.Payroll.loadBonusList(ddlBonusType, CompanyId);
                //  addAllTextInShift();

            }
            catch { }
        }
      
        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (ddlBonusType.SelectedValue == "0") { lblMessage.InnerText = "warning->Please select any Bonus Type!"; ddlBonusType.Focus(); return; }
            if (lstSelected.Items.Count == 0) { lblMessage.InnerText = "warning->Please select any Department!"; lstSelected.Focus(); return; }
            generateBonusSummary();
        }
        private void generateBonusSummary()
        {
            try
            {
                string CompanyList = "";                
                string DepartmentList = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }


                //if (chkForAllCompany.Checked)
                //{
                //    CompanyList = classes.Payroll.getCompanyList(ddlCompanyName);
                //    ShiftList = classes.Payroll.getSftIdList(ddlShiftName);
                //    DepartmentList = classes.commonTask.getDepartmentList();
                //}
                //else
                //{
                    CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();

               

                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                //}
                    string Condition = (bool.Parse(ViewState["__IsGerments__"].ToString())) ? "And EmpTypeId=" + rblEmployeeType.SelectedValue + "" : "";
                string getSQLCMD;
                DataTable dt = new DataTable();
                if (!bool.Parse(ViewState["__IsGerments__"].ToString()))
                {
                    getSQLCMD = "Select CompanyId,CompanyName,Address, DptId,DptCode,DptName, count(EmpId) as Year, Sum(BonusAmount) as BonusAmount " +
                        "From v_Payroll_YearlyBonusSheet  " +
                        "Where CompanyId in(" + CompanyList + ")  AND BID='" + ddlBonusType.SelectedValue + "' AND dptId  " + DepartmentList + " and EmpTypeId=" + rblEmployeeType.SelectedValue + "    and BonusAmount<>0  " +
                        "Group By  CompanyId,CompanyName,Address,DptId,DptCode,DptName " +
                        "Order by convert(int,DptId)";
                }
                else 
                {

                    //getSQLCMD = "Select CompanyId,CompanyName,Address, DptId,DptCode,DptName,GName,GID, count(EmpId) as Year, Sum(BonusAmount) as BonusAmount " +
                    //    "From v_Payroll_YearlyBonusSheet  " +
                    //    "Where CompanyId in(" + CompanyList + ")  AND BonusType='" + ddlBonusType.SelectedItem.Value.ToString() + "' AND dptId  " + DepartmentList + "  and BonusAmount<>0  " + Condition + " " +
                    //    "Group By  CompanyId,CompanyName,Address,DptId,DptCode,DptName,GName,GID " +
                    //    "ORder by convert(int,DptId),convert(int,Gid)";
                    getSQLCMD = "Select CompanyId,CompanyName,Address, DptId,DptCode,DptName, count(EmpId) as Year, Sum(BonusAmount) as BonusAmount " +
                        "From v_Payroll_YearlyBonusSheet  " +
                        "Where CompanyId in(" + CompanyList + ")  AND BID='" + ddlBonusType.SelectedValue+ "' AND dptId  " + DepartmentList + " and EmpTypeId=" + rblEmployeeType.SelectedValue + "   and BonusAmount<>0  " +
                        "Group By  CompanyId,CompanyName,Address,DptId,DptCode,DptName " +
                        "Order by convert(int,DptId)";
                }
                sqlDB.fillDataTable(getSQLCMD, dt);
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Data not found."; return;
                }

                Session["__Language__"] = "English";
                Session["__SummaryOfBonus__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=SummaryOfBonus-" + ddlBonusType.SelectedItem.Text.Trim() + "-" + ViewState["__IsGerments__"] .ToString()+ "-"+ddlCompanyName.SelectedValue+"-"+rblEmployeeType.SelectedItem.Text+"');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }

        protected void ddlShiftName_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                
                lstAll.Items.Clear();
                lstSelected.Items.Clear();
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();

                if (ddlShiftName.SelectedItem.ToString().Equals("All"))
                {

                    string ShiftList = classes.commonTask.getShiftList(ddlShiftName);
                    classes.commonTask.LoadDepartmentByCompanyAndShiftInListBox(CompanyId, ShiftList, lstAll);
                    return;
                }
                classes.commonTask.LoadDepartmentByCompanyAndShiftInListBox(CompanyId, "in (" + ddlShiftName.SelectedValue.ToString() + ")", lstAll);
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

            try
            {
                
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                classes.commonTask.LoadDepartmentByCompanyInListBox(CompanyId, lstAll);
                lstSelected.Items.Clear();
                //classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, CompanyId);
                //classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                //addAllTextInShift();
                classes.Payroll.loadBonusList(ddlBonusType, CompanyId);
            }
            catch { }
        }

       
    }
}