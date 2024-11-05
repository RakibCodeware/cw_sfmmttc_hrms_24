<%@ Page Title="Salary" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="salary_index.aspx.cs" Inherits="SigmaERP.payroll.salary_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar" style="border-bottom: none;">

                <ul>
                    <li><a href="/default.aspx">Dashboard</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                     <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Salary</a></li>
                </ul>

            </div>
        </div>
    </div>

    <%--  <img alt="" class="main_/images" src="/images/hrd.png">--%>
    <div>
    <%if (IsRouteExists("salary"))
        { %>
        <div class="container">
            <div class="row">
             <% if (IsRouteExists("salary/salary-entry"))
                { %>
                <div runat="server" id="divSalaryEntry" class="col-md-3">
                    <a class="ds_Settings_Basic_Text" href="/hrms/salary/salary-entry">
                        <i class="fa-solid fa-money-check-dollar FsiconStyle"></i><br />
                        <span>Salary Entry Panel</span></a>

                </div>
              <% } %>
              <% if (IsRouteExists("salary/salary-entry"))
                  { %>
                <div runat="server" id="divSalaryEntryc" class="col-md-3">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-entry">
                        <i class="fa-solid fa-money-check-dollar FsiconStyle"></i>
                       <span>Salary Entry Panel</span></a>

                </div>
             <% } %>
            <% if (IsRouteExists("salary/salary-allowance"))
               { %>
                <div runat="server" id="divAllowanceCalculation" class="col-md-3">
                    <a class="ds_Settings_Basic_Text" href="/hrms/salary/salary-allowance">
                       <i class="fa fa-hand-holding-usd FsiconStyle"></i><br />
                        Allowance Calculation </a>

                </div>   
                <% } %>

<%--                 <div runat="server" class="col-md-3" title="Punishment">
                    <a class="ds_Settings_Basic_Text " href="/payroll/salary/other_pay_deduction.aspx">
                        <i class="fas fa-money-bill-alt FsiconStyle"></i><br />
                       Punishment</a>
                </div>   --%>

                 <% if (IsRouteExists("salary/salary-process"))
                 { %>
                <div runat="server" class="col-md-3" title="Salary Processing">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-process">
                        <i class="fas fa-money-bill-alt FsiconStyle"></i><br />
                        Salary Processing (New)</a>
                </div>              
                   <% } %>
                
              <% if (IsRouteExists("salary/salary-report"))
                    { %>
                <div class=" col-md-3" title="Monthly Salary Sheet">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-report">
                        <i class="fas fa-hand-holding-usd FsiconStyle"></i><br />
                        Salary Sheet (New)</a>
                </div>            
                  <% } %>

                <% if (IsRouteExists("salary/monthly-ot-report"))
                   { %>
                   <div runat="server"  class=" col-md-3" title="Overtime Payment Sheet">

                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/monthly-ot-report">
                       <i class="fa-solid fa-money-check-dollar FsiconStyle"></i><br />
                        Only OT Payment (New)</a>
                </div>
                 <% } %>
               <% if (IsRouteExists("salary/salary-promotion"))
                  { %>
                <div runat="server" id="divPromotionEntry" class=" col-md-3" title="Employee Promotion">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-promotion">
                        <i class="fas fa-chart-line FsiconStyle"></i><br />
                        Promotion Entry Panel</a>

                </div>
                <% } %>
                   <% if (IsRouteExists("salary/salary-promotion"))
                  { %>
                <div runat="server" id="divPromotionEntryComp" class=" col-md-3" title="Employee Promotion">
                    <a class="ds_Settings_Basic_Text " href="/hmrs/salary/salary-promotion">
                        <i class="fas fa-chart-line iconStyle FsiconStyle"></i><br />
                        Promotion Entry Panel</a>
                </div>
                 <% } %>
                <% if (IsRouteExists("salary-promotion/promotion-report"))
                    { %>
                <div runat="server" id="divPromotionReport" class=" col-md-3" title="Employees Promotion Report">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-promotion">
                        <i class="fas fa-chart-line iconStyle FsiconStyle"></i><br />
                        Promotion List Report</a>

                </div>
                  <% } %>

                
              <% if (IsRouteExists("salary-promotion/promotion-report"))
               { %>
                <div runat="server" id="divPromotionReportComp" class=" col-md-3" title="Employees Promotion Report">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-promotion">
                        <img class="image_width_for_module" src="../images/common/businesstype.ico" /><br />
                        Promotion List Report</a>

                </div>
                <% } %>
              <% if (IsRouteExists("salary/salary-increment"))
                { %>
                <div runat="server" id="divIncrement" class=" col-md-3" title="Salary Increment">

                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-increment">
                        <i class="uil uil-chart-growth iconStyle"></i><br />
                        Increment Entry Panel</a>
                </div>
              <% } %>

              <% if (IsRouteExists("salary/salary-increment"))
                { %>
                <div runat="server" id="divIncrementComp" class=" col-md-3" title="Salary Increment">

                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-increment">
                         <i class="uil uil-chart-growth iconStyle"></i><br />
                        Increment Entry Panel</a>
                </div>
               <% } %>
               <% if (IsRouteExists("salary/salary-increment"))
                 { %>
                <div runat="server" id="divAutoIncrementComp" class=" col-md-3" title="Salary Increment">

                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/salary-increment">
                        <i class="uil uil-chart-growth iconStyle"></i><br />
                        Auto Increment Panel</a>
                </div>
               <% } %>
                <% if (IsRouteExists("salary-increment/increment-report"))
                  { %>
                <div runat="server" id="divIncrementReport" class=" col-md-3" title="Salary Increment Report">

                    <a class="ds_Settings_Basic_Text " href="/hrms/salary-increment/increment-report">
                         <i class="uil uil-chart-growth iconStyle"></i><br />
                        Increment List Report</a>
                </div>
                  <% } %>
                  <% if (IsRouteExists("salary-increment/increment-report"))
                                           { %>
                <div runat="server" id="divIncrementReportComp" class=" col-md-3" title="Salary Increment Report">

                    <a class="ds_Settings_Basic_Text " href="/hrms/salary-increment/increment-report">
                       <i class="uil uil-chart-growth iconStyle"></i><br />
                        Increment List Report</a>
                </div>
                 <% } %>


              <% if (IsRouteExists("salary/earn-leave-payment-processing"))
                                           { %>
                <div runat="server" id="divEarnLeavePaymentGeneration" class=" col-md-3" title="Earn Leave Payment Generation">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/earn-leave-payment-processing">
                        <i class="fas fa-sign-out-alt FsiconStyle"></i><br />
                        Earn Leave Processing</a>
                </div>
                  <% } %>

               <% if (IsRouteExists("salary/earn-leave-payment-sheet"))
                                           { %>
                <div runat="server" id="divEarnLeavePaymentGenerationComp" class=" col-md-3" title="Earn Leave Payment Generation">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/earn-leave-payment-processing">
                        <i class="fas fa-sign-out-alt FsiconStyle"></i><br />
                        Earn Leave Processing</a>
                </div>
               <% } %>

                   <% if (IsRouteExists("salary/earn-leave-payment-sheet"))   { %>
                <div runat="server" id="divEarnLeavePaymentSheet" class="col-md-3" title="Earn Leave Payment Sheet">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/earn-leave-payment-sheet">
                        <i class="fas fa-sign-out-alt FsiconStyle"></i><br />
                        Earn Leave Payment Sheet</a>
                </div>
                   <% } %>
                   <% if (IsRouteExists("salary/earn-leave-payment-sheet"))   { %>
                <div runat="server" id="divEarnLeavePaymentSheetComp" class="col-md-3" title="Earn Leave Payment Sheet">
                    <a class="ds_Settings_Basic_Text " href="/hrms/salary/earn-leave-payment-sheet">
                       <i class="fas fa-sign-out-alt FsiconStyle"></i>
                        Earn Leave Payment Sheet</a>
                </div>
                  <% } %>

                <%--<div class=" col-md-2" title="Final Bill Payment Sheet">
                      <a class="ds_Settings_Basic_Text Pbox" href="/payroll/final_bill_payment_sheet.aspx"><img class="image_width_for_module" src="/images/common/qualification.ico" /><br />Seperation Final Bill Sheet</a> 
                    
                 </div>
                 <div class=" col-md-2" title="Monthly Salary Flow">
                     <a class="ds_Settings_Basic_Text Pbox" href="/payroll/monthly_salary_flow.aspx"><img class="image_width_for_module" src="/images/common/salaryflow.ico" /><br />Monthly Salary Flow</a> 
                 
                 </div>
                 <div class=" col-md-2" title="Current Salary Structure">

                     <a class="ds_Settings_Basic_Text Pbox" href="/payroll/CurrentSalaryStructure.aspx"><img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />Emp Salary Structure</a>
                 </div>--%>

                <div class=" col-md-2"></div>
            </div>
            <%--    <div class="row">

                 <div class=" col-md-2"></div>

                 <div class=" col-md-2" title="Punishment&Other's Pay">
                      <a class="ds_Settings_Basic_Text Pbox" href="/payroll/Punishment_OthersPay.aspx"><img class="image_width_for_module" src="../images/common/qualification.ico" /><br />Punishment&Other's Pay</a> 
                    
                 </div>
                
                 <div class=" col-md-2"></div>
             </div>--%>
        </div>
       <% } %>
    </div>
    <script type="text/javascript">

        $(document).keyup(function (e) {
            if (e.keyCode == 79) {
                goToNewTabandWindow('/payroll/payroll_generation_rss.aspx');
            }
            else if (e.keyCode == 80) {
                goToNewTabandWindow('/payroll/salary_sheet_Report_2nd.aspx');
            }
        });
        function goToNewTabandWindow(url) {
            window.open(url);
        }

    </script>
</asp:Content>
