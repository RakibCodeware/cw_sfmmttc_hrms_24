<%@ Page Title="Attendance Analysis Report" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="attendance_analysis_report.aspx.cs" Inherits="SigmaERP.attendance.attendance_analysis_report" %>
<%@ Register Assembly="AjaxControlToolkit"  Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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

    <style type="text/css">
        .style {
            font-size: 17px;
            font-weight: bold;
            width: 95px;
        }

        #ContentPlaceHolder1_MainContent_gvAttendanceList th {
            text-align: center;
        }

        #ContentPlaceHolder1_MainContent_divElementContainer h2 {
            font-size: 16px;
            padding: 0px;
            text-align: center;
        }

        #ContentPlaceHolder1_MainContent_gvAttendanceList th:nth-child(2),td:nth-child(2) {
           text-align: left;
           padding-left: 3px;
        }
     
        .emp_header_left {
            font-size: 16px;
            padding-left: 10px;
            text-align: left;
            font: bold;
            margin-top:10px;
        }
           .emp_header_right {
            float: right;
            margin-top: -37px;
            font: bold;
        }
           .d-block{
               display:block;
           }
           /*.align-items-end{
               align-items:flex-end !important;
           }*/
          
           .align-bottom{
               display:flex;
               align-items:flex-end;
           }
    </style>
  <script type="text/javascript">
      //$(function () {
      //    alert();
      //    $("#btnForGet").click(function(){

      //        alert("Yes U Pressed");
      //    })
      //})
    </script>
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
                    <li><a href="#" class="ds_negevation_inactive Mactive">Attendance Analysis Report</a></li>
                </ul>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="uplMessage" runat="server">
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>

    <div style="padding:0;margin-top:25px;max-width:100%;">
    <div class="row Rrow">
                <div id="divElementContainer" runat="server" class="list_main_content_box_header MBoxheader" style="width: 100%;">
                     
                 <%--<div style="overflow: hidden;margin-bottom: 5px; border-bottom: 1px solid #ddd;">--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" />
            <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />

        </Triggers>
    <ContentTemplate>
        <div style="overflow: hidden; margin-bottom: 5px; border-bottom: 1px solid #ddd;">
           
            <%--<h3 class="emp_header_left">--%>
             <h2 style="padding:10px 0;" class="text-center">
                
               Attendance Analysis Report
           </h2>
        </div>
        <%--</div>--%>
        <div style="width: 100%;">

            <div class="container">

                <table>
                    <tr>
                        <td><label class="text-white">Company</label>
                        <asp:DropDownList ID="ddlCompanyList" ClientIDMode="Static" CssClass="form-control" runat="server">
                             </asp:DropDownList></td>
                        <td> <label class="text-white">From Date</label>
                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
                             <asp:CalendarExtender ID="txtFromDate_CalendarExtender" runat="server" Format="dd-MM-yyyy" TargetControlID="txtFromDate">
                             </asp:CalendarExtender></td>
                        <td> <label class="text-white">To Date</label>
                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
                             <asp:CalendarExtender ID="txtToDate_CalendarExtender" runat="server" Format="dd-MM-yyyy" TargetControlID="txtToDate">
                             </asp:CalendarExtender></td>
                        <td>
                            <label class="text-white"></label>
                        <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-danger d-block" Text="Search" OnClick="btnSearch_Click" /></td>
                    </tr>
                </table>

          <%--      <div class="row align-bottom">
                    <div class="col-lg-3">
                        
                    </div>
                    <div class="col-lg-3">
                       
                    </div>
                    <div class="col-lg-3">
                       
                    </div>
                    <div class="col-lg-3 d-flex align-items-end">
                  
                    </div>
                </div>--%>
            </div>
        </div>
                     <%--</h2>--%>
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
    <asp:UpdatePanel runat="server" ID="up2">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
                    </Triggers>
                    <ContentTemplate>
             <div style="width: 100%; margin:0px auto ">
                     <asp:GridView HeaderStyle-BackColor="#2B5E4E" HeaderStyle-Height="28px" ID="gvAttendanceList" runat="server" AutoGenerateColumns="false"  HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true" HeaderStyle-Font-Size="14px"  Width="100%" DataKeyNames="EmpId"  >
                         <PagerStyle CssClass="gridview" />
                          <Columns>
                               <asp:TemplateField HeaderText="SL">
                                <ItemTemplate>
                                     <%# Container.DataItemIndex + 1 %>                                  
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField> 
                              <asp:BoundField DataField="EmpCardNo" HeaderText="Card No" Visible="true"  ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                              <asp:BoundField DataField="RegId" HeaderText="Reg.Id" Visible="true"  ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                              <asp:BoundField DataField="EmpName" HeaderText="Name" />           
                              <asp:BoundField DataField="AttDays" HeaderText="Attendance Days" />           
                        
                          </Columns>                         
                     </asp:GridView>
                       <div id="divRecordMessage" runat="server" visible="false" style="color: red; background-color:white; font-weight: bold; text-align: center; padding-top: 75px; font-size: 32px; height: 118px">
                         </div>
                </div>            
     
       
    </ContentTemplate>
</asp:UpdatePanel>
                      </div>
            </div>
</asp:Content>