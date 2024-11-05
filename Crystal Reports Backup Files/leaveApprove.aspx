<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="leaveApprove.aspx.cs" Inherits="SigmaERP.hrms.Leave.leaveApprove" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .exchange-icon {
            font-size: 1.5rem;
            color: rgba(0, 0, 0, 0.5); /* Transparent icon */
            display: inline-block;
            vertical-align: middle;
        }

        .line-height {
                line-height: 96px;
        }
        .action-btn{
            margin-top: -36px;
        }
        .total-day{
            text-align:center;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
    <div class="row mt-25 application">
        <!-- Leave Cards will be appended here -->
    </div>
    </div>

    <script>
        var token = '<%= Session["__UserToken__"] %>';
        //var rootUrl = 'https://localhost:7220';
        var rootUrl = '<%= Session["__RootUrl__"]%>';
        var CompanyID = '<%= Session["__GetCompanyId__"]%>';
        var userId = '<%= Session["__GetUserId__"]%>';
        var loginempId = '<%= Session["__GetEmpId__"]%>';
        var dptId = '<%=  Session["__DptId__"]%>';
        var dsgId = '<%=  Session["__DsgId__"]%>';
        var gId = '<%=  Session["__Gid__"]%>';
        var sftId = '<%=  Session["__SftId__"]%>';
        var DataAccessLevel = '<%=Session["__UserDataAccessLevel__"]%>';

        var getLeavesApplicationUrl = rootUrl + '/api/Leave/lvApplications';

        $(document).ready(function () {
            GetLeaves();  // Fetch leave data on document ready
        });

        function GetLeaves() {
            ApiCallwithCompanyId(getLeavesApplicationUrl, token,CompanyID)
                .then(function (response) {
                    if (response.statusCode === 200) {
                        var responseData = response.data;
                        console.log(responseData);
                        $('.footable-loader').show();
                        BindAppicationData(responseData);
                    } else {
                        console.error('Error occurred while fetching data:', response.message);
                    }
                })
                .catch(function (error) {
                    $('.loaderCosting').hide();
                    console.error('Error occurred while fetching data:', error);
                });
        } 
        function BindAppicationData(responseData) {
            var leaveContainer = $('.application');
                leaveContainer.empty();
            responseData.forEach(function (leave) {
                var leaveCard = `
                <div class="col-lg-6 col-xl-6 col-md-12 mb-25">
                    <div class="card shadow-sm ">
                        <div class="card-body">
                            <div class="row g-3 mb-3 justify-content-between">
                                <div class="col-md-4">
                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <div class="d-flex align-items-center">
                                    <img src="${leave.empPicture ? '../images/' + leave.empPicture : '../user_img_default.jpg'}" 
                                         style="height: 50px; width: 50px;" 
                                         class="rounded-circle me-3" 
                                         alt="Profile Picture">
                                    <div>
                                        <span class="fw-bold">${leave.empName}</span>
                                        <span class="fst-italic d-block">${leave.dsgName}</span>
                                    </div>
                                </div>

                            </div>
                                </div>
                                <div class="col-md-2">
                                    <label for="from" class="form-label">From</label>
                                    <input type="text" class="form-control" value="${formatDate(leave.leaveStartDate)}" readonly>
                                </div>
                                <div class="col-md-1 text-center line-height">
                                   <i class="fas fa-arrow-right"></i>
                                </div>
                                <div class="col-md-2">
                                    <label for="to" class="form-label">To</label>
                                    <input type="text" class="form-control" value="${formatDate(leave.leaveEndDate)}" readonly>
                                </div>
                                <div class="col-md-1">
                                    <label for="totalDays" class="form-label">Total</label>
                                    <input type="text" class="form-control total-day" value="${leave.totalLeaveDays}" readonly>
                                </div>
                            </div>
                            <div class="d-flex action-btn justify-content-end">
                                <button class="btn btn-primary m-2">Approve</button>
                                <button class="btn btn-primary m-2">Forward</button>
                                <button class="btn btn-outline-secondary m-2">Reject</button>
                            </div>
                        </div>
                    </div>
                </div>`;

                // Append the generated card to the container
                leaveContainer.append(leaveCard);
            });
        }

        function formatDate(dateString) {
            var date = new Date(dateString);
            return date.toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'short',
                day: 'numeric'
            });
        }



    </script>

     <script src="../assets/theme_assets/js/apiHelper.js"></script>
</asp:Content>
