<%@ Page Title="Special Break Time" Language="C#" MasterPageFile="~/hrd_nested.master" AutoEventWireup="true" CodeBehind="SpecialBreakTime.aspx.cs" Inherits="SigmaERP.hrd.SpecialBreakTime" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../scripts/jquery-1.8.2.js"></script>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row Rrow">
                  <div class="col-md-12 ds_nagevation_bar">
               <div style="margin-top: 5px">
                   <ul>
                       <li><a href="/default.aspx">Dashboard</a></li>
                       <li> <a href="#">/</a></li>
                         <li><a href="<%= Session["__topMenuforSettings__"] %>">Settings</a></li>
                       <li> <a href="#">/</a></li>
                       <li> <a href="#" class="ds_negevation_inactive Ractive">Special Break Time</a></li>
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
    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnSave" />
                                <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />
                            </Triggers>
                            <ContentTemplate>

              <div class="main_box RBox">
    	<div class="main_box_header RBoxheader">
            <h2>Special Break Time</h2>
        </div>
    	<div class="main_box_body Rbody">
        	<div class="main_box_content">
                <div class="input_division_info RTable">
                    <table>                       
                        <tr>
                            <td colspan="4">
                                <span style="text-align:left;display:block;">Company <span class="requerd1">*</span></span>
                                <div style="text-align:left;display:block;">
                               
                                     <asp:DropDownList ID="ddlCompanyList" runat="server" ClientIDMode="Static" CssClass="form-control"  AutoPostBack="True" ></asp:DropDownList>
                                    </div>

                            </td>
                        </tr>  
                        <tr>
                            <td>
                               <span style="float:left">Duty Type <span class="requerd1">*</span></span> <br />
                                <asp:RadioButtonList ID="rblDutyType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Selected="True" Value="All">All</asp:ListItem>
                                    <asp:ListItem Value="Regular">Regular</asp:ListItem>
                                    <asp:ListItem Value="Roster">Roster</asp:ListItem>
                                </asp:RadioButtonList> 
                            </td>                             
                        </tr>
                        <tr>
                            <td colspan="4">
                                <span style="text-align:left;display:block;">Title <span class="requerd1">*</span></span>
                                <div style="text-align:left;display:block;">
                               
                                      <asp:TextBox ID="txtTitle" runat="server" ClientIDMode="Static" CssClass="form-control" ></asp:TextBox>
                                    </div>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <span style="text-align:left;display:block;">Date <span class="requerd1">*</span></span>
                                <div style="text-align:left;display:block;">
                               
                                      <asp:TextBox ID="txtDate" autocomplete="off" runat="server" ClientIDMode="Static" CssClass="form-control" placeHolder="DD-MM-YYYY"></asp:TextBox>

                                <asp:CalendarExtender ID="txtDate_CalendarExtender" runat="server"  Format="dd-MM-yyyy" TargetControlID="txtDate">
                                     </asp:CalendarExtender>
                                    </div>

                            </td>
                            <td>
                                 <asp:CheckBox ID="ckbNextDay" ClientIDMode="Static" runat="server" Text="Next Day?" />
                            </td>
                        </tr>
                                              
                      
                        <tr>
                           
                            <td>
                                <table>
                                    <tr>
                                        <td colspan="4"> Start Time<span class="requerd1">*</span></td>
                                    </tr>
                                    <tr>
                                        <td><asp:TextBox ID="txtStartTimeHH" style="text-align:center" runat="server" placeHolder="00" ClientIDMode="Static" CssClass="form-control text_box_width RInput" ></asp:TextBox></td>
                                        <td>MM</td>
                                        <td><asp:TextBox ID="txtStartTimeMM" runat="server" style=" text-align:center" ClientIDMode="Static" CssClass="form-control text_box_width RInput" placeHolder="00"></asp:TextBox></td>
                                        <td>
                                            <asp:DropDownList ID="ddlStartTimeAMPM" CssClass="form-control select_width RSelt" runat="server" ClientIDMode="Static">
                                                <asp:ListItem Value="AM">AM</asp:ListItem>
                                                <asp:ListItem Value="PM">PM</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                           
                             <td></td>                           
                           
                                                   
                            <td>                     
                                <table>
                                    <tr>
                                        <td colspan="4"> End Time<span class="requerd1">*</span></td>
                                    </tr>
                                    <tr>
                                        <td><asp:TextBox ID="txtEndTimeHH" style=" text-align:center" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width RInput" placeHolder="00" ></asp:TextBox></td>
                                        <td>MM</td>
                                        <td><asp:TextBox ID="txtEndTimeMM" style=" text-align:center" runat="server" ClientIDMode="Static" CssClass="form-control text_box_width RInput" placeHolder="00" ></asp:TextBox></td>
                                        <td>
                                            <asp:DropDownList ID="ddlEndTimeAMPM" CssClass="form-control select_width RSelt" runat="server" ClientIDMode="Static">
                                                <asp:ListItem Value="AM">AM</asp:ListItem>
                                                <asp:ListItem Value="PM">PM</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                             <td></td>                          
                         
                        </tr>                      
                                                                   
                        <tr>
                            <td>
                               <span style="float:left">Active <span class="requerd1">*</span></span> <br />
                                <asp:RadioButtonList ID="rblActiveInactive" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Selected="True" Value="1">Yes</asp:ListItem>
                                    <asp:ListItem Value="0">No</asp:ListItem>
                                </asp:RadioButtonList> 
                            </td>                             
                        </tr>                        
                    </table>                  
                </div>                   

               <div class="button_area Rbutton_area">
                     <asp:Button ID="btnSave" ClientIDMode="Static" CssClass="Rbutton"  runat="server" Text="Save" OnClientClick="return validateInputs();" OnClick="btnSave_Click"   />
                   <asp:Button ID="btnClear" ClientIDMode="Static" CssClass="Rbutton"  runat="server" Text="Clear" OnClick="btnClear_Click" />
                </div>

               <div class="show_division_info">

                <%--Share--%>                
                   
                    <asp:GridView ID="gvShiftConfigurationList" runat="server"  Width="100%"  AutoGenerateColumns="false" DataKeyNames="SL,CompanyID"   OnRowCommand="gvShiftConfigurationList_RowCommand" OnRowDeleting="gvShiftConfigurationList_RowDeleting" AllowPaging="True" PageSize="15" OnRowDataBound="gvShiftConfigurationList_RowDataBound" OnPageIndexChanging="gvShiftConfigurationList_PageIndexChanging">
                       <RowStyle HorizontalAlign="Center" Height="30px" />
                        <EditRowStyle Height="28px" />
                        <HeaderStyle BackColor="#0057AE" Height="28px" Font-Size="14px" ForeColor="White"  />
                        <PagerStyle  CssClass="gridview Sgridview" Height="40px" />
                         <Columns>       
                             <asp:BoundField DataField="DutyType" HeaderStyle-HorizontalAlign="Left" HeaderText="Duty Type"   ItemStyle-HorizontalAlign="Left"/> 
                             <asp:BoundField DataField="Title" HeaderStyle-HorizontalAlign="Left" HeaderText="Title"   ItemStyle-HorizontalAlign="Left"/>  
                            <asp:BoundField DataField="Date" HeaderStyle-HorizontalAlign="Left" HeaderText="Date"   ItemStyle-HorizontalAlign="Left"/>                            
                               
                             <asp:BoundField DataField="StartTime" HeaderText="Start Time"  />
                            <asp:BoundField DataField="EndTime" HeaderText="End Time"   />
                            <asp:BoundField DataField="BreakTime" HeaderText="Duration"  />
                             <asp:TemplateField >
                                  <HeaderTemplate>Next Day?</HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:CheckBox ID="ckbNextDay_gv" runat="server" Checked='<%#bool.Parse(Eval("NextDay").ToString())%>' Enabled="false" />
                                  </ItemTemplate>
                              </asp:TemplateField> 
                            <asp:BoundField DataField="IsActive" HeaderText="Active"  />                            
                                                         

                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                     <asp:Button ID="lnkAlter" runat="server" ControlStyle-CssClass="btnForAlterInGV"  Text="Edit" CommandName="Alter" CommandArgument="<%#((GridViewRow)Container).RowIndex%>" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Delete" >
                                <ItemTemplate>
                                      <asp:Button ID="lnkDelete" runat="server" CommandName="Delete" CommandArgument="<%#((GridViewRow)Container).RowIndex%>" Text="Delete" ControlStyle-CssClass="btnForDeleteInGV" OnClientClick="return confirm('Are you sure to delete?');" />
                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>         
              </div>				
            </div>
        </div>
    </div>
                                </ContentTemplate>
                </asp:UpdatePanel>
    <script type="text/javascript">
     $('#btnNew').click(function () {
              clear();
          });
          function validateInputs() {
              if (validateText('txtTitle', 1, 50, 'Enter Valid Title (Max 50 digit)') == false) return false;
              if (validateText('txtDate',10, 10, 'Enter Valid Date') == false) return false;
              if (validateText('txtStartTimeHH',1, 2, 'Enter Valid Start Hour') == false) return false;
              if (validateText('txtStartTimeMM', 1, 2, 'Enter Valid Start Minute') == false) return false;
              if (validateText('txtEndTimeHH',1, 2, 'Enter Valid End Hour') == false) return false;
              if (validateText('txtEndTimeMM',1, 2, 'Enter Valid End Minute') == false) return false;
              return true;
          }

          function clear() {
              if ($('#upSave').val() == '0') {

                  $('#btnSave').removeClass('css_btn');
                  $('#btnSave').attr('disabled', 'disabled');
              }
              else {
                  $('#btnSave').addClass('css_btn');
                  $('#btnSave').removeAttr('disabled');
              }

              $('#txtShiftName').val('');
              $('#txtEffectiveDate').val('');
              $('#txtStartTime').val('');
              $('#txtEndTime').val('');
              $('#txtAcceptableLate').val('');
              $('#txtDelayTimeOut').val('');
              $('#btnSave').val('Save');
              $('#hdnbtnStage').val("");
              $('#hdnUpdate').val("");
              $('#txtShiftName').focus();
           
          }

    </script>
</asp:Content>

