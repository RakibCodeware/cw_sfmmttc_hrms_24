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

namespace SigmaERP.payroll
{
    public partial class advance_info : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //permission=357;

            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";

            int[] pagePermission = { 357 };
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpType(rblEmpType);
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                
            }
        }

        DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];

                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();

                if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Master Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Viewer"))
                {

                    classes.commonTask.LoadBranch(ddlCompanyName);
                    classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
                  //  classes.commonTask.LoadShift(ddlShiftName, ViewState["__CompanyId__"].ToString());


                }
                else
                {
                    dtSetPrivilege = new DataTable();
                    classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                    classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
                  //  classes.commonTask.LoadShift(ddlShiftName, ViewState["__CompanyId__"].ToString());

                    //if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Admin"))
                    //{
                    //    btnPreview.CssClass = ""; btnPreview.Enabled = false;
                    //}

                    //sqlDB.fillDataTable("select * from UserPrivilege where PageName='advance_info.aspx' and UserId=" + getCookies["__getUserId__"].ToString() + "", dtSetPrivilege);

                    //if (dtSetPrivilege.Rows.Count > 0)
                    //{
                    //    if (bool.Parse(dtSetPrivilege.Rows[0]["ReadAction"].ToString()).Equals(true))
                    //    {
                    //        btnPreview.CssClass = "css_btn"; btnPreview.Enabled = true;
                    //    }
                    //    else
                    //    {
                    //        tblGenerateType.Visible = false;
                    //        WarningMessage.Visible = true;
                    //        btnPreview.CssClass = ""; btnPreview.Enabled = false;
                    //    }

                    //}
                    //else
                    //{
                    //    tblGenerateType.Visible = false;
                    //    WarningMessage.Visible = true;
                    //    btnPreview.CssClass = ""; btnPreview.Enabled = false;
                    //}

                }

                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();

                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, CompanyId);
               // addAllTextInShift();

            }
            catch { }
        }

        private void addAllTextInShift()
        {
            if (ddlShiftName.Items.Count > 2)
                ddlShiftName.Items.Insert(1, new ListItem("All", "00"));
        }
        protected void rblGenerateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblMessage.InnerText = "";

                if (!rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    txtEmpCardNo.Enabled = true;
                    txtEmpCardNo.Focus();
                    pnl1.Enabled = false;
                    ddlShiftName.Enabled = false;
                }
                else
                {
                    txtEmpCardNo.Enabled = false;
                    pnl1.Enabled = true;
                    ddlShiftName.Enabled = true;
                }
            }
            catch { }
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                addAllTextInShift();
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, CompanyId);
            }
            catch { }
        }

        protected void ddlShiftName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lstAll.Items.Clear();
                lstSelected.Items.Clear();
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();

                if (ddlShiftName.SelectedItem.ToString().Equals("All"))
                {

                    string ShiftList = classes.commonTask.getShiftList(ddlShiftName);
                    classes.commonTask.LoadDepartmentByCompanyAndShiftInListBox(CompanyId, ShiftList, lstAll);
                    return;
                }
                classes.commonTask.LoadDepartmentByCompanyAndShiftInListBox(CompanyId, "in (" + ddlShiftName.SelectedValue.ToString() + ")", lstAll);
            }
            catch { }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            //if (ddlSelectMonth.SelectedIndex < 1) 
            //{
            //    lblMessage.InnerText = "warning-> Please select any month !"; ddlSelectMonth.Focus(); return;
            //}
            if (rblGenerateType.SelectedValue == "0" && lstSelected.Items.Count == 0) 
            {
                lblMessage.InnerText = "warning-> Please select any department !"; lstSelected.Focus(); return;
            }
            if (rblGenerateType.SelectedValue != "0" && txtEmpCardNo.Text.Trim().Length==0)
            {
                lblMessage.InnerText = "warning-> Please type valid card no !"; txtEmpCardNo.Focus(); return;
            }
           
            allAdanceReport();
        }

        private void allAdanceReport()
        {
            try
            {              
                string condisions = "";
                condisions = "Where ISNULL(IsDeleted,0)=0 and ISNULL(IsDeletedDetails,0)=0 and ld.CompanyId='" + (ddlCompanyName.SelectedValue.Equals("0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString()) + "'";
                if (rblGenerateType.SelectedValue == "0")
                {
                    condisions += " and DptId " + classes.commonTask.getDepartmentList(lstSelected)+" and EmpTypeId="+rblEmpType.SelectedValue;
                }                    
                else
                    condisions += " and EmpCardNo like'%"+txtEmpCardNo.Text.Trim()+"'";

                string getSQLCMD="";

                // for find out last advance info for all employee
                if (rblReportType.SelectedValue.ToString() == "allInfo")
                {
                    getSQLCMD = @"SELECT CustomOrdering,ld.EmpId,ld.CompanyId, EmpName, DptId, DptName, DsgName, SUBSTRING(EmpCardNo,8,6) as EmpCardNo, EmpPresentSalary, BasicSalary, EmpProximityNo, CompanyName, Address, ld.LoanID, LoanAmount, sum(lmd.Amount) as PaidAmount, InstallmentAmount,Format( DeductFrom,'MM-yyyy') as  DeductFrom , ParticularAmount,convert(varchar(10), LoanTakeDate,105) as  LoanTakeDate,convert(varchar(10), LoanTakeDate,120),StatusId,Status,EmpType,EmpTypeId,SUBSTRING( convert(varchar(10), MAX(lmd.Month),105),4,7) as LastInstallmentAt,count(lmd.Month) as PaidInstallmentNo
                      FROM v_Payroll_LoanInfoDetails ld left join Payroll_LoanMonthlySetup lmd on ld.LoanID=lmd.LoanID and lmd.IsPaid=1 " + condisions + @"   group by CustomOrdering,
                      ld.EmpId,ld.CompanyId, EmpName, DptId, DptName, DsgName, SUBSTRING(EmpCardNo,8,6), EmpPresentSalary, BasicSalary, EmpProximityNo, CompanyName, Address, ld.LoanID, LoanAmount, PaidAmount, InstallmentAmount,Format( DeductFrom,'MM-yyyy')  , ParticularAmount,convert(varchar(10), LoanTakeDate,105),convert(varchar(10), LoanTakeDate,120) ,StatusId,Status,EmpType,EmpTypeId 
					  ORDER BY CompanyId,DptId,CustomOrdering,ld.LoanID desc,convert(varchar(10), LoanTakeDate,120) desc";

                    DataTable dt = new DataTable();
                    sqlDB.fillDataTable(getSQLCMD, dt);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found!"; return;
                    }
                    Session["__AllAdvanceInfo__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=AllAdvanceInfo');", true);  //Open New Tab for Sever side code

                }
                else
                {
                    //             getSQLCMD = @"SELECT CustomOrdering,ld.EmpId,ld.CompanyId, EmpName, DptId, DptName, DsgName, SUBSTRING(EmpCardNo,8,6) as EmpCardNo, EmpPresentSalary, BasicSalary, EmpProximityNo, CompanyName, Address, ld.LoanID, LoanAmount,  InstallmentAmount,Format( DeductFrom,'MM-yyyy') as  DeductFrom , ParticularAmount,convert(varchar(10), LoanTakeDate,105) as  LoanTakeDate,convert(varchar(10), LoanTakeDate,120),StatusId,Status,EmpType,EmpTypeId,ld.LoanDetailsID,Format( lmd.Month,'MM-yyyy') as InstallmentMonth ,lmd.Amount as InstallmentAmountPaid
                    //               FROM v_Payroll_LoanInfoDetails ld left join Payroll_LoanMonthlySetup lmd on ld.LoanID=lmd.LoanID and lmd.IsPaid=1 " + condisions + @" 
                    //ORDER BY CompanyId,DptId,CustomOrdering,ld.LoanID desc,convert(varchar(10), LoanTakeDate,120), lmd.Month";

                    string companyList = ddlCompanyName.SelectedValue.Equals("0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                    bool hasEmpard = AccessControl.hasEmpcardPermission(txtEmpCardNo.Text.Trim(), companyList);
                    if (!hasEmpard)
                    {
                        return;
                    }
                    getSQLCMD = @"SELECT CustomOrdering,ld.EmpId,ld.CompanyId, EmpName, DptId, DptName, DsgName, SUBSTRING(EmpCardNo,8,6) as EmpCardNo, EmpPresentSalary, BasicSalary, EmpProximityNo, CompanyName, Address, ld.LoanID, LoanAmount, sum(lmd.Amount) as PaidAmount, InstallmentAmount,Format( DeductFrom,'MM-yyyy') as  DeductFrom , ParticularAmount,convert(varchar(10), LoanTakeDate,105) as  LoanTakeDate,convert(varchar(10), LoanTakeDate,120),StatusId,Status,EmpType,EmpTypeId,SUBSTRING( convert(varchar(10), MAX(lmd.Month),105),4,7) as LastInstallmentAt,count(lmd.Month) as PaidInstallmentNo
                      FROM v_Payroll_LoanInfoDetails ld left join Payroll_LoanMonthlySetup lmd on ld.LoanID=lmd.LoanID and lmd.IsPaid=1 " + condisions + @"   group by CustomOrdering,
                      ld.EmpId,ld.CompanyId, EmpName, DptId, DptName, DsgName, SUBSTRING(EmpCardNo,8,6), EmpPresentSalary, BasicSalary, EmpProximityNo, CompanyName, Address, ld.LoanID, LoanAmount, PaidAmount, InstallmentAmount,Format( DeductFrom,'MM-yyyy')  , ParticularAmount,convert(varchar(10), LoanTakeDate,105),convert(varchar(10), LoanTakeDate,120) ,StatusId,Status,EmpType,EmpTypeId 
					  ORDER BY CompanyId,DptId,CustomOrdering,ld.LoanID desc,convert(varchar(10), LoanTakeDate,120) desc";
                    DataTable dt = new DataTable();
                    sqlDB.fillDataTable(getSQLCMD, dt);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found!";
                        return;
                    }
                    var empIds = dt.AsEnumerable() .Select(row => row.Field<string>("EmpId")).ToList();
                    // Convert the list of EmpId values to a comma-separated string
                    string empIdsAsString = string.Join(",", empIds);
                    
                    Session["__AllAdvanceInfoDetails__"] = dt;
                     dt = new DataTable();
                    getSQLCMD = " select LoanID,EmpID,format( Month,'MM-yyyy') as  Month,Amount from Payroll_LoanMonthlySetup where Isnull(IsPaid,0)=1 and EmpID in(" + empIdsAsString + ") order by year(Month) desc,Month(Month) desc";
                    sqlDB.fillDataTable(getSQLCMD, dt);
                    Session["__AllAdvanceInfoDetailsInstallment__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=AllAdvanceInfoDetails');", true);  //Open New Tab for Sever side code
                }

                
            }
            catch { }
        }

       
    }
}