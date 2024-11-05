using adviitRuntimeScripting;
using ComplexScriptingSystem;
using HRD.ModelEntities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SigmaERP.classes
{
    public class AttendanceCommonTasks
    {
        DataTable dt;
        string query;
        public DataTable getEmployees(string attDate, bool ForAllEmployee, string CompnayId, string DepartmentId, string EmpCardNo, string EmpType)
        {
            try
            {
                string sqlCmd = "";
                EmpType = EmpType == "All" ? "" : " and cs.EmpTypeId=" + EmpType;
                if (ForAllEmployee)
                {
                    if (DepartmentId == "0")
                        sqlCmd = "select cs.EmpId,Convert(int,Right(cs.EmpCardNo,LEN(cs.EmpCardNo)-7)) as EmpCardNo,cs.EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,EmpAttCard as RealProximityNo,GId,DptId,DsgId,EmpDutyType,EmpAttCard,isnull(IsDelivery,0) IsDelivery,WeekendType from v_Personnel_EmpCurrentStatus cs left join Personnel_EmpSeparation sp on cs.EmpId=sp.EmpId and cs.EmpStatus=sp.SeparationType where  cs.IsActive=1 and  CompanyId='" + CompnayId + "' and ( EmpStatus in ('1','8') or sp.EffectiveDate>='" + attDate + "' )  AND EmpAttCard !=''" + EmpType;
                    else
                        sqlCmd = "select cs.EmpId,Convert(int,Right(cs.EmpCardNo,LEN(cs.EmpCardNo)-7)) as EmpCardNo,cs.EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,EmpAttCard as RealProximityNo,GId,DptId,DsgId,EmpDutyType,EmpAttCard,isnull(IsDelivery,0) IsDelivery,WeekendType from v_Personnel_EmpCurrentStatus cs left join Personnel_EmpSeparation sp on cs.EmpId=sp.EmpId and cs.EmpStatus=sp.SeparationType where  cs.IsActive=1 and CompanyId='" + CompnayId + "' and DptId='" + DepartmentId + "' AND ( EmpStatus in ('1','8') or sp.EffectiveDate>='" + attDate + "' ) AND EmpAttCard !=''" + EmpType;
                }
                else
                    sqlCmd = "select cs.EmpId,Convert(int,Right(cs.EmpCardNo,LEN(cs.EmpCardNo)-7)) as EmpCardNo,cs.EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,SftId,EmpAttCard as RealProximityNo,GId,DptId,DsgId,EmpDutyType,EmpAttCard,isnull(IsDelivery,0) IsDelivery,WeekendType from v_Personnel_EmpCurrentStatus cs left join Personnel_EmpSeparation sp on cs.EmpId=sp.EmpId and cs.EmpStatus=sp.SeparationType  where  cs.IsActive=1 and CompanyId='" + CompnayId + "' and  (cs.EmpCardNo Like '%"+ EmpCardNo + "' or EmpAttCard='"+ EmpCardNo + "') AND(EmpStatus in ('1', '8') or sp.EffectiveDate >= '" + attDate + "') AND EmpAttCard != ''";

                return CRUD.ExecuteReturnDataTable(sqlCmd);
            }
            catch { return null; }
        }

        public string fileUpload(FileUpload FileUploader, string CompanyId)
        {
            try
            {
                string filename = "UNIS.mdb";
                if (FileUploader.HasFile == true)
                {
                    filename = Path.GetFileName(FileUploader.FileName);
                    File.Delete(HttpContext.Current.Server.MapPath("~/AccessFile/" + CompanyId + "") + filename);
                    FileUploader.SaveAs(HttpContext.Current.Server.MapPath("~/AccessFile/" + CompanyId + "") + filename);
                }
                return filename;
            }
            catch (Exception ex)
            {
                return "";
            }


        }
        public bool importRawDataFormDevice(string filename, string CompanyId, bool ForAllEmployee, string EmpId, string RealProximityNo, DateTime SelectedDate, string ProcessingID,string db)
        {
            try
            {
                return true;

                string _ProxymityNo = "";
                string table = (db == "access") ? " tEnter" : " UNIS.dbo.tEnter";
                string query = "";
                if (ForAllEmployee)
                {
                    query = "select L_UID as card_no,C_Time as PanchTime,C_Date as PanchDate from "+ table + " where C_Date = '" + SelectedDate.ToString("yyyyMMdd") + "' or C_Date = '" + SelectedDate.AddDays(1).ToString("yyyyMMdd") + "' ";
                }
                else
                {
                    _ProxymityNo = GetEmpProximityNo(EmpId, SelectedDate.ToString("yyyy-MM-dd"));
                    _ProxymityNo = (_ProxymityNo == "") ? RealProximityNo : _ProxymityNo;
                    query = "select L_UID as card_no,C_Time as PanchTime,C_Date as PanchDate from " + table + " where (C_Date = '" + SelectedDate.ToString("yyyyMMdd") + "' or C_Date = '" + SelectedDate.AddDays(1).ToString("yyyyMMdd") + "') AND L_UID =" + _ProxymityNo + "";
                }
                if (db == "access")
                {
                    OleDbConnection cont = new OleDbConnection();
                    string getFilePaht = HttpContext.Current.Server.MapPath("//AccessFile//" + CompanyId + "" + filename);
                    string connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + getFilePaht + ";Jet OLEDB:Database Password=unisamho";
                    cont.ConnectionString = connection;
                    cont.Open();
                    OleDbDataAdapter da;
                    da = new OleDbDataAdapter(query, cont);  // here selecteddate format =yyyyMMdd
                    dt = new DataTable();
                    da.Fill(dt);
                    cont.Close();
                }
                else // sql
                {
                    dt = new DataTable();
                    dt = CRUD.ExecuteReturnDataTable(query);
                }
                //--------------------------------------------- End -----------------------------------------------------------------------------------------

                //----------------------------------------------- entered punch data into tblAttendance table------------------------------------------------
                foreach (DataRow dr in dt.Rows)
                {
                    string PanchTime = dr["PanchDate"].ToString().Substring(0, 4) + "-" + dr["PanchDate"].ToString().Substring(4, 2) + "-" + dr["PanchDate"].ToString().Substring(6, 2) + " " + dr["PanchTime"].ToString().Substring(0, 2) + ":" + dr["PanchTime"].ToString().Substring(2, 2) + ":" + dr["PanchTime"].ToString().Substring(4, 2);
                    SaveAttendancePunch(ProcessingID, CompanyId, dr["card_no"].ToString(), DateTime.Parse(PanchTime));

                }
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public void DeleteTempRawData(string ProcessingID)
        {
            CRUD.Execute("delete tblAttendancePunch_temp where ProcessingID='" + ProcessingID + "'");
        }
        public string GetEmpProximityNo(string EmpId, string date)
        {
            try
            {

                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select EmpProximityNo from Personnel_EmpProximityChange_Log  where EmpId='" + EmpId + "' and FromDate<='" + date + "' and ToDate>='" + date + "'");
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["EmpProximityNo"].ToString();
                }
                else
                    return "";
            }
            catch { return ""; }
        }

        private bool SaveAttendancePunch(string ProcessingID, string CompanyID, string CardNo, DateTime PunchTime)
        {
            try {
                CRUD.Execute("insert into tblAttendancePunch_temp(CompanyID,CardNo,PunchTime,ProcessingID) " +
                        " values " +
                        "('" + CompanyID + "','" + CardNo + "','" + PunchTime.ToString("yyyy-MM-dd HH:mm:ss") + "','" + ProcessingID + "')");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteAttendance(string CompanyId, string DepartmentId, string AttDate, bool ForAllEmployee, string EmpId, string EmpType)
        {
            try
            {
                EmpType = EmpType == "All" ? "" : " and EmpTypeId=" + EmpType;
                if (!ForAllEmployee) // for all employee.that can be for all employee or just one selected employee
                    query = "delete from tblAttendanceRecord where CompanyId='" + CompanyId + "' and AttDate='" + AttDate + "' AND EmpId='" + EmpId + "' AND AttManual is null ";
                else if (DepartmentId.Equals("0"))
                    query = "delete from tblAttendanceRecord where CompanyId='" + CompanyId + "' and AttDate='" + AttDate + "' AND AttManual is null " + EmpType;
                else  // for specific one employee
                    query = "delete from tblAttendanceRecord where CompanyId='" + CompanyId + "' and AttDate='" + AttDate + "' AND DptId='" + DepartmentId + "' AND AttManual is null " + EmpType;

                return CRUD.Execute(query);

            }
            catch { return false; }
        }
        public string[] CheckHolidayWeekend(string CompanyId, string SelectedDate)
        {
            try
            {
                string[] DayStatus = new string[2];
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select HCode from tblHolydayWork where HDate='" + SelectedDate + "' and CompanyId='" + CompanyId + "'");
                if (dt.Rows.Count > 0)
                {
                    DayStatus[0] = "True";
                    DayStatus[1] = "H";
                    return DayStatus;
                }
                else
                {
                    dt = new DataTable();
                    dt = CRUD.ExecuteReturnDataTable("select SL from Attendance_WeekendInfo where WeekendDate='" + SelectedDate + "' and CompanyId='" + CompanyId + "'");
                    if (dt.Rows.Count > 0)
                    {
                        DayStatus[0] = "True";
                        DayStatus[1] = "W";

                    }
                    else
                        DayStatus[0] = "False";
                    return DayStatus;
                }
            }
            catch { return null; }
        }
        public string[] CheckLeave(string SelectedDate, string EmpId)
        {
            try
            {
                dt = new DataTable();
                string[] Leave_Info = new string[2];
                dt = CRUD.ExecuteReturnDataTable("select LACode,LeaveName from v_Leave_LeaveApplicationDetails where IsApproved=1 and LeaveDate='" + SelectedDate + "' AND EmpId='" + EmpId + "'");
                if (dt.Rows.Count > 0)
                {
                    Leave_Info[0] = dt.Rows[0]["LACode"].ToString();
                    Leave_Info[1] = dt.Rows[0]["LeaveName"].ToString();
                }
                else Leave_Info[0] = "0";
                return Leave_Info;
            }
            catch { return null; }
        }
        public string GetGeneralDayInfo(string CompanyId, string SelectedDate)
        {
            try
            {
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select SL,GeneralDay,Description,CompanyId,EmpTypeId from tblGeneralDays  Where CompanyId='" + CompanyId + "' and GeneralDay='" + SelectedDate + "'");
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0]["EmpTypeId"].ToString();
                return "";

            }
            catch { return ""; }
        }
        public string[] GetRosterInfo(string SelectedDate, string EmpId, string DutyType, string ShiftId,string EmpTypeId)
        {
            try
            {

                DataTable dt;
                string[] Gt_RosterInfo = new string[14];
                if (DutyType == "Regular")
                {
                    dt = new DataTable();
                    //dt = CRUD.ExecuteReturnDataTable("select " + ShiftId + " as SftId, SftOverTime,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,EndPunchCountTime,format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime,IsNight  from HRD_SpecialTimetable where StartDate<='" + SelectedDate + "' and EndDate>='" + SelectedDate + "'");
                    dt = CRUD.ExecuteReturnDataTable("select SL, " + ShiftId + " as SftId,SftStartTime,SftEndTime,SftAcceptableLate,StartPunchCountTime,EndPunchCountTime  from HRD_SpecialTimetable where EmpTypeId="+ EmpTypeId + " and  StartDate<='" + SelectedDate + "' and EndDate>='" + SelectedDate + "'");
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        dt = new DataTable();
                        dt = CRUD.ExecuteReturnDataTable("select SftOverTime,SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,EndPunchCountTime,format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime,IsNight  from HRD_Shift where SftId ='" + ShiftId + "'");
                    }
                    else
                    {
                        Gt_RosterInfo[10] = dt.Rows[0]["SL"].ToString();// Special Timetable Id.
                    }

                }
                else
                {
                    dt = new DataTable();
                    dt = CRUD.ExecuteReturnDataTable("select SftOverTime,SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,EndPunchCountTime,IsWeekend,Format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime,IsNight  from v_ShiftTransferInfoDetails where SDate ='" + SelectedDate + "' AND EmpId='" + EmpId + "'");
                }
                Gt_RosterInfo[0] = (dt.Rows.Count > 0) ? dt.Rows[0]["SftId"].ToString() : "0";


                TimeSpan SftStartTime = TimeSpan.Parse(dt.Rows[0]["SftStartTime"].ToString());
                TimeSpan SftEndTime = TimeSpan.Parse(dt.Rows[0]["SftEndTime"].ToString());
                Gt_RosterInfo[1] = SelectedDate + " " + dt.Rows[0]["SftStartTime"].ToString();
                if (SftStartTime > SftEndTime)
                    Gt_RosterInfo[2] = DateTime.Parse(SelectedDate).AddDays(1).ToString("yyyy-MM-dd") + " " + dt.Rows[0]["SftEndTime"].ToString();
                else
                    Gt_RosterInfo[2] = SelectedDate + " " + dt.Rows[0]["SftEndTime"].ToString();

                TimeSpan StartPunchCountTime = TimeSpan.Parse(dt.Rows[0]["StartPunchCountTime"].ToString());
                TimeSpan EndPunchCountTime = TimeSpan.Parse(dt.Rows[0]["EndPunchCountTime"].ToString());
                Gt_RosterInfo[3] = SelectedDate + " " + dt.Rows[0]["StartPunchCountTime"].ToString();
                if (StartPunchCountTime > EndPunchCountTime)
                    Gt_RosterInfo[4] = DateTime.Parse(SelectedDate).AddDays(1).ToString("yyyy-MM-dd") + " " + dt.Rows[0]["EndPunchCountTime"].ToString();
                else
                    Gt_RosterInfo[4] = SelectedDate + " " + dt.Rows[0]["EndPunchCountTime"].ToString();
                try {  } catch (Exception ex) { Gt_RosterInfo[5] = "0"; }
                
                Gt_RosterInfo[6] = dt.Rows[0]["SftAcceptableLate"].ToString();
                if (DutyType == "Regular")
                {
                    Gt_RosterInfo[5] = "0";
                    Gt_RosterInfo[8] = "False";
                    Gt_RosterInfo[9] = "0";
                }
                else
                {
                    Gt_RosterInfo[5] = dt.Rows[0]["AcceptableTimeAsOT"].ToString();
                    Gt_RosterInfo[8] =  dt.Rows[0]["IsWeekend"].ToString();
                    Gt_RosterInfo[9] = dt.Rows[0]["IsNight"].ToString();

                }              
                return Gt_RosterInfo;
            }
            catch { return null; }
        }
        public void NotCountableAttendanceLog(string EmpId, string Reason, string AttDate)
        {

            try
            {
                if (!Reason.Contains("PK_tblAttendanceRecord_1"))
                {
                    if (Reason != "Rostering Problem")
                        Reason = "";
                    CRUD.Execute("insert into tblAttendance_NotCountableLogRecord values('" + EmpId + "','" + Reason + "','" + AttDate + "')");
                }

            }
            catch (Exception ex)
            { }


        }
        public DataTable GetPunch(string ProcessingID, string DeviceType, string CompanyID, string CardNo, DateTime ShiftPunchCountStartTime, DateTime ShiftPunchCountEndTime)
        {
            try
            {
                dt = new DataTable();             
                if(DeviceType== "zkbiotime")
                    query = "select  emp_code as CardNo,FORMAT(punch_time,'yyyy-MM-dd HH:mm:ss') as PunchTime from zkbiotime.dbo.iclock_transaction where punch_time >='" + ShiftPunchCountStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and punch_time <='" + ShiftPunchCountEndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'  and emp_code='" + CardNo + "'  order by  FORMAT(punch_time,'yyyy-MM-dd HH:mm:ss')";
                else // default att2000 [Old zk]
                    query = "select  distinct u.BADGENUMBER as CardNo,format(c.CHECKTIME,'yyyy-MM-dd HH:mm:ss') as PunchTime from cw_att_zk.dbo.CHECKINOUT c inner join cw_att_zk.dbo.USERINFO u on c.USERID=u.USERID where c.CHECKTIME>='" + ShiftPunchCountStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and c.CHECKTIME<='" + ShiftPunchCountEndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'  AND u.BADGENUMBER='" + CardNo + "' order by format(c.CHECKTIME,'yyyy-MM-dd HH:mm:ss')";

                return CRUD.ExecuteReturnDataTable(query);
            }
            catch (Exception ex) { return null; }
        }
        public DataTable GetPunchWithTimetable(string BADGENUMBER,DateTime BeginningIn, DateTime EndingIn, DateTime BeginningOut, DateTime EndingOut,string DeviceType)
        {
            try
            {

                if (BeginningIn > EndingIn)
                    EndingIn = EndingIn.AddDays(1);
                if (BeginningIn > BeginningOut)
                    BeginningOut = BeginningOut.AddDays(1);
                if (BeginningIn > EndingOut)
                    EndingOut = EndingOut.AddDays(1);

                if(DeviceType== "zk_biotime")
                    query = "select top(1) emp_code as ProximityNo,FORMAT(punch_time,'yyyy-MM-dd HH:mm:ss') as CHECKTIME, format(punch_time,'HH') Hour, format(punch_time,'mm') Minute, format(punch_time,'ss') Second,format(punch_time,'HH:mm:ss') PunchTime, 'In' as PunchType from zkbiotime.dbo.iclock_transaction where punch_time >='" + BeginningIn.ToString("yyyy-MM-dd HH:mm:ss") + "' and punch_time <='" + EndingIn.ToString("yyyy-MM-dd HH:mm:ss") + "'  and emp_code='" + BADGENUMBER + "' and (terminal_sn in(" + HttpContext.Current.Session["__InMachines__"].ToString() + ") or (isnull(terminal_sn,'')='' and punch_state=0)) order by punch_time, format(punch_time,'HH') , format(punch_time,'mm') , format(punch_time,'ss') ";
                else
                    query = "select top(1) BADGENUMBER as ProximityNo,FORMAT(CHECKTIME,'yyyy-MM-dd HH:mm:ss') as CHECKTIME, format(CHECKTIME,'HH') Hour, format(CHECKTIME,'mm') Minute, format(CHECKTIME,'ss') Second,format(CHECKTIME,'HH:mm:ss') PunchTime, 'In' as PunchType from att2000.dbo.v_CHECKINOUT where CHECKTIME >='" + BeginningIn.ToString("yyyy-MM-dd HH:mm:ss") + "' and CHECKTIME <='" + EndingIn.ToString("yyyy-MM-dd HH:mm:ss") + "'  and BADGENUMBER='" + BADGENUMBER + "' and (sn in(" + HttpContext.Current.Session["__InMachines__"] .ToString()+ ") or (sn is null and SENSORID=1)) order by CHECKTIME, format(CHECKTIME,'HH') , format(CHECKTIME,'mm') , format(CHECKTIME,'ss') ";
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(query);
                if (DeviceType == "zk_biotime")
                    query = "select emp_code as ProximityNo,FORMAT(punch_time,'yyyy-MM-dd HH:mm:ss') as CHECKTIME, format(punch_time,'HH') Hour, format(punch_time,'mm') Minute, format(punch_time,'ss') Second,format(punch_time,'HH:mm:ss') PunchTime, 'Out' as PunchType from zkbiotime.dbo.iclock_transaction where punch_time >='" + BeginningOut.ToString("yyyy-MM-dd HH:mm:ss") + "' and punch_time <='" + EndingOut.ToString("yyyy-MM-dd HH:mm:ss") + "'  and emp_code='" + BADGENUMBER + "' and (terminal_sn in(" + HttpContext.Current.Session["__OutMachines__"].ToString() + ") or (isnull(terminal_sn,'')='' and punch_state=1)) order by punch_time, format(punch_time,'HH'),format(punch_time,'mm') , format(punch_time,'ss') ";
                else
                    query = "select BADGENUMBER as ProximityNo,FORMAT(CHECKTIME,'yyyy-MM-dd HH:mm:ss') as CHECKTIME, format(CHECKTIME,'HH') Hour, format(CHECKTIME,'mm') Minute, format(CHECKTIME,'ss') Second,format(CHECKTIME,'HH:mm:ss') PunchTime, 'Out' as PunchType from att2000.dbo.v_CHECKINOUT where CHECKTIME >='" + BeginningOut.ToString("yyyy-MM-dd HH:mm:ss") + "' and CHECKTIME <='" + EndingOut.ToString("yyyy-MM-dd HH:mm:ss") + "'  and BADGENUMBER='" + BADGENUMBER + "' and (sn in(" + HttpContext.Current.Session["__OutMachines__"].ToString() + ") or (sn is null and SENSORID=2)) order by CHECKTIME, format(CHECKTIME,'HH'),format(CHECKTIME,'mm') , format(CHECKTIME,'ss') ";
                DataTable dtOutPunch = new DataTable();
                    dtOutPunch = CRUD.ExecuteReturnDataTable(query);
                    if (dtOutPunch != null && dtOutPunch.Rows.Count > 0)
                    {
                        int i = dtOutPunch.Rows.Count - 1;
                        dt.Rows.Add(dtOutPunch.Rows[i]["ProximityNo"].ToString(), dtOutPunch.Rows[i]["CHECKTIME"].ToString(), dtOutPunch.Rows[i]["Hour"].ToString(), dtOutPunch.Rows[i]["Minute"].ToString(), dtOutPunch.Rows[i]["Second"].ToString(), dtOutPunch.Rows[i]["PunchTime"].ToString(), dtOutPunch.Rows[i]["PunchType"].ToString());
                    }                
                return dt;
            }
            catch (Exception ex) { return null; }
        }

        public DataTable GetPunchWithoutShift(string BADGENUMBER, string AttDate)
        {
            try
            {

                query = "select top(1) BADGENUMBER as ProximityNo,FORMAT(CHECKTIME,'yyyy-MM-dd HH:mm:ss') as CHECKTIME, format(CHECKTIME,'HH') Hour, format(CHECKTIME,'mm') Minute, format(CHECKTIME,'ss') Second,format(CHECKTIME,'HH:mm:ss') PunchTime from att2000.dbo.v_CHECKINOUT where format( CHECKTIME,'yyyy-MM-dd')='" + AttDate + "'  and BADGENUMBER='" + BADGENUMBER + "' and sn in(" + HttpContext.Current.Session["__InMachines__"].ToString() + ") order by CHECKTIME, format(CHECKTIME,'HH') , format(CHECKTIME,'mm') , format(CHECKTIME,'ss') ";
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(query);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string PuchCountEndTime = DateTime.Parse(dt.Rows[0]["CHECKTIME"].ToString()).AddHours(AttendanceStaticInfo.DurationHours).ToString("yyyy-MM-dd HH:mm:ss");
                    query = "select  BADGENUMBER as ProximityNo,FORMAT(CHECKTIME,'yyyy-MM-dd HH:mm:ss') as CHECKTIME, format(CHECKTIME,'HH') Hour, format(CHECKTIME,'mm') Minute, format(CHECKTIME,'ss') Second,format(CHECKTIME,'HH:mm:ss') PunchTime from att2000.dbo.v_CHECKINOUT where CHECKTIME>='" + dt.Rows[0]["CHECKTIME"].ToString() + "' and CHECKTIME<='" + PuchCountEndTime + "' and BADGENUMBER='" + BADGENUMBER + "' and sn in(" + HttpContext.Current.Session["__OutMachines__"].ToString() + ") order by CHECKTIME, format(CHECKTIME,'HH') , format(CHECKTIME,'mm') , format(CHECKTIME,'ss') ";
                    DataTable dtOutPunch = new DataTable();
                    dtOutPunch = CRUD.ExecuteReturnDataTable(query);
                    if (dtOutPunch != null && dtOutPunch.Rows.Count > 0)
                    {
                        int i = dtOutPunch.Rows.Count - 1;
                        dt.Rows.Add(dtOutPunch.Rows[i]["ProximityNo"].ToString(), dtOutPunch.Rows[i]["CHECKTIME"].ToString(), dtOutPunch.Rows[i]["Hour"].ToString(), dtOutPunch.Rows[i]["Minute"].ToString(), dtOutPunch.Rows[i]["Second"].ToString(), dtOutPunch.Rows[i]["PunchTime"].ToString());
                    }

                }
                return dt;
            }
            catch (Exception ex) { return null; }
        }

        public AttendanceRecord GetAttStatusWithoutShift(AttendanceRecord _attRecord, DateTime LogInTime, DateTime LogOutTime, TimeSpan WorkingTime)
        {
            
            if (_attRecord.AttStatus == "A")
            {
                _attRecord.InHour = LogInTime.ToString("HH");
                _attRecord.InMin = LogInTime.ToString("mm");
                _attRecord.InSec = LogInTime.ToString("ss");

                _attRecord.AttStatus = "P";
                _attRecord.StateStatus = "Present";
                if (LogOutTime > LogInTime)
                {
                    _attRecord.OutHour = LogOutTime.ToString("HH");
                    _attRecord.OutMin = LogOutTime.ToString("mm");
                    _attRecord.OutSec = LogOutTime.ToString("ss");

                    TimeSpan stayTime, totalOTTime = TimeSpan.Parse("00:00:00");
                    stayTime = LogOutTime - LogInTime;
                    _attRecord.StayTime = stayTime.ToString();
                    if (stayTime > WorkingTime)
                    {
                        totalOTTime = stayTime - WorkingTime;
                        _attRecord.OverTime = totalOTTime.ToString();
                        _attRecord.TotalOverTime = totalOTTime.ToString();
                    }
                                      
                   
                }                
            }
            return _attRecord;
        }
        public AttendanceRecord GetAttStatusWithCommonShift(AttendanceRecord _attRecord, DateTime? LogInTime, DateTime? LogOutTime, DateTime SftStartTime,DateTime SftEndTime, TimeSpan WorkingTime,bool IsFullTimeAsOT)
        {

            //if (_attRecord.AttStatus == "A")
            //{
                
                if (LogInTime != null)
                {
                    _attRecord.InHour = LogInTime?.ToString("HH");
                    _attRecord.InMin = LogInTime?.ToString("mm");
                    _attRecord.InSec = LogInTime?.ToString("ss");
                }
                if (LogOutTime !=null)
                {
                    _attRecord.OutHour = LogOutTime?.ToString("HH");
                    _attRecord.OutMin = LogOutTime?.ToString("mm");
                    _attRecord.OutSec = LogOutTime?.ToString("ss");
                }
                
                if (LogInTime != null && LogOutTime != null && LogOutTime > LogInTime)
                {
                    TimeSpan stayTime = TimeSpan.Parse("00:00:00"), totalOTTime = TimeSpan.Parse("00:00:00"), totalOTTimePre = TimeSpan.Parse("00:00:00");                   
                        stayTime = LogOutTime - LogInTime?? stayTime;
                    _attRecord.StayTime = stayTime.ToString();
                    if (SftStartTime > SftEndTime)
                        SftEndTime = SftEndTime.AddDays(1);
                   WorkingTime = SftEndTime - SftStartTime;
                if (IsFullTimeAsOT)
                    {
                        //if (SftStartTime < LogOutTime)
                        //{
                        //    if (SftStartTime < LogInTime)
                        //        totalOTTime = stayTime;
                        //    else
                        //        totalOTTime = LogOutTime-SftStartTime?? totalOTTime;
                        //}
                        totalOTTime= stayTime; 
                       
                    }
                    else {
                    // New code , Date: 17-04-2023
                    if (stayTime > WorkingTime)
                    {
                        if (SftEndTime < LogOutTime)
                        {
                            if (SftStartTime < LogInTime)
                            {
                               totalOTTime = stayTime - WorkingTime;
                            }
                            else
                                totalOTTime = LogOutTime - SftEndTime ?? totalOTTime;
                        }
                        if (_attRecord.DbName == "cw_marico2" && LogInTime < SftStartTime)
                        {
                            if(SftEndTime < LogOutTime)
                              totalOTTimePre = SftStartTime - LogInTime ?? totalOTTimePre;
                            else
                                totalOTTimePre = stayTime - WorkingTime;
                        }
                    }

                    // End New code , Date: 17-04-2023
                    // preview code , Date: 17-04-2023
                    //if (SftEndTime < LogOutTime)
                    //{

                    //    if (SftStartTime < LogInTime)
                    //    {
                    //        if (stayTime > WorkingTime)
                    //            totalOTTime = stayTime - WorkingTime;
                    //    }                                
                    //    else
                    //        totalOTTime = LogOutTime - SftEndTime ?? totalOTTime;

                    //}
                    //if (_attRecord.DbName == "cw_marico2" && LogInTime < SftStartTime && stayTime > WorkingTime)
                    //{
                    //    totalOTTimePre = SftStartTime - LogInTime ?? totalOTTimePre;
                    //}
                    // End preview code , Date: 17-04-2023
                }
                _attRecord.OverTime = totalOTTime.ToString();
                    _attRecord.TotalOverTime = totalOTTime.ToString();
                    _attRecord.TotalOverTimePre = totalOTTimePre.ToString();



                }
            //}
            return _attRecord;
        }
        public AttendanceRecord GetAttStatus(AttendanceRecord _attRecord, DateTime LogInTime, DateTime LogOutTime,string [] rosterInfo, TimeSpan MinWorkingTime, TimeSpan MinOverTime, bool OnePunchPresent, string DutyType)
            {
            DateTime RosterStartTime = DateTime.Parse(rosterInfo[1]); 
            DateTime RosterEndTime = DateTime.Parse(rosterInfo[2]);
            byte AcceptableLate= byte.Parse(rosterInfo[6]);

                DateTime RosterStartTimeForOT = RosterEndTime;
                _attRecord.InHour = LogInTime.ToString("HH");
                _attRecord.InMin = LogInTime.ToString("mm");
                _attRecord.InSec = LogInTime.ToString("ss");
                if (_attRecord.AttStatus == "A")
                {
                if (OnePunchPresent)
                {
                    _attRecord.AttStatus = "P";
                    _attRecord.PaybleDays = "1";
                }                        
                else
                {
                    if (LogInTime <= RosterStartTime.AddMinutes(AcceptableLate))
                        _attRecord.AttStatus = "P";
                    else
                    {
                        _attRecord.AttStatus = "L";
                        _attRecord.LateTime = (LogInTime - RosterStartTime).ToString(); // to get late time                   
                    }
                }
                _attRecord.StateStatus = "Present";
                }
                if (LogOutTime > LogInTime)
                {
                    _attRecord.OutHour = LogOutTime.ToString("HH");
                    _attRecord.OutMin = LogOutTime.ToString("mm");
                    _attRecord.OutSec = LogOutTime.ToString("ss");

                    TimeSpan stayTime, totalOTTime = TimeSpan.Parse("00:00:00");
                    stayTime = LogOutTime - LogInTime;


                    _attRecord.StayTime = stayTime.ToString();
                    if (_attRecord.AttStatus == "W" || _attRecord.AttStatus == "H")
                    {
                    //if (!IsDelivery)
                    //{
                    //    totalOTTime = stayTime;
                    //    totalOTTime = TimeSpan.Parse(NetOverTimeAfterDeductedBreaks(_attRecord.EmpId, _attRecord.AttStatus, rosterInfo, _attRecord.AttDate.ToString("yyyy-MM-dd"), TimeSpan.Parse(_attRecord.InHour + ":" + _attRecord.InMin + ":" + _attRecord.InSec), TimeSpan.Parse(_attRecord.OutHour + ":" + _attRecord.OutMin + ":" + _attRecord.OutSec), totalOTTime, DutyType));
                    //    if (totalOTTime >= TimeSpan.Parse("10:00:00"))
                    //    {
                    //        _attRecord.TiffinCount = "1";
                    //        _attRecord.NightAllowCount = "1";
                    //    }
                    //    else if (totalOTTime >= TimeSpan.Parse("07:00:00"))
                    //    {
                    //        _attRecord.NightAllowCount = "1";
                    //    }
                    //}
                    //if (stayTime >= MinWorkingTime)
                    //    _attRecord.HolidayCount = "1";//Payble Day

                    if (LogOutTime>RosterStartTime)
                    {
                        totalOTTime = LogOutTime- RosterStartTime;
                        totalOTTime = TimeSpan.Parse(NetOverTimeAfterDeductedBreaks(_attRecord.EmpId, _attRecord.AttStatus, rosterInfo, _attRecord.AttDate.ToString("yyyy-MM-dd"), TimeSpan.Parse(_attRecord.InHour + ":" + _attRecord.InMin + ":" + _attRecord.InSec), TimeSpan.Parse(_attRecord.OutHour + ":" + _attRecord.OutMin + ":" + _attRecord.OutSec), totalOTTime, DutyType));
                        
                    }                 
                }
                    else// P or L 
                    {
                        if (OnePunchPresent)
                        {
                        
                        
                        if (stayTime > AttendanceStaticInfo.dutyTimeForDelivery)
                            {
                                totalOTTime = stayTime - AttendanceStaticInfo.dutyTimeForDelivery;
                                if (totalOTTime > TimeSpan.Parse("03:00:00"))// maxmimum over time 3 hours
                                    totalOTTime = TimeSpan.Parse("03:00:00");
                            }

                        }
                        else
                        {
                        //if (RosterStartTimeForOT < LogOutTime)
                        //{
                        //    totalOTTime = LogOutTime - RosterStartTimeForOT;

                        //    totalOTTime = TimeSpan.Parse(NetOverTimeAfterDeductedBreaks(_attRecord.EmpId, _attRecord.AttStatus, rosterInfo, _attRecord.AttDate.ToString("yyyy-MM-dd"), TimeSpan.Parse(_attRecord.InHour + ":" + _attRecord.InMin + ":" + _attRecord.InSec), TimeSpan.Parse(_attRecord.OutHour + ":" + _attRecord.OutMin + ":" + _attRecord.OutSec), totalOTTime, DutyType));

                        //    if (totalOTTime >= TimeSpan.Parse("06:00:00"))
                        //    {
                        //        _attRecord.TiffinCount = "1";
                        //        _attRecord.NightAllowCount = "1";
                        //    }
                        //    else if (totalOTTime >= TimeSpan.Parse("02:00:00"))
                        //    {
                        //        _attRecord.TiffinCount = "1";
                        //    }

                        //}                        

                        if (stayTime > TimeSpan.Parse("09:00:00"))
                        {                          
                            if (LogInTime > RosterStartTime)
                            {
                                RosterStartTimeForOT = LogInTime+ TimeSpan.Parse("09:00:00");
                            }
                            if (RosterStartTimeForOT < LogOutTime)
                            {
                                totalOTTime = LogOutTime - RosterStartTimeForOT;
                                totalOTTime = TimeSpan.Parse(NetOverTimeAfterDeductedBreaks(_attRecord.EmpId, _attRecord.AttStatus, rosterInfo, _attRecord.AttDate.ToString("yyyy-MM-dd"), TimeSpan.Parse(_attRecord.InHour + ":" + _attRecord.InMin + ":" + _attRecord.InSec), TimeSpan.Parse(_attRecord.OutHour + ":" + _attRecord.OutMin + ":" + _attRecord.OutSec), totalOTTime, DutyType));
                            }
                        }

                    }
                        if (OnePunchPresent || stayTime >= MinWorkingTime)
                            _attRecord.PaybleDays = "1";//Payble Day                    

                    }
                    if (totalOTTime > TimeSpan.Parse("00:00:00"))
                    {
                    // Secially for Mollah Fashion, Less then 15 mins not countable. 
                    if(totalOTTime.Minutes<15)
                        totalOTTime= new TimeSpan(totalOTTime.Hours, 0, 0);

                    if (totalOTTime > MinOverTime)
                        {
                            _attRecord.OverTime = MinOverTime.ToString();// over time 
                            _attRecord.OtherOverTime = (totalOTTime - MinOverTime).ToString();// extra over time
                        }
                        else
                            _attRecord.OverTime = MinOverTime.ToString();// over time

                        _attRecord.TotalOverTime = totalOTTime.ToString();//total over time 
                    }
                }

                return _attRecord;
            }
            public AttendanceRecord CheckOutDuty(AttendanceRecord _attRecord, string minimumWorkingTime, bool hasPunch, DateTime rosterStartTime, DateTime rosterEndTime, DateTime inTime, DateTime outTime)
            {
                try
                {
                    string sqlcmd = "";
                    DataTable dt;
                    sqlcmd = "select SL, StraightFromHome,StraightToHome from tblOutDuty where Status=1 and EmpId='" + _attRecord.EmpId + "' and Date='" + _attRecord.AttDate.ToString("yyyy-MM-dd") + "'";
                    sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        string ODID = dt.Rows[0]["SL"].ToString();
                        bool StraightFromHome = bool.Parse(dt.Rows[0]["StraightFromHome"].ToString());
                        bool StraightToHome = bool.Parse(dt.Rows[0]["StraightToHome"].ToString());
                        _attRecord.ODID = int.Parse(ODID);
                        if (StraightFromHome && StraightToHome)
                        {
                            _attRecord.AttStatus = "P";
                            _attRecord.StateStatus = "Present";
                            _attRecord.PaybleDays = "1";
                        }
                        else if (hasPunch)
                        {
                            if (StraightFromHome)
                            {
                                _attRecord.AttStatus = "P";
                                _attRecord.StateStatus = "Present";
                                if (outTime != null)
                                {
                                    TimeSpan duration = (outTime - rosterStartTime);
                                    if (duration >= TimeSpan.Parse(minimumWorkingTime))
                                        _attRecord.PaybleDays = "1";
                                }
                            }
                            else if (StraightToHome)
                            {
                                TimeSpan duration = (rosterEndTime - inTime);
                                if (duration >= TimeSpan.Parse(minimumWorkingTime))
                                {
                                    _attRecord.AttStatus = "P";
                                    _attRecord.StateStatus = "Present";
                                    _attRecord.PaybleDays = "1";
                                }
                            }
                        }
                    }
                    return _attRecord;
                }
                catch (Exception ex) { return null; }
            }
            public string NetOverTimeAfterDeductedBreaks(string EmpId, string attStatus, string[] RosterInfo, string attDate, TimeSpan logInTime, TimeSpan logOutTime, TimeSpan TotalOverTime, string DutyType)// this block specialy for ramdan overtime . create date: 19-05-2019
            {
                try
                {
                    DataTable dt;
                    string sqlcmd = "";
                    if (!(attStatus == "W" || attStatus == "H"))
                    {
                    //if (DutyType == "Regular")
                    //{
                    //    sqlcmd = "select SftEndTime from HRD_SpecialTimetable where StartDate<='" + attDate + "' and EndDate>='" + attDate + "'";
                    //    sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                    //    if (dt == null || dt.Rows.Count == 0)
                    //    {
                    //        sqlcmd = "select SftEndTime from HRD_Shift where SftId='" + shiftId + "'";
                    //        sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                    //    }

                    //}
                    //else
                    //{
                    //    sqlcmd = "select SftEndTime from v_ShiftTransferInfoDetails where SDate ='" + attDate + "' AND EmpId='" + EmpId + "'";
                    //    sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                    //}
                    //string SftEndTime = dt.Rows[0]["SftEndTime"].ToString();

                    logInTime = TimeSpan.Parse(RosterInfo[2].Split(' ')[1]);
                    }
                    DateTime logIn, logOut;
                    logIn = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + logInTime.ToString());
                    if (logInTime > logOutTime)
                        logOut = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + logOutTime.ToString());
                    else
                        logOut = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + logOutTime.ToString());
                    TimeSpan totalBreakTime = TimeSpan.Parse("00:00:00");
                    DateTime startTime;// = TimeSpan.Parse("00:00:00");
                    DateTime endTime;// = TimeSpan.Parse("00:00:00");
                    TimeSpan breakTime = TimeSpan.Parse("00:00:00");
                sqlcmd = "select distinct StartTime,EndTime,BreakTime,NextDay  from AttSpecialBreakTime where IsActive=1 and Date='"+attDate+"' and DutyType in('All','"+ DutyType + "')";
                sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                if (dt == null || dt.Rows.Count == 0)
                {                
                string IsHoliday = (attStatus == "W" || attStatus == "H") ? "1" : "0";
                    if (RosterInfo[10] != null)
                    {
                        sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay from AttBreakTimeWithShift abs inner join AttBreakTime ab on abs.BrkID=ab.SL where SpecialTimetableId="+ RosterInfo[10] + " order by NextDay,StartTime";
                    }                
                    sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                    if (dt == null || dt.Rows.Count == 0 || DutyType != "Regular")
                    {
                        sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay from AttBreakTimeWithShift abs inner join AttBreakTime ab on abs.BrkID=ab.SL where SftID=" + RosterInfo[0] + " order by NextDay,StartTime";
                        sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay from AttBreakTime where IsActive=1 and BreakID is null and IsHoliday=" + IsHoliday + " order by NextDay,StartTime";
                            sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                    {
                        for (byte i = 0; i < dt.Rows.Count; i++)
                        {

                            if (dt.Rows[i]["NextDay"].ToString().Equals("True"))
                            {
                                startTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + dt.Rows[i]["StartTime"].ToString());
                                endTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + dt.Rows[i]["EndTime"].ToString());
                            }
                            else
                            {
                                startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + dt.Rows[i]["StartTime"].ToString());
                                endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + dt.Rows[i]["EndTime"].ToString());
                            }

                            breakTime = TimeSpan.Parse(dt.Rows[i]["BreakTime"].ToString());
                            if (logIn <= startTime && logOut >= endTime)
                            {
                                totalBreakTime += breakTime;
                            }
                            else if (logOut > startTime && logOut < endTime)
                            {
                                totalBreakTime += logOut - startTime;
                            }

                    }

                    }
                    TotalOverTime = TotalOverTime - totalBreakTime;
                    if(TotalOverTime.ToString().Contains("-"))
                        return "00:00:00";
                    else
                        return TotalOverTime.ToString();
                }
                catch { return "00:00:00"; }

            }
            public dynamic GetOthersSetting(string CompanyId) {
                try
                {

                    dt = new DataTable();
                    dt = CRUD.ExecuteReturnDataTable("select * from HRD_OthersSetting where CompanyId='" + CompanyId + "'");
                    string[] othersetting = new string[7];
                    if (dt.Rows.Count > 0)
                    {
                        othersetting[0] = dt.Rows[0]["WorkerTiffinHour"].ToString() + ":" + dt.Rows[0]["WorkerTiffinMin"].ToString() + ":00";
                        othersetting[1] = dt.Rows[0]["StaffTiffinHour"].ToString() + ":" + dt.Rows[0]["StaffTiffinMin"].ToString() + ":00";
                        othersetting[2] = dt.Rows[0]["StaffHolidayCount"].ToString();
                        othersetting[3] = dt.Rows[0]["MinWorkingHour"].ToString() + ":" + dt.Rows[0]["MinWorkingMin"].ToString() + ":00"; //Minimum Working Time
                        othersetting[4] = dt.Rows[0]["StaffHolidayTotalHour"].ToString() + ":" + dt.Rows[0]["StaffHolidayTotalMin"].ToString() + ":00"; //Minimum Staff Working Hours For Holiday Allowance
                        othersetting[5] = dt.Rows[0]["MinOverTimeHour"].ToString() + ":" + dt.Rows[0]["MinOverTimeMin"].ToString() + ":00"; //Minimum OverTime
                        othersetting[6] = dt.Rows[0]["BreakBeforeStartOTAsMin"].ToString();
                    }
                    return othersetting;
                }
                catch (Exception ex) { return null; }


            }

            public bool SaveAttendanceRecord(AttendanceRecord _attRecord)
            {
                try
                {
                //if (_attRecord.AttStatus == "L")
                //{
                //    if (TimeSpan.Parse(_attRecord.LateTime) >= TimeSpan.Parse("03:00:00"))
                //    {
                //        _attRecord.AttStatus = "A";
                //        _attRecord.PaybleDays = "0";
                //    }                  
                //}
                string[] getColumns = { "EmpId", "AttDate", "EmpTypeId", "InHour", "InMin", "InSec", "OutHour", "OutMin", "OutSec",
                                        "AttStatus", "StateStatus", "OverTime", "SftId", "DptId","DsgId", "CompanyId", "GId","LateTime","StayTime","TiffinCount","HolidayCount","PaybleDays","OtherOverTime","TotalOverTime","UserId","NightAllowCount"};

                    string[] getValues = {_attRecord.EmpId, _attRecord.AttDate.ToString("yyyy-MM-dd"),_attRecord.EmpTypeId,_attRecord.InHour,_attRecord.InMin,_attRecord.InSec,
                    _attRecord.OutHour,_attRecord.OutMin,_attRecord.OutSec,_attRecord.AttStatus,
                                                 _attRecord.StateStatus,_attRecord.OverTime,_attRecord.SftId,_attRecord.DptId,_attRecord.DsgId,_attRecord.CompanyId,_attRecord.GId,_attRecord.LateTime,_attRecord.StayTime,_attRecord.TiffinCount,_attRecord.HolidayCount,_attRecord.PaybleDays,_attRecord.OtherOverTime,_attRecord.TotalOverTime,_attRecord.UserId,_attRecord.NightAllowCount};

                if (_attRecord.ODID > 0)
                    {
                        List<string> _getColumns = getColumns.ToList();
                        List<string> _getValues = getValues.ToList();
                        _getColumns.Add("ODID");
                        _getValues.Add(_attRecord.ODID.ToString());
                        getColumns = _getColumns.ToArray();
                        getValues = _getValues.ToArray();
                    }

                if (_attRecord.DbName == "cw_marico2")
                {
                    List<string> _getColumns = getColumns.ToList();
                    List<string> _getValues = getValues.ToList();
                    _getColumns.Add("TotalOverTimePre");
                    _getValues.Add(_attRecord.TotalOverTimePre.ToString());
                    getColumns = _getColumns.ToArray();
                    getValues = _getValues.ToArray();
                }

                    try
                    {
                        SQLOperation.forSaveValue("tblAttendanceRecord", getColumns, getValues, sqlDB.connection);
                    }
                    catch (Exception ex)
                    {
                        mCommon_Module_For_AttendanceProcessing.NotCountableAttendanceLog(_attRecord.EmpId, ex.Message, _attRecord.AttDate.ToString("yyyy-MM-dd"));
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }



        }
    }
