<%@ Page Title="Provident Fund" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="pf_index.aspx.cs" Inherits="SigmaERP.pf.pf_index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar" style="border-bottom: none;">

                <ul>
                    <li><a href="/default.aspx">Dasboard</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Provident Fund(PF)</a></li>
                </ul>

            </div>
        </div>
    </div>

    <%--  <img alt="" class="main_/images" src="/images/hrd.png">--%>
    <div>

        <% if (IsRouteExists("provident-found"))
            { %>
        <div class="col-lg-12">
            <div class="row">
                <div class=" col-md-2">
                </div>
                <% if (IsRouteExists("provident-found/pf-calculation"))
                    { %>
                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/pf-calculation"><i class="fas fa-money-check-alt FsiconStyle"></i>
                        <br />
                        PF Setting</a>

                </div>
                <% } %>

                <% if (IsRouteExists("provident-found/Add"))
                    { %>
                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/Add"><i class="fas fa-file-alt FsiconStyle"></i>
                        <br />
                        PF Entry Panel </a>
                </div>
                <% } %>

                <% if (IsRouteExists("provident-found/manually-add"))
                    { %>
                <div class=" col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/manually-add"><i class="fas fa-file-alt FsiconStyle"></i>
                        <br />
                        Manualy Entry</a>
                </div>
                <% } %>

                <% if (IsRouteExists("provident-found/pf-investment"))
                    { %>
                <div class=" col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/pf-investment"><i class="uil uil-money-bill-stack iconStyle"></i>
                        <br />
                        Investment</a>
                </div>
                <% } %>
            </div>
            <div class="row">
                <div class=" col-md-2">
                </div>
                <% if (IsRouteExists("provident-found/investment-withdraw"))
                    { %>
                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/investment-withdraw"><i class="uil uil-money-withdrawal iconStyle"></i>
                        <br />
                        Investment Withdraw</a>

                </div>
                <% } %>

                <% if (IsRouteExists("provident-found/pf-expense"))
                    { %>
                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/pf-expense"><i class="fa-solid fa-hand-holding-dollar iconStyle "></i>
                        <br />
                        PF Expense</a>
                </div>
                <% } %>

                <% if (IsRouteExists("provident-found/pf-distribution"))
                    { %>
                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/pf-distribution"><i class="fa-solid fa-network-wired FsiconStyle"></i>
                        <br />
                        Profit Distribution</a>

                </div>
                <% } %>


                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/pf/pf_report.aspx"><i class="fa-solid fa-chart-line FsiconStyle"></i>
                        <br />
                        PF Reprots</a>
                </div>

            </div>
            <div class="row">
                <div class=" col-md-2">
                </div>
                <% if (IsRouteExists("provident-found/pf-withdraw"))
                    { %>
                <div class="col-md-2">
                    <a class="ds_Settings_Basic_Text " href="/hrms/provident-found/pf-withdraw"><i class="uil uil-money-withdrawal iconStyle"></i>
                        <br />
                        PF Withdraw</a>

                </div>
                <% } %>
            </div>
        </div>
        <% } %>
    </div>
</asp:Content>
