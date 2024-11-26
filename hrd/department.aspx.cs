using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using ComplexScriptingSystem;
using System.Text;
using System.Drawing;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;

namespace SigmaERP.hrd
{
    public partial class department : System.Web.UI.Page
    {
        string CompanyId = "";
        string sqlcmd="";
        DataTable dt;
        // View=191,Add=192,Edit=193,Delete=194
    
        protected void Page_Load(object sender, EventArgs e)
        {
           
            sqlDB.connectionString = Glory.getConnectionString();           
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                ViewState["__ReadAction__"] = "0";
                ViewState["__WriteAction__"] = "0";
                ViewState["__UpdateAction__"] = "0";
                ViewState["__DeletAction__"] = "0";
                int[] pagePermission = { 191, 192, 193, 194 };
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);
                setPrivilege(userPagePermition);
              
            }          

        }
        private void setPrivilege(int[] permission)
        {
            try
            {

                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__preRIndex__"] = "No";
                string getUserId = getCookies["__getUserId__"].ToString();

                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                string[] AccessPermission = new string[0];
                // AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "department.aspx", ddlCompanyName, divDepartmentList, btnSave);

                if(permission.Contains(191))
                    ViewState["__ReadAction__"] = "1";
                if(permission.Contains(192))
                    ViewState["__WriteAction__"] = "1";
                if(permission.Contains(193))
                    ViewState["__UpdateAction__"] = "1";
                if(permission.Contains(194))
                    ViewState["__DeletAction__"] = "1";

                checkInitialPermission();
                LoadCompanyInfo();
                loadDepartment();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                    //  dlExistsUser.Enabled = false;
                }

            
            catch { }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string date = txtStartDate.Text.ToString();
               
                    if (ViewState["__CardNoType__"].ToString().Equals("True")&&!CodeValidation())
                    {
                        lblMessage.InnerText = "error->This Code is used by another Department!"; return;
                    }
                
                if (dlStatus.Text == "-select-")
                {
                    lblMessage.InnerText = "warning->Please Select Status";
                    return;
                }
                Page.Validate();
                if (Page.IsValid)
                {
                    if (btnSave.Text == "Update")
                    {
                        if (UpdateDepartment() == true)
                        {
                            loadDepartment();
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UpdateSuccess()", true);
                            allClear();
                        }
                    }
                    else
                    {

                        if (SaveDepartment() == true)
                        {
                            allClear();
                            loadDepartment();
                            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "SaveSuccess()", true);
                        }
                    }
                }
                
            }
            catch
            {
                
            }
        }
        private bool CodeValidation()
        {
            try
            {
                 CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                 dt = new DataTable();
                if (btnSave.Text == "Update")
                sqlDB.fillDataTable("select DptCode From HRD_Department where CompanyId='"+CompanyId+"' and DptCode='" + txtDepartmentCode.Text + "' and SL <>" + ViewState["__getSL__"].ToString() + "", dt);
                else
                    sqlDB.fillDataTable("select DptCode From HRD_Department where CompanyId='" + CompanyId + "' and DptCode='" + txtDepartmentCode.Text + "' ", dt);
                if (dt == null || dt.Rows.Count == 0) return true;
                else
                return false;
            }
            catch { return false; }
        }
        private void allClear()
        {
            txtDepartment.Text = "";
            txtDepartmentBn.Text = "";
            if (ViewState["__CardNoType__"].ToString().Equals("True"))           
            txtDepartmentCode.Text = "";
            dlStatus.Text = "-select-";
            hdnbtnStage.Value = "";
            hdnUpdate.Value = "";
            if (ViewState["__WriteAction__"].Equals("0"))
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
        }
        private Boolean SaveDepartment()
        {
            try
            {
                int st;
                
                if (dlStatus.Text.Equals("Active")) st = 1;
                else st = 0;
                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                {
                    CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                }
                else
                {
                    CompanyId = ViewState["__CompanyId__"].ToString();
                }
                SqlCommand cmd = new SqlCommand("insert into HRD_Department values('" + classes.commonTask.LoadSL("Select Max(Sl) as SL from HRD_Department", "Department") + "','" + CompanyId + "','" + txtDepartment.Text.Trim() + "',N'" + txtDepartmentBn.Text + "','" + txtDepartmentCode.Text.Trim() + "'," + st.ToString() + ",0,0,'" + txtStartDate.Text.Trim() + "','" + txtendDate.Text + "'," + txtstipend.Text.Trim() +") ", sqlDB.connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                lblMessage.InnerText =ex.Message;
                return false;
            }
        }
        private Boolean UpdateDepartment()
        {
            try
            {
                int st;
                
                if (dlStatus.Text.Equals("Active")) st = 1;
                else st = 0;
                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                {
                    CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                }
                else
                {
                    CompanyId = ViewState["__CompanyId__"].ToString();
                }
                string getIdentifierValue = ViewState["__getSL__"].ToString();
                SqlCommand cmd = new SqlCommand("Update HRD_Department set CompanyId='" + CompanyId + "', DptName='" + txtDepartment.Text + "',DptNameBn=N'" + txtDepartmentBn.Text + "',DptCode='" + txtDepartmentCode.Text.Trim() + "',DptStatus=" + st.ToString() + ",CourseStartDate='"+txtStartDate.Text.Trim().ToString()+ "',CourseEndDate='" + txtendDate.Text.Trim().ToString() + "',StipendAmount="+txtstipend.Text.Trim()+" where SL=" + getIdentifierValue + "", sqlDB.connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = ex.Message;
                return false;
            }
        }

        private void loadDepartment()
        {
            try
            {
                CompanyId = (ddlCompanyName.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue;
                //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                //{
                //    if (string.IsNullOrEmpty(sqlcmd)) sqlcmd = "SELECT SL, DptName, DptNameBn,DptCode, DptStatus,CompanyName FROM v_HRD_Department where CompanyId='" + CompanyId + "' order by convert(int,DptCode) ";
                //}
                //else
                //{
                sqlcmd = @"SELECT  SL,CompanyId, DptName,DptNameBn,DptCode,DptStatus,CompanyName, DptId,CASE 
WHEN CourseStartDate = '1900-01-01' THEN '' ELSE ISNULL(CONVERT(varchar(10), CourseStartDate, 105), '') END AS CourseStartDate,CASE WHEN CourseEndDate = '1900-01-01' THEN '' ELSE ISNULL(CONVERT(varchar(10), CourseEndDate, 105), '')  END AS CourseEndDate,StipendAmount
 FROM  v_HRD_Department where CompanyId='" + CompanyId + "' order by convert(int,DptCode)";
                
                 dt = new DataTable();
                sqlDB.fillDataTable(sqlcmd, dt);                
                divDepartmentList.DataSource = dt;
                divDepartmentList.DataBind();                
                if (ViewState["__CardNoType__"].ToString().Equals("True"))
                {
                    divDepartmentList.Columns[4].Visible = true;
                }
                else { divDepartmentList.Columns[4].Visible = false; }
            }
            catch { }
        }
   
        public static string DecodeFromUtf8(string utf8String)
        {
            // read the string as UTF-8 bytes.
            byte[] encodedBytes = Encoding.UTF8.GetBytes(utf8String);

            // convert them into unicode bytes.
            byte[] unicodeBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, encodedBytes);

            // builds the converted string.
            return Encoding.Unicode.GetString(encodedBytes);
        }    

        protected void divDepartmentList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                
                if (e.CommandName.Equals("Alter"))
                {
                    string a = ViewState["__preRIndex__"].ToString();
                    if (!ViewState["__preRIndex__"].ToString().Equals("No")) divDepartmentList.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    
                    divDepartmentList.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                    ViewState["__preRIndex__"] = rIndex;
                    setValueToControl(divDepartmentList.DataKeys[rIndex].Values[0].ToString(), divDepartmentList.DataKeys[rIndex].Values[1].ToString());
                    btnSave.Text = "Update";
                    if (ViewState["__UpdateAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Rbutton";
                    }
                    if (deleteValidation(divDepartmentList.DataKeys[rIndex].Values[2].ToString()))
                        txtDepartmentCode.Enabled = true;
                    else txtDepartmentCode.Enabled = false;
                }
                else if (e.CommandName.Equals("deleterow"))
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    if (deleteValidation(divDepartmentList.DataKeys[rIndex].Values[2].ToString()))
                    {
                        SQLOperation.forDeleteRecordByIdentifier("HRD_Department", "SL", divDepartmentList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "deleteSuccess()", true);
                        allClear();
                        lblMessage.InnerText = "success->Successfully Department Deleted";
                        divDepartmentList.Rows[rIndex].Visible = false;
                    }
                    else
                        lblMessage.InnerText = "error->This Department is not deletable ! Because this is used for any employee.";
                }
            }
            catch { }
        }
        private bool deleteValidation(string DptID) 
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("Select EmpID from Personnel_EmpCurrentStatus where DptId='" + DptID + "'", dt);
            if (dt.Rows.Count > 0)
                return false;
            else return true;
        }
        private void setValueToControl(string getSL,string getCompanyId)
        {
            try
            {
                dt = new DataTable();
                dt = commonTask.getDepartment(getSL);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewState["__getSL__"] = getSL;
                    ddlCompanyName.SelectedValue = getCompanyId;
                    txtDepartment.Text = dt.Rows[0]["DptName"].ToString();
                    txtDepartmentBn.Text = dt.Rows[0]["DptNameBn"].ToString();
                    txtDepartmentCode.Text = dt.Rows[0]["DptCode"].ToString();
                    if (dt.Rows[0]["DptStatus"].ToString().Equals("True"))
                        dlStatus.SelectedValue = "Active";
                    else
                        dlStatus.SelectedValue = "InActive";
                    txtStartDate.Text = dt.Rows[0]["CourseStartDate"].ToString();
                    txtendDate.Text = dt.Rows[0]["CourseEndDate"].ToString();
                    if (txtStartDate.Text != "")
                    {
                        rfvStartDate.Enabled = false;
                    }
                    if (txtendDate.Text != "")
                    {
                        rfvEndDate.Enabled = false;
                    }
                    //DateTime courseStartDate;
                    //DateTime courseEndDate;

                    //if (DateTime.TryParseExact(dt.Rows[0]["CourseStartDate"].ToString(), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out courseStartDate))
                    //{
                    //    txtStartDate.Text = courseStartDate.ToString("MM-dd-yyyy");
                    //}

                    //if (DateTime.TryParseExact(dt.Rows[0]["CourseEndDate"].ToString(), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out courseEndDate))
                    //{
                    //    txtendDate.Text = courseEndDate.ToString("MM-dd-yyyy");
                    //}
                    txtstipend.Text = dt.Rows[0]["StipendAmount"].ToString();
                }              

            }
            catch { }
        }

        protected void divDepartmentList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            loadDepartment();
        }

        protected void divDepartmentList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            
            loadDepartment();
            divDepartmentList.PageIndex = e.NewPageIndex;
            divDepartmentList.DataBind();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            allClear();
            loadDepartment();
        }

        protected void divDepartmentList_RowDataBound(object sender, GridViewRowEventArgs e)
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
            //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            //{
                try
                {
                    if (ViewState["__DeletAction__"].ToString().Equals("0"))
                    {
                        LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                        lnkDelete.Enabled = false;
                        lnkDelete.OnClientClick = "return false";
                        lnkDelete.ForeColor = Color.Silver;
                    }

                }
                catch { }
                try
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {
                        LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkAlter");
                        lnkDelete.Enabled = false;
                        lnkDelete.ForeColor = Color.Silver;
                    }

                }
                catch { }
            //}

            try
            {
                if (ViewState["__DeletAction__"].ToString().Equals("0"))
                {
                    LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                    lnkDelete.Enabled = false;
                    lnkDelete.OnClientClick = "return false";
                    lnkDelete.ForeColor = Color.Silver;
                }

    }
            catch { }
}

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCompanyInfo();
            loadDepartment();
        }
        private void LoadCompanyInfo()
        {
            CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"] .ToString(): ddlCompanyName.SelectedValue;
            DataTable dtcom = new DataTable();
            sqlDB.fillDataTable("Select CardNoType,FlatCode From HRD_CompanyInfo where CompanyId='" + CompanyId + "'", dtcom);

            ViewState["__CardNoType__"] = dtcom.Rows[0]["CardNoType"].ToString();           
            if (ViewState["__CardNoType__"].ToString().Equals("True"))
            { 
                trDptCode.Visible = true;
                txtDepartmentCode.Text = "";
            }
            else
            {
                trDptCode.Visible = false;
                txtDepartmentCode.Text = dtcom.Rows[0]["FlatCode"].ToString();
            }
        }
        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("1"))
            {
                btnSave.Enabled = true;
                btnSave.CssClass = "Rbutton";
            }
            else
            {
                btnSave.Enabled = false;
                btnSave.CssClass = "";
            }
           

        }

        protected void cvDateComparison_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime startDate;
            DateTime endDate;

            bool isStartDateValid = DateTime.TryParse(txtStartDate.Text, out startDate);
            bool isEndDateValid = DateTime.TryParse(txtendDate.Text, out endDate);

            if (isStartDateValid && isEndDateValid)
            {
                args.IsValid = endDate >= startDate;
            }
            else
            {
                args.IsValid = false; // Invalid if dates cannot be parsed.
            }
        }
    }
}