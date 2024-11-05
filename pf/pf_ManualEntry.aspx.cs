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
    public partial class pf_ManualEntry : System.Web.UI.Page
    {
        string CompanyId = "";
        string sqlcmd = "";

        //permission(View=367 Add=368 Update=369 Delete=370)
        protected void Page_Load(object sender, EventArgs e)
        {
           
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            int[] pagePermission = { 367,368,369,370 };
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
        private void setPrivilege(int[] permission)
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
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "pf_ManualEntry.aspx", ddlCompanyName, gvMonthlyPFList, btnSave);

                //ViewState["__ReadAction__"] = AccessPermission[0];
                if (permission.Contains(367))
                    ViewState["__ReadAction__"] = "1";
                if(permission.Contains(368))
                    ViewState["__WriteAction__"] = "1";
                if(permission.Contains(369))
                    ViewState["__UpdateAction__"] = "1";
                if(permission.Contains(370))
                    ViewState["__DeletAction__"] = "1";

                classes.commonTask.loadPFMemberList(ddlEmployeeList, ViewState["__CompanyId__"].ToString());
                loadPFRecord();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();

            }
            catch { }

        }
        private void loadPFRecord()
        {
            try
            {
                CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;

                if(ddlEmployeeList.SelectedIndex<1)
                sqlcmd = "select PF_PFRecord.SL,PF_PFRecord.EmpId,SUBSTRING(EmpCardNo,8,10) EmpCardNo,EmpName,format(Month,'MM-yyyy') Month,EmpContribution,EmprContribution,CompanyId from PF_PFRecord inner join Personnel_EmployeeInfo on PF_PFRecord.EmpID=Personnel_EmployeeInfo.EmpId where CompanyId='" + CompanyId + "' order by convert(varchar(10),Month,120) desc";
                else
                    sqlcmd = "select PF_PFRecord.SL,PF_PFRecord.EmpId,SUBSTRING(EmpCardNo,8,10) EmpCardNo,EmpName,format(Month,'MM-yyyy') Month,EmpContribution,EmprContribution,CompanyId from PF_PFRecord inner join Personnel_EmployeeInfo on PF_PFRecord.EmpID=Personnel_EmployeeInfo.EmpId where CompanyId='" + CompanyId + "' and PF_PFRecord.EmpID='" + ddlEmployeeList.SelectedValue + "' order by convert(varchar(10),Month,120) desc";
                DataTable dt = new DataTable();
                sqlDB.fillDataTable(sqlcmd, dt);
                gvMonthlyPFList.DataSource = dt;
                gvMonthlyPFList.DataBind();

            }
            catch { }
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;
            classes.commonTask.loadPFMemberList(ddlEmployeeList, CompanyId);
            loadPFRecord();
        }
        private void SavePFRecord()
        {
            try
            {
                
                SqlCommand cmdDel = new SqlCommand("Delete PF_PFRecord where convert(varchar(10),Month,105)='01-" + txtMonth.Text.Trim() + "' and EmpID='" + ddlEmployeeList.SelectedValue + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();

                SqlCommand cmd = new SqlCommand("insert into PF_PFRecord values('" + ddlEmployeeList.SelectedValue + "','" + convertDateTime.getCertainCulture("01-" + txtMonth.Text.Trim()).ToString() + "','" +
                    txtEmpContribution.Text.Trim() + "','" + txtEmpContribution.Text.Trim() + "','1') ", sqlDB.connection);
                
                if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                {
                    loadPFRecord();
                    //-------------------------------
                    lblMessage.InnerText = "success-> Successfully "+btnSave.Text.Trim()+"d.";                   
                    allClear();
                }
                else
                    lblMessage.InnerText = "error-> Unable to " + btnSave.Text.Trim() + " !";
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
            }
        }
        private void allClear()
        {

            txtEmpContribution.Text = "";
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
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            SavePFRecord();            
        }

        protected void gvMonthlyPFList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
            if (e.CommandName.Equals("Alter"))
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                string a = ViewState["__preRIndex__"].ToString();
                if (!ViewState["__preRIndex__"].ToString().Equals("No")) gvMonthlyPFList.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");


                gvMonthlyPFList.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                ViewState["__preRIndex__"] = rIndex;

                ddlCompanyName.SelectedValue = gvMonthlyPFList.DataKeys[rIndex].Values[0].ToString();
                ddlEmployeeList.SelectedValue = gvMonthlyPFList.DataKeys[rIndex].Values[1].ToString();
                txtMonth.Text = gvMonthlyPFList.Rows[rIndex].Cells[2].Text.Trim();
                txtEmpContribution.Text = gvMonthlyPFList.Rows[rIndex].Cells[3].Text.Trim();
              
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
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                SQLOperation.forDeleteRecordByIdentifier("PF_PFRecord", "SL", gvMonthlyPFList.DataKeys[rIndex].Values[2].ToString(), sqlDB.connection);
                allClear();
                lblMessage.InnerText = "success->Successfully  Deleted";
                gvMonthlyPFList.Rows[rIndex].Visible = false;
            }

        }

        protected void gvMonthlyPFList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                        LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkAlter");
                        lnkDelete.Enabled = false;
                        lnkDelete.ForeColor = Color.Silver;
                    }

                }
                catch { }
            
        }

        protected void gvMonthlyPFList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                loadPFRecord();
            }
            catch { }
            gvMonthlyPFList.PageIndex = e.NewPageIndex;
                gvMonthlyPFList.DataBind();
        }

        protected void ddlEmployeeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPFRecord();
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