using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SigmaERP.classes
{
    public static class AttendanceStaticInfo
    {
        // RSS HRM
        public static TimeSpan dutyTimeForDelivery = TimeSpan.Parse("10:30:00");// the regular duty time has been adjusted for the month of August 2023. The new regular duty time is 10:30:00, which is a change from the previous time of 12:00:00. RSS
        // End RSS HRM


        static DataTable dt;       
       // public static int DurationHours = 16;
        public static int DurationHours = 22;
        public static TimeSpan WorkingTime = TimeSpan.Parse("08:00:00");

        
        public static void getDevices(string CompanyID)
        {
            try
            {
                string InMachines = "", OutMachines="";                
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select DeviceSN,State from AttDevices where  IsActive=1 and CompanyID='"+ CompanyID+"'");
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (byte i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["State"].ToString() == "IN")
                            InMachines += ",'" + dt.Rows[i]["DeviceSN"].ToString() + "'";
                        else
                            OutMachines += ",'" + dt.Rows[i]["DeviceSN"].ToString() + "'";
                    }
                    if (InMachines.Length > 0)
                        HttpContext.Current.Session["__InMachines__"] = InMachines.Remove(0,1);
                    if (OutMachines.Length > 0)
                        HttpContext.Current.Session["__OutMachines__"] = OutMachines.Remove(0, 1);
                }
                else // marico 1
                {
                    //HttpContext.Current.Session["__InMachines__"] = "'AF4C204160428','AF4C204160416'";
                    //HttpContext.Current.Session["__OutMachines__"] = "'AF4C204160437'";

                    HttpContext.Current.Session["__InMachines__"] = "'CQUG225160092','CQUG225160147'";
                    HttpContext.Current.Session["__OutMachines__"] = "'CQUG225160142','CQUG225160138'";
                }

                
            }
            catch (Exception ex) {  }

        }
    }
}