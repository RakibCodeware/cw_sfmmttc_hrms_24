using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.pf
{
    public partial class pf_YearlyExpense : System.Web.UI.Page
    {
        //permission(View=379,Ad=380,Update=381 Delete=382)
        string CompanyId = "";
        string sqlcmd = "";
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {

           
            int[] pagePermission = { 379, 380, 381, 382 };

            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                ViewState["__ReadAction__"] = "0";
                ViewState["__WriteAction__"] = "0";
                ViewState["__UpdateAction__"] = "0";
                ViewState["__DeletAction__"] = "0";
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);
                setPrivilege(userPagePermition);


            }
        }
        private void setPrivilege(int[]permission)
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__preRIndex__"] = "No";
                string getUserId = getCookies["__getUserId__"].ToString();

                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());

                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "pf_YearlyExpense.aspx", ddlCompanyName, gvPFSettings, btnSave);

                if (permission.Contains(379))
                    ViewState["__ReadAction__"] = "1";
                if (permission.Contains(380))
                    ViewState["__WriteAction__"] = "1";
                if (permission.Contains(381))
                    ViewState["__UpdateAction__"] = "1";
                if (permission.Contains(382))
                    ViewState["__DeletAction__"] = "1";
                checkInitialPermission();
                commonTask.loadPFCompany(ddlCompanyName);
                //if (!classes.commonTask.HasBranch())
                //    ddlCompanyName.Enabled = false;
                //ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                loadExpense();
            }
            catch { }

        }
        private void SaveExpense()
        {
            try
            {
                SQLOperation.forDeleteRecordByIdentifier("PF_YearlyExpense", "Date",txtYear.Text.Trim()+"-12-31", sqlDB.connection);
                SqlCommand cmd1 = new SqlCommand("delete PF_YearlyExpense where Date='"+txtYear.Text.Trim()+"-12-31' and CompanyId='"+ddlCompanyName.SelectedValue+"'", sqlDB.connection);
                SqlCommand cmd = new SqlCommand("insert into PF_YearlyExpense values('" + ddlCompanyName.SelectedValue + "','" + txtYear.Text.Trim() + "-12-31','" + txtExpenseAmount.Text.Trim() + "')", sqlDB.connection);
                if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                {
                    loadExpense();                   
                        lblMessage.InnerText = "success-> Successfully "+btnSave.Text.Trim()+"d.";
                    allClear();
                }
                else
                    lblMessage.InnerText = "error-> Unable to "+btnSave.Text.Trim()+" !";
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error>" + ex.Message;
            }
        }
        private void loadExpense()
        {
            try
            {
                CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;

                sqlcmd = "select SL,CompanyId,Year(Date) as Date ,Expense from PF_YearlyExpense where CompanyId='" + CompanyId + "'";

                dt = new DataTable();
                sqlDB.fillDataTable(sqlcmd, dt);
                gvPFSettings.DataSource = dt;
                gvPFSettings.DataBind();

            }
            catch { }
        }
        private void allClear()
        {


            txtYear.Text = "";
            txtExpenseAmount.Text = "";
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
            btnSave.Text = "Save";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlCompanyName.SelectedValue == "0000")
            { lblMessage.InnerText = "warning-> Please select Company !"; ddlCompanyName.Focus(); return; }
            if (txtYear.Text.Trim() == "")
            { lblMessage.InnerText = "warning-> Please select Year !"; txtYear.Focus(); return; }
            if(txtExpenseAmount.Text.Trim()=="")
            { lblMessage.InnerText = "warning-> Please Enter Expense Amount !"; txtExpenseAmount.Focus(); return; }
            SaveExpense();
        }

        protected void gvPFSettings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.Equals("Alter"))
            {
                string a = ViewState["__preRIndex__"].ToString();
                if (!ViewState["__preRIndex__"].ToString().Equals("No")) gvPFSettings.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");


                gvPFSettings.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                ViewState["__preRIndex__"] = rIndex;
                ddlCompanyName.SelectedValue = gvPFSettings.DataKeys[rIndex].Values[0].ToString();
                txtYear.Text = gvPFSettings.Rows[rIndex].Cells[0].Text.ToString();
                txtExpenseAmount.Text = gvPFSettings.Rows[rIndex].Cells[1].Text.ToString();
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
                SQLOperation.forDeleteRecordByIdentifier("PF_YearlyExpense", "SL", gvPFSettings.DataKeys[rIndex].Values[1].ToString(), sqlDB.connection);
                //-------------------------- Delete Existing Record for this FDR------------------------------------------
                SqlCommand cmdDel = new SqlCommand("delete SGHRM.dbo.PF_Expense where Year(Month)='" + gvPFSettings.Rows[rIndex].Cells[0].Text.Trim().ToString() + "'", sqlDB.connection);
                SqlCommand cmdDel1 = new SqlCommand("delete SGFHRM.dbo.PF_Expense where Year(Month)='" + gvPFSettings.Rows[rIndex].Cells[0].Text.Trim().ToString() + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();
                cmdDel1.ExecuteNonQuery();
                //---------------------------------------------------------------------------------------------------------
                allClear();
                lblMessage.InnerText = "success->Successfully  Deleted";
                gvPFSettings.Rows[rIndex].Visible = false;
            }
        }

        protected void gvPFSettings_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (ViewState["__DeletAction__"].ToString().Equals("0"))
                {
                    LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                    lnkDelete.Enabled = false;
                    lnkDelete.OnClientClick = "return false";
                    lnkDelete.ForeColor = Color.Silver;
                }

            }
            catch { }
            try
            {
                if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                {
                    LinkButton lnkAlter = (LinkButton)e.Row.FindControl("lnkAlter");
                    lnkAlter.Enabled = false;
                    lnkAlter.ForeColor = Color.Silver;
                }

            }
            catch { }

        }
        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("0"))
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
    }
}