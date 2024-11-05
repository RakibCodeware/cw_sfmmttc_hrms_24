<%@ Page Title="Advance" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="advance_entry.aspx.cs" Inherits="SigmaERP.payroll.advance.advance_entry" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">   
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
        .btn{
            margin:5px 0;
        }
        .btn-waive{
            background:#3b5998;
            color:#ffffff;
        }
          .btn-waive:hover{
            background:#3b5998;
            color:#ffffff;
        }
          .btn-refund{
            background:#B38933;
            color:#ffffff;
        }
          .btn-refund:hover{
            background:#B38933;
            color:#ffffff;
        }
           .btn-paid{
            background:#008000;
            color:#ffffff;
        }
           .btn-paid:hover{
            background:#008000;
            color:#ffffff;
        }

        
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
                    <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="<%= Session["__topMenuAdvance__"] %>">Advance</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Current Advance List</a></li>
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
            <h2>Current Advance List</h2>
        </div>
        <div class="main_box_body Pbody">
            <div class="main_box_content" style="overflow: hidden">  
                <div>
                    <div style="display:flex; justify-content:flex-end; align-items:center;" >
                        <a style="display:inline-block; margin:10px 0; text-align:center" runat="server" id="btnAddNew"  href="advance_entry_final.aspx" class="Pbutton">Add New</a>
                    </div>
                     

                                   <div id="divElementContainer" runat="server" class="list_main_content_box_header LBoxheader" visible="false"  style="width: 100%;">        
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" />                     
                            <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />                
                            <asp:AsyncPostBackTrigger ControlID="ddlDepartmentList" />                   
                        </Triggers>
                        <ContentTemplate>   
                           
               <div style="width:100%;">                    
                  <table width="99%" style="margin:0 0 5px 6px; border-collapse: collapse;">
                       <tr>
                            <td>Company</td>
                           <td>Depertment</td>                         
                            <td>Line / Grp</td>
                            <td>Card No</td>
                            <td>Year</td>
                            <td>From Date</td>
                            <td>To Date</td>
                            <td></td>
                        </tr>
                       <tr>
                            <td>
                                 <asp:DropDownList ID="ddlCompanyList" ClientIDMode="Static" CssClass="form-control inline_form_text_box_width" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged"  >              
                                 </asp:DropDownList>
                            </td>
                           <td>
                                <asp:DropDownList ID="ddlDepartmentList" CssClass="form-control inline_form_text_box_width" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlDivisionName_SelectedIndexChanged"></asp:DropDownList>
                            </td>                        
                            <td>
                                <asp:DropDownList ID="ddlGrouping" CssClass="form-control inline_form_text_box_width" runat="server" ></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCardNo" runat="server" CssClass="form-control inline_form_text_box_width" style="width:100px !important;" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
                            </td>
                            <td>
                                <asp:DropDownList runat="server" ID="ddlChoseYear" CssClass="form-control inline_form_text_box_width" style="width:100px !important;" AutoPostBack="True" OnSelectedIndexChanged="ddlChoseYear_SelectedIndexChanged"  ></asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control inline_form_text_box_width" style="width: 100px !important;" ClientIDMode="Static" MaxLength="12" autocomplete="off"></asp:TextBox>
                               <asp:CalendarExtender ID="txtFromDate_CalendarExtender" runat="server" Format="dd-MM-yyyy" TargetControlID="txtFromDate">
                               </asp:CalendarExtender>
                            </td>
                            <td>
                                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control inline_form_text_box_width" style="width: 100px !important;" ClientIDMode="Static" MaxLength="12" autocomplete="off"></asp:TextBox>
                                <asp:CalendarExtender ID="txtToDate_CalendarExtender" runat="server"  Format="dd-MM-yyyy" TargetControlID="txtToDate">
                                </asp:CalendarExtender>
                            </td>
                            <td><asp:Button runat="server" ID="btnSearch" CssClass="Lbutton" Text="Search" Width="75px" style="border:1px solid;" Height="34px" OnClick="btnSearch_Click"  /></td>
                        </tr>
                   </table>
                        
                     </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                      </div>
                   

                   <div class="loding_img">
                       <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <span style=" font-family:'Times New Roman'; font-size:20px; color:green;font-weight:bold;float:left"><p>&nbsp;</p> </span> <br />
                                        <img cursor:pointer; float:left" src="/images/loader-2.gif"/>  
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                  </div>
                </div>

                    <div>

                        <asp:UpdatePanel runat="server" ID="up2" UpdateMode="Conditional">
                            <Triggers>
                            </Triggers>
                            <ContentTemplate>
                                <asp:GridView ID="gvAdvanceInfo" runat="server" DataKeyNames="LoanID" Width="100%" HeaderStyle-BackColor="#ffa500" AutoGenerateColumns="false" HeaderStyle-Height="30px" HeaderStyle-Font-Bold="false" HeaderStyle-ForeColor="White" AllowPaging="True" OnPageIndexChanging="gvAdvanceInfo_PageIndexChanging" PageSize="25" OnRowDataBound="gvAdvanceInfo_RowDataBound" OnRowCommand="gvAdvanceInfo_RowCommand" >
                                    <PagerStyle CssClass="gridview" Height="20px" />
                                    <Columns>                                      
                                        <asp:TemplateField ItemStyle-CssClass="center" HeaderText="SL">
                                            <ItemTemplate>
                                                <label><%# Container.DataItemIndex + 1 %></label>                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>                                  
                                        <asp:BoundField DataField="EmpCardNo" HeaderText="Card No" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />
                                        <asp:BoundField DataField="EmpName" HeaderText="Name" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />
                                        <asp:BoundField DataField="LoanAmount" HeaderText="Loan" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />                                        
                                        <asp:BoundField DataField="InstallmentAmount" HeaderText="Ins.Amount" Visible="true" ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />
                                        <asp:BoundField DataField="DeductFrom" HeaderText="Deduct Start From"  ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />   
                                        <asp:BoundField DataField="PaidAmount" HeaderText="Paid"  ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />    
                                        <asp:BoundField DataField="DueAmount" HeaderText="Due"  ItemStyle-HorizontalAlign="center" ItemStyle-Height="26px" />      
                                      

                                         <asp:TemplateField>
                                  <HeaderTemplate>
                                      Edit
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Button runat="server" ID="btnAlter" Text="Edit" Font-Bold="true" CommandName="alter" CssClass="btn btn-primary" CommandArgument='<%# Eval("EmpId") %>' />
                                  </ItemTemplate>
                                   <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                                           <asp:TemplateField>
                                  <HeaderTemplate>
                                      Refund
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Button runat="server" ID="btnRefund" Text="Refund" Font-Bold="true" CommandName="refund" CssClass="btn btn-refund text-white" CommandArgument='<%# Eval("EmpId") %>' OnClientClick="return confirm('Are you sure you want to refund it?');" />
                                  </ItemTemplate>
                                   <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>

                                        <asp:TemplateField>
                                  <HeaderTemplate>
                                      Waive
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Button runat="server" ID="btnWaive" Text="Waive" Font-Bold="true" CommandName="waive" CssClass="btn btn-waive text-white" CommandArgument='<%# Eval("EmpId") %>' OnClientClick="return confirm('Are you sure you want to waive it?');" />
                                  </ItemTemplate>
                                   <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                                        <asp:TemplateField>
                                  <HeaderTemplate>
                                      Paid
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Button runat="server" ID="btnPaid" Text="Paid" Font-Bold="true" CommandName="paid" CssClass="btn btn-paid text-white" CommandArgument='<%# Eval("EmpId") %>' OnClientClick="return confirm('Are you sure you want to paid it?');" />
                                  </ItemTemplate>
                                   <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                                         <asp:TemplateField>
                                  <HeaderTemplate>
                                      Delete
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Button runat="server" ID="btnRemove" Text="Delete" Font-Bold="true" CommandName="remove" CssClass="btn btn-danger" CommandArgument='<%#((GridViewRow)Container).RowIndex %>' />
                                  </ItemTemplate>
                                   <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                                    </Columns>
                                    <SelectedRowStyle BackColor="Yellow" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>             
                </div>

            </div>
        </div>
        
        <script type="text/javascript">            
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
        </script>
</asp:Content>
