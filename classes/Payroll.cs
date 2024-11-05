using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data;
using ComplexScriptingSystem;
using adviitRuntimeScripting;
using System.IO;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace SigmaERP.classes
{
     
    public class Payroll
    {
        static DataTable dt;
        public static double getHourlyAmount(int DaysInMonth, double Salary, string isHalfOT, string IsSingleRateOT)
        {
            try
            {
                // getOndaySalary = Math.Round(GrossSalary / DaysInMonth, 0);
                // NormalHourlySalary = Math.Round(getOndaySalary/8);
                if (isHalfOT == "True" || IsSingleRateOT== "True")
                    return Math.Round(Salary / 208, 2); // here 208 is static.
                else
                    return Math.Round(Salary / 208 * 2, 2); // here 208 is static.
            }
            catch { return 0; }
        }
        public static double getHourlyAmount(int DaysInMonth, double Salary, string IsSingleRateOT)
        {
            try
            {                
                if(IsSingleRateOT=="True")
                   return Math.Round(Salary / 208 , 2); // here 208 is static.
                else
                    return Math.Round(Salary / 208 * 2, 2); // here 208 is static.
                
            }
            catch { return 0; }
        }
        public static void loadMonthIdByCompany(DropDownList ddlMonthList, string CompanyId)
        {
            try
            {
                DataTable dt=new DataTable ();
                sqlDB.fillDataTable("select Distinct case when FromDate is null then  Format(YearMonth,'MMM-yyyy') else Format(YearMonth,'MMM-yyyy')+' [ '+ convert(varchar(10),FromDate,120)+' to '+ convert(varchar(10),ToDate,120)+' ]'  end  as YearMonth,case when FromDate is null then convert(varchar(10), YearMonth, 120) else   convert(varchar(10), YearMonth, 120) + '/' + convert(varchar(10), FromDate, 120) + '/' + convert(varchar(10), ToDate, 120) end as MonthYear, year(YearMonth), month(YearMonth) from Payroll_MonthlySalarySheet where CompanyId = '"+CompanyId+"' order by year(YearMonth) desc, month(YearMonth) desc", dt);
                ddlMonthList.DataSource = dt;
                ddlMonthList.DataValueField = "MonthYear";
                ddlMonthList.DataTextField = "YearMonth";
                ddlMonthList.DataBind();
                ddlMonthList.Items.Insert(0,new ListItem (" ","0"));
            }
            catch { }
        }
        public static void loadMonthIdByCompany1(DropDownList ddlMonthList, string CompanyId)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select distinct convert(varchar(10), YearMonth,120) as MonthYear,Format(YearMonth,'MMM-yyyy') as YearMonth, year(YearMonth), month(YearMonth) from Payroll_MonthlySalarySheet1 where CompanyId = '" + CompanyId + "' order by year(YearMonth) desc, month(YearMonth) desc", dt);
                ddlMonthList.DataSource = dt;
                ddlMonthList.DataValueField = "MonthYear";
                ddlMonthList.DataTextField = "YearMonth";
                ddlMonthList.DataBind();
                ddlMonthList.Items.Insert(0, new ListItem(" ", "0"));
            }
            catch { }
        }
        public static void loadEarnleaveMonthIdByCompany(DropDownList ddlMonthList, string CompanyId)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select distinct CompanyId,EndDate, format(EndDate,'MMM-yyyy') as  Month,convert(varchar, IsSeparated)+'/'+ convert(varchar(10),StartDate,120)+'/'+ convert(varchar(10),EndDate,120) as Value,format(EndDate,'MMM-yyyy')+' ['+ convert(varchar(10),StartDate,105)+' to '+  convert(varchar(10),EndDate,105)+']'+ case when IsSeparated=1 then ' [Seperated]' else '' end  as Text ,year(EndDate) ,month(EndDate) " +
                    " from Payroll_EarnLeavePaymentSheet where CompanyId='"+CompanyId+"' order by year(EndDate) desc, month(EndDate) desc, EndDate desc", dt);
                ddlMonthList.DataSource = dt;
                ddlMonthList.DataValueField = "Value";
                ddlMonthList.DataTextField = "Text";
                ddlMonthList.DataBind();
                ddlMonthList.Items.Insert(0, new ListItem(" ", "0"));
            }
            catch { }
        }
        public static void loadEarnleaveMonthIdByCompanyc(DropDownList ddlMonthList, string CompanyId)// for compliance
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select distinct CompanyId,EndDate, format(EndDate,'MMM-yyyy') as  Month,convert(varchar, IsSeparated)+'/'+ convert(varchar(10),StartDate,120)+'/'+ convert(varchar(10),EndDate,120) as Value,format(EndDate,'MMM-yyyy')+' ['+ convert(varchar(10),StartDate,105)+' to '+  convert(varchar(10),EndDate,105)+']'+ case when IsSeparated=1 then ' [Seperated]' else '' end  as Text ,year(EndDate) ,month(EndDate) " +
                    " from Payroll_EarnLeavePaymentSheet1 where CompanyId='" + CompanyId + "' order by year(EndDate) desc, month(EndDate) desc, EndDate desc", dt);
                ddlMonthList.DataSource = dt;
                ddlMonthList.DataValueField = "Value";
                ddlMonthList.DataTextField = "Text";
                ddlMonthList.DataBind();
                ddlMonthList.Items.Insert(0, new ListItem(" ", "0"));
            }
            catch { }
        }
        public static void loadBonusType(DropDownList ddlBonusType, string CompanyId)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select Distinct BonusType,GenerateDate  from v_Payroll_YearlyBonusSheet where CompanyId='" + CompanyId + "' order by GenerateDate desc", dt);
                ddlBonusType.DataSource = dt;
                ddlBonusType.DataTextField = "BonusType";
                ddlBonusType.DataValueField = "BonusType";
                ddlBonusType.DataBind();
                ddlBonusType.Items.Insert(0, new ListItem("", "0"));
            }
            catch{ }
        }
        public static void loadBonusList(DropDownList ddlBonus, string CompanyId)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select BId,BonusName from Payroll_BonusSetup where Status=1 and CompanyId='"+ CompanyId + "' order by CalculationDate desc", dt);
                ddlBonus.DataSource = dt;
                ddlBonus.DataTextField = "BonusName";
                ddlBonus.DataValueField = "BId";
                ddlBonus.DataBind();
                ddlBonus.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void loadBonusList_Compliance(DropDownList ddlBonus, string CompanyId)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select BId,BonusName from Payroll_BonusSetup1 where Status=1 and CompanyId='" + CompanyId + "' order by CalculationDate desc", dt);
                ddlBonus.DataSource = dt;
                ddlBonus.DataTextField = "BonusName";
                ddlBonus.DataValueField = "BId";
                ddlBonus.DataBind();
                ddlBonus.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadBonusTypeByCompany(DropDownList ddlBonusType, string CompanyId)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select Distinct BonusType,BId,GenerateDate  from v_Payroll_YearlyBonusSheet where CompanyId='" + CompanyId + "' order by GenerateDate", dt);
                ddlBonusType.DataSource = dt;
                ddlBonusType.DataTextField = "BonusType";
                ddlBonusType.DataValueField = "BId";
                ddlBonusType.DataBind();
                ddlBonusType.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static string getSftIdList(DropDownList ddlSftList)
        {
            try
            {
                string setPredicate = "";
                for (byte b = 0; b < ddlSftList.Items.Count; b++)
                {
                    setPredicate +=ddlSftList.Items[b].Value.ToString() +",";
                }

                setPredicate = setPredicate.Remove(setPredicate.LastIndexOf(','));
                return setPredicate;
            }
            catch { return " "; }

        }

        public static string getCompanyList(DropDownList ddlCompanyList)
        {
            try
            {
                string setPredicate = "";
                for (byte b = 0; b < ddlCompanyList.Items.Count; b++)
                {
                    setPredicate += ddlCompanyList.Items[b].Value.ToString() + ",";
                }

                setPredicate = setPredicate.Remove(setPredicate.LastIndexOf(','));
                return setPredicate;
            }
            catch { return " "; }

        }

        public static DataTable Load_Payroll_AllowanceCalculationSetting()
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select * from Payroll_AllowanceCalculationSetting",dt);
                return dt;
            }
            catch { return null; }
        }

        public static DataTable Load_HRD_AllownceSetting()
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select * from HRD_AllownceSetting", dt);
                return dt;
            }
            catch { return null; }
        }

        public static bool Office_IsGarments()
        {
            try
            {
                DataTable dt = new DataTable();
                SQLOperation.selectBySetCommandInDatatable("select * from Payroll_Office_IsGarments", dt, sqlDB.connection);
                
                if (bool.Parse(dt.Rows[0]["IsGarments"].ToString().Trim()) == true)return true;
                
                else return false;
             
            }
            catch { return false; }
        }

        public static bool checkForActiveCommonIncrementCompliance(string CompanyId)
        {
            try
            {
                SQLOperation.selectBySetCommandInDatatable("select SN,EmpId from Personnel_EmpCurrentStatus1  where ActiveSalary='false' AND CompanyId='" + CompanyId + "' AND  TypeOfChange='i' and convert(Date, SUBSTRING(EffectiveMonth,4,4)+'-'+ SUBSTRING(EffectiveMonth,0,3)+'-01' )<='" + DateTime.Now.ToString("yyyy-MM-dd") + "'", dt = new DataTable(), sqlDB.connection);
                if (dt.Rows.Count > 0)
                {

                    for (int r = 0; r < dt.Rows.Count; r++)
                    {
                        SqlCommand upIsActive = new SqlCommand("Update Personnel_EmpCurrentStatus1 set IsActive=0 where EmpId='" + dt.Rows[r]["EmpId"].ToString() + "'", sqlDB.connection);
                        upIsActive.ExecuteNonQuery();
                        string[] getColumns2 = { "ActiveSalary", "IsActive" };
                        string[] getValues2 = { "1", "1" };
                        SQLOperation.forUpdateValue("Personnel_EmpCurrentStatus1", getColumns2, getValues2, "SN", dt.Rows[r]["SN"].ToString(), sqlDB.connection);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}