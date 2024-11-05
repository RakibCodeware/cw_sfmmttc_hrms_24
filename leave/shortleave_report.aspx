<%@ Page Title="Short Leave Report" Language="C#" MasterPageFile="~/leave_nested.master" AutoEventWireup="true" CodeBehind="shortleave_report.aspx.cs" Inherits="SigmaERP.leave.shortleave_report" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #ContentPlaceHolder1_MainContent_trForCompanyList {
            height:50px;
        }
        #ContentPlaceHolder1_MainContent_tblGenerateType td:nth-child(2), td:nth-child(4) {
            width:40%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="row">
        <div class="col-md-12 ds_nagevation_bar">
            <div style="margin-top: 5px">
                <ul>
                    <li><a href="/default.aspx">Dashboard</a></li>
                    <li>/</li>
                    <li><a href="/leave_default.aspx">Leave</a></li>
                    <li>/</li>
                    <li><a href="#" class="ds_negevation_inactive Lactive">Short Leave Report</a></li>
                </ul>
            </div>
        </div>
    </div>
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>   
     <asp:UpdatePanel ID="uplMessage" runat="server">
        <ContentTemplate>
            <p class="message" id="lblMessage" clientidmode="Static" runat="server"></p>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="main_box Lbox">
        <div class="main_box_header_leave LBoxheader">
            <h2>Short Leave Report</h2>
        </div>
    	<div class="main_box_body_leave Lbody">
            <div class="main_box_content_leave">
                <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlCompanyName" />                      
                        <asp:AsyncPostBackTrigger ControlID="lnkNew" />
                    </Triggers>
                    <ContentTemplate>
                <div class="bonus_generation" style="width: 61%; margin: 0px auto;">
                <h1  runat="server" visible="false" id="WarningMessage"  style="color:red; text-align:center"></h1>
                     <table runat="server" visible="true" id="tblGenerateType" class="division_table_leave1" style="margin:0px auto">                                                                      
                              
                                <tr id="trForCompanyList" runat="server">
                                <td>Company</td>
                                <td>
                                    <asp:DropDownList ID="ddlCompanyName" runat="server" ClientIDMode="Static" CssClass="form-control select_width" Width="96%" AutoPostBack="True"  OnSelectedIndexChanged="ddlCompanyName_SelectedIndexChanged" >
                                    </asp:DropDownList>
                                </td>
                             <td>Card No</td>
                           <td>
                                <asp:TextBox ID="txtCardNo" runat="server" PlaceHolder="For Individual" ClientIDMode="Static" CssClass="form-control text_box_width_import" Width="96%" ></asp:TextBox>
                           </td>
                             <td>
                              <asp:LinkButton runat="server" Text="New" ID="lnkNew"  ClientIDMode="Static" OnClientClick="return ClearCardNo();"></asp:LinkButton>
                           </td>
                           </tr>
               
                       <tr>
                          <td>From Date</td>                      
                           <td>
                            <asp:TextBox ID="txtFromDate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_import"></asp:TextBox>
                                <asp:CalendarExtender
                                    ID="CalendarExtender1" Format="dd-MM-yyyy" runat="server" Enabled="True" TargetControlID="txtFromDate">
                                </asp:CalendarExtender>
                           </td>                  
                           <td>To Date</td>                           
                           <td>
                             <asp:TextBox ID="txtToDate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_import"></asp:TextBox> 
                               <asp:CalendarExtender
                                    ID="CalendarExtender2" Format="dd-MM-yyyy" runat="server" Enabled="True" TargetControlID="txtToDate">
                                </asp:CalendarExtender>
                           </td>
                           
                       </tr>
                   </table>
                   
                     
                   
                </div>
                  <div id="workerlist" runat="server" class="id_card" style="background-color: white; width: 61%;">
                            <div class="id_card_left EilistL">
                                <asp:ListBox ID="lstAll" runat="server" CssClass="lstdata EilistCec" SelectionMode="Multiple"></asp:ListBox>
                            </div>
                            <div class="id_card_center EilistC">
                                <table style="margin-top: 0px;" class="employee_table">                    
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
                                        <asp:Button ID="btnRemoveItem" Class="arrow_button" runat="server" Text="<" OnClick="btnRemoveItem_Click"   />
                                    </td>
                               </tr>
                            <tr>
                                    <td>
                                        <asp:Button ID="btnRemoveAllItem" Class="arrow_button" runat="server" Text="<<" OnClick="btnRemoveAllItem_Click"   />
                                    </td>
                               </tr>
                        </table>
                    </div>
                      <div class="id_card_right EilistR">
                          <asp:ListBox ID="lstSelected" SelectionMode="Multiple" CssClass="lstdata EilistCec" ClientIDMode="Static" runat="server"></asp:ListBox>
                      </div>
                </div>
                <div class="payroll_generation_button">
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        
                                        <span style=" font-family:'Times New Roman'; font-size:20px; color:green;font-weight:bold; width:139px; float:left">
                                            <asp:Label runat="server" ID="lblProcess" text="wait processing"></asp:Label>
                                        <img style="width:26px;height:24px;cursor:pointer; margin-right:-56px" src="/images/wait.gif"  />  
                                    </ProgressTemplate>
                                </asp:UpdateProgress>

                    <asp:Button ID="btnPreview" runat="server" CssClass="Lbutton" Text="Preview" OnClick="btnPreview_Click" />
                    <asp:Button ID="Button3" runat="server" Text="Close" PostBackUrl="~/leave_default.aspx" CssClass="Lbutton" />
                </div>                
            </div>
                        </ContentTemplate>
                </asp:UpdatePanel>
        </div>
    </div>
     </div>
    <script type="text/javascript">
        
        function CloseWindowt() {
            window.close();
        }
        function ClearCardNo()
        {
            $("#txtCardNo").val("");
            $("#txtCardNo").focus();          
            
        }
            function goToNewTabandWindow(url) {
                window.open(url);

            }

         

            function CloseWindowt() {
                window.close();
            }

            function goToNewTabandWindow(url) {
                window.open(url);
            
            }
    </script>
</asp:Content>
