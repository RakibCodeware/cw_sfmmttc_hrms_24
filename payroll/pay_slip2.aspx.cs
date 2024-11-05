using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class pay_slip : System.Web.UI.Page
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
                ViewState["__IsGerments__"] = classes.commonTask.IsGarments();
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
                ViewState["__CShortName__"] = getCookies["__CShortName__"].ToString();

                //------------load privilege setting inof from db------
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "pay_slip.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
                ViewState["__ReadAction__"] = AccessPermission[0];
                classes.commonTask.LoadDepartmentByCompanyInListBox(ViewState["__CompanyId__"].ToString(), lstAll);
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, ViewState["__CompanyId__"].ToString());
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, ViewState["__CompanyId__"].ToString());
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
            //-----------------------------------------------------






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


                if (!rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    txtEmpCardNo.Enabled = true;
                    pnl1.Enabled = false;
                    ddlShiftName.Enabled = false;
                    trHideForIndividual.Visible = false;
                    pnl1.Visible = false;
                    txtEmpCardNo.Focus();

                }
                else
                {
                    txtEmpCardNo.Enabled = false;
                    pnl1.Enabled = true;
                    ddlShiftName.Enabled = true;
                    trHideForIndividual.Visible = true;
                    pnl1.Visible = true;
                    rblEmployeeType.SelectedValue = "1";
                    rblPaymentType.SelectedValue = "Cash";
                }
                //if (!bool.Parse(ViewState["__IsGerments__"].ToString()))
                //    trHideForIndividual.Visible = false;
            }
            catch { }
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lstAll.Items.Clear();
                lstSelected.Items.Clear();
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                classes.commonTask.LoadDepartmentByCompanyInListBox(CompanyId, lstAll);
                //classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                //addAllTextInShift();
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, CompanyId);
                //ddlSelectMonth.SelectedValue = DateTime.Now.ToString("MM-yyyy");
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

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (ddlSelectMonth.SelectedValue == "0") { lblMessage.InnerText = "warning->Please select any Month!"; ddlSelectMonth.Focus(); return; }
            if (rblGenerateType.SelectedItem.Text.Equals("All") && lstSelected.Items.Count < 1) { lblMessage.InnerText = "warning->Please select any Department"; lstSelected.Focus(); return; }
            if (!rblGenerateType.SelectedItem.Text.Equals("All") && txtEmpCardNo.Text.Trim().Length < 4) { lblMessage.InnerText = "warning->Please type valid Card No!(Minimum last 4 digit.)"; txtEmpCardNo.Focus(); return; }
            generatePaySlip();
        }

        private void generatePaySlip()
        {
            try
            {
                string CompanyList = "";
                string ShiftList = "";
                string DepartmentList = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }
                //if (chkForAllCompany.Checked)
                //{
                //    CompanyList = classes.Payroll.getCompanyList(ddlCompanyName);
                //    ShiftList = classes.Payroll.getSftIdList(ddlShiftName);
                //    DepartmentList = classes.commonTask.getDepartmentList();
                //}
                //else
                //{
                //    CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();

                //    //if (ddlShiftName.SelectedItem.ToString().Equals("All"))
                //    //{

                //    //    ShiftList = classes.Payroll.getSftIdList(ddlShiftName);
                //    //    DepartmentList = classes.commonTask.getDepartmentList();
                //    //}
                //    //else
                //    //{
                //    //    ShiftList = ddlShiftName.SelectedValue.ToString();
                //    //    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                //    //}
                //    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                //}
                CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                string Condition = (bool.Parse(ViewState["__IsGerments__"].ToString())) ? "And EmpTypeId=" + rblEmployeeType.SelectedValue + " And SalaryCount='" + rblPaymentType.SelectedValue + "'" : "";
                Condition = " And EmpTypeId=" + rblEmployeeType.SelectedValue + "";
                string getSQLCMD;
                DataTable dt = new DataTable();
                if (rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                    if (rblSheet.SelectedValue == "0")
                    {
                        getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpNameBn,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo , AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                            " TotalOTHour, OTRate, round(TotalOTAmount,0) as TotalOTAmount, AttendanceBonus, DptNameBn, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                            " DptId, CompanyId, DsgNameBn, TotalSalary, GrdNameBangla, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,SalaryCount " +
                            " FROM   v_MonthlySalarySheet " +
                            " where " +
                            " IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "  AND YearMonth='" + ddlSelectMonth.SelectedItem.Value.ToString() + "' " + Condition + "  AND IsSeperationGeneration='0' " +
                            " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                        Session["__ReportTitle__"] = "";
                    }
                    else
                    {
                        getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpNameBn,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo , AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                            " TotalOTHour, OTRate, round(TotalOTAmount,0) as TotalOTAmount, AttendanceBonus, DptNameBn, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                            " DptId, CompanyId, DsgNameBn, TotalSalary, GrdNameBangla, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,SalaryCount " +
                            " FROM   v_MonthlySalarySheet " +
                             " where " +
                             " IsActive='1' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "  AND YearMonth='" + ddlSelectMonth.SelectedItem.Value.ToString() + "' " + Condition + " AND IsSeperationGeneration='1' " +
                             " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                        Session["__ReportTitle__"] = "[Separation]";
                    }


                    sqlDB.fillDataTable(getSQLCMD, dt);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Sorry any record are not founded"; return;
                    }

                }
                else
                {
                    if (rblSheet.SelectedValue == "0")
                    {
                        getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpNameBn,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo , AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                             " TotalOTHour, OTRate, round(TotalOTAmount,0) as TotalOTAmount, AttendanceBonus, DptNameBn, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                             " DptId, CompanyId, DsgNameBn, TotalSalary, GrdNameBangla, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,SalaryCount " +
                            " FROM   v_MonthlySalarySheet " +
                           " where " +
                           " IsActive='1' AND CompanyId in(" + CompanyList + ") AND YearMonth='" + ddlSelectMonth.SelectedItem.Value.ToString() + "' AND EmpCardNo Like '%" + txtEmpCardNo.Text.Trim() + "' AND IsSeperationGeneration='0' " +
                           " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                        Session["__ReportTitle__"] = "";
                    }
                    else
                    {
                        getSQLCMD = "SELECT EmpProximityNo as Sl,EmpId, EmpNameBn,EmptypeId, Substring(EmpCardNo,10,6) as EmpCardNo , AbsentDay, BasicSalary, HouseRent, MedicalAllownce, AbsentDeduction, " +
                             " TotalOTHour, OTRate, round(TotalOTAmount,0) as TotalOTAmount, AttendanceBonus, DptNameBn, CompanyName, SftName, EmpPresentSalary, Address,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount," +
                             " DptId, CompanyId, DsgNameBn, TotalSalary, GrdNameBangla, GId, GName, PresentDay,WeekendHoliday,FestivalHoliday, PayableDays, Payable, OthersAllownce, ProvidentFund, ProfitTax, LateFine, TiffinDays, TiffinTaka, TiffinBillAmount,CasualLeave,SickLeave,AnnualLeave,OfficialLeave,DormitoryRent,TotalOverTime,TotalOtherOverTime,DaysInMonth,OthersPay,OthersDeduction,ShortLeave,AdvanceDeduction,LateDays,ConvenceAllownce,NightbilAmount,NightBillDays,convert(varchar(10), EmpJoiningDate,105) EmpJoiningDate,Stampdeduct,FoodAllownce,Activeday,EmpNetGross,SalaryCount " +
                            " FROM   v_MonthlySalarySheet " +
                            " where " +
                            " IsActive='1' AND CompanyId in(" + CompanyList + ") AND YearMonth='" + ddlSelectMonth.SelectedItem.Value.ToString() + "' AND EmpCardNo Like '%" + txtEmpCardNo.Text.Trim() + "' AND IsSeperationGeneration='1' " +
                            " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                        Session["__ReportTitle__"] = "[Separation]";
                    }

                    sqlDB.fillDataTable(getSQLCMD, dt);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Sorry any record are not founded"; return;
                    }
                    rblEmployeeType.SelectedValue = dt.Rows[0]["EmptypeId"].ToString();
                    rblPaymentType.SelectedValue = dt.Rows[0]["SalaryCount"].ToString();


                }
                Session["__Language__"] = "Bangla";
                Session["__PaySlip__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=PaySlip-" + ddlSelectMonth.SelectedItem.Text + "');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }
    }
}