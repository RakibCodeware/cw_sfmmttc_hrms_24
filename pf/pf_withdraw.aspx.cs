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
    public partial class pf_withdraw : System.Web.UI.Page
    {
        //permission(view=387, Add=388,Edit=389, Delete=390 )
        string CompanyId = "";
        string sqlcmd = "";
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {

            int[] pagePermission = { 387, 388, 389, 390 };

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
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "pf_withdraw.aspx", ddlCompanyName, gvPFWithdrawList, btnSave);

                classes.commonTask.loadPFMemberListForWithdraw(ddlEmployeeList, ViewState["__CompanyId__"].ToString());                
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();

                if (permission.Contains(387))
                    ViewState["__ReadAction__"] = "1";
                if (permission.Contains(388))
                    ViewState["__WriteAction__"] = "1";
                if (permission.Contains(389))
                    ViewState["__UpdateAction__"] = "1";
                if (permission.Contains(390))
                    ViewState["__DeletAction__"] = "1";
                checkInitialPermission();

                loadPFWithDrawRecord();

            }
            catch { }

        }
        private void loadPFWithDrawRecord() 
        {
            DataTable dtWithdrawnMember = new DataTable();
            string PaybableType = (rblPayableType.SelectedValue == "0") ? "" : " pfw.PayableType=" + rblPayableType.SelectedValue + " and ";
            sqlDB.fillDataTable("select pfw.EmpId,pei.EmpName,SUBSTRING(pei.EmpCardNo,10,5) EmpCardNo, convert(varchar,WithdrawDate,105) WithdrawDate,EmpContribution,EmprContribution,Profit from PF_Withdraw pfw "+
                " inner join Personnel_EmployeeInfo pei on pfw.EmpId=pei.EmpId where "+PaybableType+" pei.CompanyId='"+ddlCompanyName.SelectedValue+"'", dtWithdrawnMember);
            gvPFWithdrawList.DataSource = dtWithdrawnMember;
            gvPFWithdrawList.DataBind();
        }
        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            //CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;
            classes.commonTask.loadPFMemberListForWithdraw(ddlEmployeeList, ddlCompanyName.SelectedValue);
            loadPFWithDrawRecord();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            pfWithdraw();
        }
        private void pfWithdraw() 
        {
            //---------------get PF Year---------------------
            string PayableType;
            string[] WithdrawDate = txtWithdrawDate.Text.Trim().Split('-');
            DateTime zeroTime = new DateTime(1, 1, 1);
            string[] SelectedValue = ddlEmployeeList.SelectedValue.Split('/');
            DateTime pfStartDate = DateTime.Parse(SelectedValue[1]);
            DateTime pfWithdrawDate = DateTime.Parse(WithdrawDate[2] + "-" + WithdrawDate[1] + "-" + WithdrawDate[0]);
            TimeSpan pfYear = pfWithdrawDate.Subtract(pfStartDate);   
            float year = (zeroTime + pfYear).Year - 1;
            //--------------------End PF Year------------------------

            //---------------get PF Payable Type Info----------------

            dt = new DataTable();
            sqlDB.fillDataTable("select PEmpPartStartyear,PEmpPartEndyear,PEmpEmprStartyear,PEmpEmprEndyear,PEmpEmprIrstStartyear,PEmpEmprIrstEndyear from PF_CalculationSetting where CompanyId='"+ddlCompanyName.SelectedValue+"'", dt);
            if (dt.Rows.Count > 0)
            {
                if (int.Parse(dt.Rows[0]["PEmpEmprIrstStartyear"].ToString()) < year) //Payable ( Employee contribution + Employeer contribution + Interest)
                    PayableType = "3";
                else if (int.Parse(dt.Rows[0]["PEmpEmprStartyear"].ToString()) < year)//Payable ( Employee contribution + Employeer contribution)
                    PayableType = "2";
                else//Payable ( Employee contribution)
                    PayableType = "1";


                try
                {

                    DataTable dtPFInfo = new DataTable();
                    string sql = " with " +
                      "a as( select isnull( sum(EmpContribution),0) EmpContribution,isnull( sum(EmprContribution),0) EmprContribution,'" + SelectedValue[0] +
                      "' EmpID from PF_PFRecord   where EmpID='" + SelectedValue[0] + "')," +
                      " b as( select isnull( sum(Profit),0) Profit,'" + SelectedValue[0] + "' as EmpId from PF_Profit where EmpID='" + SelectedValue[0] + "') " +
                      "select EmpContribution,EmprContribution,Profit from a inner join b on a.EmpID=b.EmpID";
                  sqlDB.fillDataTable(sql,dtPFInfo);

                  SqlCommand cmd = new SqlCommand("insert into PF_Withdraw values('" + SelectedValue[0] + "','" +
                        commonTask.ddMMyyyyTo_yyyyMMdd(txtWithdrawDate.Text.Trim())+ "',"+dtPFInfo.Rows[0]["EmpContribution"].ToString()+
                        ","+dtPFInfo.Rows[0]["EmprContribution"].ToString()+","+dtPFInfo.Rows[0]["Profit"].ToString()+"," + PayableType + " ) ", sqlDB.connection);

                    if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                    {
                        //---------Update Employee pf Status-----------------
                        SqlCommand cmd1 = new SqlCommand("update Personnel_EmpCurrentStatus set PfMember=0 where IsActive=1 and EmpId='" + SelectedValue[0] + "'", sqlDB.connection);
                        cmd1.ExecuteNonQuery();
                        classes.commonTask.loadPFMemberListForWithdraw(ddlEmployeeList, ddlCompanyName.SelectedValue);
                        //-------------------------------------------------     
                        rblPayableType.SelectedValue = "0";
                        loadPFWithDrawRecord();

                        lblMessage.InnerText = "success->" + btnSave.Text.Trim() + "n Successfully.";
                    }
                    else
                        lblMessage.InnerText = "error-> Unable to " + btnSave.Text.Trim() + " !";
                }
                catch (Exception ex)
                {
                    lblMessage.InnerText = "error->" + ex.Message;
                }
            }
            //----------------End PF Payable Type Info---------------


        }

        protected void rblPayableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadPFWithDrawRecord();
        }

        protected void gvPFWithdrawList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("deleterow"))
            {
                int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                SQLOperation.forDeleteRecordByIdentifier("PF_Withdraw", "EmpId", gvPFWithdrawList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);

                lblMessage.InnerText = "success->Successfully  Deleted";
                gvPFWithdrawList.Rows[rIndex].Visible = false;
                //---------Update Employee pf Status-----------------
                SqlCommand cmd1 = new SqlCommand("update Personnel_EmpCurrentStatus set PfMember=1 where IsActive=1 and EmpId='" + gvPFWithdrawList.DataKeys[rIndex].Values[0].ToString() + "'", sqlDB.connection);
                cmd1.ExecuteNonQuery();
                classes.commonTask.loadPFMemberListForWithdraw(ddlEmployeeList, ddlCompanyName.SelectedValue);
                //-------------------------------------------------
            }
            else if (e.CommandName.Equals("preview"))
            {
                int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                PreviewReport(gvPFWithdrawList.DataKeys[rIndex].Values[0].ToString());
            }
        }
        private void PreviewReport(string EmpId)
        {
            try
            {

                dt = new DataTable();
                dt = classes.commonTask.returnPFMemberInfoByEmpID(EmpId);
                if (dt == null || dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No Data Found!"; return;
                }
                string ProductDb = dt.Rows[0]["ProductDB"].ToString();
                string PF_EmpId = EmpId;

                sqlcmd = "with ope as(select EmpId,sum(Expense) Expense from " + ProductDb + ".dbo.PF_Expense  where EmpId = '" + PF_EmpId + "' group by EmpId  )," +
                        "  opp as(  select EmpId,sum(Profit) Profit from " + ProductDb + ".dbo.PF_Profit  where EmpId = '" + PF_EmpId + "' group by EmpId)," +
                        " opfr as( select EmpId, sum( EmpContribution ) EmpContribution from " + ProductDb + ".dbo.PF_PFRecord  where  EmpId = '" + PF_EmpId + "' group by EmpId)," +
                        " ob as(select 'Closing Balance' as YearMonth, opfr.EmpID,SUBSTRING(opcs.EmpCardNo,8,6)+' ('+opcs.EmpProximityNo+')' as EmpCardNo,EmpName,DsgName, convert(varchar(10),PfDate,105) PfDate ,convert(varchar(10),EmpJoiningDate,105) EmpJoiningDate," +
                        " ISNULL(EmpContribution,0) ProvidentFund,isnull(Expense,0) OthersDeduction,isnull(Profit,0) OthersPay ,opcs. PFCompanyId CompanyId,PFCompanyName CompanyName,PfAddress Address from opfr left join ope on opfr.EmpID=ope.EmpId " +
                        "left join opp on opfr.EmpID=opp.EmpId  inner join v_PF_MemberListAll opcs on '" + ProductDb + "'+opfr.EmpID=opcs.ProductDB+opcs.EmpId inner join PF_CompanyInfo on opcs.PFCompanyId=PF_CompanyInfo.PFCompanyId)," +
                        " pr as(select format(pfr. Month,'MM-yyyy')  YearMonth, pfr.EmpId,SUBSTRING(pcs.EmpCardNo,8,6)+' ('+pcs.EmpProximityNo+')' as EmpCardNo,EmpName,DsgName, convert(varchar(10),pcs.PfDate,105) PfDate ,convert(varchar(10),pcs.EmpJoiningDate,105) EmpJoiningDate," +
                        " EmpContribution ProvidentFund,isnull(pe.Expense,0) OthersDeduction,isnull(pp.Profit,0) OthersPay ,pcs.PFCompanyId CompanyId, PFCompanyName CompanyName,PFAddress Address from " + ProductDb + ".dbo.PF_PFRecord" +
                        "  pfr inner join v_PF_MemberListAll pcs on '" + ProductDb + "'+pfr.EmpID=pcs.ProductDB+pcs.EmpId inner join PF_CompanyInfo on pcs.PFCompanyId=PF_CompanyInfo.PFCompanyId  left join " + ProductDb + ".dbo.PF_Expense " +
                        " pe on pfr.EmpID=pe.Empid and CONVERT(VARCHAR(7), pfr.Month  , 120)= CONVERT(VARCHAR(7), pe.Month  , 120) left join " + ProductDb + ".dbo.v_PF_MonthlyProfit pp  on pfr.EmpID=pp.EmpId and CONVERT(VARCHAR(7), pfr.Month  , 120)=  pp.Month" +
                        "  where   pcs.PFCompanyId='" +
                        ddlCompanyName.SelectedValue + "' and pcs.EmpId='" + PF_EmpId + "' ) " +
                        "select * from ob union all select * from pr ";

                    
                    sqlDB.fillDataTable(sqlcmd, dt=new DataTable());
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->No Data Found!"; return;
                    }
                  
                

                Session["__MonthlyPFSheet__"] = dt;
                sqlcmd = "select sum(EmpContribution ) EmpContribution,format(min(Month), 'MM/yyyy') as FromDate, format(max(Month), 'MM/yyyy') as ToDate from RSSHRM.dbo.PF_PFRecord  where EmpId = '" + PF_EmpId + "'";
                sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=MonthlyPFSheet-"+ dt.Rows[0]["FromDate"].ToString()+" to " + dt.Rows[0]["ToDate"].ToString() + " - ');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }
        protected void gvPFWithdrawList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                    LinkButton btnPreview = (LinkButton)e.Row.FindControl("btnPreview");
                    btnPreview.Enabled = false;
                    btnPreview.ForeColor = Color.Silver;
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