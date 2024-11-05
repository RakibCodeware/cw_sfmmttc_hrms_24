using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using ComplexScriptingSystem;
using adviitRuntimeScripting;
using SigmaERP.classes;

namespace SigmaERP.payroll
{
    public partial class ot_payment_sheet : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompanyName.Enabled = false;
                ddlCompanyName.SelectedValue = ViewState["__CompanyId__"].ToString();
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

                classes.commonTask.LoadBranch(ddlCompanyName, ViewState["__CompanyId__"].ToString());
                //------------load privilege setting inof from db------
                //string[] AccessPermission = new string[0];
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "ot_payment_sheet.aspx", ddlCompanyName, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];
                commonTask.LoadDepartmentByCompanyInListBox(ViewState["__CompanyId__"].ToString(), lstAll);
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, ViewState["__CompanyId__"].ToString());
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, ViewState["__CompanyId__"].ToString());
                //-----------------------------------------------------


               
            }
            catch { }
        }

        private void addAllTextInShift()
        {
            if (ddlShiftName.Items.Count > 2)
                ddlShiftName.Items.Insert(1, new ListItem("All", "00"));
        }
        protected void rblGenerateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                

                if (!rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    txtEmpCardNo.Enabled = true;
                    pnl1.Enabled = false;
                    ddlShiftName.Enabled = false;
                }
                else
                {
                    txtEmpCardNo.Enabled = false;
                    pnl1.Enabled = true;
                    ddlShiftName.Enabled = true;
                    txtEmpCardNo.Focus();
                }
            }
            catch { }
        }

        protected void ddlCompanyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                //classes.commonTask.LoadShift(ddlShiftName, CompanyId);
                //addAllTextInShift();
                classes.commonTask.LoadDepartmentByCompanyInListBox(CompanyId, lstAll);
                classes.Payroll.loadMonthIdByCompany(ddlSelectMonth, CompanyId);
            }
            catch { }
        }

        protected void ddlShiftName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lstAll.Items.Clear();
                lstSelected.Items.Clear();
                string CompanyId = (ddlCompanyName.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();

                if (ddlShiftName.SelectedItem.ToString().Equals("All"))
                {
                    string ShiftList = classes.commonTask.getShiftList(ddlShiftName);
                    classes.commonTask.LoadDepartmentByCompanyAndShiftInListBox(CompanyId, ShiftList, lstAll);
                    return;
                }
                classes.commonTask.LoadDepartmentByCompanyAndShiftInListBox(CompanyId, "in (" + ddlShiftName.SelectedValue.ToString() + ")", lstAll);
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

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (ddlSelectMonth.SelectedValue == "0") { lblMessage.InnerText = "warning->Please select any Month!"; ddlSelectMonth.Focus(); return; }
            if (rblReportType.SelectedValue == "Sheet")
                generateOTPaymentSheet();
            else
                generateOTPaymentSummary();

        }
        private void generateOTPaymentSummary()
        {
            try
            {
                if (lstSelected.Items.Count == 0)
                {
                    lblMessage.InnerText = "warning-> Please select department.";
                    lstAll.Focus();
                    return;
                }
                string[] monthInfo = ddlSelectMonth.SelectedValue.Split('/');
                string yearMonth = "";
                if (monthInfo.Length > 1)
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "' AND FromDate='" + monthInfo[1] + "' AND ToDate='" + monthInfo[2] + "'";
                else
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "'";
                string CompanyList = "";
                string DepartmentList = "";
                CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                string getSQLCMD;
                DataTable dt = new DataTable();
                getSQLCMD = " with s as( SELECT sum(round(case when TotalOTAmount<100 then 0 else TotalOTAmount end,0)) as TotalOTAmount,sum(round(case when TotalOTAmount<100 then TotalOTAmount  else 0 end,0)) as NetPayable ,COUNT(EmpId) as  EmpId, sum((convert(int,Substring(TotalOTHour, 1,Charindex(':', TotalOTHour)-1))*3600 ) + (convert(int,Substring(Substring(TotalOTHour, Charindex(':', TotalOTHour)+1, LEN(TotalOTHour)), 1,Charindex(':', Substring(TotalOTHour, Charindex(':', TotalOTHour)+1, LEN(TotalOTHour)))-1))*60) + convert(int,Substring(Substring(TotalOTHour, Charindex(':', TotalOTHour)+1, LEN(TotalOTHour)), Charindex(':', Substring(TotalOTHour, Charindex(':', TotalOTHour)+1, LEN(TotalOTHour)))+1, LEN(Substring(TotalOTHour, Charindex(':', TotalOTHour)+1, LEN(TotalOTHour)))))) as Total_Seconds , CompanyId, CompanyName, Address,DptId, DptName, format(YearMonth,'MMMM-yyyy') as YearMonth,IsSeperationGeneration " +
                " FROM v_MonthlySalarySheet where IsSeperationGeneration=" + rblSheet.SelectedValue + " and IsActive = 1  " + yearMonth + " and DptID " + DepartmentList+
                " and CompanyId = '"+ CompanyList + "' AND  TotalOTAmount>0   group by CompanyId, CompanyName, Address,DptId, DptName,YearMonth,IsSeperationGeneration) " +
                "select EmpId as Activeday, Total_Seconds as AbsentDay, convert(varchar, (Total_Seconds / 3600))+':' + convert(varchar, ((Total_Seconds % 3600) / 60)) + ':' + convert(varchar, ((Total_Seconds % 3600) % 60)) as TotalOverTime,CompanyId,CompanyName,Address, DptId,DptName,YearMonth,IsSeperationGeneration, TotalOTAmount,NetPayable from s ORDER BY CONVERT(int, DptId)";                
                sqlDB.fillDataTable(getSQLCMD, dt);               
                if (dt==null || dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No data found."; return;
                }
                Session["__Language__"] = "English";
                Session["__OvertimePmtSummary__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=OvertimePmtSummary');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }
        private void generateOTPaymentSheet()
        {
            try
            {
                string CompanyList = "";
               
                string DepartmentList = "";

                if (!Page.IsValid)   // If Java script are desible then 
                {
                    lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
                }
                string[] monthInfo = ddlSelectMonth.SelectedValue.Split('/');
                string yearMonth = "";
                if (monthInfo.Length > 1)
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "' AND FromDate='" + monthInfo[1] + "' AND ToDate='" + monthInfo[2] + "'";
                else
                    yearMonth = " AND YearMonth='" + monthInfo[0] + "'";


                CompanyList = (ddlCompanyName.SelectedValue.Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyName.SelectedValue.ToString();
                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                string getSQLCMD;
                DataTable dt = new DataTable();
                if (rblGenerateType.SelectedItem.Text.Equals("All"))
                {
                    getSQLCMD = " SELECT EmpName,substring(EmpCardNo,8,15) as EmpCardNo,BasicSalary,"
                                + " TotalOTHour,OTRate,round(TotalOTAmount,0) as TotalOTAmount,DsgName,"
                                + " DptId,DptName,CompanyName,SftName,Address,FORMAT(YearMonth,'MMMM-yyyy') as YearMonth,CompanyId,GId,GName ,TotalOTHour,TotalOtherOverTime,TotalOTHour,IsSeperationGeneration,EmpProximityNo "
                                + " FROM"
                                + " v_MonthlySalarySheet"
                                + " where IsSeperationGeneration="+rblSheet.SelectedValue+" and "
                                + " IsActive = 1  and CompanyId in('" + CompanyList + "') "+ yearMonth + " AND dptId  " + DepartmentList + " AND  TotalOTAmount>0"
                                + " ORDER BY "
                                + " CONVERT(int,DptId),convert(int,Gid), CustomOrdering";
                }
                else
                {

                    getSQLCMD = " SELECT EmpName,substring(EmpCardNo,8,15) as EmpCardNo,BasicSalary,"
                                + " TotalOTHour,OTRate,round(TotalOTAmount,0) as TotalOTAmount,DsgName,"
                                + " DptId,DptName,CompanyName,SftName,Address,FORMAT(YearMonth,'MMMM-yyyy') as YearMonth,CompanyId,GId ,GName,TotalOTHour,TotalOtherOverTime,TotalOTHour,IsSeperationGeneration,EmpProximityNo "
                                + " FROM"
                                + " v_MonthlySalarySheet"
                                + " where IsSeperationGeneration=" + rblSheet.SelectedValue + " and "
                                + " IsActive = 1  and CompanyId ='" + CompanyList + "' " + yearMonth + " AND EmpCardNo Like '%" + txtEmpCardNo.Text.Trim() + "' ";
                                
                              
                }

                sqlDB.fillDataTable(getSQLCMD, dt);

                /*
                SqlCommand cmd = new SqlCommand("Payroll_MonthlySalary_Payslip",sqlDB.connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@YearMonth",ddlSelectMonth.SelectedValue.ToString());
                cmd.Parameters.AddWithValue("@CompanyId",CompanyList);
                cmd.Parameters.AddWithValue("@shiftId", ShiftList);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt=new DataTable();
                da.Fill(dt);
                */

                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->No data found"; return;
                }

                Session["__Language__"] = "English";
                Session["__OvertimeSheet__"] = dt;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=OvertimeSheet-" + ddlSelectMonth.SelectedItem.Text + "');", true);  //Open New Tab for Sever side code
            }
            catch { }
        }
    }
}


