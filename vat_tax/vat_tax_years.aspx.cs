using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.vat_tax
{
    public partial class vat_tax_years : System.Web.UI.Page
    {
        string CompanyId = "";
        string sqlcmd = "";
        //permission(View=423 Add=424 Edit=425 Delete=426)  



        protected void Page_Load(object sender, EventArgs e)
        {
            

            int[] pagePermission = { 423, 424, 425, 426 };

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
        private void setPrivilege( int[] permission)
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__preRIndex__"] = "No";
                ViewState["__UserId__"] = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), ViewState["__UserId__"].ToString(), ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "vat_tax_years.aspx", ddlCompanyName, gvvatraxrateSettings, btnSave);

                if (permission.Contains(423))
                    ViewState["__ReadAction__"] = "1";
                if (permission.Contains(424))
                    ViewState["__WriteAction__"] = "1";
                if (permission.Contains(425))
                    ViewState["__UpdateAction__"] = "1";
                if (permission.Contains(426))
                    ViewState["__DeletAction__"] = "1";
                checkInitialPermission();

                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();

                commonTask.loadGenerateTaxType(ddlGenerateType,ddlCompanyName.SelectedValue);
                loadVatTaxRateSettings();
            }
            catch { }

        }
        private void loadVatTaxRateSettings()
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("SELECT format(FromMonth,'MM-yyyy') as FromMonth,format(ToMonth,'MM-yyyy') as ToMonth,Type,TaxId,CompanyId,TaxYears,OrderNo,UserId FROM v_VatTax_Years where CompanyId='" + ddlCompanyName.SelectedValue + "' order by TaxYears, OrderNo desc", dt);
            gvvatraxrateSettings.DataSource = dt;
            gvvatraxrateSettings.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //----------------------------- validation -------------------------------------------------------
                if (ddlCompanyName.SelectedIndex< 1)
                { lblMessage.InnerText = "warning-> Please, select any company !"; ddlCompanyName.Focus(); return; }
                if (txtFromMonth.Text.Trim().Length<7)
                { lblMessage.InnerText = "warning-> Please, select valid From Month !"; txtFromMonth.Focus(); return; }
                if (txtToMonth.Text.Trim().Length < 7)
                { lblMessage.InnerText = "warning-> Please, select valid To Month !"; txtToMonth.Focus(); return; }
                if (ddlGenerateType.SelectedIndex==0)
                { lblMessage.InnerText = "warning-> Please, enter valid Order no !"; ddlGenerateType.Focus(); return; }
                //------------------------------------------------------------------------------------------------
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

                string[] getColumns = { "CompanyId", "FromMonth", "ToMonth", "TaxYears", "OrderNo", "UserId",  "IsPaid" };
                string[] getValues = { ddlCompanyName.SelectedValue, convertDateTime.getCertainCulture("01-" + txtFromMonth.Text).ToString(), convertDateTime.getCertainCulture("01-" + txtToMonth.Text).ToString(), getTaxYears(), ddlGenerateType.SelectedValue, ViewState["__UserId__"].ToString(),"0" };

                if (SQLOperation.forSaveValue("VatTax_Years", getColumns, getValues, sqlDB.connection) == true)
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

                string[] getColumns = { "FromMonth", "ToMonth", "TaxYears", "OrderNo", "UserId", "IsPaid" };
                string[] getValues = { convertDateTime.getCertainCulture("01-" + txtFromMonth.Text).ToString(), convertDateTime.getCertainCulture("01-" + txtToMonth.Text).ToString(), getTaxYears(), ddlGenerateType.SelectedValue, ViewState["__UserId__"].ToString(),"0" };

                if (SQLOperation.forUpdateValue("VatTax_Years", getColumns, getValues, "TaxId", ViewState["__getSL__"].ToString(), sqlDB.connection) == true)
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

                if (e.CommandName.Equals("Alter"))
                {
                    string a = ViewState["__preRIndex__"].ToString();
                    if (!ViewState["__preRIndex__"].ToString().Equals("No")) gvvatraxrateSettings.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    gvvatraxrateSettings.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                    ViewState["__preRIndex__"] = rIndex;
                    setValueToControl(rIndex, gvvatraxrateSettings.DataKeys[rIndex].Values[0].ToString(), gvvatraxrateSettings.DataKeys[rIndex].Values[1].ToString(), gvvatraxrateSettings.DataKeys[rIndex].Values[2].ToString());
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

                    SQLOperation.forDeleteRecordByIdentifier("VatTax_Years", "TaxId", gvvatraxrateSettings.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
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
            txtFromMonth.Text = "";
            txtToMonth.Text = "";
            ddlGenerateType.SelectedIndex = 0;
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
        private void setValueToControl(int rIndex, string getSL, string getCompanyId,string getGenerateType)
        {
            try
            {
                ViewState["__getSL__"] = getSL;
                ddlCompanyName.SelectedValue = getCompanyId;
                txtFromMonth.Text = gvvatraxrateSettings.Rows[rIndex].Cells[0].Text;
                txtToMonth.Text = gvvatraxrateSettings.Rows[rIndex].Cells[1].Text;
                ddlGenerateType.SelectedValue = getGenerateType;
               // txtOrder.Text = gvvatraxrateSettings.Rows[rIndex].Cells[3].Text;
            }
            catch { }
        }

        private string getTaxYears()
        {
            DateTime FMonth = DateTime.Parse("01-"+txtFromMonth.Text);
           // DateTime TMonth = DateTime.Parse("01-" + txtToMonth.Text);
            return FMonth.Year.ToString()+"-"+FMonth.AddYears(1).Year.ToString();
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            commonTask.loadGenerateTaxType(ddlGenerateType, ddlCompanyName.SelectedValue);
            loadVatTaxRateSettings();
           
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