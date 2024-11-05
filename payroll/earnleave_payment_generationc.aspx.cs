using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class earnleave_payment_generationc : System.Web.UI.Page
    {
        DataTable dt;
        string sqlCmd = "";
        string CompanyId = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                setPrivilege();
                loadEarnLeaveGeneration();
            }
        }
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                Session["__getUserId__"] = getUserId;
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = CompanyId = getCookies["__CompanyId__"].ToString();
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForOnlyWriteAction(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "payroll_generation.aspx", ddlCompanyList, btnGenerate, btnGenerate, gvEarnLeaveGenerationList);
            }
            catch { }
        }
        private void loadEarnLeaveGeneration()
        {
            try
            {
                sqlCmd = "select distinct CompanyId,IsSeparated,case when IsSeparated=1 then 'Yes' else 'No' end as IsSeparatedText , convert(varchar(10),StartDate,105) as StartDate, convert(varchar(10),EndDate,105) as EndDate,convert(varchar(10),StartDate,120) as StartDate1, convert(varchar(10),EndDate,120) as EndDate1,format(EndDate,'MMM-yyyy') as  Month ,year(EndDate) ,month(EndDate) from Payroll_EarnLeavePaymentSheet1 where CompanyId='" + CompanyId
                + "' order by year(EndDate) desc,month(EndDate) desc,EndDate desc";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                gvEarnLeaveGenerationList.DataSource = dt;
                gvEarnLeaveGenerationList.DataBind();
            }
            catch (Exception ex)
            { }

        }
        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompanyId = (ddlCompanyList.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue;
        }
        protected void btnGenerate_Click(object sender, EventArgs e)
        {

            earnLeavePaymentGeneration();
        }
        private void earnLeavePaymentGeneration()
        {
            try
            {
                CompanyId = (ddlCompanyList.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue;

                string StartDate = commonTask.ddMMyyyyTo_yyyyMMdd(txtStartDate.Text.Trim());
                DateTime EndDate = DateTime.Parse(commonTask.ddMMyyyyTo_yyyyMMdd(txtEndDate.Text.Trim()));

                string YearMonth = EndDate.ToString("yyyy-MM") + "-01";
                string IsSeparated = "0";
                string EmpIDforIndividual = "";
                string EmpIDforIndividualCondition = "";
                if (txtEmpCardNo.Text.Trim() != "")
                {
                    sqlCmd = "select EmpId from Personnel_EmployeeInfo where EmpCardNo like '%" + txtEmpCardNo.Text.Trim() + "'";
                    sqlDB.fillDataTable(sqlCmd, dt = new DataTable());// check valid employee 
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        EmpIDforIndividual = dt.Rows[0]["EmpId"].ToString();
                    }
                    else
                    {
                        lblMessage.InnerText = "warning-> Invalid card no!";
                        imgLoading.Visible = false;
                        return;
                    }
                }
                string ExceptedEmpCardNo = "";
                if (txtExceptedEmpCardNo.Text.Trim() != "")
                {
                    ExceptedEmpCardNo = " and el.EmpID not in(select EmpId from Personnel_EmployeeInfo where SUBSTRING(EmpCardNo,8,6) in(" + txtExceptedEmpCardNo.Text.Trim() + ") and CompanyId='" + CompanyId + "')";
                }

                if (rbGenaratingType.SelectedValue == "1")
                {
                    IsSeparated = "1";
                    EmpIDforIndividualCondition = (EmpIDforIndividual == "") ? "" : " and EmpId ='" + dt.Rows[0]["EmpId"].ToString() + "'";
                    sqlCmd = "select convert(varchar(10), pei.EmpJoiningDate,120) EmpJoiningDate,el.EmpID,pe.EmpStatus,pe.EmpTypeId,sum(EarnLeaveDays) as EarnLeaveDays,pe.CompanyId,pe.DptId,pe.GId,pe.DsgId,pe.GrdName,pe.EmpPresentSalary,pe.BasicSalary from Earnleave_BalanceDetailsLog1 as el inner join Personnel_EmployeeInfo pei on el.EmpID=pei.EmpId inner join Personnel_EmpCurrentStatus1 as pe on el.EmpID=pe.EmpId and pe.IsActive=1 " +
                    "  where pe.CompanyId='" + CompanyId + "' and  EarnLeaveLastDate>='" + StartDate + "' and EarnLeaveLastDate<='" + EndDate + "' and el.EmpId in(select EmpId from Personnel_EmpSeparation  where format(EffectiveDate, 'yyyy-MM') = '" + EndDate.ToString("yyyy-MM") + "' " + EmpIDforIndividualCondition + ") " + ExceptedEmpCardNo + " and CompanyId='" + CompanyId + "')" +
                    "  group by pei.EmpJoiningDate,el.EmpID,pe.EmpStatus,pe.EmpTypeId,pe.CompanyId,pe.DptId,pe.GId,pe.DsgId,pe.GrdName,pe.EmpPresentSalary,pe.BasicSalary";
                }
                else
                {
                    IsSeparated = "0";
                    EmpIDforIndividualCondition = (EmpIDforIndividual == "") ? "" : " and el.EmpId ='" + dt.Rows[0]["EmpId"].ToString() + "'";
                    sqlCmd = "select convert(varchar(10), pei.EmpJoiningDate,120) EmpJoiningDate,el.EmpID,pe.EmpStatus,pe.EmpTypeId,sum(EarnLeaveDays) as EarnLeaveDays,pe.CompanyId,pe.DptId,pe.GId,pe.DsgId,pe.GrdName,pe.EmpPresentSalary,pe.BasicSalary from Earnleave_BalanceDetailsLog1 as el inner join Personnel_EmpCurrentStatus1 as pe on el.EmpID=pe.EmpId and pe.IsActive=1  and pe.EmpStatus in(1,8)  inner join Personnel_EmployeeInfo pei on el.EmpID=pei.EmpId " +
                    "  where pe.CompanyId='" + CompanyId + "' and EarnLeaveLastDate>='" + StartDate + "' and EarnLeaveLastDate<='" + EndDate.ToString("yyyy-MM-dd") + "' " + EmpIDforIndividualCondition + ExceptedEmpCardNo + "  group by  pei.EmpJoiningDate,el.EmpID,pe.EmpStatus,pe.EmpTypeId,pe.CompanyId,pe.DptId,pe.GId,pe.DsgId,pe.GrdName,pe.EmpPresentSalary,pe.BasicSalary";
                }


                DataTable dtEarnLeaveEmployee;
                sqlDB.fillDataTable(sqlCmd, dtEarnLeaveEmployee = new DataTable());// get emplyee's earnleave info.               
                deleteExEarnLeave(CompanyId, StartDate, EndDate.ToString("yyyy-MM-dd"), IsSeparated, EmpIDforIndividual);// delete existing record
                if (dtEarnLeaveEmployee != null && dtEarnLeaveEmployee.Rows.Count > 0)
                {
                    dt = new DataTable();
                    dt = getEarnLeaveSettings();
                    string PaymentOn = dt.Rows[0]["PaymentOn"].ToString();
                    double WithdrawableEarnLeavePer = double.Parse(dt.Rows[0]["WithdrawablePer"].ToString());
                    int MonthDays = 30;
                    double StampDeduction = 0;
                    // get stamp card price
                    DataTable dtStampDeduct;
                    sqlDB.fillDataTable("select StampDeduct from HRD_AllownceSetting where AllownceId =(select max(AllownceId) from HRD_AllownceSetting)", dtStampDeduct = new DataTable());
                    for (int i = 0; i < dtEarnLeaveEmployee.Rows.Count; i++)
                    {
                        StampDeduction = double.Parse(dtStampDeduct.Rows[0]["StampDeduct"].ToString());
                        DateTime EmpJoiningDate = DateTime.Parse(dtEarnLeaveEmployee.Rows[i]["EmpJoiningDate"].ToString());
                        double EmpPresentSalary = double.Parse(dtEarnLeaveEmployee.Rows[i]["EmpPresentSalary"].ToString());
                        double BasicSalary = double.Parse(dtEarnLeaveEmployee.Rows[i]["BasicSalary"].ToString());
                        double OneDaySalary = 0;
                        if (PaymentOn == "Basic")
                            OneDaySalary = BasicSalary / MonthDays;
                        else
                            OneDaySalary = EmpPresentSalary / MonthDays;

                        string EmpId = dtEarnLeaveEmployee.Rows[i]["EmpID"].ToString();

                        int PreviousYearEarnLeaveDays = getReserveEeanLeaveDays(EmpId, StartDate);
                        int CurrentYearEarnLeaveDays = int.Parse(dtEarnLeaveEmployee.Rows[i]["EarnLeaveDays"].ToString());
                        int TotalEarnLeaveDays = PreviousYearEarnLeaveDays + CurrentYearEarnLeaveDays;
                        int SepntEarnLeaveDays = getSpentEarnleave(EmpId, StartDate, EndDate.ToString("yyyy-MM-dd"));

                        double PayableEarnLeaveDays = TotalEarnLeaveDays - SepntEarnLeaveDays;
                        double WithdrawableEarnLeaveDays = 0;
                        double _ReserveForNextEL = 0;
                        double PayableAmount = 0;

                        if ((EmpJoiningDate.AddYears(1)).AddDays(-1) <= EndDate)// checking 1 year validation
                        {
                            if (PreviousYearEarnLeaveDays > SepntEarnLeaveDays)// PreviousYearEarnLeaveDays হচ্ছে গত বছরের অর্জিত ছুটির অবশিষ্টের ৫০ শতাংশ যা এ বছরের জন্য রিজার্ভ হয়েছিলো । তাই এ বছর ছুটি কাটানোর পরও যদি গত বছরের সেই ছুটি থেকে যায় তাহলে তা আগামী বছরের জন্য সরাসরি রিজার্ভ হবে। এবছরের পেমেন্টে আসবে নাহ কারণ গত বছর এর জন্য পেমেন্ট করা হয়েছে।  
                            {
                                WithdrawableEarnLeaveDays = (CurrentYearEarnLeaveDays * WithdrawableEarnLeavePer) / 100;

                            }
                            else
                            {
                                WithdrawableEarnLeaveDays = (PayableEarnLeaveDays * WithdrawableEarnLeavePer) / 100;

                            }

                            WithdrawableEarnLeaveDays = Math.Ceiling(WithdrawableEarnLeaveDays);
                            _ReserveForNextEL = (PayableEarnLeaveDays - WithdrawableEarnLeaveDays);


                        }
                        else
                        {
                            PayableEarnLeaveDays = TotalEarnLeaveDays - SepntEarnLeaveDays;
                            _ReserveForNextEL = PayableEarnLeaveDays;
                        }
                        double TotalAmount = 0;
                        if (WithdrawableEarnLeaveDays > 0)
                        {
                            PayableAmount = OneDaySalary * WithdrawableEarnLeaveDays;
                            if (PayableAmount > 0)
                                TotalAmount = PayableAmount - StampDeduction;
                            else
                            {
                                StampDeduction = 0;
                            }
                        }
                        else
                            StampDeduction = 0;

                        int PaymentID = saveToEarnleavePaymentSheet(CompanyId, EmpId, dtEarnLeaveEmployee.Rows[i]["EmpTypeId"].ToString(), dtEarnLeaveEmployee.Rows[i]["EmpStatus"].ToString(), YearMonth, StartDate, EndDate.ToString("yyyy-MM-dd"), CurrentYearEarnLeaveDays, PreviousYearEarnLeaveDays, TotalEarnLeaveDays, SepntEarnLeaveDays, PayableEarnLeaveDays, WithdrawableEarnLeaveDays, PayableAmount, StampDeduction, TotalAmount, EmpPresentSalary, BasicSalary, OneDaySalary, dtEarnLeaveEmployee.Rows[i]["DptId"].ToString(), dtEarnLeaveEmployee.Rows[i]["GId"].ToString(), dtEarnLeaveEmployee.Rows[i]["DsgId"].ToString(), dtEarnLeaveEmployee.Rows[i]["GrdName"].ToString(), IsSeparated, WithdrawableEarnLeavePer);
                        if (PaymentID > 0)
                        {
                            string ReserveFor = (EndDate.AddDays(1)).ToString("yyyy-MM-dd");
                            CRUD.Execute(@"INSERT INTO [dbo].[Earnleave_Reserved1]([PaymentID],[CompanyId],[EmpID],[ReserveFor],[GenerateDate],[ReserveEeanLeaveDays],[EntryDatetime])
                          VALUES(" + PaymentID + ",'" + CompanyId + "','" + EmpId + "','" + ReserveFor + "','" + EndDate.ToString("yyyy-MM-dd") + "','" + _ReserveForNextEL + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "')", sqlDB.connection);
                        }
                    }
                    lblMessage.InnerText = "success-> Successfully Generated.";
                    loadEarnLeaveGeneration();
                }
                else
                    lblMessage.InnerText = "warning-> No eligible employee found, who can get earn leave payment.";
                imgLoading.Visible = false;
            }
            catch (Exception ex)
            {

            }


        }
        private DataTable getEarnLeaveSettings()
        {
            try
            {
                sqlCmd = @"select PaymentOn,WithdrawablePer from Earnleave_Setting1";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                return dt;
            }
            catch { return null; }
        }
        private int getSpentEarnleave(string EmpId, string StartDate, string EndDate)
        {
            try
            {
                sqlCmd = @"select count(LACode) as SepntEarnLeave from v_Leave_LeaveApplicationDetails where ShortName='a/l' and LeaveDate>='" + StartDate + "' and LeaveDate<='" + EndDate + "' and EmpId='" + EmpId + "'";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                if (dt != null && dt.Rows.Count > 0)
                    return int.Parse(dt.Rows[0]["SepntEarnLeave"].ToString());
                else
                    return 0;
            }
            catch { return 0; }

        }
        private int getReserveEeanLeaveDays(string EmpId, string ReserveFor)
        {
            try
            {
                sqlCmd = @"select  ReserveEeanLeaveDays  from Earnleave_Reserved1 where ReserveFor = '" + ReserveFor + "' and EmpID='" + EmpId + "'";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                if (dt != null && dt.Rows.Count > 0)
                    return int.Parse(dt.Rows[0]["ReserveEeanLeaveDays"].ToString());
                else
                    return 0;
            }
            catch (Exception ex) { return 0; }

        }
        //private int saveToEarnleavePaymentSheet(string CompanyId, string EmpID, string EmpTypeId, string EmpStatus, string YearMonth, string StartDate, string EndDate, int TotalEarnLeaveDays, int SepntEarnLeaveDays, double PayableEarnLeaveDays, double WithdrawableEarnLeaveDays, double PayableAmount,
        //    double StampDeductions, double TotalAmount, double EmpPresentSalary, double BasicSalary, double OneDaySalary, string DptId, string GId, string DsgId, string GrdName, string IsSeparated, double WithdrawableEarnLeavePer)
        //{
        //    try
        //    {
        //        sqlCmd = @"INSERT INTO [dbo].[Payroll_EarnLeavePaymentSheet1]([CompanyId],[EmpId],[EmpTypeId],[EmpStatus],[YearMonth],[StartDate],[EndDate],[TotalEarnLeaveDays],
        //                 [SepntEarnLeaveDays],[PayableEarnLeaveDays],[WithdrawableEarnLeaveDays],[PayableAmount],[StampDeductions],[TotalAmount],[EmpPresentSalary],[BasicSalary],[OneDaySalary],[DptId],[GId],[DsgId],[GrdName],[GenerateTime],[IsSeparated],[WithdrawableEarnLeavePer])
        //                 VALUES('" + CompanyId + "','" + EmpID + "'," + EmpTypeId + "," + EmpStatus + ",'" + YearMonth + "','" + StartDate + "','" + EndDate + "'," + TotalEarnLeaveDays + "," + SepntEarnLeaveDays + "," +
        //                 PayableEarnLeaveDays + ",'" + WithdrawableEarnLeaveDays + "','" + PayableAmount + "','" + StampDeductions + "','" + TotalAmount + "','" + EmpPresentSalary + "','" + BasicSalary + "','" + OneDaySalary + "','" + DptId + "','" + GId + "','" + DsgId + "','" + GrdName + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + IsSeparated + ",'" + WithdrawableEarnLeavePer + "'); SELECT SCOPE_IDENTITY()";
        //        return CRUD.ExecuteReturnID(sqlCmd, sqlDB.connection);
        //    }
        //    catch (Exception ex)
        //    {
        //        return 0;
        //    }

        //}
        private int saveToEarnleavePaymentSheet(string CompanyId, string EmpID, string EmpTypeId, string EmpStatus, string YearMonth, string StartDate, string EndDate, int CurrentYearEarnLeaveDays, int PreviousYearEarnLeaveDays, int TotalEarnLeaveDays, int SepntEarnLeaveDays, double PayableEarnLeaveDays, double WithdrawableEarnLeaveDays, double PayableAmount,
           double StampDeductions, double TotalAmount, double EmpPresentSalary, double BasicSalary, double OneDaySalary, string DptId, string GId, string DsgId, string GrdName, string IsSeparated, double WithdrawableEarnLeavePer)
        {
            try
            {
                sqlCmd = @"INSERT INTO [dbo].[Payroll_EarnLeavePaymentSheet1]([CompanyId],[EmpId],[EmpTypeId],[EmpStatus],[YearMonth],[StartDate],[EndDate],[CurrentYearEarnLeaveDays],[PreviousYearEarnLeaveDays],[TotalEarnLeaveDays],
                         [SepntEarnLeaveDays],[PayableEarnLeaveDays],[WithdrawableEarnLeaveDays],[PayableAmount],[StampDeductions],[TotalAmount],[EmpPresentSalary],[BasicSalary],[OneDaySalary],[DptId],[GId],[DsgId],[GrdName],[GenerateTime],[IsSeparated],[WithdrawableEarnLeavePer])
                         VALUES('" + CompanyId + "','" + EmpID + "'," + EmpTypeId + "," + EmpStatus + ",'" + YearMonth + "','" + StartDate + "','" + EndDate + "'," + CurrentYearEarnLeaveDays + "," + PreviousYearEarnLeaveDays + "," + TotalEarnLeaveDays + "," + SepntEarnLeaveDays + "," +
                         PayableEarnLeaveDays + ",'" + WithdrawableEarnLeaveDays + "','" + PayableAmount + "','" + StampDeductions + "','" + TotalAmount + "','" + EmpPresentSalary + "','" + BasicSalary + "','" + OneDaySalary + "','" + DptId + "','" + GId + "','" + DsgId + "','" + GrdName + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + IsSeparated + ",'" + WithdrawableEarnLeavePer + "'); SELECT SCOPE_IDENTITY()";
                return CRUD.ExecuteReturnID(sqlCmd, sqlDB.connection);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        protected void gvEarnLeaveGenerationList_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            try
            {


                if (e.CommandName == "Remove")
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    string CompanyId = gvEarnLeaveGenerationList.DataKeys[rIndex].Values[0].ToString();
                    string StartDate = gvEarnLeaveGenerationList.DataKeys[rIndex].Values[1].ToString();
                    string EndDate = gvEarnLeaveGenerationList.DataKeys[rIndex].Values[2].ToString();
                    string IsSeparated = (gvEarnLeaveGenerationList.DataKeys[rIndex].Values[3].ToString().Equals("True")) ? "1" : "0";
                    if (deleteExEarnLeave(CompanyId, StartDate, EndDate, IsSeparated, ""))
                    {
                        lblMessage.InnerText = "warning-> Successfully Deleted.";
                        gvEarnLeaveGenerationList.Rows[rIndex].Visible = false;
                        imgLoading.Visible = false;
                    }

                }
            }
            catch { }
        }
        private bool deleteExEarnLeave(string CompanyId, string StartDate, string EndDate, string IsSeparated, string EmpId)
        {
            try
            {
                if (EmpId == "")
                    sqlCmd = "delete Payroll_EarnLeavePaymentSheet1 where IsSeparated=" + IsSeparated + " and CompanyId='" + CompanyId + "' and StartDate='" + StartDate + "' and EndDate='" + EndDate + "'";
                else
                    sqlCmd = "delete Payroll_EarnLeavePaymentSheet1 where IsSeparated=" + IsSeparated + " and CompanyId='" + CompanyId + "' and StartDate='" + StartDate + "' and EndDate='" + EndDate + "' and EmpId in(" + EmpId + ")";
                return CRUD.Execute(sqlCmd, sqlDB.connection);
            }
            catch (Exception ex) { lblMessage.InnerText = "error-> " + ex.Message; return false; }
        }
    }
}