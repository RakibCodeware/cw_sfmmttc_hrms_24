using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.attendance
{
    public partial class out_duty_approval : System.Web.UI.Page
    {

        string sqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";

            if (!IsPostBack)
            {
                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                setPrivilege();
                searchLeaveApplicationForApproved();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
        }
   
        private void setPrivilege()
        {
            try
            {


                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
               
                ViewState["__UserId__"] = getUserId;
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();       

          

                classes.commonTask.LoadBranch(ddlCompanyList);
                classes.commonTask.LoadShift(ddlShiftName, ViewState["__CompanyId__"].ToString());
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ViewState["__CompanyId__"].ToString());
                
               

            }
            catch { }
        }

        DataTable dtForApprovedList;
        private void searchLeaveApplicationForApproved()
        {
            try
            {
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000") || ddlCompanyList.SelectedValue.ToString().Equals("")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                string condition = AccessControl.getDataAccessCondition(CompanyId,"0");
                dtForApprovedList = new DataTable();
                string EmpType = rblEmpType.SelectedValue == "All" ? "" : " and od.EmpTypeId=" + rblEmpType.SelectedValue;
                sqlCmd = "select substring(od.EmpCardNo,8,6) as EmpCardNo ,od.SL,od.EmpName,od.EmpId,od.DptName,od.DsgName, Format(od.Date,'dd-MM-yyyy') as Date ,od.Type,case when od.Type=0 then 'Out Duty' else 'Training' end as TypeName,Remark,ISNULL(StraightFromHome,0) as StraightFromHome,ISNULL(StraightToHome,0) as StraightToHome from v_tblOutDuty as od inner join tblOutDutyAuthorityAccessControl ac on od.Processing=ac.AuthorityPosition and od.EmpId=ac.EmpID and od.IsActive=1 and ac.AuthorityID= '" + ViewState["__UserId__"].ToString() + "'  and od.Status=0  where od." + condition + "" + EmpType;
                sqlDB.fillDataTable(sqlCmd, dtForApprovedList = new DataTable());  
                if (dtForApprovedList.Rows == null || dtForApprovedList.Rows.Count == 0)
                {
                    gvForApprovedList.DataSource = null;
                    gvForApprovedList.DataBind();
                    divRecordMessage.InnerText = "No data available!";
                    divRecordMessage.Visible = true;
                    return;
                }
                gvForApprovedList.DataSource = dtForApprovedList;
                gvForApprovedList.DataBind();
                divRecordMessage.InnerText = "";
                divRecordMessage.Visible = false;
            }
            catch (Exception ex)
            {
                // lblMessage.InnerText = "error->" + ex.Message;
            }
        }
        protected void gvForApprovedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {


                if (e.CommandName.Equals("Show"))
                {
                    int rIndex = int.Parse(e.CommandArgument.ToString());
                    showDetails(gvForApprovedList.DataKeys[rIndex].Values[0].ToString());
                }
                    
                else if (e.CommandName.Equals("Forword"))
                {
                    int rIndex = int.Parse(e.CommandArgument.ToString());
                    ViewState["__Predecate__"] = "Forword";
                    Forward(rIndex);

                }

                else if (e.CommandName.Equals("Yes"))
                {
                    int rIndex = int.Parse(e.CommandArgument.ToString());
                    ViewState["__Predecate__"] = "Yes";
                    Label lblEmpCardNo = gvForApprovedList.Rows[rIndex].FindControl("lblEmpCardNo") as Label;
                    Label lblDate = gvForApprovedList.Rows[rIndex].FindControl("lblDate") as Label;
                    Label lblRemark = gvForApprovedList.Rows[rIndex].FindControl("lblRemark") as Label;

                    // if (SaveAttendanceRecord(lblDate.Text,lblEmpCardNo.Text, gvForApprovedList.DataKeys[rIndex].Values[2].ToString(),lblRemark.Text, ViewState["__UserId__"].ToString(), gvForApprovedList.DataKeys[rIndex].Values[0].ToString()))
                    YesApproved(rIndex);



                }
                else if (e.CommandName.Equals("No"))
                {
                    int rIndex = int.Parse(e.CommandArgument.ToString());
                    ViewState["__Predecate__"] = "No";
                    NoApproved(rIndex);
                }
                else if (e.CommandName.Equals("View"))
                {
                    int rIndex = int.Parse(e.CommandArgument.ToString());
                    viewOutDutyApplication(gvForApprovedList.DataKeys[rIndex].Values[0].ToString());
                }

            }
            catch { }
        }
        private void viewOutDutyApplication(string ODID)
        {
            try
            {
                Session["__OutDutyApplication__"] = commonTask.getOutDutyApplication(ODID);
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=OutDutyApplication');", true);  //Open New Tab for Sever side code

            }
            catch { }
        }


        private void YesApproved(int rIndex)
        {
            try
            {
                string[] getColumns = { "Status", "AuthorizedBy", "AuthorizedDate" };
                string[] getValues = { "1", ViewState["__UserId__"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                if (SQLOperation.forUpdateValue("tblOutDuty", getColumns, getValues, "SL", gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection) == true)
                {
                    saveAprovalLog(rIndex, "1");
                    lblMessage.InnerText = "success->Successfully Approved.";
                      gvForApprovedList.Rows[rIndex].Visible = false;
                }

            }
            catch { }


        }
        private void saveAprovalLog(int rIndex, string Approval)
        {
            try
            {
                string[] getColumns = { "ODID", "UserID", "DateTime", "Approval" };
                string[] getValues = { gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), ViewState["__UserId__"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Approval };
                SQLOperation.forSaveValue("tblOutDuty_ApprovalLog", getColumns, getValues, sqlDB.connection);
            }
            catch { }


        }

        private void Forward(int rIndex)
        {
            try
            {
                DataTable dt = new DataTable();
               // sqlDB.fillDataTable("Select ODAuthorityOrder FROM UserAccount where UserId ='" + ViewState["__UserId__"].ToString() + "'", dt);
                string AuthorityPosition = getODAuthority(ViewState["__UserId__"].ToString(), gvForApprovedList.DataKeys[rIndex].Values[1].ToString()); ;
                if (AuthorityPosition.Trim() == "") // OD Authority is not found. plz setup for this employee.
                {
                   
                    lblMessage.InnerText = "warning->The higher authority is not found.";
                    return;
                    // 
                }
                string[] getColumns = { "Processing", "AuthorizedBy", "AuthorizedDate" };
                string[] getValues = { AuthorityPosition, ViewState["__UserId__"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                if (SQLOperation.forUpdateValue("tblOutDuty", getColumns, getValues, "SL", gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection) == true)
                {
                    saveAprovalLog(rIndex, "0");
                    lblMessage.InnerText = "success->Successfully Forwarded.";
                    gvForApprovedList.Rows[rIndex].Visible = false;
                }

            }
            catch { }

        }
        private string getODAuthority(string AuthorityID, string EmpID)
        {
            

            try
            {
                DataTable dtLvOrder;
                sqlCmd = "select max(AuthorityPosition) as AuthorityPosition from tblOutDutyAuthorityAccessControl where EmpID="+ EmpID + " and AuthorityPosition<(select AuthorityPosition from tblOutDutyAuthorityAccessControl where EmpID="+ EmpID + " and AuthorityID= "+ AuthorityID + ")";
                sqlDB.fillDataTable(sqlCmd, dtLvOrder = new DataTable());
                if (dtLvOrder == null || dtLvOrder.Rows.Count == 0)
                    return "";
                return dtLvOrder.Rows[0]["AuthorityPosition"].ToString();
            }
            catch(Exception ex) { return ""; }
            }

        private void NoApproved(int rIndex)
        {
            try
            {

                string[] getColumns = { "Status", "AuthorizedBy", "AuthorizedDate" };
                string[] getValues = { "2", ViewState["__UserId__"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                if (SQLOperation.forUpdateValue("tblOutDuty", getColumns, getValues, "SL", gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection) == true)
                {
                    saveAprovalLog(rIndex, "2");
                    lblMessage.InnerText = "success->Successfully Rejected.";
                    gvForApprovedList.Rows[rIndex].Visible = false;
                }

            }
            catch { }

        }
        private bool SaveAttendanceRecord(string Date,string EmpCardNo,string ODType,string Remark,string RefId,string ODID)
        {
            try
            {
                try
                {

                   
                    string[] DayStatus = new string[9];
                    string InHur = "00"; string OutHur = "00";
                    DateTime AttDate = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    // to get needed employee information for count employee attendance 
                    string[] Get_Needed_EmployeeInfo = classes.mManually_Attendance_Count.Get_Needed_EmployeeeInfo(ddlCompanyList.SelectedValue, EmpCardNo);
                    DateTime joindate = DateTime.ParseExact(Get_Needed_EmployeeInfo[7], "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    if (joindate > AttDate)
                    {
                        lblMessage.InnerText = "error->Attendace date must be largest or equal to Joining Date (" + Get_Needed_EmployeeInfo[7] + ") ";
                        return false;
                    }
                    string[] Shift_Roster_InfoList = new string[9];
                    DataTable dtOtherSettings = mZK_Shrink_Data_SqlServer.LoadOTherSettings(ddlCompanyList.SelectedValue);
                    string[] othersetting = new string[8];
                    if (dtOtherSettings.Rows.Count > 0)
                    {
                        othersetting[0] = dtOtherSettings.Rows[0]["WorkerTiffinHour"].ToString();
                        othersetting[1] = dtOtherSettings.Rows[0]["WorkerTiffinMin"].ToString();
                        othersetting[2] = dtOtherSettings.Rows[0]["StaffTiffinHour"].ToString();
                        othersetting[3] = dtOtherSettings.Rows[0]["StaffTiffinMin"].ToString();
                        othersetting[4] = dtOtherSettings.Rows[0]["StaffHolidayCount"].ToString();
                        othersetting[5] = dtOtherSettings.Rows[0]["MinWorkingHour"].ToString() + ":" + dtOtherSettings.Rows[0]["MinWorkingMin"].ToString() + ":00"; //Minimum Working Hours
                        othersetting[6] = dtOtherSettings.Rows[0]["StaffHolidayTotalHour"].ToString() + ":" + dtOtherSettings.Rows[0]["StaffHolidayTotalMin"].ToString() + ":00"; //Minimum Staff Working Hours For Holiday Allowance
                        othersetting[7] = dtOtherSettings.Rows[0]["MinOverTimeHour"].ToString() + ":" + dtOtherSettings.Rows[0]["MinOverTimeMin"].ToString() + ":00"; //Minimum OverTimeHour
                    }
                    string worker = othersetting[0] + ":" + othersetting[1] + ":00";
                    string staff = othersetting[2] + ":" + othersetting[3] + ":00";
                    string tiffin = Get_Needed_EmployeeInfo[6] == "1" ? worker : staff;
                    string holidaycount = "0";

                    if (Get_Needed_EmployeeInfo[4].Equals("Roster"))  // if employee duty type is roster then execute if block
                    {
                        // to get all roster information 
                        Shift_Roster_InfoList = classes.mCommon_Module_For_AttendanceProcessing.LoadShift_Information(Get_Needed_EmployeeInfo[0].ToString(), false, "", ViewState["__AttDates__"].ToString());

                        //-- for leave information and leave count-----------------------------
                        string[] Leave_Info = classes.mZK_Shrink_Data_SqlServer.Check_Any_Leave_Are_Exist(AttDate.ToString("yyyy-MM-dd"), Get_Needed_EmployeeInfo[0]);
                        if (Leave_Info[0].ToString() != "0")  // check any type of leave. if are leave exists then execute this if block
                        {
                            DayStatus[0] = "Lv";
                            DayStatus[1] = Leave_Info[1];
                            DayStatus[2] = "00:00:00";
                            DayStatus[3] = "00:00:00";
                            DayStatus[4] = "00:00:00";
                            DayStatus[6] = "0";
                            DayStatus[7] = "00:00:00";
                            DayStatus[8] = "00:00:00";
                            classes.LeaveLibrary.LeaveCount(AttDate.ToString("yyyy-MM-dd"), Leave_Info[0]);
                            classes.
                           mManually_Attendance_Count.
                           SaveAttendance_Status_OD(Get_Needed_EmployeeInfo[0], AttDate.ToString("dd-MM-yyyy"), Get_Needed_EmployeeInfo[6], "00", "00", "00", "00", "00", "00",
                                                 DayStatus[0], DayStatus[1], DayStatus[2], Shift_Roster_InfoList[0], Get_Needed_EmployeeInfo[1],
                                                 Get_Needed_EmployeeInfo[2], ddlCompanyList.SelectedValue, Get_Needed_EmployeeInfo[3],
                                                 DayStatus[3], DayStatus[4], Shift_Roster_InfoList[3] + ":" + Shift_Roster_InfoList[6] + ":" + Shift_Roster_InfoList[4], "MC", "0", "0", DayStatus[6], DayStatus[7], DayStatus[8], ODType, RefId, ViewState["__UserId__"].ToString(), Remark, ODID);
                            return true;
                        }
                        else
                        {
                            //-------------------------End-----------------------------------------------
                            //---------------------------------------------------------------------------
                            // to checking selected date is weekend or holiday ?
                            DayStatus = classes.mZK_Shrink_Data_SqlServer.Check_Todays_Is_HolidayOrWeekend(AttDate.ToString("yyyy-MM-dd"));
                            if (bool.Parse(DayStatus[0]).Equals(true)) // if selected date is weekend or holiday then execute thi block
                            {


                               string wh = DayStatus[1];
                                    DayStatus = new string[9];
                                    DayStatus[0] = (wh.Equals("W")) ? "W" : "H";
                                    DayStatus[1] = (wh.Equals("W")) ? "Weekend" : "Holiday";
                                    DayStatus[2] = "00:00:00";
                                    DayStatus[3] = "00:00:00";
                                    DayStatus[4] = "00:00:00";
                                    DayStatus[5] = "0";
                                    DayStatus[6] = "0";
                                    DayStatus[7] = "00:00:00";
                                    DayStatus[8] = "00:00:00";
                                    holidaycount = "1";
                            classes.
                                mManually_Attendance_Count.
                                SaveAttendance_Status_OD(Get_Needed_EmployeeInfo[0], AttDate.ToString("dd-MM-yyyy"), Get_Needed_EmployeeInfo[6], "00",
                                                     "00", "00","00","00", "00",
                                                      DayStatus[0], DayStatus[1], DayStatus[2], Shift_Roster_InfoList[0], Get_Needed_EmployeeInfo[1],
                                                      Get_Needed_EmployeeInfo[2], ddlCompanyList.SelectedValue, Get_Needed_EmployeeInfo[3],
                                                      DayStatus[3], DayStatus[4], Shift_Roster_InfoList[3] + ":" + Shift_Roster_InfoList[6] + ":" + Shift_Roster_InfoList[4], "MC", DayStatus[5], holidaycount, "0", DayStatus[7], DayStatus[8], ODType,RefId, ViewState["__UserId__"].ToString(), Remark, ODID);
                            return true;
                            }
                            //---------------------------------------------------------------------------------------
                            else
                            {
                               string[] Roster_Operation_Status = classes.mManually_Attendance_Count.Roster_Operation_TimeChecking(TimeSpan.Parse(Shift_Roster_InfoList[7]), TimeSpan.Parse(Shift_Roster_InfoList[4]), TimeSpan.Parse(InHur + ":" + "00" + ":00"));

                                if (!bool.Parse(Roster_Operation_Status[0]))
                                {
                                    lblMessage.InnerText = "error->" + Roster_Operation_Status[1];
                                    return false;
                                }
                                DayStatus=new string[9];
                                DayStatus[0] = "P";
                                DayStatus[1] = "Present";
                                DayStatus[2] = "00:00:00";
                                DayStatus[3] = "00:00:00";
                                DayStatus[4] = "00:00:00";
                                DayStatus[5] = "0";
                                DayStatus[6] = "1";
                                DayStatus[7] = "00:00:00";
                                DayStatus[8] = "00:00:00";
                                classes.
                               mManually_Attendance_Count.
                               SaveAttendance_Status_OD(Get_Needed_EmployeeInfo[0], AttDate.ToString("dd-MM-yyyy"), Get_Needed_EmployeeInfo[6], "00", "00", "00", "00", "00", "00",
                                                     DayStatus[0], DayStatus[1], DayStatus[2], Shift_Roster_InfoList[0], Get_Needed_EmployeeInfo[1],
                                                     Get_Needed_EmployeeInfo[2], ddlCompanyList.SelectedValue, Get_Needed_EmployeeInfo[3],
                                                     DayStatus[3], DayStatus[4], Shift_Roster_InfoList[3] + ":" + Shift_Roster_InfoList[6] + ":" + Shift_Roster_InfoList[4], "MC", DayStatus[5], holidaycount, DayStatus[6], DayStatus[7], DayStatus[8], ODType,RefId, ViewState["__UserId__"].ToString(), Remark, ODID);
                                return true;
                            }
                        }


                    }
                    else // if employee duty type is regular then execute this bolock
                    {

                        // to get all roster information 
                        Shift_Roster_InfoList = classes.mCommon_Module_For_AttendanceProcessing.LoadShift_Information(Get_Needed_EmployeeInfo[0], true, Get_Needed_EmployeeInfo[5], AttDate.ToString("yyyy-MM-dd"));

                        //-- for leave information and leave count-----------------------------
                        string[] Leave_Info = classes.mZK_Shrink_Data_SqlServer.Check_Any_Leave_Are_Exist(AttDate.ToString("yyyy-MM-dd"), Get_Needed_EmployeeInfo[0]);
                        if (Leave_Info[0].ToString() != "0")  // check any type of leave. if are leave exists then execute this if block
                        {
                            DayStatus[0] = "Lv";
                            DayStatus[1] = Leave_Info[1];
                            DayStatus[2] = "00:00:00";
                            DayStatus[3] = "00:00:00";
                            DayStatus[4] = "00:00:00";
                            DayStatus[5] = "0";
                            DayStatus[6] = "0";
                            DayStatus[7] = "00:00:00";
                            DayStatus[8] = "00:00:00";
                            classes.LeaveLibrary.LeaveCount(AttDate.ToString("yyyy-MM-dd"), Leave_Info[0]);
                        }
                        //-------------------------End-----------------------------------------------
                        else
                        {
                            // to checking selected date is weekend or holiday ?
                            DayStatus = classes.mZK_Shrink_Data_SqlServer.Check_Todays_Is_HolidayOrWeekend(AttDate.ToString("yyyy-MM-dd"));
                            if (bool.Parse(DayStatus[0]).Equals(true)) // if selected date is weekend or holiday then execute thi block
                            {
                                string wh = DayStatus[1];
                                    DayStatus = new string[9];
                                    DayStatus[0] = (wh.Equals("W")) ? "W" : "H";
                                    DayStatus[1] = (wh.Equals("W")) ? "Weekend" : "Holiday";
                                    DayStatus[2] = "00:00:00";
                                    DayStatus[3] = "00:00:00";
                                    DayStatus[4] = "00:00:00";
                                    DayStatus[5] = "0";
                                    DayStatus[6] = "0";
                                    DayStatus[7] = "00:00:00";
                                    DayStatus[8] = "00:00:00";
                                    holidaycount="1";
                            }
                            else
                            {

                                
                                DayStatus = new string[9];
                                DayStatus[0] = "P";
                                DayStatus[1] = "Present";
                                DayStatus[2] = "00:00:00";
                                DayStatus[3] = "00:00:00";
                                DayStatus[4] = "00:00:00";
                                DayStatus[5] = "0";
                                DayStatus[6] = "1";
                                DayStatus[7] = "00:00:00";
                                DayStatus[8] = "00:00:00";
                                
                            }
                        }
                        classes.
                            mManually_Attendance_Count.
                        SaveAttendance_Status_OD(Get_Needed_EmployeeInfo[0], AttDate.ToString("dd-MM-yyyy"), Get_Needed_EmployeeInfo[6], "00", "00", "00", "00", "00", "00",
                                                  DayStatus[0], DayStatus[1], DayStatus[2], Shift_Roster_InfoList[0], Get_Needed_EmployeeInfo[1],
                                                  Get_Needed_EmployeeInfo[2], ddlCompanyList.SelectedValue, Get_Needed_EmployeeInfo[3],
                                                  DayStatus[3], DayStatus[4], Shift_Roster_InfoList[3] + ":" + Shift_Roster_InfoList[6] + ":" + Shift_Roster_InfoList[4], "MC", DayStatus[5], holidaycount, DayStatus[6], DayStatus[7], DayStatus[8], ODType, RefId, ViewState["__UserId__"].ToString(), Remark, ODID);
                        return true;
                    }




                }
                catch (Exception ex) { lblMessage.InnerText = "error-> " + ex.Message; return false; }
            }
            catch (Exception ex) { lblMessage.InnerText = "error-> " + ex.Message; return false; }
           
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
            Button btn;
            try
            {
                string EmpID = gvForApprovedList.DataKeys[e.Row.RowIndex].Values[1].ToString();
                string AuthorityAction = commonTask.getODAuthorityAction(ViewState["__UserId__"].ToString(), EmpID);
                if (AuthorityAction == "")
                {
                    btn = new Button();
                    btn = (Button)e.Row.FindControl("btnForword");
                    btn.Enabled = false;
                    btn.ForeColor = Color.Silver;

                    btn = new Button();
                    btn = (Button)e.Row.FindControl("btnYes");
                    btn.Enabled = false;
                    btn.ForeColor = Color.Silver;

                    btn = new Button();
                    btn = (Button)e.Row.FindControl("btnNot");
                    btn.Enabled = false;
                    btn.ForeColor = Color.Silver;
                }
                else
                {
                    ViewState["__ODAuthorityAction__"] = AuthorityAction;
                    if (ViewState["__ODAuthorityAction__"].ToString() != "1" && ViewState["__ODAuthorityAction__"].ToString() != "0")
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnForword");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;
                    }
                    if (ViewState["__ODAuthorityAction__"].ToString() != "2" && ViewState["__ODAuthorityAction__"].ToString() != "0")
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnYes");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;

                    }
                }
               


            }
            catch { }
        }

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000") || ddlCompanyList.SelectedValue.ToString().Equals("")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();

                classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, CompanyId);

                searchLeaveApplicationForApproved();   // searching operation
            }
            catch { }
        }

        protected void ddlShiftName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000") || ddlCompanyList.SelectedValue.ToString().Equals("")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();

                classes.commonTask.loadDepartmentListByCompanyAndShift(ddlDepartmentList, CompanyId, ddlShiftName.SelectedValue.ToString());

                searchLeaveApplicationForApproved();   // searching operation
            }
            catch { }
        }

        protected void ddlDepartmentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                searchLeaveApplicationForApproved();   // searching operation
            }
            catch { }
        }

        protected void ddlFindingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                searchLeaveApplicationForApproved();   // searching operation
            }
            catch { }
        }

        protected void lnkRefresh_Click(object sender, EventArgs e)
        {
            try
            {

                searchLeaveApplicationForApproved();   // searching operation
            }
            catch { }
        }
        private void showDetails(string ODID)
        {
            DataTable dtDetails = new DataTable();
            sqlDB.fillDataTable("select ClientName,InTime,OutTime,Purpose from tblOutdutyDetails where ODID=" + ODID + " order by OutTime", dtDetails);
            if (dtDetails != null && dtDetails.Rows.Count > 0)
            {
                gvDetails.DataSource = dtDetails;
                gvDetails.DataBind();
                mpe.Show();
            }
            
        }

        protected void ckbAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            foreach (GridViewRow row in gvForApprovedList.Rows)
            {
                CheckBox ckbChosen = (CheckBox)row.FindControl("ckbChosen");
                ckbChosen.Checked = ckb.Checked;

                //ckbChosen= 
            }
        }

        protected void btnForword_Click(object sender, EventArgs e)
        {

            foreach (GridViewRow row in gvForApprovedList.Rows)
            {
                CheckBox ckbChosen = (CheckBox)row.FindControl("ckbChosen");
                if (ckbChosen.Checked)
                {
                    if (checkOutDutyAuthority(row.RowIndex, "1"))
                    {
                        ViewState["__Predecate__"] = "Forword";
                        Forward(row.RowIndex);
                    }

                }
            }
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvForApprovedList.Rows)
            {
                CheckBox ckbChosen = (CheckBox)row.FindControl("ckbChosen");
                if (ckbChosen.Checked)
                {
                    if (checkOutDutyAuthority(row.RowIndex, "2"))
                    {
                        ViewState["__Predecate__"] = "Yes";
                        Label lblEmpCardNo = gvForApprovedList.Rows[row.RowIndex].FindControl("lblEmpCardNo") as Label;
                        Label lblDate = gvForApprovedList.Rows[row.RowIndex].FindControl("lblDate") as Label;
                        Label lblRemark = gvForApprovedList.Rows[row.RowIndex].FindControl("lblRemark") as Label;
                        YesApproved(row.RowIndex);
                    }

                }
            }
        }

        protected void btnNot_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvForApprovedList.Rows)
            {
                CheckBox ckbChosen = (CheckBox)row.FindControl("ckbChosen");
                if (ckbChosen.Checked)
                {
                    if (checkOutDutyAuthority(row.RowIndex, "1,2"))
                    {
                        ViewState["__Predecate__"] = "No";
                        NoApproved(row.RowIndex);
                    }
                }
            }
        }

        protected void ckbChosen_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckbAll = (CheckBox)gvForApprovedList.HeaderRow.FindControl("ckbAll");
            foreach (GridViewRow row in gvForApprovedList.Rows)
            {
                CheckBox ckbChosen = (CheckBox)row.FindControl("ckbChosen");
                if (!ckbChosen.Checked)
                {
                    ckbAll.Checked = false;
                    break;
                }
                else
                    ckbAll.Checked = true;
            }
        }
        private bool checkOutDutyAuthority(int rIndex, string AuthorityAction)
        {
            try
            {
                string EmpId = gvForApprovedList.DataKeys[rIndex].Values[1].ToString();
                DataTable dtLvAuth;
                string SqlCmd = "select * from tblOutDutyAuthorityAccessControl where AuthorityID=" + ViewState["__UserId__"].ToString() + " and AuthorityAction in (" + AuthorityAction + ") and EmpID='" + EmpId + "'";
                sqlDB.fillDataTable(SqlCmd, dtLvAuth = new DataTable());
                if (dtLvAuth == null || dtLvAuth.Rows.Count == 0)
                    return false;
                return true;
            }
            catch (Exception ex) { return false; }
        }
        protected void rblEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchLeaveApplicationForApproved();
        }

    }
}