<%@ Page Title="" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="Stipend_process.aspx.cs" Inherits="SigmaERP.attendance.Stipend_process" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .rowx{
            display:grid;
            grid-template-columns:auto;
            width:60%;
            padding:30px;
            margin:30px;
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
        .card-center{
            display:flex;
            justify-content:center;
            align-items:center;
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
                    <li><a href="#" class="ds_negevation_inactive Mactive">Stipend Process</a></li>
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
                        <asp:AsyncPostBackTrigger ControlID="btnProcess"/>
                    
                      
                       
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
            <asp:Label runat="server" ID="lblformdate" CssClass="lebel">From Date:</asp:Label>
             <asp:TextBox runat="server" ID="txtFormdate" type="date"></asp:TextBox>
        </div>
         <div class="col-4">
            <asp:Label runat="server" ID="lbltodate" CssClass="lebel">To Date:</asp:Label>
             <asp:TextBox runat="server" ID="txttodate" type="date"></asp:TextBox>
        </div>
        <asp:Button  runat="server" ID="btnProcess" CssClass="btn btn-success" Text="Process" OnClick="btnProcess_Click"/>
    </div>
    </div>
                    </ContentTemplate>
            </asp:UpdatePanel>
   
    

 

   

</asp:Content>
