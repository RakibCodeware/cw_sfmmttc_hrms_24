<%@ Page Title="Settings" Language="C#" MasterPageFile="~/hrd_nested.master" AutoEventWireup="true" CodeBehind="hrd_default.aspx.cs" Inherits="SigmaERP.hrd_default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar" style="border-bottom: 0;">
                <div style="margin-top: 5px">
                    <ul>
                        <li><a href="/default.aspx">Dashboard</a></li>
                        <li><a class="seperator" href="/hrd_default.aspx">/</a></li>
                        <li><a href="/hrd_default.aspx" class="ds_negevation_inactive Ractive">Settings</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <%--  <img alt="" class="main_images" src="images/hrd.png">--%>
    <div>
        <% if (IsRouteExists("settings"))
            { %>

        <div class="col-lg-12" style="margin-top: 10%">
            <div class="row">

                <div class=" col-md-2"></div>
                <% if (IsRouteExists("settings/department"))
                    { %>
                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/department">
                        <img class="image_width_for_module" src="/images/common/department.ico" /><br />
                        Course name</a>

                </div>

                <%} %>
                <% if (IsRouteExists("settings/designation"))
                    { %>
                <div class=" col-md-2">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/designation">
                        <img class="image_width_for_module" src="/images/common/designation.ico" /><br />
                        Designation</a>
                </div>
                <%} %>
                <% if (IsRouteExists("settings/grade"))
                    { %>
                <div class=" col-md-2">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/grade">
                        <img class="image_width_for_module" src="/images/common/grade.ico" /><br />
                        Grade</a>
                </div>
                <%} %>
                <% if (IsRouteExists("settings/shift"))
                    { %>
                <div class=" col-md-2">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/shift">
                        <img class="image_width_for_module" src="/images/common/Class Schedule.ico" /><br />
                        Shift</a>
                </div>
                <%} %>


                <div class=" col-md-2"></div>
            </div>

            <div class="row">

                <div class=" col-md-2"></div>

      <%--          <div class=" col-md-2  ">
                    <a class="ds_Settings_Basic_Text" href="/hrms/hrd/SpecialTimeTable.aspx">
                        <img class="image_width_for_module" src="/images/common/Class Schedule.ico" /><br />
                        Special Timetable</a>

                </div>--%>

        <%--        <div class=" col-md-2  ">
                    <a class="ds_Settings_Basic_Text" href="/hrms/hrd/SpecialBreakTime.aspx">
                        <img class="image_width_for_module" src="/images/common/Class Schedule.ico" /><br />
                        Special Breaks</a>

                </div>--%>
                <% if (IsRouteExists("settings/religion"))
                    { %>
                <div class=" col-md-2  ">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/religion">
                        <img class="image_width_for_module" src="/images/common/religion.ico" /><br />
                        Religion</a>

                </div>
                <%} %>

                <% if (IsRouteExists("settings/district"))
                    { %>
                <div class=" col-md-2  ">

                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/district">
                        <img class="image_width_for_module" src="/images/common/distric add.ico" /><br />
                        District</a>
                </div>
                <%} %>
                <div class=" col-md-2"></div>
            </div>

            <div class="row">

                <div class=" col-md-2"></div>
                <% if (IsRouteExists("settings/thana"))
                    { %>
                <div class=" col-md-2  ">

                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/thana">
                        <img class="image_width_for_module" src="/images/common/thana add.ico" /><br />
                        Thana</a>
                </div>
                <%} %>
                <% if (IsRouteExists("settings/stamp-deduction"))
                    { %>
                <div class=" col-md-2  ">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/stamp-deduction">
                        <img class="image_width_for_module" src="/images/common/add document.ico" /><br />
                        Stamp Deduction</a>

                </div>
                <%} %>

                <% if (IsRouteExists("settings/line-group"))
                    { %>
                <div class=" col-md-2  ">

                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/line-group">
                        <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                        Line/Group</a>
                </div>
                <%} %>
                <% if (IsRouteExists("settings/company"))
                    { %>
                <div class=" col-md-2  ">

                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/company">
                        <img class="image_width_for_module" src="/images/common/company.ico" /><br />
                        Company</a>
                </div>
                <%} %>
                <div class=" col-md-2"></div>
            </div>


            <div class="row">

                <% if (IsRouteExists("settings/others"))
                    { %>
                <div class=" col-md-2"></div>

                <div class=" col-md-2  ">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/others">
                        <img class="image_width_for_module" src="/images/common/others.ico" /><br />
                        Others</a>
                </div>
                <%} %>
                <% if (IsRouteExists("settings/floor"))
                    { %>
                <div class=" col-md-2  ">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/floor">
                        <img class="image_width_for_module" src="/images/common/add document.ico" /><br />
                        Floor</a>

                </div>
                <%} %>
                <% if (IsRouteExists("settings/business"))
                    { %>
                <div class=" col-md-2  ">

                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/business">
                        <img class="image_width_for_module" src="/images/common/businesstype.ico" /><br />
                        Business Type</a>
                </div>
                <%} %>
                <% if (IsRouteExists("settings/qualification"))
                    { %>

                <div class=" col-md-2  ">
                    <a class="ds_Settings_Basic_Text" href="/hrms/settings/qualification">
                        <img class="image_width_for_module" src="/images/common/qualification.ico" /><br />
                        Qualification</a>

                </div>
                <%} %>
                <%-- <div class=" col-md-2  ">

                      <a class="ds_Settings_Basic_Text" href="/hrd/CompanyInfo.aspx"><img class="image_width_for_module" src="images/common/company.ico" /><br />Company</a> 
                 </div>
                   <div class=" col-md-2  ">

                     <a class="ds_Settings_Basic_Text" href="/hrd/others_settings.aspx"><img class="image_width_for_module" src="images/common/others.ico" /><br />Others</a> 
                 </div>--%>
                <div class=" col-md-2"></div>
            </div>


        </div>
        <%} %>
    </div>
</asp:Content>
