using adviitRuntimeScripting;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.attendance
{
    public partial class absent_notification_log : System.Web.UI.Page
    {
        //permission=322;
        DataTable dt;
        string query = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 322 };
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                txtFromDate.Text = "01-" + DateTime.Now.ToString("MM-yyyy");
                txtToDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                loadNotification();
                seenByAdmin();
            }
        }
        private void loadNotification()
        {
            try {
                if (txtFromDate.Text.Trim().Length < 10)
                {
                    lblMessage.InnerText = "warning->Please select from date.";
                    txtFromDate.Focus();
                    return;
                }
                if (txtToDate.Text.Trim().Length < 10)
                {
                    lblMessage.InnerText = "warning->Please select from date.";
                    txtToDate.Focus();
                    return;
                }

                string getUserId = Session["__GetUID__"].ToString();
                if(rblEmpType.SelectedValue=="All")
                query = "select ed.CompanyName,ed.Address,  anl.EmpID,SUBSTRING(ed.EmpCardNo,8,6)+' ('+EmpProximityNo+')'  as EmpCardNo,ed.EmpName,ed.DptName,ed.DsgName,convert(varchar(10), anl.Date,105) as Date,convert(varchar(10),anl.LastWorkingDay,105) as LastWorkingDay from AttAbsentNotification_Log as anl inner join v_EmployeeDetails as ed on anl.EmpID=ed.EmpId and ed.IsActive=1 and Date>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text) + "' and Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text) + "'    where AdminID=" + getUserId + "  order by anl.Date desc, seen";
                else
                query = "select ed.CompanyName,ed.Address,  anl.EmpID,SUBSTRING(ed.EmpCardNo,8,6)+' ('+EmpProximityNo+')' as EmpCardNo,ed.EmpName,ed.DptName,ed.DsgName,convert(varchar(10), anl.Date,105) as Date,convert(varchar(10),anl.LastWorkingDay,105) as LastWorkingDay from AttAbsentNotification_Log as anl inner join v_EmployeeDetails as ed on anl.EmpID=ed.EmpId and ed.IsActive=1 and Date>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtFromDate.Text) + "' and Date<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text) + "'    where EmpTypeID="+rblEmpType.SelectedValue+" and AdminID=" + getUserId + "   order by anl.Date desc, seen";
                sqlDB.fillDataTable(query, dt = new DataTable());
                gvAbsentList.DataSource = dt;
                gvAbsentList.DataBind();
                if (dt != null && dt.Rows.Count > 0)
                {
                    hCompanyName.InnerText = dt.Rows[0]["CompanyName"].ToString();
                    hCompanyAddress.InnerText = dt.Rows[0]["Address"].ToString();
                    hCompanyName.Visible = true;
                    hCompanyAddress.Visible = true;
                    hReportTitle.Visible = true;
                }
                else
                {
                    hCompanyName.Visible = false;
                    hCompanyAddress.Visible = false;
                    hReportTitle.Visible = false;
                    lblMessage.InnerText = "warning->Data Not Found.";
                    return;
                }
            }
            catch (Exception ex) { }
            
        }
        private void seenByAdmin()
        {

            query= "update AttAbsentNotification_Log set seen=1 where AdminID="+ Session["__GetUID__"].ToString() + " and seen=0 ";
            CRUD.Execute(query, sqlDB.connection);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
           
            loadNotification();
        }

        protected void rblEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {

            loadNotification();
        }

        protected void gvAbsentList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblLastWorkingDay = (Label)e.Row.FindControl("lblLastWorkingDay");
                if (lblLastWorkingDay.Text.Trim() == "")
                {
                    Label lblDate = (Label)e.Row.FindControl("lblDate");
                    lblLastWorkingDay.Text= commonTask.ddMMyyyyTo_yyyyMMdd(commonTask.getLastWorkingDay(gvAbsentList.DataKeys[e.Row.RowIndex].Values[0].ToString(), commonTask.ddMMyyyyTo_yyyyMMdd(lblDate.Text.Trim())));
                }
            }
        }
    }
}