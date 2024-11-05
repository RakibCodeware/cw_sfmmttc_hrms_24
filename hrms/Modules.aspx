<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="modules.aspx.cs" Inherits="SigmaERP.hrms.Modules" %>
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
                           <h4>Add Module</h4>
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
                                     <label id="lblHidenModuleId" style="display:none"></label>
                                     <label for="formGroupExampleInput" class="color-dark fs-14 fw-500 align-center mb-10">Parent</label>
                                     <div class="support-form__input-id">
                                         <div class="dm-select ">
                                             <select name="ddlParent" id="ddlParent" class="select-search form-control">
                                                 <option value="0">---Select---</option>
                                             </select>
                                         </div>
                                     </div>
                                 </div>
                              </div>
                               <div class="col-lg-3">
                                   <div class="form-group">
                                       <label for="txtModuleName" class="color-dark fs-14 fw-500 align-center mb-10">
                                           Module Name <span class="text-danger">*</span>
                                       </label>
                                       <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtModuleName" placeholder="Type Module Name">
                                       <span class="text-danger" id="moduleNameError"></span>
                                   </div>
                               </div>
                               <div class="col-lg-3">
                                   <div class="form-group">
                                       <label for="txtModuleUrl" class="color-dark fs-14 fw-500 align-center mb-10">
                                           Module Url <span class="text-danger">*</span>
                                       </label>
                                       <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtModuleUrl" placeholder="Type Module Url">
                                       <span class="text-danger" id="moduleUrlError"></span>
                                   </div>
                               </div>
                              <div class="col-lg-3">
                                  <div class="form-group">
                                      <label for="txtPhysicalLocation" class="color-dark fs-14 fw-500 align-center mb-10">
                                          Physical Location <span class="text-danger">*</span>
                                      </label>
                                      <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtPhysicalLocation" placeholder="Type Physical Location">
                                      <span class="text-danger" id="physicalLocationError"></span>
                                  </div>
                              </div>
                               <div class="col-lg-3">
                                   <div class="form-group">
                                       <label for="txtIconClass" class="color-dark fs-14 fw-500 align-center mb-10">
                                           Icon Class <span class="text-danger">*</span>
                                       </label>
                                       <input type="text" class="form-control ih-medium ip-gray radius-xs b-light px-15" id="txtIconClass" placeholder="Type Icon Class">
                                       <span class="text-danger" id="iconClassError"></span>
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
                                 <button type="button" id="btnSave" onclick="validateAndPostModule()"
                                    class="btn btn-primary btn-default btn-squared px-30">Save</button>
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
                                  <h4 style="margin-top: 13px;">Modules</h4>
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
        var rootUrl = '<%= Session["__RootUrl__"]%>';
        function BoxExpland() {
            var scrollTop = $(window).scrollTop();

            $("#Cardbox").show();
            $("#addnew").text("Close");
            $(window).scrollTop(scrollTop);
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


    </script>


    <script src="assets/theme_assets/js/module.js"></script>
    <script src="assets/theme_assets/js/apiHelper.js"></script>
</asp:Content>
