using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SigmaERP.Models
{
    public class SalaryRecord:ICloneable
    {
        public string EmpId { get; set; }
        public string CompanyId { get; set; }
        public string SftId { get; set; }
        public string EmpCardNo { get; set; }
        public DateTime YearMonth { get; set; }
        public int DaysInMonth { get; set; }
        public int Activeday { get; set; }
        public int WeekendHoliday { get; set; }
        public int FestivalHoliday { get; set; }
        public int PayableDays { get; set; }
        public int CasualLeave { get; set; }
        public int SickLeave { get; set; }
        public int AnnualLeave { get; set; }
        public int OthersLeave { get; set; }
        public int ML { get; set; }
        public int LWP { get; set; }
        public int TotalLeave { get; set; }


        public int AbsentDay { get; set; }
        public int PresentDay { get; set; }

        public double EmpPresentSalary { get; set; }
        public double BasicSalary { get; set; }
        public double HouseRent { get; set; }
        public double MedicalAllownce { get; set; }
        public double ConvenceAllownce { get; set; }
        public double FoodAllownce { get; set; }
        public double TechnicalAllowance { get; set; }
        public double OthersAllownce { get; set; }
        public double LunchAllowance { get; set; }
        public double EmpNetGross { get; set; }          
        public double AdvanceDeduction { get; set; }
        public double LoanDeduction { get; set; }
        public double AbsentDeduction { get; set; }
        public double AttendanceBonus { get; set; }
        public double Payable { get; set; }

        public string OverTime { get; set; }
        public string TotalOverTime { get; set; }       
        public double OTRate { get; set; }
        public double OverTimeAmount { get; set; }
        public double TotalOTAmount { get; set; }

        public double NetPayable { get; set; }
        public double Stampdeduct { get; set; }
        public double TotalSalary { get; set; }
        public string DsgId { get; set; }
        public string DptId { get; set; }
        public string GrdName { get; set; }
        public int EmpTypeId { get; set; }
        public int EmpStatus { get; set; }
        public int UserId { get; set; }
        public string IsSeperationGeneration { get; set; }             
        
        public int LateDays { get; set; }
        public double LateFine { get; set; }
        public int TiffinDays { get; set; }
        public double TiffinTaka { get; set; }
        public double TiffinBillAmount { get; set; }
        public int HolidayWorkingDays { get; set; }
        public double HolidayTaka { get; set; }
        public double HoliDayBillAmount { get; set; }
        public double DormitoryRent { get; set; }
        public double ProvidentFund { get; set; }
        
        public double OthersPay { get; set; }
        public double OthersDeduction { get; set; }
        public double ProfitTax { get; set; }
        public double NightbilAmount { get; set; }
        public int NightBillDays { get; set; }
        
        public DateTime FromDateForAll { get; set; }
        public DateTime ToDateForAll { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime GenerateDate { get; set; }      
        public string EmpSeparationId { get; set; }      

       

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}