<%@ Page Title="Payroll" Language="C#" MasterPageFile="~/payroll_nested.master" AutoEventWireup="true" CodeBehind="payroll_default.aspx.cs" Inherits="SigmaERP.payroll_nested1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    

     <div class="row">
            <div class="col-md-12">
                <div class="ds_nagevation_bar" style="border-bottom:none;">
                     <div style="margin-top: 5px">
                           <ul>
                               <li><a href="/default.aspx">Dasboard</a></li>
                               <li> <a class="seperator" href="#">/</a></li>
                               <li> <a href="#" class="ds_negevation_inactive Pactive">Payroll</a></li>
                           </ul>               
                     </div>
                </div>
             </div>
        </div>
    
      <%--  <img alt="" class="main_images" src="images/hrd.png">--%>
    <div>
         <div class="col-lg-12">
             <div class="row ">
                 <div class="col-lg-2">
                 </div>
                 <div runat="server" id="divBonus" class="col-lg-2 col-md-3 col-sm-6">
                     <a class="ds_Settings_Basic_Text " href="/hrms/bonus"> <i class="uil uil-money-bill iconStyle"></i><br /> <span>Bonus</span></a>
                     
                 </div>
                 <div runat="server" id="divAdvance" class=" col-lg-2 col-md-3 col-sm-6">
                        <a class="ds_Settings_Basic_Text " href="/hrms/advance"><i class="fas fa-hand-holding-usd FsiconStyle "></i><br /><span>Advance</span></a>
                 </div>
                 <div runat="server" id="divSalary" class=" col-lg-2 col-md-3 col-sm-6">
                      <a class="ds_Settings_Basic_Text " href="/hrms/salary"><i class="fas fa-money-bill-wave FsiconStyle"></i><br />Salary</a>
                 </div>
                 <div runat="server" id="divSignatureEntry" class=" col-lg-2 col-md-3 col-sm-6">
                      <a class="ds_Settings_Basic_Text " href="/hrd/Signatures.aspx" target="_blank"><i class="fas fa-hand-holding-usd FsiconStyle "></i><br /><span>Signature</span></a>
                 </div>
                 <div runat="server" id="divPF" class="col-lg-2 col-md-3 col-sm-6">
                     <a class="ds_Settings_Basic_Text " href="/hrms/provident-found">
                         <i class="uil uil-money-stack FsiconStyle"></i><br />
                         <span>Provident Fund</span></a>

                 </div>
                  <div class="col-lg-2"></div>
             </div>
             <div class="row ">

                                  <div class="col-lg-2">
                 </div>
                 <div runat="server" id="divVat" class=" col-lg-2 col-md-3 col-sm-6">
                     <a class="ds_Settings_Basic_Text " href="/hrms/vat-tax-root">
                         <i class="fas fa-money-check-alt FsiconStyle"></i><br />
                         <span>Vat&Tax
                         </span></a>
                 </div>

             </div>


</div></div>

</asp:Content>
