<%@ Page Title="FDR Withdraw" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="pf_interestentry.aspx.cs" Inherits="SigmaERP.pf.pf_interestentry" %>
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
                     <li> <a href="#" class="ds_negevation_inactive Pactive">Investment Withdraw</a></li>
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
            <h2>Investment Withdraw Panel</h2>
        </div>
    	<div class="main_box_body Pbody">
        	<div class="main_box_content">
                  <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSave" />  
             <asp:AsyncPostBackTrigger ControlID="btnNew" />  
            <asp:AsyncPostBackTrigger ControlID="ddlType" />          
        </Triggers>
        <ContentTemplate>
                <div class="input_division_info">
                   <div class="row">
                        <div class="col-sm-12">
                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Company Name <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:DropDownList ID="ddlCompanyName" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" >
                                                                        
                                      </asp:DropDownList>
                                </div>
                              </div>
                                <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Investment Type <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:DropDownList ID="ddlType" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                               
                                      </asp:DropDownList>
                                </div>
                              </div>
                            <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Investment No <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:DropDownList ID="ddlFDRList" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" AutoPostBack="true" OnSelectedIndexChanged="ddlFDRList_SelectedIndexChanged" >
                                                                        
                                      </asp:DropDownList>
                                                                   
                                </div>
                              </div>
                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Amount <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtFDRAmount" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width"  Enabled="false"  ></asp:TextBox>
                                                                   
                                </div>
                              </div>
                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Profit Rate (%) <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtInterestRate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width"  Enabled="false"></asp:TextBox>                                
                                </div>
                              </div>                                
                               <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Opening Date <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtFromDate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" Enabled="false"  ></asp:TextBox>  
                                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="dd-MM-yyyy" TargetControlID="txtFromDate">
                                    </asp:CalendarExtender>                              
                                </div>
                              </div> 
                               <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Matured Date <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtToDate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" Enabled="false" ></asp:TextBox>  
                                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MM-yyyy" TargetControlID="txtToDate">
                                    </asp:CalendarExtender>                              
                                </div>
                              </div> 
                               <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Period</div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtPeriod" Enabled="false" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width"></asp:TextBox>                                
                                </div>
                              </div> 
                            <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Withdraw Date <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtWithdrawDate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" ></asp:TextBox>  
                                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd-MM-yyyy" TargetControlID="txtWithdrawDate">
                                    </asp:CalendarExtender>                              
                                </div>
                              </div> 
                            <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Interest Amount</div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtInterestAmount" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" onkeypress="return isNumberKey(event)" AutoPostBack="true" OnTextChanged="txtTaxChargePer_TextChanged" ></asp:TextBox>                                
                                </div>
                              </div> 
                               <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Tax Charge (%)</div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtTaxChargePer" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width"  onkeypress="return isNumberKey(event)" AutoPostBack="true" OnTextChanged="txtTaxChargePer_TextChanged"></asp:TextBox>                                
                                </div>
                              </div> 
                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Tax Charge</div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtTaxCharge" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" Enabled="false" ></asp:TextBox>                                
                                </div>
                              </div> 
                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Bank Charge</div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtBankCharge" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width"  onkeypress="return isNumberKey(event)" AutoPostBack="true" OnTextChanged="txtBankCharge_TextChanged" ></asp:TextBox>                                
                                </div>
                              </div> 
                             <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Net Interest Amount</div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtNetInterestAmount" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" Enabled="false" ></asp:TextBox>                                
                                </div>
                              </div>
                        </div>
                    </div>                    
                </div>      
                <div class="button_area Rbutton_area">
                    <a href="#" onclick="window.history.back()" class="Pbutton">Back</a>
                    <asp:Button ID="btnNew" ClientIDMode="Static" CssClass="Pbutton"  runat="server" Text="New" />
                    <asp:Button ID="btnSave" OnClientClick="return validateInputs();"  ClientIDMode="Static" class="Pbutton" runat="server" Text="Save"  OnClick="btnSave_Click" />
                    <asp:Button ID="btnClose" ClientIDMode="Static" CssClass="Pbutton" PostBackUrl="~/hrd_default.aspx"  runat="server" Text="Close" />
                </div>
            </ContentTemplate>
                      </asp:UpdatePanel>

                  <div class="show_division_info">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">    
        <ContentTemplate>
                     <%--<div id="ShiftConfig" class="datatables_wrapper" runat="server" style="width:100%; height:auto; max-height:500px;overflow:auto;overflow-x:hidden;"></div>--%>
                    <asp:GridView ID="gvPFSettings" runat="server"  Width="100%" AutoGenerateColumns="False" DataKeyNames="CompanyId,FdrID"   AllowPaging="True" OnRowCommand="gvPFSettings_RowCommand" OnRowDataBound="gvPFSettings_RowDataBound" >
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
                             <asp:BoundField DataField="TaxChargePer" HeaderText="Tax Charge(%)" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>
                             <asp:BoundField DataField="TaxCharge" HeaderText="Tax Charge" ItemStyle-Height="28px">
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
    </script>
</asp:Content>