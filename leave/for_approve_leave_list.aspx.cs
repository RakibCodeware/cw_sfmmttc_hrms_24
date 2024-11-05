using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.personnel
{
    public partial class for_approve_list : System.Web.UI.Page
    {
        //permission(View=301 Forward=302 Approve=303 Reject=304)
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["__ReadAction__"] = "0";
            ViewState["__WriteAction__"] = "0"; //forward
            ViewState["__UpdateAction__"] = "0"; //Approve
            ViewState["__DeletAction__"] = "0";  //reject

            int[] pagePermission = { 301, 302, 303, 304 };

            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                setPrivilege(userPagePermition);               
                searchLeaveApplicationForApproved();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
        }
        DataTable dtSetPrivilege;
        private void setPrivilege(int [] permission)
        {
            try
            {


                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserId__"] = getUserId;
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("Select LvOnlyDpt,LvAuthorityAction,DptId From UserAccount inner join Personnel_EmpCurrentStatus on UserAccount.EmpId=Personnel_EmpCurrentStatus.EmpId where UserAccount.UserId='"+getUserId+"' and UserAccount.isLvAuthority='1' and Personnel_EmpCurrentStatus.IsActive='1' ", dt);
                ViewState["__LvOnlyDpt__"] = dt.Rows[0]["LvOnlyDpt"].ToString();
                ViewState["__LvAuthorityAction__"] = dt.Rows[0]["LvAuthorityAction"].ToString();
                ViewState["__DptId__"] = dt.Rows[0]["DptId"].ToString();

                // below part for supper admin and master admin.there must be controll everythings.remember that super admin not seen another super admin information
                //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                //{
                if (permission.Contains(301))
                    ViewState["__ReadAction__"] = "1";
                if (permission.Contains(302))
                    ViewState["__WriteAction__"] = "1";
                if (permission.Contains(303))
                    ViewState["__UpdateAction__"] = "1";
                if (permission.Contains(304))
                    ViewState["__DeletAction__"] = "1";
                checkInitialPermission();

                classes.commonTask.LoadBranch(ddlCompanyList);
                    classes.commonTask.LoadShift(ddlShiftName, ViewState["__CompanyId__"].ToString());
                    classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ViewState["__CompanyId__"].ToString());
                    return;
                //}
                //else    // below part for admin and viewer.while admin just write info and viewer just see information.its for by default settings
                //{

                //    classes.commonTask.LoadBranch(ddlCompanyList);
                //    classes.commonTask.LoadShift(ddlShiftName, ViewState["__CompanyId__"].ToString());
                //    classes.commonTask.loadDepartmentListByCompany(ddlDepartmentList, ViewState["__CompanyId__"].ToString());

                //    if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin"))
                //    {
                //        gvForApprovedList.Visible = false; ;
                //        divElementContainer.EnableViewState = false;
                //    }

                //    //  here set privilege by setting master admin or supper admin 
                //    dtSetPrivilege = new DataTable();
                //    sqlDB.fillDataTable("select * from UserPrivilege where PageName='for_approve_leave_list.aspx' and UserId=" + getCookies["__getUserId__"].ToString() + "", dtSetPrivilege);

                //    if (dtSetPrivilege.Rows.Count > 0)
                //    {
                //        if (bool.Parse(dtSetPrivilege.Rows[0]["ReadAction"].ToString()).Equals(true))
                //        {
                //            gvForApprovedList.Visible = true;
                //            divElementContainer.EnableViewState = true;
                //        }


                //    }


                //}
 
            }
            catch { }
        }

        DataTable dtForApprovedList;
        private void searchLeaveApplicationForApproved()
        {
            try
            {
                string empType = "";
                string LvEmpType= Session["__LvEmpType__"].ToString();
                if (LvEmpType != "0")
                    empType = " and la.EmpTypeId=" + LvEmpType;
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000") || ddlCompanyList.SelectedValue.ToString().Equals("")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                dtForApprovedList = new DataTable();
                string sqlquery = "";
                //if (ddlFindingType.SelectedValue.ToString().Equals("Today"))
                //{

                //    if (ddlShiftName.SelectedIndex == 0 && ddlDepartmentList.SelectedIndex == 0)
                //        sqlDB.fillDataTable("select la.LACode,la.EmpName,la.LeaveName,Format(la.FromDate,'dd-MM-yyyy') as FromDate,Format(la.ToDate,'dd-MM-yyyy') as ToDate,la.TotalDays,Format(la.EntryDate,'dd-MM-yyyy') as EntryDate,la.EmpTypeId  from v_Leave_LeaveApplication as la inner join UserAccount on la.LeaveProcessingOrder=UserAccount.LvAuthorityOrder where la.IsApproved='false' AND la.IsProcessessed='false' AND EntryDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "' AND la.CompanyId='" + CompanyId + "' and UserAccount.UserId='" + ViewState["__UserId__"].ToString()+ "' and UserAccount.isLvAuthority='1'" + empType, dtForApprovedList = new DataTable());

                //    else if (ddlShiftName.SelectedIndex != 0 && ddlDepartmentList.SelectedIndex == 0)
                //        sqlDB.fillDataTable("select la.LACode,la.EmpName,la.LeaveName,Format(la.FromDate,'dd-MM-yyyy') as FromDate,Format(la.ToDate,'dd-MM-yyyy') as ToDate,la.TotalDays,Format(la.EntryDate,'dd-MM-yyyy') as EntryDate,la.EmpTypeId  from v_Leave_LeaveApplication as la inner join UserAccount on la.LeaveProcessingOrder=UserAccount.LvAuthorityOrder where la.IsApproved='false' AND la.IsProcessessed='false' AND EntryDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and UserAccount.UserId='" + ViewState["__UserId__"].ToString() + "' and UserAccount.isLvAuthority='1' " +
                //            " AND la.CompanyId='" + CompanyId + "' AND SftId=" + ddlShiftName.SelectedValue.ToString() + " " + empType, dtForApprovedList = new DataTable());
                //    else if (ddlShiftName.SelectedIndex != 0 && ddlDepartmentList.SelectedIndex != 0)
                //        sqlDB.fillDataTable("select la.LACode,la.EmpName,la.LeaveName,Format(la.FromDate,'dd-MM-yyyy') as FromDate,Format(la.ToDate,'dd-MM-yyyy') as ToDate,la.TotalDays,Format(la.EntryDate,'dd-MM-yyyy') as EntryDate,la.EmpTypeId  from v_Leave_LeaveApplication as la inner join UserAccount on la.LeaveProcessingOrder=UserAccount.LvAuthorityOrder where la.IsApproved='false' AND la.IsProcessessed='false' AND EntryDate='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and UserAccount.UserId='" + ViewState["__UserId__"].ToString() + "' and UserAccount.isLvAuthority='1' " +
                //            " AND la.CompanyId='" + CompanyId + "' AND SftId=" + ddlShiftName.SelectedValue.ToString() + " " +
                //            "AND DptId='" + ddlDepartmentList.SelectedValue.ToString() + "' " + empType, dtForApprovedList = new DataTable());
                //}
                //else
                //{

                //    //if (ddlShiftName.SelectedIndex == 0 && ddlDepartmentList.SelectedIndex == 0)
                //    if (ViewState["__LvOnlyDpt__"].ToString()!= "True")
                //        sqlquery= "select la.EmpCardNo,la.LACode,la.EmpName,la.LeaveName,Format(la.FromDate,'dd-MM-yyyy') as FromDate,Format(la.ToDate,'dd-MM-yyyy') as ToDate,la.TotalDays,Format(la.EntryDate,'dd-MM-yyyy') as EntryDate,la.EmpTypeId  from v_Leave_LeaveApplication as la inner join UserAccount on la.LeaveProcessingOrder=UserAccount.LvAuthorityOrder where la.IsApproved='false' AND la.IsProcessessed='false'   and UserAccount.UserId='" + ViewState["__UserId__"].ToString() + "' and UserAccount.isLvAuthority='1'" + empType;

                //    //else if (ddlShiftName.SelectedIndex != 0 && ddlDepartmentList.SelectedIndex == 0)
                //    //    sqlDB.fillDataTable("select la.LACode,la.EmpName,la.LeaveName,Format(la.FromDate,'dd-MM-yyyy') as FromDate,Format(la.ToDate,'dd-MM-yyyy') as ToDate,la.TotalDays,Format(la.EntryDate,'dd-MM-yyyy') as EntryDate  from v_Leave_LeaveApplication as la inner join UserAccount on la.LeaveProcessingOrder=UserAccount.LvAuthorityOrder where la.IsApproved='false' AND la.IsProcessessed='false' and UserAccount.UserId='" + ViewState["__UserId__"].ToString() + "' and UserAccount.isLvAuthority='1' " +
                //    //        " AND la.CompanyId='" + CompanyId + "' AND SftId=" + ddlShiftName.SelectedValue.ToString() + " ", dtForApprovedList = new DataTable());
                //    else
                //        sqlquery= "select la.EmpCardNo,la.LACode,la.EmpName,la.LeaveName,Format(la.FromDate,'dd-MM-yyyy') as FromDate,Format(la.ToDate,'dd-MM-yyyy') as ToDate,la.TotalDays,Format(la.EntryDate,'dd-MM-yyyy') as EntryDate,la.EmpTypeId  from v_Leave_LeaveApplication as la inner join UserAccount on la.LeaveProcessingOrder=UserAccount.LvAuthorityOrder where la.IsApproved='false' AND la.IsProcessessed='false' and UserAccount.UserId='" + ViewState["__UserId__"].ToString() + "' and UserAccount.isLvAuthority='1'" +
                //            " AND la.CompanyId='" + CompanyId + "' " +
                //            "AND DptId='" + ViewState["__DptId__"].ToString() + "' " + empType;
                //    sqlDB.fillDataTable(sqlquery, dtForApprovedList = new DataTable());
                //}
                string EmpType = rblEmpType.SelectedValue == "All" ? "" : " and la.EmpTypeId=" + rblEmpType.SelectedValue;
                sqlquery = "select la.EmpID,la.EmpCardNo,la.LACode,la.EmpName,la.LeaveName,Format(la.FromDate,'dd-MM-yyyy') as FromDate,Format(la.ToDate,'dd-MM-yyyy') as ToDate,la.TotalDays,Format(la.EntryDate,'dd-MM-yyyy') as EntryDate,la.EmpTypeId,la.EmpType,la.DptName,la.DsgName   from v_Leave_LeaveApplication as la inner join tblLeaveAuthorityAccessControl ac on la.LeaveProcessingOrder=ac.AuthorityPosition and la.EmpId=ac.EmpID and la.IsApproved='false' AND la.IsProcessessed='false' and la.FromDate >='2021-01-01' and ac.AuthorityID='" + ViewState["__UserId__"].ToString() + "'  AND la.CompanyId='" + CompanyId + "' "+ EmpType;
                sqlDB.fillDataTable(sqlquery, dtForApprovedList = new DataTable());
                if (dtForApprovedList.Rows ==null || dtForApprovedList.Rows.Count==0)
                {
                    gvForApprovedList.DataSource = null;
                    gvForApprovedList.DataBind();
                    //divRecordMessage.InnerText = "Any Leave Approved Request Are Not Founded !";
                    //divRecordMessage.Visible = true;
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
                int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                if (e.CommandName.Equals("Forword"))
                {
                   ViewState["__Predecate__"] = "Forword";
                   if(Forward(rIndex))
                    saveAprovalLog(rIndex,"0");
                }
               else if (e.CommandName.Equals("Yes"))
                {
                    ViewState["__Predecate__"] = "Yes";
                    if(YesApproved(rIndex))
                    saveAprovalLog(rIndex, "1");                    
                }
                else if (e.CommandName.Equals("No"))
                {
                    ViewState["__Predecate__"] = "No";
                    string EmpId = gvForApprovedList.DataKeys[rIndex].Values[2].ToString();
                    string LvAuthorityOrder = getLvAuthorityOrder(ViewState["__UserId__"].ToString(), EmpId);//getLvAuthorityOrder();
                    string LaCode = gvForApprovedList.DataKeys[rIndex].Values[0].ToString();
                    if (!IsValid(LvAuthorityOrder, LaCode))
                    {
                        Response.Redirect(Request.RawUrl);
                        return;
                    }
                    saveLeaveApplicationRequest_AsLog(rIndex);
                    saveAprovalLog(rIndex, "2");
                    // this function calling for save leave application log
                    
                   
                    
                }
                else if (e.CommandName.Equals("Alter"))
                {
                    
                    TextBox txtFromDate = (TextBox)gvForApprovedList.Rows[rIndex].Cells[2].FindControl("txtFromDate");
                    TextBox txtToDate = (TextBox)gvForApprovedList.Rows[rIndex].Cells[3].FindControl("txtToDate");
                    txtFromDate.Enabled = true;
                    txtToDate.Enabled = true;
                    txtFromDate.Style.Add("border-style","solid");
                    txtFromDate.Style.Add("border-color", "#0000ff");

                    txtToDate.Style.Add("border-style", "solid");
                    txtToDate.Style.Add("border-color", "#0000ff");
                }

                else if (e.CommandName.Equals("Calculation"))
                {
                    TextBox txtFromDate = (TextBox)gvForApprovedList.Rows[rIndex].Cells[2].FindControl("txtFromDate");
                    TextBox txtToDate = (TextBox)gvForApprovedList.Rows[rIndex].Cells[3].FindControl("txtToDate");

                    TextBox txtTotalDays = (TextBox)gvForApprovedList.Rows[rIndex].Cells[3].FindControl("txtTotalDays");
                    
                    DataTable dt = new DataTable();

                    string [] getFDays = txtFromDate.Text.Trim().Split('-');
                    string[] getTDays = txtToDate.Text.Trim().Split('-');
                    getFDays[0] = getFDays[2] + "-" + getFDays[1] + "-" + getFDays[0];
                    getTDays[0] = getTDays[2] + "-" + getTDays[1] + "-" + getTDays[0];
                     
                    string CompanyId=(ddlCompanyList.SelectedIndex==0)?ViewState["__CompanyId__"].ToString():ddlCompanyList.SelectedValue.ToString();
                   
                    sqlDB.fillDataTable("select distinct Convert(varchar(11),Offdate,105) as OffDate,Reason from v_AllOffDays where OffDate >='" + getFDays[0] + "' AND OffDate<='" + getTDays[0] + "'  AND CompanyId='" + CompanyId + "' ", dt = new DataTable());
                    ViewState["__WHnumber__"] = dt.Rows.Count;

                    sqlDB.fillDataTable("select DateDiff(Day,'" + getFDays[0] + "','" + getTDays[0] + "') as TotalDays", dt=new DataTable ());

                    txtTotalDays.Text = (int.Parse(dt.Rows[0]["TotalDays"].ToString())+1).ToString();

                }
                else if(e.CommandName.Equals("View"))
                {
                    string LaCode = gvForApprovedList.DataKeys[rIndex].Values[0].ToString();
                    viewLeaveApplication(LaCode);
                }
            }
            catch { }
        }

        

        private bool YesApproved(int rIndex)
        {
            try
            {
                string EmpId = gvForApprovedList.DataKeys[rIndex].Values[2].ToString();
                string LvAuthorityOrder = getLvAuthorityOrder(ViewState["__UserId__"].ToString(), EmpId);// getLvAuthorityOrder();
                string LaCode = gvForApprovedList.DataKeys[rIndex].Values[0].ToString();
                if (!IsValid(LvAuthorityOrder, LaCode))
                {
                    Response.Redirect(Request.RawUrl);
                    return false;
                }
             string [] getColumns={"IsApproved","ApprovedDate","IsProcessessed"};
	         string [] getValues={"1",DateTime.Now.ToString("yyyy-MM-dd"),"1"};
	         if (SQLOperation.forUpdateValue("Leave_LeaveApplication",getColumns,getValues,"LACode",LaCode,sqlDB.connection)==true)
	         {
                saveLeaveApplicationRequest_AsLog(rIndex);   // this function calling for save leave application log
                 lblMessage.InnerText = "success->" + gvForApprovedList.Rows[rIndex].Cells[2].Text.ToString() + " approved for " + gvForApprovedList.Rows[rIndex].Cells[1].Text;
                 gvForApprovedList.Rows[rIndex].Visible = false;
                    return true;
	         }
                return false;
            }
            catch{ return false; }
       
        }
        private bool IsValid(string LeaveProcessingOrder,string LACode)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select LACode from Leave_LeaveApplication where LeaveProcessingOrder="+ LeaveProcessingOrder + " and  IsApproved=0 and LACode="+ LACode, dt);
                if (dt != null && dt.Rows.Count > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private string getLvAuthorityOrder()
        {
            try {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("Select LvAuthorityOrder FROM UserAccount where UserId ='" + ViewState["__UserId__"].ToString() + "'", dt);
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0]["LvAuthorityOrder"].ToString();
                return "";
            } catch (Exception ex){ return ""; }
        }
        private string getLvAuthorityOrder(string AuthorityID,string EmpID)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select AuthorityPosition from tblLeaveAuthorityAccessControl where AuthorityID="+ AuthorityID + " and EmpID="+ EmpID, dt);
                if (dt != null && dt.Rows.Count > 0)
                    return dt.Rows[0]["AuthorityPosition"].ToString();
                return "";
            }
            catch (Exception ex) { return ""; }
        }
        private bool Forward(int rIndex)
        {
            try
            {
                string EmpId = gvForApprovedList.DataKeys[rIndex].Values[2].ToString();
                string LvAuthorityOrder = getLvAuthorityOrder(ViewState["__UserId__"].ToString(), EmpId);// getLvAuthorityOrder();
                string LaCode = gvForApprovedList.DataKeys[rIndex].Values[0].ToString();                
                if (!IsValid(LvAuthorityOrder, LaCode))
                {
                    Response.Redirect(Request.RawUrl);
                    return false;
                }                
                string AuthorityPosition = getLeaveAuthority(ViewState["__UserId__"].ToString(), EmpId);
                if (AuthorityPosition.Trim() == "") // Leave Authority is not found. plz setup for this employee.
                {
                    lblMessage.InnerText = "warning->The higher authority is not found.";
                    return false;                    // 
                }

                string empType = gvForApprovedList.DataKeys[rIndex].Values[1].ToString();
                //string lvaorder = getLeaveAuthority(int.Parse(LvAuthorityOrder), empType);                
                string lvaorder = AuthorityPosition;
                string[] getColumns = { "LeaveProcessingOrder" };
                string[] getValues = {lvaorder};
                if (SQLOperation.forUpdateValue("Leave_LeaveApplication", getColumns, getValues, "LACode", LaCode, sqlDB.connection) == true)
                {
                   // saveLeaveApplicationRequest_AsLog(rIndex);   // this function calling for save leave application log
                    lblMessage.InnerText = "success->" + gvForApprovedList.Rows[rIndex].Cells[2].Text.ToString() + " approved for " + gvForApprovedList.Rows[rIndex].Cells[1].Text;
                    gvForApprovedList.Rows[rIndex].Visible = false;
                    return true;
                }
                return false;
            }
            catch { return false; }

        }
        //private string getLeaveAuthority(int lvorder, string empType)
        //{
        //    try
        //    {
        //        DataTable dtLvOrder;
        //        sqlDB.fillDataTable("select ISNULL(max(LvAuthorityOrder),0) LvAuthorityOrder from v_UserAccount where DptId in(" + ViewState["__DptId__"].ToString() + ") and LvOnlyDpt=1  and isLvAuthority=1 and LvEmpType in(0," + empType + ") and LvAuthorityOrder<" + lvorder+" and status=1", dtLvOrder = new DataTable());
        //        if (!dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString().Equals("0"))
        //            return dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString();
        //        else
        //        {
        //            sqlDB.fillDataTable("select ISNULL(max(LvAuthorityOrder),0) LvAuthorityOrder from v_UserAccount where  LvOnlyDpt=0 and LvAuthorityOrder<" + lvorder + " and LvEmpType in(0," + empType + ") and status=1", dtLvOrder = new DataTable());
        //            return dtLvOrder.Rows[0]["LvAuthorityOrder"].ToString();
        //        }
        //    }
        //    catch { return "0"; }
        //}
        private string getLeaveAuthority(string AuthorityID, string EmpID)
        {         

            try
            {
                DataTable dtLvOrder;
                string SqlCmd = "select max(AuthorityPosition) as AuthorityPosition from tblLeaveAuthorityAccessControl where EmpID=" + EmpID + " and AuthorityPosition<(select AuthorityPosition from tblLeaveAuthorityAccessControl where EmpID=" + EmpID + " and AuthorityID= " + AuthorityID + ")";
                sqlDB.fillDataTable(SqlCmd, dtLvOrder = new DataTable());
                if (dtLvOrder == null || dtLvOrder.Rows.Count == 0)
                    return "";
                return dtLvOrder.Rows[0]["AuthorityPosition"].ToString();
            }
            catch (Exception ex) { return ""; }
        }
        private bool checkLeaveAuthority(int rIndex, string AuthorityAction)
        {
            try
            {
                string EmpId = gvForApprovedList.DataKeys[rIndex].Values[2].ToString();
                DataTable dtLvAuth;
                string SqlCmd = "select * from tblLeaveAuthorityAccessControl where AuthorityID="+ ViewState["__UserId__"].ToString() + " and AuthorityAction in ("+ AuthorityAction + ") and EmpID='"+ EmpId + "'";
                sqlDB.fillDataTable(SqlCmd, dtLvAuth = new DataTable());
                if (dtLvAuth == null || dtLvAuth.Rows.Count == 0)
                    return false;
                return true;
            }
            catch (Exception ex) { return false; }
        }

        private void NoApproved(int rIndex)
        {
            try
            {
              

                if (SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplication", "LACode", gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection))
                {
                    SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplicationDetails", "LACode", gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection); // for clear all leave details

                    SqlCommand cmd = new SqlCommand("update Leave_LeaveApplication_Log set ApprovedDate ='" + DateTime.Now.ToString("yyyy-MM-dd") + "' where LACode=" + gvForApprovedList.DataKeys[rIndex].Values[0].ToString() + "", sqlDB.connection);
                    cmd.ExecuteNonQuery();
                    lblMessage.InnerText = "success->" + gvForApprovedList.Rows[rIndex].Cells[2].Text.ToString() + " not approved for " + gvForApprovedList.Rows[rIndex].Cells[1].Text.ToString();
                    gvForApprovedList.Rows[rIndex].Visible = false;
                }

            }
            catch { }
        }

        private void saveLeaveApplicationRequest_AsLog(int rIndex)    // this function for save leave application log
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select LACode,LeaveId,Format(FromDate,'yyyy-MM-dd') as FromDate,Format(ToDate,'yyyy-MM-dd') as ToDate,WeekHolydayNo,TotalDays," +
                    "Remarks,StateStatus,EmpId,IsApproved,Format(ApprovedDate,'yyyy-MM-dd')as ApprovedDate,IsProcessessed,EmpTypeId,Format(PregnantDate,'yyyy-MM-dd') as PregnantDate," +
                    "Format(PrasaberaDate,'yyyy-MM-dd') as PrasaberaDate,Format(EntryDate,'yyyy-MM-dd') as EntryDate,CompanyId,SftId,DptId,DsgId,EmpStatus,Format(ApplyDate,'yyyy-MM-dd') as ApplyDate,LeaveFormSLNo,LeaveProcessingOrder,LvAddress,LvContact,Format(CreatedAt,'yyyy-MM-dd HH:mm:ss') as CreatedAt,isnull(CreatedBy,0) as CreatedBy" +
                    " from Leave_LeaveApplication where LACode=" + gvForApprovedList.DataKeys[rIndex].Values[0].ToString() + "", dt);

                ViewState["__OldFromDate__"]=dt.Rows[0]["FromDate"].ToString();
                ViewState["__OldToDate__"]=dt.Rows[0]["ToDate"].ToString();
                string CreatedAt= dt.Rows[0]["CreatedAt"].ToString();
                string CreatedBy = dt.Rows[0]["CreatedBy"].ToString();
                if (CreatedBy == "0")
                    CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                byte isApproved = (bool.Parse(dt.Rows[0]["IsApproved"].ToString()).Equals(true)) ? (byte)1 : (byte)0;
                byte isProcessed = (bool.Parse(dt.Rows[0]["IsProcessessed"].ToString()).Equals(true)) ? (byte)1 : (byte)0;


                if (dt.Rows[0]["PregnantDate"].ToString().Length ==10) // 10 Length menas=dd-MM-yyyy this type length
                {
                    if (isApproved == 1)   // while leave nature is maternity and already this leave are approved.
                    {
                        string[] getColumns = { "LACode", "LeaveId", "FromDate", "ToDate", "WeekHolydayNo", "TotalDays", "Remarks", "StateStatus","IsApproved",
                                               "ApprovedDate", "IsProcessessed", "EmpTypeId", "PregnantDate", "PrasaberaDate", "EntryDate", "CompanyId", "SftId","EmpId","ApprovedRejected","DptId","DsgId","EmpStatus","ApplyDate","LeaveFormSLNo","LeaveProcessingOrder","LvAddress","LvContact","CreatedAt","CreatedBy" };
                        string[] getValues = {dt.Rows[0]["LACode"].ToString(), dt.Rows[0]["LeaveId"].ToString(), dt.Rows[0]["FromDate"].ToString(),
                                             dt.Rows[0]["ToDate"].ToString(),dt.Rows[0]["WeekHolydayNo"].ToString(),
                                             dt.Rows[0]["TotalDays"].ToString(),dt.Rows[0]["Remarks"].ToString(),dt.Rows[0]["StateStatus"].ToString(),isApproved.ToString(),
                                             dt.Rows[0]["ApprovedDate"].ToString(),isProcessed.ToString(),
                                             dt.Rows[0]["EmpTypeId"].ToString(),dt.Rows[0]["PregnantDate"].ToString(),
                                             dt.Rows[0]["PrasaberaDate"].ToString(),
                                             dt.Rows[0]["EntryDate"].ToString(),
                                             dt.Rows[0]["CompanyId"].ToString(),dt.Rows[0]["SftId"].ToString(),dt.Rows[0]["EmpId"].ToString(),"Approved",
                                             dt.Rows[0]["DptId"].ToString(),dt.Rows[0]["DsgId"].ToString(),dt.Rows[0]["EmpStatus"].ToString(),dt.Rows[0]["ApplyDate"].ToString(),dt.Rows[0]["LeaveFormSLNo"].ToString(),dt.Rows[0]["LeaveProcessingOrder"].ToString(),dt.Rows[0]["LvAddress"].ToString(),dt.Rows[0]["LvContact"].ToString(),CreatedAt,CreatedBy};
                        SQLOperation.forSaveValue("Leave_LeaveApplication_Log", getColumns, getValues, sqlDB.connection);
                      
                        ChangeEmpTypeForML(dt.Rows[0]["EmpId"].ToString());  // for employee current status changed when leave type are maternity leave 
                    }
                    else // while leave nature is maternity and  leave are not approved
                    {
                        string[] getColumns = { "LACode", "LeaveId", "FromDate", "ToDate", "WeekHolydayNo", "TotalDays", "Remarks", "StateStatus", "IsApproved",
                                               "IsProcessessed", "EmpTypeId", "PregnantDate", "PrasaberaDate", "EntryDate", "CompanyId", "SftId","EmpId","ApprovedRejected","DptId","DsgId","EmpStatus","ApplyDate","LeaveFormSLNo","LeaveProcessingOrder","LvAddress","LvContact","CreatedAt","CreatedBy"};
                        string[] getValues = {dt.Rows[0]["LACode"].ToString(), dt.Rows[0]["LeaveId"].ToString(),dt.Rows[0]["FromDate"].ToString(),
                                             dt.Rows[0]["ToDate"].ToString(),dt.Rows[0]["WeekHolydayNo"].ToString(),
                                             dt.Rows[0]["TotalDays"].ToString(),dt.Rows[0]["Remarks"].ToString(),dt.Rows[0]["StateStatus"].ToString(),isApproved.ToString(),
                                             isProcessed.ToString(),
                                             dt.Rows[0]["EmpTypeId"].ToString(),dt.Rows[0]["PregnantDate"].ToString(),
                                             dt.Rows[0]["PrasaberaDate"].ToString(),
                                             dt.Rows[0]["EntryDate"].ToString(),
                                             dt.Rows[0]["CompanyId"].ToString(),dt.Rows[0]["SftId"].ToString(),dt.Rows[0]["EmpId"].ToString(),"Rejected",
                                             dt.Rows[0]["DptId"].ToString(),dt.Rows[0]["DsgId"].ToString(),dt.Rows[0]["EmpStatus"].ToString(),dt.Rows[0]["ApplyDate"].ToString(),dt.Rows[0]["LeaveFormSLNo"].ToString(),dt.Rows[0]["LeaveProcessingOrder"].ToString(),dt.Rows[0]["LvAddress"].ToString(),dt.Rows[0]["LvContact"].ToString(),CreatedAt,CreatedBy};
                        SQLOperation.forSaveValue("Leave_LeaveApplication_Log", getColumns, getValues, sqlDB.connection);
                    }
                }
                else  // here count every leave without maternity leave type,so here just considering approved status for approved date
                {
                    if (isApproved == 1)
                    {
                        string[] getColumns = { "LACode", "LeaveId", "FromDate", "ToDate", "WeekHolydayNo", "TotalDays", "Remarks", "StateStatus", "IsApproved",
                                               "ApprovedDate", "IsProcessessed", "EmpTypeId","EntryDate", "CompanyId", "SftId","EmpId","ApprovedRejected","DptId","DsgId","EmpStatus","ApplyDate","LeaveFormSLNo","LeaveProcessingOrder","LvAddress","LvContact","CreatedAt","CreatedBy"   };
                        string[] getValues = {dt.Rows[0]["LACode"].ToString(), dt.Rows[0]["LeaveId"].ToString(),dt.Rows[0]["FromDate"].ToString(),
                                             dt.Rows[0]["ToDate"].ToString(),dt.Rows[0]["WeekHolydayNo"].ToString(),
                                             dt.Rows[0]["TotalDays"].ToString(),dt.Rows[0]["Remarks"].ToString(),dt.Rows[0]["StateStatus"].ToString(),isApproved.ToString(),
                                             dt.Rows[0]["ApprovedDate"].ToString(),isProcessed.ToString(),
                                             dt.Rows[0]["EmpTypeId"].ToString(),                          
                                             dt.Rows[0]["EntryDate"].ToString(),
                                             dt.Rows[0]["CompanyId"].ToString(),dt.Rows[0]["SftId"].ToString(),dt.Rows[0]["EmpId"].ToString(),"Approved",
                                             dt.Rows[0]["DptId"].ToString(),dt.Rows[0]["DsgId"].ToString(),dt.Rows[0]["EmpStatus"].ToString(),dt.Rows[0]["ApplyDate"].ToString(),dt.Rows[0]["LeaveFormSLNo"].ToString(),dt.Rows[0]["LeaveProcessingOrder"].ToString(),dt.Rows[0]["LvAddress"].ToString(),dt.Rows[0]["LvContact"].ToString(),CreatedAt,CreatedBy};
                        SQLOperation.forSaveValue("Leave_LeaveApplication_Log", getColumns, getValues, sqlDB.connection);
                    }
                    else
                    {
                        string[] getColumns = { "LACode", "LeaveId", "FromDate", "ToDate", "WeekHolydayNo", "TotalDays", "Remarks", "StateStatus", "IsApproved",
                                               "IsProcessessed", "EmpTypeId","EntryDate", "CompanyId", "SftId","EmpId","ApprovedRejected","DptId","DsgId","EmpStatus","ApplyDate","LeaveFormSLNo","LeaveProcessingOrder","LvAddress","LvContact" ,"CreatedAt","CreatedBy"  };
                        string[] getValues = {dt.Rows[0]["LACode"].ToString(), dt.Rows[0]["LeaveId"].ToString(), dt.Rows[0]["FromDate"].ToString(),
                                             dt.Rows[0]["ToDate"].ToString(),dt.Rows[0]["WeekHolydayNo"].ToString(),
                                             dt.Rows[0]["TotalDays"].ToString(),dt.Rows[0]["Remarks"].ToString(),dt.Rows[0]["StateStatus"].ToString(),isApproved.ToString(),
                                             isProcessed.ToString(),
                                             dt.Rows[0]["EmpTypeId"].ToString(),                          
                                             dt.Rows[0]["EntryDate"].ToString(),
                                             dt.Rows[0]["CompanyId"].ToString(),dt.Rows[0]["SftId"].ToString(),dt.Rows[0]["EmpId"].ToString(),"Rejected",
                                             dt.Rows[0]["DptId"].ToString(),dt.Rows[0]["DsgId"].ToString(),dt.Rows[0]["EmpStatus"].ToString(),dt.Rows[0]["ApplyDate"].ToString(),dt.Rows[0]["LeaveFormSLNo"].ToString(),dt.Rows[0]["LeaveProcessingOrder"].ToString(),dt.Rows[0]["LvAddress"].ToString(),dt.Rows[0]["LvContact"].ToString(),CreatedAt,CreatedBy};

                        SQLOperation.forSaveValue("Leave_LeaveApplication_Log", getColumns, getValues, sqlDB.connection);
                    }
                
                }


                if (isApproved==1) // if and date are changed then change leave application date and total days changed
                {
                    TextBox txtFromDate = (TextBox)gvForApprovedList.Rows[rIndex].Cells[2].FindControl("txtFromDate");
                    TextBox txtToDate = (TextBox)gvForApprovedList.Rows[rIndex].Cells[3].FindControl("txtToDate");
                    string[] fDate = txtFromDate.Text.Trim().Split('-');
                    string[] tDate = txtToDate.Text.Trim().Split('-');
                    if (!ViewState["__OldFromDate__"].ToString().Equals(txtFromDate.Text.Trim()) || !ViewState["__OldToDate__"].ToString().Equals(txtToDate.Text.Trim()))
                    {
                        TextBox txtTotalDays = (TextBox)gvForApprovedList.Rows[rIndex].Cells[3].FindControl("txtTotalDays");

                        string[] getColumns = { "FromDate", "ToDate", "TotalDays", "WeekHolydayNo" };
                        string[] getValues = { fDate[2]+"-"+ fDate[1]+"-"+ fDate[0], tDate[2] + "-" + tDate[1] + "-" + tDate[0], txtTotalDays.Text.Trim(), ViewState["__WHnumber__"].ToString()};
                        SQLOperation.forUpdateValue("Leave_LeaveApplication", getColumns, getValues, "LACode", gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);

                        saveLeaveDetails(gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), txtFromDate.Text, txtToDate.Text, dt.Rows[0]["EmpId"].ToString());  // for save leave details
                    }
                }

                if (ViewState["__Predecate__"].ToString().Equals("No")) NoApproved(rIndex);
                
            }
            catch (Exception ex)
            {
              //  MessageBox.Show(ex.Message);
            }
        }


        private void ChangeEmpTypeForML(string getEmpId)   // change EmpType when any female get any Maternity Leave
        {
            try
            {
                SqlCommand cmd = new SqlCommand("update Personnel_EmpCurrentStatus set EmpStatus='8' where SN=(select Max(SN) from  Personnel_EmpCurrentStatus Where EmpId='" + getEmpId+ "' AND IsActive=1)", sqlDB.connection);
                cmd.ExecuteNonQuery();
            }
            catch { }
        }


        private void saveLeaveDetails(string LACode, string FDates,string TDates,string EmpId)
        {
            try
            {
                SQLOperation.forDeleteRecordByIdentifier("Leave_LeaveApplicationDetails", "LACode", LACode, sqlDB.connection);

                string[] getFDate = FDates.ToString().Split('-');  // dd-MM-yyyy

                string [] getToDate=TDates.ToString().Split('-');

                DateTime dtFromDate = new DateTime(int.Parse(getFDate[2]), int.Parse(getFDate[1]), int.Parse(getFDate[0]));

                DateTime dtToDate = new DateTime(int.Parse(getToDate[2]), int.Parse(getToDate[1]), int.Parse(getToDate[0]));

                while(dtFromDate<=dtToDate)
                {
                    getFDate = dtFromDate.ToString().Split('/'); // now Format m-d-yyyy

                    

                    string Date = getFDate[2] + "-" + getFDate[1] + "-" + getFDate[0]; // convert format in yyyy-MM-dd
                    string[] getColumns = { "LACode", "EmpId", "LeaveDate", "Used" };
                    string[] getValues = { LACode,EmpId, Date, "0" };
                    SQLOperation.forSaveValue("Leave_LeaveApplicationDetails", getColumns, getValues, sqlDB.connection);
                    dtFromDate=dtFromDate.AddDays(1);

 
                }
               

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        

        protected void gvForApprovedList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onmouseover"] = "javascript:SetMouseOver(this)";
                    e.Row.Attributes["onmouseout"] = "javascript:SetMouseOut(this)";

                    Button btn;
                    try
                    {
                        string EmpId = gvForApprovedList.DataKeys[e.Row.RowIndex].Values[2].ToString();
                        ViewState["__LvAuthorityAction__"] = classes.commonTask.getLvAuthorityAction(ViewState["__UserId__"].ToString(),EmpId);
                        string LaCode = gvForApprovedList.DataKeys[e.Row.RowIndex].Values[0].ToString();

                        CheckBox ckbAttachment = (CheckBox)e.Row.FindControl("ckbAttachment");
                        if (File.Exists(Server.MapPath("/EmployeeImages/LeaveDocument/" + LaCode + ".jpg")))
                            ckbAttachment.Checked = true;
                        else
                            ckbAttachment.Checked = false;

                        if (ViewState["__LvAuthorityAction__"].ToString() != "1" && ViewState["__LvAuthorityAction__"].ToString() != "0")
                        {
                            btn = new Button();
                            btn = (Button)e.Row.FindControl("btnForword");
                            btn.Enabled = false;
                            btn.ForeColor = Color.Silver;
                        }
                        if (ViewState["__LvAuthorityAction__"].ToString() != "2" && ViewState["__LvAuthorityAction__"].ToString() != "0")
                        {
                            btn = new Button();
                            btn = (Button)e.Row.FindControl("btnYes");
                            btn.Enabled = false;
                            btn.ForeColor = Color.Silver;
                        }


                    }
                    catch { }
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

        protected void btnCLeave_Click(object sender, EventArgs e)
        { 
        
        }


        //Md.Nayem 
        //Email=xxx@gmail.com
        //purpose : To set value for leave applicaiton report

        private void viewLeaveApplication(string LaCode)
        {
            try
            {
                if (LeaveLibrary.viewLeaveApplication(LaCode))
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
                //    lblMessage.InnerText = "warning->Sorry any payslip are not founded"; return;
                //}
                //string[] FDate = dt.Rows[0]["FromDate"].ToString().Split('-');
                //Session["__Language__"] = "English";
                //Session["__LeaveApplication__"] = dt;
                //getSQLCMD = "with lvd as ( select CompanyId, Leaveid, ShortName,count(ShortName) as Amount,case when Sex='Male' then 'm/l'else '' end Sex " +
                //     " from v_Leave_LeaveApplicationDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsApproved=1 and LeaveDate>='" + FDate[2] + "-01-01' and LeaveDate<'" + FDate[2] + "-" + FDate[1] + "-" + FDate[0] + "'" +
                //     " group by CompanyId,Leaveid, ShortName,Sex),pcs as (select case when Sex='Male' then 'm/l'else '' end Sex,CompanyId from v_EmployeeDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsActive=1 ) ,lc as ( select * from tblLeaveConfig where CompanyId=(select CompanyId from pcs)) ," +
                //     " la as(select  LeaveId,TotalDays from Leave_LeaveApplication where LACode=" + LaCode + ")" +
                //     " select lc.ShortName,ISNULL(lvd.Amount,0) as Amount,lc.LeaveDays,lc.CompanyId,lc.LeaveName,( lc.LeaveDays-ISNULL(lvd.Amount,0) )as Remaining,TotalDays Applied from  lc left join lvd on lc.LeaveId=lvd.LeaveId and lc.CompanyId=lvd.CompanyId  left join la on lc.LeaveId=la.LeaveId " +
                //     " where  lc.ShortName not in('sr/l',(select   Sex from pcs))";
                //sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                //Session["__LeaveCurrentStatus__"] = dt;
                //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LeaveApplication');", true);  //Open New Tab for Sever side code

            }
            catch { }
        }
        private void saveAprovalLog(int rIndex, string Approval) 
        {
            try 
            {
                string[] getColumns = { "LACode", "UserID", "DateTime", "Approval" };
                string[] getValues = { gvForApprovedList.DataKeys[rIndex].Values[0].ToString(), ViewState["__UserId__"].ToString(),DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Approval };
                SQLOperation.forSaveValue("Leave_ApprovalLog", getColumns, getValues, sqlDB.connection);
            }
            catch { }
            
              
        }

        protected void rblEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchLeaveApplicationForApproved();
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
                    if (checkLeaveAuthority(row.RowIndex, "1"))
                    {
                        ViewState["__Predecate__"] = "Forword";
                        if (Forward(row.RowIndex))
                            saveAprovalLog(row.RowIndex, "0");
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
                    if (checkLeaveAuthority(row.RowIndex,"2"))
                    {
                        ViewState["__Predecate__"] = "Yes";
                        if (YesApproved(row.RowIndex))
                            saveAprovalLog(row.RowIndex, "1");
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
                    if (checkLeaveAuthority(row.RowIndex, "1,2"))
                    {                      

                        ViewState["__Predecate__"] = "No";
                        string EmpId = gvForApprovedList.DataKeys[row.RowIndex].Values[2].ToString();
                        string LvAuthorityOrder = getLvAuthorityOrder(ViewState["__UserId__"].ToString(), EmpId);//getLvAuthorityOrder();
                        string LaCode = gvForApprovedList.DataKeys[row.RowIndex].Values[0].ToString();
                        if (!IsValid(LvAuthorityOrder, LaCode))
                        {
                            //Response.Redirect(Request.RawUrl);
                            //return;
                            continue;
                        }
                        saveLeaveApplicationRequest_AsLog(row.RowIndex);
                        saveAprovalLog(row.RowIndex, "2");
                        // this function calling for save leave application log
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

        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("1"))
            {
                btnForword.Enabled = true;
                btnForword.CssClass = "hand";
            }
            else
            {
                btnForword.Enabled = false;
                btnForword.CssClass = "";
            }
            if (ViewState["__UpdateAction__"].ToString().Equals("0"))
            {
                btnYes.Enabled = true;
                btnYes.CssClass = "hand";
            }
            else
            {
                btnYes.Enabled = false;
                btnYes.CssClass = "";
            }

            if (ViewState["__DeletAction__"].ToString().Equals("0"))
            {
                btnNot.Enabled = true;
                btnNot.CssClass = "hand";
            }
            else
            {
                btnNot.Enabled = false;
                btnNot.CssClass = "";
            }
           
        }
    }
}