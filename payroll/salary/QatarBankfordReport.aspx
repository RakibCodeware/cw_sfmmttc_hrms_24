<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QatarBankfordReport.aspx.cs" Inherits="SigmaERP.payroll.salary.QatarReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js" ></script>
     <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"/>
    <style>
        table#gvBannFord th{
            background:#198754;
            color:white;
        }
        td{
            border:1px solid gray;
        }
   
    </style>
   
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button ID="btnExport" runat="server" Text="EXPORT" CssClass="btn btn-success align-items-end float-end" OnClick="btnExport_Click" />

            <asp:GridView runat="server" ID="gvBannFord" AutoGenerateColumns="false" CssClass="table" OnRowDataBound="gvBannFord_RowDataBound">
                <Columns>

                   <asp:BoundField DataField="NationIDCardNo" HeaderText="Employee QID" ItemStyle-Width="150" />
                   <asp:BoundField DataField="EmpVisaNo" HeaderText="Employee Visa ID" ItemStyle-Width="150" />
                   <asp:BoundField DataField="EmpName" HeaderText="Employee Name" ItemStyle-Width="150" />
                   <asp:BoundField DataField="BankShortName" HeaderText="Employee Bank Short Name" ItemStyle-Width="150" />
                   <asp:BoundField DataField="EmpAccountNo" HeaderText="Employee Account" ItemStyle-Width="150" />
                   <asp:BoundField DataField="SalaryFrequency" HeaderText="Salary Frequency" ItemStyle-Width="150" />
                   <asp:BoundField DataField="PayableDays" HeaderText="Number of Working Days" ItemStyle-Width="150" />
                   <asp:BoundField DataField="TotalSalary" HeaderText="Net Salary" ItemStyle-Width="150" />
                   <asp:BoundField DataField="BasicSalary" HeaderText="Basic Salary" ItemStyle-Width="150" />
                   <asp:BoundField DataField="ExtraOtHour" HeaderText="Extra hours" ItemStyle-Width="150" />
                   <asp:BoundField DataField="ExtraOtAmount" HeaderText="Extra Income" ItemStyle-Width="150" />
                   <asp:BoundField DataField="Deduction" HeaderText="Deductions" ItemStyle-Width="150" />
                   <asp:BoundField DataField="PaymentType" HeaderText="Payment Type" ItemStyle-Width="150" />
                   <asp:BoundField DataField="Notes" HeaderText="Notes/Comments" ItemStyle-Width="150" />
                </Columns>

            </asp:GridView>

            
        </div>
    </form>
</body>
</html>
