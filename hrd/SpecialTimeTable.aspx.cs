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

namespace SigmaERP.hrd
{
    public partial class SpecialTimeTable : System.Web.UI.Page
    {
        string sqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                commonTask.LoadEmpType(rblEmployeeType);
                setPrivilege();
            }
        }

        private void setPrivilege()
        {
            try
            {


                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserId__"] = getUserId;
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "shift_config.aspx", ddlCompanyList, gvShiftConfigurationList, btnSave);

                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];

                
                ViewState["__preRIndex__"] = "No";
                // loadShift_Config("");
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                loadShiftConfiguration();
                // loadBreakList("");
              //  gvBreakTimeList.Visible = true;
            }
            catch { }

        }
        private void loadBreakList(string sftId)
        {
            if (sftId == "")
                sqlCmd = "select SL,Title,StartTime,EndTime,BreakTime,convert(bit,0) as [Set] from AttBreakTime ";
            else
                sqlCmd = "select ab.SL,ab.Title,ab.StartTime,ab.EndTime,ab.BreakTime,convert(bit,case when abs.sl is null then 0 else 1 end) as [Set] from AttBreakTime ab left join AttBreakTimeWithShift abs on ab.SL= abs.BrkID and abs.SpecialTimetableId=" + sftId;
            DataTable dt = new DataTable();
            sqlDB.fillDataTable(sqlCmd, dt);
            gvBreakTimeList.DataSource = dt;
            gvBreakTimeList.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckValidation()) return;
                if (btnSave.Text.Trim().Equals("Save")) SaveShiftConfig();
                else UpdateShiftConfig();

            }
            catch
            {
                // loadShift_Config("");
            }
        }
        private bool CheckValidation()
        {

            
            try
            {
                if (ddlCompanyList.SelectedIndex > 1)
                {
                    lblMessage.InnerText = "warning->Please, Select Company";
                    ddlCompanyList.Focus();
                    return false;
                }
                if (txtFromDate.Text.Trim()=="")
                {
                    lblMessage.InnerText = "warning->Please, Select From Date";
                    txtFromDate.Focus();
                    return false;
                }
                if (txtToDate.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Select To Date";
                    txtToDate.Focus();
                    return false;
                }

                if (txtStartTimeHH.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Shift Start Time (HH)";
                    txtStartTimeHH.Focus();
                    return false;
                }
                if (txtStartTimeMM.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Shift Start Time (MM)";
                    txtStartTimeMM.Focus();
                    return false;
                }

                if (txtEndTimeHH.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Shift End Time (HH)";
                    txtEndTimeHH.Focus();
                    return false;
                }
                if (txtEndTimeMM.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Shift End Time (MM)";
                    txtEndTimeMM.Focus();
                    return false;
                }

                if (txtPunchCountHH.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Punch Count Start Time (HH)";
                    txtPunchCountHH.Focus();
                    return false;
                }
                if (txtPunchCountMM.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Punch Count Start Time (MM)";
                    txtPunchCountMM.Focus();
                    return false;
                }

                if (txtEndPunchCountHH.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Punch Count End Time (HH)";
                    txtEndPunchCountHH.Focus();
                    return false;
                }
                if (txtEndPunchCountMM.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Punch Count End Time (MM)";
                    txtEndPunchCountMM.Focus();
                    return false;
                }
                if (txtAcceptableLate.Text.Trim() == "")
                {
                    lblMessage.InnerText = "warning->Please, Enter Acceptable Late(Minutes)";
                    txtAcceptableLate.Focus();
                    return false;
                }
                return true;
            }
            catch (Exception ex) { return false; }
        }

        private void SaveShiftConfig()
        {
            try
            {              

                string StartTime = "", EndTime = "", StartPunchCountTime = "", EndPunchCountTime = "";
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

                sqlCmd = @"INSERT INTO [dbo].[HRD_SpecialTimetable]
           ([EmpTypeId]
           ,[SftStartTime]
           ,[StartPunchCountTime]
           ,[SftEndTime]
           ,[EndPunchCountTime]
           ,[SftAcceptableLate]
           ,[CompanyId]
           ,[StartDate]
           ,[EndDate]
           ,[CreatedAt]
           ,[CreatedBy]           
           ,[IsActive]
           ,[Purpose])
     VALUES
           ("+rblEmployeeType.SelectedValue+",'" + StartTime + "','" + StartPunchCountTime + "','" + EndTime + "','" + EndPunchCountTime + "'," + txtAcceptableLate.Text.Trim() + ",'" + ddlCompanyList.SelectedValue + "','" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + "','" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ViewState["__UserId__"].ToString() + "," + rblActiveInactive.SelectedValue + ",'" + txtNotes.Text.Trim() + "')";
                if (CRUD.Execute(sqlCmd))
                {                   
                    //setBreakTime(ViewState["__getSftId__"].ToString());
                    lblMessage.InnerText = "success->Successfully Saved.";
                    loadShiftConfiguration();
                    AllClear();
                }

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = ex.Message;

            }
        }

        private void UpdateShiftConfig()
        {
            try
            {

                string StartTime = "", EndTime = "", StartPunchCountTime = "", EndPunchCountTime = "";
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



                sqlCmd = @"UPDATE [dbo].[HRD_SpecialTimetable]
   SET [EmpTypeId]="+rblEmployeeType.SelectedValue+@"
      ,[SftStartTime] = '" + StartTime + @"'
      ,[StartPunchCountTime] = '" + StartPunchCountTime + @"'
      ,[SftEndTime] = '" + EndTime + @"'
      ,[EndPunchCountTime] = '" + EndPunchCountTime + @"'
      ,[SftAcceptableLate] = " + txtAcceptableLate.Text.Trim() + @"
      ,[CompanyId] = '" + ddlCompanyList.SelectedValue + @"'
      ,[StartDate] = '" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text.Trim()) + @"'
      ,[EndDate] ='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + @"'
      ,[Purpose] = '" + txtNotes.Text.Trim() + @"'
      ,[IsActive] = " + rblActiveInactive.SelectedValue + @"      
      ,[UpdatedAt] = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
      ,[UpdatedBy] = " + ViewState["__UserId__"].ToString() + @"
      WHERE SL=" + ViewState["__getSftId__"].ToString();       


                if (CRUD.Execute(sqlCmd))
                {
                    setBreakTime(ViewState["__getSftId__"].ToString());
                    lblMessage.InnerText = "success->Successfully Updated.";
                    loadShiftConfiguration();
                    AllClear();
                }

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = ex.Message;

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
                txtStartTimeHH.Text = "00";
                txtStartTimeHH.Text = "00";
                txtEndTimeHH.Text = "00";
                txtEndTimeHH.Text = "00";
                txtPunchCountHH.Text = "00";
                txtPunchCountMM.Text = "00";
                txtEndPunchCountHH.Text = "00";
                txtEndPunchCountMM.Text = "00";                              
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
              
                    sqlCmd = " SELECT SL,convert(varchar(10),StartDate,105) as StartDate,convert(varchar(10),EndDate,105) as EndDate,Format(cast(StartPunchCountTime as datetime),'hh:mm tt') as StartPunchCountTime,Format(cast(SftStartTime as datetime),'hh:mm tt') as SftStartTime,Format(cast(SftEndTime as datetime),'hh:mm tt') as SftEndTime,Format(cast(EndPunchCountTime as datetime),'hh:mm tt') as EndPunchCountTime,SftAcceptableLate,CompanyId,IsActive,Purpose, IsNull(stt.EmpTypeId,1) as EmpTypeId,et.EmpType FROM HRD_SpecialTimetable stt left join HRD_EmployeeType et on et.EmpTypeId=stt.EmpTypeId where CompanyId='" + ddlCompanyList.SelectedValue+"' order by EndDate desc";
                
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                gvShiftConfigurationList.DataSource = dt;
                gvShiftConfigurationList.DataBind();
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "ex->" + ex.Message;
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
                    setValueToControl(gvShiftConfigurationList.DataKeys[rIndex].Values[0].ToString());
                    if (!ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Rbutton";
                    }
                    btnSave.Text = "Update";
                }
                else if (e.CommandName == "Remove")
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                
                   SQLOperation.forDeleteRecordByIdentifier("HRD_SpecialTimetable", "SL", gvShiftConfigurationList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
                  lblMessage.InnerText = "success->Successfully Deleted.";
                    gvShiftConfigurationList.Rows[rIndex].Visible = false;

                }
            }
            catch { }
        }
        private void deleteBreakTime(string sftId)
        {
            SQLOperation.forDeleteRecordByIdentifier("AttBreakTimeWithShift", "SpecialTimetableId", sftId, sqlDB.connection);
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
                    string[] getColumns = { "SpecialTimetableId", "BrkID" };
                    string[] getValues = { sftId, BrkID };
                    SQLOperation.forSaveValue("AttBreakTimeWithShift", getColumns, getValues, sqlDB.connection);
                }
            }
        }
        private void setValueToControl(string getSL)
        {
            try
            {
                loadBreakList(getSL);
                gvBreakTimeList.Visible = true;
                ViewState["__getSftId__"] = getSL;
                DataTable dtRecord = new DataTable();
                dtRecord = CRUD.ExecuteReturnDataTable(" SELECT SL,convert(varchar(10),StartDate,105) as StartDate,convert(varchar(10),EndDate,105) as EndDate,Format(cast(StartPunchCountTime as datetime),'hh:mm tt') as StartPunchCountTime,Format(cast(SftStartTime as datetime),'hh:mm tt') as SftStartTime,Format(cast(SftEndTime as datetime),'hh:mm tt') as SftEndTime,Format(cast(EndPunchCountTime as datetime),'hh:mm tt') as EndPunchCountTime,SftAcceptableLate,CompanyId,IsActive,Purpose,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy, IsNull(EmpTypeId,1) as EmpTypeId FROM HRD_SpecialTimetable Where SL=" + getSL);
                if (dtRecord != null && dtRecord.Rows.Count > 0)
                {
                    rblEmployeeType.SelectedValue = dtRecord.Rows[0]["EmpTypeId"].ToString();
                    txtFromDate.Text = dtRecord.Rows[0]["StartDate"].ToString().Trim();
                    txtToDate.Text = dtRecord.Rows[0]["EndDate"].ToString().Trim();

                    string[] getStartTime = dtRecord.Rows[0]["SftStartTime"].ToString().Trim().Split(':');
                    txtStartTimeHH.Text = getStartTime[0]; txtStartTimeMM.Text = getStartTime[1].Substring(0,2);
                    ddlStartTimeAMPM.SelectedValue = getStartTime[1].Substring(3, 2);

                    string[] getEndTime = dtRecord.Rows[0]["SftEndTime"].ToString().Trim().Split(':');
                    txtEndTimeHH.Text = getEndTime[0];
                    txtEndTimeMM.Text = getEndTime[1].Substring(0, 2);
                    ddlEndTimeAMPM.SelectedValue = getEndTime[1].Substring(3, 2);

                    string[] getPunchCountTime = dtRecord.Rows[0]["StartPunchCountTime"].ToString().Trim().Split(':');
                    txtPunchCountHH.Text = getPunchCountTime[0];
                    txtPunchCountMM.Text = getPunchCountTime[1].Substring(0, 2);
                    ddlPunchCountAMPM.SelectedValue = getPunchCountTime[1].Substring(3, 2);

                    string[] getEndPunchCountTime = dtRecord.Rows[0]["EndPunchCountTime"].ToString().Trim().Split(':');
                    txtEndPunchCountHH.Text = getEndPunchCountTime[0];
                    txtEndPunchCountMM.Text = getEndPunchCountTime[1].Substring(0, 2);
                    ddlEndPunchCountAMPM.SelectedValue = getEndPunchCountTime[1].Substring(3, 2);

                    txtAcceptableLate.Text = dtRecord.Rows[0]["SftAcceptableLate"].ToString().Trim();
                    rblActiveInactive.SelectedIndex = (dtRecord.Rows[0]["IsActive"].ToString().Trim().Equals("True")) ? 0 : 1;
                    txtNotes.Text = dtRecord.Rows[0]["Purpose"].ToString().Trim();
                }
                else
                {
                    lblMessage.InnerText = "error-> Record Not Found!";
                }                                 

            }
            catch { }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            AllClear();
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

            if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            {
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
            }
        }      

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["__CompanyId__"] = ddlCompanyList.SelectedItem.Value;
            loadShiftConfiguration();            
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
        private bool deleteValidation(string SftId)
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("Select EmpID from Personnel_EmpCurrentStatus where SftId=" + SftId + "", dt);
            if (dt.Rows.Count > 0)
                return false;
            else return true;
        }

      
    }
}