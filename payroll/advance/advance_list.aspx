<%@ Page Title="Advance List" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="advance_list.aspx.cs" Inherits="SigmaERP.payroll.advance.advance_list" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .Pbody {
    background: #ddd none repeat scroll 0 0;
    border-bottom: 0 !important;
    border-left: 0 !important;
    border-right: 0 !important;
}
        .PBoxheader {
    background: #FFA500 none repeat scroll 0 0 !important;
    border: 0 !important;
    padding: 6px 0;
}
        .PBoxheader h2 {
    font-size: 26px;
    margin: 5px 0 !important;
    padding: 0;
    font-weight: 600;
    text-shadow: 0px 0px 0px !important;
}
        .form-control {
            width: 100%;
            border: 1px solid #ddd;
        }
        th,td {
         text-align: center;
         font-size:14px;
         padding:5px;
        }

        .status{
            border-radius: 20px;
            padding: 3px;
            display: block;
            text-align: center;
            font-weight: bold;
            width: 90px;
            margin: 0 auto;
        }
        .Paid{ 
            border-radius: 6px;
            padding: 3px;
            display: block;
            text-align: center;
            font-weight: bold;
            width: 90px;
            margin: 0 auto;

            background:#EDFAEF;
            color:#0B6623;
        }  
        .Waived{
            border-radius: 6px;
            padding: 3px;
            display: block;
            text-align: center;
            font-weight: bold;
            width: 90px;
            margin: 0 auto;

            background:#d4e4ff;
            color:#395D96;
        } 
        .Refund{
            border-radius: 6px;
            padding: 3px;
            display: block;
            text-align: center;
            font-weight: bold;
            width: 90px;
            margin: 0 auto;

            background:#FFF3E5;
            color:#FE8A44;
        }
       .tbl-payroll {
    border: 1px solid #ddd;

       }
         .tbl-payroll th {
          background-color:orange;
          color:white;

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
                     <li><a href="<%= Session["__topMenuPayroll__"] %>">Payroll</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                  <li><a href="<%= Session["__topMenuAdvance__"] %>">Advance</a></li>
                    <li><a class="seperator" href="#">/</a></li>
                    <li><a href="#" class="ds_negevation_inactive Pactive">Advance List</a></li>
                </ul>
            </div>
        </div>
    </div>
    <asp:UpdatePanel ID="uplMessage" runat="server">
        <ContentTemplate>
            <p class="message" id="lblMessage" clientidmode="Static" runat="server"></p>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="main_box Mbox">
        <div class="main_box_header PBoxheader">
            <h2>Advance List</h2>
        </div>
        <div class="main_box_body Pbody">
            <div class="main_box_content" style="overflow: hidden">  
                <div>
                    
                            
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" />                 
                        </Triggers>
                        <ContentTemplate>   
                           <div>
                               <div style="align-items:flex-end; margin-top:10px; margin-bottom:20px; display:flex" class="row">
                                   <div class="col-lg-2" runat="server" visible="false">
                                       <label>Company</label>
                                                                       <asp:DropDownList ID="ddlCompanyList" ClientIDMode="Static" CssClass="form-control" runat="server"  >                
                                 

                                 </asp:DropDownList>
                                   </div>   
                                   <div class="col-lg-2">
                                       <label>Emp.Type</label>
                                                                     <asp:RadioButtonList runat="server" ClientIDMode="Static" ID="rblEmpType" RepeatDirection="Horizontal"></asp:RadioButtonList>
                                   </div>
                                   <div class="col-lg-2">
                                       <label>Status</label>
                                                                       <asp:DropDownList ID="ddlStatus" ClientIDMode="Static" CssClass="form-control" runat="server"  >              
                                 

                                 </asp:DropDownList>
                                   </div> 
                                    
                                   <div class="col-lg-2">
                                       <label>Emp. Card No</label>
                                                                     <asp:TextBox runat="server" ClientIDMode="Static" ID="txtEmpCardNo" CssClass="form-control" ></asp:TextBox>
                                   </div>
                                   <div class="col-lg-2">
                                      <asp:Button runat="server" ID="btnSearch" CssClass="Pbutton" Text="Search" OnClick="btnSearch_Click"  />
                                   </div>
                               </div>






                           </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                
                   

                   <div class="loding_img">
                       <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                                    <ProgressTemplate>
                                        <span style=" font-family:'Times New Roman'; font-size:20px; color:green;font-weight:bold;float:left"><p>&nbsp;</p> </span> <br />
                                        <img cursor:pointer; float:left" src="/images/loader-2.gif"/>  
                                    </ProgressTemplate>
                                </asp:UpdateProgress>
                  </div>
                </div>

                    <div>

                        <asp:UpdatePanel runat="server" ID="up2" UpdateMode="Conditional">
                            <Triggers>
                                <asp:AsyncPostBackTrigger  ControlID="btnSearch" EventName="Click"/>
                            </Triggers>
                            <ContentTemplate>
                                <asp:GridView ID="gvAdvanceInfo" CssClass="tbl-payroll" runat="server" DataKeyNames="LoanID" Width="100%"  AutoGenerateColumns="false" HeaderStyle-Font-Bold="false"  AllowPaging="True" OnPageIndexChanging="gvAdvanceInfo_PageIndexChanging" PageSize="25" OnRowDataBound="gvAdvanceInfo_RowDataBound" OnRowCommand="gvAdvanceInfo_RowCommand" >
                                    <PagerStyle CssClass="gridview" Height="20px" />
                                    <Columns>                                      
                                        <asp:TemplateField  HeaderText="SL">
                                            <ItemTemplate>
                                                <label><%# Container.DataItemIndex + 1 %></label>                                            </ItemTemplate>
                                           
                                        </asp:TemplateField>                                  
                                        <asp:BoundField DataField="EmpCardNo" HeaderText="Card No"/>
                                        <asp:BoundField DataField="EmpName" HeaderText="Name" />
                                        <asp:BoundField DataField="LoanAmount" HeaderText="Loan"  />                                        
                                        <asp:BoundField DataField="InstallmentAmount" HeaderText="Ins.Amount"  />
                                        <asp:BoundField DataField="DeductFrom" HeaderText="Deduct Start From" /> 
                                        <asp:BoundField DataField="LastInstallmentMonth" HeaderText="Last Installment" />   
                                        <asp:BoundField DataField="PaidAmount" HeaderText="Paid"   />    
                                        <asp:BoundField DataField="DueAmount" HeaderText="Due"  /> 
                                        <asp:TemplateField>
                                  <HeaderTemplate>
                                      Status
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:Label runat="server" ClientIDMode="Static" ID="lblStatus" CssClass='<%#Bind("Status") %>' Text='<%# Bind("Status") %>'  > </asp:Label>
                                
                                  </ItemTemplate>
                                   <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField> 
                                        <asp:TemplateField>
                                  <HeaderTemplate>
                                      Doc.
                                  </HeaderTemplate>
                                  <ItemTemplate>
                                      <asp:HyperLink runat="server" ClientIDMode="Static" ID="hlAttachment" Target="_blank" CssClass="btn btn-link">Link</asp:HyperLink>
                                     
                                                             
                                  </ItemTemplate>
                                   <ItemStyle HorizontalAlign="Center" />
                              </asp:TemplateField> 
                                        <asp:BoundField DataField="StatusDate" HeaderText="Date"  />           
                                        <asp:BoundField DataField="StatusNote" HeaderText="Comment"  />      
                                    </Columns>
                                    <SelectedRowStyle BackColor="Yellow" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
               
                    
                    
                </div>

            </div>
        </div>
        
        <script type="text/javascript">            

</script>
</asp:Content>