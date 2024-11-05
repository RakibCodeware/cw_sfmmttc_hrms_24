<%@ Page Title="Weekend Setup" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="weekend-set-emp-wise.aspx.cs" Inherits="SigmaERP.attendance.weekend_set_emp_wise" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../style/list_checkbox_style.css" rel="stylesheet" />
    <script src="../scripts/jquery-1.8.2.js"></script>
    <script type="text/javascript">
        var oldgridcolor;
        function SetMouseOver(element) {
            oldgridcolor = element.style.backgroundColor;
            element.style.backgroundColor = '#ffeb95';
            element.style.cursor = 'pointer';
            element.style.textDecoration = 'underline';
        }
        function SetMouseOut(element) {
            element.style.backgroundColor = oldgridcolor;
            element.style.textDecoration = 'none';

        }
</script>

    <style type="text/css">

        .txt{
            margin:3px;
            text-align:center;
            width:58px;
        }
        .btnn {
             margin:3px;
            text-align:center;
        }
        table tr td, table tr th{
            padding: 4px;
        }
        .id_card_left select option{
            padding:5px;
        }
        .id_card_right select option{
             padding:5px;
        }
    </style>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
         <div class="row Rrow">
        <div class="col-md-12 ds_nagevation_bar">
            <div style="margin-top: 5px">
                <ul>
                    <li><a href="/default.aspx">Dashboard</a></li>
                    <li>/</li>
                   <li><a href="<%=  Session["__topMenu__"] %>">Attendance</a></li>
                    <li>/</li>
                    <li><a href="#" class="ds_negevation_inactive Mactive">Weekend Setup</a></li>
                </ul>
            </div>
        </div>
    </div>
    
    <asp:UpdatePanel ID="uplMessage" runat="server">
        <ContentTemplate>
            <p class="message" id="lblMessage" clientidmode="Static" runat="server"></p>
        </ContentTemplate>
    </asp:UpdatePanel>
  <div id="divChangePasswordMainBox" runat="server" class="main_box Mbox">
                <div class="main_box_header MBoxheader">
                    <h2>Weekend Setup Panel</h2>
                </div>
              <%--  <div class="punishment_bottom_header" style="width: 900px;">
                    
                    
                </div>--%>
                <div class="employee_box_body" style="background-color:white; min-height:500px;overflow:hidden">                    
        <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
        <Triggers>
       
           <asp:AsyncPostBackTrigger ControlID="ddlCompany" />
           <asp:AsyncPostBackTrigger ControlID="btnAddItem" />
           <asp:AsyncPostBackTrigger ControlID="btnAddAllItem" />
           <asp:AsyncPostBackTrigger ControlID="btnRemoveItem" />
           <asp:AsyncPostBackTrigger ControlID="btnRemoveAllItem" />
           
        </Triggers>
        <ContentTemplate>           
                 <div>                   
                     <div id="workerlist" runat="server" class="id_card" style="background-color:white; width:70%;">
                         <table style="width:100%;" >
                              
                         <tr>
                             <td>
                                 Company 
                             </td>
                             <td>
                                  <asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-control select_width" Width="80%" AutoPostBack="True" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged" >
                               </asp:DropDownList>
                           </td>                           
                         </tr>
                     </table>
                     <hr /> 
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
                 </div>     
            <br />
                <div>                   
                   <asp:RadioButtonList runat="server" ID="rblEmpType" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblEmpType_SelectedIndexChanged" >
                   </asp:RadioButtonList>                        
                   
                    <asp:RadioButtonList runat="server" ID="rblSetupType" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblSetupType_SelectedIndexChanged">
                        <asp:ListItem Value="day" Selected="True">Day wise</asp:ListItem>
                        <asp:ListItem Value="date">Date wise</asp:ListItem>
                        
                    </asp:RadioButtonList>
                    <table>
                        <tr runat="server" id="trDayWise">
                            <td>From Date<span class="requerd1">*</span>
                            </td>
                            <td>:
                            </td>
                            <td>
                                <asp:TextBox ID="txtFromDate" runat="server" ClientIDMode="Static" placeholder="dd-MM-yyyy" CssClass="form-control text_box_width" autocomplete="off"></asp:TextBox>
                                <asp:CalendarExtender runat="server" Format="dd-MM-yyyy"
                            PopupButtonID="imgDate" Enabled="True"
                            TargetControlID="txtFromDate" ID="CalendarExtender2">
                        </asp:CalendarExtender>



                            </td>
                            <td>To Date<span class="requerd1">*</span>
                            </td>
                            <td>:
                            </td>
                            <td>

                                <asp:TextBox ID="txtToDate" runat="server" ClientIDMode="Static" placeholder="dd-MM-yyyy" CssClass="form-control text_box_width" autocomplete="off"></asp:TextBox>
                                <asp:CalendarExtender runat="server" Format="dd-MM-yyyy"
                            PopupButtonID="imgDate" Enabled="True"
                            TargetControlID="txtToDate" ID="CalendarExtender1">
                        </asp:CalendarExtender>
                            </td>  
                            <td>

                                <asp:TextBox ID="txtCardNoDay" runat="server" ClientIDMode="Static" placeholder="Card No for Individual" CssClass="form-control text_box_width" autocomplete="off"></asp:TextBox>
                            </td>
                            <td>Weekend Day<span class="requerd1">*</span></td>
                            <td>:</td>
                            <td>
                                <asp:DropDownList ID="ddlWeekend" ClientIDMode="Static" CssClass="form-control select_width" runat="server">
                                    <asp:ListItem Value="0">--Select--</asp:ListItem>
                                    <asp:ListItem>Friday</asp:ListItem>
                                    <asp:ListItem>Saturday</asp:ListItem>
                                    <asp:ListItem>Sunday</asp:ListItem>
                                    <asp:ListItem>Monday</asp:ListItem>
                                    <asp:ListItem>Tuesday</asp:ListItem>
                                    <asp:ListItem>Wednesday</asp:ListItem>
                                    <asp:ListItem>Thursday</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                                <td>                            
                                <asp:Button runat="server" ID="btnSearchDay" Text="Search" ClientIDMode="Static" CssClass="btn btn-primary" OnClick="btnSearchDay_Click" />
                            </td>
                        </tr>
                        <tr runat="server" id="trDateWise">
                            <td>Weekend Date<span class="requerd1">*</span>
                            </td>
                            <td>:
                            </td>
                            <td>
                                <asp:TextBox ID="txtWeekendDate" runat="server" ClientIDMode="Static" placeholder="dd-MM-yyyy" CssClass="form-control text_box_width" autocomplete="off"></asp:TextBox>
                                <asp:CalendarExtender runat="server" Format="dd-MM-yyyy"
                            PopupButtonID="imgDate" Enabled="True"
                            TargetControlID="txtWeekendDate" ID="CExtApplicationDate">
                        </asp:CalendarExtender>
                            </td>
                            <td>
                                <asp:TextBox ID="txtCardNoDate" runat="server" ClientIDMode="Static" placeholder="Card No for Individual" CssClass="form-control text_box_width" autocomplete="off"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button runat="server" ID="btnSearchDate" Text="Search" ClientIDMode="Static" CssClass="btn btn-primary" OnClick="btnSearchDate_Click"  />
                            </td>
                        </tr>
                    </table>                   
                   
                    
                    <asp:GridView ID="gvEmployeeList" runat="server"  Width="100%" AutoGenerateColumns="False" DataKeyNames="EmpId,CompanyId,DptId,GId,DsgId" OnRowCommand="gvEmployeeList_RowCommand" OnRowDataBound="gvEmployeeList_RowDataBound">
                        <HeaderStyle BackColor="#0057AE" Font-Bold="True" Font-Size="14px" ForeColor="White" Height="28px"></HeaderStyle>  
                         <Columns>
                              <asp:TemplateField HeaderText="SL">
                                <ItemTemplate>
                                     <%# Container.DataItemIndex + 1 %>                                  
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="EmpCardNo"  HeaderText="Card No" ItemStyle-Height="28px" >
                             </asp:BoundField>  
                             <asp:BoundField DataField="EmpName"  HeaderText="Name" ItemStyle-Height="28px" >
                             </asp:BoundField>  
                             <asp:BoundField DataField="DsgName"  HeaderText="Designation" ItemStyle-Height="28px" >
                             </asp:BoundField>  
                             <asp:BoundField DataField="DptName"  HeaderText="Department" ItemStyle-Height="28px" >
                             </asp:BoundField>
                             <asp:BoundField DataField="EmpType"  HeaderText="Emp.Type" ItemStyle-Height="28px" >
                             </asp:BoundField>
                             <asp:BoundField DataField="PreWeekends"  HeaderText="Last Weekend" ItemStyle-Height="28px" >
                             </asp:BoundField> 
                             <asp:BoundField DataField="OverWriteOn"  HeaderText="Current Weekends" ItemStyle-Height="28px" >
                             </asp:BoundField>
                             <asp:BoundField DataField="Weekends"  HeaderText="New Weekends" ItemStyle-Height="28px" >
                             </asp:BoundField>            
                               <asp:TemplateField>
                                   <HeaderTemplate>
                                       <asp:CheckBox runat="server" ID="ckbEmpAll" ClientIDMode="Static" Checked="true" Text="All" AutoPostBack="true" OnCheckedChanged="ckbEmpAll_CheckedChanged" />
                                   </HeaderTemplate>
                                <ItemTemplate>
                                     <asp:CheckBox runat="server" ID="ckbEmp" ClientIDMode="Static" Checked="true" />
                                </ItemTemplate>
                            </asp:TemplateField>                             
                            <asp:TemplateField HeaderText="Delete" >
                                <ItemTemplate>
                                     <asp:Button ID="lnkDeleteEmp" runat="server" CommandName="remove" CommandArgument="<%#((GridViewRow)Container).RowIndex%>" Text="Delete" CssClass="btn btn-danger btnn" OnClientClick="return confirm('Are you sure to delete?');" ></asp:Button>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <div class="pull-right" style="padding:10px 0">
                       <asp:Button runat="server" ID="btnSubmit" Text="Submit" ClientIDMode="Static" CssClass="btn btn-success"  OnClick="btnSubmit_Click" />
                   </div>
                </div> 
  <br />
       

      
                  
                   </ContentTemplate>
    </asp:UpdatePanel>    
    </div>
    </div>
</asp:Content>
