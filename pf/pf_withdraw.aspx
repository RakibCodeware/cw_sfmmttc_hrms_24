<%@ Page Title="PF Withdraw Panel" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="pf_withdraw.aspx.cs" Inherits="SigmaERP.pf.pf_withdraw" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
        #ContentPlaceHolder1_ContentPlaceHolder1_gvMonthlyPFList th {
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
                     <li> <a href="#" class="ds_negevation_inactive Pactive">PF Withdraw Panel</a></li>
                </ul>
            </div>
        </div>
       </div>  
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<asp:UpdatePanel ID="uplMessage" runat="server" >
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>
    
    <div class="main_box Mbox">
    	<div class="main_box_header PBoxheader">
            <h2> PF Withdraw Panel</h2>
        </div>
    	<div class="main_box_body Pbody">
        	<div class="main_box_content">
                  <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSave" />               
            <asp:AsyncPostBackTrigger ControlID="ddlCompanyName" />  
            <asp:AsyncPostBackTrigger ControlID="ddlEmployeeList" />        
        </Triggers>
        <ContentTemplate>
                <div class="input_division_info">
                   <div class="row">
                        <div class="col-sm-12">

                              

                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Company Name <span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                     <asp:DropDownList ID="ddlCompanyName" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" AutoPostBack="true"  OnSelectedIndexChanged="ddlCompanyName_SelectedIndexChanged" >
                                                                        
                                      </asp:DropDownList>
                                </div>
                              </div>                          
                            <div class="row tbl-controlPanel">
                                <div class="col-sm-4">PF Member List<span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                   <asp:DropDownList ID="ddlEmployeeList" runat="server" ClientIDMode="Static"  CssClass="form-control select_width"  >                                                                        
                                      </asp:DropDownList>                                                                   
                                </div>
                              </div>
                               <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Withdraw/Closing Date<span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtWithdrawDate" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" autocomplete="off" ></asp:TextBox>  
                                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="dd-MM-yyyy" TargetControlID="txtWithdrawDate">
                                    </asp:CalendarExtender>                              
                                </div>
                              </div></div>
                    </div>                    
                </div>      
                <div class="button_area Rbutton_area">                                        
                    <asp:Button ID="btnSave" OnClientClick="return validateInputs();"  ClientIDMode="Static" class="Pbutton" runat="server" Text="Withdraw" OnClick="btnSave_Click" />
                    <asp:Button ID="btnClose" ClientIDMode="Static" CssClass="Pbutton" PostBackUrl="~/hrd_default.aspx"  runat="server" Text="Close" />
                </div>
            <br />
            <div>
            <center>
                <asp:RadioButtonList runat="server" ID="rblPayableType" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="rblPayableType_SelectedIndexChanged">
                    <asp:ListItem Value="0" Selected="True">All</asp:ListItem>
                    <asp:ListItem Value="1">Emp. Contribution</asp:ListItem>
                    <asp:ListItem Value="2">Emp.+Empr. Contribution</asp:ListItem>
                      <asp:ListItem Value="3">Emp.+Empr. Contribution + Profit</asp:ListItem>
                </asp:RadioButtonList>
               </center>     
            </div>
            </ContentTemplate>
                      </asp:UpdatePanel>

                  <div class="show_division_info">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">  
                           
        <ContentTemplate>                    
                    <asp:GridView ID="gvPFWithdrawList" runat="server"  Width="100%" AutoGenerateColumns="False" DataKeyNames="EmpId"   AllowPaging="True"  PageSize="25"  OnRowCommand="gvPFWithdrawList_RowCommand" OnRowDataBound="gvPFWithdrawList_RowDataBound" >
<HeaderStyle BackColor="#FFA500" Font-Bold="True" Font-Size="14px" ForeColor="White" Height="28px"></HeaderStyle>
                        <PagerStyle CssClass="gridview Sgridview" Height="40px" />
                       <RowStyle HorizontalAlign="Center" />
                         <Columns> 
                             <asp:BoundField DataField="EmpCardNo" HeaderText="Card No"  ItemStyle-Height="28px"   >
<ItemStyle Height="28px" ></ItemStyle>
                             </asp:BoundField>
                            <asp:BoundField DataField="EmpName" HeaderText="Name" ItemStyle-Height="28px">
<ItemStyle Height="28px" ></ItemStyle>
                             </asp:BoundField>           
                             <asp:BoundField DataField="WithdrawDate" HeaderText="Withdraw Date" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>                   
                             <asp:BoundField DataField="EmpContribution" HeaderText="Emp. Contribution" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>
                             <asp:BoundField DataField="EmprContribution" HeaderText="Empr. Contribution"  ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>      
                             <asp:BoundField DataField="Profit" HeaderText="Profit"  ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>                  
                            <asp:TemplateField HeaderText="Preview"  >                               
                                <ItemTemplate>
                                     <asp:LinkButton ID="btnPreview" runat="server" CommandName="preview" CommandArgument="<%#((GridViewRow)Container).RowIndex%>" Text="Preview" Font-Bold="true" ForeColor="Blue"></asp:LinkButton>
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
         $(document).ready(function () {
            load();           
         });

         function load() {
             $("#ddlEmployeeList").select2();
         }
         function isNumberKey(evt) {
             var charCode = (evt.which) ? evt.which : evt.keyCode;
             if (charCode != 46 && charCode > 31
               && (charCode < 48 || charCode > 57))
                 return false;

             return true;
         }        
         function goToNewTabandWindow(url) {
             load();
            window.open(url);

        }
         


       
    </script>
</asp:Content>
