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
using SigmaERP.classes;

namespace SigmaERP.payroll
{
    public partial class loan : System.Web.UI.Page
    {
        DataTable dt;
        DataTable dtSetPrivilege;
        string query = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                clearLoanDetailsDatatable();
                setPrivilege();
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
                loadExistsLoan();
                //classes.Employee.LoadEmpCardNoWithNameByCompanyRShift(ddlEmpCardNo,ddlCompanyList.SelectedValue);
                classes.Employee.LoadEmpCardNoForPayroll_EmpID(ddlEmpCardNo, ViewState["__CompanyId__"].ToString());
                ViewState["AdvanceId"] = "0";
                ViewState["__EmpId__"] = "0";
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
            }
        }


        private void setPrivilege()
        {
            try
            {
                ViewState["__WriteAction__"] = "1";
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserId__"] = getCookies["__getUserId__"].ToString();

                if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Master Admin"))
                {
                    classes.commonTask.LoadBranch(ddlCompanyList);
                    // classes.commonTask.LoadShift(ddlShiftList, ViewState["__CompanyId__"].ToString());
                    return;
                }
                else
                {
                    dtSetPrivilege = new DataTable();
                    dtSetPrivilege = CRUD.ExecuteReturnDataTable("select * from UserPrivilege where ModulePageName='advance.aspx' and UserId=" + getUserId, sqlDB.connection);
                    if (dtSetPrivilege != null && dtSetPrivilege.Rows.Count > 0)
                    {
                        classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());
                        if (bool.Parse(dtSetPrivilege.Rows[0]["ReadAction"].ToString()).Equals(false))
                        {

                            gvAdvanceInfo.Visible = false;
                        }

                        if (bool.Parse(dtSetPrivilege.Rows[0]["WriteAction"].ToString()).Equals(false))
                        {
                            btnSave.CssClass = "";
                            btnSave.Enabled = false;


                        }
                     
                    }
                }
            }
            catch { }
        }

        private void clearLoanDetailsDatatable()
        {
            try
            {
                dt = new DataTable();
                dt.Columns.Add("LoanDetailsID", typeof(string));
                dt.Columns.Add("TakenDate", typeof(string));
                dt.Columns.Add("Amount", typeof(int));
                dt.Columns.Add("Remarks", typeof(string));               
                gvLoans.DataSource = dt;
                gvLoans.DataBind();
                ViewState["__addedLoanList__"] = dt;
            }
            catch { }
        }
        private void addLoanDetails(string TakenDate,string Amount,string Remarks)
        {
            try
            {
                dt = new DataTable();
                dt = (DataTable)ViewState["__addedLoanList__"];
                string LoanDetailsID= DateTime.Now.ToFileTime().ToString();

                dt.Rows.Add(LoanDetailsID,TakenDate, Amount, Remarks);
                loadLoanDetails(dt);
                clearLoanDetails();

            }
            catch (Exception ex) { lblMessage.InnerText = "error->" + ex.Message; }
        }
        private void updateLoanDetails(string TakenDate, string Amount, string Remarks)
        {
            try
            {
                dt = new DataTable();
                dt = (DataTable)ViewState["__addedLoanList__"];

                foreach (DataRow row in dt.Rows)
                {
                    if (row["LoanDetailsID"].ToString() == ViewState["__loanDetailsID__"].ToString())                    {
                        row["TakenDate"]= TakenDate;
                        row["Amount"] = Amount;
                        row["Remarks"] = Remarks;
                        break;
                    }

                }
                dt.AcceptChanges();
                loadLoanDetails(dt);
                clearLoanDetails();

            }
            catch (Exception ex) { lblMessage.InnerText = "error->" + ex.Message; }
        }
        private void clearLoanDetails()
        {
            txtEntryDate.Text = "";
            txtAdvanceAmount.Text = "";            
            txtRemarks.Text = "";
            btnAdd.Text = "Add";
            ViewState["__loanDetailsID__"] = "";
        }
        private void removeLoanDetails(string loanDetailsID)
        {
            try
            {
                dt = new DataTable();
                dt = (DataTable)ViewState["__addedLoanList__"];                
                foreach (DataRow row in dt.Rows)
                {
                    if (row["LoanDetailsID"].ToString() == loanDetailsID)
                    {
                        row.Delete();
                        break;
                    }
                        
                }
                dt.AcceptChanges();                
                loadLoanDetails(dt);                

            }
            catch (Exception ex) { lblMessage.InnerText = "error->" + ex.Message; }
        }
        private void loadLoanDetails(DataTable dt)
        {
            try
            {
            gvLoans.DataSource = dt;
            gvLoans.DataBind();
            int TotalPrice = dt.AsEnumerable().Sum(row => row.Field<int>("Amount"));
            txtTotalAmount.Text = TotalPrice.ToString();
            ViewState["__addedLoanList__"] = dt;
            }
            catch (Exception ex) { }
        }



        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            lblMessage.InnerText = "";
            if (btnAdd.Text.Trim() == "Update")
                updateLoanDetails(txtEntryDate.Text.Trim(), txtAdvanceAmount.Text.Trim(), txtRemarks.Text.Trim());
            else
                addLoanDetails(txtEntryDate.Text.Trim(),txtAdvanceAmount.Text.Trim(),txtRemarks.Text.Trim());


        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            if (ddlEmpCardNo.SelectedItem.ToString().Trim() == "")
            {
                lblMessage.InnerText = "warning->Please select an employee card no";
                return;
            }
            if (gvLoans==null || gvLoans.Rows.Count==0)
            {
                lblMessage.InnerText = "warning->Please, add particular loan!";
                return;
            }
            if (txtStartMonth.Text.Trim().Length==0)
            {
                lblMessage.InnerText = "warning->Please, select the month of deduction starting!";
                return;
            }
            if (txtInstallmentAmount.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning->Please, enter the instalment amount!";
                return;
            }
            if (saveLoanInfo())
            {
                lblMessage.InnerText = "success->Successfully Submitted.";
                clearLoan();
                loadExistsLoan();
            }
        }
        private void clearLoan()
        {
            ddlEmpCardNo.SelectedValue = "0";
            txtStartMonth.Text = "";
            txtInstallmentAmount.Text = "";
            clearLoanDetailsDatatable();
            btnSave.Text = "Submit";
        }
        private bool saveLoanInfo()
        {
                       
                
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                try
                {
                    query = @"INSERT INTO [dbo].[Payroll_LoanInfo]
                        ([CompanyID]
                       ,[EmpId]
                       ,[LoanAmount]   
                       ,[DeductFrom] 
                       ,[InstallmentAmount]                                                            
                       ,[CreatedAt]
                       ,[CreatedBy]
                       )" + @"
                    VALUES
                       ('"+ddlCompanyList.SelectedValue+"','"+ddlEmpCardNo.SelectedValue+"',"+txtTotalAmount.Text.Trim()+",'"+commonTask.ddMMyyyyTo_yyyyMMdd("01-"+txtStartMonth.Text.Trim())+"',"+txtInstallmentAmount.Text.Trim()+",'"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"',"+ ViewState["__UserId__"].ToString()+ ");​select scope_identity()";
                  int loanID=  CRUD.ExecuteReturnID(query);
                if (loanID > 0)
                {
                    dt = new DataTable();
                    dt = (DataTable)ViewState["__addedLoanList__"];
                    foreach (DataRow row in dt.Rows)
                    {
                        saveLoanDetails(ddlCompanyList.SelectedValue, loanID,commonTask.ddMMyyyyTo_yyyyMMdd(row["TakenDate"].ToString()), row["Amount"].ToString(), row["Remarks"].ToString());
                    }
                    return true;
                }
                return false;
                }
                catch (Exception ex)
                {                
                    lblMessage.InnerText = "error->" + ex.Message;
                return false;
            }

           
        }
        private void saveLoanDetails(string CompanyID, int LoanID,string LoanTakeDate,string ParticularAmount,string ParticularRemarks)
        {
            try
            {
                query = @"INSERT INTO [dbo].[Payroll_LoanDetails]
           ([CompanyID]
           ,[LoanID]
           ,[LoanTakeDate]
           ,[ParticularAmount]
           ,[ParticularRemarks]
           ,[CreatedAt]
           ,[CreatedBy]
           )
            VALUES('"+ CompanyID + "',"+ LoanID + ",'"+ LoanTakeDate + "',"+ ParticularAmount + ",'"+ ParticularRemarks + "','"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"',"+ ViewState["__UserId__"].ToString() + ")";
                CRUD.Execute(query);

            }
            catch (Exception ex)
            { }
        }

      
       


        private void loadExistsLoan()
        {
            try
            {
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                sqlDB.fillDataTable("select SUBSTRING(ei.EmpCardNo,8,6)+' ('+ei.EmpProximityNo+')' as EmpCardNo,ei.EmpId,ei.EmpName, LoanID,LoanAmount,InstallmentAmount,format(DeductFrom,'MM-yyyy') as DeductFrom,Isnull(li.PaidAmount,0) as PaidAmount,li.LoanAmount- Isnull(li.PaidAmount,0) as DueAmount from Payroll_LoanInfo as li inner join Personnel_EmployeeInfo as ei on li.EmpId=ei.EmpId where li.CompanyId='" + CompanyId + "' and  ISNULL(IsPaid,0)=0 and ISNULL(IsExemption,0)=0 and ISNULL(IsDeleted,0)=0 ", dt = new DataTable());
                gvAdvanceInfo.DataSource = dt;
                gvAdvanceInfo.DataBind();
            }
            catch { }
        }

        protected void gvAdvanceInfo_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                loadExistsLoan();
                gvAdvanceInfo.PageIndex = e.NewPageIndex;
                gvAdvanceInfo.DataBind();

            }
            catch (Exception ex)
            {

            }
        }

        protected void gvAdvanceInfo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);

                lblMessage.InnerText = "";
                int index = int.Parse(e.CommandArgument.ToString());
                ViewState["AdvanceId"] = gvAdvanceInfo.DataKeys[index].Value.ToString();
            }
            catch { }
            //Delete(e.CommandArgument.ToString());// btnRefresh_Click(sender, e);
        }

        



        protected void ddlEmpCardNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            string[] getEmpCard = ddlEmpCardNo.SelectedItem.Text.Split(' ');
           
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
        }

     

        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            try
            {
                lblMessage.InnerText = "";
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
               
                classes.Employee.LoadEmpCardNoForPayroll_EmpID(ddlEmpCardNo, CompanyId);

                loadExistsLoan();
            }
            catch { }
        }

     

        protected void gvAdvanceInfo_RowDataBound(object sender, GridViewRowEventArgs e)
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

        protected void gvLoans_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "alter")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                Label lblTakenDate = (Label)gvLoans.Rows[index].FindControl("lblTakenDate");
                Label lblAmount = (Label)gvLoans.Rows[index].FindControl("lblAmount");
                Label lblRemarks = (Label)gvLoans.Rows[index].FindControl("lblRemarks");
                txtEntryDate.Text = lblTakenDate.Text.Trim();
                txtAdvanceAmount.Text = lblAmount.Text.Trim();
                txtRemarks.Text = lblRemarks.Text.Trim();
                btnAdd.Text = "Update";
                ViewState["__loanDetailsID__"] = gvLoans.DataKeys[index].Value.ToString();
            }
            else if(e.CommandName == "removerow")
            {                
                string loanDetailsID = e.CommandArgument.ToString();
                removeLoanDetails(loanDetailsID);
            }
            
        }
    }
}