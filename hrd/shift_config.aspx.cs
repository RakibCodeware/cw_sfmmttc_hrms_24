using adviitRuntimeScripting;
using ComplexScriptingSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;

namespace SigmaERP.hrd
{
    public partial class shift_config : System.Web.UI.Page
    {
        // permission(View=203 Add=204 Edit=205 Delete=206)
        string sqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["__ReadAction__"] = "0";
            ViewState["__WriteAction__"] = "0";
            ViewState["__UpdateAction__"] = "0";
            ViewState["__DeletAction__"] = "0";
            int[] pagePermission = { 203, 204, 205, 206 };
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);
                setPrivilege(userPagePermition);
                
            }
        }

        private void setPrivilege(int[] permission)
        {
            try
            {

                
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
               // string[] AccessPermission = new string[0];
               // AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "shift_config.aspx", ddlCompanyList, gvShiftConfigurationList, btnSave);


                if (permission.Contains(203))
                    ViewState["__ReadAction__"] = "1";
                if (permission.Contains(204))
                    ViewState["__WriteAction__"] = "1";
                if (permission.Contains(205))
                    ViewState["__UpdateAction__"] = "1";
                if (permission.Contains(206))
                    ViewState["__DeletAction__"] = "1";
                checkInitialPermission();

                txtShiftName.Focus();
                ViewState["__preRIndex__"] = "No";
                // loadShift_Config("");
               
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ddlCompanyList.SelectedValue);
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ddlCompanyList.SelectedValue);
                loadShiftConfiguration();
                // loadBreakList("");
                gvBreakTimeList.Visible = true;
            }
            catch { }

        }
        private void loadBreakList(string sftId)
        {
            if(sftId=="")
            sqlCmd = "select SL,Title,StartTime,EndTime,BreakTime,convert(bit,0) as [Set] from AttBreakTime ";
            else
            sqlCmd = "select ab.SL,ab.Title,ab.StartTime,ab.EndTime,ab.BreakTime,convert(bit,case when abs.sl is null then 0 else 1 end) as [Set] from AttBreakTime ab left join AttBreakTimeWithShift abs on ab.SL= abs.BrkID and abs.SftId=" + sftId ;
            DataTable dt = new DataTable();
            sqlDB.fillDataTable(sqlCmd,dt);
            gvBreakTimeList.DataSource = dt;
            gvBreakTimeList.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Text.Trim().Equals("Save")) SaveShift_Config();
                else UpdateShiftConfig();
 
            }
            catch
            {
               // loadShift_Config("");
            }
        }
        private void SaveShift_Config()
        {
            try
            {
                //string getEffectiveDate = ""; //(txtEffectiveDate.Text.Trim().Length==0)?convertDateTime.getCertainCulture(DateTime.Now.ToString("dd-MM-yyyy")).ToString():convertDateTime.getCertainCulture(txtEffectiveDate.Text).ToString();

                string StartTime = "", EndTime = "", StartPunchCountTime = "", EndPunchCountTime = "", BreakStartTime = "", BreakEndTime = "";
                string originalStartTime = txtStartTimeHH.Text + ":" + txtStartTimeMM.Text + " " + ddlStartTimeAMPM.SelectedValue.ToString();
                DateTime dt;
                if (DateTime.TryParse(originalStartTime, out dt))
                    StartTime = dt.ToString("HH:mm");

                string originalEndTime = txtEndTimeHH.Text + ":" + txtEndTimeMM.Text + " " + ddlEndTimeAMPM.SelectedValue.ToString();                
                DateTime dt1;
                if (DateTime.TryParse(originalEndTime, out dt1))
                    EndTime = dt1.ToString("HH:mm");

                string PunchCountTime = txtPunchCountHH.Text + ":" + txtPunchCountMM.Text + " " + ddlPunchCountAMPM.SelectedValue.ToString(); 
                DateTime dt2;
                if (DateTime.TryParse(PunchCountTime,out dt2))
                    StartPunchCountTime = dt2.ToString("HH:mm");

                string _PunchCountTime = txtEndPunchCountHH.Text + ":" + txtEndPunchCountMM.Text + " " + ddlEndPunchCountAMPM.SelectedValue.ToString();
                DateTime dt3;
                if (DateTime.TryParse(_PunchCountTime, out dt3))
                    EndPunchCountTime = dt3.ToString("HH:mm");

                if (DateTime.TryParse(txtBreakSHour.Text.Trim() + ":" + txtBreakSMinute.Text.Trim() + " " + ddlBreakStartTime.SelectedValue, out dt))
                    BreakStartTime = dt.ToString("HH:mm");
                if (DateTime.TryParse(txtBreakEndHour.Text.Trim() + ":" + txtBreakEndMinute.Text.Trim() + " " + ddlBreakEndTime.SelectedValue, out dt))
                    BreakEndTime = dt.ToString("HH:mm");

                string[] getColumns = { "SftName", "SftEffectiveDate", "SftStartTime", "StartPunchCountTime","EndPunchCountTime", "SftEndTime", "SftAcceptableLate", "SftOverTime", "IsActive", "Notes", "AcceptableTimeAsOT", "CompanyId", "SftStartTimeIndicator", "SftEndTimeIndicator", "IsInitial", "DptId", "BreakStartTime", "BreakEndTime" , "IsNight", "IsNightShiftForReport" };
                string[] getValues = { txtShiftName.Text.Trim(),DateTime.Now.ToString("yyyy-MM-dd"), StartTime,StartPunchCountTime,EndPunchCountTime, EndTime, txtAcceptableLate.Text,
                                         rblOverTime.SelectedValue.ToString(), rblActiveInactive.SelectedValue.ToString(), txtNotes.Text.Trim(), 
                                         
                                     txtAcceptableOTMin.Text.Trim(),ViewState["__CompanyId__"].ToString(),
                                     ddlStartTimeAMPM.SelectedValue.ToString(),ddlEndTimeAMPM.SelectedValue.ToString(),chkIsInitial.Checked.ToString(),ddlDepartmentList.SelectedValue,BreakStartTime,BreakEndTime,chkIsNight.Checked.ToString(),chkIsNightForReport.Checked.ToString()};

                if (SQLOperation.forSaveValue("HRD_Shift", getColumns, getValues, sqlDB.connection) == true)
                {
                  //  saveShiftConfigDateLog(false, StartTime, EndTime);
                    lblMessage.InnerText = "success->Successfully Shift Configuration Saved";
                    loadShiftConfiguration();                    
                    AllClear();
                }
                
            } 
            catch (Exception ex)
            {
               
            }
        }
        private void UpdateShiftConfig()
        {
            try
            {
             //   string getEffectiveDate = ""; //(txtEffectiveDate.Text.Trim().Length == 0) ? convertDateTime.getCertainCulture(DateTime.Now.ToString("dd-MM-yyyy")).ToString() : convertDateTime.getCertainCulture(txtEffectiveDate.Text).ToString();

                string StartTime = "", EndTime = "", StartPunchCountTime="",EndPunchCountTime="", BreakStartTime="", BreakEndTime="";
                string originalStartTime = txtStartTimeHH.Text + ":" + txtStartTimeMM.Text + " " + ddlStartTimeAMPM.SelectedValue.ToString();
                DateTime dt;
                if (DateTime.TryParse(originalStartTime, out dt))
                    StartTime = dt.ToString("HH:mm");
                string originalEndTime = txtEndTimeHH.Text + ":" + txtEndTimeMM.Text + " " + ddlEndTimeAMPM.SelectedValue.ToString();
                DateTime dt1;
                if (DateTime.TryParse(originalEndTime, out dt1))
                    EndTime = dt1.ToString("HH:mm");

                string PunchCountTime = txtPunchCountHH.Text + ":" + txtPunchCountMM.Text + " " + ddlPunchCountAMPM.SelectedValue.ToString();
                DateTime dt2;
                if (DateTime.TryParse(PunchCountTime, out dt2))
                    StartPunchCountTime = dt2.ToString("HH:mm");

                string _PunchCountTime = txtEndPunchCountHH.Text + ":" + txtEndPunchCountMM.Text + " " + ddlEndPunchCountAMPM.SelectedValue.ToString();
                DateTime dt3;
                if (DateTime.TryParse(_PunchCountTime, out dt3))
                    EndPunchCountTime = dt3.ToString("HH:mm");

                if (DateTime.TryParse(txtBreakSHour.Text.Trim() + ":" + txtBreakSMinute.Text.Trim() + " " + ddlBreakStartTime.SelectedValue, out dt))
                    BreakStartTime = dt.ToString("HH:mm");
                if (DateTime.TryParse(txtBreakEndHour.Text.Trim() + ":" + txtBreakEndMinute.Text.Trim() + " " + ddlBreakEndTime.SelectedValue, out dt))
                    BreakEndTime = dt.ToString("HH:mm");


                string[] getColumns = { "SftName", "SftEffectiveDate", "SftStartTime", "StartPunchCountTime", "EndPunchCountTime", "SftEndTime", "SftAcceptableLate", "SftOverTime", "IsActive", "Notes", "AcceptableTimeAsOT", "SftStartTimeIndicator", "SftEndTimeIndicator", "IsInitial", "DptId", "BreakStartTime", "BreakEndTime", "IsNight", "IsNightShiftForReport" };
                string[] getValues = { txtShiftName.Text.Trim(),DateTime.Now.ToString("yyyy-MM-dd"), StartTime,StartPunchCountTime,EndPunchCountTime, EndTime, txtAcceptableLate.Text, 
                                         rblOverTime.SelectedValue.ToString(), rblActiveInactive.SelectedValue.ToString(), txtNotes.Text.Trim(),
                                     txtAcceptableOTMin.Text.Trim(),
                                     ddlStartTimeAMPM.SelectedValue.ToString(),ddlEndTimeAMPM.SelectedValue.ToString(),chkIsInitial.Checked.ToString(),ddlDepartmentList.SelectedValue,BreakStartTime,BreakEndTime,chkIsNight.Checked.ToString(),chkIsNightForReport.Checked.ToString()};

                if (SQLOperation.forUpdateValue("HRD_Shift", getColumns, getValues, "SftId", ViewState["__getSftId__"].ToString(), sqlDB.connection) == true)
                {
                    // saveShiftConfigDateLog(true, StartTime, EndTime);
                    setBreakTime(ViewState["__getSftId__"].ToString());
                    lblMessage.InnerText = "success->Successfully Shift Configuration Updated";
                    loadShiftConfiguration();
                    AllClear();
                }
              
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = ex.Message;
                
            }
        }

        private void saveShiftConfigDateLog(bool isUpdateTime,string StartTime,string EndTime)
        {
            try
            {
                //if (isUpdateTime)
                //{
                //    string[] fromdates = ViewState["__FDate__"].ToString().Split('-');
                //    ViewState["__FDate__"] = fromdates[2] + "-" + fromdates[1] + "-" + fromdates[0];
                //    string[] todates = ViewState["__TDate__"].ToString().Split('-');
                //    ViewState["__TDate__"] = todates[2] + "-" + todates[1] + "-" + todates[0];

                //    System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand("delete from HRD_Shift_DateLog where  SftId=" + ViewState["__getSftId__"].ToString() + " AND FromDate='" + ViewState["__FDate__"].ToString() + "' AND ToDate='" + ViewState["__TDate__"].ToString() + "'", sqlDB.connection);
                //    cmd.ExecuteNonQuery();

                //}
                DataTable dt;
                sqlDB.fillDataTable("select SftId from HRD_Shift where SftName='" + txtShiftName.Text.Trim() + "' AND CompanyId='" + ViewState["__CompanyId__"].ToString() + "'", dt = new DataTable());
                string[] getColumns = { "CompanyId", "SftId", "FromDate", "ToDate", "SftStartTime", "SftEndTime" };
                string[] getValues = {ViewState["__CompanyId__"] .ToString(),dt.Rows[0]["SftId"].ToString(),convertDateTime.getCertainCulture(txtFromDate.Text.Trim()).ToString(),
                                     convertDateTime.getCertainCulture(txtToDate.Text.Trim()).ToString(),StartTime,EndTime};
                SQLOperation.forSaveValue("HRD_Shift_DateLog", getColumns, getValues, sqlDB.connection);
               
            }
            catch (Exception ex)
            {
                //   MessageBox.Show(ex.Message);
            }
        }

        private void AllClear()
        {
            try
            {
                loadBreakList("");
                gvBreakTimeList.Visible = false;
                hdnbtnStage.Value = "";
                hdnUpdate.Value = "";
               
                txtAcceptableLate.Text = "";
             
                txtShiftName.Text = "";
                txtShiftName.Focus();
                txtStartTimeHH.Text = "00";
                txtStartTimeHH.Text = "00";
                txtEndTimeHH.Text = "00";
                txtEndTimeHH.Text = "00";
                txtPunchCountHH.Text = "00";
                txtPunchCountMM.Text = "00";
                txtEndPunchCountHH.Text = "00";
                txtEndPunchCountMM.Text = "00";
                txtBreakSHour.Text = "00";
                txtBreakSMinute.Text = "00";
                txtBreakEndHour.Text = "00";
                txtBreakEndMinute.Text = "00";
                txtAcceptableOTMin.Text = "";
                txtFromDate.Text = "";
                txtToDate.Text = "";
                if (ViewState["__WriteAction__"].ToString().Equals("0"))
                {

                    btnSave.Enabled = false;
                    btnSave.CssClass = "";
                }
                else
                {
                    btnSave.Enabled = true;
                    btnSave.CssClass = "Rbutton";
                }
                btnSave.Text = "Save";
                txtNotes.Text = "";
                chkIsInitial.Checked = false;
                chkIsNight .Checked = false;
                chkIsNightForReport.Checked = false;
                gvShiftConfigurationList.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            }
            catch { }
        }

      
        private void loadShiftConfiguration()
        {
            try
            {
                DataTable dt;
                //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                //{
                if (ddlDepartmentList.SelectedIndex < 1)
                    sqlCmd = " SELECT sftId,SftName,StartTime12Fromat,EndTime12Fomat,PunchCountTime12Fomat,EndPunchCountTime12Fomat,SftAcceptableLate,OTStatus,AcceptableTimeAsOT,ActiveStatus,IsInitial, " +
                    " Notes,DptId,dptName,CompanyId,Format(cast(BreakStartTime as datetime),'hh:mm tt') as BreakStartTime,Format( cast(BreakEndTime as datetime),'hh:mm tt') as BreakEndTime,IsNight,isnull(IsNightShiftForReport,0) as IsNightShiftForReport FROM v_HRD_Shift group by  sftId,SftName,StartTime12Fromat,EndTime12Fomat,PunchCountTime12Fomat,EndPunchCountTime12Fomat,SftAcceptableLate,OTStatus,AcceptableTimeAsOT,ActiveStatus,IsInitial," +
                    "Notes,DptId,dptName,CompanyId,BreakStartTime,BreakEndTime,IsNight,IsNightShiftForReport having CompanyId='" + ddlCompanyList.SelectedValue + "' order by IsInitial,DptId";
                    
                else
                    sqlCmd = " SELECT sftId,SftName,StartTime12Fromat,EndTime12Fomat,PunchCountTime12Fomat,EndPunchCountTime12Fomat,SftAcceptableLate,OTStatus,AcceptableTimeAsOT,ActiveStatus,IsInitial, " +
                    " Notes,DptId,dptName,CompanyId,Format(cast(BreakStartTime as datetime),'hh:mm tt') as BreakStartTime,Format( cast(BreakEndTime as datetime),'hh:mm tt') as BreakEndTime,IsNight,isnull(IsNightShiftForReport,0) as IsNightShiftForReport FROM v_HRD_Shift group by  sftId,SftName,StartTime12Fromat,EndTime12Fomat,PunchCountTime12Fomat,EndPunchCountTime12Fomat,SftAcceptableLate,OTStatus,AcceptableTimeAsOT,ActiveStatus,IsInitial," +
                    "Notes,DptId,dptName,CompanyId,BreakStartTime,BreakEndTime,IsNight,IsNightShiftForReport having CompanyId='" + ddlCompanyList.SelectedValue + "' and DptId='" + ddlDepartmentList.SelectedValue + "' order by IsInitial,DptId";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable()); 
                gvShiftConfigurationList.DataSource = dt;
                gvShiftConfigurationList.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "ex->"+ex.Message;
            }
            
        }

        protected void gvShiftConfigurationList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                lblMessage.InnerText = "";
                if (e.CommandName.Equals("Alter"))
                {
                    if (!ViewState["__preRIndex__"].ToString().Equals("No")) gvShiftConfigurationList.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    gvShiftConfigurationList.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                    ViewState["__preRIndex__"] = rIndex;
                    setValueToControl(rIndex, gvShiftConfigurationList.DataKeys[rIndex].Values[0].ToString());
                    if (!ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Rbutton";
                    }
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Delete")
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    if (deleteValidation(gvShiftConfigurationList.DataKeys[rIndex].Values[0].ToString()))
                    {
                    SQLOperation.forDeleteRecordByIdentifier("HRD_Shift", "SftId", gvShiftConfigurationList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
                    lblMessage.InnerText = "success->Successfully Shift Configuration Deleted";
                    }
                    else
                        lblMessage.InnerText = "error->Warning! Can't delete this Shift. It is used for emplloyes.";
                   
                }
            }
            catch { }
        }
        private void deleteBreakTime(string sftId)
        {
            SQLOperation.forDeleteRecordByIdentifier("AttBreakTimeWithShift", "SftId", sftId, sqlDB.connection);
        }
        private void setBreakTime(string sftId)
        {
            deleteBreakTime(sftId);
            foreach (GridViewRow row in gvBreakTimeList.Rows)
            {
                CheckBox ckbSet = (CheckBox)row.FindControl("ckbSet");
                if (ckbSet.Checked)
                {
                    string BrkID = gvBreakTimeList.DataKeys[row.RowIndex].Values[0].ToString();
                    string[] getColumns = { "SftID", "BrkID" };
                    string[] getValues = { sftId, BrkID };
                    SQLOperation.forSaveValue("AttBreakTimeWithShift", getColumns, getValues, sqlDB.connection);
                }
            }
        }
        private void setValueToControl(int rIndex, string getSL)
        {
            try
            {
                loadBreakList(getSL);
                gvBreakTimeList.Visible = true;
                ViewState["__getSftId__"] = getSL;
                txtShiftName.Text = gvShiftConfigurationList.Rows[rIndex].Cells[1].Text.Trim();

                try
                {
                    string[] getStartTime = gvShiftConfigurationList.Rows[rIndex].Cells[3].Text.Trim().Split(':');
                    txtStartTimeHH.Text = getStartTime[0]; txtStartTimeMM.Text = getStartTime[1];
                    ddlStartTimeAMPM.SelectedValue = getStartTime[2].Substring(3, 2);

                    string[] getEndTime = gvShiftConfigurationList.Rows[rIndex].Cells[4].Text.Trim().Split(':');
                    txtEndTimeHH.Text = getEndTime[0];
                    txtEndTimeMM.Text = getEndTime[1];
                    ddlEndTimeAMPM.SelectedValue = getEndTime[2].Substring(3, 2);

                    string[] getPunchCountTime = gvShiftConfigurationList.Rows[rIndex].Cells[5].Text.Trim().Split(':');
                    txtPunchCountHH.Text = getPunchCountTime[0];
                    txtPunchCountMM.Text = getPunchCountTime[1];
                    ddlPunchCountAMPM.SelectedValue = getPunchCountTime[2].Substring(3, 2);

                    string[] getEndPunchCountTime = gvShiftConfigurationList.Rows[rIndex].Cells[6].Text.Trim().Split(':');
                    txtEndPunchCountHH.Text = getEndPunchCountTime[0];
                    txtEndPunchCountMM.Text = getEndPunchCountTime[1];
                    ddlEndPunchCountAMPM.SelectedValue = getEndPunchCountTime[2].Substring(3, 2);
                }
                catch { }
                txtAcceptableLate.Text = gvShiftConfigurationList.Rows[rIndex].Cells[7].Text.Trim();

                rblOverTime.SelectedIndex = (gvShiftConfigurationList.Rows[rIndex].Cells[8].Text.Trim().Equals("Yes")) ? 0 : 1;
                if (rblOverTime.SelectedIndex == 0) txtAcceptableOTMin.Enabled = true;
                txtAcceptableOTMin.Text = gvShiftConfigurationList.Rows[rIndex].Cells[9].Text.Trim();
                rblActiveInactive.SelectedIndex = (gvShiftConfigurationList.Rows[rIndex].Cells[10].Text.Trim().Equals("Yes")) ? 0 : 1;
                txtNotes.Text = gvShiftConfigurationList.DataKeys[rIndex].Values[1].ToString();

                

                CheckBox chk=(CheckBox)gvShiftConfigurationList.Rows[rIndex].FindControl("chkInitialShift");
               

                chkIsInitial.Checked = chk.Checked;
                CheckBox chkIsN= (CheckBox)gvShiftConfigurationList.Rows[rIndex].FindControl("chkIsNightShift");
                chkIsNight.Checked = chkIsN.Checked;
                CheckBox chkIsNR = (CheckBox)gvShiftConfigurationList.Rows[rIndex].FindControl("chkIsNightShiftForReport");
                chkIsNightForReport.Checked = chkIsNR.Checked;
                ddlDepartmentList.SelectedValue = gvShiftConfigurationList.DataKeys[rIndex].Values[2].ToString();

                string [] breakTimes= gvShiftConfigurationList.Rows[rIndex].Cells[12].Text.ToString().Split(':');
                txtBreakSHour.Text = breakTimes[0];
                breakTimes = breakTimes[1].Split(' ');
                txtBreakSMinute.Text = breakTimes[0];
                ddlBreakStartTime.SelectedValue = breakTimes[1];

                breakTimes= gvShiftConfigurationList.Rows[rIndex].Cells[13].Text.ToString().Split(':');
                txtBreakEndHour.Text= breakTimes[0];
                breakTimes = breakTimes[1].Split(' ');
                txtBreakEndMinute.Text = breakTimes[0];
                ddlBreakEndTime.SelectedValue = breakTimes[1];

            }
            catch { }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            AllClear();
        }

        protected void gvShiftConfigurationList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            loadShiftConfiguration();
        }

        protected void gvShiftConfigurationList_RowDataBound(object sender, GridViewRowEventArgs e)
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
            //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            //{
                Button lnk = new Button();
                try
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        lnk = (Button)e.Row.FindControl("lnkAlter");
                        lnk.Enabled = false;
                        lnk.ForeColor = Color.Black;
                    }

                }
                catch { }
                try
                {
                    if (ViewState["__DeletAction__"].ToString().Equals("0"))
                    {
                        lnk = (Button)e.Row.FindControl("lnkDelete");
                        lnk.Enabled = false;
                        lnk.ForeColor = Color.Black;
                        lnk.OnClientClick = "return false";
                    }

                }
                catch { }
            //}
        }

        protected void gvShiftConfigurationList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvShiftConfigurationList.PageIndex = e.NewPageIndex;
                loadShiftConfiguration();
            }
            catch { }
        }
        protected void rblOverTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblOverTime.SelectedIndex == 0)
            {
                txtAcceptableOTMin.Enabled = true;
                txtAcceptableOTMin.Focus();
            }
            else txtAcceptableOTMin.Enabled = false;
        }

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["__CompanyId__"] = ddlCompanyList.SelectedItem.Value;
            loadShiftConfiguration();
            classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ddlCompanyList.SelectedValue);
        }

       
        private bool deleteValidation(string SftId)
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("Select EmpID from Personnel_EmpCurrentStatus where SftId=" + SftId + "", dt);
            if (dt.Rows.Count > 0)
                return false;
            else return true;
        }

        protected void ddlDepartmentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadShiftConfiguration(); 
        }

        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("1"))
            {
                btnSave.Enabled = true;
                btnSave.CssClass = "Rbutton";
            }
            else
            {
                btnSave.Enabled = false;
                btnSave.CssClass = "";
            }


        }
    }
}