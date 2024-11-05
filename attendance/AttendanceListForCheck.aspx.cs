using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System.Drawing;

namespace SigmaERP.attendance
{
    public partial class AttendanceListForCheck : System.Web.UI.Page
    {
        DataTable dt;
        string query = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {           

                setPrivilege();
               // loadAttendanceList();               
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
        }

        //DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForList(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "attendance.aspx", ddlCompanyList, gvAttendanceList, btnSearch);

                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentName, ViewState["__CompanyId__"].ToString());
            }
            catch { }
        }

      

       
      
        protected void gvAttendanceList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                SearchAttendanceList();    

                gvAttendanceList.PageIndex = e.NewPageIndex;
                gvAttendanceList.DataBind();
            }
            catch { }
        }

        protected void gvAttendanceList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.Equals("Delete"))
                {
                    lblMessage.InnerText = "";
                    string[] getValus = e.CommandArgument.ToString().Split(',');


                    SqlCommand cmd = new SqlCommand("delete from tblAttendanceRecord where EmpId='" + getValus[0] + "' And Attdate='" + getValus[2].Substring(6, 4) + "-" + getValus[2].Substring(3, 2) + "-" + getValus[2].Substring(0, 2) + "'", sqlDB.connection);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        ChangeLeaveStatusByeEmpIdAndDate(getValus[0], getValus[3], getValus[2].Substring(6, 4) + "-" + getValus[2].Substring(3, 2) + "-" + getValus[2].Substring(0, 2));
                        lblMessage.InnerText = "success->Successfully Deleted";
                        //gvAttendanceList.Rows[].Visible = false;
                    }
                }

                if (e.CommandName.Equals("Alter"))
                {
                    lblMessage.InnerText = "";
                    int index = Convert.ToInt32(e.CommandArgument.ToString());
                    string EmpId = gvAttendanceList.DataKeys[index].Values[0].ToString();
                    string EmpCardNo = gvAttendanceList.DataKeys[index].Values[1].ToString();
                    string AttDate = gvAttendanceList.DataKeys[index].Values[2].ToString();
                    string AttStatus = gvAttendanceList.DataKeys[index].Values[3].ToString();
                    string InTime = gvAttendanceList.DataKeys[index].Values[4].ToString();
                    string OutTime = gvAttendanceList.DataKeys[index].Values[5].ToString();
                    string EmpType = gvAttendanceList.DataKeys[index].Values[6].ToString();
                    string EmpName = gvAttendanceList.DataKeys[index].Values[7].ToString();
                    string StateStatus = gvAttendanceList.DataKeys[index].Values[8].ToString();


                    Response.Redirect("/attendance/attendance.aspx?eid_cn_at=" + EmpId + "_" + EmpCardNo + "_" + AttDate + "_" + AttStatus + "_" + InTime + "_" + OutTime + "_" + EmpType + "_" + EmpName + "_" + StateStatus + "");
                }
            }
            catch { }
        }

        private void ChangeLeaveStatusByeEmpIdAndDate(string EmpId, string AttStatus, string AttDate)
        {
            try
            {
                if (AttStatus.Equals("lv"))
                {
                    sqlDB.fillDataTable("select LACode from Leave_LeaveApplicationDetails where LeaveDate='" + AttDate + "' AND EmpId='" + EmpId + "'", dt = new DataTable());
                    if (dt.Rows.Count > 0)
                    {
                        SqlCommand cmd = new SqlCommand("Update Leave_LeaveApplicationDetails set Used='0' where EmpId='" + EmpId + "' AND LeaveDate='" + AttDate + "'", sqlDB.connection);
                        cmd.ExecuteNonQuery();

                        cmd = new SqlCommand("Update Leave_LeaveApplication set IsProcessessed='0' where LACode=" + dt.Rows[0]["LACode"].ToString() + " AND EmpId='" + EmpId + "' AND ToDate='" + AttDate + "'", sqlDB.connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

      

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SearchAttendanceList();
        }
        private void SearchAttendanceList()
        {
            try
            {


                //9. Search by Company,Department,Date
                if (ddlCompanyList.SelectedItem.Text.Trim() != "" && ddlDepartmentName.SelectedValue != "0" && txtToDate.Text.Trim().Length > 0)                    
                query = "select  EmpId,EmpCardNo,MonthId,Format(AttDate,'dd-MM-yyyy') as AttDate,AttStatus,AttManual,InTime,OutTime,EmpType,EmpName,StateStatus from v_tblAttendanceRecord where CompanyId='" + ddlCompanyList.SelectedValue + "' and DptId='" + ddlDepartmentName.SelectedValue + "' and ATTDate>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and ATTDate<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and (EmpId in (select EmpId   from v_tblAttendanceRecord where CompanyId='" + ddlCompanyList.SelectedValue + "' and DptId='" + ddlDepartmentName.SelectedValue + "' and ATTDate>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and ATTDate<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' group by EmpId,ATTDate having count(EmpId)>1) or InTime='00:00:00' or OutTime='00:00')   order by AttDate desc";
                //9. Search by Company,Department,Date
                else if (ddlCompanyList.SelectedItem.Text.Trim() != "" && ddlDepartmentName.SelectedValue == "0" && txtToDate.Text.Trim().Length > 0)
                    query = "select  EmpId,EmpCardNo,MonthId,Format(AttDate,'dd-MM-yyyy') as AttDate,AttStatus,AttManual,InTime,OutTime,EmpType,EmpName,StateStatus from v_tblAttendanceRecord where CompanyId='" + ddlCompanyList.SelectedValue + "' and ATTDate>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and ATTDate<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and (EmpId in (select EmpId   from v_tblAttendanceRecord where CompanyId='" + ddlCompanyList.SelectedValue + "' and ATTDate>='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' and ATTDate<='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtToDate.Text.Trim()) + "' group by EmpId,ATTDate having count(EmpId)>1) or InTime='00:00:00' or OutTime='00:00')   order by AttDate desc";

                //------------------------------------------------------------------------------------------------
                sqlDB.fillDataTable(query, dt = new DataTable());
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Data not found!";
                    gvAttendanceList.DataSource = null;
                    gvAttendanceList.DataBind();
                    return;
                }
                gvAttendanceList.DataSource = dt;
                gvAttendanceList.DataBind();

            }
            catch { }
        }


    


        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblMessage.InnerText = "";
                //  classes.commonTask.loadDivision(ddlDivisionName, ddlCompanyList.SelectedValue.ToString(), "Admin");
                //classes.commonTask.LoadShift(ddlShift, ddlCompanyList.SelectedValue.ToString(), "Admin");
                if (ddlCompanyList.SelectedValue == "0000")
                {
                    ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                }                
                classes.commonTask.loadDepartmentListByCompany(ddlDepartmentName, ddlCompanyList.SelectedValue.ToString());

                gvAttendanceList.DataSource = null;
                gvAttendanceList.DataBind();
                SearchAttendanceList();

            }
            catch { }
        }

     

        protected void ddlShift_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchAttendanceList();
        }
        protected void ddlGrouping_SelectedIndexChanged(object sender, EventArgs e)
        {
            SearchAttendanceList();
        }


        protected void ddlChoseYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            txtToDate.Text = "";
            SearchAttendanceList();
        }

        protected void gvAttendanceList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                Button btn;
                try
                {
                    if (ViewState["__DeletAction__"].ToString().Equals("0"))
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnRemove");
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
                        btn = (Button)e.Row.FindControl("btnAlter");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;
                    }

                }
                catch { }

            }


        }

    




    }
}