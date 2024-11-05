<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="roster-missing-log.aspx.cs" Inherits="SigmaERP.roster_missing_log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <div style="width: 100%; background-color: #fff; margin: auto;">
                <asp:GridView HeaderStyle-BackColor="#750000" ID="gvEmpList" runat="server" AutoGenerateColumns="false" HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true" HeaderStyle-Height="25px" HeaderStyle-Font-Size="14px" Width="100%" DataKeyNames="EmpId">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>SL</HeaderTemplate>
                            <ItemTemplate>
                                <%# Container.DataItemIndex+1 %>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" ForeColor="Red" Font-Bold="true" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="EmpCardNo" HeaderText="CardNo" ItemStyle-HorizontalAlign="Center" ItemStyle-Height="21px" />
                        <asp:BoundField DataField="EmpName" HeaderText="Name" />
                        <asp:BoundField DataField="DsgName" HeaderText="Designation" />
                        <asp:BoundField DataField="Date" HeaderText="Date" />
                        <asp:BoundField DataField="SftName" HeaderText="Roster" />
                        <asp:BoundField DataField="InsertTime" HeaderText="Entry Time" />

                    </Columns>
                </asp:GridView>
            </div>

        </div>
    </form>
</body>
</html>
