using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;

namespace SigmaERP.attendance
{
    public partial class WeekendInfoReport : System.Web.UI.Page
    {
        //permission=326;
        DataTable dt;
        DataTable dtSetPrivilege;
        string CompanyId = "";
        string query = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 326 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                txtToDate.Text= txtFromDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompany.Enabled = false;
                ddlCompany.SelectedValue = ViewState["__CompanyId__"].ToString();
                Session["__MinDigits__"] = "6";

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
                classes.commonTask.LoadBranch(ddlCompany, ViewState["__CompanyId__"].ToString());
                //------------load privilege setting inof from db------
                //------------load privilege setting inof from db------
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "holyday.aspx", ddlCompany, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];                
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);                //-----------------------------------------------------





            }
            catch { }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (rblReportType.SelectedValue == "empwise")
                GenerateReportEmpWise();
            else
                GenerateReportDateWise();


        }

         private void GenerateReportEmpWise()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }

            string _EmpType = rblEmpType.SelectedItem.Text;
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();         

            if (txtCardNo.Text.Trim().Length == 0)
            {
                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and ei.EmpTypeId=" + rblEmpType.SelectedValue + " ";
                string  DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                query = @" select wi.EmpID,substring(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' as EmpCardNo,ei.EmpName,ei.EmpTypeId,et.EmpType,wi.DptID,dpt.DptName,wi.DsgID,dsg.DsgName,wi.GID,grp.GName,ci.CompanyId,ci.CompanyName,ci.Address,ei.CustomOrdering,convert(int,ei.DptCode),STRING_AGG(convert(varchar(10),wi.Date,105)+' ('+format(wi.Date,'dddd')+')' ,',') WITHIN GROUP (ORDER BY convert(varchar(10), wi.Date,120) ASC) as Weekends  from tblEmpWiseWeekendinfo wi inner join  v_EmployeeDetails ei on wi.EmpID=ei.EmpId and ei.IsActive=1 left join HRD_Department dpt on wi.DptID=dpt.DptId left join HRD_Designation dsg on wi.DsgID=dsg.DsgId inner join HRD_EmployeeType et on ei.EmpTypeId=et.EmpTypeId left join HRD_Group grp on wi.GID=grp.GId
                 inner join HRD_CompanyInfo ci on wi.CompanyID=ci.CompanyId
                 where ei.WeekendType='Roster' and wi.CompanyID = '" + CompanyId + @"' and wi.Date>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "' and wi.Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim())+"' and wi.DptID "+ DepartmentList + " "+EmpTypeID+ @" group by wi.EmpID,substring(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')',ei.EmpName,ei.EmpTypeId,et.EmpType,wi.DptID,dpt.DptName,wi.DsgID,dsg.DsgName,wi.GID,grp.GName,ci.CompanyId,ci.CompanyName,ci.Address,ei.CustomOrdering,convert(int,ei.DptCode) order by  convert(int,ei.DptCode),wi.GId,CustomOrdering";
            }               

            else
            {
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                query = @" select wi.EmpID,substring(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' as EmpCardNo,ei.EmpName,ei.EmpTypeId,et.EmpType,wi.DptID,dpt.DptName,wi.DsgID,dsg.DsgName,wi.GID,grp.GName,ci.CompanyId,ci.CompanyName,ci.Address,STRING_AGG(convert(varchar(10),wi.Date,105)+' ('+format(wi.Date,'dddd')+')' ,',') WITHIN GROUP (ORDER BY convert(varchar(10), wi.Date,120) ASC) as Weekends  from tblEmpWiseWeekendinfo wi inner join  v_EmployeeDetails ei on wi.EmpID=ei.EmpId and ei.IsActive=1 left join HRD_Department dpt on wi.DptID=dpt.DptId left join HRD_Designation dsg on wi.DsgID=dsg.DsgId inner join HRD_EmployeeType et on ei.EmpTypeId=et.EmpTypeId left join HRD_Group grp on wi.GID=grp.GId
                 inner join HRD_CompanyInfo ci on wi.CompanyID=ci.CompanyId
                 where ei.WeekendType='Roster' and wi.CompanyID = '" + CompanyId + @"' and wi.Date>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "' and wi.Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and ei.EmpCardNo like'%"+txtCardNo.Text.Trim()+"' group by wi.EmpID,substring(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')',ei.EmpName,ei.EmpTypeId,et.EmpType,wi.DptID,dpt.DptName,wi.DsgID,dsg.DsgName,wi.GID,grp.GName,ci.CompanyId,ci.CompanyName,ci.Address";
                _EmpType = "Individual";

            }
            sqlDB.fillDataTable(query, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__WeekendInfoEmpWise__"] = dt;
            string daterange = txtFromDate.Text.Trim() + " to " + txtToDate.Text.Trim();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=WeekendInfoEmpWise-" + daterange.Replace('-','/') +"-"+ _EmpType + "');", true);  //Open New Tab for Sever side code

        }
        private void GenerateReportDateWise()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }

            string _EmpType = rblEmpType.SelectedItem.Text;
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();

            if (txtCardNo.Text.Trim().Length == 0)
            {
                string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and ei.EmpTypeId=" + rblEmpType.SelectedValue + " ";
                string DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                query = @" select wi.EmpID,substring(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' as EmpCardNo,ei.EmpName,ei.EmpTypeId,et.EmpType,wi.DptID,dpt.DptName,wi.DsgID,dsg.DsgName,wi.GID,grp.GName,ci.CompanyId,ci.CompanyName,ci.Address,convert(varchar(10),wi.Date,105)+' ('+format(wi.Date,'dddd')+')'  as Weekends  from tblEmpWiseWeekendinfo wi inner join  v_EmployeeDetails ei on wi.EmpID=ei.EmpId and ei.IsActive=1 left join HRD_Department dpt on wi.DptID=dpt.DptId left join HRD_Designation dsg on wi.DsgID=dsg.DsgId inner join HRD_EmployeeType et on ei.EmpTypeId=et.EmpTypeId left join HRD_Group grp on wi.GID=grp.GId
                 inner join HRD_CompanyInfo ci on wi.CompanyID=ci.CompanyId
                 where ei.WeekendType='Roster' and wi.CompanyID = '" + CompanyId + @"' and wi.Date>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "' and wi.Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and wi.DptID " + DepartmentList + " " + EmpTypeID + @"  order by convert(int,ei.DptCode),convert(int,wi.GId),wi.Date,CustomOrdering ";
            }

            else
            {
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                query = @" select wi.EmpID,substring(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' as EmpCardNo,ei.EmpName,ei.EmpTypeId,et.EmpType,wi.DptID,dpt.DptName,wi.DsgID,dsg.DsgName,wi.GID,grp.GName,ci.CompanyId,ci.CompanyName,ci.Address,convert(varchar(10),wi.Date,105)+' ('+format(wi.Date,'dddd')+')'  as Weekends  from tblEmpWiseWeekendinfo wi inner join  v_EmployeeDetails ei on wi.EmpID=ei.EmpId and ei.IsActive=1 left join HRD_Department dpt on wi.DptID=dpt.DptId left join HRD_Designation dsg on wi.DsgID=dsg.DsgId inner join HRD_EmployeeType et on ei.EmpTypeId=et.EmpTypeId left join HRD_Group grp on wi.GID=grp.GId
                 inner join HRD_CompanyInfo ci on wi.CompanyID=ci.CompanyId
                 where ei.WeekendType='Roster' and wi.CompanyID = '" + CompanyId + @"' and wi.Date>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "' and wi.Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and ei.EmpCardNo like'%" + txtCardNo.Text.Trim() + "' ";
                _EmpType = "Individual";

            }
            sqlDB.fillDataTable(query, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }

            Session["__WeekendInfoDateWise__"] = dt;
            string daterange = txtFromDate.Text.Trim() + " to " + txtToDate.Text.Trim();
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=WeekendInfoDateWise-" + daterange.Replace('-', '/') + "-" + _EmpType + "');", true);  //Open New Tab for Sever side code

        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }

     

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;          
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
            lstSelected.Items.Clear();           

        }





    }
}