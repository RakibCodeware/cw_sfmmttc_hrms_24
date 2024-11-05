using adviitRuntimeScripting;
using ComplexScriptingSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SigmaERP.classes
{
    public class mCommon_Module_For_AttendanceProcessing
    {
        // this method for retrive current employee list by applied conditions
        public static DataTable loadRunningEmployee(bool ForAllEmployee,string CompanyId, string DepartmentId, string EmpCardNo)
        {
            try
            {
                DataTable dtEmpInfo = new DataTable();
                if (ForAllEmployee)
                {
                    if (DepartmentId == "0")
                        sqlDB.fillDataTable("select EmpId,Convert(int,Right(EmpCardNo,LEN(EmpCardNo)-7)) as EmpCardNo,EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,RealProximityNo,GId,DptId,DsgId,EmpDutyType from v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' and EmpStatus in ('1','8') ", dtEmpInfo);
                    else
                        sqlDB.fillDataTable("select EmpId,Convert(int,Right(EmpCardNo,LEN(EmpCardNo)-7)) as EmpCardNo,EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,RealProximityNo,GId,DptId,DsgId,EmpDutyType from v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' and DptId='" + DepartmentId + "' AND EmpStatus in ('1','8') ", dtEmpInfo);
                }
                else
                    sqlDB.fillDataTable("select EmpId,Convert(int,Right(EmpCardNo,LEN(EmpCardNo)-7)) as EmpCardNo,EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,RealProximityNo,GId,DptId,DsgId,EmpDutyType from v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' and EmpCardNo Like '%" + EmpCardNo + "%' AND EmpStatus in ('1','8') ", dtEmpInfo);
                return dtEmpInfo;
            }
            catch { return null; }
        }

        public static DataTable RMSA_loadRunningEmployee(string attDate, bool ForAllEmployee, string CompnayId, string DepartmentId, string EmpCardNo)
        {
            try
            {
                string sqlCmd = "";
                DataTable dtEmpInfo = new DataTable();
                if (ForAllEmployee)
                {
                    if (DepartmentId == "0")
                        sqlCmd = "select cs.EmpId,Convert(int,Right(cs.EmpCardNo,LEN(cs.EmpCardNo)-7)) as EmpCardNo,cs.EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,EmpAttCard as RealProximityNo,GId,DptId,DsgId,EmpDutyType,EmpAttCard,isnull(IsDelivery,0) IsDelivery,WeekendType from v_Personnel_EmpCurrentStatus cs left join Personnel_EmpSeparation sp on cs.EmpId=sp.EmpId and cs.EmpStatus=sp.SeparationType where  cs.IsActive=1 and  CompanyId='" + CompnayId + "' and ( EmpStatus in ('1','8') or sp.EffectiveDate>='" + attDate + "' )  AND EmpAttCard !=''";                    
                    else
                        sqlCmd = "select cs.EmpId,Convert(int,Right(cs.EmpCardNo,LEN(cs.EmpCardNo)-7)) as EmpCardNo,cs.EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,EmpAttCard as RealProximityNo,GId,DptId,DsgId,EmpDutyType,EmpAttCard,isnull(IsDelivery,0) IsDelivery,WeekendType from v_Personnel_EmpCurrentStatus cs left join Personnel_EmpSeparation sp on cs.EmpId=sp.EmpId and cs.EmpStatus=sp.SeparationType where  cs.IsActive=1 and CompanyId='" + CompnayId + "' and DptId='" + DepartmentId + "' AND ( EmpStatus in ('1','8') or sp.EffectiveDate>='" + attDate + "' ) AND EmpAttCard !=''";                  
                }
                else
                    sqlCmd = "select cs.EmpId,Convert(int,Right(cs.EmpCardNo,LEN(cs.EmpCardNo)-7)) as EmpCardNo,cs.EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,EmpAttCard as RealProximityNo,GId,DptId,DsgId,EmpDutyType,EmpAttCard,isnull(IsDelivery,0) IsDelivery,WeekendType from v_Personnel_EmpCurrentStatus cs left join Personnel_EmpSeparation sp on cs.EmpId=sp.EmpId and cs.EmpStatus=sp.SeparationType  where  cs.IsActive=1 and CompanyId='" + CompnayId + "' and cs.EmpCardNo Like '%" + EmpCardNo + "%' AND(EmpStatus in ('1', '8') or sp.EffectiveDate >= '" + attDate + "') AND EmpAttCard != ''";
                sqlDB.fillDataTable(sqlCmd, dtEmpInfo);
                return dtEmpInfo;
            }
            catch { return null; }
        }

        // this method for delete current or exeisting attendance of selected date 
        public static void delete_Attendance(string CompanyId, string DepartmentId, string AttDate, bool ForAllEmployee, string EmpId)
        {
            try
            {
                SqlCommand cmd;
                if (!ForAllEmployee) // for all employee.that can be for all employee or just one selected employee
                    cmd = new SqlCommand("delete from tblAttendanceRecord where CompanyId='" + CompanyId + "' and AttDate='" + AttDate + "' AND EmpId='" + EmpId + "' AND AttManual is null ", sqlDB.connection);
                else if (DepartmentId.Equals("0"))
                    cmd = new SqlCommand("delete from tblAttendanceRecord where CompanyId='" + CompanyId + "' and AttDate='" + AttDate + "' AND AttManual is null ", sqlDB.connection);
                else  // for specific one employee
                    cmd = new SqlCommand("delete from tblAttendanceRecord where CompanyId='" + CompanyId + "' and AttDate='" + AttDate + "' AND DptId='" + DepartmentId + "' AND AttManual is null ", sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        // for load all attendance data after attendance processing 
        public static DataTable Load_Process_AttendanceData(string CompanyId, string DepartmentId, string AttDate, bool ForAllEmployee, string EmpId)
        {
            try
            {
                DataTable dt = new DataTable();
                if (!ForAllEmployee)
                {
                    string query = "select Right(ecs.EmpCardNo,Len(ecs.EmpCardNo)-7) as EmpCardNo,ecs.EmpName,ecs.DptName,ecs.DsgName, case when atd.ODID >0 then atd.ATTStatus+'(OD)' else atd.ATTStatus end as ATTStatus, " +
                   "   atd.InHour+':'+atd.InMin+':'+atd.InSec as Intime,atd.OutHour+':'+atd.OutMin+':'+atd.OutSec as Outtime,Format(ATTDate,'dd-MMM-yyyy') as ATTDate " +
                   " from " +
                   " tblAttendanceRecord as atd inner join " +
                   " v_Personnel_EmpCurrentStatus ecs on " +
                   " atd.EmpId=ecs.EmpId and ecs.IsActive=1 AND atd.ATTDate='" + AttDate + "' and atd.CompanyId='" + CompanyId + "' AND ecs.EmpCardNo  Like '%" + EmpId + "%'";
                    sqlDB.fillDataTable(query, dt);
                }
                else if (DepartmentId.Equals("0"))
                    sqlDB.fillDataTable("select Right(ecs.EmpCardNo,Len(ecs.EmpCardNo)-7) as EmpCardNo,ecs.EmpName,ecs.DptName,ecs.DsgName, case when atd.ODID >0 then atd.ATTStatus+'(OD)' else atd.ATTStatus end as ATTStatus, " +
                        "  atd.InHour+':'+atd.InMin+':'+atd.InSec as Intime,atd.OutHour+':'+atd.OutMin+':'+atd.OutSec as Outtime,Format(ATTDate,'dd-MMM-yyyy') as ATTDate " +
                        " from " +
                        " tblAttendanceRecord as atd inner join " +
                        " v_Personnel_EmpCurrentStatus ecs on " +
                        " atd.EmpId=ecs.EmpId and ecs.IsActive=1 and atd.CompanyId='" + CompanyId + "' AND atd.ATTDate='" + AttDate + "' order by ecs.DptId,ecs.CustomOrdering", dt);
                else
                    sqlDB.fillDataTable("select Right(ecs.EmpCardNo,Len(ecs.EmpCardNo)-7) as EmpCardNo,ecs.EmpName,ecs.DptName,ecs.DsgName, case when atd.ODID >0 then atd.ATTStatus+'(OD)' else atd.ATTStatus end as ATTStatus, " +
                    "  atd.InHour+':'+atd.InMin+':'+atd.InSec as Intime,atd.OutHour+':'+atd.OutMin+':'+atd.OutSec as Outtime,Format(ATTDate,'dd-MMM-yyyy') as ATTDate " +
                    " from " +
                    " tblAttendanceRecord as atd inner join " +
                    " v_Personnel_EmpCurrentStatus ecs on " +
                    " atd.EmpId=ecs.EmpId and ecs.IsActive=1 AND atd.ATTDate='" + AttDate + "' and atd.CompanyId='" + CompanyId + "' AND atd.DptId='" + DepartmentId + "' order by ecs.CustomOrdering", dt);
                return dt;

            }
            catch { return null; }
        }

        public static DataTable Load_Process_AttendanceData(string AttDate)
        {
            try
            {
                DataTable dt = new DataTable();
                
                sqlDB.fillDataTable("select Right(ecs.EmpCardNo,Len(ecs.EmpCardNo)-7) as EmpCardNo,ecs.EmpName,ecs.DptName,ecs.DsgName, atd.ATTStatus, " +
                "   atd.InHour+':'+atd.InMin+':'+atd.InSec as Intime,atd.OutHour+':'+atd.OutMin+':'+atd.OutSec as Outtime,Format(ATTDate,'dd-MMM-yyyy') as ATTDate " +
                " from " +
                " tblAttendanceRecord as atd inner join " +
                " v_Personnel_EmpCurrentStatus ecs on " +
                " atd.EmpId=ecs.EmpId AND atd.ATTDate='" + AttDate + "' AND AttManual ='mc'", dt);                            
                return dt;

            }
            catch { return null; }
        }

        public static void NotCountableAttendanceLog(string EmpId,string Reason,string AttDate)
        {

            try {
                if (!Reason.Contains("PK_tblAttendanceRecord_1"))
                {
                    if (Reason != "Rostering Problem")
                        Reason = "";
                  //  string msg = Reason.Replace('\'', ' ');
                    string sqlCmd = "insert into tblAttendance_NotCountableLogRecord values('" + EmpId + "','" + Reason + "','" + AttDate + "')";
                    SqlCommand cmd = new SqlCommand(sqlCmd, sqlDB.connection);
                    cmd.ExecuteNonQuery();
                }
                
            }
            catch (Exception ex)
            {}

            
        }

        public static void LoadAttendanceMissingLog(GridView GridView)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select pecs.EmpCardNo,pecs.EmpName,pecs.DptName,pecs.DsgName,alog.AttDate,alog.Reason  from tblAttendance_NotCountableLogRecord alog " +
                                    " inner join "+
                                    " v_Personnel_EmpCurrentStatus as pecs on "+
                                    " alog.EmpId=pecs.EmpId and pecs.IsActive=1", dt);
                GridView.DataSource = dt;
                GridView.DataBind();
            }
            catch { }
        }

        public static string [] LoadShift_Information(string EmpId,bool IsRegularDuty_Type,string ShiftId,string AttDate)
        {
            try
            {
                HttpContext.Current.Session["__IsRegular__"] = IsRegularDuty_Type.ToString();
                DataTable dt;
                string[] Gt_RosterInfo = new string[10];
                string sqlCMD = "";
                if (IsRegularDuty_Type)
                {
                    sqlCMD= "select "+ ShiftId + " as  SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,IsNight  from HRD_SpecialTimetable where StartDate <='"+ AttDate + "' and EndDate>='"+ AttDate + "'";
                    sqlDB.fillDataTable(sqlCMD, dt = new DataTable());
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        sqlCMD = "select SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,IsNight  from HRD_Shift where SftId ='" + ShiftId + "'";
                        sqlDB.fillDataTable(sqlCMD, dt = new DataTable());
                    }                   
                }
                else
                    sqlDB.fillDataTable("select SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,IsWeekend,IsNight  from v_ShiftTransferInfoDetails where SDate ='" + AttDate + "' AND EmpId='" + EmpId + "'", dt = new DataTable());
                Gt_RosterInfo[0] = (dt.Rows.Count > 0) ? dt.Rows[0]["SftId"].ToString() : "0";
                Gt_RosterInfo[1] = dt.Rows[0]["SftStartTimeIndicator"].ToString();
                Gt_RosterInfo[2] = dt.Rows[0]["SftEndTimeIndicator"].ToString();
                Gt_RosterInfo[3] = dt.Rows[0]["SftStartTime"].ToString();
                Gt_RosterInfo[4] = dt.Rows[0]["SftEndTime"].ToString();
                Gt_RosterInfo[5] = dt.Rows[0]["AcceptableTimeAsOT"].ToString();
                Gt_RosterInfo[6] = dt.Rows[0]["SftAcceptableLate"].ToString();
                Gt_RosterInfo[7] = dt.Rows[0]["StartPunchCountTime"].ToString();
                Gt_RosterInfo[8] = (IsRegularDuty_Type) ? "False" : dt.Rows[0]["IsWeekend"].ToString();
                Gt_RosterInfo[9] = dt.Rows[0]["IsNight"].ToString();
                return Gt_RosterInfo;
            }
            catch { return null; }
        }

        public static string [] Counting_BreakTime(string AttDate, string EmpCardNo, string BreakStratTime,string BreakEndTime)
        {
            string[] CountBreakTime = { "00:00:00", "00:00:00" };
            try
            {               
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select distinct Badgenumber, FORMAT(CHECKTIME,'HH:mm:ss') as PunchTime from v_CHECKINOUT where CONVERT(date,CHECKTIME)='" + AttDate + "' AND FORMAT(CHECKTIME,'HH:mm:ss')>='"+BreakStratTime+ "' AND FORMAT(CHECKTIME,'HH:mm:ss')<='" + BreakEndTime + "' AND Badgenumber Like '%" + EmpCardNo + "'  order by PunchTime", dt);
                if (dt.Rows.Count>0)
                {
                    if ((Convert.ToDateTime(dt.Rows[0]["PunchTime"].ToString()).Hour== Convert.ToDateTime(dt.Rows[dt.Rows.Count-1]["PunchTime"].ToString()).Hour) &&
                        (Convert.ToDateTime(dt.Rows[0]["PunchTime"].ToString()).Minute == Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()).Minute))
                    {
                        CountBreakTime[0] = dt.Rows[0]["PunchTime"].ToString();
                        CountBreakTime[1] = "00:00:00";
                    }
                    else
                    {
                        CountBreakTime[0] = dt.Rows[0]["PunchTime"].ToString();
                        CountBreakTime[1] = dt.Rows[dt.Rows.Count-1]["PunchTime"].ToString();
                    }                 
                }
                return CountBreakTime;
            }
            catch { return CountBreakTime ; }
        }
    }
}