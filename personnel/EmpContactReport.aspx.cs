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

namespace SigmaERP.personnel
{
    public partial class EmpContactReport : System.Web.UI.Page
    {
        DataTable dt;
        DataTable dtSetPrivilege;
        string CompanyId = "";
        string Cmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 275 };
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
                //string[] AccessPermission = new string[0];
                //System.Web.UI.HtmlControls.HtmlTable a = tblGenerateType;
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "EmpContactReport.aspx", ddlCompany, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];

               
                classes.commonTask.LoadInitialShift(ddlShiftList, ViewState["__CompanyId__"].ToString());  
                addAllTextInShift();
                ddlCompany.SelectedValue = ViewState["__CompanyId__"].ToString();
                classes.commonTask.LoadDepartment(ddlCompany.SelectedValue, lstAll);               
                classes.commonTask.LoadEmpCardNoByEmpType(ddlCardNo, ddlCompany.SelectedValue, rblEmpType.SelectedValue);
                if (ddlCardNo != null)
                    ddlCardNo.Items.Insert(0, new ListItem("Select For Individual", "0"));
            }
            catch { }
        }
        private void addAllTextInShift()
        {
            ddlShiftList.Items.RemoveAt(0);
            if (ddlShiftList.Items.Count > 1)
                ddlShiftList.Items.Insert(0, new ListItem("All", "00"));
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            classes.commonTask.LoadInitialShift(ddlShiftList, CompanyId);
            addAllTextInShift();
            lstSelected.Items.Clear();
            classes.commonTask.LoadDepartment(CompanyId, lstAll); 
            classes.commonTask.LoadEmpCardNoByEmpType(ddlCardNo, CompanyId, rblEmpType.SelectedValue);
            ddlCardNo.Items.Insert(0, new ListItem("Select For Individual", "0"));
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if(rblLanguage.SelectedValue=="EN")
            gererateContactReport();
            else
                gererateContactReportBangla();
        }

        private void gererateContactReport() 
        {
            string ownempId = AccessControl.hasOwnEmpIdWithOtherDepartment();
            if (ddlCardNo.SelectedValue=="0") { 
            if (ddlShiftList.SelectedItem.Text.Trim() == "")
            {
                lblMessage.InnerText = "warning-> Please Select Any Shift!";
                ddlShiftList.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
                return;
            }
            if (lstSelected.Items.Count == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
                return;
            }
            //  string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            if (ddlShiftList.SelectedItem.ToString().Equals("All"))
                ShiftList = classes.commonTask.getShiftList(ddlShiftList);
            else
                ShiftList = "in ('" + ddlShiftList.SelectedValue.ToString() + "')";

            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                
                string EmpTypeID=(rblEmpType.SelectedValue.ToString().Equals("All"))?"":" and EmpTypeId="+ rblEmpType.SelectedValue+"";
            if(rblReportType.SelectedValue=="0")
            Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName,DsgName, EmpId, SUBSTRING(EmpCardNo,8,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,MobileNo,convert(varchar(10),EmpJoiningDate,105) as EmpJoiningDate from v_EmployeeDetails " +
                "where IsActive=1 and CompanyId='" + CompanyId + "' and SftId " + ShiftList + " and DptId " + DepartmentList + " and EmpStatus in(1,8)  "+ ownempId + " order by CustomOrdering";
            else
                Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName, EmpId, SUBSTRING(EmpCardNo,8,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,ContactName,EmergencyAddress, EmergencyPhoneNo from V_EmpContactInfo " +
                "where IsActive=1 and CompanyId='" + CompanyId + "' and SftId " + ShiftList + " and DptId " + DepartmentList + " and EmpStatus in(1,8) " + EmpTypeID + " "+ ownempId + " order by CustomOrdering";
            }
            else
            {
                
                CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
                if (rblReportType.SelectedValue == "0")
                    Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName,DsgName, EmpId, SUBSTRING(EmpCardNo,8,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,MobileNo,convert(varchar(10),EmpJoiningDate,105) as EmpJoiningDate from v_EmployeeDetails " +
                        "where  IsActive=1 and CompanyId='" + CompanyId + "' and SN="+ddlCardNo.SelectedValue+" and EmpStatus in(1,8) "+ ownempId + "";
                else
                    Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName, EmpId, SUBSTRING(EmpCardNo,8,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,ContactName,EmergencyAddress, EmergencyPhoneNo from V_EmpContactInfo " +
                    "where  IsActive=1 and CompanyId='" + CompanyId + "'  and SN=" + ddlCardNo.SelectedValue + " and EmpStatus in(1,8) "+ ownempId + "";
               
            }
            sqlDB.fillDataTable(Cmd, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Any Records Are Not Founded !";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
                return;
            }
            Session["__ContacList__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=ContactList-" + rblReportType.SelectedValue + "');", true);  //Open New Tab for Sever side code
        
        }
        private void gererateContactReportBangla()
        {
            if (ddlCardNo.SelectedValue == "0")
            {
                if (ddlShiftList.SelectedItem.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning-> Please Select Any Shift!";
                    ddlShiftList.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                    return;
                }
                if (lstSelected.Items.Count == 0)
                {
                    lblMessage.InnerText = "warning-> Please Select Any Department!";
                    lstSelected.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                    return;
                }
                //  string CompanyList = "";
                string ShiftList = "";
                string DepartmentList = "";
                CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
                if (ddlShiftList.SelectedItem.ToString().Equals("All"))
                    ShiftList = classes.commonTask.getShiftList(ddlShiftList);
                else
                    ShiftList = "in ('" + ddlShiftList.SelectedValue.ToString() + "')";

                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                string EmpTypeID = (rblEmpType.SelectedValue.ToString().Equals("All")) ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + "";
                if (rblReportType.SelectedValue == "0")
                    Cmd = "select CompanyId,CompanyNameBangla CompanyName,AddressBangla Address,DptId,DptNameBn DptName,SftId, SftName,DsgNameBn DsgName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpNameBn EmpName,MobileNo from v_EmployeeDetails " +
                        "where  IsActive=1 and CompanyId='" + CompanyId + "' and SftId " + ShiftList + " and DptId " + DepartmentList + " and EmpStatus in(1,8) " + EmpTypeID + " order by CustomOrdering";
                else
                    Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpName,ContactName,EmergencyAddress, EmergencyPhoneNo from v_EmployeeDetails " +
                    "where IsActive=1 and CompanyId='" + CompanyId + "' and SftId " + ShiftList + " and DptId " + DepartmentList + " and EmpStatus in(1,8) " + EmpTypeID + " order by CustomOrdering";
            }
            else
            {
                //if (txtCardNo.Text.Length < 4)
                //{
                //    lblMessage.InnerText = "warning-> Please Type Mininmum 4 Character of Card No !";
                //    txtCardNo.Focus(); return;

                //}
                CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
                if (rblReportType.SelectedValue == "0")
                    Cmd = "select CompanyId,CompanyNameBangla CompanyName,AddressBangla Address,DptId,DptNameBn DptName,SftId, SftName,DsgNameBn DsgName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpNameBn EmpName,MobileNo from v_EmployeeDetails " +
                        "where IsActive=1 and CompanyId='" + CompanyId + "' and SN=" + ddlCardNo.SelectedValue + " and EmpStatus in(1,8)";
                else
                    Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpName,ContactName,EmergencyAddress, EmergencyPhoneNo from v_EmployeeDetails " +
                    "where IsActive=1 and CompanyId='" + CompanyId + "'  and SN=" + ddlCardNo.SelectedValue + " and EmpStatus in(1,8)";

            }
            sqlDB.fillDataTable(Cmd, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Any Records Are Not Founded !";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                return;
            }
            Session["__ContacListBangla__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=ContactListBangla-" + rblReportType.SelectedValue + "');", true);  //Open New Tab for Sever side code

        }
        protected void rblEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;          
            classes.commonTask.LoadEmpCardNoByEmpType(ddlCardNo, CompanyId, rblEmpType.SelectedValue);
            ddlCardNo.Items.Insert(0, new ListItem("Select For Individual", "0"));
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
        }

       
    }
}