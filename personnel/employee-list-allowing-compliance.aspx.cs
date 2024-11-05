using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.personnel
{

    //view=286;
    public partial class employee_list_allowing_compliance : System.Web.UI.Page
    {
        DataTable dt;
        string query = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 286 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);
                
                setPrivilege();
                loadPendingWorkers();
            }
        }
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__dptID__"] = getCookies["__DptId__"].ToString();
                ViewState["__empId__"] = getCookies["__getEmpId__"].ToString();

                DropDownList ddlCompanyList = new DropDownList();
                Button btnSearch = new Button();
                string[] AccessPermission = new string[0];
                classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForList(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "Employee.aspx", ddlCompanyList, gvForApprovedList, btnSearch);
                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];
            }
            catch { }
        }
        private void loadPendingWorkers()
        {
            string condition = AccessControl.getDataAccessCondition(ViewState["__CompanyId__"].ToString(),"0");

            query = "select EmpId,CompanyId,EmpType,SUBSTRING(EmpCardNo,8,6)+' ('+EmpProximityNo+')' as EmpCardNo,convert(varchar(10),EmpJoiningDate,105) as EmpJoiningDate,EmpName,DptName,DsgName,DptId from v_EmployeeDetails where  EmpTypeId=1 and IsActive=1 and EmpStatus=1 and IsTransferredToCompliance is null and " + condition + " order by DptId";
            sqlDB.fillDataTable(query, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Data not found";
                gvForApprovedList.DataSource = null;
                gvForApprovedList.DataBind();
                return;
            }
            gvForApprovedList.DataSource = dt;
            gvForApprovedList.DataBind();
        }
        protected void gvForApprovedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

                

                if (e.CommandName == "edit")
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    string EmpID = gvForApprovedList.DataKeys[rIndex].Values[0].ToString();
                    string CompanyId = gvForApprovedList.DataKeys[rIndex].Values[1].ToString();
                    Response.Redirect("/personnel/employee.aspx?EmpId=" + EmpID + "&CompanyId=" + CompanyId + "&Edit=True &Transfer=False");
                }
                else if (e.CommandName == "allow")
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    string EmpID = gvForApprovedList.DataKeys[rIndex].Values[0].ToString();

                    if (allowToCompliance(EmpID))
                    {
                        updateIsTrasfered(EmpID);
                        lblMessage.InnerText = "success-> Successfully Done.";
                        gvForApprovedList.Rows[rIndex].Visible = false;
                    }
                    else
                        lblMessage.InnerText = "error-> Unable to Submit.";

                }
                
            }
            catch { }
        }
        private void updateIsTrasfered(string EmpID)
        {
            try {
                query = "update Personnel_EmployeeInfo set IsTransferredToCompliance=1 where  EmpId='"+EmpID+"'";
                CRUD.Execute(query, sqlDB.connection);
            }
            catch(Exception ex) { }
        }
        private bool allowToCompliance(string EmpID)
        {
            try {
                query= @"INSERT INTO [dbo].[Personnel_EmpCurrentStatus1]
           ([EmpId]
           ,[PreCompanyId]
           ,[CompanyId]
           ,[EmpCardNo]
           ,[PreEmpTypeId]
           ,[EmpTypeId]
           ,[PreSalaryType]
           ,[SalaryType]
           ,[EmpJoinigSalary]
           ,[PreEmpSalary]
           ,[EmpPresentSalary]
           ,[PreIncrementAmount]
           ,[IncrementAmount]
           ,[PreBasicSalary]
           ,[BasicSalary]
           ,[PreMedicalAllownce]
           ,[MedicalAllownce]
           ,[PreFoodAllownce]
           ,[FoodAllownce]
           ,[PreConvenceAllownce]
           ,[ConvenceAllownce]
           ,[PreHouseRent]
           ,[HouseRent]
           ,[PreTechnicalAllownce]
           ,[TechnicalAllownce]
           ,[PreDptId]
           ,[DptId]
           ,[PreDsgId]
           ,[DsgId]
           ,[PreEmpStatus]
           ,[EmpStatus]
           ,[PreGrdName]
           ,[GrdName]
           ,[PreOthersAllownce]
           ,[OthersAllownce]
           ,[PreHolidayAllownce]
           ,[HolidayAllownce]
           ,[PreTiffinAllownce]
           ,[TiffinAllownce]
           ,[PreNightAllownce]
           ,[NightAllownce]
           ,[PreAttendanceBonus]
           ,[AttendanceBonus]
           ,[PreLunchAllownce]
           ,[LunchAllownce]
           ,[LunchCount]
           ,[DateofUpdate]
           ,[TypeOfChange]
           ,[EffectiveMonth]
           ,[OrderRefNo]
           ,[OrderRefDate]
           ,[Remarks]
           ,[ActiveSalary]
           ,[EarnLeaveDate]
           ,[IsActive]
           ,[PreShiftTransferDate]
           ,[ShiftTransferDate]
           ,[ShiftTransferToDate]
           ,[SftId]
           ,[GId]
           ,[PreGId]
           ,[SalaryCount]
           ,[BankId]
           ,[EmpAccountNo]
           ,[PfMember]
           ,[PfDate]
           ,[PFAmount]
           ,[CustomOrdering]
           ,[OverTime]
           ,[PreEmpDutyType]
           ,[EmpDutyType]
           ,[DormitoryRent]
           ,[PreIncomeTax]
           ,[IncomeTax]
           ,[Tin]
           ,[EmpContributionPer]
           ,[EmpContributionAmount]
           ,[EmprContributionPer]
           ,[EmprContributionAmount])
    select[EmpId]
           ,[PreCompanyId]
           ,[CompanyId]
           ,[EmpCardNo]
           ,[PreEmpTypeId]
           ,[EmpTypeId]
           ,[PreSalaryType]
           ,[SalaryType]
           ,[EmpJoinigSalary]
           ,[PreEmpSalary]
           ,[EmpPresentSalary]
           ,[PreIncrementAmount]
           ,[IncrementAmount]
           ,[PreBasicSalary]
           ,[BasicSalary]
           ,[PreMedicalAllownce]
           ,[MedicalAllownce]
           ,[PreFoodAllownce]
           ,[FoodAllownce]
           ,[PreConvenceAllownce]
           ,[ConvenceAllownce]
           ,[PreHouseRent]
           ,[HouseRent]
           ,[PreTechnicalAllownce]
           ,[TechnicalAllownce]
           ,[PreDptId]
           ,[DptId]
           ,[PreDsgId]
           ,[DsgId]
           ,[PreEmpStatus]
           ,[EmpStatus]
           ,[PreGrdName]
           ,[GrdName]
           ,[PreOthersAllownce]
           ,[OthersAllownce]
           ,[PreHolidayAllownce]
           ,[HolidayAllownce]
           ,[PreTiffinAllownce]
           ,[TiffinAllownce]
           ,[PreNightAllownce]
           ,[NightAllownce]
           ,[PreAttendanceBonus]
           ,[AttendanceBonus]
           ,[PreLunchAllownce]
           ,[LunchAllownce]
           ,[LunchCount]
           ,[DateofUpdate]
           ,[TypeOfChange]
           ,[EffectiveMonth]
           ,[OrderRefNo]
           ,[OrderRefDate]
           ,[Remarks]
           ,[ActiveSalary]
           ,[EarnLeaveDate]
           ,[IsActive]
           ,[PreShiftTransferDate]
           ,[ShiftTransferDate]
           ,[ShiftTransferToDate]
           ,[SftId]
           ,[GId]
           ,[PreGId]
           ,[SalaryCount]
           ,[BankId]
           ,[EmpAccountNo]
           ,[PfMember]
           ,[PfDate]
           ,[PFAmount]
           ,[CustomOrdering]
           ,[OverTime]
           ,[PreEmpDutyType]
           ,[EmpDutyType]
           ,[DormitoryRent]
           ,[PreIncomeTax]
           ,[IncomeTax]
           ,[Tin]
           ,[EmpContributionPer]
           ,[EmpContributionAmount]
           ,[EmprContributionPer]
           ,[EmprContributionAmount]
        FROM[dbo].[Personnel_EmpCurrentStatus]
        where EmpId = '"+ EmpID + "' and IsActive = 1";
              return   CRUD.Execute(query,sqlDB.connection);
                
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        protected void gvForApprovedList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onmouseover"] = "javascript:SetMouseOver(this)";
                    e.Row.Attributes["onmouseout"] = "javascript:SetMouseOut(this)";
                }
            }
            catch { }
            if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            {
                Button btn;               
                try
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnEdit");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;
                    }

                }
                catch { }
                try
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnTransfer");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;
                    }

                }
                catch { }
            }
        }
    }
}