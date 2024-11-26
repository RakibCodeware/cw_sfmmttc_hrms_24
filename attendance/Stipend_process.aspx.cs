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
                bool isSuccess = generateStipend(ddlCourseList.SelectedValue);
                if (isSuccess)
                {
                    lblMessage.InnerText = "Stipend generated Successfull";
                }
                else
                {
                    lblMessage.InnerText = "Stipend generated Failed";
                }
            }
            else
                lblMessage.InnerText = "Please select Course";


        }

        private bool generateStipend(string courseID)
        {
            try
            {
                string query = "select ecs.EmpId,ecs.DptId,hd.DptName,hd.CourseStartDate,hd.CourseEndDate,hd.StipendAmount from Personnel_EmpCurrentStatus ecs left join HRD_Department hd on ecs.DptId=hd.DptId where ecs.DptId='"+ courseID + "'";
                DataTable dt = CRUD.ExecuteReturnDataTable(query);
                foreach (DataRow row in dt.Rows)
                {
                    string empId = row["EmpId"].ToString();
                    string dptId = row["DptId"].ToString();
                    decimal StipendAmount = Decimal.Parse(row["StipendAmount"].ToString());

                    stipendCalculationIndividually(empId, dptId, txtFormdate.Text.Trim().ToString(), txttodate.Text.Trim().ToString(), StipendAmount);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }


    

        private void stipendCalculationIndividually(string empId, string dptId,string fromDate, string toDate,decimal StipndAmount)
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
                    presntRation = (presntDays * 100) / activeDays;

                if (presntRation >= 80)
                {
                    string insertQuery = "Insert into StudentMonthlyStipend (StudentId,CourseID,StartDate,EndDate,ActiveDays,WHDays,PresentDays,PresentRatio,Amount) values('" + empId + "'," + dptId + ",'" + fromDate + "','" + toDate + "'," + activeDays + "," + wh + "," + presntDays + ",'" + presntRation + "'," + StipndAmount + ")";
                    CRUD.ExecuteReturnID(insertQuery);
                }
           
            }
            catch (Exception ex)
            {

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
    }
}