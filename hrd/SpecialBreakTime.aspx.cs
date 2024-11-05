using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrd
{
    public partial class SpecialBreakTime : System.Web.UI.Page
    {
        string sqlCmd = "";
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                setPrivilege();

            }
        }

        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__UserId__"] = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), ViewState["__UserId__"].ToString(), ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "shift_config.aspx", ddlCompanyList, gvShiftConfigurationList, btnSave);

                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];
              
                ViewState["__preRIndex__"] = "No";                
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();                
                loadShiftConfiguration();                
            }
            catch { }

        }
       
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSave.Text.Trim().Equals("Save")) SaveBreak();
                else UpdateBreak();

            }
            catch
            {
                // loadShift_Config("");
            }
        }
        private void SaveBreak()
        {
            try
            {
               
                string StartTime = "", EndTime = "", BreakTime = "";
                string originalStartTime = txtStartTimeHH.Text + ":" + txtStartTimeMM.Text + " " + ddlStartTimeAMPM.SelectedValue.ToString();
                DateTime st;
                if (DateTime.TryParse(originalStartTime, out st))
                    StartTime = st.ToString("HH:mm");

                string originalEndTime = txtEndTimeHH.Text + ":" + txtEndTimeMM.Text + " " + ddlEndTimeAMPM.SelectedValue.ToString();
                DateTime et;
                if (DateTime.TryParse(originalEndTime, out et))
                    EndTime = et.ToString("HH:mm");
                if (st > et)
                    BreakTime = (et.AddDays(1) - st).ToString();
                else
                    BreakTime = (et- st).ToString();

                if (CRUD.Execute(@"INSERT INTO[dbo].[AttSpecialBreakTime]
           ([Title]
           ,[StartTime]
           ,[EndTime]
           ,[BreakTime]
           ,[NextDay]
           ,[IsActive]
           ,[Date]
           ,[DutyType]
           ,[CompanyId]
           ,[CreatedAt]
           ,[CreatedBy])
     VALUES
           ('" + txtTitle.Text.Trim() + @"'
           ,'" + StartTime + @"'
           ,'" + EndTime + @"'
           ,'" + BreakTime + @"'
           ,'" + ckbNextDay.Checked.ToString() + @"'
           ," + rblActiveInactive.SelectedValue + @"
           ,'" + commonTask.ddMMyyyyTo_yyyyMMdd(txtDate.Text.Trim()) + @"'
           ,'" + rblDutyType.SelectedValue + @"'
           ,'" + ddlCompanyList.SelectedValue + @"'
           ,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
           ," + ViewState["__UserId__"].ToString() + ")"))
                {
                    lblMessage.InnerText = "success->Successfully Saved.";
                    loadShiftConfiguration();
                    AllClear();
                }
                                   

            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateBreak()
        {
            try
            {

                string StartTime = "", EndTime = "", BreakTime = "";
                string originalStartTime = txtStartTimeHH.Text + ":" + txtStartTimeMM.Text + " " + ddlStartTimeAMPM.SelectedValue.ToString();
                DateTime st;
                if (DateTime.TryParse(originalStartTime, out st))
                    StartTime = st.ToString("HH:mm");

                string originalEndTime = txtEndTimeHH.Text + ":" + txtEndTimeMM.Text + " " + ddlEndTimeAMPM.SelectedValue.ToString();
                DateTime et;
                if (DateTime.TryParse(originalEndTime, out et))
                    EndTime = et.ToString("HH:mm");
                if (st > et)
                    BreakTime = (et.AddDays(1) - st).ToString();
                else
                    BreakTime = (et - st).ToString();

                if (CRUD.Execute(@"UPDATE [dbo].[AttSpecialBreakTime]
   SET [Title] = '" + txtTitle.Text.Trim() + @"'
      ,[StartTime] = '" + StartTime + @"'
      ,[EndTime] ='" + EndTime + @"'
      ,[BreakTime] ='" + BreakTime + @"'
      ,[NextDay] = '" + ckbNextDay.Checked.ToString() + @"'
      ,[IsActive] =" + rblActiveInactive.SelectedValue + @"
      ,[Date] ='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtDate.Text.Trim()) + @"'
      ,[DutyType] = '" + rblDutyType.SelectedValue + @"'      
      ,[UpdatedAt] = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
      ,[UpdatedBy] = " + ViewState["__UserId__"].ToString() + @"
 WHERE SL=" + ViewState["__getSftId__"].ToString()))
                {
                    lblMessage.InnerText = "success->Successfully Updated";
                    loadShiftConfiguration();
                    AllClear();
                }


            }
            catch (Exception ex)
            {

            }
        }

        private void AllClear()
        {
            try
            {

                txtTitle.Text = "";
                txtDate.Text = "";
                ckbNextDay.Checked = false;
                txtStartTimeHH.Text = "00";
                txtStartTimeHH.Text = "00";
                txtEndTimeHH.Text = "00";
                txtEndTimeHH.Text = "00";               
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
             
                gvShiftConfigurationList.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
            }
            catch { }
        }


        private void loadShiftConfiguration()
        {
            try
            {              
             
                    sqlCmd = "  select SL ,CompanyId,Title,format( convert(datetime,convert(varchar(10),Date,120)+' '+ convert(varchar(8),StartTime)),'hh:mm:ss tt') as StartTime,format( convert(datetime,convert(varchar(10),Date,120)+' '+ convert(varchar(8),EndTime)),'hh:mm:ss tt') as EndTime,BreakTime,NextDay,case when IsActive=1 then 'Yes' else 'No' end as IsActive,convert(varchar(10),Date,105) as Date,DutyType from AttSpecialBreakTime Where CompanyId='" + ddlCompanyList.SelectedValue+ "'  order by  convert(varchar(10),Date,120) desc ";

                
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
                   
                        SQLOperation.forDeleteRecordByIdentifier("AttSpecialBreakTime", "SL", gvShiftConfigurationList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
                        lblMessage.InnerText = "success->Successfully Shift Configuration Deleted";
                    gvShiftConfigurationList.Rows[rIndex].Visible = false;

                }
            }
            catch { }
        }
        private void setValueToControl(int rIndex, string getSL)
        {
            try
            {
              
                ViewState["__getSftId__"] = getSL;
                
                    rblDutyType.SelectedValue= gvShiftConfigurationList.Rows[rIndex].Cells[0].Text.Trim();
                txtTitle.Text = gvShiftConfigurationList.Rows[rIndex].Cells[1].Text.Trim();
                txtDate.Text = gvShiftConfigurationList.Rows[rIndex].Cells[2].Text.Trim();
                    string[] getStartTime = gvShiftConfigurationList.Rows[rIndex].Cells[3].Text.Trim().Split(':');
                    txtStartTimeHH.Text = getStartTime[0]; txtStartTimeMM.Text = getStartTime[1];
                    ddlStartTimeAMPM.SelectedValue = getStartTime[2].Substring(3, 2);
                    string[] getEndTime = gvShiftConfigurationList.Rows[rIndex].Cells[4].Text.Trim().Split(':');
                    txtEndTimeHH.Text = getEndTime[0];
                    txtEndTimeMM.Text = getEndTime[1];
                    ddlEndTimeAMPM.SelectedValue = getEndTime[2].Substring(3, 2);
                ckbNextDay.Checked = ((CheckBox)gvShiftConfigurationList.Rows[rIndex].FindControl("ckbNextDay_gv")).Checked;
                rblActiveInactive.SelectedIndex = (gvShiftConfigurationList.Rows[rIndex].Cells[7].Text.Trim().Equals("Yes")) ? 0 : 1;       



                
           

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

        protected void gvShiftConfigurationList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gvShiftConfigurationList.PageIndex = e.NewPageIndex;
                loadShiftConfiguration();
            }
            catch { }
        }

        
    }
}