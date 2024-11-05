<%@ Page Title="Advance Entry Panel" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="advance_entry_final.aspx.cs" Inherits="SigmaERP.payroll.advance.advance_entry_final" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-control {
    width: 100%;
}
        .conMsg {
            color: red;
            
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
            <div class="col-md-12">
                <div class="ds_nagevation_bar">                   
                           <ul>
                                <li><a href="/default.aspx">Dasboard</a></li>
                                <li><a class="seperator" href="#">/</a></li>
                                <li><a href="/payroll_default.aspx">Payroll</a></li>
                                <li><a class="seperator" href="#">/</a></li>
                                <li><a href="/payroll/advance/advance_index.aspx">Advance</a></li>
                                <li><a class="seperator" href="#">/</a></li>
                                <li><a href="/payroll/advance/advance_entry.aspx">Current Advance List</a></li>
                                <li><a class="seperator" href="#">/</a></li>
                                <li><a href="#" class="ds_negevation_inactive Pactive">Advance Entry Panel</a></li>
                           </ul>                 
                </div>
             </div>
        </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="uplMessage" runat="server">
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>

    <div class="main_box Mbox">
    	<div class="main_box_header PBoxheader">
            <h2>Advance Entry Panel</h2>
        </div>
    	<div class="main_box_body Pbody">
        	<div class="main_box_content">

                <asp:UpdatePanel ClientIDMode="Static" ID="up1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true" >
                    <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />  
                        <asp:AsyncPostBackTrigger ControlID="ddlEmpCardNo" />                     
                        <asp:AsyncPostBackTrigger ControlID="gvLoans" />                     
                        <asp:AsyncPostBackTrigger ControlID="btnAdd" />                     
                        <asp:PostBackTrigger ControlID="btnRefundWaive" />                     
                    </Triggers>
                    <ContentTemplate>
                        <div style="width:40%; margin:0 auto;"> 

                            <p>Company<span class="requerd1">*</span></p>
                           
                                   
                                  
                                        <asp:DropDownList ID="ddlCompanyList" runat="server" AutoPostBack="True" ClientIDMode="Static" CssClass="form-control select_width" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged" >
                                        </asp:DropDownList>
                                  <p>Employee<span class="requerd1">*</span></p>  

                                        <asp:DropDownList ID="ddlEmpCardNo" runat="server" AutoPostBack="True" ClientIDMode="Static" CssClass="form-control select_width" OnSelectedIndexChanged="ddlEmpCardNo_SelectedIndexChanged">
                                        </asp:DropDownList>
                                  

                           
                             <p>Advance<span class="requerd1">*</span></p>
                             <asp:Panel runat="server" ClientIDMode="Static" ID="pnlForEdit"> 
                                  <div class="row">

                                      <div class="col-lg-6">
                                       
                                                <asp:TextBox ID="txtEntryDate" runat="server" autocomplete="off" ClientIDMode="Static" CssClass="form-control" placeholder="Taken Date"></asp:TextBox>
                                          <asp:CalendarExtender ID="txtEntryDate_CalendarExtender" runat="server" Format="dd-MM-yyyy" TargetControlID="txtEntryDate">
                                                </asp:CalendarExtender>
                                    </div>
                                

                                      <div class="col-lg-6">
                                                      <asp:TextBox ID="txtAdvanceAmount" runat="server" autocomplete="off" ClientIDMode="Static" CssClass="form-control" placeholder="Amount"></asp:TextBox>
                                                </div>
                                  </div>
                                    

                           
                                    

                                                
                                            
                                              
                                          
                                                <asp:TextBox ID="txtRemarks" runat="server" autocomplete="off" ClientIDMode="Static" CssClass="form-control text_box_width_loan" placeholder="Remarks"></asp:TextBox>
                                           
                                                <asp:Button  style="margin:10px 0" ID="btnAdd" runat="server" ClientIDMode="Static" CssClass="Pbutton" OnClick="btnAdd_Click" OnClientClick="return ValidationForAddLoan();" Text="Add" />
                            </asp:Panel>
                                            
                                            <asp:GridView style="margin:10px 0"  ID="gvLoans" runat="server" AutoGenerateColumns="false" ClientIDMode="Static" CssClass="loan_table" DataKeyNames="LoanDetailsID" HeaderStyle-BackColor="#ffa500" HeaderStyle-ForeColor="White" HeaderStyle-Height="22px" OnRowCommand="gvLoans_RowCommand" Width="98%">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Taken Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblTakenDate" runat="server" Text='<%# Eval("TakenDate") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Amount">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("Amount") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Remarks">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblRemarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Edit">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkEdit" runat="server" ClientIDMode="Static" CommandArgument="<%# Container.DataItemIndex %>" CommandName="alter" Text="Alter"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Remove">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkRemove" runat="server" ClientIDMode="Static" CommandArgument='<%# Eval("LoanDetailsID") %>' CommandName="removerow" OnClientClick="return confirm('Are you sure, you want to remove the record?')" Text="Remove"></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                                
                                
                                Total Amount<span class="requerd1">*</span>
                                        <asp:TextBox ID="txtTotalAmount" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" Enabled="false" >0</asp:TextBox>
                                       
                                    
                                <div runat="server" id="trPaid" visible="false">
                                    Paid
                                        <asp:TextBox ID="txtPaid" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" Enabled="false" >0</asp:TextBox>                                       
                                  
                                </div>
                                <div runat="server"  id="trDue" visible="false">
                                    Due
                                    
                                        <asp:TextBox ID="txtDue" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" Enabled="false" >0</asp:TextBox>
                                 </div>
                                    Deduction Start From<span class="requerd1">*</span>
                                        <asp:TextBox ID="txtStartMonth" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" autocomplete="off" ></asp:TextBox>
                                        <asp:CalendarExtender ID="txtStartMonth_CalendarExtender" Format="MM-yyyy" runat="server" TargetControlID="txtStartMonth">
                                        </asp:CalendarExtender>
                                   Installment Amount<span class="requerd1">*</span>
                                        <asp:TextBox ID="txtInstallmentAmount" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan"></asp:TextBox>  
                            <asp:Panel runat="server" ClientIDMode="Static" ID="pnlRefundWaive">
                            <div class="alert alert-danger">
                              <p runat="server" id="pRefundWaiveMsg" class="alert-heading" style="font-size:18px;font-weight:bold">Do you want to refund the due?</p>
                              <p>Please, fill up bellow and submit.</p>             
                            </div>                          
                              <span runat="server" id="lblRefundWaiveDate">Refund Date</span><span class="requerd1">*</span>
                                        <asp:TextBox ID="txtRefundWaiveDate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" autocomplete="off" ></asp:TextBox>
                                        <asp:CalendarExtender ID="CalendarExtender1" Format="dd-MM-yyyy" runat="server" TargetControlID="txtRefundWaiveDate">
                                        </asp:CalendarExtender>
                                <div runat="server" id="divRefundAmount">
                                Refund Amount<span class="requerd1">*</span>
                                        <asp:TextBox ID="txtRefund" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan"></asp:TextBox>                          
                                    </div>
                                Comment<span class="requerd1">*</span>
                                        <asp:TextBox ID="txtComment" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" TextMode="MultiLine"></asp:TextBox> 
                                Attach Document (if any)
                                <asp:FileUpload ID="FileUploadDoc" runat="server" Width="211px" /> 
                                </asp:Panel>
                        </div>
                        
                         <div style="width:40%; margin:10px auto;">
                 <asp:Button ID="btnSave" runat="server" Text="Submit" ClientIDMode="Static" CssClass="Pbutton" OnClick="btnSave_Click" />    
                             <asp:Button ID="btnRefundWaive" runat="server" Text="Submit" ClientIDMode="Static" CssClass="Pbutton" OnClick="btnRefundWaive_Click"/> 
                </div>
                </ContentTemplate>
            </asp:UpdatePanel>

              <%--  <asp:UpdatePanel runat="server" ClientIDMode="Static" ID="up2" >
                    <Triggers>

                    </Triggers>
                    <ContentTemplate>--%>
                         
                   <%-- </ContentTemplate>
                </asp:UpdatePanel>--%>
                
            </div>         
        </div>
    </div>

    <script type="text/javascript">
            $(document).ready(function () {

                $("#ddlEmpCardNo").select2();
            });

            function load() {
                $("#ddlEmpCardNo").select2();
            }
            function InputValidationBasket() {
                try {

                    if ($('#txtEntryDate').val().trim().length == 0) {
                        showMessage('Please select advance taken date', 'error');
                        $('#txtEntryDate').focus(); return false;
                    }

                    if ($('#txtAdvanceAmount').val().trim().length == 0) {
                        showMessage('Please enter advance amount', 'error');
                        $('#txtAdvanceAmount').focus(); return false;
                    }
                    if ($('#txtStartMonth').val().trim().length == 0) {
                        showMessage('Please select start month', 'error');
                        $('#txtStartMonth').focus(); return false;
                    }

                    if ($('#txtNoOfInstallment').val().trim().length == 0) {
                        showMessage('Please type number of installment', 'error');
                        $('#txtNoOfInstallment').focus(); return false;
                    }

                    return true;
                }
                catch (exception) {
                    showMessage(exception, error)
                }
            }
             function ValidationForAddLoan() {
                try {

                    if ($('#txtEntryDate').val().trim().length == 0) {
                        showMessage('Please select advance taken date', 'error');
                        $('#txtEntryDate').focus(); return false;
                    }

                    if ($('#txtAdvanceAmount').val().trim().length == 0) {
                        showMessage('Please enter advance amount', 'error');
                        $('#txtAdvanceAmount').focus(); return false;
                    }
                    


                    return true;
                }
                catch (exception) {
                    showMessage(exception, error)
                }
        }

        </script>

</asp:Content>
