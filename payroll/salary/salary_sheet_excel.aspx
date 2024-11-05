<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="salary_sheet_excel.aspx.cs" Inherits="SigmaERP.payroll.salary.salary_sheet_excel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h3>Records</h3>

            <asp:Button runat="server" ID="btnExport" Text="Export" OnClick="btnExport_Click"/>
             <asp:GridView runat="server" ID="gvSalarySheetExcel">

            </asp:GridView> 
        </div>
    </form>
</body>
</html>
