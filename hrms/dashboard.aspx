<%@ Page Title="" Language="C#" MasterPageFile="~/hrms/HRMS.Master" AutoEventWireup="true" CodeBehind="dashboard.aspx.cs" Inherits="SigmaERP.hrms.dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .ui-datepicker {
            position: fixed !important;
            top: 108px !important;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <%--<div class="Dashbord">--%>
    <div class="crm mb-25">
        <div class="container-fluid">

      
                <div class="date-parent">
                            <div class="row position-relative">
            
                    <div class="dm-date-picker d-flex justify-content-end gap-1 w-100">
                        <div class="form-group mb-0 form-group-calender position-relative">
                            <div class="positon-fixed top-0">
                                <input type="text" onload="getDate()" class="form-control top-0 date-wrapper form-control-default " id="datepicker4" placeholder="select date">
                         
                             
                            </div>

                            <%--<span><i class="fab fa-facebook"></i></span>--%>
                                  <img class="svg cicon" src="img/svg/calendar.svg" alt="calendar">
                        </div>
                        <button style="line-height: 32px; display: block;" type="button" onclick="SearchData()" class="btn btn-info btn-default btn-squared ">
                            <i class="fas fa-search"></i>
                        </button>
                    </div>
            </div>
            </div>
                            
            



            <div class="row mt-2">
                <div class="col-lg-8  d-block d-lg-flex">
                    <div class="row">
                        <div class="col-lg-4 col-md-4 col-sm-12 mb-25 d-block">
                            <div class="ap-po-details ap-po-details--luodcy  overview-card-shape radius-xl d-flex justify-content-between">
                                <div class=" ap-po-details-content d-flex flex-wrap justify-content-between w-100">
                                    <div class="ap-po-details__titlebar">
                                        <p class="fs-6 fw-bold">Total Student</p>
                                           <div class="loader-size loaderDaily">
                                        <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                        </div>
                                            </div>
                                        <div class="h1" id="totalEmp">
                                        </div>
                                        <%--Loading--%>

                                        <div class="ap-po-details-time">
                                            <span class="color-success"><i class="las la-arrow-up"></i>
                                                <strong id="totalEmpRatio"></strong></span>
                                            <small></small>
                                        </div>
                                    </div>
                                    <div class="ap-po-details__icon-area color-info">
                                        <i class="uil uil-user"></i>
                                    </div>
                                </div>

                            </div>
                            <!-- Card 1 End  -->
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12 mb-25">
                            <!-- Card 2 -->
                            <div class="ap-po-details ap-po-details--luodcy  overview-card-shape radius-xl d-flex justify-content-between">
                                <div class=" ap-po-details-content d-flex flex-wrap justify-content-between w-100">
                                    <div class="ap-po-details__titlebar">
                                        <p class="fs-6 fw-bold">Today's Present</p>
                                        <div class="loader-size loaderDaily">
                                            <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                        </div>
                                        <div class="h1" id="todayPresent">

                                        </div>
                                        <div class="ap-po-details-time">
                                            <span class="color-success"><i class="las la-arrow-up"></i>
                                                <strong id="todayPresentRatio"></strong></span>
                                        </div>
                                    </div>
                                    <div class="ap-po-details__icon-area color-success">
                                        <i class="uil uil-user-check"></i>
                                    </div>
                                </div>
                            </div>
                            <!-- Card 2 End  -->
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12 mb-25">
                            <!-- Card 2 -->
                            <div class="ap-po-details ap-po-details--luodcy  overview-card-shape radius-xl d-flex justify-content-between">
                                <div class=" ap-po-details-content d-flex flex-wrap justify-content-between w-100">
                                    <div class="ap-po-details__titlebar">
                                        <p class="fs-6 fw-bold">Today's Absent</p>
                                            <div class="loader-size loaderDaily">
                                        <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                        </div>
                                            </div>
                                        <div class="h1" id="todayAbsent">


                                        </div>
                                        <div class="ap-po-details-time">
                                            <span class="color-danger"><i class="las la-arrow-down"></i>
                                                <strong id="todayAbsentRatio"></strong></span>
                                        </div>
                                    </div>
                                    <div class="ap-po-details__icon-area color-danger">
                                        <i class="uil uil-user-minus"></i>
                                    </div>
                                </div>
                            </div>
                            <!-- Card 2 End  -->
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12 mb-25">
                            <!-- Card 3 -->
                            <div class="ap-po-details ap-po-details--luodcy  overview-card-shape radius-xl d-flex justify-content-between">
                                <div class=" ap-po-details-content d-flex flex-wrap justify-content-between w-100">
                                    <div class="ap-po-details__titlebar">
                                        <p class="fs-6 fw-bold">
                                            Today's Late
                                        </p>
                                         <div class="loader-size loaderDaily">
                                        <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                        </div>
                                            </div>
                                        <div class="h1" id="todayLate">

                                        </div>
                                        <div class="ap-po-details-time">
                                            <span class="color-danger"><i class="las la-arrow-down"></i>
                                                <strong id="todayLateRatio"></strong></span>
                                        </div>
                                    </div>
                                    <div class="ap-po-details__icon-area color-warning">
                                        <i class="uil uil-stopwatch-slash"></i>
                                    </div>
                                </div>

                            </div>
                            <!-- Card 3 End  -->
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12 mb-25">
                            <!-- Card 4  -->
                            <div class="ap-po-details ap-po-details--luodcy  overview-card-shape radius-xl d-flex justify-content-between">
                                <div class=" ap-po-details-content d-flex flex-wrap justify-content-between w-100">
                                    <div class="ap-po-details__titlebar">
                                        <p class="fs-6 fw-bold">Today's Leave</p>
                                            <div class="loader-size loaderDaily">
                                        <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                        </div>
                                            </div>
                                        <div class="h1" id="todayLeave">
                                        </div>
                                        <div class="ap-po-details-time">
                                            <span class="color-success"><i class="las la-arrow-up"></i>
                                                <strong id="todayLeaveRatio"></strong></span>
                                        </div>
                                    </div>
                                    <div class="ap-po-details__icon-area color-primary">
                                        <i class="uil uil-calendar-slash"></i>

                                    </div>
                                </div>

                            </div>
                            <!-- Card 4 End  -->
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12 mb-25">
                            <!-- Card 4  -->
                            <div class="ap-po-details ap-po-details--luodcy  overview-card-shape radius-xl d-flex justify-content-between">
                                <div class=" ap-po-details-content d-flex flex-wrap justify-content-between w-100">
                                    <div class="ap-po-details__titlebar">
                                        <p class="fs-6 fw-bold">W/H Off Duty</p>
                                         <div class="loader-size loaderDaily">
                                        <div class="dm-spin-dots  dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                            <span class="spin-dot badge-dot dot-primary"></span>
                                        </div>
                                            </div>
                                        <div class="h1" id="WHOffDuty">

                                        </div>
                                        <div class="ap-po-details-time">
                                            <span class="color-success"><i class="las la-arrow-up"></i>
                                                <strong id="WHOffDeutyPers"></strong></span>
                                        </div>
                                    </div>
                                    <div class="ap-po-details__icon-area color-warning">
                                        <i class="uil uil-bag"></i>

                                    </div>
                                </div>

                            </div>
                            <!-- Card 4 End  -->
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 mb-25 d-block">
                    <div class="card border-0 px-25 paratioStyle">
                        <div class="card-header px-0 border-0">
                            <h6>Last 7 Days Attendance Ratio</h6>


                        </div>

                        <ul class="legend-static mb-30 mt-1 d-flex justify-content-center">
                            <li class="custom-label">
                                <span class="dotcolornew"></span>Present
                            </li>
                            <li class="custom-label">
                                <span class="dotcolorrel"></span>Absent
                            </li>
                        </ul>
                        <div class="card-body p-0 pb-sm-25 position-relative">
                            <div class="tab-content">
                                <div class="tab-pane active show" id="salesgrowth" role="tabpanel"
                                    aria-labelledby="salesgrowth-tab">
                                    <div class="parentContainer">
                                        <div>
                                            <div class="dm-spin-dots loaderSevenDays position-absolute load-center dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                            <div class="height-present-ratio ">
                                               <canvas id="salesGrowthToday"></canvas>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!---Today Costing Over time start---->
                <!---Attendance Summary start---->
                <div class="col-lg-12 col-md-12 col-sm-12 mb-25">

                    <div class="card border-0 px-25 position-relative">
                        <div class="card-header px-0 border-0">
                            <h2 class="mt-3 mb-4">Daily Attendance Summary</h2>
                        </div>
                        <div class=" p-0">
                            <div class="tab-content">
                                <div class="tab-pane fade active show" id="t_selling-today" role="tabpanel"
                                    aria-labelledby="t_selling-today-tab">
                                    <div class="selling-table-wrap">
                                        <div class="table-responsive table-height">
                                            <div class="dm-spin-dots dot-size table-loader loaderDailySum dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                            <table class="attendance-table table table-border " id="tblDailyAttSummary">
                                                <thead class="sticky-top text-center">
                                                    <tr>
                                                        <th>SL </th>
                                                        <th>Course Name </th>
                                                        <th>Total Student</th>
                                                        <th>Male</th>
                                                        <th>Female</th>
                                                        <th>Present</th>
                                                        <th>Present %</th>
                                                        <th>Absent</th>
                                                        <th>Absent %</th>
                                                        <th>Late</th>
                                                        <th>Late %</th>
                                                        <th>Leave</th>
                                                        <th>leave %</th>
                                                        <th>Off day</th>
                                                        <th>Off day %</th>
                                                    </tr>
                                                </thead>
                                                <tbody class="text-center">
                                                   <!-- Data Bind From Api ---->
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <!---Attendance Summary End---->





                <!---Today Costing Over time End ---->

          

            </div>
                  <div class="row">

                    <div class="col-lg-7 col-md-7 col-sm-12 mb-25">
                        <!-- Card 1  -->
                        <div class="card">
                            <div class="card-header">
                              <h6>Current Student Status</h6>  
                            </div>
                                  <div class="ap-po-details ap-po-details--3x radius-xl d-flex">
                            <div class="row">
                                <div class="col-lg-4 overview-content overview-content3 bg-none mb-3">
                                    <div class="d-flex">
                                        <div class="revenue-chart-box__Icon me-20 order-bg-opacity-secondary color-secondary">
                                            <i class="uil uil-user-minus"></i>
                                        </div>
                                        <div class="d-flex align-items-start flex-wrap">
                                            <div class="me-25">
                                                <div class="loader-size loaderCurrStatusRel">
                                                    <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                    </div>
                                                </div>
                                                <div class="h1" id="Released">
          
                                                </div>
                                                <p class="mt-1 mb-0">
                                                    Released
                                                </p>
                                            </div>

                                            <!-- <div class="ap-po-details bg-none">
                                       <span class="color-success"><i class="las la-arrow-up"></i>
                                          <strong>25.36%</strong></span>
                                    </div> -->
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 overview-content overview-content3 bg-none mb-3">
                                    <div class="d-flex">
                                        <div class="revenue-chart-box__Icon me-20 order-bg-opacity-secondary color-secondary">

                                            <i class="uil uil-user-plus"></i>
                                        </div>
                                        <div class="d-flex align-items-start flex-wrap">
                                            <div class="me-25">
                                                <div class="loader-size loaderCurrStatusRel">
                                                    <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                    </div>
                                                </div>
                                                <div class="h1" id="Recruitment">
                             
                                                </div>
                                                <p class="mt-1 mb-0">Recruitment</p>
                                            </div>

                                            <!-- <div class="ap-po-details bg-none">
                                       <span class="color-success"><i class="las la-arrow-up"></i>
                                          <strong>25.36%</strong></span>
                                    </div> -->
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 overview-content overview-content3 bg-none mb-3">
                                    <div class="d-flex">
                                        <div class="revenue-chart-box__Icon me-20 order-bg-opacity-secondary color-secondary">
                                            <i class="uil uil-users-alt"></i>
                                        </div>
                                        <div class="d-flex align-items-start flex-wrap">
                                            <div class="me-25">
                                                <div class="loader-size loaderCurrStatusRel">
                                                    <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                    </div>
                                                </div>
                                                <div class="h1" id="totalEmpReg">

                                                </div>
                                                <p class="mt-1 mb-0">Total</p>
                                            </div>
                                
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-4 overview-content overview-content3 bg-none mb-3">
                                    <div class="d-flex">
                                        <div class="revenue-chart-box__Icon me-20 order-bg-opacity-success color-success">

                                            <i class="uil uil-user-md"></i>
                                        </div> 
                                        <div class="d-flex align-items-start flex-wrap">
                                            <div class="me-25">
                                                <div class="loader-size loaderCurrStatusRel">
                                                    <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                    </div>
                                                </div>
                                                <div class="h1" id="male">
            
                                                </div>
                                                <p class="mt-1 mb-0">Male</p>
                                            </div>

                                      
                                    
                                        </div>
                                    </div>
                                </div>

                                <%--Male Female ratio--%>
                                <div class="col-lg-4 overview-content overview-content3 bg-none mb-3">
                                    <div class="d-flex">
                                        <div class="revenue-chart-box__Icon me-20 order-bg-opacity-success color-success">

                                            <i class="uil uil-user-nurse"></i>
                                        </div>
                                        <div class="d-flex align-items-start flex-wrap">
                                            <div class="me-25">
                                                <div class="loader-size loaderCurrStatusRel">
                                                    <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                    </div>
                                                </div>
                                                <div class="h1" id="female">
      
                                                </div>
                                                <p class="mt-1 mb-0">Female</p>
                                            </div>

                                            <!-- <div class="ap-po-details bg-none">
                                    <span class="color-danger"><i class="las la-arrow-down"></i>
                                       <strong>25.36%</strong></span>
                                 </div> -->
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-4 overview-content overview-content3 bg-none mb-3">
                                    <div class="d-flex">
                                        <div class="revenue-chart-box__Icon me-20 order-bg-opacity-success color-success">

                                            <i class="uil uil-clock-seven"></i>
                                        </div>
                                        <div class="d-flex align-items-start flex-wrap">
                                            <div class="me-25">
                                                <div class="loader-size loaderCurrStatusRel">
                                                    <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                        <span class="spin-dot badge-dot dot-primary"></span>
                                                    </div>
                                                </div>
                                                <div class="h1" id="overtime">

                                                </div>
                                                <p class="mt-1 mb-0">Assigned OT</p>
                                            </div>

                                            <!-- <div class="ap-po-details bg-none">
                                    <span class="color-danger"><i class="las la-arrow-down"></i>
                                       <strong>25.36%</strong></span>
                                 </div> -->
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        </div>
                  
                        <!-- Card 1 End  -->

                      
                     


                    </div>



                    <div class="col-lg-5 d-block ">
                        <div class="row">
                            <div class="col-lg-6 col-md-6 col-sm-12 mb-25 d-block ">
                                <div class="device-chart-box">

                                    <div class="card position-relative border-0 MaleFemale">
                                        <div class="card-header">
                                            <h6>Male Female Ratio</h6>
                                        </div>
                                        <!-- ends: .card-header -->
                                        <div class="card-body cardBodyCenter ">
                                            <div id="google-pieChartBasic"></div>
                                            <div class="tab-content">
                                                <div class="tab-pane fade active show" id="se_device-today" role="tabpanel"
                                                    aria-labelledby="se_device-today-tab">
                                                    <div class="device-pieChart-wrap position-relative">

                                                        <div class="worker-loader-x">
                                                            <div class="dm-spin-dots dot-size loaderCurrStatus dot-sizedot-sizedot-sizedot-size spin-sm">
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                            </div>
                                                        </div>
                                                        <div class="MaleFemaleRatio"></div>


                                                    </div>
                                                    <div class="session-wrap session-wrap--top--4">
                                                        <div class="session-single">
                                                            <div class="chart-label">
                                                                <span class="label-dot dot-info " ></span>
                                                                Male
                                                            </div>
                                                        </div>
                                                        <div class="session-single">
                                                            <div class="chart-label">
                                                                <span class="label-dot" style="background-color:#EC49A6 !important"></span>
                                                                Female
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>

                                            </div>
                                            <!-- ends: .session-wrap -->
                                        </div>
                                        <!-- ends: .card-body -->
                                    </div>

                                </div>
                            </div>
                            <div class="col-lg-6 col-md-6 col-sm-12 mb-25 d-block ">
                                <div class="device-chart-box">

                                    <div class="card position-relative border-0 MaleFemale">
                                        <div class="card-header">
                                            <h6>Student Teacher Ratio</h6>
                                        </div>
                                        <!-- ends: .card-header -->
                                        <div class="card-body cardBodyCenter">
                                            <div class="tab-content">
                                                <div class="tab-pane fade active show" id="se_device-today" role="tabpanel"
                                                    aria-labelledby="se_device-today-tab">
                                                    <div class="device-pieChart-wrap position-relative">

                                                        <div class="worker-loader-x">
                                                            <div class="dm-spin-dots loaderCurrStatus dot-size dot-sizedot-sizedot-sizedot-size spin-sm">
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                            </div>
                                                             </div>

                                                            <div class="StapWorkerRatio">
                                                                                                                        
                                                            </div>
                                                       

                                                    </div>
                                                    <div class="session-wrap session-wrap--top--4">
                                                        <div class="session-single">
                                                            <div class="chart-label">
                                                                <span class="label-dot staff"></span>
                                                                Teacher
                                                            </div>
                                                        </div>
                                                        <div class="session-single">
                                                            <div class="chart-label">
                                                                <span class="label-dot worker"></span>
                                                                Student
                                                            </div>
                                                        </div>

                                                    </div>
                                                </div>

                                            </div>
                                            <!-- ends: .session-wrap -->
                                        </div>
                                        <!-- ends: .card-body -->
                                    </div>

                                </div>
                            </div>
                        </div>
                        <!-------Stap Workar Ratio------>
                    </div>

                </div>

                <div class="row">
                    <div class="col-lg-6">
                        
                      <div class="card position-relative border-0 cashflowChart2">
                                     <div class="card-header">
                                         <h6>Monthly New Join & Release</h6>
                                     </div>
                                     <!-- ends: .card-header -->
                                     <div  class="card-body monthlyNewJoinRatio pt-0 ">
                                         <div class="cashflow-chart">
                                             <ul class="legend-static mt-1 d-flex justify-content-center">
                                                 <li class="custom-label">
                                                     <span class="dotcolornew"></span>New Join
                                                 </li>
                                                 <li class="custom-label">
                                                     <span class="dotcolorrel"></span>Release
                                                 </li>
                                             </ul>
                                            
                                                     <div class="worker-loader">
                                                <div class="dm-spin-dots dot-size loaderMonthlyNewRel dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                                         </div>
                                                     <canvas id="profitGrowthToday"></canvas>
                                                 </div>


                                             
                                         </div>
                                     </div>
                                     <!-- ends: .card-body -->
                                 </div>

                           <div class="col-lg-6">
                                  <div class="card position-relative">
                                <div class="card-header">
                                    <div class="chartLine-po-details__titlebar">

                                        <h1>Salary Comparison(Last 12th Month)</h1>
                                    </div>
                                </div>
                                <div class="card-body salary-height">

                                    <div class="worker-loader">
                                     <div class="dm-spin-dots dot-size loaderSelaryCom dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                            </div>
                                        <div class="SalaryComparisonChart"></div>

                                </div>
                            </div>
                        </div>

                    </div>
                     
                    <div class="row">
            <div class="col-12 mb-25  mt-25">
                    <!-- Card 1  -->
                    <div class="ap-po-details ap-po-details--3 radius-xl d-flex py-25">

                        <div class="col-lg-3 overview-content overview-content3 bg-none b-none mb-lg-0 mb-20">
                            <div class="d-flex">
                                <div class="revenue-chart-box__Icon me-20 bg-success color-white rounded-circle">
                                    <i class="uil uil-money-withdraw"></i>
                                </div>
                                <div class="d-flex align-items-start flex-wrap">
                                    <div  class="me-10">

                                        <div class="loader-size loaderLastMonthCosting">
                                            <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                        </div>
                                        <div class="h1" id="LastMonthCosting">

                                        </div>
                                        <p class="mt-1 mb-0">Last Month Salary</p>
                                    </div>
                                    <!-- <div class="ap-po-details bg-none">
                                    <span class="color-success fs-14 d-inline-block mt-2"><i
                                          class="las la-arrow-up"></i>
                                       <strong>25.36%</strong></span>
                                 </div> -->
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-3 overview-content overview-content3 bg-none b-none mb-lg-0 mb-20">
                            <div class="d-flex">
                                <div class="revenue-chart-box__Icon me-20 bg-warning color-white rounded-circle">
                                    <i class="uil uil-money-bill"></i>
                                </div>
                                <div class="d-flex align-items-start flex-wrap">
                                    <div class="me-10">
                                           <div class="loader-size loaderLastMonthCosting">
                                            <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                        </div>
                                        <div class="h1" id="lastMonthOtAmount">

                                        </div>
                                        <p class="mt-1 mb-0">
                                            Last Month OT Amount
                                        </p>
                                    </div>

                                    <!-- <div class="ap-po-details bg-none">
                                    <span class="color-danger fs-14 d-inline-block mt-2"><i
                                          class="las la-arrow-down"></i>
                                       <strong>25.36%</strong></span>
                                 </div> -->
                                </div>
                            </div>
                        </div>

                                                <div class="col-lg-3 overview-content overview-content3 b-none bg-none mb-lg-0 mb-20">
                            <div class="d-flex">
                                <div class="revenue-chart-box__Icon me-20 bg-primary color-white rounded-circle">
                                    <i class="uil uil-money-withdraw"></i>

                                </div>
                                <div class="d-flex align-items-start flex-wrap">
                                    <div class="me-10">
                                        <div class="loader-size loaderCosting">
                                            <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                        </div>

                                        <div class="h1" id="TodaySalaryAmt">

                                        </div>
                                        <p class="mt-1 mb-0">Today’s Salary</p>
                                    </div>

                                    <!-- <div class="ap-po-details bg-none">
                                    <span class="color-success fs-14  d-inline-block mt-2"><i
                                          class="las la-arrow-up"></i>
                                       <strong>25.36%</strong></span>
                                 </div> -->
                                </div>
                            </div>
                        </div>
                        <div class="col-lg-3 overview-content overview-content3 bg-none b-none mb-lg-0 mb-20">
                            <div class="d-flex">
                                <div class="revenue-chart-box__Icon me-20 bg-info color-white rounded-circle">
                                    <i class="uil uil-money-bill"></i>
                                </div>
                                <div class="d-flex align-items-start flex-wrap">
                                    <div class="me-10">
                                        <div class="loader-size loaderDailyOt">
                                            <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                                <span class="spin-dot badge-dot dot-primary"></span>
                                            </div>
                                        </div>
                                        <div class="h1" id="todayOtAmount">

                                        </div>
                                        <p class="mt-1 mb-0">Today's OT Amount</p>
                                    </div>

                                    <!-- <div class="ap-po-details bg-none">
                                    <span class="color-success fs-14 d-inline-block mt-2"><i
                                          class="las la-arrow-up"></i>
                                       <strong>25.36%</strong></span>
                                 </div> -->
                                </div>
                            </div>
                        </div>

                    </div>
                    <!-- Card 1 End  -->
                </div>
        </div>
                

              <div class="row">
                     <div class="col-lg-6">
                        <div class="card position-relative">

                           <div class="card-header">
                              <h6>This Month's OT</h6>
                           </div>

                           <div style="height:170px" class="card-body">
                              <div class="table-responsive">
                                 <table class="table table-border">
                                     <div class="loader-size loaderMontOt worker-loader">
                                         
                                         <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                         </div>
                                         

                                     </div>
                                    <thead>
                                       <tr>
                                          <th>Regular </th>
                                          <th>Extra</th>
                                          <th>Total</th>
                                          <th>Average</th>
                                       </tr>
                                    </thead>
                                    <tbody>

                                       <tr>
                                          <td><div id="MonRegularOtH"></div>
                                          </td>
                                           <td>
                                               <div id="MonExtOtH"></div>
                                           </td>
                                           <td>
                                               <div id="MonTotalOtH"></div>
                                           </td>
                                           <td>
                                               <div id="MonAvarageOtH"></div>
                                           </td>
 

                                       </tr>
                                        <tr>
                                            <td>
                                                <div id="MonRegularOtAmt"></div>
                                            </td>
                                            <td>
                                                <div id="MonExtOtAmt"></div>
                                            </td>
                                            <td>
                                                <div id="MonTotalOtAmt"></div>
                                            </td>
                                            <td>
                                                <div id="MonAvarageOtAmt"></div>
                                            </td>


                                        </tr>



                                    </tbody>
                                 </table>
                              </div>
                           </div>
                        </div>
                     </div>
                     <div class="col-lg-6">
                        <div class="card">

                           <div class="card-header">
                              <h6>Today's OT</h6>
                           </div>

                           <div style="height:170px" class="card-body">
                              <div class="table-responsive">
                                 <table class="table table-border">
                                      <div class="loader-size loaderDailyOt worker-loader">
                                         
                                         <div class="dm-spin-dots dot-size  dot-sizedot-sizedot-sizedot-size spin-sm">
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                             <span class="spin-dot badge-dot dot-primary"></span>
                                         </div>
                                         

                                     </div>
                                    <thead>
                                       <tr>
                                          <th>Regular </th>
                                          <th>Extra</th>
                                          <th>Total</th>
                                       </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><span id="DailyRegOTH"></span>
                                            </td>
                                            <td><span id="DailyExtOTH"></span></td>
                                            <td><span id="DailyTotalOTH"></span></td>
                                        </tr>
                                        <tr>
                                            <td><span id="DailyRegOtTk"></span>
                                            </td>
                                            <td><span id="DailyExtOtTk"></span></td>
                                            <td><span id="DailyTotalOtTk"></span></td>
                                        </tr>
                                    </tbody>
                                 </table>
                              </div>
                           </div>
                        </div>
                     </div>
                  </div>
                     
                </div>

                <!-----Start -->

               
                <!---Salary Comparison Table Start ---->


        </div>
    

    <%--<div id="overlayer">--%>
    <%--<div class="loader-overlay">--%>

    <%--</div>--%>
    <%--</div>--%>



    <script>

        //var rootUrl = 'http://cw-hrms-api.codehosting.xyz';
          var rootUrl = '<%= Session["__RootUrl__"]%>';
        //var rootUrl = 'http://localhost:8081';

        var DailyAttUrl = rootUrl + '/api/DailyAttendance/dailyAttendanceStatus';
        var DailyAttSumUrl = rootUrl + '/api/DailyAttendance/dailyAttendanceSummary';
        var CurrentEmpStatusUrl = rootUrl + '/api/DailyAttendance/currentEmployeeStatus';
        var GetTodaysCostingUrl = rootUrl + '/api/DailyAttendance/getTodaysCosting';
        var GetMonthlyCostingUrl = rootUrl + '/api/DailyAttendance/getlastMonthCosting';
        var GetLast7DaysPARatioUrl = rootUrl + '/api/DailyAttendance/dailyAttendancehistory';
        var GetNewJoinAndReleaseUrl = rootUrl + '/api/DailyAttendance/newJoinReleaseHistory';
        var GetSalaryComparisonLast12MonthUrl = rootUrl + '/api/DailyAttendance/salaryComparison';
        var GetMonthlyNewJoinRelaseUrl = rootUrl + '/api/DailyAttendance/monthlyNewjoinAndRelease';
        var GetMonthlyOtCalculationUrl = rootUrl + '/api/DailyAttendance/monthlyOtCalculation';
        var GetDailyOtCalculationUrl = rootUrl + '/api/DailyAttendance/dailyOtCalculation';

        var companyId = '<%= Session["__GetCompanyId__"] %>';
        var token = '<%= Session["__UserToken__"] %>';
        //var token = 'eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiIiLCJpYXQiOjE3MTQ2MjQ5MjYsImV4cCI6MTc0NjE2MDkyNiwiYXVkIjoiIiwic3ViIjoiSldUU2VydmljZUFjY2Vzc1Rva2VuIn0.tVlIuOLas2VxEnBohuaIXXQR2Lju_2h8yVjCDizQh9o';

        function SearchData() {
            var formattedDate = $(".hasDatepicker").val();
            var promises = [];

            // Push promises for each function call
            promises.push(new Promise(function (resolve, reject) {
                GetDailyAttendanceStatus(formattedDate);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetLast7DaysPARatio(formattedDate);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetDailyAttSummary(formattedDate);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetCurrentEmpStatus();
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetMonthlyNewJoinRelase(formattedDate);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetNewJoinAndRelease(formattedDate);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetSalaryComparisonLast12Month(formattedDate);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetMonthlyCosting(formattedDate);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetTodaysCosting(formattedDate);
                resolve();
            }));
            promises.push(new Promise(function (resolve, reject) {
                GetDailyOtCalculation(formattedDate);
                resolve();
            }));
            promises.push(new Promise(function (resolve, reject) {
                GetMonthlyOtCalculation(formattedDate);
                resolve();
            }));

            promises.reduce(function (previousPromise, nextPromise) {
                return previousPromise.then(function () {
                    return nextPromise;
                });
            }, Promise.resolve());
        }
        $(document).ready(function () {
            var initialDate = new Date();
            var options = { day: 'numeric', month: 'long', year: 'numeric' };
            var formattedDate = initialDate.toLocaleDateString('en-US', options);
            document.querySelector('.hasDatepicker').value = formattedDate;

            var date = formattedDate;
            // Define an array to hold promises
            var promises = [];
            promises.push(new Promise(function (resolve, reject) {
                GetDailyAttendanceStatus(date);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetLast7DaysPARatio(date);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetDailyAttSummary(date);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetCurrentEmpStatus();
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetMonthlyNewJoinRelase(date);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetNewJoinAndRelease(date);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetSalaryComparisonLast12Month(date);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetMonthlyCosting(date);
                resolve();
            }));

            promises.push(new Promise(function (resolve, reject) {
                GetTodaysCosting(date);
                resolve();
            }));
            promises.push(new Promise(function (resolve, reject) {
                GetDailyOtCalculation(formattedDate);
                resolve();
            }));
            promises.push(new Promise(function (resolve, reject) {
                GetMonthlyOtCalculation(formattedDate);
                resolve();
            }));
            // Execute promises sequentially
            promises.reduce(function (previousPromise, nextPromise) {
                return previousPromise.then(function () {
                    return nextPromise;
                });
            }, Promise.resolve());
        });


        function SalaryComparisonChart(idName, width, height, salaryData, monthData) {

            var height;
            if (window.innerWidth <= 1200) {
                height = 100;
            }
            else if (window.innerWidth <= 1400) {
                height = 240;
            }
            else if (window.innerWidth <= 1600) {
                height = 250;
            }
            else if (window.innerWidth <= 1920) {
                height =250;
            }
            else if (window.innerWidth <= 3840) {
                height = 250;
            }

            var optionRadial = {
                series: [{
                    data: salaryData // Use the salary data here
                }],
                colors: ['#8231D3'],
                chart: {
                    width: width,
                    height: height,
                    type: 'bar',

                },
                tooltip: {
                    enabled: true,
                    y: {
                        formatter: function (val) {
                            return "Salary: Tk" + val; // Format tooltip to show salary
                        }
                    }
                },
                legend: {
                    show: false
                },
                plotOptions: {
                    bar: {
                        borderRadius: 4,
                        horizontal: false,
                    }
                },
                dataLabels: {
                    enabled: false
                },
                xaxis: {
                    categories: monthData // Use the month data here
                }
            };



            if ($(idName).length > 0) {
                new ApexCharts(document.querySelector(idName), optionRadial).render();
            }
        }




        //function SalaryComparisonChart(idName, width, height = "100", salaryData, monthData) {
        //    var optionRadial = {
        //        series: [{
        //            data: salaryData // Use the salary data here
        //        }],
        //        colors: ['#8231D3'],
        //        chart: {
        //            width: width,
        //            height: height,
        //            type: 'bar',
        //        },
        //        legend: {
        //            show: false
        //        },
        //        plotOptions: {
        //            bar: {
        //                borderRadius: 4,
        //                horizontal: false,
        //            }
        //        },
        //        dataLabels: {
        //            enabled: false
        //        },
        //        xaxis: {
        //            categories: monthData // Use the month data here
        //        }
        //    };

        //    if ($(idName).length > 0) {
        //        new ApexCharts(document.querySelector(idName), optionRadial).render();
        //    }
        //}


        function GetDailyAttendanceStatus(date) {
            // Show loader
            $('#totalEmp').text('');
            $('#totalEmpRatio').text('');
            $('#todayPresent').text('');
            $('#todayPresentRatio').text('');
            $('#todayAbsent').text('');
            $('#todayAbsentRatio').text('');
            $('#todayLate').text('');
            $('#todayLateRatio').text('');
            $('#todayLeave').text('');
            $('#todayLeaveRatio').text('');
            $('#WHOffDuty').text('');

            $('.loaderDaily').show();
            date = date;
            ApiCall(DailyAttUrl, companyId, date, token)
                .then(function (response) {
                    // Hide loader
                    $('.loaderDaily').hide();

                    // Update UI with data
                    $('#totalEmp').text(response.total);
                    $('#totalEmpRatio').text(response.totalPerc +'%');
                    $('#todayPresent').text(response.present);
                    $('#todayPresentRatio').text(response.pressentRatio +'%');
                    $('#todayAbsent').text(response.absent);
                    $('#todayAbsentRatio').text(response.absentRatio +'%');
                    $('#todayLate').text(response.late);
                    $('#todayLateRatio').text(response.lateRatio +'%');
                    $('#todayLeave').text(response.leave);
                    $('#todayLeaveRatio').text(response.leavePers +'%');
                    $('#WHOffDuty').text(response.offDayDuty);
                    $('#WHOffDeutyPers').text(response.offDayDutyPerc +'%');
                })
                .catch(function (error) {
                    // Hide loader
                    $('#loaderDaily').hide();
                    console.error('Error occurred while fetching data:', error);
                });
        }

        function GetLast7DaysPARatio(date) {
    $('.loaderSevenDays').show();
    var date = date;
    // Clear chart data arrays
    var p = [];
    var a = [];
    var w = [];
    ApiCall(GetLast7DaysPARatioUrl, companyId, date, token)
        .then(function(response) {
            $('.loaderSevenDays').hide();
            var GetLast7DaysPARatioData = response;
            

            // Clear previous data
            p = [];
            a = [];
            w = [];

            var i = 0;
            for (i; i < GetLast7DaysPARatioData.length; i++) {
                p.push(GetLast7DaysPARatioData[i].present);
                a.push(GetLast7DaysPARatioData[i].absent);
                w.push(GetLast7DaysPARatioData[i].weekDay.substring(0, 3));
            }

            chartjsBarChart("salesGrowthToday", p, a, w, 350, "Present", "Absent");

        })
        .catch(function(error) {
            $('.loaderSevenDays').hide();
            console.error('Error occurred while fetching data:', error);
        });
}

       
        
        function GetNewJoinAndRelease(date) {
            $('.loaderMonthlyNewRel').show();
            var NewJoin = [];
            var Release = [];
            var Month = [];
            var date = date;
            ApiCall(GetNewJoinAndReleaseUrl, companyId, date, token)
                .then(function (response) {
                    $('.loaderMonthlyNewRel').hide();

                    var GetNewJoinAndRelease = response;
                     NewJoin = [];
                     Release = [];
                     Month = [];
                    var i = 0;
                    for (i; i < GetNewJoinAndRelease.length; i++) {
                        NewJoin.push(GetNewJoinAndRelease[i].newJoin);
                        Release.push(GetNewJoinAndRelease[i].release);
                        Month.push(GetNewJoinAndRelease[i].month);
                    }

                    chartjsBarChart(
                        "profitGrowthToday",
                        NewJoin,
                        Release,
                        Month,
                        140,
                        "New Join",
                        "Released"
                    );

                })
                .catch(function (error) {
                    console.error('Error occurred while fetching data:', error);
                });
        }
        function GetTodaysCosting(date) {
            $('.loaderCosting').show();
            $('#TodaySalaryAmt').text('');
   
            // Initialize responseData

            ApiCall(GetTodaysCostingUrl, companyId, date, token)
                .then(function (response) {
                    $('.loaderCosting').hide();
                    responseData = response[0];
                    $('#TodaySalaryAmt').text(responseData.dailySalaryAmnt);
                   
                })
                .catch(function (error) {
                    $('.loaderCosting').hide();
                    console.error('Error occurred while fetching data:', error);
                });
        }


        function GetSalaryComparisonLast12Month(date) {
            $('.loaderSelaryCom').show();
            var salary = [];
            var monthname = [];

            $(".SalaryComparisonChart").html("");
         
   
            ApiCall(GetSalaryComparisonLast12MonthUrl, companyId, date, token)
                .then(function (response) {
                    $('.loaderSelaryCom').hide();
                    var GetSalaryComparison = response;
                    salary = [];
                    monthname = [];
                    for (var i = 0; i < GetSalaryComparison.length; i++) {
                        salary.push(GetSalaryComparison[i].salary);
                        monthname.push(GetSalaryComparison[i].monthname);
                    }
                    SalaryComparisonChart('.SalaryComparisonChart', '100%', height=height, salary, monthname);
                })
                .catch(function (error) {
                    console.error('Error occurred while fetching data:', error);
                });
        }

        function GetMonthlyCosting(date) {
            $('.loaderLastMonthCosting').show();
            $('#LastMonthCosting').text('');
            $('#lastMonthOtAmount').text('');
            ApiCall(GetMonthlyCostingUrl, companyId, date, token).then(function (response) {
                $('.loaderLastMonthCosting').hide();
                GetDataFromArray = response[0];
                $('#LastMonthCosting').text(GetDataFromArray.monthlyPaySalaryamnt);
                $('#lastMonthOtAmount').text(GetDataFromArray.monthlyOTPay);
            }).catch(function (error) {
                console.error('Error occurred while fetching data:', error);
            });
        }

        function GetMonthlyNewJoinRelase(date) {
            $('.loaderCurrStatusRel').show();
            $('#Recruitment').text('');
            $('#Released').text('');
            ApiCall(GetMonthlyNewJoinRelaseUrl, companyId, date, token).then(function (response) {
                $('.loaderCurrStatusRel').hide();
                var GetDataFromArray = response[0];
                $('#Recruitment').text(GetDataFromArray.newJoin);
                $('#Released').text(GetDataFromArray.release);
            }).catch(function (error) {
                console.error('Error occurred while fetching data:', error);
            });
        }

        function GetCurrentEmpStatus() {
            $('.loaderCurrStatus').show();
            // Clear previous data
            var maleFemaleRatio = [];
            var StaffWorkerRatio = [];
            // Clear text values
            $('#totalEmpReg').text('');
            $('#female').text('');
            $('#male').text('');
            $('#overtime').text('');

            console.log("Rubel")
          
            ApiCallSam(CurrentEmpStatusUrl, companyId, token)
                .then(function (response) {
                    $('.loaderCurrStatus').hide();
                    // Update text values
                    $('#totalEmpReg').text(response.totalEmployee);
                    $('#female').text(response.female);
                    $('#male').text(response.male);
                    $('#overtime').text(response.overTime);
                    $('.MaleFemaleRatio').html("").text('');
                    $('.StapWorkerRatio').html("").text('');

                    // Populate maleFemaleRatio array
                    malePercentage = response.malePers;
                    femalePercentage = response.femalePers;
                    maleFemaleRatio = [malePercentage, femalePercentage];
                    DonutChart('.MaleFemaleRatio', maleFemaleRatio, 180, 180, ['Male', 'Female'], ['#02A3FE', '#EC49A6'], "60%");

                    // Populate StaffWorkerRatio array
                    StaffPercentage = response.staffPers;
                    WorkerPercentage = response.workerpers;
                    StaffWorkerRatio = [StaffPercentage, WorkerPercentage];
                    // Pass the arrays to the DonutChart function
                    DonutChart('.StapWorkerRatio', StaffWorkerRatio, 180, 180, ['Staff', 'Worker'], ['#385e97', '#fb6542'], "60%");

                })
                .catch(function (error) {
                    $('.loaderCurrStatus').hide();
                    console.error('Error occurred while fetching data:', error);
                });
        }

        function GetMonthlyOtCalculation(date) {

            $('.loaderMontOt').show();
            // Clear text values
            $('#MonTotalOtH').text('');
            $('#MonExtOtH').text('');
            $('#MonRegularOtH').text('');
            $('#MonAvarageOtH').text('');

            //Clear Ot Amt
            $('#MonTotalOtAmt').text('');
            $('#MonExtOtAmt').text('');
            $('#MonRegularOtAmt').text('');
            $('#MonAvarageOtAmt').text('');

            ApiCall(GetMonthlyOtCalculationUrl, companyId, date, token)
                .then(function (response) {
                    $('.loaderMontOt').hide();
                    // Update text values
                    $('#MonRegularOtH').text(response.regularOtHour +'H');
                    $('#MonExtOtH').text(response.extraOtHour +'H');
                    $('#MonTotalOtH').text(response.totalOtHour +'H');
                    $('#MonAvarageOtH').text(response.avgOtHour +'H');

                    //Update Ot Amt
                    $('#MonRegularOtAmt').text(response.regularOTAmount+'TK');
                    $('#MonExtOtAmt').text(response.extraOtAmnt +'TK');
                    $('#MonTotalOtAmt').text(response.totalOtAmount +'TK');
                    $('#MonAvarageOtAmt').text(response.avgOtAmnt +'TK');

                })
                .catch(function (error) {
                    $('.loaderCurrStatus').hide();
                    console.error('Error occurred while fetching data:', error);
                });
        }

        function GetDailyOtCalculation(date) {
            console.log('Hello World');
            $('.loaderDailyOt').show();
            // Clear text values
            $('#DailyRegOTH').text('');
            $('#DailyExtOTH').text('');
            $('#DailyTotalOTH').text('');


            //Clear Ot Amt
            $('#DailyRegOtTk').text('');
            $('#DailyExtOtTk').text('');
            $('#DailyTotalOtTk').text('');
            $('#todayOtAmount').text('');


            ApiCall(GetDailyOtCalculationUrl, companyId, date, token)
                .then(function (response) {
                    console.log('Test Ot Hours' + response.regularOtHour);
                    $('.loaderDailyOt').hide();
                    // Update text values
                    $('#DailyRegOTH').text(response.regularOtHour+'H');
                    $('#DailyExtOTH').text(response.extraOtHour+'H');
                    $('#DailyTotalOTH').text(response.totalOtHour+'H');


                    //Clear Ot Amt
                    $('#DailyRegOtTk').text(response.regularOTAmount+'Tk');
                    $('#DailyExtOtTk').text(response.extraOtAmnt +'Tk');
                    $('#todayOtAmount').text(response.totalOtAmount);
                    $('#DailyTotalOtTk').text(response.totalOtAmount +'Tk');

                })
                .catch(function (error) {
                    $('.loaderCurrStatus').hide();
                    console.error('Error occurred while fetching data:', error);
                });
        }


        function GetDailyAttSummary(date) {
            $('.loaderDailySum').show();
    var tableBody = $('#tblDailyAttSummary tbody');
    $('.loaderDailySum').show().promise().done(function() {
        tableBody.empty(); // Clear table when loader is shown
    });

    ApiCall(DailyAttSumUrl, companyId, date, token)
        .then(function(responsedata) {
            $('.loaderDailySum').hide(); // Hide loader
            // Build table rows with new data
            var newRow = '';
            var i = 1;
           let totalEmployeesSum = 0;
            let totalMalesSum = 0;
            let totalFemalesSum = 0;
            let totalPresentSum = 0;
            let totalPresentRatioSum = 0;
            let totalAbsentSum = 0;
            let totalAbsentRatioSum = 0;
            let totalLeaveSum = 0;
            let totalLeaveRatioSum = 0;
            let totalLateSum = 0;
            let totalLateRatioSum = 0;
            let totalOffDaySum = 0;
            let totalOffDayRatioSum = 0;
            responsedata.forEach(item => {

                newRow += `<tr>
                    <td>${i++}</td> 
                    <td>${item.department}</td>
                    <td>${item.totalEmployee}</td>
                    <td>${item.male}</td>
                    <td>${item.female}</td>
                    <td>${item.present}</td>
                    <td>${item.presentRatio}</td>
                    <td>${item.absent}</td>
                    <td>${item.absentRatio}</td>
                    <td>${item.leave}</td>
                    <td>${item.leaveRatio}</td>
                    <td>${item.late}</td>
                    <td>${item.lateRatio}</td>
                    <td>${item.offDay}</td>
                    <td>${item.offdayRatio}</td>
                </tr>`;


                totalEmployeesSum += item.totalEmployee;
                totalMalesSum += item.male;
                totalFemalesSum += item.female;
                totalPresentSum += item.present;
                totalPresentRatioSum += item.presentRatio;
                totalAbsentSum += item.absent;
                totalAbsentRatioSum += item.absentRatio;
                totalLeaveSum += item.leave;
                totalLeaveRatioSum += item.leaveRatio;
                totalLateSum += item.late;
                totalLateRatioSum += item.lateRatio;
                totalOffDaySum += item.offDay;
                totalOffDayRatioSum += item.offdayRatio;
            });
            tableBody.append(newRow);
            var debed = i - 1;
            console.log(debed);
            var totatPreRet = (totalPresentSum * 100) / totalEmployeesSum ;
            var totalAbsRet = (totalAbsentSum * 100)/ totalEmployeesSum;
            var totalLvRet =(totalLeaveSum * 100)/ totalEmployeesSum ;
            var totalLateRatio = (totalLateSum * 100)/ totalEmployeesSum;;
            var totalWHOFRatio = (totalOffDaySum * 100)/ totalEmployeesSum;;
            let summaryRow = `<tr>
            <td colspan="2"><strong>Total</strong></td>
            <td><strong>${totalEmployeesSum}</strong></td>
            <td><strong>${totalMalesSum}</strong></td>
            <td><strong>${totalFemalesSum}</strong></td>
            <td><strong>${totalPresentSum}</strong></td>
            <td><strong>${totatPreRet.toFixed(2)}</strong></td>
            <td><strong>${totalAbsentSum}</strong></td>
            <td><strong>${totalAbsRet.toFixed(2)}</strong></td>
            <td><strong>${totalLateSum}</strong></td>
            <td><strong>${totalLateRatio.toFixed(2)}</strong></td>
            <td><strong>${totalLeaveSum}</strong></td>
            <td><strong>${totalLvRet.toFixed(2)}</strong></td>

            <td><strong>${totalOffDaySum}</strong></td>
            <td><strong>${totalWHOFRatio.toFixed(2)}</strong></td>
        </tr>`;
            tableBody.append(summaryRow);  
        })
        .catch(function(error) {
            $('.loaderDailySum').hide(); // Hide loader on error
            console.error('Error occurred while fetching data:', error);
        });
}



        function ApiCallGetCosting(url, date, token) {
            return new Promise(function (resolve, reject) {
                $.ajax({
                    url: url,
                    type: 'GET',
                    dataType: 'json',
                    headers: {
                        'Authorization': 'Bearer ' + token  // Corrected header field
                    },
                    data: {
                        date: date,
                    },
                    success: function (data) {
                        resolve(data);

                    },
                    error: function (xhr, status, error) {
                        console.error('Error occurred while fetching data:', status, error);
                        reject(error);
                    }
                });
            });
        }

        //api calling function

        function ApiCall(url, companyId, date, token) {
            return new Promise(function (resolve, reject) {
                $.ajax({
                    url: url,
                    type: 'GET',
                    dataType: 'json',
                    headers: {
                        'Authorization': 'Bearer ' + token  // Corrected header field
                    },
                    data: {
                        companyId: companyId,
                        date: date
                    },
                    success: function (data) {
                        resolve(data);

                    },
                    error: function (xhr, status, error) {
                        console.error('Error occurred while fetching data:', status, error);
                        reject(error);
                    }
                });
            });
        }

        function ApiCallSam(url, companyId, token) {
            return new Promise(function (resolve, reject) {
                $.ajax({
                    url: url,
                    type: 'GET',
                    dataType: 'json',
                    headers: {
                        'Authorization': 'Bearer ' + token  // Corrected header field
                    },
                    data: {
                        companyId: companyId,
                    },
                    success: function (data) {
                        resolve(data);

                    },
                    error: function (xhr, status, error) {
                        console.error('Error occurred while fetching data:', status, error);
                        reject(error);
                    }
                });
            });
        }

    </script>

</asp:Content>
