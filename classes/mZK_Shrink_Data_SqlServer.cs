using adviitRuntimeScripting;
using ComplexScriptingSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SigmaERP.classes
{
    public class mZK_Shrink_Data_SqlServer
    {
        public SqlCommand cmd;
        // this method return an arry that is contain some data.DayStatus[0] = "True", DayStatus[1] = "W\H";
        public static string[] Check_Todays_Is_HolidayOrWeekend(string SelectedDate, string CompanyId)
        {
            try
            {
                string[] DayStatus = new string[2];
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select SL from Attendance_WeekendInfo where WeekendDate='" + SelectedDate + "' and CompanyId='" + CompanyId + "'", dt);
                if (dt.Rows.Count > 0)
                {
                    DayStatus[0] = "True";
                    DayStatus[1] = "W";
                    return DayStatus;
                }
                else
                {
                    dt = new DataTable();
                    sqlDB.fillDataTable("select HCode from tblHolydayWork where HDate='" + SelectedDate + "' and CompanyId='" + CompanyId + "'", dt);
                    if (dt.Rows.Count > 0)
                    {
                        DayStatus[0] = "True";
                        DayStatus[1] = "H";
                    }
                    else
                        DayStatus[0] = "False";
                    return DayStatus;
                }

            }
            catch { return null; }
        }
        public static string[] Check_Todays_Is_HolidayOrWeekend(string SelectedDate)
        {
            try
            {
                string[] DayStatus = new string[2];
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select HCode from tblHolydayWork where HDate='" + SelectedDate + "'", dt);
                if (dt.Rows.Count > 0)
                {
                    DayStatus[0] = "True";
                    DayStatus[1] = "H";
                    return DayStatus;
                }
                
                else
                {
                    dt = new DataTable();
                    sqlDB.fillDataTable("select SL from Attendance_WeekendInfo where WeekendDate='" + SelectedDate + "'", dt);
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
        public static DataTable LoadOTherSettings(string CompanyId)
        {
            DataTable dt=new DataTable();
            try
            {               
                sqlDB.fillDataTable("select * from HRD_OthersSetting where CompanyId='" + CompanyId + "'", dt);
                return dt;
            }
            catch { return dt; }
        }
        public static DataTable LoadShorLeave(string Empid,string date)
        {
            DataTable dt = new DataTable();
            try
            {
                sqlDB.fillDataTable("select * from Leave_ShortLeave where EmpId='" + Empid + "' and LvDate='" + date + "' and LvStatus='1'", dt);
                return dt;
            }
            catch { return dt; }
        }

        public static void SaveAttendance_Status(string EmpId, string selectedDate, string EmpTypeId, string InHour, string InMin, string InSec, string OutHour, string OutMin, string OutSec,
                                            string AttStatus, string StateStatus, string OverTime, string SftId, string DptId, string DsgId, string CompanyId, string GId, string LateTime, string StayTime, string DailyStartTimeALT_CloseTime, string TiffinCount, string HolidayCount, string PaybleDays, string OtherOverTime, string TotalOverTime, string UserId)
        {
            try
            {
                // RMS Attendance Saved Here
                DateTime dtTimeConvert;
                LateTime = (LateTime == null) ? "00:00:00" : LateTime;
                if (DateTime.TryParse(LateTime, out dtTimeConvert)) LateTime = dtTimeConvert.ToString("HH:mm:ss");
                dtTimeConvert = new DateTime();
                StayTime = (StayTime == null) ? "00:00:00" : StayTime;
               
                if (DateTime.TryParse(StayTime, out dtTimeConvert)) StayTime = dtTimeConvert.ToString("HH:mm:ss");
                string[] getColumns = { "EmpId", "AttDate", "EmpTypeId", "InHour", "InMin", "InSec", "OutHour", "OutMin", "OutSec",
                                        "AttStatus", "StateStatus",
                                        "DailyStartTimeALT_CloseTime", "OverTime", "SftId", "DptId","DsgId", "CompanyId", "GId","LateTime","StayTime","TiffinCount","HolidayCount","PaybleDays","OtherOverTime","TotalOverTime","UserId"};
                string[] d = selectedDate.Split('-');
                string[] getValues = { EmpId, d[2]+"-"+d[1]+d[0],
                                                 EmpTypeId,InHour,InMin,InSec,OutHour,OutMin,OutSec,AttStatus,
                                                 StateStatus,DailyStartTimeALT_CloseTime,OverTime,SftId,DptId,DsgId,CompanyId,GId,LateTime,StayTime,TiffinCount,HolidayCount,PaybleDays,OtherOverTime,TotalOverTime,UserId};
                try
                {
                    SQLOperation.forSaveValue("tblAttendanceRecord", getColumns, getValues, sqlDB.connection);
                }
                catch { }


                //------------------Attendance Punch time save in Punch log for manual att report by Nayem-----------
                SqlCommand cmd = new SqlCommand("delete tblAttendanceRecordPunchLog where EmpId='" + EmpId + "' and AttDate='" + d[2] + "-" + d[1] + d[0] + "' ", sqlDB.connection);
                cmd.ExecuteNonQuery();
                string[] getColumns1 = { "EmpId", "AttDate", "PInHour", "PInMin", "PInSec", "POutHour", "POutMin", "POutSec" };

                string[] getValues1 = { EmpId, d[2]+"-"+d[1]+d[0]
                                                 ,InHour,InMin,InSec,OutHour,OutMin,OutSec};
                SQLOperation.forSaveValue("tblAttendanceRecordPunchLog", getColumns1, getValues1, sqlDB.connection);
                //----------------------------------------------------------------------------------------------
            }
            catch { }
        }
   
        public static string SaveAttendance_Status(string EmpId, string selectedDate, string EmpTypeId, string InHour, string InMin, string InSec, string OutHour, string OutMin, string OutSec,
                                          string BreakStartTime,string BreakEndTime,string AttStatus, string StateStatus,string DailyStartTimeALT_CloseTime, string OverTime, string SftId, string DptId, string DsgId, string CompanyId,
                                          string GId, string LateTime, string StayTime, string TiffinCount, string HolidayCount,string PaybleDays,string OtherOverTime,string TotalOverTime,string IsDelivery,int ODID)
        {
            try
            {
                string[] d = selectedDate.Split('-');
                LateTime = (LateTime == null) ? "00:00:00" : LateTime;
                DateTime dtTimeConvert;
                if (DateTime.TryParse(LateTime, out dtTimeConvert)) LateTime = dtTimeConvert.ToString("HH:mm:ss");
                dtTimeConvert = new DateTime();

                StayTime = (StayTime == null) ? "00:00:00" : StayTime;

                string BreakStay_Time= (TimeSpan.Parse(BreakEndTime) - TimeSpan.Parse(BreakStartTime)).ToString();
                
                StayTime = (BreakStartTime.Contains("-"))?StayTime:(TimeSpan.Parse(StayTime) - TimeSpan.Parse(BreakEndTime)).ToString();

                if (DateTime.TryParse(StayTime, out dtTimeConvert)) StayTime = dtTimeConvert.ToString("HH:mm:ss");

                if (DateTime.TryParse(BreakStartTime, out dtTimeConvert)) BreakStartTime = dtTimeConvert.ToString("HH:mm:ss");
                if (DateTime.TryParse(BreakEndTime, out dtTimeConvert)) BreakEndTime = dtTimeConvert.ToString("HH:mm:ss");

                ////----Start Night Allow. Calculation by Nayem at 30/07/2017 ---------
                //string NightAllowCount = "0";
                //if (EmpTypeId == "2")
                //{
                //    TimeSpan _StayTime = TimeSpan.Parse(StayTime);
                //    if (_StayTime >= TimeSpan.Parse("15:45:00"))
                //        NightAllowCount = "1";
                //}
                ////---End Night Allow. Calculation--------------------
                //----Start Night,tiffin Allow. Calculation by Nayem at 30/01/2019 ---------
                string NightAllowCount = "0";
                TiffinCount = "0";
                if (!IsDelivery.Equals("True"))
                {
                    
                    if (TotalOverTime != "00:00:00")
                    {
                        TotalOverTime = commonAtt.totalOverTime(EmpId, AttStatus, SftId, d[2] + "-" + d[1] + "-" + d[0], TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
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
                    //    TotalOverTime = commonAtt.totalOverTimeAtWeekendHoliday(d[2] + "-" + d[1] + "-" + d[0],TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
                    //}
                    //else if (TotalOverTime != "00:00:00")
                    //{
                    //    TotalOverTime = commonAtt.totalOverTimeAtWeekendHoliday(d[2] + "-" + d[1] + "-" + d[0], TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
                    //}
                    
                }
                //---End Night Allow. Calculation--------------------
                if (AttStatus.ToLower() == "lv")// if leave then 
                {
                    InHour = "00"; InMin = "00"; InSec = "00"; OutHour = "00"; OutMin = "00"; OutSec = "00";
                    OverTime = "00:00:00";
                    OtherOverTime = "00:00:00";
                    TotalOverTime = "00:00:00";
                    LateTime = "00:00:00";
                    StayTime = "00:00:00";
                    TiffinCount = "0";
                    HolidayCount = "0";
                    NightAllowCount = "0";
                }                
                string[] getColumns = { "EmpId", "AttDate", "EmpTypeId", "InHour", "InMin", "InSec", "OutHour", "OutMin", "OutSec",
                                        "BreakStartTime","BreakEndTime","AttStatus", "StateStatus",
                                        "DailyStartTimeALT_CloseTime", "OverTime", "SftId", "DptId","DsgId", "CompanyId", "GId","LateTime","StayTime","TiffinCount","HolidayCount","PaybleDays","OtherOverTime","TotalOverTime" ,"NightAllowCount"};
                string[] getValues = { EmpId, d[2]+"-"+d[1]+"-"+d[0],
                                                 EmpTypeId,InHour,InMin,InSec,OutHour,OutMin,OutSec,BreakStartTime,BreakEndTime,AttStatus,
                                                 StateStatus,DailyStartTimeALT_CloseTime,OverTime,SftId,DptId,DsgId,CompanyId,GId,LateTime,StayTime,TiffinCount,HolidayCount,PaybleDays,OtherOverTime,TotalOverTime,NightAllowCount};
                if (ODID > 0)
                {
                    List<string> _getColumns = getColumns.ToList();
                    List<string> _getValues = getValues.ToList();
                    _getColumns.Add("ODID");
                    _getValues.Add(ODID.ToString());

                    getColumns = _getColumns.ToArray();
                    getValues = _getValues.ToArray();
                }
                try
                {
                    SQLOperation.forSaveValue("tblAttendanceRecord", getColumns, getValues, sqlDB.connection);
                    return "success";
                }
                catch (Exception ex) { mCommon_Module_For_AttendanceProcessing.NotCountableAttendanceLog(EmpId, ex.Message, d[2] + "-" + d[1] + "-" + d[0]); return ",SaveAttendance_Status->(252) ex:" + ex.Message; }
                
           
            }
            catch(Exception ex) { return ",SaveAttendance_Status->(256) ex:" + ex.Message; }
        }
        public void Store_In_Attendance_Log(string CompanyId, DateTime SelectedDate, bool ForAllEmployee, string DepartmentId, string EmpCardNo, string UserId)
        {
            try
            {
              
                DataTable dtEmpInfo =mCommon_Module_For_AttendanceProcessing.loadRunningEmployee(ForAllEmployee,CompanyId, DepartmentId, EmpCardNo);  // for load all running employee
                DataTable dt = new DataTable();

                classes.mCommon_Module_For_AttendanceProcessing.delete_Attendance(CompanyId, DepartmentId, SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee, dtEmpInfo.Rows[0]["EmpId"].ToString()); // delete existing attendance record by att date

                //--------------------------------------------- To konwing selected date is weekend or holyday ?---------------------------------------------
              string[] DayStatus =mZK_Shrink_Data_SqlServer.Check_Todays_Is_HolidayOrWeekend(SelectedDate.ToString("yyyy-MM-dd"));
                string TempDayStatus = DayStatus[1];
                DataTable dtOtherSettings = mZK_Shrink_Data_SqlServer.LoadOTherSettings(CompanyId);
               
                string[] othersetting=new string[8];
                if(dtOtherSettings.Rows.Count>0)
                {
                    othersetting[0]=dtOtherSettings.Rows[0]["WorkerTiffinHour"].ToString();
                    othersetting[1]=dtOtherSettings.Rows[0]["WorkerTiffinMin"].ToString();
                    othersetting[2]=dtOtherSettings.Rows[0]["StaffTiffinHour"].ToString();
                    othersetting[3]=dtOtherSettings.Rows[0]["StaffTiffinMin"].ToString();
                    othersetting[4] = dtOtherSettings.Rows[0]["StaffHolidayCount"].ToString();
                    othersetting[5] = dtOtherSettings.Rows[0]["MinWorkingHour"].ToString() + ":" + dtOtherSettings.Rows[0]["MinWorkingMin"].ToString() + ":00"; //Minimum Working Hours
                    othersetting[6] = dtOtherSettings.Rows[0]["StaffHolidayTotalHour"].ToString() + ":" + dtOtherSettings.Rows[0]["StaffHolidayTotalMin"].ToString() + ":00"; //Minimum Staff Working Hours For Holiday Allowance
                    othersetting[7] = dtOtherSettings.Rows[0]["MinOverTimeHour"].ToString() + ":" + dtOtherSettings.Rows[0]["MinOverTimeMin"].ToString() + ":00"; //Minimum OverTime
                }
                bool isgarments = classes.Payroll.Office_IsGarments();

                if (bool.Parse(DayStatus[0]))  // checking date is holiday or weekend ?. If date is weekend or holi day then execute this block
                {
                    for (int i = 0; i < dtEmpInfo.Rows.Count; i++)
                    {
                        DateTime joindate = DateTime.ParseExact(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        if (joindate > SelectedDate)
                        {
                            continue;
                        }
                        else
                        {
                            DataTable dtshortleave = mZK_Shrink_Data_SqlServer.LoadShorLeave(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                            dt = new DataTable();
                            string[] WHO_DayStatus = { "00:00:00", "00:00:00", "00:00:00", "0", "0", "00:00:00", "00:00:00" };
                            string worker = othersetting[0] + ":" + othersetting[1] + ":00";
                            string staff = othersetting[2] + ":" + othersetting[3] + ":00";
                            string tiffin = dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "1" ? worker : staff;
                            // if day is holiday or weekend and worker are punched then calculate overtime and find roster ..........................                         
                            string[] Roster_Info;                        
                       
                        if (dtEmpInfo.Rows[i]["EmpDutyType"].ToString().Equals("Roster"))
                        {

                            Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), false, "");
                            if (Roster_Info == null) mCommon_Module_For_AttendanceProcessing.NotCountableAttendanceLog(dtEmpInfo.Rows[i]["EmpId"].ToString(), "Rostering Problem", SelectedDate.ToString("MMM-dd-yyyy")); // This employee does not asigned in any roster.
                            else
                            {
                                if (!bool.Parse(Roster_Info[8])) // if this date is not set weekend or holyday for this roster duty type emplyee .then its counted as weekend or holiday  
                                {
                                    TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);
                                    string[] Leave_Info = Check_Any_Leave_Are_Exist(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                             //   sqlDB.fillDataTable("select distinct Badgenumber, FORMAT(CHECKTIME,'HH') as Hour,FORMAT(CHECKTIME,'mm') as Minute,FORMAT(CHECKTIME,'ss') as Second,FORMAT(CHECKTIME,'HH:mm:ss') as PunchTime from v_CHECKINOUT where CONVERT(date,CHECKTIME)='" + SelectedDate.ToString("yyyy-MM-dd") + "' AND Badgenumber Like '%" + dtEmpInfo.Rows[i]["EmpCardNo"].ToString() + "%'  order by CHECKTIME", dt);
                                sqlDB.fillDataTable("select distinct Badgenumber,FORMAT(CHECKTIME,'HH') as Hour,FORMAT(CHECKTIME,'mm') as Minute,FORMAT(CHECKTIME,'ss') as Second, FORMAT(CHECKTIME,'HH:mm:ss') as PunchTime from v_CHECKINOUT where CONVERT(date,CHECKTIME)='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7),CHECKTIME ) >='" + ShiftPunchCountStartTime + "' AND Badgenumber ='" + dtEmpInfo.Rows[i]["EmpCardNo"].ToString() + "'  order by PunchTime", dt);
                                if (dt.Rows.Count > 0)
                                {
                                    DataTable dtPunchList = new DataTable();

                                    DateTime tempDate = SelectedDate.AddDays(1);
                                    TimeSpan FirstPunch = TimeSpan.Parse(dt.Rows[0]["PunchTime"].ToString());
                                    //  byte TotalRows = (byte)dtPunchList.Rows.Count;
                                    TimeSpan LastPunch = TimeSpan.Parse("00:00:00");
                                    if (dt.Rows.Count > 1)
                                        LastPunch = TimeSpan.Parse(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString());
                                    DataTable dttemp;
                                    if ((Roster_Info[1].Equals("PM") && Roster_Info[2].Equals("AM")))
                                    {

                                        //sqlDB.fillDataTable("select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime from tblAttendance where PunchDate='" + tempDate.ToString("yyyy-MM-dd") + "' AND ProximityNo='" + _ProxymityNo + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>='00:00:00' and Hour<=11 and Minute<=59 and Second<= 59 order by Hour,Minute,Second ", dttemp = new DataTable());
                                        sqlDB.fillDataTable("select distinct Badgenumber,FORMAT(CHECKTIME,'HH') as Hour,FORMAT(CHECKTIME,'mm') as Minute,FORMAT(CHECKTIME,'ss') as Second, FORMAT(CHECKTIME,'HH:mm:ss') as PunchTime from v_CHECKINOUT where CONVERT(date,CHECKTIME)='" + tempDate.ToString("yyyy-MM-dd") + "' and convert(time(7),CHECKTIME ) >='00:00:00' and convert(time(7),CHECKTIME ) <='11:59:59' AND Badgenumber ='" + dtEmpInfo.Rows[i]["EmpCardNo"].ToString() + "'  order by PunchTime", dttemp = new DataTable());
                                        if (dttemp.Rows.Count > 0)
                                        {
                                            for (byte b = 0; b < dttemp.Rows.Count; b++)
                                            {
                                                dt.Rows.Add(dttemp.Rows[b]["Badgenumber"].ToString(), dttemp.Rows[b]["Hour"].ToString(), dttemp.Rows[b]["Minute"].ToString(),
                                                 dttemp.Rows[b]["Second"].ToString(), dttemp.Rows[b]["PunchTime"].ToString());
                                            }
                                            LastPunch = TimeSpan.Parse(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()); // to get out time of duty .so last punch are counted 


                                        }
                                    }

                                    WHO_DayStatus = OverTime_Calculation_ForWeekend_Holiday(FirstPunch, LastPunch, int.Parse(Roster_Info[5]), TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(tiffin), TimeSpan.Parse(othersetting[7]), dtshortleave.Rows.Count);
                                    if (dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "2" && TimeSpan.Parse(WHO_DayStatus[2]) >= TimeSpan.Parse(othersetting[6]))
                                    {
                                        WHO_DayStatus[4] = "1";
                                    }
                                    else
                                    {
                                        WHO_DayStatus[4] = "0";
                                    }
                                    if (!isgarments)
                                    {
                                        WHO_DayStatus[3] = "0";
                                    }
                                }
                                if (Leave_Info[0].ToString() != "0")
                                {
                                    DayStatus[0] = "Lv";
                                    DayStatus[1] = Leave_Info[1];
                                    classes.LeaveLibrary.LeaveCount(SelectedDate.ToString("yyyy-MM-dd"), Leave_Info[0]);
                                }
                                else
                                {
                                    DayStatus[0] = TempDayStatus;
                                    DayStatus[1] = (TempDayStatus == "W") ? "Weekend" : "Holiday";

                                }
                                // send parameter for count attendance 
                                SaveAttendance_Status(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("dd-MM-yyyy"), dtEmpInfo.Rows[i]["EmpTypeId"].ToString(),
                                    (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Length == 1) ? "0" + dt.Rows[0]["Hour"].ToString() : dt.Rows[0]["Hour"].ToString(),
                                    (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Minute"].ToString().Length == 1) ? "0" + dt.Rows[0]["Minute"].ToString() : dt.Rows[0]["Minute"].ToString(),
                                    (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Second"].ToString().Length == 1) ? "0" + dt.Rows[0]["Second"].ToString() : dt.Rows[0]["Second"].ToString(),
                                    (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : (dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Hour"].ToString() : dt.Rows[dt.Rows.Count - 1]["Hour"].ToString(),
                                    (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : (dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Minute"].ToString() : dt.Rows[dt.Rows.Count - 1]["Minute"].ToString(),
                                    (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : (dt.Rows[dt.Rows.Count - 1]["Second"].ToString().Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Second"].ToString() : dt.Rows[dt.Rows.Count - 1]["Second"].ToString(),
                                    DayStatus[0], DayStatus[1], WHO_DayStatus[0],
                                    Roster_Info[0].ToString(),
                                    dtEmpInfo.Rows[i]["DptId"].ToString(), dtEmpInfo.Rows[i]["DsgId"].ToString(),
                                    CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), WHO_DayStatus[1], WHO_DayStatus[2], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], WHO_DayStatus[3], WHO_DayStatus[4], "0", WHO_DayStatus[5], WHO_DayStatus[6], UserId);


                            }
                            else
                            {
                                zkRosterOperation_Shrink_Data.RoserOperationProcessing(SelectedDate, dtEmpInfo.Rows[i]["EmpId"].ToString(), byte.Parse(dtEmpInfo.Rows[i]["EmpTypeId"].ToString()), dtEmpInfo.Rows[i]["EmpCardNo"].ToString(),
                                    bool.Parse(Roster_Info[8]), Roster_Info[1], Roster_Info[2], true, TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(Roster_Info[4]), TimeSpan.Parse(Roster_Info[7]), Roster_Info[6], int.Parse(Roster_Info[5]), Roster_Info[0], dtEmpInfo.Rows[i]["DptId"].ToString(), dtEmpInfo.Rows[i]["DsgId"].ToString(), CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), Roster_Info[9], Roster_Info[10], bool.Parse(Roster_Info[11]), TimeSpan.Parse(tiffin), bool.Parse(othersetting[4]), isgarments,UserId);
                            }

                            }
                        }
                        else   // if employee type is regular then successfully executed this block
                        {
                            Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), true, dtEmpInfo.Rows[i]["SftId"].ToString());
                            string[] Leave_Info = Check_Any_Leave_Are_Exist(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                            TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);                         
                            sqlDB.fillDataTable("select distinct Badgenumber,FORMAT(CHECKTIME,'HH') as Hour,FORMAT(CHECKTIME,'mm') as Minute,FORMAT(CHECKTIME,'ss') as Second, FORMAT(CHECKTIME,'HH:mm:ss') as PunchTime from v_CHECKINOUT where CONVERT(date,CHECKTIME)='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7),CHECKTIME ) >='" + ShiftPunchCountStartTime + "' and Badgenumber ='" + dtEmpInfo.Rows[i]["EmpCardNo"].ToString() + "'  order by PunchTime", dt);
                            if (dt.Rows.Count > 0)
                            {
                                WHO_DayStatus = OverTime_Calculation_ForWeekend_Holiday(TimeSpan.Parse(dt.Rows[0]["PunchTime"].ToString()), TimeSpan.Parse(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()), int.Parse(Roster_Info[5]), TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(tiffin), TimeSpan.Parse(othersetting[7]), dtshortleave.Rows.Count);
                                if (dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "2" && TimeSpan.Parse(WHO_DayStatus[2]) >= TimeSpan.Parse(othersetting[6]))
                                {
                                    WHO_DayStatus[4] = "1";
                                }
                                else
                                {
                                    WHO_DayStatus[4] = "0";
                                }
                                if (!isgarments)
                                {
                                    WHO_DayStatus[3] = "0";
                                }
                            }
                            if (Leave_Info[0].ToString() != "0")
                            {
                                DayStatus[0] = "Lv";
                                DayStatus[1] = Leave_Info[1];
                                classes.LeaveLibrary.LeaveCount(SelectedDate.ToString("yyyy-MM-dd"), Leave_Info[0]);
                            }
                            else
                            {
                                DayStatus[0] = TempDayStatus;
                                DayStatus[1] = (TempDayStatus == "W") ? "Weekend" : "Holiday";

                            }
                        }
                        //------------------------------ End ------------------------------------------------------------------------------------

                        // send parameter for count attendance 
                        SaveAttendance_Status(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("dd-MM-yyyy"), dtEmpInfo.Rows[i]["EmpTypeId"].ToString(),
                            (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Length == 1) ? "0" + dt.Rows[0]["Hour"].ToString() : dt.Rows[0]["Hour"].ToString(),
                            (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Minute"].ToString().Length == 1) ? "0" + dt.Rows[0]["Minute"].ToString() : dt.Rows[0]["Minute"].ToString(),
                            (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Second"].ToString().Length == 1) ? "0" + dt.Rows[0]["Second"].ToString() : dt.Rows[0]["Second"].ToString(),
                            (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : (dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Hour"].ToString() : dt.Rows[dt.Rows.Count - 1]["Hour"].ToString(),
                            (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : (dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Minute"].ToString() : dt.Rows[dt.Rows.Count - 1]["Minute"].ToString(),
                            (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : (dt.Rows[dt.Rows.Count - 1]["Second"].ToString().Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Second"].ToString() : dt.Rows[dt.Rows.Count - 1]["Second"].ToString(),
                            DayStatus[0], DayStatus[1], WHO_DayStatus[0],
                            Roster_Info[0].ToString(),
                            dtEmpInfo.Rows[i]["DptId"].ToString(), dtEmpInfo.Rows[i]["DsgId"].ToString(),
                            CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), WHO_DayStatus[1], WHO_DayStatus[2], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], WHO_DayStatus[3], WHO_DayStatus[4], "0", WHO_DayStatus[5], WHO_DayStatus[6], UserId);

                         
                        } 

                }
                }//------------------------------------------------ End all weekend or holiday transaction--------------------------------------------
                else  // date is not holiday or weekend.that's mean working date then 
                {
                    for (int i = 0; i < dtEmpInfo.Rows.Count; i++)
                    {
                        DateTime joindate = DateTime.ParseExact(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        if (joindate > SelectedDate)
                        {
                            continue;
                        }
                        else
                        {
                            DataTable dtshortleave = mZK_Shrink_Data_SqlServer.LoadShorLeave(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                            DayStatus = new string[9];
                        string[] Roster_Info;
                      
                        if (dtEmpInfo.Rows[i]["EmpDutyType"].ToString().Equals("Roster"))   // if employee duty type is Roster then executed this block
                        {
                         
                            string worker = othersetting[0] + ":" + othersetting[1] + ":00";
                            string staff = othersetting[2] + ":" + othersetting[3] + ":00";
                            string tiffin = dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "1" ? worker : staff;
                            Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), false, "");

                            if (Roster_Info == null) mCommon_Module_For_AttendanceProcessing.NotCountableAttendanceLog(dtEmpInfo.Rows[i]["EmpId"].ToString(), "Rostering Problem", SelectedDate.ToString("MMM-dd-yyyy"));
                            else
                                zkRosterOperation_Shrink_Data.RoserOperationProcessing(SelectedDate, dtEmpInfo.Rows[i]["EmpId"].ToString(), byte.Parse(dtEmpInfo.Rows[i]["EmpTypeId"].ToString()), dtEmpInfo.Rows[i]["EmpCardNo"].ToString(),
                                    bool.Parse(Roster_Info[8]), Roster_Info[1], Roster_Info[2], true, TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(Roster_Info[4]), TimeSpan.Parse(Roster_Info[7]), Roster_Info[6], int.Parse(Roster_Info[5]), Roster_Info[0], dtEmpInfo.Rows[i]["DptId"].ToString(), dtEmpInfo.Rows[i]["DsgId"].ToString(), CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), Roster_Info[9], Roster_Info[10], bool.Parse(Roster_Info[11]), TimeSpan.Parse(tiffin), bool.Parse(othersetting[4]), isgarments,UserId);
                
                        }
                        else   // for regular duty type employee
                        {
                            Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), true, dtEmpInfo.Rows[i]["SftId"].ToString());
                            TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);
                         // for leave------------------------------------------------------------------------------------------------------------------
                             string[] Leave_Info = Check_Any_Leave_Are_Exist(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                            if (Leave_Info[0].ToString() != "0")  // check any type of leave. if are leave exists then execute this if block
                                {
                                    DayStatus[0] = "Lv";
                                    DayStatus[1] = Leave_Info[1];
                                    DayStatus[2] = "00:00:00";// OT
                                    DayStatus[3] = "00:00:00"; // Late Time
                                    DayStatus[4] = "00:00:00"; // Stay Time
                                    DayStatus[5] = "0"; //Tiffin Count
                                    DayStatus[6] = "0"; //PaybleDays
                                    DayStatus[7] = "00:00:00"; //OtherOverTime
                                    DayStatus[8] = "00:00:00"; //TotaOverTime

                                    classes.LeaveLibrary.LeaveCount(SelectedDate.ToString("yyyy-MM-dd"), Leave_Info[0]);
                                    dt = new DataTable();
                                } //End-----------------------------------------------------------------------------------------------------------------------
                            else // without leave---------------------------------------------------------------------------------------------------------
                            {
                                dt = new DataTable();
                               // sqlDB.fillDataTable("select distinct Badgenumber,FORMAT(CHECKTIME,'HH') as Hour,FORMAT(CHECKTIME,'mm') as Minute,FORMAT(CHECKTIME,'ss') as Second, FORMAT(CHECKTIME,'HH:mm:ss') as PunchTime from v_CHECKINOUT where CONVERT(date,CHECKTIME)='" + SelectedDate.ToString("yyyy-MM-dd") + "' AND Badgenumber Like '%" + dtEmpInfo.Rows[i]["EmpCardNo"].ToString() + "'  order by PunchTime", dt);
                                sqlDB.fillDataTable("select distinct Badgenumber,FORMAT(CHECKTIME,'HH') as Hour,FORMAT(CHECKTIME,'mm') as Minute,FORMAT(CHECKTIME,'ss') as Second, FORMAT(CHECKTIME,'HH:mm:ss') as PunchTime from v_CHECKINOUT where CONVERT(date,CHECKTIME)='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7),CHECKTIME ) >='" + ShiftPunchCountStartTime + "' and Badgenumber ='" + dtEmpInfo.Rows[i]["EmpCardNo"].ToString() + "'  order by PunchTime", dt);
                                if (dt.Rows.Count == 0)  // any punched are not exists of selectd day
                                {
                                    DayStatus[0] = "A";
                                    DayStatus[1] = "Absent";
                                    DayStatus[2] = "00:00:00";
                                    DayStatus[3] = "00:00:00"; // Late Time
                                    DayStatus[4] = "00:00:00"; // Stay Time
                                    DayStatus[5] = "0"; //Tiffin Count
                                    DayStatus[6] = "0"; //PaybleDays
                                    DayStatus[7] = "00:00:00"; //OtherOverTime
                                    DayStatus[8] = "00:00:00"; //TotaOverTime
                                }
                                else
                                {
                                    string worker = othersetting[0] + ":" + othersetting[1] + ":00";
                                    string staff = othersetting[2] + ":" + othersetting[3] + ":00";
                                    string tiffin = dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "1" ? worker : staff;
                                    string[] PresentDays_Status = OverTime_Calculation_ForRegularDuty(TimeSpan.Parse(dt.Rows[0]["PunchTime"].ToString()), TimeSpan.Parse(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()), TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(Roster_Info[4]), byte.Parse(Roster_Info[6]), byte.Parse(Roster_Info[5]), TimeSpan.Parse(tiffin), TimeSpan.Parse(othersetting[5]), TimeSpan.Parse(othersetting[7]), dtshortleave.Rows.Count, isgarments);
                                    DayStatus[0] = PresentDays_Status[0];
                                    DayStatus[1] = PresentDays_Status[1];
                                    DayStatus[2] = PresentDays_Status[2]; //OT
                                    DayStatus[3] = PresentDays_Status[3]; // Late Time
                                    DayStatus[4] = PresentDays_Status[4]; // Stay Time
                                    DayStatus[5] = PresentDays_Status[5];//Tiffin Count
                                    DayStatus[6] = PresentDays_Status[6]; //PaybleDays
                                    DayStatus[7] = PresentDays_Status[7]; //OtherOverTime
                                    DayStatus[8] = PresentDays_Status[8]; //TotaOverTime
                                }
                            }
                            string inHour = (dt.Rows.Count == 0) ? "00" : dt.Rows[0]["Hour"].ToString();
                            inHour = (inHour.Length == 1) ? "0" + dt.Rows[0]["Hour"].ToString() : inHour;
                            string inMinute = (dt.Rows.Count == 0) ? "00" : dt.Rows[0]["Minute"].ToString();
                            inMinute = (inMinute.Length == 0) ? "0" + dt.Rows[0]["Minute"].ToString() : inMinute;
                            string inSecond = (dt.Rows.Count == 0) ? "00" : dt.Rows[0]["Second"].ToString();
                            inSecond = (inSecond.Length == 0) ? "0" + dt.Rows[0]["Second"].ToString() : inSecond;
                            string outHour = (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Hour"].ToString();
                            outHour = (outHour.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Hour"].ToString() : outHour;
                            string outMinute = (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Minute"].ToString();
                            outMinute = (outMinute.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Minute"].ToString() : outMinute;
                            string outSecond = (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Second"].ToString();
                            outSecond = (outSecond.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Second"].ToString() : outSecond;
                            // send parameter for count attendance 
                            SaveAttendance_Status(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("dd-MM-yyyy"), dtEmpInfo.Rows[i]["EmpTypeId"].ToString(), inHour, inMinute, inSecond,
                                outHour, outMinute, outSecond, DayStatus[0], DayStatus[1].ToString(), DayStatus[2].ToString(), Roster_Info[0].ToString(), dtEmpInfo.Rows[i]["DptId"].ToString(),
                                dtEmpInfo.Rows[i]["DsgId"].ToString(), CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), DayStatus[3], DayStatus[4], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], DayStatus[5], "0", DayStatus[6], DayStatus[7], DayStatus[8], UserId);

                             }

                        }
                    }

                        
                }
                //---------------------------------------------------End--------------------------------------------------------------------------------------
            }
            catch { }
        }

        public static string[] Check_Any_Leave_Are_Exist(string SelectedDate, string EmpId)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] Leave_Info = new string[2];
                sqlDB.fillDataTable("select LACode,LeaveName from v_Leave_LeaveApplicationDetails where LeaveDate='" + SelectedDate + "' AND EmpId='" + EmpId + "' and IsApproved=1", dt);
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

        private static string[] GetRosterId(string SelectedDate, string EmpId, bool IsRegularDuty_Type, string ShiftId)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] Gt_RosterInfo = new string[12];
                if (IsRegularDuty_Type)
                    sqlDB.fillDataTable("select SftOverTime,SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,Format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime  from HRD_Shift where SftId ='" + ShiftId + "'", dt);

                else
                    sqlDB.fillDataTable("select SftOverTime,SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,IsWeekend,Format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime  from v_ShiftTransferInfoDetails where SDate ='" + SelectedDate + "' AND EmpId='" + EmpId + "'", dt);

                Gt_RosterInfo[0] = (dt.Rows.Count > 0) ? dt.Rows[0]["SftId"].ToString() : "0";
                Gt_RosterInfo[1] = dt.Rows[0]["SftStartTimeIndicator"].ToString();
                Gt_RosterInfo[2] = dt.Rows[0]["SftEndTimeIndicator"].ToString();
                Gt_RosterInfo[3] = dt.Rows[0]["SftStartTime"].ToString();
                Gt_RosterInfo[4] = dt.Rows[0]["SftEndTime"].ToString();
                Gt_RosterInfo[5] = dt.Rows[0]["AcceptableTimeAsOT"].ToString();
                Gt_RosterInfo[6] = dt.Rows[0]["SftAcceptableLate"].ToString();
                Gt_RosterInfo[7] = dt.Rows[0]["StartPunchCountTime"].ToString();
                Gt_RosterInfo[8] = (IsRegularDuty_Type) ? "False" : dt.Rows[0]["IsWeekend"].ToString();
                Gt_RosterInfo[9] = dt.Rows[0]["BreakStartTime"].ToString();
                Gt_RosterInfo[10] = dt.Rows[0]["BreakEndTime"].ToString();
                Gt_RosterInfo[11] = dt.Rows[0]["SftOverTime"].ToString();

                return Gt_RosterInfo;
            }
            catch { return null; }
        }
       
         public static string[] OverTime_Calculation_ForWeekend_Holiday(TimeSpan LogInTime, TimeSpan LogOutTime, int AcceptableOTMin, TimeSpan RosterStartTime, TimeSpan TiffinTime, TimeSpan MinOverTime, int shortleave)
        {
            try
            {

                string[] WHO_DayStatus = new string[7];
                string ExtraTime;
                string Get_OTHour;

                // int Get_OTMinute;

                // Get_OTHour = (Get_OTMinute > AcceptableOTMin) ? (byte)(int.Parse(Get_OTHour.ToString()) + 1) : Get_OTHour;

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

                // string StayTime = ((LogOutTime - LogInTime) < TimeSpan.Parse("00:00:00")) ? "00:00:00" : (LogOutTime - LogInTime).ToString();

                WHO_DayStatus[0] = Get_OTHour;
                WHO_DayStatus[1] = "00:00:00";
                WHO_DayStatus[2] = StayTime;
                WHO_DayStatus[5] = ExtraTime;
                WHO_DayStatus[6] = StayTime;
                //---------------------to get TiffineCount------------------
                if (StayTime == "00:00:00")
                {
                    WHO_DayStatus[0] = "00:00:00";
                    WHO_DayStatus[3] = "0";
                    WHO_DayStatus[5] = "00:00:00";
                    WHO_DayStatus[6] = "00:00:00";
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
                        //tiffinstaytime = LogOutTime - LogInTime;
                    }
                    if (tiffinstaytime >= TiffinTime && shortleave == 0)
                    {
                        WHO_DayStatus[3] = "1";
                    }
                    else
                    {
                        WHO_DayStatus[3] = "0";
                    }
                }

                return WHO_DayStatus;
            }
            catch { return null; }
        }
        
        public static string[] OverTime_Calculation_ForRegularDuty(TimeSpan LogInTime, TimeSpan LogOutTime, TimeSpan RosterStartTime, TimeSpan RosterEndTime, byte AcceptableLate, byte OverTimeMin, TimeSpan TiffinTime, TimeSpan MinWorkingHours, TimeSpan MinOverTime, int Shortleave, bool isgerments)
        {
            try
            {
                string[] DayStatus = new string[9];
                string LateTime = "00:00:00";
                string ExtraTime = "";
                string Get_OTHour = "";
                if (LogInTime <= RosterStartTime + TimeSpan.Parse("00:" + AcceptableLate.ToString() + ":00")) DayStatus[0] = "P";
                else if (LogInTime > RosterStartTime + TimeSpan.Parse("00:" + AcceptableLate.ToString() + ":00"))
                {
                    DayStatus[0] = "L";
                    LateTime = (LogInTime - RosterStartTime).ToString(); // to get late time                   
                }

                DayStatus[1] = "Present";

                TimeSpan totalTime = (LogOutTime - RosterEndTime);
                if (totalTime > MinOverTime)
                {
                    Get_OTHour = MinOverTime.ToString();
                    ExtraTime = (totalTime - MinOverTime).ToString();
                }

                else
                {
                    if (totalTime < TimeSpan.Parse("00:00:00"))
                    {
                        totalTime = TimeSpan.Parse("00:00:00");
                        Get_OTHour = "00:00:00";
                    }
                    else
                        Get_OTHour = totalTime.ToString();
                    ExtraTime = "00:00:00";
                }

                // int Get_OTMinutea = (int)TimeSpan.Parse(ExtraTime).Minutes;
                //int Get_OTMinute = (((int)TimeSpan.Parse(ExtraTime).Minutes) > 0) ? (int)TimeSpan.Parse(ExtraTime).Minutes : 0;

                //Get_OTHour = (Get_OTMinute > OverTimeMin) ? (byte)(int.Parse(Get_OTHour.ToString()) + 1) : Get_OTHour;

                //-------------- to get stay time---------------------------

                string StayTime = (LogOutTime - LogInTime).ToString();

                //----------------- end ------------------------------------


                DayStatus[2] = Get_OTHour;
                DayStatus[3] = LateTime;
                DayStatus[4] = StayTime;
                DayStatus[6] = "1";//Payble Day
                DayStatus[7] = ExtraTime;//otherovertime
                DayStatus[8] = totalTime.ToString();//otherovertime

                //---------------------to get TiffineCount------------------
                if (isgerments)
                {
                    if (StayTime == "00:00:00" || TimeSpan.Parse(StayTime) < MinWorkingHours)
                    {
                        DayStatus[2] = "00:00:00";//OT
                        DayStatus[5] = "0";//TiffinCount
                        DayStatus[6] = "0";//Payble Day
                        DayStatus[7] = "00:00:00";//otherovertime
                        DayStatus[8] = "00:00:00";//otherovertime
                    }
                    else
                    {
                        TimeSpan tiffinstaytime;
                        if (LogInTime < RosterStartTime)
                        {
                            tiffinstaytime = LogOutTime - RosterStartTime;
                        }
                        else
                        {
                            tiffinstaytime = LogOutTime - LogInTime;
                        }
                        if (tiffinstaytime >= TiffinTime && Shortleave == 0)
                        {
                            DayStatus[5] = "1";
                        }
                        else
                        {
                            DayStatus[5] = "0";
                        }
                    }
                    //-------------------end---------------------------------------


                    if (TimeSpan.Parse(LateTime) > TimeSpan.Parse("04:00:00"))
                    {
                        DayStatus[0] = "A";
                        DayStatus[1] = "Absent";
                        DayStatus[2] = "00:00:00";
                        DayStatus[3] = LateTime;
                        DayStatus[4] = StayTime;
                        DayStatus[5] = "0";
                        DayStatus[6] = "0";
                        DayStatus[7] = "00:00:00";
                        DayStatus[8] = "00:00:00";//otherovertime
                    }
                }
                else
                {
                    if (StayTime == "00:00:00")
                    {
                        DayStatus[2] = "00:00:00";//OT
                        DayStatus[5] = "0";//TiffinCount
                        DayStatus[6] = "0";//Payble Day
                        DayStatus[7] = "00:00:00";//otherovertime
                        DayStatus[8] = "00:00:00";//otherovertime
                    }
                    else
                    {

                        if (LogOutTime >= TiffinTime && Shortleave == 0)
                        {
                            DayStatus[5] = "1";
                        }
                        else
                        {
                            DayStatus[5] = "0";
                        }
                    }
                    //-------------------end---------------------------------------

                }

                return DayStatus;
            }
            catch { return null; }
        }
     
    }
}