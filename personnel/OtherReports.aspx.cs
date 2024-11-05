using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.personnel
{
    public partial class OtherReports : System.Web.UI.Page
    {
        DataTable dt;
        DataTable dtSetPrivilege;
        string CompanyId = "";
        string Cmd = "";


        //string Cmd = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            int[] pagePermission = { 276 };

            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                classes.commonTask.LoadBranch(ddlCompany);
                ddlCompany.SelectedIndex = 1;

                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);

                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                setPrivilege();
                if (!classes.commonTask.HasBranch())
                    ddlCompany.Enabled = false;
               
            }
        }
        //private void addAllTextInShift()
        //{
        //    ddlShiftList.Items.RemoveAt(0);
        //    if (ddlShiftList.Items.Count > 1)
        //        ddlShiftList.Items.Insert(0, new ListItem("All", "00"));
        //}

        protected void rblEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            classes.commonTask.LoadEmpCardNoByEmpType(ddlCardNo, CompanyId, rblEmpType.SelectedValue);
            ddlCardNo.Items.Insert(0, new ListItem("Select For Individual", "0"));
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
        }
        private void setPrivilege()
        {
            try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                classes.commonTask.LoadBranch(ddlCompany, ViewState["__CompanyId__"].ToString());

                //string[] AccessPermission = new string[0];
                ////System.Web.UI.HtmlControls.HtmlTable a = tblGenerateType;
                //AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "EmpContactReport.aspx", ddlCompany, WarningMessage, tblGenerateType, btnPreview);
               // ViewState["__ReadAction__"] = AccessPermission[0];


                //classes.commonTask.LoadInitialShift(ddlShiftList, ViewState["__CompanyId__"].ToString());
                //addAllTextInShift();
                ddlCompany.SelectedValue = ViewState["__CompanyId__"].ToString();
                classes.commonTask.LoadDepartment(ddlCompany.SelectedValue, lstAll);
                classes.commonTask.LoadEmpCardNoByEmpType(ddlCardNo, ddlCompany.SelectedValue, rblEmpType.SelectedValue);
                if (ddlCardNo != null)
                    ddlCardNo.Items.Insert(0, new ListItem("Select For Individual", "0"));
                int[] reportPermission = { 451, 452, 453, 454, 455, 456, 457, 458, 459, 460, 461, 462, 463, 464, 465 };
                int[] userPagePermition = AccessControl.hasPermission(reportPermission);
                commonTask.loadReportName(ddlReportType, userPagePermition);

            }
            catch { }
        }
        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            //classes.commonTask.LoadInitialShift(ddlShiftList, CompanyId);
            //addAllTextInShift();
            lstSelected.Items.Clear();
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
            classes.commonTask.LoadEmpCardNoByEmpType(ddlCardNo, CompanyId, rblEmpType.SelectedValue);
            ddlCardNo.Items.Insert(0, new ListItem("Select For Individual", "0"));
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            genaretOtherReports();
        }

        private void genaretOtherReports()
        {

            string condition = " Where pei.CompanyId='" + ddlCompany.SelectedValue + "' and pecs.EmpStatus in(1, 8) ";
            if (ddlCardNo.SelectedValue == "0")
            {
                if (lstSelected.Items.Count == 0)
                {
                    lblMessage.InnerText = "warning-> Please Select Any Department!";
                    lstSelected.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                    return;
                }
                //  condition= condition+" and pecs.DptId " + classes.commonTask.getDepartmentList(lstSelected);
                condition += " and pecs.DptId " + classes.commonTask.getDepartmentList(lstSelected);
                condition += (rblEmpType.SelectedValue.ToString().Equals("All")) ? " " : " and pecs.EmpTypeId=" + rblEmpType.SelectedValue + "";
            }
            else
                condition += " and pecs.SN=" + ddlCardNo.SelectedValue;

            if (ddlReportType.SelectedValue == "452")
            {
                Cmd = "SELECT pep.EmpId, pei.EmpCardNo,pecs.BasicSalary,pecs.HouseRent,pecs.MedicalAllownce,pecs.ConvenceAllownce,pecs.FoodAllownce,pecs.EmpPresentSalary,grd.GrdNameBangla,dsg.DsgNameBn,pep.HusbandOrWifeNameBN,cmpi.CompanyLogo, cmpi.CompanyNameBangla, cmpi.AddressBangla, pei.EmpNameBn, pea.PreVillageBangla + ',' + pea.PerPOBangla + ',' + preThana.ThaNameBangla + ',' + preDistrict.DstBangla AS PresentAddress, pep.FatherNameBn, pep.MotherNameBN, CONVERT(VARCHAR(10), pep.DateOfBirth, 105) AS DateOfBirth, pea.PerVillageBangla + ',' + pea.PerPOBangla + ',' + perThana.ThaNameBangla + ',' + perDistrict.DstBangla AS PermanentAddress, CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate, pep.Sex, pep.Age FROM  dbo.Personnel_EmployeeInfo AS pei left JOIN dbo.Personnel_EmpPersonnal AS pep ON pei.EmpId = pep.EmpId INNER JOIN HRD_CompanyInfo as cmpi ON pei.CompanyId = cmpi.CompanyId INNER JOIN Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId LEFT JOIN HRD_Grade as grd on pecs.GrdName = grd.GrdName LEFT OUTER JOIN dbo.Personnel_EmpAddress AS pea ON pei.EmpId = pea.EmpId LEFT JOIN HRD_Designation as dsg ON pecs.DsgId = dsg.DsgId LEFT OUTER JOIN dbo.Personnel_EmpNominee AS pen ON pei.EmpId = pen.EmpId LEFT OUTER JOIN dbo.HRD_District AS perDistrict ON pea.PreCity = perDistrict.DstId LEFT OUTER JOIN dbo.HRDThanaInfo AS perThana ON pea.PerThanaId = perThana.ThaId LEFT OUTER JOIN dbo.HRD_District AS preDistrict ON pea.PreCity = preDistrict.DstId LEFT OUTER JOIN dbo.HRDThanaInfo AS preThana ON pea.PreThanaId = preThana.ThaId " + condition + " order by CustomOrdering ";

                if (DataFill(Cmd))
                {
                    Session["__AppoinmentLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=AppointmentLetter-joiningletter');", true);
                }

            } //Appointment Letter
            else if (ddlReportType.SelectedValue == "453")
            {
                Cmd = "SELECT pep.EmpId, comp.CompanyNameBangla,comp.AddressBangla,comp.CompanyLogo, pei.EmpNameBn, dsg.DsgNameBN,dpt.DptNameBn, grp.GNameBn, CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105)as EmpJoiningDate  FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.DsgId= dsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId inner join HRD_CompanyInfo as comp on pecs.CompanyId = comp.CompanyId " + condition + " order by CustomOrdering";
                if (DataFill(Cmd))
                {
                    Session["__JoiningLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=joiningletter');", true);
                }
            } //Joining Letter
            else if (ddlReportType.SelectedValue == "457")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn, dsg.DsgNameBN, pei.EmpCardNo, dpt.DptNameBn, grp.GNameBN, CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105)as EmpJoiningDate ,cmp.CompanyNameBangla,cmp.AddressBangla,cmp.CompanyLogo  FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId Inner join HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.DsgId= dsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId " + condition + " and pep.Sex='Female' order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__LadyWorkerNightFormate__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=LadyWorkerNightFormate');", true);
                }
            } //Lady worker night format
            else if (ddlReportType.SelectedValue == "456")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpCardNo, cmp.CompanyNameBangla, cmp.AddressBangla,cmp.CompanyLogo, pei.EmpNameBn,pea.PreVillageBangla + ', ' + pea.PerPOBangla + ', ' + prethana.ThaNameBangla + ', ' + preDistrict.DstBangla as PresentAddress,pep.FatherNameBn,pep.MotherNameBN,CONVERT(VARCHAR(10), pep.DateOfBirth, 105) AS DateOfBirth,pea.PerVillageBangla + ',' + pea.PerPOBangla + ', ' + perthana.ThaNameBangla + ', ' +perDistrict.DstBangla as PermanentAddress,CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate,pep.Sex,pep.Age FROM Personnel_EmployeeInfo as pei LEFT JOIN Personnel_EmpPersonnal as pep ON pei.EmpId = pep.EmpId LEFT JOIN Personnel_EmpAddress as pea ON pei.EmpId = pea.EmpId LEFT JOIN Personnel_EmpNominee as pen ON pei.EmpId = pen.EmpId LEFT JOIN HRD_District as perDistrict ON pea.PreCity = perDistrict.DstId LEFT JOIN HRDThanaInfo as perThana ON pea.PerThanaId = perThana.ThaId LEFT JOIN HRD_District as preDistrict ON pea.PreCity = preDistrict.DstId Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 INNER JOIN HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId LEFT JOIN HRDThanaInfo as preThana ON pea.PreThanaId = preThana.ThaId left join HRD_Department as dpt on pecs.DptId = dpt.DptId " + condition + " order by CustomOrdering";
                if (DataFill(Cmd))
                {
                    Session["__medical_formet__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=medical_formet');", true);
                }


            } //Medical Format
            else if (ddlReportType.SelectedValue == "451")
            {
                Cmd = "SELECT pep.EmpId,cmp.CompanyNameBangla,cmp.CompanyLogo, cmp.AddressBangla ,pei.EmpCardNo,dsg.DsgNameBn ,dpt.DptNameBn, grp.GNameBn,grd.GrdNameBangla,pecs.EmpPresentSalary, pei.EmpNameBn, pep.FatherNameBn,pep.MotherNameBN,pea.PreVillageBangla, pea.PerPOBangla,  prethana.ThaNameBangla, preDistrict.DstBangla  ,pea.PerVillageBangla ,pea.PerPOBangla,  perthana.ThaNameBangla ,perDistrict.DstBangla, pep.LastEdQualification,CONVERT(VARCHAR(10), pep.DateOfBirth, 105) AS DateOfBirth,pep.Age,pep.MaritialStatus, pep.Sex, CONVERT(VARCHAR(10) , pei.EmpJoiningDate, 105) AS EmpJoiningDate  FROM Personnel_EmployeeInfo as pei LEFT JOIN Personnel_EmpPersonnal as pep ON pei.EmpId = pep.EmpId inner join HRD_CompanyInfo as cmp on pei.CompanyId =cmp.CompanyId LEFT JOIN Personnel_EmpAddress as pea ON pei.EmpId = pea.EmpId LEFT JOIN Personnel_EmpNominee as pen ON pei.EmpId = pen.EmpId LEFT JOIN HRD_District as perDistrict ON pea.PreCity = perDistrict.DstId LEFT JOIN HRDThanaInfo as perThana ON pea.PerThanaId = perThana.ThaId LEFT JOIN HRD_District as preDistrict ON pea.PreCity = preDistrict.DstId Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN HRDThanaInfo as preThana ON pea.PreThanaId = preThana.ThaId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.DsgId= dsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId left join HRD_Grade as Grd on pecs.GrdName=Grd.GrdName " + condition + " order by CustomOrdering";
                if (DataFill(Cmd))
                {
                    Session["__job_application__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=JobApplicationWorkerCopy');", true);
                }
            }  //Job Application
            else if (ddlReportType.SelectedValue == "454")
            {
                Cmd = "SELECT pep.EmpId, pei.EmpCardNo, pei.EmpNameBn, cmp.CompanyLogo, cmp.CompanyNameBangla, cmp.AddressBangla, pea.PreVillageBangla + ',' + pea.PerPOBangla + ',' + preThana.ThaNameBangla + ',' + preDistrict.DstBangla AS PresentAddress, pep.FatherNameBn, pep.MotherNameBN, pep.HusbandOrWifeNameBN, CONVERT(VARCHAR(10), pep.DateOfBirth, 105) AS DateOfBirth, pea.PerVillageBangla, pea.PerPOBangla, perThana.ThaNameBangla, perDistrict.DstBangla AS PermanentAddress, CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate, dsg.DsgNameBn, dpt.DptNameBn, grp.GNameBn, pen.NomineeNameBN, pen.NomineeAge, pen.NomineeNID, pen.NomineeAddressBN, pep.Sex, pep.Age, pei.CompanyId, pei.EmpStatus, pecs.DptId, pecs.CustomOrdering, pen.NomineeRelationBN FROM dbo.Personnel_EmployeeInfo AS pei LEFT JOIN dbo.Personnel_EmpPersonnal AS pep ON pei.EmpId = pep.EmpId INNER JOIN dbo.Personnel_EmpCurrentStatus AS pecs ON pei.EmpId = pecs.EmpId AND pecs.IsActive = 1 INNER JOIN HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId LEFT OUTER JOIN dbo.Personnel_EmpAddress AS pea ON pei.EmpId = pea.EmpId LEFT OUTER JOIN dbo.HRD_Department AS dpt ON pecs.DptId = dpt.DptId LEFT OUTER JOIN dbo.HRD_Designation AS dsg ON pecs.DsgId = dsg.DsgId LEFT OUTER JOIN dbo.HRD_Group AS grp ON pecs.GId = grp.GId LEFT OUTER JOIN dbo.HRD_Grade AS Grd ON pecs.GrdName = Grd.GrdName LEFT OUTER JOIN dbo.Personnel_EmpNominee AS pen ON pei.EmpId = pen.EmpId LEFT OUTER JOIN dbo.HRD_District AS perDistrict ON pea.PreCity = perDistrict.DstId LEFT OUTER JOIN dbo.HRDThanaInfo AS perThana ON pea.PerThanaId = perThana.ThaId LEFT OUTER JOIN dbo.HRD_District AS preDistrict ON pea.PreCity = preDistrict.DstId LEFT OUTER JOIN dbo.HRDThanaInfo AS preThana ON pea.PreThanaId = preThana.ThaId " + condition + " order by CustomOrdering ";



                //Cmd = "select EmpId,EmpCardNo,EmpNameBn, PresentAddress,FatherNameBn, MotherNameBN,HusbandOrWifeNameBN,DateOfBirth,PerVillageBangla,PerPOBangla,ThaNameBangla,PermanentAddress,EmpJoiningDate,DsgNameBn,DptNameBn,GNameBn,NomineeNameBN, NomineeAge,NomineeNId,NomineeAddressBN,Sex,Age,NomineeRelationBN from v_Nominee_info   " + condition + " order by CustomOrdering ";

                if (DataFill(Cmd))
                {
                    Session["__nominee_report__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=nominee_report');", true);
                }


            } //Nominee Report

            else if (ddlReportType.SelectedValue == "455")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn, pei.EmpCardNo, dsg.DsgNameBn,			DPT.DptNameBn,grp.GNameBn, pecs.BasicSalary, pecs.HouseRent, pecs.MedicalAllownce, pecs.ConvenceAllownce, pecs.FoodAllownce,  pecs.EmpPresentSalary,pecs.OthersAllownce, grd.GrdNameBangla, cmpi.CompanyNameBangla,cmpi.CompanyLogo, cmpi.AddressBangla, CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate FROM Personnel_EmployeeInfo AS pei LEFT JOIN Personnel_EmpPersonnal AS pep ON pei.EmpId = pep.EmpId INNER JOIN HRD_CompanyInfo AS cmpi ON pei.CompanyId = cmpi.CompanyId LEFT JOIN Personnel_EmpCurrentStatus AS pecs ON pei.EmpId = pecs.EmpId LEFT JOIN HRD_Group as grp ON pecs.GId = grp.GId INNER JOIN HRD_Department AS DPT ON pecs.DptId = DPT.DptId  LEFT JOIN HRD_Grade AS grd ON pecs.GrdName = grd.GrdName LEFT JOIN HRD_Designation AS dsg ON pecs.DsgId = dsg.DsgId " + condition + " order by CustomOrdering ";
                if (DataFill(Cmd))
                {
                    Session["__wages_statment__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=wages_statment');", true);
                }
            }  //Wages Statement
            else if (ddlReportType.SelectedValue == "458")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn, dsg.DsgNameBN, pei.EmpCardNo, dpt.DptNameBn, grp.GNameBN,cmp.CompanyLogo,cmp.CompanyNameBangla,cmp.AddressBangla  FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId Inner join HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.DsgId= dsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId " + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__Dismiss_Letter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DismissLetter');", true);
                }
            }  //Dismiss Letter

            else if (ddlReportType.SelectedValue == "459")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn, dsg.DsgNameBN, pei.EmpCardNo, dpt.DptNameBn, grp.GNameBN,cmp.CompanyLogo,cmp.CompanyNameBangla,cmp.AddressBangla  FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId Inner join HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.DsgId= dsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId " + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__ShowCauseLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=ShowCauseLetter');", true);
                }
            } //Show Cause Letter
            else if (ddlReportType.SelectedValue == "460")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn,CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate, pecs.EffectiveMonth ,promDsg.DsgNameBn as PromotionDsgNameBn, dsg.DsgNameBN, pei.EmpCardNo, dpt.DptNameBn, grp.GNameBN,cmp.CompanyLogo, cmp.CompanyNameBangla,cmp.AddressBangla  FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId Inner join HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.PreDsgId= dsg.DsgId left join HRD_Designation as promDsg on pecs.DsgId= promDsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId" + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__Promotion_Letter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=PromotionLetter');", true);
                }
            } //Promotion Letter
            else if (ddlReportType.SelectedValue == "467")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn,CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate, pecs.EffectiveMonth ,promDsg.DsgNameBn as PromotionDsgNameBn, dsg.DsgNameBN, pei.EmpCardNo, dpt.DptNameBn, grp.GNameBN,cmp.CompanyLogo, cmp.CompanyNameBangla,cmp.AddressBangla  FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId Inner join HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.PreDsgId= dsg.DsgId left join HRD_Designation as promDsg on pecs.DsgId= promDsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId" + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__Confirmation_Letter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=ConfirmationLetter');", true);
                }
            }  //Confirmation letter

            else if (ddlReportType.SelectedValue == "462")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn,CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate, pecs.EffectiveMonth ,pecs.EffectiveMonth,CONVERT(VARCHAR(10), pecs.OrderRefDate, 105) AS OrderRefDate, promDsg.DsgNameBn as PromotionDsgNameBn, dsg.DsgNameBN, pei.EmpCardNo, dpt.DptNameBn, pecs.BasicSalary,pecs.HouseRent,pecs.MedicalAllownce,pecs.FoodAllownce,pecs.FoodAllownce,pecs.ConvenceAllownce,pecs.OthersAllownce,pecs.EmpPresentSalary, pecs.PreBasicSalary,pecs.PreHouseRent,pecs.PreMedicalAllownce,pecs.PreFoodAllownce,pecs.PreConvenceAllownce,pecs.PreOthersAllownce,pecs.PreEmpSalary, grp.GNameBN,cmp.CompanyLogo,cmp.CompanyNameBangla,cmp.AddressBangla FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId Inner join HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.PreDsgId = dsg.DsgId left join HRD_Designation as promDsg on pecs.DsgId = promDsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId" + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__Increment_With_Promotion__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=IncrementWithPromotion');", true);
                }
            } //Increment With Promotion Letter
            else if (ddlReportType.SelectedValue == "463")
            {
                Cmd = @"SELECT 
                        pep.EmpId, 
                        pei.EmpCardNo, 
                        pei.EmpNameBn, 
                        pei.EmpName,
                        cmp.CompanyLogo,
                        cmp.CompanyNameBangla,
                        cmp.CompanyName,
                        cmp.Address AS CompanyAddress,
                        cmp.AddressBangla,
                        pea.PreVillageBangla,
                        pea.PreVillage,
                        pea.PrePOBangla,
                        pea.PrePO,
                        preThana.ThaNameBangla AS preThanaBangla,
                        preThana.ThaName AS preThana,
                        preDistrict.DstBangla AS preDistrictBangla,
                        preDistrict.DstName AS preDistrict,
                        pep.FatherNameBn,
                        pep.FatherName,
                        pep.MotherNameBN,
                        pep.MotherName,
                        pep.HusbandOrWifeNameBN,
                        pep.HusbandOrWifeName,
                        CONVERT(VARCHAR(10), pep.DateOfBirth, 105) AS DateOfBirth,
                        pea.PerVillageBangla AS PerVillageBangla,
                        pea.PerVillage,
                        pea.PerPOBangla ,
                        pea.PerPO,
                        perThana.ThaNameBangla AS perThanaBangla,
                        perThana.ThaName AS perThana,
                        perDistrict.DstBangla AS perDistrictBangla,
                        perDistrict.DstName AS perDistrict,
                        CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate,
                        dsg.DsgNameBn,
                        dsg.DsgName,
                        dpt.DptNameBn,
                        dpt.DptName,
                        grp.GNameBn,
                        grp.GName,
                        pep.Sex,
                        pep.Age,
                        pei.CompanyId,
                        pei.EmpStatus,
                        pecs.DptId,
                        pecs.CustomOrdering
                    FROM 
                        dbo.Personnel_EmployeeInfo AS pei 
                    LEFT JOIN 
                        dbo.Personnel_EmpPersonnal AS pep ON pei.EmpId = pep.EmpId 
                    INNER JOIN 
                        dbo.Personnel_EmpCurrentStatus AS pecs ON pei.EmpId = pecs.EmpId AND pecs.IsActive = 1 
                    INNER JOIN 
                        HRD_CompanyInfo AS cmp ON pei.CompanyId = cmp.CompanyId 
                    LEFT OUTER JOIN 
                        dbo.Personnel_EmpAddress AS pea ON pei.EmpId = pea.EmpId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Department AS dpt ON pecs.DptId = dpt.DptId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Designation AS dsg ON pecs.DsgId = dsg.DsgId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Group AS grp ON pecs.GId = grp.GId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Grade AS Grd ON pecs.GrdName = Grd.GrdName 
                    LEFT OUTER JOIN 
                        dbo.HRD_District AS perDistrict ON pea.PreCity = perDistrict.DstId 
                    LEFT OUTER JOIN 
                        dbo.HRDThanaInfo AS perThana ON pea.PerThanaId = perThana.ThaId 
                    LEFT OUTER JOIN 
                        dbo.HRD_District AS preDistrict ON pea.PreCity = preDistrict.DstId 
                LEFT OUTER JOIN 
                    dbo.HRDThanaInfo AS preThana ON pea.PreThanaId = preThana.ThaId" + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__1stAbsentLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=1stAbsentLetter');", true);
                }
            } //1st Absent Letter

            else if (ddlReportType.SelectedValue == "464")
            {
                Cmd = @"SELECT 
                        pep.EmpId, 
                        pei.EmpCardNo, 
                        pei.EmpNameBn, 
                        pei.EmpName,
                        cmp.CompanyLogo,
                        cmp.CompanyNameBangla,
                        cmp.CompanyName,
                        cmp.Address AS CompanyAddress,
                        cmp.AddressBangla,
                        pea.PreVillageBangla,
                        pea.PreVillage,
                        pea.PrePOBangla,
                        pea.PrePO,
                        preThana.ThaNameBangla AS preThanaBangla,
                        preThana.ThaName AS preThana,
                        preDistrict.DstBangla AS preDistrictBangla,
                        preDistrict.DstName AS preDistrict,
                        pep.FatherNameBn,
                        pep.FatherName,
                        pep.MotherNameBN,
                        pep.MotherName,
                        pep.HusbandOrWifeNameBN,
                        pep.HusbandOrWifeName,
                        CONVERT(VARCHAR(10), pep.DateOfBirth, 105) AS DateOfBirth,
                        pea.PerVillageBangla AS PerVillageBangla,
                        pea.PerVillage,
                        pea.PerPOBangla ,
                        pea.PerPO,
                        perThana.ThaNameBangla AS perThanaBangla,
                        perThana.ThaName AS perThana,
                        perDistrict.DstBangla AS perDistrictBangla,
                        perDistrict.DstName AS perDistrict,
                        CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate,
                        dsg.DsgNameBn,
                        dsg.DsgName,
                        dpt.DptNameBn,
                        dpt.DptName,
                        grp.GNameBn,
                        grp.GName,
                        pep.Sex,
                        pep.Age,
                        pei.CompanyId,
                        pei.EmpStatus,
                        pecs.DptId,
                        pecs.CustomOrdering
                    FROM 
                        dbo.Personnel_EmployeeInfo AS pei 
                    LEFT JOIN 
                        dbo.Personnel_EmpPersonnal AS pep ON pei.EmpId = pep.EmpId 
                    INNER JOIN 
                        dbo.Personnel_EmpCurrentStatus AS pecs ON pei.EmpId = pecs.EmpId AND pecs.IsActive = 1 
                    INNER JOIN 
                        HRD_CompanyInfo AS cmp ON pei.CompanyId = cmp.CompanyId 
                    LEFT OUTER JOIN 
                        dbo.Personnel_EmpAddress AS pea ON pei.EmpId = pea.EmpId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Department AS dpt ON pecs.DptId = dpt.DptId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Designation AS dsg ON pecs.DsgId = dsg.DsgId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Group AS grp ON pecs.GId = grp.GId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Grade AS Grd ON pecs.GrdName = Grd.GrdName 
                    LEFT OUTER JOIN 
                        dbo.HRD_District AS perDistrict ON pea.PreCity = perDistrict.DstId 
                    LEFT OUTER JOIN 
                        dbo.HRDThanaInfo AS perThana ON pea.PerThanaId = perThana.ThaId 
                    LEFT OUTER JOIN 
                        dbo.HRD_District AS preDistrict ON pea.PreCity = preDistrict.DstId 
                LEFT OUTER JOIN 
                    dbo.HRDThanaInfo AS preThana ON pea.PreThanaId = preThana.ThaId" + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__2ndAbsentLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=2ndAbsentLetter');", true);
                }
            } //2nd Absent Letter

            else if (ddlReportType.SelectedValue == "465")
            {
                Cmd = @"SELECT 
                        pep.EmpId, 
                        pei.EmpCardNo, 
                        pei.EmpNameBn, 
                        pei.EmpName,
                        cmp.CompanyLogo,
                        cmp.CompanyNameBangla,
                        cmp.CompanyName,
                        cmp.Address AS CompanyAddress,
                        cmp.AddressBangla,
                        pea.PreVillageBangla,
                        pea.PreVillage,
                        pea.PrePOBangla,
                        pea.PrePO,
                        preThana.ThaNameBangla AS preThanaBangla,
                        preThana.ThaName AS preThana,
                        preDistrict.DstBangla AS preDistrictBangla,
                        preDistrict.DstName AS preDistrict,
                        pep.FatherNameBn,
                        pep.FatherName,
                        pep.MotherNameBN,
                        pep.MotherName,
                        pep.HusbandOrWifeNameBN,
                        pep.HusbandOrWifeName,
                        CONVERT(VARCHAR(10), pep.DateOfBirth, 105) AS DateOfBirth,
                        pea.PerVillageBangla AS PerVillageBangla,
                        pea.PerVillage,
                        pea.PerPOBangla ,
                        pea.PerPO,
                        perThana.ThaNameBangla AS perThanaBangla,
                        perThana.ThaName AS perThana,
                        perDistrict.DstBangla AS perDistrictBangla,
                        perDistrict.DstName AS perDistrict,
                        CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate,
                        dsg.DsgNameBn,
                        dsg.DsgName,
                        dpt.DptNameBn,
                        dpt.DptName,
                        grp.GNameBn,
                        grp.GName,
                        pep.Sex,
                        pep.Age,
                        pei.CompanyId,
                        pei.EmpStatus,
                        pecs.DptId,
                        pecs.CustomOrdering
                    FROM 
                        dbo.Personnel_EmployeeInfo AS pei 
                    LEFT JOIN 
                        dbo.Personnel_EmpPersonnal AS pep ON pei.EmpId = pep.EmpId 
                    INNER JOIN 
                        dbo.Personnel_EmpCurrentStatus AS pecs ON pei.EmpId = pecs.EmpId AND pecs.IsActive = 1 
                    INNER JOIN 
                        HRD_CompanyInfo AS cmp ON pei.CompanyId = cmp.CompanyId 
                    LEFT OUTER JOIN 
                        dbo.Personnel_EmpAddress AS pea ON pei.EmpId = pea.EmpId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Department AS dpt ON pecs.DptId = dpt.DptId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Designation AS dsg ON pecs.DsgId = dsg.DsgId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Group AS grp ON pecs.GId = grp.GId 
                    LEFT OUTER JOIN 
                        dbo.HRD_Grade AS Grd ON pecs.GrdName = Grd.GrdName 
                    LEFT OUTER JOIN 
                        dbo.HRD_District AS perDistrict ON pea.PreCity = perDistrict.DstId 
                    LEFT OUTER JOIN 
                        dbo.HRDThanaInfo AS perThana ON pea.PerThanaId = perThana.ThaId 
                    LEFT OUTER JOIN 
                        dbo.HRD_District AS preDistrict ON pea.PreCity = preDistrict.DstId 
                LEFT OUTER JOIN 
                    dbo.HRDThanaInfo AS preThana ON pea.PreThanaId = preThana.ThaId" + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__3rdAbsentLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=3rddAbsentLetter');", true);
                }
            } // 3rd Absent Letter


            else if (ddlReportType.SelectedValue == "461")
            {
                Cmd = "SELECT pep.EmpId,pei.EmpNameBn,CONVERT(VARCHAR(10), pei.EmpJoiningDate, 105) AS EmpJoiningDate, pecs.EffectiveMonth ,pecs.EffectiveMonth,CONVERT(VARCHAR(10), pecs.OrderRefDate, 105) AS OrderRefDate, promDsg.DsgNameBn as PromotionDsgNameBn, dsg.DsgNameBN, pei.EmpCardNo, dpt.DptNameBn, pecs.BasicSalary,pecs.HouseRent,pecs.MedicalAllownce,pecs.FoodAllownce,pecs.FoodAllownce,pecs.ConvenceAllownce,pecs.OthersAllownce,pecs.EmpPresentSalary, pecs.PreBasicSalary,pecs.PreHouseRent,pecs.PreMedicalAllownce,pecs.PreFoodAllownce,pecs.PreConvenceAllownce,pecs.PreOthersAllownce,pecs.PreEmpSalary, grp.GNameBN,cmp.CompanyLogo,cmp.CompanyNameBangla,cmp.AddressBangla FROM Personnel_EmployeeInfo as pei Inner join Personnel_EmpCurrentStatus as pecs on pei.EmpId = pecs.EmpId and pecs.IsActive = 1 LEFT JOIN Personnel_EmpPersonnal as pep  ON pei.EmpId = pep.EmpId Inner join HRD_CompanyInfo as cmp on pei.CompanyId = cmp.CompanyId left join HRD_Department as dpt on pecs.DptId = dpt.DptId left join HRD_Designation as dsg on pecs.PreDsgId = dsg.DsgId left join HRD_Designation as promDsg on pecs.DsgId = promDsg.DsgId left join HRD_Group as grp on pecs.GId = grp.GId" + condition + " order by CustomOrdering";

                if (DataFill(Cmd))
                {
                    Session["__IncrementLetter__"] = dt;
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=IncrementLetter');", true);
                }
            } //Increment Letter

            else
            {
                lblMessage.InnerText = "warning->Please Select Report Type";
            }






        }
        private bool DataFill(string query)
        {
            sqlDB.fillDataTable(query, dt = new DataTable());
            if (dt == null || dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Data not found!";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                return false;
            }
            return true;
        }

        private void gererateContactReportBangla()
        {
            if (ddlCardNo.SelectedValue == "0")
            {

                if (lstSelected.Items.Count == 0)
                {
                    lblMessage.InnerText = "warning-> Please Select Any Department!";
                    lstSelected.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                    return;
                }
                //  string CompanyList = "";
                string ShiftList = "";
                string DepartmentList = "";
                CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;


                DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
                string EmpTypeID = (rblEmpType.SelectedValue.ToString().Equals("All")) ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + "";
                if (ddlReportType.SelectedValue == "0")
                    Cmd = "select CompanyId,CompanyNameBangla CompanyName,AddressBangla Address,DptId,DptNameBn DptName,SftId, SftName,DsgNameBn DsgName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpNameBn EmpName,MobileNo from V_EmpContactInfo " +
                        "where CompanyId='" + CompanyId + "' and SftId " + ShiftList + " and DptId " + DepartmentList + " and EmpStatus in(1,8) " + EmpTypeID + " order by CustomOrdering";
                else
                    Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpName,ContactName,EmergencyAddress, EmergencyPhoneNo from V_EmpContactInfo " +
                    "where CompanyId='" + CompanyId + "' and SftId " + ShiftList + " and DptId " + DepartmentList + " and EmpStatus in(1,8) " + EmpTypeID + " order by CustomOrdering";
            }
            else
            {
                //if (txtCardNo.Text.Length < 4)
                //{
                //    lblMessage.InnerText = "warning-> Please Type Mininmum 4 Character of Card No !";
                //    txtCardNo.Focus(); return;

                //}
                CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
                if (ddlReportType.SelectedValue == "0")
                    Cmd = "select CompanyId,CompanyNameBangla CompanyName,AddressBangla Address,DptId,DptNameBn DptName,SftId, SftName,DsgNameBn DsgName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpNameBn EmpName,MobileNo from V_EmpContactInfo " +
                        "where CompanyId='" + CompanyId + "' and SN=" + ddlCardNo.SelectedValue + " and EmpStatus in(1,8)";
                else
                    Cmd = "select CompanyId,CompanyName,Address,DptId,DptName,SftId,SftName, EmpId, SUBSTRING(EmpCardNo,8,15) as EmpCardNo,EmpName,ContactName,EmergencyAddress, EmergencyPhoneNo from V_EmpContactInfo " +
                    "where CompanyId='" + CompanyId + "'  and SN=" + ddlCardNo.SelectedValue + " and EmpStatus in(1,8)";

            }
            sqlDB.fillDataTable(Cmd, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->Any Records Are Not Founded !";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "loadcardNo();", true);
                return;
            }

            Session["__ContacListBangla__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=ContactListBangla-" + ddlReportType.SelectedValue + "');", true);  //Open New Tab for Sever side code

        }


       
    }
}