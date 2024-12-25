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



namespace SigmaERP.attendance
{
    public partial class monthly_in_out_report : System.Web.UI.Page
    {
        string CompanyId="";
        //view=267;
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            int[] pagePermission = { 267 };
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                Session["__MinDigits__"] = "6";
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

               // string[] AccessPermission = new string[0];
                //System.Web.UI.HtmlControls.HtmlTable a = tblGenerateType;
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                // AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "monthly_in_out_report.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
              //  ViewState["__ReadAction__"] = AccessPermission[0];
                classes.commonTask.loadMonthIdByCompany(ddlMonthList, ViewState["__CompanyId__"].ToString());
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
            }
            catch { }

        }

  
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }      

        protected void ddlShiftName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                lstAll.Items.Clear();
                lstSelected.Items.Clear();
                 CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();         
                }
            catch { }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {//------------------------Validation--------------------------------
                if (rblGenerateType.SelectedIndex == 0)
                {
                    if (ddlMonthList.SelectedValue == "0")
                    {
                        lblMessage.InnerText = "warning-> Please Select Any Month!"; ddlMonthList.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }
                    if (lstSelected.Items.Count == 0)
                    {
                        lblMessage.InnerText = "warning-> Please Select Any Department!"; lstSelected.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }
                }
                else
                {
                    if (ddlMonthList.SelectedValue == "0")
                    {
                        lblMessage.InnerText = "warning-> Please Select Any Month!"; ddlMonthList.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }
                    if (txtCardNo.Text.Trim().Length ==0)
                    {
                        lblMessage.InnerText = "warning-> Please Type Valid Card Number!";
                        txtCardNo.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }
                }
                //----------------------------End-------------------------------
                if (rblReportType.SelectedValue == "3")
                {
                    if (ViewState["__CShortName__"].ToString().Equals("MRC"))// Marico
                        GenerateJobCardReport_Marico();
                    else
                        GenerateJobCardReport();
                }
                else if (rblReportType.SelectedValue == "5")
                {
                    if (ViewState["__CShortName__"].ToString().Equals("MRC"))// Marico
                        GenerateJobCardReport_Marico();
                    else
                        GenerateJobCardReportForActualAndCompliance();
                }
                else if (rblReportType.SelectedValue == "4")
                {
                    GenerateHolidayAndWeekendReport();
                }
                else
                {
                    if (rblLanguage.SelectedValue == "EN")
                        GenerateReportEnglish();
                    else
                        GenerateReportBangla();
                }
            }
            catch { }
        }
        private void GenerateReportEnglish()
        {
            try
            {
                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId= " + rblEmpType.SelectedValue + "";
                string CompanyList = "";
                string ShiftList = "";
                string DepartmentList = "";
                string ReportTitle = "";
                string ReportDate = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }

                CompanyList = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                CompanyList = "in ('" + CompanyList + "')";
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);




                DataTable dt = new DataTable();

                string[] MY = ddlMonthList.SelectedItem.Value.ToString().Split('-');
                string type = "";

                if (rblReportType.SelectedValue == "0")
                {
                    dt = classes.BusinessLogic.get_MonthlyLoginLogOutTime(CompanyList, DepartmentList, MY[0], MY[1], rblGenerateType.SelectedIndex, txtCardNo.Text, EmpTypeID);
                    type = "Log InOut";
                }
                else if (rblReportType.SelectedValue == "1")
                {
                    dt = classes.BusinessLogic.get_Moanthly_Attendance_Sheet(CompanyList, DepartmentList, MY[0], MY[1], rblGenerateType.SelectedIndex, txtCardNo.Text, EmpTypeID);
                    type = "Att Status";
                }
                else
                {
                    dt = classes.BusinessLogic.get_Moanthly_Attendance_Sheet_Summary(CompanyList, DepartmentList, MY[0], MY[1], rblGenerateType.SelectedIndex, txtCardNo.Text, EmpTypeID);
                    type = "Att Summary";
                }

                //  sqlDB.fillDataTable("select SftEndTime from HRD_Shift where SftId=" + ddlShiftName.SelectedItem.Value.ToString() + "", dt);







                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No Attendance Available";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                Session["__MonthlyLoginLogoutReport__"] = dt;

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=MonthlyLoginLogoutReport-" + ddlMonthList.SelectedItem.Value.ToString() + "-" + type + "');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }
    
        private void GenerateReportBangla()
        {
            try
            {
                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId= " + rblEmpType.SelectedValue + "";
                string CompanyList = "";
                string ShiftList = "";
                string DepartmentList = "";
                string ReportTitle = "";
                string ReportDate = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }

                CompanyList = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                CompanyList = "in ('" + CompanyList + "')";
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);




                DataTable dt = new DataTable();

                string[] MY = ddlMonthList.SelectedItem.Value.ToString().Split('-');
                string type = "";

                if (rblReportType.SelectedIndex == 0)
                {
                    dt = classes.BusinessLogic.get_MonthlyLoginLogOutTimeBangla(CompanyList, DepartmentList, MY[0], MY[1], rblGenerateType.SelectedIndex, txtCardNo.Text, EmpTypeID);
                    type = "Log InOut";
                }
                else if (rblReportType.SelectedIndex == 1)
                {
                    dt = classes.BusinessLogic.get_Moanthly_Attendance_SheetBangla(CompanyList, DepartmentList, MY[0], MY[1], rblGenerateType.SelectedIndex, txtCardNo.Text, EmpTypeID);
                    type = "Att Status";
                }
                else
                {
                    dt = classes.BusinessLogic.get_Moanthly_Attendance_Sheet_SummaryBangla(CompanyList, DepartmentList, MY[0], MY[1], rblGenerateType.SelectedIndex, txtCardNo.Text, EmpTypeID);
                    type = "Att Summary";
                }

                //  sqlDB.fillDataTable("select SftEndTime from HRD_Shift where SftId=" + ddlShiftName.SelectedItem.Value.ToString() + "", dt);







                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No Attendance Available";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                Session["__MonthlyLoginLogoutReportBangla__"] = dt;

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=MonthlyLoginLogoutReportBangla-" + classes.commonTask.GenerateBanglaMonthNameMY(ddlMonthList.SelectedValue) + "-" + type + "');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }
    

        protected void rblGenerateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                if (rblGenerateType.SelectedIndex == 0) txtCardNo.Enabled = false;
                else { txtCardNo.Enabled = true; txtCardNo.Focus(); }
            }
            catch { }
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
            classes.commonTask.loadMonthIdByCompany(ddlMonthList,CompanyId) ;
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
            lstSelected.Items.Clear();
          //  classes.commonTask.LoadShift(ddlShiftName, CompanyId, ViewState["__UserType__"].ToString());
          
        }
        private void GenerateJobCardReport_Marico()
        {


            try
            {

                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId= " + rblEmpType.SelectedValue + "";
                string DepartmentList = "";
                if (rblGenerateType.SelectedValue == "0")
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);

                string[] Month = ddlMonthList.SelectedValue.Split('-');
                string sql = "";
                DataTable dt = new DataTable();
                if (rblGenerateType.SelectedValue == "1")
                {
                    bool hasEmpCard = AccessControl.hasEmpcardPermission(txtCardNo.Text.Trim(), ddlCompanyName.SelectedValue);

                    if (!hasEmpCard)
                    {
                        lblMessage.InnerText = "warning-> You have no permission on this Employee";
                        txtCardNo.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }

                    sql = " Select EmpId,SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime, 1 as OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,TotalOverTime,TotalDays,PaybleDays From v_tblAttendanceRecord " +
                     " Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "'  order by  ATTDate";
                }                  
                 


                else                    
                    sql = " Select EmpId,SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,1 as OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,TotalOverTime,TotalDays,PaybleDays From v_tblAttendanceRecord " +
                        " Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + "  Order By convert(int,DptId), CustomOrdering,Empid, ATTDate";
                //    sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, StayTime )-convert(varchar(8),OtherOverTime,114)),'hh:mm:ss') ) else   StayTime end as StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime as TotalOverTime,TotalDays,OtherOverTime,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, OutHour+':'+OutMin+':'+OutSec )-convert(varchar(8),OtherOverTime,114)) +convert(datetime,'00:00:'+OutSec ),'HH:mm:ss')) else   OutHour+':'+OutMin+':'+OutSec end as OutTime From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime,TotalDays,OtherOverTime order by  ATTDate  ", dt);
                //else sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, StayTime )-convert(varchar(8),OtherOverTime,114)),'hh:mm:ss') ) else   StayTime end as StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime as TotalOverTime,TotalDays,OtherOverTime,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, OutHour+':'+OutMin+':'+OutSec )-convert(varchar(8),OtherOverTime,114))+convert(datetime,'00:00:'+OutSec ),'HH:mm:ss') ) else   OutHour+':'+OutMin+':'+OutSec end as OutTime From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + " Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime,TotalDays,GId,CustomOrdering ,OtherOverTime Order By convert(int,DptId), CustomOrdering,Empid, ATTDate   ", dt);
                sqlDB.fillDataTable(sql, dt);
                Session["__dtJobCard__"] = dt;
                if (dt.Rows.Count > 0)
                {
                    DataTable dtSummary = new DataTable();
                   

                    if (rblGenerateType.SelectedValue == "1") sqlDB.fillDataTable("Select EmpId,SUM(CASE WHEN StateStatus = 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'Casual Leave' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'Sick Leave' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'Maternity Leave' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'Annual Leave' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus = 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum(PaybleDays) AS 'APday' From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' group by EmpId", dtSummary);
                    else sqlDB.fillDataTable("Select EmpId,SUM(CASE WHEN StateStatus = 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'Casual Leave' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'Sick Leave' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'Maternity Leave' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'Annual Leave' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus = 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum(PaybleDays) AS 'APday' From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' " + EmpTypeID + " and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " group by EmpId", dtSummary);
                    Session["__dtSummary__"] = dtSummary;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=JobCardReportActualMarico');", true);  //Open New Tab for Sever side code         
                }
                else
                {
                    lblMessage.InnerText = "warning->No Attendance Available";
                }
            }
            catch { }
        }

        private void GenerateJobCardReport() 
        {

          
            try
            {

                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId= " + rblEmpType.SelectedValue + "";
                string DepartmentList = "";
                if(rblGenerateType.SelectedValue=="0")
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);

                string[] Month = ddlMonthList.SelectedValue.Split('-');
                string sql = "";
                DataTable dt = new DataTable();
                if (rblGenerateType.SelectedValue == "1")
                    //sql = "with" +
                    //      " att as (SELECT a.*, s.SftStartTime,s.SftEndTime," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end as OutHour1 ," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec as Out,SUBSTRING(OutHour + ':' + OutMin + ':' + OutSec, 5, 4) OutTemp, case when(s.SftEndTime < convert(time," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec)) then(FORMAT((convert(datetime," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec) - convert(varchar(8), s.SftEndTime, 114)), 'HH:mm:ss')) else '00:00:00' end as ActualOverTime,  case when(case when(s.SftEndTime < convert(time," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec)) then(FORMAT((convert(datetime," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec) - convert(varchar(8), s.SftEndTime, 114)), 'HH:mm:ss')) else '00:00:00' end) > '02:00:00' then 1 else 0 end as exOT from v_tblAttendanceRecord as a inner join HRD_Shift as s on a.SftId = s.SftId)" +
                    sql= " with s as( select SftId, SftStartTime as SftStartTime1 , convert(time(7), dateadd(hour, 9, '2019-01-01 ' + convert(varchar(8), SftStartTime))) as SftEndTime1  from HRD_Shift)," +
                        " att as(SELECT a.*,case when InTime<>'00:00:00' then case when InTime<DATEADD(MINUTE,-30,s.SftStartTime1) then convert(varchar(6), DATEADD(MINUTE, -30, s.SftStartTime1))+case when LEN(InSec)=1 then '0'+InSec else InSec end else case when LEN(InHour)=1 then '0'+InHour else InHour end + ':' + case when LEN(InMin)=1 then '0'+InMin else InMin end + ':' + case when LEN(InSec)=1 then '0'+InSec else InSec end end else '00:00:00' end as InTimeA,SftEndTime1,  OutHour+':'+OutMin+':'+OutSec  as Out,SUBSTRING( OutHour+':'+OutMin+':'+OutSec,5,4) OutTemp," +
                        " case when (s.SftEndTime1< convert(time,OutHour+':'+OutMin+':'+OutSec) or OverTime<>'00:00:00') then CONVERT(time(0), DATEADD(SECOND, DATEDIFF(SECOND, '2018/01/01 '+convert(varchar(8), s.SftEndTime1, 114), '2018/01/01 '+OutHour+':'+OutMin+':'+OutSec), 0))else '00:00:00' end as ActualOverTime,case when(case when (s.SftEndTime1< convert(time,OutHour+':'+OutMin+':'+OutSec) or OverTime<>'00:00:00') " +
                        " then CONVERT(time(0), DATEADD(SECOND, DATEDIFF(SECOND, '2018/01/01 '+convert(varchar(8), s.SftEndTime1, 114), '2018/01/01 '+OutHour+':'+OutMin+':'+OutSec), 0))else '00:00:00' end) > '02:00:00' then 1 else 0 end as exOT from v_tblAttendanceRecord1 as a inner join s on a.SftId = s.SftId ) " +
                        " Select EmpId, SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,format(ATTDate, 'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,case when LEN(convert(varchar(2),InTimeA))=1 then '0'+convert(varchar(2),InTimeA) else convert(varchar(2),InTimeA) end as InHour,case when LEN(SUBSTRING(InTimeA,4,2))=1 then '0'+SUBSTRING(InTimeA,4,2) else SUBSTRING(InTimeA,4,2) end as InMin,case when LEN(InSec)=1 then '0'+InSec else InSec end as  InSec, OutHour,OutMin,ATTStatus,OverTime,DptId," +
                        " StateStatus,Convert(varchar(11), EmpJoiningDate, 105) as EmpJoiningDate,GrdName,EmpType,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime, case when exOT = '1' then '02:0' + OutTemp else ActualOverTime end as TotalOverTime,TotalDays,OtherOverTime,case when exOT = '1' then format(convert(datetime,'02:0' + OutTemp)+convert(varchar(8), SftEndTime1, 114),'HH:mm:ss') else Out end as OutTime, " +
                        " case when OutHour+':'+OutMin+':'+OutSec<>'00:00:00' then  CONVERT(time(0), DATEADD(SECOND, DATEDIFF(SECOND, '2018/01/01 '+InTimeA, '2018/01/01 '+case when exOT = '1' then format(convert(datetime,'02:0' + OutTemp)+convert(varchar(8), SftEndTime1, 114),'HH:mm:ss') else Out end), 0)) else '00:00:00' end as StayTime" +
                        " From att" +
                        " Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "'  order by  ATTDate";
                else
                    //sql = "with" +
                    //      " att as (SELECT a.*, s.SftStartTime,s.SftEndTime," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end as OutHour1 ," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec as Out,SUBSTRING(OutHour + ':' + OutMin + ':' + OutSec, 5, 4) OutTemp, case when(s.SftEndTime < convert(time," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec)) then(FORMAT((convert(datetime," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec) - convert(varchar(8), s.SftEndTime, 114)), 'HH:mm:ss')) else '00:00:00' end as ActualOverTime,  case when(case when(s.SftEndTime < convert(time," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec)) then(FORMAT((convert(datetime," +
                    //      " case when(OtherOverTime <> '00:00:00.0000000' and OutHour = '00') then '23' else OutHour end" +
                    //      " + ':' + OutMin + ':' + OutSec) - convert(varchar(8), s.SftEndTime, 114)), 'HH:mm:ss')) else '00:00:00' end) > '02:00:00' then 1 else 0 end as exOT from v_tblAttendanceRecord as a inner join HRD_Shift as s on a.SftId = s.SftId)" +

                    sql = " with s as(select SftId, SftStartTime as SftStartTime1 , convert(time(7), dateadd(hour, 9, '2019-01-01 ' + convert(varchar(8), SftStartTime))) as SftEndTime1  from HRD_Shift)," +
                        " att as(SELECT a.*,case when InTime<>'00:00:00' then case when InTime<DATEADD(MINUTE,-30,s.SftStartTime1) then convert(varchar(6), DATEADD(MINUTE, -30, s.SftStartTime1))+case when LEN(InSec)=1 then '0'+InSec else InSec end else case when LEN(InHour)=1 then '0'+InHour else InHour end + ':' + case when LEN(InMin)=1 then '0'+InMin else InMin end + ':' + case when LEN(InSec)=1 then '0'+InSec else InSec end end else '00:00:00' end as InTimeA,SftEndTime1, OutHour+':'+OutMin+':'+OutSec  as Out,SUBSTRING( OutHour+':'+OutMin+':'+OutSec,5,4) OutTemp,case when (s.SftEndTime1< convert(time,OutHour+':'+OutMin+':'+OutSec) or OverTime<>'00:00:00') then CONVERT(time(0), DATEADD(SECOND, DATEDIFF(SECOND, '2018/01/01 '+convert(varchar(8), s.SftEndTime1, 114), '2018/01/01 '+OutHour+':'+OutMin+':'+OutSec), 0))else '00:00:00' end as ActualOverTime,case when(case when (s.SftEndTime1< convert(time,OutHour+':'+OutMin+':'+OutSec) or OverTime<>'00:00:00') then CONVERT(time(0), DATEADD(SECOND, DATEDIFF(SECOND, '2018/01/01 '+convert(varchar(8), s.SftEndTime1, 114), '2018/01/01 '+OutHour+':'+OutMin+':'+OutSec), 0))else '00:00:00' end) > '02:00:00' then 1 else 0 end as exOT from v_tblAttendanceRecord1 as a inner join s on a.SftId = s.SftId ) " +
                        " Select EmpId, SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,format(ATTDate, 'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,case when LEN(convert(varchar(2),InTimeA))=1 then '0'+convert(varchar(2),InTimeA) else convert(varchar(2),InTimeA) end as InHour,case when LEN(SUBSTRING(InTimeA,4,2))=1 then '0'+SUBSTRING(InTimeA,4,2) else SUBSTRING(InTimeA,4,2) end as InMin,case when LEN(InSec)=1 then '0'+InSec else InSec end as  InSec,OutHour,OutMin,ATTStatus,OverTime,DptId," +
                        " StateStatus,Convert(varchar(11), EmpJoiningDate, 105) as EmpJoiningDate,GrdName,EmpType,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime, case when exOT = '1' then '02:0' + OutTemp else ActualOverTime end as TotalOverTime,TotalDays,OtherOverTime,case when exOT = '1' then format(convert(datetime,'02:0' + OutTemp)+convert(varchar(8), SftEndTime1, 114),'HH:mm:ss') else Out end as OutTime, " +
                        " case when OutHour+':'+OutMin+':'+OutSec<>'00:00:00' then  CONVERT(time(0), DATEADD(SECOND, DATEDIFF(SECOND, '2018/01/01 '+InTimeA, '2018/01/01 '+case when exOT = '1' then format(convert(datetime,'02:0' + OutTemp)+convert(varchar(8), SftEndTime1, 114),'HH:mm:ss') else Out end), 0)) else '00:00:00' end as StayTime" +
                        " From att" +
                        " Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + "  Order By convert(int,DptId), CustomOrdering,Empid, ATTDate";
              
                
                //    sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, StayTime )-convert(varchar(8),OtherOverTime,114)),'hh:mm:ss') ) else   StayTime end as StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime as TotalOverTime,TotalDays,OtherOverTime,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, OutHour+':'+OutMin+':'+OutSec )-convert(varchar(8),OtherOverTime,114)) +convert(datetime,'00:00:'+OutSec ),'HH:mm:ss')) else   OutHour+':'+OutMin+':'+OutSec end as OutTime From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime,TotalDays,OtherOverTime order by  ATTDate  ", dt);
                //else sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, StayTime )-convert(varchar(8),OtherOverTime,114)),'hh:mm:ss') ) else   StayTime end as StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime as TotalOverTime,TotalDays,OtherOverTime,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, OutHour+':'+OutMin+':'+OutSec )-convert(varchar(8),OtherOverTime,114))+convert(datetime,'00:00:'+OutSec ),'HH:mm:ss') ) else   OutHour+':'+OutMin+':'+OutSec end as OutTime From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + " Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime,TotalDays,GId,CustomOrdering ,OtherOverTime Order By convert(int,DptId), CustomOrdering,Empid, ATTDate   ", dt);
                sqlDB.fillDataTable(sql, dt);
                Session["__dtJobCard__"] = dt;
                if (dt.Rows.Count > 0)
                {
                    DataTable dtSummary = new DataTable();
                    //if (rblGenerateType.SelectedValue == "1") sqlDB.fillDataTable("Select EmpId,SUM(CASE WHEN StateStatus = 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'C/L' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'S/L' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'M/L' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'E/L' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus = 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum(PaybleDays) AS 'APday' From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' group by EmpId", dtSummary);
                    //else sqlDB.fillDataTable("Select EmpId,SUM(CASE WHEN StateStatus = 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'C/L' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'S/L' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'M/L' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'E/L' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus = 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum(PaybleDays) AS 'APday' From  v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' " + EmpTypeID + " and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " group by EmpId", dtSummary);

                   if (rblGenerateType.SelectedValue == "1") sqlDB.fillDataTable("Select EmpId,SUM(CASE WHEN StateStatus= 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'Casual Leave' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'Sick Leave' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'Maternity Leave' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'Annual Leave' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus= 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum( CASE WHEN StateStatus= 'Present' then 1 else 0 end ) AS 'APday' From v_tblAttendanceRecord1 Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' group by EmpId", dtSummary);
                    else sqlDB.fillDataTable("Select EmpId,SUM(CASE WHEN StateStatus= 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'Casual Leave' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'Sick Leave' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'Maternity Leave' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'Annual Leave' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus= 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum( CASE WHEN StateStatus= 'Present' then 1 else 0 end ) AS 'APday' From  v_tblAttendanceRecord1 Where CompanyId='" + ddlCompanyName.SelectedValue + "' " + EmpTypeID + " and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " group by EmpId", dtSummary);
                    Session["__dtSummary__"] = dtSummary;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=JobCardReport');", true);  //Open New Tab for Sever side code         
                }
                else
                {
                    lblMessage.InnerText = "warning->No Attendance Available";
                }
            }
            catch { }
        }

        private void GenerateJobCardReportForActualAndCompliance()
        {


            try
            {

                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId= " + rblEmpType.SelectedValue + "";
                string DepartmentList = "";
                if (rblGenerateType.SelectedValue == "0")
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);

                string[] Month = ddlMonthList.SelectedValue.Split('-');
                string sql = "";
                DataTable dt = new DataTable();

                // before fix 9hr for Delivery & Admin Department 
                //if (rblGenerateType.SelectedValue == "1")
                //    sql = @"DECLARE @maxOT VARCHAR(8) = '02:00:00'
                //           select TotalOverTime as actualTotalOverTime, case when ATTStatus='W' or ATTStatus='H' then '00:00:00' else case when TotalOverTime>@maxOT then  '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else TotalOverTime end end as TotalOverTime,
                //         case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),OutHour+':'+OutMin+':'+OutSec)) else OutHour+':'+OutMin+':'+OutSec end as OutTime,
                //         OutHour,OutMin,OutSec,case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),StayTime)) else StayTime end as StayTime,
                //        StayTime as actualStayTime,InHour,InMin,InSec,EmpId,SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,
                //        format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,
                //        StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,
                //        GName,MonthId,BreakStartTime,BreakEndTime,TotalDays,PaybleDays From v_tblAttendanceRecord 
                //        Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "'  order by  ATTDate";
                //else
                //    sql = @"DECLARE @maxOT VARCHAR(8) = '02:00:00'
                //           select TotalOverTime as actualTotalOverTime, case when ATTStatus='W' or ATTStatus='H' then '00:00:00' else case when TotalOverTime>@maxOT then  '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else TotalOverTime end end as TotalOverTime,
                //         case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),OutHour+':'+OutMin+':'+OutSec)) else OutHour+':'+OutMin+':'+OutSec end as OutTime,
                //         OutHour,OutMin,OutSec,case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),StayTime)) else StayTime end as StayTime,
                //        StayTime as actualStayTime,InHour,InMin,InSec,EmpId,SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,
                //        format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,
                //        StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,
                //        GName,MonthId,BreakStartTime,BreakEndTime,TotalDays,PaybleDays From v_tblAttendanceRecord  Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + "  Order By convert(int,DptId), CustomOrdering,Empid, ATTDate";

                //below fix 9hr for Delivery & Admin Department 
                if (rblGenerateType.SelectedValue == "1")
                    sql = @"DECLARE @maxOT VARCHAR(8) = '02:00:00'
DECLARE @maxStayTime VARCHAR(8) = '09:00:00' --for delivery(0043),Admin
                           select TotalOverTime as actualTotalOverTime, case when ATTStatus='W' or ATTStatus='H' then '00:00:00' else case when TotalOverTime>@maxOT then  '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else TotalOverTime end end as TotalOverTime,
						  case when DptId in('0043','0075') and StayTime>@maxStayTime then
						 CONVERT(TIME,  DATEADD(SECOND, DATEDIFF(SECOND, 0, '09:0'+SUBSTRING(OutMin,2,1)+':'+OutSec), '2022-01-01 '+InHour+':'+InMin+':'+InSec))
						   else 
                         case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),OutHour+':'+OutMin+':'+OutSec)) else OutHour+':'+OutMin+':'+OutSec end End as OutTime
						 ,
                         OutHour,OutMin,OutSec,
						 case when DptId in('0043','0075') and StayTime>@maxStayTime then '09:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else 
						 case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),StayTime)) else StayTime end end as StayTime			 
						 ,
                        StayTime as actualStayTime,InHour,InMin,InSec,EmpId,SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,
                        format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,
                        StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,
                        GName,MonthId,BreakStartTime,BreakEndTime,TotalDays,PaybleDays From v_tblAttendanceRecord 
                        Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "'  order by  ATTDate";
                else
                    sql = @"DECLARE @maxOT VARCHAR(8) = '02:00:00'
DECLARE @maxStayTime VARCHAR(8) = '09:00:00' --for delivery(0043),Admin
                           select TotalOverTime as actualTotalOverTime, case when ATTStatus='W' or ATTStatus='H' then '00:00:00' else case when TotalOverTime>@maxOT then  '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else TotalOverTime end end as TotalOverTime,
						  case when DptId in('0043','0075') and StayTime>@maxStayTime then
						 CONVERT(TIME,  DATEADD(SECOND, DATEDIFF(SECOND, 0, '09:0'+SUBSTRING(OutMin,2,1)+':'+OutSec), '2022-01-01 '+InHour+':'+InMin+':'+InSec))
						   else 
                         case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),OutHour+':'+OutMin+':'+OutSec)) else OutHour+':'+OutMin+':'+OutSec end End as OutTime
						 ,
                         OutHour,OutMin,OutSec,
						 case when DptId in('0043','0075') and StayTime>@maxStayTime then '09:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else 
						 case when TotalOverTime>@maxOT then CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, CONVERT(TIME, DATEADD(SECOND, DATEDIFF(SECOND, '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec, '00:00:00'),TotalOverTime)), '00:00:00'),StayTime)) else StayTime end end as StayTime			 
						 ,
                        StayTime as actualStayTime,InHour,InMin,InSec,EmpId,SubString(EmpCardNo,8,15)+' ( '+EmpProximityNo+' )' as EmpCardNo,EmpName,SftName,
                        format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,
                        StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,
                        GName,MonthId,BreakStartTime,BreakEndTime,TotalDays,PaybleDays From v_tblAttendanceRecord  Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + "  Order By convert(int,DptId), CustomOrdering,Empid, ATTDate";
                //    sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, StayTime )-convert(varchar(8),OtherOverTime,114)),'hh:mm:ss') ) else   StayTime end as StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime as TotalOverTime,TotalDays,OtherOverTime,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, OutHour+':'+OutMin+':'+OutSec )-convert(varchar(8),OtherOverTime,114)) +convert(datetime,'00:00:'+OutSec ),'HH:mm:ss')) else   OutHour+':'+OutMin+':'+OutSec end as OutTime From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime,TotalDays,OtherOverTime order by  ATTDate  ", dt);
                //else sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, StayTime )-convert(varchar(8),OtherOverTime,114)),'hh:mm:ss') ) else   StayTime end as StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime as TotalOverTime,TotalDays,OtherOverTime,case when (OtherOverTime<>'00:00:00') then ( FORMAT(( convert(datetime, OutHour+':'+OutMin+':'+OutSec )-convert(varchar(8),OtherOverTime,114))+convert(datetime,'00:00:'+OutSec ),'HH:mm:ss') ) else   OutHour+':'+OutMin+':'+OutSec end as OutTime From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + " Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime,TotalDays,GId,CustomOrdering ,OtherOverTime Order By convert(int,DptId), CustomOrdering,Empid, ATTDate   ", dt);
                sqlDB.fillDataTable(sql, dt);
                Session["__dtJobCard__"] = dt;
                if (dt.Rows.Count > 0)
                {
                    DataTable dtSummary = new DataTable();
                    if (rblGenerateType.SelectedValue == "1")
                        sql = "Select EmpId,SUM(CASE WHEN StateStatus = 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'Casual Leave' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'Sick Leave' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'Maternity Leave' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'Annual Leave' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus = 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum(PaybleDays) AS 'APday' From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' group by EmpId";
                    else
                        sql = "Select EmpId,SUM(CASE WHEN StateStatus = 'Absent' THEN 1 ELSE 0 END) AS 'Absent',SUM(CASE WHEN StateStatus = 'Casual Leave' THEN 1 ELSE 0 END) AS 'CL',SUM(CASE WHEN StateStatus = 'Sick Leave' THEN 1 ELSE 0 END) AS 'SL',SUM(CASE WHEN StateStatus = 'Maternity Leave' THEN 1 ELSE 0 END) AS 'ML',SUM(CASE WHEN StateStatus = 'Annual Leave' THEN 1 ELSE 0 END) AS 'EL',SUM(CASE WHEN StateStatus = 'Holiday' THEN 1 ELSE 0 END) AS 'Holiday',SUM(CASE WHEN StateStatus = 'Present' THEN 1 ELSE 0 END) AS 'Present',SUM(CASE WHEN StateStatus = 'Weekend' THEN 1 ELSE 0 END) AS 'Weekend',Sum(PaybleDays) AS 'APday' From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' " + EmpTypeID + " and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " group by EmpId";
                    sqlDB.fillDataTable(sql, dtSummary);
                    Session["__dtSummary__"] = dtSummary;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=JobCardReportNew');", true);  //Open New Tab for Sever side code         
                }
                else
                {
                    lblMessage.InnerText = "warning->No Attendance Available";
                }
            }
            catch { }
        }
        private void GenerateHolidayAndWeekendReport()
        {


            try
            {

                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId= " + rblEmpType.SelectedValue + "";
                string DepartmentList = "";
                if (rblGenerateType.SelectedValue == "0")
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);

                string[] Month = ddlMonthList.SelectedValue.Split('-');

                DataTable dt = new DataTable();
                if (rblGenerateType.SelectedValue == "1") sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime, TotalOverTime,TotalDays From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and (ATTStatus ='W' or ATTStatus='H') Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,TotalOverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,OverTime,TotalDays order by  ATTDate  ", dt);
                else sqlDB.fillDataTable("Select EmpId,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,SftName,format(ATTDate,'dd-MM-yyyy') as ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,Convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,TotalOverTime,TotalDays From v_tblAttendanceRecord Where CompanyId='" + ddlCompanyName.SelectedValue + "' and MonthName='" + Month[1] + "-" + Month[0] + "' and DptId " + DepartmentList + " " + EmpTypeID + "  and (ATTStatus ='W' or ATTStatus='H') Group By EmpId,EmpCardNo,EmpName,SftName,ATTDate,DptName,DsgName,MonthName,InHour,InMin,OutHour,OutMin,ATTStatus,StayTime,OverTime,DptId,StateStatus,EmpJoiningDate,GrdName,EmpType,InSec,OutSec,LateTime,OverTimeCheck,CompanyName,Address,GName,MonthId,BreakStartTime,BreakEndTime,TotalOverTime,TotalDays,GId,CustomOrdering  Order By convert(int,DptId),CustomOrdering,Empid, ATTDate  ", dt);
                Session["__dtWHStatus__"] = dt;
                if (dt.Rows.Count > 0)
                {                   
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=HolidayAndWeekendStatus');", true);  //Open New Tab for Sever side code         
                }
                else
                {
                    lblMessage.InnerText = "warning->No Attendance Available";
                }
            }
            catch { }
        }

      
       

    

  

        

    }
}