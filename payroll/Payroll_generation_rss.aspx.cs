using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class Payroll_generation_rss : System.Web.UI.Page
    {
        DataTable dt;
        SqlCommand cmd;
        string sqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["OPERATION_PROGRESS"] = 0;

            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();

            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 100;
            ProgressBar1.BackColor = System.Drawing.Color.Blue;
            ProgressBar1.ForeColor = Color.White;
            ProgressBar1.Height = new Unit(20);
            ProgressBar1.Width = new Unit(100);

            if (!IsPostBack)
            {
                setPrivilege();
                // classes.commonTask.loadEmpTypeInRadioButtonList(rbEmpTypeList);
                Session["__CID__"] = ddlCompanyList.SelectedValue;
                classes.Employee.LoadEmpCardNoWithNameByCompanyRShift(ddlEmpCardNo, ddlCompanyList.SelectedValue);
                //if (classes.Payroll.Office_IsGarments()) IsGarments = true;
                //else IsGarments = false;
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                classes.Employee.LoadEmpCardNoWithNameByCompanyRShift(ddlEmpCardNo, ddlCompanyList.SelectedValue);
                ViewState["___IsGerments__"] = classes.Payroll.Office_IsGarments();
                if (ViewState["___IsGerments__"].ToString().Equals("False"))
                {
                    if (ddlCompanyList.SelectedValue == "0001")
                        txtNotTiffinCardno.Text = "";
                    else
                        txtNotTiffinCardno.Text = "0069,0037";
                }
                loadExistingSalary();
            }

            lblMessage.InnerText = "";
        }
        DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {
                payroll_generation pg = new payroll_generation();
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                Session["__getUserId__"] = getUserId;
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                string[] AccessPermission = new string[0];
                //System.Web.UI.HtmlControls.HtmlTable a = tblGenerateType;
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForOnlyWriteAction(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "payroll_generation.aspx", ddlCompanyList, btnGenerate, btnBDTNoteGenerate,gvSalaryList);
                classes.commonTask.LoadShift(ddlShiftList, ViewState["__CompanyId__"].ToString());




            }
            catch { }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            
            string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();

            // getSelectedShiftId();

            string[] getDays = txtGenerateMonth.Text.Trim().Split('-');
            int DaysInMonth = DateTime.DaysInMonth(int.Parse(getDays[2]), int.Parse(getDays[1]));
            ViewState["__FromDate__"] = getDays[2] + "-" + getDays[1] + "-01" ;
            ViewState["__ToDate__"] = getDays[2]+"-"+getDays[1] + "-" + getDays[0];
            setFromDateToDate(getDays[2],getDays[1],getDays[0]);
            loadMonthSetup("1", getDays[1], getDays[2], CompanyId);
            


            generateMonthlySalarySheet(getDays[1] + "-" + getDays[2], getDays[1], getDays[2], DaysInMonth, getDays[0]);
            loadExistingSalary();
        }
        private void setFromDateToDate(string sy,string sm,string sd)
        {
            sqlCmd = "select Distinct convert(varchar(10), ToDate,120) as ToDate from Payroll_MonthlySalarySheet where IsSeperationGeneration=0  and CompanyId='" + ddlCompanyList.SelectedValue+"' and YearMonth='"+ sy + "-"+sm+"-01' and ToDate<'"+sy+"-"+sm+"-"+sd+"'";            
            sqlDB.fillDataTable(sqlCmd, dt=new DataTable());
            if (dt != null && dt.Rows.Count > 0)
            {
                ViewState["__FromDate__"] =( DateTime.Parse(dt.Rows[0]["ToDate"].ToString()).AddDays(1)).ToString("yyyy-MM-dd");
            }
        }
        protected void btnBDTNoteGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();

                // getSelectedShiftId();                
                string[] getDays = txtGenerateMonth.Text.Trim().Split('-');
                BDTNoteGeneration(getDays[1], getDays[2]);


                DataTable dt = new DataTable();
                string getMonthName = getDays[1] + "-" + getDays[2];
                sqlDB.fillDataTable("select NoteName,Amount,DptName,MonthName,DptId,CompanyId,CompanyName,SftId,SftName from v_Payroll_MonthlyNoteAmount group by NoteName,Amount,DptName,MonthName,DptId,CompanyId,CompanyName,SftId,SftName having CompanyId='" + CompanyId + "'  AND MonthName='" + getMonthName + "' order by CompanyId,SftId,DptId ", dt);
                Session["__NoteAmount__"] = dt;

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=NoteGenerate-" + getMonthName + "');", true);  //Open New Tab for Sever side code

            }
            catch { }
        }
        private void BDTNoteGeneration(string month, string year)
        {
            try
            {
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();


                deleteBDTNotes(CompanyId);
                DataTable dtBDTNotes = new DataTable();
                sqlDB.fillDataTable("select SL,Note from HRD_BDTNote where Chosen='true'", dtBDTNotes);



                DataTable dtGetSalarySheet = new DataTable();
                sqlDB.fillDataTable("select CompanyId,SftId,DptId,DptName,Format(YearMonth,'MM-yyyy') as YearMonth,sum(TotalSalary) AS TotalSalary from "
                + "v_MonthlySalarySheet where Month(YearMonth) ='" + month + "' AND Year(YearMonth)='" + year + "' AND CompanyId='" + CompanyId + "'  group by CompanyId,SftId,DptId,DptName,YearMonth order  by "
                + "DptId,SftId ", dtGetSalarySheet);

                int[] noteAmount = new int[dtBDTNotes.Rows.Count];

                double getTime = Math.Round((double.Parse(dtGetSalarySheet.Rows.Count.ToString())) / 100, 0);
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(getTime));

                for (int i = 0; i < dtGetSalarySheet.Rows.Count; i++)
                {
                    double getSalaryAmount = double.Parse(dtGetSalarySheet.Rows[i]["TotalSalary"].ToString());
                    double temp;
                    for (byte j = (byte)(dtBDTNotes.Rows.Count - 1); j >= 0; j--)
                    {
                        if (getSalaryAmount >= int.Parse(dtBDTNotes.Rows[j]["Note"].ToString()))
                        {
                            temp = getSalaryAmount % int.Parse(dtBDTNotes.Rows[j]["Note"].ToString());
                            getSalaryAmount -= temp;
                            noteAmount[j] += (int)getSalaryAmount / int.Parse(dtBDTNotes.Rows[j]["Note"].ToString());
                            getSalaryAmount = temp;
                            if ((int)getSalaryAmount == 0 || j == 0) break;
                        }

                        if (j == 0) break;
                    }


                    if (i == 1088)
                    {

                    }

                    //string a = dtGetSalarySheet.Rows[i]["DptId"].ToString();
                    //string ab = dtGetSalarySheet.Rows[i + 1]["DptId"].ToString();

                    //string aa = dtGetSalarySheet.Rows[i]["LnId"].ToString();

                    //string aaa = dtGetSalarySheet.Rows[i + 1]["LnId"].ToString();

                    //if (a == "24" && aa == "43")
                    //{ 

                    //}

                    saveGenerateNotes(noteAmount, dtBDTNotes, dtGetSalarySheet, i);
                    noteAmount = new int[dtBDTNotes.Rows.Count];
                    /*
                    if (i == dtGetSalarySheet.Rows.Count - 1)    // for last value
                    {
                        saveGenerateNotes(noteAmount, dtBDTNotes, dtGetSalarySheet, i);
                        noteAmount = new int[dtBDTNotes.Rows.Count];
                    }
                    else if (dtGetSalarySheet.Rows[i]["DptId"].ToString().Equals(dtGetSalarySheet.Rows[i + 1]["DptId"].ToString()))
                    {
                       
                        saveGenerateNotes(noteAmount, dtBDTNotes, dtGetSalarySheet, i);
                        noteAmount = new int[dtBDTNotes.Rows.Count];

                    }
                    else if (dtGetSalarySheet.Rows[i]["DptId"].ToString() != dtGetSalarySheet.Rows[i + 1]["DptId"].ToString())
                    {
                        saveGenerateNotes(noteAmount, dtBDTNotes, dtGetSalarySheet, i);
                        noteAmount = new int[dtBDTNotes.Rows.Count];
                    }
                     */
                }

            }
            catch { }
        }
        private void saveGenerateNotes(int[] noteAmount, DataTable dtBDTNotes, DataTable dtGetSalarySheet, int i)
        {
            try
            {
                for (byte b = (byte)(dtBDTNotes.Rows.Count - 1); b >= 0; b--)
                {
                    try
                    {
                        string[] getColumns = { "CompanyId", "SftId", "DptId", "NoteName", "Amount", "MonthName" };
                        string[] getValues = { dtGetSalarySheet.Rows[i]["CompanyId"].ToString(), dtGetSalarySheet.Rows[i]["SftId"].ToString(), dtGetSalarySheet.Rows[i]["DptId"].ToString(), dtBDTNotes.Rows[b]["Note"].ToString(), noteAmount[b].ToString(), dtGetSalarySheet.Rows[i]["YearMonth"].ToString() };
                        SQLOperation.forSaveValue("Payroll_MonthlyNoteAmount", getColumns, getValues, sqlDB.connection);
                        if (b == 0)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show(ex.Message);
                    }
                }
            }
            catch { }
        }
        private void getSelectedShiftId()
        {
            try
            {
                string getRequiredSftId = "";
                if (!ddlShiftList.SelectedItem.Text.Trim().Equals("All")) getRequiredSftId = ddlShiftList.SelectedItem.Value.ToString() + ",";
                else
                    for (byte r = 0; r < ddlShiftList.Items.Count; r++)
                    {
                        getRequiredSftId += ddlShiftList.Items[r].Value.ToString() + ",";

                    }
                ViewState["__getRequiredSftId__"] = getRequiredSftId.Remove(getRequiredSftId.LastIndexOf(','));
            }
            catch { }
        }
        private void deleteBDTNotes(string CompanyId)
        {
            try
            {
                string[] getMonthName = txtGenerateMonth.Text.Split('-');
                SQLOperation.forDeleteRecordByIdentifier("Payroll_MonthlyNoteAmount", "MonthName", getMonthName[1] + "-" + getMonthName[2], sqlDB.connection);
                SqlCommand cmd = new SqlCommand("delete from Payroll_MonthlyNoteAmount where MonthName ='" + getMonthName[1] + "-" + getMonthName[2] + "' AND CompanyId='" + CompanyId + "' ");
            }
            catch { }
        }

        DataTable dtRunningEmp;
        DataTable dtCertainEmp;
        DataTable dtLeaveInfo;
        DataTable dtLeaveInfoLWP;
        DataTable dtLeaveInfoForAttBonus;
        DataTable dtPresent;
        DataTable dtPresentForAttBonus;
        DataTable dtAbsent;
        DataTable dtAbsentForAttBonus;
        DataTable dtLate;
        DataTable dtLateForAttBouns;
        DataTable dtAdvanceInfo;
        DataTable dtCutAdvance;
        DataTable dtLoanInfo;
        DataTable dtCutLoan;
        DataTable dtStampDeduct;
        DataTable dtOverTime_stayTime;
        DataTable dtTiffin_Staff_WorkerTaka;
        DataTable dtShortleave;
        DataTable dtOtherspay;
        DataTable dtOthersDeduction;
        // DataTable dtPFDeduction;
        DataTable dtTaxDeduction;
        DataTable dtWeekendForAttBonus;
        DataTable dtHolidayForAttBonus;
       

        bool isgarments = false;
        private void generateMonthlySalarySheet(string getMonthYear, string month, string year, int DaysInMonth, string selectDays)
        {
            try
            {
                payroll_generation pg = new payroll_generation();

                double getTotalOT;
                string getJoingingDate;

                string MonthName = year + "-" + month;
                // for get company id
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                // get stamp card price
                sqlDB.fillDataTable("select StampDeduct from HRD_AllownceSetting where AllownceId =(select max(AllownceId) from HRD_AllownceSetting)", dtStampDeduct = new DataTable());
                sqlDB.fillDataTable("select WorkerTiffinTaka,StaffTiffinTaka,NightBillTk from HRD_OthersSetting where CompanyId='" + CompanyId + "'", dtTiffin_Staff_WorkerTaka = new DataTable());








                //check overTime is active ?


                if (rbGenaratingType.SelectedValue.ToString().Equals("0"))   // generating type for all employee
                {
                    string ExceptedEmpCardNo = "";
                    if (txtExceptedEmpCardNo.Text.Trim() != "")
                    {
                        ExceptedEmpCardNo = " and EmpID not in(select EmpId from Personnel_EmployeeInfo where SUBSTRING(EmpCardNo,8,6) in(" + txtExceptedEmpCardNo.Text.Trim() + ") and CompanyId='" + CompanyId + "')";
                    }
                    // for delete existing salary sheet of this month
                    salarySheetClearByMonthYear(selectDays,month, year, CompanyId);

                    // get all regular employee at this time
                    sqlCmd = "select distinct EmpId,EmpCardNo,EmpName, max(SN) as SN,EmpType,EmpTypeId,EmpStatus,ActiveSalary,IsActive,CompanyId,SftId,OverTime  from v_Personnel_EmpCurrentStatus group by EmpId,EmpCardNo,EmpName,EmpTypeId,EmpType,EmpStatus,ActiveSalary,IsActive,CompanyId,SftId,OverTime,EmpJoiningDate having EmpStatus in('1','8') AND ActiveSalary='true' AND IsActive='1' " + ExceptedEmpCardNo + " AND CompanyId='" + CompanyId + "' and EmpJoiningDate<='" + year + "-" + month + "-" + selectDays + "' order by SN";

                    sqlDB.fillDataTable(sqlCmd, dtRunningEmp = new DataTable());
                }

                else    // generating type for single employee
                {
                    if (txtEmpCardNo.Text.Trim().Length >= 4)  // valid card justification
                    {
                        salarySheetClearByMonthYear(selectDays, month, year, CompanyId, txtEmpCardNo.Text);
                        // get max SN of employee,whose active salary status is true 
                        sqlDB.fillDataTable("select EmpCardNo,EmpName,MAX(SN) as SN,EmpType,EmpTypeId,EmpStatus,ActiveSalary,IsActive,CompanyId,SftId,OverTime  from v_Personnel_EmpCurrentStatus group by EmpCardNo,EmpName,EmpType,EmpTypeId,EmpStatus,ActiveSalary,IsActive,CompanyId,SftId,OverTime,EmpJoiningDate having EmpCardNo Like'%" + txtEmpCardNo.Text + "' AND  EmpStatus in ('1','8') AND ActiveSalary='true' AND IsActive='1' AND CompanyId='" + CompanyId + "'and EmpJoiningDate<='" + year + "-" + month + "-" + selectDays + "' ", dtRunningEmp = new DataTable());

                    }
                    else
                    {
                        lblMessage.InnerText = "error->Please type valid card no of an employee";
                        txtEmpCardNo.Focus();
                    }
                }               
                isgarments = bool.Parse(ViewState["___IsGerments__"].ToString());
                for (int i = 0; i < dtRunningEmp.Rows.Count; i++)
                {
                    ViewState["__EmpCardNo__"] = dtRunningEmp.Rows[i]["EmpCardNo"].ToString();
                    if (ViewState["__EmpCardNo__"].ToString().Equals("00001455"))
                    {

                    }
                    // get essential information of a certain employee 
                    sqlDB.fillDataTable("select EmpId,EmpCardNo,BasicSalary,MedicalAllownce,FoodAllownce,ConvenceAllownce,HouseRent,TechnicalAllownce,OthersAllownce,EmpPresentSalary,case when EmpTypeId=1 then 200 else 400 end as AttendanceBonus,LunchCount,LunchAllownce,DptId,GrdName,DsgId,sftId,DormitoryRent,PFAmount,IncomeTax,IsHalfOT,isNull(IsDelivery,0) IsDelivery,isNull(TiffinStatus,1) TiffinStatus,isNull(NightBillStatus,1) NightBillStatus,isNull(IsSingleRateOT,0) as IsSingleRateOT,EmpDutyType,PfMember,convert(varchar(10), PfDate,120) as PfDate from v_Personnel_EmpCurrentStatus where SN=" + dtRunningEmp.Rows[i]["SN"].ToString() + "", dtCertainEmp = new DataTable());
                    // get Proximity number of a certain employee
                    sqlDB.fillDataTable("select convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate from Personnel_EmployeeInfo where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "'", dt = new DataTable());
                    ViewState["__getJoingingDate__"] = dt.Rows[0]["EmpJoiningDate"].ToString();

                    // get leave information of a certain employee
                    sqlDB.fillDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId,StateStatus from v_tblAttendanceRecord where ATTStatus='lv' and StateStatus <>'Leave Without Pay (LWP)' AND MonthName ='" + year + '-' + month + "' AND EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' And AttDate >='" + ViewState["__FromDate__"].ToString() + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "'", dtLeaveInfo = new DataTable());
                    sqlDB.fillDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId,StateStatus from v_tblAttendanceRecord where ATTStatus='lv' and StateStatus ='Leave Without Pay (LWP)' AND MonthName ='" + year + '-' + month + "' AND EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' And AttDate >='" + ViewState["__FromDate__"].ToString() + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "'", dtLeaveInfoLWP = new DataTable()); // Leave without Pay
                    sqlDB.fillDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId,StateStatus from v_tblAttendanceRecord where ATTStatus='lv' AND MonthName ='" + year + '-' + month + "' AND EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' And AttDate >='" + year + '-' + month + '-' + "01" + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "'", dtLeaveInfoForAttBonus = new DataTable());
                    getAllLeaveInformation();

                    // get present information of a certain employee                    
                    sqlDB.fillDataTable("select distinct SftId, EmpId,Convert(varchar(11),ATTDate,111) as ATTDate,InHour,InMin,InSec,OutHour,OutMin,OutSec,ATTStatus from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND ATTStatus In ('P','L') AND MonthName='" + MonthName + "' AND AttDate >='" + ViewState["__FromDate__"].ToString() + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' AND PaybleDays='1' ", dtPresent = new DataTable());

                    sqlDB.fillDataTable("select distinct SftId, EmpId,Convert(varchar(11),ATTDate,111) as ATTDate,InHour,InMin,InSec,OutHour,OutMin,OutSec,ATTStatus from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND ATTStatus In ('P','L') AND MonthName='" + MonthName + "' AND AttDate >='" + year + '-' + month + '-' + "01" + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' AND PaybleDays='1' ", dtPresentForAttBonus = new DataTable());

                    // get late information of a certain employee                    
                    sqlDB.fillDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate, EmpId from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND ATTStatus='L' AND MonthName='" + MonthName + "' AND AttDate >='" + ViewState["__FromDate__"].ToString() + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' AND PaybleDays='1' ", dtLate = new DataTable());

                    // get late information of a certain employee                    
                    sqlDB.fillDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate, EmpId from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND ATTStatus='L' AND MonthName='" + MonthName + "' AND AttDate >='" + year + "-" + month +"-01' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' AND PaybleDays='1' ", dtLateForAttBouns = new DataTable());



                    // get Absent
                    sqlDB.fillDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND (ATTStatus='A' or StateStatus='Leave Without Pay (LWP)') AND MonthName='" + MonthName + "' AND AttDate >='" + year + '-' + month + '-' + "01" + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' Union select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND ATTStatus In ('P','L') AND MonthName='" + MonthName + "' AND AttDate >='" + year + '-' + month + '-' + "01" + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' AND PaybleDays='0' ", dtAbsentForAttBonus = new DataTable());

                    sqlDB.fillDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND (ATTStatus='A' or StateStatus='Leave Without Pay (LWP)') AND MonthName='" + MonthName + "' AND AttDate >='" + ViewState["__FromDate__"].ToString() + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' Union select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND ATTStatus In ('P','L') AND MonthName='" + MonthName + "' AND AttDate >='" + year + '-' + month + '-' + "01" + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "' AND PaybleDays='0' ", dtAbsent = new DataTable());


                    ViewState["__hasAttBonus__"] = "True";
                    try {
                        if (i == 60)
                        {

                        }
                    if (dtLateForAttBouns.Rows.Count > 0 || dtLeaveInfoForAttBonus.Rows.Count >0 || dtAbsentForAttBonus.Rows.Count >0)
                    {
                        ViewState["__hasAttBonus__"] = "False";
                    }
                    else
                    {                        
                        for (int p = 0; p < dtPresentForAttBonus.Rows.Count; p++)
                        {
                            DataTable dtTimeTable=new DataTable();
                            DateTime Date =DateTime.Parse(dtPresentForAttBonus.Rows[p]["ATTDate"].ToString());
                               
                            string _LoginTime = dtPresentForAttBonus.Rows[p]["InHour"].ToString()+":"+ dtPresentForAttBonus.Rows[p]["InMin"].ToString() + ":" + dtPresentForAttBonus.Rows[p]["InSec"].ToString();
                                if (dtCertainEmp.Rows[0]["EmpDutyType"].ToString().Equals("Regular"))
                                {
                                    sqlDB.fillDataTable("select SftStartTime from HRD_SpecialTimetable where StartDate<= '" + Date.ToString("yyyy-MM-dd") + "' and  EndDate>= '" + Date.ToString("yyyy-MM-dd") + "'", dtTimeTable = new DataTable());
                                }                               
                                if (dtTimeTable == null || dtTimeTable.Rows.Count == 0)
                                {
                                    sqlDB.fillDataTable("select SftStartTime from HRD_Shift where SftId=" + dtPresentForAttBonus.Rows[p]["SftId"].ToString(), dtTimeTable = new DataTable());
                                }
                                if (dtTimeTable != null && dtTimeTable.Rows.Count > 0)
                                {
                                DateTime SftStartTime = DateTime.Parse(Date.ToString("yyyy-MM-dd") + " " + dtTimeTable.Rows[0]["SftStartTime"].ToString());
                                DateTime LoginTime = DateTime.Parse(Date.ToString("yyyy-MM-dd") + " " + _LoginTime);
                                if (SftStartTime < LoginTime)
                                {
                                    ViewState["__hasAttBonus__"] = "False";
                                    break;
                                }
                            }

                        }
                    }
                    }
                    catch (Exception ex) { }
                    //get short leave

                    sqlDB.fillDataTable("select distinct convert(varchar(11),LvDate,111) as LvDate,EmpId from Leave_ShortLeave where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND LvDate >='" + ViewState["__FromDate__"].ToString() + "' AND LvDate <= '" + year + '-' + month + '-' + selectDays + "' ", dtShortleave = new DataTable());



                    //get Other's Pay
                    sqlDB.fillDataTable("select  ISNULL(Sum(OtherPay),0) OtherPay from Payroll_OthersPay where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND IsActive='1' ", dtOtherspay = new DataTable());

                    //get Other's Deduction

                    sqlDB.fillDataTable("select ISNULL(Sum(PAmount),0) PAmount from Payroll_Punishment where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND MonthName ='" + month + '-' + year + "' ", dtOthersDeduction = new DataTable());

                    //get PF Deduction 
                    if (ckbPF.Checked)
                    {
                        string _pfDate = dtCertainEmp.Rows[0]["PfDate"].ToString();
                        if (_pfDate.Trim().Length > 1)
                        {
                            string PFMember = dtCertainEmp.Rows[0]["PfMember"].ToString();
                            DateTime PfDate = DateTime.Parse(dtCertainEmp.Rows[0]["PfDate"].ToString());
                            if (PFMember.Equals("True") && PfDate <= DateTime.Parse(year + "-" + month + "-" + selectDays))
                                ViewState["__PFAmount__"] = dtCertainEmp.Rows[0]["PFAmount"].ToString();
                            else
                                ViewState["__PFAmount__"] = "0";

                        }
                        else
                            ViewState["__PFAmount__"] = "0";
                    }
                    else
                        ViewState["__PFAmount__"] = "0";
                    //   sqlDB.fillDataTable("select ISNULL(Sum(EmpContributionAmount),0) PFAmount from PF_CalculationDetails where  EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND CONVERT(VARCHAR(7), convert(date,'01-'+MonthName),120) ='" + year + '-' + month + "' ", dtPFDeduction = new DataTable());

                    //------------get Tax Deduction-------------
                    sqlDB.fillDataTable("select isnull(IncomeTax,0) as TaxAmount from Personnel_EmpCurrentStatus where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and IsActive=1", dtTaxDeduction = new DataTable());
                    ViewState["__TaxAmount__"] = (dtTaxDeduction.Rows.Count > 0) ? dtTaxDeduction.Rows[0]["TaxAmount"].ToString() : "0";

                    //   ViewState["__TaxAmount__"] = 0;
                    //------------End Tax Deduction-------------

                    //----- Over Time---
                    if (dtRunningEmp.Rows[i]["EmpTypeId"].ToString() == "1")
                    {
                        OverTimeHourlySalary =Payroll.getHourlyAmount(DaysInMonth, double.Parse(dtCertainEmp.Rows[0]["BasicSalary"].ToString()), dtCertainEmp.Rows[0]["IsHalfOT"].ToString(), dtCertainEmp.Rows[0]["IsSingleRateOT"].ToString());
                        // Call Over time Callculation for count OT taka

                        dtOverTime_stayTime = new DataTable();
                        string sql = "Select  isnull(CAST(SUM(DATEDIFF(second, 0, OverTime)) / 3600 AS varchar(12)) + ':' + RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, OverTime)) / 60 % 60 AS varchar(2)), 2) + ':' +RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, OverTime)) % 60 AS varchar(2)), 2),'00:00:00') AS OverTime,isnull(CAST(SUM(DATEDIFF(second, 0, OtherOverTime)) / 3600 AS varchar(12)) + ':' +RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, OtherOverTime)) / 60 % 60 AS varchar(2)), 2) + ':' + RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, OtherOverTime)) % 60 AS varchar(2)), 2),'00:00:00') AS OtherOverTime,isnull(CAST(SUM(DATEDIFF(second, 0, TotalOverTime)) / 3600 AS varchar(12)) + ':' + RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, TotalOverTime)) / 60 % 60 AS varchar(2)), 2) + ':' +RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, TotalOverTime)) % 60 AS varchar(2)), 2),'00:00:00') AS TotalOverTime from v_tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND AttDate >='" + ViewState["__FromDate__"].ToString() + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "'  and IsOverTime='1' and IsActive='1'";
                        sqlDB.fillDataTable(sql, dtOverTime_stayTime);

                        string time = dtOverTime_stayTime.Rows[0]["TotalOverTime"].ToString();
                        string[] spltTime = time.Split(':');
                        string time2 = "00:00:00";
                        string[] spltTime2 = time2.Split(':');
                        double hours = double.Parse(spltTime[0]) + double.Parse(spltTime2[0]);
                        double min = double.Parse(spltTime[1]) + double.Parse(spltTime2[1]);
                        double secods = double.Parse(spltTime[2]) + double.Parse(spltTime2[2]);


                        string totalOverTime = time;
                        double secOttk = (OverTimeHourlySalary / 3600) * secods;
                        double minOttk = (OverTimeHourlySalary / 60) * min;
                        double hourlyot = OverTimeHourlySalary * hours;

                        ViewState["__getTotalOvertimeAmt__"] = minOttk + hourlyot + secOttk;
                        ViewState["__getTotalOverTime__"] = totalOverTime;
                        ViewState["__OT_Amt_Hour_ForBuyer_AsRegular__"] = "0";
                        ViewState["__Extra_OT_Amt_OfEmp__"] = "0";
                        ViewState["__getOverTime__"] = dtOverTime_stayTime.Rows[0]["OverTime"].ToString();
                        ViewState["__getOtherOverTime__"] = dtOverTime_stayTime.Rows[0]["OtherOverTime"].ToString();
                    }
                    else
                    {
                        ViewState["__getTotalOvertimeAmt__"] = "0";
                        ViewState["__getTotalOverTime__"] = "00:00:00";
                        ViewState["__OT_Amt_Hour_ForBuyer_AsRegular__"] = "0";
                        ViewState["__Extra_OT_Amt_OfEmp__"] = "0";
                        ViewState["__getOverTime__"] = "00:00:00";
                        ViewState["__getOtherOverTime__"] = "00:00:00";
                    }
                     

                    //-----End Over Time-----

                    //if (isgarments == true) // this condition use to avoid this block . because this is not necessary for RSS
                    //{
                    //-----------Tiffin & Holiday Allow.-----------------
                    ViewState["__Tiffindays__"] = "0";
                    ViewState["__TiffinBillAmount__"] = "0";
                    ViewState["__Holidays__"] = "0";
                    ViewState["__HolidayBillAmount__"] = "0";
                    ViewState["__HolidayTaka__"] = "0";
                    string tiffincount = dtRunningEmp.Rows[i]["EmpTypeId"].ToString() == "1" ? dtTiffin_Staff_WorkerTaka.Rows[0]["WorkerTiffinTaka"].ToString() : dtTiffin_Staff_WorkerTaka.Rows[0]["StaffTiffinTaka"].ToString();
                    ViewState["__TiffinTaka__"] = tiffincount;
                    DataTable dtTiffin_Holidays = new DataTable();                    
                    sqlDB.fillDataTable("select ISNULL(Sum(case when IsOverTime='1' then  TiffinCount else 0 end ),0) as TiffinCount,ISNULL(Sum(HolidayCount),0)  as HolidayCount from v_tblAttendanceRecord where   IsActive='1' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND AttDate >='" + ViewState["__FromDate__"].ToString() + "' AND AttDate <= '" + year + '-' + month + '-' + selectDays + "'", dtTiffin_Holidays);// If anyone is not allowed for Overtime then he/she is not allowed for tiffin

                    if (dtTiffin_Holidays.Rows.Count > 0)
                    {
                        if (dtRunningEmp.Rows[i]["EmpTypeId"].ToString() == "1") // Tiffin allowance only for Worker
                        {
                            if (ckbTiffinBill.Checked)
                            {
                                try
                                {

                                    string[] NotTiffinCardno = txtNotTiffinCardno.Text.Split(',');
                                    if (NotTiffinCardno.Contains(dtCertainEmp.Rows[0]["EmpCardNo"].ToString().Substring(9, dtCertainEmp.Rows[0]["EmpCardNo"].ToString().Length - 9)))
                                        ViewState["__Tiffindays__"] = "0";
                                    else
                                        ViewState["__Tiffindays__"] = dtTiffin_Holidays.Rows[0]["TiffinCount"].ToString();
                                    if (!dtCertainEmp.Rows[0]["TiffinStatus"].ToString().Equals("True"))
                                        ViewState["__Tiffindays__"] = "0";
                                }
                                catch { }
                                ViewState["__TiffinBillAmount__"] = (float.Parse(ViewState["__Tiffindays__"].ToString()) * float.Parse(tiffincount)).ToString();

                              

                            }
                            if (ckbHolidayAllow.Checked && dtCertainEmp.Rows[0]["IsDelivery"].ToString().Equals("True"))// if delivery department
                            {
                                ViewState["__HolidayTaka__"] = (float.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / 30).ToString();
                                ViewState["__Holidays__"] = dtTiffin_Holidays.Rows[0]["HolidayCount"].ToString();
                                ViewState["__HolidayBillAmount__"] = ((float.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / 30) * (float.Parse(dtTiffin_Holidays.Rows[0]["HolidayCount"].ToString()))).ToString();
                            }
                        }
                        else // Holiday's  allowance only for Staff
                        {
                            if (ckbHolidayAllow.Checked)
                            {
                                ViewState["__HolidayTaka__"] = (float.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / 30).ToString();
                                ViewState["__Holidays__"] = dtTiffin_Holidays.Rows[0]["HolidayCount"].ToString();
                                ViewState["__HolidayBillAmount__"] = ((float.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / 30) * (float.Parse(dtTiffin_Holidays.Rows[0]["HolidayCount"].ToString()))).ToString();
                            }
                            

                        }
                    }

                    //-----------Tiffin & Holiday Allow.End ----------------
                    //}
                    //else 
                    //{
                    //    ViewState["__Tiffindays__"] = "0";
                    //    ViewState["__TiffinBillAmount__"] = "0";
                    //    ViewState["__Holidays__"] = "0";
                    //    ViewState["__HolidayBillAmount__"] = "0";
                    //    ViewState["__TiffinTaka__"] = "0";
                    //    ViewState["__HolidayTaka__"] = "0";
                    //}
                    

                    // get loan information of a certain employee 
                    sqlDB.fillDataTable("select SL,LoanID,Month,Amount from Payroll_LoanMonthlySetup where Month='" + year + "-" + month + "- 01' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "'", dtCutAdvance = new DataTable());
                    dtCutLoan = new DataTable();

                    //if (rbEmpTypeList.SelectedItem.ToString().ToLower().Equals("staff"))checkLunchCost();
                    //else getLunchCost = 0;

                    //-------- get Night Allow.------------------------
                    ViewState["__NightBillDays__"] = "0";
                    ViewState["__NightbilAmount__"] = "0";
                    if (dtRunningEmp.Rows[i]["EmpTypeId"].ToString() == "1" && ckbNightBill.Checked && dtCertainEmp.Rows[0]["TiffinStatus"].ToString().Equals("True")) // Night bill allowance only for Worker
                    {
                        DataTable dtNightAllow;
                        sqlCmd = @"select isnull( sum(isnull(NightAllowCount,0)),0) NightAllowCount  from v_tblAttendanceRecord where  IsOverTime='1' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTDate>='" + ViewState["__FromDate__"].ToString() + "' and ATTDate<='" + year + "-" + month + "-" + selectDays + "'  ";
                       // sqlDB.fillDataTable("select isnull( sum(isnull(NightAllowCount,0)),0) NightAllowCount  from tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTDate>='" + year + "-" + month + "-01' and ATTDate<='" + year + "-" + month + "-" + selectDays + "'  ", dtNightAllow = new DataTable());
                        sqlDB.fillDataTable(sqlCmd, dtNightAllow = new DataTable());

                        if (dtNightAllow.Rows.Count > 0)
                        {
                            ViewState["__NightBillDays__"] = dtNightAllow.Rows[0]["NightAllowCount"].ToString();
                            //  ViewState["__NightbilAmount__"] = ((int.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / 30) * int.Parse(dtNightAllow.Rows[0]["NightAllowCount"].ToString())).ToString();
                            ViewState["__NightbilAmount__"] = (int.Parse(dtTiffin_Staff_WorkerTaka.Rows[0]["NightBillTk"].ToString()) * int.Parse(dtNightAllow.Rows[0]["NightAllowCount"].ToString())).ToString();
                        }
                    }

                    //----------End Night Allow.------------------------
                    string a = Session["__getUserId__"].ToString();

                    saveMonthlyPayrollSheet(month, year, DaysInMonth, dtRunningEmp.Rows[i]["EmpName"].ToString(), i, selectDays, int.Parse(Session["__getUserId__"].ToString()), CompanyId, pg, txtGenerateMonth.Text);

                    //if (!isActiveLoop) break;
                }

                lblMessage.InnerText = "success->Successfully payroll generated of " + dtRunningEmp.Rows.Count;
                rbGenaratingType.SelectedValue = "0";
                imgLoading.Visible = false;
                Response.Redirect("/payroll/payroll_generation_rss.aspx");

            }
            catch (Exception ex)
            {
                string a = ViewState["__EmpCardNo__"].ToString();

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ProcessingEror(" + ex.Message + ");", true);
            }
        }
       
        private static string gettotalovertime(string time)
        {
            char[] delimiters = new char[] { ':', ' ' };
            string[] spltTime = time.Split(delimiters);

            string[] splthour = spltTime[0].Split('.');
            if (splthour.Count() == 1)
            {
                int hour = int.Parse(spltTime[0]);
                int minute = int.Parse(spltTime[1]);
                int seconds = int.Parse(spltTime[2].Substring(0, 2));
                return hour + ":" + minute + ":" + seconds;
            }
            else
            {
                int hour = int.Parse(splthour[0]);
                int minute = int.Parse(splthour[1]);
                int seconds = int.Parse(spltTime[1]);
                return hour + ":" + minute + ":" + seconds;
            }

        }
        private void saveMonthlyPayrollSheet(string getMonth, string getYear, int DaysInMonth, string empName, int i, string selectedDay, int userId, string CompanyId, payroll_generation pg, string SDate)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("insert into Payroll_MonthlySalarySheet(CompanyId,SftId,EmpId,EmpCardNo,YearMonth,DaysInMonth,Activeday,WeekendHoliday,PayableDays," +
                      "CasualLeave,SickLeave,AnnualLeave,OfficialLeave,OthersLeave,FestivalHoliday,AbsentDay,PresentDay,EmpPresentSalary,BasicSalary,HouseRent,MedicalAllownce,ConvenceAllownce,FoodAllownce,TechnicalAllowance," +
                      "OthersAllownce,LunchAllowance,AdvanceDeduction,LoanDeduction,AbsentDeduction,AttendanceBonus,Payable,TotalOTHour,OTRate,TotalOTAmount,NetPayable,Stampdeduct,TotalSalary,DptId," +
                      "DsgId,GrdName,EmpTypeId,EmpStatus,UserId,IsSeperationGeneration,GenerateDate,OTHourForBuyer,OTAmountForBuyer,ExtraOTHour,ExtraOTAmount,NetPayableWithAllOTAmt,LateDays,LateFine,TiffinDays,TiffinTaka,TiffinBillAmount,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount,DormitoryRent,ProvidentFund,TotalOverTime,TotalOtherOverTime,OthersPay,OthersDeduction,ShortLeave,ProfitTax,NightbilAmount,NightBillDays,EmpNetGross,FromDate,ToDate,LWP) " +

                      "values(@CompanyId,@SftId,@EmpId,@EmpCardNo,@YearMonth,@DaysInMonth,@Activeday,@WeekendHoliday,@PayableDays,@CasualLeave," +
                      "@SickLeave,@AnnualLeave,@OfficialLeave,@OthersLeave,@FestivalHoliday,@AbsentDay,@PresentDay,@EmpPresentSalary,@BasicSalary,@HouseRent,@MedicalAllownce,@ConvenceAllownce,@FoodAllownce," +
                      "@TechnicalAllowance,@OthersAllownce,@LunchAllowance,@AdvanceDeduction,@LoanDeduction,@AbsentDeduction,@AttendanceBonus,@Payable,@TotalOTHour,@OTRate,@TotalOTAmount,@NetPayable,@Stampdeduct,@TotalSalary,@DptId," +
                      "@DsgId,@GrdName,@EmpTypeId,@EmpStatus,@UserId,@IsSeperationGeneration,@GenerateDate,@OTHourForBuyer,@OTAmountForBuyer,@ExtraOTHour,@ExtraOTAmount,@NetPayableWithAllOTAmt,@LateDays,@LateFine,@TiffinDays,@TiffinTaka,@TiffinBillAmount,@HolidayWorkingDays,@HolidayTaka,@HoliDayBillAmount,@DormitoryRent,@ProvidentFund,@TotalOverTime,@TotalOtherOverTime,@OthersPay,@OthersDeduction,@ShortLeave,@ProfitTax,@NightbilAmount,@NightBillDays,@EmpNetGross,@FromDate,@ToDate,@LWP)", sqlDB.connection);


                cmd.Parameters.AddWithValue("@CompanyId", dtRunningEmp.Rows[i]["CompanyId"].ToString());
                cmd.Parameters.AddWithValue("@SftId", dtRunningEmp.Rows[i]["SftId"].ToString());
                cmd.Parameters.AddWithValue("@EmpId", dtCertainEmp.Rows[0]["EmpId"].ToString());
                cmd.Parameters.AddWithValue("@EmpCardNo", dtCertainEmp.Rows[0]["EmpCardNo"].ToString());

                string getYearMonth = getYear + "-" + getMonth + "-01";

                cmd.Parameters.AddWithValue("@YearMonth", getYearMonth);
                if (joiningMonthIsEqual(getMonth, getYear, CompanyId, SDate, pg, DaysInMonth) == false)
                {

                    PayableDaysCalculation(getYear, getMonth, selectedDay, CompanyId, pg, DaysInMonth);
                    checkForAttendanceBonus(getMonth, getYear, dtCertainEmp.Rows[0]["EmpId"].ToString());
                    getNetPayableCalculation(DaysInMonth, pg);   // this function call to get net payable  amount


                }
                cmd.Parameters.AddWithValue("@DaysInMonth", dtGetMonthSetup.Rows[0]["TotalDays"].ToString());
                cmd.Parameters.AddWithValue("@Activeday", ViewState["__WorkingDays__"].ToString());

                cmd.Parameters.AddWithValue("@WeekendHoliday", int.Parse(ViewState["__WeekendCount__"].ToString()) - int.Parse(ViewState["__WeekendAsLeave__"].ToString()));
                cmd.Parameters.AddWithValue("@PayableDays", ViewState["__PayableDays__"].ToString());

                cmd.Parameters.AddWithValue("@CasualLeave", ViewState["__cl__"].ToString());
                cmd.Parameters.AddWithValue("@SickLeave", ViewState["__sl__"].ToString());
                cmd.Parameters.AddWithValue("@AnnualLeave", ViewState["__al__"].ToString());
                cmd.Parameters.AddWithValue("@OfficialLeave", ViewState["__ofl__"].ToString());
                cmd.Parameters.AddWithValue("@OthersLeave", ViewState["__othl__"].ToString());

                cmd.Parameters.AddWithValue("@FestivalHoliday", ViewState["__HolidayCount__"].ToString());
                cmd.Parameters.AddWithValue("@AbsentDay", ViewState["__TotalAbsentDays__"].ToString());
                cmd.Parameters.AddWithValue("@PresentDay", dtPresent.Rows.Count.ToString());

                cmd.Parameters.AddWithValue("@EmpPresentSalary", dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString());
                cmd.Parameters.AddWithValue("@BasicSalary", dtCertainEmp.Rows[0]["BasicSalary"].ToString());
                cmd.Parameters.AddWithValue("@HouseRent", dtCertainEmp.Rows[0]["HouseRent"].ToString());
                cmd.Parameters.AddWithValue("@MedicalAllownce", dtCertainEmp.Rows[0]["MedicalAllownce"].ToString());
                cmd.Parameters.AddWithValue("@ConvenceAllownce", dtCertainEmp.Rows[0]["ConvenceAllownce"].ToString());
                cmd.Parameters.AddWithValue("@FoodAllownce", dtCertainEmp.Rows[0]["FoodAllownce"].ToString());
                cmd.Parameters.AddWithValue("@TechnicalAllowance", dtCertainEmp.Rows[0]["TechnicalAllownce"].ToString());
                cmd.Parameters.AddWithValue("@OthersAllownce", dtCertainEmp.Rows[0]["OthersAllownce"].ToString());
                cmd.Parameters.AddWithValue("@LunchAllowance", 0);

                cmd.Parameters.AddWithValue("@AdvanceDeduction", getAdvanceInstallment);
                cmd.Parameters.AddWithValue("@LoanDeduction", getLoanInstallment);

                // cmd.Parameters.AddWithValue("@AbsentDeduction", ViewState["__absentFine__"].ToString());

                cmd.Parameters.AddWithValue("@AbsentDeduction", ViewState["__AbsentDeduction__"].ToString());
                cmd.Parameters.AddWithValue("@AttendanceBonus", getAttendanceBonus);

                cmd.Parameters.AddWithValue("@Payable", getPayable);
                cmd.Parameters.AddWithValue("@TotalOTHour", ViewState["__getTotalOverTime__"].ToString());

                cmd.Parameters.AddWithValue("@OTRate", OverTimeHourlySalary);
                cmd.Parameters.AddWithValue("@TotalOTAmount", ViewState["__getTotalOvertimeAmt__"].ToString());
                cmd.Parameters.AddWithValue("@NetPayable", getNetPayable);
                cmd.Parameters.AddWithValue("@Stampdeduct", Math.Round(getStampDeduction, 0));
                cmd.Parameters.AddWithValue("@TotalSalary", getTotalSalary);


                cmd.Parameters.AddWithValue("@DptId", dtCertainEmp.Rows[0]["DptId"].ToString());
                cmd.Parameters.AddWithValue("@DsgId", dtCertainEmp.Rows[0]["DsgId"].ToString());
                cmd.Parameters.AddWithValue("@GrdName", dtCertainEmp.Rows[0]["GrdName"].ToString());
                cmd.Parameters.AddWithValue("@EmpTypeId", dtRunningEmp.Rows[i]["EmpTypeId"].ToString());
                cmd.Parameters.AddWithValue("@EmpStatus", '1');

                cmd.Parameters.AddWithValue("@UserId", userId.ToString());
                cmd.Parameters.AddWithValue("@IsSeperationGeneration", "0");
                cmd.Parameters.AddWithValue("@GenerateDate", convertDateTime.getCertainCulture(DateTime.Now.ToString("dd-MM-yyyy"))).ToString();
                cmd.Parameters.AddWithValue("@OTHourForBuyer", 0);
                cmd.Parameters.AddWithValue("@OTAmountForBuyer", 0);
                cmd.Parameters.AddWithValue("@ExtraOTHour", 0);
                cmd.Parameters.AddWithValue("@ExtraOTAmount", 0);
                cmd.Parameters.AddWithValue("@NetPayableWithAllOTAmt", getNetPayableWithAllOTAmt);

                cmd.Parameters.AddWithValue("@LateDays", ViewState["__LateDays__"].ToString());
                cmd.Parameters.AddWithValue("@LateFine", ViewState["__LateFine__"].ToString());
                cmd.Parameters.AddWithValue("@TiffinDays", ViewState["__Tiffindays__"].ToString());
                cmd.Parameters.AddWithValue("@TiffinTaka", ViewState["__TiffinTaka__"].ToString());
                cmd.Parameters.AddWithValue("@TiffinBillAmount", ViewState["__TiffinBillAmount__"].ToString());
                cmd.Parameters.AddWithValue("@HolidayWorkingDays", ViewState["__Holidays__"].ToString());
                cmd.Parameters.AddWithValue("@HolidayTaka", ViewState["__HolidayTaka__"].ToString());
                cmd.Parameters.AddWithValue("@HoliDayBillAmount", ViewState["__HolidayBillAmount__"].ToString());
                cmd.Parameters.AddWithValue("@DormitoryRent", dtCertainEmp.Rows[0]["DormitoryRent"].ToString());
                cmd.Parameters.AddWithValue("@ProvidentFund", ViewState["__PFAmount__"].ToString());
                cmd.Parameters.AddWithValue("@TotalOverTime", ViewState["__getOverTime__"].ToString());
                cmd.Parameters.AddWithValue("@TotalOtherOverTime", ViewState["__getOtherOverTime__"].ToString());
                cmd.Parameters.AddWithValue("@OthersPay", dtOtherspay.Rows[0]["OtherPay"].ToString());
                cmd.Parameters.AddWithValue("@OthersDeduction", dtOthersDeduction.Rows[0]["PAmount"].ToString());
                cmd.Parameters.AddWithValue("@ShortLeave", dtShortleave.Rows.Count);
                cmd.Parameters.AddWithValue("@ProfitTax", ViewState["__TaxAmount__"].ToString());
                cmd.Parameters.AddWithValue("@NightbilAmount", ViewState["__NightbilAmount__"].ToString());
                cmd.Parameters.AddWithValue("@NightBillDays", ViewState["__NightBillDays__"].ToString());

                cmd.Parameters.AddWithValue("@EmpNetGross", ViewState["__presentSalary__"].ToString());
                cmd.Parameters.AddWithValue("@FromDate", ViewState["__FromDate__"].ToString());
                cmd.Parameters.AddWithValue("@ToDate", ViewState["__ToDate__"].ToString());
                cmd.Parameters.AddWithValue("@LWP", dtLeaveInfoLWP.Rows.Count);
                //  int sl = (int)cmd.ExecuteScalar();

                if (int.Parse(cmd.ExecuteNonQuery().ToString()) > 0)
                {
                    if (ckbAdvanceDeduction.Checked)
                        loan_status_update();
                    try
                    {
                        SqlCommand cmd1 = new SqlCommand("update VatTax_IncomeTaxDetailsLog set isPaid=1 where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and Month='" + getYear + "-" + getMonth + "-01'", sqlDB.connection);
                        cmd1.ExecuteNonQuery();
                    }
                    catch { }

                    if (int.Parse(ViewState["__PFAmount__"].ToString())>0)
                        SavePFDetails(getYear+"-" + getMonth + "-" + "01");

                }


                

            }
            catch (Exception ex)
            {
                //lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        private void SavePFDetails(string YearMonth)
        {
            try
            {
                SqlCommand cmdDel = new SqlCommand("Delete PF_PFRecord where convert(varchar(10), Month,120)='" + YearMonth + "' and EmpID='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();

                SqlCommand cmd = new SqlCommand("insert into PF_PFRecord values('" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "','" + YearMonth + "','" +
                    ViewState["__PFAmount__"].ToString() + "','" + ViewState["__PFAmount__"].ToString() + "','0') ", sqlDB.connection);
                cmd.ExecuteNonQuery();

            }
            catch { }
        }
        double getTotalSalary;
        double getTotalSalaryWithAllOT;
        double getNetPayable;
        double getAdvanceInstallment;
        double getLoanInstallment;
        double getPayable;
        double getNetPayableWithAllOTAmt;
        
        double getStampDeduction;
        private void getNetPayableCalculation(int DaysInMonth, payroll_generation pg)   // net payable calculation
        {
            try
            {

                getNetPayable = 0;
                getAdvanceInstallment = 0;
                getLoanInstallment = 0;
                getNetPayableWithAllOTAmt = 0;
                getStampDeduction = 0;
                double getPresentSalary = 0;
                getTotalSalary = 0;
               
                // for advance deduction
                try
                {
                    if (ckbAdvanceDeduction.Checked)
                    {
                        if (dtCutAdvance!=null && dtCutAdvance.Rows.Count > 0)
                            getAdvanceInstallment = Math.Round(double.Parse(dtCutAdvance.Rows[0]["Amount"].ToString()), 0);
                    }
                    

                }
                catch { }

              
                //Late Diduction
                int getAbsendDaysFromLate = 0;
                double lateDeduction = 0;
                //if (isgarments == true) // this condition use to avoid this block . because this is not necessary for RSS
                //{
                if (ckbLateDeduction.Checked)
                {
                    getAbsendDaysFromLate = (dtLate.Rows.Count >= 3) ? (int)dtLate.Rows.Count / 3 : 0;
                    //  lateDeduction = Math.Round(double.Parse(dtCertainEmp.Rows[0]["BasicSalary"].ToString()) / DaysInMonth * getAbsendDaysFromLate, 2);
                    lateDeduction = Math.Round(double.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / DaysInMonth * getAbsendDaysFromLate, 2);                //}
                }                
                ViewState["__LateFine__"] = lateDeduction;
                ViewState["__LateDays__"] = dtLate.Rows.Count;

                //Absent Decuction
                ViewState["__TotalAbsentDays__"] = dtAbsent.Rows.Count;
                //   double getAbsentAmount = Math.Round(double.Parse(dtCertainEmp.Rows[0]["BasicSalary"].ToString()) / DaysInMonth * dtAbsent.Rows.Count, 2);// Always 30 days in month count for Absent Diduction at RSS
                double getAbsentAmount = Math.Round(double.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / DaysInMonth * dtAbsent.Rows.Count, 2);
                //get one day salary 
                //   double onDaySalary = (1 * double.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString())) / DaysInMonth;

                ViewState["__AbsentDeduction__"] = getAbsentAmount.ToString();

                //  ViewState["__PFDeduction"] = dtPFDeduction.Rows[0]["PFAmount"].ToString();

                // getPresentSalary = onDaySalary * double.Parse(ViewState["__PayableDays__"].ToString());
                getPresentSalary = double.Parse(ViewState["__presentSalary__"].ToString());
                getPayable = 0;
                getPayable = Math.Round(((getPresentSalary + double.Parse(dtOtherspay.Rows[0]["OtherPay"].ToString())) - (getAbsentAmount + lateDeduction  + getAdvanceInstallment + getLoanInstallment + double.Parse(dtOthersDeduction.Rows[0]["PAmount"].ToString()) + double.Parse(dtCertainEmp.Rows[0]["DormitoryRent"].ToString()))), 0);

                if (getPayable > double.Parse(ViewState["__PFAmount__"].ToString()))
                    getPayable -= double.Parse(ViewState["__PFAmount__"].ToString());
                else
                    ViewState["__PFAmount__"] = "0";

                if (getPayable > double.Parse(ViewState["__TaxAmount__"].ToString()))
                    getPayable -= double.Parse(ViewState["__TaxAmount__"].ToString());
                else
                    ViewState["__TaxAmount__"] = "0";

                double totalovertimeamt = double.Parse(ViewState["__getTotalOvertimeAmt__"].ToString());
                getNetPayable = getPayable + totalovertimeamt + getAttendanceBonus + double.Parse(ViewState["__TiffinBillAmount__"].ToString()) + double.Parse(ViewState["__HolidayBillAmount__"].ToString()) + double.Parse(ViewState["__NightbilAmount__"].ToString());
                getNetPayable = Math.Round(getNetPayable, 0, MidpointRounding.AwayFromZero);
                
                if (ckbStampDeduction.Checked)
                    getStampDeduction = double.Parse(dtStampDeduct.Rows[0]["StampDeduct"].ToString());
                else
                    getStampDeduction = 0;
                // to get finaly payble amount
                if (getNetPayable >= getStampDeduction)
                    getTotalSalary = Math.Round((getNetPayable - getStampDeduction), 0);
                else
                    getStampDeduction = 0;

            }
            catch (Exception ex)
            {
                // lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        private void PayableDaysCalculation(string Year, string Month, string SelectedDay, string CompanyId, payroll_generation pg, int DaysinMonth)    // For Runing employee
        {
            try
            {
                DataTable dt;
                // string monthYear ="";       
                //sqlDB.fillDataTable("select distinct format(WeekendDate,'yyyy-MM-dd') as WeekendDate from v_Attendance_WeekendInfo where CompanyId='" + CompanyId + "'  AND  MonthName='" + MonthName + "' And WeekendDate >='" + monthYear + "-01' AND WeekendDate <='" + monthYear + '-' + SelectedDay + "' AND WeekendDate not in (select AttDate from tblAttendanceRecord where EmpId='" +   dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND AttStatus='lv')", dt = new DataTable());
                //sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + Year + "-" + Month + "-01' and  ATTDate<='" + Year + "-" + Month + "-" + SelectedDay + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='W' ", dt = new DataTable());
                sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from v_tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + ViewState["__FromDate__"].ToString() + "' and  ATTDate<='" + Year + "-" + Month + "-" + SelectedDay + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='W' ", dt = new DataTable());

                sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from v_tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + Year + "-" + Month + "-" + "01" + "' and  ATTDate<='" + Year + "-" + Month + "-" + SelectedDay + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='W' ", dtWeekendForAttBonus = new DataTable());

                DataTable dtTemp = new DataTable();
                DataTable dtWC = new DataTable();
                if (isgarments == true) // this condition use to avoid this block . because this is not necessary for RSS
                {
                    dtTemp.Columns.Add("AttDate", typeof(string));

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        int count = 0;
                        string date1 = Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).AddDays(-1).ToString("yyyy/MM/dd");
                        string date2 = Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).AddDays(1).ToString("yyyy/MM/dd");
                        DataRow[] result = dtAbsent.Select("AttDate='" + date1 + "'");
                        if (result.Count() > 0)
                        {
                            count++;
                        }
                        DataRow[] result2 = dtAbsent.Select("AttDate='" + date2 + "'");
                        if (result2.Count() > 0)
                        {
                            count++;
                        }
                        if (count > 1)
                        {

                            sqlDB.fillDataTable("SELECT EmpId FROM tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTDate='" + Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).ToString("yyyy-MM-dd") + "' AND StayTime>'04:00:00' ", dtWC);
                            if (dtWC.Rows.Count == 0)
                                dtTemp.Rows.Add(Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).ToString("yyyy/MM/dd"));
                        }
                    }

                    if (dtTemp.Rows.Count > 0)
                    {
                        for (int k = 0; k < dtTemp.Rows.Count; k++)
                        {
                            dtAbsent.Rows.Add(dtTemp.Rows[k]["AttDate"].ToString(), "");
                            dt.Rows.RemoveAt(0);
                        }
                    }
                }
                ViewState["__WeekendCount__"] = dt.Rows.Count.ToString();

                DataTable dtHoliday = new DataTable();
                // sqlDB.fillDataTable("select * from tblHolydayWork where CompanyId='" + CompanyId + "' AND HDate >='" + Year + "-" + Month + "-01' AND HDate <='" + Year + "-" + Month + '-' + SelectedDay + "' AND HDate not in (select AttDate from tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND AttStatus='lv') ", dtHoliday);
               // sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + Year + "-" + Month + "-01' and  ATTDate<='" + Year + "-" + Month + "-" + SelectedDay + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='H' ", dtHoliday = new DataTable());// this line add for RSS ,Date: 05-02-2018
                sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + ViewState["__FromDate__"].ToString() + "' and  ATTDate<='" + Year + "-" + Month + "-" + SelectedDay + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='H' ", dtHoliday = new DataTable());

                sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + Year + "-" + Month + "-01' and  ATTDate<='" + Year + "-" + Month + "-" + SelectedDay + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='H' ", dtHolidayForAttBonus = new DataTable());


                if (isgarments == true) // this condition use to avoid this block . because this is not necessary for RSS
                {
                    dtTemp = new DataTable();
                    dtTemp.Columns.Add("AttDate", typeof(string));
                    for (int i = 0; i < dtHoliday.Rows.Count; i++)
                    {
                        int count = 0;
                        string date1 = Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).AddDays(-1).ToString("yyyy/MM/dd");
                        string date2 = Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).AddDays(1).ToString("yyyy/MM/dd");
                        DataRow[] result = dtAbsent.Select("AttDate='" + date1 + "'");
                        if (result.Count() > 0)
                        {
                            count++;
                        }
                        DataRow[] result2 = dtAbsent.Select("AttDate='" + date2 + "'");
                        if (result2.Count() > 0)
                        {
                            count++;
                        }
                        if (count > 1)
                        {
                            dtWC = new DataTable();
                            sqlDB.fillDataTable("SELECT EmpId FROM tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTDate='" + Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).ToString("yyyy-MM-dd") + "' AND StayTime>'04:00:00' ", dtWC);
                            if (dtWC.Rows.Count == 0)
                                dtTemp.Rows.Add(Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).ToString("yyyy/MM/dd"));
                        }
                    }
                    if (dtTemp.Rows.Count > 0)
                    {
                        for (int k = 0; k < dtTemp.Rows.Count; k++)
                        {
                            dtAbsent.Rows.Add(dtTemp.Rows[k]["AttDate"].ToString(), "");
                            dtHoliday.Rows.RemoveAt(0);
                        }
                    }
                }
                ViewState["__HolidayCount__"] = dtHoliday.Rows.Count.ToString();
                DateTime fromDate = DateTime.Parse(ViewState["__FromDate__"].ToString());
                DateTime toDate = DateTime.Parse(ViewState["__ToDate__"].ToString());
                if (fromDate.Day>1 || int.Parse(SelectedDay) < DaysinMonth)
                {
                    //   int TotalDays = (int.Parse(SelectedDay) - 1) + 1;  // this line find out active days
                    int TotalDays =( toDate.Day-fromDate.Day)+1;  // this line find out active days


                    // int TotalDays = int.Parse((DateTime.Parse(getYear + "-" + getMonth +"-"+ DaysinMonth) - DateTime.Parse(getYear + "-" + getMonth + "-01").AddDays(-1)).TotalDays.ToString());

                    string WorkingDays = (TotalDays - (int.Parse(ViewState["__WeekendCount__"].ToString()) + int.Parse(ViewState["__HolidayCount__"].ToString()))).ToString();
                    ViewState["__WorkingDays__"] = WorkingDays;

                    //-----------Get NetGross--------------------
                    double eps = double.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString());
                    double PresentSalary = (eps / DaysinMonth) * TotalDays;
                    double frac = PresentSalary % 1;
                    if (frac >= 0.5)
                        PresentSalary = Math.Ceiling(PresentSalary);
                    else
                        PresentSalary = Math.Floor(PresentSalary);
                    ViewState["__presentSalary__"] = PresentSalary;
                    //-----------End Get NetGross--------------------
                }
                else
                {
                    ViewState["__presentSalary__"] = dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString();
                    ViewState["__WorkingDays__"] = dtGetMonthSetup.Rows[0]["TotalWorkingDays"].ToString();

                }
                if (ckbSpecialGrossPer.Checked)
                {
                    double presentSalary = double.Parse(ViewState["__presentSalary__"].ToString());
                    double percent = double.Parse(txtSpecialGrossPer.Text.Trim());
                    presentSalary = presentSalary * (percent / 100);
                    ViewState["__presentSalary__"] = Math.Round(presentSalary).ToString();
                }
                ViewState["__PayableDays__"] = "0";
                float PayableDays = float.Parse(dt.Rows.Count.ToString()) + float.Parse(ViewState["__cl__"].ToString()) + float.Parse(ViewState["__sl__"].ToString()) + float.Parse(ViewState["__al__"].ToString()) + dtPresent.Rows.Count + float.Parse(dtHoliday.Rows.Count.ToString());
                ViewState["__PayableDays__"] = PayableDays.ToString();

                ViewState["__PayableDaysForAttBonus__"] = "0";
                float PayableDaysForAttBonus = float.Parse(dtWeekendForAttBonus.Rows.Count.ToString()) + dtPresentForAttBonus.Rows.Count + float.Parse(dtHolidayForAttBonus.Rows.Count.ToString());
                ViewState["__PayableDaysForAttBonus__"] = PayableDaysForAttBonus.ToString();



            }
            catch(Exception ex) { }

        }

        private void getPayableDaysCalculationForML(string MonthName, string SelectedDay, payroll_generation pg)
        {
            try
            {
                DataTable dt;
                string monthYear = MonthName.Substring(3, 4) + "-" + MonthName.Substring(0, 2);
                if (dtPresent.Rows.Count == 0)
                {
                    ViewState["__WeekendCount__"] = "0";
                    ViewState["__HolidayCount__"] = "0";

                }
                else
                {
                    sqlDB.fillDataTable("select * from Attendance_WeekendInfo where  MonthName='" + MonthName + "' And WeekendDate >='" + dtPresent.Rows[0]["ATTDate"].ToString() + "' AND WeekendDate <='" + dtPresent.Rows[dtPresent.Rows.Count - 1]["ATTDate"].ToString() + "'", dt = new DataTable());
                    ViewState["__WeekendCount__"] = dt.Rows.Count.ToString();

                    DataTable dtHoliday = new DataTable();
                    sqlDB.fillDataTable("select * from tblHolydayWork where HDate >='" + dtPresent.Rows[0]["ATTDate"].ToString() + "' AND HDate <='" + dtPresent.Rows[dtPresent.Rows.Count - 1]["ATTDate"].ToString() + "'", dtHoliday);
                    ViewState["__HolidayCount__"] = dtHoliday.Rows.Count.ToString();
                }


                ViewState["__PayableDays__"] = "0";
                //int PayableDays = int.Parse(  ViewState["__WeekendCount__"].ToString()) + int.Parse(  ViewState["__cl__"].ToString()) + int.Parse(  ViewState["__sl__"].ToString()) + int.Parse(  ViewState["__al__"].ToString()) + int.Parse(  ViewState["__ofl__"].ToString()) + int.Parse(  ViewState["__othl__"].ToString()) +   dtPresent.Rows.Count + int.Parse(  ViewState["__HolidayCount__"].ToString()) +   dtAbsent.Rows.Count;
                float PayableDays = float.Parse(ViewState["__WeekendCount__"].ToString()) + float.Parse(ViewState["__cl__"].ToString()) + float.Parse(ViewState["__sl__"].ToString()) + float.Parse(ViewState["__al__"].ToString()) + float.Parse(ViewState["__ofl__"].ToString()) + float.Parse(ViewState["__othl__"].ToString()) + float.Parse(ViewState["__ml__"].ToString()) + float.Parse(dtPresent.Rows.Count.ToString()) + float.Parse(ViewState["__HolidayCount__"].ToString()) + float.Parse(dtAbsent.Rows.Count.ToString());
                ViewState["__PayableDays__"] = PayableDays.ToString();
            }
            catch { }
        }

        private bool joiningMonthIsEqual(string getMonth, string getYear, string CompanyId, string selectdates, payroll_generation pg, int DaysinMonth)   //net payable calculation,compier joining time for generate salary sheet of month
        {
            try
            {
                string[] getJoiningMonth = ViewState["__getJoingingDate__"].ToString().Split('-');

                string getJoinMonth = getJoiningMonth[1] + "-" + getJoiningMonth[2];

                string getCurrentMonth = getMonth + "-" + getYear;

                string[] selectDates = selectdates.Trim().Split('-');


                // below option for checking joining date-month-year is equal of current month 
                if (getJoinMonth.Equals(getCurrentMonth) && int.Parse(getJoiningMonth[0].ToString()) != 1)
                {

                    //-------------Count Payable Days------------------------------------------------------------------

                    string getYearMonth = getYear + "-" + getMonth;

                    DataTable dtHoliday = new DataTable();
                    //  sqlDB.fillDataTable("select convert(varchar(11),HDate,105) as HDate from tblHolydayWork where CompanyId='" + CompanyId + "' AND HDate>='" + getYearMonth + "-" + getJoiningMonth[0] + "' AND HDate <='" + selectDates[2] + "-" + selectDates[1] + "-" + selectDates[0] + "' AND HDate not in (select AttDate from tblAttendanceRecord where EmpId='" +   dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND AttStatus='lv') ", dtHoliday);
                    sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + getYear + "-" + getMonth + "-" + getJoiningMonth[0] + "' and  ATTDate<='" + getYear + "-" + getMonth + "-" + selectDates[0] + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='H' ", dtHoliday = new DataTable());// this line add for RSS ,Date: 05-02-2018
                    
                    byte HDCount = 0;


                    if (getJoiningMonth[0] != "1") getAttendanceBonus = 0;


                    DataTable dtTemp = new DataTable();
                    DataTable dtWC = new DataTable();
                    if (isgarments == true) // this condition use to avoid this block . because this is not necessary for RSS
                    {
                        dtTemp.Columns.Add("AttDate", typeof(string));

                        for (int i = 0; i < dtHoliday.Rows.Count; i++)
                        {
                            int count = 0;
                            string date1 = Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).AddDays(-1).ToString("yyyy/MM/dd");
                            string date2 = Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).AddDays(1).ToString("yyyy/MM/dd");
                            DataRow[] result = dtAbsent.Select("AttDate='" + date1 + "'");
                            if (result.Count() > 0)
                            {
                                count++;
                            }
                            DataRow[] result2 = dtAbsent.Select("AttDate='" + date2 + "'");
                            if (result2.Count() > 0)
                            {
                                count++;
                            }
                            if (count > 1)
                            {
                                dtWC = new DataTable();
                                sqlDB.fillDataTable("SELECT EmpId FROM tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTDate='" + Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).ToString("yyyy-MM-dd") + "' AND StayTime>'04:00:00' ", dtWC);
                                if (dtWC.Rows.Count == 0)
                                    dtTemp.Rows.Add(Convert.ToDateTime(dtHoliday.Rows[i]["HDate"].ToString()).ToString("yyyy/MM/dd"));
                            }
                        }
                        if (dtTemp.Rows.Count > 0)
                        {
                            for (int k = 0; k < dtTemp.Rows.Count; k++)
                            {
                                dtAbsent.Rows.Add(dtTemp.Rows[k]["AttDate"].ToString(), "");
                                dtHoliday.Rows.RemoveAt(0);
                            }
                        }
                    }
                    ViewState["__HolidayCount__"] = dtHoliday.Rows.Count.ToString();

                    //for (byte b = 0; b < dtHoliday.Rows.Count; b++)
                    //{
                    //    string[] dates = dtHoliday.Rows[b]["HDate"].ToString().Split('-');

                    //    DateTime HDay = new DateTime(int.Parse(dates[2]), int.Parse(dates[1]), int.Parse(dates[0]));
                    //    DateTime join = new DateTime(int.Parse(getJoiningMonth[2]), int.Parse(getJoiningMonth[1]), int.Parse(getJoiningMonth[0]));

                    //    if (HDay >= join) HDCount += 1;
                    //}


                    DataTable dtWDInfo;
                    string monthYear = getMonth + "-" + getYear;

                    // sqlDB.fillDataTable("select distinct format(WeekendDate,'dd-MM-yyyy') as WeekendDate from v_Attendance_WeekendInfo where CompanyId='" + CompanyId + "' AND MonthName='" + monthYear + "' And WeekendDate>='" + getYearMonth + "-" + getJoiningMonth[0] + "' AND WeekendDate <='" + selectDates[2] + "-" + selectDates[1] + "-" + selectDates[0] + "' AND WeekendDate not in (select AttDate from tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' AND AttStatus='lv')", dt = new DataTable());
                    sqlDB.fillDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from tblAttendanceRecord where CompanyId='" + CompanyId + "'  AND ATTDate>='" + getYear + "-" + getMonth + "-" + getJoiningMonth[0] + "' and  ATTDate<='" + getYear + "-" + getMonth + "-" + selectDates[0] + "' and EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTStatus='W' ", dt = new DataTable());

                 

                    if (isgarments == true) // this condition use to avoid this block . because this is not necessary for RSS
                    {
                        dtTemp = new DataTable();
                        dtWC = new DataTable();


                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int count = 0;
                            string date1 = Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).AddDays(-1).ToString("yyyy/MM/dd");
                            string date2 = Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).AddDays(1).ToString("yyyy/MM/dd");
                            DataRow[] result = dtAbsent.Select("AttDate='" + date1 + "'");
                            if (result.Count() > 0)
                            {
                                count++;
                            }
                            DataRow[] result2 = dtAbsent.Select("AttDate='" + date2 + "'");
                            if (result2.Count() > 0)
                            {
                                count++;
                            }
                            if (count > 1)
                            {

                                sqlDB.fillDataTable("SELECT EmpId FROM tblAttendanceRecord where EmpId='" + dtCertainEmp.Rows[0]["EmpId"].ToString() + "' and ATTDate='" + Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).ToString("yyyy-MM-dd") + "' AND StayTime>'04:00:00' ", dtWC);
                                if (dtWC.Rows.Count == 0)
                                    dtTemp.Rows.Add(Convert.ToDateTime(dt.Rows[i]["WeekendDate"].ToString()).ToString("yyyy/MM/dd"));
                            }
                        }
                        if (dtTemp.Rows.Count > 0)
                        {
                            for (int k = 0; k < dtTemp.Rows.Count; k++)
                            {
                                dtAbsent.Rows.Add(dtTemp.Rows[k]["AttDate"].ToString(), "");
                                dt.Rows.RemoveAt(0);
                            }
                        }
                    }
                    ViewState["__WeekendCount__"] = dt.Rows.Count.ToString();


                    int TotalDays = (int.Parse(selectDates[0]) - int.Parse(getJoiningMonth[0])) + 1;  // this line find out active days


                    // int TotalDays = int.Parse((DateTime.Parse(getYear + "-" + getMonth +"-"+ DaysinMonth) - DateTime.Parse(getYear + "-" + getMonth + "-01").AddDays(-1)).TotalDays.ToString());

                    string WorkingDays = (TotalDays - (int.Parse(ViewState["__WeekendCount__"].ToString()) + int.Parse(ViewState["__HolidayCount__"].ToString()))).ToString();
                    ViewState["__WorkingDays__"] = WorkingDays;

                    //-----------Get NetGross--------------------
                    ViewState["__presentSalary__"] = Math.Round((double.Parse(dtCertainEmp.Rows[0]["EmpPresentSalary"].ToString()) / DaysinMonth) * TotalDays);
                    //-----------End Get NetGross--------------------

                    ViewState["__PayableDays__"] = "0";
                    float PayableDays = float.Parse(ViewState["__WeekendCount__"].ToString()) + float.Parse(ViewState["__cl__"].ToString()) + float.Parse(ViewState["__sl__"].ToString()) + float.Parse(ViewState["__al__"].ToString()) + float.Parse(ViewState["__ofl__"].ToString()) + float.Parse(ViewState["__othl__"].ToString()) + float.Parse(dtPresent.Rows.Count.ToString()) + float.Parse(ViewState["__HolidayCount__"].ToString());
                    ViewState["__PayableDays__"] = PayableDays.ToString();
                    //--------------------------End--------------------------------------------------------------------                    
                    ViewState["__PayableDaysForAttBonus__"] = "0";
                    float PayableDaysForAttBonus = float.Parse(ViewState["__WeekendCount__"].ToString()) + dtPresentForAttBonus.Rows.Count + float.Parse(ViewState["__HolidayCount__"].ToString());
                    ViewState["__PayableDaysForAttBonus__"] = PayableDaysForAttBonus.ToString();





                    checkForAttendanceBonus(getMonth, getYear, dtCertainEmp.Rows[0]["EmpId"].ToString());

                    getNetPayableCalculation(DaysinMonth, pg);




                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void loan_status_update()
        {
            try
            {
                if (dtCutAdvance!=null && dtCutAdvance.Rows.Count>0)   // for change Advance status
                {                    
                    CRUD.Execute("Update Payroll_LoanMonthlySetup set IsPaid=1 where  SL="+ dtCutAdvance.Rows[0]["SL"].ToString());
                    DataTable _dt=new DataTable();
                    _dt = CRUD.ExecuteReturnDataTable("select LoanAmount from Payroll_LoanInfo  where LoanID=" + dtCutAdvance.Rows[0]["LoanID"].ToString());
                    float LoanAmount =float.Parse( _dt.Rows[0]["LoanAmount"].ToString());
                    
                    //get paid info
                    _dt = new DataTable();
                    _dt = CRUD.ExecuteReturnDataTable("select isNull(sum(Amount),0) as PaidAmount,count(SL) as PaidInstallmentNo from Payroll_LoanMonthlySetup  where LoanID=" + dtCutAdvance.Rows[0]["LoanID"].ToString()+" and IsNull(IsPaid,0)=1");
                    float PaidAmount= float.Parse(_dt.Rows[0]["PaidAmount"].ToString());
                    if (PaidAmount< LoanAmount)
                        CRUD.Execute("update Payroll_LoanInfo set PaidAmount="+ _dt.Rows[0]["PaidAmount"].ToString()
                    + ",PaidInstallmentNo=" + _dt.Rows[0]["PaidInstallmentNo"].ToString()
                    + "   where LoanID=" + dtCutAdvance.Rows[0]["LoanID"].ToString());
                    else
                        CRUD.Execute("update Payroll_LoanInfo set PaidAmount=" + _dt.Rows[0]["PaidAmount"].ToString()
                    + ",PaidInstallmentNo=" + _dt.Rows[0]["PaidInstallmentNo"].ToString()
                    + " ,IsPaid=1  where LoanID=" + dtCutAdvance.Rows[0]["LoanID"].ToString());

                }
            }
            catch { }

        }

        double getOndaySalary;
        double NormalHourlySalary;
        double OverTimeHourlySalary;
        
        

        public DataTable dtGetMonthSetup;
        private void loadMonthSetup(string days, string month, string year, string CompanyId)
        {
            try
            {
                string monthName = new DateTime(int.Parse(year), int.Parse(month), int.Parse(days)).ToString("MMM", CultureInfo.InvariantCulture);
                monthName += year.ToString().Substring(2, 2);
                string monthName2 = month + "-" + year;
                SQLOperation.selectBySetCommandInDatatable("select TotalDays,TotalWeekend ,FromDate,ToDate,TotalHoliday,TotalWorkingDays from tblMonthSetup where CompanyId='" + CompanyId + "' AND ( MonthName='" + monthName + "' OR MonthName='" + monthName2 + "') ", dtGetMonthSetup = new DataTable(), sqlDB.connection);

            }
            catch (Exception ex)
            {
            }
        }

        int getAttendanceBonus;
        private void checkForAttendanceBonus(string month, string year, string EmpId)   // check attendance bonus
        {
            try
            {
                //DataTable dtApplydate = new DataTable();
                //// get applydate information of a certain employee
                //sqlDB.fillDataTable("SELECT EmpId,Convert(varchar(7),Lv_date,126) FROM Leave_ApplyDate where EmpId='" + EmpId + "' and Convert(varchar(7),Lv_date,126)='" + year + "-" + month + "'", dtApplydate);

                //if (int.Parse(dtGetMonthSetup.Rows[0]["TotalWorkingDays"].ToString()) == dtPresent.Rows.Count)
                //{
                // if (dtLate.Rows.Count >= 1 || dtApplydate.Rows.Count > 0 || dtAbsent.Rows.Count >= 1 || int.Parse(dtGetMonthSetup.Rows[0]["TotalDays"].ToString()) != int.Parse(ViewState["__PayableDays__"].ToString())) getAttendanceBonus = 0;
                if (ckbAttendanceBonus.Checked)
                {
                    int __TotalDays__ = int.Parse(dtGetMonthSetup.Rows[0]["TotalDays"].ToString());
                    int __PayableDaysForAttBonus__ = int.Parse(ViewState["__PayableDaysForAttBonus__"].ToString());
                    string __hasAttBonus__= ViewState["__hasAttBonus__"].ToString();
                    if (__hasAttBonus__.Equals("False")|| __TotalDays__ != __PayableDaysForAttBonus__) getAttendanceBonus = 0;
                    else getAttendanceBonus = int.Parse(dtCertainEmp.Rows[0]["AttendanceBonus"].ToString());
                }
                else
                    getAttendanceBonus = 0;

                //}
                //else getAttendanceBonus = 0;
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        private void salarySheetClearByMonthYear(string selectDays, string month, string year, string CompanyId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("delete from Payroll_MonthlySalarySheet where CompanyId='" + CompanyId + "'  AND YearMonth='" + year + "-" + month + "-01' And ToDate='" + year+"-"+month+"-"+selectDays+"' AND EmpStatus in ('1','8') AND IsSeperationGeneration='0'", sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }
        private void salarySheetClearByMonthYear(string selectDays, string month, string year, string CompanyId, string empcardno)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("delete from Payroll_MonthlySalarySheet where CompanyId='" + CompanyId + "'  AND YearMonth='" + year + "-" + month + "-01' And ToDate='" + year + "-" + month + "-" + selectDays + "' AND EmpStatus in ('1','8') AND EmpCardNo LIKE '%" + empcardno + "' AND IsSeperationGeneration='0'", sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        private void getAllLeaveInformation()   // all leave information
        {
            try
            {
                ViewState["__cl__"] = 0; ViewState["__sl__"] = 0; ViewState["__al__"] = 0; ViewState["__WeekendAsLeave__"] = "0"; ViewState["__ml__"] = "0";
                ViewState["__ofl__"] = 0; ViewState["__othl__"] = 0;
                if (dtLeaveInfo.Rows.Count > 0)
                {

                    DataRow[] dr = dtLeaveInfo.Select("StateStatus ='Casual Leave'");
                    ViewState["__cl__"] = dr.Length;

                    DataRow[] dr1 = dtLeaveInfo.Select("StateStatus ='Sick Leave'");
                    ViewState["__sl__"] = dr1.Length;


                    DataRow[] dr2 = dtLeaveInfo.Select("StateStatus ='Annual Leave'");
                    ViewState["__al__"] = dr2.Length;

                    DataRow[] dr3 = dtLeaveInfo.Select("StateStatus ='Maternity Leave'");
                    ViewState["__ml__"] = dr3.Length;

                    DataRow[] dr4 = dtLeaveInfo.Select("StateStatus ='Official Purpose Leave'");
                    ViewState["__ofl__"] = dr4.Length;

                    DataRow[] dr5 = dtLeaveInfo.Select("StateStatus ='Others Leave'");
                    ViewState["__othl__"] = dr5.Length;

                    DataRow[] drWeekendAsLeave = dtLeaveInfo.Select("AttDate " + Session["__setPredicate__"].ToString());
                    ViewState["__WeekendAsLeave__"] = drWeekendAsLeave.Length;




                }

            }
            catch (Exception ex)
            {
                //   lblMessage.InnerText ="error->"+ex.Message;
            }
        }

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            classes.Employee.LoadEmpCardNoWithNameByCompanyRShift(ddlEmpCardNo, ddlCompanyList.SelectedValue);
            if (rbGenaratingType.SelectedValue == "0")
            {
                ddlEmpCardNo.Enabled = false;
                txtEmpCardNo.Enabled = false;
            }
            else
            {
                ddlEmpCardNo.Enabled = true;
                txtEmpCardNo.Enabled = true;
            }


            ViewState["___IsGerments__"] = classes.Payroll.Office_IsGarments();
            if (ViewState["___IsGerments__"].ToString().Equals("False"))
            {
                if (ddlCompanyList.SelectedValue == "0001")
                    txtNotTiffinCardno.Text = "";
                else
                    txtNotTiffinCardno.Text = "0069,0037";
            }
            loadExistingSalary();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
        }
        private void removeTiffinCount(string Month)
        {
            try
            {

                if (txtNotTiffinCardno.Text.Trim() != "")
                {
                    SqlCommand cmd = new SqlCommand("update tblAttendanceRecord set TiffinCount=0  where EmpId in(select EmpId from v_EmployeeDetails where substring(EmpCardNo,10,10) in(" + txtNotTiffinCardno.Text + ") and CompanyId='" + ddlCompanyList.SelectedValue + "') and format(ATTDate,'MM-yyyy')='" + Month + "' ", sqlDB.connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private void SaveProvidentFundDetails(string Month)
        {
            try
            {

                if (txtNotTiffinCardno.Text.Trim() != "")
                {
                    SqlCommand cmd = new SqlCommand("update tblAttendanceRecord set TiffinCount=0  where EmpId in(select EmpId from v_EmployeeDetails where substring(EmpCardNo,10,10) in(" + txtNotTiffinCardno.Text + ") and CompanyId='" + ddlCompanyList.SelectedValue + "') and format(ATTDate,'MM-yyyy')='" + Month + "' ", sqlDB.connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }
        private void loadExistingSalary()
        {
            try
            {
                DataTable dtExSalary = new DataTable();
                sqlDB.fillDataTable("select distinct top(3) convert(varchar(10), FromDate,120) as FromDate,convert(varchar(10), ToDate,120) as ToDate,format(FromDate,'MMM-yyyy') as MonthYear, count(EmpId) as Total,sum( case when EmpTypeId=1 then 1 else 0 end) as Worker,sum( case when EmpTypeId=2 then 1 else 0 end) as Staff  from Payroll_MonthlySalarySheet where IsSeperationGeneration=0 and FromDate is not null and CompanyId='"+ddlCompanyList.SelectedValue+"' group by  convert(varchar(10), FromDate,120),convert(varchar(10), ToDate,120),format(FromDate,'MMM-yyyy') order by ToDate desc", dtExSalary);// this line add for RSS ,Date: 05-02-2018
                gvSalaryList.DataSource = dtExSalary;
                gvSalaryList.DataBind();
            }
            catch { }
        }

        protected void gvSalaryList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

               
                if (e.CommandName == "Remove")
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    string ToDate = gvSalaryList.DataKeys[rIndex].Values[0].ToString();
                    if (deleteExSalary(ToDate))
                    {
                        lblMessage.InnerText = "warning-> Successfully Deleted.";
                        gvSalaryList.Rows[rIndex].Visible = false;
                    }
                        
                }
            }
            catch { }
        }
        private bool deleteExSalary(string ToDate)
        {
            try
            {
                sqlCmd = "delete Payroll_MonthlySalarySheet where IsSeperationGeneration=0 and ToDate='" + ToDate + "' and CompanyId='" + ddlCompanyList.SelectedValue + "'";
                return CRUD.Execute(sqlCmd, sqlDB.connection);
                   
            }
            catch (Exception ex) { lblMessage.InnerText = "error-> "+ex.Message; return false; }
            
        }
    }


}