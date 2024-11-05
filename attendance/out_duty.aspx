<%@ Page Title="Out Duty" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="out_duty.aspx.cs" Inherits="SigmaERP.attendance.out_duty" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <%-- <script src="../scripts/jquery-1.8.2.js"></script>--%>
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
        #ContentPlaceHolder1_MainContent_gvOutDuty th, td {
            text-align:center;
        }
           #ContentPlaceHolder1_MainContent_gvOutDuty th:nth-child(3),td:nth-child(3) {
            text-align:left;
            padding-left:3px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar">
                <ul>
                    <li><a href="/default.aspx">Dashboard</a></li>
                    <li>/</li>
                   <li><a href="<%=  Session["__topMenu__"] %>">Attendance</a></li>
                    <li>/</li>
                    <li><a href="#" class="ds_negevation_inactive Mactive">Out Duty Entry</a></li>
                </ul>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
   <asp:UpdatePanel ID="uplMessage" runat="server">
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>
    
    <div class="main_box Mbox">
        <div class="main_box_header MBoxheader">
            <h2>Out Duty</h2>
        </div>
        <div class="employee_box_body">
            <div class="employee_box_content">
                
       
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <Triggers>           
            <asp:AsyncPostBackTrigger ControlID="btnSave" />
       <asp:AsyncPostBackTrigger ControlID="gvOutDuty" />
            <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />
        </Triggers>
        <ContentTemplate>
                <div class="row">
                    <center>
                    <%--<div class="col-md-4 col-md-offset-4">--%>
                     
                     <div id="divFindInfo" runat="server" style="font-weight:bold"></div>
                     
                       <table >
                        <tr>
                                <td>Company<span class="requerd1">*</span></td>
                                <td>:</td>
                                <td colspan="3"> 
                                    <asp:DropDownList ID="ddlCompanyList" runat="server" ClientIDMode="Static" AutoPostBack="true" CssClass="form-control select_width" Width="98%"  OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged" >
                                    </asp:DropDownList>
                                </td>
                               
                               
                            </tr> 
              
                            <tr  >
                                <td>Date <span class="requerd1">*</span></td>
                                <td>: </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtDate" runat="server" autocomplete="off" ClientIDMode="Static" CssClass="form-control text_box_width"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="dd-MM-yyyy" TargetControlID="txtDate">
                                    </asp:CalendarExtender>
                                </td>
                            </tr>  
                           <tr>
                                        <td>From <span class="requerd1">*</span></td>
                                        <td>:</td>
                                        <td>
                                            <asp:TextBox ID="txtInHur" runat="server" ClientIDMode="Static" CssClass="form-control attend_text_box_width" MaxLength="2" Text="00" Style="text-align: center; font-weight: bold; width: 60px; float: left;"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtInMin" runat="server" ClientIDMode="Static" CssClass="form-control attend_text_box_width" MaxLength="2" Text="00" Style="text-align: center; font-weight: bold; width: 60px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlInTimeAMPM" runat="server" CssClass="attend_select_min" Width="67px">                                                
                                                <asp:ListItem Value="PM">PM</asp:ListItem>
                                                <asp:ListItem Value="AM">AM</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>    
                               </tr>
                           <tr>
                                        <td>To <span class="requerd1">*</span></td>
                                        <td>:</td>                                  
                                        <td>
                                            <asp:TextBox ID="txtOutHur" runat="server" ClientIDMode="Static" CssClass="form-control attend_text_box_width" MaxLength="2" Text="00" Style="text-align: center; font-weight: bold; width: 60px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtOutMin" runat="server" ClientIDMode="Static" CssClass="form-control attend_text_box_width" MaxLength="2" Text="00" Style="text-align: center; font-weight: bold; width: 60px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlOutTimeAMPM" runat="server" CssClass="attend_select_min" Width="67px">
                                                <asp:ListItem Value="PM" Selected="True">PM</asp:ListItem>
                                                <asp:ListItem Value="AM">AM</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                 
                        <tr runat="server" id="trEmpCardNo">
                            <td>Card No<span class="requerd1">*</span>
                            </td>
                            <td>:
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="txtEmpCardNo" ClientIDMode="Static"  runat="server" CssClass="form-control text_box_width" Width="95%" MaxLength="13" ></asp:TextBox> 
                             
                             </td>
                          <td>
                              <asp:Button ID="btnFindEmpInfo" runat="server" Text="Find " CssClass="Mbutton" Width="60px" Height="37px" OnClientClick="return InputValidation();" OnClick="btnFindEmpInfo_Click"></asp:Button>
                          </td>

                        </tr>
                             <tr>
                                <td>Assigned By</span></td>
                                <td>:</td>
                                <td colspan="3"> 
                                    <asp:TextBox ID="txtAssignedBy" ClientIDMode="Static"  runat="server" CssClass="form-control select_width" Width="98%"   ></asp:TextBox> 
                             
                                    
                                </td>
                               
                               
                            </tr> 
                                                
                            <tr runat="server" visible="false">
                                <td>Type <span class="requerd1">*</span></td>
                                <td>: </td>
                               <td>
                              <asp:RadioButtonList runat="server" ID="rblDutyType" RepeatDirection="Horizontal" >
                                  <asp:ListItem Value="0" Selected="True">Out Duty</asp:ListItem>
                                  <asp:ListItem Value="1">Training</asp:ListItem>
                              </asp:RadioButtonList>
                           </td>

                            </tr>
                                                                                
                        
                                   <tr  >
                                <td>Purpose</td>
                                <td>: </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtPurpose" runat="server" TextMode="MultiLine" ClientIDMode="Static" CssClass="form-control text_box_width" Height="35px"></asp:TextBox>
                                  
                                </td>
                            </tr>
                                  <tr  >
                                <td>Place</td>
                                <td>: </td>
                                <td colspan="3">
                                    <asp:TextBox ID="txtPlace" runat="server" TextMode="MultiLine" ClientIDMode="Static" CssClass="form-control text_box_width" Height="35px"></asp:TextBox>
                                  
                                </td>
                            </tr>
                        
                    </table>
                        <%--</div>--%>
                        </center>
                </div>
                <div class="button_area" style="margin-top:10px;">
                    <%--<div class="col-md-4 col-md-offset-4">--%>
                    <center>

                    <table class="button_table">
                        <tr>
                          
                            <th>
                                <asp:Button ID="btnSave" CssClass="Mbutton" runat="server" Text="Save" OnClientClick="return InputValidationBasket();" OnClick="btnSave_Click" />
                            </th>
                            <th>
                                <asp:Button ID="btnClear" CssClass="Mbutton" runat="server" Text="Clear" />
                           <th> 
                               <asp:Button ID="Button3" runat="server" Text="Close" PostBackUrl="~/attendance_default.aspx" CssClass="Mbutton" />
                           </th>
                           
                        
                        </tr>
                    </table>
                        <br />
                        <table class="button_table">
                        <tr>
                          <th>From : </th>
                            <th>
                                <asp:TextBox ID="txtFromDate" runat="server" autocomplete="off" ClientIDMode="Static" CssClass="form-control text_box_width"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MM-yyyy" TargetControlID="txtFromDate">
                                    </asp:CalendarExtender>
                            </th>
                            <th>To : </th>
                            <th>
                                <asp:TextBox ID="txtToDate" runat="server" autocomplete="off" ClientIDMode="Static" CssClass="form-control text_box_width"></asp:TextBox>
                                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd-MM-yyyy" TargetControlID="txtToDate">
                                    </asp:CalendarExtender>
                            </th>
                             <th>Card No : </th>
                            <th>
                               <asp:TextBox ID="txtCardNoForSearch" runat="server" autocomplete="off" ClientIDMode="Static" CssClass="form-control text_box_width"></asp:TextBox>
                           <th> 
                               <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="Mbutton" OnClick="btnSearch_Click" />
                           </th>
                           
                        
                        </tr>
                    </table>
                        
                    </center>
                
                <%--</div>--%>
                </div>
            
                                </ContentTemplate>
    </asp:UpdatePanel>  
                <br />
                
                 <div class="dataTables_wrapper">
                     <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <div class="table-responsive">
                    <asp:GridView ID="gvOutDuty" runat="server" HeaderStyle-BackColor="#2B5E4E" HeaderStyle-Height="28px" HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true" HeaderStyle-Font-Size="14px" AllowPaging="true" PageSize="40" Width="100%" AutoGenerateColumns="False" DataKeyNames="SL" CellPadding="4" ForeColor="#333333" Height="13px" OnRowCommand="gvOutDuty_RowCommand" OnRowDataBound="gvOutDuty_RowDataBound">
                        <PagerStyle CssClass="gridview" Height="20px" />
                        <AlternatingRowStyle BackColor="White" />
                        <Columns>

                             <asp:TemplateField HeaderText="S.No" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>                                        
                                        <%# Container.DataItemIndex+1%>
                                    </ItemTemplate>

                                    <ItemStyle HorizontalAlign="Center" ForeColor="green" />
                                </asp:TemplateField>

                            <asp:TemplateField HeaderText="Card No">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmpCode" runat="server" Text='<%# Eval("EmpCardNo") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmpName" runat="server" Text='<%# Eval("EmpName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Department">
                                <ItemTemplate>
                                    <asp:Label ID="lblDptName" runat="server" Text='<%# Eval("DptName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField  HeaderText="Designation">
                                <ItemTemplate>
                                    <asp:Label ID="lblDsgName" runat="server" Text='<%# Eval("DsgName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date" >
                                <ItemTemplate>
                                    <asp:Label ID="lblDate" runat="server" Text='<%# Eval("Date") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="From" >
                                <ItemTemplate>
                                    <asp:Label ID="lblIn" runat="server" Text='<%# Eval("InTime") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="To" >
                                <ItemTemplate>
                                    <asp:Label ID="lblOut" runat="server" Text='<%# Eval("OutTime") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Type" >
                                <ItemTemplate>
                                    <asp:Label ID="lblType" Font-Bold="true" runat="server" Text='<%# Eval("TypeName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Status">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" ForeColor="Blue" Font-Bold="true" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Purpose">
                                <ItemTemplate>
                                    <asp:Label Font-Bold="true" ID="lblOuttime"  runat="server" Text='<%# Eval("Remark") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Processing">
                                <ItemTemplate>
                                    <asp:Label Font-Bold="true" ID="lblProcessing"  runat="server" Text='<%# Eval("Processing") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Authorized By">
                                <ItemTemplate>
                                    <asp:Label Font-Bold="true" ID="lblAuthorizedBy"  runat="server" Text='<%# Eval("AuthorizedByName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                               <asp:TemplateField>
                                  <HeaderTemplate>
                                      View
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Button ID="btnView" runat="server" Text="View" Font-Bold="true" CommandName="View" ForeColor="Green"                                          
                                          CommandArgument=<%# Container.DataItemIndex%> />
                                  </ItemTemplate>
                                  <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                                     <asp:TemplateField>
                                  <HeaderTemplate>
                                      Delete
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Button ID="btnDelete" runat="server" Text="Delete" Font-Bold="true" CommandName="deleterow" ForeColor="Red"
                                          OnClientClick="return confirm('Are you sure, you want to delete the record?')"
                                          CommandArgument=<%# Container.DataItemIndex%> />
                                  </ItemTemplate>
                                  <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField>
                          
                        </Columns>                        
                    </asp:GridView>
                    </div>
                   </ContentTemplate>
    </asp:UpdatePanel> 
                </div>
                                           
            </div>
             

        </div>
    </div>

     <script type="text/javascript">
  
         $(document).ready(function () {
             loadcardNo();

         });
         function loadcardNo() {
             $("#ddlAssigned").select2();
         }
         function goToNewTabandWindow(url) {
             window.open(url);
             loadcardNo();
         }
    </script>
</asp:Content>
