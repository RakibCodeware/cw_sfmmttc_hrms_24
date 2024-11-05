using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll.salary
{
    public partial class QatarReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["__company__"] = "";
            ViewState["__Month__"] = "";
        

            BindData();
        }

        private void BindData()
        {
            string _for = Request.QueryString["for"].ToString();
            ViewState["__company__"] = Request.QueryString["company"].ToString();
            getCompanyInfo(ViewState["__company__"].ToString());
            ViewState["__Month__"] = Request.QueryString["month"].ToString();
            gvBannFord.DataSource = Session["__SalarySheetBankFord__"];
            gvBannFord.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            ExportData();
        }



        private void ExportData()
        {
            string bankShortName = Session["__bankShortname__"].ToString(); 
            // Get necessary session and view state values
            if (Session["__bankShortname__"].ToString() == "")
            {
                bankShortName = "CBQ and QIB";
            }
          

            string totalSalaryString = ViewState["__totalSalary__"]?.ToString();
            string registrationID = ViewState["__registrationID__"]?.ToString();
            string establishmentID = ViewState["__establishmentID__"]?.ToString();
            string totalRows = ViewState["__totalRows__"]?.ToString() ?? "0";

            // Initialize CSV content
            var csvContent = new StringBuilder();
            csvContent.AppendLine("EMPLOYER EID:,File Creation Date,File Creation Time,Payer EID,Payer QID,Payer Bank Short Name,Total Salaries,Total records:");
            csvContent.AppendLine($"{registrationID},{DateTime.Today:yyyyMMdd},{DateTime.Now:HH:mm},{establishmentID},,{bankShortName},{totalSalaryString},{totalRows}");

            // Add headers to CSV
            var headerValues = new List<string> { "Record ID" };
            foreach (DataControlField header in gvBannFord.Columns)
            {
                headerValues.Add(header.HeaderText);
            }
            csvContent.AppendLine(string.Join(",", headerValues));

            // Process each row in the GridView
            decimal totalSalary = 0;
            int totalRecord = 0;
            foreach (GridViewRow row in gvBannFord.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    var cellValues = new List<string> { (row.RowIndex + 1).ToString() };

                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        // Clean the cell text and replace &nbsp; with an empty string
                        string cellText = CleanText(row.Cells[i].Text).Replace("&nbsp;", string.Empty);
                        cellValues.Add(cellText);
                    }

                    // Add the row data to CSV content
                    csvContent.AppendLine(string.Join(",", cellValues));

                    // Accumulate salary data
                    if (decimal.TryParse(cellValues[8], out decimal netAmount))
                    {
                        totalSalary += netAmount;
                    }
                    totalRecord++;
                }
            }

            // Update total salary and total records in CSV content
            csvContent.AppendLine();
            csvContent.Replace(totalSalaryString, totalSalary.ToString("F2"));
            csvContent.Replace(totalRows, totalRecord.ToString());

            // Output CSV file for download
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=banksheet.csv");
            Response.ContentType = "text/csv";
            Response.Write(csvContent.ToString());
            Response.Flush();
            Response.End();
        }

        // Method to clean any HTML tags from text
        private string CleanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove any HTML tags and trim spaces
            return Regex.Replace(input, @"<[^>]*>", string.Empty).Trim();
        }


        private string ProcessCellText(string cellText)
        {
            cellText = HttpUtility.HtmlDecode(cellText?.Trim() ?? "");
            return string.IsNullOrWhiteSpace(cellText) || cellText == "&nbsp;" ? "" : cellText.Contains(",") ? $"\"{cellText}\"" : cellText;
        }




        public override void VerifyRenderingInServerForm(Control control)
        {
            // Required for exporting GridView to Excel
        }

        private void getCompanyInfo(string companyId)
        {
            string query = "select RegistrationID, EstablishmentID from hrd_companyinfo where companyID = '"+ companyId + "'";
            DataTable dt = CRUD.ExecuteReturnDataTable(query);
            ViewState["__registrationID__"] = dt.Rows[0]["RegistrationID"].ToString();
            ViewState["__establishmentID__"] = dt.Rows[0]["EstablishmentID"].ToString();
        }
        private decimal totalSalary = 0; 
        private int totalRows = 0;
        protected void gvBannFord_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                decimal salary = Convert.ToDecimal(DataBinder.Eval(e.Row.DataItem, "TotalSalary"));

              
                totalSalary += salary;

           
                totalRows++;
            }
            ViewState["__totalSalary__"] = totalSalary;
            ViewState["__totalRows__"] = totalRows;

        }
    }
}