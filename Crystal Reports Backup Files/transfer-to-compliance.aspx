<%@ Page Title="" Language="C#" MasterPageFile="~/personnel_NestedMaster.master" AutoEventWireup="true" CodeBehind="transfer-to-compliance.aspx.cs" Inherits="SigmaERP.personnel.transfer_to_compliance" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
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
    <style>
        .h1, .h1, h2, .h2, h3, .h3 {
            margin-bottom: 2px;
            margin-top: 10px;
        }

        .emp_header_left {
            font-size: 16px;
            padding-left: 10px;
            text-align: left;
            font: bold;
        }

        .total_emp {
            color: yellow;
            display: inline;
        }

        .emp_header_right {
            float: right;
            margin-top: -37px;
            font: bold;
        }

        #ContentPlaceHolder1_MainContent_gvForApprovedList th, td {
            text-align: center;
        }

            #ContentPlaceHolder1_MainContent_gvForApprovedList th:nth-child(3), td:nth-child(3) {
                text-align: left;
                padding-left: 3px;
            }

        #ContentPlaceHolder1_MainContent_gvForApprovedList td:nth-child(1) {
            color: red;
            font: bold;
            font-family: 'Times New Roman';
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
                  <div class="col-md-12 ">
               <div class="ds_nagevation_bar">
                   <ul>
                       <li><a href="/default.aspx">Dashboard</a></li>
                       <li><a href="#">/</a></li>
                       <li><a href="/personnel_defult.aspx">Personnel</a></li>
                       <li><a href="#">/</a></li>
                       <li><a href="/personnel/employee_index.aspx">Employee Information</a></li>
                       <li><a href="#">/</a></li>
                       <li><a href="#" class="ds_negevation_inactive Ptactive">Employees Details</a></li>
                   </ul>
               </div>
          
             </div>
       </div>

    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<asp:UpdatePanel ID="uplMessage" runat="server" >
    <ContentTemplate>
        <p class="message" id="lblMessage" clientidmode="Static" runat="server"></p>
    </ContentTemplate>
</asp:UpdatePanel>
    
<div style="padding:0;margin-top:25px;max-width:100%;">
    <div class="row Rrow">


                <div style="width: 100%; background:#750000 none repeat scroll 0 0 !important;"  class="list_main_content_box_header personal_color_header"  id="divElementContainer"  runat="server"> 
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <Triggers>
                     
                            <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />                            
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
                        </Triggers>
                        <ContentTemplate>
                            <div style="overflow: hidden;margin-bottom: 5px; border-bottom: 1px solid #ddd;">
                                <h3 class="emp_header_left">
                                <p style="font-size: 20px;text-align: center;font-weight: 500; text-shadow: 5px 5px 5px #000;">Employee List Panel</p>                   
                            </div>
                   <div>

                       <table>
                           <tr>
                           <td>Company</td>
                           <td>
                               <asp:DropDownList ID="ddlCompanyList" ClientIDMode="Static" CssClass="form-control inline_form_text_box_width" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged">
                               </asp:DropDownList>
                           </td>
                               <td>Employee Type</td>
                           <td>
                               <asp:RadioButtonList runat="server" ID="rblEmpType" AutoPostBack="true" RepeatDirection="Horizontal" >
                                    </asp:RadioButtonList>
                           </td>

                           <td>
                               <asp:Button runat="server" ID="btnSearch" CssClass="css_btn Ptbut" Text="Search" Width="75px" Height="34px" OnClick="btnSearch_Click" />
                           </td>
                           </tr>
                    </table>
                            
                  
                     </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div >
                   <div class="loding_img">
                       <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <span style=" font-family:'Times New Roman'; font-size:20px; color:green;font-weight:bold;float:left"><p>&nbsp;</p> </span> <br />
                                        <img cursor:pointer; float:left" src="/images/loader-2.gif"/>  
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                  </div>
                <asp:UpdatePanel runat="server" ID="up2">
                    <Triggers>                        
                       
                    </Triggers>
                    <ContentTemplate>
                     <div  style="width: 100%; background-color:#fff;margin: auto;">
                     <asp:GridView HeaderStyle-BackColor="#750000" ID="gvForApprovedList" runat="server" AutoGenerateColumns="false"  HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true" HeaderStyle-Height="25px" HeaderStyle-Font-Size="14px" Width="100%" DataKeyNames="EmpId,CompanyId" OnRowCommand="gvForApprovedList_RowCommand" OnRowDataBound="gvForApprovedList_RowDataBound">
                        
                          <Columns>  
                              <asp:TemplateField HeaderText="SL">
                                <ItemTemplate>
                                     <%# Container.DataItemIndex + 1 %>                                  
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>                         
                           <asp:BoundField DataField="EmpCardNo" HeaderText="Card No (Reg.)" ItemStyle-HorizontalAlign="Center" />
                                 <asp:BoundField DataField="EmpName" HeaderText="Name" />
                                 <asp:BoundField DataField="EmpJoiningDate" HeaderText="Join Date" ItemStyle-HorizontalAlign="Center" />
                                 <asp:BoundField DataField="DptName" HeaderText="Department" ItemStyle-HorizontalAlign="Center" />
                                 <asp:BoundField DataField="DsgName" HeaderText="Designation" ItemStyle-HorizontalAlign="Center" />
                                 <asp:BoundField DataField="SftName" HeaderText="Shift" ItemStyle-HorizontalAlign="Center" />
                                 <asp:BoundField DataField="EmpType" HeaderText="Type" ItemStyle-HorizontalAlign="Center" />
                                 <asp:BoundField DataField="EmpStatusName" HeaderText="Status" ItemStyle-HorizontalAlign="Center" />                                
                                <asp:TemplateField HeaderText="Change"  HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                                  <ItemTemplate >
                                      <asp:Button ID="btnEdit" runat="server" CommandName="Edit" Width="55px" Height="30px" Font-Bold="true" ForeColor="green" Text="Edit" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' />
                                  </ItemTemplate>
                              </asp:TemplateField>
                               <asp:TemplateField HeaderText="Transfer" HeaderStyle-Width="30px">
                                  <ItemTemplate>
                                      <asp:Button ID="btnTransfer" runat="server" CommandName="Transfer" Font-Bold="true" ForeColor="Blue" Text="Transfer" Width="62px" Height="30px" OnClientClick="return confirm('Do you want to transfer this employe ?')" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' />
                                  </ItemTemplate>
                              </asp:TemplateField> 
                               <asp:TemplateField HeaderText="Delete" HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                                  <ItemTemplate>
                                      <asp:Button ID="btnView" runat="server" CommandName="Remove" Font-Bold="true" ForeColor="red" Text="Delete" Width="55px" Height="30px" OnClientClick="return confirm('Do you want to delete this record ?')" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' />
                                  </ItemTemplate>
                              </asp:TemplateField>                                                    
                          </Columns>
                     </asp:GridView>
                         <div id="divRecordMessage" runat="server" visible="false" style="color: red; font-weight: bold; text-align: center; padding-top: 75px; font-size: 32px; height: 118px">
                           
                         </div>
                </div>
                   </ContentTemplate>
                </asp:UpdatePanel>
        
        </div>
    </div>
    <style type=”text/css”>
    .hand { cursor: pointer; cursor: hand; } /* cross browser hand */
</style>
       <script type="text/javascript">
           $(document).ready(function () {              
               $(document).on("keypress", "body", function (e) {
                   if (e.keyCode == 13) e.preventDefault();
                   // alert('deafault prevented');

               });
           });
           function loadEmpInfo(e) {              
               if (e.keyCode == 13) {                   
                   //jQuery.ajax({
                   //    url: 'employee_list.aspx/LoadEmpInfo',
                   //    type: "POST",
                   //    data: "",
                   //    contentType: "application/json; charset=utf-8",
                   //    dataType: "json",
                   //    beforeSend: function () {
                   //        //alert("Start!!! ");
                   //    },
                   //    success: function (data) {
                   //        alert("a");
                   //    },
                   //    failure: function (msg) { alert("Sorry!!! "); }
                   //});
                   
               }                               
           }
           function deleteRow(id) {
               var answer = confirm("Are you sure you want to Delete");
               if (answer == true) {
                   jx.load('/ajax.aspx?id=' + id + '&todo=DeleteEmployee', function (data) {
                       document.location = '/personnel/employee_list.aspx';
                       // $('#divEmployeeList').html(data);
                   });
                   return true;
               }
               else {
                   return false;
               }


           }
           function editEmployee(id) {
               goURL('/personnel/employee.aspx?EmpId=' + id + "&Edit=True");
           }
           function confirmDelete() {
               alert("confirmComplete");
               var answer = confirm("Are you sure you want to Delete");
               if (answer == true) {
                   $('#hdfdeleteconfirm').val('true');
                   //return true;
               }
               else {
                   $('#hdfdeleteconfirm').val('false');
                   // return false;
               }
           }
    </script>
</asp:Content>