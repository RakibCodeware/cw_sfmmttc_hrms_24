<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="SigmaERP.hrms.test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

      <%--  <style>
                   ul {
            list-style-type: none;
            padding-left: 0;
            margin: 0;
        }
        li {
            position: relative; /* Needed for the child menu to be positioned correctly */
            padding: 5px 0;
        }
        li ul {
            margin-left: 0;
            padding-left: 0;
            position: absolute; /* Position child menu absolutely */
            top: 100%; /* Position below the parent item */
            left: 0;
            display: none; /* Hide by default */
            background-color: #f9f9f9; /* Background color for child menu */
            border: 1px solid #ddd; /* Border for child menu */
            z-index: 1; /* Ensure it appears on top */
        }
        li:hover > ul {
            display: block; /* Show child menu on hover */
        }
        li ul li {
            padding: 5px 10px; /* Padding for child items */
        }
        li ul ul {
            top: 0; /* Position the sub-menu to the right of its parent menu item */
            left: 100%; /* Shift the sub-menu to the right of the parent menu */
            display: none; /* Hide by default */
            background-color: #f9f9f9; /* Background color for sub-menu */
            border: 1px solid #ddd; /* Border for sub-menu */
        }
        li ul li:hover > ul {
            display: block; /* Show sub-menu on hover */
        }--%>
<%--    </style>--%>
    <h1>Dynamic Manu teest</h1>

      <asp:Literal ID="ltMenu" runat="server"></asp:Literal>
</asp:Content>
