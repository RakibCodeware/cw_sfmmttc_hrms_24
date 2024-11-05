using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP
{
    public partial class chatnotify : System.Web.UI.Page
    {
        public static string sqlCmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            string getStatus = Request.QueryString["f"];
            if (!string.IsNullOrEmpty(getStatus))
            {
                signout(); // so=singout
                return;
            }
        }

        [WebMethod]
        public static string loadMessage(string ReceiverId)
        {
            try
            {

                string getUserId = HttpContext.Current.Session["__GetUID__"].ToString();
                string getCompanyId = HttpContext.Current.Session["__GetCompanyId__"].ToString();

                string isLvAuthority = HttpContext.Current.Session["__isLvAuthority__"].ToString();
                string LvOnlyDpt = HttpContext.Current.Session["__LvOnlyDpt__"].ToString();
                string LvEmpType = HttpContext.Current.Session["__LvEmpType__"].ToString();
             
             
                DataTable dt;
                string notify = "";

                if (isLvAuthority == "True")
                {
                    //string dpt = "";
                    //string empType = "";




                    //// for load leave
                    //if (LvOnlyDpt == "True")
                    //    dpt = " and lv.DptId=ua.DptId";
                    //if (LvEmpType != "0")
                    //    empType = " and lv.EmpTypeId=" + LvEmpType;
                    //string query = "";
                    //query = "SELECT lv.EmpId,lv.DptId FROM v_UserAccount ua inner join Leave_LeaveApplication lv on ua.LvAuthorityOrder=lv.LeaveProcessingOrder "+dpt+"  where UserId='" + getUserId + "' and lv.IsApproved='False'" + empType;
                    //sqlDB.fillDataTable(query, dt = new DataTable());
                    sqlCmd = "SELECT lv.EmpId FROM tblLeaveAuthorityAccessControl ac inner join Leave_LeaveApplication lv on ac.AuthorityPosition=lv.LeaveProcessingOrder and ac.EmpID=lv.EmpId and  ac.AuthorityID='" + getUserId + "'  and lv.IsApproved='False' and lv.FromDate >='2021-01-01'";
                    sqlDB.fillDataTable(sqlCmd, dt = new DataTable());                   
                    if (dt.Rows.Count > 0)
                    {
                        notify = "forlv_" + dt.Rows.Count;
                    }
                    dt = new DataTable();
                    // for load short leave
                    sqlDB.fillDataTable("SELECT Leave_ShortLeave.EmpId FROM UserAccount inner join Leave_ShortLeave on UserAccount.LvAuthorityOrder=Leave_ShortLeave.LeaveProcessingOrder where UserId='" + getUserId + "' and Leave_ShortLeave.LvStatus='0'", dt = new DataTable());
                    if (dt.Rows.Count > 0)
                    {
                        if (notify != "")
                            notify += "_" + dt.Rows.Count;
                        else
                        {
                            notify = "forshortlv_" + dt.Rows.Count;
                        }
                    }
                }

                return notify;
               
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        [WebMethod]
        //public static string loadOutDuty(string ReceiverId)
        //{
        //    try
        //    {

        //        string getUserId = HttpContext.Current.Session["__GetUID__"].ToString();
        //        string getCompanyId = HttpContext.Current.Session["__GetCompanyId__"].ToString();

        //        string isLvAuthority = HttpContext.Current.Session["__isLvAuthority__"].ToString();
        //        string LvOnlyDpt = HttpContext.Current.Session["__LvOnlyDpt__"].ToString();
        //        string LvEmpType = HttpContext.Current.Session["__LvEmpType__"].ToString();


        //        DataTable dt;
        //        string notify = "";

        //        if (isLvAuthority == "True")
        //        {
        //            string dpt = "";
        //            string empType = "";

        //            // for load leave
        //            if (LvOnlyDpt == "True")
        //                dpt = " and lv.DptId=ua.DptId";
        //            if (LvEmpType != "0")
        //                empType = " and lv.EmpTypeId=" + LvEmpType;
        //            string query = "";
        //            query = "SELECT lv.EmpId,lv.DptId FROM v_UserAccount ua inner join v_tblOutDuty lv on ua.LvAuthorityOrder=lv.Processing " + dpt + "  where UserId='" + getUserId + "' and lv.Status=0" + empType;
        //            sqlDB.fillDataTable(query, dt = new DataTable());
        //            if (dt.Rows.Count > 0)
        //            {
        //                notify =  dt.Rows.Count.ToString();
        //            }                  
        //        }

        //        return notify;

        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}
        public static string loadOutDuty(string ReceiverId)
        {
            try
            {

                string getUserId = HttpContext.Current.Session["__GetUID__"].ToString();
                string getCompanyId = HttpContext.Current.Session["__GetCompanyId__"].ToString();
                DataTable dt;
                string notify = "";
                string sqlCmd = "SELECT od.EmpId FROM v_tblOutDuty od inner join tblOutDutyAuthorityAccessControl ac on ac.AuthorityPosition=od.Processing and ac.EmpID=od.EmpId and ac.AuthorityID='" + getUserId + "' and od.Status='0' and  IsActive=1 ";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                if (dt.Rows.Count > 0)
                {
                    notify = dt.Rows.Count.ToString();
                }
                return notify;

            }
            catch (Exception ex)
            {
                return "";
            }
        }
        [WebMethod]
        public static string loadAbsentNotification(string ReceiverId)
        {
            try
            {

                string getUserId = HttpContext.Current.Session["__GetUID__"].ToString();
                string getCompanyId = HttpContext.Current.Session["__GetCompanyId__"].ToString();

                string isLvAuthority = HttpContext.Current.Session["__isLvAuthority__"].ToString();
                string LvOnlyDpt = HttpContext.Current.Session["__LvOnlyDpt__"].ToString();
                string LvEmpType = HttpContext.Current.Session["__LvEmpType__"].ToString();
                DataTable dt;
                string notify = "";              
                    string query = "";
                    query = "select EmpID from AttAbsentNotification_Log where AdminID="+ getUserId+ " and seen=0";
                    sqlDB.fillDataTable(query, dt = new DataTable());
                    if (dt.Rows.Count > 0)
                    {
                        notify =dt.Rows.Count.ToString();
                    }
                  return notify;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        [WebMethod]
        public static string updateLoginDateTime(string ReceiverId)
        {
            try
            {
                string getUserId = HttpContext.Current.Session["__GetUID__"].ToString();

                string dateFormat = "dd-MM-yyyy hh:mm:ss";
                string datTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                DateTime LoginDateTime = DateTime.ParseExact(datTime, dateFormat, CultureInfo.InvariantCulture);
                SqlCommand cmd = new SqlCommand("Update UserAccount Set IsLogin='1',LoginDateTime='" + LoginDateTime + "' where UserId=" + getUserId + "", sqlDB.connection);
                cmd.ExecuteNonQuery();

                return " ";
            }
            catch { return " "; }
        }

        public void signout()
        {
            try
            {
                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();
                FormsAuthentication.SignOut();
                Response.Redirect("~/ControlPanel/Login.aspx",true);

            }
            catch (Exception ex)
            {
                string a = ex.Message;
            }
        }
    }
}