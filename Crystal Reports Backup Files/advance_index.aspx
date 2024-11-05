<%@ Page Title="Advance" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="advance_index.aspx.cs" Inherits="SigmaERP.payroll.advance_index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
     <div class="row">
            <div class="col-md-12">
               <div class="ds_nagevation_bar" style="border-bottom:none;">                    
                    <ul>
                        <li><a href="/default.aspx">Dasboard</a></li>
                        <li> <a class="seperator" href="/hrd_default.aspx">/</a></li>
                        <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li>
                        <li> <a class="seperator" href="/hrd_default.aspx">/</a></li>
                        <li> <a href="/hrd_default.aspx" class="ds_negevation_inactive Pactive">Advance</a></li>
                    </ul>               
                </div>
             </div>
        </div>
 
      <%--  <img alt="" class="main_images" src="images/hrd.png">--%>
    <div>
    <%if (IsRouteExists("advance"))
         { %>
         <div class="col-lg-12">
             <div class="row rowCenter">
                <% if (IsRouteExists("advance/advance"))
                    { %>
                 <div class="col-md-2" title="Loan Entry">
                     <a class="ds_Settings_Basic_Text" href="/hrms/advance/advance"> <i class="fas fa-file-invoice-dollar FsiconStyle"></i><br /> <span>Advance Entry Panel</span></a>                     
                 </div> 
              
                  <% } %>

                 <% if (IsRouteExists("advance/adv-monthly-installment"))
                 { %>
                 <div class=" col-md-2" title="Advance Setting">
                        <a class="ds_Settings_Basic_Text" href="/hrms/advance/adv-monthly-installment"><i class="fas fa-calendar-day FsiconStyle"></i><br />Monthly Installment Setup</a>
                 </div>
                  <% } %>

                   <% if (IsRouteExists("advance/advance-report"))
                   { %>

                 <div class=" col-md-2" title="Advance Reprot">
                      <a class="ds_Settings_Basic_Text" href="/hrms/advance/advance-report"><i class="far fa-file-alt FsiconStyle"></i><br /><span>Advance Reports</span></a>
                 </div>
                  <% } %>

                 <% if (IsRouteExists("advance/list"))
                    { %>
                  <div class=" col-md-2" title="Advance List">
                      <a class="ds_Settings_Basic_Text" href="/hrms/advance/list"><i class="fa-solid fa-money-check-dollar FsiconStyle"></i><br /><span>Advance List</span></a>
                 </div>
                   <% } %>
                 <%-- <div class=" col-md-2" title="Employee Profile">
                       <img class="image_width_for_module" style="width: 86%" src="../images/common/blankImageForManu.png" /><br />
                   </div>--%>
  
             </div>
</div>
         <% } %>
        </div>
</asp:Content>
