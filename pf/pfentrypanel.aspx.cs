using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.pf
{
    public partial class pfentrypanel : System.Web.UI.Page
    {
        string query = "";
        //Permission(add=366)
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
               
                sqlDB.connectionString = Glory.getConnectionString();
                sqlDB.connectDB();
                lblMessage.InnerText = "";
                int[] pagePermission = { 366 };
                if (!IsPostBack)
                {
                    ViewState["__WriteAction__"] = "0";
                    int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                    if (!userPagePermition.Any())
                        Response.Redirect(Routing.defualtUrl);
                    classes.commonTask.loadEmpTye(rblEmployeeType);
                    rblEmployeeType.SelectedValue = "1";
                    classes.commonTask.loadEmpTye(rblEmployeeType2);
                    rblEmployeeType2.SelectedValue = "1";
                    setPrivilege(userPagePermition);

                }
            }
            catch { }
           

        }
        private void setPrivilege(int[]permission)
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__preRIndex__"] = "No";
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserId__"] = getUserId;
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                //string[] AccessPermission = new string[0];
                classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
                classes.commonTask.LoadBranch(ddlCompanyList2, ViewState["__CompanyId__"].ToString());
               // AccessPermission = checkUserPrivilege.checkUserPrivilegeForpfentrypanel(getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "pfentrypanel.aspx", gvpfpendinglist, gvpflist, btnSubmit, ViewState["__CompanyId__"].ToString(), ddlCompanyList, ddlCompanyList2);

                //ViewState["__ReadAction__"] = AccessPermission[0];
                //ViewState["__UpdateAction__"] = AccessPermission[2];
                //ViewState["__DeletAction__"] = AccessPermission[3];  
                
                if(permission.Contains(366))
                    ViewState["__WriteAction__"] ="1";
                checkInitialPermission();
                    if (!classes.commonTask.HasBranch())
                {
                    ddlCompanyList.Enabled = false;
                    ddlCompanyList2.Enabled = false;
                }                    
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                ddlCompanyList2.SelectedValue = ViewState["__CompanyId__"].ToString();
                EmployeeMaturityTime();
                pfpendinglist();
            }


            catch { }

        }
        private void EmployeeMaturityTime() 
        {
            try 
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select PFStartYear from PF_CalculationSetting where CompanyId='" + ddlCompanyList.SelectedValue + "'", dt);
                if (dt.Rows.Count > 0)
                {
                    rblEmpMaturity.Items.Add(new ListItem("All", "00"));
                    rblEmpMaturity.Items.Add(new ListItem(dt.Rows[0]["PFStartYear"].ToString() + " years", dt.Rows[0]["PFStartYear"].ToString()));
                    rblEmpMaturity.Items.Add(new ListItem("Less than " + dt.Rows[0]["PFStartYear"].ToString() + " years", "0"));
                    rblEmpMaturity.SelectedIndex = 0; 
                }
            }
            catch { }
        }
        protected void hdChk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)gvpfpendinglist.HeaderRow.FindControl("hdChk");
                if (chk.Checked)
                {
                    foreach (GridViewRow row in gvpfpendinglist.Rows)
                    {
                        chk = (CheckBox)row.Cells[4].FindControl("chkStatus");
                        chk.Checked = true;

                    }
                }
                else
                {
                    foreach (GridViewRow row in gvpfpendinglist.Rows)
                    {
                        chk = (CheckBox)row.Cells[4].FindControl("chkStatus");
                        chk.Checked = false;

                    }
                }


            }
            catch { }
        }
        protected void chkStatus_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gvr = ((GridViewRow)((Control)sender).Parent.Parent);
                int index_row = gvr.RowIndex;

                CheckBox chk = (CheckBox)gvpfpendinglist.Rows[index_row].Cells[9].FindControl("chkStatus");

                byte Action = (chk.Checked) ? (byte)1 : (byte)0;

                //--for checked and select header rows----------------------------------------
                byte checkedRowsAmount = 0;
                CheckedRowsAmount(4, "chkStatus", out  checkedRowsAmount);
                chk = (CheckBox)gvpfpendinglist.HeaderRow.FindControl("hdChk");

                if (checkedRowsAmount == gvpfpendinglist.Rows.Count)
                {

                    chk.Checked = true;
                }
                else { chk.Checked = false; }
                //----------------------------------------------------------------------------
            }
            catch { }
        }
        private void CheckedRowsAmount(byte cIndex, string ControlName, out byte checkedRowsAmount)
        {
            try
            {
                byte i = 0;
                foreach (GridViewRow gvr in gvpfpendinglist.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.Cells[cIndex].FindControl(ControlName);
                    if (chk.Checked) i++;
                }
                checkedRowsAmount = i;
            }
            catch { checkedRowsAmount = 0; }
        }

        protected void gvpfpendinglist_RowDataBound(object sender, GridViewRowEventArgs e)
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
        private void pfpendinglist()
        {
            DataTable dtpf_setting = new DataTable();
            sqlDB.fillDataTable("Select EmpContribution,EmprContribution,PFStartYear from PF_CalculationSetting where CompanyId='" + ddlCompanyList.SelectedValue + "'", dtpf_setting);
            string condition = AccessControl.getDataAccessCondition(ddlCompanyList.SelectedValue,"0");
            if (dtpf_setting.Rows.Count == 0)
            {
                gvpfpendinglist.DataSource = null;
                gvpfpendinglist.DataBind();
                return;
            }
            string emp_type = " and EmpTypeID=" + rblEmployeeType.SelectedValue;
           DataTable dtpf_pendinglist = new DataTable();
            if (rblEmpMaturity.SelectedIndex == 0)
                query = "SELECT EmpId,EmpType,SubString(EmpCardNo,8,16)+' ('+EmpProximityNo+')' EmpCardNo,EmpName,EmpJoiningDate,isnull(PfOpeningBalance,0) PfOpeningBalance," + dtpf_setting.Rows[0]["EmpContribution"].ToString() + " EmpContribution,BasicSalary,round((" + dtpf_setting.Rows[0]["EmpContribution"].ToString() + "*BasicSalary)/100,0) PFAmount,convert(varchar(11),DATEADD(year," + dtpf_setting.Rows[0]["PFStartYear"].ToString() + ", EmpJoiningDate),105) as PfDate  from v_Personnel_EmpCurrentStatus where EmpStatus in(1,8) and IsActive=1 and PFMember=0 and  " + condition + " " + emp_type;
            else if (rblEmpMaturity.SelectedIndex == 1)
                query = "SELECT EmpId,EmpType,SubString(EmpCardNo,8,16)+' ('+EmpProximityNo+')' EmpCardNo,EmpName,EmpJoiningDate,isnull(PfOpeningBalance,0) PfOpeningBalance," + dtpf_setting.Rows[0]["EmpContribution"].ToString() + " EmpContribution,BasicSalary,round((" + dtpf_setting.Rows[0]["EmpContribution"].ToString() + "*BasicSalary)/100,0) PFAmount,convert(varchar(11),DATEADD(year," + dtpf_setting.Rows[0]["PFStartYear"].ToString() + ", EmpJoiningDate),105) as PfDate  from v_Personnel_EmpCurrentStatus where EmpStatus in(1,8) and IsActive=1 and PFMember=0 and  datediff(day,EmpJoiningDate,GETDATE()) / 365.2425 >=" + dtpf_setting.Rows[0]["PFStartYear"].ToString() + "  and  " + condition + " " + emp_type;
            else
                query = "SELECT EmpId,EmpType,SubString(EmpCardNo,8,16)+' ('+EmpProximityNo+')' EmpCardNo,EmpName,EmpJoiningDate,isnull(PfOpeningBalance,0) PfOpeningBalance," + dtpf_setting.Rows[0]["EmpContribution"].ToString() + " EmpContribution,BasicSalary,round((" + dtpf_setting.Rows[0]["EmpContribution"].ToString() + "*BasicSalary)/100,0) PFAmount,convert(varchar(11),DATEADD(year," + dtpf_setting.Rows[0]["PFStartYear"].ToString() + ", EmpJoiningDate),105) as PfDate  from v_Personnel_EmpCurrentStatus where EmpStatus in(1,8) and IsActive=1 and PFMember=0 and  datediff(day,EmpJoiningDate,GETDATE()) / 365.2425 <" + dtpf_setting.Rows[0]["PFStartYear"].ToString() + "  and  " + condition + " " + emp_type;

           sqlDB.fillDataTable(query, dtpf_pendinglist);
           gvpfpendinglist.DataSource = dtpf_pendinglist;
           gvpfpendinglist.DataBind();

        }

        protected void txtEmpContribution_TextChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gvr = ((GridViewRow)((Control)sender).Parent.Parent);
                int index_row = gvr.RowIndex;

                TextBox txtEmpContribution = (TextBox)gvpfpendinglist.Rows[index_row].Cells[5].FindControl("txtEmpContribution");

                Label lblbasic = (Label)gvpfpendinglist.Rows[index_row].Cells[6].FindControl("lblbasic");
                if(txtEmpContribution.Text!="")
                {
                    Label lblpfamount = (Label)gvpfpendinglist.Rows[index_row].Cells[7].FindControl("lblpfamount");
                    lblpfamount.Text = Math.Round(float.Parse(txtEmpContribution.Text) * float.Parse(lblbasic.Text) / 100,0).ToString();
                }

               

                
                //----------------------------------------------------------------------------
            }
            catch { }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int count = 0;
                foreach (GridViewRow row in gvpfpendinglist.Rows)
                {
                    CheckBox chkStatus = row.FindControl("chkStatus") as CheckBox;
                    if (chkStatus.Checked)
                    {
                        count++;
                        HiddenField EmpId = row.FindControl("hideEmpId") as HiddenField;
                        TextBox txtBalance = row.FindControl("txtBalance") as TextBox;
                        TextBox txtEmpContribution = row.FindControl("txtEmpContribution") as TextBox;
                        Label lblpfamount = row.FindControl("lblpfamount") as Label;
                        TextBox txtpfstartDate = row.FindControl("txtpfstartDate") as TextBox;
                        SqlCommand cmd = new SqlCommand("update Personnel_EmployeeInfo set PfMember=@PfMember,PfDate=@PfDate,PFAmount=@PFAmount,PfEmpContribution=@PfEmpContribution where EmpId='"+EmpId.Value.ToString()+"' ", sqlDB.connection);
                        cmd.Parameters.AddWithValue("@PfMember","True");
                        cmd.Parameters.AddWithValue("@PfDate", convertDateTime.getCertainCulture(txtpfstartDate.Text.Trim()));
                        cmd.Parameters.AddWithValue("@PFAmount", lblpfamount.Text.Trim());                        
                        cmd.Parameters.AddWithValue("@PfEmpContribution",txtEmpContribution.Text.Trim());                        
                        cmd.ExecuteNonQuery();

                        SqlCommand cmd1 = new SqlCommand("update Personnel_EmpCurrentStatus set PfDate=@PfDate,PfMember=@PfMember,PFAmount=@PFAmount,EmpContributionPer=@EmpContributionPer where IsActive=1 and EmpId='" + EmpId.Value.ToString() + "' ", sqlDB.connection);
                        cmd1.Parameters.AddWithValue("@PfMember", "True");
                        cmd1.Parameters.AddWithValue("@PfDate", convertDateTime.getCertainCulture(txtpfstartDate.Text.Trim()));
                        cmd1.Parameters.AddWithValue("@PFAmount", lblpfamount.Text.Trim());
                        cmd1.Parameters.AddWithValue("@EmpContributionPer",txtEmpContribution.Text.Trim());
                      int result=int.Parse(cmd1.ExecuteNonQuery().ToString());
                      if(result>0)
                        CRUD.Execute(@"INSERT INTO [dbo].[PF_Activity_log]
                              ([EmpId]
                              ,[PFDate]
                              ,[PFAmount]
                              ,[ActivityStatus]
                              ,[CreatedAt]
                              ,[CreatedBy])
                        VALUES
                              ('" + EmpId.Value.ToString() + @"','" + commonTask.ddMMyyyyTo_yyyyMMdd(txtpfstartDate.Text.Trim()) + @"',"+ lblpfamount.Text.Trim() + @",1
                              ,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                              ,"+ ViewState["__UserId__"] .ToString()+ ")");
                    }

                }
                if(count>0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "Messageshow('success','Successfully Submitted "+count+" Person.');", true);
                    pfpendinglist();
                }
            }
            catch { }            
        }

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            pfpendinglist();
        }

        protected void tc1_ActiveTabChanged(object sender, EventArgs e)
        {
            if (tc1.ActiveTabIndex==1)
            {
                pflist();
            }           
        }
        private void pflist()
        {
            string condition = AccessControl.getDataAccessCondition(ddlCompanyList2.SelectedValue,"0");
            DataTable dtpf_list = new DataTable();
            sqlDB.fillDataTable("SELECT ROW_NUMBER() Over (partition by DptId Order by EmpId ) As SLDpt, DptId, EmpId,EmpType,SubString(EmpCardNo,8,16)+' ('+EmpProximityNo+')' EmpCardNo,EmpName,convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,isnull(PfOpeningBalance,0) PfOpeningBalance,PfEmpContribution,BasicSalary,PFAmount,convert(varchar(11),PfDate,105) as PfDate,DptName  from v_Personnel_EmpCurrentStatus where EmpTypeId=" + rblEmployeeType2.SelectedValue+" and " + condition + " and PfMember='1' and IsActive=1 ", dtpf_list);
            gvpflist.DataSource = dtpf_list;
            string query = "SELECT ROW_NUMBER() Over (partition by DptId Order by EmpId ) As SLDpt, DptId, EmpId,EmpType,SubString(EmpCardNo,8,16)+' ('+EmpProximityNo+')' EmpCardNo,EmpName,convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,isnull(PfOpeningBalance,0) PfOpeningBalance,PfEmpContribution,BasicSalary,PFAmount,convert(varchar(11),PfDate,105) as PfDate,DptName  from v_Personnel_EmpCurrentStatus where EmpTypeId=" + rblEmployeeType2.SelectedValue + " and " + condition + " and PfMember='1' and IsActive=1";
            gvpflist.DataBind();
        }

        protected void ddlCompanyList2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pflist();
        }

        protected void rblEmpMaturity_SelectedIndexChanged(object sender, EventArgs e)
        {
            pfpendinglist();
        }

        protected void gvpflist_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try 
            {
                if (e.CommandName == "remove") 
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    string EmpId = gvpflist.DataKeys[rIndex].Values[0].ToString();
                    string PFDate = gvpflist.DataKeys[rIndex].Values[1].ToString();
                    string PFAmount = gvpflist.DataKeys[rIndex].Values[2].ToString();
                    SqlCommand cmd = new SqlCommand("update Personnel_EmpCurrentStatus set PfMember=0 where EmpId='" + EmpId + "' and IsActive='1'", sqlDB.connection);
                    SqlCommand cmd1 = new SqlCommand("update Personnel_EmployeeInfo set PfMember=0 where EmpId='" + EmpId + "' ", sqlDB.connection);
                    if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1 && int.Parse(cmd1.ExecuteNonQuery().ToString()) == 1)
                    {
                        CRUD.Execute(@"INSERT INTO [dbo].[PF_Activity_log]
                              ([EmpId]
                              ,[PFDate]
                              ,[PFAmount]
                              ,[ActivityStatus]
                              ,[CreatedAt]
                              ,[CreatedBy])
                        VALUES
                              ('" + EmpId + @"','" + commonTask.ddMMyyyyTo_yyyyMMdd(PFDate) + @"'," + PFAmount + @",0
                              ,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                              ," + ViewState["__UserId__"].ToString() + ")");
                        gvpflist.Rows[rIndex].Visible = false;
                        lblMessage.InnerText = "success-> Successfully removed form PF .";
                    }
                        
                    
                }
            }
            catch { }
        }

        protected void rblEmployeeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            pfpendinglist();
        }

        protected void rblEmployeeType2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pflist();
        }
        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("0"))
            {
                btnSubmit.Enabled = false;
                btnSubmit.CssClass = "";

            }
            else
            {
                btnSubmit.Enabled = true;
                btnSubmit.CssClass = "Pbutton";
            }
        }
    }
}