<%@ Page Title="Absent Notifications" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="absent_notification_log.aspx.cs" Inherits="SigmaERP.attendance.absent_notification_log" %>
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
    <style>
        #ContentPlaceHolder1_MainContent_gvAbsentList tr th:nth-child(6),
        #ContentPlaceHolder1_MainContent_gvAbsentList tr th:nth-child(7)
        {
           text-align: center;
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
                    <li><a href="#" class="ds_negevation_inactive Mactive">Absent Notification List</a></li>
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
                     
              
                </div>
     
    <asp:UpdatePanel runat="server" ID="up2">
                    <Triggers>                       
                    </Triggers>
                    <ContentTemplate>
                <div style="width: 100%; margin:0px auto ">
                     <br />
                        <table>
                            <tr>
                                <td></td>
                                <td><asp:RadioButtonList runat="server" ID="rblEmpType" AutoPostBack="true" RepeatDirection="Horizontal" OnSelectedIndexChanged="rblEmpType_SelectedIndexChanged">
                                </asp:RadioButtonList></td>
                                <td>From Date</td>
                                <td><asp:TextBox ID="txtFromDate" ClientIDMode="Static" runat="server" CssClass="form-control text_box_width"></asp:TextBox>
                                <asp:CalendarExtender
                                    ID="TextBoxDate_CalendarExtender" Format="dd-MM-yyyy" runat="server" Enabled="True" TargetControlID="txtFromDate">
                                </asp:CalendarExtender></td>
                                <td>To Date</td>
                                <td><asp:TextBox ID="txtToDate" ClientIDMode="Static" runat="server" CssClass="form-control text_box_width"></asp:TextBox>
                                <asp:CalendarExtender
                                    ID="CalendarExtender1" Format="dd-MM-yyyy" runat="server" Enabled="True" TargetControlID="txtToDate">
                                </asp:CalendarExtender></td>
                                <td>
                                    <asp:Button ID="btnSearch" CssClass="Mbutton" runat="server" Text="Search" OnClick="btnSearch_Click"   />
                                </td>
                                <td>
                                    <asp:Button ID="btnPrint" CssClass="Mbutton" runat="server" Text="Print"  OnClientClick="portraitPrintHTML('divAbsentNotificationList')"  />
								<%--<a class="btn btn-info" onclick="portraitPrintHTML('divAbsentNotificationList')" >Print</a>--%>
                                </td>
                            </tr>
                        </table>       
                    <br />
                    <div id="divAbsentNotificationList">    
                        <center>
                       <h2 runat="server" id="hCompanyName" visible="false"></h2> 
                        <a runat="server" id="hCompanyAddress"  visible="false"></a>
                        <h3 runat="server" id="hReportTitle" visible="false">Absent Notification List</h3> 
                            </center>

                         <asp:GridView HeaderStyle-BackColor="#2B5E4E" HeaderStyle-Height="28px" ID="gvAbsentList" runat="server" AutoGenerateColumns="false"  HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true" HeaderStyle-Font-Size="14px"  Width="100%" DataKeyNames="EmpId" OnRowDataBound="gvAbsentList_RowDataBound" >
                          <Columns>   
                              <asp:TemplateField HeaderText="S.No" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex+1%>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" ForeColor="green" />
                              </asp:TemplateField>
                             <asp:BoundField DataField="EmpCardNo" HeaderText="Card No"/>                              
                             <asp:BoundField DataField="EmpName" HeaderText="Name"/>
                             <asp:BoundField DataField="DptName" HeaderText="Department" />
                             <asp:BoundField DataField="DsgName" HeaderText="Designation"/>
                              <asp:TemplateField HeaderText="Date" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                               <asp:Label runat="server" ID="lblDate" ClientIDMode="Static" Text='<%# Bind("Date") %>' ></asp:Label>
                                            </ItemTemplate>                                           
                              </asp:TemplateField>
                              <asp:TemplateField HeaderText="Last Working Day" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                               <asp:Label runat="server" ID="lblLastWorkingDay" ClientIDMode="Static" Font-Bold="true" Text='<%# Bind("LastWorkingDay") %>' ></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" ForeColor="green" />
                              </asp:TemplateField>
                          </Columns>                         
                     </asp:GridView>     
                         </div>
                </div>      
    </ContentTemplate>
</asp:UpdatePanel>
             </div>
            </div>
    <script>
	 function portraitPrintHTML(divPrint) {

      //$(".hidePrint").css("display", "none");

      var mywindow = window.open('', 'PRINT', 'height=400,width=600');

      //  mywindow.document.write('<html><head><title>' + document.title  + '</title>');
      //  mywindow.document.write('</head><body >');
      //  mywindow.document.write('<h1>' + document.title  + '</h1>');
      mywindow.document.write('<html><head><title></title>');
      mywindow.document.write('<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css">');
      mywindow.document.write('<link href="./styles.css" rel="stylesheet">');
      //mywindow.document.write('<link href="./assets/fonts/nikosh/stylesheet.css" rel="stylesheet">');
      mywindow.document.write('<style>');
      //mywindow.document.write('@page { size: 210mm 297mm;margin: 1cm 1cm 1cm 1cm; }');
      mywindow.document.write('@media print{');
      mywindow.document.write('.page-break{page-break-after: always;}');
      mywindow.document.write('table {width:100%}');
      mywindow.document.write('.table-bordered th{vertical-align: middle!important;background: #ededed!important;text-align:center!important}');
      mywindow.document.write('.table-bordered th, .table-bordered td {border: 1px solid #000 !important;font-size:16px!important;color:#000!important}');
      mywindow.document.write('.print-report-header table th {font-size:18px!important;font-weight:500!important;}');
      mywindow.document.write('.small-gap-td td {padding: 7px!important}');
      mywindow.document.write('.table-footer td {background: #ededed!important}');
      mywindow.document.write('.page-break{page-break-after: always;}');
      mywindow.document.write('}');
      mywindow.document.write('</style>');
      //mywindow.document.write('@media print{*,:after,:before{color:#000!important;text-shadow:none!important;background:0 0!important;-webkit-box-shadow:none!important;box-shadow:none!important}');
      // mywindow.document.write('<>');
      // mywindow.document.write('.table>tbody>tr>td, .table>tbody>tr>th, .table>tfoot>tr>td, .table>tfoot>tr>th, .table>thead>tr>td, .table>thead>tr>th {');
      // mywindow.document.write('border: solid black !important;border-width: 1px 0 0 1px !important;}</>');
      mywindow.document.write('</head><body style="background-color: #fff !important;" >');
      mywindow.document.write(document.getElementById(divPrint).innerHTML);
      mywindow.document.write('</body></html>');

      mywindow.document.close(); // necessary for IE >= 10
      mywindow.focus(); // necessary for IE >= 10*/

      //mywindow.print();
      //mywindow.close();

      setTimeout(function () {
        mywindow.print();
        var ival = setInterval(function () {
          mywindow.close();
          clearInterval(ival);
        }, 200);
      }, 500);

      //$(".hidePrint").css("display", "block");

      return true;
    }

	</script>
</asp:Content>
