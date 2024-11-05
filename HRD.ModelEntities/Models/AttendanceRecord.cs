using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRD.ModelEntities.Models
{
    public class AttendanceRecord : ICloneable
    {

        public string EmpId { get; set; }
        public DateTime AttDate { get; set; }
        public string EmpTypeId { get; set; }
        public string InHour { get; set; }
        public string InMin { get; set; }
        public string InSec { get; set; }
        public string OutHour { get; set; }
        public string OutMin { get; set; }
        public string OutSec { get; set; }
        public string AttStatus { get; set; }
        public string StateStatus { get; set; }
        
        public string SftId { get; set; }
        public string DptId { get; set; }
        public string DsgId { get; set; }
        public string CompanyId { get; set; }
        public string GId { get; set; }
        public string LateTime { get; set; }
        public string StayTime { get; set; }
        public string TiffinCount { get; set; }
        public string HolidayCount { get; set; }
        public string PaybleDays { get; set; }
        public string OverTime { get; set; }
        public string OtherOverTime { get; set; }
        public string TotalOverTime { get; set; }
        
        public string UserId { get; set; }
        public string NightAllowCount { get; set; }
        public int ODID { get; set; }
        public string TotalOverTimePre { get; set; }
        public string DbName { get; set; }
         


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
