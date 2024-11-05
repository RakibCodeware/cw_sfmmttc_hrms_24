﻿using adviitRuntimeScripting;
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
    public partial class Earn_leave_Report : System.Web.UI.Page
    {
        DataTable dt;
        string companyId = "";
        string sqlCmd = "";
        //permission=450
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 449 };

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
                    if (txtCardNo.Text.Trim().Length < 4)
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
                   " where Year ='" + ddlYear.SelectedValue + "'  AND CompanyId " + CompanyList + " and EmpCardNo like '%" + txtCardNo.Text.Trim() + "'" +
                   "ORDER BY v_v_v_Leave_Yearly_Status.CompanyName,v_v_v_Leave_Yearly_Status.SftName,v_v_v_Leave_Yearly_Status.DptName";
                }
                else
                {
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
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=YearlyLeaveStatus-" + IsIndividual + "');", true);  //Open New Tab for Sever side code
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

                    sqlCmd = @"SELECT
                        ed.EmpName,FORMAT(ed.EmpJoiningDate, 'dd-MM-yyyy') AS EmpJoiningDate,SUBSTRING( ed.EmpCardNo,10,4)+' ('+ed.EmpProximityNo+')' as EmpCardNo, ed.DptName,ed.DsgName,ed.CompanyId,ed.CompanyName,ed.Address,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 1 THEN elb.EarnLeaveDays ELSE 0 END) AS January,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 2 THEN elb.EarnLeaveDays ELSE 0 END) AS February,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 3 THEN elb.EarnLeaveDays ELSE 0 END) AS March,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 4 THEN elb.EarnLeaveDays ELSE 0 END) AS April,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 5 THEN elb.EarnLeaveDays ELSE 0 END) AS May,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 6 THEN elb.EarnLeaveDays ELSE 0 END) AS June,
                      SUM(CASE WHEN MONTH(elb.GenerateDate) = 7 THEN elb.EarnLeaveDays ELSE 0 END) AS Julay,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 8 THEN elb.EarnLeaveDays ELSE 0 END) AS August,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 9 THEN elb.EarnLeaveDays ELSE 0 END) AS September,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 10 THEN elb.EarnLeaveDays ELSE 0 END) AS October,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 11 THEN elb.EarnLeaveDays ELSE 0 END) AS November,
                        SUM(CASE WHEN MONTH(elb.GenerateDate) = 12 THEN elb.EarnLeaveDays ELSE 0 END) AS December,
                      sum(elb.EarnLeaveDays)as TotalEarnLeaveDays
                        FROM
                           Earnleave_BalanceDetailsLog   as elb
                        INNER JOIN
                            v_EmployeeDetails as ed ON elb.EmpID = ed.EmpId and ed.IsActive=1

                        WHERE
                           Year( elb.GenerateDate )=" + ddlYear.SelectedValue + @" and DptId " + DepartmentList + empType + @" 
                        GROUP BY ed.EmpName,ed.EmpJoiningDate,SUBSTRING( ed.EmpCardNo,10,4)+' ('+ed.EmpProximityNo+')',ed.DptName,ed.DptId,ed.GId,ed.CustomOrdering, ed.DsgName,ed.CompanyId,ed.CompanyName,ed.Address order by CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                }
                else if (txtCardNo.Text.Trim().Length >= 4)
                {
                    if (rblEmpType.SelectedValue != "All")
                    {

                        _empType = "(" + rblEmpType.SelectedItem.Text + ")";
                    }
                    //sqlCmd = @"with bEL as (
                    //select EmpID, ReserveEeanLeaveDays as EarnLeaveDays from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue + "-01-01' union all select EmpID, EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + "-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + "-12-31'), bEL1 as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from bEL  group by EmpID) " +
                    //"SELECT ld.EmpId, EmpName,substring(EmpCardNo,10,6) EmpCardNo, Sex, SUM(case when ShortName='c/l' then TotalDays else 0 end) AS CL, SUM(case when ShortName='s/l' then TotalDays else 0 end) AS SL, SUM(case when ShortName='m/l' then TotalDays else 0 end) AS ML, SUM(case when ShortName='a/l' then TotalDays else 0 end) AS AL,DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,ISNULL(bEL1.EarnLeaveDays,0) as  bEL FROM dbo.v_Leave_LeaveApplication AS ld left join bEL1 on ld.EmpId=bEL1.EmpID   where IsApproved=1 and ld.FromDate>='" + ViewState["__FromDate__"].ToString() + "' AND ld.FromDate<='" + ViewState["__ToDate__"].ToString() + "' AND CompanyId " + CompanyList + " AND " +
                    //                   " EmpCardNo like'%" + txtCardNo.Text + "'" +
                    //sqlCmd"GROUP BY ld.EmpId, EmpName, EmpCardNo, Sex, DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,bEL1.EarnLeaveDays";


                    sqlCmd = @"SELECT
                                ed.EmpName,
                                FORMAT(ed.EmpJoiningDate, 'dd-MM-yyyy') AS EmpJoiningDate,
                                SUBSTRING(ed.EmpCardNo, 10, 4) + ' (' + ed.EmpProximityNo + ')' AS EmpCardNo,
                                ed.DptName,
                                ed.DsgName,
                                ed.CompanyId,
                                ed.CompanyName,
                                ed.Address,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 1 THEN elb.EarnLeaveDays ELSE 0 END) AS January,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 2 THEN elb.EarnLeaveDays ELSE 0 END) AS February,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 3 THEN elb.EarnLeaveDays ELSE 0 END) AS March,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 4 THEN elb.EarnLeaveDays ELSE 0 END) AS April,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 5 THEN elb.EarnLeaveDays ELSE 0 END) AS May,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 6 THEN elb.EarnLeaveDays ELSE 0 END) AS June,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 7 THEN elb.EarnLeaveDays ELSE 0 END) AS July,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 8 THEN elb.EarnLeaveDays ELSE 0 END) AS August,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 9 THEN elb.EarnLeaveDays ELSE 0 END) AS September,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 10 THEN elb.EarnLeaveDays ELSE 0 END) AS October,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 11 THEN elb.EarnLeaveDays ELSE 0 END) AS November,
                                SUM(CASE WHEN MONTH(elb.GenerateDate) = 12 THEN elb.EarnLeaveDays ELSE 0 END) AS December,
                                SUM(elb.EarnLeaveDays) AS TotalEarnLeaveDays
                            FROM
                                Earnleave_BalanceDetailsLog AS elb
                            INNER JOIN
                                v_EmployeeDetails AS ed ON elb.EmpID = ed.EmpId AND ed.IsActive = 1
                            WHERE
                                Year(elb.GenerateDate) = " + ddlYear.SelectedValue + @"
                                
                                AND EmpCardNo LIKE '%" + txtCardNo.Text.Trim() + @"%'
                            GROUP BY
                                ed.EmpName,
                                ed.EmpJoiningDate,
                                SUBSTRING(ed.EmpCardNo, 10, 4) + ' (' + ed.EmpProximityNo + ')',
                                ed.DptName,
                                ed.DsgName,
                                ed.CompanyId,
                                ed.CompanyName,
                                ed.Address";



                    //                       sqlCmd = @" with  pEL as(select EmpID, ReserveEeanLeaveDays from Earnleave_Reserved where ReserveFor = '" + ddlYear.SelectedValue + @"-01-01'),
                    //bEL as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + ddlYear.SelectedValue + @"-01-01' and GenerateDate<= '" + ddlYear.SelectedValue + @"-12-31' group by EmpID),
                    //                      lv as (
                    //                      select EmpId, sum(case when ShortName='c/l' then TotalDays else 0 end ) as CL, sum(case when ShortName='s/l' then TotalDays else 0 end ) as SL, sum(case when ShortName='a/l' then TotalDays else 0 end ) as AL, sum(case when ShortName='m/l' then TotalDays else 0 end ) as ML, sum(case when ShortName='wp/l' then TotalDays else 0 end ) as LWP from v_Leave_LeaveApplication where  IsApproved=1 and FromDate >= '" + ddlYear.SelectedValue + @"-01-01' AND FromDate <= '" + ddlYear.SelectedValue + @"-12-31' AND EmpCardNo like'%" + txtCardNo.Text + @"' group by EmpId)
                    //				   , ed as (select * from v_EmployeeDetails where IsActive=1 AND EmpCardNo like'%" + txtCardNo.Text + "')" +
                    //                    " SELECT ed.EmpId,convert(varchar(10),EmpJoiningDate,105) as SftName, EmpName,substring(EmpCardNo,10,6)+' ('+ed.EmpProximityNo+')' EmpCardNo, Sex,  isnull(CL,0) as CL,ISNULL(SL,0) as SL,ISNULL(ML,0) as ML,ISNULL(AL,0) as AL,ISNULL(LWP,0) as LWP,DptId, DptName, DsgId, DsgName, CompanyId, SftId, SftName,CompanyName,Address,ISNULL(bEL.EarnLeaveDays,0) as  bEL,ISNULL(pEL.ReserveEeanLeaveDays,0) as pEL FROM lv right join ed on lv.EmpId=ed.EmpId left join bEL on ed.EmpId=bEL.EmpID left join pEl on ed.EmpId=pEl.EmpID   where ed.IsActive=1  and ed.EmpJoiningDate < '" + ddlYear.SelectedValue + @"-12-31' AND CompanyId " + CompanyList +
                    //                       " AND EmpCardNo like'%" + txtCardNo.Text + "' order by convert(int, DptId),convert(int,SUBSTRING(EmpCardNo,10,6)) ";
                }

                else
                {
                    lblMessage.InnerText = "warning->Please Enter minimum 4 digit of your card no ."; return;
                }
                //sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(sqlCmd);
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Data not found."; return;
                }

                Session["__EarnLeaveBalance__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=EarnLeaveBalance-" + ddlYear.SelectedValue + "-" + rblEmpType.SelectedItem.Text + "');", true);  //Open New Tab for Sever side code


            }
            catch { }
        }


        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {


            companyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
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