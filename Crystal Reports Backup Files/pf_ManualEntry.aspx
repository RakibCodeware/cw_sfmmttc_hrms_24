<%@ Page Title="Manulay PF Entry Panel" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="pf_ManualEntry.aspx.cs" Inherits="SigmaERP.pf.pf_ManualEntry" %>
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
        #ContentPlaceHolder1_ContentPlaceHolder1_gvMonthlyPFList th,td {
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
                     <li> <a href="#" class="ds_negevation_inactive Pactive">Manulay PF Entry Panel</a></li>
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
            <h2>Manulay PF Entry Panel</h2>
        </div>
    	<div class="main_box_body Pbody">
        	<div class="main_box_content">
                  <asp:UpdatePanel ID="UpdatePanel4" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSave" />  
             <asp:AsyncPostBackTrigger ControlID="btnNew" />  
            <asp:AsyncPostBackTrigger ControlID="ddlCompanyName" />  
            <asp:AsyncPostBackTrigger ControlID="ddlEmployeeList" />        
        </Triggers>
        <ContentTemplate>
                <div class="input_division_info">
                   <div class="row">
                        <div class="col-sm-12">

                               <div class="row tbl-controlPanel">
                                <div class="col-sm-4">PF Month<span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtMonth" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" ></asp:TextBox>  
                                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="MM-yyyy" TargetControlID="txtMonth">
                                    </asp:CalendarExtender>                              
                                </div>
                              </div> 

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
                                   <asp:DropDownList ID="ddlEmployeeList" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployeeList_SelectedIndexChanged">                                                                        
                                      </asp:DropDownList>                                                                   
                                </div>
                              </div>
                              <div class="row tbl-controlPanel">
                                <div class="col-sm-4">Employee Contribution<span class="requerd1">*</span></p></div>
                                <div class="col-sm-8">
                                    <asp:TextBox ID="txtEmpContribution" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                   
                                </div>
                              </div>
                                                        
                            
                               
                         

                        </div>
                    </div>                    
                </div>      
                <div class="button_area Rbutton_area">
                    <a href="#" onclick="window.history.back()" class="Pbutton">Back</a>
                    <asp:Button ID="btnNew" ClientIDMode="Static" CssClass="Pbutton"  runat="server" Text="New" />
                    <asp:Button ID="btnSave" OnClientClick="return validateInputs();"  ClientIDMode="Static" class="Pbutton" runat="server" Text="Save" OnClick="btnSave_Click"/>
                    <asp:Button ID="btnClose" ClientIDMode="Static" CssClass="Pbutton" PostBackUrl="~/hrd_default.aspx"  runat="server" Text="Close" />
                </div>
            </ContentTemplate>
                      </asp:UpdatePanel>

                  <div class="show_division_info">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">  
                           
        <ContentTemplate>                    
                    <asp:GridView ID="gvMonthlyPFList" runat="server"  Width="100%" AutoGenerateColumns="False" DataKeyNames="CompanyId,EmpID,SL"   AllowPaging="True"  PageSize="25"  OnPageIndexChanging="gvMonthlyPFList_PageIndexChanging" OnRowCommand="gvMonthlyPFList_RowCommand" OnRowDataBound="gvMonthlyPFList_RowDataBound"  >
<HeaderStyle BackColor="#FFA500" Font-Bold="True" Font-Size="14px" ForeColor="White" Height="28px"></HeaderStyle>
                        <PagerStyle CssClass="gridview Sgridview" Height="40px" />
                       <RowStyle HorizontalAlign="Center" />
                         <Columns> 
                             <asp:BoundField DataField="EmpCardNo" HeaderText="Card No"  ItemStyle-Height="28px" ItemStyle-HorizontalAlign="center"   >
<ItemStyle Height="28px" ></ItemStyle>
                             </asp:BoundField>
                            <asp:BoundField DataField="EmpName" HeaderText="FDR Amount" ItemStyle-Height="28px" ItemStyle-HorizontalAlign="center" >
<ItemStyle Height="28px" ></ItemStyle>
                             </asp:BoundField>           
                             <asp:BoundField DataField="Month" HeaderText="PF Month" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>                   
                             <asp:BoundField DataField="EmpContribution" HeaderText="Emp. Contribution" ItemStyle-Height="28px">
                                 <ItemStyle Height="28px"></ItemStyle>
                             </asp:BoundField>
                             <asp:BoundField DataField="EmprContribution" HeaderText="Empr. Contribution"  ItemStyle-Height="28px">
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
         $(document).ready(function () {
             $("#ddlEmployeeList").select2();
           
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

         


       
    </script>
</asp:Content>

