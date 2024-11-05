<%@ Page Title="PF Profit Distribution " Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="pf_interest_distribution.aspx.cs" Inherits="SigmaERP.pf.pf_interest_distribution" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <style>
        #ContentPlaceHolder1_ContentPlaceHolder1_gvPFSettings th,td {
            text-align:center;
        }
    </style>
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
                     <li> <a href="#" class="ds_negevation_inactive Pactive">PF Profit Distribution</a></li>
                </ul>
            </div>
        </div>
       </div>  
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<asp:UpdatePanel ID="uplMessage" runat="server" >
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>
    <asp:HiddenField ID="hdnUpdate" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdnbtnStage" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="upSave" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="upupdate" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="updelete" runat="server" ClientIDMode="Static" />
    <div class="main_box Mbox">
    	<div class="main_box_header PBoxheader">
            <h2>PF Profit Distribution</h2>
        </div>
    	<div class="main_box_body Pbody">
        	<div class="main_box_content">
                  <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSave" />  
             <asp:AsyncPostBackTrigger ControlID="btnNew" />  
            <asp:AsyncPostBackTrigger ControlID="ddlCompanyName" />   
            <asp:AsyncPostBackTrigger  ControlID="rblDistribution"/>       
        </Triggers>
        <ContentTemplate>
                <div class="input_division_info">
                   <div class="row">
                        <div class="col-sm-12">
                             <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Company Name <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:RadioButtonList runat="server" ID="rblDistribution" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblDistribution_SelectedIndexChanged">
                                        <asp:ListItem Value="Profit" Selected="True">Profit</asp:ListItem>
                                        <asp:ListItem Value="Expense">Expense</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                              </div>
                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Company Name <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:DropDownList ID="ddlCompanyName" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" AutoPostBack="true"  OnSelectedIndexChanged="ddlCompanyName_SelectedIndexChanged" >
                                                                        
                                      </asp:DropDownList>
                                </div>
                              </div>
                            <div class="row tbl-controlPanel" runat="server" id="divFdrNo">
                                <div class="col-sm-4">Investment No <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:DropDownList ID="ddlFDRList" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" >                                                                        
                                      </asp:DropDownList>                                                                   
                                </div>

                              </div>  
                             <div class="row tbl-controlPanel" runat="server" id="divDateRange">
                                <div class="col-sm-4">Date <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:TextBox runat="server" ID="txtDate" placeholder="yyyy-MM-dd" ClientIDMode="Static" CssClass="form-control select_width" ></asp:TextBox>  
                                     <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="yyyy-MM-dd" TargetControlID="txtDate">
                                    </asp:CalendarExtender>  
                                      <asp:TextBox runat="server" ID="txtToDate" placeholder="yyyy-MM-dd" ClientIDMode="Static" CssClass="form-control select_width" ></asp:TextBox>  
                                     <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="yyyy-MM-dd" TargetControlID="txtToDate">
                                    </asp:CalendarExtender>                                                               
                                </div>

                              </div>  
                          
                            <div class="row tbl-controlPanel" runat="server" id="divYear" visible="false">
                                <div class="col-sm-4">Year<span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:DropDownList ID="ddlYear" runat="server" ClientIDMode="Static"  CssClass="form-control select_width"  >                                                                        
                                      </asp:DropDownList>                                                                   
                                </div>
                              </div>     
                              <div class="row tbl-controlPanel">
                                <center>
                                  
                                    <asp:Image ID="imgLoading" runat="server" ImageUrl="~/images/loading.gif" ClientIDMode="Static"  />
                                </center>
                               
                            </div>                        
                        </div>
                    </div>                    
                </div>      
                <div class="button_area Rbutton_area">
                    <a href="#" onclick="window.history.back()" class="Pbutton">Back</a>
                    <asp:Button ID="btnNew" ClientIDMode="Static" CssClass="Pbutton"  runat="server" Text="New" />
                    <asp:Button ID="btnSave" Visible="false" OnClientClick="return validateInputs();"  ClientIDMode="Static" class="Pbutton" runat="server" Text="Process" OnClick="btnSave_Click"   />
                    <asp:Button ID="Button1" OnClientClick="return processing();" ClientIDMode="Static" class="Pbutton" runat="server" Text="Process PerDay" OnClick="Button1_Click"   />
                    <asp:Button ID="btnClose" ClientIDMode="Static" CssClass="Pbutton" PostBackUrl="~/hrd_default.aspx"  runat="server" Text="Close" />
                </div>
            </ContentTemplate>
                      </asp:UpdatePanel>

                  <div class="show_division_info">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">    
        <ContentTemplate>
                     <%--<div id="ShiftConfig" class="datatables_wrapper" runat="server" style="width:100%; height:auto; max-height:500px;overflow:auto;overflow-x:hidden;"></div>--%>
                    <asp:GridView ID="gvPFSettings" runat="server"  Width="100%" AutoGenerateColumns="False" DataKeyNames="CompanyId,FdrID" AllowPaging="True" >
<HeaderStyle BackColor="#FFA500" Font-Bold="True" Font-Size="14px" ForeColor="White" Height="28px"></HeaderStyle>
                        <PagerStyle CssClass="gridview Sgridview" Height="40px" />
                       <RowStyle HorizontalAlign="Center" />
                         <Columns> 
                             <asp:BoundField DataField="FdrNo" HeaderText="FDR No"  ItemStyle-Height="28px" ItemStyle-HorizontalAlign="center"   >
<ItemStyle Height="28px" ></ItemStyle>
                             </asp:BoundField>                     
                             <asp:BoundField DataField="InterestAmount" HeaderText="Interest" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>
                             <asp:BoundField DataField="Charge" HeaderText="Bank Charge" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>
                         
                                <asp:BoundField DataField="NetInterest" HeaderText="Net Interest" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>                             
                                     <asp:BoundField DataField="WithdrawDate" HeaderText="Withdraw Date" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>
                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkAlter" runat="server" CommandName="Alter" CommandArgument="<%#((GridViewRow)Container).RowIndex%>" Text="Edit" Font-Bold="true" ForeColor="Green" ></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete" >
                                <ItemTemplate>
                                     <asp:LinkButton ID="lnkDelete" runat="server" CommandName="deleterow" CommandArgument="<%#((GridViewRow)Container).RowIndex%>" Text="Delete" Font-Bold="true" ForeColor="Red" OnClientClick="return confirm('Are you sure to delete?');" ></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
         </ContentTemplate>
                           </asp:UpdatePanel>
                </div>
				
            </div>
        </div>
    </div>
     <script type="text/javascript">

         //$('#btnNew').click(function () {
         //    clear();
         //});
         $('#imgLoading').hide();
         function validateInputs() {
             if (validateText('txtWithdrawDate', 1, 60, 'Enter Withdraw Date') == false) return false;
             if (validateText('txtInterestAmount', 1, 60, 'Enter Interest Amount') == false) return false;
             if (validateText('txtBankCharge', 1, 60, 'Enter Bank Chagre') == false) return false;
                    
             return true;
         }
         function isNumberKey(evt) {
             var charCode = (evt.which) ? evt.which : evt.keyCode;
             if (charCode != 46 && charCode > 31
               && (charCode < 48 || charCode > 57))
                 return false;

             return true;
         }
        

         function deleteSuccess() {
             showMessage('Deleted successfully', 'success');
             $('#btnSave').val('Save');
             $('#hdnbtnStage').val("");
             $('#hdnUpdate').val("");
             clear();
         }
         function UpdateSuccess() {
             showMessage('Updated successfully', 'success');
             $('#btnSave').val('Save');
             $('#hdnbtnStage').val("");
             $('#hdnUpdate').val("");
             clear();
         }
         function SaveSuccess() {
             showMessage('Save successfully', 'success');
             $('#btnSave').val('Save');
             $('#hdnbtnStage').val("");
             $('#hdnUpdate').val("");
             clear();
         }
         function processing() {
             if ($('#txtDate').val().trim().length == 0) {
                 showMessage("warning->Please Select From Date ");
                 $('#txtDate').focus();
                 return false;
             }
             if ($('#txtToDate').val().trim().length == 0) {
                 showMessage("warning->Please Select To Date ");
                 $('#txtToDate').focus();
                 return false;
             }
             $('#imgLoading').show();
             return true;
         }
         function ProcessSuccess() {
             showMessage('Distributed successfully', 'success');
             $('#imgLoading').hide();
         }
       
    </script>
</asp:Content>
