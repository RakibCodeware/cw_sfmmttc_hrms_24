<%@ Page Title="Auto Increment Panel" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="AutoIncrementPanel.aspx.cs" Inherits="SigmaERP.payroll.AutoIncrementPanel" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .txtAllow {
            width:100%;
            
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="false"></asp:ScriptManager>
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
                    <li><a class="seperator" href="#"></a>/</li>
                    <li><a href="/payroll_default.aspx">Payroll</a></li>
                    <li><a class="seperator" href="#"></a>/</li>
                    <li><a href="/payroll/salary_index.aspx">Salary</a></li>
                    <li><a class="seperator" href="#"></a>/</li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Auto Increment Panel</a></li>
                </ul>
            </div>
        </div>
    </div>
    <div class="main_box Lbox">
        <div class="main_box_header PBoxheader">
            <h2>Auto Increment Panel</h2>
        </div>
        <div class="main_box_body Pbody">
            <div class="main_box_content">
                <asp:UpdatePanel ID="up2" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSearch" />
                        <asp:AsyncPostBackTrigger ControlID="gvList" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="bonus_generation" style="width:500px; margin: 0px auto;">                         
                                <table runat="server"  style="width:90%; margin:0px auto">                            
                                    <tr>
                                        <td>Effective Month
                                        </td>
                                        <td>
                                            :
                                        </td>
                                        <td>
                                            <asp:TextBox CssClass="form-control text_box_width"  ClientIDMode="Static" ID="txtEffectiveMonth" runat="server" autocomplete="off"></asp:TextBox>
                                            <asp:CalendarExtender ID="txtGenerateMonth_CalendarExtender" Format="MM-yyyy" runat="server" TargetControlID="txtEffectiveMonth">
                                            </asp:CalendarExtender>
                                        </td>
                                        <td>
                                            <asp:Button ID="btnSearch" runat="server" CssClass="Pbutton" Text="Search" OnClick="btnSearch_Click" />
                                        </td>
                                    </tr>                                    
                                </table>  
                            </div>
                        <div class="bonus_generation" style="margin: 0px auto;">
                            <asp:GridView runat="server" ID="gvList" CssClass="gvdisplay1" DataKeyNames="EmpId" AutoGenerateColumns="false" HeaderStyle-BackColor="#ffa500" HeaderStyle-Height="28px" HeaderStyle-ForeColor="White" Width="100%" OnRowCommand="gvList_RowCommand">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                        <HeaderTemplate>SL</HeaderTemplate>
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="EmpCardNo" HeaderText="Card No" />
                                    <asp:BoundField DataField="EmpName" HeaderText="Name" />
                                    <asp:BoundField DataField="DsgName" HeaderText="Designation" />
                                    <asp:BoundField DataField="DptName" HeaderText="Department" />
                                    <asp:BoundField DataField="EmpJoiningDate" HeaderText="Joining Date" />
                                    <asp:BoundField DataField="AutoIncrementMonth" HeaderText="Last Auto Increment Month" />
                                    <asp:BoundField DataField="EmpPresentSalary" HeaderText="Current Gross" />
                                    <asp:TemplateField HeaderText="Increment Amount(5%)" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false" CssClass="txtAllow" ClientIDMode="Static" ID="txtIncrementAmount" Text='<%# Eval("IncrementAmount")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="New Gross" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false" CssClass="txtAllow" ClientIDMode="Static" ID="txtNewGross" Text='<%# Eval("newGross")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="New Basic" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false" CssClass="txtAllow" ClientIDMode="Static" ID="txtNewBasic" Text='<%# Eval("newBasic")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="New Medical" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false" CssClass="txtAllow"  ClientIDMode="Static" ID="txtNewMedical" Text='<%# Eval("newMedical")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="New Food" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false" CssClass="txtAllow" ClientIDMode="Static" ID="txtNewFoodAllowance" Text='<%# Eval("newFood")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="New Conveyance" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false" CssClass="txtAllow" ClientIDMode="Static" ID="txtNewConveyance" Text='<%# Eval("newConveyance")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="New House Rent" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false" CssClass="txtAllow" ClientIDMode="Static" ID="txtNewHouseRent" Text='<%# Eval("newHouseRent")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Effective From" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" Enabled="false"  ClientIDMode="Static" ID="txtEffectiveFrom" Text='<%# Eval("IncrementMonth")%>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Submit"  HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Button ID="btnSubmit" runat="server" CommandName="Submit" Width="55px" Height="30px" Font-Bold="true" ForeColor="Green" Text="Submit" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' />
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
</script>
</asp:Content>
