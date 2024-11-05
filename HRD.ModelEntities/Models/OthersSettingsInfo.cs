using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRD.ModelEntities.Models
{
    public class OthersSettingsInfo : IDisposable
    {

        public string EmpId { get; set; }
        public DateTime AttDate { get; set; }
        public int EmpTypeId { get; set; }
        public string InHour { get; set; }
        public string InMin { get; set; }
        public string InSec { get; set; }
        public string OutHour { get; set; }
        public string OutMin { get; set; }
        public string OutSec { get; set; }
        public string AttStatus { get; set; }
        public string StateStatus { get; set; }
        public string OverTime { get; set; }
        public int SftId { get; set; }
        public string DptId { get; set; }
        public string DsgId { get; set; }
        public string CompanyId { get; set; }
        public int GId { get; set; }
        public string LateTime { get; set; }
        public string StayTime { get; set; }
        public int TiffinCount { get; set; }
        public int HolidayCount { get; set; }
        public int PaybleDays { get; set; }
        public string OtherOverTime { get; set; }
        public string TotalOverTime { get; set; }
        public int UserId { get; set; }
        public int NightAllowCount { get; set; }


        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            disposed = true;
        }
    }
}