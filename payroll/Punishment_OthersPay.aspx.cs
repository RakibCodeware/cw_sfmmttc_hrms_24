using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class Punishment_OthersPay : System.Web.UI.Page
    {
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                setPrivilege();
                ViewState["__IsChanged__"] = "no";
                Session["OPERATION_PROGRESS"] = 0;              
                if (!classes.commonTask.HasBranch())
                {
                    ddlCompanyList.Enabled = false;
                    ddlCompanyList2.Enabled = false;
                }
            }
        }


        private void setPrivilege()
        {
            try
            {

                ViewState["__WriteAction__"] = "1";
                ViewState["__DeletAction__"] = "1";
                ViewState["__ReadAction__"] = "1";
                ViewState["__UpdateAction__"] = "1";
                ViewState["__preRIndex__"] = "No";
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();


                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForpayrollentrypanel(getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "payroll_entry_panel.aspx", gvpunishment, btnSave, ViewState["__CompanyId__"].ToString(), ddlCompanyList, ddlCompanyList2);

                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                ddlCompanyList2.SelectedValue = ViewState["__CompanyId__"].ToString();

                classes.Employee.LoadEmpCardNoForPayrollwithEmpId(ddlEmpCardNo, ddlCompanyList.SelectedValue);
                classes.Employee.LoadEmpCardNoForPayrollwithEmpId(ddlEmpCardNo2, ddlCompanyList2.SelectedValue);

                loadPunishment();
                loadotherpay();

            }
            catch { }

        }
        private void loadPunishment()
        {
            DataTable dtPunishment = new DataTable();
            sqlDB.fillDataTable("Select * from v_Payroll_Punishment where CompanyId='"+ddlCompanyList.SelectedValue+"'", dtPunishment);
            gvpunishment.DataSource = dtPunishment;
            gvpunishment.DataBind();
        }
        private void loadotherpay()
        {
            DataTable dtotherpay = new DataTable();
            sqlDB.fillDataTable("Select * from v_Payroll_OthersPay where CompanyId='" + ddlCompanyList2.SelectedValue + "'", dtotherpay);
            gvotherspay.DataSource = dtotherpay;
            gvotherspay.DataBind();
        }
        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            classes.Employee.LoadEmpCardNoForPayrollwithEmpId(ddlEmpCardNo, ddlCompanyList.SelectedValue);
            loadPunishment();

        }
        protected void ddlCompanyList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            classes.Employee.LoadEmpCardNoForPayrollwithEmpId(ddlEmpCardNo2, ddlCompanyList2.SelectedValue);

        }
        static DataTable dt_AllowanceSettings = new DataTable();
       
       
        protected void btnSave_Click(object sender, EventArgs e)
        {

            if(btnSave.Text=="Update")
            {
                UpdatePunishment();
            }
            else
            {
                savePunishment();
            }
            
        }


        private void savePunishment()
        {
            try
            {                
                string[] getColumns = { "CompanyId", "EmpId", "PName", "PAmount", "MonthName"};
                string[] getValues = { ddlCompanyList.SelectedValue,ddlEmpCardNo.SelectedValue, txtpunishment.Text, txtPAmount.Text,txtmonthname.Text};

                if (SQLOperation.forSaveValue("Payroll_Punishment", getColumns, getValues, sqlDB.connection) == true)
                {
                    if (ViewState["__WriteAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Pbutton";
                    }
                    allClearPunishment();
                    loadPunishment();

                    // lblMessage.InnerText = "success->Successfully Submitted.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow('success','Successfully Submitted.');", true);
                }
                else
                {

                    // lblMessage.InnerText = "error->Unable to Submit.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Submit.');", true);
                }

            }
            catch
            {
               
                //  lblMessage.InnerText = "error->Unable to Submit."; 
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Submit.');", true);
            }

        }
        private void UpdatePunishment()
        {
            try
            {
                string[] getColumns = { "CompanyId", "EmpId", "PName", "PAmount", "MonthName" };
                string[] getValues = { ddlCompanyList.SelectedValue, ddlEmpCardNo.SelectedValue, txtpunishment.Text, txtPAmount.Text, txtmonthname.Text };

                if (SQLOperation.forUpdateValue("Payroll_Punishment", getColumns, getValues, "PSN", ViewState["__getSL__"].ToString(), sqlDB.connection) == true)
                {
                    if (ViewState["__WriteAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Pbutton";
                    }
                    allClearPunishment();
                    loadPunishment();

                    // lblMessage.InnerText = "success->Successfully Submitted.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow('success','Successfully Update.');", true);
                }
                else
                {

                    // lblMessage.InnerText = "error->Unable to Submit.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Update.');", true);
                }

            }
            catch
            {

                //  lblMessage.InnerText = "error->Unable to Submit."; 
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Submit.');", true);
            }

        }


       

        protected void ddlEmpCardNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEmpCardNo.SelectedIndex != 0)
               // IndividualSalary();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
        }

      
        private void allClearPunishment()
        {
            txtpunishment.Text = "";
            txtPAmount.Text = "0";
            txtmonthname.Text = "";
            btnSave.Text = "Submit";
        }

        private void loadSalaryInfo(string companyId)
        {
            try
            {
                SQLOperation.selectBySetCommandInDatatable("select distinct EmpCardNo,EmpName, max(SN) as SN,EmpType,EmpStatus,EmpTypeId,BasicSalary,MedicalAllownce,HouseRent ,EmpPresentSalary,CompanyId," +
                    " ActiveSalary,IsActive,PFAmount,case when SalaryCount='False' Then 'Cash' Else case when SalaryCount='True' then 'Bank' else 'Check' End End As SalaryCount from v_Personnel_EmpCurrentStatus group by EmpCardNo,EmpName,EmpTypeId,EmpType,BasicSalary,MedicalAllownce,HouseRent ," +
                    " EmpPresentSalary,EmpStatus,CompanyId,ActiveSalary,IsActive,PFAmount,SalaryCount having EmpStatus in('1','8') AND ActiveSalary='true' AND IsActive='1' AND CompanyId='" + companyId + "' order by SN ", dt = new DataTable(), sqlDB.connection);
                //gvSalaryList.DataSource = dt;
                //gvSalaryList.DataBind();
                ViewState["__IsChanged__"] = "no";
            }
            catch { }
        }

        protected void tc1_ActiveTabChanged(object sender, EventArgs e)
        {

            if (tc1.ActiveTabIndex == 1)
            {
               // txtFinding.Visible = true;
                //if (gvSalaryList.Rows.Count == 0 || ViewState["__IsChanged__"].ToString().Equals("yes")) loadSalaryInfo(ddlCompanyList2.SelectedValue);
            }
            //else txtFinding.Visible = false;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
        }

        protected void gvSalaryList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onmouseover"] = "javascript:SetMouseOver(this)";
                    e.Row.Attributes["onmouseout"] = "javascript:SetMouseOut(this)";
                }
            }
            catch { }
            if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            {


                try
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        Button btn = (Button)e.Row.FindControl("btnEdit");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;
                    }

                }
                catch { }
            }
        }     
        protected void gvpunishment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

                if (e.CommandName.Equals("Alter"))
                {
                    string a = ViewState["__preRIndex__"].ToString();
                    if (!ViewState["__preRIndex__"].ToString().Equals("No")) gvpunishment.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    gvpunishment.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                    ViewState["__preRIndex__"] = rIndex;
                    setValueToControl(rIndex, gvpunishment.DataKeys[rIndex].Values[0].ToString(), gvpunishment.DataKeys[rIndex].Values[1].ToString(), gvpunishment.DataKeys[rIndex].Values[2].ToString());
                    btnSave.Text = "Update";
                    if (ViewState["__UpdateAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Pbutton";
                    }

                }
                else if (e.CommandName.Equals("deleterow"))
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    SQLOperation.forDeleteRecordByIdentifier("Payroll_Punishment", "PSN", gvpunishment.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "deleteSuccess()", true);
                    allClearPunishment();
                    lblMessage.InnerText = "success->Successfully  Deleted";
                    gvpunishment.Rows[rIndex].Visible = false;

                }
            }
            catch { }
        }
        private void setValueToControl(int rIndex, string getSL, string getCompanyId,string EmpId)
        {
            try
            {
                ViewState["__getSL__"] = getSL;
               ddlCompanyList.SelectedValue = getCompanyId;
               ddlEmpCardNo.SelectedValue = EmpId;
               txtpunishment.Text = gvpunishment.Rows[rIndex].Cells[3].Text;
               txtPAmount.Text = gvpunishment.Rows[rIndex].Cells[4].Text;
               txtmonthname.Text = gvpunishment.Rows[rIndex].Cells[5].Text;
              


            }
            catch { }
        }

        protected void btnSave2_Click(object sender, EventArgs e)
        {
            if (btnSave2.Text == "Update")
            {
                Updateotherpay();
            }
            else
            {
                saveotherpay();
            }
        }
        private void saveotherpay()
        {
            try
            {
                string active = "";
                if(checkActive.Checked)
                {
                    active = "1";
                }
                else
                {
                    active = "0";
                }
                string[] getColumns = { "CompanyId", "EmpId", "OPpurpose", "OtherPay", "IsActive" };
                string[] getValues = { ddlCompanyList2.SelectedValue, ddlEmpCardNo2.SelectedValue, txtpurpose.Text, txtotherpayAmount.Text, active };

                if (SQLOperation.forSaveValue("Payroll_OthersPay", getColumns, getValues, sqlDB.connection) == true)
                {
                    if (ViewState["__WriteAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Pbutton";
                    }
                    allClearOtherpay();
                    loadotherpay();

                    // lblMessage.InnerText = "success->Successfully Submitted.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow('success','Successfully Submitted.');", true);
                }
                else
                {

                    // lblMessage.InnerText = "error->Unable to Submit.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Submit.');", true);
                }

            }
            catch
            {

                //  lblMessage.InnerText = "error->Unable to Submit."; 
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Submit.');", true);
            }

        }
        private void Updateotherpay()
        {
            try
            {
                string active = "";
                if (checkActive.Checked)
                {
                    active = "1";
                }
                else
                {
                    active = "0";
                }
                string[] getColumns = { "CompanyId", "EmpId", "OPpurpose", "OtherPay", "IsActive" };
                string[] getValues = { ddlCompanyList2.SelectedValue, ddlEmpCardNo2.SelectedValue, txtpurpose.Text, txtotherpayAmount.Text, active };

                if (SQLOperation.forUpdateValue("Payroll_OthersPay", getColumns, getValues, "OPSN", ViewState["__getSL__"].ToString(), sqlDB.connection) == true)
                {
                    if (ViewState["__WriteAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Pbutton";
                    }
                    allClearOtherpay();
                    loadotherpay();

                    // lblMessage.InnerText = "success->Successfully Submitted.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow('success','Successfully Update.');", true);
                }
                else
                {

                    // lblMessage.InnerText = "error->Unable to Submit.";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Update.');", true);
                }

            }
            catch
            {

                //  lblMessage.InnerText = "error->Unable to Submit."; 
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow(error,'Unable to Submit.');", true);
            }

        }
        private void allClearOtherpay()
        {
            txtpurpose.Text = "";
            txtotherpayAmount.Text = "";
            checkActive.Checked = true;
            btnSave2.Text = "Submit";
        }

        protected void gvotherspay_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            try
            {

                if (e.CommandName.Equals("Alter"))
                {
                    string a = ViewState["__preRIndex__"].ToString();
                    if (!ViewState["__preRIndex__"].ToString().Equals("No")) gvotherspay.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    gvotherspay.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                    ViewState["__preRIndex__"] = rIndex;
                    setValueToControl2(rIndex, gvotherspay.DataKeys[rIndex].Values[0].ToString(), gvotherspay.DataKeys[rIndex].Values[1].ToString(), gvpunishment.DataKeys[rIndex].Values[2].ToString());
                    btnSave2.Text = "Update";
                    if (ViewState["__UpdateAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Pbutton";
                    }

                }
                else if (e.CommandName.Equals("deleterow"))
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    SQLOperation.forDeleteRecordByIdentifier("Payroll_OthersPay", "OPSN", gvotherspay.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "deleteSuccess()", true);
                    allClearPunishment();
                    lblMessage.InnerText = "success->Successfully  Deleted";
                    gvotherspay.Rows[rIndex].Visible = false;

                }
            }
            catch { }
        }
        private void setValueToControl2(int rIndex, string getSL, string getCompanyId, string EmpId)
        {
            try
            {
                ViewState["__getSL__"] = getSL;
                ddlCompanyList2.SelectedValue = getCompanyId;
                ddlEmpCardNo2.SelectedValue = EmpId;
                txtpurpose.Text = gvotherspay.Rows[rIndex].Cells[3].Text;
                txtotherpayAmount.Text= gvotherspay.Rows[rIndex].Cells[4].Text;
                 if(gvotherspay.Rows[rIndex].Cells[5].Text=="Yes")
                 {
                     checkActive.Checked = true;
                 }
                 else
                 {
                     checkActive.Checked = false;
                 }
            }
            catch { }
        }
    }
}