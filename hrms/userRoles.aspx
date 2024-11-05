<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="userRoles.aspx.cs" Inherits="SigmaERP.hrms.userRoles" %>
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
                                    <h4>Add Role</h4>
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
                                     <div class="col-lg-8">
                                            <div class="row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label id="lblHidenRolesId" style="display: none"></label>

                                                <label for="txtRole" class="color-dark fs-14 fw-500 align-center mb-10">
                                                    Role Name <span class="text-danger">*</span>
                                                </label>
                                                <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtRole" placeholder="Type Role Name">
                                                <span class="text-danger" id="errortxtRole"></span>
                                            </div>
                                        </div>

                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <label for="txtOrdaring" class="color-dark fs-14 fw-500 align-center mb-10">
                                                    Ordering <span class="text-danger">*</span>
                                                </label>
                                                <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtOrdaring" placeholder="Type Ordering">
                                                <span class="text-danger" id="orderingError"></span>
                                            </div>
                                        </div>
                                        <div class="col-lg-3 " style="display: flex; justify-content: space-between">
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
                                                <label style="opacity: 0;" for="formGroupExampleInput"
                                                    class="color-dark fs-14 fw-500 align-center mb-10">
                                                    Name <span
                                                        class="text-danger"></span>
                                                </label>
                                                <button type="button" id="btnSave" onclick="ValidateAndPostModule()"
                                                    class="btn btn-primary btn-default btn-squared px-30">
                                                    Save</button>
                                            </div>

                                        </div>
                                        <div class="col-lg-4" id="">
                                            <div class="form-group">
                                                <label for="ddlDataAccessLevel" class="color-dark fs-14 fw-500 align-center mb-10">Data Access Level <span class="text-danger">*</span></label>
                                                <div class="support-form__input-id">
                                                    <div class="dm-select ">
                                                        <select name="ddlDataAccessLevel" id="ddlDataAccessLevel" class="select-search form-control">
                                                            <option value="0">---Select---</option>
                                                            <option value="3">All</option>
                                                            <option value="2">Own Department</option>
                                                            <option value="1">Only Me</option>
                                                            <option value="4">Custom</option>
                                                        </select>
                                                    </div>
                                                    <span class="text-danger" id="errorDataAccessLevel"></span>
                                                </div>
                                            </div>
                                        </div>
                                       </div>
                                    </div>




                                       <div class="col-lg-4 " id="treeSection">
                                        <p>Select permission</p>
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
                                    </div>




                         





                            </div>
                        </div>
                    </div>
                </div>

                    <div class="row">
               <div class="col-lg-12">
                  <div class="card ">
                     <div class="card-body">

                        <div class="userDatatable adv-table-table global-shadow border-light-0 w-100 ">
                           <div class="table-responsive">
                              <div class="ad-table-table__header d-flex justify-content-between">
                                  <h4 style="margin-top: 13px;">Roles</h4>
                              <div id="filter-form-container">

                              </div>
                              </div>
                               <table class="table mb-0 packagesTable table-borderless adv-table" data-sorting="true" data-filtering="true" data-filter-container="#filter-form-container" data-paging="true" data-paging-size="10">
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
        //var rootUrl = 'http://localhost:5081';

        var rootUrl = '<%= Session["__RootUrl__"]%>';
        var getStpPkgFeaturesWithParentUrl = rootUrl + '/api/UserPackagesSetup/SetupedPackagesWithParent';
        var postRolesUrl = rootUrl + '/api/UserRoles/create';
         var getRolesUrl = rootUrl + '/api/UserRoles/userRoles';
         var getRolesByIdUrl = rootUrl + '/api/UserRoles/userRoles';
         var updateRolesUrl = rootUrl + '/api/UserRoles/update';
         var DeleteRoleUrl = rootUrl + '/api/UserRoles/delete';
        var getStpPkgFeaturesUrl = rootUrl + '/api/UserPackagesSetup/SetupPackage';
        var token = '<%= Session["__UserToken__"] %>';
        console.log('this is token you can use it :', token);
        //var token = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJpYXQiOjE3MTQ2MjQ5MjYsImV4cCI6MTc0NjE2MDkyNiwiYXVkIjoiIiwic3ViIjoiSldUU2VydmljZUFjY2Vzc1Rva2VuIn0.tVlIuOLas2VxEnBohuaIXXQR2Lju_2h8yVjCDizQh9o';

        $(document).ready(function () {
           
            //GetModule();
            GetStpPkgFeatures();
            GetRoles();
            //$('#ddlDataAccessLevel').change(function () {
            //    var selectedValue = $(this).val();

            //    // If "Department wise" option is selected (value = 3)
            //    if (selectedValue == '3') {
            //        GetUserDepartment();
            //    }
            //});
            
        });

        function Cardbox() {
            var CardboxElement = $("#Cardbox");
            var addnewElement = $("#addnew");

            if (addnewElement.html() === "Add New") {
                CardboxElement.show();
                addnewElement.text("Close");
            } else {
                ClearTextBox();
                CardboxElement.hide();
                addnewElement.html("Add New");

            }
        }
        function ClearTextBox() {
            $('#txtRole').val("");
            $('#txtOrdaring').val("");
            $('#chkIsActive').prop('checked', true);
            $('#btnSave').text("Save");
        }

         function ValidateAndPostModule() {
            var isValid = true;
            if ($('#txtRole').val().trim() === "") {
                $('#errortxtRole').html("Packages Name is required.");
                $("#txtRole").focus();
                isValid = false;
            } else {
                $('#errortxtRole').html("");
            }
            if ($('#txtOrdaring').val().trim() === "" || isNaN($('#orderingError').val())) {
                $('#orderingError').html("Ordering is required and must be a number.");
                $("#txtOrdaring").focus();
                isValid = false;
            } else {
                $('#orderingError').html("");
             }

             let selectedRole = $('#ddlDataAccessLevel').val();
             if (selectedRole == "0") {
                 $('#errorDataAccessLevel').html("Please select Data Access Level.");
                 $("#ddlDataAccessLevel").focus();
                 isValid = false;
             } else {
                 $('#errorDataAccessLevel').html("");
             }

             if (isValid) {
                 var addnewElement = $("#btnSave").text().trim();
                 if (addnewElement === "Save") {
                     PostStpPkgFeatures();
                 }
                 else {
                     updateRoles(); 

                 }
             }
        }


         function PostStpPkgFeatures() {
            var roleName = $('#txtRole').val();
             var ordering = parseInt($('#txtOrdaring').val());
            var dataAccessLevel = parseInt($('#ddlDataAccessLevel').val());
            var isActive = $('#chkIsActive').is(':checked');
            var treeInstance = $('#treeContainer').jstree(true);

            var jsonString = JSON.stringify(selectedPermissionIDs);
            console.log(jsonString);

            var postData = {
                UserRole: roleName,
                dataAccessLevel: dataAccessLevel,
                Permissions: jsonString,
                Ordering: ordering,
                IsActive: isActive
 
            };

            ApiCallPost(postRolesUrl, token, postData)
                .then(function (response) {
                    console.log('Data saved successfully:', response);
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Data saved successfully!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            //GetModule();
                            GetRoles();
                            //GetPackages();
                        }
                    });
                })
                .catch(function (error) {
                    console.error('Error saving data:', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Failed to save data. Please try again.'
                    });
                });
        }

         function updateRoles() {
                var roleId = $('#lblHidenRolesId').val();
                var txtRole = $('#txtRole').val();
                var dataAccessLevel = parseInt($('#ddlDataAccessLevel').val());

                var ordering = parseInt($('#txtOrdaring').val());
                var isActive = $('#chkIsActive').is(':checked');
                var treeInstance = $('#treeContainer').jstree(true);

                var jsonString = JSON.stringify(selectedPermissionIDsUpdate);
                console.log(jsonString);

                var updateData = {
                    UserRole: txtRole,
                    dataAccessLevel: dataAccessLevel,
                    Permissions: jsonString,
                    Ordering: ordering,
                    IsActive: isActive
                };

            ApiCallUpdate(updateRolesUrl, token, updateData, roleId)
                .then(function (response) {
                    console.log('Data updated successfully:', response);
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Data updated successfully!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            GetRoles();
                            //GetPackages();
                        }
                    });
                })
                .catch(function (error) {
                    console.error('Error updating data:', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Failed to update data. Please try again.'
                    });
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
                    ApiDeleteById(DeleteRoleUrl, token, id)
                        .then(function (response) {
                            Swal.fire({
                                title: 'Success!',
                                text: 'Packages deleted successfully.',
                                icon: 'success',
                                confirmButtonText: 'OK'
                            }).then(() => {
                                GetRoles();
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





        function GetRoles() {
            ApiCall(getRolesUrl, token)
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
       // This function binds data to the table with serial numbers and user role-specific actions
        function bindTableData(data) {

            // Step 1: Destroy any existing Footable instance to avoid conflicts when reinitializing
            if ($('.adv-table').data('footable')) {
                $('.adv-table').data('footable').destroy();
            }

            // Step 2: Clear the HTML content of the table and filter container
            $('.adv-table').html('');
            $('#filter-form-container').empty();

            // Step 3: Loop through the data array and modify the rows for display
            data.forEach((row, index) => {
                // Assign a serial number based on the loop index
                row.serialNo = index + 1;

                // Create the HTML for userRoleName column including actions (View, Edit, Delete)
                row.userRoleName = `
            <div class="permission-name-container">
                ${row.userRoleName}
                <div class="actions-container">
                    <ul class="orderDatatable_actions mb-0 d-flex flex-wrap">
                        <li><a href="javascript:void(0)" class="view-btn view" data-id="${row.userRoleId}"><i class="uil uil-eye"></i></a></li>
                        <li><a href="javascript:void(0)" data-id="${row.userRoleId}" class="edit-btn edit"><i class="uil uil-edit"></i></a></li>
                        <li><a href="javascript:void(0)" data-id="${row.userRoleId}" class="delete-btn remove"><i class="uil uil-trash-alt"></i></a></li>
                    </ul>
                </div>
            </div>
        `;

                // Create the HTML for isActive column with a toggle switch
                row.isActive = `
            <div class="form-check form-switch form-switch-primary form-switch-sm">
                <input type="checkbox" class="form-check-input" id="switch-${row.userRoleId}" ${row.isActive ? 'checked' : ''}>
                <label class="form-check-label" for="switch-${row.userRoleId}"></label>
            </div>
        `;

                // Create the HTML for permissions column with a feature icon
                row.permissions = `
            <div class="features-icon-container">
                <a href="javascript:void(0)" class="feature-btn" data-id="${row.userRoleId}"><i class="uil uil-star"></i></a>
            </div>
        `;

                // Step 4: Conditionally modify the Data Access Level column (1: All, 2: Only Me)
                row.dataAccessLevel = row.dataAccessLevel === 1 ? 'Only Me' : row.dataAccessLevel === 2 ? 'Own Department' : row.dataAccessLevel === 3 ? 'All' : row.dataAccessLevel === 4 ? 'Custom' : 'NA';
            });

            // Step 5: Define the columns for the table, including custom rendering logic for some columns
            const columns = [
                { "name": "serialNo", "title": "SL", "breakpoints": "xs sm", "type": "number", "className": "userDatatable-content" }, // Serial number column
                { "name": "userRoleName", "title": "Role Name", "className": "userDatatable-content" },
                { "name": "permissions", "title": "Permissions", "className": "userDatatable-content" },
                { "name": "dataAccessLevel", "title": "Data Access Level", "className": "userDatatable-content" },
                { "name": "isActive", "title": "Is Active", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                { "name": "ordering", "title": "Ordering", "type": "number", "className": "userDatatable-content" },
            ];

            // Step 6: Initialize Footable with the columns and data, enable filtering
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
                    // Hide loader after table initialization
                    $('.footable-loader').hide();
                });
            } catch (error) {
                console.error("Error initializing Footable:", error);
            }

            // Step 7: Attach event listeners for actions (Edit, Delete, View, Feature)

            // Clear and re-attach the edit button click event
            $('.adv-table').off('click', '.edit-btn').on('click', '.edit-btn', function () {
                const userRoleId = $(this).data('id');
                FetchDataForEdit(userRoleId); // Custom function to handle edit logic
                console.log('Edit button clicked for userRoleId:', userRoleId);
            });

            // Clear and re-attach the delete button click event
            $('.adv-table').off('click', '.delete-btn').on('click', '.delete-btn', function () {
                const userRoleId = $(this).data('id');
                Delete(userRoleId); // Custom function to handle delete logic
                console.log('Delete button clicked for userRoleId:', userRoleId);
            });

            // Clear and re-attach the view button click event
            $('.adv-table').off('click', '.view-btn').on('click', '.view-btn', function () {
                const userRoleId = $(this).data('id');
                console.log('View button clicked for userRoleId:', userRoleId);
                // You can add your view logic here
            });

            // Clear and re-attach the feature button click event
            $('.adv-table').off('click', '.feature-btn').on('click', '.feature-btn', function () {
                const userRoleId = $(this).data('id');
                console.log('Feature button clicked for userRoleId:', userRoleId);
                // Add logic for feature handling
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

       

        function BoxExpland() {
            var scrollTop = $(window).scrollTop();

            $("#Cardbox").show();
            $("#addnew").text("Close");
            $(window).scrollTop(scrollTop);
        }


        var selectedPermissionIDsUpdate = []

        function FetchDataForEdit(moduleID) {
            ApiCallById(getRolesByIdUrl, token, moduleID)
                .then(function (response) {
                    console.log('Data:', response);
                    var data = response.data;
                    $('#lblHidenRolesId').val(data.userRoleId);
                    $('#txtRole').val(data.userRoleName);
                    $('select[name="ddlDataAccessLevel"]').val(data.dataAccessLevel).change();

                    $('#txtOrdaring').val(data.ordering);
                    $('#chkIsActive').prop('checked', data.isActive);
                    $('#btnSave').html('Update');
                    BoxExpland();
                    var selectedPermissionIDs = JSON.parse(data.permissions);
                    if (Array.isArray(selectedPermissionIDs)) {
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
                            selectedPermissionIDs.forEach(function (id) {
                                console.log("id.toString()", id.toString())
                                data.instance.select_node(id.toString());
                            });
                        }).on('changed.jstree', function (e, data) {
                            selectedPermissionIDsUpdate = [];

                            for (i = 0, j = data.selected.length; i < j; i++) {

                                var node = data.instance.get_node(data.selected[i]);
                                if (node && node.children.length === 0) {
                                    selectedPermissionIDsUpdate.push(parseInt(node.id, 10));
                                }
                                console.log('node:', node);
                            }
                            console.log('petch Child Node IDs:', selectedPermissionIDsUpdate)
                        });
                    } else {
                        console.error('responseData.features is not an array:', responseData.features);
                    }
                })
                .catch(function (error) {
                    console.error('Error:', error);
                });
        }


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

      





    </script>

<%--    <script src="assets/theme_assets/js/TreeViewHepler.js"></script>--%>

   <script src="/hrms/assets/theme_assets/js/apiHelper.js"></script>

</asp:Content>
