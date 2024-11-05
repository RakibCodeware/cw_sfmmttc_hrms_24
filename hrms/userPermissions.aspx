<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="userPermissions.aspx.cs" Inherits="SigmaERP.hrms.userPermissions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">  <main class="main-content">
      <div class="Dashbord">
         <div class="crm mb-25">
            <div class="container-fulid">
               <div class="card card-Vertical card-default card-md mt-4 mb-4">

                  <div class="card-header d-flex align-items-center">
                     <div class="card-title d-flex align-items-center justify-content-between">
                        <div class="d-flex align-items-center gap-3">
                           <h4>Add Permission</h4>
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
                               <div class="col-lg-3">
                                   <div class="form-group">
                                        <label id="lblHidenPermissionId" style="display:none"></label>
                                       <label for="formGroupExampleInput" class="color-dark fs-14 fw-500 align-center mb-10">Module <span
                                          class="text-danger">*</span></label>
                                       <div class="support-form__input-id">
                                           <div class="dm-select ">
                                               <select name="ddlModule" id="ddlModule" class="select-search form-control">
                                                   <option value="0">---Select---</option>
                                               </select>
                                           </div>
                                       </div>
                                   </div>
                               </div>
                              <div class="col-lg-3">
                                 <div class="form-group">
                                    <label for="formGroupExampleInput"
                                       class="color-dark fs-14 fw-500 align-center mb-10">Permission Name <span
                                          class="text-danger">*</span></label>
                                    <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15"
                                       id="txtPermissionName" placeholder="Type Permission Name ">
                                        <span class="text-danger" id="PermissionError"></span>
                                 </div>
                              </div>
                              <div class="col-lg-3">
                                 <div class="form-group">
                                    <label for="formGroupExampleInput"
                                       class="color-dark fs-14 fw-500 align-center mb-10">Permission Url<span
                                          class="text-danger">*</span></label>
                                    <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15"
                                       id="txtPermissionsUrl" placeholder="Type permission Url">
                                      <span class="text-danger" id="UrlError"></span>
                                 </div>
                              </div>
                              <div class="col-lg-3">
                                 <div class="form-group">
                                    <label for="formGroupExampleInput"
                                       class="color-dark fs-14 fw-500 align-center mb-10">Physical Location <span
                                          class="text-danger">*</span></label>
                                    <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15"
                                       id="txtPerPhysicalLocation" placeholder="Type PhysicalLocation">
                                      <span class="text-danger" id="perPhysicalLocationError"></span>
                                 </div>
                              </div>
                               <div class="col-lg-3">
                                   <div class="form-group">
                                       <label for="formGroupExampleInput"
                                           class="color-dark fs-14 fw-500 align-center mb-10">
                                            Ordaring <span
                                               class="text-danger">*</span></label>
                                       <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15"
                                           id="txtPerOrdaring" placeholder="Type Ordaring">
                                        <span class="text-danger" id="OrderingError"></span>
                                   </div>
                               </div>
                               <div class="col-lg-3">
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
                               <div class="col-lg-3">
                                 <label style="opacity: 0;" for="formGroupExampleInput"
                                    class="color-dark fs-14 fw-500 align-center mb-10">Name <span
                                       class="text-danger"></span></label>
                                 <button type="button" id="btnSave" onclick="ValidateAndPostModule()"
                                    class="btn btn-primary btn-default btn-squared px-30">Save</button>
                              </div>
                           </div>
             
                     </div>
                  </div>
               </div>
            </div>
               <!-- Department List  -->
                        <div class="row">
               <div class="col-lg-12 mb-30">
                  <div class="card">
                     <div class="card-body">

                        <div class="userDatatable adv-table-table global-shadow border-light-0 w-100 ">
                           <div class="table-responsive">
                              <div class="ad-table-table__header d-flex justify-content-between">
                                  <h4 style="margin-top: 13px;">Permissions</h4>
                              <div id="filter-form-container">

                            

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
        //function Cardbox() {
        //    $("#Cardbox").toggle();
        //    var currentText = $("#addnew").text();
        //    var newText = currentText === "Close" ? "Add New" : "Close";
        //    $("#addnew").text(newText);
        //}

        var token = '<%= Session["__UserToken__"] %>';
        console.log('this is token you can use it :', token);
        //var rootUrl = 'http://localhost:5081';
        var rootUrl = '<%= Session["__RootUrl__"]%>';
        var GetByIdPermissionUrl = rootUrl + '/api/UserPermissions/permissions';
        var GetModuleForDdlUrl = rootUrl + '/api/UserModules/modules';
        var GetPermissioneUrl = rootUrl + '/api/UserPermissions/permissions';
        var PostPermissionUrl = rootUrl + '/api/UserPermissions/permissions/create';
        var updatePermissioneUrl = rootUrl + '/api/UserPermissions/permissions/update';
        var DeleteUrl = rootUrl + '/api/UserPermissions/permissions/delete';

        //var token = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJpYXQiOjE3MTQ2MjQ5MjYsImV4cCI6MTc0NjE2MDkyNiwiYXVkIjoiIiwic3ViIjoiSldUU2VydmljZUFjY2Vzc1Rva2VuIn0.tVlIuOLas2VxEnBohuaIXXQR2Lju_2h8yVjCDizQh9o';

        function GetPermission() {
            ApiCall(GetPermissioneUrl, token)
                .then(function (response) {
                    if (response.statusCode === 200) {
                        var responseData = response.data;
                        $('.footable-loader').show();
                        BindTableData(responseData);
                    } else {
                        console.error('Error occurred while fetching data:', response.message);
                    }
                })
                .catch(function (error) {
                    $('.loaderCosting').hide();
                    console.error('Error occurred while fetching data:', error);
                });
        }

        function GetModule() {
            ApiCall(GetModuleForDdlUrl, token)
                .then(function (response) {
                    if (response.statusCode === 200) {
                        var responseData = response.data;
                        populateDropdown(responseData);
                    } else {
                        console.error('Error occurred while fetching data:', response.message);
                    }
                })
                .catch(function (error) {
                    console.error('Error occurred while fetching data:', error);
                });
        }

        function populateDropdown(data) {
            const dropdown = document.getElementById('ddlModule');
            dropdown.innerHTML = '<option value="0">---Select---</option>'; // Clear existing options

            data.forEach(item => {
                const option = document.createElement('option');
                option.value = item.moduleId;
                option.textContent = item.moduleName;
                dropdown.appendChild(option);
            });
        }
        function BindTableData(data) {
            if ($('.adv-table').data('footable')) {
                $('.adv-table').data('footable').destroy();
            }

            $('.adv-table').html('');
            $('#filter-form-container').empty();

            let serialNumber = 1; // Initialize serial number

            data.forEach(row => {
                row.serialNumber = serialNumber++; // Auto-increment serial number

                row.permissionName = `
            <div class="permission-name-container">
                ${row.permissionName}
                <div class="actions-container">
                    <ul class="orderDatatable_actions mb-0 d-flex flex-wrap">
                        <li><a href="javascript:void(0)" class="view-btn view" data-id="${row.permissionId}"><i class="uil uil-eye"></i></a></li>
                        <li><a href="javascript:void(0)" data-id="${row.permissionId}" class="edit-btn edit"><i class="uil uil-edit"></i></a></li>
                        <li><a href="javascript:void(0)" data-id="${row.permissionId}" class="delete-btn remove"><i class="uil uil-trash-alt"></i></a></li>
                    </ul>
                </div>
            </div>
        `;

                row.isActive = `
            <div class="form-check form-switch form-switch-primary form-switch-sm">
                <input type="checkbox" class="form-check-input" id="switch-${row.permissionId}" ${row.isActive ? 'checked' : ''}>
                <label class="form-check-label" for="switch-${row.permissionId}"></label>
            </div>
        `;
            });

            const columns = [
                { "name": "serialNumber", "title": "SL", "type": "number", "className": "userDatatable-content" }, // New serial number column
                { "name": "permissionId", "title": "ID", "breakpoints": "xs sm", "type": "number", "className": "userDatatable-content" },
                { "name": "permissionName", "title": "Name", "className": "userDatatable-content custom-td-width" },
                { "name": "moduleName", "title": "ModuleName", "className": "userDatatable-content" },
                { "name": "url", "title": "URL", "className": "userDatatable-content" },
                { "name": "physicalLocation", "title": "Physical Location", "className": "userDatatable-content" },
                { "name": "ordering", "title": "Ordering", "type": "number", "className": "userDatatable-content" },
                { "name": "isActive", "title": "Is Active", "sortable": false, "filterable": false, "className": "userDatatable-content" }
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
                const id = $(this).data('id');
                FetchDataForEdit(id);
                console.log('Edit button clicked for ID:', id);
            });

            $('.adv-table').off('click', '.delete-btn').on('click', '.delete-btn', function () {
                const id = $(this).data('id');
                Delete(id);
                console.log('Delete button clicked for ID:', id);
            });

            $('.adv-table').off('click', '.view-btn').on('click', '.view-btn', function () {
                const id = $(this).data('id');
                // Handle the view action
                console.log('View button clicked for ID:', id);
            });
        }


        function ValidateAndPostModule() {
            var isValid = true;
            if ($('#txtPermissionName').val().trim() === "") {
                $('#PermissionError').html("Module Name is required.");
                $("#txtPermissionName").focus();
                isValid = false;
            } else {
                $('#PermissionError').html("");
            }

            if ($('#txtPerOrdaring').val().trim() === "" || isNaN($('#txtPerOrdaring').val())) {
                $('#OrderingError').html("Ordering is required and must be a number.");
                $("#txtPerOrdaring").focus();
                isValid = false;
            } else {
                $('#OrderingError').html("");
            }
            if (isValid) {
                var addnewElement = $("#btnSave");
                if (addnewElement.html() === "Save") {
                    PostModule();
                    ClearTextBox();
                }
                else {
                    UpdateModule();
                    ClearTextBox();
                }
            }
        }
        function ClearTextBox() {
            
            $('select[name="ddlModule"]').val('0').change();
            $('#txtPermissionName').val("");
            $('#txtPermissionsUrl').val("");
            $('#txtPerPhysicalLocation').val("");
            $('#txtPerOrdaring').val("");
            $('#chkIsActive').prop('checked', false);
            $('#btnSave').text("Save");
        }

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

        function PostModule() {
            // Capture form data
            var PermissionName = $('#txtPermissionName').val();
           var ModuleID = parseInt($('select[name="ddlModule"]').val());
            var url = $('#txtPermissionsUrl').val();
            var physicalLocation = $('#txtPerPhysicalLocation').val();
            var ordering = parseInt($('#txtPerOrdaring').val());
            var isActive = $('#chkIsActive').is(':checked'); // Changed to boolean

            // Create postData object
            var postData = {
                PermissionName: PermissionName,
                ModuleID: ModuleID,
                url: url,
                physicalLocation: physicalLocation,
                isActive: isActive,
                ordering: ordering,
            };

            ApiCallPost(PostPermissionUrl, token, postData)
                .then(function (response) {
                    console.log('Data saved successfully:', response);
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Data saved successfully!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            GetPermission();
                            GetPackages();
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

        function FetchDataForEdit(moduleID) {
            ApiCallById(GetByIdPermissionUrl, token, moduleID)
                .then(function (responseData) {
                    console.log('Data:', responseData);
                    var data = responseData.data;
                    $('#lblHidenPermissionId').val(data.userPermId);
                    $('#txtPermissionName').val(data.permissionName);
                    $('select[name="ddlModule"]').val(data.moduleID).change();     
                    $('#txtPermissionsUrl').val(data.url);
                    $('#txtPerPhysicalLocation').val(data.physicalLocation);
                    $('#txtPerOrdaring').val(data.ordering);
                    $('#chkIsActive').prop('checked', data.isActive);
                    $('#btnSave').html('Update');
                    BoxExpland()
                })
                .catch(function (error) {
                    console.error('Error:', error);
                });

        }

        function UpdateModule() {
            // Capture form data
            var permissionId= $('#lblHidenPermissionId').val();
            var PermissionName = $('#txtPermissionName').val();
            var ModuleID = parseInt($('select[name="ddlModule"]').val());
            var url = $('#txtPermissionsUrl').val();
            var physicalLocation = $('#txtPerPhysicalLocation').val();
            var ordering = parseInt($('#txtPerOrdaring').val());
            var isActive = $('#chkIsActive').is(':checked');

            // Create updateData object
            var updateData = {
                PermissionName: PermissionName,
                ModuleID: ModuleID,
                url: url,
                physicalLocation: physicalLocation,
                isActive: isActive,
                ordering: ordering,
            };
            //var updateUrl = `${GetByIdModuleUrl}/${moduleId}`;
            ApiCallUpdate(updatePermissioneUrl, token, updateData, permissionId)
                .then(function (response) {
                    console.log('Data updated successfully:', response);
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Data updated successfully!'
                    }).then(() => {
                        GetPermission();
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
        function Delete(ID) {
            Swal.fire({
                title: 'Are you sure?',
                text: "Do you really want to delete this module?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    ApiDeleteById(DeleteUrl, token, ID)
                        .then(function (response) {
                            Swal.fire({
                                title: 'Success!',
                                text: 'Module deleted successfully.',
                                icon: 'success',
                                confirmButtonText: 'OK'
                            }).then(() => {
                                GetPermission();
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
            $(window).scrollTop(scrollTop);
        }

        $(document).ready(function () {

            GetPermission();
            GetModule()
        });

    </script>
    <script src="assets/theme_assets/js/RootUrl.js"></script>
    <script src="assets/theme_assets/js/apiHelper.js"></script>
    <style>
        .jstree .jstree-loading {
    background: none !important;
}

    </style>
</asp:Content>
