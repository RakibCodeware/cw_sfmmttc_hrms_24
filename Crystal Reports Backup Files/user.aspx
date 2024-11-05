<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="user.aspx.cs" Inherits="SigmaERP.hrms.user" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <main class="main-content">
      <div class="Dashbord">
         <div class="crm mb-25">
            <div class="container-fulid">
               <div class="card card-Vertical card-default card-md mt-4 mb-4">

                  <div class="card-header d-flex align-items-center">
                     <div class="card-title d-flex align-items-center justify-content-between">
                        <div class="d-flex align-items-center gap-3">
                           <h4>Add User</h4>
                        </div>

                     </div>
                      <div class="col-lg-3 " id="IsGuest" style="display:none">
                          <div class="form-group d-flex">
                              <label for="chkIsGetUser" class="color-dark fs-14 fw-500 align-center">
                                  Guest User <span class="text-danger"></span>
                              </label>
                              <div class="radio-horizontal-list d-flex">
                                  <div class="form-check form-switch form-switch-primary form-switch-sm mx-3">
                                      <input type="checkbox" class="form-check-input" id="chkIsGetUser">
                                      <label class="form-check-label" for="chkIsGetUser"></label>
                                  </div>
                              </div>
                          </div>
                      </div>


                     <div class="btn-wrapper">
                        <div class="dm-button-list d-flex flex-wrap align-items-end">
                        <button type="button" id="addnew" onclick="Cardbox();" class="btn btn-secondary btn-default btn-squared">Add New</button>                          
                        </div>
                     </div>
                  </div>
                   
                  <div style="display: none;" id="Cardbox" class="card-body pb-md-30">
                     <div class="Vertical-form">
                           <div class="row">
                               <div class="col-lg-7">
                                   <div class="row">
                                       <div class="col-lg-4" ">
                                           <div class="form-group">
                                               <label id="lblHidenUserId" style="display: none"></label>
                                               <label for="ddlCompany" class="color-dark fs-14 fw-500 align-center mb-10">Company</label>
                                               <div class="support-form__input-id">
                                                   <div class="dm-select ">

                                          <%--             <select name="ddlCompany" runat="server" id="ddlCompany" class="select-search form-control">
                                                           <option value="0">---Select---</option>
                                                       </select>--%>
                                                        <asp:DropDownList runat="server" ID="ddlCompany" ClientIDMode="Static" class="select-search form-control"></asp:DropDownList>

                                                   </div>
                                                   <span class="text-danger" id="ddlcompanyError"></span>
                                               </div>
                                           </div>
                                       </div>
                                       <div class="col-lg-4" id="ddlReference">
                                           <div class="form-group">
                                               <label for="ddlReferenceEmp" class="color-dark fs-14 fw-500 align-center mb-10">Employee</label>
                                               <div class="support-form__input-id">
                                                   <div class="dm-select ">
                                                       <select name="ddlReferenceEmp" id="ddlReferenceEmp" class="select-search form-control">
                                                           <option value="0">---Select---</option>
                                                       </select>
                                                   </div>
                                                   <span class="text-danger" id="repEmpError"></span>
                                               </div>
                                           </div>
                                       </div>

                                       <div id="FirstName" style="display: none" class="col-lg-4">
                                           <div class="form-group">

                                               <label for="txtFirstName" class="color-dark fs-14 fw-500 align-center mb-10">
                                                   First Name
                                               </label>
                                               <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtFirstName" placeholder="Type First Name" pattern="[^\d]*" title="Numbers are not allowed">
                                               <span class="text-danger" id="firstNameError"></span>
                                           </div>
                                       </div>
                                       <div id="LastName" style="display: none" class="col-lg-4">
                                           <div class="form-group">
                                               <label for="txtLastName" class="color-dark fs-14 fw-500 align-center mb-10">
                                                   Last Name
                                               </label>
                                               <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtLastName" placeholder="Type Last name">
                                               <span class="text-danger" id="lastNameError"></span>
                                           </div>
                                       </div>
                                       <div class="col-lg-4">
                                   <div class="form-group">
                                       <label for="txtUserName" class="color-dark fs-14 fw-500 align-center mb-10">
                                           User Name <span class="text-danger">*</span>
                                       </label>
                                       <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtUserName" placeholder="Type User Name">
                                       <span class="text-danger" id="userNameError"></span>
                                   </div>
                               </div>
                               <div class="col-lg-4">
                                   <div class="form-group">
                                       <label for="txtUserPassword" class="color-dark fs-14 fw-500 align-center mb-10">
                                           Password <span class="text-danger">*</span>
                                       </label>
                                       <input type="password" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtUserPassword" placeholder="Type User Password">
                                       <span class="text-danger" id="userPasswordError"></span>
                                   </div>
                               </div>
                               <div class="col-lg-4">
                                   <div class="form-group">
                                       <label for="txtUserEmail" class="color-dark fs-14 fw-500 align-center mb-10">
                                           User Email <span class="text-danger">*</span>
                                       </label>
                                       <input type="email" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtUserEmail" placeholder="Type User Email">
                                       <span class="text-danger" id="userEmailError"></span>
                                   </div>
                               </div>
                               <div class="col-lg-4">
                                 <div class="form-group">
                                     <label for="ddlUserRole" class="color-dark fs-14 fw-500 align-center mb-10">User Role<span class="text-danger">*</span></label>
                                     <div class="support-form__input-id">
                                         <div class="dm-select ">
                                             <select name="ddlUserRole" id="ddlUserRole" class="select-search form-control">
                                                 <option value="0">---Select---</option>
                                             </select>
                                         </div>
                                           <span class="text-danger" id="roleError"></span>
                                     </div>
                                 </div>
                              </div>

<%--                                       <div class="col-lg-4 " id="departmentcheck">
                                           <div class="form-group">
                                               <label for="departmentCheckboxes" class="color-dark fs-14 fw-500 align-center mb-10">
                                                   Data Access Level <span class="text-danger">*</span>
                                               </label>
                                               <label id="DataAccessStatus"></label>
                                               <div id="departmentCheckboxes"></div>

                                           </div>
                                       </div>--%>


                                       <div class="col-lg-4 " style="display: flex; justify-content: space-between">
                                           <div class="LeftSite">
                                               <input style="opacity: 0" type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="">
                                               <div class="form-group d-flex">
                                                   <label for="chkIsActive" class="color-dark fs-14 fw-500 align-center">
                                                       Status <span class="text-danger"></span>
                                                   </label>
                                                   <div class="radio-horizontal-list d-flex">
                                                       <div class="form-check form-switch form-switch-primary form-switch-sm mx-3">
                                                           <input type="checkbox" checked class="form-check-input" id="chkIsActive">
                                                           <label class="form-check-label" for="chkIsActive"></label>
                                                       </div>
                                                   </div>
                                               </div>
                                           </div>
                                           <div class="rightSite">
                                               <input style="opacity: 0" type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="">
                                               <div class="form-group d-flex">
                                                   <label for="chkIsActive" class="color-dark fs-14 fw-500 align-center">
                                                       Authority <span class="text-danger"></span>
                                                   </label>
                                                   <div class="radio-horizontal-list d-flex">
                                                       <div class="form-check form-switch form-switch-primary form-switch-sm mx-3">
                                                           <input type="checkbox" checked class="form-check-input" id="chkIsAuthPerm">
                                                           <label class="form-check-label" for="chkIsAuthPerm"></label>
                                                       </div>
                                                   </div>
                                               </div>
                                           </div>

                                       </div>
<%--                                       <div class="col-lg-4">
                                           <div class="form-group">
                                               <div class="card card-default card-md mb-4">
                                                   <div class="card-body">
                                                       <div class="dm-tag-wrap">

                                                           <div class="dm-upload">
                                                               <div class="dm-upload-avatar">
                                                                   <img class="avatrSrc" src="img/upload.png" alt="Avatar Upload">
                                                               </div>
                                                               <div class="avatar-up">
                                                                   <input type="file" name="userImage" class="upload-avatar-input">
                                                               </div>
                                                           </div>

                                                       </div>
                                                   </div>
                                               </div>
                                           </div>
                                       </div>--%>



                                       <div class="col-lg-4">
                                           <div class="form-group">
                                               <label style="opacity: 0;" for="formGroupExampleInput"
                                                   class="color-dark fs-14 fw-500 align-center mb-10">
                                                   Name <span
                                                       class="text-danger"></span>
                                               </label>
                                               <button type="button" id="btnSave" onclick="validateUser()"
                                                   class="btn btn-primary btn-default btn-squared px-30">Save</button>
                                           </div>
                                       </div>


                                   </div>

                               </div>

                            

                               <div class="col-lg-3" id="treeSection">
                                   <p>Select User Permissions</p>
                                   <div class="loader-size loaderPackages " style="display: none">
                                       <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                           <span class="spin-dot badge-dot dot-primary"></span>
                                           <span class="spin-dot badge-dot dot-primary"></span>
                                           <span class="spin-dot badge-dot dot-primary"></span>
                                           <span class="spin-dot badge-dot dot-primary"></span>
                                       </div>
                                   </div>
                                   <div id="treeContainer"></div>
                               </div>


                               <div class="col-lg-2 " id="departmentcheck">
                                   <div class="form-group">
                                       <label for="departmentCheckboxes" class="color-dark fs-14 fw-500 align-center">
                                            Data Access Level <span class="text-danger">*</span>
                                       </label>
                                       <label id="DataAccessStatus"></label>
                                       <div id="departmentCheckboxes"></div>

                                   </div>
                               </div>




             
                     </div>
                  </div>
               </div>
            </div>
               <!-- Department List  -->
                        <div class="row">
               <div class="col-lg-12">
                  <div class="card ">
                     <div class="card-body">

                        <div class="userDatatable adv-table-table global-shadow border-light-0 w-100 ">
                           <div class="table-responsive">
                              <div class="ad-table-table__header d-flex justify-content-between">
                                  <h4 style="margin-top: 13px;">Users</h4>
                              <div id="filter-form-container">

                            

                              </div>
                              </div>

                               <div class="loader-size loaderModulesList " style="display: none">
                                   <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                       <span class="spin-dot badge-dot dot-primary"></span>
                                       <span class="spin-dot badge-dot dot-primary"></span>
                                       <span class="spin-dot badge-dot dot-primary"></span>
                                       <span class="spin-dot badge-dot dot-primary"></span>
                                   </div>
                               </div>

                               <table class="table mb-0 table-borderless adv-table" data-sorting="true" data-filtering="true" data-filter-container="#filter-form-container" data-paging="true" data-paging-size="10">
                               </table>
                           </div>
                        </div>

                     </div>
                  </div>
               </div>
            </div>
            </div>
         </div>


   </main>
     <script>

         var token = '<%= Session["__UserToken__"] %>';
         console.log('this is token you can use it :',token);


         //var rootUrl = 'https://localhost:7220';
         var rootUrl = '<%= Session["__RootUrl__"]%>';
         var CompanyID = '<%= Session["__GetCompanyId__"]%>';

         var getUsersUrl = rootUrl + '/api/User/users';
         var getRolesUrl = rootUrl + '/api/UserRoles/userRoles';
         var getRolesWithGuestUrl = rootUrl + '/api/UserRoles/userRolesWithGuestUser';
         var getEmployeeUrl = rootUrl + '/api/Employee/EmployeeName';
         var GetFeturesUrl = rootUrl + '/api/UserModules/Packages';
         var getRolesByIdUrl = rootUrl + '/api/UserRoles/userRoles';
         var createUserUrl = rootUrl + '/api/User/users/create';
         var updateUserUrl = rootUrl + '/api/User/users/update';
         var DeleteUserUrl = rootUrl + '/api/User/users/delete';
         var getStpPkgFeaturesWithParentUrl = rootUrl + '/api/UserPackagesSetup/SetupedPackagesWithParent';
         var getUserDepartmentUrl = rootUrl + '/api/UserRoles/UserDepartment';



         //var token = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJpYXQiOjE3MTQ2MjQ5MjYsImV4cCI6MTc0NjE2MDkyNiwiYXVkIjoiIiwic3ViIjoiSldUU2VydmljZUFjY2Vzc1Rva2VuIn0.tVlIuOLas2VxEnBohuaIXXQR2Lju_2h8yVjCDizQh9o';

         $(document).ready(function () {
             IsGuestUser();
             GetRoles(false);
             GetEmployee();
             GetUsers();
             GetStpPkgFeatures();
             GetRolesFeatures();
             $('#chkIsGetUser').change(function () {
                 toggleReferenceEmp();
             });


         });
         function toggleReferenceEmp() {
             if ($('#chkIsGetUser').is(':checked')) {
                 $('#ddlReference').hide("");
                 $('#repEmpError').html("");
                 $('#userEmailError').html("");
                 $('#userPasswordError').html("");
                 $('#userNameError').html("");
                  $('#roleError').html("");
             } else {
                 $('#ddlReference').show();
                 $('#userEmailError').html("");
                 $('#roleError').html("");
                 $('#userPasswordError').html("");
                 $('#userNameError').html("");
                
             }
         }

         async function validateUser() {
             var isValid = true;
             let alphabeticPattern = /^[a-zA-Z]+( [a-zA-Z]+)*$/;
             let emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

             if ($('#chkIsGetUser').is(':checked')) {
                 $('#ddlReference').hide();
                 $('#repEmpError').html("");
             } else {
                 $('#ddlReference').show();
                 let selectedReferenceEmp = $('#ddlReferenceEmp').val();
                 if (selectedReferenceEmp == "0") {
                     $('#repEmpError').html("Please select a valid reference employee.");
                     $("#ddlReferenceEmp").focus();
                     isValid = false;  // Mark as invalid
                 } else {
                     $('#repEmpError').html("");
                 }
             }

             let firstName = $('#txtFirstName').val().trim();
             if (firstName !== "" && !alphabeticPattern.test(firstName)) {
                 $('#firstNameError').html("Only alphabetic characters are allowed.");
                 $("#txtFirstName").focus();
                 isValid = false;
             } else {
                 $('#firstNameError').html("");
             }

             let lastName = $('#txtLastName').val().trim();
             if (lastName !== "" && !alphabeticPattern.test(lastName)) {
                 $('#lastNameError').html("Only alphabetic characters are allowed.");
                 $("#txtLastName").focus();
                 isValid = false;
             } else {
                 $('#lastNameError').html("");
             }

             if ($('#txtUserName').val().trim() === "") {
                 $('#userNameError').html("User Name is required.");
                 $("#txtUserName").focus();
                 isValid = false;
             } else if ($('#txtUserName').val().trim().length < 6) {
                 $('#userNameError').html("User Name must be at least 6 characters.");
                 $("#txtUserName").focus();
                 isValid = false;
             } else {
                 $('#userNameError').html("");
             }

             if ($('#txtUserPassword').val().trim() === "") {
                 $('#userPasswordError').html("Password is required.");
                 $("#txtUserPassword").focus();
                 isValid = false;
             } else if ($('#txtUserPassword').val().trim().length < 6) {
                 $('#userPasswordError').html("Password must be at least 6 characters.");
                 $("#txtUserPassword").focus();
                 isValid = false;
             } else {
                 $('#userPasswordError').html("");
             }

             let userEmail = $('#txtUserEmail').val().trim();
             if (userEmail === "") {
                 $('#userEmailError').html("Email is required.");
                 $("#txtUserEmail").focus();
                 isValid = false;
             } else if (!emailPattern.test(userEmail)) {
                 $('#userEmailError').html("Please enter a valid email address.");
                 $("#txtUserEmail").focus();
                 isValid = false;
             } else {
                 $('#userEmailError').html("");
             }

             let selectedRole = $('#ddlUserRole').val();
             if (selectedRole == "0") {
                 $('#roleError').html("Please select a role.");
                 $("#ddlUserRole").focus();
                 isValid = false;
             } else {
                 $('#roleError').html("");
             }

             // If validation passes, proceed with save or update
             if (isValid) {
                 var addnewElement = $("#btnSave").text().trim();  // Get the button text

                 // If adding new user
                 if (addnewElement === "Save") {
                     try {
                         var result = await PostUsers(true);  // Wait for PostUsers to finish

                         if (result === true) {  // Check if PostUsers returned true
                             ClearTextBox();  // Clear the form fields
                         }
                     } catch (error) {
                         console.error("An error occurred:", error);  // Handle any errors
                     }
                 }
                 // If updating existing user
                 else {

                     try {
                         var result = await  updateUsers(true);  // Wait for PostUsers to finish

                         if (result === true) {  // Check if PostUsers returned true
                             ClearTextBox();  // Clear the form fields
                         }
                     } catch (error) {
                         console.error("An error occurred:", error);  // Handle any errors
                     } ; 



                 }
             }
         }





         function IsGuestUser () {
                $('#chkIsGetUser').change(function() {
                if ($(this).is(':checked')) {
                    $('#FirstName, #LastName').show();
                    GetRoles(true);
                } else {
                    $('#FirstName, #LastName').hide();
                    GetRoles(false);
                }
            });
         }

         function GetRolesFeatures() {
             $('#ddlUserRole').change(function () {
                 var selectedValue = $(this).val();
                 console.log("Selected value: " + selectedValue);

                 
                 FetchDataRolesWise(selectedValue);
                 //GetDataAccessLevel(selectedValue);


             });
         }



         //         function GetDataAccessLevel(roleID) {
         //    ApiCallById(getRolesByIdUrl, token, roleID)
         //        .then(function (response) {
         //            console.log('Data from Data Access:', response);
         //            var data = response.data;
         //            $('#lblHidenRolesId').val(data.userRoleId);
         //            console.log(data.dataAccessLevel)


         //        })
         //        .catch(function (error) {
         //            console.error('Error:', error);
         //        });
         //}




        






         //function GetRoles() {
         //    ApiCall(getRolesUrl, token)
         //        .then(function (response) {
         //            if (response.statusCode === 200) {
         //                var responseData = response.data;
         //                RolesPopulateDropdown(responseData)
         //            } else {
         //                console.error('Error occurred while fetching data:', response.message);
         //            }
         //        })
         //        .catch(function (error) {
         //            $('.loaderCosting').hide();
         //            console.error('Error occurred while fetching data:', error);
         //        });
         //}

         function GetRoles(IsGuestUser) {
             return new Promise((resolve, reject) => {  // Ensure GetRoles returns a promise
                 ApiCallByGuestUser(getRolesWithGuestUrl, token, IsGuestUser)
                     .then(function (response) {
                         if (response.statusCode === 200) {
                             var responseData = response.data;
                             console.log('this from dropdown:', responseData);
                             RolesPopulateDropdown(responseData);
                             resolve();  // Resolve the promise once roles are populated
                         } else {
                             console.error('Error occurred while fetching data:', response.message);
                             reject(response.message);  // Reject the promise on error
                         }
                     })
                     .catch(function (error) {
                         $('.loaderCosting').hide();
                         console.error('Error occurred while fetching data:', error);
                         reject(error);  // Reject the promise if API call fails
                     });
             });
         }

         function RolesPopulateDropdown(data) {
             const dropdown = document.getElementById('ddlUserRole');
             dropdown.innerHTML = '<option value="0">---Select---</option>'; // Clear existing options

             data.forEach(item => {
                 const option = document.createElement('option');
                 option.value = item.rolesID; // Use rolesID from the response
                 option.textContent = item.roleName; // Use roleName from the response
                 dropdown.appendChild(option);
             });
         }
         function GetEmployee() {
             ApiCall(getEmployeeUrl, token)
                 .then(function (response) {
                     if (response.statusCode === 200) {
                         var responseData = response.data;
                         EmployeePopulateDropdown(responseData)
                     } else {
                         console.error('Error occurred while fetching data:', response.message);
                     }
                 })
                 .catch(function (error) {
                     $('.loaderCosting').hide();
                     console.error('Error occurred while fetching data:', error);
                 });
         }
         function EmployeePopulateDropdown(data) {
             const dropdown = document.getElementById('ddlReferenceEmp');
             dropdown.innerHTML = '<option value="0">---Select---</option>'; // Clear existing options

             data.forEach(item => {
                 const option = document.createElement('option');
                 option.value = item.empId;
                 option.textContent = item.fullName;
                 dropdown.appendChild(option);
             });
         }


         function GetUsers() {
             ApiCall(getUsersUrl, token)
                 .then(function (response) {
                     if (response.statusCode === 200) {
                         var responseData = response.data;
                         console.log(responseData);
                         $('.footable-loader').show();
                         bindTableData(responseData);
                     } else {
                         console.error('Error occurred while fetching data:', response.message);
                     }
                 })
                 .catch(function (error) {
                     $('.loaderCosting').hide();
                     console.error('Error occurred while fetching data:', error);
                 });
         }
         function bindTableData(data) {
             if ($('.adv-table').data('footable')) {
                 $('.adv-table').data('footable').destroy();
             }
             $('.adv-table').html('');
             $('#filter-form-container').empty();

             let serialNumber = 1; // Initialize serial number
             const defaultImage = '/hrms/user_img_default.jpg'; // Set the path to your default image here

             data.forEach(row => {
                 row.serial = serialNumber++; // Assign serial number to each row

                 var readAction = '<%= Session["__ReadAction__"] %>';
                 var updateAction = '<%= Session["__UpdateAction__"] %>';
                 var deleteAction = '<%= Session["__DeletAction__"] %>';

                 // Use default image if userImage is not found or invalid
                 const userImage = row.userImage ? row.userImage : defaultImage;

                 // Combine user image, name, and role
                 row.userImage = `
                    <div class="user-details-container d-flex align-items-center">
                        <img src="${userImage}" alt="User Image" class="user-image" style="width: 40px; height: 40px; margin-right: 10px;">
                        <div>
                            <a href="javascript:void(0)" class="user-name" data-id="${row.userId}">${row.name}</a>
                            <div class="user-role">${row.userRoleName}</div>
                        </div>
                    </div>
                `;

                 row.isActive = `
                <div class="form-check form-switch form-switch-primary form-switch-sm">
                    <input type="checkbox" class="form-check-input" id="switch-${row.userId}" ${row.isActive ? 'checked' : ''}>
                    <label class="form-check-label" for="switch-${row.userId}"></label>
                </div>
                `;

                 row.isGuestUser = `
            <div class="form-check form-switch form-switch-primary form-switch-sm">
                <input type="checkbox" class="form-check-input" id="switch-${row.userId}" ${row.isGuestUser ? 'checked' : ''}>
                <label class="form-check-label" for="switch-${row.userId}"></label>
            </div>
        `;

                 row.dataAccessLevel = row.dataAccessLevel === 1
                     ? '<span class="badge bg-onlyme">Only Me</span>'
                     : row.dataAccessLevel === 2
                         ? '<span class="badge bg-warning">Own Department</span>'
                         : row.dataAccessLevel === 3
                             ? '<span class="badge bg-success">All</span>'
                             : row.dataAccessLevel === 4
                                 ? '<span class="badge bg-info">Custom</span>'
                                 : '<span class="badge bg-secondary">NA</span>';


                 row.isApprovingAuthority = `
            <div class="form-check form-switch form-switch-primary form-switch-sm">
                <input type="checkbox" class="form-check-input" id="switch-${row.userId}" ${row.isApprovingAuthority ? 'checked' : ''}>
                <label class="form-check-label" for="switch-${row.userId}"></label>
            </div>
        `;

                 // Define the action column HTML with icons for view, edit, and delete
                 row.action = `
            <div class="actions">
                <ul class="orderDatatable_actions mb-0 d-flex flex-wrap">
                    <li><a href="javascript:void(0)" class="view-btn view" data-id="${row.userId}"><i class="uil uil-eye"></i></a></li>
                    <li><a href="javascript:void(0)" data-id="${row.userId}" class="edit-btn edit"><i class="uil uil-edit"></i></a></li>
                    <li><a href="javascript:void(0)" data-id="${row.userId}" class="delete-btn remove"><i class="uil uil-trash-alt"></i></a></li> 
                </ul>
            </div>
        `;
             });

             const columns = [
                 { "name": "serial", "title": "SL", "breakpoints": "xs sm", "type": "number", "className": "userDatatable-content" }, // Serial number column
                 { "name": "userImage", "title": "User", "className": "userDatatable-content" }, // Combined user image, name, and role column
                 { "name": "dataAccessLevel", "title": "Data Access Level", "className": "userDatatable-content" },
                 { "name": "email", "title": "Email", "className": "userDatatable-content" },
                 { "name": "isGuestUser", "title": "Guest User", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                 { "name": "isApprovingAuthority", "title": "Authority", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                 { "name": "isActive", "title": "Status", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                 { "name": "action", "title": "Action", "sortable": false, "filterable": false, "className": "userDatatable-content" }, // Action column
             ];

             try {
                 $('.adv-table').footable({
                     "columns": columns,
                     "rows": data,
                     "filtering": {
                         "enabled": true,
                         "placeholder": "Search...",
                         "dropdownTitle": "Search in:",
                         "position": "left",
                         "containers": "#filter-form-container",
                         "space": true
                     }
                 }).on('postinit.ft.table', function (e) {
                     $('.footable-loader').hide();
                 });
             } catch (error) {
                 console.error("Error initializing Footable:", error);
             }

             // Clear and re-attach event listeners
             $('.adv-table').off('click', '.edit-btn').on('click', '.edit-btn', function () {
                 const userId = $(this).data('id');
                 FetchDataForEdit(userId);
                 console.log('Edit button clicked for ID:', userId);
             });

             $('.adv-table').off('click', '.delete-btn').on('click', '.delete-btn', function () {
                 const id = $(this).data('id');
                 Delete(id);
                 console.log('Delete button clicked for ID:', id);
             });

             $('.adv-table').off('click', '.view-btn').on('click', '.view-btn', function () {
                 const id = $(this).data('id');
                 FetchDataForView(id);
                 console.log('View button clicked for ID:', id);
             });

             $('.adv-table').off('click', '.feature-btn').on('click', '.feature-btn', function () {
                 const id = $(this).data('id');
                 console.log('Feature button clicked for ID:', id);
                 // FetchDataForEdit(id);
             });

            $('.adv-table').off('click', '.user-name').on('click', '.user-name', function () {
                const userId = $(this).data('id');
                console.log('User name clicked for ID:', userId);

                // Redirect to userProfile.aspx with userId as a query parameter
                window.open(`/hrms/profile?userId=${userId}`, '_blank');
            });

         }


       
         var selectedPermissionIDs = [];
         var responseData = null;
         function GetStpPkgFeatures() {
             console.log('Calling GetModule');
             $('.loaderPackages').show();
             ApiCall(getStpPkgFeaturesWithParentUrl, token)
                 .then(function (response) {
                     if (response.statusCode === 200) {
                         responseData = response.data;
                         var treeData = transformToJSTreeFormat(responseData);
                         console.log("TreeData :", treeData)
                         $('#treeContainer').jstree({
                             'core': {
                                 'data': treeData,
                                 'themes': {
                                     'dots': true
                                 },
                                 'multiple': true,
                                 'animation': true,
                                 'check_callback': true
                             },
                             'checkbox': {
                                 'keep_selected_style': false,
                                 'tie_selection': true
                             },
                             'plugins': ['checkbox', 'wholerow']
                         }).on('changed.jstree', function (e, data) {
                             selectedPermissionIDs = [];
                             for (i = 0, j = data.selected.length; i < j; i++) {

                                 var node = data.instance.get_node(data.selected[i]);
                                 if (node && node.children.length === 0) {
                                     selectedPermissionIDs.push(parseInt(node.id, 10));
                                 }
                                 console.log('node:', node);
                             }
                             console.log('Child Node IDs:', selectedPermissionIDs);

                         });
                         $('.loaderPackages').hide();
                     } else {
                         console.error('Error occurred while fetching data:', response.message);
                     }
                 })
                 .catch(function (error) {
                     console.error('Error occurred while fetching data:', error.message || error);
                 });
         }
          function transformToJSTreeFormat(data) {
            return data.map(function (item) {
                let hasSelectedChild = item.children && item.children.some(child => child.state && child.state.selected);

                return {
                    "id": item.permissionId,
                    "text": item.name,
                    "state": {
                        "opened": true,
                        "selected": hasSelectedChild
                    },
                    "children": item.children && item.children.length > 0 ? transformToJSTreeFormat(item.children) : [],
                    "li_attr": {
                        "id":  item.permissionId 
                    },
                    "original": {
                        "isPermission": item.isPermission
                    },
                    "icon": item.isPermission ? "fa fa-key custom-permission-icon" : "fa fa-lock custom-module-icon"
                };
            });
         }
         var selectedIds = []; // This will hold the selected department IDs (for update)
         var isUpdateMode = false; // This will determine if it's update mode or add mode

         // Call this function with the appropriate mode and selected department IDs
         function GetUserDepartment(selectedDepartmentIds = [], mode = 'add') {
             isUpdateMode = (mode === 'update'); // Set mode

             ApiCallById(getUserDepartmentUrl, token ,CompanyID)
                 .then(function (response) {
                     if (response.statusCode === 200) {
                         var responseData = response.data;

                         $('#departmentCheckboxes').empty();

                         var selectAllHtml = `
                    <label>
                        <input type="checkbox" id="selectAllDepartments">
                        Select All
                    </label><br>
                `;
                         $('#departmentCheckboxes').append(selectAllHtml);

                         // Ensure that selectedDepartmentIds are converted to strings (if needed)
                         selectedDepartmentIds = selectedDepartmentIds.map(id => id.toString());

                         // Check if selectedDepartmentIds has values, and only apply checked logic if true
                         var hasSelectedDepartments = selectedDepartmentIds.length > 0;
                         console.log(selectedDepartmentIds);

                         responseData.forEach(function (department) {
                             var isChecked = hasSelectedDepartments && selectedDepartmentIds.includes(department.dptId.toString());

                             var checkboxHtml = `
                        <label>
                            <input type="checkbox" name="departments" value="${department.dptId}" class="departmentCheckbox" ${isChecked ? 'checked' : ''}>
                            ${department.dptName}
                        </label><br>
                    `;

                             $('#departmentCheckboxes').append(checkboxHtml);
                         });

                         // Handle the select all functionality
                         $('#selectAllDepartments').on('change', function () {
                             var isChecked = $(this).is(':checked');
                             $('.departmentCheckbox').prop('checked', isChecked);
                             logSelectedDepartmentIds();
                         });
                         $('.departmentCheckbox').on('change', function () {
                             var allChecked = $('.departmentCheckbox').length === $('.departmentCheckbox:checked').length;
                             $('#selectAllDepartments').prop('checked', allChecked);
                             logSelectedDepartmentIds();
                         });

                         function logSelectedDepartmentIds() {
                             selectedIds = [];
                             $('.departmentCheckbox:checked').each(function () {
                                 selectedIds.push($(this).val());
                             });
                             console.log('Selected Department IDs:', selectedIds);
                         }

                     } else {
                         console.error('Error occurred while fetching data:', response.message);
                     }
                 })
                 .catch(function (error) {
                     $('.loaderCosting').hide();
                     console.error('Error occurred while fetching data:', error);
                 });
         }









         //var selectedIds = [];
         //function GetUserDepartment() {
         //    ApiCall(getUserDepartmentUrl, token)
         //        .then(function (response) {
         //            if (response.statusCode === 200) {
         //                var responseData = response.data;

         //                console.log('department :', responseData);

         //                $('#departmentCheckboxes').empty();

         //                var selectAllHtml = `
         //           <label>
         //               <input type="checkbox" id="selectAllDepartments">
         //               Select All
         //           </label><br>
         //       `;
         //                $('#departmentCheckboxes').append(selectAllHtml);

         //                responseData.forEach(function (department) {
         //                    var checkboxHtml = `
         //               <label>
         //                   <input type="checkbox" name="departments" value="${department.dptId}" class="departmentCheckbox">
         //                   ${department.dptName}
         //               </label><br>
         //           `;
         //                    $('#departmentCheckboxes').append(checkboxHtml);
         //                });

         //                $('#selectAllDepartments').on('change', function () {
         //                    var isChecked = $(this).is(':checked');
         //                    $('.departmentCheckbox').prop('checked', isChecked);
         //                    logSelectedDepartmentIds(); 
         //                });

         //                $('.departmentCheckbox').on('change', function () {
         //                    var allChecked = $('.departmentCheckbox').length === $('.departmentCheckbox:checked').length;
         //                    $('#selectAllDepartments').prop('checked', allChecked);
         //                    logSelectedDepartmentIds();
         //                });

         //                function logSelectedDepartmentIds() {
         //                     selectedIds = [];
         //                    $('.departmentCheckbox:checked').each(function () {
         //                        selectedIds.push($(this).val()); 
         //                    });
         //                    console.log('Selected Department IDs:', selectedIds); // Log selected IDs to console
         //                }

         //                $('.footable-loader').show();
         //            } else {
         //                console.error('Error occurred while fetching data:', response.message);
         //            }
         //        })
         //        .catch(function (error) {
         //            $('.loaderCosting').hide();
         //            console.error('Error occurred while fetching data:', error);
         //        });
         //}



         var selectedPermissionIDsUpdate = [];
         var selectedPermissionIDsRolesWise = [];
         var PreviusPermissionsID = [];
         var isEdit;
         function FetchDataRolesWise(moduleID) {
             ApiCallById(getRolesByIdUrl, token, moduleID)
                 .then(function (response) {
                     var data = response.data;
                     $('#lblHidenRolesId').val(data.userRoleId);
                     if (data.dataAccessLevel == 4) {
                         //var isEdit = true;
                         if (IsEditData == true) {

                             GetUserDepartment(selectedIds, 'update');

                         }
                         else {
                             GetUserDepartment([], 'add');

                         }

                         $('#DataAccessStatus').text('');
                         $('#DataAccessStatus').append('Custom (Department)');

                         $('#departmentCheckboxes').show();
                     }
                     else if (data.dataAccessLevel == 1) {
                         $('#DataAccessStatus').text('');
                         $('#DataAccessStatus').append('Only ME');
                        selectedIds = null;
                         $('#departmentCheckboxes').hide();
                     }
                     else if (data.dataAccessLevel == 2) {
                         $('#DataAccessStatus').text('');
                         $('#DataAccessStatus').append('Own Department');
                         selectedIds = null;
                         $('#departmentCheckboxes').hide();
                     }
                     else {
                         $('#DataAccessStatus').text('');
                         $('#DataAccessStatus').append('All');
                         selectedIds = null;
                         $('#departmentCheckboxes').hide();
                     }
                     // BoxExpland();
                     selectedPermissionIDsRolesWise = [];
                     selectedPermissionIDsRolesWise = JSON.parse(data.permissions);
                     PreviusPermissionsID = [];
                     PreviusPermissionsID = JSON.parse(data.permissions);


                     if (IsEditData) {
                         if (removedPermissions.length > 0 || additionalPermissions.length > 0) {
                             // Remove permissions from selectedPermissionIDsRolesWise
                             selectedPermissionIDsRolesWise = selectedPermissionIDsRolesWise.filter(id => !removedPermissions.includes(id));

                             // Add new permissions to selectedPermissionIDsRolesWise
                             additionalPermissions.forEach(id => {
                                 if (!selectedPermissionIDsRolesWise.includes(id)) {
                                     selectedPermissionIDsRolesWise.push(id);
                                 }
                             });

                         } else {
                             // If no removed or additional permissions, update selectedPermissionIDsRolesWise with existing permissions
                             selectedPermissionIDsRolesWise = JSON.parse(data.permissions);
                         }

                         // Set IsEditData to false
                         IsEditData = false;
                     }

                     if (Array.isArray(selectedPermissionIDsRolesWise)) {
                         var treeData = transformToJSTreeFormats(responseData);
                         $('#treeContainer').jstree("destroy").empty();
                         $('#treeContainer').jstree({
                             'core': {
                                 'data': treeData,
                                 'themes': {
                                     'dots': true
                                 },
                                 'multiple': true,
                                 'animation': true,
                                 'check_callback': true
                             },
                             'checkbox': {
                                 'keep_selected_style': false,
                                 'tie_selection': true
                             },
                             'plugins': ['checkbox', 'wholerow']
                         }).on('ready.jstree', function (e, data) {

                             selectedPermissionIDsRolesWise.forEach(function (id) {
                                 //console.log("id.toString()", id.toString());
                                 data.instance.select_node(id.toString());
                             });
                         }).on('changed.jstree', function (e, data) {
                             selectedPermissionIDsUpdate = [];

                             for (i = 0, j = data.selected.length; i < j; i++) {
                                 var node = data.instance.get_node(data.selected[i]);
                                 if (node && node.children.length === 0) {
                                     selectedPermissionIDsUpdate.push(parseInt(node.id, 10));
                                 }
                             }
                         });
                     } else {
                     }
                 })
                 .catch(function (error) {
                     console.error('Error:', error);
                 });
         }

         

         //var selectedPermissionIDsUpdate = []
         //var selectedPermissionIDsRolesWise = []

         //function FetchDataRolesWise(moduleID) {
         //    ApiCallById(getRolesByIdUrl, token, moduleID)
         //        .then(function (response) {
         //            console.log('Data:', response);
         //            var data = response.data;
         //            $('#lblHidenRolesId').val(data.userRoleId);
         //            //BoxExpland();
         //            selectedPermissionIDs = [];
         //            selectedPermissionIDs = JSON.parse(data.permissions);
         //            console.log('additionalPermissions:',additionalPermissions);
         //            console.log('removedPermissions:', removedPermissions);

         //            if (IsEditData) {
         //                if (removedPermissions.length > 0 || additionalPermissions.length > 0) {
         //                    // Remove permissions from selectedPermissionIDs
         //                    selectedPermissionIDs = selectedPermissionIDs.filter(id => !removedPermissions.includes(id));

         //                    // Add new permissions to selectedPermissionIDs
         //                    additionalPermissions.forEach(id => {
         //                        if (!selectedPermissionIDs.includes(id)) {
         //                            selectedPermissionIDs.push(id);
         //                        }
         //                    });

         //                    console.log('After update - selectedPermissionIDs:', selectedPermissionIDs);
         //                } else {
         //                    // If no removed or additional permissions, update selectedPermissionIDs with existing permissions
         //                    console.log('Only roles data:', selectedPermissionIDs);
         //                    selectedPermissionIDs = JSON.parse(data.permissions);
         //                }

         //                // Set IsEditData to false
         //                IsEditData = false;
         //            }





         //            console.log('Roles data:',selectedPermissionIDs);
         //            if (Array.isArray(selectedPermissionIDs)) {
         //                var treeData = transformToJSTreeFormats(responseData);
         //                $('#treeContainer').jstree("destroy").empty();
         //                $('#treeContainer').jstree({
         //                    'core': {
         //                        'data': treeData,
         //                        'themes': {
         //                            'dots': true
         //                        },
         //                        'multiple': true,
         //                        'animation': true,
         //                        'check_callback': true
         //                    },
         //                    'checkbox': {
         //                        'keep_selected_style': false,
         //                        'tie_selection': true
         //                    },
         //                    'plugins': ['checkbox', 'wholerow']
         //                }).on('ready.jstree', function (e, data) {
                          
         //                    selectedPermissionIDs.forEach(function (id) {
         //                        console.log("id.toString()", id.toString())
         //                        data.instance.select_node(id.toString());
         //                    });
         //                }).on('changed.jstree', function (e, data) {
         //                    selectedPermissionIDsUpdate = [];

         //                    for (i = 0, j = data.selected.length; i < j; i++) {

         //                        var node = data.instance.get_node(data.selected[i]);
         //                        if (node && node.children.length === 0) {
         //                            selectedPermissionIDsUpdate.push(parseInt(node.id, 10));
         //                        }
         //                        //console.log('node:', node);
         //                    }
         //                    console.log('roles Data and Additional Data :', selectedPermissionIDsUpdate)
         //                });
         //            } else {
         //                console.error('responseData.features is not an array:', responseData.features);
         //            }
         //        })
         //        .catch(function (error) {
         //            console.error('Error:', error);
         //        });
         //}
         function transformToJSTreeFormats(data) {
             return data.map(function (item) {

                 return {
                     "id": item.isPermission,
                     "text": item.name,
                     "state": {
                         "opened": true
                     },
                     "children": item.children && item.children.length > 0 ? transformToJSTreeFormat(item.children) : [],
                     "li_attr": {
                         "id": item.isPermission 
                     },
                     "original": {
                         "isPermission": item.isPermission
                     }
                 };
             });
         }

         function findDifferences(array1, array2) {
             return array1.filter(item => !array2.includes(item));
         }
         async function PostUsers(IsSave) {
             var refaranceEmp = $('#ddlReferenceEmp').val();
             var Company = $('#ddlCompany').val();
             var firstName = $('#txtFirstName').val().trim();
             var lastName = $('#txtLastName').val().trim();
             var userName = $('#txtUserName').val().trim();
             var userPassword = $('#txtUserPassword').val().trim();
             var userEmail = $('#txtUserEmail').val().trim();
             var userRole = parseInt($('#ddlUserRole').val());
             var isActive = $('#chkIsActive').is(':checked');
             var isGuest = $('#chkIsGetUser').is(':checked');
             var isAuthPerm = $('#chkIsAuthPerm').is(':checked');
             var dataAccessPermission = JSON.stringify(selectedIds);
             var jsonRolesData = JSON.stringify(PreviusPermissionsID);
             var jsonUpdateData = JSON.stringify(selectedPermissionIDsUpdate);

             var rolesDataArray = JSON.parse(jsonRolesData);
             var updateDataArray = JSON.parse(jsonUpdateData);
             var removedItems = findDifferences(rolesDataArray, updateDataArray);
             var addedItems = findDifferences(updateDataArray, rolesDataArray);

             if (removedItems.length > 0 || addedItems.length > 0) {
                 rolesDataArray = rolesDataArray.filter(item => !removedItems.includes(item));
             }

             var additionalPermissions = addedItems.length > 0 ? JSON.stringify(addedItems) : "";
             var removedPermissions = removedItems.length > 0 ? JSON.stringify(removedItems) : "";

             var postData = {
                 CompanyId: Company,
                 FirstName: firstName,
                 LastName: lastName,
                 UserName: userName,
                 UserPassword: userPassword,
                 Email: userEmail,
                 UserRoleID: userRole,
                 IsGuestUser: isGuest,
                 IsApprovingAuthority: isAuthPerm,
                 ReferenceID: refaranceEmp,
                 AdditionalPermissions: additionalPermissions,
                 RemovedPermissions: removedPermissions,
                 dataAccessPermission: dataAccessPermission,
                 IsActive: isActive,
             };

             try {
                 const response = await ApiCallPost(createUserUrl, token, postData);

                 if (response.statusCode === 200) {
                     Swal.fire({
                         icon: 'success',
                         title: 'Success',
                         text: 'Data saved successfully!'
                     }).then((result) => {
                         if (result.isConfirmed) {
                             GetUsers();  // Update user list or perform necessary actions
                         }
                     });
                     return true; // Indicate success
                 } else if (response.statusCode === 400) {
                     Swal.fire({
                         icon: 'error',
                         title: 'Username Exists',
                         text: 'This Username already exists. Please choose a different username.'
                     });
                     return false; // Username exists, return false
                 } else {
                     Swal.fire({
                         icon: 'error',
                         title: 'Error',
                         text: 'An error occurred. Please try again.'
                     });
                     return false; // Some other error occurred
                 }
             } catch (error) {
                 Swal.fire({
                     icon: 'error',
                     title: 'Error',
                     text: 'Failed to save data. Please try again.'
                 });
                 return false; // Return false on error
             }
         }

        
         async function updateUsers(IsSave) {
             try {
                 // Gather form values
                 var userId = $('#lblHidenUserId').val();
                 var refaranceEmp = $('#ddlReferenceEmp').val();
                 var Company = $('#ddlCompany').val();
                 var firstName = $('#txtFirstName').val().trim();
                 var lastName = $('#txtLastName').val().trim();
                 var userName = $('#txtUserName').val().trim();
                 var userPassword = $('#txtUserPassword').val().trim();
                 var userEmail = $('#txtUserEmail').val().trim();
                 var userRole = parseInt($('#ddlUserRole').val());
                 var isActive = $('#chkIsActive').is(':checked');
                 var isGuest = $('#chkIsGetUser').is(':checked');
                 var isAuthPerm = $('#chkIsAuthPerm').is(':checked');
                 var dataAccessPermission = JSON.stringify(selectedIds);

                 // Serialize permission data
                 var jsonRolesData = JSON.stringify(PreviusPermissionsID);
                 var jsonUpdateData = JSON.stringify(selectedPermissionIDsUpdate);

                 // Parse JSON data into arrays
                 var rolesDataArray = JSON.parse(jsonRolesData);
                 var updateDataArray = JSON.parse(jsonUpdateData);

                 // Find differences (removed and added permissions)
                 var removedItems = findDifferences(rolesDataArray, updateDataArray);
                 var addedItems = findDifferences(updateDataArray, rolesDataArray);

                 // Filter roles data to remove removed items
                 if (removedItems.length > 0 || addedItems.length > 0) {
                     rolesDataArray = rolesDataArray.filter(item => !removedItems.includes(item));
                 }

                 // Convert added and removed permissions back to JSON
                 var additionalPermissions = addedItems.length > 0 ? JSON.stringify(addedItems) : "";
                 var removedPermissions = removedItems.length > 0 ? JSON.stringify(removedItems) : "";

                 // Prepare the update data object
                 var updateData = {
                     CompanyId: Company,
                     FirstName: firstName,
                     LastName: lastName,
                     UserName: userName,
                     UserPassword: userPassword,
                     Email: userEmail,
                     UserRoleID: userRole,
                     IsGuestUser: isGuest,
                     isApprovingAuthority: isAuthPerm,
                     ReferenceID: refaranceEmp,
                     AdditionalPermissions: additionalPermissions,
                     RemovedPermissions: removedPermissions,
                     dataAccessPermission: dataAccessPermission,
                     IsActive: isActive
                 };

                 // Make API call to update user
                 const response = await ApiCallUpdate(updateUserUrl, token, updateData, userId);

                 // Handle API response
                 if (response.statusCode === 200) {
                     Swal.fire({
                         icon: 'success',
                         title: 'Success',
                         text: 'Data saved successfully!'
                     }).then((result) => {
                         if (result.isConfirmed) {
                             GetUsers(); // Refresh user list or other necessary actions
                         }
                     });
                     return true; // Indicate success
                 } else if (response.statusCode === 400) {
                     Swal.fire({
                         icon: 'error',
                         title: 'Username Exists',
                         text: 'This Username already exists. Please choose a different username.'
                     });
                     return false; // Username exists, indicate failure
                 } else {
                     Swal.fire({
                         icon: 'error',
                         title: 'Error',
                         text: 'An error occurred. Please try again.'
                     });
                     return false; // General error, indicate failure
                 }
             } catch (error) {
                 // Handle any unexpected errors
                 Swal.fire({
                     icon: 'error',
                     title: 'Error',
                     text: 'Failed to save data. Please try again.'
                 });
                 return false; // Indicate failure
             }
         }




         var additionalPermissions = [];
         var removedPermissions = [];

         var IsEditData =false;

         function FetchDataForEdit(moduleId) {
             // Make API call to fetch user data based on moduleId
             ApiCallById(getUsersUrl, token, moduleId)
                 .then(function (responseData) {
                     var data = responseData.data;
                     IsEditData = true; // Set edit mode

                     // Populate form fields with fetched data
                     $('#lblHidenUserId').val(data.userId);
                     $('#txtFirstName').val(data.firstName);
                     $('#txtLastName').val(data.lastName);
                     $('#txtUserName').val(data.userName);
                     $('#txtUserPassword').val(data.userPassword);
                     $('#txtUserEmail').val(data.email);
                     $('#chkIsActive').prop('checked', data.isActive);
                     $('#chkIsGetUser').prop('checked', data.isGuestUser);
                     $('#chkIsAuthPerm').prop('checked', data.isApprovingAuthority);
                     $('select[name="ddlReferenceEmp"]').val(data.referenceID).change();
                     $('#ddlCompany').val(data.companyId).change();

                     // Handle permissions and roles
                     additionalPermissions = [];
                     removedPermissions = [];
                     selectedIds = JSON.parse(data.dataAccessPermission);

                     if (data.additionalPermissions.length > 0) {
                         additionalPermissions = JSON.parse(data.additionalPermissions);
                     }

                     if (data.removedPermissions.length > 0) {
                         removedPermissions = JSON.parse(data.removedPermissions);
                     }

                     const GuestUser = data.isGuestUser;
                     const isChecked = $('#chkIsGetUser').is(':checked');

                     // Handle GuestUser specific logic
                     if (GuestUser) {
                         if (!isChecked) {
                             $('#chkIsGetUser').prop('checked', true);
                         }
                         $('#FirstName, #LastName').show(); // Show first name and last name fields

                         // Fetch roles and set the user role after fetching
                         GetRoles(true).then(() => {
                             $('select[name="ddlUserRole"]').val(data.userRoleID).change(); // Set the user's role
                         }).catch(error => {
                             console.error('Failed to fetch roles:', error); // Handle role fetching errors
                         });

                         $('#ddlReference').hide(); // Hide reference field
                     } else {
                         if (isChecked) {
                             $('#chkIsGetUser').prop('checked', false);
                         }
                         $('#FirstName, #LastName').hide(); // Hide first name and last name fields

                         // Fetch roles and set the user role after fetching
                         GetRoles(false).then(() => {
                             $('select[name="ddlUserRole"]').val(data.userRoleID).change(); // Set the user's role
                         }).catch(error => {
                             console.error('Failed to fetch roles:', error); // Handle role fetching errors
                         });

                         $('#ddlReference').show(); // Show reference field
                     }

                     console.log(GuestUser);

                     // Change the button label to 'Update'
                     $('#btnSave').html('Update');

                     BoxExpland(); // Additional UI logic if needed
                 })
                 .catch(function (error) {
                     console.error('Error:', error); // Handle API errors
                 });
         }


         function FetchDataForView(moduleId) {
             ApiCallById(getUsersUrl, token, moduleId)
                 .then(function (responseData) {
                     var data = responseData.data;
                     Swal.fire({
                         title: 'User Information',
                         html: `
                    <strong>UserName:</strong> ${data.userName}<br>
                    <strong>Password:</strong> ${data.userPassword}
                `,
                         icon: 'info',
                         confirmButtonText: 'OK'
                     });
                     $('#txtUserName').val(data.userName);
                     $('#txtUserPassword').val(data.userPassword);
                 })
                 .catch(function (error) {
                     console.error('Error:', error);
                 });
         }




         function Delete(id) {
             Swal.fire({
                 title: 'Are you sure?',
                 text: "Do you really want to delete this Packages?",
                 icon: 'warning',
                 showCancelButton: true,
                 confirmButtonColor: '#3085d6',
                 cancelButtonColor: '#d33',
                 confirmButtonText: 'Yes, delete it!'
             }).then((result) => {
                 if (result.isConfirmed) {
                     ApiDeleteById(DeleteUserUrl, token, id)
                         .then(function (response) {
                             Swal.fire({
                                 title: 'Success!',
                                 text: 'Packages deleted successfully.',
                                 icon: 'success',
                                 confirmButtonText: 'OK'
                             }).then(() => {
                                 GetUsers();
                             });
                         })
                         .catch(function (error) {
                             Swal.fire({
                                 title: 'Error!',
                                 text: 'An error occurred while deleting the module.',
                                 icon: 'error',
                                 confirmButtonText: 'OK'
                             });
                         });
                 }
             });
         }




        function BoxExpland() {
            var scrollTop = $(window).scrollTop();

            $("#Cardbox").show();
            $("#addnew").text("Close");
             $("#IsGuest").show();
            $(window).scrollTop(scrollTop);
        }

         function Cardbox() {
             var CardboxElement = $("#Cardbox");
             var addnewElement = $("#addnew");

             if (addnewElement.html() === "Add New") {
                 CardboxElement.show();
                  $("#IsGuest").show();
                 addnewElement.text("Close");
             } else {
                 ClearTextBox();
                 CardboxElement.hide();
                 addnewElement.html("Add New");
                   $("#IsGuest").hide();
             }
              var writeAction = '<%= Session["__WriteAction__"] %>';

                if (writeAction == "0") {
                    $("#btnSave").hide(); 
                } else {
                    $("#btnSave").show(); 
                }
         }

         function ClearTextBox() {
             $('#txtFirstName').val("");
             $('#txtLastName').val("");
             $('#txtUserName').val("");
             $('#txtUserPassword').val("");
             $('#txtUserEmail').val("");
             $('select[name="ddlUserRole"]').val('0').change();
             $('select[name="ddlReferenceEmp"]').val('0').change();
             $('#txtPermissionName').val("");
             $('#txtPermissionsUrl').val("");
             $('#txtPerPhysicalLocation').val("");
             $('#txtPerOrdaring').val("");
             //$('#chkIsActive').prop('checked', false);
             $('#chkIsGetUser').prop('checked', false);
             //$('#chkIsAuthPerm').prop('checked', false);
             $('#btnSave').text("Save");
         }




     </script>
    <style>

        
        .custom-permission-icon {
            color: #8231d3;
        }

        .custom-module-icon {
            color: #d331d3; 
        }

        #treeSection {
            height:50vh;
            overflow: auto;
            margin-bottom:10px;
            /*background-color:#f0f8ff;*/
            
        }
        
    </style>
<%--    <script src="assets/theme_assets/js/TreeViewHepler.js"></script>--%>

    <script src="/hrms/assets/theme_assets/js/apiHelper.js"></script>

</asp:Content>
