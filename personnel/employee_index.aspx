<%@ Page Title="Employee Information" Language="C#" MasterPageFile="~/personnel_NestedMaster.master" AutoEventWireup="true" CodeBehind="employee_index.aspx.cs" Inherits="SigmaERP.personnel.EmpInfo_Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
     <div class="row">
            <div class="col-md-12">
                <div class="ds_nagevation_bar" style="border-bottom:none;">
                   
                           <ul>
                               <li><a href="/default.aspx">Dasboard</a></li>
                               <li> <a class="seperator" href="#">/</a></li>
                               <li> <a href="/personnel_defult.aspx">Personnel</a></li>
                               
                           </ul>               
                    
                </div>
             </div>
        </div>
    <div>
   <% if (IsRouteExists("personnel"))
                       { %>
       <div class="col-lg-12">
        <div class="row">
            <div class=" col-lg-2"></div>
       <% if (IsRouteExists("employees/add"))
          { %>
            <div runat="server" id="divEmployeeEntry" class="col-lg-2 col-md-3 col-sm-6" title="New Employee Entry">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/add">
                    <img class="image_width_for_module" src="/images/common/addemployee.ico"/><br />
                    Employee Entry Panel</a>

            </div>
          <% } %>

         <% if (IsRouteExists("employees/list"))
          { %>

            <div runat="server" id="divEmployeeList" class=" col-lg-2 col-md-3 col-sm-6" title="All Employee Details">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/list">
                    <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                    All Employee List</a>
            </div>
          <% } %>
           <% if (IsRouteExists("employees/allow-to-compliance"))
           { %>


            <div runat="server" id="divPendingWorker" class=" col-lg-2 col-md-3 col-sm-6" title="All Employee Details">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/allow-to-compliance">
                    <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                    Pending Worker List</a>
            </div>
            <% } %>

                                            

            <% if (IsRouteExists("employees/profile"))
             { %>
            <div runat="server" id="divEmployeeProfileReport" class=" col-lg-2 col-md-3 col-sm-6" title="Employee Profile">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/profile">
                    <img class="image_width_for_module" src="/images/common/grade.ico" /><br />
                    Employee Profile Report</a>
            </div>
          <% } %>

      
           
            <div class="col-lg-2" ></div>
        </div>



        <div class="row">

            <div  class=" col-lg-2"></div>
           <% if (IsRouteExists("employees/info-report"))
             { %>
             <div runat="server" id="divEmployeeListReport" class=" col-lg-2 col-md-3 col-sm-6" title="Employee List Report">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/info-report">
                    <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                    Employee List Report</a>
            </div>
           <% } %>

      <% if (IsRouteExists("employees/separation/entry"))
          { %>
            <div runat="server" id="divSeparation" class=" col-lg-2 col-md-3 col-sm-6" title="Employee Seperation">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/separation/entry">
                    <img class="image_width_for_module" src="/images/common/add document.ico" /><br />
                    Seperation Entry Panel</a>

            </div>
        <% } %>

       <% if (IsRouteExists("employees/separation/entry"))
         { %>
            <div runat="server" id="divSeparationComp" class=" col-lg-2 col-md-3 col-sm-6" title="Employee Seperation">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/separation/entry">
                    <img class="image_width_for_module" src="/images/common/add document.ico" /><br />
                    Seperation Entry Panel</a>

            </div>

         <% } %>

       <% if (IsRouteExists("employees/separation/report"))
         { %>

            <div runat="server" id="divSeparationReport" class=" col-lg-2 col-md-3 col-sm-6" title="Employee Seperation Report">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/separation/report">
                    <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                    Seperation List Report</a>
            </div>

          <% } %>

     <% if (IsRouteExists("employees/separation/report"))
           { %>
            <div runat="server" id="divSeparationReportComp" class=" col-lg-2 col-md-3 col-sm-6" title="Employee Seperation Report">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/separation/report">
                    <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                    Seperation List Report</a>
            </div>
       <% } %>


       <% if (IsRouteExists("employees/man-power-status"))
       { %>
            
            <div runat="server" id="divManPowerStatusReport" class=" col-lg-2 col-md-3 col-sm-6" title="Provident Fund Report">

                <a class="ds_Settings_Basic_Text" href="/hrms/employees/man-power-status">
                    <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                    Man Power Status Report</a>
            </div>
            
        <% } %>

            <div class=" col-lg-2 "></div>
        </div>
        <div class="row">

            <div class=" col-lg-2 "></div>
            
          <% if (IsRouteExists("employees/monthly-man-power-status"))
             { %>
             <div  runat="server" id="divMonthlyManPowerReport"  class=" col-lg-2 col-md-3 col-sm-6" title="Provident Fund Report">

                <a class="ds_Settings_Basic_Text" href="/hrms/employees/monthly-man-power-status">
                    <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                    Monthly Man Power Report</a>
            </div>
            <% } %>
          <% if (IsRouteExists("employees/contact-list"))
             { %>
            <div runat="server" id="divContactListReport" class=" col-lg-2 col-md-3 col-sm-6" title="Blood Group Report">

                <a class="ds_Settings_Basic_Text" href="/hrms/employees/contact-list">
                    <img class="image_width_for_module" src="/images/common/Allowance.ico" /><br />
                    Contact List Report </a>
            </div>
          <% } %>

          <% if (IsRouteExists("employees/id-card"))
          { %>
            <div runat="server" id="divIDCardReport" class=" col-lg-2 col-md-3 col-sm-6" title="Staff ID Card">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/id-card">
                    <img class="image_width_for_module" src="/images/common/employee detail.ico" /><br />
                      ID Card Report</a>

            </div>
                <% } %>
          <%--  <div class=" col-lg-2 col-md-3 col-sm-6" title="Worker ID Card">

                <a class="ds_personnel_Basic_Text" href="/personnel/worker_id_card.aspx">
                    <img class="image_width_for_module" src="/images/common/employee detail.ico" /><br />
                    Worker ID Card Report</a>
            </div>--%>
                   <% if (IsRouteExists("employees/blood-group"))
                                       { %>
            <div runat="server" id="divBloodGroup" class=" col-lg-2 col-md-3 col-sm-6" title="Blood Group Report">
                <a class="ds_Settings_Basic_Text" href="/hrms/employees/blood-group">
                    <img class="image_width_for_module" src="/images/common/add document.ico" /><br />
                    Blood Group Report</a>
            </div>
              <% } %>
            <div class=" col-lg-2 "></div>
        </div>
        </div>

          <% } %>
        </div>
</asp:Content>
