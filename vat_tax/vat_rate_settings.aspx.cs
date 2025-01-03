﻿using adviitRuntimeScripting;
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

namespace SigmaERP.vat_tax
{
    public partial class vat_rate_settings : System.Web.UI.Page
    {
        //permission(View=411 Add=412 Update=413 Delete=414)
        string CompanyId = "";
        string sqlcmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {


            int[] pagePermission = { 411, 412, 413,414 };

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
              //  string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "vat_rate_settings.aspx", ddlCompanyName, gvvatraxrateSettings, btnSave);
                if(permission.Contains(411))
                    ViewState["__ReadAction__"] = "1";
                if(permission.Contains(412))
                    ViewState["__WriteAction__"] = "1";
                if(permission.Contains(413))
                    ViewState["__UpdateAction__"] = "1";
                if(permission.Contains(414))
                    ViewState["__DeletAction__"] = "1";
                checkInitialPermission();
                string jku = ViewState["__UpdateAction__"].ToString();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                loadVatTaxRateSettings();
            }
            catch { }

        }
        private void loadVatTaxRateSettings()
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("SELECT * FROM v_VatTax_Rate where CompanyId='" + ddlCompanyName.SelectedValue + "' and Taxpayer="+rblTaxpayerType.SelectedValue+"", dt);
            gvvatraxrateSettings.DataSource = dt;
            gvvatraxrateSettings.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Text == "Update")
                {
                    Update();
                }
                else
                {
                    Save();
                }
            }
            catch { }
        }
        private void Save()
        {
            try
            {
                string[] getColumns = { "CompanyId", "SlabName", "FromTaka", "ToTaka", "IncomeTaxRate", "RateOrder", "Taxpayer" };
                string[] getValues = { ddlCompanyName.SelectedValue, txtSlabName.Text, txtFromTaka.Text, txttoTaka.Text, txtincometaxrate.Text, txtOrder.Text, rblTaxpayerType.SelectedValue };

                if (SQLOperation.forSaveValue("VatTax_Rate", getColumns, getValues, sqlDB.connection) == true)
                {
                    allClear();
                    loadVatTaxRateSettings();
                    lblMessage.InnerText = "success->Successfully Save";
                }
            }
            catch { }
        }
        private void Update()
        {
            try
            {
                string[] getColumns = { "CompanyId", "SlabName", "FromTaka", "ToTaka", "IncomeTaxRate", "RateOrder", "Taxpayer" };
                string[] getValues = { ddlCompanyName.SelectedValue, txtSlabName.Text, txtFromTaka.Text, txttoTaka.Text, txtincometaxrate.Text, txtOrder.Text,rblTaxpayerType.SelectedValue };

                if (SQLOperation.forUpdateValue("VatTax_Rate", getColumns, getValues, "RSN", ViewState["__getSL__"].ToString(), sqlDB.connection) == true)
                {
                    // saveShiftConfigDateLog(true, StartTime, EndTime);
                    lblMessage.InnerText = "success->Successfully  Updated";
                    loadVatTaxRateSettings();
                    allClear();
                }
            }
            catch { }
        }

        protected void gvvatraxrateSettings_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string hhh = ViewState["__UpdateAction__"].ToString();
                if (e.CommandName.Equals("Alter"))
                {
                    string a = ViewState["__preRIndex__"].ToString();
                    if (!ViewState["__preRIndex__"].ToString().Equals("No")) gvvatraxrateSettings.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    gvvatraxrateSettings.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                    ViewState["__preRIndex__"] = rIndex;
                    setValueToControl(rIndex, gvvatraxrateSettings.DataKeys[rIndex].Values[0].ToString(), gvvatraxrateSettings.DataKeys[rIndex].Values[1].ToString(), gvvatraxrateSettings.DataKeys[rIndex].Values[2].ToString());
                    btnSave.Text = "Update";
                    string kkk = ViewState["__UpdateAction__"].ToString();
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
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

                    SQLOperation.forDeleteRecordByIdentifier("VatTax_Rate", "RSN", gvvatraxrateSettings.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "deleteSuccess()", true);
                        allClear();
                        lblMessage.InnerText = "success->Successfully  Deleted";
                        gvvatraxrateSettings.Rows[rIndex].Visible = false;
                 
                }
            }
            catch { }
        }
        private void allClear()
        {
            txtFromTaka.Text = "";
            txtincometaxrate.Text = "";
            txtOrder.Text = "";
            txtSlabName.Text = "";
            txttoTaka.Text = "";
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
        private void setValueToControl(int rIndex, string getSL, string getCompanyId,string getTaxpayer)
        {
            try
            {
                ViewState["__getSL__"] = getSL;
                ddlCompanyName.SelectedValue = getCompanyId;
                rblTaxpayerType.SelectedValue = getTaxpayer;
                txtSlabName.Text = gvvatraxrateSettings.Rows[rIndex].Cells[1].Text;
                txtFromTaka.Text = gvvatraxrateSettings.Rows[rIndex].Cells[2].Text;
                txttoTaka.Text = gvvatraxrateSettings.Rows[rIndex].Cells[3].Text;
                txtincometaxrate.Text = gvvatraxrateSettings.Rows[rIndex].Cells[4].Text;
                txtOrder.Text = gvvatraxrateSettings.Rows[rIndex].Cells[5].Text;

                
            }
            catch { }
        }

        protected void gvvatraxrateSettings_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void rblTaxpayerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadVatTaxRateSettings(); 
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