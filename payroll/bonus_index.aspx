<%@ Page Title="Bonus" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="bonus_index.aspx.cs" Inherits="SigmaERP.payroll.bonus_index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
     <div class="row">
            <div class="col-md-12">
               <div class="ds_nagevation_bar" style="border-bottom:none;">
                   
                           <ul>
                               <li><a href="/default.aspx">Dasboard</a></li>
                               <li> <a class="seperator" href="#">/</a></li>
                              <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li>
                               <li> <a class="seperator" href="#">/</a></li>
                               <li> <a href="#" class="ds_negevation_inactive Pactive">Bouns</a></li>
                           </ul>               
                    
                </div>
             </div>
        </div>
    
      <%--  <img alt="" class="main_/imagess" src="/imagess/hrd.png">--%>
    <div>
        <%if (IsRouteExists("bonus"))
          {%>
         <div class="col-lg-12 ">
             <div class="row rowCenter">
                 <% if (IsRouteExists("bonus/bonus-setup"))
                    { %>
                 <div class="col-md-2" title="Bonus Setup">
                     <a runat="server" id="aBonusSetup" class="ds_Settings_Basic_Text " href="/hrms/bonus/bonus-setup"> <i class="fa-solid fa-money-check-dollar FsiconStyle"></i><br /> <span>Bonus Setup</span></a>
                     <a runat="server" id="aBonusSetupc" class="ds_Settings_Basic_Text " href="/payroll/bonus_setupc.aspx"> <i class="fa-solid fa-money-check-dollar FsiconStyle"></i><br /> <span>Bonus Setup</span></a>
                     
                 </div>
                  <% } %>

                 <% if (IsRouteExists("bonus/bonus-month-setup"))
                   { %>

                 <div class=" col-md-2" title="Bonus Month Setup">
                        <a runat="server" id="aBonusMonthSetup" class="ds_Settings_Basic_Text " href="/hrms/bonus/bonus-month-setup"><i class="fa-regular fa-calendar-days FsiconStyle"></i><br /><span>B. Month Setup</span></a>
                     <a runat="server" id="aBonusMonthSetupc" class="ds_Settings_Basic_Text " href="/payroll/bonus_month_setupc.aspx"><i class="fa-regular fa-calendar-days FsiconStyle"></i><br /><span>B. Month Setup</span></a>
                 </div>
                  <% } %>

                <% if (IsRouteExists("bonus/bonus-process"))
                   { %>
                 <div class=" col-md-2" title="Bonus Generate">
                      <a runat="server" id="aBonusGenerate" class="ds_Settings_Basic_Text " href="/hrms/bonus/bonus-process"><i class="fa-solid fa-circle-dollar-to-slot FsiconStyle"></i><br /><span>B. Generate</span></a>
                     <a runat="server" id="aBonusGeneratec" class="ds_Settings_Basic_Text " href="/payroll/bonus_generationc.aspx"><i class="fa-solid fa-circle-dollar-to-slot FsiconStyle"></i><br /><span>B. Process</span></a>
                 </div>
                   <% } %>

                  <% if (IsRouteExists("bonus/bonus-inc-dec"))
                     { %>
                   <div class=" col-md-2" title="Bonus Rise & Fall">
                      <a runat="server" id="aBonusRiseFall" class="ds_Settings_Basic_Text " href="/hrms/bonus/bonus-inc-dec"><i class="fa-solid fa-money-bill-trend-up FsiconStyle"></i><br /><span>B. Rise & Fall</span></a>
                       
                       
                 </div>
                 <% } %>
             </div>

               <div class="row rowCenter">

        
               <% if (IsRouteExists("bonus/bonus-report"))
                   { %>
                 <div class=" col-md-2" title="Bonus Sheet">
                      <a runat="server" id="aBonusSheet" class="ds_Settings_Basic_Text " href="/hrms/bonus/bonus-report"><i class="fas fa-receipt FsiconStyle"></i><br /><span>Bonus Sheet</span></a>    
                     <a runat="server" id="aBonusSheetc" class="ds_Settings_Basic_Text " href="/payroll/bonus_sheet_Reportc.aspx"><i class="fas fa-receipt FsiconStyle"></i><br /><span>Bonus Sheet</span></a>                    
                 </div>
                 <% } %>
           

               <% if (IsRouteExists("bonus/bonus-summary"))
                   { %>
                 <div runat="server" id="aBonusSummary" class=" col-md-2" title="Bonus Summary">
                     <a class="ds_Settings_Basic_Text " href="/hrmrs/bonus/bonus-summary"><i class="fas fa-money-check FsiconStyle"></i><br /><span>B. Summary</span></a> 
                 
                 </div>
                 <% } %>
                 <% if (IsRouteExists("bonus/bonus-miss-report"))
                   { %>

                 <div class=" col-md-2" title="Bonus Miss Sheet">
                     <a runat="server" id="aBonusMissSheet" class="ds_Settings_Basic_Text " href="/hrms/bonus/bonus-miss-report"><i class="uil uil-money-bill-slash FsiconStyle"></i><br />B. Miss Sheet</a>                     
                 </div>  
                 <% } %>
                   <div class="col-md-2">

                   </div>
 
             </div>
</div>

          <% } %>
        </div>
</asp:Content>
