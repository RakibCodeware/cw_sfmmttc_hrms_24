<%@ Page Title="Separation Generation" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="separation_generation_rss.aspx.cs" Inherits="SigmaERP.payroll.separation_generation_rss" %>
<%@ Register Assembly="ComplexScriptingWebControl" Namespace="ComplexScriptingWebControl" TagPrefix="cc1" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" ></asp:ScriptManager>   
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
                       <li> <a href="#" class="ds_negevation_inactive Pactive">Separation Generation</a></li>
                   </ul>               
             </div>          
             </div>
       </div>
      <div class="main_box Mbox">
        <div class="main_box_header PBoxheader">
            <h2>Separation Generation</h2>
        </div>
    	<div class="main_box_body Pbody">
            <div class="main_box_content">
                <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="rblGenerateType" />
                        <asp:AsyncPostBackTrigger ControlID="btnGeneration" />
                        <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />
                        <asp:AsyncPostBackTrigger ControlID="ddlEmpCardNo" />
                        <asp:AsyncPostBackTrigger ControlID="ddlMonthID" />
                    </Triggers>
                    <ContentTemplate>
                          <div class="bonus_generation" style="width: 61%; margin: 0px auto;">     
                           <table runat="server" visible="true" id="tblGenerateType" class="division_table_leave1"> 
                                <tr>
                                      <td>
                                          Company
                                      </td>
                                        <td collspan="2">
                                            <asp:DropDownList ID="ddlCompanyList" runat="server" AutoPostBack="true" CssClass="form-control select_width" ClientIDMode="Static" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged" ></asp:DropDownList>
                                        </td>                         
                                    </tr>
                                <tr>
                                    <td>Select Month </td>                                       
                                    <td>
                                        <asp:DropDownList ID="ddlMonthID" ClientIDMode="Static" CssClass="form-control select_width" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlMonthID_SelectedIndexChanged"></asp:DropDownList>
                                    </td>
                                    <td>Type</td>
                                    <td>
                                    <asp:RadioButtonList ID="rblGenerateType" runat="server" ClientIDMode="Static" RepeatDirection="Horizontal" RepeatLayout="Flow" Onchange="radio_button_indaxChanged();" >
                                        <asp:ListItem Selected="True" Text="Full" Value="0"></asp:ListItem>
                                        <asp:ListItem Selected="False" Text="Partial" Value="1"></asp:ListItem>
                                    </asp:RadioButtonList>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>   
                                        <td>
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbTiffinBill" Text="Tiffin Bill" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbNightBill" Text="Night Bill" Checked="true" />
                                            <asp:CheckBox runat="server" ClientIDMode="Static" ID="ckbHolidayAllow" Text="Holiday Allow. " Checked="true" />
                                        </td>                                    
                                        

                                    </tr>
                                <tr>
                                    <td>
                                        Card No
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="ddlEmpCardNo" runat="server" ClientIDMode="Static"  CssClass="form-control select_width" Enabled="False" onChange="ddlEmpCardNo_ChangedItem();" ></asp:DropDownList>
                                    </td>
                                    <td></td>
                                    <td>
                                        <asp:TextBox ID="txtEmpCardNo" ClientIDMode="Static" runat="server" CssClass="form-control text_box_width" PLaceHolder="Type or Select Card No" Enabled="False" EnableTheming="True"></asp:TextBox>
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
                    </ContentTemplate>
                </asp:UpdatePanel>
                <div class="payroll_generation_button">                  
                    <asp:Button ID="btnGeneration" CssClass="Pbutton" runat="server" OnClientClick="return processing();"  OnClick="btnGeneration_Click"  Text="Generation" />
               <%--      <input  type="button" class="Pbutton" id="generate" value="Generation" Visible="false"/>--%>
                    <asp:Button ID="Button3" runat="server" Text="Close" PostBackUrl="~/payroll_default.aspx" CssClass="Pbutton" />                   
                 </div>
                <div id="progressbar" style="width:100%;position:relative;"></div>
                <asp:Label ID="lblM" runat="server" ClientIDMode="Static" Text="" style=" font-weight:bold; position: absolute; margin-top: -23px; margin-left: 22%;"  ></asp:Label>
                <p ID="lblErrorMessage" style="color:red;display:none">iiii</p>
            </div>
        </div>
    </div>
      <script type="text/javascript">
            $(document).keyup(function (e) {
           if (e.keyCode == 79) {
               goToNewTabandWindow('/payroll/separation_generation.aspx');
           }
          });

          $('#imgLoading').hide(); 
          $(document).ready(function () {              
              load();           

          });
        
          function load() {
              $("#ddlEmpCardNo").select2();              
          }     
        function InputValidationBasket() {
            try {
                if ($('#ddlMonthID').val().trim().length <= 1) {
                    showMessage('Please select seperation generation month', 'error');
                    $('#ddlMonthID').focus(); return false;
                }
                return true;
            }
            catch (exception) {
                showMessage(exception, error)
            }
        }
        function InputValidationBasket2() {
            try {
                if ($('#ddlMonthID').val().trim().length <= 1) {
                    showMessage('Please select seperation generation month', 'error');
                    $('#ddlMonthID').focus(); return false;
                }
                else if ($('#txtEmpCardNo').val().trim().length < 4)
                {
                    showMessage('Please select or type valid card no', 'error');
                    $('#txtEmpCardNo').focus(); return false;
                }
                return true;
            }
            catch (exception) {
                showMessage(exception, error)
            }
        }
        function radio_button_indaxChanged() {
          
            var sradio = $('#rblGenerateType input:checked');
            if (sradio.val() == "0") {
                $('#ddlEmpCardNo').attr('disabled', true);
                $('#txtEmpCardNo').attr('disabled',true);
            }
            else {
                $('#ddlEmpCardNo').attr('disabled', false);
                $('#txtEmpCardNo').attr('disabled',false);
            }
            $('#imgLoading').hide();
        }

        function ddlEmpCardNo_ChangedItem()
        {
            try
            {
                var getEmpCardNo = $('#ddlEmpCardNo option:selected').text().split(' ');
                $('#txtEmpCardNo').val(getEmpCardNo[0]);
            }
            catch (exception)
            { }
        }
        function processing() {         
            $('#imgLoading').show();
            return true;
        }

        function alertMessage() {
           
            setTimeout(function () {
                $('#lblErrorMessage').css('display', 'block');
                $('#lblErrorMessage').fadeOut("slow", function () {
                    $('#lblErrorMessage').remove();
                    $('#lblErrorMessage').val('');
                });

            }, 10000);
        }
        function ProcessingEnd(total) {
            showMessage("Successfully separation generated of " + total + "","success");
            $('#imgLoading').hide();
        }
        function ProcessingEror(total) {
            showMessage("Unable to separation generate " + total + "","error");
            $('#imgLoading').hide();
        }
          function MessageBox(msg) {
           showMessage(msg);
           $('#imgLoading').hide();
           load();
       }
          function goToNewTabandWindow(url) {
              window.open(url);
          
       }
    </script>

</asp:Content>

