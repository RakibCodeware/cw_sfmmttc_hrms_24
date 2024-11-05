<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="AppSettings.aspx.cs" Inherits="SigmaERP.hrms.AppSettings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
          .card {
      cursor: pointer;
      transition: transform 0.2s;
    }

    .card:hover {
      transform: scale(1.05);
    }

    .card i {
      font-size:40px;
      margin: 20px auto;
    }

    .card-body {
      text-align: center;
    }
    </style>
   <div class="container my-5" runat="server" id="DevloperContent">
       <asp:Button ID="logout" runat="server" Text="logout" OnClick="logout_Click" CssClass="btn btn-danger" />
    <div class="row g-4">
      <!-- Module Card -->
      <div class="col-lg-3 col-md-6">
        <a href="Modules.aspx" class="text-decoration-none">
          <div class="card h-100 shadow-sm">
          <%--  <img src="path-to-your-logo1.png" class="card-img-top" alt="Module Logo">--%>
              <i class="fas fa-th"></i>
            <div class="card-body">
              <h5 class="card-title">Module</h5>
            </div>
          </div>
        </a>
      </div>
      
      <!-- Permission Card -->
      <div class="col-lg-3 col-md-6">
        <a href="userPermissions.aspx" class="text-decoration-none">
          <div class="card h-100 shadow-sm">
            <i class="fas fa-key"></i>
            <div class="card-body">
              <h5 class="card-title">Permission</h5>
            </div>
          </div>
        </a>
      </div>

      <!-- User Packages Card -->
      <div class="col-lg-3 col-md-6">
        <a href="userPackages.aspx" class="text-decoration-none">
          <div class="card h-100 shadow-sm">
            <i class="fas fa-box-open"></i>
            <div class="card-body">
              <h5 class="card-title">User Packages</h5>
            </div>
          </div>
        </a>
      </div>

      <!-- Package Setup Card -->
      <div class="col-lg-3 col-md-6">
        <a href="userPackagesSetup.aspx" class="text-decoration-none">
          <div class="card h-100 shadow-sm">
           <i class="fas fa-tools"></i>
            <div class="card-body">
              <h5 class="card-title">Package Setup</h5>
            </div>
          </div>
        </a>
      </div>

    </div>
  </div>

<asp:HiddenField ID="devhiddenPassword" runat="server" />

<script type="text/javascript">
    document.addEventListener("DOMContentLoaded", function () {
        Swal.fire({
            title: 'Enter your password',
            input: 'password',
            inputPlaceholder: 'Enter your password',
            inputAttributes: {
                autocapitalize: 'off'
            },
            showCancelButton: true,
            confirmButtonText: 'Submit',
            showLoaderOnConfirm: true,
            preConfirm: (password) => {
                if (!password) {
                    Swal.showValidationMessage('Password cannot be empty');
                } else {
                    document.getElementById('<%= devhiddenPassword.ClientID %>').value = password;
                    __doPostBack('passwordSubmit', '');
                    return false; 
                }
            },
            willClose: () => {
                document.getElementById('<%= devhiddenPassword.ClientID %>').value = '';
            },
            allowOutsideClick: false 
        });
    });
</script>




</asp:Content>
