<%@ Page Title="Loan" Language="C#" MasterPageFile="~/payroll_nested.Master" AutoEventWireup="true" CodeBehind="loan.aspx.cs" Inherits="SigmaERP.payroll.loan" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <%--<script src="../scripts/jquery-1.8.2.js"></script>--%>
    <script type="text/javascript">

        var oldgridcolor;
        function SetMouseOver(element) {
            oldgridcolor = element.style.backgroundColor;
            element.style.backgroundColor = '#ffeb95';
            element.style.cursor = 'pointer';
            // element.style.textDecoration = 'underline';
        }
        function SetMouseOut(element) {
            element.style.backgroundColor = oldgridcolor;
            // element.style.textDecoration = 'none';
        }
    </script>
    <style>
        #ContentPlaceHolder1_ContentPlaceHolder1_gvAdvanceInfo th, td {
            text-align: center;
        }

            #ContentPlaceHolder1_ContentPlaceHolder1_gvAdvanceInfo th:nth-child(3), td:nth-child(3) {
                text-align: left;
                padding-left: 3px;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar">
                <ul>
                    <li><a href="/default.aspx">Dasboard</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="/payroll_default.aspx">Payroll</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="/payroll/advance_index.aspx">Advance</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Loan Entry Panel</a></li>
                </ul>
            </div>
        </div>
    </div>
    <asp:UpdatePanel ID="uplMessage" runat="server">
        <ContentTemplate>
            <p class="message" id="lblMessage" clientidmode="Static" runat="server"></p>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="main_box Mbox">
        <div class="main_box_header PBoxheader">
            <h2>Loan Info Reports</h2>
        </div>
        <div class="main_box_body Pbody">
            <div class="main_box_content" style="overflow: hidden">

                <div>
                    <div style="float:left; width:50%">
                         <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnAdd" />                                         
                            <asp:AsyncPostBackTrigger ControlID="ddlEmpCardNo" />
                            <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />
                            <asp:AsyncPostBackTrigger ControlID="gvLoans" />
                            <%--<asp:AsyncPostBackTrigger ControlID="btnComplain" />--%>
                        </Triggers>
                        <ContentTemplate>
                            <table style="width: 100%;">
                                <tr>
                                    <td>Company<span class="requerd1">*</span></td>
                                    <td>
                                        <asp:DropDownList ID="ddlCompanyList" runat="server" AutoPostBack="True" ClientIDMode="Static" CssClass="form-control select_width" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Employee<span class="requerd1">*</span></td>
                                    <td>
                                        <asp:DropDownList ID="ddlEmpCardNo" runat="server" AutoPostBack="True" ClientIDMode="Static" CssClass="form-control select_width" OnSelectedIndexChanged="ddlEmpCardNo_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <panel>
                                         <table  style="width:98%;">                                       
                                        <tr>
                                            <td>Loan<span class="requerd1">*</span></td>
                                            <td>
                                                <asp:TextBox ID="txtEntryDate" placeholder="Taken Date" runat="server" ClientIDMode="Static" autocomplete="off" CssClass="form-control" ></asp:TextBox>
                                                <asp:CalendarExtender ID="txtEntryDate_CalendarExtender" Format="dd-MM-yyyy" runat="server" TargetControlID="txtEntryDate">
                                                </asp:CalendarExtender>
                                            </td>             
                                        
                                            <td>
                                                <asp:TextBox ID="txtAdvanceAmount" placeholder="Amount" autocomplete="off" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan"></asp:TextBox>

                                            </td>
                                            <td></td>                           
                                                 
                                          </tr>
                                           <tr>
                                               <td></td>
                                               <td colspan="2">
                                                <asp:TextBox ID="txtRemarks" placeholder="Remarks" autocomplete="off" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan"></asp:TextBox>

                                            </td>
                                             
                                               <td>
                                                      <asp:Button ID="btnAdd" runat="server" Text="Add" ClientIDMode="Static" CssClass="Pbutton"  OnClientClick="return ValidationForAddLoan();" OnClick="btnAdd_Click" />
                                                 </td>

                                           </tr>
                                          <tr>
                                            <asp:GridView ID="gvLoans" DataKeyNames="LoanDetailsID" HeaderStyle-BackColor="#ffa500" runat="server" AutoGenerateColumns="false" Width="98%" CssClass="loan_table" HeaderStyle-ForeColor="White" HeaderStyle-Height="22px" OnRowCommand="gvLoans_RowCommand" ClientIDMode="Static">
                                                <Columns>
                                                  <asp:TemplateField HeaderText="Taken Date">
                                                      <ItemTemplate>
                                                          <asp:Label runat="server" ID="lblTakenDate" Text='<%# Eval("TakenDate") %>'></asp:Label>
                                                      </ItemTemplate>
                                                  </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Amount">
                                                      <ItemTemplate>
                                                          <asp:Label runat="server" ID="lblAmount" Text='<%# Eval("Amount") %>'></asp:Label>
                                                      </ItemTemplate>
                                                  </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Remarks">
                                                      <ItemTemplate>
                                                          <asp:Label runat="server" ID="lblRemarks" Text='<%# Eval("Remarks") %>'></asp:Label>
                                                      </ItemTemplate>
                                                  </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Edit">
                                                      <ItemTemplate>
                                                        <asp:LinkButton ID="lnkEdit" runat="server" Text="Alter" CommandName="alter" CommandArgument='<%# Container.DataItemIndex %>' ClientIDMode="Static"></asp:LinkButton>
                                                      </ItemTemplate>
                                                  </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Remove">
                                                      <ItemTemplate>
                                                           <asp:LinkButton ID="lnkRemove" runat="server" Text="Remove" CommandName="removerow" ClientIDMode="Static" CommandArgument='<%# Eval("LoanDetailsID") %>'
                                                OnClientClick="return confirm('Are you sure, you want to remove the record?')"
                                                ></asp:LinkButton>
                                                         
                                                      </ItemTemplate>
                                                  </asp:TemplateField>
                                                    
                                                </Columns>
                                            </asp:GridView>
                                        </tr>
                                    </table> 
                                    </panel>
                                </tr>   
                                
                                <tr>
                                    <td>Total Amount<span class="requerd1">*</span></td>
                                    <td>
                                        <asp:TextBox ID="txtTotalAmount" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" Enabled="false" >0</asp:TextBox>
                                       
                                    </td>
                                </tr>
                                <tr>
                                    <td>Deduction Start From<span class="requerd1">*</span></td>
                                    <td>
                                        <asp:TextBox ID="txtStartMonth" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan" autocomplete="off" ></asp:TextBox>
                                        <asp:CalendarExtender ID="txtStartMonth_CalendarExtender" Format="MM-yyyy" runat="server" TargetControlID="txtStartMonth">
                                        </asp:CalendarExtender>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Installment Amount<span class="requerd1">*</span></td>
                                    <td>
                                        <asp:TextBox ID="txtInstallmentAmount" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width_loan"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                            <div style="text-align:right">
                                <asp:Button ID="btnSave" runat="server" Text="Submit" ClientIDMode="Static" CssClass="Pbutton"  OnClick="btnSave_Click"  />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    </div>
                   
                    <div>
                        Individual Details
                    </div>

                    <div>

                        <asp:UpdatePanel runat="server" ID="up2" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:GridView ID="gvAdvanceInfo" runat="server" DataKeyNames="LoanID" Width="100%" HeaderStyle-BackColor="#ffa500" AutoGenerateColumns="false" HeaderStyle-Height="30px" HeaderStyle-Font-Bold="false" HeaderStyle-ForeColor="White" AllowPaging="True" OnPageIndexChanging="gvAdvanceInfo_PageIndexChanging" PageSize="25" OnRowCommand="gvAdvanceInfo_RowCommand" OnRowDataBound="gvAdvanceInfo_RowDataBound">
                                    <PagerStyle CssClass="gridview" Height="20px" />
                                    <Columns>
                                        <%-- <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Bind("AdvanceId") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>--%>
                                        <asp:TemplateField ItemStyle-CssClass="center" HeaderText="SL">
                                            <ItemTemplate>
                                                <label><%# Container.DataItemIndex + 1 %></label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        
                                        <asp:BoundField DataField="EmpCardNo" HeaderText="Card No" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />
                                        <asp:BoundField DataField="EmpName" HeaderText="Name" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />
                                        <asp:BoundField DataField="LoanAmount" HeaderText="Loan" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />                                        
                                        <asp:BoundField DataField="InstallmentAmount" HeaderText="Ins.Amount" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />
                                        <asp:BoundField DataField="DeductFrom" HeaderText="Deduct Start From"  ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />   
                                        <asp:BoundField DataField="PaidAmount" HeaderText="Paid"  ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />    
                                        <asp:BoundField DataField="DueAmount" HeaderText="Due"  ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />                                       
                                      

                                        
                                    </Columns>
                                    <SelectedRowStyle BackColor="Yellow" />
                                </asp:GridView>


                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
               </div>
                    
                    
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
                        showMessage('Please select entry date', 'error');
                        $('#txtEntryDate').focus(); return false;
                    }

                    if ($('#txtAdvanceAmount').val().trim().length == 0) {
                        showMessage('Please type advance amount', 'error');
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
                        showMessage('Please select entry date', 'error');
                        $('#txtEntryDate').focus(); return false;
                    }

                    if ($('#txtAdvanceAmount').val().trim().length == 0) {
                        showMessage('Please type advance amount', 'error');
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
