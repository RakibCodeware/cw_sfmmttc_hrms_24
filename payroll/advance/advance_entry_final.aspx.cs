using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll.advance
{
    public partial class advance_entry_final : System.Web.UI.Page
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
                btnRefundWaive.Visible = false;
                pnlRefundWaive.Visible = false;
                clearLoanDetailsDatatable();
                setPrivilege();
                ddlCompanyList.SelectedValue = ViewState["__CompanyId__"].ToString();
              //  loadExistsLoan();
              
             //   classes.Employee.LoadEmpCardNoForPayroll_EmpID(ddlEmpCardNo, ViewState["__CompanyId__"].ToString());
                ViewState["AdvanceId"] = "0";
                ViewState["__EmpId__"] = "0";
                ViewState["__Action__"] = "";
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
                string EmpId = "";
                string Action = "";
                try
                {
                    Action = Request.QueryString["Action"].ToString();
                    ViewState["__Action__"] = Action;
                    EmpId = Request.QueryString["EmpId"].ToString();
                } catch(Exception ex) { }
                if (EmpId != "")
                {
                    if (Action != "alter")
                    {
                        btnRefundWaive.Visible = true;
                        btnSave.Visible = false;
                        pnlForEdit.Visible = false;
                        pnlRefundWaive.Visible = true;
                        txtInstallmentAmount.Enabled = false;
                        gvLoans.Columns[gvLoans.Columns.Count-1].Visible = false;
                        gvLoans.Columns[gvLoans.Columns.Count-2].Visible = false;
                        if (Action == "refund")
                        {
                            divRefundAmount.Visible = true;
                            pRefundWaiveMsg.InnerText = "Do you want to refund it?";
                            lblRefundWaiveDate.InnerText = "Refund Date";
                        }
                        else if (Action == "paid")
                        {
                            pRefundWaiveMsg.InnerText = "Do you want to paid it?";
                            lblRefundWaiveDate.InnerText = "Paid Date";
                            divRefundAmount.Visible = false;
                        }
                        else
                        {
                            pRefundWaiveMsg.InnerText = "Do you want to waive it?";
                            lblRefundWaiveDate.InnerText = "Waive Date";
                            divRefundAmount.Visible = false;
                        }
                        
                    }
                    classes.Employee.LoadEmpCardNoForPayroll_EmpID(ddlEmpCardNo, ViewState["__CompanyId__"].ToString(), EmpId);
                    ddlCompanyList.Enabled = false;
                    ddlEmpCardNo.SelectedValue = EmpId;
                    ddlEmpCardNo_SelectedIndexChanged();
                    ddlEmpCardNo.Enabled = false;
                }
                else
                    classes.Employee.LoadEmpCardNoForPayroll_EmpID(ddlEmpCardNo, ViewState["__CompanyId__"].ToString());


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

                         //   gvAdvanceInfo.Visible = false;
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
        private void clearLoan()
        {
            ddlEmpCardNo.SelectedValue = "0";
            clearParticalLoan();
        }
        private void clearParticalLoan()
        {
            txtStartMonth.Text = "";
            txtTotalAmount.Text = "";
            txtInstallmentAmount.Text = "";
            clearLoanDetailsDatatable();
            ViewState["__LoanID__"] = "0";
            trPaid.Visible = false;
            trDue.Visible = false;
            btnSave.Text = "Submit";
        }
        private void clearLoanDetails()
        {
            txtEntryDate.Text = "";
            txtAdvanceAmount.Text = "";
            txtRemarks.Text = "";
            btnAdd.Text = "Add";
            ViewState["__loanDetailsID__"] = "";
        }
        private void addLoanDetails(string LoanDetailsID, string TakenDate, string Amount, string Remarks)
        {
            try
            {
                dt = new DataTable();
                dt = (DataTable)ViewState["__addedLoanList__"];
                if (LoanDetailsID == "0")
                {
                    LoanDetailsID = DateTime.Now.ToFileTime().ToString();
                    dt.Rows.Add(LoanDetailsID, TakenDate, Amount, Remarks);
                    loadLoanDetails(dt);
                    clearLoanDetails();
                }
                else
                {
                    dt.Rows.Add(LoanDetailsID, TakenDate, Amount, Remarks);
                    loadLoanDetails(dt);
                }

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
                    if (row["LoanDetailsID"].ToString() == ViewState["__loanDetailsID__"].ToString())
                    {
                        row["TakenDate"] = TakenDate;
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
        private void loadRunningAdvance(string EmpId)
        {
            DataTable dtEx = new DataTable();
            dtEx = CRUD.ExecuteReturnDataTable("select l.LoanID,l.LoanAmount,ISNULL(l.PaidAmount,0) as PaidAmount,l.InstallmentAmount,format( l.DeductFrom,'MM-yyyy') as DeductFrom, IsNull(l.PaidInstallmentNo,0) as PaidInstallmentNo,ld.LoanDetailsID,convert(varchar(10), ld.LoanTakeDate,105) as LoanTakeDate,ld.ParticularAmount,ld.ParticularRemarks from Payroll_LoanInfo l left join Payroll_LoanDetails ld on l.LoanID=ld.LoanID and ISNULL(ld.IsDeleted,0)=0 where l.Status=0 and ISNULL(l.IsDeleted,0)=0 and l.EmpId='" + EmpId + "' order by ld.LoanDetailsID");
            if (dtEx != null && dtEx.Rows.Count > 0)
            {
                ViewState["__LoanID__"] = dtEx.Rows[0]["LoanID"].ToString();
                txtStartMonth.Text = dtEx.Rows[0]["DeductFrom"].ToString();
                txtPaid.Text = dtEx.Rows[0]["PaidAmount"].ToString();
                trPaid.Visible = true;
                trDue.Visible = true;
                txtInstallmentAmount.Text = dtEx.Rows[0]["InstallmentAmount"].ToString();

                if (int.Parse(dtEx.Rows[0]["PaidAmount"].ToString()) > 0)
                    txtStartMonth.Enabled = false;

                for (byte i = 0; i < dtEx.Rows.Count; i++)
                    addLoanDetails(dtEx.Rows[i]["LoanDetailsID"].ToString(), dtEx.Rows[i]["LoanTakeDate"].ToString(), dtEx.Rows[i]["ParticularAmount"].ToString(), dtEx.Rows[i]["ParticularRemarks"].ToString());



            }


        }
        private void loadLoanDetails(DataTable dt)
        {
            try
            {
                gvLoans.DataSource = dt;
                gvLoans.DataBind();
                int TotalPrice = dt.AsEnumerable().Sum(row => row.Field<int>("Amount"));
                txtTotalAmount.Text = TotalPrice.ToString();
                dueCalc();
                ViewState["__addedLoanList__"] = dt;
            }
            catch (Exception ex) { }
        }
        private void dueCalc()
        {
            txtDue.Text = (int.Parse(txtTotalAmount.Text.Trim())
                - int.Parse(txtPaid.Text.Trim())).ToString();
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
        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            try
            {
                lblMessage.InnerText = "";
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();

                classes.Employee.LoadEmpCardNoForPayroll_EmpID(ddlEmpCardNo, CompanyId);
                //loadExistsLoan();
            }
            catch { }
        }

        protected void ddlEmpCardNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlEmpCardNo_SelectedIndexChanged();
        }
        private void ddlEmpCardNo_SelectedIndexChanged()
        {
            clearParticalLoan();
            clearLoanDetails();
            loadRunningAdvance(ddlEmpCardNo.SelectedValue);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
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
            else if (e.CommandName == "removerow")
            {
                string loanDetailsID = e.CommandArgument.ToString();
                removeLoanDetails(loanDetailsID);
            }
        }
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            lblMessage.InnerText = "";
            if (btnAdd.Text.Trim() == "Update")
                updateLoanDetails(txtEntryDate.Text.Trim(), txtAdvanceAmount.Text.Trim(), txtRemarks.Text.Trim());
            else
                addLoanDetails("0", txtEntryDate.Text.Trim(), txtAdvanceAmount.Text.Trim(), txtRemarks.Text.Trim());
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            if (ddlEmpCardNo.SelectedItem.ToString().Trim() == "")
            {
                lblMessage.InnerText = "warning->Please select an employee!";
                return;
            }
            if (gvLoans == null || gvLoans.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Please, add particular advance!";
                return;
            }
            if (txtStartMonth.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning->Please, select the month of deduction starting!";
                return;
            }
            if (txtInstallmentAmount.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning->Please, enter the instalment amount!";
                return;
            }

            if (SubmitLoan())
            {
                lblMessage.InnerText = "success->Successfully Submitted.";
                clearLoan();
                Response.Redirect("/payroll/advance/advance_entry.aspx", false);
            }
        }
        private int _SaveLoanInfo()
        {
            try
            {
                query = @"INSERT INTO [dbo].[Payroll_LoanInfo]
                        ([CompanyID]
                       ,[EmpId]
                       ,[LoanAmount]   
                       ,[DeductFrom] 
                       ,[InstallmentAmount]   
                       ,[Status]
                       ,[CreatedAt]
                       ,[CreatedBy]                            
                       )" + @"
                    VALUES
                       ('" + ddlCompanyList.SelectedValue + "','" + ddlEmpCardNo.SelectedValue + "'," + txtTotalAmount.Text.Trim() + ",'" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtStartMonth.Text.Trim()) + "'," + txtInstallmentAmount.Text.Trim() + ",0,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ViewState["__UserId__"].ToString() + ");​select scope_identity()";
                return CRUD.ExecuteReturnID(query);

            }
            catch (Exception ex) { return 0; }
        }
        private bool _UpdateLoanInfo()
        {
            try
            {
                query = @"UPDATE [dbo].[Payroll_LoanInfo]
                       SET [LoanAmount] =" + txtTotalAmount.Text.Trim() + @"     
                          ,[InstallmentAmount] = " + txtInstallmentAmount.Text.Trim() + @"
                          ,[DeductFrom] ='" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtStartMonth.Text.Trim()) + @"'
                          ,[Status]=0,[UpdatedAt] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                          ,[UpdatedBy] = " + ViewState["__UserId__"].ToString() + @"
                     WHERE LoanID=" + ViewState["__LoanID__"].ToString();
                return CRUD.Execute(query);

            }
            catch (Exception ex) { return false; }
        }

        private bool SubmitLoan()
        {


            string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
            try
            {

                int loanID = 0;
                if (ViewState["__LoanID__"].ToString() == "0")
                    loanID = _SaveLoanInfo();
                else
                {
                    loanID = int.Parse(ViewState["__LoanID__"].ToString());
                    if (!_UpdateLoanInfo()) return false;
                }

                if (loanID > 0)
                {
                    dt = new DataTable();
                    dt = (DataTable)ViewState["__addedLoanList__"];
                    if (ViewState["__LoanID__"].ToString() == "0")
                        foreach (DataRow row in dt.Rows)
                        {
                            _SaveLoanDetails(ddlCompanyList.SelectedValue, loanID, commonTask.ddMMyyyyTo_yyyyMMdd(row["TakenDate"].ToString()), row["Amount"].ToString(), row["Remarks"].ToString());
                        }
                    else
                    {
                        string _LoanDetailsIDs = "";
                        foreach (DataRow row in dt.Rows)
                        {
                            if (checkLoanDetailsID(loanID.ToString(), row["LoanDetailsID"].ToString()))
                            {
                                _LoanDetailsIDs += "," + row["LoanDetailsID"].ToString();
                                _UpdateLoanDetails(loanID, row["LoanDetailsID"].ToString(), commonTask.ddMMyyyyTo_yyyyMMdd(row["TakenDate"].ToString()), row["Amount"].ToString(), row["Remarks"].ToString());
                            }
                            else
                                _LoanDetailsIDs += "," + _SaveLoanDetails(ddlCompanyList.SelectedValue, loanID, commonTask.ddMMyyyyTo_yyyyMMdd(row["TakenDate"].ToString()), row["Amount"].ToString(), row["Remarks"].ToString());
                        }
                        _LoanDetailsIDs = _LoanDetailsIDs.Remove(0, 1);
                        _softDeleteLoanDetails(loanID, _LoanDetailsIDs, "");

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
       
        private bool checkLoanDetailsID(string LoanID, string LoanDetailsID)
        {
            try
            {
                DataTable dtLoanDetails = new DataTable();
                dtLoanDetails = CRUD.ExecuteReturnDataTable("select * from Payroll_LoanDetails where LoanID=" + LoanID + " and LoanDetailsID=" + LoanDetailsID);
                if (dtLoanDetails != null && dtLoanDetails.Rows.Count > 0)
                    return true;
                else return false;
            }
            catch { return false; }
        }
        private int _SaveLoanDetails(string CompanyID, int LoanID, string LoanTakeDate, string ParticularAmount, string ParticularRemarks)
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
            VALUES('" + CompanyID + "'," + LoanID + ",'" + LoanTakeDate + "'," + ParticularAmount + ",'" + ParticularRemarks + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + ViewState["__UserId__"].ToString() + ");​select scope_identity()";
                return CRUD.ExecuteReturnID(query);

            }
            catch (Exception ex)
            { return 0; }
        }
        private void _UpdateLoanDetails(int LoanID, string LoanDetailsID, string LoanTakeDate, string ParticularAmount, string ParticularRemarks)
        {
            try
            {
                query = @"UPDATE [dbo].[Payroll_LoanDetails]
                        SET [LoanTakeDate] = '" + LoanTakeDate + @"'
                            ,[ParticularAmount] =" + ParticularAmount + @"
                            ,[ParticularRemarks] = '" + ParticularRemarks + @"'      
                            ,[UpdatedAt] = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                            ,[UpdatedBy] =" + ViewState["__UserId__"].ToString() + @"  
                        WHERE LoanID=" + LoanID + " and LoanDetailsID=" + LoanDetailsID;
                CRUD.Execute(query);

            }
            catch (Exception ex)
            { }
        }
        private void _softDeleteLoanDetails(int LoanID, string LoanDetailsIDs, string DeletedReason)
        {
            try
            {
                query = @"UPDATE [dbo].[Payroll_LoanDetails]
                        SET [IsDeleted] =1
                            ,[DeletedAt] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"',[DeletedBy] =" + ViewState["__UserId__"].ToString() + @"
                            ,[DeletedReason] = '" + DeletedReason + @"'
                        WHERE IsNull(IsDeleted,0)=0 and LoanID=" + LoanID + " and LoanDetailsID not in (" + LoanDetailsIDs + ")";
                CRUD.Execute(query);
            }
            catch (Exception ex)
            { }
        }

        protected void btnRefundWaive_Click(object sender, EventArgs e)
        {
            if (ddlEmpCardNo.SelectedItem.ToString().Trim() == "")
            {
                lblMessage.InnerText = "warning->Please select an employee!";
                return;
            }
            if (txtRefundWaiveDate.Text.Trim().Length != 10)
            {
              //  lblMessage.InnerText = "warning->Please select "+ ViewState["__Action__"].ToString() + " date!";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "showMessage('Please select " + ViewState["__Action__"].ToString() + " date!', 'warning');", true);
                txtRefundWaiveDate.Focus();
                return;
            }
            if (divRefundAmount.Visible)
            {
                try { int a = int.Parse(txtRefund.Text.Trim());} catch(Exception ex)
                {
                  //  lblMessage.InnerText = "warning->Please enter valid refund amount!";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "showMessage('Please enter valid refund amount!', 'warning');", true);
                    txtRefund.Focus();
                    return;
                }
              
            }
            if (txtComment.Text.Trim() == "")
            {
            //    lblMessage.InnerText = "warning->Please enter important comment!";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "showMessage('Please enter important comment!', 'warning');", true);
                txtComment.Focus();
                return;
            }
            _RefundWaive();

        }
        private void _RefundWaive()
        {
            try
            {
                string msg = "";
                query = "";
                if (ViewState["__Action__"].ToString() == "refund")// status=2
                {
                    query = @"UPDATE [dbo].[Payroll_LoanInfo]
                       SET [Status]=2,[RefundAmount]=" + txtRefund.Text.Trim().ToString() + ",[StatusDate]='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtRefundWaiveDate.Text.Trim()) + "',[StatusNote]=N'" + txtComment.Text.Trim()+"',[StatusUpdatedAt] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                          ,[StatusUpdatedBy] = " + ViewState["__UserId__"].ToString() + @"
                     WHERE LoanID=" + ViewState["__LoanID__"].ToString();
                    msg = "Refund";
                }                    
                else if (ViewState["__Action__"].ToString() == "waive")// status=3
                {
                    query = @"UPDATE [dbo].[Payroll_LoanInfo]
                       SET [Status]=3,[StatusDate]='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtRefundWaiveDate.Text.Trim()) + "',[StatusNote]=N'" + txtComment.Text.Trim() + "',[StatusUpdatedAt] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                          ,[StatusUpdatedBy] = " + ViewState["__UserId__"].ToString() + @"
                     WHERE LoanID=" + ViewState["__LoanID__"].ToString();
                    msg = "Waived";
                }
                else if (ViewState["__Action__"].ToString() == "paid")// status=1
                {
                    query = @"UPDATE [dbo].[Payroll_LoanInfo]
                       SET [Status]=1,[StatusDate]='" + commonTask.ddMMyyyyTo_yyyyMMdd(txtRefundWaiveDate.Text.Trim()) + "',[StatusNote]=N'" + txtComment.Text.Trim() + "',[StatusUpdatedAt] ='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"'
                          ,[StatusUpdatedBy] = " + ViewState["__UserId__"].ToString() + @"
                     WHERE LoanID=" + ViewState["__LoanID__"].ToString();
                    msg = "Paid";
                }

                if (CRUD.Execute(query))
                {
                    SaveAttachDocument(ViewState["__LoanID__"].ToString());
                //    lblMessage.InnerText = "success-> Successfully "+msg;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "showMessage('Successfully "+ msg + "', 'success');", true);
                Response.Redirect("/payroll/advance/advance_entry.aspx", false);
                }

            }
            catch (Exception ex) {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "showMessage('Unable to submit.(" + ex.Message + ")', 'error');", true);
                // lblMessage.InnerText = "error-> Unable to submit.("+ex.Message+")";
            }
        }
        private void SaveAttachDocument(string filename)
        {
            try
            {
                if (FileUploadDoc.HasFile)
                {
                    if (System.IO.File.Exists(Server.MapPath("/EmployeeImages/AdvanceDocument/" + filename + ".jpg"))) 
                    {
                        System.IO.File.Delete(Server.MapPath("/EmployeeImages/AdvanceDocument/" + filename + ".jpg"));
                    }
                    FileUploadDoc.SaveAs(Server.MapPath("/EmployeeImages/AdvanceDocument/" + filename + ".jpg"));

                }
            }
            catch (Exception ex) { }
        }

    }
  
}