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

namespace SigmaERP.attendance
{
    public partial class out_duty : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();


            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
              
                setPrivilege();                
                txtDate.Text = DateTime.Now.ToString("dd-MM-yyyy");               
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
        }

        static DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__getUserId__"] = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__EmpId__"] = getCookies["__getEmpId__"].ToString();


                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), ViewState["__getUserId__"].ToString(), ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "aplication.aspx", ddlCompanyList, btnSave);

                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];


                if (ViewState["__ReadAction__"].ToString().Equals("0"))
                {
                    gvOutDuty.Visible = false;                   
                }
                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()) == "User")
                {
                    findEmpInfo();
                    trEmpCardNo.Visible = false;
                   
                }
            //    commonTask.loadEmpCardNoByCompany(ddlAssigned, ViewState["__CompanyId__"].ToString());
                loadOutDuty(ViewState["__CompanyId__"].ToString());

            }
            catch { }
        }
        DataTable dt;
        string CompanyId;
        private void findEmpInfo() 
        {
            try
            {
               
                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()) == "User")
                {
                    CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                   
                    sqlDB.fillDataTable("select EmpId,EmpName,CompanyName,DptName,DsgName,EmpTypeId,DptId,DsgId,SftId from v_Personnel_EmpCurrentStatus where EmpId='" + ViewState["__EmpId__"].ToString() + "' AND IsActive='1' AND  EmpStatus in ('1','8') ",  dt = new DataTable());
                   
                    

                }
                else 
                {
                    if (txtEmpCardNo.Text.Trim().Length < 4)
                    {
                        lblMessage.InnerText = "warning-> Please type valid Card No !";
                        txtEmpCardNo.Focus();
                        return;
                    }
                    CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                    sqlDB.fillDataTable("select EmpId,EmpName,CompanyName,DptName,DsgName,EmpTypeId,DptId,DsgId,SftId from v_Personnel_EmpCurrentStatus where EmpCardNo like '%" + txtEmpCardNo.Text.Trim() + "'" +
                                 " AND IsActive='1' AND  EmpStatus in ('1','8')  " +
                                 " AND CompanyId='" + CompanyId + "' ",  dt = new DataTable());
                }
                

              
                if (dt== null || dt.Rows.Count==0) 
                {
                    lblMessage.InnerText = "error->Please type valid employee card no";
                    divFindInfo.InnerText = "";
                }
                else
                {
                    divFindInfo.Style.Add("Color", "Green");
                    divFindInfo.InnerText = "Name: " + dt.Rows[0]["EmpName"].ToString() + " , Designation : " + dt.Rows[0]["DsgName"].ToString() + " , Department: " + dt.Rows[0]["DptName"].ToString();
                    ViewState["__EmpId__"] = dt.Rows[0]["EmpId"].ToString();
                    ViewState["__DptId__"] = dt.Rows[0]["DptId"].ToString();
                    ViewState["__DsgId__"] = dt.Rows[0]["DsgId"].ToString();

                }
            }
            catch { }
        }


        protected void btnFindEmpInfo_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
            findEmpInfo();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
            if (txtDate.Text.Trim().Length < 8)
            {
                lblMessage.InnerText = "warning-> Please select date !";
                txtDate.Focus();
                return;
            }
            if (divFindInfo.InnerText == "")
            {
                lblMessage.InnerText = "warning-> Please Find any Employee by Card No ! "; btnFindEmpInfo.Focus(); return;
            }
           saveOutDuty();           
        }
        private bool Validation() 
        {
            try 
            {
                sqlDB.fillDataTable("select EmpId, case  when Status=0 then 'Pending' when Status=1 then 'Approved' when Status=2 then 'Rejected'  end as Status,Date from v_tblOutDuty where EmpId='" + ViewState["__EmpId__"].ToString() + "' and convert(varchar(10),Date,105)='" + txtDate.Text.Trim() + "'", dt = new DataTable());
                if (dt.Rows.Count > 0)
                {
                    lblMessage.InnerText = "warning-> This date already applied and " + dt.Rows[0]["Status"].ToString();
                    return false;
                }
                return true;
            }
            catch { return false; }
           
        }
        private void saveOutDuty()
        {
            try
            {
                if (!Validation()) return;// this validation use to check existing record
                string LeaveProcessingOrder = getLeaveAuthority();
                string Status ="0";
                if (LeaveProcessingOrder == "0")
                    Status = "1";
                     string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                DateTime? inTime = null;
                DateTime? outTime = null;
                if( !((txtInHur.Text.Trim()==""|| txtInHur.Text.Trim() == "0" || txtInHur.Text.Trim() == "00")&& (txtInMin.Text.Trim() == "" || txtInMin.Text.Trim() == "0" || txtInMin.Text.Trim() == "00")))
                    inTime= DateTime.Parse("2018-01-01 "+txtInHur.Text.Trim()+":"+txtInMin.Text.Trim()+":00 "+ddlInTimeAMPM.SelectedValue);
                if (!((txtOutHur.Text.Trim() == "" || txtOutHur.Text.Trim() == "0" || txtOutHur.Text.Trim() == "00") && (txtOutMin.Text.Trim() == "" || txtOutMin.Text.Trim() == "0" || txtOutMin.Text.Trim() == "00")))
                    outTime = DateTime.Parse("2018-01-01 "+txtOutHur.Text.Trim()+":"+txtOutMin.Text.Trim()+":00 "+ddlOutTimeAMPM.SelectedValue);
               

                    string[] getColumns = { "EmpId", "Date", "Type", "Remark","Place", "Status", "Processing", "AppliedBy", "AppiedDate", "DptId", "DsgId", "AssignedBy","InTime", "OutTime" };
                    string[] getValues = { ViewState["__EmpId__"].ToString(), commonTask.ddMMyyyyTo_yyyyMMdd(txtDate.Text.Trim()),
                                             rblDutyType.SelectedValue,txtPurpose.Text.Trim(),txtPlace.Text.Trim(),Status,LeaveProcessingOrder,ViewState["__getUserId__"].ToString(),
                                             DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),ViewState["__DptId__"].ToString(),ViewState["__DsgId__"].ToString(),txtAssignedBy.Text.Trim(),inTime?.ToString("HH:mm:ss"),outTime?.ToString("HH:mm:ss")};

                    if (SQLOperation.forSaveValue("tblOutDuty", getColumns, getValues, sqlDB.connection) == true)
                    {
                        loadOutDuty(CompanyId);
                        lblMessage.InnerText = "success-> Successfully saved.";

                    }

                }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
            }
        }
        private string getLeaveAuthority()
        {
            try
            {
                DataTable dtLvOrder;
                sqlDB.fillDataTable("select ISNULL(max(LvAuthorityOrder),0) LvAuthorityOrder from v_UserAccountforLeave where isLvAuthority=1 and DptId in(" + ViewState["__DptId__"].ToString() + ")", dtLvOrder = new DataTable());
                if (!dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString().Equals("0"))
                    return dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString();
                else
                {
                    sqlDB.fillDataTable("select ISNULL(max(LvAuthorityOrder),0) LvAuthorityOrder from v_UserAccountforLeave where isLvAuthority=1 and  LvOnlyDpt=0", dtLvOrder = new DataTable());
                    return dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString();
                }
            }
            catch { return "0"; }
        }
        private void loadOutDuty(string CompanyId) 
        {

            string sqlCmd = "";
            if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()) == "User")
            {
                if(txtFromDate.Text.Trim()!="" && txtToDate.Text.Trim() != "")
                        sqlCmd = "select SL,EmpId,substring(EmpCardNo,8,10)+' ('+ EmpProximityNo+')' as EmpCardNo,EmpName,DptName,DsgName,convert(varchar(10),Date,105) as Date,case  when Status=0 then 'Pending' when Status=1 then 'Approved' when Status=2 then 'Rejected'  end as Status ,Type,case when Type=0 then 'Out Duty' else 'Training' end as TypeName,Remark,CONVERT(varchar(15),InTime ,100) InTime, CONVERT(varchar(15),OutTime ,100) OutTime,Processing,AuthorizedByName+' ('+ SUBSTRING(AuthorizedByEmpCardNo,8,6)+') '+ format(AuthorizedDate ,'dd-MM-yyyy hh:mm:ss tt') as AuthorizedByName from v_tblOutDuty where EmpId='" + ViewState["__EmpId__"].ToString() + "' and CompanyId='" + CompanyId + "' and  FORMAT(Date,'dd-MM-yyyy')>='"+txtFromDate.Text.Trim()+ "' and  FORMAT(Date,'dd-MM-yyyy')<='" +txtToDate.Text.Trim() + "' order by year(Date) desc,month(Date) desc,date desc";
                else
                    sqlCmd = "select SL,EmpId,substring(EmpCardNo,8,10)+' ('+ EmpProximityNo+')' as EmpCardNo,EmpName,DptName,DsgName,convert(varchar(10),Date,105) as Date,case  when Status=0 then 'Pending' when Status=1 then 'Approved' when Status=2 then 'Rejected'  end as Status ,Type,case when Type=0 then 'Out Duty' else 'Training' end as TypeName,Remark,CONVERT(varchar(15),InTime ,100) InTime, CONVERT(varchar(15),OutTime ,100) OutTime,Processing,AuthorizedByName+' ('+ SUBSTRING(AuthorizedByEmpCardNo,8,6)+') '+ format(AuthorizedDate ,'dd-MM-yyyy hh:mm:ss tt') as AuthorizedByName from v_tblOutDuty where EmpId='" + ViewState["__EmpId__"].ToString() + "' and CompanyId='" + CompanyId + "' order by year(Date) desc,month(Date) desc,date desc";

            }

            else
            {
                if (txtFromDate.Text.Trim() != "" && txtToDate.Text.Trim() != "" && txtCardNoForSearch.Text.Trim()!= "")
                    sqlCmd = "select SL,EmpId,substring(EmpCardNo,8,10)+' ('+ EmpProximityNo+')' as EmpCardNo,EmpName,DptName,DsgName,convert(varchar(10),Date,105) as Date,case  when Status=0 then 'Pending' when Status=1 then 'Approved' when Status=2 then 'Rejected'  end as Status ,Type,case when Type=0 then 'Out Duty' else 'Training' end as TypeName,Remark,CONVERT(varchar(15),InTime ,100) InTime, CONVERT(varchar(15),OutTime ,100) OutTime,Processing,AuthorizedByName+' ('+ SUBSTRING(AuthorizedByEmpCardNo,8,6)+') '+ format(AuthorizedDate ,'dd-MM-yyyy hh:mm:ss tt') as AuthorizedByName from v_tblOutDuty where CompanyId='" + CompanyId + "' and  FORMAT(Date,'dd-MM-yyyy')>='" + txtFromDate.Text.Trim() + "' and  FORMAT(Date,'dd-MM-yyyy')<='" + txtToDate.Text.Trim() + "' and EmpCardNo like'%"+ txtCardNoForSearch.Text.Trim()+"' order by year(Date) desc,month(Date) desc,date desc";
                else if (txtFromDate.Text.Trim() != "" && txtToDate.Text.Trim() != "")
                    sqlCmd = "select SL,EmpId,substring(EmpCardNo,8,10)+' ('+ EmpProximityNo+')' as EmpCardNo,EmpName,DptName,DsgName,convert(varchar(10),Date,105) as Date,case  when Status=0 then 'Pending' when Status=1 then 'Approved' when Status=2 then 'Rejected'  end as Status ,Type,case when Type=0 then 'Out Duty' else 'Training' end as TypeName,Remark,CONVERT(varchar(15),InTime ,100) InTime, CONVERT(varchar(15),OutTime ,100) OutTime,Processing,AuthorizedByName+' ('+ SUBSTRING(AuthorizedByEmpCardNo,8,6)+') '+ format(AuthorizedDate ,'dd-MM-yyyy hh:mm:ss tt') as AuthorizedByName from v_tblOutDuty where CompanyId='" + CompanyId + "' and  FORMAT(Date,'dd-MM-yyyy')>='" + txtFromDate.Text.Trim() + "' and  FORMAT(Date,'dd-MM-yyyy')<='" + txtToDate.Text.Trim() + "'  order by year(Date) desc,month(Date) desc,date desc";               
                else if(txtCardNoForSearch .Text.Trim() != "")
                    sqlCmd = "select SL,EmpId,substring(EmpCardNo,8,10)+' ('+ EmpProximityNo+')' as EmpCardNo,EmpName,DptName,DsgName,convert(varchar(10),Date,105) as Date,case  when Status=0 then 'Pending' when Status=1 then 'Approved' when Status=2 then 'Rejected'  end as Status ,Type,case when Type=0 then 'Out Duty' else 'Training' end as TypeName,Remark,CONVERT(varchar(15),InTime ,100) InTime, CONVERT(varchar(15),OutTime ,100) OutTime,Processing,AuthorizedByName+' ('+ SUBSTRING(AuthorizedByEmpCardNo,8,6)+') '+ format(AuthorizedDate ,'dd-MM-yyyy hh:mm:ss tt') as AuthorizedByName from v_tblOutDuty where CompanyId='" + CompanyId + "' and   EmpCardNo like'%" + txtCardNoForSearch.Text.Trim() + "' order by year(Date) desc,month(Date) desc,date desc";
                else
                    sqlCmd = "select SL,EmpId,substring(EmpCardNo,8,10)+' ('+ EmpProximityNo+')' as EmpCardNo,EmpName,DptName,DsgName,convert(varchar(10),Date,105) as Date,case  when Status=0 then 'Pending' when Status=1 then 'Approved' when Status=2 then 'Rejected'  end as Status ,Type,case when Type=0 then 'Out Duty' else 'Training' end as TypeName,Remark,CONVERT(varchar(15),InTime ,100) InTime, CONVERT(varchar(15),OutTime ,100) OutTime,Processing,AuthorizedByName+' ('+ SUBSTRING(AuthorizedByEmpCardNo,8,6)+') '+ format(AuthorizedDate ,'dd-MM-yyyy hh:mm:ss tt') as AuthorizedByName from v_tblOutDuty where CompanyId='" + CompanyId + "' order by year(Date) desc,month(Date) desc,date desc";
                
            }
            sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
            gvOutDuty.DataSource = dt;
            gvOutDuty.DataBind();
                  
        }

        protected void gvOutDuty_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int rIndex = int.Parse(e.CommandArgument.ToString());
            if (e.CommandName.Equals("deleterow"))
            {
                
                if (SQLOperation.forDeleteRecordByIdentifier("tblOutDuty", "SL", gvOutDuty.DataKeys[rIndex].Value.ToString(), sqlDB.connection) == true)
                {
                    SQLOperation.forDeleteRecordByIdentifier("tblAttendanceRecord", "ODID", gvOutDuty.DataKeys[rIndex].Value.ToString(), sqlDB.connection);
                    gvOutDuty.Rows[rIndex].Visible = false;
                    lblMessage.InnerText = "success-> Successfully Deleted";

                }
            }
            else if (e.CommandName.Equals("View"))
            {
                viewOutDutyApplication(gvOutDuty.DataKeys[rIndex].Value.ToString());
            }
        }

        protected void gvOutDuty_RowDataBound(object sender, GridViewRowEventArgs e)
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
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                if (lblStatus.Text == "Approved")
                    lblStatus.ForeColor = Color.Green;
                else if (lblStatus.Text == "Rejected")
                    lblStatus.ForeColor = Color.Red;
            }
            catch { }
            if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("User") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            {
                try
                {
                    Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                    if (ViewState["__DeletAction__"].ToString().Equals("0")||lblStatus.Text!="Pending")
                    {
                        Button lnkDelete = (Button)e.Row.FindControl("btnDelete");
                        lnkDelete.Enabled = false;
                        lnkDelete.OnClientClick = "return false";
                        lnkDelete.ForeColor = Color.Silver;
                    }

                }
                catch { }
              
            }
        }

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true); 
            CompanyId = (ddlCompanyList.SelectedIndex == 0) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue;
           // commonTask.loadEmpCardNoByCompany(ddlAssigned, CompanyId);
            loadOutDuty(CompanyId);
        }
        private void viewOutDutyApplication(string ODID)
        {
            try
            {
                string getSQLCMD;
                DataTable dt = new DataTable();
                DataTable dtApprovedRejectedDate = new DataTable();
                getSQLCMD = " SELECT SL,EmpId, CompanyName, EmpName, substring(EmpCardNo,8,10) as EmpCardNo, DsgName,convert(varchar(10), Date,105) as Date, Remark, Place, DptName, AssignedBy," +
                    " Address,CONVERT(varchar(15),InTime ,100) InTime, CONVERT(varchar(15),OutTime ,100) OutTime FROM   v_tblOutDuty where SL=" + ODID;
                sqlDB.fillDataTable(getSQLCMD, dt);
                Session["__OutDutyApplication__"] = dt;               
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=OutDutyApplication');", true);  //Open New Tab for Sever side code

            }
            catch { }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
            loadOutDuty(CompanyId);
        }
    }
}