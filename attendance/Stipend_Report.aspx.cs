using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.attendance
{
    public partial class Stipend_Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                classes.commonTask.LoadBranch(ddlCompany);
                ddlCompany.SelectedIndex = 1;
                classes.commonTask.loadCourseListByCompany(ddlCourseList, ddlCompany.SelectedValue);

            }
          
        }

        private void loadStipendList(string courseId,string companyId)
        {
            
            string query = "select StudentId,ef.EmpName,hd.DptName,DATEDIFF(MONTH, hd.CourseStartDate, hd.CourseEndDate) AS TotalMonths,stp.PresentDays,stp.Amount,'' as Signature from StudentMonthlyStipend stp inner join Personnel_EmployeeInfo ef on stp.StudentId=ef.EmpId inner join Personnel_EmpCurrentStatus ecs on  ef.empid=ecs.EmpId and ecs.IsActive=1 inner join HRD_Department hd on ecs.DptId=hd.DptId   where stp.CourseID='"+ courseId + "' and ecs.CompanyId='" + companyId + "'";
            DataTable dt = CRUD.ExecuteReturnDataTable(query);
            if (dt.Rows.Count > 0)
            {
                gvstipendList.DataSource = dt;
                gvstipendList.DataBind();             
            }
            else
            {
                lblMessage.InnerText = "Data not Found";
            }
         
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (ddlCourseList.SelectedIndex > 0)
            {
                loadStipendList(ddlCourseList.SelectedValue.ToString(), ddlCompany.SelectedValue);
            }
            else
            {
                lblMessage.InnerText = "Please select your course";
            }
           
           
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            ExportGridViewToExcel();

 

        }
        private void ExportGridViewToExcel()
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=stipnedSheet.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    gvstipendList.HeaderRow.Style.Add("background-color", "#FFFFFF");
                    foreach (TableCell cell in gvstipendList.HeaderRow.Cells)
                    {
                        cell.Style["background-color"] = "#A55129"; 
                    }

                    foreach (GridViewRow row in gvstipendList.Rows)
                    {
                        row.BackColor = System.Drawing.Color.White;
                        foreach (TableCell cell in row.Cells)
                        {
                            cell.Attributes["class"] = "textmode";
                        }
                    }
                    gvstipendList.RenderControl(hw);

                    string style = @"<style> .textmode { } </style>";
                    Response.Write(style);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
            }
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Confirm that an HtmlForm control is rendered for the GridView
        }
    }
}