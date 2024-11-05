using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using ComplexScriptingSystem;
using System.Globalization;

namespace SigmaERP.classes
{
    public class mRMS_Shrink_data_MSAccess
    {
       public SqlCommand cmd;
             
        public static void SaveAttendance_Status(string EmpId,string selectedDate, string EmpTypeId,string InHour,string InMin,string InSec, string OutHour, string OutMin, string OutSec,
                                           string AttStatus, string StateStatus, string OverTime, string SftId, string DptId, string DsgId, string CompanyId, string GId, string LateTime, string StayTime, string DailyStartTimeALT_CloseTime, string TiffinCount, string HolidayCount,string PaybleDays,string OtherOverTime,string TotalOverTime,string UserId,string IsDelivery,int ODID)
        {
            //try
            //{
            // RMS Attendance Saved Here
            string[] d = selectedDate.Split('-');
            string sDate = d[2] + "-" + d[1] + "-" + d[0];
                DateTime dtTimeConvert;
                LateTime = (LateTime == null) ? "00:00:00" : LateTime;
                if (DateTime.TryParse(LateTime, out dtTimeConvert)) LateTime = dtTimeConvert.ToString("HH:mm:ss");
                dtTimeConvert = new DateTime();
                StayTime = (StayTime == null) ? "00:00:00" : StayTime;
            //    //----Start Night Allow. Calculation by Nayem at 30/07/2017 ---------
            //    string NightAllowCount = "0";
            //    if (EmpTypeId == "1")
            //    {
            //        TimeSpan _StayTime = TimeSpan.Parse(StayTime);
            //        if (_StayTime >= TimeSpan.Parse("15:00:00"))
            //            NightAllowCount = "1";
            //    }
            ////---End Night Allow. Calculation--------------------
            //----Start Night,tiffin Allow. Calculation by Nayem at 30/01/2019 ---------
            string NightAllowCount = "0";
            TiffinCount = "0";
            if (!IsDelivery.Equals("True"))
            {
                if (TotalOverTime != "00:00:00")
                {
                    TotalOverTime = commonAtt.totalOverTime(EmpId, AttStatus, SftId, sDate, TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
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
                //    TotalOverTime = commonAtt.totalOverTimeAtWeekendHoliday(sDate,TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
                //}
                //else if (TotalOverTime != "00:00:00")
                //{
                //    TotalOverTime = commonAtt.totalOverTimeAtWeekendHoliday(sDate, TimeSpan.Parse(InHour + ":" + InMin + ":" + InSec), TimeSpan.Parse(OutHour + ":" + OutMin + ":" + OutSec), TimeSpan.Parse(TotalOverTime));
                //}
                
            }
            //---End Night Allow. Calculation--------------------
            //------- sort leave check for late--------------
            if (AttStatus == "L")
                if (commonTask.notLateForShortleave(sDate, EmpId))
                    AttStatus = "P";
            if (TiffinCount == "1")
                if (commonTask.isShortleave(sDate, EmpId))
                    TiffinCount = "0";
            //------- End sort leave check for late----------
            if (DateTime.TryParse(StayTime, out dtTimeConvert)) StayTime = dtTimeConvert.ToString("HH:mm:ss");

            //  string SqlCmd = @" insert into tblAttendanceRecord ( EmpId, AttDate, EmpTypeId, InHour, InMin, InSec, OutHour, OutMin, OutSec,
            //                              AttStatus, StateStatus,
            //                              DailyStartTimeALT_CloseTime, OverTime, SftId, DptId,DsgId, CompanyId, GId,LateTime,StayTime,TiffinCount,HolidayCount,PaybleDays,OtherOverTime,TotalOverTime,UserId,NightAllowCount)
            //values('"+EmpId+"','"+ sDate + "','" + EmpTypeId + "','"+ InHour + "','" + InMin + "','" + InSec + "','" + OutHour + "','" + OutMin + "','" + 
            //                              OutSec + "','" + AttStatus + "','" + StateStatus + "','" + DailyStartTimeALT_CloseTime + "','" + OverTime + "','" + SftId + "','" + DptId + "','" + DsgId + "','" + CompanyId + "','" + 
            //                              GId + "','" + LateTime + "','" + StayTime + "','" + TiffinCount + "','" + HolidayCount + "','" + PaybleDays + "','" + OtherOverTime + "','" + 
            //                              TotalOverTime + "','" + UserId + "','" + NightAllowCount+"')";


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
                                        "AttStatus", "StateStatus",
                                        "DailyStartTimeALT_CloseTime", "OverTime", "SftId", "DptId","DsgId", "CompanyId", "GId","LateTime","StayTime","TiffinCount","HolidayCount","PaybleDays","OtherOverTime","TotalOverTime","UserId","NightAllowCount"};

            string[] getValues = { EmpId, sDate,
                                                 EmpTypeId,InHour,InMin,InSec,OutHour,OutMin,OutSec,AttStatus,
                                                 StateStatus,DailyStartTimeALT_CloseTime,OverTime,SftId,DptId,DsgId,CompanyId,GId,LateTime,StayTime,TiffinCount,HolidayCount,PaybleDays,OtherOverTime,TotalOverTime,UserId,NightAllowCount};
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
                //SqlCommand cmdatt = new SqlCommand(SqlCmd, sqlDB.connection);
                //cmdatt.ExecuteNonQuery();
                SQLOperation.forSaveValue("tblAttendanceRecord", getColumns, getValues, sqlDB.connection);
                }
                catch (Exception ex) {
                mCommon_Module_For_AttendanceProcessing.NotCountableAttendanceLog(EmpId, ex.Message, sDate); 
            }
                

                //------------------Attendance Punch time save in Punch log for manual att report by Nayem-----------
                SqlCommand cmd = new SqlCommand("delete tblAttendanceRecordPunchLog where EmpId='" + EmpId + "' and AttDate='" + sDate + "' ",sqlDB.connection);
                cmd.ExecuteNonQuery();
                string[] getColumns1 = { "EmpId", "AttDate", "PInHour", "PInMin", "PInSec", "POutHour", "POutMin", "POutSec"};

                string[] getValues1 = { EmpId,sDate,InHour,InMin,InSec,OutHour,OutMin,OutSec};
                SQLOperation.forSaveValue("tblAttendanceRecordPunchLog", getColumns1, getValues1, sqlDB.connection);
                //----------------------------------------------------------------------------------------------
            //}
            //catch { }
        }
      
               
        public void Store_In_Attendance_Log(string CompanyId,DateTime SelectedDate, FileUpload FileUploader,bool ForAllEmployee,string DepartmentId,string EmpCardNo,string UserId, Label lblErrorMessage)
        {
            try
            {
                string sqlCmd = "";
                 SQLOperation.forDelete("tblAttendance", sqlDB.connection);  // for clear full tblattendance table

                //------------------------------Connection with MSAccess database file and Retrived Data from table ---------------------------------
                
                string _ProxymityNo = "";
                // file saved in Server path Access file 
                lblErrorMessage.Text += ",78";
                string filename = "UNIS.mdb";
                            if (FileUploader.HasFile == true)
                            {
                                filename = Path.GetFileName(FileUploader.FileName);
                                File.Delete(HttpContext.Current.Server.MapPath("~/AccessFile/" + CompanyId + "") + filename);
                                FileUploader.SaveAs(HttpContext.Current.Server.MapPath("~/AccessFile/" + CompanyId + "") + filename);
                            }
                DataTable dtEmpInfo =mCommon_Module_For_AttendanceProcessing.RMSA_loadRunningEmployee(SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee,CompanyId, DepartmentId, EmpCardNo);  // for load all running employee

                OleDbConnection cont = new OleDbConnection();
                string getFilePaht = HttpContext.Current.Server.MapPath("//AccessFile//"+CompanyId+"" + filename);
                lblErrorMessage.Text += ",91";
                string connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + getFilePaht + ";Jet OLEDB:Database Password=unisamho";
                cont.ConnectionString = connection;
                cont.Open();
                lblErrorMessage.Text += ",95";
                OleDbDataAdapter da;
                if (ForAllEmployee)
                    da = new OleDbDataAdapter("select L_UID as card_no,C_Time as PanchTime,C_Date as d_card from tEnter where C_Date = '" + SelectedDate.ToString("yyyyMMdd") + "' or C_Date = '" + SelectedDate.AddDays(1).ToString("yyyyMMdd") + "' ", cont);  // here selecteddate format =yyyyMMdd
                else
                {
                    _ProxymityNo = ReturnEmpProximityNo(dtEmpInfo.Rows[0]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                    _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[0]["RealProximityNo"].ToString() : _ProxymityNo;
                    da = new OleDbDataAdapter("select L_UID as card_no,C_Time as PanchTime,C_Date as d_card from tEnter where (C_Date = '" + SelectedDate.ToString("yyyyMMdd") + "' or C_Date = '" + SelectedDate.AddDays(1).ToString("yyyyMMdd") + "') AND L_UID =" + _ProxymityNo + "", cont);  // here selecteddate format =yyyyMMdd
                }
                lblErrorMessage.Text += ",107";
                DataTable dt = new DataTable();
                da.Fill(dt);
                cont.Close();
                lblErrorMessage.Text += ",111("+dt.Rows.Count+")";
                //--------------------------------------------- End -----------------------------------------------------------------------------------------

                //----------------------------------------------- entered punch data into tblAttendance table------------------------------------------------
                foreach (DataRow dr in dt.Rows)
                {
                    string Date = dr["d_card"].ToString().Substring(0, 4) + "-" + dr["d_card"].ToString().Substring(4, 2) + "-" + dr["d_card"].ToString().Substring(6, 2);
                    cmd = new SqlCommand("insert into tblAttendance(ProximityNo, PunchDate, Hour, Minute,Second) " +
                        " values " +
                        "('" + dr["card_no"].ToString() + "','" + Date + "','" + dr["PanchTime"].ToString().Substring(0, 2) + "','" + dr["PanchTime"].ToString().Substring(2, 2) + "','" + dr["PanchTime"].ToString().Substring(4, 2) + "')", sqlDB.connection);
                    cmd.ExecuteNonQuery();
                }
                lblErrorMessage.Text += ",124";
                //--------------------------------------------------End--------------------------------------------------------------------------------------
                //==============================================   Data Restore Function Totally Cloased Here================================================


                // DataTable dtEmpInfo = loadRunningEmployee(ForAllEmployee, DepartmentId, EmpCardNo);  // for load all running employee
                classes.mCommon_Module_For_AttendanceProcessing.delete_Attendance(CompanyId, DepartmentId, SelectedDate.ToString("yyyy-MM-dd"), ForAllEmployee, dtEmpInfo.Rows[0]["EmpId"].ToString()); // delete existing attendance record by att date
               
                SQLOperation.forDelete("tblAttendance_NotCountableLogRecord", sqlDB.connection);  // for clear full tblAttendance_NotCountableLogRecord table
                
                //--------------------------------------------- To konwing selected date is weekend or holyday ?---------------------------------------------
                string[] DayStatus =mZK_Shrink_Data_SqlServer.Check_Todays_Is_HolidayOrWeekend(SelectedDate.ToString("yyyy-MM-dd"));
                
                string TempDayStatus = DayStatus[1];
                DataTable dtOtherSettings = mZK_Shrink_Data_SqlServer.LoadOTherSettings(CompanyId);
                
                string[] othersetting=new string[9];
             //   string BreakBeforeStartOTAsMin = "";
                if (dtOtherSettings.Rows.Count>0)
                {
                    othersetting[0]=dtOtherSettings.Rows[0]["WorkerTiffinHour"].ToString();
                    othersetting[1]=dtOtherSettings.Rows[0]["WorkerTiffinMin"].ToString();
                    othersetting[2]=dtOtherSettings.Rows[0]["StaffTiffinHour"].ToString();
                    othersetting[3]=dtOtherSettings.Rows[0]["StaffTiffinMin"].ToString();
                    othersetting[4] = dtOtherSettings.Rows[0]["StaffHolidayCount"].ToString();
                    othersetting[5] = dtOtherSettings.Rows[0]["MinWorkingHour"].ToString() + ":" + dtOtherSettings.Rows[0]["MinWorkingMin"].ToString() + ":00"; //Minimum Working Hours
                    othersetting[6] = dtOtherSettings.Rows[0]["StaffHolidayTotalHour"].ToString() + ":" + dtOtherSettings.Rows[0]["StaffHolidayTotalMin"].ToString() + ":00"; //Minimum Staff Working Hours For Holiday Allowance
                    othersetting[7] = dtOtherSettings.Rows[0]["MinOverTimeHour"].ToString() + ":" + dtOtherSettings.Rows[0]["MinOverTimeMin"].ToString() + ":00"; //Minimum OverTime
                    othersetting[8] = dtOtherSettings.Rows[0]["BreakBeforeStartOTAsMin"].ToString();
                     
                }
                lblErrorMessage.Text += ",152";
                //bool isgarments = classes.Payroll.Office_IsGarments();
                bool isgarments = true;// RSS Attendance processing like a germents.
                int ODID = 0;
                bool IsRegularWeekend = false;
                bool IsHoliday = false;
                //string GeneralDayInfo = commonTask.GeneralDayInfo(CompanyId, SelectedDate.ToString("yyyy-MM-dd"));
                //if (GeneralDayInfo != "")
                //{

                //}
                if (bool.Parse(DayStatus[0]))
                {
                    if (DayStatus[1].Equals("W"))
                        IsRegularWeekend = true;
                    else
                        IsHoliday = true;
                }        



                    if (IsHoliday)  // checking date is holiday?
                {

                    for (int i = 0; i < dtEmpInfo.Rows.Count; i++)
                    {
                        if (i == 156)
                        {

                        }
                        DateTime joindate = DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString())); //DateTime.ParseExact(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
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
                                    if (ForAllEmployee)
                                    {
                                        _ProxymityNo = ReturnEmpProximityNo(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                                        _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[i]["RealProximityNo"].ToString() : _ProxymityNo;
                                    }
                                    if (!bool.Parse(Roster_Info[8])) // if this date is not set weekend or holyday for this roster duty type emplyee .then its counted as weekend or holiday  
                                    {
                                        TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);
                                        TimeSpan ShiftPunchCountEndTime = TimeSpan.Parse(Roster_Info[12]);
                                        string[] Leave_Info = Check_Any_Leave_Are_Exist(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                                        if (ShiftPunchCountStartTime < ShiftPunchCountEndTime)
                                            sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where PunchDate='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>='" + ShiftPunchCountStartTime + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<='" + ShiftPunchCountEndTime + "'  AND ProximityNo='" + _ProxymityNo + "' order by Hour,Minute,Second ";
                                        else
                                            sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where   convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>= convert(datetime,'" + SelectedDate.ToString("yyyy-MM-dd") + " " + ShiftPunchCountStartTime + "') and convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<= convert(datetime,'" + SelectedDate.AddDays(1).ToString("yyyy-MM-dd") + " " + ShiftPunchCountEndTime + "') AND ProximityNo='" + _ProxymityNo + "' order by PunchDate, Hour,Minute,Second ";

                                        sqlDB.fillDataTable(sqlCmd, dt);
                                        if (dt.Rows.Count > 0)
                                        {
                                            DataTable dtPunchList = new DataTable();
                                            DateTime tempDate = SelectedDate.AddDays(1);
                                            TimeSpan FirstPunch = TimeSpan.Parse(dt.Rows[0]["PunchTime"].ToString());
                                            TimeSpan LastPunch = TimeSpan.Parse("00:00:00");
                                            if (dt.Rows.Count > 1)
                                                LastPunch = TimeSpan.Parse(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString());

                                            if (dtEmpInfo.Rows[i]["IsDelivery"].ToString().Equals("True"))
                                            {
                                                WHO_DayStatus = commonAtt.getDeliverySectionsOT(DateTime.Parse(dt.Rows[0]["PunchDate"].ToString() + " " + dt.Rows[0]["PunchTime"].ToString()), DateTime.Parse(dt.Rows[dt.Rows.Count - 1]["PunchDate"].ToString() + " " + dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()));
                                                WHO_DayStatus[4] = "1";
                                            }
                                            else
                                            {
                                                WHO_DayStatus = OverTime_Calculation_ForWeekend_Holiday(FirstPunch, LastPunch, int.Parse(Roster_Info[5]), TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(tiffin), TimeSpan.Parse(othersetting[7]), dtshortleave.Rows.Count);
                                                if (dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "2" && TimeSpan.Parse(WHO_DayStatus[2]) >= TimeSpan.Parse(othersetting[6]))
                                                {
                                                    WHO_DayStatus[4] = "1";
                                                }
                                                else
                                                {
                                                    WHO_DayStatus[4] = "0";
                                                }
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
                                            CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), WHO_DayStatus[1], WHO_DayStatus[2], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], WHO_DayStatus[3], WHO_DayStatus[4], "0", WHO_DayStatus[5], WHO_DayStatus[6], UserId, dtEmpInfo.Rows[i]["IsDelivery"].ToString(), ODID);

                                    }
                                    else
                                    {
                                        // this date is weekend or holiday but It's normal day for this employee then its counted as a successfully normal attendance status
                                        string BreakBeforeStartOTAsMin = (Roster_Info[13].ToString().Equals("True")) ? "0" : othersetting[8];
                                        mRosterOperation_Shrink_Data.RoserOperationProcessing(SelectedDate, dtEmpInfo.Rows[i]["EmpId"].ToString(), byte.Parse(dtEmpInfo.Rows[i]["EmpTypeId"].ToString()), dtEmpInfo.Rows[i]["EmpCardNo"].ToString(),
                                        bool.Parse(Roster_Info[8]), Roster_Info[1], Roster_Info[2], true, TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(Roster_Info[4]), TimeSpan.Parse(Roster_Info[7]), TimeSpan.Parse(Roster_Info[12]), Roster_Info[6], int.Parse(Roster_Info[5]), Roster_Info[0], dtEmpInfo.Rows[i]["DptId"].ToString(), dtEmpInfo.Rows[i]["DsgId"].ToString(), CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), Roster_Info[9], Roster_Info[10], _ProxymityNo, bool.Parse(Roster_Info[11]), TimeSpan.Parse(tiffin), bool.Parse(othersetting[4]), isgarments, BreakBeforeStartOTAsMin, dtEmpInfo.Rows[i]["IsDelivery"].ToString());
                                    }


                                }
                            }
                            else
                            {
                                if (ForAllEmployee)
                                {
                                    _ProxymityNo = ReturnEmpProximityNo(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                                    _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[i]["RealProximityNo"].ToString() : _ProxymityNo;
                                }
                                Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), true, dtEmpInfo.Rows[i]["SftId"].ToString());
                                string[] Leave_Info = Check_Any_Leave_Are_Exist(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                                TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);
                                TimeSpan ShiftPunchCountEndTime = TimeSpan.Parse(Roster_Info[12]);
                                if (ShiftPunchCountStartTime < ShiftPunchCountEndTime)
                                    sqlCmd = "select distinct ProximityNo,Hour,Minute,Second, Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where PunchDate='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>='" + ShiftPunchCountStartTime + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<='" + ShiftPunchCountEndTime + "'  AND ProximityNo='" + _ProxymityNo + "' order by Hour,Minute,Second ";
                                else
                                    sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where   convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>= convert(datetime,'" + SelectedDate.ToString("yyyy-MM-dd") + " " + ShiftPunchCountStartTime + "') and convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<= convert(datetime,'" + SelectedDate.AddDays(1).ToString("yyyy-MM-dd") + " " + ShiftPunchCountEndTime + "') AND ProximityNo='" + _ProxymityNo + "' order by PunchDate, Hour,Minute,Second ";
                                sqlDB.fillDataTable(sqlCmd, dt);
                                if (dt.Rows.Count > 0)
                                {
                                    if (dtEmpInfo.Rows[i]["IsDelivery"].ToString().Equals("True"))
                                        WHO_DayStatus = commonAtt.getDeliverySectionsOT(DateTime.Parse(dt.Rows[0]["PunchDate"].ToString() + " " + dt.Rows[0]["PunchTime"].ToString()), DateTime.Parse(dt.Rows[dt.Rows.Count - 1]["PunchDate"].ToString() + " " + dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()));
                                    else
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
                                // send parameter for count attendance 
                                lblErrorMessage.Text += ",387(after save " + dt.Rows.Count + "," + SelectedDate.ToString("dd-MM-yyyy") + ")";
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
                                    CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), WHO_DayStatus[1], WHO_DayStatus[2], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], WHO_DayStatus[3], WHO_DayStatus[4], "0", WHO_DayStatus[5], WHO_DayStatus[6], UserId, dtEmpInfo.Rows[i]["IsDelivery"].ToString(), ODID);

                            }
                            //------------------------------ End ------------------------------------------------------------------------------------
                        }
                    }
                }//------------------------------------------------ End all weekend or holiday transaction--------------------------------------------
                else  // date is not holiday  
                {
                    lblErrorMessage.Text += ",354(" + dtEmpInfo.Rows.Count + ")";
                    for (int i = 0; i < dtEmpInfo.Rows.Count; i++)
                    {
                        ODID = 0;
                        string inHour = "00", inMinute = "00", inSecond = "00", outHour = "00", outMinute = "00", outSecond = "00";
                        

                        DateTime joindate = DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString())); //DateTime.ParseExact(dtEmpInfo.Rows[i]["EmpJoiningDate"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                        if (joindate > SelectedDate)
                        {
                            continue;
                        }
                        else
                        {
                            bool IsWeekend = false;
                            if (dtEmpInfo.Rows[i]["WeekendType"].ToString().Equals("Roster"))
                            {
                                IsWeekend = commonTask.IsRosterWeekend(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                            }
                            else
                            {
                                IsWeekend = IsRegularWeekend;
                            }
                            if (IsWeekend)
                            {
                                TempDayStatus = "W";
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
                                        if (ForAllEmployee)
                                        {
                                            _ProxymityNo = ReturnEmpProximityNo(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                                            _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[i]["RealProximityNo"].ToString() : _ProxymityNo;
                                        }
                                        if (!bool.Parse(Roster_Info[8])) // if this date is not set weekend or holyday for this roster duty type emplyee .then its counted as weekend or holiday  
                                        {
                                            TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);
                                            TimeSpan ShiftPunchCountEndTime = TimeSpan.Parse(Roster_Info[12]);
                                            string[] Leave_Info = Check_Any_Leave_Are_Exist(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                                            if (ShiftPunchCountStartTime < ShiftPunchCountEndTime)
                                                sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where PunchDate='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>='" + ShiftPunchCountStartTime + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<='" + ShiftPunchCountEndTime + "'  AND ProximityNo='" + _ProxymityNo + "' order by Hour,Minute,Second ";
                                            else
                                                sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where   convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>= convert(datetime,'" + SelectedDate.ToString("yyyy-MM-dd") + " " + ShiftPunchCountStartTime + "') and convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<= convert(datetime,'" + SelectedDate.AddDays(1).ToString("yyyy-MM-dd") + " " + ShiftPunchCountEndTime + "') AND ProximityNo='" + _ProxymityNo + "' order by PunchDate, Hour,Minute,Second ";

                                            sqlDB.fillDataTable(sqlCmd, dt);
                                            if (dt.Rows.Count > 0)
                                            {
                                                DataTable dtPunchList = new DataTable();
                                                DateTime tempDate = SelectedDate.AddDays(1);
                                                TimeSpan FirstPunch = TimeSpan.Parse(dt.Rows[0]["PunchTime"].ToString());
                                                TimeSpan LastPunch = TimeSpan.Parse("00:00:00");
                                                if (dt.Rows.Count > 1)
                                                    LastPunch = TimeSpan.Parse(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString());

                                                if (dtEmpInfo.Rows[i]["IsDelivery"].ToString().Equals("True"))
                                                {
                                                    WHO_DayStatus = commonAtt.getDeliverySectionsOT(DateTime.Parse(dt.Rows[0]["PunchDate"].ToString() + " " + dt.Rows[0]["PunchTime"].ToString()), DateTime.Parse(dt.Rows[dt.Rows.Count - 1]["PunchDate"].ToString() + " " + dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()));
                                                    WHO_DayStatus[4] = "1";
                                                }
                                                else
                                                {
                                                    WHO_DayStatus = OverTime_Calculation_ForWeekend_Holiday(FirstPunch, LastPunch, int.Parse(Roster_Info[5]), TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(tiffin), TimeSpan.Parse(othersetting[7]), dtshortleave.Rows.Count);
                                                    if (dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "2" && TimeSpan.Parse(WHO_DayStatus[2]) >= TimeSpan.Parse(othersetting[6]))
                                                    {
                                                        WHO_DayStatus[4] = "1";
                                                    }
                                                    else
                                                    {
                                                        WHO_DayStatus[4] = "0";
                                                    }
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
                                                CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), WHO_DayStatus[1], WHO_DayStatus[2], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], WHO_DayStatus[3], WHO_DayStatus[4], "0", WHO_DayStatus[5], WHO_DayStatus[6], UserId, dtEmpInfo.Rows[i]["IsDelivery"].ToString(), ODID);

                                        }
                                        else
                                        {
                                            // this date is weekend or holiday but It's normal day for this employee then its counted as a successfully normal attendance status
                                            string BreakBeforeStartOTAsMin = (Roster_Info[13].ToString().Equals("True")) ? "0" : othersetting[8];
                                            mRosterOperation_Shrink_Data.RoserOperationProcessing(SelectedDate, dtEmpInfo.Rows[i]["EmpId"].ToString(), byte.Parse(dtEmpInfo.Rows[i]["EmpTypeId"].ToString()), dtEmpInfo.Rows[i]["EmpCardNo"].ToString(),
                                            bool.Parse(Roster_Info[8]), Roster_Info[1], Roster_Info[2], true, TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(Roster_Info[4]), TimeSpan.Parse(Roster_Info[7]), TimeSpan.Parse(Roster_Info[12]), Roster_Info[6], int.Parse(Roster_Info[5]), Roster_Info[0], dtEmpInfo.Rows[i]["DptId"].ToString(), dtEmpInfo.Rows[i]["DsgId"].ToString(), CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), Roster_Info[9], Roster_Info[10], _ProxymityNo, bool.Parse(Roster_Info[11]), TimeSpan.Parse(tiffin), bool.Parse(othersetting[4]), isgarments, BreakBeforeStartOTAsMin, dtEmpInfo.Rows[i]["IsDelivery"].ToString());
                                        }


                                    }
                                }
                                else
                                {
                                    if (ForAllEmployee)
                                    {
                                        _ProxymityNo = ReturnEmpProximityNo(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                                        _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[i]["RealProximityNo"].ToString() : _ProxymityNo;
                                    }
                                    Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), true, dtEmpInfo.Rows[i]["SftId"].ToString());
                                    string[] Leave_Info = Check_Any_Leave_Are_Exist(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString());
                                    TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);
                                    TimeSpan ShiftPunchCountEndTime = TimeSpan.Parse(Roster_Info[12]);
                                    if (ShiftPunchCountStartTime < ShiftPunchCountEndTime)
                                        sqlCmd = "select distinct ProximityNo,Hour,Minute,Second, Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where PunchDate='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>='" + ShiftPunchCountStartTime + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<='" + ShiftPunchCountEndTime + "'  AND ProximityNo='" + _ProxymityNo + "' order by Hour,Minute,Second ";
                                    else
                                        sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where   convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>= convert(datetime,'" + SelectedDate.ToString("yyyy-MM-dd") + " " + ShiftPunchCountStartTime + "') and convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<= convert(datetime,'" + SelectedDate.AddDays(1).ToString("yyyy-MM-dd") + " " + ShiftPunchCountEndTime + "') AND ProximityNo='" + _ProxymityNo + "' order by PunchDate, Hour,Minute,Second ";
                                    sqlDB.fillDataTable(sqlCmd, dt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        if (dtEmpInfo.Rows[i]["IsDelivery"].ToString().Equals("True"))
                                            WHO_DayStatus = commonAtt.getDeliverySectionsOT(DateTime.Parse(dt.Rows[0]["PunchDate"].ToString() + " " + dt.Rows[0]["PunchTime"].ToString()), DateTime.Parse(dt.Rows[dt.Rows.Count - 1]["PunchDate"].ToString() + " " + dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()));
                                        else
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
                                    // send parameter for count attendance 
                                    lblErrorMessage.Text += ",387(after save " + dt.Rows.Count + "," + SelectedDate.ToString("dd-MM-yyyy") + ")";
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
                                        CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), WHO_DayStatus[1], WHO_DayStatus[2], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], WHO_DayStatus[3],WHO_DayStatus[4], "0", WHO_DayStatus[5], WHO_DayStatus[6], UserId, dtEmpInfo.Rows[i]["IsDelivery"].ToString(), ODID);

                                }
                                //------------------------------ End ------------------------------------------------------------------------------------

                            }
                            else
                            { 

                                DataTable dtshortleave = mZK_Shrink_Data_SqlServer.LoadShorLeave(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                            DayStatus = new string[9];

                            string[] Roster_Info;
                            if (dtEmpInfo.Rows[i]["EmpDutyType"].ToString().Equals("Roster"))
                            {
                                string worker = othersetting[0] + ":" + othersetting[1] + ":00";
                                string staff = othersetting[2] + ":" + othersetting[3] + ":00";
                                string tiffin = dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "1" ? worker : staff;
                                Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), false, "");
                                if (Roster_Info == null) mCommon_Module_For_AttendanceProcessing.NotCountableAttendanceLog(dtEmpInfo.Rows[i]["EmpId"].ToString(), "Rostering Problem", SelectedDate.ToString("MMM-dd-yyyy"));
                                // calling roster operation function
                                else
                                {

                                    if (ForAllEmployee)
                                    {
                                        _ProxymityNo = ReturnEmpProximityNo(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                                        _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[i]["RealProximityNo"].ToString() : _ProxymityNo;
                                    }
                                    string BreakBeforeStartOTAsMin = (Roster_Info[13].ToString().Equals("True")) ? "0" : othersetting[8];
                                    lblErrorMessage.Text += "481";
                                    lblErrorMessage.Text +=  mRosterOperation_Shrink_Data.RoserOperationProcessing(SelectedDate, dtEmpInfo.Rows[i]["EmpId"].ToString(), byte.Parse(dtEmpInfo.Rows[i]["EmpTypeId"].ToString()), dtEmpInfo.Rows[i]["EmpCardNo"].ToString(),
                                    bool.Parse(Roster_Info[8]), Roster_Info[1], Roster_Info[2], true, TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(Roster_Info[4]), TimeSpan.Parse(Roster_Info[7]), TimeSpan.Parse(Roster_Info[12]), Roster_Info[6], int.Parse(Roster_Info[5]), Roster_Info[0], dtEmpInfo.Rows[i]["DptId"].ToString(), dtEmpInfo.Rows[i]["DsgId"].ToString(), CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), Roster_Info[9], Roster_Info[10], _ProxymityNo, bool.Parse(Roster_Info[11]), TimeSpan.Parse(tiffin), bool.Parse(othersetting[4]), isgarments, BreakBeforeStartOTAsMin, dtEmpInfo.Rows[i]["IsDelivery"].ToString());
                                }

                            }
                            else   // for regular duty type employee
                            {
                                Roster_Info = GetRosterId(SelectedDate.ToString("yyyy-MM-dd"), dtEmpInfo.Rows[i]["EmpId"].ToString(), true, dtEmpInfo.Rows[i]["SftId"].ToString());

                                TimeSpan ShiftPunchCountStartTime = TimeSpan.Parse(Roster_Info[7]);
                                TimeSpan ShiftPunchCountEndTime = TimeSpan.Parse(Roster_Info[12]);

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
                                    if (ForAllEmployee)
                                    {
                                        _ProxymityNo = ReturnEmpProximityNo(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("yyyy-MM-dd"));
                                        _ProxymityNo = (_ProxymityNo == "") ? dtEmpInfo.Rows[i]["RealProximityNo"].ToString() : _ProxymityNo;
                                    }
                                    dt = new DataTable();
                                    if (ShiftPunchCountStartTime < ShiftPunchCountEndTime)
                                        sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where PunchDate='" + SelectedDate.ToString("yyyy-MM-dd") + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>='" + ShiftPunchCountStartTime + "' and convert(time(7), Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<='" + ShiftPunchCountEndTime + "'  AND ProximityNo='" + _ProxymityNo + "' order by Hour,Minute,Second ";
                                    else
                                        sqlCmd = "select distinct ProximityNo,Hour,Minute,Second,Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second) as PunchTime,PunchDate from tblAttendance where   convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))>= convert(datetime,'" + SelectedDate.ToString("yyyy-MM-dd") + " " + ShiftPunchCountStartTime + "') and convert(datetime, Convert(varchar,PunchDate)+' '+Convert(varchar(2),Hour)+':'+CONVERT(varchar(2),Minute)+':'+Convert(varchar(2),Second))<= convert(datetime,'" + SelectedDate.AddDays(1).ToString("yyyy-MM-dd") + " " + ShiftPunchCountEndTime + "') AND ProximityNo='" + _ProxymityNo + "' order by PunchDate, Hour,Minute,Second ";

                                    sqlDB.fillDataTable(sqlCmd, dt);

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

                                        //--------------Outduty Checked-----
                                        string[] status = {ODID.ToString(), DayStatus[0], DayStatus[1], DayStatus[6] };
                                        string[] statusOutduty = commonAtt.checkOutDuty(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate, othersetting[5], status, false, DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + Roster_Info[3]), DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + Roster_Info[4]), DateTime.Now, DateTime.Now);
                                        if (statusOutduty != null && int.Parse(statusOutduty[0]) > 0)
                                        {
                                            ODID = int.Parse(statusOutduty[0]);
                                            DayStatus[0] = statusOutduty[1];
                                            DayStatus[1] = statusOutduty[2];
                                            DayStatus[6] = statusOutduty[3];
                                        }
                                        //--------------Outduty Checked-----
                                    }
                                    else
                                    {
                                        string worker = othersetting[0] + ":" + othersetting[1] + ":00";
                                        string staff = othersetting[2] + ":" + othersetting[3] + ":00";
                                        string tiffin = dtEmpInfo.Rows[i]["EmpTypeId"].ToString() == "1" ? worker : staff;
                                        string BreakBeforeStartOTAsMin = (Roster_Info[13].ToString().Equals("True")) ? "0" : othersetting[8];
                                        string[] PresentDays_Status = new string[9];
                                        if (dtEmpInfo.Rows[i]["IsDelivery"].ToString().Equals("True"))
                                            PresentDays_Status = commonAtt.getDeliverySectionsOT(DateTime.Parse(dt.Rows[0]["PunchDate"].ToString() + " " + dt.Rows[0]["PunchTime"].ToString()), DateTime.Parse(dt.Rows[dt.Rows.Count - 1]["PunchDate"].ToString() + " " + dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()), TimeSpan.Parse(othersetting[5]), TimeSpan.Parse(othersetting[7]));
                                        else
                                            PresentDays_Status = OverTime_Calculation_ForRegularDuty(TimeSpan.Parse(dt.Rows[0]["PunchTime"].ToString()), TimeSpan.Parse(dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()), TimeSpan.Parse(Roster_Info[3]), TimeSpan.Parse(Roster_Info[4]), byte.Parse(Roster_Info[6]), byte.Parse(Roster_Info[5]), TimeSpan.Parse(tiffin), TimeSpan.Parse(othersetting[5]), TimeSpan.Parse(othersetting[7]), dtshortleave.Rows.Count, isgarments, BreakBeforeStartOTAsMin);
                                        DayStatus[0] = PresentDays_Status[0];
                                        DayStatus[1] = PresentDays_Status[1];
                                        DayStatus[2] = PresentDays_Status[2]; //OT
                                        DayStatus[3] = PresentDays_Status[3]; // Late Time
                                        DayStatus[4] = PresentDays_Status[4]; // Stay Time
                                        DayStatus[5] = PresentDays_Status[5];//Tiffin Count
                                        DayStatus[6] = PresentDays_Status[6]; //PaybleDays
                                        DayStatus[7] = PresentDays_Status[7]; //OtherOverTime
                                        DayStatus[8] = PresentDays_Status[8]; //TotaOverTime

                                        inHour = dt.Rows[0]["Hour"].ToString();
                                        inHour = (inHour.Length == 1) ? "0" + dt.Rows[0]["Hour"].ToString() : inHour;
                                        inMinute = dt.Rows[0]["Minute"].ToString();
                                        inMinute = (inMinute.Length == 0) ? "0" + dt.Rows[0]["Minute"].ToString() : inMinute;
                                        inSecond = dt.Rows[0]["Second"].ToString();
                                        inSecond = (inSecond.Length == 0) ? "0" + dt.Rows[0]["Second"].ToString() : inSecond;

                                        outHour = (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Hour"].ToString();
                                        outHour = (outHour.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Hour"].ToString() : outHour;
                                        outMinute = (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Minute"].ToString();
                                        outMinute = (outMinute.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Minute"].ToString() : outMinute;
                                        outSecond = (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Second"].ToString();
                                        outSecond = (outSecond.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Second"].ToString() : outSecond;

                                        //--------------Outduty Checked-----
                                        string[] status = { ODID.ToString(), DayStatus[0], DayStatus[1], DayStatus[6] };
                                        string[] statusOutduty = commonAtt.checkOutDuty(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate, othersetting[5], status, true, DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + Roster_Info[3]), DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + Roster_Info[4]), DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + dt.Rows[0]["PunchTime"].ToString()), DateTime.Parse(SelectedDate.ToString("yyyy-MM-dd") + " " + dt.Rows[dt.Rows.Count - 1]["PunchTime"].ToString()));
                                        if (statusOutduty != null && int.Parse(statusOutduty[0]) > 0)
                                        {
                                            ODID = int.Parse(statusOutduty[0]);
                                            DayStatus[0] = statusOutduty[1];
                                            DayStatus[1] = statusOutduty[2];
                                            DayStatus[6] = statusOutduty[3];
                                        }
                                        //--------------Outduty Checked-----
                                    }
                                    // inHour = (dt.Rows.Count == 0) ? "00" : dt.Rows[0]["Hour"].ToString();
                                    // inHour = (inHour.Length == 1) ? "0" + dt.Rows[0]["Hour"].ToString() : inHour;
                                    // inMinute = (dt.Rows.Count == 0) ? "00" : dt.Rows[0]["Minute"].ToString();
                                    // inMinute = (inMinute.Length == 0) ? "0" + dt.Rows[0]["Minute"].ToString() : inMinute;
                                    // inSecond = (dt.Rows.Count == 0) ? "00" : dt.Rows[0]["Second"].ToString();
                                    // inSecond = (inSecond.Length == 0) ? "0" + dt.Rows[0]["Second"].ToString() : inSecond;
                                    // outHour = (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Hour"].ToString();
                                    // outHour = (outHour.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Hour"].ToString() : outHour;
                                    // outMinute = (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Minute"].ToString();
                                    //outMinute = (outMinute.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Minute"].ToString() : outMinute;
                                    // outSecond = (dt.Rows.Count == 0) ? "00" : (dt.Rows[0]["Hour"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Hour"].ToString().Trim() && dt.Rows[0]["Minute"].ToString().Trim() == dt.Rows[dt.Rows.Count - 1]["Minute"].ToString().Trim()) ? "00" : dt.Rows[dt.Rows.Count - 1]["Second"].ToString();
                                    //outSecond = (outSecond.Length == 1) ? "0" + dt.Rows[dt.Rows.Count - 1]["Second"].ToString() : outSecond;
                                    // send parameter for count attendance 

                                    

                                }

                                SaveAttendance_Status(dtEmpInfo.Rows[i]["EmpId"].ToString(), SelectedDate.ToString("dd-MM-yyyy"), dtEmpInfo.Rows[i]["EmpTypeId"].ToString(), inHour, inMinute, inSecond,
                                    outHour, outMinute, outSecond, DayStatus[0], DayStatus[1].ToString(), DayStatus[2].ToString(), Roster_Info[0].ToString(), dtEmpInfo.Rows[i]["DptId"].ToString(),
                                    dtEmpInfo.Rows[i]["DsgId"].ToString(), CompanyId, dtEmpInfo.Rows[i]["GId"].ToString(), DayStatus[3], DayStatus[4], Roster_Info[3] + ":" + Roster_Info[6] + ":" + Roster_Info[4], DayStatus[5], "0", DayStatus[6], DayStatus[7], DayStatus[8], UserId, dtEmpInfo.Rows[i]["IsDelivery"].ToString(), ODID);
                            }

                            }
                        }
                        //---------------------------------------------------End--------------------------------------------------------------------------------------
                    }
                    lblErrorMessage.Text += ",482";
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text += ex.Message;
            }
        }

        private string ReturnEmpProximityNo(string EmpId, string date)
        {
            try
            {
              
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select EmpProximityNo from Personnel_EmpProximityChange_Log  where EmpId='" + EmpId + "' and FromDate<='" + date + "' and ToDate>='" + date + "'", dt);
                if (dt.Rows.Count > 0)
                {
                 return  dt.Rows[0]["EmpProximityNo"].ToString();
                }
                else 
                return "";
            }
            catch { return ""; }
        }
        private static string[] Check_Any_Leave_Are_Exist(string SelectedDate,string EmpId)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] Leave_Info = new string[2];
                sqlDB.fillDataTable("select LACode,LeaveName from v_Leave_LeaveApplicationDetails where IsApproved=1 and LeaveDate='" + SelectedDate + "' AND EmpId='"+EmpId+"'", dt);
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

        private static string [] GetRosterId(string SelectedDate,string EmpId, bool IsRegularDuty_Type,string ShiftId)
        {
            try
            {
                HttpContext.Current.Session["__IsRegular__"] = IsRegularDuty_Type.ToString();
                string sqlCmd = "";
                DataTable dt;
                string[] Gt_RosterInfo = new string[14];
                if (IsRegularDuty_Type)
                {
                    sqlCmd = "select " + ShiftId + " as SftId, SftOverTime,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,EndPunchCountTime,format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime,IsNight  from HRD_SpecialTimetable where StartDate<='" + SelectedDate + "' and EndDate>='" + SelectedDate + "'";
                    sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        sqlCmd = "select SftOverTime,SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,EndPunchCountTime,format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime,IsNight  from HRD_Shift where SftId ='" + ShiftId + "'";
                        sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                    }

                }
                else
                {
                    sqlCmd = "select SftOverTime,SftId,SftStartTimeIndicator,SftEndTimeIndicator,SftStartTime,SftEndTime,SftAcceptableLate,AcceptableTimeAsOT,StartPunchCountTime,EndPunchCountTime,IsWeekend,Format(Cast(BreakStartTime as datetime),'HH:mm:ss') as BreakStartTime,Format(Cast(BreakEndTime as datetime),'HH:mm:ss') as BreakEndTime,IsNight  from v_ShiftTransferInfoDetails where SDate ='" + SelectedDate + "' AND EmpId='" + EmpId + "'";
                    sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                }
                    

                Gt_RosterInfo[0]=(dt.Rows.Count>0)?dt.Rows[0]["SftId"].ToString():"0";
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
                Gt_RosterInfo[12] = dt.Rows[0]["EndPunchCountTime"].ToString();
                Gt_RosterInfo[13] = dt.Rows[0]["IsNight"].ToString();


                return Gt_RosterInfo;
            }
            catch { return null; }
        }
        public static string[] OverTime_Calculation_ForWeekend_Holiday(TimeSpan LogInTime, TimeSpan LogOutTime, int AcceptableOTMin, TimeSpan RosterStartTime, TimeSpan TiffinTime,TimeSpan MinOverTime,int shortleave)
        {
            try
            {

                string[] WHO_DayStatus = new string[7];
                string ExtraTime;
                string Get_OTHour;
                string a = (DateTime.Today + LogInTime).ToString("HH:mm");
                string b = (DateTime.Today + LogOutTime).ToString("HH:mm");

                if ( a== b)
                    LogOutTime = LogInTime;

                DateTime time = DateTime.Today + LogInTime;
                String result = time.ToString("tt");
                TimeSpan k;
                if((DateTime.Today+LogInTime).ToString("tt")=="PM"&&(DateTime.Today+LogOutTime).ToString("tt")=="AM")
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
                if (LogOutTime.ToString() != "00:00:00")              
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
                        //tiffinstaytime = LogOutTime - LogInTime;
                    }
                    if (tiffinstaytime >= TiffinTime&&shortleave==0)
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
        public static string[] OverTime_Calculation_ForRegularDuty(TimeSpan LogInTime, TimeSpan LogOutTime, TimeSpan RosterStartTime, TimeSpan RosterEndTime, byte AcceptableLate, byte OverTimeMin, TimeSpan TiffinTime, TimeSpan MinWorkingHours, TimeSpan MinOverTime, int Shortleave, bool isgerments, string BreakBeforeStartOTAsMin)
        {
            try
            {
                // TimeSpan RosterStartTimeForOT = RosterEndTime.Add(TimeSpan.FromMinutes(int.Parse(BreakBeforeStartOTAsMin)));// OT start after 30 minutes(Break)  of Shift End Time
                TimeSpan RosterStartTimeForOT = RosterEndTime;
                RosterStartTimeForOT = TimeSpan.Parse(RosterStartTimeForOT.Hours.ToString() + ":" + RosterStartTimeForOT.Minutes.ToString() + ":" + RosterStartTimeForOT.Seconds.ToString());
                string[] DayStatus = new string[9];
                string LateTime = "00:00:00";
                string ExtraTime="";
                string Get_OTHour="";
                if (LogInTime <= RosterStartTime + TimeSpan.Parse("00:" + AcceptableLate.ToString() + ":00")) DayStatus[0] = "P";
                else if (LogInTime > RosterStartTime + TimeSpan.Parse("00:" + AcceptableLate.ToString() + ":00"))
                {
                    DayStatus[0] = "L";
                    LateTime = (LogInTime-RosterStartTime).ToString(); // to get late time                   
                }                    
                DayStatus[1] = "Present";

              
               

               // int Get_OTMinutea = (int)TimeSpan.Parse(ExtraTime).Minutes;
                //int Get_OTMinute = (((int)TimeSpan.Parse(ExtraTime).Minutes) > 0) ? (int)TimeSpan.Parse(ExtraTime).Minutes : 0;

                //Get_OTHour = (Get_OTMinute > OverTimeMin) ? (byte)(int.Parse(Get_OTHour.ToString()) + 1) : Get_OTHour;

                ////-------------- to get stay time---------------------------

                //string StayTime = (LogOutTime - LogInTime).ToString();

                ////----------------- end ------------------------------------

                //------ to get stay time-------Modifyed by nayem at 30/07/2017------------------------
              
                string StayTime = "00:00:00";
                TimeSpan totalTime = TimeSpan.Parse("00:00:00");
                if (LogOutTime.ToString() != "00:00:00")
                {                  
                    DateTime time = DateTime.Today + LogInTime;
                    String result = time.ToString("tt");
                    TimeSpan k;
                    if ((DateTime.Today + LogInTime).ToString("tt") == "PM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM")
                    {

                        k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(LogInTime.ToString()));
                        if (RosterStartTimeForOT < LogOutTime)
                            totalTime = (LogOutTime - RosterStartTimeForOT);

                    }
                    else if ((DateTime.Today + LogInTime).ToString("tt") == "AM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM" && (LogOutTime < LogInTime))
                    {

                        k = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(LogInTime.ToString()));
                       // OverTime = (!IsWHO_DaysTask) ? (DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(ShiftEndTime.ToString()))).ToString() : k.ToString();
                        try
                        {
                            if (RosterStartTimeForOT.Hours < 12)// AM
                            {
                                if (RosterStartTimeForOT < LogOutTime)
                                {
                                    
                                    totalTime = (LogOutTime - RosterStartTimeForOT);
                                }
                            }
                            else //PM
                            {
                                totalTime = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(RosterStartTimeForOT.ToString()));
                            }

                            //if (RosterStartTimeForOT < LogOutTime)
                            //    totalTime = (LogOutTime - RosterStartTimeForOT);
                            //else
                            //totalTime = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(RosterStartTimeForOT.ToString()));
                        }
                        catch (Exception ex)
                        {
                            
                        }
                        
                    }
                    else
                    {
                        //k = (LogOutTime - LogInTime);

                        //if (RosterStartTimeForOT < LogOutTime)
                        //{
                        //    totalTime = (LogOutTime - RosterStartTimeForOT);
                        //    if (k < totalTime)
                        //        totalTime = TimeSpan.Parse("00:00:00");
                        //}
                        k = (LogOutTime - LogInTime);
                        string RosterStartTimeForOT_tt = (DateTime.Today + RosterStartTimeForOT).ToString("tt");
                        string LogOutTime_tt = (DateTime.Today + LogOutTime).ToString("tt");
                        if (RosterStartTimeForOT_tt == "AM" && LogOutTime_tt == "PM")
                        {
                            totalTime = TimeSpan.Parse("00:00:00");
                        }
                        else
                        {
                            if (RosterStartTimeForOT < LogOutTime)
                                totalTime = (LogOutTime - RosterStartTimeForOT);
                        }

                    }
                    StayTime = k.ToString();
                }
                   
           
                //----------------- end stay time--------------------------------
                //----------OT Modifyed by Nayem at 02-08-2017---------------
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
                //----------End OT --------------------------------------------
                

                DayStatus[2] = Get_OTHour;
                DayStatus[3] = LateTime;
                DayStatus[4] = StayTime;
                DayStatus[6] = "1";//Payble Day
                DayStatus[7] = ExtraTime;//otherovertime
                DayStatus[8] = totalTime.ToString();//otherovertime
                
                //---------------------to get TiffineCount------------------
                if(isgerments)
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

                        if ((DateTime.Today + LogInTime).ToString("tt") == "PM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM")
                        {
                          
                            if (LogInTime < RosterStartTime)
                            {
                                tiffinstaytime = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(RosterStartTime.ToString()));// LogOutTime - RosterStartTime;
                            }
                            else
                            {
                                tiffinstaytime = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(LogInTime.ToString()));// LogOutTime - LogInTime;
                            }
                        }
                        else if ((DateTime.Today + LogInTime).ToString("tt") == "AM" && (DateTime.Today + LogOutTime).ToString("tt") == "AM" && (LogOutTime < LogInTime))
                        {
                           if (LogInTime < RosterStartTime)                            
                                tiffinstaytime = DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(RosterStartTime.ToString()));// LogOutTime - RosterStartTime;
                            
                            else                            
                                tiffinstaytime =DateTime.Parse(LogOutTime.ToString()).AddDays(1).Subtract(DateTime.Parse(LogInTime.ToString()));// LogOutTime - LogInTime;
                        }
                        else
                        {
                           if (LogInTime < RosterStartTime)                            
                            tiffinstaytime= LogOutTime - RosterStartTime;                            
                           else
                           tiffinstaytime= LogOutTime - LogInTime;
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
                        DayStatus[6] = "1";//Payble Day// 
                        DayStatus[7] = "00:00:00";//otherovertime
                        DayStatus[8] = "00:00:00";//otherovertime
                    }
                    else
                    {
                      if (LogOutTime >= TiffinTime && Shortleave==0)
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