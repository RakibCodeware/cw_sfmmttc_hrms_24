<%@ Page Title="Salary Generation" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="Payroll_generation_rss.aspx.cs" Inherits="SigmaERP.payroll.Payroll_generation_rss" %>
<%@ Register Assembly="ComplexScriptingWebControl" Namespace="ComplexScriptingWebControl" TagPrefix="cc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="false" ></asp:ScriptManager>
    <asp:UpdatePanel ID="uplMessage" runat="server">
        <ContentTemplate>
            <p class="message" id="lblMessage" clientidmode="Static" runat="server"></p>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="row">
                  <div class="col-md-12 ds_nagevation_bar">
               <div style="margin-top: 5px">
                   <ul>
                       <li><a href="/default.aspx">Dashboard</a></li>
                       <li> <a class="seperator" href="#"></a>/</li>
                        <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li> 
                       <li> <a class="seperator" href="#"></a>/</li>                  
                        <li> <a href="/payroll/salary_index.aspx">Salary</a></li>
                        <li> <a class="seperator" href="#"></a>/</li>
                       <li> <a href="#" class="ds_negevation_inactive Pactive">Payroll Generation</a></li>
                   </ul>               
             </div>
          
             </div>
       </div>

    <div class="main_box Mbox">
        <div class="main_box_header PBoxheader">
            <h2 >Salary Generation (Actual)</h2>
        </div>
    	<div class="main_box_body Pbody">
            <div class="main_box_content">
                <asp:UpdatePanel ID="up2" runat="server" UpdateMode="Conditional">
                        <Triggers>                           
                            <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />
                            <asp:AsyncPostBackTrigger ControlID="ddlShiftList" />
                           <%-- <asp:AsyncPostBackTrigger ControlID="btnGenerate" />--%>
                            <asp:PostBackTrigger ControlID="btnGenerate" />
                            <asp:AsyncPostBackTrigger ControlID="ddlEmpCardNo" />                           
                        </Triggers>
                        <ContentTemplate>
                           <div class="bonus_generation" style="width: 66%; margin: 0px auto;">   
                              <center>
                                 <asp:RadioButtonList ID="rbGenaratingType" runat="server"  AutoPostBack="false" ClientIDMode="Static" onChange="EnableControl(this);" RepeatDirection="Horizontal" Width="274px">
                                                <asp:ListItem Selected="True" Value="0">Full Generate</asp:ListItem>
                                                <asp:ListItem Value="1">Partial Generate</asp:ListItem>
                                            </asp:RadioButtonList>
                               </center>
                                <table runat="server" visible="true" id="tblGenerateType"  style="width:60%; margin:0px auto"> 
                                    <tr>                          
                                        <td>
                                            <asp:DropDownList Visible="false" ID="ddlShiftList" runat="server" AutoPostBack="true" CssClass="form-control select_width" ClientIDMode="Static" >
                                            </asp:DropDownList>
                                        </td>

                                    </tr>
                                    <tr>
                                         <td>
                                            Company
                                        </td>
                                      
                                        <td  >
                                            <asp:DropDownList ID="ddlCompanyList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged" CssClass="form-control select_width" ClientIDMode="Static"  ></asp:DropDownList>
                                        </td>
                                       
                                    </tr>
                                    <tr>
                                        <td>Wages Generate Month
                                        </td>
                                        <td>
                                            <asp:TextBox CssClass="form-control text_box_width"  ClientIDMode="Static" ID="txtGenerateMonth" runat="server" autocomplete="off"></asp:TextBox>
                                            <asp:CalendarExtender ID="txtGenerateMonth_CalendarExtender" Format="dd-MM-yyyy" runat="server" TargetControlID="txtGenerateMonth">
                                            </asp:CalendarExtender>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtEmpCardNo" ClientIDMode="Static" Enabled="false" runat="server" CssClass="form-control text_box_width" PLaceHolder="Type or Select Card No" EnableTheming="True"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:DropDownList ID="ddlEmpCardNo" runat="server" Enabled="false" ClientIDMode="Static" Width="310px"  CssClass="form-control select_width" onChange="getCardNo()"></asp:DropDownList>
                                        </td>
                                    </tr>
                                      <tr>
                                        <td>
                                            Excepted Card No
                                        </td>
                                        <td>
                                           <asp:TextBox ID="txtExceptedEmpCardNo"  ClientIDMode="Static" runat="server" CssClass="form-control text_box_width" PLaceHolder="990001,990002,990003,......n" autocomplete="off"></asp:TextBox>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td colspan="3">
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbPF" Text="PF" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbTiffinBill" Text="Tiffin Bill" Checked="true" />
                                       <%-- </td>
                                        <td>--%>
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbNightBill" Text="Night Bill" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbAttendanceBonus" Text="Attendance Bonus" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbHolidayAllow" Text="Holiday Allowance" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbLateDeduction" Text="Late Deduction" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbAdvanceDeduction" Text="Advance Deduction" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbStampDeduction" Text="Stamp Deduction" Checked="true" />
                                        </td>
                                    </tr>
                                      <tr>
                                        <td colspan="3">
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbSpecialGrossPer" Text="Special Gross (%)" Checked="false" />
                                       <asp:TextBox runat="server" ID="txtSpecialGrossPer" placeholder="%" CssClass="form-control text_box_width" Text="60"></asp:TextBox>
                                        </td>
                                    </tr>

                                          <tr id="Tr1" runat="server" visible="false">
                                        <td>
                                            Tiffin not count (Card no) 
                                        </td>
                                        <td>
                                           <asp:TextBox runat="server" ID="txtNotTiffinCardno" placeholder="0000,0001,......,nnnn" CssClass="form-control text_box_width"></asp:TextBox>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td>
                                           
                                        </td>
                                        <td>
                                            <asp:Image ID="imgLoading" runat="server" ImageUrl="~/images/loading.gif" ClientIDMode="Static"  />
                                        </td>
                                    </tr>
                                </table>
  
                            </div>
                            <br />
                            <div  style="width: 61%; margin: 0px auto; overflow: hidden">
                                <div style="width: 50%; margin: 0px auto; overflow: hidden">
                                    <asp:Button ID="btnGenerate" CssClass="Pbutton" ClientIDMode="Static" runat="server" Text="Generate" OnClientClick="return processing();" Style="float: left" OnClick="btnGenerate_Click" />
                                    
                                    <asp:Button ID="Button3" runat="server" Text="Close" PostBackUrl="~/payroll_default.aspx" CssClass="Pbutton" Style="float: left" />
                                    <asp:Button ID="btnBDTNoteGenerate" runat="server" Text="BDT Note Generate" CssClass="Pbutton" Style="float: left; width: 166px;" OnClientClick="return validation();" OnClick="btnBDTNoteGenerate_Click"  /><br />
                                </div>
                            </div>
                            <br />
                        <hr />
                            <br />
                     <div id="progressbar" style="width:100%;position:relative;"></div>
                     <asp:Label ID="lblM" runat="server" ClientIDMode="Static" Text="" style=" font-weight:bold; position: absolute; margin-top: -23px; margin-left: 22%;"  ></asp:Label>

                         <cc1:ProgressBar runat="server" ID="ProgressBar1" />
                            <div class="bonus_generation" style="width: 66%; margin: 0px auto;">   
                            <asp:GridView runat="server" ID="gvSalaryList" CssClass="gvdisplay1" DataKeyNames="ToDate" AutoGenerateColumns="false" HeaderStyle-BackColor="#ffa500" HeaderStyle-Height="28px" HeaderStyle-ForeColor="White" Width="100%" OnRowCommand="gvSalaryList_RowCommand">
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <HeaderTemplate>SL</HeaderTemplate>
                                                    <ItemTemplate >
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="MonthYear" HeaderText="Month" />
                                                 <asp:BoundField DataField="FromDate" HeaderText="From Date" />
                                                 <asp:BoundField DataField="ToDate" HeaderText="To Date" />                                 
                                                 <asp:BoundField DataField="Worker" HeaderText="Worker" />                                  
                                                 <asp:BoundField DataField="Staff" HeaderText="Staff" />                                    
                                                 <asp:BoundField DataField="Total" HeaderText="Total" />                                   
                                                 <asp:TemplateField HeaderText="Delete"  HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                                  <ItemTemplate >
                                      <asp:Button ID="btnRemove" runat="server" CommandName="Remove" Width="55px" Height="30px" Font-Bold="true" ForeColor="red" Text="Delete" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' />
                                  </ItemTemplate>
                              </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                </div>
                     </ContentTemplate>
                    </asp:UpdatePanel> 
            </div>
        </div>
    </div>

   <script type="text/javascript">

       $(document).ready(function () {

           $(document).on("keypress", "body", function (e) {
               if (e.keyCode == 13) e.preventDefault();
               // alert('deafault prevented');

           });
           $("#ddlEmpCardNo").select2();
       });
       $(document).keyup(function (e) {
           if (e.keyCode == 79) {
               goToNewTabandWindow('/payroll/payroll_generation1.aspx');
           }
       });
       function load() {
           $("#ddlEmpCardNo").select2();
           $('#imgLoading').hide();
       }
       $('#imgLoading').hide();
    function getCardNo() {

        var e = document.getElementById('ddlEmpCardNo');
        var text = e.options[e.selectedIndex].text;
        var splitValue = text.split(' ');

        document.getElementById('txtEmpCardNo').value = splitValue[0];

    }
    function indivisualvalidation()
    {
        if (validation() == false) return false;
        if ($('#ddlEmpCardNo option:selected').text().length == 0) {
            showMessage("warning->Please Select Card No ");
            $('#ddlEmpCardNo').focus();
            return false;
        }
        return true;
    }
    function validation()
    {
        if ($('#txtGenerateMonth').val().trim().length == 0) {
            showMessage("warning->Please Select Date ");
            $('#txtGenerateMonth').focus();
            return false;
        }
    }
    function EnableControl(e)
    {
        var checked_radio = $("[id*=rbGenaratingType] input:checked");

        if (checked_radio.val() == 0) {
            $("#txtEmpCardNo").prop("disabled", true);
            $("#ddlEmpCardNo").prop("disabled", true);
         
        }
        else {
            $("#txtEmpCardNo").prop("disabled", false);
            $("#ddlEmpCardNo").prop("disabled", false);            
          
        }
    }
    function processing()
    {
        var checked_radio = $("[id*=rbGenaratingType] input:checked");

        if (checked_radio.val() == 0) {
            if (validation() == false) return false;
        }
        else
        {
           if (indivisualvalidation() == false) return false;
        }         
        $('#imgLoading').show();
        return true;
    }
    function ProcessingEnd(total) {
        showMessage("success->Successfully payroll generated of " + total + "");
        $('#imgLoading').hide();
        load();
    }
    function ProcessingEror(total) {
        showMessage("error->" + total + "");
        $('#imgLoading').hide();
        load();
    }
    function goToNewTabandWindow(url) {       
        window.open(url);
        ProcessingEnd();
    }

</script>

</asp:Content>
