using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.leave
{
    public partial class yearly_leaveStatus_report : System.Web.UI.Page
    {
        //permission=311;
        DataTable dt;
        string companyId = "";
        string sqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 311 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                setPrivilege();
                loadYear();
                if (ddlYear != null && ddlYear.Items.Count > 0)
                    ddlYear.SelectedValue = DateTime.Now.Year.ToString();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
            }

        }
        private void loadYear()
        {
            try
            {
                DataTable dtYear = new DataTable();
                companyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                sqlDB.fillDataTable(" select distinct Year(ToDate) as Year  from Leave_LeaveApplication   where CompanyId='" + companyId + "' order by Year(ToDate) desc", dtYear);
                ddlYear.DataTextField = "Year";
                ddlYear.DataValueField = "Year";
                ddlYear.DataSource = dtYear;
                ddlYear.DataBind();
            }
            catch { }
        }
        DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];

                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                //string[] AccessPermission = new string[0];
                ////System.Web.UI.HtmlControls.HtmlTable a = tblGenerateType;
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "yearly_leaveStatus_report.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];               
                commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);              
                

            }
            catch { }

        }
        private void GenerateYearlyLeaveStarus() 
        {
            try
            {
                string CompanyList = "";
                string DepartmentList = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }         

                if (chkForAllCompany.Checked)
                {
                    CompanyList = classes.commonTask.getCompaniesList(ddlCompanyName);                    
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                }
                else
                {
                    string Cid = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                    CompanyList = "in ('" + Cid + "')";
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                    
                }
                string sqlCmd = "";
                string IsIndividual = "";
                if (txtCardNo.Text.Trim().Length > 0)
                {
                    if(txtCardNo.Text.Trim().Length<4)
                    {
                        lblMessage.InnerText = "warning-> Please Type Mininmum 4 Character of Card Number!";
                        return;
                    }
                    IsIndividual = "Yes";
                    sqlCmd = "SELECT  v_v_v_Leave_Yearly_Status.Year, v_v_v_Leave_Yearly_Status.EmpName,v_v_v_Leave_Yearly_Status.EmpCardNo,Sex, v_v_v_Leave_Yearly_Status.DptName," +
                   "v_v_v_Leave_Yearly_Status.SftName,v_v_v_Leave_Yearly_Status.CompanyName,v_v_v_Leave_Yearly_Status.Address,v_v_v_Leave_Yearly_Status.DsgName," +                
                   "v_v_v_Leave_Yearly_Status.CL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.CL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.CL_Remaining END As CL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.CL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.CL_Total END AS CL_Total," +
                   "v_v_v_Leave_Yearly_Status.SL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.SL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.SL_Remaining END As SL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.SL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.SL_Total END AS SL_Total," +
                    "v_v_v_Leave_Yearly_Status.AL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.AL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.AL_Remaining END As AL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.AL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.AL_Total END AS AL_Total," +
                   "v_v_v_Leave_Yearly_Status.ML_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.ML_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.ML_Remaining END As ML_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.ML_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.ML_Total END AS ML_Total," +
                   "v_v_v_Leave_Yearly_Status.OPL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.OPL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OPL_Remaining END As OPL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.OPL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OPL_Total END AS OPL_Total," +
                   "v_v_v_Leave_Yearly_Status.OL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.OL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OL_Remaining END As OL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.OL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OL_Total END AS OL_Total" +                   
                   " FROM  dbo.v_v_v_Leave_Yearly_Status" +
                   " where Year ='" + ddlYear.SelectedValue + "'  AND CompanyId " + CompanyList + " and EmpCardNo like '%"+txtCardNo.Text.Trim()+"'" +
                   "ORDER BY v_v_v_Leave_Yearly_Status.CompanyName,v_v_v_Leave_Yearly_Status.SftName,v_v_v_Leave_Yearly_Status.DptName";
                }
                else {
                    IsIndividual = "No";
                    sqlCmd = "SELECT  v_v_v_Leave_Yearly_Status.Year, v_v_v_Leave_Yearly_Status.EmpName,substring(v_v_v_Leave_Yearly_Status.EmpCardNo,8,15) as EmpCardNo, v_v_v_Leave_Yearly_Status.DptName," +
                   "v_v_v_Leave_Yearly_Status.SftName,v_v_v_Leave_Yearly_Status.CompanyName,v_v_v_Leave_Yearly_Status.Address,v_v_v_Leave_Yearly_Status.DsgName," +
                   "v_v_v_Leave_Yearly_Status.CL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.CL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.CL_Remaining END As CL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.CL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.CL_Total END AS CL_Total," +
                   "v_v_v_Leave_Yearly_Status.SL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.SL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.SL_Remaining END As SL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.SL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.SL_Total END AS SL_Total," +
                    "v_v_v_Leave_Yearly_Status.AL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.AL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.AL_Remaining END As AL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.AL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.AL_Total END AS AL_Total," +
                   "v_v_v_Leave_Yearly_Status.ML_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.ML_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.ML_Remaining END As ML_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.ML_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.ML_Total END AS ML_Total," +
                   "v_v_v_Leave_Yearly_Status.OPL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.OPL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OPL_Remaining END As OPL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.OPL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OPL_Total END AS OPL_Total," +
                   "v_v_v_Leave_Yearly_Status.OL_Spend, CASE  WHEN v_v_v_Leave_Yearly_Status.OL_Remaining IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OL_Remaining END As OL_Remaining,CASE  WHEN v_v_v_Leave_Yearly_Status.OL_Total IS NULL THEN 0 ELSE v_v_v_Leave_Yearly_Status.OL_Total END AS OL_Total" +  
                " FROM  dbo.v_v_v_Leave_Yearly_Status" +
                " where Year ='" + ddlYear.SelectedValue + "'  AND CompanyId " + CompanyList + " AND " +
                " DptId " + DepartmentList + "" +
                "ORDER BY v_v_v_Leave_Yearly_Status.CompanyName,v_v_v_Leave_Yearly_Status.SftName,v_v_v_Leave_Yearly_Status.DptName";
 
                }
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());

                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Sorry any record are not founded"; return;         
                }
               
                Session["__YearlyLeaveStatus__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=YearlyLeaveStatus-"+IsIndividual+"');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }

        private void GenerateYearlyLeaveStarus_SG()
        {
            try
            {

              
                if (lstSelected.Items.Count < 1 && txtCardNo.Text.Trim().Length == 0)
                { lblMessage.InnerText = "warning-> Please Select Department!"; lstSelected.Focus(); return; }
                string CompanyList = "";
                string DepartmentList = "";
                string empType = "";
                string _empType = "";
          
                    
                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }
             
                ViewState["__FromDate__"] = ddlYear.SelectedValue + "-01-01";
                ViewState["__ToDate__"] = ddlYear.SelectedValue + "-12-31"; 

                if (chkForAllCompany.Checked)
                {
                    CompanyList = classes.commonTask.getCompaniesList(ddlCompanyName);                  
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                }
                else
                {
                    string Cid = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                    CompanyList = "in ('" + Cid + "')";
                   
                }
                if (!ckbIndividualDetails.Checked)
                {
                    
                        
                    if (txtCardNo.Text.Trim().Length == 0)
                    {
                        if (rblEmpType.SelectedValue != "All")
                        {
                            empType = " and EmpTypeID=" + rblEmpType.SelectedValue;
                            _empType = "(" + rblEmpType.SelectedItem.Text + ")";
                        }
                        DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                        //sqlCmd = @"with bEL as (
                        //select EmpID, ReserveEeanLeaveDays as EarnLeaveDays from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue + "-01-01' union all select EmpID, EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + "-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + "-12-31'), bEL1 as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from bEL  group by EmpID) " +
                        //"SELECT ld.EmpId, EmpName,substring(EmpCardNo,10,6) EmpCardNo, Sex, SUM(case when ShortName='c/l' then TotalDays else 0 end) AS CL, SUM(case when ShortName='s/l' then TotalDays else 0 end) AS SL, SUM(case when ShortName='m/l' then TotalDays else 0 end) AS ML, SUM(case when ShortName='a/l' then TotalDays else 0 end) AS AL,DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,ISNULL(bEL1.EarnLeaveDays,0) as  bEL FROM dbo.v_Leave_LeaveApplication AS ld left join bEL1 on ld.EmpId=bEL1.EmpID   where IsApproved=1 and ld.FromDate>='" + ViewState["__FromDate__"].ToString() + "' AND ld.FromDate<='" + ViewState["__ToDate__"].ToString() + "' AND CompanyId " + CompanyList + " AND " +
                        //"  DptId " + DepartmentList + " " + empType + "  " +
                        //"GROUP BY ld.EmpId, EmpName, EmpCardNo, Sex, DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,bEL1.EarnLeaveDays order by substring(EmpCardNo,10,6) ";

                        sqlCmd = @" with  pEL as(select EmpID, ReserveEeanLeaveDays from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue + @"-01-01'),
 bEL as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + @"-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + @"-12-31' group by EmpID),
                       lv as (
                       select EmpId, sum(case when ShortName='c/l' then TotalDays else 0 end ) as CL, sum(case when ShortName='s/l' then TotalDays else 0 end ) as SL, sum(case when ShortName='a/l' then TotalDays else 0 end ) as AL, sum(case when ShortName='m/l' then TotalDays else 0 end ) as ML, sum(case when ShortName='wp/l' then TotalDays else 0 end ) as LWP from Leave_LeaveApplication lva left join  tblLeaveConfig c on lva.LeaveId=c.LeaveId where  IsApproved=1 and FromDate >= '" + ddlYear.SelectedValue + @"-01-01' AND FromDate <= '" + ddlYear.SelectedValue + @"-12-31'  group by EmpId)
					   , ed as (select * from v_EmployeeDetails where IsActive=1 "+ empType + @" AND CompanyId " + CompanyList + " AND " +
                        "  DptId " + DepartmentList + @")
                      SELECT ed.EmpId,convert(varchar(10),EmpJoiningDate,105) as SftName, EmpName,substring(EmpCardNo,10,6)+' ('+ed.EmpProximityNo+')' EmpCardNo, Sex,  isnull(CL,0) as CL,ISNULL(SL,0) as SL,ISNULL(ML,0) as ML,ISNULL(AL,0) as AL,ISNULL(LWP,0) as LWP,DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,ISNULL(bEL.EarnLeaveDays,0) as  bEL,ISNULL(pEL.ReserveEeanLeaveDays,0) as pEL FROM lv right join ed on lv.EmpId=ed.EmpId left join bEL on ed.EmpId=bEL.EmpID left join pEl on ed.EmpId=pEl.EmpID where ed.IsActive=1 and lv.EmpId is not null or ( lv.EmpId  is null  and ed.EmpStatus in(1,8)) and ed.EmpJoiningDate < '" + ddlYear.SelectedValue + @"-12-31'   order by convert(int, DptId),convert(int,SUBSTRING(EmpCardNo,10,6))";
                    }
                    else
                    {
                        if (rblEmpType.SelectedValue != "All")                        {
                            
                            _empType = "(" + rblEmpType.SelectedItem.Text + ")";
                        }
                        //sqlCmd = @"with bEL as (
                        //select EmpID, ReserveEeanLeaveDays as EarnLeaveDays from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue + "-01-01' union all select EmpID, EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + "-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + "-12-31'), bEL1 as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from bEL  group by EmpID) " +
                        //"SELECT ld.EmpId, EmpName,substring(EmpCardNo,10,6) EmpCardNo, Sex, SUM(case when ShortName='c/l' then TotalDays else 0 end) AS CL, SUM(case when ShortName='s/l' then TotalDays else 0 end) AS SL, SUM(case when ShortName='m/l' then TotalDays else 0 end) AS ML, SUM(case when ShortName='a/l' then TotalDays else 0 end) AS AL,DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,ISNULL(bEL1.EarnLeaveDays,0) as  bEL FROM dbo.v_Leave_LeaveApplication AS ld left join bEL1 on ld.EmpId=bEL1.EmpID   where IsApproved=1 and ld.FromDate>='" + ViewState["__FromDate__"].ToString() + "' AND ld.FromDate<='" + ViewState["__ToDate__"].ToString() + "' AND CompanyId " + CompanyList + " AND " +
                        //                   " EmpCardNo like'%" + txtCardNo.Text + "'" +
                        //                   "GROUP BY ld.EmpId, EmpName, EmpCardNo, Sex, DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,bEL1.EarnLeaveDays";
                        string EmpId = Employee.getEmpIdByCardNo(ddlCompanyName.SelectedValue, txtCardNo.Text);
                        if (EmpId == "")
                        {
                            lblMessage.InnerText = "warning-> Invalid Card No!";
                            return;
                        }
                        sqlCmd = @" with  pEL as(select EmpID, ReserveEeanLeaveDays from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue + @"-01-01'),
 bEL as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + @"-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + @"-12-31' group by EmpID),
                       lv as (
                       select EmpId, sum(case when ShortName='c/l' then TotalDays else 0 end ) as CL, sum(case when ShortName='s/l' then TotalDays else 0 end ) as SL, sum(case when ShortName='a/l' then TotalDays else 0 end ) as AL, sum(case when ShortName='m/l' then TotalDays else 0 end ) as ML, sum(case when ShortName='wp/l' then TotalDays else 0 end ) as LWP from Leave_LeaveApplication lva left join  tblLeaveConfig c on lva.LeaveId=c.LeaveId where  IsApproved=1 and FromDate >= '" + ddlYear.SelectedValue + @"-01-01' AND FromDate <= '" + ddlYear.SelectedValue + @"-12-31' AND EmpId='"+ EmpId + @"' group by EmpId)
					   , ed as (select * from v_EmployeeDetails where IsActive=1 AND EmpId='" + EmpId + @"')" +
                     " SELECT ed.EmpId,convert(varchar(10),EmpJoiningDate,105) as SftName, EmpName,substring(EmpCardNo,10,6)+' ('+ed.EmpProximityNo+')' EmpCardNo, Sex,  isnull(CL,0) as CL,ISNULL(SL,0) as SL,ISNULL(ML,0) as ML,ISNULL(AL,0) as AL,ISNULL(LWP,0) as LWP,DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,ISNULL(bEL.EarnLeaveDays,0) as  bEL,ISNULL(pEL.ReserveEeanLeaveDays,0) as pEL FROM lv right join ed on lv.EmpId=ed.EmpId left join bEL on ed.EmpId=bEL.EmpID left join pEl on ed.EmpId=pEl.EmpID   where ed.IsActive=1  and ed.EmpJoiningDate < '" + ddlYear.SelectedValue + @"-12-31' AND CompanyId " + CompanyList +
                        " AND ed.EmpId='" + EmpId + @"' order by convert(int, DptId),convert(int,SUBSTRING(EmpCardNo,10,6)) ";
                    }
                    //sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                    dt = new DataTable();
                    dt=CRUD.ExecuteReturnDataTable(sqlCmd);
                    if (dt==null|| dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found."; return;
                    }
                    DataTable dtLeave;
                    sqlDB.fillDataTable("select sum( case when ShortName='c/l' then LeaveDays end) as CL ,sum( case when ShortName='s/l' then LeaveDays end) as SL ,sum( case when ShortName='m/l' then LeaveDays end) as ML ,sum( case when ShortName='a/l' then LeaveDays end) as EL from tblLeaveConfig where CompanyId " + CompanyList + "", dtLeave = new DataTable());
                    Session["__dtLeave__"] = dtLeave;
                    Session["__LeaveYearlySummary__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveYearlySummary-" + ddlYear.SelectedValue + "-" + _empType + "');", true);  //Open New Tab for Sever side code
                }
                else 
                {
                   
                    if (txtCardNo.Text.Trim().Length == 0)
                    {
                        if (rblEmpType.SelectedValue != "All")
                            empType = " and lv.EmpTypeID=" + rblEmpType.SelectedValue;
                        DepartmentList = classes.commonTask.getDepartmentList(lstSelected);      
                        sqlCmd = @"with  ed as (select * from v_EmployeeDetails where IsActive=1  AND CompanyId " + CompanyList + " AND   DptId " + DepartmentList 
                            +"), " +
                         " pEL as(select EmpID, ReserveEeanLeaveDays  from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue+ "-01-01') , bEL as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + "-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + "-12-31'  group  by EmpID) " +
                         "select ed.CompanyId,lv.EmpId,ed.CompanyName,ed.Address,TotalDays, LACode, ShortName,LeaveName,SUBSTRING(ed.EmpCardNo,10,6) EmpCardNo,ed.EmpName,ed.DptId,ed.DptName,ed.DsgId,ed.DsgName, convert(varchar, ApplyDate,105) ApplyDate,convert(varchar, FromDate,105) FromDate,convert(varchar, ToDate,105) ToDate,case when ShortName='s/l'  then TotalDays else 0  end SL,case when ShortName='c/l' then TotalDays else 0  end CL,case when ShortName='a/l' then TotalDays else 0 end EL ,case when  ShortName='m/l' then TotalDays else 0 end ML  ,ed.Sex as EmpType,ISNULL(bEL.EarnLeaveDays,0) as  bEL,ISNULL(pEL.ReserveEeanLeaveDays,0) as pEL,ed.EmpProximityNo as OPL,convert(varchar(10), ed.EmpJoiningDate,105) as OL from v_Leave_LeaveApplication lv inner join ed on lv.EmpId=ed.EmpId  left join bEL on ed.EmpId=bEL.EmpID left join pEL on ed.EmpID=pEL.EmpID  where IsApproved=1   " + empType + " and year(FromDate)='" + ddlYear.SelectedValue + "' and lv.CompanyId " + CompanyList + " AND lv.DptId " + DepartmentList +
                            " order by convert(int, ed.DptId),convert(int,SUBSTRING(ed.EmpCardNo,10,6)),year(FromDate),month(FromDate),FromDate ";
                    }
                    else
                    {
                        string EmpId = Employee.getEmpIdByCardNo(ddlCompanyName.SelectedValue,txtCardNo.Text);
                        if (EmpId == "")
                        {
                            lblMessage.InnerText = "warning->Invalid Card No!"; return;
                        }
                        sqlCmd = @"with  ed as (select * from v_EmployeeDetails where IsActive=1  AND CompanyId " + CompanyList + " and EmpId='"+ EmpId + @"')," +
                         " pEL as(select EmpID, ReserveEeanLeaveDays  from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue + "-01-01') , bEL as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + "-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + "-12-31'  group  by EmpID) " +
                                    "select ed.CompanyId,lv.EmpId,ed.CompanyName,ed.Address,TotalDays, LACode, ShortName,LeaveName,SUBSTRING(ed.EmpCardNo,10,6) EmpCardNo,ed.EmpName,ed.DptId,ed.DptName,ed.DsgId,ed.DsgName, convert(varchar, ApplyDate,105) ApplyDate,convert(varchar, FromDate,105) FromDate,convert(varchar, ToDate,105) ToDate,case when ShortName='s/l'  then TotalDays else 0  end SL,case when ShortName='c/l' then TotalDays else 0  end CL,case when ShortName='a/l' then TotalDays else 0 end EL ,case when  ShortName='m/l' then TotalDays else 0 end ML  ,ed.Sex as EmpType,ISNULL(bEL.EarnLeaveDays,0) as  bEL,ISNULL(pEL.ReserveEeanLeaveDays,0) as pEL,ed.EmpProximityNo as OPL,convert(varchar(10), ed.EmpJoiningDate,105) as OL from v_Leave_LeaveApplication lv inner join ed on lv.EmpId=ed.EmpId  left join bEL on ed.EmpId=bEL.EmpID left join pEL on ed.EmpID=pEL.EmpID  where IsApproved=1  and year(FromDate)='" + ddlYear.SelectedValue + "' and lv.EmpId='" + EmpId + @"'  order by year(FromDate),month(FromDate),  FromDate ";
                    }

                    //sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                    dt = new DataTable();
                    dt = CRUD.ExecuteReturnDataTable(sqlCmd);
                    if (dt.Rows.Count == 0)
                    {
                        lblMessage.InnerText = "warning->Data not found."; return;
                    }
                    DataTable dtLeave;
                    sqlDB.fillDataTable("select sum( case when ShortName='c/l' then LeaveDays end) as CL ,sum( case when ShortName='s/l' then LeaveDays end) as SL ,sum( case when ShortName='m/l' then LeaveDays end) as ML ,sum( case when ShortName='a/l' then LeaveDays end) as EL from tblLeaveConfig where CompanyId " + CompanyList + "", dtLeave = new DataTable());
                    Session["__dtLeave__"] = dtLeave;
                    Session["__LeaveYearlySummaryIndividualDetails__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveYearlySummaryIndividualDetails-" + ddlYear.SelectedValue + "');", true);  //Open New Tab for Sever side code              
                  
                }
            }
            catch { }
        }
        

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {


            companyId= (ddlCompanyName.SelectedValue=="0000")?ViewState["__CompanyId__"].ToString():ddlCompanyName.SelectedValue.ToString();
            lstSelected.Items.Clear();
            commonTask.LoadDepartment(companyId, lstAll);
            loadYear();
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
           //GenerateYearlyLeaveStarus();
            GenerateYearlyLeaveStarus_SG();
        }
    }
}