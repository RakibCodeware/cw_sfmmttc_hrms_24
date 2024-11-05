using SigmaERP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SigmaERP.classes
{
    public class SalaryProcessing
    {
        string query = "";
        DataTable dt;
        SalaryRecord salaryRecord;
        public string  salaryProcessing(string IsSeperationGeneration, string UserId, string CompanyId,string EmpId,string SelectedDate,bool hasPF, bool hasSpesialGross, string PersentOfGross,bool hasAdvanceDeduction,bool hasStampDeduction,bool hasLateDeduction,string ExceptedEmpCardNo)
        {
            //Note: ProcessNo is 1 for Separation Employees and 0 for Regular Employees
            try
            {
                string errorData = "";
            string[] getDays = SelectedDate.Split('-');
            DateTime FromDate=DateTime.Parse( getDays[2] + "-" + getDays[1] + "-01");
            DateTime ToDate = DateTime.Parse(getDays[2] + "-" + getDays[1] + "-" + getDays[0]);
            
              
                // getting selected employees 
                DataTable dtEmployees = new DataTable();
                if (IsSeperationGeneration == "0")// regular employee 
                {
                    // check half month salary and set From Date            
                    FromDate = setFromDate(CompanyId, FromDate, ToDate);
                    dtEmployees = getEmployees(CompanyId, EmpId, ToDate.ToString("yyyy-MM-dd"));
                }                    
                else
                    dtEmployees = getSeparationEmployees(CompanyId, EmpId, ToDate.ToString("yyyy-MM"));

                if (dtEmployees != null && dtEmployees.Rows.Count > 0)
                {
                    
                    /// deleteing existing salary 
                    if (IsSeperationGeneration == "1")
                        salarySheetClearForSeparation(ToDate, CompanyId, EmpId);
                    else
                        salarySheetClear(ToDate, CompanyId, EmpId);

                // getting month info 
                dt = new DataTable();
                dt = getMonthInfo(CompanyId,ToDate.ToString("MM-yyyy"));
                int TotalDays= int.Parse(dt.Rows[0]["TotalDays"].ToString());              
                int Activeday = int.Parse(dt.Rows[0]["TotalWorkingDays"].ToString());

                //get stamp Amount
                double stampDeduct = (hasStampDeduction)?getStampDeduction():0;
                    //int count = 0;
                    //int countPost = 0;
                    //int countSuccess= 0;
                foreach (DataRow employee in dtEmployees.Rows)
                {
                        // excepted employee ignore here 
                        if (ExceptedEmpCardNo!= "")
                        {
                            string EmpCardNo = employee["EmpCardNo"].ToString().Substring(employee["EmpCardNo"].ToString().Length - 6);

                            if (ExceptedEmpCardNo.Contains(EmpCardNo))
                                continue;
                        }

                        //count++;
                    DateTime _FromDateForAll = FromDate;
                    DateTime _ToDateForAll = ToDate;
                    DateTime _FromDate = FromDate;
                    DateTime _ToDate = ToDate;
                    string _EmpSeparationId = "0";
                    if (IsSeperationGeneration == "1")// separation employee 
                    {
                        _EmpSeparationId =employee["EmpSeparationId"].ToString();
                        _ToDate = DateTime.Parse(employee["EffectiveDate"].ToString());
                    }
                        

                   // check new joining employees 
                    DateTime empJoiningDate = DateTime.Parse(employee["EmpJoiningDate"].ToString());
                    if (_FromDate < empJoiningDate)
                        _FromDate = empJoiningDate;
                    //initial 
                    try {
                            salaryRecord = new SalaryRecord
                            {
                                UserId = int.Parse(UserId),
                                EmpId = employee["EmpId"].ToString(),
                                EmpCardNo = employee["EmpCardNo"].ToString(),
                                EmpTypeId = int.Parse(employee["EmpTypeId"].ToString()),
                                EmpStatus = int.Parse(employee["EmpStatus"].ToString()),
                                CompanyId = employee["CompanyId"].ToString(),
                                DptId = employee["DptId"].ToString(),
                                DsgId = employee["DsgId"].ToString(),
                                GrdName = employee["GrdName"].ToString(),
                                SftId = employee["SftId"].ToString(),
                                EmpPresentSalary = double.Parse(employee["EmpPresentSalary"].ToString()),
                                EmpNetGross = 0,
                                BasicSalary = double.Parse(employee["BasicSalary"].ToString()),
                                HouseRent = double.Parse(employee["HouseRent"].ToString()),
                                MedicalAllownce = double.Parse(employee["MedicalAllownce"].ToString()),
                                ConvenceAllownce = double.Parse(employee["ConvenceAllownce"].ToString()),
                                FoodAllownce = double.Parse(employee["FoodAllownce"].ToString()),
                                TechnicalAllowance = double.Parse(employee["TechnicalAllownce"].ToString()),
                                OthersAllownce = double.Parse(employee["OthersAllownce"].ToString()),
                                AttendanceBonus=double.Parse(employee["AttendanceBonus"].ToString()),
                                DaysInMonth = TotalDays,
                                Activeday = Activeday,
                                WeekendHoliday = 0,
                                FestivalHoliday = 0,
                                AbsentDay = 0,
                                PresentDay = 0,
                                LateDays = 0,
                                PayableDays = 0,
                                CasualLeave = 0,
                                SickLeave = 0,
                                AnnualLeave = 0,
                                OthersLeave = 0,
                                LWP = 0,
                                AdvanceDeduction = 0,
                                AbsentDeduction = 0,
                                LateFine = 0,
                                ProvidentFund = 0,
                                ProfitTax = 0,
                                Payable = 0,
                                TiffinDays = 0,
                                TiffinTaka = 0,
                                TiffinBillAmount = 0,
                                HolidayWorkingDays = 0,
                                HolidayTaka = 0,
                                HoliDayBillAmount = 0,
                                NightbilAmount = 0,
                                NightBillDays = 0,
                                OTRate = 0,
                                OverTime = "00:00:00",
                                OverTimeAmount = 0,
                                TotalOverTime = "00:00:00",
                                TotalOTAmount = 0,
                                Stampdeduct = stampDeduct,
                                NetPayable = 0,
                                TotalSalary = 0,
                                YearMonth = DateTime.Parse(FromDate.ToString("yyyy-MM") + "-01"),
                                FromDate = _FromDate,
                                ToDate = _ToDate,
                                FromDateForAll = _FromDateForAll,
                                ToDateForAll = _ToDateForAll,
                                IsSeperationGeneration= IsSeperationGeneration,
                                EmpSeparationId= _EmpSeparationId

                            };
                    }
                    catch (Exception ex)
                        {
                            continue;
                        }
                    //getting Attendance Status (Leave,Absent,Present,Late) 
                    salaryRecord = getAttendanceStatus(salaryRecord);

                    //check Attendance bonus
                    salaryRecord = checkAttendanceBonus(salaryRecord, employee["EmpDutyType"].ToString());

                    //get Others Pay
                    //salaryRecord.OthersPay = getOthersPay(salaryRecord.EmpId);

                    //get Others Deduction
                    //salaryRecord.OthersPay = getOthersDeduction(salaryRecord.EmpId, salaryRecord.ToDate.ToString("MM-yyyy"));
                    
                    //get PF Deduction 
                    if (hasPF)
                        salaryRecord.ProvidentFund = getPF(employee,ToDate);
                    //get Advance Deduction
                    if (hasAdvanceDeduction)
                        salaryRecord.AdvanceDeduction = getAdvanceDeduction(salaryRecord.EmpId, salaryRecord.FromDate);
                    //get Tax Deduction
                    salaryRecord.ProfitTax =Round(double.Parse(employee["TaxAmount"].ToString()));
                    // get Late Deduction
                    if (hasLateDeduction)
                        salaryRecord.LateFine = getLateFine();
                   //get Punishment Deduction
                    salaryRecord.OthersDeduction = getPunishmentDeduction(salaryRecord.EmpId, salaryRecord.FromDate);

                        //OverTime 
                        if (employee["EmpTypeId"].ToString() == "1")// for worker 
                    {
                        salaryRecord = getOverTime(salaryRecord, employee);
                    }
                    //Payable days calculations 
                    salaryRecord = getPayableDaysCalculation(salaryRecord, hasSpesialGross, PersentOfGross);
                    //Payable amount calculation
                    salaryRecord = getNetPayableCalculation(salaryRecord, hasAdvanceDeduction);

                        if (saveSalary(salaryRecord))
                        {
                          //  countSuccess++;

                            if (salaryRecord.ProvidentFund > 0)
                                savePFRecord(salaryRecord.EmpId, salaryRecord.ToDate.ToString("yyyy-MM") + "-01", salaryRecord.ProvidentFund);
                            if (salaryRecord.ProfitTax > 0)
                                updateTaxRecord(salaryRecord.EmpId, salaryRecord.ToDate.ToString("yyyy-MM") + "-01");
                            if (salaryRecord.AdvanceDeduction > 0)
                                updateLoanStatus(salaryRecord.EmpId, salaryRecord.ToDate.ToString("yyyy-MM") + "-01");
                        }
                        else
                        {
                            errorData += "," + salaryRecord.EmpId;
                        }

                       // countPost++;
                }

                    //int check = count;
                    // check = countPost;
                    // check = countSuccess;

            }


                return errorData;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private DateTime setFromDate(string CompanyId,DateTime FromDate, DateTime ToDate)
        {
            dt = new DataTable();
            dt =CRUD.ExecuteReturnDataTable( "select Distinct convert(varchar(10), ToDate,120) as ToDate from Payroll_MonthlySalarySheet where IsSeperationGeneration=0  and CompanyId='" + CompanyId + "' and YearMonth='" + ToDate.ToString("yyyy-MM") + "-01' and ToDate<'" + ToDate.ToString("yyyy-MM-dd") + "'");           
            if (dt != null && dt.Rows.Count > 0)
            {
                return DateTime.Parse(dt.Rows[0]["ToDate"].ToString()).AddDays(1);
            }
            return FromDate;
        }
        private DataTable getEmployees(string CompanyId,string EmpId, string SelectDate)
        {

            EmpId = (EmpId == "0" )?"": " and EmpId='" + EmpId + "'";
             return CRUD.ExecuteReturnDataTable("select CompanyId,DptId,DsgId,GrdName,EmpId,EmpCardNo,EmpName,EmpType,EmpTypeId,EmpStatus,ActiveSalary,IsActive,CompanyId,SftId,OverTime,EmpDutyType,PfMember,convert(varchar(10), PfDate,120) as PfDate,ISNULL(PFAmount,0) as PFAmount,isnull(IncomeTax,0) as TaxAmount ,BasicSalary,MedicalAllownce,FoodAllownce,ConvenceAllownce,HouseRent,TechnicalAllownce,OthersAllownce,EmpPresentSalary,AttendanceBonus,LunchCount,LunchAllownce,convert(varchar(10), EmpJoiningDate,120) as EmpJoiningDate from v_Personnel_EmpCurrentStatus Where  EmpStatus in ('1','8') AND ActiveSalary='true' AND IsActive='1' AND CompanyId='" + CompanyId + "' and EmpJoiningDate<='" + SelectDate + "' "+EmpId);
        }
        private DataTable getSeparationEmployees(string CompanyId,string EmpId, string YearMonth)
        {

            EmpId = (EmpId == "0" )?"": " and s.EmpId='" + EmpId + "'";
             return CRUD.ExecuteReturnDataTable("select s.EmpSeparationId,c.CompanyId,DptId,DsgId,GrdName,c.EmpId,c.EmpCardNo,c.EmpName,c.EmpType,c.EmpTypeId,c.EmpStatus,ActiveSalary,c.IsActive,c.CompanyId,c.SftId,OverTime,EmpDutyType,PfMember,convert(varchar(10), PfDate,120) as PfDate,ISNULL(PFAmount,0) as PFAmount,isnull(IncomeTax,0) as TaxAmount ,BasicSalary,MedicalAllownce,FoodAllownce,ConvenceAllownce,HouseRent,TechnicalAllownce,OthersAllownce,EmpPresentSalary,AttendanceBonus,LunchCount,LunchAllownce,convert(varchar(10), EmpJoiningDate,120) as EmpJoiningDate,convert(varchar(10), s.EffectiveDate,120) as EffectiveDate from v_Personnel_EmpSeparation s inner join v_Personnel_EmpCurrentStatus as c on s.EmpId=c.EmpId and c.IsActive=1 and c.EmpStatus not in(1,8) where s.CompanyId ='" + CompanyId+"' AND YearMonth='"+ YearMonth + "' AND s.IsActive='True' "+EmpId);
        }
        private DataTable getMonthInfo(string  CompanyId,string Month)
        {
            return CRUD.ExecuteReturnDataTable("select TotalDays,TotalWeekend ,FromDate,ToDate,TotalHoliday,TotalWorkingDays from tblMonthSetup where CompanyId='" + CompanyId + "' and MonthName='" + Month + "'"); 
            
        }
        private SalaryRecord getAttendanceStatus(SalaryRecord salaryRecord)
        {
            //Leave 
            dt = new DataTable();
            dt=CRUD.ExecuteReturnDataTable("select EmpId,Sum(case when StateStatus='Casual Leave' then 1 else 0 end) as 'CL',Sum(case when StateStatus = 'Sick Leave' then 1 else 0 end) as 'SL',Sum(case when StateStatus = 'Annual Leave' then 1 else 0 end) as 'EL', Sum(case when StateStatus = 'Maternity Leave' then 1 else 0 end) as 'ML'," +
                " Sum(case when StateStatus = 'Leave Without Pay (LWP)' then 1 else 0 end) as 'LWP', count(EmpId) as Lv from v_tblAttendanceRecord where ATTStatus='lv'  AND EmpId='" + salaryRecord.EmpId + "' And AttDate >='" + salaryRecord.FromDate + "' AND AttDate <= '" + salaryRecord.ToDate + "' group by EmpId");
            if (dt!=null && dt.Rows.Count > 0)
            {             
                salaryRecord.CasualLeave = int.Parse(dt.Rows[0]["CL"].ToString());
                salaryRecord.SickLeave = int.Parse(dt.Rows[0]["SL"].ToString());
                salaryRecord.AnnualLeave = int.Parse(dt.Rows[0]["EL"].ToString());                
                salaryRecord.LWP = int.Parse(dt.Rows[0]["LWP"].ToString());
                salaryRecord.ML = int.Parse(dt.Rows[0]["ML"].ToString());
                salaryRecord.TotalLeave = int.Parse(dt.Rows[0]["Lv"].ToString());
                salaryRecord.OthersLeave = salaryRecord.TotalLeave - (salaryRecord.CasualLeave+salaryRecord.SickLeave+salaryRecord.AnnualLeave+salaryRecord.LWP+salaryRecord.ML);
               
            }
            //Present(Payable) 
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select distinct SftId, EmpId,Convert(varchar(11),ATTDate,111) as ATTDate,InHour,InMin,InSec,OutHour,OutMin,OutSec,ATTStatus from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND ATTStatus In ('P','L')  AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM-dd") + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' AND PaybleDays='1' ");
            if (dt != null && dt.Rows.Count > 0)
                salaryRecord.PresentDay = dt.Rows.Count;
            //Late(Payable) 
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate, EmpId from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND ATTStatus='L' AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM-dd") + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' AND PaybleDays='1' ");
            if (dt != null && dt.Rows.Count > 0)
                salaryRecord.LateDays = dt.Rows.Count;

            //Absent(not Payable) 
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND (ATTStatus='A' or StateStatus='Leave Without Pay (LWP)') AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM-dd") + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' Union select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND ATTStatus In ('P','L')  AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM") + '-' + "01" + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' AND PaybleDays='0' ");
            if (dt != null && dt.Rows.Count > 0)
                salaryRecord.AbsentDay = dt.Rows.Count;

            return salaryRecord;

        }

        private SalaryRecord getPayableDaysCalculation(SalaryRecord salaryRecord,bool hasSpesialGross,string PersentOfGross)
        {
            //WeekendHoliday
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from v_tblAttendanceRecord where ATTDate>='" + salaryRecord.FromDate.ToString("yyyy-MM-dd") + "' and  ATTDate<='" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' and EmpId='" + salaryRecord.EmpId + "' and ATTStatus='W' ");
            salaryRecord.WeekendHoliday = dt.Rows.Count;

            //FestivalHoliday
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from v_tblAttendanceRecord where ATTDate>='" + salaryRecord.FromDate.ToString("yyyy-MM-dd") + "' and  ATTDate<='" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' and EmpId='" + salaryRecord.EmpId + "' and ATTStatus='H' ");           
            salaryRecord.FestivalHoliday = dt.Rows.Count;

            double PresentSalary = salaryRecord.EmpPresentSalary;
            if (salaryRecord.FromDate.Day > 1 || salaryRecord.ToDate.Day < salaryRecord.DaysInMonth)
            {

                int TotalDays = (salaryRecord.ToDate.Day-salaryRecord.FromDate.Day) + 1;  // this line find out active days

                salaryRecord.Activeday = TotalDays - salaryRecord.WeekendHoliday - salaryRecord.FestivalHoliday;

                //-----------Get NetGross--------------------                
                PresentSalary = Round((salaryRecord.EmpPresentSalary / salaryRecord.DaysInMonth) * TotalDays);
                
                //-----------End Get NetGross----------------
            }  // else part is commented. becouse Active days for all regular employee is same, comes from Month setup.
            //else
            //{
            //    salaryRecord.Activeday = salaryRecord.DaysInMonth - salaryRecord.WeekendHoliday - salaryRecord.FestivalHoliday;
            //}
            if (hasSpesialGross)
            {              
                double percent = double.Parse(PersentOfGross);
                PresentSalary = PresentSalary * (percent / 100);
            }

            salaryRecord.EmpNetGross = PresentSalary;
            salaryRecord.PayableDays = salaryRecord.CasualLeave + salaryRecord.AnnualLeave + salaryRecord.SickLeave + salaryRecord.PresentDay + salaryRecord.WeekendHoliday + salaryRecord.FestivalHoliday;

            return salaryRecord;
        }
        private Double Round(Double Amount)
        {
            double frac = Amount % 1;
            if (frac >= 0.5)
                Amount = Math.Ceiling(Amount);
            else
                Amount = Math.Floor(Amount);
            return Amount;
        }
        private Double getLateFine()
        {
            //Late Deduction
            if (salaryRecord.LateDays > 2)
            {
                int LateFineDays = salaryRecord.LateDays / 3;
                // salaryRecord.LateFine = Round(salaryRecord.BasicSalary / 30 * LateFineDays) ;
               return  Round(salaryRecord.EmpPresentSalary / 30 * LateFineDays); //static for Mollah Fashion
            }
            return 0;

        }

        private SalaryRecord getNetPayableCalculation(SalaryRecord salaryRecord,bool ckbAdvanceDeduction)
        {

            
            // Absent Deduction
           // salaryRecord.AbsentDeduction =Round(salaryRecord.BasicSalary / 30 * salaryRecord.AbsentDay); //Always 30 days in month count for Absent Diduction at RSS
            salaryRecord.AbsentDeduction =Round(salaryRecord.EmpPresentSalary / 30 * salaryRecord.AbsentDay); //static for Mollah Fashion
            
            double totalDeductions = salaryRecord.LateFine + salaryRecord.AbsentDeduction + salaryRecord.AdvanceDeduction + salaryRecord.ProvidentFund + salaryRecord.ProfitTax + salaryRecord.OthersDeduction;
            //Payable
            salaryRecord.Payable = Round(salaryRecord.EmpNetGross- totalDeductions);
            // Attendance Bonus

            //NetPayable (with normal OT)
            salaryRecord.NetPayable = salaryRecord.Payable + salaryRecord.AttendanceBonus + salaryRecord.OverTimeAmount;
            //NetPayable (with actual OT)
            salaryRecord.TotalSalary = salaryRecord.Payable + salaryRecord.AttendanceBonus + salaryRecord.TotalOTAmount;
            if (salaryRecord.NetPayable > 0)
            {
                salaryRecord.NetPayable -= salaryRecord.Stampdeduct;
                salaryRecord.TotalSalary -= salaryRecord.Stampdeduct;
            }
            else
                salaryRecord.Stampdeduct = 0;

            return salaryRecord;
        }

        private SalaryRecord checkAttendanceBonus(SalaryRecord salaryRecord,string EmpDutyType)
        {
            if (salaryRecord.AbsentDay > 0 || salaryRecord.LateDays > 0 || salaryRecord.TotalLeave > 0)
                salaryRecord.AttendanceBonus = 0;
            else
            {
                //check absent 
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND (ATTStatus='A' or StateStatus='Leave Without Pay (LWP)')  AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM") + '-' + "01" + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd")+ "' Union select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND ATTStatus In ('P','L')  AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM") + '-' + "01" + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' AND PaybleDays='0' ");
                if (dt != null && dt.Rows.Count > 0)
                    salaryRecord.AttendanceBonus = 0;
                else
                {
                    //check leave 
                    dt = new DataTable();
                    dt = CRUD.ExecuteReturnDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate,EmpId,StateStatus from v_tblAttendanceRecord where ATTStatus='lv'  AND EmpId='" + salaryRecord.EmpId + "' And AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM") + '-' + "01" + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "'");
                    if (dt != null && dt.Rows.Count > 0)
                        salaryRecord.AttendanceBonus = 0;
                    else
                    {
                        //check late 
                        dt = new DataTable();
                        dt = CRUD.ExecuteReturnDataTable("select distinct convert(varchar(11),AttDate,111) as AttDate, EmpId from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND ATTStatus='L' AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM") + "-01' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' AND PaybleDays='1' ");
                        if (dt != null && dt.Rows.Count > 0)
                            salaryRecord.AttendanceBonus = 0;
                        else
                        {
                            
                            //check late 
                            dt = new DataTable();
                            dt = CRUD.ExecuteReturnDataTable("select distinct format(ATTDate,'yyyy-MM-dd') as WeekendDate from v_tblAttendanceRecord where  ATTDate>='" + salaryRecord.FromDate.ToString("yyyy-MM") + "-" + "01" + "' and  ATTDate<='" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' and EmpId='" + salaryRecord.EmpId + "' and ATTStatus in('W','H') ");
                            int totalDays = dt.Rows.Count;
                            dt = new DataTable();
                            dt = CRUD.ExecuteReturnDataTable("select distinct SftId, EmpId,Convert(varchar(11),ATTDate,111) as ATTDate,InHour,InMin,InSec,OutHour,OutMin,OutSec,ATTStatus from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND ATTStatus In ('P','L')  AND AttDate >='" + salaryRecord.FromDate.ToString("yyyy-MM") + '-' + "01" + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "' AND PaybleDays='1' ");
                            totalDays+= dt.Rows.Count;
                            if(totalDays!=salaryRecord.DaysInMonth)
                                salaryRecord.AttendanceBonus = 0;
                            else 
                            {                                
                                    for (int p = 0; p < dt.Rows.Count; p++)
                                    {
                                        DataTable dtTimeTable = new DataTable();
                                        DateTime Date = DateTime.Parse(dt.Rows[p]["ATTDate"].ToString());

                                        string _LoginTime = dt.Rows[p]["InHour"].ToString() + ":" + dt.Rows[p]["InMin"].ToString() + ":" + dt.Rows[p]["InSec"].ToString();
                                        if (EmpDutyType.Equals("Regular"))
                                        {
                                            dtTimeTable = new DataTable();
                                            dtTimeTable = CRUD.ExecuteReturnDataTable("select SftStartTime from HRD_SpecialTimetable where StartDate<= '" + Date.ToString("yyyy-MM-dd") + "' and  EndDate>= '" + Date.ToString("yyyy-MM-dd") + "'");

                                        }
                                        if (dtTimeTable == null || dtTimeTable.Rows.Count == 0)
                                        {
                                            dtTimeTable = new DataTable();
                                            dtTimeTable = CRUD.ExecuteReturnDataTable("select SftStartTime from HRD_Shift where SftId=" + dt.Rows[p]["SftId"].ToString());
                                        }
                                        if (dtTimeTable != null && dtTimeTable.Rows.Count > 0)
                                        {
                                            DateTime SftStartTime = DateTime.Parse(Date.ToString("yyyy-MM-dd") + " " + dtTimeTable.Rows[0]["SftStartTime"].ToString());
                                            DateTime LoginTime = DateTime.Parse(Date.ToString("yyyy-MM-dd") + " " + _LoginTime);
                                            if (SftStartTime < LoginTime)
                                            {
                                                salaryRecord.AttendanceBonus = 0;
                                                break;
                                            }
                                        }

                                    }
                            }
                           
                        }

                    }
                }

        
            }
            return salaryRecord;

        }

        private double getOthersPay(string EmpId)
        {
            dt = new DataTable();
            dt=CRUD.ExecuteReturnDataTable("select  ISNULL(Sum(OtherPay),0) OtherPay from Payroll_OthersPay where EmpId='" + EmpId + "' AND IsActive='1' ");
            return double.Parse(dt.Rows[0]["OtherPay"].ToString());
        }
        private double getStampDeduction()
        {
          
            dt = new DataTable();
            dt=CRUD.ExecuteReturnDataTable("select StampDeduct from HRD_AllownceSetting where AllownceId =(select max(AllownceId) from HRD_AllownceSetting)");
            return double.Parse(dt.Rows[0]["StampDeduct"].ToString());
        }
        private double getOthersDeduction(string EmpId,string MonthName)
        {
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select ISNULL(Sum(PAmount),0) PAmount from Payroll_Punishment where EmpId='" + EmpId + "' AND MonthName ='" + MonthName + "' ");
            return double.Parse(dt.Rows[0]["PAmount"].ToString());
        }
        private double getPF(DataRow employee,DateTime ToDate)
        {
            string _pfDate = employee["PfDate"].ToString();
            if (_pfDate.Trim().Length > 1)
            {
                string PFMember = employee["PfMember"].ToString();
                DateTime PfDate = DateTime.Parse(employee["PfDate"].ToString());
                if (PFMember.Equals("True") && PfDate <= ToDate)
                    return Round(double.Parse(employee["PFAmount"].ToString()));
               
            }
             return 0;
        }
        private double getAdvanceDeduction(string EmpId,DateTime FromDate)
        {
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select SL,LoanID,Month,Amount from Payroll_LoanMonthlySetup where Month='" +FromDate.ToString("yyyy-MM") + "- 01' and EmpId='" + EmpId + "'");
            if (dt != null && dt.Rows.Count > 0)
                return Round(double.Parse(dt.Rows[0]["Amount"].ToString()));
            return 0;
        }
        private double getPunishmentDeduction(string EmpId,DateTime FromDate)
        {
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select PAmount from Payroll_Punishment where EmpId='" + EmpId + "' and MonthName='"+ FromDate.ToString("MM-yyyy") + "'");
            if (dt != null && dt.Rows.Count > 0)
                return Round(double.Parse(dt.Rows[0]["PAmount"].ToString()));
            return 0;
        }
        private double getOTRate(double Salary)
        {
          //return Math.Round((Salary / 208) *2, 2); // here 208 is static.                
          return Math.Round((Salary * .005), 2); // 0.5 % of Gross for Mollah Fashion          
        }
        private double getOTAmout(string OverTime,double OTRate)
        {            
            string[] spltTime = OverTime.Split(':');
         
            double hours = double.Parse(spltTime[0]);
            double min = double.Parse(spltTime[1]);
            double secods = double.Parse(spltTime[2]);


      
            double secOttk = (OTRate / 3600) * secods;
            double minOttk = (OTRate / 60) * min;
            double hourlyot = OTRate * hours;
            return Round(secOttk+ minOttk+ hourlyot);             
        }
        private SalaryRecord getOverTime(SalaryRecord salaryRecord,DataRow employee)
        {
            //salaryRecord.OTRate = getOTRate(double.Parse(employee["BasicSalary"].ToString()));
            salaryRecord.OTRate = getOTRate(double.Parse(employee["EmpPresentSalary"].ToString())); // Gross for Mollah Fashion
            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable(@"DECLARE @maxOT VARCHAR(8) = '02:00:00' 
                                           Select  isnull(CAST(SUM(DATEDIFF(second, 0, case when ATTStatus='W' or ATTStatus='H' then '00:00:00' else case when TotalOverTime>@maxOT then  '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else TotalOverTime end end)) / 3600 AS varchar(12)) + ':' + RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, case when ATTStatus='W' or ATTStatus='H' then '00:00:00' else case when TotalOverTime>@maxOT then  '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else TotalOverTime end end)) / 60 % 60 AS varchar(2)), 2) + ':' +RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, case when ATTStatus='W' or ATTStatus='H' then '00:00:00' else case when TotalOverTime>@maxOT then  '02:0'+SUBSTRING(OutMin,2,1)+':'+OutSec else TotalOverTime end end)) % 60 AS varchar(2)), 2),'00:00:00') AS OverTime,isnull(CAST(SUM(DATEDIFF(second, 0, TotalOverTime)) / 3600 AS varchar(12)) + ':' + RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, TotalOverTime)) / 60 % 60 AS varchar(2)), 2) + ':' +RIGHT('0' + CAST(SUM(DATEDIFF(second, 0, TotalOverTime)) % 60 AS varchar(2)), 2),'00:00:00') AS TotalOverTime from v_tblAttendanceRecord where EmpId='" + salaryRecord.EmpId + "' AND AttDate >='" +salaryRecord.FromDate.ToString("yyyy-MM-dd") + "' AND AttDate <= '" + salaryRecord.ToDate.ToString("yyyy-MM-dd") + "'  and IsOverTime='1' and IsActive='1'");
            if (dt!=null && dt.Rows.Count > 0)
            {
                //// normal overtime as per compliance
                //salaryRecord.OverTime = dt.Rows[0]["OverTime"].ToString();
                //salaryRecord.OverTimeAmount = getOTAmout(salaryRecord.OverTime, salaryRecord.OTRate);

                // normal overtime as per Mollah Fassion
                salaryRecord.OverTime = dt.Rows[0]["TotalOverTime"].ToString();
                salaryRecord.OverTimeAmount = getOTAmout(salaryRecord.OverTime, salaryRecord.OTRate);


                //total overtime for regular
                salaryRecord.TotalOverTime = dt.Rows[0]["TotalOverTime"].ToString();
                salaryRecord.TotalOTAmount = getOTAmout(salaryRecord.TotalOverTime, salaryRecord.OTRate);              

            }
            return salaryRecord;
        }
        private bool saveSalary(SalaryRecord salaryRecord)
        {
           return  CRUD.Execute(@"insert into Payroll_MonthlySalarySheet(CompanyId,SftId,EmpId,EmpCardNo,YearMonth,DaysInMonth,Activeday,WeekendHoliday,PayableDays,CasualLeave,SickLeave,
                            AnnualLeave,OthersLeave,FestivalHoliday,AbsentDay,PresentDay,EmpPresentSalary,BasicSalary,HouseRent,MedicalAllownce,ConvenceAllownce,FoodAllownce,TechnicalAllowance,
                            OthersAllownce,AdvanceDeduction,AbsentDeduction,AttendanceBonus,Payable,OverTime,OverTimeAmount,TotalOTHour,OTRate,TotalOTAmount,NetPayable,Stampdeduct,
                            TotalSalary,DptId,DsgId,GrdName,EmpTypeId,EmpStatus,UserId,IsSeperationGeneration,GenerateDate,LateDays,LateFine,TiffinDays,TiffinTaka,TiffinBillAmount,HolidayWorkingDays,HolidayTaka,HoliDayBillAmount,ProvidentFund,
                            OthersPay,OthersDeduction,ProfitTax,NightbilAmount,NightBillDays,EmpNetGross,FromDate,ToDate,LWP,EmpSeparationId)
                            values('" + salaryRecord .CompanyId+@"',"+ salaryRecord.SftId + @",'"+ salaryRecord .EmpId+ @"','"+salaryRecord.EmpCardNo + @"','"+ salaryRecord.YearMonth.ToString("yyyy-MM-dd") + 
                            @"',"+salaryRecord.DaysInMonth+@","+salaryRecord.Activeday+@","+ salaryRecord.WeekendHoliday+ @","+ salaryRecord.PayableDays + @","+ salaryRecord.CasualLeave +
                            @"," + salaryRecord.SickLeave + @"," + salaryRecord.AnnualLeave + @"," + salaryRecord.OthersLeave + @"," + salaryRecord.FestivalHoliday + @"," + salaryRecord.AbsentDay +
                            @"," + salaryRecord.PresentDay + @",'" + salaryRecord.EmpPresentSalary + @"','" + salaryRecord.BasicSalary + @"','" + salaryRecord.HouseRent +
                            @"','" + salaryRecord.MedicalAllownce + @"','" + salaryRecord.ConvenceAllownce + @"','" + salaryRecord.FoodAllownce +
                            @"','" + salaryRecord.TechnicalAllowance + @"','" + salaryRecord.OthersAllownce + @"','" + salaryRecord.AdvanceDeduction + @"','" + salaryRecord.AbsentDeduction +
                            @"','" + salaryRecord.AttendanceBonus + @"','" + salaryRecord.Payable +"','"+ salaryRecord .OverTime+ @"','"+ salaryRecord.OverTimeAmount + @"','" + salaryRecord.TotalOverTime + @"','" + salaryRecord.OTRate + @"','" + salaryRecord.TotalOTAmount +
                            @"','" + salaryRecord.NetPayable + @"','" + salaryRecord.Stampdeduct + @"','" + salaryRecord.TotalSalary + @"','" + salaryRecord.DptId + @"','" + salaryRecord.DsgId + @"','" + salaryRecord.GrdName +
                            @"','" + salaryRecord.EmpTypeId + @"','" + salaryRecord.EmpStatus + @"','" + salaryRecord.UserId + @"',"+ salaryRecord .IsSeperationGeneration+ ",'" + salaryRecord.GenerateDate.ToString("yyyy-MM-dd") +
                            @"','" + salaryRecord.LateDays + @"','" + salaryRecord.LateFine + @"','" + salaryRecord.TiffinDays + @"','" + salaryRecord.TiffinTaka +
                            @"','" + salaryRecord.TiffinBillAmount + @"','" + salaryRecord.HolidayWorkingDays + @"','" + salaryRecord.HolidayTaka + @"','" + salaryRecord.HoliDayBillAmount + @"','" + salaryRecord.ProvidentFund +
                            @"','" + salaryRecord.OthersPay + @"','" + salaryRecord.OthersDeduction + @"','" + salaryRecord.ProfitTax + @"','" + salaryRecord.NightbilAmount +
                            @"','" + salaryRecord.NightBillDays + @"','" + salaryRecord.EmpNetGross + @"','" + salaryRecord.FromDateForAll.ToString("yyyy-MM-dd") + @"','" + salaryRecord.ToDateForAll.ToString("yyyy-MM-dd") + @"','" + salaryRecord.LWP + @"',"+ salaryRecord.EmpSeparationId + ")");

            
         
        }
        private void savePFRecord(string EmpId, string YearMonth,double PFAmount)
        {
            try
            {
                CRUD.Execute("Delete PF_PFRecord where convert(varchar(10), Month,120)='" + YearMonth + "' and EmpID='" + EmpId + "'");
                CRUD.Execute("insert into PF_PFRecord values('" + EmpId + "','" + YearMonth + "','" +
                   PFAmount + "','" + PFAmount + "','0') ");
            }
            catch { }
        }
        private void updateTaxRecord(string EmpId, string YearMonth)
        {
            try
            {
                
                CRUD.Execute("update VatTax_IncomeTaxDetailsLog set isPaid=1 where EmpId='" + EmpId + "' and Month='" + YearMonth + "'");
            }
            catch { }
        }
        private void updateLoanStatus(string EmpId,string Month )
        {
            try
            {               
                    CRUD.Execute("Update Payroll_LoanMonthlySetup set IsPaid=1 Where EmpID='"+ EmpId + "' and Month='"+ Month + "'" );          
            }
            catch { }

        }
        private void salarySheetClear(DateTime ToDate, string CompanyId,string EmpId)
        {
            try
            {
                EmpId = (EmpId == "0") ? "" : " and EmpId ='" + EmpId + "'";
                CRUD.Execute("delete from Payroll_MonthlySalarySheet where CompanyId='" + CompanyId + "'  AND YearMonth='" + ToDate.ToString("yyyy-MM") + "-01' And ToDate='" + ToDate.ToString("yyyy-MM-dd") + "' AND EmpStatus in ('1','8') AND IsSeperationGeneration='0' "+ EmpId);
            }
            catch { }
        }
        private void salarySheetClearForSeparation(DateTime ToDate, string CompanyId, string EmpId)
        {
            try
            {
                EmpId = (EmpId == "0") ? "" : " and EmpId ='" + EmpId + "'";
                CRUD.Execute("delete from Payroll_MonthlySalarySheet where CompanyId='" + CompanyId + "'  AND YearMonth='" + ToDate.ToString("yyyy-MM") + "-01'  AND IsSeperationGeneration='1' " + EmpId);
            }
            catch { }
        }
    }
}