<%@ Page Title="" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="att_report_daterange.aspx.cs" Inherits="SigmaERP.attendance.att_report_daterange" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .division_table_leave1 {
            width: 100%;
        }

        .division_table_leave1 tr {
                height: 35px;
        }
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar">
                <ul>
                    <li><a href="/default.aspx">Dashboard</a></li>
                    <li>/</li>
                   <li><a href="<%=  Session["__topMenu__"] %>">Attendance</a></li>
                    <li>/</li>
                    <li><a href="#" class="ds_negevation_inactive Mactive">Attendance Report By Date Range</a></li>
                </ul>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="uplMessage" runat="server" >
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <Triggers>
           <asp:AsyncPostBackTrigger ControlID="ddlCompany" />          
        </Triggers>
        <ContentTemplate>
            <div class="main_box Mbox">
         <div class="main_box_header MBoxheader">
                    
                    <h2>Attendance Report By Date Range</h2>
                </div>
               <%-- <div class="main_box_body">
                    <div class="main_box_content">
                        <div style="text-align:-moz-center" class="bonus_generation">--%>
                <div class="employee_box_body">
                    <div class="employee_box_content">

                <div class="bonus_generation" style="width:61%; margin:0px auto;">

<h1  runat="server" visible="false" id="WarningMessage"  style="color:red; text-align:center"></h1>
                    <center>
                          <asp:RadioButtonList  class="rb" ID="rblReportType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0" Selected="True">Daily Attendance Staus</asp:ListItem>
                                    <asp:ListItem Value="1">Attendance Summary</asp:ListItem>
                                    <%--<asp:ListItem Value="2">Individual Group</asp:ListItem>--%>
                                    <asp:ListItem Value="3">Absent Report</asp:ListItem>
                                </asp:RadioButtonList>                        
                    </center>
                    
                    <table runat="server" visible="true" id="tblGenerateType" class="division_table_leave1">

                        <tr id="trCompanyName" runat="server" visible="false">
                            <td>Company
                            </td>
                            <td>:</td>
                            <td>
                                <asp:DropDownList ID="ddlCompany" runat="server" AutoPostBack="true" ClientIDMode="Static" CssClass="form-control select_width" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>Shift 
                            </td>
                            <td>:
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlShift" ClientIDMode="Static" CssClass="form-control select_width" runat="server">
                                </asp:DropDownList>

                            </td>
                        
                        </tr>
                        <tr>
                           
                                  <td>Employee Type
                            </td>
                            <td>:</td>
                            <td>
                                <asp:RadioButtonList runat="server" ID="rblEmpType" AutoPostBack="true" RepeatDirection="Horizontal">
                                </asp:RadioButtonList>
                            </td>
                            
                          
                            <td runat="server" visible="false">Status</td>
                            <td runat="server" visible="false">:</td>
                            <td runat="server" visible="false">
                                <asp:RadioButtonList class="rb" ID="rblAttStatus" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="All" Selected="True">All</asp:ListItem>
                                    <asp:ListItem Value="P">Pesent</asp:ListItem>
                                    <asp:ListItem Value="L">Late</asp:ListItem>
                                    <asp:ListItem Value="A">Absent</asp:ListItem>
                                    <asp:ListItem Value="Lv">Leave</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                          
                        </tr>
                        <tr>
                              <td>From Date
                            </td>
                            <td>:</td>
                            <td>
                                <asp:TextBox ID="txtDate" ClientIDMode="Static" runat="server" CssClass="form-control text_box_width"></asp:TextBox>
                                <asp:CalendarExtender
                                    ID="TextBoxDate_CalendarExtender" Format="dd-MM-yyyy" runat="server" Enabled="True" TargetControlID="txtDate">
                                </asp:CalendarExtender>
                            </td>
                            <td>To Date
                            </td>
                            <td>:</td>
                            <td>
                                <asp:TextBox ID="txtToDate" ClientIDMode="Static" runat="server" CssClass="form-control text_box_width"></asp:TextBox>
                                <asp:CalendarExtender
                                    ID="CalendarExtender1" Format="dd-MM-yyyy" runat="server" Enabled="True" TargetControlID="txtToDate">
                                </asp:CalendarExtender>
                            </td>

                         

                        </tr>
                        <tr runat="server">
                               
                            <td>Sort by 
                            </td>
                            <td>:</td>
                            <td>
                                <asp:RadioButtonList  class="rb" ID="rblOrderBy" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="0" Selected="True">Emp. Wise</asp:ListItem>
                                    <asp:ListItem Value="1">Date Wise</asp:ListItem>
                                </asp:RadioButtonList>   
                            </td>
                            <td>Card No
                            </td>
                            <td>:</td>
                            <td>
                                <asp:TextBox ID="txtCardNo" ClientIDMode="Static" runat="server" PlaceHolder=" For Individual" CssClass="form-control text_box_width"></asp:TextBox>

                            </td>
                            <td>
                                <asp:LinkButton ID="lnkNew" Text="New" runat="server" OnClientClick="InputBoxNew()"></asp:LinkButton>

                            </td>
                        </tr>


                    </table>
                </div>
                      <div id="workerlist" runat="server" class="id_card" style="background-color:white; width:61%;">
                            <div class="id_card_left EilistL">
                                <asp:ListBox ID="lstAll" runat="server" CssClass="lstdata EilistCec" style="height:270px !important" SelectionMode="Multiple"></asp:ListBox>
                            </div>
                            <div class="id_card_center EilistC" >
                                <table style="margin-top:60px;" class="employee_table">                    
                              <tr>
                                    <td >
                                        <asp:Button ID="btnAddItem" Class="arrow_button" runat="server" Text=">" OnClick="btnAddItem_Click"  />
                                    </td>
                               </tr>
                            <tr>
                                    <td>
                                        <asp:Button ID="btnAddAllItem" Class="arrow_button" runat="server" Text=">>" OnClick="btnAddAllItem_Click"  />
                                    </td>
                               </tr>
                            <tr>
                                    <td>
                                        <asp:Button ID="btnRemoveItem" Class="arrow_button" runat="server" Text="<" OnClick="btnRemoveItem_Click"  />
                                    </td>
                               </tr>
                            <tr>
                                    <td>
                                        <asp:Button ID="btnRemoveAllItem" Class="arrow_button" runat="server" Text="<<" OnClick="btnRemoveAllItem_Click"  />
                                    </td>
                               </tr>
                        </table>
                    </div>
                     <div class="id_card_right EilistR">
                                <asp:ListBox ID="lstSelected" SelectionMode="Multiple" CssClass="lstdata EilistCec"  style="height:270px !important"  ClientIDMode="Static" runat="server"></asp:ListBox>
                            </div>
                </div>
                        <div class="job_card_button_area">
                            <div style="display:none">
                                <center>
                              <asp:RadioButtonList class="rb" ID="rblPrintType" runat="server"  RepeatDirection="Horizontal" >
                                    <asp:ListItem Text="For View" Value="0" ></asp:ListItem>
                                     <asp:ListItem Text="For Print" Value="1" Selected="True" ></asp:ListItem>
                                </asp:RadioButtonList>                                    
                                    <asp:RadioButtonList class="rb" ID="rblLanguage" runat="server"  RepeatDirection="Horizontal" >
                                    <asp:ListItem Text="English" Value="EN" Selected="True" ></asp:ListItem>
                                     <asp:ListItem Text="Bangla" Value="BN"></asp:ListItem>
                                </asp:RadioButtonList>
                                    </center>
                            </div>
                            <asp:Button ID="btnPreview" CssClass="Mbutton" runat="server" ValidationGroup="save" OnClientClick="return InputValidation();" Text="Preview" OnClick="btnPreview_Click" />
                            &nbsp; &nbsp; &nbsp;
                    <asp:Button ID="Button3" runat="server" Text="Close" PostBackUrl="~/default.aspx" CssClass="Mbutton" />
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
     <script type="text/javascript">  
         $(document).ready(function () {
             $("#ddlShiftList").select2();
         });
         function load() {
             $("#ddlShiftList").select2();
         }
         function goToNewTabandWindow(url) {
             window.open(url);
             load();
         }        
         function InputBoxNew()
         {
           
             $('#txtCardNo').val('');
         }
         function InputValidation()
         {
             if (validateText('txtDate', 1, 100, "Please Select Attendance Date ") == false) return false;
             return true;             
         }
    </script>
</asp:Content>
