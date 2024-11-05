using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll.advance
{
    public partial class advance_list : System.Web.UI.Page
    {// Status {0=Due/Current Loan,1=Paid,2=Cash Refund,3=Waived }
        DataTable dt;
        DataTable dtSetPrivilege;
        string query = "";

        //permission=358;
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            int[] pagePermission = { 357 };
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                //txtFromDate.Text = "01-" + DateTime.Now.ToString("MM-yyyy");
                //txtToDate.Text = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) + "-" + DateTime.Now.ToString("MM-yyyy");
                commonTask.LoadEmpTypeWithAll(rblEmpType,"");
                setPrivilege();
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                commonTask.LoadAdvanceStatus(ddlStatus);
                loadExistsLoan();
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
                string DptId = getCookies["__DptId__"].ToString();
                //string[] AccessPermission = new string[0];
                classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
               // AccessPermission = checkUserPrivilege.checkUserPrivilegeForList(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "advance.aspx", ddlCompanyList, gvAdvanceInfo, btnSearch);

                //ViewState["__ReadAction__"] = AccessPermission[0];
                //ViewState["__WriteAction__"] = AccessPermission[1];
                //ViewState["__UpdateAction__"] = AccessPermission[2];
                //ViewState["__DeletAction__"] = AccessPermission[3];



            }
            catch { }
        }

        private void loadExistsLoan()
        {
            try
            {
                string CompanyId = (ddlCompanyList.SelectedIndex < 1) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue;
                string Status = (ddlStatus.SelectedValue == "00") ? " and  li.Status<>0" : " and  li.Status="+ddlStatus.SelectedValue;
                if (txtEmpCardNo.Text.Trim().Length > 0)
                    Status += " and ei.EmpCardNo like'%" + txtEmpCardNo.Text.Trim() + "'";
                else if (rblEmpType.SelectedValue != "0")
                    Status += " and ei.EmpTypeId=" + rblEmpType.SelectedValue;

                //string query = @"select SUBSTRING(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' as EmpCardNo,ei.EmpId,ei.EmpName, li.LoanID,LoanAmount,InstallmentAmount,format(DeductFrom,'MM-yyyy') as DeductFrom,Isnull(li.PaidAmount,0) as PaidAmount,li.LoanAmount- Isnull(li.PaidAmount,0) as DueAmount, ads.Status,convert(varchar(10),li.StatusDate,105) as StatusDate ,li.StatusNote,format( max(ins.Month),'MM-yyyy') as LastInstallmentMonth,max(ins.Month) from Payroll_LoanInfo as li inner join Personnel_EmployeeInfo as ei on li.EmpId=ei.EmpId left join HRD_AdvanceStatus ads on li.Status=ads.StatusID left join Payroll_LoanMonthlySetup ins on li.LoanID=ins.LoanID and ins.IsPaid=1 where li.CompanyId='" + CompanyId + "' " + Status + "  and ISNULL(IsDeleted,0)=0  group by SUBSTRING(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' ,ei.EmpId,ei.EmpName, li.LoanID,LoanAmount,InstallmentAmount,format(DeductFrom,'MM-yyyy'),Isnull(li.PaidAmount,0) ,li.LoanAmount- Isnull(li.PaidAmount,0) , ads.Status,convert(varchar(10),li.StatusDate,105)  ,li.StatusNote order by max(ins.Month) Desc";

                string query = @"select SUBSTRING(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' as EmpCardNo,ei.EmpId,ei.EmpName, li.LoanID,LoanAmount,InstallmentAmount,format(DeductFrom,'MM-yyyy') as DeductFrom, ISNULL(sum(ins.Amount),0) as PaidAmount, li.LoanAmount- ISNULL(sum(ins.Amount),0) as DueAmount, ads.Status,convert(varchar(10),li.StatusDate,105) as StatusDate ,li.StatusNote,format( max(ins.Month),'MM-yyyy') as LastInstallmentMonth,max(ins.Month) from Payroll_LoanInfo as li inner join Personnel_EmployeeInfo as ei on li.EmpId=ei.EmpId left join HRD_AdvanceStatus ads on li.Status=ads.StatusID left join Payroll_LoanMonthlySetup ins on li.LoanID=ins.LoanID and ins.IsPaid=1 where li.CompanyId='" + CompanyId + "' and  li.Status<>0  and ISNULL(IsDeleted,0)=0  group by SUBSTRING(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' ,ei.EmpId,ei.EmpName, li.LoanID,LoanAmount,InstallmentAmount,format(DeductFrom,'MM-yyyy'),Isnull(li.PaidAmount,0) ,li.LoanAmount- Isnull(li.PaidAmount,0) , ads.Status,convert(varchar(10),li.StatusDate,105)  ,li.StatusNote order by max(ins.Month) Desc";

                sqlDB.fillDataTable(query, dt = new DataTable());
                gvAdvanceInfo.DataSource = dt;
                gvAdvanceInfo.DataBind();
            }
            catch { }
        }

        protected void gvAdvanceInfo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                loadExistsLoan();
                gvAdvanceInfo.PageIndex = e.NewPageIndex;
                gvAdvanceInfo.DataBind();

            }
            catch (Exception ex)
            {

            }
        }









        protected void gvAdvanceInfo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onmouseover"] = "javascript:SetMouseOver(this)";
                    e.Row.Attributes["onmouseout"] = "javascript:SetMouseOut(this)";
                    HyperLink hlAttachment = (HyperLink)e.Row.FindControl("hlAttachment");
                    string LoanID = gvAdvanceInfo.DataKeys[e.Row.RowIndex].Value.ToString();
                    if (File.Exists(Server.MapPath("/EmployeeImages/AdvanceDocument/" + LoanID + ".jpg")))
                    {
                        hlAttachment.Visible = true;
                        hlAttachment.NavigateUrl = "~/EmployeeImages/AdvanceDocument/" + LoanID + ".jpg";
                    }
                        
                    else
                        hlAttachment.Visible = false;


                }
            }
            catch { }
        }     
      
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                loadExistsLoan();
            }
            catch { }
        }

        protected void gvAdvanceInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("alter"))
            {
                Response.Redirect("/payroll/advance/advance_entry_final.aspx?Action=alter&&EmpId=" + e.CommandArgument.ToString(), false);
            }
            else if (e.CommandName.Equals("refund"))
            {
                Response.Redirect("/payroll/advance/advance_entry_final.aspx?Action=refund&&EmpId=" + e.CommandArgument.ToString(), false);
            }
            else if (e.CommandName.Equals("waive"))
            {
                Response.Redirect("/payroll/advance/advance_entry_final.aspx?Action=waive&&EmpId=" + e.CommandArgument.ToString(), false);
            }
           
        }
        

    }
}