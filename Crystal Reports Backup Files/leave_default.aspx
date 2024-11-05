<%@ Page Title="Leave" Language="C#" MasterPageFile="~/leave_nested.master" AutoEventWireup="true" CodeBehind="leave_default.aspx.cs" Inherits="SigmaERP.leave_default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
            <div class="col-md-12">
                <div class="ds_nagevation_bar" style="border-bottom:none;">
                     <div style="margin-top: 5px">
                           <ul>
                               <li><a href="/default.aspx">Dashboard</a></li>
                               <li> <a class="seperator" href="#">/</a></li>
                               <li> <a href="#" class="ds_negevation_inactive Lactive">Leave</a></li>
                           </ul>               
                     </div>
                </div>
             </div>
        </div>
    <div>

        <% if (IsRouteExists("leave-root"))
                        { %>
    <div class="col-lg-12">
             <div class="row ">
                 <div class="col-lg-2"></div>
       
                   <% if (IsRouteExists("leave-root/leave-configuration"))
                                     { %>
                 <div runat="server" id="divConfiguration" class=" col-lg-2 col-md-3 col-sm-6 " title="Leave Configuration" >
                     <a class="ds_Settings_Basic_Text" href="leave-root/leave-configuration"> <i class="uil uil-cog iconStyle"></i><br /> Configuration </a>
                     
                 </div>
               <% } %>

              <% if (IsRouteExists("leave-root/holiday-settings"))
                  { %>

                 <div runat="server" id="divHoliday" class=" col-lg-2 col-md-3 col-sm-6" title="All Holiday Setup">
                        <a class="ds_Settings_Basic_Text" href="leave-root/holiday-settings"><i class="uil  uil-clock-nine  iconStyle"></i><br />Holiday Setup </a>
                 </div>
                 <% } %>

               <% if (IsRouteExists("leave-root/holiday-settings"))
                  { %>
                 <div runat="server" id="divHolidayComp" class="col-lg-2 col-md-3 col-sm-6" title="All Holiday Setup">
                        <a class="ds_Settings_Basic_Text" href="leave-root/holiday-settings"><i class="uil uil-clock-nine iconStyle"></i><br />Holiday Setup </a>
                 </div>
               <% } %>

                 
           <% if (IsRouteExists("leave/leave-application"))
               { %>
                 <div runat="server" id="divApplication" class="col-lg-2 col-md-3 col-sm-6" title="Leave Application">
                      <a class="ds_Settings_Basic_Text" href="leave/leave-application"><i class="uil uil-file-bookmark-alt iconStyle"></i><br />Application</a>
                 </div>

             <% } %>

            <% if (IsRouteExists("leave-root/short-leave-application"))
            { %>
                    <div runat="server" id="divShortLeave" class="col-lg-2 col-md-3 col-sm-6" title="Earn Leave Report">
                     <a class="ds_Settings_Basic_Text" href="leave-root/short-leave-application"><i class="uil uil-signout iconStyle"></i><br />Short Leave </a> 
                 
                 </div>   
                 

                    <% } %>
               <div class="col-lg-2"></div>
             </div>

               <div class="row ">
               <div class="col-lg-2"></div>
                   
                     <% if (IsRouteExists("leave-root/leave-approval"))
                          { %>
                      <div  runat="server" id="divLeaveApproval" class="col-lg-2 col-md-3 col-sm-6" title="Pending Leave Approved">
                      <a class="ds_Settings_Basic_Text" href="leave-root/leave-approval"><i class="uil uil-cog uil-check-square iconStyle"></i><br />Leave Approval</a>
                     </div>
                     <% } %>

                    <% if (IsRouteExists("leave-root/short-leave-approval"))
                        { %>
                        <div runat="server" id="divShortLvApproval" class="col-lg-2 col-md-3 col-sm-6" title="Pending Leave Approved">
                      <a class="ds_Settings_Basic_Text" href="leave-root/short-leave-approval"><i class="uil uil-cog uil-check-square iconStyle"></i><br />Short Lv Approval</a>
                      </div>
                     <% } %>

                   <% if (IsRouteExists("leave/leave-list"))
                       { %>
                      <div runat="server" id="divLeaveList" class=" col-lg-2 col-md-3 col-sm-6" title="All Leave List">
                     <a class="ds_Settings_Basic_Text" href="leave/leave-list"><i class="uil uil-list-ol-alt iconStyle"></i><br />All Leave List</a>
                 </div>

                 <% } %>

                 <% if (IsRouteExists("leave/leave-balance-report"))
                     { %>
                    <div runat="server" id="divBalanceReport" class=" col-lg-2 col-md-3 col-sm-6 " title="Leave Balance Report">
                    <a class="ds_Settings_Basic_Text" href="leave/leave-balance-report"><i class="uil uil-file-info-alt iconStyle"></i><br />Balance Report</a> 
                 </div>  
                    <% } %>
                  <div class="col-lg-2"></div>
             </div>    
               <div class="row ">
                  <div class="col-lg-2"></div>
                 <% if (IsRouteExists("leave/yearly-leave-summary-report"))
                      { %>
                     <div runat="server" id="divSummaryReport" class=" col-lg-2 col-md-3 col-sm-6 " title="Yearly Purpose Report">
                      <a class="ds_Settings_Basic_Text" href="leave/yearly-leave-summary-report"><i class="uil  uil-file-bookmark-alt iconStyle"></i><br />Summary Report</a> 
                 </div>
                  <% } %>

                <% if (IsRouteExists("leave/el-processing"))
                   { %>
                    <%--      <div class=" col-lg-2 col-md-3 col-sm-6" title="Official Purpose Report">
                     <a class="ds_leave_Basic_Text" href="/leave/company_purpose_leave_report.aspx"><img class="image_width_for_module" src="images/common/add document.ico" /><br />Office Purpose</a>                    
                 </div>--%>
               
                <div runat="server" id="divEarnLeaveGeneration"  class=" col-lg-2 col-md-3 col-sm-6" title="Earn Leave Generate">
                      <a class="ds_Settings_Basic_Text" href="leave/el-processing"><i class="uil uil-spinner-alt iconStyle"></i><br />EL Processing</a>                     
                 </div>
                        <% } %>
                   
                  <% if (IsRouteExists("leave/el-processing"))
                   { %>
                   <div runat="server" id="divEarnLeaveGenerationComp" class=" col-lg-2 col-md-3 col-sm-6" title="Earn Leave Generate">
                      <a class="ds_Settings_Basic_Text" href="/leave/earnleave_generationc.aspx"><img class="image_width_for_module" src="images/common/generate.ico" /><br />EL Generate</a>                     
                 </div>
                 <% } %>
                <% if (IsRouteExists("leave/el-report"))
                    { %>
                 <div  runat="server" id="divEarnLeaveReport" class=" col-lg-2 col-md-3 col-sm-6" title="Earn Leave Report">
                     <a class="ds_Settings_Basic_Text" href="leave/el-report"><i class="uil  uil-file-bookmark-alt iconStyle"></i><br />EL Report </a>                 
                 </div>
                  <% } %>
                   <div class=" col-lg-2">
                   </div>
              
                 <%--<div class=" col-lg-2 col-md-3 col-sm-6 " title="Maternity Application">
                      <a class="ds_leave_Basic_Text" href="/leave/MaternityLeaveApplication.aspx"><img class="image_width_for_module" src="images/common/application.ico" /><br />Ma. Application</a> 
                 </div>
                   <div class=" col-lg-2 col-md-3 col-sm-6 " title="Maternity Voucher">
                     <a class="ds_leave_Basic_Text" href="/leave/maternity.aspx"><img class="image_width_for_module" src="images/common/businesstype.ico" /><br />Ma. Voucher</a> 
                 </div>--%>
              
             </div>
    
       
    

</div></div>
       <% } %>

</asp:Content>
