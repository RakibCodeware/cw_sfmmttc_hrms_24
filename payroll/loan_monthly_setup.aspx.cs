using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ComplexScriptingSystem;
using System.Drawing;
using System.Data.SqlClient;
using SigmaERP.classes;

namespace SigmaERP.payroll
{
    public partial class loan_monthly_setup : System.Web.UI.Page
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
                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                txtDate.Text = DateTime.Now.ToString("MM-yyyy");
                ViewState["__EditSuccessStatus__"] = "True";
                ddlCompanyList.SelectedIndex = 0;
                lblTtile.Text = "Advanced List For Accept Advance Installment Of " + DateTime.Now.ToString("MM-yyyy");
                setPrivilege();               
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
            }
        }

        DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {
                ViewState["__WriteAction__"] = "1";
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Master Admin"))
                {
                    classes.commonTask.LoadBranch(ddlCompanyList);

                    return;
                }
                else
                {
                    classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());

                    if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Admin"))
                    {

                        gvAdvaceList.Visible = false;
                    }
                    else
                    {
                        btnSet.CssClass = "";
                        btnSet.Enabled = false;
                    }

                    dtSetPrivilege = new DataTable();
                    sqlDB.fillDataTable("select * from UserPrivilege where PageName='advancsetting.aspx' and UserId=" + getCookies["__getUserId__"].ToString() + "", dt);
                    if (dtSetPrivilege.Rows.Count > 0)
                    {
                        if (bool.Parse(dtSetPrivilege.Rows[0]["ReadAction"].ToString()).Equals(true))
                        {

                            gvAdvaceList.Visible = true;
                        }

                        if (bool.Parse(dtSetPrivilege.Rows[0]["WriteAction"].ToString()).Equals(true))
                        {
                            btnSet.CssClass = "";
                            btnSet.Enabled = false;


                        }
                    }
                }
            }
            catch { }
        }

        private void loadExistsLoan()
        {
            try
            {
                
                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                query = "with lms as(select* from Payroll_LoanMonthlySetup where CompanyID = '" + CompanyId + "' and Month = '" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtDate.Text.Trim()) + "') select ISNULL(lms.SL,0) as SL,li.CompanyID,li.LoanID, li.EmpId,li.LoanAmount,ISNULL(li.PaidAmount,0) as PaidAmount,li.LoanAmount-ISNULL(li.PaidAmount,0) as DueAmount,ISNULL(PaidInstallmentNo,0) as PaidInstallmentNo,case when ISNULL(lms.Amount,li.InstallmentAmount)>( li.LoanAmount-ISNULL(li.PaidAmount,0)) then li.LoanAmount-ISNULL(li.PaidAmount,0) else ISNULL(lms.Amount,li.InstallmentAmount) end as InstallmentAmount,Format(li.DeductFrom,'MM-yyyy') as DeductFrom,'" + txtDate.Text.Trim() + "' as Month, '" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtDate.Text.Trim()) + "' as MonthID,ed.EmpName,ed.EmpType,SUBSTRING(ed.EmpCardNo,8,6)+' ('+ed.EmpProximityNo+')' as EmpCardNo,li.InstallmentAmount as DefaultInstallmentAmount from Payroll_LoanInfo li left join lms on li.EmpId=lms.EmpId inner join v_EmployeeDetails as ed on li.EmpId=ed.EmpId and ed.IsActive=1 where li.CompanyID = '" + CompanyId + "' and ISNULL(li.IsPaid, 0) = 0 and ISNULL(li.IsExemption, 0) = 0 ";
                if (rblEmpType.SelectedValue != "All")
                    query += " and ed.EmpTypeId=" + rblEmpType.SelectedValue;
                sqlDB.fillDataTable(query, dt = new DataTable());
                if (dt.Rows.Count > 0) gvAdvaceList.DataSource = dt;
                else gvAdvaceList.DataSource = null;
                gvAdvaceList.DataBind();
            }
            catch { }
        }

        protected void btnSet_Click(object sender, EventArgs e)
        {

            saveLoanMonthlySetup();
        }

        private void saveLoanMonthlySetup()
        {
            try
            {
                

                if (ViewState["__EditSuccessStatus__"].ToString() == "False")
                {
                    lblMessage.InnerText = "warning->Please must be complete edit status of installment number !";
                    return;
                }
               
              
                
                   
                    foreach( GridViewRow row in gvAdvaceList.Rows)
                    {
                        CheckBox chk = (CheckBox)row.FindControl("SelectCheckBox");
                        string SL = gvAdvaceList.DataKeys[row.RowIndex].Values[0].ToString();
                        string LoanID = gvAdvaceList.DataKeys[row.RowIndex].Values[1].ToString();
                        string EmpID = gvAdvaceList.DataKeys[row.RowIndex].Values[2].ToString();
                        string CompanyID = gvAdvaceList.DataKeys[row.RowIndex].Values[3].ToString();
                        string MonthID = gvAdvaceList.DataKeys[row.RowIndex].Values[4].ToString();
                        string Amount = ((TextBox)row.FindControl("txtInstallmentAmount")).Text.Trim();
                        CRUD.Execute("delete from Payroll_LoanMonthlySetup where SL="+SL);
                        if (chk.Checked)
                        {
                            CRUD.Execute(@"INSERT INTO[dbo].[Payroll_LoanMonthlySetup]
                                        ([LoanID]
                                        ,[EmpID]
                                        ,[CompanyID]
                                        ,[Month]
                                        ,[Amount]
                                        )VALUES
                                        ("+ LoanID + ",'"+ EmpID + "','"+ CompanyID + "','"+ MonthID + "',"+ Amount + ")");
                        }
                    }           
                
                    lblMessage.InnerText = "success-> Successfully  Setting Saved";
                    loadExistsLoan();
                




            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
            }
        }

        private bool checkedAlreadyAssigned(out DataTable dtAssigneList)
        {

            sqlDB.fillDataTable("select  *  from Payroll_AdvanceSetting inner join Payroll_AdvanceInfo on Payroll_AdvanceSetting.AdvanceId=Payroll_AdvanceInfo.AdvanceId where  PaidStatus='False' and PaidMonth='" + txtDate.Text + "' and CompanyId='" + ddlCompanyList.SelectedValue + "'", dtAssigneList = new DataTable());
            if (dtAssigneList.Rows.Count > 0) return true;
            else return false;

        }

        private bool checkAlreadySetForThisMonth()  // test already exists ?
        {
            try
            {
                sqlDB.fillDataTable("select PaidMonth from Payroll_AdvanceSetting where PaidMonth='" + DateTime.Now.ToString("MM-yyyy") + "'", dt = new DataTable());
                if (dt.Rows.Count > 0)
                {
                    lblMessage.InnerText = "warning->Sorry,Already set advance installment for this month !";
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }
        protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";

            string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();

            loadExistsLoan();
        }

        protected void ddlShiftList_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadExistsLoan();
        }

        protected void gvAdvaceList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                lblMessage.InnerText = "";



                int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                Button btnEdit = (Button)gvAdvaceList.Rows[rIndex].FindControl("btnEdit");
                TextBox txtInstallmentAmount = (TextBox)gvAdvaceList.Rows[rIndex].FindControl("txtInstallmentAmount");
                Label lblDueAmount = (Label)gvAdvaceList.Rows[rIndex].FindControl("lblDueAmount");

                if (e.CommandName.Equals("Alter"))
                {

                    if (btnEdit.Text.Equals("Edit"))
                    {
                        if (ViewState["__EditSuccessStatus__"].ToString() == "False")
                        {
                            lblMessage.InnerText = "warning->Please must be complete previous installment number !";
                            return;
                        }


                        txtInstallmentAmount.Enabled = true;

                        btnEdit.Text = "Ok";
                        btnEdit.ForeColor = Color.Red;
                        txtInstallmentAmount.Style.Add("border-style", "solid");
                        txtInstallmentAmount.Style.Add("border-color", "#0000ff");
                        ViewState["__EditSuccessStatus__"] = "False";
                    }
                    else
                    {
                        if (int.Parse(lblDueAmount.Text.Trim())<int.Parse(txtInstallmentAmount.Text.Trim()))
                        {
                            lblMessage.InnerText = "error->Cut. Installment Amount is larger than Due Amount!";
                            return;
                        }
                        btnEdit.Text = "Edit";
                        btnEdit.ForeColor = Color.Green;
                        ViewState["__EditSuccessStatus__"] = "True";                        
                        txtInstallmentAmount.Style.Add("border-style", "solid");
                        txtInstallmentAmount.Style.Add("border-color", "#116530");
                        txtInstallmentAmount.Enabled = false;
                    }
                }
            }
            catch { }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            loadExistsLoan();
            lblTtile.Text = "Advanced List For Accept Advance Installment Of " + txtDate.Text.Trim();
        }

    }
}