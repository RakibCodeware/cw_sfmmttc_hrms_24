<%@ Page Title="" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="IncrementEntitleReport.aspx.cs" Inherits="SigmaERP.payroll.salary.IncrementEntitleReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
 <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        #ContentPlaceHolder1_ContentPlaceHolder1_gvEmplye th{
            background-color:#198754 !important;
            color:white;
        }
     #date{
            border: 1px solid #d2d2d2 !important;
     }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <p class="message" id="lblMessage"  runat="server"></p>
      <h1  runat="server" visible="false" id="WarningMessage"  style="color:red; text-align:center"></h1>
     <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar">
                <ul>
                    <li><a href="/default.aspx">Dashboard</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Increment Entitled Report</a></li>
                </ul>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManger1" runat="Server" EnablePageMethods="false"></asp:ScriptManager>
    <asp:UpdatePanel ID="uplMessage" runat="server">
        <ContentTemplate>
            <p class="message" id="P1" clientidmode="Static" runat="server"></p>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="row justify-content-center mt-2">
        <div class="col-lg-6">
            <div class="row">
                  <div class="col-md-3">
                 <asp:Label runat="server" CssClass="fw-bold" ID="lbldepartmetn">Department</asp:Label>
               <div class="card">
               
                     <asp:DropDownList CssClass="form-control" runat="server" ID="ddlDepartment"></asp:DropDownList>
                </div>
          </div>

                 <div class="col-md-3">
              <asp:Label runat="server" CssClass="fw-bold" ID="lblDate">Date<span class="text-danger fw-bold">*</span></asp:Label>
               <div class="card">
                   
                    <asp:TextBox ID="txtDate" CssClass="date" style="border: 1px solid #d2d2d2"  runat="server" type="Date"></asp:TextBox>
             
                  

                </div>
          </div>

                 <div class="col-md-3">
                       <asp:Label runat="server" CssClass="fw-bold" ID="lblCard">Card No</asp:Label>
            <div class="card">
              
                <asp:TextBox runat="server" ID="txtCard" CssClass="form-control"></asp:TextBox>
            </div>
           
        </div>
       
          <div class="col-md-3">
                     <asp:Label runat="server" ID="lblHIdden" CssClass="opacity-0 d-block">SUMON</asp:Label>
                      <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary p-3" Text="Search" OnClick="btnSearch_Click" />        
                    <asp:Button runat="server" ID="btnExport" CssClass="btn btn-success p-3" Text="Export" OnClick="btnExport_Click" />    
           
        </div>
        
         
      </div>
             
   </div>
 </div>
      <asp:GridView ID="gvEmplye" runat="server"  CssClass="table mt-3" AutoGenerateColumns="false">
          <Columns>
        <asp:TemplateField HeaderText="SL">
            <ItemTemplate>
                <%# Container.DataItemIndex + 1 %>
            </ItemTemplate>
        </asp:TemplateField>
            <asp:BoundField DataField="empCardNo" HeaderText="Card No" SortExpression="ID" />
            <asp:BoundField DataField="empName" HeaderText="Name" SortExpression="Name" />
            <asp:BoundField DataField="departmentName" HeaderText="Department" SortExpression="Department" />
            <asp:BoundField DataField="designationName" HeaderText="Designation " SortExpression="Designation" />
            <asp:BoundField DataField="empJoiningDate" HeaderText="Joining Date" SortExpression="JoiningDate" />
            <asp:BoundField DataField="lastIncrementMonth" HeaderText="LastIncrement Month" SortExpression="LastIncrement" />
            <asp:BoundField DataField="empPresentSalary" HeaderText="Current Salary" SortExpression="CurrentSalary" />
            <asp:BoundField DataField="increment" HeaderText="Incremented Amount" SortExpression="IncrementedAmount" />
             <asp:BoundField DataField="incrementSalary" HeaderText="Incremented Salary" SortExpression="IncrementedSalary" />


    </Columns>
      </asp:GridView>

    <script type="text/javascript">
         $(document).keyup(function (e) {
            if (e.keyCode == 80) {
                goToNewTabandWindow('/payroll/salary_sheet_Report.aspx');
            }
        });
        function InputValidationBasket() {
            try {

                if ($('#txtEmpCardNo').val().trim().length < 4) {
                    showMessage('Please select To Date', 'error');
                    $('#txtToDate').focus(); return true;
                }
                return true;
            }
            catch (exception) {
                showMessage(exception, error)
            }
        }

        function CloseWindowt() {
            window.close();
        }

        function goToNewTabandWindow(url) {
            window.open(url);

        }

        function getSalaryMonth() {

            var val = document.getElementById('ddlMonthID').value;
            document.getElementById('txtMonthId').value = val;

        }

        function CloseWindowt() {
            window.close();
        }

        function goToNewTabandWindow(url) {
            window.open(url);

        }

    </script>
</asp:Content>
