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
            "id": item.isPermission,
            "text": item.name,
            "state": {
                "opened": true,
                "selected": hasSelectedChild
            },
            "children": item.children && item.children.length > 0 ? transformToJSTreeFormat(item.children) : [],
            "li_attr": {
                "id": item.isPermission 
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
//            }
//        };
//    });
//}

var selectedPermissionIDsUpdate = []

function FetchDataForEdit(moduleID) {
    ApiCallById(getRolesByIdUrl, token, moduleID)
        .then(function (response) {
            console.log('Data:', response);
            var data = response.data;
            $('#lblHidenRolesId').val(data.userRoleId);
            $('#txtRole').val(data.userRoleName);
            $('#txtOrdaring').val(data.ordering);
            $('#chkIsActive').prop('checked', data.isActive);
            $('#btnSave').html('Update');
            //BoxExpland();
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
