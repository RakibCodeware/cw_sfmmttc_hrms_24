using adviitRuntimeScripting;
using ComplexScriptingSystem;
using HRD.ModelEntities.Models;
using SigmaERP.hrms.repo.repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SigmaERP.classes
{
    public class AttendanceProcessing
    {
        AttendanceCommonTasks _attCommon;
        string query = "";

        public void _AttendanceProcessingWithoutShift(string CompanyId, DateTime SelectedDate , bool ForAllEmployee, string DepartmentId, string EmpCardNo, string UserId, string EmpType)//Marico
        {
            try
            {
                if (_attCommon == null)
                    _attCommon = new AttendanceCommonTasks();
                // load employees
                DataTable dtEmpInfo = _attCommon.getEmployees(SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee, CompanyId, DepartmentId, EmpCardNo, EmpType);
                if (dtEmpInfo != null && dtEmpInfo.Rows.Count > 0)
                {                    
                            // delete existing attendance
                            if (_attCommon.DeleteAttendance(CompanyId, DepartmentId, SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee, dtEmpInfo.Rows[0]["EmpID"].ToString(), EmpType))
                            {                                                          
                                
                                for (int i = 0; i < dtEmpInfo.Rows.Count; i++)
                                {
                                    if (DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString())) > SelectedDate)
                                    {
                                        continue;
                                    }
                                    else
                                    {                                      

                                        AttendanceRecord _attRecord = new AttendanceRecord
                                        {
                                            EmpId = dtEmpInfo.Rows[i]["EmpId"].ToString(),
                                            AttDate = SelectedDate,
                                            EmpTypeId = dtEmpInfo.Rows[i]["EmpTypeId"].ToString(),
                                            InHour = "00",
                                            InMin = "00",
                                            InSec = "00",
                                            OutHour = "00",
                                            OutMin = "00",
                                            OutSec = "00",
                                            AttStatus = "A",
                                            StateStatus = "Absent",
                                            DptId = dtEmpInfo.Rows[i]["DptId"].ToString(),
                                            DsgId = dtEmpInfo.Rows[i]["DsgId"].ToString(),
                                            CompanyId = CompanyId,
                                            GId = dtEmpInfo.Rows[i]["GId"].ToString(),
                                            LateTime = "00:00:00",
                                            StayTime = "00:00:00",
                                            TiffinCount = "0",
                                            HolidayCount = "0",
                                            PaybleDays = "0",
                                            OverTime = "00:00:00",
                                            OtherOverTime = "00:00:00",
                                            TotalOverTime = "00:00:00",
                                            NightAllowCount = "0",
                                            UserId = UserId

                                        };                                       
                                        _attRecord.SftId = "0";
                                        //end check roster 
                                        string[] Leave_Info = _attCommon.CheckLeave(SelectedDate.ToString("yyyy-MM-dd"), _attRecord.EmpId);
                                        if (Leave_Info[0] != "0")// leave
                                        {
                                            _attRecord.AttStatus = "Lv";
                                            _attRecord.StateStatus = Leave_Info[1];
                                        }
                                        else // not  Leave (W/H/P/A/L)
                                        {
                                    string _ProxymityNo = dtEmpInfo.Rows[i]["RealProximityNo"].ToString();
                                            DataTable dtPunch = new DataTable();
                                            dtPunch = _attCommon.GetPunchWithoutShift(_ProxymityNo, SelectedDate.ToString("yyyy-MM-dd"));

                                            if (dtPunch != null && dtPunch.Rows.Count > 0)
                                            {
                                        _attRecord = _attCommon.GetAttStatusWithoutShift(_attRecord, DateTime.Parse(dtPunch.Rows[0]["CHECKTIME"].ToString()), DateTime.Parse(dtPunch.Rows[dtPunch.Rows.Count - 1]["CHECKTIME"].ToString()),AttendanceStaticInfo.WorkingTime);
                                        }                                       


                                        }
                                        _attCommon.SaveAttendanceRecord(_attRecord);

                                    }
                                }
                                //_attCommon.DeleteTempRawData(ProcessingID);
                            }
                        
                    
                }
            }
            catch (Exception ex)
            {
                //_attCommon.DeleteTempRawData(ProcessingID);
            }
        }
        public void _AttendanceProcessingWithCommonShift(string CompanyId, DateTime SelectedDate, bool ForAllEmployee, string DepartmentId, string EmpCardNo, string UserId, string EmpType)//Marico
        {
            try
            {
                string dbName =Glory. getDBName();

                string DeviceType = (dbName== "cw_marico" || dbName == "cw_marico3") ?"zk_biotime": "";
                
                if (_attCommon == null)
                    _attCommon = new AttendanceCommonTasks();
                // load employees
                DataTable dtEmpInfo = _attCommon.getEmployees(SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee, CompanyId, DepartmentId, EmpCardNo, EmpType);
                if (dtEmpInfo != null && dtEmpInfo.Rows.Count > 0)
                {
                   string[] WH=  _attCommon.CheckHolidayWeekend(CompanyId, SelectedDate.ToString("yyyy-MM-dd"));

                    // delete existing attendance
                    if (_attCommon.DeleteAttendance(CompanyId, DepartmentId, SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee, dtEmpInfo.Rows[0]["EmpID"].ToString(), EmpType))
                    {


                        for (int i = 0; i < dtEmpInfo.Rows.Count; i++)
                        {
                            if (DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString())) > SelectedDate)
                            {
                                continue;
                            }
                            else
                            {
                                DataTable dtTimeTable = new DataTable();
                                dtTimeTable = CRUD.ExecuteReturnDataTable("select SftId,SftName,SftStartTime,SftEndTime,BeginningIn,EndingIn,BeginningOut,EndingOut from HRD_Shift where IsActive=1 and CompanyId='" + CompanyId + "' order by SftStartTime");

                                AttendanceRecord _attRecordDefault = new AttendanceRecord
                                {
                                    EmpId = dtEmpInfo.Rows[i]["EmpId"].ToString(),
                                    AttDate = SelectedDate,
                                    EmpTypeId = dtEmpInfo.Rows[i]["EmpTypeId"].ToString(),
                                    InHour = "00",
                                    InMin = "00",
                                    InSec = "00",
                                    OutHour = "00",
                                    OutMin = "00",
                                    OutSec = "00",
                                    AttStatus = "A",
                                    StateStatus = "Absent",
                                    DptId = dtEmpInfo.Rows[i]["DptId"].ToString(),
                                    DsgId = dtEmpInfo.Rows[i]["DsgId"].ToString(),
                                    CompanyId = CompanyId,
                                    GId = dtEmpInfo.Rows[i]["GId"].ToString(),
                                    LateTime = "00:00:00",
                                    StayTime = "00:00:00",
                                    TiffinCount = "0",
                                    HolidayCount = "0",
                                    PaybleDays = "0",
                                    OverTime = "00:00:00",
                                    OtherOverTime = "00:00:00",
                                    TotalOverTime = "00:00:00",
                                    NightAllowCount = "0",
                                    SftId = "0",
                                    UserId = UserId,
                                    TotalOverTimePre = "00:00:00",
                                    DbName = dbName
                                    };
                                AttendanceRecord _attRecord;
                                //end check roster 
                                string[] Leave_Info = _attCommon.CheckLeave(SelectedDate.ToString("yyyy-MM-                                         dd"), _attRecordDefault.EmpId);
                                    List<AttendanceRecord> _attRecords = new List<AttendanceRecord>();
                                    if (Leave_Info[0] != "0")// leave
                                    {
                                    _attRecord = new AttendanceRecord();
                                    _attRecord = (AttendanceRecord)_attRecordDefault.Clone();
                                    _attRecord.AttStatus = "Lv";
                                    _attRecord.StateStatus = Leave_Info[1];                                        
                                    }
                                    else // not  Leave (W/H/P/A/L)
                                    {
                                        string _ProxymityNo = dtEmpInfo.Rows[i]["RealProximityNo"].ToString();
                                    bool IsFullTimeAsOT = false;
                                    
                                    for (int t = 0; t < dtTimeTable.Rows.Count; t++)
                                    { 
                                        DataTable dtPunch = new DataTable();
                                        dtPunch = _attCommon.GetPunchWithTimetable(_ProxymityNo,DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd")+" "+ dtTimeTable.Rows[t]["BeginningIn"].ToString()), DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + dtTimeTable.Rows[t]["EndingIn"].ToString()), DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + dtTimeTable.Rows[t]["BeginningOut"].ToString()), DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + dtTimeTable.Rows[t]["EndingOut"].ToString()), DeviceType);
                                        if (dtPunch != null && dtPunch.Rows.Count > 0)
                                        {
                                            DateTime? LoginTime=null, LogoutTime=null;
                                            for (byte p = 0; p < dtPunch.Rows.Count; p++)
                                            {
                                                if (dtPunch.Rows[p]["PunchType"].ToString() == "In")
                                                    LoginTime = DateTime.Parse(dtPunch.Rows[p]["CHECKTIME"].ToString());
                                                else
                                                    LogoutTime = DateTime.Parse(dtPunch.Rows[p]["CHECKTIME"].ToString());

                                            }                                             
                                            _attRecord = new AttendanceRecord();
                                            _attRecord =(AttendanceRecord)_attRecordDefault.Clone();
                                            if (WH[0] == "True")
                                            {
                                                IsFullTimeAsOT = true;
                                                _attRecord = new AttendanceRecord();
                                                _attRecord = (AttendanceRecord)_attRecordDefault.Clone();
                                                _attRecord.AttStatus = WH[1];
                                                _attRecord.StateStatus = WH[1] == "W" ? "Weekend" : "Holiday";

                                            }
                                            else
                                            {
                                                IsFullTimeAsOT = false;
                                                _attRecord.AttStatus = "P";
                                                _attRecord.StateStatus = "Present";
                                            }
                                            DateTime SftStartTime= DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + dtTimeTable.Rows[t]["SftStartTime"].ToString()), SftEndTime=
                                            DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + dtTimeTable.Rows[t]["SftEndTime"].ToString());
                                            //if (SftStartTime > SftEndTime)
                                            //    SftEndTime = SftEndTime.AddDays(1);

                                            _attRecord = _attCommon.GetAttStatusWithCommonShift(_attRecord,LoginTime,LogoutTime, SftStartTime,SftEndTime, AttendanceStaticInfo.WorkingTime,IsFullTimeAsOT);
                                            _attRecord.SftId = dtTimeTable.Rows[t]["SftId"].ToString();
                                            if (_attRecord.OverTime != "00:00:00")
                                                IsFullTimeAsOT = true;
                                            _attRecords.Add(_attRecord);
                                        }
                                    }

                                    if (_attRecords.Count == 0)
                                    {
                                        _attRecord = new AttendanceRecord();
                                        _attRecord = (AttendanceRecord)_attRecordDefault.Clone();
                                        _attRecords.Add(_attRecord);
                                    }


                                    if (dbName == "cw_marico2")
                                    {
                                        AttendanceRecord _att = new AttendanceRecord();
                                        foreach (AttendanceRecord attTemp in _attRecords)
                                        {
                                            _att = attTemp;
                                            if (attTemp.InHour!= "00" || attTemp.InMin != "00" || attTemp.InSec != "00")
                                                break;

                                        }
                                        _attCommon.SaveAttendanceRecord(_att);

                                    }
                                    else
                                    {
                                        foreach (AttendanceRecord _att in _attRecords)
                                        {
                                            _attCommon.SaveAttendanceRecord(_att);
                                        }
                                    }                                   
                                    


                                }
                            }
                        }
                        //_attCommon.DeleteTempRawData(ProcessingID);
                    }


                }
            }
            catch (Exception ex)
            {
                //_attCommon.DeleteTempRawData(ProcessingID);
            }
        }
        public void _AttendanceProcessing(string ProcessingID,string DeviceType,string CompanyId, DateTime SelectedDate, FileUpload FileUploader, bool ForAllEmployee, string DepartmentId, string EmpCardNo, string UserId,string EmpType,string db, Label lblErrorMessage)
        {
            try
            {
                if (_attCommon == null)
                    _attCommon = new AttendanceCommonTasks();
                // load employees
                DataTable dtEmpInfo = _attCommon.getEmployees(SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee, CompanyId, DepartmentId, EmpCardNo, EmpType);
                if (dtEmpInfo != null && dtEmpInfo.Rows.Count > 0)
                {
                    // file upload
                    string fileName = "sql";
                    if (db=="access")
                        fileName=_attCommon.fileUpload(FileUploader, CompanyId);
                    if (fileName!="")
                    {
                        // import raw data form uploaded file(db)
                        if (_attCommon.importRawDataFormDevice(fileName, CompanyId, ForAllEmployee, dtEmpInfo.Rows[0]["EmpID"].ToString(), dtEmpInfo.Rows[0]["RealProximityNo"].ToString(), SelectedDate, ProcessingID,db))
                        {
                            // delete existing attendance
                            if (_attCommon.DeleteAttendance(CompanyId,DepartmentId,SelectedDate.ToString("yyyy-MM-dd"),ForAllEmployee, dtEmpInfo.Rows[0]["EmpID"].ToString(), EmpType))
                            {
                                SQLOperation.forDelete("tblAttendance_NotCountableLogRecord", sqlDB.connection);  // for clear full tblAttendance_NotCountableLogRecord table

                                string GeneralDayEmpType= _attCommon.GetGeneralDayInfo(CompanyId, SelectedDate.ToString("yyyy-MM-dd"));
                              
                                bool IsRegularWeekend = false;
                                bool IsHoliday = false;
                                if (GeneralDayEmpType != "0")// "0" means, General Day for all employee
                                {
                                    string[] WH = _attCommon.CheckHolidayWeekend(CompanyId, SelectedDate.ToString("yyyy-MM-dd"));

                                    if (bool.Parse(WH[0]))
                                    {
                                        if (WH[1].Equals("W"))
                                            IsRegularWeekend = true;
                                        else
                                            IsHoliday = true;
                                    }
                                }  
                                
                                string[] othersetting = _attCommon.GetOthersSetting(CompanyId);
                                TimeSpan workerTiffinTime = TimeSpan.Parse(othersetting[0]);
                                TimeSpan staffTiffinTime = TimeSpan.Parse(othersetting[1]);
                                for (int i = 0; i < dtEmpInfo.Rows.Count; i++)
                                {                                   
                                    if (DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString())) > SelectedDate)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        
                                        bool IsWeekend = false;
                                        AttendanceRecord _attRecord = new AttendanceRecord{
                                            EmpId = dtEmpInfo.Rows[i]["EmpId"].ToString(),
                                            AttDate = SelectedDate,
                                            EmpTypeId =dtEmpInfo.Rows[i]["EmpTypeId"].ToString(),
                                            InHour = "00", InMin = "00",InSec = "00",
                                            OutHour = "00", OutMin = "00", OutSec = "00",
                                            AttStatus = "A",StateStatus="Absent",                                          
                                            DptId = dtEmpInfo.Rows[i]["DptId"].ToString(),
                                            DsgId = dtEmpInfo.Rows[i]["DsgId"].ToString(),
                                            CompanyId = CompanyId,
                                            GId= dtEmpInfo.Rows[i]["GId"].ToString(),
                                            LateTime = "00:00:00",
                                            StayTime = "00:00:00",
                                            TiffinCount="0",
                                            HolidayCount="0",
                                            PaybleDays= "0",
                                            OverTime = "00:00:00",
                                            OtherOverTime = "00:00:00",
                                            TotalOverTime = "00:00:00",
                                            NightAllowCount = "0",
                                            UserId=UserId                                          
                                            
                                        };
                                        //check roster 
                                        string[] rosterInfo = _attCommon.GetRosterInfo(SelectedDate.ToString("yyyy-MM-dd"), _attRecord.EmpId, dtEmpInfo.Rows[i]["EmpDutyType"].ToString(), dtEmpInfo.Rows[i]["SftID"].ToString(), dtEmpInfo.Rows[i]["EmpTypeId"].ToString());
                                        if (rosterInfo == null || rosterInfo[0] == "0")
                                        {
                                            _attCommon.NotCountableAttendanceLog(_attRecord.EmpId, "Roster Missing", SelectedDate.ToString("yyyy-MM-dd"));
                                            continue;
                                        }
                                        _attRecord.SftId = rosterInfo[0];
                                        //end check roster 
                                        string[] Leave_Info = _attCommon.CheckLeave(SelectedDate.ToString("yyyy-MM-                                         dd"),_attRecord.EmpId);
                                        
                                        if (Leave_Info[0] != "0")// leave
                                        {
                                            _attRecord.AttStatus = "Lv";
                                            _attRecord.StateStatus = Leave_Info[1];
                                        }
                                        else // not  Leave (W/H/P/A/L)
                                        {
                                            if (GeneralDayEmpType == "0" || GeneralDayEmpType == dtEmpInfo.Rows[i]["EmpTypeId"].ToString()) // checke Is General Day?
                                            {
                                            // this is a general day 
                                            }
                                            else // not is general day 
                                            {
                                                if (dtEmpInfo.Rows[i]["WeekendType"].ToString().Equals("Roster"))
                                                {
                                                    IsWeekend = commonTask.IsRosterWeekend(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                                                }
                                                else
                                                {
                                                    IsWeekend = IsRegularWeekend;
                                                }
                                                if (IsHoliday)// Holiday
                                                {
                                                    _attRecord.AttStatus = "H";
                                                    _attRecord.StateStatus = "Holiday";
                                                }
                                                else if (IsWeekend)// Weekend
                                                {
                                                    _attRecord.AttStatus = "W";
                                                    _attRecord.StateStatus = "Weekend";
                                                }
                                            }                                     
                                            string _ProxymityNo = _attCommon.GetEmpProximityNo(_attRecord.EmpId, SelectedDate.ToString("yyyy-MM-dd"));
                                            _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[i]["RealProximityNo"].ToString() : _ProxymityNo;
                                            DataTable dtPunch = new DataTable();
                                           dtPunch = _attCommon.GetPunch(ProcessingID, DeviceType, CompanyId, _ProxymityNo, DateTime.Parse(rosterInfo[3]), DateTime.Parse(rosterInfo[4]));
                                           

                                            if (dtPunch != null && dtPunch.Rows.Count > 0)
                                            { bool OnePunchPresent = Glory.getDBName()== "cw_hrms_tmc_hospital"?true:false;
                                                
                                                _attRecord = _attCommon.GetAttStatus(_attRecord, DateTime.Parse(dtPunch.Rows[0]["PunchTime"].ToString()), DateTime.Parse(dtPunch.Rows[dtPunch.Rows.Count - 1]["PunchTime"].ToString()),rosterInfo , TimeSpan.Parse(othersetting[3]), TimeSpan.Parse(othersetting[5]), OnePunchPresent, dtEmpInfo.Rows[i]["EmpDutyType"].ToString());

                                                if (_attRecord.StateStatus == "Absent" || _attRecord.StateStatus == "Present")
                                                    _attRecord = _attCommon.CheckOutDuty(_attRecord, othersetting[3], true, DateTime.Parse(rosterInfo[1]), DateTime.Parse(rosterInfo[2]), DateTime.Parse(dtPunch.Rows[0]["PunchTime"].ToString()), DateTime.Parse(dtPunch.Rows[dtPunch.Rows.Count - 1]["PunchTime"].ToString()));
                                            }
                                            else
                                            {
                                                if (_attRecord.StateStatus == "Absent" || _attRecord.StateStatus == "Present")
                                                    _attRecord = _attCommon.CheckOutDuty(_attRecord,othersetting[3], false, DateTime.Parse(rosterInfo[1]), DateTime.Parse(rosterInfo[2]),DateTime.Now, DateTime.Now);
                                            }
                                            

                                        }
                                        _attCommon.SaveAttendanceRecord(_attRecord);

                                    }
                                }
                                //_attCommon.DeleteTempRawData(ProcessingID);
                                }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //_attCommon.DeleteTempRawData(ProcessingID);
            }
        }
        public DataTable LoadProcessedAttendanceData(string CompanyId, string DepartmentId, string AttDate, bool ForAllEmployee, string EmpId,string EmpType,string dataAccesLevel)
        {
            try
            {
                EmpType = EmpType == "All" ? "" : " and atd.EmpTypeId=" + EmpType;
                DataTable dt = new DataTable();
                if (!ForAllEmployee)
                    query = "select Right(ecs.EmpCardNo,Len(ecs.EmpCardNo)-7)+' ('+ecs.EmpProximityNo+')' as EmpCardNo,ecs.EmpType,ecs.EmpDutyType,ISNULL( ecs.WeekendType,'Regular') as WeekendType,ecs.EmpName,ecs.DptName,ecs.DsgName, case when atd.ODID >0 then atd.ATTStatus+'(OD)' else atd.ATTStatus end as ATTStatus, " +
                   "   atd.InHour+':'+atd.InMin+':'+atd.InSec as Intime,atd.OutHour+':'+atd.OutMin+':'+atd.OutSec as Outtime,Format(ATTDate,'dd-MMM-yyyy') as ATTDate,atd.AttManual,case when sft.SftId is null then '' else sft.SftName +' [ '+ convert (varchar(8), SftStartTime)+' - '+convert (varchar(8),sft.SftEndTime)+' ]' end as sftinfo  from  tblAttendanceRecord as atd left join HRD_Shift sft on atd.SftId=sft.SftId  inner join " +
                   " v_Personnel_EmpCurrentStatus ecs on " +
                   " atd.EmpId=ecs.EmpId and ecs.IsActive=1 AND atd.ATTDate='" + AttDate + "' and atd.CompanyId='" + CompanyId + "' AND  (ecs.EmpCardNo  Like '%" + EmpId + "' or ecs.EmpAttCard='" + EmpId + "')";

                else if (DepartmentId.Equals("0")) {
                    string condition = "and atd.CompanyId='" + CompanyId + "'";
                    if (dataAccesLevel=="4" || dataAccesLevel == "2")
                    {
                        condition += "and atd.CompanyId='" + CompanyId + "' and atd.DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ")";
                    }
                    query = "select Right(ecs.EmpCardNo,Len(ecs.EmpCardNo)-7)+' ('+ecs.EmpProximityNo+')' as EmpCardNo,ecs.EmpType,ecs.EmpDutyType,ISNULL( ecs.WeekendType,'Regular') as WeekendType,ecs.EmpName,ecs.DptName,ecs.DsgName, case when atd.ODID >0 then atd.ATTStatus+'(OD)' else atd.ATTStatus end as ATTStatus, " +
                       "  atd.InHour+':'+atd.InMin+':'+atd.InSec as Intime,atd.OutHour+':'+atd.OutMin+':'+atd.OutSec as Outtime,Format(ATTDate,'dd-MMM-yyyy') as ATTDate,atd.AttManual ,case when sft.SftId is null then '' else sft.SftName +' [ '+ convert (varchar(8), SftStartTime)+' - '+convert (varchar(8),sft.SftEndTime)+' ]' end as sftinfo  from  tblAttendanceRecord as atd left join HRD_Shift sft on atd.SftId=sft.SftId inner join " +
                       " v_Personnel_EmpCurrentStatus ecs on " +
                       " atd.EmpId=ecs.EmpId and ecs.IsActive=1 "+ condition + "  AND atd.ATTDate='" + AttDate + "' " + EmpType + " order by ecs.DptId,ecs.CustomOrdering";
                }
                   

                else
                    query = "select Right(ecs.EmpCardNo,Len(ecs.EmpCardNo)-7)+' ('+ecs.EmpProximityNo+')' as EmpCardNo,ecs.EmpType,ecs.EmpDutyType,ISNULL( ecs.WeekendType,'Regular') as WeekendType,ecs.EmpName,ecs.DptName,ecs.DsgName, case when atd.ODID >0 then atd.ATTStatus+'(OD)' else atd.ATTStatus end as ATTStatus, " +
                    "  atd.InHour+':'+atd.InMin+':'+atd.InSec as Intime,atd.OutHour+':'+atd.OutMin+':'+atd.OutSec as Outtime,Format(ATTDate,'dd-MMM-yyyy') as ATTDate,atd.AttManual ,case when sft.SftId is null then '' else sft.SftName +' [ '+ convert (varchar(8), SftStartTime)+' - '+convert (varchar(8),sft.SftEndTime)+' ]' end as sftinfo  from  tblAttendanceRecord as atd left join HRD_Shift sft on atd.SftId=sft.SftId inner join " +
                    " v_Personnel_EmpCurrentStatus ecs on " +
                    " atd.EmpId=ecs.EmpId and ecs.IsActive=1 AND atd.ATTDate='" + AttDate + "' and atd.CompanyId='" + CompanyId + "' AND atd.DptId='" + DepartmentId + "'" + EmpType + " order by ecs.CustomOrdering";

                return CRUD.ExecuteReturnDataTable(query);

            }
            catch { return null; }
        }
    }
}