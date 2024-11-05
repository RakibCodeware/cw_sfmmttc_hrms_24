using adviitRuntimeScripting;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class AutoIncrementPanel : System.Web.UI.Page
    {
        string qurey = "";
        DataTable dt; 
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                setPrivilege();
            }
        }
        private void setPrivilege()
        {
            try
            {
                ViewState["__ReadAction__"] = "0";
                ViewState["__WriteAction__"] = "0";
                ViewState["__UpdateAction__"] = "0";
                ViewState["__DeletAction__"] = "0";
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                if (getCookies["__IsCompliance__"].ToString().Equals("True"))
                {
                    ViewState["__ReadAction__"] = "1";
                    ViewState["__WriteAction__"] = "1";
                    ViewState["__UpdateAction__"] = "1";
                    ViewState["__DeletAction__"] = "1";
                   
                }
                if (ViewState["__ReadAction__"].ToString().Equals("0"))
                {
                  btnSearch.Enabled = false;
                  btnSearch.CssClass = "";
                }                
                LoadAllownceSetting();
                dt = new DataTable();
                dt = commonTask.getAttendanceBonus(ViewState["__CompanyId__"].ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewState["__AttBonusWorker__"] = dt.Rows[0]["AttBonus"].ToString();
                    ViewState["__AttBonusStaff__"] = dt.Rows[1]["AttBonus"].ToString();
                }

            }
            catch { }

        }
        static DataTable dt_AllowanceSettings = new DataTable();
        private void LoadAllownceSetting()
        {
            try
            {
                dt_AllowanceSettings = new DataTable();
                sqlDB.fillDataTable("Select acs.SalaryType ,has.BasicAllowance,has.MedicalAllownce,has.FoodAllownce,has.ConvenceAllownce,has.TechnicalAllowance, " +
                    " has.HouseRent,has.OthersAllowance,has.PFAllowance,has.AlCalId,acs.EmpTypeId," +
                    "acs.BasicAllowance as BasicStatus,acs.MedicalAllownce as MedicalStatus,acs.FoodAllownce as FoodStatus,acs.ConvenceAllownce as ConStatus," +
                    " acs.TechnicalAllowance as TecStatus,acs.HouseRent as HouseStatus,acs.OthersAllowance as OthStatus, acs.ProvidentFund as PFStatus " +
                    " from HRD_AllownceSetting as has inner join Payroll_AllowanceCalculationSetting acs on has.AlCalId=acs.AlCalId where CalculationType='salary' ", dt_AllowanceSettings);

            }
            catch { }
        }
        private void loadIncrementableData()
        {
            try
            {
                if (txtEffectiveMonth.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning-> Please, Select The Month!";
                    return;
                }
                Payroll.checkForActiveCommonIncrementCompliance(ViewState["__CompanyId__"].ToString());
                string[] month = txtEffectiveMonth.Text.Trim().Split('-');
                qurey = "select EmpId,SUBSTRING(EmpCardNo,8,6) as EmpCardNo,EmpName,DsgName,DptName,convert(varchar(10),EmpJoiningDate,105) as EmpJoiningDate,EmpPresentSalary,BasicSalary,MedicalAllownce,ConvenceAllownce,HouseRent,OthersAllownce,'" + txtEffectiveMonth.Text.Trim() + "' as IncrementMonth, convert(varchar(10),AutoIncrementMonth,105) as  AutoIncrementMonth,round((EmpPresentSalary*.05),0) as IncrementAmount ,round( EmpPresentSalary+(EmpPresentSalary*.05),0) as newGross,round((round(EmpPresentSalary+(EmpPresentSalary*.05),0)-1850)/1.5,0) as newBasic,round(EmpPresentSalary+(EmpPresentSalary*.05),0)-round((round(EmpPresentSalary+(EmpPresentSalary*.05),0)-1850)/1.5,0)-1850 as newHouseRent,600 as newMedical,900 as newFood,350 as newConveyance from v_Personnel_EmpCurrentStatus1 where IsActive=1 and EmpStatus in(1,8) and month(EmpJoiningDate)='" + month[0]+ "' and year(ISNULL(AutoIncrementMonth,EmpJoiningDate))<" + month[1] + " and EmpTypeId=1";
             
                sqlDB.fillDataTable(qurey, dt = new DataTable());
                gvList.DataSource = dt;
                gvList.DataBind();
            }
            catch (Exception ex)
            {
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {            
            loadIncrementableData();
        }
        private bool saveSalaryIncrement(string EmpID,TextBox txtNewGross,TextBox txtIncrementAmount,TextBox txtNewBasic,TextBox txtNewMedical,TextBox txtNewFoodAllowance,TextBox txtNewConveyance,TextBox txtNewHouseRent,TextBox txtEffectiveFrom)

        {
            try
            {
                DataTable dtPreStatus = new DataTable();
                sqlDB.fillDataTable("Select SN, TiffinAllownce,NightAllownce,AttendanceBonus,  convert(varchar(10),EarnLeaveDate,120) as EarnLeaveDate, LunchAllownce, LunchCount, PreDptId, PreDsgId, PreGrdName, CompanyId, EmpId, EmpCardNo, PreEmpTypeId, EmpTypeId, PreSalaryType, SalaryType, EmpPresentSalary, IncrementAmount, BasicSalary, MedicalAllownce, FoodAllownce, ConvenceAllownce, HouseRent, PreTechnicalAllownce, TechnicalAllownce, DptId, DsgId, EmpStatus, GrdName, OthersAllownce, convert(varchar(10), EarnLeaveDate, 120) as EarnLeaveDate, PreShiftTransferDate, convert(varchar(10), ShiftTransferDate, 120) as ShiftTransferDate, ShiftTransferToDate, SftId, GId, PreGId, SalaryCount, BankId, EmpAccountNo, PfMember, convert(varchar(10), PfDate, 120) as PfDate, PFAmount, CustomOrdering, OverTime, PreEmpDutyType, EmpDutyType, EmpJoinigSalary, DormitoryRent, PreIncomeTax, IncomeTax From Personnel_EmpCurrentStatus1 where IsActive=1 and EmpID='" + EmpID + "' ", dtPreStatus);

                SqlCommand cmd = new SqlCommand("Insert into  Personnel_EmpCurrentStatus1 (EmpId, PreCompanyId, CompanyId, EmpCardNo, PreEmpTypeId, EmpTypeId, PreSalaryType, SalaryType, PreEmpSalary, EmpPresentSalary, PreIncrementAmount, IncrementAmount, PreBasicSalary, BasicSalary,PreMedicalAllownce, MedicalAllownce,PreFoodAllownce, FoodAllownce,PreConvenceAllownce, ConvenceAllownce, PreHouseRent, HouseRent,PreTechnicalAllownce,TechnicalAllownce, PreDptId, DptId, PreDsgId, DsgId, PreEmpStatus, EmpStatus, PreGrdName, GrdName, PreOthersAllownce, OthersAllownce, HolidayAllownce, TiffinAllownce, NightAllownce, AttendanceBonus, LunchAllownce, LunchCount, DateofUpdate, TypeOfChange, EffectiveMonth, OrderRefNo, OrderRefDate, Remarks, ActiveSalary, EarnLeaveDate, IsActive,PreShiftTransferDate,ShiftTransferDate,ShiftTransferToDate,SftId,GId,PreGId,SalaryCount,BankId,EmpAccountNo,PfMember,PfDate,PFAmount,CustomOrdering,OverTime,PreEmpDutyType,EmpDutyType,EmpJoinigSalary,DormitoryRent,PreIncomeTax,IncomeTax,AutoIncrementMonth)  values (@EmpId, @PreCompanyId, @CompanyId, @EmpCardNo, @PreEmpTypeId, @EmpTypeId, @PreSalaryType, @SalaryType, @PreEmpSalary, @EmpPresentSalary, @PreIncrementAmount, @IncrementAmount, @PreBasicSalary, @BasicSalary,@PreMedicalAllownce, @MedicalAllownce,@PreFoodAllownce,@FoodAllownce,@PreConvenceAllownce, @ConvenceAllownce, @PreHouseRent, @HouseRent,@PreTechnicalAllownce,@TechnicalAllownce, @PreDptId, @DptId, @PreDsgId, @DsgId, @PreEmpStatus, @EmpStatus, @PreGrdName, @GrdName, @PreOthersAllownce, @OthersAllownce, @HolidayAllownce, @TiffinAllownce, @NightAllownce, @AttendanceBonus, @LunchAllownce, @LunchCount, @DateofUpdate, @TypeOfChange, @EffectiveMonth, @OrderRefNo, @OrderRefDate, @Remarks, @ActiveSalary, @EarnLeaveDate, @IsActive,@PreShiftTransferDate,@ShiftTransferDate,@ShiftTransferToDate,@SftId,@GId,@PreGId,@SalaryCount,@BankId,@EmpAccountNo,@PfMember,@PfDate,@PFAmount,@CustomOrdering,@OverTime,@PreEmpDutyType,@EmpDutyType,@EmpJoinigSalary,@DormitoryRent,@PreIncomeTax,@IncomeTax,@AutoIncrementMonth) ", sqlDB.connection);
                cmd.Parameters.AddWithValue("@EmpId", dtPreStatus.Rows[0]["EmpId"].ToString());
                cmd.Parameters.AddWithValue("@PreCompanyId", dtPreStatus.Rows[0]["CompanyId"].ToString());
                cmd.Parameters.AddWithValue("@CompanyId", ViewState["__CompanyId__"].ToString());
                cmd.Parameters.AddWithValue("@EmpCardNo", dtPreStatus.Rows[0]["EmpCardNo"].ToString());
                cmd.Parameters.AddWithValue("@PreEmpTypeId", dtPreStatus.Rows[0]["PreEmpTypeId"].ToString());
                cmd.Parameters.AddWithValue("@EmpTypeId", dtPreStatus.Rows[0]["EmpTypeId"].ToString());
                cmd.Parameters.AddWithValue("@PreSalaryType", dtPreStatus.Rows[0]["PreSalaryType"].ToString());
                cmd.Parameters.AddWithValue("@SalaryType", dtPreStatus.Rows[0]["SalaryType"].ToString());
                cmd.Parameters.AddWithValue("@PreEmpSalary", dtPreStatus.Rows[0]["EmpPresentSalary"].ToString());
                cmd.Parameters.AddWithValue("@EmpPresentSalary", txtNewGross.Text.Trim());
                cmd.Parameters.AddWithValue("@PreIncrementAmount", dtPreStatus.Rows[0]["IncrementAmount"].ToString());
                cmd.Parameters.AddWithValue("@IncrementAmount", txtIncrementAmount.Text.Trim());
                cmd.Parameters.AddWithValue("@PreBasicSalary", dtPreStatus.Rows[0]["BasicSalary"].ToString());
                cmd.Parameters.AddWithValue("@BasicSalary", txtNewBasic.Text.Trim());
                cmd.Parameters.AddWithValue("@PreMedicalAllownce", dtPreStatus.Rows[0]["MedicalAllownce"].ToString());
                cmd.Parameters.AddWithValue("@MedicalAllownce", txtNewMedical.Text.Trim());
                cmd.Parameters.AddWithValue("@PreFoodAllownce", dtPreStatus.Rows[0]["FoodAllownce"].ToString());
                cmd.Parameters.AddWithValue("@FoodAllownce", txtNewFoodAllowance.Text.Trim());
                cmd.Parameters.AddWithValue("@PreConvenceAllownce", dtPreStatus.Rows[0]["ConvenceAllownce"].ToString());
                cmd.Parameters.AddWithValue("@ConvenceAllownce", txtNewConveyance.Text.Trim());
                cmd.Parameters.AddWithValue("@PreHouseRent", dtPreStatus.Rows[0]["HouseRent"].ToString());
                cmd.Parameters.AddWithValue("@HouseRent", txtNewHouseRent.Text.Trim());
                cmd.Parameters.AddWithValue("@PreTechnicalAllownce", dtPreStatus.Rows[0]["PreTechnicalAllownce"].ToString());
                cmd.Parameters.AddWithValue("@TechnicalAllownce", dtPreStatus.Rows[0]["TechnicalAllownce"].ToString());
                cmd.Parameters.AddWithValue("@PreDptId", dtPreStatus.Rows[0]["PreDptId"].ToString());
                cmd.Parameters.AddWithValue("@DptId", dtPreStatus.Rows[0]["DptId"].ToString());
                cmd.Parameters.AddWithValue("@PreDsgId", dtPreStatus.Rows[0]["PreDsgId"].ToString());
                cmd.Parameters.AddWithValue("@DsgId", dtPreStatus.Rows[0]["DsgId"].ToString());
                cmd.Parameters.AddWithValue("@PreEmpStatus", dtPreStatus.Rows[0]["EmpStatus"].ToString());
                cmd.Parameters.AddWithValue("@EmpStatus", "1");
                cmd.Parameters.AddWithValue("@PreGrdName", dtPreStatus.Rows[0]["PreGrdName"].ToString());
                cmd.Parameters.AddWithValue("@GrdName", dtPreStatus.Rows[0]["GrdName"].ToString());
                cmd.Parameters.AddWithValue("@PreOthersAllownce", dtPreStatus.Rows[0]["OthersAllownce"].ToString());
                cmd.Parameters.AddWithValue("@OthersAllownce", dtPreStatus.Rows[0]["OthersAllownce"].ToString());
                cmd.Parameters.AddWithValue("@HolidayAllownce", "0");
                cmd.Parameters.AddWithValue("@TiffinAllownce", dtPreStatus.Rows[0]["TiffinAllownce"].ToString());
                cmd.Parameters.AddWithValue("@NightAllownce", dtPreStatus.Rows[0]["NightAllownce"].ToString());

                if (dtPreStatus.Rows[0]["EmpTypeId"].ToString() == "1")
                    cmd.Parameters.AddWithValue("@AttendanceBonus", ViewState["__AttBonusWorker__"].ToString());
                else
                    cmd.Parameters.AddWithValue("@AttendanceBonus", ViewState["__AttBonusStaff__"].ToString());
                cmd.Parameters.AddWithValue("@LunchAllownce", dtPreStatus.Rows[0]["LunchAllownce"].ToString());
                byte getLounchCount = (bool.Parse(dtPreStatus.Rows[0]["LunchCount"].ToString()) == true) ? (byte)1 : (byte)0;
                cmd.Parameters.AddWithValue("@LunchCount", getLounchCount.ToString());

                cmd.Parameters.AddWithValue("@DateofUpdate", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@TypeOfChange", "i");
                cmd.Parameters.AddWithValue("@EffectiveMonth", txtEffectiveFrom.Text);
                cmd.Parameters.AddWithValue("@OrderRefNo","");
                cmd.Parameters.AddWithValue("@OrderRefDate", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@Remarks", "Auto Incrememt");
                cmd.Parameters.AddWithValue("@ActiveSalary", 0);
                if (dtPreStatus.Rows[0]["EarnLeaveDate"].ToString() == "" || dtPreStatus.Rows[0]["EarnLeaveDate"].ToString() == " ") cmd.Parameters.AddWithValue("@EarnLeaveDate", commonTask.ddMMyyyyTo_yyyyMMdd("01-01-2050"));
                else cmd.Parameters.AddWithValue("@EarnLeaveDate", dtPreStatus.Rows[0]["EarnLeaveDate"].ToString());
                cmd.Parameters.AddWithValue("@IsActive", 0);
                cmd.Parameters.AddWithValue("@PreShiftTransferDate", dtPreStatus.Rows[0]["PreShiftTransferDate"].ToString());
                cmd.Parameters.AddWithValue("@ShiftTransferDate", dtPreStatus.Rows[0]["ShiftTransferDate"].ToString());
                cmd.Parameters.AddWithValue("@ShiftTransferToDate", dtPreStatus.Rows[0]["ShiftTransferToDate"].ToString());
                cmd.Parameters.AddWithValue("@SftId", dtPreStatus.Rows[0]["SftId"].ToString());
                cmd.Parameters.AddWithValue("@GId", dtPreStatus.Rows[0]["GId"].ToString());
                cmd.Parameters.AddWithValue("@PreGId", dtPreStatus.Rows[0]["PreGId"].ToString());
                cmd.Parameters.AddWithValue("@SalaryCount", dtPreStatus.Rows[0]["SalaryCount"].ToString());
                cmd.Parameters.AddWithValue("@BankId", dtPreStatus.Rows[0]["BankId"].ToString());
                cmd.Parameters.AddWithValue("@EmpAccountNo", dtPreStatus.Rows[0]["EmpAccountNo"].ToString());
                cmd.Parameters.AddWithValue("@PfMember", dtPreStatus.Rows[0]["PfMember"].ToString());
                cmd.Parameters.AddWithValue("@PfDate", dtPreStatus.Rows[0]["PfDate"].ToString());
                
                cmd.Parameters.AddWithValue("@PFAmount", dtPreStatus.Rows[0]["PFAmount"].ToString());
                cmd.Parameters.AddWithValue("@CustomOrdering", dtPreStatus.Rows[0]["CustomOrdering"].ToString());
                cmd.Parameters.AddWithValue("@OverTime", dtPreStatus.Rows[0]["OverTime"].ToString());
                cmd.Parameters.AddWithValue("@PreEmpDutyType", dtPreStatus.Rows[0]["PreEmpDutyType"].ToString());
                cmd.Parameters.AddWithValue("@EmpDutyType", dtPreStatus.Rows[0]["EmpDutyType"].ToString());
                cmd.Parameters.AddWithValue("@EmpJoinigSalary", dtPreStatus.Rows[0]["EmpJoinigSalary"].ToString());
                cmd.Parameters.AddWithValue("@DormitoryRent", dtPreStatus.Rows[0]["DormitoryRent"].ToString());
                cmd.Parameters.AddWithValue("@PreIncomeTax", dtPreStatus.Rows[0]["PreIncomeTax"].ToString());
                cmd.Parameters.AddWithValue("@IncomeTax", dtPreStatus.Rows[0]["IncomeTax"].ToString());
                string[] month = txtEffectiveFrom.Text.Trim().Split('-');
                cmd.Parameters.AddWithValue("@AutoIncrementMonth", month[1]+"-"+ month[0]+"-01");
                int i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    lblMessage.InnerText = "success->Successfully Salary Incremented";
                    return true;
                }
                else
                {
                    lblMessage.InnerText = "error->Unable to Submit!";
                    return false;
                }

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
                return false;
            }
        }

        protected void gvList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Submit")
            {
                int rIndex = int.Parse(e.CommandArgument.ToString());
                string EmpID = gvList.DataKeys[rIndex].Values[0].ToString();
                TextBox txtNewGross =(TextBox)gvList.Rows[rIndex].FindControl("txtNewGross");
                TextBox txtIncrementAmount = (TextBox)gvList.Rows[rIndex].FindControl("txtIncrementAmount");
                TextBox txtNewBasic = (TextBox)gvList.Rows[rIndex].FindControl("txtNewBasic");
                TextBox txtNewMedical = (TextBox)gvList.Rows[rIndex].FindControl("txtNewMedical");
                TextBox txtNewFoodAllowance = (TextBox)gvList.Rows[rIndex].FindControl("txtNewFoodAllowance");
                TextBox txtNewConveyance = (TextBox)gvList.Rows[rIndex].FindControl("txtNewConveyance");
                TextBox txtNewHouseRent = (TextBox)gvList.Rows[rIndex].FindControl("txtNewHouseRent");
                TextBox txtEffectiveFrom = (TextBox)gvList.Rows[rIndex].FindControl("txtEffectiveFrom");
                if (saveSalaryIncrement(EmpID, txtNewGross, txtIncrementAmount, txtNewBasic, txtNewMedical, txtNewFoodAllowance, txtNewConveyance, txtNewHouseRent, txtEffectiveFrom))
                {
                    gvList.Rows[rIndex].Visible = false;
                }
            }
        }
    }
}