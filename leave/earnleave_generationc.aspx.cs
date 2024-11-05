using adviitRuntimeScripting;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.leave
{
    public partial class earnleave_generationc : System.Web.UI.Page
    {
        DataTable dt;
        string sqlCmd = "";
        string CompanyID = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                ViewState["___IsGerments__"] = classes.Payroll.Office_IsGarments();
                ViewState["__FindName__"] = "No";
                classes.commonTask.LoadBranch(ddlCompanyList);
                HttpCookie getCookies = Request.Cookies["userInfo"];
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                loadGenerationList();
            }
        }

        private void loadGenerationList()
        {
            try
            {
                CompanyID = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue;
                sqlCmd = "select distinct CompanyID,convert(varchar(10),GenerateDate,102) as GenerateDate, format(GenerateDate,'MMM-yyyy') as GenerateMonth from Earnleave_BalanceDetailsLog1 where CompanyID='" + CompanyID + "' order by GenerateDate desc";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                gvEarnLeaveGenerationList.DataSource = dt;
                gvEarnLeaveGenerationList.DataBind();

            }
            catch (Exception ex) { }

        }
        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            generateEarnleave();
        }
        private void generateEarnleave()
        {
            try
            {
                lblErrorMsg.Text = "";
                lblErrorMsg.Text += "generateEarnleave->57";
                CompanyID = (ddlCompanyList.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlCompanyList.SelectedValue;
                string EmpID = "";
                if (txtEmpCardNo.Text.Trim() != "")
                {
                    sqlCmd = "select EmpId from Personnel_EmployeeInfo where CompanyId='" + CompanyID + "' and EmpCardNo like'%" + txtEmpCardNo.Text.Trim() + "'";
                    sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        EmpID = dt.Rows[0]["EmpId"].ToString();
                    }
                    else
                    {
                        lblMessage.InnerText = "warning-> Invalid Cardno!";
                        return;
                    }
                }
                sqlCmd = "select convert(varchar(10),ELStartDate,120) as ELStartDate,Status,DurationDays from Earnleave_Setting1";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                lblErrorMsg.Text += ",76";
                if (dt.Rows.Count > 0)
                {
                    lblErrorMsg.Text += ",79";
                    DateTime ELStartDate = DateTime.Parse(dt.Rows[0]["ELStartDate"].ToString());
                    string[] _Status = dt.Rows[0]["Status"].ToString().Split(',');
                    string Status = "";
                    foreach (string item in _Status)
                    {
                        Status += ",'" + item + "'";
                    }
                    Status = Status.Remove(0, 1);
                    int DurationDays = int.Parse(dt.Rows[0]["DurationDays"].ToString());
                    string generateDate = classes.commonTask.ddMMyyyyTo_yyyyMMdd(txtGenerateMonth.Text);
                    if (EmpID == "")
                        sqlCmd = "select distinct EmpId,convert(varchar(10),EmpJoiningDate,120) as EmpJoiningDate  from v_tblAttendanceRecord1 where    AttStatus in(" + Status + ") and CompanyId = '" + CompanyID + "' and  ATTDate>= '" + generateDate.Substring(0, 7) + "-01" + "' and ATTDate<= '" + generateDate + "' ";
                    else
                        sqlCmd = "select distinct EmpId,convert(varchar(10),EmpJoiningDate,120) as EmpJoiningDate  from v_tblAttendanceRecord1 where     AttStatus in(" + Status + ") and CompanyId = '" + CompanyID + "' and  ATTDate>= '" + generateDate.Substring(0, 7) + "-01" + "' and ATTDate<= '" + generateDate + "' and EmpID='" + EmpID + "'";

                    DataTable dtEmp = new DataTable();
                    dtEmp = CRUD.ExecuteReturnDataTable(sqlCmd, sqlDB.connection);// load att employees
                    lblErrorMsg.Text += ",97";
                    if (dtEmp != null && dtEmp.Rows.Count > 0)
                        for (int i = 0; i < dtEmp.Rows.Count; i++)
                        {
                            lblErrorMsg.Text += "\n,101";
                            DateTime EmpJoingDate = DateTime.Parse(dtEmp.Rows[i]["EmpJoiningDate"].ToString());
                            EmpID = dtEmp.Rows[i]["EmpID"].ToString();
                            deleteEarnleaveBalance(generateDate.Substring(0, 7) + "-01", EmpID);// delete existing data
                            string StartDate = checkLastEarnLeaveDate(EmpID, (EmpJoingDate > ELStartDate) ? EmpJoingDate.ToString("yyyy-MM-dd") : ELStartDate.ToString("yyyy-MM-dd"), generateDate);
                            sqlCmd = "select convert(varchar(10),ATTDate,120) as ATTDate from v_tblAttendanceRecord1 where IsActive=1 and AttStatus in(" + Status + ") and EmpID='" + EmpID + "'  and ATTDate>= '" + StartDate + "' and ATTDate<= '" + generateDate + "' order by convert(varchar(10), ATTDate, 120)";
                            DataTable dtAttRecords = new DataTable();
                            dtAttRecords = CRUD.ExecuteReturnDataTable(sqlCmd, sqlDB.connection);// load attendance records for an employee
                            if (dtAttRecords == null)
                                lblErrorMsg.Text += ",dtAttRecords is null[" + sqlCmd + "]";
                            else
                                lblErrorMsg.Text += ",dtAttRecords is " + dtAttRecords.Rows.Count;
                            lblErrorMsg.Text += ",109";
                            string EarnLeaveLastDate = "";
                            if (dtAttRecords != null && dtAttRecords.Rows.Count >= DurationDays)
                            {
                                lblErrorMsg.Text += ",112";
                                int ELDays = dtAttRecords.Rows.Count / DurationDays;
                                int LastElDateIndex = ELDays * DurationDays;
                                EarnLeaveLastDate = dtAttRecords.Rows[LastElDateIndex - 1]["ATTDate"].ToString();

                                sqlCmd = "INSERT INTO [dbo].[Earnleave_BalanceDetailsLog1] ([CompanyID],[EmpID],[EarnLeaveLastDate],[GenerateDate],[EarnLeaveDays],[EntryDatetime])" +
                                " VALUES('" + CompanyID + "','" + EmpID + "','" + EarnLeaveLastDate + "','" + generateDate.Substring(0, 7) + "-01" + "'," + ELDays + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                                lblErrorMsg.Text += ",119";
                                classes.CRUD.Execute(sqlCmd, sqlDB.connection);
                                lblErrorMsg.Text += ",121";
                            }
                            lblErrorMsg.Text += ",123";
                            saveEarnleaveMonthlyInformation(ddlCompanyList.SelectedValue, Status, generateDate.Substring(0, 7) + "-01", generateDate, EmpID, StartDate, EarnLeaveLastDate);
                            lblErrorMsg.Text += ",125";

                        }

                }
                loadGenerationList();
                lblMessage.InnerText = "success-> Successfully Generated.";
                //imgLoading.Visible = false;
            }
            catch (Exception ex)
            {
                lblErrorMsg.Text += ",136";
            }
        }
        private string checkLastEarnLeaveDate(string EmpID, string InitialDate, string GenerateDate)
        {
            try
            {
                sqlCmd = " select  ISNULL(convert(varchar(10), DATEADD(DAY,1,max(EarnLeaveLastDate)),120),'" + InitialDate + "') as EarnLeaveLastDate from Earnleave_BalanceDetailsLog1 where  EmpID='" + EmpID + "'  and EarnLeaveLastDate<= '" + GenerateDate + "'";
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(sqlCmd, sqlDB.connection);
                return dt.Rows[0]["EarnLeaveLastDate"].ToString();
            }
            catch (Exception ex) { return InitialDate; }

        }
       
        //private void saveEarnleaveMonthlyInformation(string CompanyID, string Status, string Month, string ToDate, string EmpID)
        //{
        //    try
        //    {

        //        deleteEarnleaveMonthlyInformation(CompanyID, Month, EmpID);
        //        if (EmpID == "")
        //            sqlCmd = "select EmpId,count(EmpId) as P  from v_tblAttendanceRecord1 where IsActive=1 and CompanyID='" + CompanyID + "' and ATTStatus in(" + Status + ")  and ATTDate>=(case when EmpJoiningDate>'" + Month + "' then EmpJoiningDate else '" + Month + "' end)  and ATTDate<='" + ToDate + "' and EmpJoiningDate<'" + ToDate + "'  group by EmpId";
        //        else
        //            sqlCmd = "select EmpId,count(EmpId) as P  from v_tblAttendanceRecord1 where IsActive=1 and CompanyID='" + CompanyID + "'  and ATTStatus in(" + Status + ") and ATTDate>=(case when EmpJoiningDate>'" + Month + "' then EmpJoiningDate else '" + Month + "' end) and ATTDate<='" + ToDate + "' and EmpJoiningDate<'" + ToDate + "' and EmpID='" + EmpID + "' group by EmpId";
        //        sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
        //        if (dt != null && dt.Rows.Count > 0)
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                sqlCmd = "INSERT INTO [dbo].[Earnleave_MonthlyInfo1] ([CompanyID],[EmpID],[Month],[PresentDays],[EntryTime])" +
        //                    " VALUES('" + CompanyID + "','" + dt.Rows[i]["EmpID"].ToString() + "','" + Month + "','" + dt.Rows[i]["P"].ToString() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
        //                classes.CRUD.Execute(sqlCmd, sqlDB.connection);
        //            }
        //    }
        //    catch (Exception ex) { }
        //}
        private void saveEarnleaveMonthlyInformation(string CompanyID, string Status, string Month, string ToDate, string EmpID, string _StartDate, string _LastDateOfEL)
        {
            try
            {
                int PresentDays = 0;
                int PreMonthDays = 0;
                int NextMonthDays = 0;
                deleteEarnleaveMonthlyInformation(CompanyID, Month, EmpID);
                //---get present month's days ---
                sqlCmd = "select EmpId,count(EmpId) as P  from v_tblAttendanceRecord1 where IsActive=1 and CompanyID='" + CompanyID + "'  and ATTStatus in(" + Status + ")  and ATTDate>=(case when EmpJoiningDate>'" + Month + "' then EmpJoiningDate else '" + Month + "' end) and ATTDate<='" + ToDate + "' and EmpJoiningDate<'" + ToDate + "' and EmpID='" + EmpID + "' group by EmpId";
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(sqlCmd, sqlDB.connection);
                if (dt != null && dt.Rows.Count > 0)
                {
                    PresentDays = int.Parse(dt.Rows[0]["P"].ToString());
                }
                //---end present month's days ---
                //---get previous month's days for this month ---
                sqlCmd = "select EmpId,count(EmpId) as P  from v_tblAttendanceRecord where IsActive=1 and CompanyID='" + CompanyID + "'  and ATTStatus in(" + Status + ") and PaybleDays=1 and ATTDate>=(case when EmpJoiningDate>'" + _StartDate + "' then EmpJoiningDate else '" + _StartDate + "' end) and ATTDate<'" + Month + "' and EmpJoiningDate<'" + Month + "' and EmpID='" + EmpID + "' group by EmpId";
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable(sqlCmd, sqlDB.connection);
                if (dt != null && dt.Rows.Count > 0)
                {
                    PreMonthDays = int.Parse(dt.Rows[0]["P"].ToString());
                }
                //---end previous month's days for this month ---

                //---get this month's days for next month ---
                if (_LastDateOfEL != "")
                {

                    sqlCmd = "select EmpId,count(EmpId) as P  from v_tblAttendanceRecord1 where IsActive=1 and CompanyID='" + CompanyID + "'  and ATTStatus in(" + Status + ")  and ATTDate>=(case when EmpJoiningDate>'" + DateTime.Parse(_LastDateOfEL).AddDays(1).ToString("yyyy-MM-dd") + "' then EmpJoiningDate else '" + DateTime.Parse(_LastDateOfEL).AddDays(1).ToString("yyyy-MM-dd") + "' end) and ATTDate<='" + ToDate + "' and EmpJoiningDate<'" + ToDate + "' and EmpID='" + EmpID + "' group by EmpId";
                    dt = new DataTable();
                    dt = CRUD.ExecuteReturnDataTable(sqlCmd, sqlDB.connection);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        NextMonthDays = int.Parse(dt.Rows[0]["P"].ToString());
                    }
                }
                //---end previous month's days for this month ---
                sqlCmd = "INSERT INTO [dbo].[Earnleave_MonthlyInfo1] ([CompanyID],[EmpID],[Month],[PresentDays],[EntryTime],[PreMonthDays],[NextMonthDays])" +
                            " VALUES('" + CompanyID + "','" + EmpID + "','" + Month + "','" + PresentDays + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'," + PreMonthDays + "," + NextMonthDays + ")";
                classes.CRUD.Execute(sqlCmd, sqlDB.connection);
            }
            catch (Exception ex) { }
        }
        private bool deleteEarnleaveMonthlyInformation(string CompanyID, string Month, string EmpID)
        {
            try
            {
                if (EmpID == "")
                    sqlCmd = "delete Earnleave_MonthlyInfo1 where CompanyID='" + CompanyID + "' and Month='" + Month + "'";
                else
                    sqlCmd = "delete Earnleave_MonthlyInfo1 where CompanyID='" + CompanyID + "'  and Month='" + Month + "' and EmpID='" + EmpID + "'";
                return CRUD.Execute(sqlCmd, sqlDB.connection);

            }
            catch (Exception ex) { lblMessage.InnerText = "error-> " + ex.Message; return false; }
        }
        private bool deleteEarnleaveBalance(string Month, string EmpID)
        {
            try
            {
                sqlCmd = "delete Earnleave_BalanceDetailsLog1 where EmpID='" + EmpID + "' and GenerateDate='" + Month + "'";

                return CRUD.Execute(sqlCmd, sqlDB.connection);

            }
            catch (Exception ex) { lblMessage.InnerText = "error-> " + ex.Message; return false; }
        }

        protected void gvEarnLeaveGenerationList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {


                if (e.CommandName == "Remove")
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());
                    string CompanyID = gvEarnLeaveGenerationList.DataKeys[rIndex].Values[0].ToString();
                    string GenerateDate = gvEarnLeaveGenerationList.DataKeys[rIndex].Values[1].ToString();
                    if (deleteExEarnLeave(CompanyID, GenerateDate))
                    {
                        deleteEarnleaveMonthlyInformation(CompanyID, GenerateDate, "");
                        lblMessage.InnerText = "success-> Successfully Deleted.";
                        gvEarnLeaveGenerationList.Rows[rIndex].Visible = false;
                        imgLoading.Visible = false;
                    }
                }
            }
            catch { }
        }
        private bool deleteExEarnLeave(string CompanyID, string GenerateDate)
        {
            try
            {
                sqlCmd = "delete Earnleave_BalanceDetailsLog1 where CompanyID='" + CompanyID + "' and GenerateDate='" + GenerateDate + "'";
                return CRUD.Execute(sqlCmd, sqlDB.connection);

            }
            catch (Exception ex) { lblMessage.InnerText = "error-> " + ex.Message; return false; }

        }


    }
}