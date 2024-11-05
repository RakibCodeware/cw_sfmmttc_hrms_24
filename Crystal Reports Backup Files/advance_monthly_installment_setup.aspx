<%@ Page Title="Monthly Installment Setup" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="advance_monthly_installment_setup.aspx.cs" Inherits="SigmaERP.payroll.advance.advance_monthly_installment_setup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        #ContentPlaceHolder1_ContentPlaceHolder1_gvAdvaceList th, td {
            text-align:center;
        }
           #ContentPlaceHolder1_ContentPlaceHolder1_gvAdvaceList th:nth-child(3), td:nth-child(3) {
            text-align:left;
            padding-left:3px;
        }
          
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div class="row">
        <div class="col-md-12">
            <div class="ds_nagevation_bar">
                <ul>
                    <li><a href="/default.aspx">Dasboard</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="/payroll_default.aspx">Payroll</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                   <li><a href="<%= Session["__topMenuAdvance__"] %>">Advance</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Advance Monthly Installment Setup</a></li>
                </ul>
            </div>
        </div>
    </div>
<asp:UpdatePanel ID="uplMessage" runat="server" >
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>
    <div class="main_box Mbox">
        <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
        <Triggers>
           <%-- <asp:AsyncPostBackTrigger ControlID="btnSearch" />--%>
            <asp:AsyncPostBackTrigger ControlID="ddlCompanyList" />
           
        </Triggers>
        <ContentTemplate>
             <div class="main_box_header PBoxheader">
            <h2>
                <asp:Label runat="server" ID="lblTtile"></asp:Label>
            </h2>
        </div>

        <div class="main_box_body Pbody">
            <div class="main_box_content">
              <div class="bonus_generation" style="width: 61%; margin: 0px auto; overflow:hidden">
                  <center>
                      <asp:RadioButtonList runat="server" ID="rblEmpType" AutoPostBack="true" RepeatDirection="Horizontal">
                                </asp:RadioButtonList>
                  </center>
            <table style="margin:0px auto">
                <tr>
                    <td>
                        <asp:UpdateProgress ID="UpdateProgress1" runat="server" style="margin-top:-23px;" >
                                    <ProgressTemplate>
                                        <span style=" font-family:'Times New Roman'; font-size:20px; color:green;font-weight:bold;float:left;margin-top: 19px"><p>Wait processing</p> </span> 
                                        <img style="width:26px;height:26px;cursor:pointer; float:left;margin-top: 19px;margin-left: 7px;" src="/images/wait.gif"  />  
                                    </ProgressTemplate>
                                </asp:UpdateProgress> <br>
                    </td>
                   <td>Company :</td>
                    <td>
                        <asp:DropDownList ID="ddlCompanyList" runat="server" AutoPostBack="true" CssClass="form-control select_width" OnSelectedIndexChanged="ddlCompanyList_SelectedIndexChanged"  ></asp:DropDownList>
                    </td>
                    <td>Month :</td>
                   <td>
                       <asp:TextBox ID="txtDate" runat="server" style=" text-align:center;font-weight:bold;color:red" ClientIDMode="Static" CssClass="form-control text_box_width" Width="85px"></asp:TextBox>
                       <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtDate" Format="MM-yyyy" runat="server"></asp:CalendarExtender>
                   </td>
                    <td>
                        <asp:Button id="btnSearch" runat="server" Text="Search" CssClass="Pbutton" style=" float:right; width:80px;" OnClick="btnSearch_Click" /><br />
                    </td>

                    <td>
                        <asp:Button id="btnSet" runat="server" Text="Set" CssClass="Pbutton" style=" float:right; width:80px;" OnClick="btnSet_Click"/><br />
                    </td>
                </tr>
            </table>
                  </div>
              <br />
         
           <%-- <br />
            <br />--%>
               <hr />
            <br />
              <asp:GridView ID="gvAdvaceList" runat="server" AutoGenerateColumns="false" HeaderStyle-Height="26px" HeaderStyle-Font-Bold="false" HeaderStyle-ForeColor="White" HeaderStyle-BackColor="#ffa500" DataKeyNames="SL,LoanID,EmpID,CompanyID,MonthID" Width="100%" OnRowCommand="gvAdvaceList_RowCommand" OnRowDataBound="gvAdvaceList_RowDataBound" ShowFooter="true">
            <Columns>
                <asp:TemplateField HeaderText="SL">
                                <ItemTemplate>
                                     <%# Container.DataItemIndex + 1 %>                                  
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>                
                <asp:BoundField DataField="EmpCardNo" HeaderText="EmpCardNo" ItemStyle-Width="100px" itemstyle-horizontalalign="center" ItemStyle-Height="26px" />
                <asp:BoundField DataField="EmpName" HeaderText="Name"  itemstyle-horizontalalign="center" ItemStyle-Height="26px" />   
                <asp:BoundField DataField="EmpType" HeaderText="EmpType" Visible="true" itemstyle-horizontalalign="center" />
                <asp:BoundField DataField="LoanAmount" HeaderText="Loan"  ItemStyle-Width="100px" itemstyle-horizontalalign="center" />
                <asp:BoundField DataField="DeductFrom" HeaderText="Start Month" Visible="true" ItemStyle-Width="100px" itemstyle-horizontalalign="center" />
                <asp:BoundField DataField="PaidInstallmentNo" HeaderText="Paid Ins. No"  ItemStyle-Width="100px" itemstyle-horizontalalign="center" />
                <asp:BoundField DataField="PaidAmount" HeaderText="Paid"  ItemStyle-Width="100px" itemstyle-horizontalalign="center" />
                <asp:TemplateField HeaderText="Due">
                    <ItemTemplate>
                        <asp:Label ID="lblDueAmount" runat="server" Enabled="false" Text='<%# Eval("DueAmount")%>' Font-Bold="true" ForeColor="red" style="text-align: center;"  ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>   

                <asp:BoundField DataField="DefaultInstallmentAmount" HeaderText="Def. Ins. Amount"  ItemStyle-Width="100px" itemstyle-horizontalalign="center" />   
                <asp:BoundField DataField="Month" HeaderText="Cut. Month"  ItemStyle-Width="100px" itemstyle-horizontalalign="center" />
                <asp:TemplateField HeaderText="Cut Ins. Amount">
                    <ItemTemplate>
                        <asp:TextBox ID="txtInstallmentAmount" runat="server" Enabled="false" Text='<%# Eval("InstallmentAmount")%>' Font-Bold="true" ForeColor="black" style="text-align: center; margin-left: 4px; autocomplete:off"  ></asp:TextBox>
                    </ItemTemplate>
                    <FooterTemplate>  
                        <asp:Label ID="lblTotalInstallmentAmount" Font-Bold="true" ForeColor="Black" Text="0" runat="server" />  
                    </FooterTemplate>
                </asp:TemplateField>         
                 <asp:TemplateField HeaderText="Choose" ItemStyle-Width="100px" itemstyle-horizontalalign="center">
                    <ItemTemplate>
                    <asp:CheckBox ID="SelectCheckBox" runat="server" ItemStyle-Width="100px" Checked="true" OnCheckedChanged="SelectCheckBox_CheckedChanged"  AutoPostBack="true" />   
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Alter" >
                   <ItemTemplate>
                       <asp:Button ID="btnEdit" Text="Edit" runat="server" CommandName="Alter" CommandArgument="<%#((GridViewRow)Container).RowIndex %>" Height="27px" Width="66px" Font-Bold="true" ForeColor="Green"  /> 
                   </ItemTemplate>
                </asp:TemplateField>

            </Columns>
                 
        </asp:GridView>     
                    </div>
            </div>
             </ContentTemplate>
    </asp:UpdatePanel>

                    </div>
            



</asp:Content>
