using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using ComplexScriptingSystem;

namespace SigmaERP.classes
{
    public class mManually_Attendance_Count
    {
       
        public static string []  Find_IsRunningEmployee(string CompanyId,string EmpCardNo )
        {
            try
            {
                DataTable dt;                          
                dt = new DataTable();
                string [] EmployeeInfo = new string[1];
                SqlDataAdapter da = new SqlDataAdapter("select EmpId,EmpName,DptName from v_Personnel_EmpCurrentStatus where EmpCardNo like '%" + EmpCardNo + "'" +
                         " AND IsActive='1' AND  EmpStatus in ('1','8')  " +
                         " AND CompanyId='" + CompanyId + "'", sqlDB.connection);
                da.Fill(dt);
                //sqlDB.fillDataTable("select EmpId,EmpName,DptName from v_Personnel_EmpCurrentStatus where EmpCardNo like '%" + EmpCardNo + "'" +
                //         " AND IsActive='1' AND  EmpStatus in ('1','8')  " +
                //         " AND CompanyId='" + CompanyId + "'", dt);

                if (dt.Rows.Count > 0)
                {
                    //divFindInfo.Style.Add("Color", "Green");                
                     EmployeeInfo[0] = "Name:" + dt.Rows[0]["EmpName"].ToString() + ",Department: " + dt.Rows[0]["DptName"].ToString();
                    
                    return EmployeeInfo;                
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }


        public static string[] Get_Needed_EmployeeeInfo(string CompanyId, string EmpCardNo)
        {
            try
            {
                DataTable dt;
                dt = new DataTable();
                string[] EmployeeInfo = new string[10];
                sqlDB.fillDataTable("select EmpId,DptId,DsgId,GID,CompanyId,EmpDutyType,SftId,EmpTypeId,Format(EmpJoiningDate,'dd-MM-yyyy')as EmpJoiningDate,IsDelivery,WeekendType from v_Personnel_EmpCurrentStatus where EmpCardNo like '%" + EmpCardNo + "'" +
                         " AND IsActive='1' AND  EmpStatus in ('1','8')  " +
                         " AND CompanyId='" + CompanyId + "'", dt);

                if (dt.Rows.Count > 0)
                {
                    //divFindInfo.Style.Add("Color", "Green"); 
                    EmployeeInfo[0] = dt.Rows[0]["EmpId"].ToString();
                    EmployeeInfo[1] = dt.Rows[0]["DptId"].ToString();
                    EmployeeInfo[2] = dt.Rows[0]["DsgId"].ToString();
                    EmployeeInfo[3] = dt.Rows[0]["GID"].ToString();
                    EmployeeInfo[4] = dt.Rows[0]["EmpDutyType"].ToString();
                    EmployeeInfo[5] = dt.Rows[0]["SftId"].ToString();
                    EmployeeInfo[6] = dt.Rows[0]["EmpTypeId"].ToString();
                    EmployeeInfo[7] = dt.Rows[0]["EmpJoiningDate"].ToString();
                    EmployeeInfo[8] = dt.Rows[0]["IsDelivery"].ToString();
                    EmployeeInfo[9] = dt.Rows[0]["WeekendType"].ToString();
                    return EmployeeInfo;
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }
        public static void deleteExistingAttendanceByDate_EmpId(string AttDate, string EmpId)
        {
            try
            {
                string[] date = AttDate.Split('-');
                SqlCommand cmd;
                cmd = new System.Data.SqlClient.SqlCommand("delete from tblAttendanceRecord where attDate='" + date[2] + "-" + date[1] + "-" + date[0] + "' AND EmpId='" + EmpId + "' ", sqlDB.connection);
                cmd.ExecuteNonQuery();
                //if (result > 0) ChangeLeaveStatusByeEmpIdAndDate(dtEmpInfo.Rows[0]["EmpId"].ToString(), AttDate);

            }
            catch { }
        }
        public static string[] getTotalOverTime(TimeSpan LogInTime, TimeSpan LogOutTime, TimeSpan ShiftStartTime, TimeSpan ShiftEndTime, string AcceptableMinAsOT, string AcceptableMinAsLate, bool IsWHO_DaysTask, TimeSpan TiffinTime, TimeSpan MinWorkingHours,TimeSpan MinOverTime,string BreakBeforOTAsMin)
        {
            try
            {
                //TimeSpan StartOT = ShiftEndTime.Add(TimeSpan.FromMinutes(int.Parse(BreakBeforOTAsMin)));// OT start after 30 minutes(Break)  of Shift End Time
                TimeSpan StartOT = ShiftEndTime;
                StartOT = TimeSpan.Parse(StartOT.Hours.ToString() + ":" + StartOT.Minutes.ToString() + ":" + StartOT.Seconds.ToString());
                string[] DayStatus = new string[9];

                DayStatus[3] = "00:00:00";

                if (!IsWHO_DaysTask)
                {
                    if (LogInTime <= ShiftStartTime + TimeSpan.Parse("00:" + AcceptableMinAsLate + ":00")) DayStatus[0] = "P";
                    else if (LogInTime > ShiftStartTime + TimeSpan.Parse("00:" + AcceptableMinAsLate.ToString() + ":00"))
                    {
                        DayStatus[0] = "L";
                        DayStatus[3] = (LogInTime - ShiftStartTime).ToString(); // to get late time
                    }
                    DayStatus[1] = "Present";
                }

                //string OverTime= (!IsWHO_DaysTask) ? (LogOutTime - ShiftEndTime).ToString() : (LogOutTime - LogInTime).ToString();
                string OverTime = "00:00:00";
                TimeSpan totalTime=TimeSpan.Parse("00:00:00");
                //------ to get stay time-------Modifyed by nayem at 30/07/2017------------------------
                DateTime time = DateTime.Today + LogInTime;
                String result = time.ToString("tt");
                TimeSpan k;
                if ((DateTime.Today + LogInTime).ToString("tt") == "PM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM")
                {
                   
                     k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(LogInTime.ToString()));
                    if (StartOT.Hours < 12 )// AM
                    {
                        if (StartOT < LogOutTime)
                        {
                            OverTime = (!IsWHO_DaysTask) ? (LogOutTime - StartOT).ToString() : k.ToString();
                            totalTime = (LogOutTime - StartOT);
                        }
                        
                        
                    }
                    else //PM
                    {
                        OverTime = (!IsWHO_DaysTask) ? (DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(StartOT.ToString()))).ToString() : k.ToString();
                        totalTime = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(StartOT.ToString()));
                    }


                }
                else if ((DateTime.Today + LogInTime).ToString("tt") == "AM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM" && (LogOutTime < LogInTime))
                {
                  
                        k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(LogInTime.ToString()));
                    try
                    {
                        //string st = "23:59:00";// StartOT.ToString("HH:mm:ss");
                        //TimeSpan d = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(st));
                        if (StartOT.Hours < 12)// AM
                        {
                            if (StartOT < LogOutTime)
                            {
                                OverTime = (!IsWHO_DaysTask) ? (LogOutTime - StartOT).ToString() : k.ToString();
                                totalTime = (LogOutTime - StartOT);
                            }
                        }
                        else //PM
                        {
                            OverTime = (!IsWHO_DaysTask) ? (DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(StartOT.ToString()))).ToString() : k.ToString();
                            totalTime = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(StartOT.ToString()));
                        }
                      
                       
                    }
                    catch (Exception ex) { }
                    
                }
                else
                {
                    k = (LogOutTime - LogInTime);
                    
                    if (StartOT < LogOutTime)
                    {
                        OverTime = (!IsWHO_DaysTask) ? (LogOutTime - StartOT).ToString() : k.ToString();
                        totalTime = (LogOutTime - StartOT);
                        if (k < totalTime)
                        {
                             OverTime = "00:00:00";
                             totalTime = TimeSpan.Parse("00:00:00");
                        }
                    }
                }
                string StayTime = "00:00:00";
                if (LogOutTime.ToString() == "00:00:00")
                {
                    StayTime = StayTime;
                }
                else
                    StayTime = k.ToString();
                //----------------- end stay time--------------------------------
         
                string Get_OTHour;
                string ExtraTime;                
                if (totalTime > MinOverTime)
                {
                    Get_OTHour = MinOverTime.ToString();
                    ExtraTime = (totalTime - MinOverTime).ToString();
                }
                else
                {
                    if(totalTime<TimeSpan.Parse("00:00:00"))
                    {
                        totalTime = TimeSpan.Parse("00:00:00");
                        Get_OTHour = "00:00:00";
                    }
                    else Get_OTHour = totalTime.ToString();
                    ExtraTime = "00:00:00";
                }
                DayStatus[2] = Get_OTHour;
                DayStatus[4] = StayTime;
                DayStatus[6] = "1";
                DayStatus[7] = ExtraTime;
                DayStatus[8] = totalTime.ToString();
                //---------------------to get TiffineCount------------------
                if (StayTime == "00:00:00"||TimeSpan.Parse(StayTime)<MinWorkingHours)
                {
                    DayStatus[2] = "00:00:00";
                    DayStatus[5] = "0";
                    DayStatus[6] = "0";
                    DayStatus[7] = "00:00:00";
                    DayStatus[8] = "00:00:00";
                }
                else
                {
                    TimeSpan tiffinstaytime;
                    if (LogInTime < ShiftStartTime)
                    {
                        time = DateTime.Today + ShiftStartTime;
                        result = time.ToString("tt");
                        if ((DateTime.Today + ShiftStartTime).ToString("tt") == "PM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM")
                        {
                            //TimeSpan z = LogOutTime - TimeSpan.Parse("00:00:00");
                            //TimeSpan i = (TimeSpan.Parse("23:59:59") - ShiftStartTime) + TimeSpan.Parse("00:00:01");
                            //k = z + i;
                            k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(ShiftStartTime.ToString()));
                        }
                        else if ((DateTime.Today + ShiftStartTime).ToString("tt") == "AM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM" && (LogOutTime < ShiftStartTime))
                        {                           
                            k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(ShiftStartTime.ToString()));
                        }
                        else
                        {
                            k = (LogOutTime - ShiftStartTime);
                        }                      
                        tiffinstaytime = k;
                    }
                    else
                    {
                        tiffinstaytime = TimeSpan.Parse(StayTime);
                        
                    }
                    if (tiffinstaytime >= TiffinTime)
                    {
                        DayStatus[5] = "1";
                    }
                    else
                    {
                        DayStatus[5] = "0";
                    }
                }
                if (TimeSpan.Parse(DayStatus[3]) > TimeSpan.Parse("04:00:00"))
                {
                    DayStatus[0] = "A";
                    DayStatus[1] = "Absent";
                    DayStatus[2] = "00:00:00";
                    DayStatus[3] = DayStatus[3];
                    DayStatus[4] = StayTime;
                    DayStatus[5] = "0";
                    DayStatus[6] = "0";
                    DayStatus[7] = "00:00:00";
                    DayStatus[8] = "00:00:00";
                }
               
                return DayStatus;
                
            }
            catch { return null; }
        }
        public static string[] OverTime_Calculation_ForWeekend_Holiday(TimeSpan LogInTime, TimeSpan LogOutTime, int AcceptableOTMin, TimeSpan RosterStartTime, TimeSpan TiffinTime, string attstatus, string StateStatus,TimeSpan MinOverTime)
        {
            try
            {

                string[] WHO_DayStatus = new string[9];
                string ExtraTime;
                string Get_OTHour;

                int Get_OTMinute;

                //Get_OTHour = (Get_OTMinute > AcceptableOTMin) ? (byte)(int.Parse(Get_OTHour.ToString()) + 1) : Get_OTHour;

                DateTime time = DateTime.Today + LogInTime;
                String result = time.ToString("tt");
                TimeSpan k;
                if ((DateTime.Today + LogInTime).ToString("tt") == "PM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM")
                {
                    TimeSpan z = LogOutTime - TimeSpan.Parse("00:00:00");
                    TimeSpan i = (TimeSpan.Parse("23:59:59") - LogInTime) + TimeSpan.Parse("00:00:01");
                    k = z + i;
                    if (k > MinOverTime)
                    {
                        Get_OTHour = MinOverTime.ToString();
                        ExtraTime = (k - MinOverTime).ToString();
                    }

                    else
                    {
                        Get_OTHour = k.ToString();
                        ExtraTime = "00:00:00";
                    }
                }
                else if ((DateTime.Today + LogInTime).ToString("tt") == "AM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM" && (LogOutTime < LogInTime))
                {

                    k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(LogInTime.ToString()));
                    if (k > MinOverTime)
                    {
                        Get_OTHour = MinOverTime.ToString();
                        ExtraTime = (k - MinOverTime).ToString();
                    }

                    else
                    {
                        Get_OTHour = k.ToString();
                        ExtraTime = "00:00:00";
                    }
                }
                else
                {
                    k = (LogOutTime - LogInTime);
                    if (k > MinOverTime)
                    {
                        Get_OTHour = MinOverTime.ToString();
                        ExtraTime = (k - MinOverTime).ToString();
                    }

                    else
                    {
                        Get_OTHour = k.ToString();
                        ExtraTime = "00:00:00";
                    }
                }

                string StayTime = "00:00:00";
                if (LogOutTime.ToString() == "00:00:00")
                {
                    StayTime = StayTime;
                }
                else
                    StayTime = k.ToString();

                //string StayTime = ((LogOutTime - LogInTime) < TimeSpan.Parse("00:00:00")) ? "00:00:00" : (LogOutTime - LogInTime).ToString();
                WHO_DayStatus[0] = attstatus;
                WHO_DayStatus[1] = StateStatus;
                WHO_DayStatus[2] = Get_OTHour;
                WHO_DayStatus[3] = "00:00:00";
                WHO_DayStatus[4] = StayTime;
                WHO_DayStatus[6] = "0";
                WHO_DayStatus[7] = ExtraTime;
                WHO_DayStatus[8] = StayTime;
                //---------------------to get TiffineCount------------------
                if (StayTime == "00:00:00")
                {
                    WHO_DayStatus[5] = "0";
                    WHO_DayStatus[2] = "00:00:00";
                    WHO_DayStatus[7] = "00:00:00";
                    WHO_DayStatus[8] = "00:00:00";
                }
                else
                {
                    TimeSpan tiffinstaytime;
                    if (LogInTime < RosterStartTime)
                    {
                        time = DateTime.Today + RosterStartTime;
                        result = time.ToString("tt");
                        if ((DateTime.Today + RosterStartTime).ToString("tt") == "PM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM")
                        {
                            TimeSpan z = LogOutTime - TimeSpan.Parse("00:00:00");
                            TimeSpan i = (TimeSpan.Parse("23:59:59") - RosterStartTime) + TimeSpan.Parse("00:00:01");
                            k = z + i;
                        }
                        else if ((DateTime.Today + RosterStartTime).ToString("tt") == "AM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM" && (LogOutTime < RosterStartTime))
                        {
                            k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(RosterStartTime.ToString()));
                        }
                        else
                        {
                            k = (LogOutTime - RosterStartTime);
                        }                       
                        tiffinstaytime = k;
                       // tiffinstaytime = LogOutTime - RosterStartTime;
                    }
                    else
                    {
                        tiffinstaytime = TimeSpan.Parse(StayTime);
                       // tiffinstaytime = LogOutTime - LogInTime;
                    }
                    if (tiffinstaytime >= TiffinTime)
                    {
                        WHO_DayStatus[5] = "1";
                    }
                    else
                    {
                        WHO_DayStatus[5] = "0";
                    }
                }

                return WHO_DayStatus;
            }
            catch { return null; }
        }

        public static void SaveAttendance_Status(string EmpId, string selectedDate, string EmpTypeId, string InHour, string InMin, string InSec, string OutHour, string OutMin, string OutSec,
                                           string AttStatus, string StateStatus, string OverTime, string[] rosterInfo, string DptId, string DsgId, string CompanyId, string GId, string LateTime, string StayTime, string DailyStartTimeALT_CloseTime, string AttManual, string TiffinCount, string HolidayCount, string PaybleDays, string OtherOverTime,string TotalOverTime,string OutDuty, string ReferenceID,string UserId,string Remark,string IsDelivery,string DutyType)
        {
            try
            {

                deleteExistingAttendanceByDate_EmpId(selectedDate, EmpId); // for delete existing attendance record
                string[] d = selectedDate.Split('-');
                string sDate = d[2] + "-" + d[1] + "-" + d[0];
                DateTime dtTimeConvert;
                LateTime = (LateTime == null) ? "00:00:00" : LateTime;
                if (DateTime.TryParse(LateTime, out dtTimeConvert)) LateTime = dtTimeConvert.ToString("HH:mm:ss");
                dtTimeConvert = new DateTime();
                StayTime = (StayTime == null) ? "00:00:00" : StayTime;
                if (OutDuty == "1")
                {
                    TiffinCount = "1";
                    PaybleDays = "1";
                }
                ////---Start Night Allow. Calculation by Nayem at 30/07/2017 --
                //string NightAllowCount = "0";
                //if (EmpTypeId == "1")
                //{
                //    TimeSpan _StayTime = TimeSpan.Parse(StayTime);
                //    if (_StayTime >= TimeSpan.Parse("15:00:00"))
                //        NightAllowCount = "1";
                //}
                ////---End Night Allow. Calculation---------------------------
                //----Start Night,tiffin Allow. Calculation by Nayem at 30/01/2019 ---------
                string NightAllowCount = "0";
                TiffinCount = "0";
                if (!IsDelivery.Equals("True"))
                {
                    
                    if (TotalOverTime != "00:00:00")
                    {
                        AttendanceCommonTasks commonTasks = new AttendanceCommonTasks();
                        TotalOverTime = commonTasks.NetOverTimeAfterDeductedBreaks(EmpId, AttStatus, rosterInfo, sDate, TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime), DutyType);
                    }
                    TimeSpan _TotalOverTime = TimeSpan.Parse(TotalOverTime);
                    if (AttStatus == "W" || AttStatus == "H")
                    {

                        if (_TotalOverTime >= TimeSpan.Parse("10:00:00"))
                        {
                            TiffinCount = "1";
                            NightAllowCount = "1";
                        }
                        else if (_TotalOverTime >= TimeSpan.Parse("07:00:00"))
                        {
                            NightAllowCount = "1";
                        }

                    }
                    else
                    {
                        if (_TotalOverTime >= TimeSpan.Parse("06:00:00"))
                        {
                            TiffinCount = "1";
                            NightAllowCount = "1";
                        }
                        else if (_TotalOverTime >= TimeSpan.Parse("02:00:00"))
                        {
                            TiffinCount = "1";
                        }
                    }
                    // deduct break time from weekend/holiday overtime
                    //if ((AttStatus == "W" || AttStatus == "H") && TotalOverTime != "00:00:00")
                    //{
                    //    TotalOverTime = commonAtt.totalOverTimeAtWeekendHoliday(sDate, TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
                    //}
                    //else if (TotalOverTime != "00:00:00")
                    //{
                    //    TotalOverTime = commonAtt.totalOverTimeAtWeekendHoliday(sDate, TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
                    //}

                    
                }

                //------- sort leave check for late--------------
                if (AttStatus == "L")
                {
                    if (TimeSpan.Parse(LateTime) >=TimeSpan.Parse("03:00:00"))
                    {
                        AttStatus = "A";
                        PaybleDays = "0";
                    }
                    if (commonTask.notLateForShortleave(sDate, EmpId))
                        AttStatus = "P";
                }                    
                if (TiffinCount == "1")
                    if (commonTask.isShortleave(sDate, EmpId))
                        TiffinCount = "0";
                //------- End sort leave check for late----------
                //---End Night Allow. Calculation--------------------
                if (DateTime.TryParse(StayTime, out dtTimeConvert)) StayTime = dtTimeConvert.ToString("HH:mm:ss");
                string[] getColumns = { "EmpId", "AttDate", "EmpTypeId", "InHour", "InMin", "InSec", "OutHour", "OutMin", "OutSec",
                                        "AttStatus", "StateStatus",
                                        "DailyStartTimeALT_CloseTime", "OverTime", "SftId", "DptId","DsgId", "CompanyId", "GId","LateTime","StayTime","AttManual","TiffinCount","HolidayCount","PaybleDays","OtherOverTime","TotalOverTime","OutDuty" ,"ReferenceID","UserId","Remark","NightAllowCount"};

                string[] getValues = { EmpId, sDate,                                                 EmpTypeId,InHour,InMin,InSec,OutHour,OutMin,OutSec,AttStatus,
                                                 StateStatus,DailyStartTimeALT_CloseTime,OverTime,rosterInfo[0],DptId,DsgId,CompanyId,GId,LateTime,StayTime,AttManual,TiffinCount,HolidayCount,PaybleDays,OtherOverTime,TotalOverTime,OutDuty, ReferenceID,UserId,Remark,NightAllowCount};
                SQLOperation.forSaveValue("tblAttendanceRecord", getColumns, getValues, sqlDB.connection);
            }
            catch (Exception ex) { }
        }

        public static string [] Roster_Operation_TimeChecking(TimeSpan ShiftPunchStartTime,TimeSpan ShiftEndTime,TimeSpan LogInTime)
        {
            try
            {
                string[] Roster_Operation_Status = new string[2];
                if (LogInTime< ShiftPunchStartTime)
                {
                    Roster_Operation_Status[0] = "False";
                    Roster_Operation_Status[1] = "Please type valid shift start time .{0} is shift start punch count time " +ShiftPunchStartTime;
                    return Roster_Operation_Status;
                }
                else
                {
                    Roster_Operation_Status[0] = "True";
                    return Roster_Operation_Status;
                }
                    
                
            }
            catch { return null; }
        }
        public static void SaveAttendance_Status_OD(string EmpId, string selectedDate, string EmpTypeId, string InHour, string InMin, string InSec, string OutHour, string OutMin, string OutSec,
                                    string AttStatus, string StateStatus, string OverTime, string SftId, string DptId, string DsgId, string CompanyId, string GId, string LateTime, string StayTime, string DailyStartTimeALT_CloseTime, string AttManual, string TiffinCount, string HolidayCount, string PaybleDays, string OtherOverTime, string TotalOverTime, string OutDuty, string ReferenceID, string UserId, string Remark, string ODID)
        {
            try
            {
                deleteExistingAttendanceByDate_EmpId(selectedDate, EmpId); // for delete existing attendance record

                DateTime dtTimeConvert;
                LateTime = (LateTime == null) ? "00:00:00" : LateTime;
                if (DateTime.TryParse(LateTime, out dtTimeConvert)) LateTime = dtTimeConvert.ToString("HH:mm:ss");
                dtTimeConvert = new DateTime();
                StayTime = (StayTime == null) ? "00:00:00" : StayTime;
                //if (OutDuty == "1")
                //{
                TiffinCount = "1";
                PaybleDays = "1";
                // }

                if (DateTime.TryParse(StayTime, out dtTimeConvert)) StayTime = dtTimeConvert.ToString("HH:mm:ss");
                string[] getColumns = { "EmpId", "AttDate", "EmpTypeId", "InHour", "InMin", "InSec", "OutHour", "OutMin", "OutSec",
                                        "AttStatus", "StateStatus",
                                        "DailyStartTimeALT_CloseTime", "OverTime", "SftId", "DptId","DsgId", "CompanyId", "GId","LateTime","StayTime","AttManual","TiffinCount","HolidayCount","PaybleDays","OtherOverTime","TotalOverTime","OutDuty" ,"ReferenceID","UserId","Remark","ODID"};

                string[] getValues = { EmpId, convertDateTime.getCertainCulture(selectedDate).ToString(),
                                                 EmpTypeId,InHour,InMin,InSec,OutHour,OutMin,OutSec,AttStatus,
                                                 StateStatus,DailyStartTimeALT_CloseTime,OverTime,SftId,DptId,DsgId,CompanyId,GId,LateTime,StayTime,AttManual,TiffinCount,HolidayCount,PaybleDays,OtherOverTime,TotalOverTime,OutDuty, ReferenceID,UserId,Remark,ODID};
                SQLOperation.forSaveValue("tblAttendanceRecord", getColumns, getValues, sqlDB.connection);
            }
            catch { }
        }

    }
}