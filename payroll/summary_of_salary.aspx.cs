using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class summary_of_salary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                classes.commonTask.loadEmpTye(rblEmployeeType);
                rblEmployeeType.SelectedValue = "1";
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                //ViewState["__IsGerments__"] = classes.commonTask.IsGarments();
                ViewState["__IsGerments__"] = "True";
                if (!bool.Parse(ViewState["__IsGerments__"].ToString()))
                    trHideForIndividual.Visible = false;
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

                //------------load privilege setting inof from db------
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "summary_of_salary.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
                ViewState["__ReadAction__"] = AccessPermission[0];
                classes.commonTask.LoadDepartmentByCompanyInListBox(ViewState["__CompanyId__"].ToString(), lstAll);
                classes.Payroll.loadMonthIdByCompany(ddlMonth, ViewState["__CompanyId__"].ToString());
                //-----------------------------------------------------
            }
            catch { }
        }
        private void addAllTextInShift()
        {
            if (ddlShiftName.Items.Count > 2)
                ddlShiftName.Items.Insert(1, new ListItem("All", "00"));
        }
        protected void btnPreview_Click(object sender, EventArgs e)
        {
           
            if (ddlMonth.SelectedValue == "0") { lblMessage.InnerText = "warning->Please select any Month!"; ddlMonth.Focus(); return; }
            if (lstSelected.Items.Count < 1) { lblMessage.InnerText = "warning->Please select any Department"; lstSelected.Focus(); return; }
            generateSalarySummary();
        }

        private void generateSalarySummary()
        {
            try
            {
                string CompanyList = "";               
                string DepartmentList = "";
                string[] monthInfo = ddlMonth.SelectedValue.Split('/');
                string yearMonth = "";
                if (monthInfo.Length > 1)
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "' AND FromDate='" + monthInfo[1] + "' AND ToDate='" + monthInfo[2] + "'";
                else
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "'";
                CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                string getSQLCMD;
                DataTable dt;
                string EmpType = rblEmployeeType.SelectedValue;

                if (rblEmployeeType.SelectedValue=="1")
                    getSQLCMD = " SELECT sum( round(LateFine,0)) as LateFine  ,sum(round(AbsentDeduction,0)) as AbsentDeduction,sum(round(ProvidentFund,0)) as ProvidentFund , sum(EmpNetGross) as EmpNetGross, sum(TotalSalary) as TotalSalary,sum(round( TotalOTAmount,0)) as TotalOTAmount , sum(AttendanceBonus) as AttendanceBonus,sum(AdvanceDeduction) as AdvanceDeduction,sum(Stampdeduct) as Stampdeduct,sum(TiffinBillAmount) as TiffinBillAmount,sum(HoliDayBillAmount) as HoliDayBillAmount,sum(NightbilAmount) as NightbilAmount,  CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId), case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end as YearMonth" +
                    " FROM v_MonthlySalarySheet where IsActive = 1 and EmpTypeId = 1 and IsSeperationGeneration = " + rblSheet.SelectedValue + " "+ yearMonth + " and DptID "+DepartmentList+" and CompanyId = '"+ CompanyList + "'"+
                    " group by CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId),case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end" +
                    " ORDER BY CONVERT(int,DptId)";
           else
                    getSQLCMD = " SELECT sum(round( LateFine,0)) as LateFine  ,sum(round(AbsentDeduction,0)) as AbsentDeduction,sum(round(ProvidentFund,0)) as ProvidentFund , sum(EmpNetGross) as EmpNetGross, sum(TotalSalary) as TotalSalary,sum(round( TotalOTAmount,0)) as TotalOTAmount , sum(AttendanceBonus) as AttendanceBonus,sum(AdvanceDeduction) as AdvanceDeduction,sum(Stampdeduct) as Stampdeduct,sum(TiffinBillAmount) as TiffinBillAmount,sum(HoliDayBillAmount) as HoliDayBillAmount,sum(NightbilAmount) as NightbilAmount,sum(ProfitTax) as ProfitTax,  CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId), case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end as YearMonth" +
                    " FROM v_MonthlySalarySheet where IsActive = 1 and EmpTypeId = 2 and IsSeperationGeneration = "+ rblSheet.SelectedValue+ " "+ yearMonth + " and DptID " + DepartmentList + " and CompanyId = '" + CompanyList + "'" +
                    " group by CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId),case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end" +
                    " ORDER BY CONVERT(int,DptId)";
                sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Data not found."; return;
                }
                Session["__SummaryReportTitle__"] = (rblSheet.SelectedValue == "0") ? "" : "[ Seperation ]";                
                Session["__SummaryOfSalary__"] = dt;               
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=SummaryOfSalary-" + rblReportType.SelectedValue + "-" + EmpType + "');", true);  //Open New Tab for Sever side code
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

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lstSelected.Items.Clear();
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                classes.commonTask.LoadDepartmentByCompanyInListBox(CompanyId, lstAll);

                //classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, CompanyId);
                //classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                //addAllTextInShift();
                classes.Payroll.loadMonthIdByCompany(ddlMonth, CompanyId);
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

    }
}