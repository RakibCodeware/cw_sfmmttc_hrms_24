<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="userPackages.aspx.cs" Inherits="SigmaERP.hrms.userPackages" %>

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
                                    <h4>Add Package</h4>
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
                                            <label id="lblHidenPackagesId" style="display:none"></label>

                                            <label for="txtPackagesName" class="color-dark fs-14 fw-500 align-center mb-10">
                                                Package Name <span class="text-danger">*</span>
                                            </label>
                                            <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtPackagesName" placeholder="Type Package Name">
                                            <span class="text-danger" id="PackagesNameError"></span>
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
                                        <div class="loader-size loaderPackages">
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
               <!-- Department List  -->
                        <div class="row">
               <div class="col-lg-12">
                  <div class="card ">
                     <div class="card-body">

                        <div class="userDatatable adv-table-table global-shadow border-light-0 w-100 ">
                           <div class="table-responsive">
                              <div class="ad-table-table__header d-flex justify-content-between">
                                  <h4 style="margin-top: 13px;">Packages</h4>
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
        var GetFeturesUrl = rootUrl + '/api/UserModules/Packages';//working
        var GetPackagesUrl = rootUrl + '/api/UserPackages/packages';
     
        var PostPackagesUrl = rootUrl + '/api/UserPackages/Packages/create';//working

        var updatePackagesUrl = rootUrl + '/api/UserPackages/Packages/update';
        var DeleteUrl = rootUrl + '/api/UserPackages/packages/delete';
        var token = '<%= Session["__UserToken__"] %>';
       
        //var token = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJpYXQiOjE3MTQ2MjQ5MjYsImV4cCI6MTc0NjE2MDkyNiwiYXVkIjoiIiwic3ViIjoiSldUU2VydmljZUFjY2Vzc1Rva2VuIn0.tVlIuOLas2VxEnBohuaIXXQR2Lju_2h8yVjCDizQh9o';

        $(document).ready(function () {
            GetModule();
            GetPackages();
        });
        function Cardbox() {
            $("#Cardbox").toggle();
            var currentText = $("#addnew").text();
            var newText = currentText === "Close" ? "Add New" : "Close";
            $("#addnew").text(newText);
        }

        function ValidateAndPostModule() {
            var isValid = true;
            if ($('#txtPackagesName').val().trim() === "") {
                $('#PackagesNameError').html("Packages Name is required.");
                $("#txtPackagesName").focus();
                isValid = false;
            } else {
                $('#PackagesNameError').html("");
            }
            if ($('#txtOrdaring').val().trim() === "" || isNaN($('#orderingError').val())) {
                $('#orderingError').html("Ordering is required and must be a number.");
                $("#txtOrdaring").focus();
                isValid = false;
            } else {
                $('#orderingError').html("");
            }
            if (isValid) {
                var addnewElement = $("#btnSave");
                if (addnewElement.html() === "Save") {
                    PostModule();
                    ClearTextBox();
                }
                else {
                    updatePackages();
                    ClearTextBox();
                }
            }
        }
        function ClearTextBox() {
            $('#txtPackagesName').val("");
            $('#txtOrdaring').val("");
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
        var selectedPermissionIDs = [];

        var responseData = null;

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
        //            "id": item.isPermission ? item.permissionId.toString() : item.moduleID.toString(),
        //            "text": item.name,
        //            "state": {
        //                "opened": true,
        //                "selected": hasSelectedChild
        //            },
        //            "children": item.children && item.children.length > 0 ? transformToJSTreeFormat(item.children) : [],
        //            "li_attr": {
        //                "id": item.isPermission ? item.permissionId.toString() : item.moduleID.toString()
        //            },
        //            "original": {
        //                "isPermission": item.isPermission
        //            }
        //        };
        //    });
        //}



        function PostModule() {
            var PackageName = $('#txtPackagesName').val();
            var ordering = parseInt($('#txtOrdaring').val());
            var isActive = $('#chkIsActive').is(':checked');
            var treeInstance = $('#treeContainer').jstree(true);

            var jsonString = JSON.stringify(selectedPermissionIDs);
            console.log(jsonString);

            var postData = {
                PackageName: PackageName,
                features: jsonString,
                isActive: isActive,
                ordering: ordering,
   
            };

          

            ApiCallPost(PostPackagesUrl, token, postData)
                .then(function (response) {
                    console.log('Data saved successfully:', response);
                    Swal.fire({
                        icon: 'success',
                        title: 'Success',
                        text: 'Data saved successfully!'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            GetModule();
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

        function GetPackages() {
            ApiCall(GetPackagesUrl, token)
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


        function BoxExpland() {
            var scrollTop = $(window).scrollTop();

            $("#Cardbox").show();
            $("#addnew").text("Close");
            $(window).scrollTop(scrollTop);
        }


        function bindTableData(data) {
            if ($('.adv-table').data('footable')) {
                $('.adv-table').data('footable').destroy();
            }
            $('.adv-table').html('');
            $('#filter-form-container').empty();

            // Loop through the data and add serial numbers
            data.forEach((row, index) => {
                // Assign serial number based on the index (starting from 1)
                row.serialNo = index + 1;

                row.packageName = `
        <div class="permission-name-container">
            ${row.packageName}
            <div class="actions-container">
                <ul class="orderDatatable_actions mb-0 d-flex flex-wrap">
                    <li><a href="javascript:void(0)" class="view-btn view" data-id="${row.id}"><i class="uil uil-eye"></i></a></li>
                    <li><a href="javascript:void(0)" data-id="${row.id}" class="edit-btn edit"><i class="uil uil-edit"></i></a></li>
                    <li><a href="javascript:void(0)" data-id="${row.id}" class="delete-btn remove"><i class="uil uil-trash-alt"></i></a></li>
                </ul>
            </div>
        </div>
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

            // Define the table columns, including the serial number (SL)
            const columns = [
                { "name": "serialNo", "title": "SL", "breakpoints": "xs sm", "type": "number", "className": "userDatatable-content" }, // Serial number column
                { "name": "packageName", "title": "Package Name", "className": "userDatatable-content" },
                { "name": "features", "title": "Features", "className": "userDatatable-content" },
                { "name": "isActive", "title": "Is Active", "sortable": false, "filterable": false, "className": "userDatatable-content" },
                { "name": "ordering", "title": "Ordering", "type": "number", "className": "userDatatable-content" },
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
                console.log('View button clicked for ID:', id);
            });

            $('.adv-table').off('click', '.feature-btn').on('click', '.feature-btn', function () {
                const id = $(this).data('id');
                console.log('Feature button clicked for ID:', id);
                FetchDataForEdit(id);
            });
        }



        var selectedPermissionIDsUpdate = []
        function FetchDataForEdit(moduleID) {
            ApiCallById(GetByIdPackagesUrl, token, moduleID)
                .then(function (response) {
                    console.log('Data:', response);
                    var data = response.data;
                    $('#lblHidenPackagesId').val(data.id);
                    $('#txtPackagesName').val(data.packageName);
                    $('#txtOrdaring').val(data.ordering);
                    $('#chkIsActive').prop('checked', data.isActive);
                    $('#btnSave').html('Update');
                    BoxExpland();
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
                    "id": item.isPermission ,
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

        
        function updatePackages() {
                var PackageId = $('#lblHidenPackagesId').val();
                var PackageName = $('#txtPackagesName').val();
                var ordering = parseInt($('#txtOrdaring').val());
                var isActive = $('#chkIsActive').is(':checked');
                var treeInstance = $('#treeContainer').jstree(true);

                var jsonString = JSON.stringify(selectedPermissionIDsUpdate);
                console.log(jsonString);

                var updateData = {
                    PackageName: PackageName,
                    features: jsonString,
                    isActive: isActive,
                    ordering: ordering,
                };

                ApiCallUpdate(updatePackagesUrl, token, updateData, PackageId)
                    .then(function (response) {
                        console.log('Data updated successfully:', response);
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: 'Data updated successfully!'
                        }).then((result) => {
                            if (result.isConfirmed) {
                                GetModule();
                                GetPackages();
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
                                GetPackages();
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

        console.log("Git Test From Home ");


    </script>


    <script src="assets/theme_assets/js/apiHelper.js"></script>
</asp:Content>
