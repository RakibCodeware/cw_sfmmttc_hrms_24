using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SigmaERP.classes
{
    public static class commonAtt
    {
        
       
        public static string[] attendanceCalculation(DateTime logIn,DateTime logOut , TimeSpan RosterStartTime, TimeSpan RosterEndTime, byte AcceptableLate, byte OverTimeMin, TimeSpan TiffinTime, TimeSpan MinWorkingHours, TimeSpan MinOverTime, int Shortleave, bool isgerments, string BreakBeforeStartOTAsMin)
        {
            try
            {
                TimeSpan LogInTime = logIn.TimeOfDay;
                TimeSpan LogOutTime = logIn.TimeOfDay;
                TimeSpan RosterStartTimeForOT = RosterEndTime.Add(TimeSpan.FromMinutes(int.Parse(BreakBeforeStartOTAsMin)));// OT start after 30 minutes(Break)  of Shift End Time
                RosterStartTimeForOT = TimeSpan.Parse(RosterStartTimeForOT.Hours.ToString() + ":" + RosterStartTimeForOT.Minutes.ToString() + ":" + RosterStartTimeForOT.Seconds.ToString());
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

                string StayTime = "00:00:00";
                TimeSpan totalTime = TimeSpan.Parse("00:00:00");
                if (LogOutTime.ToString() != "00:00:00")
                {
                    TimeSpan k;
                    k = LogOutTime.Subtract(LogInTime);
                }

                    return DayStatus;
            }
            catch { return null; }
        }
        public static string[] getDeliverySectionsOT(DateTime logIn, DateTime logOut, TimeSpan MinWorkingHours, TimeSpan MinOverTime)
        {
            try
            {
                TimeSpan StayTime = TimeSpan.Parse("00:00:00");
                TimeSpan OverTime = TimeSpan.Parse("00:00:00");
                TimeSpan ExtraOverTime = TimeSpan.Parse("00:00:00");
                //TimeSpan TotalOverTime = TimeSpan.Parse("00:00:00");
                string[] DayStatus = new string[9];
                DayStatus[0] = "P";//Status
                DayStatus[1] = "Present";// StateStatus
                DayStatus[2] = "00:00:00";// OT
                DayStatus[3] = "00:00:00"; // Late Time
                DayStatus[4] = "00:00:00"; // Stay Time
                DayStatus[5] = "0"; //Tiffin Count
                DayStatus[6] = "0"; //PaybleDays
                DayStatus[7] = "00:00:00"; //OtherOverTime
                DayStatus[8] = "00:00:00"; //TotaOverTime
                StayTime = logOut.Subtract(logIn);
                DayStatus[4] = StayTime.ToString(); // Stay Time
                if (MinWorkingHours <= StayTime)
                {
                    DayStatus[6] = "1"; //PaybleDays
                    if (StayTime > TimeSpan.Parse("12:00:00"))
                    {
                        OverTime = StayTime - TimeSpan.Parse("12:00:00");// over time start after 12 hours
                        if (OverTime > TimeSpan.Parse("03:00:00"))// maxmimum over time 3 hours
                            OverTime = TimeSpan.Parse("03:00:00");

                        DayStatus[8] = OverTime.ToString(); //TotaOverTime
                        if (OverTime > MinOverTime)
                        {
                            ExtraOverTime = (OverTime - MinOverTime);
                            DayStatus[7] = ExtraOverTime.ToString(); //OtherOverTime
                            OverTime = MinOverTime;
                        }
                        DayStatus[2] = OverTime.ToString();// OT

                    }


                }
                

                

                return DayStatus;
            }
            catch { return null; }
            

        }
        public static string[] getDeliverySectionsOTManual(TimeSpan logInTime, TimeSpan logOutTime, TimeSpan MinWorkingHours, TimeSpan MinOverTime)
        {
            try
            {

                DateTime logIn;
                DateTime logOut;
                string inTT = "";
                string outTT = "";

                logIn = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + logInTime.ToString());
                logOut = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + logOutTime.ToString());
                inTT = logIn.ToString("tt");
                outTT = logOut.ToString("tt");
                if ((inTT == "PM" && outTT == "AM")|| (inTT == "AM" && outTT == "AM" && logOutTime < logInTime))
                    logOut= DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + logOutTime.ToString());
             
                    
                TimeSpan StayTime = TimeSpan.Parse("00:00:00");
                TimeSpan OverTime = TimeSpan.Parse("00:00:00");
                TimeSpan ExtraOverTime = TimeSpan.Parse("00:00:00");
                //TimeSpan TotalOverTime = TimeSpan.Parse("00:00:00");
                StayTime = logOut.Subtract(logIn);
                string[] DayStatus = new string[9];
                DayStatus[0] = "P";
                DayStatus[1] = "Present";
                DayStatus[2] = "00:00:00";
                DayStatus[4] = StayTime.ToString();
                DayStatus[3] = "00:00:00";  // OT             
                DayStatus[5] = "0";
                DayStatus[6] = "0"; //PaybleDays
                DayStatus[7] = "00:00:00"; //OtherOverTime;
                DayStatus[8] = "00:00:00"; //TotaOverTime;
                if (MinWorkingHours <= StayTime)
                {
                    DayStatus[6] = "1"; //PaybleDays

                    if (StayTime > AttendanceStaticInfo.dutyTimeForDelivery)
                    {
                        OverTime = StayTime - AttendanceStaticInfo.dutyTimeForDelivery;// over time start after 12 hours
                        if (OverTime > TimeSpan.Parse("03:00:00"))// maxmimum over time 3 hours
                            OverTime = TimeSpan.Parse("03:00:00");

                        DayStatus[8] = OverTime.ToString(); //TotaOverTime
                        if (OverTime > MinOverTime)
                        {
                            ExtraOverTime = (OverTime - MinOverTime);
                            DayStatus[7] = ExtraOverTime.ToString(); //OtherOverTime
                            OverTime = MinOverTime;
                        }
                        DayStatus[2] = OverTime.ToString();// OT

                    }


                }

               
               
                
                
                
               


                return DayStatus;
            }
            catch { return null; }


        }
        public static string[] getDeliverySectionsOTManual(TimeSpan logInTime, TimeSpan logOutTime,string attstatus, string StateStatus)// weekend and holiday
        {
            try
            {

                
                DateTime logIn;
                DateTime logOut;
                string inTT = "";
                string outTT = "";

                logIn = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + logInTime.ToString());
                logOut = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + logOutTime.ToString());
                inTT = logIn.ToString("tt");
                outTT = logOut.ToString("tt");
                if ((inTT == "PM" && outTT == "AM") || (inTT == "AM" && outTT == "AM" && logOutTime < logInTime))
                    logOut = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + logInTime.ToString());


                TimeSpan StayTime = TimeSpan.Parse("00:00:00");
                TimeSpan OverTime = TimeSpan.Parse("00:00:00");
                TimeSpan ExtraOverTime = TimeSpan.Parse("00:00:00");
                //TimeSpan TotalOverTime = TimeSpan.Parse("00:00:00");
                StayTime = logOut.Subtract(logIn);
                string[] WHO_DayStatus = new string[9];
                WHO_DayStatus[0] = attstatus;
                WHO_DayStatus[1] = StateStatus;
                WHO_DayStatus[2] = "00:00:00"; //StayTime.ToString(); //Get_OTHour;
                WHO_DayStatus[3] = "00:00:00";
                WHO_DayStatus[4] = StayTime.ToString();
                WHO_DayStatus[6] = "0";
                WHO_DayStatus[7] = "00:00:00"; //ExtraTime;
                WHO_DayStatus[8] = "00:00:00";// StayTime.ToString();




                return WHO_DayStatus;
            }
            catch { return null; }


        }
        public static string[] getDeliverySectionsOT(DateTime logIn, DateTime logOut)// weekend and holiday
        {
            try
            {
                TimeSpan StayTime = TimeSpan.Parse("00:00:00");
                TimeSpan OverTime = TimeSpan.Parse("00:00:00");
                TimeSpan ExtraOverTime = TimeSpan.Parse("00:00:00");
                //TimeSpan TotalOverTime = TimeSpan.Parse("00:00:00");
                string[] WHO_DayStatus = new string[7];
                StayTime = logOut.Subtract(logIn);
                WHO_DayStatus[0] = "00:00:00";// StayTime.ToString();//Get_OTHour;
                WHO_DayStatus[1] = "00:00:00";
                WHO_DayStatus[2] = StayTime.ToString();//StayTime;
                WHO_DayStatus[5] = "00:00:00";// ExtraTime;
                WHO_DayStatus[6] = "00:00:00"; //StayTime.ToString();// StayTime;

                return WHO_DayStatus;
            }
            catch { return null; }


        }
        //public static string  totalOverTimeAtWeekendHoliday(string attDate, TimeSpan logInTime, TimeSpan logOutTime,TimeSpan TotalOverTime)
        //{
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        DateTime logIn, logOut;
        //        logIn = DateTime.Parse( DateTime.Now.ToString("yyyy-MM-dd") + " " + logInTime.ToString());
        //        if(logInTime> logOutTime)
        //            logOut= DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + logOutTime.ToString());
        //        else
        //            logOut = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + logOutTime.ToString());
        //        TimeSpan totalBreakTime =TimeSpan.Parse("00:00:00");
        //        DateTime startTime;// = TimeSpan.Parse("00:00:00");
        //        DateTime endTime;// = TimeSpan.Parse("00:00:00");
        //        TimeSpan breakTime = TimeSpan.Parse("00:00:00");
        //        string sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay,Note  from v_AttBreakTime where BreakID is not null and FromDate<='"+ attDate + "' and ToDate>='"+ attDate + "'";
        //        sqlDB.fillDataTable(sqlcmd, dt=new DataTable());
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay from AttBreakTime where BreakID is null order by Ordering";
        //            sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
        //        }
        //        if (dt.Rows.Count > 0)
        //        {
        //            for (byte i=0;i<dt.Rows.Count;i++)
        //            {

        //                if (dt.Rows[i]["NextDay"].ToString().Equals("True"))
        //                {
        //                    startTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + dt.Rows[i]["StartTime"].ToString());
        //                    endTime = DateTime.Parse(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") + " " + dt.Rows[i]["EndTime"].ToString());
        //                }                        
        //                else
        //                {
        //                    startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + dt.Rows[i]["StartTime"].ToString());
        //                    endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " " + dt.Rows[i]["EndTime"].ToString());
        //                }

        //                breakTime = TimeSpan.Parse(dt.Rows[i]["BreakTime"].ToString());
        //                if (logIn <= startTime && logOut >= endTime)
        //                {
        //                    totalBreakTime += breakTime;
        //                }

        //            }

        //        }
        //        TotalOverTime = TotalOverTime - totalBreakTime;
        //        return TotalOverTime.ToString();
        //    }
        //    catch {return "00:00:00";}

        //}
     
        public static string totalOverTime(string EmpId, string attStatus,string shiftId, string attDate, TimeSpan logInTime, TimeSpan logOutTime, TimeSpan TotalOverTime)// this block specialy for ramdan overtime . create date: 19-05-2019
        {
            try
            {
                DataTable dt;
                string sqlcmd = "";
                if (!(attStatus == "W" || attStatus == "H"))
                {
                    if (HttpContext.Current.Session["__IsRegular__"].ToString().Equals("True"))
                    {
                        sqlcmd = "select SftEndTime from HRD_SpecialTimetable where StartDate<='" + attDate + "' and EndDate>='" + attDate + "'";
                        sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                        if (dt == null || dt.Rows.Count == 0)
                        {
                            sqlcmd = "select SftEndTime from HRD_Shift where SftId='"+ shiftId + "'";
                            sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                        }

                    }
                    else
                    {
                        sqlcmd = "select SftEndTime from v_ShiftTransferInfoDetails where SDate ='" + attDate + "' AND EmpId='" + EmpId + "'";
                        sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                    }
                    string SftEndTime = dt.Rows[0]["SftEndTime"].ToString();
                    logInTime = TimeSpan.Parse(SftEndTime);
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
                string IsHoliday = (attStatus == "W"|| attStatus == "H") ? "1" : "0";
                 sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay,Note  from v_AttBreakTime where IsActive=1 and BreakID is not null and IsHoliday=" + IsHoliday + " and FromDate<='" + attDate + "' and ToDate>='" + attDate + "'";
                sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                if (dt == null || dt.Rows.Count == 0 || !HttpContext.Current.Session["__IsRegular__"].ToString().Equals("True"))
                {
                    sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay from AttBreakTimeWithShift abs inner join AttBreakTime ab on abs.BrkID=ab.SL where SftID="+ shiftId+" order by Ordering";
                    sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        sqlcmd = "select Title,StartTime,EndTime,BreakTime,NextDay from AttBreakTime where IsActive=1 and BreakID is null and IsHoliday=" + IsHoliday + " order by Ordering";
                        sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
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

                    }

                }
                TotalOverTime = TotalOverTime - totalBreakTime;
                return TotalOverTime.ToString();
            }
            catch { return "00:00:00"; }

        }
        public static string[] checkOutDuty(string empID,DateTime attDate,string minimumWorkingTime, string[] status,bool hasPunch, DateTime rosterStartTime, DateTime rosterEndTime, DateTime inTime, DateTime outTime)
        {
            try
            {
                string sqlcmd="";                
                DataTable dt;                
                
                sqlcmd = "select SL, StraightFromHome,StraightToHome from tblOutDuty where Status=1 and EmpId='"+ empID + "' and Date='"+ attDate.ToString("yyyy-MM-dd") + "'";
                sqlDB.fillDataTable(sqlcmd, dt = new DataTable());
                if (dt != null && dt.Rows.Count > 0)
                {
                    string ODID= dt.Rows[0]["SL"].ToString();
                    bool StraightFromHome =bool.Parse(dt.Rows[0]["StraightFromHome"].ToString());
                    bool StraightToHome = bool.Parse(dt.Rows[0]["StraightToHome"].ToString());
                    status[0] = ODID;
                    if (StraightFromHome && StraightToHome)
                    {
                        status[1] = "P";
                        status[2] = "Present";
                        status[3] = "1";
                    }
                    else if (hasPunch)
                    {
                        if (StraightFromHome)
                        {
                            status[1] = "P";
                            status[2] = "Present";
                            if (outTime != null)
                            {
                                TimeSpan duration = (outTime - rosterStartTime);
                                if (duration >= TimeSpan.Parse(minimumWorkingTime))
                                    status[3] = "1";
                            }

                        }
                        else if (StraightToHome)
                        {
                            TimeSpan duration = (rosterEndTime - inTime);
                            if (duration >= TimeSpan.Parse(minimumWorkingTime))
                            {
                                if (status[1] == "A")
                                {
                                    status[1] = "P";
                                    status[2] = "Present";
                                }
                                status[3] = "1";
                            }
                        }
                    }
                }
                return status;
            }
            catch (Exception ex) { return null; }
        }

    }
}