<%@ Page Title="PF Reports" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="pf_report.aspx.cs" Inherits="SigmaERP.pf.pf_report" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div class="row Rrow">
                  <div class="col-md-12">
            <div class="ds_nagevation_bar">
                <ul>
                    <li><a href="/default.aspx">Dasboard</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li> 
                    <li><a class="seperator" href="#">/</a></li>
                      <li><a href="<%= Session["__topMenuPf__"] %>"> Provident Fund</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                     <li> <a href="#" class="ds_negevation_inactive Pactive">Provident Fund Reports</a></li>
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
       <div class="main_box Mbox">
        <div class="main_box_header PBoxheader">
            <h2>Provident Fund Reports</h2>
        </div>
        <div class="main_box_body Pbody">
            <div class="main_box_content">
                <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlCompanyName" />                       
                        <asp:AsyncPostBackTrigger ControlID="rblGenerateType" />
                        
                    </Triggers>
                    <ContentTemplate>
                  <div class="bonus_generation" style="width: 61%; margin: 0px auto;">           
                    <h1  runat="server" visible="false" id="WarningMessage"  style="color:red; text-align:center"></h1>
                      
                     <table runat="server" visible="true" id="tblGenerateType" class="bonus_generation_table super_admin_option">                                                                      
                      
                                <tr id="trForCompanyList" runat="server">
                                <td>Company&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                <td>
                                    <asp:DropDownList ID="ddlCompanyName" runat="server" ClientIDMode="Static" CssClass="form-control select_width"  AutoPostBack="True" OnSelectedIndexChanged="ddlCompanyName_SelectedIndexChanged"   >
                                    </asp:DropDownList>
                                </td>
                             <td>&nbsp;Type</td>
                           <td>
                                        <asp:RadioButtonList ID="rblGenerateType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True" OnSelectedIndexChanged="rblGenerateType_SelectedIndexChanged"   >
                                        <asp:ListItem Selected="True" Text="All" Value="0"></asp:ListItem>
                                        <asp:ListItem Selected="False" Text="Individual" Value="1"></asp:ListItem>
                                    </asp:RadioButtonList>
                                
                                    </td>
                                    
                           </tr>                  
               
                       <tr>
                          <td runat="server" id="tdMonth">Month</td>
                      
                           <td>
                                <asp:TextBox ID="txtFromMonth"   runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_import" PLaceHolder="Click For Calendar" autocomplete="off"></asp:TextBox>
                                            <asp:CalendarExtender ID="txtPartialAttDate_CalendarExtender" runat="server" Format="dd-MM-yyyy" TargetControlID="txtFromMonth">
                                            </asp:CalendarExtender>
                                <%--<asp:TextBox ID="txtMonth" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_import" PLaceHolder="Click For Calendar"></asp:TextBox>
                                            <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="MM-yyyy" TargetControlID="txtMonth">
                                            </asp:CalendarExtender>--%>

                           </td>
                         
                            <td> to Month</td>
                      
                           <td>
                               <asp:TextBox Enabled="false" ID="txtToMonth" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_import" PLaceHolder="Click For Calendar" autocomplete="off"></asp:TextBox>
                                            <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MM-yyyy" TargetControlID="txtToMonth">
                                            </asp:CalendarExtender>
                           </td>
                  
                           <td>&nbsp;Card No &nbsp;</td>                           
                           <td>
                               <asp:TextBox ID="txtEmpCardNo" runat="server" ClientIDMode="Static" PlaceHolder="For Individual" CssClass="form-control text_box_width_import" Enabled="False" ></asp:TextBox>
                               
                           </td>
                       </tr>
                       
                          <tr>
                          <td>Report &nbsp;</td>                      
                           <td colspan="5">
                                 <asp:RadioButtonList ID="rblReportType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="rblReportType_SelectedIndexChanged">
				                    <asp:ListItem  Value="MonthlySheet" Text="Monthly PF Sheet" Selected="True"></asp:ListItem>                                   
                                    <asp:ListItem  Value="BalanceSheet" Text="PF Balance Sheet"></asp:ListItem>
                                     <asp:ListItem  Value="BalanceSummary" Text="PF Balance Summary"></asp:ListItem>                                   
			                    </asp:RadioButtonList>
                           </td>                                           
                       </tr> 
                          <tr>
                            <td>Employee Type</td>                           
                            <td><asp:RadioButtonList ID="rblEmployeeType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"></asp:RadioButtonList>
                            </td>
                           </tr>
                   </table>
                          
                </div>
                       
                           <div runat="server" visible="false">
                        <asp:Panel id="pnl1" runat="server" >
                   <div id="divDepartmentList" runat="server" class="id_card" style="background-color: white; width: 61%;">
                               <div class="id_card_left EilistL">
                                <asp:ListBox ID="lstAll" runat="server" CssClass="lstdata EilistCec" style="height:270px !important" SelectionMode="Multiple"></asp:ListBox>
                            </div>
                            <div class="id_card_center EilistC" > 
                                 <table style="margin-top:60px;" class="employee_table">                      
                              <tr>
                                    <td>
                                        <asp:Button ID="btnAddItem" Class="arrow_button" runat="server" Text=">" OnClick="btnAddItem_Click"   />
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
                                    <asp:ListBox ID="lstSelected" SelectionMode="Multiple" CssClass="lstdata EilistCec" style="height:270px !important" ClientIDMode="Static" runat="server"></asp:ListBox>
                                </div>
                </div>
                            </asp:Panel>
                               </div>
                            
                <div class="payroll_generation_button">
                    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        
                                        <span style=" font-family:'Times New Roman'; font-size:20px; color:green;font-weight:bold; width:139px; float:left">
                                            <asp:Label runat="server" ID="lblProcess" text="wait processing"></asp:Label>
                                        <img style="width:26px;height:24px;cursor:pointer; margin-right:-56px" src="/images/wait.gif"  />  
                                    </ProgressTemplate>
                                </asp:UpdateProgress>

                    <asp:Button ID="btnPreview" runat="server" CssClass="Pbutton" Text="Preview" OnClick="btnPreview_Click"  />
                    <asp:Button ID="Button3" runat="server" Text="Close" PostBackUrl="~/payroll_default.aspx" CssClass="Pbutton" />
                </div>
                
            </div>
                        </ContentTemplate>
                </asp:UpdatePanel>
        </div>
    </div>
     </div>

    <script type="text/javascript">
        function InputValidationBasket() {
            try {

                if ($('#txtEmpCardNo').val().trim().length < 4) {
                    showMessage('Please select To Date', 'error');
                    $('#txtToDate').focus(); return true;
                }
                return true;
            }
            catch (exception) {
                showMessage(exception, error)
            }
        }

        function CloseWindowt() {
            window.close();
        }

        function goToNewTabandWindow(url) {
            window.open(url);

        }

        function getSalaryMonth() {

            var val = document.getElementById('ddlMonthID').value;
            document.getElementById('txtMonthId').value = val;

        }

        function CloseWindowt() {
            window.close();
        }

        function goToNewTabandWindow(url) {
            window.open(url);

        }



    </script>
</asp:Content>