using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.personnel
{
    public partial class transfer_to_compliance : System.Web.UI.Page
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

                classes.commonTask.LoadEmpType(rblEmpType);
                setPrivilege();
                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                EmployeeList();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                if (Session["IsRedirect"] != null)
                {
                    if (Session["pageNumber"] != null)
                    {
                        gvForApprovedList.PageIndex = (int)Session["pageNumber"];
                        gvForApprovedList.DataBind();
                    }
                }
                Session["IsRedirect"] = null;
                Session["pageNumber"] = gvForApprovedList.PageIndex;
            }


        }
       
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForList(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "Employee.aspx", ddlCompanyList, gvForApprovedList, btnSearch);

                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];
            }
            catch { }
        }      
        /*Nayem.......
       .............For Searching............*/
        private void EmployeeList()
        {
            try
            {
                if (ddlCompanyList.SelectedItem.Text.Trim() == "")
                {
                    ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                }

                query = "Select CompanyId, EmpId,EmpCardNo+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,convert(varchar(11),EmpJoiningDate,105) as EmpJoiningDate,DptName,DsgName,SftName, convert(varchar(11),EmpShiftStartDate,105) as EmpShiftStartDate,EmpStatusName,EmpType From v_EmployeeDetails where EmpStatus in ('1','8') and IsActive='1' and ActiveSalary='True' and CompanyId='" + ddlCompanyList.SelectedValue + "' order by DptCode, CustomOrdering";
                sqlDB.fillDataTable(query, dt = new DataTable());
                //-----------------------------------------
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->Data not found";
                    gvForApprovedList.DataSource = null;
                    gvForApprovedList.DataBind();
                    return;
                }
                gvForApprovedList.DataSource = dt;
                gvForApprovedList.DataBind();

            }
            catch { }
        }
       
        protected void gvForApprovedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

                int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                string getId = gvForApprovedList.DataKeys[rIndex].Values[0].ToString();
                string CompanyId = gvForApprovedList.DataKeys[rIndex].Values[1].ToString();
                if (e.CommandName == "Transfer")
                {
                  
                }
            
            }
            catch { }
        }
        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlCompanyList.SelectedValue == "0000")
            {
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
            EmployeeList();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            EmployeeList();
        }
        

  

        

        protected void gvForApprovedList_RowDataBound(object sender, GridViewRowEventArgs e)
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
                try
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        btn = new Button();
                        btn = (Button)e.Row.FindControl("btnTransfer");
                        btn.Enabled = false;
                        btn.ForeColor = Color.Silver;
                    }

                }
                catch { }
            }
        }

     

       
    }
}