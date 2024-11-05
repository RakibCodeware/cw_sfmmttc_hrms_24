using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.attendance
{
    public partial class AttendanceProcessing : System.Web.UI.Page
    {
        string sqlCmd = "";
        //Dataacees Level= OnlyMe=1 All=3 Own=2 custom=4
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 260 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
           
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

               
                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                ViewState["__OT__"] = "0";
                setPrivilege();
            }
            if (!classes.commonTask.HasBranch())
                ddlCompanyList.Enabled = false;
           
         
        }

        private void setPrivilege()
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__getUserId__"] = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CShortName__"] = getCookies["__CShortName__"].ToString();
                ViewState["__dptID__"]= getCookies["__DptId__"].ToString();


                //  string[] AccessPermission = new string[0];
                //System.Web.UI.HtmlControls.HtmlTable a = tblGenerateType;
                classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
               // AccessPermission = checkUserPrivilege.checkUserPrivilegeForOnlyWriteAction(ViewState["__CompanyId__"].ToString(), ViewState["__getUserId__"].ToString(), ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "import_data.aspx", ddlCompanyList, btnImport);

                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                if (Session["__dataAceesLevel__"].ToString() == "4")
                {
                    classes.commonTask.loadDepartmentListByCompany_ForShrink(ddlDepartmentList, ddlCompanyList.SelectedValue);
                }
                else if(Session["__dataAceesLevel__"].ToString() == "3")
                {
                    classes.commonTask.loadDepartmentListByCompany_ForShrink(ddlDepartmentList, ddlCompanyList.SelectedValue);
                }
                 else if(Session["__dataAceesLevel__"].ToString() == "2")
                {
                    classes.commonTask.loadDepartmentListByCompany_ForShrink(ddlDepartmentList, ddlCompanyList.SelectedValue);
                }

                ViewState["__AttMachineName__"] = classes.commonTask.loadAttMachineName(ddlCompanyList.SelectedValue);
                if (ViewState["__AttMachineName__"].ToString().Equals("RMS"))
                {                    
                    trImportFrom.Visible = true;
                }
                else
                {
                    trImportFrom.Visible = false;                    
                }
               

                tdFileUpload.Visible = false;
                tdSelectFile.Visible = false;
            }
            catch { }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (validationBasket())
                {
                    DataTable DtEmpAttList = null;
                    DateTime AttendanceDate = (rblImportType.SelectedItem.Value.Equals("FullImport")) ? DateTime.ParseExact(txtFullAttDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture) : DateTime.ParseExact(txtPartialAttDate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    bool forAllEmployee = (rblImportType.SelectedItem.Value.Equals("FullImport")) ? true : false;

                    
                    
                    Label lblErrorMessage1 = new Label();
                    classes.AttendanceProcessing attendanceProcessing = new classes.AttendanceProcessing();

                    if (ViewState["__CShortName__"].ToString().Equals("MRC"))// Marico
                    {
                        /*attendanceProcessing._AttendanceProcessingWithoutShift(ddlCompanyList.SelectedValue, AttendanceDate, forAllEmployee, ddlDepartmentList.SelectedValue, txtCardNo.Text.Trim(), ViewState["__getUserId__"].ToString(), rblEmpType.SelectedValue); */

                        attendanceProcessing._AttendanceProcessingWithCommonShift(ddlCompanyList.SelectedValue, AttendanceDate, forAllEmployee, ddlDepartmentList.SelectedValue, txtCardNo.Text.Trim(), ViewState["__getUserId__"].ToString(), rblEmpType.SelectedValue);
                    }
                    else
                    {
                        Random rnd = new Random();
                        string ProcessingID = DateTime.Now.ToFileTime().ToString() + "_" + rnd.Next().ToString();
                        lblErrorMessage.Text = ProcessingID;
                        attendanceProcessing._AttendanceProcessing(ProcessingID, ViewState["__AttMachineName__"].ToString(), ddlCompanyList.SelectedValue, AttendanceDate, FileUpload1, forAllEmployee, ddlDepartmentList.SelectedValue, txtCardNo.Text.Trim(), ViewState["__getUserId__"].ToString(), rblEmpType.SelectedValue,rblImportFrom.SelectedValue, lblErrorMessage1);
                        generateAbsentNotification(AttendanceDate);
                    }
                    
                    DtEmpAttList =attendanceProcessing.LoadProcessedAttendanceData(ddlCompanyList.SelectedValue, ddlDepartmentList.SelectedValue, AttendanceDate.ToString("yyyy-MM-dd"), forAllEmployee, txtCardNo.Text.Trim(),rblEmpType.SelectedValue, Session["__dataAceesLevel__"].ToString());
                    gvAttendance.DataSource = DtEmpAttList;
                    gvAttendance.DataBind();
                    ulAttMissingLog.Visible = true;
                }
            }
            catch { }
        }
        private void deleteAbsentNotification(DateTime selectdDate, string condition)
        {
            sqlCmd = "delete AttAbsentNotification_Log where Date='" + selectdDate.ToString("yyyy-MM-dd") + "' " + condition;
            CRUD.Execute(sqlCmd, sqlDB.connection);
        }
        private void generateAbsentNotification(DateTime selectdDate)
        {
            try
            {


                DataTable dtEmpForNotification = new DataTable();
                DataTable dtAdminList = new DataTable();
                DataTable dtSettings = new DataTable();
                sqlCmd = "select Days,StatusCount,NotificationStatus from  AttAbsentNotificationSetting where NotificationStatus=1";
                sqlDB.fillDataTable(sqlCmd, dtSettings);
                if (dtSettings == null || dtSettings.Rows.Count == 0)
                    return;
                string condition = "";
                string conditionDel = "";
                if (rblImportType.SelectedItem.Value.Equals("FullImport"))
                {
                    if (ddlDepartmentList.SelectedValue == "0")
                    {
                        condition = "";
                        conditionDel = " and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and EmpStatus in(1,8)  and CompanyId='" + ddlCompanyList.SelectedValue + "')";
                    }
                    else
                    {
                        condition = " and DptID='" + ddlDepartmentList.SelectedValue + "'";
                        conditionDel = " and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and EmpStatus in(1,8) and DptID='" + ddlDepartmentList.SelectedValue + "' and CompanyId='" + ddlCompanyList.SelectedValue + "')";
                    }

                }
                else
                {
                    bool hasEmpCard = AccessControl.hasEmpcardPermission(txtCardNo.Text.Trim(), ddlCompanyList.SelectedValue);
                    if (!hasEmpCard)
                    {
                        return;
                    }
                    conditionDel = condition = " and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and EmpStatus in(1,8) and EmpCardNo like'%" + txtCardNo.Text.Trim() + "' and CompanyId='" + ddlCompanyList.SelectedValue + "')";
                }
                deleteAbsentNotification(selectdDate, conditionDel);
                int days = int.Parse(dtSettings.Rows[0]["Days"].ToString());
                string status = "";

                string[] statusCount = dtSettings.Rows[0]["StatusCount"].ToString().Split(',');
                if (statusCount.Length == 1)
                    status = "'" + statusCount[0] + "'";
                else
                {
                    foreach (string item in statusCount)
                    {
                        status += ",'" + item + "'";
                    }
                    status = status.Remove(0, 1);
                }

                DateTime fromDate = selectdDate.AddDays(-days);
                sqlCmd = "select EmpId, count(EmpId) as AbsentDays from tblAttendanceRecord where ATTDate >'" + fromDate.ToString("yyyy-MM-dd") + "' and ATTDate <='" + selectdDate.ToString("yyyy-MM-dd") + "' and ATTStatus in(" + status + ") and  EmpId in(select EmpId from tblAttendanceRecord where  ATTDate='" + selectdDate.ToString("yyyy-MM-dd") + "' and ATTStatus in(" + status + ") " + condition + ") group by EmpId having count(EmpId)=" + days + "";
                sqlDB.fillDataTable(sqlCmd, dtEmpForNotification);
                if (dtEmpForNotification != null && dtEmpForNotification.Rows.Count > 0)
                {
                    sqlCmd = "select AdminID from AttAbsentNotificationAdminList where status=1";
                    sqlDB.fillDataTable(sqlCmd, dtAdminList);
                    for (int i = 0; i < dtEmpForNotification.Rows.Count; i++)
                    {
                        string LastWorkingDay = commonTask.getLastWorkingDay(dtEmpForNotification.Rows[i]["EmpId"].ToString(), selectdDate.ToString("yyyy-MM-dd"));
                        for (int j = 0; j < dtAdminList.Rows.Count; j++)
                        {
                            sqlCmd = "INSERT INTO [dbo].[AttAbsentNotification_Log]([EmpID] ,[AdminID] ,[Date],[LastWorkingDay],[seen]) VALUES " +
                                     "('" + dtEmpForNotification.Rows[i]["EmpId"].ToString() + "','" + dtAdminList.Rows[j]["AdminID"].ToString() + "','" + selectdDate.ToString("yyyy-MM-dd") + "','"+ LastWorkingDay + "','0')";
                            CRUD.Execute(sqlCmd, sqlDB.connection);
                        }
                    }

                }
            }
            catch { }

        }

        private bool validationBasket()
        {
            try
            {

                //if (!FileUpload1.HasFile && FileUpload1.Visible)
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "alertMessage();", true);
                //    lblErrorMessage.Text = "Please select access database file";
                //    FileUpload1.Focus();
                //    return false;
                //}
                //if (!FileUpload1.HasFile && !File.Exists(HttpContext.Current.Server.MapPath("~/AccessFile/" + ddlCompanyList.SelectedValue + "UNIS.mdb")))
                //{
                //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "alertMessage();", true);
                //    lblErrorMessage.Text = "Please select  access database file (UNIS)";
                //    FileUpload1.Focus();
                //    return false;
                //}
                if (rblImportType.SelectedValue == "FullImport" && rblDateType.SelectedValue == "SingleDate" && txtFullAttDate.Text.Trim().Length < 10)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "alertMessage();", true);
                    lblErrorMessage.Text = "Please select attendance date";
                    txtFullAttDate.Focus();
                    return false;
                }

                if (rblImportType.SelectedValue != "FullImport" && txtCardNo.Text.Trim().Length < 4)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "alertMessage();", true);
                    lblErrorMessage.Text = "Please type valid card no";
                    txtCardNo.Focus();
                    return false;
                }
                if (rblImportType.SelectedValue != "FullImport" && txtPartialAttDate.Text.Trim().Length < 10)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "alertMessage();", true);
                    lblErrorMessage.Text = "Please select partial attendance date";
                    txtPartialAttDate.Focus();
                    return false;
                }

                if (txtFullToDate.Visible == true && txtFullToDate.Text.Trim().Length < 10)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "alertMessage();", true);
                    lblErrorMessage.Text = "Please select To date";
                    txtFullToDate.Focus();
                    return false;
                }
                else if (txtPartialToDate.Visible == true && txtPartialToDate.Text.Trim().Length < 10)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "alertMessage();", true);
                    lblErrorMessage.Text = "Please select To date";
                    txtPartialToDate.Focus();
                    return false;
                }


                return true;
            }
            catch { return false; }
        }

        protected void gvAttendance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvAttendance.PageIndex = e.NewPageIndex;
                gvAttendance.DataBind();
            }
            catch { }
        }

        protected void gvAttendance_RowDataBound(object sender, GridViewRowEventArgs e)
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
        }

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["__AttMachineName__"] = classes.commonTask.loadAttMachineName(ddlCompanyList.SelectedValue);
            classes.commonTask.loadDepartmentListByCompany_ForShrink(ddlDepartmentList, ddlCompanyList.SelectedValue);
            if (ViewState["__AttMachineName__"].ToString().Equals("RMS"))
            {
                tdFileUpload.Visible = true;
                tdSelectFile.Visible = true;
            }
            else
            {
                tdFileUpload.Visible = false;
                tdSelectFile.Visible = false;
            }
        }

        protected void rblImportFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblImportFrom.SelectedValue == "sql")
            {
                tdFileUpload.Visible = false;
                tdSelectFile.Visible = false;
            }
            else
            {
                tdFileUpload.Visible = true;
                tdSelectFile.Visible = true;
            }
                
        }
    }
}