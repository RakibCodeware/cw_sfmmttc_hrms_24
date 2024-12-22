using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.attendance
{
    public partial class Stipend_process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                classes.commonTask.LoadBranch(ddlCompany);
                ddlCompany.SelectedIndex = 1;
                classes.commonTask.loadCourseListByCompany(ddlCourseList,ddlCompany.SelectedValue);
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            if (ddlCourseList.SelectedIndex > 0)
            {
                DataTable dt = generateStipend(ddlCourseList.SelectedValue);
                gvstudentList.DataSource = dt;
                gvstudentList.DataBind();
        
            }
            else
                lblMessage.InnerText = "warning->Please Select Course Name.";


        }

        private DataTable generateStipend(string courseID)
        {
            DataTable stipendDataTable = new DataTable();
            try
            {
                string query = "select ecs.EmpId,ei.EmpName,ecs.DptId,hd.DptName,hd.CourseStartDate,hd.CourseEndDate,hd.StipendAmount from Personnel_EmpCurrentStatus ecs left join Personnel_EmployeeInfo ei on ecs.EmpId=ei.EmpId left join HRD_Department hd on ecs.DptId=hd.DptId where ecs.DptId='" + courseID + "'";
                DataTable dt = CRUD.ExecuteReturnDataTable(query);

                // Create a DataTable to hold stipend data
           
                stipendDataTable.Columns.Add("StudentId", typeof(string));
                stipendDataTable.Columns.Add("StudentName", typeof(string));
                stipendDataTable.Columns.Add("DptId", typeof(string));
                stipendDataTable.Columns.Add("CourseName", typeof(string));
                stipendDataTable.Columns.Add("StartDate", typeof(DateTime));
                stipendDataTable.Columns.Add("EndDate", typeof(DateTime));
                stipendDataTable.Columns.Add("ActiveDays", typeof(int));
                stipendDataTable.Columns.Add("WHDays", typeof(int));
                stipendDataTable.Columns.Add("PresentDays", typeof(int));
                stipendDataTable.Columns.Add("PresentRatio", typeof(decimal));
                stipendDataTable.Columns.Add("Amount", typeof(decimal));

                // Process each row in the query result
                foreach (DataRow row in dt.Rows)
                {
                    string empId = row["EmpId"].ToString();
                    string dptId = row["DptId"].ToString();
                    string dptname= row["DptName"].ToString();
                    string studentName= row["EmpName"].ToString();
                    decimal StipendAmount = Decimal.Parse(row["StipendAmount"].ToString());

                    // Perform stipend calculations and populate the DataTable
                    stipendCalculationIndividually(empId, studentName, dptname, dptId, txtFormdate.Text.Trim(), txttodate.Text.Trim(), StipendAmount, stipendDataTable);
                }

                // Bind the DataTable to the GridView
               

                return stipendDataTable;
            }
            catch (Exception ex)
            {
                return stipendDataTable;
            }
        }

        private void stipendCalculationIndividually(string empId,string studentName,string CourseName,string dptId, string fromDate, string toDate, decimal StipndAmount, DataTable stipendDataTable)
        {
            try
            {
                decimal presntRation = 0;
                int wh = getWeekendHolydayes(fromDate, toDate);
                DateTime fromDat = DateTime.Parse(fromDate);
                DateTime toDat = DateTime.Parse(toDate);
                int totalDays = (toDat - fromDat).Days + 1;
                int activeDays = totalDays - wh;
                int presntDays = getStudentPresntDays(empId, fromDate, toDate);

                if (presntDays != 0)
                {
                    presntRation = (presntDays * 100) / activeDays;
                }

                if (presntRation >= 10)
                {
                    // Add a new row to the DataTable
                    DataRow row = stipendDataTable.NewRow();
                    row["StudentId"] = empId;
                    row["StudentName"] = studentName;
                    row["DptId"] = dptId;

                    row["CourseName"] = CourseName;
                    row["StartDate"] = DateTime.Parse(fromDate);
                    row["EndDate"] = DateTime.Parse(toDate);
                    row["ActiveDays"] = activeDays;
                    row["WHDays"] = wh;
                    row["PresentDays"] = presntDays;
                    row["PresentRatio"] = presntRation;
                    row["Amount"] = StipndAmount;

                    stipendDataTable.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw;
            }
        }




        private int getWeekendHolydayes(string formDate,string toDate)
        {
            try
            {
                string query = "select HDate from tblHolydayWork where HDate>='" + formDate + "' and HDate<='" + toDate + "'  union all select WeekendDate from Attendance_WeekendInfo where WeekendDate >= '" + formDate + "' and WeekendDate<= '" + toDate + "'";
                DataTable dt = CRUD.ExecuteReturnDataTable(query);
                return dt.Rows.Count;
            }
            catch(Exception ex)
            {
                return 0;
            }
            
        }

        private int getStudentPresntDays(string empId,string formDate,string toDate)
        {

          
            string query = "select  count(EmpId) as PresntDays from tblAttendanceRecord where EmpId = " + empId + " and StateStatus = 'Present' and AttDate>='"+ formDate + "' and ATTDate<='"+ toDate + "'";
            DataTable dt = CRUD.ExecuteReturnDataTable(query);
            if (dt.Rows.Count > 0)
            {
                int presntDays = int.Parse(dt.Rows[0]["PresntDays"].ToString());
                return presntDays;
            }
            else
                return 0;
          
        }


        protected void hdChk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckBox chk = (CheckBox)gvstudentList.HeaderRow.FindControl("hdChk");
                if (chk.Checked)
                {
                    foreach (GridViewRow row in gvstudentList.Rows)
                    {
                        chk = (CheckBox)row.Cells[4].FindControl("chkStatus");
                        chk.Checked = true;

                    }
                }
                else
                {
                    foreach (GridViewRow row in gvstudentList.Rows)
                    {
                        chk = (CheckBox)row.Cells[4].FindControl("chkStatus");
                        chk.Checked = false;

                    }
                }


            }
            catch { }
        }

        protected void chkStatus_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                GridViewRow gvr = ((GridViewRow)((Control)sender).Parent.Parent);
                int index_row = gvr.RowIndex;

                CheckBox chk = (CheckBox)gvstudentList.Rows[index_row].Cells[9].FindControl("chkStatus");

                byte Action = (chk.Checked) ? (byte)1 : (byte)0;

                //--for checked and select header rows----------------------------------------
                byte checkedRowsAmount = 0;
                CheckedRowsAmount(4, "chkStatus", out checkedRowsAmount);
                chk = (CheckBox)gvstudentList.HeaderRow.FindControl("hdChk");

                if (checkedRowsAmount == gvstudentList.Rows.Count)
                {

                    chk.Checked = true;
                }
                else { chk.Checked = false; }
                //----------------------------------------------------------------------------
            }
            catch { }
        }

        private void CheckedRowsAmount(byte cIndex, string ControlName, out byte checkedRowsAmount)
        {
            try
            {
                byte i = 0;
                foreach (GridViewRow gvr in gvstudentList.Rows)
                {
                    CheckBox chk = (CheckBox)gvr.Cells[cIndex].FindControl(ControlName);
                    if (chk.Checked) i++;
                }
                checkedRowsAmount = i;
            }
            catch { checkedRowsAmount = 0; }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvstudentList.Rows)
            {
                string courseId = gvstudentList.DataKeys[row.RowIndex].Value.ToString();
                //string courseId = row.Cells[2].Text;
                //decimal amount = decimal.Parse(row.Cells[3].Text); 
                CheckBox chkStatus = (CheckBox)row.FindControl("chkStatus");
                bool isSelected = chkStatus != null && chkStatus.Checked;
                
                if (isSelected)
                {
                    string studenId = row.Cells[1].Text; // Column 1: StudentId
                    string studentName = row.Cells[2].Text; // Column 2: StudentName
                    string courseName = row.Cells[3].Text; // Column 3: CourseName
                    string startDate = row.Cells[4].Text; // Column 4: StartDate
                    string endDate = row.Cells[5].Text; // Column 5: EndDate
                    string activeDays = row.Cells[6].Text; // Column 6: ActiveDays
                    string whDays = row.Cells[7].Text; // Column 7: WHDays
                    string presentDays = row.Cells[8].Text; // Column 8: PresentDays
                    string presentRatio = row.Cells[9].Text; // Column 9: PresentRatio
                    string stipend_amount = row.Cells[10].Text; // Column 10: Amount

                    // Parse data as required
                
        
                    int activeDaysInt = int.Parse(activeDays);
                    int whDaysInt = int.Parse(whDays);
                    int presentDaysInt = int.Parse(presentDays);
                    decimal presentRatioDecimal = decimal.Parse(presentRatio);
                    decimal amountDecimal = decimal.Parse(stipend_amount);

                    string insertQuery = "Insert into StudentMonthlyStipend (StudentId,CourseID,StartDate,EndDate,ActiveDays,WHDays,PresentDays,PresentRatio,Amount) values('" + studenId + "'," + courseId + ",'" + startDate + "','" + endDate + "'," + activeDaysInt + "," + whDaysInt + "," + presentDaysInt + ",'" + presentRatioDecimal + "'," + amountDecimal + ")";
                    CRUD.ExecuteReturnID(insertQuery);
                    

                }
            }
        }
    }
}