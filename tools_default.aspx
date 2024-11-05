<%@ Page Title="Tools" Language="C#" MasterPageFile="~/Tools_Nested.master" AutoEventWireup="true" CodeBehind="tools_default.aspx.cs" Inherits="SigmaERP.tools_defaults" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar" style="border-bottom:none;">
                <div style="margin-top: 5px">
                    <ul>
                        <li><a href="/default.aspx">Dasboard</a></li>
                        <li>/</li>
                        <li><a href="#" class="ds_negevation_inactive Tactive">Tools</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <%--  <img alt="" class="main_images" src="images/hrd.png">--%>
    <div>
    <div class="col-lg-12" style="margin-top: 10%">
        <div class="row">

            <div class=" col-lg-2"></div>

            <div class="col-lg-2 col-md-3 col-sm-6">
                <a class="ds_Settings_Basic_Text " href="/ControlPanel/CreateAccount.aspx">
                    <i class="fas fa-users FsiconStyle"></i><br />
                   <span> User Account</span></a>

            </div>
            <div class=" col-lg-2 col-md-3 col-sm-6">
                <a class="ds_Settings_Basic_Text" href="/ControlPanel/changepassword.aspx">
                    <i class="fas fa-unlock-alt FsiconStyle"></i><br />
                    <span>C. Password</span></a>
            </div>
            <div class=" col-lg-2 col-md-3 col-sm-6">
                <a class="ds_Settings_Basic_Text " href="/ControlPanel/user_privilege.aspx">
                    <i class="fas fa-users-cog FsiconStyle"></i><br />
                   <span> User Privilege</span></a>
            </div> 
            <div class=" col-lg-2 col-md-3 col-sm-6">
                <a class="ds_Settings_Basic_Text " href="/ControlPanel/authority-access-control.aspx">
                    <i class="fas fa-universal-access FsiconStyle"></i><br />
                    <span>Authority Access Control</span></a>
            </div>
          <%--  <div class=" col-lg-2 col-md-3 col-sm-6">
                <a class="ds_Settings_Basic_Text Tbox" href="/hrd/shift_config.aspx">
                    <img class="image_width_for_module" src="images/common/Class Schedule.ico" /><br />
                    Db Backup</a>
            </div>--%>
             <%--<div class=" col-lg-2 col-md-3 col-sm-6">
                <a class="ds_Settings_Basic_Text Tbox" href="/ModifyPage.aspx">
                    <img class="image_width_for_module" src="images/common/Class Schedule.ico" /><br />
                   Modify</a>
            </div>--%>
            <div class=" col-lg-2 "></div>
        </div>
    </div></div>
</asp:Content>
