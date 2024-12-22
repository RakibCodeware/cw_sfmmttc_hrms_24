<%@ Page Title="" Language="C#" MasterPageFile="~/attendance_nested.master" AutoEventWireup="true" CodeBehind="Stipend_process.aspx.cs" Inherits="SigmaERP.attendance.Stipend_process" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .rowx{
           display:flex;
          gap:5px;
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
               <asp:DropDownList runat="server" CssClass="form-control" ID="ddlCompany"></asp:DropDownList>
        </div>

        <div class="col-4">
            <asp:Label runat="server" ID="lblCoursename" CssClass="lebel">Course Name</asp:Label>
             <asp:DropDownList runat="server" CssClass="form-control" ID="ddlCourseList"></asp:DropDownList>
        </div>
          <div class="col-4">
            <asp:Label runat="server" ID="lblformdate" CssClass="lebel">From Date:</asp:Label>
             <asp:TextBox runat="server" CssClass="form-control" ID="txtFormdate" type="date"></asp:TextBox>
        </div>
         <div class="col-4">
            <asp:Label runat="server" ID="lbltodate" CssClass="lebel">To Date:</asp:Label>
             <asp:TextBox runat="server" CssClass="form-control" ID="txttodate" type="date"></asp:TextBox>
        </div>
       
    </div>
           <asp:Button  runat="server" ID="btnProcess" CssClass="btn btn-success" style="margin-top:12px; margin-left:5px;" Text="Preview" OnClick="btnProcess_Click"/>                   
    </div>

                    </ContentTemplate>
            </asp:UpdatePanel>
    <asp:UpdatePanel runat="server" ID="up5">
          <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnProcess"/>
                 </Triggers>
        <ContentTemplate>
                 <asp:GridView runat="server" ID="gvstudentList" CssClass="table" AutoGenerateColumns="false" DataKeyNames="DptId">
 <Columns>
       
        <asp:TemplateField HeaderText="SL">
            <ItemTemplate>
                <%# Container.DataItemIndex + 1 %>
            </ItemTemplate>
        </asp:TemplateField>

 
        <asp:BoundField DataField="StudentId" HeaderText="Student ID" />
        <asp:BoundField DataField="StudentName" HeaderText="StudentcName" />
        <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
        <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:yyyy-MM-dd}" />
        <asp:BoundField DataField="ActiveDays" HeaderText="Active Days" />
        <asp:BoundField DataField="WHDays" HeaderText="Weekend/Holidays" />
        <asp:BoundField DataField="PresentDays" HeaderText="Present Days" />
        <asp:BoundField DataField="PresentRatio" HeaderText="Present Ratio (%)" DataFormatString="{0:N2}" />
        <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:N2}" />

   
        <asp:TemplateField HeaderText="Select" ItemStyle-HorizontalAlign="Center">
                                                                    <HeaderTemplate>
                                                                        <asp:CheckBox runat="server" ItemStyle-HorizontalAlign="Center" ID="hdChk" Text="All" Checked="true" AutoPostBack="True" OnCheckedChanged="hdChk_CheckedChanged"/><br />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkStatus" ItemStyle-HorizontalAlign="Center" runat="server" AutoPostBack="true" Checked="true" OnCheckedChanged="chkStatus_CheckedChanged"/>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
    </Columns>

    </asp:GridView>

            <asp:Button  runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click"/>
        </ContentTemplate>
    </asp:UpdatePanel>
   
    



 

   

</asp:Content>
