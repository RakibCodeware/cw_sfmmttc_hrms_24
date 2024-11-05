using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll.salary
{
    public partial class salary_sheet_excel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                bindData();
            }

        }

        private void bindData()
        {
            DataTable dt = (DataTable)Session["__salarySheetExcel__"];
            gvSalarySheetExcel.DataSource = dt;
            gvSalarySheetExcel.DataBind();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string _for = Request.QueryString["for"].ToString();
            string company = Request.QueryString["company"].ToString();
            string month = Request.QueryString["month"].ToString();

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename="+ _for+"_"+ DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() + ".xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
                     
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                DataTable dt = GetDataTableFromGridView(gvSalarySheetExcel);

                hw.Write("<style>table, th, td { border: 1px solid black; border-collapse: collapse; }</style>");
                hw.Write("<table>");


                int columnCount = dt.Columns.Count + 1;


             

                // Write custom header lines in the middle without borders
                hw.Write("<table>");
                hw.Write("<tr><td colspan='" + columnCount + "' style='text-align:center; border:none;'><b>"+company+ "</b></td></tr>");
                hw.Write("<tr><td colspan='" + columnCount + "' style='text-align:center; border:none;'><b>Salary Sheet for the Month of "+ month + "</b></td></tr>");
                hw.Write("<tr><td colspan='" + columnCount + "' style='border:none;'>&nbsp;</td></tr>"); // Empty row for spacing

                hw.Write("<tr>");
                hw.Write("<th>SL</th>"); 
                foreach (DataColumn column in dt.Columns)
                {
                    hw.Write("<th>" + column.ColumnName + "</th>");
                }
                hw.Write("</tr>");
                var groupedData = dt.AsEnumerable()
              .GroupBy(row => row.Field<string>("Department"))
              .Select(g => new
              {
                  Department = g.Key,
                  Rows = g.CopyToDataTable(),
                  Sum = g.Sum(row => Convert.ToDecimal(row["Net Payable"]))
              });
                //if (_for == "SalarySheet")
                //{
                //     groupedData = dt.AsEnumerable()
                // .GroupBy(row => row.Field<string>("Department"))
                // .Select(g => new
                // {
                //     Department = g.Key,
                //     Rows = g.CopyToDataTable(),
                //     Sum = g.Sum(row => Convert.ToDecimal(row["Net Payable"]))
                // });


                //}

                foreach (var group in groupedData)
                {

                    hw.Write("<tr><td colspan='" + (dt.Columns.Count + 1) + "'><b>Department: " + group.Department + "</b></td></tr>");
                    int sl = 1;
                    foreach (DataRow dataRow in group.Rows.Rows)
                    {
                        hw.Write("<tr>");
                        hw.Write("<td style='mso-number-format:0;'>" + sl++ + "</td>");
                        foreach (var item in dataRow.ItemArray)
                        {
                            hw.Write("<td>" + item.ToString() + "</td>");
                        }
                        hw.Write("</tr>");
                    }

                    hw.Write("<tr>");
                    hw.Write("<td colspan='" + dt.Columns.Count + "'><b>Total:</b></td>");
                    hw.Write("<td style='mso-number-format:0;'><b>" + group.Sum.ToString("N2") + "</b></td>");
                    hw.Write("</tr>");

                    hw.Write("<tr><td colspan='" + (dt.Columns.Count + 1) + "'>&nbsp;</td></tr>");
                }

                hw.Write("</table>");

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }

        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            // Verifies that the control is rendered
        }

 

        private DataTable GetDataTableFromGridView(GridView gridView)
        {
            DataTable dt = new DataTable();
            foreach (TableCell cell in gridView.HeaderRow.Cells)
            {
                dt.Columns.Add(cell.Text);
            }
            foreach (GridViewRow row in gridView.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }


    }
}