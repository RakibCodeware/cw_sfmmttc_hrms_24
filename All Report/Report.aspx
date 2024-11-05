<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="SigmaERP.All_Report.Repor" %>
<%--<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>--%>
<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../style/report.css" rel="stylesheet" />
<script lang="javaScript" type="text/javascript" src="crystalreportviewers13/js/crviewer/crv.js"></script> 
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <asp:ScriptManager ID="ScriptManager1" runat="server">

    </asp:ScriptManager>  
     <asp:UpdatePanel runat="server" ID="up1">
        <ContentTemplate>
    <div runat="server" id="divError"></div>
    <div>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server"  AutoDataBind="true" ToolPanelView="None" PrintMode="ActiveX"   />
    </div>
        </ContentTemplate>
         </asp:UpdatePanel>     
    <br />
    </div>
    </form>
</body>
</html>
