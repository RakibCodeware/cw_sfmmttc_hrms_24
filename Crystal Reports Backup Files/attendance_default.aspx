<%@ Page Title="Attendance" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="attendance_default.aspx.cs" Inherits="SigmaERP.attendance_default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
            <div class="col-md-12">
                <div class="ds_nagevation_bar" style="border-bottom:none;">
                     <div style="margin-top: 5px">
                           <ul>
                               <li><a href="/default.aspx">Dashboard</a></li>
                               <li> <a class="seperator" href="/hrd_default.aspx">/</a></li>
                               <li> <a href="#" class="ds_negevation_inactive Mactive">Attendance</a></li>
                           </ul>               
                     </div>
                </div>
             </div>
        </div>
    <div >
    <% if (IsRouteExists("attendance"))
                       { %>
    <div class="col-lg-12">
             <div class="row ">
                 <div class="col-lg-2 ">

                 </div>
                  <% if (IsRouteExists("attendance/month-setup"))
                       { %>
                 <div runat="server" id="divMonthSetup" class="col-lg-2 col-md-3 col-sm-6" title="Attendance Month Setup" >
                     <a class="ds_Settings_Basic_Text cardStyle"  href="hrms/attendance/month-setup"> <i class="uil uil-calendar-alt iconStyle"></i><br /> Month Setup </a>
                     
                 </div>
                    <% } %>

                <% if (IsRouteExists("attendance/month-setup"))
                       { %>
                 <div  runat="server" id="divMonthSetupComp"  class="col-lg-2 col-md-3 col-sm-6" title="Attendance Month Setup" >
                     <a class="ds_Settings_Basic_Text cardStyle" href="attendance/month-setup"> <i class="uil uil-calendar-alt iconStyle"></i><br /> Month Setup </a>
                     
                 </div>
               <% } %>

                <% if (IsRouteExists("attendance/processing"))
                                       { %>
                 <div runat="server" id="divAttProcessing" class=" col-lg-2 col-md-3 col-sm-6" title="Machine Data Import">
                      <a class="ds_Settings_Basic_Text cardStyle" href="attendance/processing"><i class="uil uil-calendar-alt iconStyle"></i><br /> Att. Processing</a>
                 </div> 

                 <% } %>

                <% if (IsRouteExists("attendance/processing"))
                                       { %>
                 <div runat="server" id="divAttProcessingNew" class=" col-lg-2 col-md-3 col-sm-6" title="Machine Data Import">
                      <a class="ds_Settings_Basic_Text cardStyle" href="attendance/processing"><i class="uil uil-user-check iconStyle"></i><br /> Att. Processing (New)</a>
                 </div> 

                 <% } %>

                 <%--<div class="col-lg-2 col-md-3 col-sm-6" title="Daily Logout Setup" >
                     <a class="ds_attendance_Basic_Text" href="/attendance/monthly_logout_setup.aspx"> <img class="image_width_for_module" src="images/common/grade.ico" /><br /> Logout Setup</a>
                     
                 </div>
                 <div class=" col-lg-2 col-md-3 col-sm-6" title="Attendance Late Deduction">
                        <a class="ds_attendance_Basic_Text" href="/attendance/late_deduction.aspx"><img class="image_width_for_module" src="images/common/qualification.ico" /><br />Late Deduction</a>
                 </div>
                   <div class=" col-lg-2 col-md-3 col-sm-6" title="Attendance Late Deduction">
                        <a class="ds_attendance_Basic_Text" href="/attendance/employee-wise_hw_setup.aspx"><img class="image_width_for_module" src="images/common/qualification.ico" /><br />Emp Weekend</a>
                 </div>--%>

               <% if (IsRouteExists("attendance/manually-count"))
                                       { %>
                   <div runat="server" id="divManuallyCount" class=" col-lg-2 col-md-3 col-sm-6" title="Attendance Manually Count ">
                      <a class="ds_Settings_Basic_Text cardStyle" href="attendance/manually-count"><i class="uil uil-stopwatch iconStyle"></i><br />Manually Count</a>
                 </div>
                 <% } %>

                <% if (IsRouteExists("attendance/list"))
                                       { %>
                 <div runat="server" id="divAttendanceList" class=" col-lg-2 col-md-3 col-sm-6" title="All Attendance List">
                      <a class="ds_Settings_Basic_Text cardStyle" href="attendance/list"><i class="uil uil-document-layout-center iconStyle"></i><br />Attendance List</a>                    
                 </div>      
                   <% } %>
                 <div class="col-lg-2">

                 </div>
             
             </div>

               <div class="row ">
                   <div class="col-lg-2 ">
                   </div>
 
                     <% if (IsRouteExists("attendance/daily-summary-report"))
                                       { %>
                     <div runat="server" id="divAttSummary"  class=" col-lg-2 col-md-3 col-sm-6 " title="Daily Attendance Summary">
                         <a class="ds_Settings_Basic_Text cardStyle" href="attendance/daily-summary-report"><i class="uil uil-file-bookmark-alt iconStyle"></i><br />Att Summary </a> 
                 
                     </div>
                     <% } %>

                     <% if (IsRouteExists("attendance/daily-in-out-report"))
                                       { %>
                   <div runat="server" id="divInOutReport" class=" col-lg-2 col-md-3 col-sm-6 " title="Daily In-Out Report">

                       <a class="ds_Settings_Basic_Text cardStyle" href="attendance/daily-in-out-report"><i class="uil uil-signout iconStyle"></i><br />In-Out Report</a>
                   </div>
                      <% } %>
                   <% if (IsRouteExists("attendance/manual-report"))
                        { %>

                   <div  runat="server" id="divManualReport" class=" col-lg-2 col-md-3 col-sm-6 " title="Todays Attendance Stutus">
                    <a class="ds_Settings_Basic_Text cardStyle" href="attendance/manual-report"><i class="uil uil-file-bookmark-alt iconStyle"></i><br />Manual Report</a> 
                 </div>
                          <% } %>
                    <%--<div class=" col-lg-2 col-md-3 col-sm-6 " title="Daily Early Late Out Report">
                    <a class="ds_attendance_Basic_Text" href="/attendance/early_out_late_out.aspx"><img class="image_width_for_module" src="images/common/businesstype.ico" /><br />Early In-Out</a> 
                 </div>--%>
                <% if (IsRouteExists("attendance/monthly-report"))
                     { %>
                 <div runat="server" id="divMonthlyStatus"  class=" col-lg-2 col-md-3 col-sm-6" title="Monthly Attendance Status">
                     <a class="ds_Settings_Basic_Text cardStyle" href="attendance/monthly-report"><i class="uil uil-calender iconStyle"></i><br />Monthly Status</a>                
                 </div>
                <% } %>
                   <div class="col-lg-2 ">
                   </div>
             </div>
    
               <div class="row ">
                   <div class="col-lg-2">
                   </div>
                  
                   
                 <div runat="server" id="divAttReportDaterange" class=" col-lg-2 col-md-3 col-sm-6 " title="Manpower Wise Attendance Report">
                     <a class="ds_Settings_Basic_Text cardStyle" href="/attendance/att_report_daterange.aspx"><i class="uil uil-user-arrows iconStyle"></i><br />Attendance By Date Range</a>
                 </div> 

                     <% if (IsRouteExists("attendance/manpower-wise"))
                          { %>
                  <div runat="server" id="divManpowerWiseAttendance" class=" col-lg-2 col-md-3 col-sm-6 " title="Manpower Wise Attendance Report">
                     <a class="ds_Settings_Basic_Text cardStyle" href="attendance/manpower-wise"><i class="uil uil-user-arrows iconStyle"></i><br />Manpower Wise Attendance</a>
                 </div> 
                    <% } %>


                 <div runat="server" id="divOvertimeReport" class=" col-lg-2 col-md-3 col-sm-6 " title="Overtime Report">
                     <a class="ds_Settings_Basic_Text cardStyle" href="/attendance/overtime_report.aspx"><i class="uil uil-clock-eight iconStyle"></i><br />Overtime Report</a>
                 </div>  

                  <% if (IsRouteExists("attendance/out-duty-add"))
                     { %>
                     <div runat="server" id="divOutduty" class=" col-lg-2 col-md-3 col-sm-6 " title="Manpower Wise Attendance Report">
                         <a class="ds_Settings_Basic_Text cardStyle" href="attendance/out-duty-add"><i class="uil uil-signout iconStyle"></i><br />Outduty</a>
                     </div>
                 <% } %>

                 <% if (IsRouteExists("attendance/out-duty-list"))
                                   { %>
                       <div runat="server" id="divOutdutyList" class=" col-lg-2 col-md-3 col-sm-6 " title="Manpower Wise Attendance Report">
                         <a class="ds_Settings_Basic_Text cardStyle" href="attendance/out-duty-list"><i class="uil uil-document-layout-center iconStyle"></i><br />Outduty List</a>
                     </div>
                <% } %>
               
                <%-- <div class=" col-lg-2 col-md-3 col-sm-6 " title="Job Card">
                      <a class="ds_attendance_Basic_Text" href="/attendance/job_card_with_summary.aspx"><img class="image_width_for_module" src="images/common/application.ico" /><br />Job Card</a> 
                 </div> --%>
                   <div class="col-lg-2">
                   </div>
             </div>
               <div class="row ">
                   <div class="col-lg-2">
                   </div>

                     <% if (IsRouteExists("attendance/out-duty-approval"))
                                   { %>
                   <div runat="server" id="divOutdutyApproval" class=" col-lg-2 col-md-3 col-sm-6 " title="Overtime Report">
                     <a class="ds_Settings_Basic_Text" href="attendance/out-duty-approval"><i class="uil uil-check-circle iconStyle"></i><br />Outduty Approval</a>
                   </div>
                     <% } %>

                     <% if (IsRouteExists("attendance/out-duty-report"))
                                   { %>
                 <div runat="server" id="divOutdutyReport" class=" col-lg-2 col-md-3 col-sm-6 " title="Overtime Report">
                     <a class="ds_Settings_Basic_Text" href="attendance/out-duty-report"><i class="uil uil-file-bookmark-alt iconStyle"></i><br />Outduty Report</a>
                 </div>   
                <% } %>
                     <% if (IsRouteExists("attendance/absent-notification"))
                                   { %>
                   <div runat="server" id="divAbsentNotification" class=" col-lg-2 col-md-3 col-sm-6 " title="Absent Notification">
                     <a class="ds_Settings_Basic_Text" href="attendance/absent-notification"><i class="uil uil-snapchat-square iconStyle"></i><br />Absent Notification</a>
                 </div> 
                     <% } %>
                 <% if (IsRouteExists("attendance/manpower-statement"))
                      { %>
                   <div runat="server" id="divManpowerStatement" class=" col-lg-2 col-md-3 col-sm-6 " title="Manpower Statement">
                     <a class="ds_Settings_Basic_Text" href="attendance/manpower-statement"><i class="uil uil-file-bookmark-alt iconStyle"></i><br />Manpower Statement</a>
                 </div> 
                     <% } %>
                     <div class="col-lg-2">

                 </div>
             </div>

        <div class="row ">
                             <div class="col-lg-2">

                 </div>
                <% if (IsRouteExists("attendance/weekend-setup"))
                                   { %>
                 <div runat="server" id="divWeekendSetEmpWise" class="col-lg-2 col-md-3 col-sm-6" title="Weekend Setup" >
                     <a class="ds_attendance_Basic_Text" href="attendance/weekend-setup"> <i class="uil uil-calendar-alt iconStyle"></i><br /> Weekend Setup </a>
                     
                 </div>
               <% } %>
               <% if (IsRouteExists("attendance/general-day-setup"))
                     { %>
                 <div  runat="server" id="divGeneralDay"  class="col-lg-2 col-md-3 col-sm-6" title="General Day Setup" >
                     <a class="ds_Settings_Basic_Text" href="attendance/general-day-setup"> <i class="uil uil-calendar-alt iconStyle"></i><br /> General Day Setup </a>                
                 </div>
              <% } %>

             <% if (IsRouteExists("attendance/weekend-info-report"))
                { %>
              <div  runat="server" id="divWeekendInfoReport"  class="col-lg-2 col-md-3 col-sm-6" title="Weekend Info Report" >
                     <a class="ds_Settings_Basic_Text" href="attendance/weekend-info-report"> <i class="uil uil-file-bookmark-alt iconStyle"></i><br /> Weekend Info Report </a>                
                 </div>
              <% } %>

            <% if (IsRouteExists("attendance/attendance-analysis"))
               { %>
            <div  runat="server" id="div"  class="col-lg-2 col-md-3 col-sm-6" title="Analysis Report" >
                     <a class="ds_Settings_Basic_Text" href="attendance/attendance-analysis"> <i class="uil uil-document-layout-center iconStyle"></i><br />Analysis Report </a>                
                 </div>

              <% } %>
                                     <div class="col-lg-2">

                 </div>                       
    
             </div>
       <!--Attendace and Out Duty end here-->


        <!--Roster Configuration start-->


       <div class="row">


            <div class=" col-lg-2"></div>
         <% if (IsRouteExists("roster/create"))
             { %>
                 
            <div class="col-lg-2 col-md-3 col-sm-6" title="New Employee Entry">
                <a class="ds_Settings_Basic_Text" href="roster/create">
                    <img class="image_width_for_module" src="../images/common/addemployee.ico" /><br />
                    Roster Create &nbsp;&nbsp; Panel</a>
            </div>
           <% } %>
           <% if (IsRouteExists("roster/extend"))
             { %>
                <div class=" col-lg-2 col-md-3 col-sm-6" title="All Employee Details">
                <a class="ds_Settings_Basic_Text" href="roster/extend">
                    <img class="image_width_for_module" src="../images/common/employee detail.ico" /><br />
                    Roster Extend &nbsp;&nbsp; Panel</a>
            </div>
           <% } %>

           <% if (IsRouteExists("roster/view-remove"))
            { %>
            <div class=" col-lg-2 col-md-3 col-sm-6" title="Employee List Report">
                <a class="ds_Settings_Basic_Text" href="roster/view-remove">
                    <img class="image_width_for_module" src="../images/common/businesstype.ico" /><br />
                    Roster View & Remove</a>
            </div>
              <% } %>
            <% if (IsRouteExists("roster/transfer"))
            { %>
             <div class=" col-lg-2 col-md-3 col-sm-6" title="Roster Transer Panel">
                <a class="ds_Settings_Basic_Text" href="roster/transfer">
                    <img class="image_width_for_module" src="../images/common/employee detail.ico" /><br />
                    Roster Transfer Panel</a>
            </div> 
               <% } %>

            <div class=" col-lg-2"></div>
        </div>

        <div class="row">
             
            <div class=" col-lg-2"></div>
            
           <% if (IsRouteExists("roster/missing"))
            { %>
                  
             <div class=" col-lg-2 col-md-3 col-sm-6" title="All Employee Details">
                <a class="ds_Settings_Basic_Text" href="/personnel/roster_missing.aspx">
                    <img class="image_width_for_module" src="../images/common/employee detail.ico" /><br />
                    Roster Missing Panel</a>
            </div> 
            <% } %>

           <% if (IsRouteExists("roster/place-assign"))
            { %>

            <div class=" col-lg-2 col-md-3 col-sm-6" title="Employee List Report">
                <a class="ds_Settings_Basic_Text" href="roster/place-assign">
                    <img class="image_width_for_module" src="../images/common/businesstype.ico" /><br />
                   Place Assign  &nbsp;&nbsp; &nbsp;&nbsp;Panel</a>
            </div>

             <% } %>

           <% if (IsRouteExists("roster/report"))
            { %>

            <div class="col-lg-2 col-md-3 col-sm-6" title="New Employee Entry">
                <a class="ds_Settings_Basic_Text" href="roster/report">
                    <img class="image_width_for_module" src="../images/common/addemployee.ico" /><br />
                    Roster Manage Report</a>
            </div>
                                             <% } %>
           <% if (IsRouteExists("roster/report-by-date"))
            { %>
            <div class=" col-lg-2 col-md-3 col-sm-6" title="All Employee Details">
                <a class="ds_Settings_Basic_Text" href="roster/report-by-date">
                    <img class="image_width_for_module" src="../images/common/employee detail.ico" /><br />
                    Roster Report By Date Range</a>
            </div>
           <% } %>
                     
            <div class="col-lg-2"></div>
        </div>


         <!--Roster Configuration End-->








</div></div>
       <% } %>

</asp:Content>
