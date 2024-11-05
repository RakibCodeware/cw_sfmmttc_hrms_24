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

namespace SigmaERP.attendance
{

    //permission=324;
    public partial class weekend_set_emp_wise : System.Web.UI.Page
    {
        DataTable dt;
        string CompanyId = "";
        string sqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 324 }; 
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);                
                setPrivilege();
                SetupTypeWiseShowHide();
            }
        }
        private void setPrivilege()
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                classes.commonTask.LoadBranch(ddlCompany, ViewState["__CompanyId__"].ToString());
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "holyday.aspx", ddlCompany, gvEmployeeList, btnSubmit);
                //ViewState["__ReadAction__"] = AccessPermission[0];
                //ViewState["__WriteAction__"] = AccessPermission[1];
                //ViewState["__UpdateAction__"] = AccessPermission[2];
                //ViewState["__DeletAction__"] = AccessPermission[3];
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
               
                //     classes.commonTask.getAuthorityList(ViewState["__CompanyId__"].ToString(),ckblAuthorityList);
                if (!classes.commonTask.HasBranch())
                    ddlCompany.Enabled = false;
                ddlCompany.SelectedValue = ViewState["__CompanyId__"].ToString();

            }


            catch { }

        }
        private void loadEmployeeDayWise()
        {
            try
            {

                string EmpID = "";
                string EmpID1 = "";
                string EmpType = "";
                string DptID = "";
                string EmpStatus = "";
                if (txtCardNoDay.Text.Trim().Length > 0)
                {
                    EmpID = commonTask.getEmpIDByEmpCardNo(txtCardNoDay.Text.Trim());
                    if (EmpID == "")
                    {
                        lblMessage.InnerText = "warning-> Invalid card no!";
                        txtCardNoDay.Focus();
                        return;
                    }
                    EmpID1 = " and EmpId='" + EmpID + "'";
                    EmpID = " and e.EmpId='" + EmpID + "'";

                }
                else
                {
                    EmpType = rblEmpType.SelectedValue == "All" ? "" : " and e.EmpTypeId=" + rblEmpType.SelectedValue;
                    DptID = " and e.DptId " + classes.commonTask.getDepartmentList(lstSelected);
                    EmpStatus = " and e.EmpStatus in(1,8) ";
                }

                ViewState["__FromDate__"]  = commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim());
                ViewState["__ToDate__"] = commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim());
                CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
                
                DateTime ToDate = DateTime.Parse(ViewState["__ToDate__"].ToString());
                List<DateTime> WDates = new List<DateTime>();
                string WDatesString = "";
                for(DateTime FromDate = DateTime.Parse(ViewState["__FromDate__"].ToString());FromDate<= ToDate;FromDate= FromDate.AddDays(1))
                {
                    string Day = FromDate.ToString("dddd");
                    if (Day == ddlWeekend.SelectedItem.Text)
                    {
                        WDates.Add(FromDate);
                        WDatesString +=","+FromDate.ToString("dd-MM-yyyy") + "("+Day+")";
                    }
                    

                }
                if (WDatesString.Length > 0)
                    WDatesString = WDatesString.Remove(0, 1);
                ViewState["__WDates__"] = WDates;

                sqlCmd = @"with o as (
select EmpID ,STRING_AGG(convert(varchar(10),Date,105)+' ('+format(Date,'dddd')+')' ,',') as OverWriteOn from tblEmpWiseWeekendinfo where CompanyID='"+ CompanyId + "' and Date>='" + ViewState["__FromDate__"].ToString() + @"' and Date<='"+ ViewState["__ToDate__"].ToString()+ @"' "+EmpID1+@" Group By EmpID)
select e.EmpId,substring(e.EmpCardNo,8,6)+' ('+e.EmpProximityNo+')' as EmpCardNo, e.EmpName,e.DsgName,e.DptName,e.CompanyId,e.DptId,e.GId,e.DsgId, CustomOrdering,e.EmpType,ISNULL(convert(varchar(10),max(w.Date),105)+' ('+format(max(w.Date),'dddd')+')','') as PreWeekends,o.OverWriteOn,'" + WDatesString + @"' as Weekends 
from v_EmployeeDetails e left join tblEmpWiseWeekendinfo w on e.EmpId=w.EmpID left join o on e.EmpId=o.EmpID  where e.CompanyId='" + CompanyId + "' " + EmpStatus + EmpType+DptID+EmpID + @"and IsActive=1 and e.WeekendType='Roster'  
group by e.EmpId,substring(e.EmpCardNo,8,6)+' ('+e.EmpProximityNo+')', e.EmpName,e.DsgName,e.DptName,e.CompanyId,e.DptId,e.GId,e.DsgId, CustomOrdering,e.EmpType ,o.OverWriteOn
order by e.DptId, CustomOrdering";

                DataTable dt = new DataTable();
                sqlDB.fillDataTable(sqlCmd, dt);
                gvEmployeeList.DataSource = dt;
                gvEmployeeList.DataBind();
            }
            catch (Exception ex) { }
        }
        private void loadEmployeeDateWise()
        {
            try
            {
                string EmpID = "";
                string EmpID1 = "";
                string EmpType = "";
                string DptID = "";
                string EmpStatus = "";
                if (txtCardNoDate.Text.Trim().Length > 0)
                {
                    EmpID = commonTask.getEmpIDByEmpCardNo(txtCardNoDate.Text.Trim());
                    if (EmpID == "")
                    {
                        lblMessage.InnerText = "warning-> Invalid card no!";
                        txtCardNoDate.Focus();
                        return;
                    }
                    EmpID1 = " and EmpId='"+EmpID+"'";
                    EmpID= " and e.EmpId='" + EmpID + "'";

                }
                else
                {
                    EmpType = rblEmpType.SelectedValue == "All" ? "" : " and e.EmpTypeId=" + rblEmpType.SelectedValue;
                    DptID = " and e.DptId " + classes.commonTask.getDepartmentList(lstSelected);
                    EmpStatus = " and e.EmpStatus in(1,8) ";
                }
                ViewState["__WeekendDate__"] = commonTask.ddMMyyyyTo_yyyyMMdd(txtWeekendDate.Text.Trim());
                
                CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;

                DateTime WeekendDate = DateTime.Parse(ViewState["__WeekendDate__"].ToString());
                string WDatesString = WeekendDate.ToString("dd-MM-yyyy") + "(" + WeekendDate.ToString("dddd")+ ")";

                sqlCmd = @"with o as (
select EmpID ,STRING_AGG(convert(varchar(10),Date,105)+' ('+format(Date,'dddd')+')' ,',') as OverWriteOn from tblEmpWiseWeekendinfo where CompanyID='" + CompanyId + "' and Date='" + ViewState["__WeekendDate__"].ToString() +@"' "+ EmpID1 + @" Group By EmpID)
select e.EmpId,substring(e.EmpCardNo,8,6)+' ('+e.EmpProximityNo+')' as EmpCardNo, e.EmpName,e.DsgName,e.DptName,e.CompanyId,e.DptId,e.GId,e.DsgId, CustomOrdering,e.EmpType,ISNULL(convert(varchar(10),max(w.Date),105)+' ('+format(max(w.Date),'dddd')+')','') as PreWeekends,o.OverWriteOn,'" + WDatesString + @"' as Weekends 
from v_EmployeeDetails e left join tblEmpWiseWeekendinfo w on e.EmpId=w.EmpID  left join o on e.EmpId=o.EmpID  where e.CompanyId='" + CompanyId + "' " + EmpStatus + EmpType +DptID+EmpID+ @" and IsActive=1 and e.WeekendType='Roster'  
group by e.EmpId,substring(e.EmpCardNo,8,6)+' ('+e.EmpProximityNo+')', e.EmpName,e.DsgName,e.DptName,e.CompanyId,e.DptId,e.GId,e.DsgId, CustomOrdering,e.EmpType ,o.OverWriteOn
order by e.DptId, CustomOrdering";

                DataTable dt = new DataTable();
                sqlDB.fillDataTable(sqlCmd, dt);
                gvEmployeeList.DataSource = dt;
                gvEmployeeList.DataBind();
            }
            catch (Exception ex) { }
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

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstSelected.Items.Clear();
            gvEmployeeList = null;
            gvEmployeeList.DataBind();
            classes.commonTask.LoadDepartment(ddlCompany.SelectedValue, lstAll);
          
        }

      

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (rblSetupType.SelectedValue == "day")
                    saveWeekendDayWise();
                else
                    saveWeekendDateWise();
            }
            catch (Exception ex)
            {
            }
        }
        private void saveWeekendDayWise()
        {
            if (gvEmployeeList != null && gvEmployeeList.Rows.Count > 0)
            {
                List<DateTime> WDates = (List<DateTime>)ViewState["__WDates__"];
                foreach (GridViewRow row in gvEmployeeList.Rows)
                {
                    CheckBox ckbEmp = (CheckBox)row.FindControl("ckbEmp");
                    
                    if (ckbEmp.Checked)
                    {
                        
                        string EmpID = gvEmployeeList.DataKeys[row.RowIndex].Values[0].ToString();
                        string CompanyID = gvEmployeeList.DataKeys[row.RowIndex].Values[1].ToString();
                        string DptID = gvEmployeeList.DataKeys[row.RowIndex].Values[2].ToString();
                        string GID = gvEmployeeList.DataKeys[row.RowIndex].Values[3].ToString();
                        string DsgID = gvEmployeeList.DataKeys[row.RowIndex].Values[4].ToString();
                        delete(EmpID,ViewState["__FromDate__"].ToString(), ViewState["__ToDate__"].ToString());                        
                        foreach (DateTime WDate in WDates)
                        {
                            save(CompanyID, DptID, GID, DsgID, EmpID, WDate.ToString("yyyy-MM-dd"));
                        }
                    }
                }
                lblMessage.InnerText = "success-> Successfully Submited.";
                loadEmployeeDayWise();
            }
            else
            {
                lblMessage.InnerText = "warning-> Please, Add Employee";
            }
        }
        private void saveWeekendDateWise()
        {
            if (gvEmployeeList != null && gvEmployeeList.Rows.Count > 0)
            {
                
                foreach (GridViewRow row in gvEmployeeList.Rows)
                {
                    CheckBox ckbEmp = (CheckBox)row.FindControl("ckbEmp");

                    if (ckbEmp.Checked)
                    {
                        string EmpID = gvEmployeeList.DataKeys[row.RowIndex].Values[0].ToString();
                        string CompanyID = gvEmployeeList.DataKeys[row.RowIndex].Values[1].ToString();
                        string DptID = gvEmployeeList.DataKeys[row.RowIndex].Values[2].ToString();
                        string GID = gvEmployeeList.DataKeys[row.RowIndex].Values[3].ToString();
                        string DsgID = gvEmployeeList.DataKeys[row.RowIndex].Values[4].ToString();
                        delete(EmpID, ViewState["__WeekendDate__"].ToString());                        
                        save(CompanyID, DptID, GID, DsgID, EmpID, ViewState["__WeekendDate__"].ToString());                        
                    }
                }
                lblMessage.InnerText = "success-> Successfully Submited.";
                loadEmployeeDateWise();
            }
            else
            {
                lblMessage.InnerText = "warning-> Please, Add Employee";
            }
        }
        private void clearData() {
            ViewState["__WeekendDate__"] = "";
            ViewState["__FromDate__"] = "";
            ViewState["__ToDate__"] = "";
            gvEmployeeList.DataSource = null;
            gvEmployeeList.DataBind();

        }
        private void save(string CompanyID,string DptID,string GID,string DsgID,string EmpID ,string Date) {
            try {
                sqlCmd = @"INSERT INTO [dbo].[tblEmpWiseWeekendinfo]
           ([CompanyID]
           ,[DptID]
           ,[GID]
           ,[DsgID]
           ,[EmpID]
           ,[Date]
           ,[CreatedAt])
            VALUES
           ('" + CompanyID + "','" + DptID + "'," + GID + ",'" + DsgID + "','" + EmpID + "','" + Date + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                CRUD.Execute(sqlCmd, sqlDB.connection);
            }
            catch (Exception ex) { }
            
        }
       
        private void delete(string EmpID,string FromDate, string ToDate)
        {
            sqlCmd = "Delete tblEmpWiseWeekendinfo where EmpID='" + EmpID + "' and Date>='"+ FromDate + "' and Date<='"+ ToDate + "' ";
            CRUD.Execute(sqlCmd, sqlDB.connection);
        }
        private void delete(string EmpID, string WeekendDate)
        {
            sqlCmd = "Delete tblEmpWiseWeekendinfo where EmpID='" + EmpID + "' and Date='" + WeekendDate + "'";
            CRUD.Execute(sqlCmd, sqlDB.connection);
        }
        protected void gvEmployeeList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("remove"))
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    string EmpID = gvEmployeeList.DataKeys[rIndex].Values[0].ToString();
                    if (rblSetupType.SelectedValue == "day")
                    {
                        delete(EmpID, ViewState["__FromDate__"].ToString(), ViewState["__ToDate__"].ToString());
                        loadEmployeeDayWise();
                    }
                    else
                    {
                        delete(EmpID, ViewState["__WeekendDate__"].ToString());
                        loadEmployeeDateWise();
                    }                    
                    lblMessage.InnerText = "success-> Successfully Deleted.";
                }
              
            }
            catch (Exception ex) { }
        }

        protected void ckbEmpAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckbAll = (CheckBox)gvEmployeeList.HeaderRow.FindControl("ckbEmpAll");
            foreach (GridViewRow row in gvEmployeeList.Rows)
            {
                CheckBox ckb = (CheckBox)row.FindControl("ckbEmp");
                ckb.Checked = ckbAll.Checked;
            }
        }

        protected void gvEmployeeList_RowDataBound(object sender, GridViewRowEventArgs e)
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
            try
            {
                CheckBox ckbEmp = (CheckBox)e.Row.FindControl("ckbEmp");
                if (gvEmployeeList.DataKeys[e.Row.RowIndex].Values[1].ToString().Equals(""))
                    ckbEmp.Checked = true;
                else
                    ckbEmp.Checked = false;
                if (ckbEmp.Checked == false)
                    ((CheckBox)gvEmployeeList.HeaderRow.FindControl("ckbEmpAll")).Checked = false;
            }
            catch (Exception ex) { }
        }

        protected void rblEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        protected void rblSetupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupTypeWiseShowHide();
            clearData();
        }
        private void SetupTypeWiseShowHide()
        {
            if (rblSetupType.SelectedValue == "day")
            {
                trDateWise.Visible = false;
                trDayWise.Visible = true;
            }
            else
            {
                trDateWise.Visible = true;
                trDayWise.Visible = false;
            }            
        }

        protected void btnSearchDay_Click(object sender, EventArgs e)
        {
            if (txtCardNoDay.Text.Trim().Length==0 && lstSelected.Items.Count == 0)
            {
                lblMessage.InnerText = "warning-> Please, select department";
                lstSelected.Focus();
                return;
            }
            if (txtFromDate.Text.Trim().Length != 10)
            {
                lblMessage.InnerText = "warning-> Please, enter a valid date";
                txtFromDate.Focus();
                return;
            }
            if (txtToDate.Text.Trim().Length != 10)
            {
                lblMessage.InnerText = "warning-> Please, enter a valid date";
                txtToDate.Focus();
                return;
            }
            loadEmployeeDayWise();
        }

        protected void btnSearchDate_Click(object sender, EventArgs e)
        {
            if (txtCardNoDate.Text.Trim().Length == 0 && lstSelected.Items.Count == 0)
            {
                lblMessage.InnerText = "warning-> Please, select department";
                lstSelected.Focus();
                return;
            }
            if (txtWeekendDate.Text.Trim().Length != 10)
            {
                lblMessage.InnerText = "warning-> Please, enter a valid date";
                txtWeekendDate.Focus();
                return;
            }           
            loadEmployeeDateWise();
        }
    }
}