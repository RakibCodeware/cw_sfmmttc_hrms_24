<%@ Page Title="" Language="C#" MasterPageFile="~/personnel_NestedMaster.master" AutoEventWireup="true" CodeBehind="employee-list-allowing-compliance.aspx.cs" Inherits="SigmaERP.personnel.employee_list_allowing_compliance" %>

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
        #gvForApprovedList  th,#gvForApprovedList td {
            padding:3px;
        }
        #gvForApprovedList th:first-child{
            text-align:center;
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
                    <li> <a href="<%= Session["__topMenuForPersonnel__"] %>">Personnel</a></li>
                    <li><a href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Ptactive">Pending Worker List</a></li>
                </ul>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="uplMessage" runat="server">
        <ContentTemplate>
            <p class="message" id="lblMessage" clientidmode="Static" runat="server"></p>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div style="padding: 0; margin-top: 25px; max-width: 100%;">
        <div class="row Rrow">
            <div style="width: 100%; background: #750000 none repeat scroll 0 0 !important;" class="list_main_content_box_header personal_color_header" id="divElementContainer" runat="server">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <Triggers>
                    </Triggers>
                    <ContentTemplate>
                        <div style="overflow: hidden; margin-bottom: 5px; border-bottom: 1px solid #ddd;">
                            <h3 class="emp_header_left">
                            <p style="font-size: 20px; text-align: center; font-weight: 500; text-shadow: 5px 5px 5px #000;">Pending Worker List</p>
                        </div>
                        <div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <asp:UpdatePanel runat="server" ID="up2">
                <Triggers>
                </Triggers>
                <ContentTemplate>
                    <div style="width: 100%; background-color: #fff; margin: auto;">
                        <asp:GridView HeaderStyle-BackColor="#750000" ID="gvForApprovedList" ClientIDMode="Static" runat="server" AutoGenerateColumns="false" HeaderStyle-ForeColor="White" HeaderStyle-Font-Bold="true" HeaderStyle-Height="25px" HeaderStyle-Font-Size="14px" Width="100%" DataKeyNames="EmpId,CompanyId" OnRowCommand="gvForApprovedList_RowCommand" OnRowDataBound="gvForApprovedList_RowDataBound">
                            <Columns>
                                <asp:TemplateField HeaderText="SL">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="EmpCardNo" HeaderText="Card No (Reg.)" />
                                <asp:BoundField DataField="EmpName" HeaderText="Name" />
                                <asp:BoundField DataField="EmpJoiningDate" HeaderText="Join Date"  />
                                <asp:BoundField DataField="DptName" HeaderText="Department"  />
                                <asp:BoundField DataField="DsgName" HeaderText="Designation"  />
                                <asp:BoundField DataField="EmpType" HeaderText="Type" />
                                
                             <%--   <asp:TemplateField HeaderText="Change" HeaderStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Button ID="btnEdit" runat="server" CommandName="edit" Width="55px" Height="30px" Font-Bold="true" ForeColor="green" Text="Edit" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="Allowing" HeaderStyle-Width="30px">
                                    <ItemTemplate>
                                        <asp:Button ID="btnAllow" runat="server" CommandName="allow" Font-Bold="true" ForeColor="Blue" Text="Allow to Compliance"  Height="30px" OnClientClick="return confirm('Do you want to allow this Worker ?')" CommandArgument='<%#((GridViewRow)Container).RowIndex%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $(document).on("keypress", "body", function (e) {
                if (e.keyCode == 13) e.preventDefault();
                // alert('deafault prevented');

            });
        });
    </script>
</asp:Content>
