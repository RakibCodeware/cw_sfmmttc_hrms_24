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
    public partial class pf_FDR : System.Web.UI.Page
    {
        string CompanyId = "";
        string sqlcmd = "";

        //permission(View=371,Ad=372,Update=373 Delete=374)
        protected void Page_Load(object sender, EventArgs e)
        {
          
            int[] pagePermission = { 371, 372, 373, 374 };
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

                txtFromDateP.Text = "01-01-" + DateTime.Now.ToString("yyyy");
                txtToDateP.Text = "31-12-" + DateTime.Now.ToString("yyyy");
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
               // string[] AccessPermission = new string[0];
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "pf_FDR.aspx", ddlCompanyName, gvPFSettings, btnSave);

                if (permission.Contains(371))
                    ViewState["__ReadAction__"] = "1";
                if (permission.Contains(372))
                    ViewState["__WriteAction__"] = "1";
                if (permission.Contains(373))
                    ViewState["__UpdateAction__"] = "1";
                if (permission.Contains(374))
                    ViewState["__DeletAction__"] = "1";
                checkInitialPermission();
                commonTask.loadPFInvestmentType(ddlType);
                commonTask.loadPFCompany(ddlCompanyName);
                loadPFSettings();
                //if (!classes.commonTask.HasBranch())
                //    ddlCompanyName.Enabled = false;
                //ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
              
            }
            catch { }

        }
        private void loadPFSettings()
        {
            try
            {
               // CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;

                sqlcmd = "select FdrID,CompanyID,FdrNo,FdrAmount,InterestRate,InterestAmount,CONVERT(VARCHAR(10),FromDate, 105) as FromDate,"+
                    "convert(varchar(10),ToDate,105) as ToDate,Period,Bank,Branch,FdrAmount+InterestAmount as TotalWithInterest,Type,InvestmentType from PF_FDR inner join PF_InvestmentType on PF_FDR.Type=PF_InvestmentType.ID where CompanyId='" + ddlCompanyName.SelectedValue + "' order by convert(int,CompanyId)";
                DataTable dt = new DataTable();
                sqlDB.fillDataTable(sqlcmd, dt);
                gvPFSettings.DataSource = dt;
                gvPFSettings.DataBind();
            }
            catch { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (btnSave.Text == "Save")
                SavePFSettings();
            else
                UpdatePFSettings();
        }
        private void SavePFSettings()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("insert into PF_FDR values('" + ddlCompanyName.SelectedValue + "','" + txtFDRNo.Text.Trim() + "','"  + 
                    txtFDRAmount.Text.Trim() + "','" + txtInterestRate.Text.Trim() + "'," + txtInterestAmount.Text.Trim() + ",'"
                    + convertDateTime.getCertainCulture(txtFromDate.Text).ToString() + "','" + convertDateTime.getCertainCulture(txtToDate.Text).ToString() + 
                    "',"+txtPeriod.Text.Trim() + ",'" + txtBank.Text.Trim() + "','" + txtBranch.Text + "','"+ddlType.SelectedValue+"') ", sqlDB.connection);
              //  SQLOperation.forDeleteRecordByIdentifier("PF_CalculationSetting", "CompanyId", ddlCompanyName.SelectedValue, sqlDB.connection);
                if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                {
                    loadPFSettings();
                    lblMessage.InnerText = "success-> Successfully Saved.";
                    allClear();
                }
                else
                    lblMessage.InnerText = "error-> Unable to Save !";
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error>" + ex.Message;
            }
        }

        private void UpdatePFSettings()
        {
            try
            {

                SqlCommand cmd = new SqlCommand("Update  PF_FDR set FdrNo='" + txtFDRNo.Text.Trim() + "',FdrAmount='" +txtFDRAmount.Text.Trim()+
                    "',InterestRate='" + txtInterestRate.Text.Trim() + "',InterestAmount='" + txtInterestAmount.Text.Trim() + "',FromDate='" + convertDateTime.getCertainCulture(txtFromDate.Text).ToString() +
                    "',ToDate='" + convertDateTime.getCertainCulture(txtToDate.Text).ToString() + "',Period=" + txtPeriod.Text.Trim() + ",Bank='" + txtBank.Text.Trim() + "',Branch='" + txtBranch.Text.Trim() +
                    "' , Type='"+ddlType.SelectedValue+"' where FdrID='" + ViewState["__FdrID__"].ToString() + "'", sqlDB.connection);
                if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                {
                    loadPFSettings();
                    lblMessage.InnerText = "success-> Successfully Update.";
                    allClear();
                }
                else
                    lblMessage.InnerText = "error-> Unable to Update !";

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error>" + ex.Message;

            }
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
                ViewState["__FdrID__"] = gvPFSettings.DataKeys[rIndex].Values[1].ToString();
                ddlType.SelectedValue = gvPFSettings.DataKeys[rIndex].Values[2].ToString();
                txtFDRNo.Text = gvPFSettings.Rows[rIndex].Cells[1].Text.Trim();
                txtFDRAmount.Text = gvPFSettings.Rows[rIndex].Cells[2].Text.Trim();
                //  txtRateOfInterest.Text = gvPFSettings.Rows[rIndex].Cells[2].Text.Trim();
                txtInterestRate.Text = gvPFSettings.Rows[rIndex].Cells[3].Text.Trim();              
                txtFromDate.Text = gvPFSettings.Rows[rIndex].Cells[4].Text.Trim();
                txtToDate.Text = gvPFSettings.Rows[rIndex].Cells[5].Text.Trim();
                txtPeriod.Text = gvPFSettings.Rows[rIndex].Cells[6].Text.Trim();
                txtInterestAmount.Text = gvPFSettings.Rows[rIndex].Cells[7].Text.Trim();
                txtTotalWithInterest.Text = gvPFSettings.Rows[rIndex].Cells[8].Text.Trim();
                txtBank.Text = (gvPFSettings.Rows[rIndex].Cells[9].Text.Trim().Equals("&nbsp;")) ? "" : gvPFSettings.Rows[rIndex].Cells[9].Text.Trim();
                txtBranch.Text = (gvPFSettings.Rows[rIndex].Cells[10].Text.Trim().Equals("&nbsp;")) ? "" : gvPFSettings.Rows[rIndex].Cells[10].Text.Trim();
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
                SQLOperation.forDeleteRecordByIdentifier("PF_FDR", "FdrID", gvPFSettings.DataKeys[rIndex].Values[1].ToString(), sqlDB.connection);
                allClear();
                lblMessage.InnerText = "success->Successfully  Deleted";

                //-------------------------- Delete Existing Record from  PF_Profit by this FDR [Both Database]------------------------------------------
                SqlCommand cmdDel = new SqlCommand("delete SGHRM.dbo.PF_Profit where FdrID='" + gvPFSettings.DataKeys[rIndex].Values[1].ToString() + "'", sqlDB.connection);
                SqlCommand cmdDel1 = new SqlCommand("delete SGFHRM.dbo.PF_Profit where FdrID='" + gvPFSettings.DataKeys[rIndex].Values[1].ToString() + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();
                cmdDel1.ExecuteNonQuery();
                //---------------------------------------------------------------------------------------------------------
                gvPFSettings.Rows[rIndex].Visible = false;
            }
        }
        private void allClear()
        {

            txtFDRNo.Text = "";
            txtFDRAmount.Text = "";
            // txtRateOfInterest.Text = "";
            txtInterestRate.Text = "";
            txtInterestAmount.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";
            txtPeriod.Text = "";
            txtBank.Text = "";
            txtBranch.Text = "";
            txtTotalWithInterest.Text = "";
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
        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPFSettings();
        }

        protected void gvPFSettings_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void btnNew_Click(object sender, EventArgs e)
        {
            allClear();
        }
         private void PeriodDays() 
        {
            if (txtFromDate.Text.Trim().Length == 10 && txtToDate.Text.Trim().Length == 10)
             {
                 string[] FD = txtFromDate.Text.Trim().Split('-');
                 string[] TD = txtToDate.Text.Trim().Split('-');
                 DateTime FDate = DateTime.Parse(""+FD[2]+"-"+FD[1]+"-"+FD[0]+"");
                 DateTime TDate = DateTime.Parse("" + TD[2] + "-" + TD[1] + "-" + TD[0] + "");
                 txtPeriod.Text = (TDate - FDate).Days.ToString();
             }
            
        }

         private void Interest()
         {
             if (txtFDRAmount.Text.Trim().Length>0 && txtInterestRate.Text.Trim().Length>0 && txtFromDate.Text.Trim().Length == 10 && txtToDate.Text.Trim().Length == 10)
             {
                 float P = float.Parse(txtFDRAmount.Text.Trim());
                 float r = float.Parse(txtInterestRate.Text.Trim())/100;
                 float t = float.Parse(txtPeriod.Text.Trim()) / 365;
                // float A = P*(1 + (r*t));
                 float A =(r * P) * t;
                 
                 txtInterestAmount.Text =Math.Round(A).ToString();
                 txtTotalWithInterest.Text = (P + Math.Round(A)).ToString();
             }

         }
         protected void txtFromDate_TextChanged(object sender, EventArgs e)
         {
             try 
             { 
                 PeriodDays();
                 Interest();
             }
             catch { }
             
         }

         protected void txtFDRAmount_TextChanged(object sender, EventArgs e)
         {
             try
             {                 
                 Interest();
             }
             catch { }
         }

         protected void gvPFSettings_PageIndexChanging(object sender, GridViewPageEventArgs e)
         {
             loadPFSettings();
             gvPFSettings.PageIndex = e.NewPageIndex;
             gvPFSettings.DataBind();
         }

         protected void btnPrint_Click(object sender, EventArgs e)
         {
          
             PreviewInvestment();
         }
         private void PreviewInvestment() 
         {
             try
             {
                 string[]FDate=txtFromDateP.Text.Trim().Split('-');
                 string[]TDate=txtToDateP.Text.Trim().Split('-');
                 // CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;

                 sqlcmd = "select PFCompanyName,PFAddress,InvestmentType,FdrNo,FdrAmount,InterestRate,InterestAmount,convert(varchar(10),FromDate,105) FromDate,convert(varchar(10),ToDate,105) ToDate,Period,Bank,Type from v_PF_Investment where CompanyID='" + ddlCompanyName.SelectedValue
                     + "' and FromDate>='" + FDate[2] + "-" + FDate[1] + "-" + FDate[0] + "' and FromDate<='" + TDate[2] + "-" + TDate[1] + "-" + TDate[0] + "'";
                 DataTable dt = new DataTable();
                 sqlDB.fillDataTable(sqlcmd, dt);
                 if (dt.Rows.Count == 0)
                 {
                     lblMessage.InnerText = "warning->Sorry any record are not founded"; return;

                 }
                 Session["__PFInvestment__"] = dt;
                 ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=PFInvestment- " + txtFromDateP.Text.Trim().Replace('-', '/') + " to " + txtToDateP.Text.Trim().Replace('-', '/') + "');", true);  //Open New Tab for Sever side code
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
            if (ViewState["__ReadAction__"].ToString().Equals("0"))
            {
                btnPrint.Enabled = false;
                btnPrint.CssClass = "";
            }
            else
            {
                btnPrint.Enabled = true;
                btnPrint.CssClass = "Pbutton";
            }
        }

    }
}