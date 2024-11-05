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

namespace SigmaERP.payroll
{
    public partial class earnleave_payment_sheetc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                classes.commonTask.LoadEmpType(rblEmployeeType);
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
                ViewState["__IsGerments__"] = classes.commonTask.IsGarments();
                //if (!bool.Parse(ViewState["__IsGerments__"].ToString()))
                //    trHideForIndividual.Visible = false;

            }
        }
        DataTable dtSetPrivilege;
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CShortName__"] = getCookies["__CShortName__"].ToString();
                //------------load privilege setting inof from db------
                string[] AccessPermission = new string[0];
                AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "salary_sheet_Report.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
                ViewState["__ReadAction__"] = AccessPermission[0];
                commonTask.LoadDepartmentByCompanyInListBox(ViewState["__CompanyId__"].ToString(), lstAll);
                classes.Payroll.loadEarnleaveMonthIdByCompanyc(ddlSelectMonth, ViewState["__CompanyId__"].ToString());
                //-----------------------------------------------------
            }
            catch { }
        }


        protected void rblGenerateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {


                if (!rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    txtEmpCardNo.Enabled = true;
                    pnl1.Enabled = false;
                    trHideForIndividual.Visible = false;
                    pnl1.Visible = false;
                    txtEmpCardNo.Focus();

                }
                else
                {
                    txtEmpCardNo.Enabled = false;
                    pnl1.Enabled = true;
                    trHideForIndividual.Visible = true;
                    pnl1.Visible = true;
                    rblEmployeeType.SelectedValue = "1";
                }
            }
            catch { }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (ddlSelectMonth.SelectedValue == "0")
            { lblMessage.InnerText = "warning->Please select any Month!"; ddlSelectMonth.Focus(); return; }
            if (rblGenerateType.SelectedItem.Text.Equals("All") && lstSelected.Items.Count < 1) { lblMessage.InnerText = "warning->Please select any Department"; lstSelected.Focus(); return; }
            if (rblReportType.SelectedValue.Equals("Sheet"))
            {
                if (!rblGenerateType.SelectedItem.Text.Equals("All") && txtEmpCardNo.Text.Trim().Length < 4) { lblMessage.InnerText = "warning->Please type valid Card No!(Minimum last 4 digit.)"; txtEmpCardNo.Focus(); return; }
                generateSalarySheet();
            }
            else
            {
                generateEarnLeaveSummary();
            }

        }
        private void generateEarnLeaveSummary()
        {
            try
            {
                string CompanyList = "";
                string DepartmentList = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }

                CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                string Condition = "";
                if (rblEmployeeType.SelectedValue != "All")
                    Condition = " And EmpTypeId=" + rblEmployeeType.SelectedValue + "";

                string isSeperated = "", startDate = "", endDate = "";
                string[] values = ddlSelectMonth.SelectedValue.Split('/');
                isSeperated = values[0];
                startDate = values[1];
                endDate = values[2];

                string getSQLCMD;
                DataTable dt = new DataTable();

                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);

                getSQLCMD = "Select Count(EmpID) as ActiveDay,sum(case when TotalAmount>0 then 1 else 0 end )as PayableDays, sum(round( TotalAmount,0)) as TotalOTAmount,DptId,DptName,CompanyId,CompanyName,Address, EmpTypeID FROM v_Payroll_EarnLeavePaymentSheet1 " +
                     " where " +
                     " IsSeparated='" + isSeperated + "' and convert(varchar(10),StartDate,120)='" + startDate + "' and convert(varchar(10),EndDate,120)='" + endDate + "' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "    " + Condition +
                     " Group By DptId,DptName,CompanyId,CompanyName,Address, EmpTypeID " +
                     " ORDER BY CONVERT(int,DptId) ";


                sqlDB.fillDataTable(getSQLCMD, dt);
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No data found."; return;
                }
                Session["__EarnleavePaymentSummary__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=EarnleavePaymentSummary-" + ddlSelectMonth.SelectedItem.Text.Replace('-', '/') + "');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }
        private void generateSalarySheet()
        {
            try
            {
                string CompanyList = "";
                string DepartmentList = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }

                CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                string Condition = "";
                if (rblEmployeeType.SelectedValue != "All")
                    Condition = " And EmpTypeId=" + rblEmployeeType.SelectedValue + "";

                string isSeperated = "", startDate = "", endDate = "";
                string[] values = ddlSelectMonth.SelectedValue.Split('/');
                isSeperated = values[0];
                startDate = values[1];
                endDate = values[2];

                string getSQLCMD;
                DataTable dt = new DataTable();
                if (rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    if (rblPayableType.SelectedValue == "1")
                        Condition += " and TotalAmount>0";
                    else if (rblPayableType.SelectedValue == "0")
                        Condition += " and TotalAmount<1";
                    DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                    getSQLCMD = " SELECT DptId, CompanyId, DsgName,EmpId, EmpName, EmpPresentSalary, DptName, GrdName, CompanyName, Address,SUBSTRING(EmpCardNo,10,4) as EmpCardNo, PayableEarnLeaveDays, WithdrawableEarnLeaveDays, PayableAmount, OneDaySalary, convert(varchar(10), EmpJoiningDate,105) as EmpJoiningDate,preJan,pJan,pFeb,pMar,pApr,pMay,pJun,pJul,pAug,pSep,pOct,pNov,pDec,nDec,(pJan+pFeb+pMar+pApr+pMay+pJun+pJul+pAug+pSep+pOct+pNov+pDec) as TotalPresent,StampDeductions,round(TotalAmount,0) as TotalAmount,WithdrawableEarnLeavePer,TotalEarnLeaveDays,SepntEarnLeaveDays,EmpType,EmpTypeID,BasicSalary,ReserveEeanLeaveDays,GName,EmpProximityNo,CurrentYearEarnLeaveDays,PreviousYearEarnLeaveDays FROM v_Payroll_EarnLeavePaymentSheet1 " +
                         " where " +
                         " IsSeparated='" + isSeperated + "' and convert(varchar(10),StartDate,120)='" + startDate + "' and convert(varchar(10),EndDate,120)='" + endDate + "' and CompanyId  in(" + CompanyList + ") and DptId " + DepartmentList + "    " + Condition +
                         " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";

                }
                else
                {

                    getSQLCMD = " SELECT DptId, CompanyId, DsgName,EmpId, EmpName, EmpPresentSalary, DptName, GrdName, CompanyName, Address,SUBSTRING(EmpCardNo,10,4) as EmpCardNo, PayableEarnLeaveDays, WithdrawableEarnLeaveDays, PayableAmount, OneDaySalary, convert(varchar(10), EmpJoiningDate,105) as EmpJoiningDate,preJan,pJan,pFeb,pMar,pApr,pMay,pJun,pJul,pAug,pSep,pOct,pNov,pDec,nDec,(pJan+pFeb+pMar+pApr+pMay+pJun+pJul+pAug+pSep+pOct+pNov+pDec) as TotalPresent,StampDeductions,round(TotalAmount,0) as TotalAmount,WithdrawableEarnLeavePer,TotalEarnLeaveDays,SepntEarnLeaveDays,EmpType,EmpTypeID,BasicSalary,ReserveEeanLeaveDays,GName,EmpProximityNo,CurrentYearEarnLeaveDays,PreviousYearEarnLeaveDays FROM v_Payroll_EarnLeavePaymentSheet1 " +
                          " where " +
                          " IsSeparated='" + isSeperated + "' and convert(varchar(10),StartDate,120)='" + startDate + "' and convert(varchar(10),EndDate,120)='" + endDate + "' and CompanyId  in(" + CompanyList + ") and  EmpCardNo Like'%" + txtEmpCardNo.Text.Trim() + "'    " +
                          " ORDER BY CONVERT(int,DptId),convert(int,Gid), CustomOrdering";

                }
                sqlDB.fillDataTable(getSQLCMD, dt);
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No data found."; return;
                }
                //foreach (DataRow dtRow in dt.Rows)
                //{
                //    int preJan = int.Parse(dtRow["preJan"].ToString());
                //    int nDec = int.Parse(dtRow["nDec"].ToString());
                //    getSQLCMD = " select  EmpID,Month,PreMonthDays,NextMonthDays from Earnleave_MonthlyInfo1 where EmpID='" + dtRow["EmpID"] + "' and Month>='" + startDate + "' and  Month<='" + endDate + "'  order by Month ";
                //    DataTable dtMonth = new DataTable();
                //    sqlDB.fillDataTable(getSQLCMD, dtMonth);
                //    if (dtMonth != null && dtMonth.Rows.Count > 0)
                //    {
                //        if (dtRow.Table.Columns["TotalPresent"].ReadOnly)
                //        {
                //            // If read-only, set it to read-write
                //            dtRow.Table.Columns["TotalPresent"].ReadOnly = false;
                //        }

                //        int totalPresent = int.Parse(dtRow["TotalPresent"].ToString());
                //        if (preJan == 0)
                //        {
                //            string PreMonthDays = dtMonth.Rows[0]["PreMonthDays"].ToString();
                //            dtRow["preJan"] = PreMonthDays;
                //            totalPresent = totalPresent + int.Parse(PreMonthDays);
                //        }

                //        int currentYearEarnLeaveDays = int.Parse(dtRow["CurrentYearEarnLeaveDays"].ToString());

                //        dtRow["nDec"] = totalPresent - (currentYearEarnLeaveDays * 18);
                //        dtRow["TotalPresent"] = currentYearEarnLeaveDays * 18;

                //    }
                //}
                Session["__EarnleavePaymentSheet__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=EarnleavePaymentSheetc-" + ddlSelectMonth.SelectedItem.Text.Replace('-', '/') + "');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                classes.commonTask.LoadDepartmentByCompanyInListBox(CompanyId, lstAll);
                //classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                //addAllTextInShift();
                classes.Payroll.loadEarnleaveMonthIdByCompanyc(ddlSelectMonth, CompanyId);
            }
            catch { }
        }


        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }
        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }
    }
}