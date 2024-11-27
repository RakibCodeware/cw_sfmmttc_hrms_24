<%@ Page Title="" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="Stipend_Report.aspx.cs" Inherits="SigmaERP.attendance.Stipend_Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
       <style>
        .rowx{
            /*display:grid;
      
            width:60%;
            padding:30px;
            margin:30px;*/
            display:flex;
            column-gap:8px;
            margin-top:5px;
        }
        .py-5{
            padding: 30px 0;
        }

        .rowx input, textarea, select, button {
    outline: none !important;
    height: 40px;
    width: 100%;
    margin: 6px 0;
}
     
        .lebel{
            width:100px !important;
            display: inline-block;
        }
        .hlebel{
            opacity:0;
        }
        .card-center{
            display:flex;
            /*justify-content:center;
            align-items:center;*/
        }
        th {
    background: green;
    color: white;
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
                    <li><a href="#" class="ds_negevation_inactive Mactive">Stipend Sheet</a></li>
                </ul>
            </div>
        </div>
    </div>

      <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>

        <asp:UpdatePanel ID="uplMessage" runat="server" >
    <ContentTemplate><p class="message"  id="lblMessage" clientidmode="Static" runat="server"></p></ContentTemplate>
</asp:UpdatePanel>
       <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnPreview" />
                        <asp:PostBackTrigger ControlID="btnExport" />
                      
                       
                    </Triggers>
                    <ContentTemplate>
    <div class="card-center">
        <div class="rowx">
            <div class="col-4">
                <asp:Label runat="server" ID="lblCompanyname" CssClass="lebel">Company Name</asp:Label>
                <asp:DropDownList runat="server" ID="ddlCompany"></asp:DropDownList>
            </div>

            <div class="col-4">
                <asp:Label runat="server" ID="lblCoursename" CssClass="lebel">Course Name</asp:Label>
                <asp:DropDownList runat="server" ID="ddlCourseList"></asp:DropDownList>
            </div>
            <div class="col-4">
                 <asp:Label runat="server" ID="Label1" CssClass="hlebel">Course Name</asp:Label>
                 <asp:Button  runat="server" ID="btnPreview" OnClick="btnPreview_Click" CssClass="btn btn-success" Text="Preview"/>             
            </div>
            <div class="col-4">
                 <asp:Label runat="server" ID="Label2" CssClass="hlebel">Course Name</asp:Label>
                   <asp:Button runat="server"  ID="btnExport" OnClick="btnExport_Click" Text="EXPORT" CssClass="btn btn-success"/>
            </div>
        </div>
    </div>


                           <div>
        <asp:GridView runat="server" ID="gvstipendList" AutoGenerateColumns="false" CssClass="table">
            <Columns>
                <asp:TemplateField HeaderText="SL">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="StudentId" HeaderText="ID" Visible="false" />
                <asp:BoundField DataField="EmpName" HeaderText="Student Name" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="DptName" HeaderText="Course Name" />
                <asp:BoundField DataField="TotalMonths" HeaderText="Total Months" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="PresentDays" HeaderText="Present Days" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="Amount" HeaderText="Amount" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" ItemStyle-Font-Bold="true" />
                <asp:BoundField DataField="Signature" HeaderText="Signature" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" />

            </Columns>
        </asp:GridView>
    </div>
   </ContentTemplate>
           </asp:UpdatePanel>

 
</asp:Content>
