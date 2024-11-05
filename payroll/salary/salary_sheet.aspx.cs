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

namespace SigmaERP.payroll.salary
{
    public partial class salary_sheet : System.Web.UI.Page
    {
        //Permission=337
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 337, 474 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.loadEmpTye(rblEmployeeType);
                rblEmployeeType.SelectedValue = "1";
                setPrivilege(userPagePermition);
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                ViewState["__IsGerments__"] = classes.commonTask.IsGarments();
                //if (!bool.Parse(ViewState["__IsGerments__"].ToString()))
                //    trHideForIndividual.Visible = false;

            }
        }
        DataTable dtSetPrivilege;
        private void setPrivilege(int[] permissions)
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];

                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CShortName__"] = "MRC";
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                if (permissions.Contains(474))
                {
                    chkbanksheet.Visible = true;
                }
                //------------load privilege setting inof from db------
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "salary_sheet_Report.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];
                commonTask.LoadDepartmentByCompanyInListBox(ViewState["__CompanyId__"].ToString(), lstAll);
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, ViewState["__CompanyId__"].ToString());
                commonTask.loadBankNameCompanyWise(ViewState["__CompanyId__"].ToString(), ddlBankSheet);
                //-----------------------------------------------------


            }
            catch { }
        }
       

        protected void rblGenerateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {


                if (!rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    txtEmpCardNo.Enabled = true;
                    pnl1.Enabled = false;
                    rblEmployeeType.Visible = false;
                    trHideForIndividualLabel.Visible = false;
                    pnl1.Visible = false;
                    txtEmpCardNo.Focus();

                }
                else
                {
                    txtEmpCardNo.Enabled = false;
                    pnl1.Enabled = true;
                    rblEmployeeType.Visible = true;
                    trHideForIndividualLabel.Visible = true;
                    pnl1.Visible = true;
                    rblEmployeeType.SelectedValue = "1";
                    rblPaymentType.SelectedValue = "Cash";
                }
                //if (!bool.Parse(ViewState["__IsGerments__"].ToString()))
                //    trHideForIndividual.Visible = false;
            }
            catch { }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (ddlSelectMonth.SelectedValue == "0")
            { lblMessage.InnerText = "warning->Please select any Month!"; ddlSelectMonth.Focus(); return; }
            if (rblGenerateType.SelectedItem.Text.Equals("All") && lstSelected.Items.Count < 1) { lblMessage.InnerText = "warning->Please select any Department"; lstSelected.Focus(); return; }
            if (!rblGenerateType.SelectedItem.Text.Equals("All") && txtEmpCardNo.Text.Trim().Length < 4) { lblMessage.InnerText = "warning->Please type valid Card No!(Minimum last 4 digit.)"; txtEmpCardNo.Focus(); return; }
        
               generateSalarySheet();
        }
        private void generateSalarySheet()
        {
            try
            {
                string CompanyList = "";
                string DepartmentList = "";

                string[] monthInfo = ddlSelectMonth.SelectedValue.Split('/');
                string yearMonth = "";
                if (monthInfo.Length > 1)
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "' AND FromDate='" + monthInfo[1] + "' AND ToDate='" + monthInfo[2] + "'";
                else
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "'";
                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }
                if (chkForAllCompany.Checked)
                {
                    CompanyList = classes.Payroll.getCompanyList(ddlCompanyName);
                    DepartmentList = classes.commonTask.getDepartmentList();
                }
                else
                {
                    CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();

                    //if (ddlShiftName.SelectedItem.ToString().Equals("All"))
                    //{

                    //    ShiftList = classes.Payroll.getSftIdList(ddlShiftName);
                    //    DepartmentList = classes.commonTask.getDepartmentList();
                    //}
                    //else
                    //{
                    //    ShiftList = ddlShiftName.SelectedValue.ToString();
                    //    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                    //}
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                }

                string Condition = (bool.Parse(ViewState["__IsGerments__"].ToString())) ? " And EmpTypeId=" + rblEmployeeType.SelectedValue + " And SalaryCount='" + rblPaymentType.SelectedValue + "'" : "";
                Condition = " And EmpTypeId=" + rblEmployeeType.SelectedValue + "";
                string getSQLCMD;
                DataTable dt = new DataTable();
                if (chkIsBankfordQatar.Checked)
                {
                    if (ddlBankSheet.SelectedIndex >= 0)
                    {
                        banksheetGenarate(yearMonth, DepartmentList);

                    }
                }
       
              
                if (chkBankForwardingLetter.Checked)
                {
                    getSQLCMD = "SELECT  EmpProximityNo as Sl,EmpId, EmpName, Substring(EmpCardNo,10,6) as EmpCardNo, DptName, DptId, CompanyId, TotalSalary, MobileNo,Format(YearMonth,'MMMM-yyyy') as YearMonth ,CompanyName ,EmpAccountNo  FROM   v_MonthlySalarySheet where " +
                           " IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + " " + yearMonth + " " + Condition + "  AND SalaryCount='Bank' and IsSeperationGeneration='0' " +
                           " ORDER BY CONVERT(int,DptId), CustomOrdering ";
                    Session["__ReportTitle__"] = "";
                    sqlDB.fillDataTable(getSQLCMD, dt);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found."; return;
                    }
                    Session["__SalarySheetBankFordLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=SalarySheetBankFordLetter');", true);
                }
                else if (chkExcel.Checked)
                {
                    getSQLCMD = @"select SUBSTRING(EmpCardNo,8,6) as [Card No],EmpName as [Name],DptName as [Department],DsgName as [Designation],PresentDay as [Present] ,AbsentDay as [Absent],(CasualLeave + SickLeave + AnnualLeave) as [Leave],(WeekendHoliday+FestivalHoliday
) as [W&H],EmpPresentSalary as [Gross Salary],AbsentDeduction as [Absent Deduction],AdvanceDeduction as [Advance],ProfitTax as [Tax],OthersDeduction as [Others Deduction],(AbsentDeduction + AdvanceDeduction + OthersDeduction + ProfitTax) as [Total Deduction],TotalSalary as [Net Payable] from  v_MonthlySalarySheet where IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + " " + yearMonth + " " + Condition + "  and IsSeperationGeneration='0' " +
                           " ORDER BY CONVERT(int,DptId), CustomOrdering ";
                    sqlDB.fillDataTable(getSQLCMD, dt);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found."; return;
                    }
                    Session["__salarySheetExcel__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/payroll/salary/salary_sheet_excel.aspx?for=SalarySheet&&company=" + ddlCompanyName.SelectedItem.Text + "&&month=" + ddlSelectMonth.SelectedItem.Text.Trim() + "');", true);
                }
                else if (chkBankForwardingLetterXL.Checked)
                {
                    getSQLCMD = @"select 'All' as [Department], SUBSTRING(EmpCardNo,8,6) as [Card No],EmpName as [Name],EmpAccountNo as[Account No],TotalSalary as [Net Payable] from  v_MonthlySalarySheet where SalaryCount='Bank' AND IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + " " + yearMonth + " " + Condition + "  and IsSeperationGeneration='0' " +
                           " ORDER BY CONVERT(int,DptId), CustomOrdering ";
                    sqlDB.fillDataTable(getSQLCMD, dt);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found."; return;
                    }
                    Session["__salarySheetExcel__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/payroll/salary/salary_sheet_excel.aspx?for=SalaryBankForwardingSheet&&company=" + ddlCompanyName.SelectedItem.Text + "&&month=" + ddlSelectMonth.SelectedItem.Text.Trim() + "');", true);
                }
                else if (rblReportType.SelectedValue == "sheet" || rblReportType.SelectedValue == "slip")
                {
                    if (rblGenerateType.SelectedItem.Text.Equals("All"))
                    {
                        if (rblSheet.SelectedValue == "0")
                        {
                            getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpName,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo , AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                                " OverTime as TotalOTHour, OTRate, round(OverTimeAmount,0) as TotalOTAmount, AttendanceBonus, DptName, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                                " DptId, CompanyId, DsgName, TotalSalary, GrdName, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable,NetPayable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,EmpNameBn, DptNameBn, DsgNameBn, GrdNameBangla " +
                                " FROM   v_MonthlySalarySheet " +
                                " where " +
                                " IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "  " + yearMonth + " " + Condition + "  AND IsSeperationGeneration='0' " +
                                " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                            Session["__ReportTitle__"] = "";
                        }
                        else
                        {
                            getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpName,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo , AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                                 " OverTime as TotalOTHour, OTRate, round(OverTimeAmount,0) as TotalOTAmount, AttendanceBonus, DptName, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                                 " DptId, CompanyId, DsgName, TotalSalary, GrdName, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable, NetPayable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,SeparationTypeName,EmpNameBn, DptNameBn, DsgNameBn, GrdNameBangla " +
                                 " FROM   v_MonthlySalarySheet " +
                                 " where " +
                                 " IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "  " + yearMonth + " " + Condition + " AND IsSeperationGeneration='1' " +
                                 " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                            Session["__ReportTitle__"] = "[Separation]";
                        }


                        sqlDB.fillDataTable(getSQLCMD, dt);
                        if (dt.Rows.Count == 0)
                        {
                            lblMessage.InnerText = "warning->Data not found."; return;
                        }

                    }
                    else
                    {
                        if (rblSheet.SelectedValue == "0")
                        {
                            getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpName,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo, AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                               " OverTime as TotalOTHour, OTRate, round(OverTimeAmount,0) as TotalOTAmount, AttendanceBonus, DptName, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                               " DptId, CompanyId, DsgName, TotalSalary, GrdName, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable,round(NetPayable,0) as NetPayable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,SalaryCount,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,EmpNameBn, DptNameBn, DsgNameBn, GrdNameBangla " +
                               " FROM   v_MonthlySalarySheet " +
                               " where " +
                               " IsActive='1' AND CompanyId in(" + CompanyList + ") " + yearMonth + " AND EmpCardNo Like '%" + txtEmpCardNo.Text.Trim() + "' AND IsSeperationGeneration='0' " +
                               " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                            Session["__ReportTitle__"] = "";
                        }
                        else
                        {
                            getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpName,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo, AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                                " OverTime as TotalOTHour, OTRate, round(OverTimeAmount,0) as TotalOTAmount, AttendanceBonus, DptName, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                                " DptId, CompanyId, DsgName, TotalSalary, GrdName, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable,round(NetPayable,0) as NetPayable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,SalaryCount,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,SeparationTypeName,EmpNameBn, DptNameBn, DsgNameBn, GrdNameBangla " +
                                " FROM   v_MonthlySalarySheet " +
                                " where " +
                                " IsActive='1' AND CompanyId in(" + CompanyList + ") " + yearMonth + " AND EmpCardNo Like '%" + txtEmpCardNo.Text.Trim() + "' AND IsSeperationGeneration='1' " +
                                " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                            Session["__ReportTitle__"] = "[Separation]";
                        }

                        sqlDB.fillDataTable(getSQLCMD, dt);
                        if (dt.Rows.Count == 0)
                        {
                            lblMessage.InnerText = "warning-> Data no found."; return;
                        }
                        rblEmployeeType.SelectedValue = dt.Rows[0]["EmptypeId"].ToString();
                        rblPaymentType.SelectedValue = dt.Rows[0]["SalaryCount"].ToString();


                    }

                    if (rblReportType.SelectedValue == "slip")
                    {
                        Session["__Language__"] = "Bangla";
                        Session["__PaySlip__"] = dt;
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=PaySlipNew-" + ddlSelectMonth.SelectedItem.Text + "');", true);  //Open New Tab for Sever side code
                    }
                    else
                    {
                        Session["__Language__"] = "English";
                        Session["__SalarySheet__"] = dt;
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=SalarySheetNew-" + ddlSelectMonth.SelectedItem.Text.Replace('-', '/') + "-True-" + rblEmployeeType.SelectedValue + "-" + rblPaymentType.SelectedValue + "-" + rblSheet.SelectedValue + "');", true);  //Open New Tab for Sever side code

                    }

                }
                else // summary
                {
                    if (rblSheet.SelectedValue == "0")
                    {
                        getSQLCMD = "SELECT count(Empid) as ActiveDay,sum(round(ProfitTax,0)) as ProfitTax, sum(round(AbsentDeduction,0)) as AbsentDeduction,sum(round(ProvidentFund,0)) as ProvidentFund , sum(EmpNetGross) as EmpNetGross, sum(round(Payable,0)) as Payable, sum(round(NetPayable,0)) as NetPayable,sum(round( OverTimeAmount,0)) as TotalOTAmount , sum(AttendanceBonus) as AttendanceBonus,sum(AdvanceDeduction) as AdvanceDeduction,sum(Stampdeduct) as Stampdeduct,  CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId), case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end as YearMonth" +
                            " From v_MonthlySalarySheet where " +
                            " IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "  " + yearMonth + " " + Condition + "  AND IsSeperationGeneration='0' " +
                            " group by CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId),case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end" +
                            " ORDER BY CONVERT(int,DptId)";
                        Session["__SummaryReportTitle__"] = "";
                    }
                    else
                    {
                        getSQLCMD = "SELECT count(Empid) as ActiveDay,sum(round(ProfitTax,0)) as ProfitTax, sum(round(AbsentDeduction,0)) as AbsentDeduction,sum(round(ProvidentFund,0)) as ProvidentFund , sum(EmpNetGross) as EmpNetGross, sum(round(Payable,0)) as Payable, sum(round(NetPayable,0)) as NetPayable,sum(round( OverTimeAmount,0)) as TotalOTAmount , sum(AttendanceBonus) as AttendanceBonus,sum(AdvanceDeduction) as AdvanceDeduction,sum(Stampdeduct) as Stampdeduct,  CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId), case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end as YearMonth" +
                             " From v_MonthlySalarySheet where " +
                             " IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "  " + yearMonth + " " + Condition + " AND IsSeperationGeneration='1' " +
                             " group by CompanyId, CompanyName, Address, DptName,CONVERT(int,DptId),case when FromDate is null then FORMAT(YearMonth,'MMMM-yyyy') else FORMAT(YearMonth,'MMMM-yyyy')+' ['+ convert(varchar(10), FromDate,105)+' to '+convert(varchar(10), ToDate,105) +']' end" +
                             " ORDER BY CONVERT(int,DptId)";
                        Session["__SummaryReportTitle__"] = "[Separation]";
                    }
                    sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found."; return;
                    }
                    if (rblEmployeeType.SelectedValue == "1")
                        Session["__SummaryReportTitle__"] = "Worker Wages of " + dt.Rows[0]["YearMonth"].ToString() + " " + Session["__SummaryReportTitle__"].ToString();
                    else
                        Session["__SummaryReportTitle__"] = "Executive Salary of " + dt.Rows[0]["YearMonth"].ToString() + " " + Session["__SummaryReportTitle__"].ToString();
                    Session["__SummaryOfSalary__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=SummaryOfSalaryNew-" + rblReportType.SelectedValue + "-" + rblEmployeeType.SelectedValue + "');", true);  //Open New Tab for Sever side code

                }
            }
            catch { }
        }
        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                classes.commonTask.LoadDepartmentByCompanyInListBox(CompanyId, lstAll);
                //classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                //addAllTextInShift();
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, CompanyId);
            }
            catch { }
        }

      
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);

        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }       
       
        private void banksheetGenarate(string yearmonth,string departmentList)
        {
           Session["__bankShortname__"] = "";
            DataTable dt = new DataTable();
            string paymentType = "";      


            string condition = "";
            if (ddlBankSheet.SelectedIndex>0)
            {
                string[] bankIdandShortname = ddlBankSheet.SelectedValue.Split('_');


                string bankId = bankIdandShortname[0];
                Session["__bankShortname__"] = bankIdandShortname[1];
                condition = "and ecs.BankId="+ bankId + "";
            }
          
            string getSQLCMD = "select  ep.NationIDCardNo, ecs.BankId, Isnull(ep.EmpVisaNo,'') as EmpVisaNo,ei.EmpName,bi.BankShortName,ecs.EmpAccountNo,'M' as SalaryFrequency,pms.PayableDays,pms.EmpPresentSalary as BasicSalary,pms.TotalSalary,ExtraOtHour,ExtraOtAmount,(pms.AdvanceDeduction+pms.AbsentDeduction) as Deduction,case when ecs.BankId=54 then 'Salary' else 'Normal Payment' end  as PaymentType,''  as Notes from Payroll_MonthlySalarySheet pms inner join Personnel_EmployeeInfo ei on ei.EmpId = pms.EmpId  inner join Personnel_EmpCurrentStatus ecs on ei.EmpId = ecs.EmpId left join Personnel_EmpPersonnal ep on ei.EmpId = ep.EmpId left join Hrd_BankInfo bi on ecs.BankId = bi.BankId  where ecs.companyId in (" + ddlCompanyName.SelectedValue+") and ecs.Isactive = 1 and ei.EmpTypeId = 1 "+ condition + " "+yearmonth+" and pms.DptId "+departmentList+"";
            sqlDB.fillDataTable(getSQLCMD, dt);
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Data not found."; return;
            }
             Session["__SalarySheetBankFord__"] = dt;
          ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/payroll/salary/QatarBankfordReport.aspx?for=SalarySheet&&company=" + ddlCompanyName.SelectedValue + "&&month=" + ddlSelectMonth.SelectedItem.Text.Trim() + "');", true);

        }

        protected void chkIsBankfordQatar_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsBankfordQatar.Checked)
            {
                bankshhet.Visible = true;
                chkExcel.Visible = false;
                chkBankForwardingLetterXL.Visible = false;
            }
            else
            {
                bankshhet.Visible = false;
                chkExcel.Visible = true;
                chkBankForwardingLetterXL.Visible =true;
            }
        }
    }
}