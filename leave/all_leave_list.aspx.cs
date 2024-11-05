using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using adviitRuntimeScripting;
using ComplexScriptingSystem;
using System.Data.SqlClient;
using System.Drawing;
using SigmaERP.classes;
using System.IO;
using SigmaERP.hrms.BLL;

namespace SigmaERP.personnel
{
    public partial class week_end_list_all : System.Web.UI.Page
    {
        //permission=309
        DataTable dt;
       static DataTable dtSetPrivilege;
        string query = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 309};
            try { 
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                    int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                    if (!userPagePermition.Any())
                        Response.Redirect(Routing.defualtUrl);
                        classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                    txtFromDate.Text = "01-" + DateTime.Now.ToString("MM-yyyy");                  
                    txtToDate.Text = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)+"-"+ DateTime.Now.ToString("MM-yyyy");
                ViewState["__LineORGroupDependency__"] = classes.commonTask.GroupORLineDependency();
                setPrivilege();
                if (ViewState["__LineORGroupDependency__"].ToString().Equals("False"))
                    classes.commonTask.LoadGrouping(ddlGrouping, ViewState["__CompanyId__"].ToString());
                SearchLeaveApplication();
                
                //ddlEmpType.Items.Insert(0, new ListItem(string.Empty, "0"));
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                    loadYear(ddlCompanyList.SelectedValue);
                    //SearchLeaveApplication();
                    if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
               
            }
            }
            catch { }
        }
 

        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                string DptId= getCookies["__DptId__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForList(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "aplication.aspx", ddlCompanyList, gvLeaveList, btnSearch);

                //ViewState["__ReadAction__"] = AccessPermission[0];
                //ViewState["__WriteAction__"] = AccessPermission[1];
                //ViewState["__UpdateAction__"] = AccessPermission[2];
                //ViewState["__DeletAction__"] = AccessPermission[3];
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ViewState["__CompanyId__"].ToString());
                //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin"))
                //{
                //    ddlDepartmentList.SelectedValue = DptId;
                //    ddlDepartmentList.Enabled = false;
                //    classes.commonTask.LoadGrouping(ddlGrouping, ViewState["__CompanyId__"].ToString(), ddlDepartmentList.SelectedValue);
                //}
      


            }
            catch { }
        }
        private void loadYear(string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select distinct year(FromDate) as FromYear from Leave_LeaveApplication Where CompanyId='"+ CompanyId + "' order by year( FromDate) DESC", dt = new DataTable());
                ddlChoseYear.DataTextField = "FromYear";
                ddlChoseYear.DataValueField = "FromYear";
                ddlChoseYear.DataSource = dt;
                ddlChoseYear.DataBind();
               // ddlChoseYear.SelectedIndex = 0;
               // ddlChoseYear.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

       

        protected void gvLeaveList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("Delete"))
                {
                    string a = e.CommandArgument.ToString();
                    Delete(int.Parse(a));
                   

                }

                if (e.CommandName.Equals("Alter"))
                {
                   
                    int index = Convert.ToInt32(e.CommandArgument);
                    string LACode = gvLeaveList.DataKeys[index].Value.ToString();
                    Response.Redirect("/leave/aplication.aspx?LC="+LACode+"");
                }
                if (e.CommandName.Equals("View"))
                    viewLeaveApplication(e.CommandArgument.ToString());
            }
            catch { }
        }

        //private void delete(string getLACode)
        //{
        //    try
        //    {
        //        if (SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplication", "LACode", getLACode, sqlDB.connection) == true)
        //        {


        //            lblMessage.Text = "Successfully Deleted.";
        //            loadALlLeaveInfo();
        //        }
        //    }
        //    catch { }
        //}

        private string getAuthorizedBy(string LaCode)
        {
            try
            {
                query = "select ua.EmpName+' ('+substring(ua.EmpCardNo,8,6)+') '+' '+FORMAT(al.DateTime,'dd-MM-yyyy hh:mm:ss tt') as AuthorizedBy from Leave_ApprovalLog al left join v_UserAccount ua on al.UserID=ua.UserId where LACode=" + LaCode + "  order by  Approval desc, SL desc";            
                sqlDB.fillDataTable(query, dt = new DataTable());
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0]["AuthorizedBy"].ToString();
                else
                    return "";

            } catch(Exception ex) { return ""; }

        }
        protected void gvLeaveList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onmouseover"] = "javascript:SetMouseOver(this)";
                    e.Row.Attributes["onmouseout"] = "javascript:SetMouseOut(this)";

                   
                    if (e.Row.Cells[7].Text == "Done") e.Row.Cells[7].ForeColor = Color.Blue;               
                    else e.Row.Cells[7].ForeColor = Color.Green;

                    string LACode = gvLeaveList.DataKeys[e.Row.RowIndex].Value.ToString();
                    Label lblAuthorizedBy = (Label)e.Row.FindControl("lblAuthorizedBy");
                    lblAuthorizedBy.Text = getAuthorizedBy(LACode);
                    CheckBox ckbAttachment = (CheckBox)e.Row.FindControl("ckbAttachment");
                    if (File.Exists(Server.MapPath("/EmployeeImages/LeaveDocument/" + LACode + ".jpg")))
                        ckbAttachment.Checked = true;
                    else
                        ckbAttachment.Checked = false;

                }
               
               
            }
            catch { }
            if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            {
                Button btn;
                try
                {
                    if (ViewState["__DeletAction__"].ToString().Equals("0"))
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnView");
                        btn.Enabled = false;
                        btn.OnClientClick = "return false";
                        btn.ForeColor = Color.Silver;
                    }

                }
                catch { }
                try
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnEdit");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;
                    }

                }
                catch { }
               
            }
        }
        private void viewLeaveApplication(string LaCode)
        {
            try
            {
                if(LeaveLibrary.viewLeaveApplication(LaCode))
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveApplication');", true);  //Open New Tab for Sever side code
                else
                    lblMessage.InnerText = "warning->No data found.";
                //string getSQLCMD;
                //DataTable dt = new DataTable();
                //DataTable dtApprovedRejectedDate = new DataTable();
                //getSQLCMD = " SELECT LACode,EmpId, format(FromDate,'dd-MM-yyyy') as FromDate, format(ToDate,'dd-MM-yyyy') as ToDate, TotalDays,"
                //    + "  Remarks, LeaveName, DsgName, DptName, CompanyName,format(EmpJoiningDate,'dd-MM-yyyy') as EmpJoiningDate, SUBSTRING(EmpCardNo,10,6) as EmpCardNo, "
                //    + " EmpName, Address, LvAddress, LvContact, CompanyId, DptId, format(ApplyDate,'dd-MM-yyyy') as ApplyDate "
                //    + " FROM"
                //    + " dbo.v_Leave_LeaveApplication"
                //    + " where LACode=" + LaCode + "";
                //sqlDB.fillDataTable(getSQLCMD, dt);
                //if (dt.Rows.Count == 0)
                //{
                //    lblMessage.InnerText = "warning->No data found."; return;
                //}
                //string[] FDate = dt.Rows[0]["FromDate"].ToString().Split('-');
                //Session["__Language__"] = "English";
                //Session["__LeaveApplication__"] = dt;
                //getSQLCMD = "with lvd as ( select CompanyId, Leaveid, ShortName,count(ShortName) as Amount,case when Sex='Male' then 'm/l'else '' end Sex " +
                //     " from v_Leave_LeaveApplicationDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsApproved=1 and LeaveDate>='" + FDate[2] + "-01-01' and LeaveDate<'" + FDate[2] + "-" + FDate[1] + "-" + FDate[0] + "'" +
                //     " group by CompanyId,Leaveid, ShortName,Sex),pcs as (select case when Sex='Male' then 'm/l'else '' end Sex,CompanyId from v_EmployeeDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsActive=1 ) ,lc as ( select * from tblLeaveConfig where CompanyId=(select CompanyId from pcs)) ,"+
                //     " la as(select  LeaveId,TotalDays from Leave_LeaveApplication where LACode=" + LaCode + ")" +
                //     " select lc.ShortName,ISNULL(lvd.Amount,0) as Amount,lc.LeaveDays,lc.CompanyId,lc.LeaveName,( lc.LeaveDays-ISNULL(lvd.Amount,0) )as Remaining,TotalDays Applied from  lc left join lvd on lc.LeaveId=lvd.LeaveId or lc.ShortName=lvd.ShortName and lc.CompanyId=lvd.CompanyId  left join la on lc.LeaveId=la.LeaveId " +
                //     " where  lc.ShortName not in('sr/l',(select   Sex from pcs))";
                //sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                //Session["__LeaveCurrentStatus__"] = dt;
                //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveApplication');", true);  //Open New Tab for Sever side code

            }
            catch { }
        }
        private void viewLeaveApplication_Rejected(string LaCode)
        {
            try
            {
                if(LeaveLibrary.viewLeaveApplication_Rejected(LaCode))
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveApplication');", true);  //Open New Tab for Sever side code
                else
                    lblMessage.InnerText = "warning->No data found.";
                //string getSQLCMD;
                //DataTable dt = new DataTable();
                //DataTable dtApprovedRejectedDate = new DataTable();
                //getSQLCMD = " SELECT LACode,EmpId, format(FromDate,'dd-MM-yyyy') as FromDate, format(ToDate,'dd-MM-yyyy') as ToDate, TotalDays,"
                //    + "  Remarks, LeaveName, DsgName, DptName, CompanyName,format(EmpJoiningDate,'dd-MM-yyyy') as EmpJoiningDate, SUBSTRING(EmpCardNo,10,6) as EmpCardNo, "
                //    + " EmpName, Address, LvAddress, LvContact, CompanyId, DptId, format(ApplyDate,'dd-MM-yyyy') as ApplyDate "
                //    + " FROM"
                //    + " dbo.v_Leave_LeaveApplication_Log"
                //    + " where LACode=" + LaCode + "";
                //sqlDB.fillDataTable(getSQLCMD, dt);
                //if (dt.Rows.Count == 0)
                //{
                //    lblMessage.InnerText = "warning->No data found."; return;
                //}
                //string[] FDate = dt.Rows[0]["FromDate"].ToString().Split('-');
                //Session["__Language__"] = "English";
                //Session["__LeaveApplication__"] = dt;
                //getSQLCMD = "with lvd as ( select CompanyId, Leaveid, ShortName,count(ShortName) as Amount,case when Sex='Male' then 'm/l'else '' end Sex " +
                //     " from v_Leave_LeaveApplicationDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsApproved=1 and LeaveDate>='" + FDate[2] + "-01-01' and LeaveDate<'" + FDate[2] + "-" + FDate[1] + "-" + FDate[0] + "'" +
                //     " group by CompanyId,Leaveid, ShortName,Sex),pcs as (select case when Sex='Male' then 'm/l'else '' end Sex,CompanyId from v_EmployeeDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsActive=1 ) ,lc as ( select * from tblLeaveConfig where CompanyId=(select CompanyId from pcs)) ," +
                //     " la as(select  LeaveId,TotalDays from Leave_LeaveApplication where LACode=" + LaCode + ")" +
                //     " select lc.ShortName,ISNULL(lvd.Amount,0) as Amount,lc.LeaveDays,lc.CompanyId,lc.LeaveName,( lc.LeaveDays-ISNULL(lvd.Amount,0) )as Remaining,TotalDays Applied from  lc left join lvd on lc.LeaveId=lvd.LeaveId or lc.ShortName=lvd.ShortName and lc.CompanyId=lvd.CompanyId  left join la on lc.LeaveId=la.LeaveId " +
                //     " where  lc.ShortName not in('sr/l',(select   Sex from pcs))";
                //sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                //Session["__LeaveCurrentStatus__"] = dt;
                //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveApplication');", true);  //Open New Tab for Sever side code

            }
            catch { }
        }

        private void Delete(int rIndex)
        {
            string LACode = gvLeaveList.DataKeys[rIndex].Value.ToString();
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("select EmpId,Convert(varchar(11),FromDate,111) as FromDate,Convert(varchar(11),ToDate,111) as ToDate,LeaveName,EmpCardNo,EmpTypeId from v_Leave_LeaveApplication where Lacode =" + LACode + "", dt);

            if (dt.Rows[0]["LeaveName"].ToString().ToLower().Equals("maternity leave") || dt.Rows[0]["LeaveName"].ToString().ToLower().Equals("m/l") || dt.Rows[0]["LeaveName"].ToString().ToLower().Equals("ml"))
            {
                changeEmpTypeOnBaseMaternityLeaveDelete(dt.Rows[0]["EmpId"].ToString());

            }
           
            if (SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplication", "LACode", LACode, sqlDB.connection) == true)
            {
                
                string getEmpTypeId = dt.Rows[0]["EmpTypeId"].ToString();
                string getEmpCardNo = dt.Rows[0]["EmpCardNo"].ToString();

                sqlDB.fillDataTable("select ATTStatus,convert(varchar(11),AttDate,111)as AttDate from tblAttendanceRecord where EmpCardNo like'%" + getEmpCardNo + "' AND EmpTypeId=" + getEmpTypeId + " AND ATTDate >='" + dt.Rows[0]["FromDate"].ToString() + "' AND AttDate <='" + dt.Rows[0]["ToDate"].ToString() + "'", dt = new DataTable());

                for (byte b = 0; b < dt.Rows.Count; b++)
                {
                    if (!dt.Rows[b]["ATTStatus"].ToString().Equals("W"))
                    {

                        SqlCommand cmd = new SqlCommand("update tblAttendanceRecord set ATTStatus='A',StateStatus=' ',DailyStartTimeALT='00:00:00:00' where EmpCardNo like'%" + getEmpCardNo + "' AND EmpTypeId=" + getEmpTypeId + " AND ATTDate ='" + dt.Rows[b]["AttDate"].ToString() + "' ", sqlDB.connection);
                        cmd.ExecuteNonQuery();
                    }

                }
                gvLeaveList.Rows[rIndex].Visible = false;
                lblMessage.InnerText = "success-> Deleted";
            }
        }

        private void changeEmpTypeOnBaseMaternityLeaveDelete(string EmpId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("update Personnel_EmpCurrentStatus set EmpStatus='1' where SN=(select Max (SN) from Personnel_EmpCurrentStatus where EmpId='" + EmpId + "' and ActiveSalary='true' and IsActive=1)", sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        protected void gvLeaveList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            SearchLeaveApplication();
            gvLeaveList.PageIndex = e.NewPageIndex;
            gvLeaveList.DataBind();
            //try
            //{
            //    SearchLeaveApplication();
            //}
            //catch { }
            //gvLeaveList.PageIndex = e.NewPageIndex;
            //Session["pageNumber"] = e.NewPageIndex;
            //gvLeaveList.DataBind();
        }

        protected void gvLeaveList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            SearchLeaveApplication();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {

                
                SearchLeaveApplication();
            }
            catch { }
        }
        private void SearchLeaveApplication()
        {
            try
            {
                if (rblStatus.SelectedValue == "Rejected")
                {
                    SearchLeaveApplicationRejected();
                    return;
                }
                

                    //if (ddlCompanyList.SelectedIndex == 0 && txtCardNo.Text.Trim().Length == 0)
                    //{
                    //    lblMessage.InnerText = "warning-> Please, Select Company Name.";
                    //    return;
                    //}

                if (txtCardNo.Text.Trim() != "")
                {
                    if (txtCardNo.Text.Length < 6)
                    { lblMessage.InnerText = "warning-> Please Type Employee Card No Minimum 6 Character!"; return; }
                }
                if (txtFromDate.Text.Trim().Length != 0 || txtToDate.Text.Trim().Length != 0)
                {
                    string[] dates = txtFromDate.Text.Trim().Split('-');
                    ViewState["__FDate__"] = dates[2] + "-" + dates[1] + "-" + dates[0];
                    dates = txtToDate.Text.Trim().Split('-');
                    ViewState["__TDate__"] = dates[2] + "-" + dates[1] + "-" + dates[0];
                    ddlChoseYear.SelectedIndex = 0;
                }
                //if (txtCardNo.Text.Trim() == "" && ddlCompanyList.Text.Trim() != ""&& (ddlDepartmentList.SelectedIndex == -1 || ddlDepartmentList.SelectedIndex == 0) && ((txtToDate.Text.Trim() != "" && txtFromDate.Text.Trim() != "") || ddlChoseYear.SelectedItem.Text.Trim() != ""))
                //{
                //    lblMessage.InnerText = "warning-> Please, Select a Department.";
                //    return;
                //}
                string query = "select LACode,SUBSTRING(EmpCardNo,8,10) +' ('+EmpProximityNo+')'  as EmpCardNo,EmpName,format(FromDate,'dd-MM-yyyy') as FromDate,format(ToDate,'dd-MM-yyyy') as ToDate,format(ApplyDate,'dd-MM-yyyy') as ApplyDate,WeekHolydayNo,TotalDays,IsApproved,CurrentProcessStatus,LeaveName,AuthorizedBy,LeaveProcessingOrder,DptName from v_Leave_LeaveApplication ";
                string condition = "";
                if(!rblEmpType.SelectedValue.Equals("All"))
                    condition = " and EmpTypeId=" + rblEmpType.SelectedValue;

                if (ddlCompanyList.SelectedItem.Text.Trim()=="")
                {
                    ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                }
                string forAllDepartment = AccessControl.getDataAccessCondition(ddlCompanyList.SelectedValue, "0");
                //0. Search by Company,Year
                if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                    {
                      query += " where " + forAllDepartment + " and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "')  and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                    }
                //1. Search by Company,Forom Date & To Date
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)
                     {

                        query += " where " + forAllDepartment + " and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "'))  and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                    }
                   
                //2. Search by Company,Year,Department
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                   {
                     
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "' and DptId='" + ddlDepartmentList.SelectedValue + "'   and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "')   and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                  }
                    
                //3. Search by Company,Forom Date & To Date,Department
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)
                    {

                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "' and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                  }
                   


                //4. Search by Company,Year,Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                    {
                        query += " where " + forAllDepartment + "  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and GId=" + ddlGrouping.SelectedValue + "   and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                    }
                    
                //5. Search by Company,From Date & To Date,Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)
                    {
                        query += " where " + forAllDepartment + "  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and GId=" + ddlGrouping.SelectedValue + "   and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                    }
                    
                //6. Search by Company,Year,Department, Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                    {
                        query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and DptId='" + ddlDepartmentList.SelectedValue + "' and GId=" + ddlGrouping.SelectedValue + "   and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                    }
                    
                //7. Search by Company,From Date & To Date,Department, Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)

                    {
                        query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and GId=" + ddlGrouping.SelectedValue + "   and  IsApproved =" + rblStatus.SelectedValue + condition + " order by FromDate desc, EmpCardNo";
                    }
    


                //8. Search by Company,Year,CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                    {
                        query += " where " + forAllDepartment + "  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                    }
                 
                //9. Search by Company,From Date & To Date,CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                    {
                        query += " where " + forAllDepartment + "  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'  and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                    }
                   
                //10. Search by Company,Year,Department, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                {
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and DptId='" + ddlDepartmentList.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                }
                    
                //11. Search by Company,From Date & To Date,Department, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                    {
                        query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                    }
                    

                //12. Search by Company,Year,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                {
                    query += " where " + forAllDepartment + "  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                }
                   
                //13. Search by Company,From Date & To Date,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                {
                    query += " where " + forAllDepartment + "  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                }
                   
                //14. Search by Company,Year,Department,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                {
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and DptId='" + ddlDepartmentList.SelectedValue + "' and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                }
                  
                //15. Search by Company,From Date & To Date,Department,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                {
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  IsApproved =" + rblStatus.SelectedValue + " order by FromDate desc, EmpCardNo";
                }
                   



                //------------------------------------------------------------------------------------------------
                sqlDB.fillDataTable(query, dt=new DataTable());   
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No data found.";
                    gvLeaveList.DataSource = null;
                    gvLeaveList.DataBind();
                    gvRejectedList.Visible = false;
                    gvLeaveList.Visible = true;
                    return;
                }
                gvLeaveList.DataSource = dt;
                gvLeaveList.DataBind();
                gvRejectedList.Visible = false;
                gvLeaveList.Visible = true;

            }
            catch { }
        }
        private void SearchLeaveApplicationRejected()
        {
            try
            {

                //if (ddlCompanyList.SelectedIndex == 0 && txtCardNo.Text.Trim().Length == 0)
                //{
                //    lblMessage.InnerText = "warning-> Please, Select Company Name.";
                //    return;
                //}
               
               
                if (txtCardNo.Text.Trim() != "")
                {
                    if (txtCardNo.Text.Length < 6)
                    { lblMessage.InnerText = "warning-> Please Type Employee Card No Minimum 6 Character!"; return; }
                }
                if (txtFromDate.Text.Trim().Length != 0 || txtToDate.Text.Trim().Length != 0)
                {
                    string[] dates = txtFromDate.Text.Trim().Split('-');
                    ViewState["__FDate__"] = dates[2] + "-" + dates[1] + "-" + dates[0];
                    dates = txtToDate.Text.Trim().Split('-');
                    ViewState["__TDate__"] = dates[2] + "-" + dates[1] + "-" + dates[0];
                    ddlChoseYear.SelectedIndex = 0;
                }
                //if (txtCardNo.Text.Trim() == "" && ddlCompanyList.Text.Trim() != ""  && (ddlDepartmentList.SelectedIndex == -1 || ddlDepartmentList.SelectedIndex == 0) && ((txtToDate.Text.Trim() != "" && txtFromDate.Text.Trim() != "") || ddlChoseYear.SelectedItem.Text.Trim() != ""))
                //{
                //    lblMessage.InnerText = "warning-> Please, Select a Department.";
                //    return;
                //}
                string query = "select LACode,SUBSTRING(EmpCardNo,8,10) as EmpCardNo,EmpName,format(FromDate,'dd-MM-yyyy') as FromDate,format(ToDate,'dd-MM-yyyy') as ToDate,format(ApplyDate,'dd-MM-yyyy') as ApplyDate,WeekHolydayNo,TotalDays,IsApproved,CurrentProcessStatus,LeaveName,AuthorizedBy,DptName from v_Leave_LeaveApplication_Log ";
                string condition = "";
                if (!rblEmpType.SelectedValue.Equals("All"))
                    condition = " and EmpTypeId=" + rblEmpType.SelectedValue;
                if (ddlCompanyList.SelectedItem.Text.Trim() == "")
                {
                    ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                }
                //0. Search by Company,Year
                if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                    query +=" where CompanyId='" + ddlCompanyList.SelectedValue + "' and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "')  and  ApprovedRejected ='" + rblStatus.SelectedValue + "' "+condition+" order by FromDate desc, EmpCardNo";
                //1. Search by Company,Forom Date & To Date
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "' and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "'))  and  ApprovedRejected ='" + rblStatus.SelectedValue + "'" + condition + " order by FromDate desc, EmpCardNo";
                //2. Search by Company,Year,Department
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and DptId='" + ddlDepartmentList.SelectedValue + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "'" + condition + " order by FromDate desc, EmpCardNo";
                //3. Search by Company,Forom Date & To Date,Department
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "' and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and  ApprovedRejected ='" + rblStatus.SelectedValue + "'" + condition + " order by FromDate desc, EmpCardNo";


                //4. Search by Company,Year,Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and GId=" + ddlGrouping.SelectedValue + "   and  ApprovedRejected ='" + rblStatus.SelectedValue + "'" + condition + " order by FromDate desc, EmpCardNo";
                //5. Search by Company,From Date & To Date,Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and GId=" + ddlGrouping.SelectedValue + "   and  ApprovedRejected ='" + rblStatus.SelectedValue + "'" + condition + " order by FromDate desc, EmpCardNo";
                //6. Search by Company,Year,Department, Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length == 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and DptId='" + ddlDepartmentList.SelectedValue + "' and GId=" + ddlGrouping.SelectedValue + "   and  ApprovedRejected ='" + rblStatus.SelectedValue + "'" + condition + " order by FromDate desc, EmpCardNo";
                //7. Search by Company,From Date & To Date,Department, Line/Group
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length == 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and GId=" + ddlGrouping.SelectedValue + "   and  ApprovedRejected ='" + rblStatus.SelectedValue + "'" + condition + " order by FromDate desc, EmpCardNo";


                //8. Search by Company,Year,CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";
                //9. Search by Company,From Date & To Date,CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'  and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";
                //10. Search by Company,Year,Department, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and DptId='" + ddlDepartmentList.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";
                //11. Search by Company,From Date & To Date,Department, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && (ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";

                //12. Search by Company,Year,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";
                //13. Search by Company,From Date & To Date,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue == "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";
                //14. Search by Company,Year,Department,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length == 0 && txtToDate.Text.Trim().Length == 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and (FromYear='" + ddlChoseYear.SelectedValue + "' OR ToYear='" + ddlChoseYear.SelectedValue + "') and DptId='" + ddlDepartmentList.SelectedValue + "' and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";
                //15. Search by Company,From Date & To Date,Department,Line/Group, CardNo
                else if (ddlCompanyList.SelectedValue != "0000" && ddlDepartmentList.SelectedValue != "0" && !(ddlGrouping.SelectedIndex == -1 || ddlGrouping.SelectedValue == "0") && txtFromDate.Text.Trim().Length != 0 && txtToDate.Text.Trim().Length != 0 && txtCardNo.Text.Trim().Length != 0)
                    query += " where CompanyId='" + ddlCompanyList.SelectedValue + "'  and( (FromDate<='" + ViewState["__FDate__"].ToString() + "' and ToDate>='" + ViewState["__TDate__"].ToString() + "') or (FromDate>='" + ViewState["__FDate__"].ToString() + "' and FromDate<='" + ViewState["__TDate__"].ToString() + "') or (ToDate>='" + ViewState["__FDate__"].ToString() + "' and ToDate<='" + ViewState["__TDate__"].ToString() + "')) and DptId='" + ddlDepartmentList.SelectedValue + "' and GId='" + ddlGrouping.SelectedValue + "' and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "'   and  ApprovedRejected ='" + rblStatus.SelectedValue + "' order by FromDate desc, EmpCardNo";

                //------------------------------------------------------------------------------------------------
                sqlDB.fillDataTable(query, dt = new DataTable());
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Sorry,data not found";
                    gvRejectedList.DataSource = null;
                    gvLeaveList.Visible = false;
                    gvRejectedList.Visible = true;
                    gvRejectedList.DataBind();
                    return;
                }
                gvRejectedList.DataSource = dt;
                gvRejectedList.DataBind();
                gvLeaveList.Visible = false;
                gvRejectedList.Visible = true;

            }
            catch { }
        }

       
     

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblMessage.InnerText = "";
                //classes.commonTask.loadDivision(ddlDepartmentList, ddlCompanyList.SelectedValue.ToString(), "Admin");
                //classes.commonTask.LoadShift(ddlShift, ddlCompanyList.SelectedValue.ToString(), "Admin");
                if (ddlCompanyList.SelectedValue == "0000")
                {
                    ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                }
               // classes.commonTask.LoadShift(ddlShift, ddlCompanyList.SelectedValue.ToString());
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ddlCompanyList.SelectedValue.ToString());
                gvLeaveList.DataSource = null;
                gvLeaveList.DataBind();
                SearchLeaveApplication();
            }
            catch { }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            allClear();
           
        }
        private void allClear() 
        {
            lblMessage.InnerText = "";
            ddlChoseYear.SelectedIndex = 0;
            //ddlCompanyList.SelectedIndex = 0;
            ddlDepartmentList.SelectedIndex = -1;
           
            txtToDate.Text = "";
            txtFromDate.Text = "";
            txtCardNo.Text = "";
        }

        protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchLeaveApplication();
           
        }

        protected void ddlDivisionName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["__LineORGroupDependency__"].ToString().Equals("True"))
            {
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue;
                classes.commonTask.LoadGrouping(ddlGrouping, CompanyId, ddlDepartmentList.SelectedValue);
            }
            //classes.commonTask.LoadInitialShiftByDepartment(ddlShift, ddlCompanyList.SelectedValue, ddlDepartmentList.SelectedValue);
            SearchLeaveApplication();
           
        }

        protected void ddlChoseYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFromDate.Text = "";
            txtToDate.Text = "";
            SearchLeaveApplication();
           
        }

        protected void ddlGrouping_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchLeaveApplication();
        }

        protected void rblStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
           
                SearchLeaveApplication();

        }

        protected void gvRejectedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                
                if (e.CommandName.Equals("View"))
                    viewLeaveApplication_Rejected(e.CommandArgument.ToString());
            }
            catch { }
        }

        protected void gvRejectedList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onmouseover"] = "javascript:SetMouseOver(this)";
                    e.Row.Attributes["onmouseout"] = "javascript:SetMouseOut(this)";
                    e.Row.Cells[7].ForeColor = Color.Red;
                }


            }
            catch { }
        }

        protected void rblEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchLeaveApplication();
        }
    }
}