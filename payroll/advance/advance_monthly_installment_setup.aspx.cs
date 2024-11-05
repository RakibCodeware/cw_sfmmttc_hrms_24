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
using SigmaERP.hrms.BLL;

namespace SigmaERP.payroll.advance
{
    public partial class advance_monthly_installment_setup : System.Web.UI.Page
    {
        //permission {View=354 Add=355 Delete=356}
    DataTable dt;
        string query = "";
        int InstallmentTotalAmount = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["__ReadAction__"] = "0";
            ViewState["__WriteAction__"] = "0";
            ViewState["__UpdateAction__"] = "0";

            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            int[] pagePermission = { 354,355,356 };
            if (!IsPostBack)
            {
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);


                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                txtDate.Text = DateTime.Now.ToString("MM-yyyy");
                ViewState["__EditSuccessStatus__"] = "True";
                ddlCompanyList.SelectedIndex = 0;
                lblTtile.Text = "Advanced List For Accept Advance Installment Of " + DateTime.Now.ToString("MM-yyyy");
                setPrivilege(userPagePermition);
                if (!classes.commonTask.HasBranch())
                    ddlCompanyList.Enabled = false;
            }
        }

        DataTable dtSetPrivilege;
        private void setPrivilege(int[]permissions)
        {
            try
            {
                ViewState["__WriteAction__"] = "1";
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                //if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Master Admin"))
                //{
                //    classes.commonTask.LoadBranch(ddlCompanyList);

                //    return;
                //}
                //else
                //{
                    classes.commonTask.LoadBranch(ddlCompanyList, ViewState["__CompanyId__"].ToString());

                    //if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Admin"))
                    //{

                    //    gvAdvaceList.Visible = false;
                    //}
                    //else
                    //{
                    //    btnSet.CssClass = "";
                    //    btnSet.Enabled = false;
                    //}

                    //dtSetPrivilege = new DataTable();
                  //  sqlDB.fillDataTable("select * from UserPrivilege where PageName='advancsetting.aspx' and UserId=" + getCookies["__getUserId__"].ToString() + "", dt);
                    //if (dtSetPrivilege.Rows.Count > 0)
                    //{
                    //    if (bool.Parse(dtSetPrivilege.Rows[0]["ReadAction"].ToString()).Equals(true))
                    //    {

                    //        gvAdvaceList.Visible = true;
                    //    }

                    //    if (bool.Parse(dtSetPrivilege.Rows[0]["WriteAction"].ToString()).Equals(true))
                    //    {
                    //        btnSet.CssClass = "";
                    //        btnSet.Enabled = false;


                    //    }
                    //}

                   if(permissions.Contains(354))
                            ViewState["__ReadAction__"] = "1";
                    if (permissions.Contains(355))
                        ViewState["__WriteAction__"] = "1";
                    if (permissions.Contains(356))
                        ViewState["__UpdateAction__"] = "1";
                    checkInitialPermission();
                //}
            }
            catch { }
        }

        private void loadExistsLoan()
        {
            try
            {

                string CompanyId = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue.ToString();
                //query = "with lms as(select* from Payroll_LoanMonthlySetup where CompanyID = '" + CompanyId + "' and Month = '" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtDate.Text.Trim()) + "') select ISNULL(lms.SL,0) as SL,li.CompanyID,li.LoanID, li.EmpId,li.LoanAmount,ISNULL(li.PaidAmount,0) as PaidAmount,li.LoanAmount-ISNULL(li.PaidAmount,0) as DueAmount,ISNULL(PaidInstallmentNo,0) as PaidInstallmentNo,case when ISNULL(lms.Amount,li.InstallmentAmount)>( li.LoanAmount-ISNULL(li.PaidAmount,0)) then li.LoanAmount-ISNULL(li.PaidAmount,0) else ISNULL(lms.Amount,li.InstallmentAmount) end as InstallmentAmount,Format(li.DeductFrom,'MM-yyyy') as DeductFrom,'" + txtDate.Text.Trim() + "' as Month, '" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtDate.Text.Trim()) + "' as MonthID,ed.EmpName,ed.EmpType,SUBSTRING(ed.EmpCardNo,8,6)+' ('+ed.EmpProximityNo+')' as EmpCardNo,li.InstallmentAmount as DefaultInstallmentAmount from Payroll_LoanInfo li left join lms on li.EmpId=lms.EmpId inner join v_EmployeeDetails as ed on li.EmpId=ed.EmpId and ed.IsActive=1 where li.CompanyID = '" + CompanyId + "' and li.Status=0 ";

                query = @"WITH lms AS (
                        SELECT *
                        FROM Payroll_LoanMonthlySetup
                        WHERE CompanyID = '" + CompanyId + "' AND Month = '" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtDate.Text.Trim()) + "') SELECT ISNULL(lms.SL, 0) AS SL, li.CompanyID, li.LoanID, li.EmpId, li.LoanAmount, ISNull( SUM(lmd.Amount),0) AS PaidAmount, li.LoanAmount - ISNull( SUM(lmd.Amount),0) AS DueAmount, ISNull (count(lmd.Amount),0) AS PaidInstallmentNo, CASE WHEN ISNULL(lms.Amount, li.InstallmentAmount) > (li.LoanAmount - ISNULL(li.PaidAmount, 0)) THEN li.LoanAmount - ISNULL(li.PaidAmount, 0) ELSE ISNULL(lms.Amount, li.InstallmentAmount) END AS InstallmentAmount, Format(li.DeductFrom, 'MM-yyyy') AS DeductFrom, '" + txtDate.Text.Trim() + "' AS Month, '" + commonTask.ddMMyyyyTo_yyyyMMdd("01-" + txtDate.Text.Trim()) + "' AS MonthID, ed.EmpName, ed.EmpType, SUBSTRING(ed.EmpCardNo, 8, 6) + ' (' + ed.EmpProximityNo + ')' AS EmpCardNo, li.InstallmentAmount AS DefaultInstallmentAmount  FROM  Payroll_LoanInfo li  LEFT JOIN  lms ON li.EmpId = lms.EmpId  INNER JOIN   v_EmployeeDetails AS ed ON li.EmpId = ed.EmpId AND ed.IsActive = 1 LEFT JOIN   Payroll_LoanMonthlySetup AS lmd ON li.LoanID = lmd.LoanID WHERE   li.CompanyID = '" + CompanyId + "' AND li.Status = 0 ";

                if (rblEmpType.SelectedValue != "All")
                    query += " and ed.EmpTypeId=" + rblEmpType.SelectedValue;

                query += "GROUP BY  lms.SL,lms.Amount,  li.CompanyID, li.LoanID,   li.EmpId, li.LoanAmount, li.PaidAmount,  li.DeductFrom, ed.EmpName,  ed.EmpType,   ed.EmpCardNo,  ed.EmpProximityNo,  li.InstallmentAmount, PaidInstallmentNo";
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




                foreach (GridViewRow row in gvAdvaceList.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("SelectCheckBox");
                    string SL = gvAdvaceList.DataKeys[row.RowIndex].Values[0].ToString();
                    string LoanID = gvAdvaceList.DataKeys[row.RowIndex].Values[1].ToString();
                    string EmpID = gvAdvaceList.DataKeys[row.RowIndex].Values[2].ToString();
                    string CompanyID = gvAdvaceList.DataKeys[row.RowIndex].Values[3].ToString();
                    string MonthID = gvAdvaceList.DataKeys[row.RowIndex].Values[4].ToString();
                    string Amount = ((TextBox)row.FindControl("txtInstallmentAmount")).Text.Trim();
                    CRUD.Execute("delete from Payroll_LoanMonthlySetup where SL=" + SL);
                    if (chk.Checked)
                    {
                        CRUD.Execute(@"INSERT INTO[dbo].[Payroll_LoanMonthlySetup]
                                        ([LoanID]
                                        ,[EmpID]
                                        ,[CompanyID]
                                        ,[Month]
                                        ,[Amount]
                                        )VALUES
                                        (" + LoanID + ",'" + EmpID + "','" + CompanyID + "','" + MonthID + "'," + Amount + ")");
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
                        if (int.Parse(lblDueAmount.Text.Trim()) < int.Parse(txtInstallmentAmount.Text.Trim()))
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
                        InstallmentTotal();
                    }
                }
            }
            catch { }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            InstallmentTotalAmount = 0;
            loadExistsLoan();
            lblTtile.Text = "Advanced List For Accept Advance Installment Of " + txtDate.Text.Trim();
        }
     
        protected void gvAdvaceList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox SelectCheckBox = (CheckBox)e.Row.FindControl("SelectCheckBox");
                if (SelectCheckBox.Checked)
                {
                    TextBox txtInstallmentAmount = (TextBox)e.Row.FindControl("txtInstallmentAmount");
                    InstallmentTotalAmount += int.Parse(txtInstallmentAmount.Text.Trim());
                }

            }
            if (e.Row.RowType == DataControlRowType.Footer)
            {
                Label lblTotalInstallmentAmount = (Label)e.Row.FindControl("lblTotalInstallmentAmount");
                lblTotalInstallmentAmount.Text = InstallmentTotalAmount.ToString();
            }

            try
            {
                if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                {
                    Button btnEdit = (Button)e.Row.FindControl("btnEdit");
                    btnEdit.Enabled = false;
                    btnEdit.ForeColor= Color.Silver;

                }
            }
            catch 
            {

                
            }
 
        }

       
        private void InstallmentTotal()
        {
            try
            {
                InstallmentTotalAmount = 0;
                foreach (GridViewRow row in gvAdvaceList.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox SelectCheckBox = (CheckBox)row.FindControl("SelectCheckBox");
                        if (SelectCheckBox.Checked)
                        {
                            TextBox txtInstallmentAmount = (TextBox)row.FindControl("txtInstallmentAmount");
                            InstallmentTotalAmount += int.Parse(txtInstallmentAmount.Text.Trim());
                        }
                    }
                  
                }
                Label lblTotalInstallmentAmount = (Label)gvAdvaceList.FooterRow.FindControl("lblTotalInstallmentAmount");
                lblTotalInstallmentAmount.Text = InstallmentTotalAmount.ToString();

            }
            catch (Exception ex) { }
        }

        protected void SelectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            InstallmentTotal();
        }

        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("0"))
            {
                btnSet.Enabled = false;
                btnSet.CssClass = "";
            }
            else
            {
                btnSet.Enabled = true;
                btnSet.CssClass = "Pbutton";
            }
        }
    }
}