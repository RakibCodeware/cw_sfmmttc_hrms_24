using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using ComplexScriptingSystem;
using adviitRuntimeScripting;
using System.Data.SqlClient;
using System.Drawing;
using SigmaERP.classes;
using System.IO;
using SigmaERP.hrms.BLL;

namespace SigmaERP.personnel
{
    public partial class aplication : System.Web.UI.Page
    {
        string sqlCmd = "";

        //permission=296
        DataTable DTLocal = null;
        string[] getColumns;
        string[] getValues;
        SqlCommand cmd;
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = {296};
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                txtApplyDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                txtApplyDate.Enabled = false;
                ViewState["__FindName__"] = "No";
                classes.commonTask.LoadBranch(ddlBranch);
                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                trpregnatn.Visible = false;
                trprasabera.Visible = false;
                ViewState["yesAlater"] = "False";
                string[] query={" "};
                try
                {
                    query = Request.QueryString["LC"].ToString().Split('-');
                }
                catch
                { }

                setPrivilege();
                
                
             //   classes.commonTask.loadLeaveTypes(ddlLeaveName);
              
               // loadLeaveApplication();
               
                
                ddlLeaveName.SelectedIndex = 0;
                // loadPunismentInfo();
                TabContainer1.ActiveTabIndex = 0;
                if (query[0] != " ")
                {
                    chkProcessed.Enabled = true;
                    SetValueToControl(query[0]);               
                    ViewState["yesAlater"] = "True";
                    ViewState["__LACode__"] = query[0];
                }

                if (!classes.commonTask.HasBranch())
                    ddlBranch.Enabled = false;
                ddlBranch.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
           
        }       
       

        static DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserId__"] = getUserId;
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                if (ViewState["__UserId__"].ToString().Equals("1008") || ViewState["__UserId__"].ToString().Equals("7080"))//  master admin/ rohol amin
                {
                    txtApplyDate.Text = "";
                    txtApplyDate.Enabled = true;
                }
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "aplication.aspx", ddlBranch, btnSaveLeave);

                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];
                ViewState["__IsLeaveAuthority__"] = commonTask.IsLeaveAuthority(ViewState["__CompanyId__"].ToString());
                if (ViewState["__ReadAction__"].ToString().Equals("0"))
                {
                    gvLeaveApplication.Visible = false;
                    gvRejectedList.Visible = false;
                    btnSelectAll.Enabled = false;
                    btnSelectAll.CssClass = "";
                }
                classes.commonTask.loadDepartmentListByCompany(ddlDepartment, ViewState["__CompanyId__"].ToString());
                classes.commonTask.loadLeaveNameByCompany(ddlLeaveName, ViewState["__CompanyId__"].ToString());



            }


            catch { }

        }

     
        void loadLeaveApplication(string EmpID)   // all approved leave application
        {
            string query = "select LACode,SUBSTRING(EmpCardNo,8,10) +' ('+EmpProximityNo+')'  as EmpCardNo,EmpName,format(FromDate,'dd-MM-yyyy') as FromDate,format(ToDate,'dd-MM-yyyy') as ToDate,format(ApplyDate,'dd-MM-yyyy') as ApplyDate,WeekHolydayNo,TotalDays,IsApproved,CurrentProcessStatus,LeaveName,AuthorizedBy,LeaveProcessingOrder,DptName from v_Leave_LeaveApplication Where LACode=(select Max(LACode) from Leave_LeaveApplication  where EmpId='"+ EmpID + "')";

            sqlDB.fillDataTable(query, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
               
                gvLeaveList.DataSource = null;
                gvLeaveList.DataBind();

            }
            else
            {
                gvLeaveList.DataSource = dt;
                gvLeaveList.DataBind();
            }
            
            
        }

        void loadRejected_LeaveApplication()  // for display all rejected leave application
        {           
        }

        private string getLeaveAuthority()
        {
            try
            {
                DataTable dtLvOrder;
                sqlCmd = "select case when IsDirectApprove=1 then -1 else   max(AuthorityPosition) end as LvAuthorityOrder from tblLeaveAuthorityAccessControl where EmpID='" + ViewState["EmpId"].ToString() + "' group by IsDirectApprove";
                sqlDB.fillDataTable(sqlCmd, dtLvOrder = new DataTable());
                if (dtLvOrder == null || dtLvOrder.Rows.Count == 0)
                    return "";
                return dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString();

                //--- old code bellow---               
                sqlDB.fillDataTable("select ISNULL(max(LvAuthorityOrder),0) LvAuthorityOrder from v_UserAccount where  isLvAuthority=1 and DptId in(" + ddlDepartment.SelectedValue + ") and LvOnlyDpt=1 and LvEmpType in(0," + ViewState["__EmpTypeId__"].ToString()+ ") and status=1", dtLvOrder = new DataTable());
                if (!dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString().Equals("0"))
                    return dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString();
                else
                {
                    sqlDB.fillDataTable("select ISNULL(max(LvAuthorityOrder),0) LvAuthorityOrder from v_UserAccount where isLvAuthority=1 and LvOnlyDpt=0 and LvEmpType in(0," + ViewState["__EmpTypeId__"].ToString()+ ") and status=1", dtLvOrder = new DataTable());
                    return dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString();
                }
            }
            catch { return "0"; }
        }
        private void saveLeaveApplication()
        {
            try
            {
                string LeaveProcessingOrder = "0";
                string IsApprove = "0";
                string CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string CreatedBy = ViewState["__UserId__"].ToString();
                if(CreatedBy.Equals("1008"))
                    IsApprove = "1";
                else if (bool.Parse(ViewState["__IsLeaveAuthority__"].ToString()))// if this company use leave appoval system
                {
                    LeaveProcessingOrder = getLeaveAuthority();
                    if (LeaveProcessingOrder.Trim() == "") // OD Authority is not found. plz setup for this employee.
                    {
                       
                       ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('You are not able to apply for Leave. Please contact with Authority.');", true);
                        return;
                        // 
                    }
                    else if (LeaveProcessingOrder.Trim() == "-1")// This Employee is allowed for direct approval
                    {
                        IsApprove = "1";
                    }
                   
                }
                else
                    IsApprove = "1";

                

              //  byte process = (DateTime.ParseExact(txtFromDate.Text.Trim(),"dd-MM-yyyy",null) >= DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"),"dd-MM-yyyy",null)) ? (byte)1 : (byte)0;
                byte process = 0;
                string CompanyId = (ddlBranch.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlBranch.SelectedValue.ToString();
                string[] fd = txtFromDate.Text.Trim().Split('-');
                string[] td = txtToDate.Text.Trim().Split('-');
                string[] apd = txtApplyDate.Text.Trim().Split('-');


                if (!ddlLeaveName.SelectedItem.ToString().Contains("m/l"))
                {

                    string[] getColumns = { "LeaveId", "EmpId", "FromDate", "ToDate", "WeekHolydayNo", "TotalDays", "IsApproved", "IsProcessessed", "EmpTypeId", "EntryDate", "CompanyId", "SftId", "Remarks", "DptId", "DsgId", "EmpStatus", "LeaveFormSLNo", "ApplyDate", "LeaveProcessingOrder", "LvAddress", "LvContact", "CreatedAt","CreatedBy" };

                    string[] getValues = { ddlLeaveName.SelectedValue.ToString(), ViewState["EmpId"].ToString(), fd[2]+"-"+fd[1]+"-"+fd[0],
                                             td[2]+"-"+td[1]+"-"+td[0], txtTotalHolydays.Text.Trim(), 
                                             txtNoOfDays.Text.Trim(), 
                                             IsApprove, process.ToString(),ViewState["__EmpTypeId__"].ToString(),DateTime.Now.ToString("yyyy-MM-dd"),CompanyId,ViewState["__SftId__"].ToString(),txtNotes.Text.Trim(),
                                         ViewState["__DptId__"].ToString(),ViewState["__DsgId__"].ToString(),"1",classes.LeaveLibrary.GetLeaveFormSerialNo().ToString(), apd[2]+"-"+apd[1]+"-"+apd[0],getLeaveAuthority(),txtLvAddress.Text.Trim(),txtLvContact.Text.Trim(),CreatedAt,CreatedBy};

                    
                   
                        if (SQLOperation.forSaveValue("Leave_LeaveApplication", getColumns, getValues, sqlDB.connection) == true)
                        {
                            PeocessLeaveDetails();
                        if (ViewState["yesAlater"].ToString().Equals("True"))
                        {
                            Response.Redirect("/leave/all_leave_list.aspx");
                        }
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "SuccessMsg('Successfully Leave Application Saved.');", true);
                            //lblMessage.InnerText = "success->Successfully Leave Application Saved";
                            loadLeaveApplication(ViewState["EmpId"].ToString());
                            Clear();
                        }
                    
                    

                }
                else
                {
                    if (!checkSex()) return;
                    string[] pgt = txtPregnantDate.Text.Trim().Split('-');
                    string[] psb = txtPrasabaDate.Text.Trim().Split('-');

                    string[] getColumns = { "LeaveId", "EmpId", "FromDate", "ToDate", "WeekHolydayNo", "TotalDays", "IsApproved", "IsProcessessed", "EmpTypeId", "PregnantDate", "PrasaberaDate", "EntryDate", "CompanyId", "SftId", "Remarks", "DptId", "DsgId", "EmpStatus", "LeaveFormSLNo", "ApplyDate", "LeaveProcessingOrder", "LvAddress", "LvContact", "CreatedAt", "CreatedBy" };

                    string[] getValues = { ddlLeaveName.SelectedValue.ToString(), ViewState["EmpId"].ToString(), fd[2]+"-"+fd[1]+"-"+fd[0],
                                             td[2]+"-"+td[1]+"-"+td[0], txtTotalHolydays.Text.Trim(), txtNoOfDays.Text.Trim(), 
                                             IsApprove, process.ToString(),
                                             ViewState["__EmpTypeId__"].ToString(),pgt[2]+"-"+pgt[1]+"-"+pgt[0],
                                            psb[2]+"-"+psb[1]+"-"+psb[0],DateTime.Now.ToString("yyyy-MM-dd"),CompanyId,ViewState["__SftId__"].ToString(),txtNotes.Text.Trim(),
                                            ViewState["__DptId__"].ToString(),ViewState["__DsgId__"].ToString(),"8",classes.LeaveLibrary.GetLeaveFormSerialNo().ToString(), apd[2]+"-"+apd[1]+"-"+apd[0],getLeaveAuthority(),txtLvAddress.Text.Trim(),txtLvContact.Text.Trim(), CreatedAt,CreatedBy};

                    if (SQLOperation.forSaveValue("Leave_LeaveApplication", getColumns, getValues, sqlDB.connection) == true)
                    {
                       // ChangeEmpTypeForML();
                        PeocessLeaveDetails();
                        if (ViewState["yesAlater"].ToString().Equals("True"))
                        {
                            Response.Redirect("/leave/all_leave_list.aspx");
                        }
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "SuccessMsg('Successfully Leave Application Saved.');", true);
                       // lblMessage.InnerText = "success->Successfully Leave Application Saved";
                        loadLeaveApplication(ViewState["EmpId"].ToString());
                        Clear();
                    }
                }

                

            }
            catch (Exception ex)
            {

                exceptionMsg(ex.Message);
            }
        }
        private void exceptionMsg(string msg)
        {
             
            if (msg.Contains("PK_"))
                msg = "This date is already applied";

            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('" + msg + "');", true);
        }

        private void PeocessLeaveDetails()
        {
            try
            {
                sqlDB.fillDataTable("select MAX(LACode) as LACode from Leave_LeaveApplication where EmpId=" + ViewState["EmpId"].ToString() + "", dt = new DataTable());
                DateTime FromDate = new DateTime(int.Parse(txtFromDate.Text.Trim().Substring(6, 4)), int.Parse(txtFromDate.Text.Trim().Substring(3, 2)), int.Parse(txtFromDate.Text.Trim().Substring(0, 2)));
                DateTime ToDate = new DateTime(int.Parse(txtToDate.Text.Trim().Substring(6, 4)), int.Parse(txtToDate.Text.Trim().Substring(3, 2)), int.Parse(txtToDate.Text.Trim().Substring(0, 2)));

                DateTime ApplyDate = new DateTime(int.Parse(txtApplyDate.Text.Trim().Substring(6, 4)), int.Parse(txtApplyDate.Text.Trim().Substring(3, 2)), int.Parse(txtApplyDate.Text.Trim().Substring(0, 2)));

                DateTime FromDate1 = FromDate;

                while (FromDate <= ToDate)
                {
                    
                    if (ApplyDate > FromDate1)             
                        saveApplyDate(dt.Rows[0]["LACode"].ToString(), FromDate.ToString("yyyy-MM-dd"));
                    
                    
                    saveLeaveDetails(dt.Rows[0]["LACode"].ToString(), FromDate.ToString("yyyy-MM-dd"));
                    FromDate = FromDate.AddDays(1);
                }


                SaveAttachDocument(dt.Rows[0]["LACode"].ToString());      
            }
            catch { }

        }
        private void saveLeaveDetails(string LACode, string Date)
        {
            try
            {
                string[] getColumns = { "LACode", "EmpId", "LeaveDate", "Used" };
                string[] getValues = { LACode, ViewState["EmpId"].ToString(),Date, "0" };
                SQLOperation.forSaveValue("Leave_LeaveApplicationDetails", getColumns, getValues, sqlDB.connection);

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('" + ex.Message + "');", true);
              //  lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        private void saveApplyDate(string LACode, string FromDate) // While Apply Date < From Date then execute this function().
        {
            try
            {
                string[] getColumns = { "EmpId", "LACode", "Lv_date"};
                string[] getValues = { ViewState["EmpId"].ToString(), LACode,FromDate};
                SQLOperation.forSaveValue("Leave_ApplyDate", getColumns, getValues, sqlDB.connection);

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('" + ex.Message + "');", true);
                //lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        private bool checkSex()
        {
            try
            {
                sqlDB.fillDataTable("select Sex from Personnel_EmpPersonnal where EmpId='"+ViewState["EmpId"].ToString()+"'",dt=new DataTable ());
                if (dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('Please set sex or gender of this employee !');", true);
                    //lblMessage.InnerText = "error->Please set sex or gender of this employee !";
                    return false;
                }
                bool sex = (dt.Rows[0]["Sex"].ToString().Equals("Female")) ? true : false;
                if (!sex)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('This Employee Is Male !');", true);
                    //lblMessage.InnerText = "error->This Employee Is Male !";
                    return false;
                }
                else return true;
            }
            catch { return false;}
        }

        private void ChangeEmpTypeForML()   // change EmpType when any female get any Maternity Leave
        {
            try
            {
                cmd = new SqlCommand("update Personnel_EmpCurrentStatus set EmpStatus='8' where SN=(select Max(SN) from  Personnel_EmpCurrentStatus Where EmpId='" + ViewState["EmpId"].ToString() + "' AND IsActive=1)", sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }
       
        private void deleteExistsAttendance(string DateFrom)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("delete from tblAttendanceRecord where EmpId ='" + ViewState["EmpId"].ToString() + "' and ATTDate='" + DateFrom + "'",sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        
        }

        private void saveLeavedayAndToadyIsEquql()
        {
            try
            {
                sqlDB.fillDataTable("select EmpProximityNo from Personnel_EmployeeInfo where EmpId='" + ViewState["EmpId"].ToString() + "'",dt=new DataTable ());


                string[] fd = txtFromDate.Text.Trim().Split('-');
                string[] getColumns = { "EmpProximityNo", "EmpCardNo", "ATTDate", "InHour", "InMin", "InSec", "OutHour", "OutMin", "OutSec", "BreakStartHour", "BreakStartMin", "BreakEndHour", "BreakEndMin", "ATTStatus", "StateStatus", "Remarks", "EmpTypeId", "AttManual", "EmpId", "DailyStartTimeALT" };
                string[] getValues = { dt.Rows[0]["EmpProximityNo"].ToString(), txtEmpCardNo.Text.Trim(), fd[2]+"-"+fd[1]+"-"+fd[0], 
                                         "00", "00", "00", "00", "00", "00", "00", "00", "00", "00", "LV", ddlLeaveName.SelectedItem.Text," ",ViewState["__EmpTypeId__"].ToString(), "Manual Attendance", ViewState["EmpId"].ToString(), "00-00-00-00" };
                SQLOperation.forSaveValue("tblAttendanceRecord", getColumns, getValues, sqlDB.connection);
              
            }
            catch { }
        }

        private void changeEmpTypeOnBaseMaternityLeaveDelete(string EmpId,string EmpTypeId)
        {
            try
            {
                cmd = new SqlCommand("update Personnel_EmpCurrentStatus set EmpStatus='1' where SN=(select Max (SN) from Personnel_EmpCurrentStatus where EmpId='" + EmpId + "' and IsActive=1)", sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }
       
        void Clear()
        {
            ViewState["__FindName__"] = "No";
            lblDepartment.Text = "";
            lblSftTime.Text = "";
            ViewState["yesAlater"] = "False";
            txtEmpCardNo.Text = "";
            txtEmpName.Text = "";
            txtApplyDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            txtFromDate.Text = "";
            txtToDate.Text = "";
            txtTotalHolydays.Text = "";
            txtNoOfDays.Text = "";
            txtNotes.Text = "";
            chkApproved.Checked = true;
            chkProcessed.Checked = true;
            txtCardNo.Enabled = true;
            txtPregnantDate.Text = "";
            txtPrasabaDate.Text = "";
            trprasabera.Visible = false;
            trpregnatn.Visible = false;
            trStatusBar.Visible = false;

            if (ViewState["__WriteAction__"].Equals("0"))
            {
                btnSaveLeave.Enabled = false;
                btnSaveLeave.CssClass = "";
            }
            else
            {
                btnSaveLeave.Enabled = true;
                btnSaveLeave.CssClass = "Rbutton";
            }
            btnSaveLeave.Text = "Save";
            ddlLeaveName.SelectedIndex = 0;
            chkProcessed.Enabled = false;            
            ddlEmpCardNo.SelectedIndex = 0;
            txtFileName.Text = "";
         
            
        }

        private void SetValueToControl(string LAId)
        {
            try
            {
                string strSQL = @" select la.EmpCardNo,la.LACode, la.EmpId, la.FromDate,la.IsApproved,la.IsProcessessed,la.ToDate,la.ApplyDate,
                                la.WeekHolydayNo,la.EmpTypeId, la.TotalDays, lc.LeaveId,et.EmpType,la.PregnantDate,la.PrasaberaDate,la.CompanyId,la.SftId,
                                la.Remarks,la.DptId,la.LvAddress,la.LvContact
                                from   dbo.v_Leave_LeaveApplication la
                                join dbo.tblLeaveConfig lc on lc.LeaveId=la.LeaveId inner join HRD_EmployeeType et on et.EmpTypeId=la.EmpTypeId
                                where la.LACode='" + LAId + "'";
                DataTable DTLocal = new DataTable();
                

                sqlDB.fillDataTable(strSQL, DTLocal);

                ddlBranch.SelectedValue = DTLocal.Rows[0]["CompanyId"].ToString();
               // ddlShiftName.SelectedValue = DTLocal.Rows[0]["SftId"].ToString();
                ddlDepartment.SelectedValue = DTLocal.Rows[0]["DptId"].ToString();
                classes.commonTask.LoadEmpCardNoByDepartment(ddlEmpCardNo, ddlBranch.SelectedValue, ddlDepartment.SelectedValue);
                hdLeaveApplicationId.Value = DTLocal.Rows[0]["LACode"].ToString();
                ddlEmpCardNo.SelectedValue = DTLocal.Rows[0]["EmpCardNo"].ToString();
                txtEmpCardNo.Text = DTLocal.Rows[0]["EmpCardNo"].ToString();
                ViewState["EmpId"] = DTLocal.Rows[0]["EmpId"].ToString();
                //txtEmpName.Text = DTLocal.Rows[0]["EmpName"].ToString();
                //txtStartDate.Text = Convert.ToDateTime(retutnObject.StartDate).ToString("dd-MMM-yyyy");
                ViewState["OldApplyDate"] = txtApplyDate.Text = Convert.ToDateTime(DTLocal.Rows[0]["ApplyDate"]).ToString("dd-MM-yyyy");
                ViewState["OldFromDate"]=txtFromDate.Text = Convert.ToDateTime(DTLocal.Rows[0]["FromDate"]).ToString("dd-MM-yyyy");
                ViewState["OldToDate"]=txtToDate.Text = Convert.ToDateTime(DTLocal.Rows[0]["ToDate"]).ToString("dd-MM-yyyy");   
    
                txtTotalHolydays.Text = DTLocal.Rows[0]["WeekHolydayNo"].ToString();
                txtNoOfDays.Text = DTLocal.Rows[0]["TotalDays"].ToString();
                ddlLeaveName.SelectedValue = DTLocal.Rows[0]["LeaveId"].ToString();
                chkApproved.Checked = ((DTLocal.Rows[0]["IsApproved"].ToString()).Equals("True")) ? true : false;
                chkProcessed.Checked = ((DTLocal.Rows[0]["IsProcessessed"].ToString()).Equals("True")) ? true : false;
                txtLvAddress.Text = DTLocal.Rows[0]["LvAddress"].ToString();
                txtLvContact.Text = DTLocal.Rows[0]["LvContact"].ToString();
                trStatusBar.Visible = true;             
                btnSaveLeave.Text = "Update";
                if (File.Exists(Server.MapPath("/EmployeeImages/LeaveDocument/" +LAId.ToString()+".jpg"))) 
                {
                    txtFileName.Text = LAId.ToString();
                }
                if (ViewState["__UpdateAction__"].Equals("0"))
                {
                    btnSaveLeave.Enabled = false;
                    btnSaveLeave.CssClass = "";
                }
                else
                {
                    btnSaveLeave.Enabled = true;
                    btnSaveLeave.CssClass = "Lbutton";
                }
                txtCardNo.Enabled = false;
                txtNotes.Text = DTLocal.Rows[0]["Remarks"].ToString();
                if (ddlLeaveName.SelectedItem.Text == "M/L")
                {
                    if (DTLocal.Rows[0]["PregnantDate"].ToString() == "")
                        txtPregnantDate.Text = DTLocal.Rows[0]["PregnantDate"].ToString();
                    else txtPregnantDate.Text = DateTime.Parse(DTLocal.Rows[0]["PregnantDate"].ToString()).ToString("dd-MM-yyyy");
                    if (DTLocal.Rows[0]["PrasaberaDate"].ToString()=="")
                        txtPrasabaDate.Text = DTLocal.Rows[0]["PrasaberaDate"].ToString();
                    else txtPrasabaDate.Text = DateTime.Parse(DTLocal.Rows[0]["PrasaberaDate"].ToString()).ToString("dd-MM-yyyy");
                    trprasabera.Visible = true;
                    trpregnatn.Visible = true;
                }

                findOutShiftTimeAndDptName();
                findEmpName();
            }
            catch { }
        }

        private void findOutShiftTimeAndDptName()
        {
            try
            {
                //sqlDB.fillDataTable("select  StartTime12Fromat,EndTime12Fomat From v_HRD_Shift where CompanyId='" + ddlBranch.SelectedValue.ToString() + "' AND SftId=" + ddlShiftName.SelectedValue.ToString() + "", dt = new DataTable());
                //if (dt.Rows.Count > 0) lblSftTime.Text = dt.Rows[0]["StartTime12Fromat"].ToString() + "-" + dt.Rows[0]["EndTime12Fomat"].ToString();
            }
            catch { }
        }
        protected void gvLeaveApplication_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
               // ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
               
                if (e.CommandName == "Alter")
                {
                    int index = Convert.ToInt32(e.CommandArgument.ToString());
                    string LACode = gvLeaveApplication.DataKeys[index].Value.ToString();
                    ViewState["__LACode__"] = LACode;
                   
                    ViewState["yesAlater"] = "True";
                  //  ViewState["OldFromDate"] = txtFromDate.Text = gvLeaveApplication.Rows[index].Cells[2].Text;
                   // ViewState["OldToDate"] = txtToDate.Text = gvLeaveApplication.Rows[index].Cells[3].Text;
                    SetValueToControl(LACode);

                }
                else if (e.CommandName == "Delete")
                {
                    
                    Delete(Convert.ToInt32(e.CommandArgument.ToString())); // delete leave and change essential record status
                   
                   // loadLeaveApplication();
                    Clear();
                }

                else if (e.CommandName == "Status")
                {
                    string LACode = gvLeaveApplication.DataKeys[Convert.ToInt32(e.CommandArgument)].Value.ToString();
                    sqlDB.fillDataTable("select EmpId from Leave_LeaveApplication where LACode="+LACode+"",dt=new DataTable ());
                    sqlDB.fillDataTable("select EmpName,EmpType,EmpCardNo from v_Personnel_EmpCurrentStatus where SN=(select Max(SN) from Personnel_EmpCurrentStatus where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' AND IsActive=1)",dt=new DataTable ());
                    
                    txtEmployeeType.Text = dt.Rows[0]["EmpType"].ToString();
                    txtCardNo.Text = dt.Rows[0]["EmpCardNo"].ToString();
                    txtName.Text = dt.Rows[0]["EmpName"].ToString();

                    sqlDB.fillDataTable("select Used from Leave_LeaveApplicationDetails where LACode="+LACode+"",dt=new DataTable ());

                    DataRow[] dr={};
                  
                    try
                    {
                        dr = dt.Select("Used='true'",null);
                    }
                    catch { }

                    txtTotalLeave.Text = dt.Rows.Count.ToString();
                      
                    txtUsed.Text = dr.Length.ToString();

                    txtUnused.Text = (Convert.ToInt16(txtTotalLeave.Text.Trim()) - Convert.ToInt16(txtUsed.Text.Trim())).ToString();
                    
                    
                    ModalPopupExtender1.Show();
                }

                else if (e.CommandName.Equals("View"))
                { 
                viewLeaveApplication(e.CommandArgument.ToString());
                }

            }
            catch (Exception ex)
            {
                //lblMessage.Text = ex.ToString();
            }
        }


        void Delete(int id)  // delete leave and change essential record status
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("select EmpId,Convert(varchar(11),FromDate,111) as FromDate,Convert(varchar(11),ToDate,111) as ToDate,LeaveName,EmpCardNo,EmpTypeId from v_Leave_LeaveApplication where Lacode =" + id.ToString() + "", dt);

            if (dt.Rows[0]["LeaveName"].ToString().ToLower().Equals("maternity leave") || dt.Rows[0]["LeaveName"].ToString().ToLower().Equals("m/l") || dt.Rows[0]["LeaveName"].ToString().ToLower().Equals("ml"))
            {
                changeEmpTypeOnBaseMaternityLeaveDelete(dt.Rows[0]["EmpId"].ToString(), dt.Rows[0]["EmpTypeId"].ToString());

            }

            if (SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplication", "LACode", id.ToString(), sqlDB.connection) == true)
            {
                
                string getEmpTypeId = dt.Rows[0]["EmpTypeId"].ToString();
                string getEmpCardNo = dt.Rows[0]["EmpCardNo"].ToString();

                sqlDB.fillDataTable("select ATTStatus,convert(varchar(11),AttDate,111)as AttDate from tblAttendanceRecord where EmpCardNo='" + getEmpCardNo + "' AND EmpTypeId=" + getEmpTypeId + " AND ATTDate in (select leaveDate from Leave_LeaveApplicationDetails where LACode="+id.ToString()+"AND used='true')", dt = new DataTable());

                for (byte b = 0; b < dt.Rows.Count; b++)
                {
                    if (!dt.Rows[b]["ATTStatus"].ToString().Equals("W"))
                    {

                        SqlCommand cmd = new SqlCommand("update tblAttendanceRecord set ATTStatus='A',StateStatus=' ',DailyStartTimeALT='00:00:00:00' where EmpCardNo='" + getEmpCardNo + "' AND EmpTypeId=" + getEmpTypeId + " AND ATTDate ='" + dt.Rows[b]["AttDate"].ToString() + "' ", sqlDB.connection);
                        cmd.ExecuteNonQuery();
                    }

                }

                SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplicationDetails", "LACode", id.ToString(), sqlDB.connection);

            
            }           

            btnSaveLeave.Text = "Save";
        }
        
        protected void gvLeaveApplication_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            loadLeaveApplication("");
        }

        protected void gvLeaveApplication_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }


        private void Read_N_Write_WH(int Fdays, int Fmonth, string Fyear, int Tdays, int Tmonth, string Tyear)    // N=And,WH=Weekly Holiday
        {
            try
            {
                string month;
                if (Fmonth.ToString().Length == 1) month = "0" + Fmonth.ToString();
                else month = Fmonth.ToString();
                DataTable dt=new DataTable ();
               // sqlDB.fillDataTable("select convert(varchar(11),WeekendDate,105) as WeekendDate from Attendance_WeekendInfo where MonthName='" + month+'-'+Fyear + "'",dt);

                sqlDB.fillDataTable("select convert(varchar(11),WeekendDate,105) as WeekendDate from Attendance_WeekendInfo where WeekendDate >='" + Fyear + '-' + Fmonth + '-' + Fdays + "'AND WeekendDate <='" + Tyear + '-' + Tmonth + '-' + Tdays + "'", dt);

                DateTime begin = new DateTime(int.Parse(Fyear), Fmonth,Fdays);
                DateTime end = new DateTime(int.Parse(Tyear), Tmonth, Tdays);
                int totalWeekendDays=0;
                while (begin <= end)
                {
                    if (begin.DayOfWeek == DayOfWeek.Friday)
                    {
                        string[] getDates = begin.ToString().Split('/');
                        string getDate=(getDates[1].Length == 1)? "0" + getDates[1]:getDates[1];
                        string getMonth = (getDates[0].Length == 1) ?"0" + getDates[0] : getDates[0];
                        bool yesCount = false;
                        for (byte b = 0; b < dt.Rows.Count;b++ )
                        {
                            if (getDate + "-" + getMonth + "-" + Fyear == dt.Rows[b]["WeekendDate"].ToString())
                            {
                                yesCount = true;
                                break;
                            }
                        }

                        if (yesCount) totalWeekendDays += 1;
                    }
                     begin=begin.AddDays(1);
                }
                txtTotalHolydays.Text = totalWeekendDays.ToString();
            }
            catch { }
        }

        DataTable dt;
        protected void btnFind_Click(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            if (txtEmpCardNo.Text.Trim().Length < 4)
            {
                lblMessage.InnerText = "error->Please type valid card no"; 
                return;
            }
            findEmpName();
        }

        private void findEmpName()
        {
            try
            {
                //sqlDB.fillDataTable("select empName,EmpId from Personnel_EmployeeInfo where EmpCardNo Like '%" +txtEmpCardNo.Text+ "' AND EmpTypeId =" + rbEmpList.SelectedValue.ToString() + " AND SftId="+ddlShiftName.SelectedValue.ToString()+"", dt = new DataTable());
                //lblMessage.InnerText = "";
                lblDepartment.Text = "";
                txtEmpName.Text = "";
                string CompanyId = (ddlBranch.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlBranch.SelectedValue.ToString();
                dt = new DataTable();
                sqlDB.fillDataTable("select EmpId,EmpName,CompanyName,DptName,EmpTypeId,DptId,DsgId,SftId from v_Personnel_EmpCurrentStatus where EmpCardNo like '%" + ddlEmpCardNo.SelectedValue+ "'" +
                         " AND IsActive='1' AND  EmpStatus in ('1','8')  " +
                         " AND CompanyId='" + CompanyId + "' AND DptId=" + ddlDepartment.SelectedValue + "", dt);

                if (dt.Rows.Count==0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('Please check your selected information or type valid card no !');", true);
                   // lblMessage.InnerText = "error->Please check your selected information or type valid card no."; 
                    return;
                }

                //------------------------------------------------------------------------------------------------
                // this validation for user type employee.that will be give only access own data access
                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("User"))
                {
                    if (!classes.UserPrivilege.UserPrivilegeForUserType(txtEmpCardNo.Text, CompanyId, ViewState["__UserId__"].ToString()))
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('Please type your correct cardno !');", true);
                      //  lblMessage.InnerText = "Please type your correct cardno !";
                        return;
                    
                    }
                }
                //--------------------------------------------------------------------------------------------------

                txtEmpName.Text = dt.Rows[0]["empName"].ToString();
                lblDepartment.Text = "Department : " + dt.Rows[0]["DptName"].ToString();
                ViewState["EmpId"] = dt.Rows[0]["EmpId"].ToString();
                ViewState["__EmpTypeId__"] = dt.Rows[0]["EmpTypeId"].ToString();
                ViewState["__SftId__"] = dt.Rows[0]["SftId"].ToString();
                ViewState["__DptId__"] = dt.Rows[0]["DptId"].ToString();
                ViewState["__DsgId__"] = dt.Rows[0]["DsgId"].ToString();
                ViewState["__FindName__"] = "Yes";
            }
            catch { }
        }
       
        private bool validationExistsRecord()
        {
            try
            {
                if (txtEmpName.Text.Trim().Length == 0) return false;
                if (ddlLeaveName.SelectedValue.ToString().Equals("50"))  // 50 is default value which is denoted empty string 
                {
                    return false ;
                }
                if (ddlLeaveName.SelectedItem.Text == "M/L")
                {
                    if (txtPregnantDate.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('Please Select Pregnant Date !');", true);
                       // lblMessage.InnerText = "warning->Please Select Pregnant Date";
                        txtPregnantDate.Focus();
                        return false;
                    }
                    else if (txtPregnantDate.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('Please Select Prasabera Date !');", true);
                       // lblMessage.InnerText = "warning->Please Select Prasabera Date";
                        txtPregnantDate.Focus();
                        return false;
                    }
                }
                return true;
            }
            catch { return false; }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
            Clear();
        }

        string[] fromDates;
        string[] toDates;
        protected void btnSaveLeave_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);


            if (ddlDepartment.SelectedIndex < 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Please select any Department !');", true);
                //lblMessage.InnerText = "warning-> Please select any Department !";
                ddlDepartment.Focus();
                return;
            }
            if (ddlEmpCardNo.SelectedValue == "0")
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Please select any Employee !');", true);
                /// lblMessage.InnerText = "warning-> Please select any Employee !";
                ddlEmpCardNo.Focus();
                return;
            }
            if (txtApplyDate.Text.Trim().Length < 8)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Please select Valid Apply Date !');", true);
                // lblMessage.InnerText = "warning-> Please select Valid Apply Date !";
                txtApplyDate.Focus();
                return;
            }
            if (txtFromDate.Text.Trim().Length < 8)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Please select Valid From Date !');", true);
                // lblMessage.InnerText = "warning-> Please select Valid From Date !";
                txtFromDate.Focus();
                return;
            }
            if (txtToDate.Text.Trim().Length < 8)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Please select Valid To Date !');", true);

                txtToDate.Focus();
                return;
            }
            dateCalculation();
            if (ddlLeaveName.SelectedIndex < 1)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Please select Leave Name !');", true);
                //lblMessage.InnerText = "warning-> Please select Leave Name !";
                ddlLeaveName.Focus();
                return;
            }
            sqlDB.fillDataTable("select LeaveDays,ShortName from tblLeaveConfig where LeaveId=" + ddlLeaveName.SelectedValue.ToString() + "", dt = new DataTable());
            string ShortName = dt.Rows[0]["ShortName"].ToString();
            string LeaveDays = dt.Rows[0]["LeaveDays"].ToString();
            if (ShortName.Equals("a/l"))
            {
                if (!commonTask.checkJoiningDateForEL(ddlEmpCardNo.SelectedValue, commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()), ViewState["__UserId__"].ToString()))
                {

                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('As per our company rules, before completed 1 year, employees are not capable to enjoy Earn leave facility.');", true);
                    return;
                }
            }
            sqlDB.fillDataTable("select MinimumCompletedDaysForLv, LeaveEntryValidateDays from HRD_OthersSetting", dt = new DataTable());
            string MinimumCompletedDaysForLv = dt.Rows[0]["MinimumCompletedDaysForLv"].ToString();
            string LeaveEntryValidateDays = dt.Rows[0]["LeaveEntryValidateDays"].ToString();

            if (!(ViewState["__UserId__"].ToString().Equals("1008") || ViewState["__UserId__"].ToString().Equals("7080")))
            {
            if (ShortName.Equals("c/l"))
            {
                float appliedLeaveDays = float.Parse(txtNoOfDays.Text.Trim());
                if (appliedLeaveDays > 2)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Casual Leave can not be applied more than 2 days at a time.');", true);
                    return;
                }
                sqlDB.fillDataTable("select Used from v_Leave_LeaveApplicationDetails where EmpId='" + ViewState["EmpId"].ToString() + "' And ShortName='c/l' AND FORMAT(LeaveDate,'MM-yyyy')= '" + txtFromDate.Text.Remove(0, 3) + "' ", dt = new DataTable());
                if ((appliedLeaveDays + dt.Rows.Count) > 2)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Casual Leave can not be applied more than 2 days in every month.');", true);
                    return;
                }

            }
            if (!ShortName.Equals("s/l"))
            {
                if (!commonTask.checkJoiningDate(ddlEmpCardNo.SelectedValue, int.Parse(MinimumCompletedDaysForLv)))
                {

                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('As per our company rules, before completed 3 months (" + MinimumCompletedDaysForLv + " Days), employees are not capable to enjoy leave facility except sick leave.');", true);
                    return;
                }
            }
            }
            

            //if (ViewState["__FindName__"].Equals("No"))

            if (ViewState["yesAlater"].Equals("True"))
            {


                DataTable dtLeaveDetails = new DataTable();

                fromDates = txtFromDate.Text.Trim().Split('-');
                toDates = txtToDate.Text.Trim().Split('-');

                string DailyStartTimeALT_CloseTime = "00:00:00:00:00:00";
                btnDateCalculation_Click(sender, e);

                if (!txtFromDate.Text.Trim().Equals(ViewState["OldFromDate"].ToString()) || !txtToDate.Text.Trim().Equals(ViewState["OldToDate"].ToString()))
                {
                    calculationForAlterOperation();

                    checkNotSameDate();
                    for (int i = 0; i < dtNotSameDate.Rows.Count; i++)
                    {
                        updateDailyAttendance(dtNotSameDate.Rows[i]["LeaveDate"].ToString(), "00", "00", "00", "00", "00", "00", DailyStartTimeALT_CloseTime, dtNotSameDate.Rows[i]["Status"].ToString());
                    }

                    SQLOperation.forDelete("temp_NewLeaveDateForAlter", sqlDB.connection);
                    SQLOperation.forDelete("temp_OldLeaveDateForAlter", sqlDB.connection);
                }
                if (!checkLeaveEntryDaysValidation(LeaveEntryValidateDays)) return;
                if (!checkLeaveDaysValidation(ShortName, LeaveDays, ViewState["__LACode__"].ToString()))
                {

                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('You do not have sufficient leave balance.');", true);
                    return;
                }
                SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplication", "LACode", ViewState["__LACode__"].ToString(), sqlDB.connection);
                SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplicationDetails", "LACode", ViewState["__LACode__"].ToString(), sqlDB.connection);

            }
            else
            {
                if (!checkLeaveEntryDaysValidation(LeaveEntryValidateDays)) return;
            }
            if (!checkLeaveDaysValidation(ShortName, LeaveDays,"0"))
            {

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('You do not have sufficient leave balance.');", true);
                return;
            } 
            
            saveLeaveApplication();
            ViewState["yesAlater"] = "False";
        }

       
        private bool checkLeaveEntryDaysValidation(string LeaveEntryValidateDays)
        {
            try
            {
                if (ViewState["__UserId__"].ToString().Equals("1008")|| ViewState["__UserId__"].ToString().Equals("7080"))
                    return true;
                string[] TDate = txtToDate.Text.Trim().Split('-');
                DateTime ToDate = DateTime.Parse(TDate[2]+"-"+TDate[1]+"-"+TDate[0]);
                //sqlDB.fillDataTable("select MinimumCompletedDaysForLv, LeaveEntryValidateDays from HRD_OthersSetting", dt = new DataTable());                
                int day = 3;
                if (dt != null && dt.Rows.Count > 0)
                    day = int.Parse(LeaveEntryValidateDays);
                int dateDiff = 0;
                if (ToDate.Date < DateTime.Now.Date)
                {
                    for (DateTime d = ToDate.AddDays(1).Date; d <= DateTime.Now.Date; d= d.AddDays(1))
                    {
                        if (!commonTask.isWH(d.ToString("yyyy-MM-dd")))
                            dateDiff++;
                        if (day < dateDiff)
                        {
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Entry time out.You must have to enter your leave application within "+ day + " days of joining period.');", true);
                            return false;
                        }
                            
                    }
                                   
                }
                return true;
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message; 
                return false;
            }
        }
        private bool checkLeaveDaysValidation(string ShortName,string LeaveDays,string LaCode)
        {
            try
            {
                string[] FromDate = txtFromDate.Text.Trim().Split('-');
                //sqlDB.fillDataTable("select LeaveDays,ShortName from tblLeaveConfig where LeaveId="+ddlLeaveName.SelectedValue.ToString()+"",dt=new DataTable ());

                if (ShortName.Equals("op/l") || ShortName.Equals("o/l")) return true;  // because officeal purpose leave or others leave has not needed any validation

                int getLeaveDays = int.Parse(LeaveDays);
                if (ShortName.Equals("sr/l"))
                {
                    sqlDB.fillDataTable("select Used from v_Leave_LeaveApplicationDetails where LACode not in ("+ LaCode + ") and  EmpId='" + ViewState["EmpId"].ToString() + "' And ShortName=(select ShortName from tblLeaveConfig where LeaveId=" + ddlLeaveName.SelectedValue.ToString() + ") AND FORMAT(LeaveDate,'MM-yyyy')= '" + txtFromDate.Text.Remove(0, 3) + "' ", dt = new DataTable());
                    if (dt.Rows.Count + int.Parse(txtNoOfDays.Text.Trim()) > getLeaveDays)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Already you've spent " + dt.Rows.Count + " days of  " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " of this Month Total allocated days for " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " is " + getLeaveDays + " !');", true);
                        //   lblMessage.InnerText = "error->Already you are spanted " + dt.Rows.Count + " days of  " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " of this Month Total allocated days for " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " is " + getLeaveDays + " !";
                        return false;
                    }
                    else return true;
                }                
                else
                {

                    sqlDB.fillDataTable("select Used from v_Leave_LeaveApplicationDetails where LACode not in (" + LaCode + ") and EmpId='" + ViewState["EmpId"].ToString() + "' And ShortName=(select ShortName from tblLeaveConfig where LeaveId=" + ddlLeaveName.SelectedValue.ToString() + ") AND FromYear='" + DateTime.Now.Year + "' ", dt = new DataTable());
                    if (ShortName.Equals("a/l"))
                    {
                        try {
                            DataTable dtELBalance = new DataTable();
                            sqlCmd = "with a as(select isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + FromDate[2] + "-01-01' and GenerateDate <= '" + FromDate[2] + "-12-31' and EmpID = '" + ViewState["EmpId"].ToString() + "' union all select ReserveEeanLeaveDays as EarnLeaveDays from Earnleave_Reserved where Year(ReserveFor) = '" + FromDate[2] + "' and EmpID = '" + ViewState["EmpId"].ToString() + "') select sum(EarnLeaveDays) as EarnLeaveDays from a";
                            sqlDB.fillDataTable(sqlCmd, dtELBalance);
                            if (dtELBalance != null && dtELBalance.Rows.Count > 0)
                                getLeaveDays = int.Parse(dtELBalance.Rows[0]["EarnLeaveDays"].ToString());
                            else
                                getLeaveDays = 0;
                        }
                        catch (Exception ex) { getLeaveDays = 0; }
                        
                    }
                        if (dt.Rows.Count + int.Parse(txtNoOfDays.Text.Trim()) > getLeaveDays)
                    {
                        //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Already you've spent " + dt.Rows.Count + " days of  " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " of this Year Total allocated days for " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " is " + getLeaveDays + " !');", true);
                        //lblMessage.InnerText = "error->Already you've spent " + dt.Rows.Count + " days of  " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " of this Year Total allocated days for " + ddlLeaveName.SelectedItem.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3) + " is " + getLeaveDays + "!";
                        return false;
                    }
                    else return true;
                }
                

               
            }
            catch { return false; }
        }

        private void updateDailyAttendance(string attDate, string InHour, string InMin, string InSec, string OutHur, string OutMin, string OutSec, string DailyStartTimeALT_CloseTime, string Status)
        {
            try
            {
                sqlDB.fillDataTable("select * from DailyAttendanceRecord where attDate='" + attDate + "'", dt = new DataTable());

                if (dt.Rows.Count > 0)
                {


                    string attStatus;
                    if (Status.Equals("Old"))
                    {
                        sqlDB.fillDataTable("select Convert(varchar(11),OffDate,105) as OffDate,Purpose from OffdaySettings where OffDate='" + attDate + "'", dt = new DataTable());
                        if (dt.Rows.Count > 0) attStatus = (dt.Rows[0]["Purpose"].ToString().Trim().Equals("Weekly Holiday")) ? "w" : "h";
                        else attStatus = "a";
                    }
                    else attStatus = "lv";

                    string stateStatus = (Status.Equals("Old")) ? "Absent" : ddlLeaveName.SelectedValue.ToString().Substring(0, ddlLeaveName.SelectedItem.ToString().Length - 3);


                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("update DailyAttendanceRecord set InHur='" + InHour + "',InMin='" + InMin + "', " +
                    "InSec='" + InSec + "',OutHur='" + OutHur + "',OutMin='" + OutMin + "',OutSec='" + OutSec + "',AttStatus='" + attStatus + "',StateStatus='" + stateStatus + "', " +
                    "AttManual='Manual Attendance',DailyStartTimeALT_CloseTime='" + DailyStartTimeALT_CloseTime + "',DId='" + ViewState["__SftId__"].ToString() + "' where EmpId=" + ViewState["EmpId"].ToString() + " and AttDate='" + attDate + "'", sqlDB.connection);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        DataTable dtNotSameDate;
        private void checkNotSameDate()
        {
            try
            {
                sqlDB.fillDataTable("select FORMAT(LeaveDate,'yyyy-MM-dd') as LeaveDate,Status from temp_NewLeaveDateForAlter where LeaveDate  not in (select LeaveDate from temp_OldLeaveDateForAlter)", dtNotSameDate = new DataTable());
                sqlDB.fillDataTable("select FORMAT(LeaveDate,'yyyy-MM-dd') as LeaveDate,Status from temp_OldLeaveDateForAlter where LeaveDate  not in (select LeaveDate from temp_NewLeaveDateForAlter)", dtNotSameDate);
            }
            catch { }
        }
        private void calculationForAlterOperation()
        {
            try
            {
                SQLOperation.forDelete("temp_NewLeaveDateForAlter", sqlDB.connection);
                SQLOperation.forDelete("temp_OldLeaveDateForAlter", sqlDB.connection);

                DateTime NewFromDate = new DateTime(int.Parse(txtFromDate.Text.Substring(6, 4)), int.Parse(txtFromDate.Text.Substring(3, 2)), int.Parse(txtFromDate.Text.Substring(0, 2)));
                DateTime NewToDate = new DateTime(int.Parse(txtToDate.Text.Substring(6, 4)), int.Parse(txtToDate.Text.Substring(3, 2)), int.Parse(txtToDate.Text.Substring(0, 2)));



                while (NewFromDate <= NewToDate)
                {
                    string[] newFromDates = NewFromDate.ToString().Split('/');
                    string getDay = (newFromDates[1].Length == 0) ? "0" + newFromDates[1] : newFromDates[1];
                    string getMonth = (newFromDates[0].Length == 0) ? "0" + newFromDates[0] : newFromDates[0];

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("insert into temp_NewLeaveDateForAlter (LeaveDate,Status) values(@LeaveDate,@Status)", sqlDB.connection);
                    cmd.Parameters.AddWithValue("@LeaveDate", newFromDates[2].Substring(0, 4) + "-" + getMonth + "-" + getDay);
                    cmd.Parameters.AddWithValue("@Status", "New");
                    cmd.ExecuteNonQuery();

                    NewFromDate = NewFromDate.AddDays(1);
                }


                fromDates = ViewState["OldFromDate"].ToString().Split('-');
                toDates = ViewState["OldToDate"].ToString().Split('-');

                DateTime oldFromDate = new DateTime(int.Parse(fromDates[2]), int.Parse(fromDates[1]), int.Parse(fromDates[0]));
                DateTime oldToDate = new DateTime(int.Parse(toDates[2]), int.Parse(toDates[1]), int.Parse(toDates[0]));

                while (oldFromDate <= oldToDate)
                {
                    fromDates = oldFromDate.ToString().Split('/');
                    string getDay = (fromDates[1].Length == 0) ? "0" + fromDates[1] : fromDates[1];
                    string getMonth = (fromDates[0].Length == 0) ? "0" + fromDates[0] : fromDates[0];

                    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("insert into temp_OldLeaveDateForAlter (LeaveDate,Status) values(@LeaveDate,@Status)", sqlDB.connection);
                    cmd.Parameters.AddWithValue("@LeaveDate", fromDates[2].Substring(0, 4) + "-" + getMonth + "-" + getDay);
                    cmd.Parameters.AddWithValue("@Status", "Old");
                    cmd.ExecuteNonQuery();
                    oldFromDate = oldFromDate.AddDays(1);
                }
            }
            catch { }
        }
        protected void gvLeaveApplication_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
               // loadLeaveApplication();
                gvLeaveApplication.PageIndex = e.NewPageIndex;
                gvLeaveApplication.DataBind();
            }
            catch { }
        }

        protected void gvLeaveApplication_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (dtSetPrivilege.Rows.Count > 0)
                {
                    if (bool.Parse(dtSetPrivilege.Rows[0]["__DeletAction__"].ToString()).Equals(false))
                    {
                        LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                        lnkDelete.Enabled = false;
                        lnkDelete.OnClientClick = "return false";
                        lnkDelete.ForeColor = Color.Black;
                    }

                    if (bool.Parse(dtSetPrivilege.Rows[0]["__UpdateAction__"].ToString()).Equals(false))
                    {
                        Button btn = (Button)e.Row.FindControl("btnAlter");
                        btn.Enabled = false;
                        btn.CssClass = "";
                    }

                }
                else
                {
                    if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
                    {
                        LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                        lnkDelete.Enabled = false;
                        lnkDelete.OnClientClick = "return false";
                        lnkDelete.ForeColor = Color.Black;

                        Button btn = (Button)e.Row.FindControl("btnAlter");
                        btn.Enabled = false;
                        btn.CssClass = "";
                    }
                }
                
            }
            catch { }

            try
            {

            }
            catch { }
        }

        protected void gvLeaveApplication_DataBinding(object sender, EventArgs e)
        {
            
        }

        protected void btnDateCalculation_Click(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                dateCalculation();


            }
            catch { }
        }
        private void dateCalculation()
        {
            try {
                string FromDate = txtFromDate.Text.Substring(6, 4) + "-" + txtFromDate.Text.Trim().Substring(3, 2) + "-" + txtFromDate.Text.Trim().Substring(0, 2);
                string ToDate = txtToDate.Text.Substring(6, 4) + "-" + txtToDate.Text.Trim().Substring(3, 2) + "-" + txtToDate.Text.Trim().Substring(0, 2);

                string CompanyId = (ddlBranch.SelectedIndex == 0) ? ViewState["__CompanyId__"].ToString() : ddlBranch.SelectedValue.ToString();
                sqlDB.fillDataTable("select distinct Convert(varchar(11),Offdate,105) as OffDate,Reason from v_AllOffDays where OffDate >='" + FromDate + "' AND OffDate<='" + ToDate + "'  AND CompanyId='" + CompanyId + "' ", dt = new DataTable());

                txtTotalHolydays.Text = dt.Rows.Count.ToString();

                sqlDB.fillDataTable("select DateDiff(Day,'" + FromDate + "','" + ToDate + "') as TotalDays", dt = new DataTable());
                txtNoOfDays.Text = (int.Parse(dt.Rows[0]["TotalDays"].ToString()) + 1).ToString();
            }
            catch (Exception ex){ }
        }

        protected void ddlLeaveName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
            if (ddlLeaveName.SelectedItem.ToString().Contains("m/l"))
            {
                trpregnatn.Visible = true;
                trprasabera.Visible = true;
            }
            else
            {
                trpregnatn.Visible = false;
                trprasabera.Visible = false;
            }
        }
 
        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);      
               // classes.commonTask.LoadShift(ddlShiftName,CompanyId);
                classes.commonTask.loadDepartmentListByCompany(ddlDepartment, ddlBranch.SelectedValue);
                classes.commonTask.loadLeaveNameByCompany(ddlLeaveName, ddlBranch.SelectedValue);
                //loadLeaveApplication();
            }
            catch { }
        }

        //protected void ddlShiftName_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
        //        lblMessage.InnerText = "";
        //        lblSftTime.Text = "";
        //        string CompanyId = (ddlBranch.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlBranch.SelectedValue.ToString();
        //        sqlDB.fillDataTable("select  StartTime12Fromat,EndTime12Fomat From v_HRD_Shift where CompanyId='" + CompanyId + "' AND SftId=" + ddlShiftName.SelectedValue.ToString() + "", dt = new DataTable());
        //        if (dt.Rows.Count > 0) lblSftTime.Text = dt.Rows[0]["StartTime12Fromat"].ToString() + "-" + dt.Rows[0]["EndTime12Fomat"].ToString();

        //        loadLeaveApplication();  // load leave application
        //        classes.commonTask.LoadEmpCardNoByShift(ddlEmpCardNo, CompanyId, ddlShiftName.SelectedValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        lblMessage.InnerText = "error->"+ex.Message;
        //    }
        //}

        protected void btnComplain_Click(object sender, EventArgs e)
        {
            Session["__forCompose__"] = "No";
            Session["__ModuleType__"] = "Leave";
            Session["__PreviousPage__"] = Request.ServerVariables["HTTP_REFERER"].ToString();       
            Response.Redirect("/mail/complain.aspx");
        }


        protected void TabContainer1_ActiveTabChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
            if (TabContainer1.ActiveTabIndex == 1) loadRejected_LeaveApplication();
            else loadLeaveApplication("");
        }

        //Md.Nayem 
        //Email=xxx@gmail.com
        //purpose : To set value for leave applicaiton report

        private void viewLeaveApplication(string LaCode)
        {
            try
            {
                string getSQLCMD;
                DataTable dt = new DataTable();
                DataTable dtApprovedRejectedDate = new DataTable();
                getSQLCMD = " SELECT LACode,CompanyId,ShortName,EmpId, Format(v_Leave_LeaveApplication.FromDate,'dd-MM-yyyy') as FromDate , Format(v_Leave_LeaveApplication.ToDate,'dd-MM-yyyy') as ToDate ,v_Leave_LeaveApplication.TotalDays,v_Leave_LeaveApplication.Remarks,"
                    + " v_Leave_LeaveApplication.EmpName, v_Leave_LeaveApplication.DptName, v_Leave_LeaveApplication.DsgName, v_Leave_LeaveApplication.CompanyName, "
                    + " v_Leave_LeaveApplication.SftName, Format(v_Leave_LeaveApplication.EntryDate,'dd-MM-yyyy') as EntryDate"
                    + " FROM"
                    + " dbo.v_Leave_LeaveApplication"
                    + " where LACode=" + LaCode + "";
                sqlDB.fillDataTable(getSQLCMD, dt);
               
                

                if (dt.Rows.Count == 0)
                {
                    getSQLCMD = "SELECT LACode ,CompanyId,ShortName,EmpId, Format(FromDate,'dd-MM-yyyy') as FromDate , Format(ToDate,'dd-MM-yyyy') as ToDate ,TotalDays,Remarks,EmpName,DptName,DsgName,CompanyName, SftName, Format(EntryDate,'dd-MM-yyyy') as EntryDate From dbo.v_Leave_LeaveApplication_Log where LACode="+LaCode+"";
                    sqlDB.fillDataTable(getSQLCMD, dt);
                
                }

                if (dt.Rows.Count == 0)
                {
                    //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "WarningMsg('Sorry any record are not founded !');", true);
                   // lblMessage.InnerText = "warning->Sorry any record are not founded"; 
                    return;

                }

                Session["__Language__"] = "English";
                Session["__LeaveApplication__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveApplication');", true);  //Open New Tab for Sever side code

            }
            catch { }
        }

        

        protected void ddlEmpCardNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
            txtCardNo.Text = ddlEmpCardNo.SelectedValue;
            findEmpName();
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
              /*//*/  ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                
               
                string CompanyId = (ddlBranch.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlBranch.SelectedValue.ToString();
                //loadLeaveApplication();  // load leave application
                classes.commonTask.LoadEmpCardNoByDepartment(ddlEmpCardNo, CompanyId, ddlDepartment.SelectedValue);
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ErrorMsg('" + ex.Message + "');", true);
              
                //lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        protected void gvRejectedList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
               
            }
            catch { }
        }

        protected void gvRejectedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                
                if (e.CommandName.Equals("View"))
                {
                    string aa = e.CommandArgument.ToString();
                    viewLeaveApplication(e.CommandArgument.ToString());
                }
            }
            catch { }
        }


        private void SaveAttachDocument(string filename) 
        {
            
            if (FileUploadDoc.HasFile)
            {
                if (txtFileName.Text.Trim().Length > 0) 
                {
                    System.IO.File.Delete(Request.PhysicalApplicationPath + "/EmployeeImages/LeaveDocument/" + txtFileName.Text.Trim()+".jpg");
                }
                FileUploadDoc.SaveAs(Server.MapPath("/EmployeeImages/LeaveDocument/" + filename+".jpg"));
              
            }
        }

        protected void gvLeaveList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("Delete"))
                {
                    int index = Convert.ToInt32(e.CommandArgument.ToString());
                    int LACode = int.Parse(gvLeaveList.DataKeys[index].Value.ToString());      
                    Delete(LACode);
                    gvLeaveList.Rows[index].Visible = false;


                }                
                if (e.CommandName.Equals("View"))
                    viewLeaveApplication(e.CommandArgument.ToString());
            }
            catch { }
        }
        protected void gvLeaveList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                   
                    string LACode = gvLeaveList.DataKeys[e.Row.RowIndex].Value.ToString();
                    CheckBox ckbAttachment = (CheckBox)e.Row.FindControl("ckbAttachment");
                    if (File.Exists(Server.MapPath("/EmployeeImages/LeaveDocument/" + LACode + ".jpg")))
                        ckbAttachment.Checked = true;
                    else
                        ckbAttachment.Checked = false;

                }
            }
            catch { }
        }

        //public bool ThumbnailCallback()
        //{
        //    return false;
        //}




    }
}