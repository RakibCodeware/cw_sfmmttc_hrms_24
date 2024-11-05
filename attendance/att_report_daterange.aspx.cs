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

namespace SigmaERP.attendance
{
    public partial class att_report_daterange : System.Web.UI.Page
    {
        DataTable dt;
        string CompanyId = "";
        string SqlQuery = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompany.Enabled = false;
                ddlCompany.SelectedValue = ViewState["__CompanyId__"].ToString();
                Session["__MinDigits__"] = "6";
                txtDate.Text = txtToDate.Text = DateTime.Now.ToString("dd-MM-yyyy");

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

                //------------load privilege setting inof from db------
                //------------load privilege setting inof from db------
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "daily_movement.aspx", ddlCompany, WarningMessage, tblGenerateType, btnPreview);
                ViewState["__ReadAction__"] = AccessPermission[0];
                classes.commonTask.LoadShiftNameByCompany(ViewState["__CompanyId__"].ToString(), ddlShift);
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
               
                //-----------------------------------------------------





            }
            catch { }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (rblReportType.SelectedValue == "0")
                GenerateReportInEnglish();
            //else if (rblReportType.SelectedValue == "1")
            //    GenerateSummaryReport();
            else if (rblReportType.SelectedValue == "1")
                AttendanceSummaryReport();
            else if (rblReportType.SelectedValue == "2")
                GenerateReportIndividualInEnglish();
            else if (rblReportType.SelectedValue == "3")
                AbsentReport();

        }

        private void GenerateOutTimeMissingReport()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            if (classes.commonTask.IsWeekendORHoliday(y + "-" + m + "-" + d))
            {
                if (rblAttStatus.SelectedValue == "P")

                    AttStatus = " and InHour<>'00' ";
                else if (rblAttStatus.SelectedValue == "A")
                    AttStatus = " and InHour='00'  ";
                else if (rblAttStatus.SelectedValue == "Lv")
                    AttStatus = " and AttStatus='Lv'  ";
                else
                    AttStatus = "";
            }

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";



            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            if (txtCardNo.Text.Trim().Length == 0) sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,ATTStatus,CompanyId,DptId,SftId,GId,GName,StayTime From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and CompanyId " + CompanyList + "  ANd InHour<>'00' and OutHour='00'  AND DptId " + DepartmentList + " " + EmpTypeID + " " + AttStatus + " " + ShiftName + "  order by convert(int,DptCode),convert(int,GId), convert(int,SftId),CustomOrdering ", dt = new DataTable());
            else
            {
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,ATTStatus,CompanyId,DptId,SftId,GId,GName,StayTime From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and InHour<>'00' and OutHour='00' And ActiveSalary='True' and IsActive=1 and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and CompanyId " + CompanyList + " " + AttStatus + " ", dt = new DataTable());
            }

            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyMovement__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovement-" + txtDate.Text + "-" + rblPrintType.SelectedValue + "- Daily Out Time Missing Report');", true);  //Open New Tab for Sever side code

        }
        private void GenerateReportInEnglish()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please select any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string[] Tdmy = txtToDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            if (classes.commonTask.IsWeekendORHoliday(y + "-" + m + "-" + d))
            {
                if (rblAttStatus.SelectedValue == "P")

                    AttStatus = " and InHour<>'00' ";
                else if (rblAttStatus.SelectedValue == "A")
                    AttStatus = " and InHour='00'  ";
                else if (rblAttStatus.SelectedValue == "Lv")
                    AttStatus = " and AttStatus='Lv'  ";
                else
                    AttStatus = "";
            }

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and a.EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";
            string OrderBy = (rblOrderBy.SelectedValue == "0") ? " order by convert(int, SubString(e.EmpCardNo,8,15)), ATTDate" : " order by ATTDate,convert(int, SubString(e.EmpCardNo,8,15)) ";


            string dbName = Glory.getDBName();

            if (dbName == "cw_marico" || dbName == "cw_marico3")// marico mouchak & mirshorai
            {
                if (txtCardNo.Text.Trim().Length == 0)
                {
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                    SqlQuery = @"Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,Address,ATTStatus,a.CompanyId,a.DptId,a.SftId,a.GId,StayTime,a.TotalOverTime,
0 as Deduct,
Round(case when a.TotalOverTime<'00:30:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 45 then .5 else 1 end end) end,1) as OtherOverTime,
s.SftName,d.DptName From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Shift s on a.SftId=s.SftId left join HRD_Department d on d.DptId=a.DptId where  (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND a.DptId " + DepartmentList + " " + EmpTypeID + OrderBy;
                }
                else
                {
                    if (txtCardNo.Text.Trim().Length < 4)
                    {
                        lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum 4 Digits)";
                        txtCardNo.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }
                    SqlQuery = @"Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,Address,ATTStatus,a.CompanyId,a.DptId,a.SftId,a.GId,StayTime,a.TotalOverTime,
0 as Deduct,
Round(case when a.TotalOverTime<'00:30:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 45 then .5 else 1 end end) end,1) as OtherOverTime,
s.SftName,d.DptName From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Shift s on a.SftId=s.SftId left join HRD_Department d on d.DptId=a.DptId where (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND ( e.EmpCardNo like'%" + txtCardNo.Text.Trim() + "' or e.RealProximityNo=" + txtCardNo.Text.Trim() + ")" + EmpTypeID + OrderBy;
                }
            }
            else
            { 
            if (txtCardNo.Text.Trim().Length == 0)
            {
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                SqlQuery = @"Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,Address,ATTStatus,a.CompanyId,a.DptId,a.SftId,a.GId,StayTime,a.TotalOverTime,a.TotalOverTimePre,
case when SftName<>'G' and ATTStatus='P' and a.StayTime>'10:30:00' then -1 else 0 end as Deduct,
case when SftName<>'G' and ATTStatus='P' and a.StayTime>'10:30:00' then
Round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 55 then .5 else 1 end end) end,0) +case when a.TotalOverTime<'00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 55 then .5 else 1 end end) end,1)-1 
else 
Round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 55 then .5 else 1 end end) end,0) +case when a.TotalOverTime<'00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 55 then .5 else 1 end end) end,1) 
end as OtherOverTime,
s.SftName,d.DptName From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Shift s on a.SftId=s.SftId left join HRD_Department d on d.DptId=a.DptId where  (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND a.DptId " + DepartmentList + " " + EmpTypeID + OrderBy;
            }                         
            else
            {
                if (txtCardNo.Text.Trim().Length < 4)
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum 4 Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                SqlQuery = @"Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,Address,ATTStatus,a.CompanyId,a.DptId,a.SftId,a.GId,StayTime,a.TotalOverTime,a.TotalOverTimePre,
case when SftName<>'G' and ATTStatus='P' and a.StayTime>'10:30:00' then -1 else 0 end as Deduct,
case when SftName<>'G' and ATTStatus='P' and a.StayTime>'10:30:00' then
Round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 55 then .5 else 1 end end) end,0) +case when a.TotalOverTime<'00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 55 then .5 else 1 end end) end,1)-1 
else 
Round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre)))< 55 then .5 else 1 end end) end,0) +case when a.TotalOverTime<'00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 30 then 0 else case when DATEPART(minute, convert(datetime,'2023-01-01 ' + convert(varchar(8), a.TotalOverTime)))< 55 then .5 else 1 end end) end,1) 
end as OtherOverTime,
s.SftName,d.DptName From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Shift s on a.SftId=s.SftId left join HRD_Department d on d.DptId=a.DptId where (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND ( e.EmpCardNo like'%"+txtCardNo.Text.Trim()+ "' or e.RealProximityNo=" + txtCardNo.Text.Trim() + ")" + EmpTypeID + OrderBy;
            }
            }
            dt = new DataTable();
            dt=CRUD.ExecuteReturnDataTable(SqlQuery);
            if ( dt==null ||dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyMovementByDateRange__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovementByDateRange-" + (txtDate.Text + " to " + txtToDate.Text).Replace('-', '/') + "-" + rblPrintType.SelectedValue + "- Daily Attendance Report');", true);  //Open New Tab for Sever side code

        }

        private void AttendanceSummaryReport()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please select any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string[] Tdmy = txtToDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            if (classes.commonTask.IsWeekendORHoliday(y + "-" + m + "-" + d))
            {
                if (rblAttStatus.SelectedValue == "P")

                    AttStatus = " and InHour<>'00' ";
                else if (rblAttStatus.SelectedValue == "A")
                    AttStatus = " and InHour='00'  ";
                else if (rblAttStatus.SelectedValue == "Lv")
                    AttStatus = " and AttStatus='Lv'  ";
                else
                    AttStatus = "";
            }

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and a.EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";
            string OrderBy = " order by convert(int, SubString(e.EmpCardNo,8,15))";

            string dbName = Glory.getDBName();
            if (dbName == "cw_marico" || dbName == "cw_marico3")// Marico Mouchak
            {
                if (txtCardNo.Text.Trim().Length == 0)
                {
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                    SqlQuery = @"Select SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName,
sum(round( case when a.ATTStatus = 'W' or a.ATTStatus='H' then  case when a.TotalOverTime < '00:30:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 45 then .5 else 1 end end) end else 0 end, 1) ) as TotalOverTime,

sum(   

round(case when a.TotalOverTime < '00:30:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 45 then .5 else 1 end end) end, 1) ) as OtherOverTime, 

sum(case when a.ATTStatus = 'P' then 1 else 0 end) as ATTStatus, sum(case when a.ATTStatus = 'W' or a.ATTStatus = 'H' then 1 else 0 end) as TotalDays  From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Department dpt on a.DptId=dpt.DptId where  (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND a.DptId " + DepartmentList + " " + EmpTypeID + " group by SubString(e.EmpCardNo,8,15) ,e.EmpProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName " + OrderBy;
                }
                else
                {
                    if (txtCardNo.Text.Trim().Length < 4)
                    {
                        lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum 4 Digits)";
                        txtCardNo.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }
                    SqlQuery = @"Select SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName,
sum(round( case when a.ATTStatus = 'W' or a.ATTStatus='H' then  case when a.TotalOverTime < '00:30:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 45 then .5 else 1 end end) end else 0 end, 1) ) as TotalOverTime,

sum(   

round(case when a.TotalOverTime < '00:30:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 45 then .5 else 1 end end) end, 1) ) as OtherOverTime, 

sum(case when a.ATTStatus = 'P' then 1 else 0 end) as ATTStatus, sum(case when a.ATTStatus = 'W' or a.ATTStatus = 'H' then 1 else 0 end) as TotalDays From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Department dpt on a.DptId=dpt.DptId where (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND ( e.EmpCardNo like'%" + txtCardNo.Text.Trim() + "' or e.RealProximityNo=" + txtCardNo.Text.Trim() + ") group by SubString(e.EmpCardNo,8,15) ,e.EmpProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName " + OrderBy;
                }

            }
            else
            {
                if (txtCardNo.Text.Trim().Length == 0)
                {
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                    SqlQuery = @"Select SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName,
sum(round( case when a.ATTStatus = 'W' or a.ATTStatus='H' then  case when a.TotalOverTime < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 55 then .5 else 1 end end) end else 0 end, 1) ) as TotalOverTime,

sum( case when a.SftId<>5 and ATTStatus='P' and a.StayTime>'10:30:00' then  round(case when a.TotalOverTime < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 55 then .5 else 1 end end) end, 1) + round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 55 then .5 else 1 end end) end, 0),1)-1 else 

round(case when a.TotalOverTime < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 55 then .5 else 1 end end) end, 1) + round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 55 then .5 else 1 end end) end, 0),1) end ) as OtherOverTime, 

sum(case when a.ATTStatus = 'P' then 1 else 0 end) as ATTStatus, sum(case when a.ATTStatus = 'W' or a.ATTStatus = 'H' then 1 else 0 end) as TotalDays  From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Department dpt on a.DptId=dpt.DptId where  (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND a.DptId " + DepartmentList + " " + EmpTypeID + " group by SubString(e.EmpCardNo,8,15) ,e.EmpProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName " + OrderBy;
                }
                else
                {
                    if (txtCardNo.Text.Trim().Length < 4)
                    {
                        lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum 4 Digits)";
                        txtCardNo.Focus();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                        return;
                    }
                    SqlQuery = @"Select SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName,
sum(round( case when a.ATTStatus = 'W' or a.ATTStatus='H' then  case when a.TotalOverTime < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 55 then .5 else 1 end end) end else 0 end, 1) ) as TotalOverTime,

sum( case when a.SftId<>5 and ATTStatus='P' and a.StayTime>'10:30:00' then  round(case when a.TotalOverTime < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 55 then .5 else 1 end end) end, 1) + round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 55 then .5 else 1 end end) end, 0),1)-1 else 

round(case when a.TotalOverTime < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTime))) < 55 then .5 else 1 end end) end, 1) + round(IsNull(case when a.TotalOverTimePre < '00:55:00' then 0 else DATEPART(HOUR, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) + (case when DATEPART(MINUTE, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 30 then 0 else case when DATEPART(minute, convert(datetime, '2023-01-01 ' + convert(varchar(8), a.TotalOverTimePre))) < 55 then .5 else 1 end end) end, 0),1) end ) as OtherOverTime, 

sum(case when a.ATTStatus = 'P' then 1 else 0 end) as ATTStatus, sum(case when a.ATTStatus = 'W' or a.ATTStatus = 'H' then 1 else 0 end) as TotalDays  From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Department dpt on a.DptId=dpt.DptId where (InHour<>'00' or InMin<>'00' or InSec<>'00') and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND ( e.EmpCardNo like'%" + txtCardNo.Text.Trim() + "' or e.RealProximityNo=" + txtCardNo.Text.Trim() + ") group by SubString(e.EmpCardNo,8,15) ,e.EmpProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName " + OrderBy;
                }

            }

            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable(SqlQuery);
            if (dt == null || dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__AttendanceSummaryByDateRange__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovementByDateRange-" + (txtDate.Text + " to " + txtToDate.Text).Replace('-', '/') + "-" + rblPrintType.SelectedValue + "-Attendance Summary Report');", true);  //Open New Tab for Sever side code

        }
        private void AbsentReport()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please select any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string[] Tdmy = txtToDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            if (classes.commonTask.IsWeekendORHoliday(y + "-" + m + "-" + d))
            {
                if (rblAttStatus.SelectedValue == "P")

                    AttStatus = " and InHour<>'00' ";
                else if (rblAttStatus.SelectedValue == "A")
                    AttStatus = " and InHour='00'  ";
                else if (rblAttStatus.SelectedValue == "Lv")
                    AttStatus = " and AttStatus='Lv'  ";
                else
                    AttStatus = "";
            }

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and a.EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";
            string OrderBy = (rblOrderBy.SelectedValue == "0") ? " order by convert(int, SubString(e.EmpCardNo,8,15)), ATTDate" : " order by ATTDate,convert(int, SubString(e.EmpCardNo,8,15)) ";



            if (txtCardNo.Text.Trim().Length == 0)
            {
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                SqlQuery = "Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,dpt.DptName,CompanyName,Address,a.CompanyId,a.DptId, sum( case when InHour='00' and InMin='00' and InSec='00' then 0 else 1 end),0 as GID  From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Department dpt on a.DptId=dpt.DptId where  ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND a.DptId " + DepartmentList + " " + EmpTypeID + " group by  Format(ATTDate,'dd-MM-yyyy') ,SubString(e.EmpCardNo,8,15) ,e.EmpProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName having sum( case when InHour='00' and InMin='00' and InSec='00' then 0 else 1 end)=0 " + OrderBy;
            }
            else
            {
                if (txtCardNo.Text.Trim().Length < 4)
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum 4 Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                SqlQuery = "Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(e.EmpCardNo,8,15) as EmpCardNo,e.EmpProximityNo as RealProximityNo,EmpName,dpt.DptName,CompanyName,Address,a.CompanyId,a.DptId, sum( case when InHour='00' and InMin='00' and InSec='00' then 0 else 1 end),0 as GID  From tblAttendanceRecord a left join Personnel_EmployeeInfo e on a.EmpId=e.EmpId  left join HRD_CompanyInfo c on a.CompanyId=c.CompanyId left join HRD_Department dpt on a.DptId=dpt.DptId where ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and a.CompanyId " + CompanyList + "   AND (e.EmpCardNo like'%" + txtCardNo.Text.Trim() + "' or e.RealProximityNo=" + txtCardNo.Text.Trim() + ") group by  Format(ATTDate,'dd-MM-yyyy') ,SubString(e.EmpCardNo,8,15) ,e.EmpProximityNo,EmpName,CompanyName,Address,a.CompanyId,a.DptId,dpt.DptName having sum( case when InHour='00' and InMin='00' and InSec='00' then 0 else 1 end)=0 " + OrderBy;
            }
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable(SqlQuery);
            if (dt == null || dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Absent Record Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyAbsentByDateRange__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovementByDateRange-" + (txtDate.Text + " to " + txtToDate.Text).Replace('-', '/') + "-" + rblPrintType.SelectedValue + "- Daily Absent Report');", true);  //Open New Tab for Sever side code

        }
        private void GenerateReportIndividualInEnglish()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please select any employee!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string[] Tdmy = txtToDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            if (classes.commonTask.IsWeekendORHoliday(y + "-" + m + "-" + d))
            {
                if (rblAttStatus.SelectedValue == "P")

                    AttStatus = " and InHour<>'00' ";
                else if (rblAttStatus.SelectedValue == "A")
                    AttStatus = " and InHour='00'  ";
                else if (rblAttStatus.SelectedValue == "Lv")
                    AttStatus = " and AttStatus='Lv'  ";
                else
                    AttStatus = "";
            }

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";



            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            if (txtCardNo.Text.Trim().Length == 0) sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,ATTStatus,CompanyId,DptId,SftId,GId,GName,StayTime,EmpId From v_tblAttendanceRecord where ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "' and ActiveSalary='True' and IsActive=1 and CompanyId " + CompanyList + "   AND EmpID " + DepartmentList + " " + EmpTypeID + " " + AttStatus + " " + ShiftName + "  order by convert(int,DptCode),convert(int,GId), convert(int,SftId),CustomOrdering ", dt = new DataTable());
            else
            {
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,8,15) as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,ATTStatus,CompanyId,DptId,SftId,GId,GName,StayTime,EmpId From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and CompanyId " + CompanyList + " " + AttStatus + " ", dt = new DataTable());
            }

            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyMovementByDateRangeIndividual__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovementByDateRangeIndividual-" + (txtDate.Text + " to " + txtToDate.Text).Replace('-', '/') + "-" + rblPrintType.SelectedValue + "- Daily Attendance Report');", true);  //Open New Tab for Sever side code

        }

        private void GenerateSummaryReport()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please select any employee!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string[] Tdmy = txtToDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";


            CompanyList = "in ('" + CompanyId + "')";



            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            sqlDB.fillDataTable(@"with temp as(
select   EmpId, SUBSTRING(EmpCardNo, 8, 6) as EmpCardNo, EmpName, DptName, DsgName, CompanyName, SftName, Address, CompanyId, DptId, SftId, GId, GName, count(EmpId) TotalDays, sum((DATEPART(HOUR, StayTime) * 3600) + (DATEPART(MINUTE, StayTime) * 60) + DATEPART(SECOND, StayTime)) as total,
 sum((DATEPART(HOUR, StayTime) * 3600) + (DATEPART(MINUTE, StayTime) * 60) + DATEPART(SECOND, StayTime)) / count(EmpId) as avg from v_tblAttendanceRecord 
where IsActive = 1 and OutDuty<>1 and StateStatus = 'Present' and StayTime <> '' and ATTDate>='" + y + "-" + m + "-" + d + "' and ATTDate<='" + Tdmy[2] + "-" + Tdmy[1] + "-" + Tdmy[0] + "'  and IsActive=1 and CompanyId " + CompanyList + "   AND EmpID " + DepartmentList + " " + EmpTypeID + " " + ShiftName + "   group by EmpId, SUBSTRING(EmpCardNo, 8, 6), EmpName, DptName, DsgName, CompanyName, SftName, Address, CompanyId, DptId, SftId, GId, GName) " +
    " select EmpId, EmpCardNo, EmpName, DptName, DsgName, CompanyName, SftName, Address, CompanyId, DptId, SftId, GId, GName, TotalDays,RIGHT('0' + CAST(total / 3600 AS VARCHAR), 2) +':' +RIGHT('0' + CAST((total / 60) % 60 AS VARCHAR), 2) + ':' +RIGHT('0' + CAST(total % 60 AS VARCHAR), 2) as StayTime,RIGHT('0' + CAST(avg / 3600 AS VARCHAR), 2) + ':' +RIGHT('0' + CAST((avg / 60) % 60 AS VARCHAR), 2) + ':' +RIGHT('0' + CAST(avg % 60 AS VARCHAR), 2) as OutTime from temp order by convert(int, EmpCardNo)  ", dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Data not found.";
                return;
            }
            Session["__AttendanceSummaryByDateRange__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=AttendanceSummaryByDateRange-" + (txtDate.Text + " to " + txtToDate.Text).Replace('-', '/') + "-" + rblPrintType.SelectedValue + "-Attendance Summary Report');", true);  //Open New Tab for Sever side code

        }
        private void GenerateReportInBangla()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();
            string[] dmy = txtDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];

            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";

            //if (ddlShiftList.SelectedItem.ToString().Equals("All"))
            //ShiftList = classes.commonTask.getShiftList(ddlShiftList);          
            //else
            //ShiftList = "in ('" + ddlShiftList.SelectedValue.ToString() + "')";

            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            if (txtCardNo.Text.Trim().Length == 0) sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,8,15) as EmpCardNo,EmpNameBn EmpName,DsgNameBn DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyNameBangla CompanyName,DptNameBn DptName,SftNameBangla SftName,AddressBangla Address,ATTStatus,CompanyId,DptId,SftId,GId,GName,StayTime From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and CompanyId " + CompanyList + " " + ShiftName + "  AND DptId " + DepartmentList + " " + EmpTypeID + " " + AttStatus + " order by convert(int,DptCode),convert(int,GId), convert(int,SftId),CustomOrdering ", dt = new DataTable());
            else
            {
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,8,15) as EmpCardNo,EmpNameBn EmpName,DsgNameBn DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyNameBangla CompanyName,DptNameBn DptName,SftNameBangla SftName,AddressBangla Address,ATTStatus,CompanyId,DptId,SftId,GId,GName,StayTime From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and CompanyId " + CompanyList + " " + AttStatus + " ", dt = new DataTable());
            }

            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyMovementBangla__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovementBangla-" + txtDate.Text + "-" + rblPrintType.SelectedValue + "');", true);  //Open New Tab for Sever side code

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

        private void addAllTextInShift()
        {
            //    if (ddlShiftList.Items.Count > 2)
            //        ddlShiftList.Items.Insert(1, new ListItem("All", "00"));
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);

            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            classes.commonTask.LoadShiftNameByCompany(CompanyId, ddlShift);
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
            lstSelected.Items.Clear();
            //classes.commonTask.LoadShift(ddlShiftList, CompanyId , ViewState["__UserType__"].ToString()); 
            // classes.commonTask.LoadShiftByNumber(ddlShiftList, CompanyId, rblShiftNumber.SelectedValue);
            // addAllTextInShift();

        }


    }
}