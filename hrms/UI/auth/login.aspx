
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="SigmaERP.hrms.UI.auth.login" %>

<!DOCTYPE html>

<html>
<head runat="server">
      <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.2/css/all.min.css" />
    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">

    <!-- jQuery library -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>

    <!-- Popper.js -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.16.0/umd/popper.min.js"></script>

    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link rel="stylesheet" href="https://unicons.iconscout.com/release/v4.0.0/css/line.css">
    <!-- Latest compiled JavaScript -->
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <link rel="icon" type="image/png" sizes="16x16" href="../../img/favicon.png" />
    <title>Sign In | CW-HRMS</title>
    <style>
        .login-logo {
            width: 70%;
            display: block;
            margin: 0 auto;
        }

        body {
            font-family: sans-serif;
        }

        .sign-in-form {
            background-color: #fff;
            padding: 40px;
            margin-top:40px;
        }

        .h1 {
            text-align: center;
            margin-bottom: 20px;
            font-weight: bold;
            font-family: monospace;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .label {
            display: block;
            margin-bottom: 5px;
        }

.input {
    width: 100%;
    padding: 10px 8px;
    border: 0;
    box-shadow: 0 0 10px white;
    background-color: transparent;
    line-height: 20px;
}

        .input:focus {
            outline: none;
            /* Remove default focus outline */
        }

        .input2 {
    border: double 1em transparent;
    border-radius: 30px;
    border: double 1px transparent;
    border-radius: 6px;
    background-image: linear-gradient(white, white), linear-gradient(to right, #ffec52, #ff9b15);
    background-origin: border-box;
    background-clip: content-box, border-box;
    height: 40px;
}
        input:active,
        input:focus {
            border: 0;
            outline: 0;
            background-color: transparent;
        }

        .input2:focus {
            outline: none;
            background-color: transparent;
        }
        .login-bg {
            background-color: #ffe2c1;
            height: 100vh;
        }

        .button {
            display: block;
            width: 100% !important;
            margin: 0 auto;
            color: #312f2f;
            padding: 10px 20px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
             font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-weight: bold;
            height: 40px;
            line-height: 20px;
            background: #f6921e;
            color: white;
            text-align:center;
            text-decoration:none;

        }
        .button:hover{
            color:#fff;
            text-decoration:none;
            background-color:coral;
        }

        .button:focus {
            outline: none;
            text-decoration:none;
        }

        .forgot-password {
            text-align: center;
            margin-top: 15px;
        }

        .login-as {
            margin-bottom: 15px;
            text-align: center;
        }

        .icon {
    /* background: hsla(33, 100%, 53%, 1);
    background: linear-gradient(180deg, hsla(33, 100%, 53%, 1) 0%, hsla(58, 100%, 68%, 1) 100%);
    background: -moz-linear-gradient(180deg, hsla(33, 100%, 53%, 1) 0%, hsla(58, 100%, 68%, 1) 100%);
    background: -webkit-linear-gradient(180deg, hsla(33, 100%, 53%, 1) 0%, hsla(58, 100%, 68%, 1) 100%);
    filter: progid: DXImageTransform.Microsoft.gradient(startColorstr="#FF930F", endColorstr="#FFF95B", GradientType=1); */
    color: #212529;
    height: 45px;
    line-height: 45px;
    width: 45px;
    text-align: center;
    border-radius: 20px 0px 20px 20px;
    display: block;
    line-height: 38px;
}

.icon1 {
    /* background: hsla(33, 100%, 53%, 1);
    background: linear-gradient(180deg, hsla(33, 100%, 53%, 1) 0%, hsla(58, 100%, 68%, 1) 100%);
    background: -moz-linear-gradient(180deg, hsla(33, 100%, 53%, 1) 0%, hsla(58, 100%, 68%, 1) 100%);
    background: -webkit-linear-gradient(180deg, hsla(33, 100%, 53%, 1) 0%, hsla(58, 100%, 68%, 1) 100%);
    filter: progid: DXImageTransform.Microsoft.gradient(startColorstr="#FF930F", endColorstr="#FFF95B", GradientType=1);
    color: ; */
    height: 45px;
    line-height: 45px;
    width: 45px;
    text-align: center;
    border-radius: 20px 0px 20px 20px;
    display: block;
    line-height: 40px;
}


        img {
            object-fit: contain;
        }

        input {
            background-color: transparent;
            width: 100%;
        }
        .welcome-text{
            font-weight: bold;
        }
        .login-image{
            width: 50%;
        }
        .select-input {
        width: 100%;
        padding: 10px 3px;
        border: 0;
        box-shadow: 0 0 10px white;
        background-color: transparent;
        line-height: 20px;
    }
    .copyright p{
        font-size: 12px;
    }

    .text-lighten {
        color: #A0A0A0 !important;
    }
     .fs-15 {

        margin-top:5px;
    }
.toggle-password2 {
    cursor: pointer;
}
    </style>
</head>
<body>
    <div class="container-fluid">

        <div class="row gx-0">
            <div class="col-lg-4 p-0">
                <div class="sign-in-form">
                    <img class="login-logo" src="/hrms/UI/auth/logo.png" alt="logo">
                    <p class="text-center fw-bold mt-2 mb-4 welcome-text">Welcome to CW-HRMS</p>

                    <form class="mt-3" id="form1" runat="server">
                        <div class="form-group">


                            <div class="password-wrapper input2 mb-3 d-flex">
                                <div class="icon1">
                                    <i class="fa fa-home" aria-hidden="true"></i>
                                </div>
                              <%--  <select type="" id="companyList" name="username" class="select-input">
                                    <option>Company 1</option>
                                    <option>Company 2</option>
                                    <option>Company 3</option>
                                </select>--%>
                                <asp:DropDownList ID="ddlCompany" runat="server" CssClass="select-input">
                                                                </asp:DropDownList>
                            </div>
                            <!-- <label for="username" class="label">Username</label> -->
                            <div class="password-wrapper input2 d-flex">
                                <div class="icon1">
                                    <i class="fa fa-user" aria-hidden="true"></i>
                                </div>
                                <asp:TextBox ID="txtUsername" runat="server" ClientIDMode="Static" CssClass="input" placeholder="Username"></asp:TextBox>
                                <%--<input type="text" id="username" name="username" class="input" placeholder="Username">--%>
                            </div>
                            <span class="text-danger" id="txtUserNameError"></span>
                        </div>
                        <div class="form-group">

                            <!-- <label for="password" class="label">Password</label> -->

                            <div class="password-wrapper input2 d-flex">
                                <div class="icon">
                                    <i class="fas fa-lock" aria-hidden="true"></i>
                                </div>
                                <asp:TextBox ID="txtPassword" runat="server" ClientIDMode="Static" CssClass="border-0 input" placeholder="Password" type="Password"></asp:TextBox>


                                <%--<span class="uil uil-eye text-lighten fs-15 field-icon toggle-password2"></span>--%>
                                <span class="uil uil-eye-slash text-lighten fs-15 field-icon toggle-password2"></span>
                            </div>
                            <span class="text-danger" id="txtUserPasswordError"></span>

                        </div>
                        <div class="btn-wrapper">
                            <asp:LinkButton ID="btnLogin" runat="server" CssClass="button" OnClick="btnLogin_Click" OnClientClick="return validateLogIn();">Sign In</asp:LinkButton>

                        </div>

                        <div class="copyright mt-3">
                            <p class="text-center">2024 &copy; <a href="https://www.codewareltd.com">Codeware LTD</a></p>
                        </div>

                        <!--         
                        <div class="forgot-password">
                            <a href="#">Forgot Password?</a>
                        </div>
         -->
                    </form>

                </div>
            </div>

            <div class="col-lg-8 d-lg-flex justify-content-center align-items-center login-bg d-none  p-0">
                <img class="login-image" src="/hrms/UI/auth/cw-hrms.png" alt="image">
            </div>
        </div>




    </div>
</body>
  
    <script>
        function validateLogIn() {
            var isValid = true;  // Assume the form is valid by default

            // Validate Username
            var username = $('#txtUsername').val().trim();
            if (username === "") {
                $('#txtUserNameError').html("User Name is required.");
                $("#txtUsername").focus();
                isValid = false;
            } else if (username.length < 6) {
                $('#txtUserNameError').html("User Name must be at least 6 characters.");
                $("#txtUsername").focus();
                isValid = false;
            } else {
                $('#txtUserNameError').html(""); // Clear any previous errors
            }


            var password = $('#txtPassword').val().trim();
            if (password === "") {
                $('#txtUserPasswordError').html("Password is required.");
                $("#txtPassword").focus();
                isValid = false;
            } else if (password.length < 6) {
                $('#txtUserPasswordError').html("Password must be at least 6 characters.");
                $("#txtPassword").focus();
                isValid = false;
            } else {
                $('#txtUserPasswordError').html(""); // Clear any previous errors
            }

            return isValid; // Return whether the form is valid or not
        }


        function eye_Loginpass() {
            $(".toggle-password2").click(function () {
                let input = $(this).parent().find("#txtPassword");
                let icon = $(this);

                if (input.attr("type") === "password") {
                    input.attr("type", "text");
                    icon.removeClass("uil-eye-slash").addClass("uil-eye"); // Show eye-slash icon when password is visible
                } else {
                    input.attr("type", "password");

                    icon.removeClass("uil-eye").addClass("uil-eye-slash");  // Show eye-slash icon when password is visible

                    // Show normal eye icon when password is hidden
                }
            });
        }


        eye_Loginpass();

    </script>
</html>
