<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="userPackagesSetup.aspx.cs" Inherits="SigmaERP.hrms.userPackagesSetup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <main class="main-content">
        <div class="Dashbord">
            <div class="crm mb-25">
                <div class="container-fulid">
                    <div class="card card-Vertical card-default card-md mt-4 mb-4">

                        <div id="Cardbox" class="card-body pb-md-30">
                            <div class="Vertical-form">
                                <div class="row">

                                    <div class="col-lg-3">
                                      <div class="form-group">
                                        <label id="lblHidenPksId" style="display:none"></label>
                                       <label for="formGroupExampleInput" class="color-dark fs-14 fw-500 align-center mb-10">Package <span
                                          class="text-danger">*</span></label>
                                       <div class="support-form__input-id">
                                           <div class="dm-select ">
                                               <select name="ddlPackages" id="ddlPackages" class="select-search form-control">
                                                   <option value="0">---Select---</option>
                                               </select>
                                           </div>
                                       </div>
                                   </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label for="txtOrdaring" class="color-dark fs-14 fw-500 align-center mb-10">
                                                Ordering <span class="text-danger">*</span>
                                            </label>
                                            <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtOrdaring" placeholder="Type Ordering">
                                            <span class="text-danger" id="orderingError"></span>
                                        </div>
                                    </div>
                              <%--      <div class="col-lg-3">
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

                                        <div class="col-lg-1">
                                        <label style="opacity: 0;" for="formGroupExampleInput"
                                            class="color-dark fs-14 fw-500 align-center mb-10">
                                            Name <span
                                                class="text-danger"></span>
                                        </label>
                                        <button type="button" id="btnSave" onclick="ValidateAndPostModule()"
                                            class="btn btn-primary btn-default btn-squared px-30">Save</button>
                                    </div>
                                    </div>--%>

                                      <div class="col-lg-3 " style="display:flex; justify-content:space-between">
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
                                                   class="btn btn-primary btn-default btn-squared px-30">Save</button>
                                           </div>

                                       </div>
                                    <div class="col-lg-4" id="treeSection">
                                        <p>Select features and permission</p>
                                        <div class="loader-size loaderPackages " style="display:none">
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
                                  <h4 style="margin-top: 13px;">Setuped Package</h4>
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
        var GetByIdPackagesUrl = rootUrl + '/api/UserPackages/packages';
        var GetFeturesUrl = rootUrl + '/api/UserModules/Packages';
        var GetPackagesUrl = rootUrl + '/api/UserPackages/packages';
        var GetPackageSetupsUrl = rootUrl + '/api/UserPackagesSetup/SetupPackage';
        var PostPackagesSetUrl = rootUrl + '/api/UserPackagesSetup/packagesSetup/create';//working

        var updatePackagesUrl = rootUrl + '/api/UserPackages/Packages/update';
        var DeleteUrl = rootUrl + '/api/UserPackagesSetup/packagesSetup/delete';
        var token = '<%= Session["__UserToken__"] %>';
        console.log('this is token you can use it :', token);
        //var token = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJpYXQiOjE3MTQ2MjQ5MjYsImV4cCI6MTc0NjE2MDkyNiwiYXVkIjoiIiwic3ViIjoiSldUU2VydmljZUFjY2Vzc1Rva2VuIn0.tVlIuOLas2VxEnBohuaIXXQR2Lju_2h8yVjCDizQh9o';

        $(document).ready(function () {
            $('#treeContainer').hide();
            $('#ddlPackages').change(function () {
                var selectedValue = $(this).val();
              
                FetchDataForEdit(selectedValue);
                $('#treeContainer').show();
            });
            GetPackages();
            GetPackagesSetupList();
            GetModule();
        });

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
         //function transformToJSTreeFormat(data) {
         //    return data.map(function (item) {
         //        let hasSelectedChild = item.children && item.children.some(child => child.state && child.state.selected);

         //        return {
         //            "id": item.isPermission ? item.permissionId : item.moduleID,
         //            "text": item.name,
         //            "state": {
         //                "opened": true,
         //                "selected": hasSelectedChild
         //            },
         //            "children": item.children && item.children.length > 0 ? transformToJSTreeFormat(item.children) : [],
         //            "li_attr": {
         //                "id": item.isPermission ? item.permissionId : item.moduleID
         //            },
         //            "original": {
         //                "isPermission": item.isPermission
         //            },
         //            "icon": item.isPermission ? "fa fa-key custom-permission-icon" : "fa fa-lock custom-module-icon"
         //        };
         //    });
         //}
        
        //function transformToJSTreeFormat(data) {
        //    return data.map(function (item) {
        //        let hasSelectedChild = item.children && item.children.some(child => child.state && child.state.selected);

        //        return {
        //            "id": item.isPermission ? item.permissionId : item.moduleID,
        //            "text": item.name,
        //            "state": {
        //                "opened": true,
        //                "selected": hasSelectedChild
        //            },
        //            "children": item.children && item.children.length > 0 ? transformToJSTreeFormat(item.children) : [],
        //            "li_attr": {
        //                "id": item.isPermission ? item.permissionId : item.moduleID
        //            },
        //            "original": {
        //                "isPermission": item.isPermission
        //            }
        //        };
        //    });
        //}
       

        function GetPackages() {
            ApiCall(GetPackagesUrl, token)
                .then(function (response) {
                    if (response.statusCode === 200) {
                        var responseData = response.data;
                        console.log(responseData);
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
            const dropdown = document.getElementById('ddlPackages');
            dropdown.innerHTML = '<option value="0">---Select---</option>'; // Clear existing options

            data.forEach(item => {
                const option = document.createElement('option');
                option.value = item.id;
                option.textContent = item.packageName;
                dropdown.appendChild(option);
            });
        }

        var selectedPermissionIDs = [];
         function GetModule() {
            console.log('Calling GetModule');
            ApiCall(GetFeturesUrl, token)
                .then(function (response) {
                    if (response.statusCode === 200) {
                        $('.loaderPackages').show();
                        responseData = response.data;
                        var treeData = transformToJSTreeFormat(responseData);
                        console.log("TreeData :",treeData)
                           $('.loaderPackages').hide();
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

                            for(i = 0, j = data.selected.length; i < j; i++) {

                                var node = data.instance.get_node(data.selected[i]);
                                if (node && node.children.length === 0) {
                                    selectedPermissionIDs.push(parseInt(node.id, 10));
                                }
                                 console.log('node:', node);
                            }
                            console.log('Child Node IDs:', selectedPermissionIDs);
                            
                        });
                    } else {
                        console.error('Error occurred while fetching data:', response.message);
                    }
                })
                .catch(function (error) {
                    console.error('Error occurred while fetching data:', error.message || error);
                });
        }

          var responseData = null;
        var selectedPermissionIDsUpdate = [];
        function FetchDataForEdit(moduleID) {
            ApiCallById(GetByIdPackagesUrl, token, moduleID)
                .then(function (response) {
                    console.log('Data:', response);
                    $('#loaderPackages').show();
                    var data = response.data;
                    $('#lblHidenPksId').val(data.id);
                    //$('#txtPackagesName').val(data.packageName);
                    //$('#txtOrdaring').val(data.ordering);
                    //$('#chkIsActive').prop('checked', data.isActive);
                    //$('#btnSave').html('Update');
                    //BoxExpland();
                    var selectedPermissionIDs = JSON.parse(data.features);
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
                        $('#loaderPackages').hide();
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
        function ValidateAndPostModule() {
            var isValid = true;
            if ($('#ddlPackages').val() <= 0) {
                $('#PackagesNameError').html("Please select a package name.");
                $("#txtPackagesName").focus();
                isValid = false;
            } else {
                $('#PackagesNameError').html("");
            }
            if (isValid) {
                var addnewElement = $("#btnSave");
                if (addnewElement.html() === "Save") {
                    PostModule();
                    //ClearTextBox();
                }
                else {
                    //updatePackages();
                    //ClearTextBox();
                }
            }
        }
        function GetPackagesSetupList() {
            ApiCall(GetPackageSetupsUrl, token)
                .then(function (response) {
                    if (response.statusCode === 200) {
                        var responseData = response.data;
                        console.log('responseData',responseData);
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
            // Destroy existing Footable instance if it exists
            if ($('.adv-table').data('footable')) {
                $('.adv-table').data('footable').destroy();
            }

            // Clear the table and filter form container
            $('.adv-table').html('');
            $('#filter-form-container').empty();

            // Iterate through data and format rows
            data.forEach((row, index) => {
                // Add serial number (SL)
                row.serialNo = index + 1;  // Starts SL from 1

                row.actions = `
        <ul class="orderDatatable_actions mb-0 d-flex flex-wrap">
            <li><a href="javascript:void(0)" class="view-btn view" data-id="${row.psid}"><i class="uil uil-eye"></i></a></li>
            <li><a href="javascript:void(0)" data-id="${row.psid}" class="edit-btn edit"><i class="uil uil-edit"></i></a></li>  
            <li><a href="javascript:void(0)" data-id="${row.psid}" class="delete-btn remove"><i class="uil uil-trash-alt"></i></a></li>
        </ul>
        `;

                row.isActive = `
        <div class="form-check form-switch form-switch-primary form-switch-sm">
            <input type="checkbox" class="form-check-input" id="switch-${row.id}" ${row.isActive ? 'checked' : ''}>
            <label class="form-check-label" for="switch-${row.id}"></label>
        </div>
        `;

                row.features = `
        <div class="features-icon-container">
            <a href="javascript:void(0)" class="feature-btn" data-id="${row.id}"><i class="uil uil-star"></i></a>
        </div>
        `;
            });

            // Define the table columns
            const columns = [
                { "name": "serialNo", "title": "SL", "breakpoints": "xs sm", "type": "number", "className": "userDatatable-content" }, // Serial number column
                { "name": "packageName", "title": "Package Name", "className": "userDatatable-content" },
                { "name": "features", "title": "Features", "className": "userDatatable-content" },
                { "name": "isActive", "title": "Is Active", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                { "name": "activatedAt", "title": "Activated At", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                { "name": "deActivatedAt", "title": "DeActivated At", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                { "name": "actions", "title": "Action", "sortable": false, "filterable": false, "className": "userDatatable-content" }
            ];

            try {
                // Initialize Footable
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
                }).on('postinit.ft.table', function () {
                    $('.footable-loader').hide();
                });
            } catch (error) {
                console.error("Error initializing Footable:", error);
            }

            // Clear and reattach event listeners
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
                console.log('View button clicked for ID:', id);
            });
        }


        function PostModule() {
            var packageId = parseInt($('#ddlPackages').val(), 10);
            var isActive = $('#chkIsActive').is(':checked');
            var treeInstance = $('#treeContainer').jstree(true);

            var jsonString = JSON.stringify(selectedPermissionIDsUpdate);
            console.log(jsonString);

            var postData = {
                packageId: packageId,
                features: jsonString,
                isActive: isActive,
            };
            ApiCallPost(PostPackagesSetUrl, token, postData)
                .then(function (response) {
                    console.log('Data saved successfully:', response);
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Data saved successfully!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            GetPackagesSetupList();
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

        function Delete(ID) {
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
                    ApiDeleteById(DeleteUrl, token, ID)
                        .then(function (response) {
                            Swal.fire({
                                title: 'Success!',
                                text: 'Packages deleted successfully.',
                                icon: 'success',
                                confirmButtonText: 'OK'
                            }).then(() => {
                                GetPackagesSetupList();
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

    </script>
     <script src="assets/theme_assets/js/apiHelper.js"></script>

</asp:Content>
