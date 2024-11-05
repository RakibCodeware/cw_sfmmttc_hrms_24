using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.pf
{
    public partial class pf_interest_distribution : System.Web.UI.Page
    {
        //permission(view=383 Add/Proceess=384)
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["__ReadAction__"] = "0";
            ViewState["__WriteAction__"] = "0";

            int[] pagePermission = { 383,384 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            
            if (!IsPostBack)
            {
                txtDate.Text = txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
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
                ViewState["__UserId__"] = getUserId;
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
               // string[] AccessPermission = new string[0];
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "pf_interest_distribution.aspx", ddlCompanyName, gvPFSettings, btnSave);
                if (btnSave.Enabled)
                    Button1.Enabled = true;
                else
                    Button1.Enabled = false;
              
                if(permission.Contains(383))
                    ViewState["__ReadAction__"] = "1";
                if(permission.Contains(384))
                    ViewState["__WriteAction__"] = "1";
                checkInitialPermission();
                //loadInterest();
                commonTask.loadPFCompany(ddlCompanyName);
                //if (!classes.commonTask.HasBranch())
                //    ddlCompanyName.Enabled = false;
                //ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
               // classes.commonTask.loadFDRListForProfitDistribution(ddlFDRList, ddlCompanyName.SelectedValue);
                classes.commonTask.loadPFInvestment(ddlFDRList, ddlCompanyName.SelectedValue);
                classes.commonTask.loadPFExpenseYear(ddlYear,ddlCompanyName.SelectedValue);

            }


            catch { }

        }
        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            classes.commonTask.loadPFInvestment(ddlFDRList, ddlCompanyName.SelectedValue);
            //classes.commonTask.loadFDRListForProfitDistribution(ddlFDRList, ddlCompanyName.SelectedValue);
            classes.commonTask.loadPFExpenseYear(ddlYear, ddlCompanyName.SelectedValue);
           // loadInterest();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (rblDistribution.SelectedValue == "Profit")
                ProfitDistribute();
            else
                ExpenseDistribute();
        }
        private void ProfitDistribute()
        {
            try
            {
                //------------------------------- Get FDR Information ----------------------------------------------------
                DataTable dtFDR = new DataTable();
                sqlDB.fillDataTable("select NetInterest,WithdrawDate from PF_FDR_Interest where  FdrID="+ddlFDRList.SelectedValue+"", dtFDR);
                string MaturedDate = dtFDR.Rows[0]["WithdrawDate"].ToString();
                string ProfitMonth = dtFDR.Rows[0]["WithdrawDate"].ToString();// DateTime.Parse(MaturedDate).AddMonths(1).ToString("MM-yyyy");

                //-------------------- Get PF Members --------------------------------------------------------------------
                DataTable dtPfMember = new DataTable();
                sqlDB.fillDataTable("with prall as("+
                    "select SL,EmpID,Month,EmpContribution,EmprContribution,'SGHRM' as ProductDB from SGHRM.dbo.PF_PFRecord union all   select SL,EmpID,Month,EmpContribution,EmprContribution,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_PFRecord)"+
                    ",ppall as(select *,'SGHRM' as ProductDB from SGHRM.dbo.PF_Profit union all   select *,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_Profit )"+
                    ",pp as(select ProductDB,EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from ppall where Month<'" + MaturedDate + "' and ProductDB+EmpID in(select EmpId from v_PF_MemberListAll))" +
                    ",pr as (select  ProductDB,EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from prall where Month<'" + MaturedDate + "' and (ProductDB+EmpId) in(select ProductDB+EmpId from v_PF_MemberListAll))" +
                    ",pr1 as (select * from pr union select * from  pp)"+
                    ",pr2 as(select ProductDB, EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','" + MaturedDate + "')/30*30) Amount from pr1 group by ProductDB,Empid,Month) " +
                    "select ProductDB,EmpID,sum(Amount) Amount from pr2 group by ProductDB,Empid",dtPfMember);
                
                //sqlDB.fillDataTable("  with pr as (select EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_PFRecord where Month<'" + MaturedDate +
                //   "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1))," +
                //   " pp as (select EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_Profit where Month<'" + MaturedDate +
                //   "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1)), " +
                //   " pr1 as (select * from pr union select * from  pp)," +
                //   " pr2 as(select EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','" + MaturedDate + "')/30*30) Amount from pr1 group by Empid,Month) " +
                //   " select EmpID,sum(Amount) Amount from pr2 group by Empid", dtPfMember);

                //sqlDB.fillDataTable(" with a as ( "+
                //    " select pcs.EmpID,(pr.EmpContribution+pr.EmprContribution+sum( isnull(pp.Profit,0))) * (DATEDIFF(DAY,pr.Month,'"+MaturedDate+"')/30*30) TK "+
                //    " from Personnel_EmpCurrentStatus pcs inner join PF_PFRecord pr on pcs.EmpId=pr.EmpID and pcs.IsActive=1 and PfMember=1 left join PF_Profit pp on pr.EmpID=pp.EmpId and pr.Month=pp.Month "+
                //    " where pr.Month<'" + MaturedDate + "' and pcs.CompanyId='"+ddlCompanyName.SelectedValue+"' " +
                //    " group by pcs.EmpID, pr.Month,pr.EmpContribution,pr.EmprContribution ) "+
                //    " select a.EmpID, sum(a.TK) Amount from a  group by a.EmpID", dtPfMember);
                //--------------------------------------------------------------------------------------------------------

                float TotalAmount = float.Parse( dtPfMember.Compute("sum(Amount)", "").ToString());
                float TotalProfit = float.Parse(dtFDR.Rows[0]["NetInterest"].ToString());
                float Profit = 0;
                int count = 0;
                //-------------------------- Delete Existing Record for this FDR------------------------------------------
                SqlCommand cmdDel = new SqlCommand("delete SGHRM.dbo.PF_Profit where FdrID='" + ddlFDRList.SelectedValue + "'", sqlDB.connection);
                SqlCommand cmdDel1 = new SqlCommand("delete SGFHRM.dbo.PF_Profit where FdrID='" + ddlFDRList.SelectedValue + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();
                cmdDel1.ExecuteNonQuery();
                //---------------------------------------------------------------------------------------------------------
                for (int i = 0; i < dtPfMember.Rows.Count; i++)
                {
                    Profit = TotalProfit / TotalAmount * float.Parse(dtPfMember.Rows[i]["Amount"].ToString());
                    SqlCommand cmd = new SqlCommand("insert into " + dtPfMember.Rows[i]["ProductDB"].ToString() + ".dbo.PF_Profit values('" + ddlFDRList.SelectedValue + "','" + dtPfMember.Rows[i]["EmpId"].ToString() + "','" +
                    ProfitMonth + "','" + Profit + "') ", sqlDB.connection);
                    if (int.Parse(cmd.ExecuteNonQuery().ToString())== 1)
                        count++;
                }

                lblMessage.InnerText = "success-> Successfully  processed for " + count + " emplyee.";

            }
            catch { lblMessage.InnerText="error-> Unable to process !"; }
        }
        private void ProfitDistributePerDay()
        {
            try
            {
                //------------------------------- Get FDR Information ----------------------------------------------------

                DataTable dtFDR = new DataTable();
                sqlDB.fillDataTable(" select  (InterestAmount/Period) as NetInterest  from PF_FDR  where  FdrID=" + ddlFDRList.SelectedValue + "", dtFDR);
                string MaturedDate = txtDate.Text.Trim();
                string ProfitMonth =txtDate.Text.Trim() ;// DateTime.Parse(MaturedDate).AddMonths(1).ToString("MM-yyyy");

                //-------------------- Get PF Members --------------------------------------------------------------------
                DataTable dtPfMember = new DataTable();
                sqlDB.fillDataTable("with prall as(" +
                    "select SL,EmpID,Month,EmpContribution,EmprContribution,'SGHRM' as ProductDB from SGHRM.dbo.PF_PFRecord union all   select SL,EmpID,Month,EmpContribution,EmprContribution,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_PFRecord)" +
                    ",ppall as(select *,'SGHRM' as ProductDB from SGHRM.dbo.PF_Profit union all   select *,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_Profit )" +
                    ",pp as(select ProductDB,EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from ppall where Month<'" + MaturedDate + "' and ProductDB+EmpID in(select EmpId from v_PF_MemberListAll))" +
                    ",pr as (select  ProductDB,EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from prall where Month<'" + MaturedDate + "' and (ProductDB+EmpId) in(select ProductDB+EmpId from v_PF_MemberListAll))" +
                    ",pr1 as (select * from pr union select * from  pp)" +
                    ",pr2 as(select ProductDB, EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','" + MaturedDate + "')/30*30) Amount from pr1 group by ProductDB,Empid,Month) " +
                    "select ProductDB,EmpID,sum(Amount) Amount from pr2 group by ProductDB,Empid", dtPfMember);

                //sqlDB.fillDataTable("  with pr as (select EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_PFRecord where Month<'" + MaturedDate +
                //   "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1))," +
                //   " pp as (select EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_Profit where Month<'" + MaturedDate +
                //   "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1)), " +
                //   " pr1 as (select * from pr union select * from  pp)," +
                //   " pr2 as(select EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','" + MaturedDate + "')/30*30) Amount from pr1 group by Empid,Month) " +
                //   " select EmpID,sum(Amount) Amount from pr2 group by Empid", dtPfMember);

                //sqlDB.fillDataTable(" with a as ( "+
                //    " select pcs.EmpID,(pr.EmpContribution+pr.EmprContribution+sum( isnull(pp.Profit,0))) * (DATEDIFF(DAY,pr.Month,'"+MaturedDate+"')/30*30) TK "+
                //    " from Personnel_EmpCurrentStatus pcs inner join PF_PFRecord pr on pcs.EmpId=pr.EmpID and pcs.IsActive=1 and PfMember=1 left join PF_Profit pp on pr.EmpID=pp.EmpId and pr.Month=pp.Month "+
                //    " where pr.Month<'" + MaturedDate + "' and pcs.CompanyId='"+ddlCompanyName.SelectedValue+"' " +
                //    " group by pcs.EmpID, pr.Month,pr.EmpContribution,pr.EmprContribution ) "+
                //    " select a.EmpID, sum(a.TK) Amount from a  group by a.EmpID", dtPfMember);
                //--------------------------------------------------------------------------------------------------------

                float TotalAmount = float.Parse(dtPfMember.Compute("sum(Amount)", "").ToString());
                float TotalProfit = float.Parse(dtFDR.Rows[0]["NetInterest"].ToString());
                float Profit = 0;
                int count = 0;
                //-------------------------- Delete Existing Record for this FDR------------------------------------------
                SqlCommand cmdDel = new SqlCommand("delete SGHRM.dbo.PF_Profit where month='"+txtDate.Text+"' and FdrID='" + ddlFDRList.SelectedValue + "'", sqlDB.connection);
                SqlCommand cmdDel1 = new SqlCommand("delete SGFHRM.dbo.PF_Profit where month='" + txtDate.Text + "' and FdrID='" + ddlFDRList.SelectedValue + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();
                cmdDel1.ExecuteNonQuery();
                //---------------------------------------------------------------------------------------------------------
                for (int i = 0; i < dtPfMember.Rows.Count; i++)
                {
                    Profit = TotalProfit / TotalAmount * float.Parse(dtPfMember.Rows[i]["Amount"].ToString());
                    SqlCommand cmd = new SqlCommand("insert into " + dtPfMember.Rows[i]["ProductDB"].ToString() + ".dbo.PF_Profit values('" + ddlFDRList.SelectedValue + "','" + dtPfMember.Rows[i]["EmpId"].ToString() + "','" +
                    ProfitMonth + "','" + Profit + "') ", sqlDB.connection);
                    if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                        count++;
                }

                lblMessage.InnerText = "success-> Successfully  processed for " + count + " emplyee.";

            }
            catch { lblMessage.InnerText = "error-> Unable to process !"; }
        }
        private void ExpenseDistribute()
        {
            try
            {
                //------------------------------- Get FDR Information ----------------------------------------------------
                string[] value = ddlYear.SelectedValue.Split('/');
                string ExpenseDate = value[0];
                float TotalExpense = float.Parse(value[1]);


                //-------------------- Get PF Members --------------------------------------------------------------------
                DataTable dtPfMember = new DataTable();
                sqlDB.fillDataTable(" with prall as(select SL,EmpID,Month,EmpContribution,EmprContribution,'SGHRM' as ProductDB from SGHRM.dbo.PF_PFRecord "+
                    "union all   select SL,EmpID,Month,EmpContribution,EmprContribution,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_PFRecord), "+
                    "ppall as(select *,'SGHRM' as ProductDB from SGHRM.dbo.PF_Profit union all   select *,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_Profit ),"+
                    "pr as (select ProductDB,EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from prall "+
                    "where Month<='" + ExpenseDate + "' and ProductDB+EmpID in(select ProductDB+EmpId from v_PF_MemberListAll)), " +
                    "pp as (select ProductDB,EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from ppall "+
                    "where Month<='" + ExpenseDate + "' and ProductDB+EmpID in(select ProductDB+EmpId from v_PF_MemberListAll)),  " +
                    "pr1 as (select * from pr union select * from  pp), "+
                    "pr2 as(select ProductDB,EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','" + ExpenseDate + "')/30*30) Amount from pr1 " +
                    "group by ProductDB,Empid,Month)  select  ProductDB,EmpID,sum(Amount) Amount from pr2 group by ProductDB,Empid",dtPfMember);
                //sqlDB.fillDataTable("  with pr as (select EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_PFRecord where Month<='"+ExpenseDate+
                //    "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1)),"+
                //    " pp as (select EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_Profit where Month<='"+ExpenseDate+
                //    "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1)), "+
                //    " pr1 as (select * from pr union select * from  pp),"+
                //    " pr2 as(select EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','"+ExpenseDate+"')/30*30) Amount from pr1 group by Empid,Month) "+
                //    " select EmpID,sum(Amount) Amount from pr2 group by Empid", dtPfMember);
                //--------------------------------------------------------------------------------------------------------

                float TotalAmount = float.Parse(dtPfMember.Compute("sum(Amount)", "").ToString());
                
                float Expense = 0;
                int count = 0;
                //-------------------------- Delete Existing Record for this FDR------------------------------------------
                SqlCommand cmdDel = new SqlCommand("delete from SGHRM.dbo.PF_Expense where Month='" + ExpenseDate + "'", sqlDB.connection);
                SqlCommand cmdDel1 = new SqlCommand("delete from SGFHRM.dbo.PF_Expense where Month='" + ExpenseDate + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();
                cmdDel1.ExecuteNonQuery();
                //---------------------------------------------------------------------------------------------------------
                for (int i = 0; i < dtPfMember.Rows.Count; i++)
                {
                    Expense = TotalExpense / TotalAmount * float.Parse(dtPfMember.Rows[i]["Amount"].ToString());
                    SqlCommand cmd = new SqlCommand("insert into " + dtPfMember.Rows[i]["ProductDB"].ToString() + ".dbo.PF_Expense values('" + dtPfMember.Rows[i]["EmpId"].ToString() + "','" + ExpenseDate + "','" 
                        + Expense + "') ", sqlDB.connection);
                    if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                        count++;
                }

                lblMessage.InnerText = "success-> Successfully  processed for " + count + " emplyee.";

            }
            catch { lblMessage.InnerText = "error-> Unable to process !"; }
        }
        protected void rblDistribution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblDistribution.SelectedValue == "Profit")
            {
                divYear.Visible = false;
                divFdrNo.Visible = true;
                btnSave.Visible = false;
                Button1.Visible = true;
                divDateRange.Visible = true;
            }
            else 
            {
                divYear.Visible = true;
                divFdrNo.Visible = false;
                btnSave.Visible = true;
                Button1.Visible = false;
                divDateRange.Visible = false;
            }
            imgLoading.Visible = false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (ddlFDRList.Items.Count < 2)
            {
                lblMessage.InnerText = "warning-> No Investment Available!"; ddlFDRList.Focus();
                imgLoading.Visible = false;
                return;
            }
            //if (txtDate.Text.Trim().Length < 8)
            //{
            //    lblMessage.InnerText = "warning-> Please select From Date !"; txtDate.Focus(); return;
            //}
            //if (txtToDate.Text.Trim().Length < 8)
            //{
            //    lblMessage.InnerText = "warning-> Please select To Date !"; txtDate.Focus(); return;
            //}
            DateTime FromDate=DateTime.Parse(txtDate.Text.Trim());
            DateTime ToDate=DateTime.Parse(txtToDate.Text.Trim());
            for (DateTime date = FromDate; date <= ToDate; date = date.AddDays(1))
            {
                for (int i = 1; i < ddlFDRList.Items.Count; i++)
                {
                    int? resutl;

                    resutl = classes.PF_ProfitDistribution.ProfitDistributePerDay(ddlCompanyName.SelectedValue,date.ToString("yyyy-MM-dd"), ddlFDRList.Items[i].Value.ToString(), "1", ViewState["__UserId__"].ToString());
                    //if (resutl != null)
                    //    lblMessage.InnerText = "success-> Successfully  processed for " + resutl + " emplyee.";
                    //else
                    //    lblMessage.InnerText = "error-> Unable to process !";
                    //lblMessage.InnerText = "";
                }
               
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ProcessSuccess();", true);  //Open New Tab for Sever side code
            imgLoading.Visible = false;
                
            
            
        }

        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("0"))
            {
                Button1.Enabled = false;
                Button1.CssClass = "";

            }
            else
            {
                Button1.Enabled = true;
                Button1.CssClass = "Pbutton";
            }

        }
    }
}