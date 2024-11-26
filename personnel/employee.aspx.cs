using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;
using ComplexScriptingSystem;
using System.Globalization;
using System.Reflection;
using CrystalDecisions.Shared;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using SigmaERP.classes;

using IronXL;
using SigmaERP.hrms.BLL;

namespace SigmaERP.personnel
{
    public partial class Employee : System.Web.UI.Page
    {
        string CompanyId = "";
        static string imageName = "";
        static string SignatureImage = "";
        DataTable dt;
        private static Random rnd = new Random();

        //View=270
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int[] pagePermission = { 270 };

                sqlDB.connectionString = Glory.getConnectionString();
                sqlDB.connectDB();
                lblMessage.InnerText = "";
                if (!IsPostBack)
                {
                    txtJoiningDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    txtExpireDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                    int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                    if (!userPagePermition.Any())
                        Response.Redirect(Routing.defualtUrl);

                    ViewState["__IsSave__"] = "";
                    txtProximityNo.Enabled = false;
                    classes.commonTask.LoadEmpType(rblEmpType);
                    ViewState["__LineORGroupDependency__"] =classes.commonTask.GroupORLineDependency();
                    setPrivilege();
                    //classes.commonTask.LoadBranch(ddlBranch);
                    ddlBranch.SelectedValue = ViewState["__CompanyId__"].ToString();
                    try { 
                        string CompanyId = Request.QueryString["CompanyId"].ToString();
                        ddlBranch.SelectedValue = CompanyId;
                    }
                    catch { }
                    LoadCompanyInfo();      
                     //classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue);
                 
                   
                    classes.Employee.LoadEmpStatus(ddlEmpStatus);
                   classes.commonTask.LoadShiftOnlyIndependent(ddlShift, ddlBranch.SelectedValue);
                    if (ViewState["__CardNoType__"].ToString().Equals("True"))
                        loadDepartMentWithCode();
                    //classes.commonTask.SearchDepartmentWithCode(ddlBranch.SelectedValue, ddlDepartment);
                    else
                    {
                        AutoGenerateEmpId();
                        loadDepartMents();
                        //classes.commonTask.SearchDepartment(ddlBranch.SelectedValue, ddlDepartment); 
                    }
                    classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue, ddlBranch.SelectedValue, txtEmpCardNo.Text.Trim());
                    FlatCustomOrdering();        
                    classes.commonTask.LoadDesignation(ddlDepartment.SelectedValue.ToString(), ddlDesingnation);
                    if (ViewState["__LineORGroupDependency__"].ToString().Equals("False"))
                        classes.commonTask.LoadGrouping(ddlGrouping, ViewState["__CompanyId__"].ToString());
                    //-----------------For Employee Personel---------------------
                    classes.commonTask.LoadRligion(dsReligion);
                    classes.commonTask.LoadEducationalQui(ddlLastEdQualification);
                    // ..............................................
                    //LoadAllownceSetting();
                    Session["_EditStatus_"] = "Save";
                    string EmpId = Request.QueryString["EmpId"];
                    Session["_EmpId_"] = EmpId;
                    if (EmpId != null)
                    {
                        Session["_EditStatus_"] = "Update";
                        btnSave.Text = "Update";
                        btnSave.Enabled = true;
                        btnSave.CssClass = "css_btn Ptbut";
                        ViewState["__CheckEmpCardNo__"] = "True";
                        LoadEmployeeCardNowiseData(EmpId, "1");
                        Transfer();
                        LoadEmpPersonalInfo(EmpId);// For Personal
                        //rblusertype.Enabled = false;
                    }
                    else
                    {
                        ViewState["__CheckEmpCardNo__"] = "False";
                    }
                    //txtEmpCardNo.Enabled = false;
                    if (!classes.commonTask.HasBranch())
                        ddlBranch.Enabled = false;
                }

            }
            catch { }
        }
        private void Transfer()
        {
            try
            {
                if (Request.QueryString["Transfer"].Equals("True"))
                {
                    ViewState["IsTranster"] = "True";
                    ddlDepartment.Enabled = true;
                    chkAlternativeEmpCard.Enabled = false;
                    txtName.Enabled = false;
                    txtNickName.Enabled = false;
                    ddlEmpStatus.Enabled = false;
                    txtEmpCardNo.Enabled = false;
                    txtNameBangla.Enabled = false;
                    rblPunchType.Enabled = false;
                    cskDptWise.Enabled = false;
                    cskFlatOrder.Enabled = false;
                    txtDptWise.Enabled = false;
                    txtFlatOrder.Enabled = false;
                    dsPlaceOfBirth.Enabled = false;
                    dsHeight.Enabled = false;
                    dsWeight.Enabled = false;
                    dsNoOfExperience.Enabled = false;
                    rblEmpType.Enabled = false;
                    rblSalaryType.Enabled = false;
                //    ddlShift.Enabled = false;
                    txtJoiningDate.Enabled = false;
                    ddlType.Enabled = false;
                   
                    ddlLastEdQualification.Enabled = false;
                    dsFatherName.Enabled = false;
                    dsMotherName.Enabled = false;
                    dsNationality.Enabled = false;
                    dsMotherName.Enabled = false;
                    dsReligion.Enabled = false;
                    dsMaritialStatus.Enabled = false;
                    dsSex.Enabled = false;
                    dsNationIDCardNo.Enabled = false;
                    dsBloodGroup.Enabled = false;
                    dsDateOfBirth.Enabled = false;
                    rblDutyType.Enabled = false;
                    rblWeekendType.Enabled = false;
                   
                }
            }
            catch { ViewState["IsTranster"] = ""; }
        }
        private void setPrivilege()
        {
            try
            {                
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__dptID__"] = getCookies["__DptId__"].ToString();
                classes.commonTask.LoadBranch(ddlBranch, ViewState["__CompanyId__"].ToString());

                string[] AccessPermission = new string[0];
               // AccessPermission = checkUserPrivilege.checkUserPrivilegeForSettigs(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "Employee.aspx", ddlBranch, btnSave);
                ViewState["__ReadAction__"] = AccessPermission[0];
                ViewState["__WriteAction__"] = AccessPermission[1];
                ViewState["__UpdateAction__"] = AccessPermission[2];
                ViewState["__DeletAction__"] = AccessPermission[3];
                //SetUserType();
               
            }
            catch { }

        }
        /* private void SetUserType()
        {
            try
            {

                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin"))
                {
                    for (int i = 0; i < rblusertype.Items.Count; i++)
                    {
                        if (rblusertype.Items[i].Text == "Master Admin")
                            rblusertype.Items.RemoveAt(i);
                    }
                }
                else if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin"))
                {
                    for (int i = 0; i < rblusertype.Items.Count; i++)
                    {
                        if (rblusertype.Items[i].Text == "Master Admin" || rblusertype.Items[i].Text == "Super Admin")
                            rblusertype.Items.RemoveAt(i);
                    }
                }
            }
            catch { }
        }
       */
        private void LoadCompanyInfo()
        {
            DataTable dtcom = new DataTable();
            sqlDB.fillDataTable("Select CompanyName,StartCardNo,ShortName,CardNoType,FlatCode,CardNoDigits From HRD_CompanyInfo where CompanyId='" + ddlBranch.SelectedValue + "'", dtcom);
            ViewState["__CardNoType__"] = dtcom.Rows[0]["CardNoType"].ToString();
            ViewState["__FlatCode__"] = dtcom.Rows[0]["FlatCode"].ToString();
            ViewState["__ShortName__"] = dtcom.Rows[0]["ShortName"].ToString();
            ViewState["__StartCardNo__"] = dtcom.Rows[0]["StartCardNo"].ToString();
            ViewState["__CardNoDigits__"] = dtcom.Rows[0]["CardNoDigits"].ToString();
            hdfCardnoDigitsSet.Value= hdfCardnoDigits.Value = dtcom.Rows[0]["CardNoDigits"].ToString();
            txtAlternativeCard.Style.Add("MaxLength",ViewState["__CardNoDigits__"].ToString().Length.ToString());
            if (ViewState["__CardNoType__"].ToString().Equals("True"))
            { 
                cskDptWise.Checked = true;
                cskFlatOrder.Checked = false;
                cskDptWise.Visible = true;
                txtDptWise.Visible = true;
                cskFlatOrder.Visible = true;
                txtFlatOrder.Style.Add("Width", "39%");
                
            }
            else 
            { 
                cskDptWise.Checked = false; 
                cskFlatOrder.Checked = true;
                cskFlatOrder.Visible = false;
                txtDptWise.Visible = false;
                cskDptWise.Visible = false;
                txtFlatOrder.Style.Add("Width","98%");
            }
        }
        private void AutoGenerateEmpId()
        {
            try
            {
                 
               
                DataTable dt = new DataTable();
                int CodeLeanth =ViewState["__FlatCode__"].ToString().Length;
                int startIndex = 8 + CodeLeanth;
                string cmd = "select max(convert(int,substring(EmpCardNo," + startIndex + ",15) ))as MaxCardNo,substring(EmpCardNo," + startIndex + ",15) as EmpCardNo from v_Personnel_EmpCurrentStatus  " +
                  "where CompanyId='" + ddlBranch.SelectedValue + "'  group by substring(EmpCardNo," + startIndex + ",15) having max(convert(int,substring(EmpCardNo," + startIndex + ",15) ))=(select max(convert(int,substring(EmpCardNo," + startIndex + ",15) )) from v_Personnel_EmpCurrentStatus  " +
                  "where CompanyId='" + ddlBranch.SelectedValue + "')";
                sqlDB.fillDataTable(cmd, dt);
                //sqlDB.fillDataTable("select max(convert(int,substring(EmpCardNo," + startIndex + ",15) ))as MaxCardNo from v_Personnel_EmpCurrentStatus   where CompanyId='" + ddlBranch.SelectedValue + "' ", dt);
                if (dt.Rows.Count < 1 || dt.Rows[0]["MaxCardNo"].ToString() == "")
                {
                    ViewState["__txtEmpCardNo__"] =txtEmpCardNo.Text = ViewState["__ShortName__"].ToString() + DateTime.Now.Year + ViewState["__FlatCode__"].ToString() + ViewState["__StartCardNo__"].ToString();
                    ViewState["__txtRegistrationId__"] = txtRegistrationId.Text = ViewState["__FlatCode__"].ToString() + ViewState["__StartCardNo__"].ToString();
                }
                else
                {
                    string NewCardNo = "";
                    string MaxCardNo = dt.Rows[0]["EmpCardNo"].ToString();
                    if (MaxCardNo.ToString().Length >int.Parse( hdfCardnoDigits.Value))
                    hdfCardnoDigits.Value = MaxCardNo.ToString().Length.ToString();
                    NewCardNo = (int.Parse(MaxCardNo) + 1).ToString();
                    if (hdfCardnoDigits.Value.ToString() == "3")
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "0" + NewCardNo;                        
                    }
                    else if (hdfCardnoDigits.Value.ToString() == "4")
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "000" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 3) NewCardNo = "0" + NewCardNo;                       
                    }
                    else if (hdfCardnoDigits.Value.ToString() == "5")
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "0000" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "000" + NewCardNo;
                        else if (NewCardNo.Length == 3) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 4) NewCardNo = "0" + NewCardNo;                        
                    }
                    else if (hdfCardnoDigits.Value.ToString() == "6")
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "00000" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "0000" + NewCardNo;
                        else if (NewCardNo.Length == 3) NewCardNo = "000" + NewCardNo;
                        else if (NewCardNo.Length == 4) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 5) NewCardNo = "0" + NewCardNo;                        
                    }
                    ViewState["__txtEmpCardNo__"] =   txtEmpCardNo.Text = ViewState["__ShortName__"].ToString() + DateTime.Now.Year + ViewState["__FlatCode__"].ToString() + NewCardNo;
                    ViewState["__txtRegistrationId__"] =   txtRegistrationId.Text = ViewState["__FlatCode__"].ToString() + NewCardNo;
                }
            }
            catch { }
        }
       
        private void AutoGenerateEmpIdByDepartment()
        {
            try
            {


                string DptCode = ViewState["__DptCode__"].ToString();
                int CodeLeanth = DptCode.ToString().Length;
                int startIndex = 8 + CodeLeanth;
                DataTable dt = new DataTable();
                string cmd = "select max(convert(int,substring(EmpCardNo," + startIndex + ",15) ))as MaxCardNo,substring(EmpCardNo," + startIndex + ",15) as EmpCardNo from v_Personnel_EmpCurrentStatus  " +
                    "where CompanyId='" + ddlBranch.SelectedValue + "' and DptId='" + ViewState["__DptId__"].ToString() + "' group by substring(EmpCardNo," + startIndex + ",15) having max(convert(int,substring(EmpCardNo," + startIndex + ",15) ))=(select max(convert(int,substring(EmpCardNo," + startIndex + ",15) )) from v_Personnel_EmpCurrentStatus  " +
                    "where CompanyId='" + ddlBranch.SelectedValue + "' and DptId='" + ViewState["__DptId__"].ToString() + "')";
                sqlDB.fillDataTable(cmd, dt);
               // sqlDB.fillDataTable("select max(convert(int,substring(EmpCardNo," + startIndex + ",15) ))as MaxCardNo from v_Personnel_EmpCurrentStatus  where CompanyId='" + ddlBranch.SelectedValue + "' and DptId='" + ViewState["__DptId__"].ToString() + "' ", dt);
                if (dt.Rows.Count<1 || dt.Rows[0]["MaxCardNo"].ToString() == "")
                {
                    txtEmpCardNo.Text = ViewState["__ShortName__"].ToString() + DateTime.Now.Year + DptCode + ViewState["__StartCardNo__"].ToString();
                }
                else
                {
                    string NewCardNo = "";
                    string MaxCardNo = dt.Rows[0]["EmpCardNo"].ToString();
                    if (MaxCardNo.ToString().Length > int.Parse(hdfCardnoDigits.Value))
                        hdfCardnoDigits.Value = MaxCardNo.ToString().Length.ToString();
                    NewCardNo = (int.Parse(MaxCardNo) + 1).ToString();
                    if (hdfCardnoDigits.Value.ToString() == "3")
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "0" + NewCardNo;                        
                    }
                   else if (hdfCardnoDigits.Value.ToString() == "4") 
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "000" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 3) NewCardNo = "0" + NewCardNo;                           
                    }
                    else if (hdfCardnoDigits.Value.ToString() == "5")
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "0000" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "000" + NewCardNo;
                        else if (NewCardNo.Length == 3) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 4) NewCardNo = "0" + NewCardNo;                       
                    }
                    else if (hdfCardnoDigits.Value.ToString() == "6")
                    {
                        if (NewCardNo.Length == 1) NewCardNo = "00000" + NewCardNo;
                        else if (NewCardNo.Length == 2) NewCardNo = "0000" + NewCardNo;
                        else if (NewCardNo.Length == 3) NewCardNo = "000" + NewCardNo;
                        else if (NewCardNo.Length == 4) NewCardNo = "00" + NewCardNo;
                        else if (NewCardNo.Length == 5) NewCardNo = "0" + NewCardNo;                       
                    }
                    txtEmpCardNo.Text = ViewState["__ShortName__"].ToString() + DateTime.Now.Year + DptCode + NewCardNo;
                    txtRegistrationId.Text = DptCode + NewCardNo;
                }
                ddlEmpCardNo.Items.RemoveAt(0);
                ddlEmpCardNo.Items.Insert(0, new ListItem(txtEmpCardNo.Text, "0"));              
            }
            catch { }
        }
        /*
        private void AutoGenerateWorkerID()
        {
            try
            {
                string getsortcom="";
                DataTable dtcom = new DataTable();
                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                    sqlDB.fillDataTable("Select CompanyName,StartCardNo,ShortName From HRD_CompanyInfo where CompanyId=" + ddlBranch.SelectedValue + "", dtcom);
                else sqlDB.fillDataTable("Select CompanyName,StartCardNo,ShortName From HRD_CompanyInfo where CompanyId=" + ViewState["__CompanyId__"] + "", dtcom);
                getsortcom = dtcom.Rows[0]["ShortName"].ToString();
                ViewState["__ShortName__"] = getsortcom;
                DataTable dt = new DataTable();
                if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                    sqlDB.fillDataTable("Select MAX(WorkerCardNo) as WorkerCardNo From Personnel_WorkerCardNoSequence  where CompanyId='"+ddlBranch.SelectedValue+ "' ", dt);
                else
                sqlDB.fillDataTable("Select MAX(WorkerCardNo) as WorkerCardNo From Personnel_WorkerCardNoSequence  where CompanyId='" + ViewState["__CompanyId__"] + "' ", dt);
                if (dt.Rows[0]["WorkerCardNo"].ToString() == "")
                {
                    txtEmpCardNo.Text = getsortcom + DateTime.Now.Year + dtcom.Rows[0]["StartCardNo"].ToString();
                }
                else
                {
                    string NewCardNo = "";
                    string WorkerCardNo = dt.Rows[0]["WorkerCardNo"].ToString();
                    NewCardNo =  (int.Parse(WorkerCardNo) + 1).ToString();
                    txtEmpCardNo.Text =getsortcom+DateTime.Now.Year+NewCardNo;
                }
            }
            catch { }
        }
         */


        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
           
            if (ViewState["__CardNoType__"].ToString().Equals("True"))
            {
                string[] dptID = ddlDepartment.SelectedValue.Split('_');
                ViewState["__DptId__"] = dptID[0].ToString();
                ViewState["__DptCode__"] = dptID[1].ToString();
                if (ViewState["IsTranster"]==null || ViewState["IsTranster"].ToString().Equals(""))
                AutoGenerateEmpIdByDepartment();
                classes.commonTask.LoadDesignation(ViewState["__DptId__"].ToString(), ddlDesingnation);
               classes.commonTask.LoadInitialShiftByDepartment(ddlShift, ddlBranch.SelectedValue, ViewState["__DptId__"].ToString());
               // classes.commonTask.LoadShiftByDepartment(ddlShift, ddlBranch.SelectedValue, ViewState["__DptId__"].ToString());
                DepartmentCustomOrdering();
            }
            else
            {
                ViewState["__DptId__"] = ddlDepartment.SelectedValue;
                classes.commonTask.LoadDesignation(ddlDepartment.SelectedValue, ddlDesingnation);
                classes.commonTask.LoadInitialShiftByDepartment(ddlShift, ddlBranch.SelectedValue, ddlDepartment.SelectedValue);
                //classes.commonTask.LoadShiftByDepartment(ddlShift, ddlBranch.SelectedValue, ddlDepartment.SelectedValue);
            }
            if (ViewState["__LineORGroupDependency__"].ToString().Equals("True"))
            {
                CompanyId = (ddlBranch.SelectedValue.ToString().Equals("0000")) ? ViewState["__CompanyId__"].ToString() : ddlBranch.SelectedValue;
                classes.commonTask.LoadGrouping(ddlGrouping, CompanyId, ViewState["__DptId__"].ToString());
            }
                
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                if (Session["_EditStatus_"] == null)
                {
                    Session["_EditStatus_"] = "Save";
                }
                if (Session["_EditStatus_"].ToString() == "Save")
                {
                    if (SaveCardValidation() == false) return;
                    //if (SaveValidation() == false) return;                    
                    if (saveEmployeeInfo() == true && saveEmpPersonnal() == true)
                    {
                        if(txtMobileNo.Text.Trim().Length>0)
                            saveEmpAddress();//only mobile no
                        AllClear();
                        classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue, ddlBranch.SelectedValue, txtEmpCardNo.Text.Trim());
                        ddlEmpCardNo.SelectedValue = "0";
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeeAddress.aspx');", true);
                        //if (ViewState["__Username__"]!=null)
                        //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeePersonal.aspx?UserName=" + ViewState["__Username__"].ToString() + "&Password=" + ViewState["__Password__"] .ToString()+ "');", true);  //Open New Tab for Sever side code
                        //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeePersonal.aspx');", true);  //Open New Tab for Sever side code
                    }
                }
                else
                {
                    
                    string EmpId = Request.QueryString["EmpId"];
                    
                        if (UpdateCardValidation() == false) return;                        
                        if (UpdateEmployeeInfo(ddlEmpCardNo.SelectedValue) == true)
                        {
                        if (updateEmpPersonnal(ddlEmpCardNo.SelectedValue) == true)
                        {
                            if (txtMobileNo.Text.Trim().Length > 0)
                                updateEmpAddress(EmpId);
                            AllClear();
                            if (EmpId == null)
                                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UpdateSuccess();", true);
                            else
                            {
                                Session["IsRedirect"] = "Yes";
                                Response.Redirect("~/personnel/employee_list.aspx");
                            }
                        }
                     }
                        
                        else ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UnableUpdate();", true);
                   
                }
            }
            catch { }
        }
        private string LoadEmpId()
        {
            try
            {
                string EMPID = "";
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SL) as SL From Personnel_EmployeeInfo", dt);
                if (dt.Rows[0]["SL"].ToString() == "")
                {
                    EMPID = "00000001";
                }
                else
                {
                    DataTable dtEMPID = new DataTable();
                    sqlDB.fillDataTable("Select EmpId From Personnel_EmployeeInfo where SL=" + dt.Rows[0]["SL"].ToString() + "", dtEMPID);

                    string ID = int.Parse(dtEMPID.Rows[0]["EmpId"].ToString()).ToString();
                    if (ID.Length == 1) EMPID = "0000000" + (int.Parse(ID) + 1);
                    else if (ID.Length == 2) EMPID = "000000" + (int.Parse(ID) + 1);
                    else if (ID.Length == 3) EMPID = "00000" + (int.Parse(ID) + 1);
                    else if (ID.Length == 4) EMPID = "0000" + (int.Parse(ID) + 1);
                    else if (ID.Length == 5) EMPID = "000" + (int.Parse(ID) + 1);
                    else if (ID.Length == 6) EMPID = "00" + (int.Parse(ID) + 1);
                    else if (ID.Length == 7) EMPID = "0" + (int.Parse(ID) + 1);
                    else if (ID.Length == 8) EMPID = (int.Parse(ID) + 1).ToString();

                }
                return EMPID;
            }
            catch { return ""; }
        }
        private Boolean saveEmployeeInfo()
        {
            try
            {
                System.Data.SqlTypes.SqlDateTime getDate;
                getDate = SqlDateTime.Null;
                string EmpId = LoadEmpId();
                ViewState["__EmpId__"] = EmpId;
                // string CompanyId = "0001";
                SqlCommand cmd = new SqlCommand("saveEmployeeInfo", sqlDB.connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", ViewState["__EmpId__"].ToString());
                //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                // {
                //     CompanyId=ddlBranch.SelectedValue;
                //     cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                // }
                // else
                // {
                //     CompanyId=ViewState["__CompanyId__"].ToString();
                //     cmd.Parameters.AddWithValue("@CompanyId", CompanyId);
                // }
                cmd.Parameters.AddWithValue("@CompanyId", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpTypeId", rblEmpType.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpName", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@NickName", txtNickName.Text.Trim());
                cmd.Parameters.AddWithValue("@EmpNameBn", txtNameBangla.Text.Trim());
                if (chkAlternativeEmpCard.Checked)
                {
                    if (ViewState["__CardNoType__"].ToString().Equals("True"))
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + ViewState["__DptCode__"].ToString() + txtAlternativeCard.Text);
                        //cmd.Parameters.AddWithValue("@EmpProximityNo", txtRegistrationId.Text.Trim());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + ViewState["__FlatCode__"].ToString() + txtAlternativeCard.Text);
                        //cmd.Parameters.AddWithValue("@EmpProximityNo", txtRegistrationId.Text.Trim());
                    }
                    
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EmpCardNo", txtEmpCardNo.Text.Trim());
                    string cardNo = new String(txtEmpCardNo.Text.Where(Char.IsNumber).ToArray());
                    //int ProximityNo = int.Parse(cardNo.Substring(DateTime.Now.Year.ToString().Length, cardNo.Length - DateTime.Now.Year.ToString().Length));
                    //cmd.Parameters.AddWithValue("@EmpProximityNo", ProximityNo);
                }
                cmd.Parameters.AddWithValue("@EmpProximityNo", txtRegistrationId.Text.Trim());
                cmd.Parameters.AddWithValue("@PunchType", rblPunchType.SelectedValue);
                cmd.Parameters.AddWithValue("@RealProximityNo", txtProximityNo.Text.Trim());
                cmd.Parameters.AddWithValue("@EmpStatus", ddlEmpStatus.SelectedValue);
             
          
            
              
                cmd.Parameters.AddWithValue("@SftId", ddlShift.SelectedValue);
               
                cmd.Parameters.AddWithValue("@EmpJoiningDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
               cmd.Parameters.AddWithValue("@ShiftTransferDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
              
                

                
               
                cmd.Parameters.AddWithValue("@EarnedLeave", txtEarnedLeave.Text);
                if (txtElStart.Text.Length == 0)
                {
                    cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", getDate);
                }
                else cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", convertDateTime.getCertainCulture(txtElStart.Text.Trim()));
              




                if (HiddenField1.Value.ToString().Length == 0)
                {
                    cmd.Parameters.AddWithValue("@EmpPicture", "");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EmpPicture", txtEmpCardNo.Text.Trim() + HiddenField1.Value.ToString());
                }
                if (FileUpload2.HasFile == true)
                {
                    cmd.Parameters.AddWithValue("@SignatureImage", txtEmpCardNo.Text + FileUpload2.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SignatureImage", "");
                }
             
                cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Text);


                cmd.Parameters.AddWithValue("@ExpireDate", convertDateTime.getCertainCulture(txtExpireDate.Text.Trim()));

                cmd.Parameters.AddWithValue("@PreCompanyId", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@PreEmpTypeId", rblEmpType.SelectedValue);


                if (ViewState["__CardNoType__"].ToString().Equals("True"))
                {

                    cmd.Parameters.AddWithValue("@PreDptId", ViewState["__DptId__"].ToString());
                    cmd.Parameters.AddWithValue("@DptId", ViewState["__DptId__"].ToString());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PreDptId", ddlDepartment.SelectedValue);
                    cmd.Parameters.AddWithValue("@DptId", ddlDepartment.SelectedValue);
                }

                cmd.Parameters.AddWithValue("@PreDsgId", ddlDesingnation.SelectedValue);
                cmd.Parameters.AddWithValue("@DsgId", ddlDesingnation.SelectedValue);
                cmd.Parameters.AddWithValue("@PreGId", ddlGrouping.SelectedValue);
                cmd.Parameters.AddWithValue("@GId", ddlGrouping.SelectedValue);
                cmd.Parameters.AddWithValue("@PreEmpStatus", ddlEmpStatus.SelectedValue);
                
               
                cmd.Parameters.AddWithValue("@DateofUpdate", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@TypeOfChange", "s");
                cmd.Parameters.AddWithValue("@EffectiveMonth", "");
                cmd.Parameters.AddWithValue("@OrderRefNo", "");
                cmd.Parameters.AddWithValue("@OrderRefDate", getDate);
                cmd.Parameters.AddWithValue("@Remarks", "");
                cmd.Parameters.AddWithValue("@ActiveSalary", 1);
                cmd.Parameters.AddWithValue("@EarnLeaveDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
                cmd.Parameters.AddWithValue("@IsActive", 1);
                if(cskFlatOrder.Checked==true)
                cmd.Parameters.AddWithValue("@CustomOrdering", txtFlatOrder.Text.Trim());
                else
                    cmd.Parameters.AddWithValue("@CustomOrdering", txtDptWise.Text.Trim());

                cmd.Parameters.AddWithValue("@TIN", txtTIN.Text.Trim());
                cmd.Parameters.AddWithValue("@PreSalaryType", rblSalaryType.SelectedValue);
                cmd.Parameters.AddWithValue("@SalaryType", rblSalaryType.SelectedValue);

                cmd.Parameters.AddWithValue("@PreEmpDutyType",rblDutyType.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpDutyType", rblDutyType.SelectedValue);
                cmd.Parameters.AddWithValue("@AuthorizedPerson", ckbAuthorized.Checked);                
                cmd.Parameters.AddWithValue("@WeekendType", rblWeekendType.SelectedValue);

                int result = (int)cmd.ExecuteScalar();

                if (result > 0)
                {
                    DataTable dtMaxEmpId = new DataTable();
                    sqlDB.fillDataTable("SELECT Max(EmpId) as EmpId From Personnel_EmployeeInfo", dtMaxEmpId);
                    //  saveShiftTransferDetails(dtMaxEmpId.Rows[0]["EmpId"].ToString());
                    try
                    {
                        if (chkAlternativeEmpCard.Checked)
                        {
                            SqlCommand insertcmd = new SqlCommand("insert into Personnel_WorkerCardNoSequence values(" + int.Parse(txtAlternativeCard.Text) + ",'" + ddlBranch.SelectedValue + "'," + DateTime.Now.Year + ")", sqlDB.connection);
                            insertcmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand insertcmd = new SqlCommand("insert into Personnel_WorkerCardNoSequence values(" + int.Parse(txtEmpCardNo.Text.Substring(Math.Max(0, txtEmpCardNo.Text.Length - 4))) + ",'" + ddlBranch.SelectedValue + "'," + DateTime.Now.Year + ")", sqlDB.connection);
                            insertcmd.ExecuteNonQuery();
                        }

                    }
                    catch { }
                    //if(rblEmpType.SelectedValue=="2")
                    //    SaveUserAccount(EmpId, CompanyId);

                    Session["_EditStatus_"] = "Save";
                    DataTable dtEmpId = new DataTable();
                    sqlDB.fillDataTable("Select MAX(EmpId) as EmpId From Personnel_EmployeeInfo", dtEmpId);
                    if (dtEmpId.Rows.Count > 0)
                    {
                        Session["_EmpId_"] = dtEmpId.Rows[0]["EmpId"].ToString();
                        Session["_EmpStatus_"] = dtEmpId.Rows[0]["EmpId"].ToString();
                    }
                    if (HiddenField1.Value.ToString().Length != 0)
                    {
                        saveImg();
                    }
                    if (FileUpload2.HasFile == true)
                    {
                        SaveSignature();
                    }
                    SaveAttachFile();
                    //rblEmpType.SelectedValue = "2"; // Replace Selected index to Selected Value.
                    //fs.Visible = true; 
                    //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "SaveSuccess();", true);   
                    //ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeePersonal.aspx');", true); // For Test

                }
                else ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UnableSave();", true);

                return true;

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
                return false;
            }
        }
        private Boolean ImportEmployeeInfo()
        {
            try
            {
                System.Data.SqlTypes.SqlDateTime getDate;
                getDate = SqlDateTime.Null;
                string EmpId = LoadEmpId();                
                SqlCommand cmd = new SqlCommand("saveEmployeeInfo", sqlDB.connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpId", EmpId);                
                cmd.Parameters.AddWithValue("@CompanyId", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpTypeId", ViewState["__EmpTypeID__"].ToString());
                cmd.Parameters.AddWithValue("@EmpName", ViewState["__Name__"].ToString());
                cmd.Parameters.AddWithValue("@NickName", ViewState["__Name__"].ToString());
                cmd.Parameters.AddWithValue("@EmpNameBn", txtNameBangla.Text.Trim());
                cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__txtEmpCardNo__"].ToString());
                cmd.Parameters.AddWithValue("@EmpProximityNo", ViewState["__RegID__"].ToString());                
                cmd.Parameters.AddWithValue("@PunchType", rblPunchType.SelectedValue);
                cmd.Parameters.AddWithValue("@RealProximityNo", ViewState["__RegID__"].ToString());
                cmd.Parameters.AddWithValue("@EmpStatus", ddlEmpStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@SftId", "0");
                cmd.Parameters.AddWithValue("@EmpJoiningDate", ViewState["__JoiningDate__"].ToString());
                cmd.Parameters.AddWithValue("@ShiftTransferDate", ViewState["__JoiningDate__"].ToString());
                cmd.Parameters.AddWithValue("@EarnedLeave", txtEarnedLeave.Text);
                if (txtElStart.Text.Length == 0)
                {
                    cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", getDate);
                }
                else cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", convertDateTime.getCertainCulture(txtElStart.Text.Trim()));
                if (HiddenField1.Value.ToString().Length == 0)
                {
                    cmd.Parameters.AddWithValue("@EmpPicture", "");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EmpPicture", txtEmpCardNo.Text.Trim() + HiddenField1.Value.ToString());
                }
                if (FileUpload2.HasFile == true)
                {
                    cmd.Parameters.AddWithValue("@SignatureImage", txtEmpCardNo.Text + FileUpload2.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SignatureImage", "");
                }

                cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Text);


                cmd.Parameters.AddWithValue("@ExpireDate", getDate);

                cmd.Parameters.AddWithValue("@PreCompanyId", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@PreEmpTypeId", rblEmpType.SelectedValue);


               
                    cmd.Parameters.AddWithValue("@PreDptId", ViewState["__DptID__"].ToString());
                    cmd.Parameters.AddWithValue("@DptId", ViewState["__DptID__"].ToString());
                

                cmd.Parameters.AddWithValue("@PreDsgId", ViewState["__DsgID__"].ToString());
                cmd.Parameters.AddWithValue("@DsgId", ViewState["__DsgID__"].ToString());
                cmd.Parameters.AddWithValue("@PreGId", ViewState["__GID__"].ToString());
                cmd.Parameters.AddWithValue("@GId", ViewState["__GID__"].ToString());
                cmd.Parameters.AddWithValue("@PreEmpStatus", ddlEmpStatus.SelectedValue);


                cmd.Parameters.AddWithValue("@DateofUpdate", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@TypeOfChange", "s");
                cmd.Parameters.AddWithValue("@EffectiveMonth", "");
                cmd.Parameters.AddWithValue("@OrderRefNo", "");
                cmd.Parameters.AddWithValue("@OrderRefDate", getDate);
                cmd.Parameters.AddWithValue("@Remarks", "");
                cmd.Parameters.AddWithValue("@ActiveSalary", 1);
                cmd.Parameters.AddWithValue("@EarnLeaveDate", ViewState["__JoiningDate__"].ToString());
                cmd.Parameters.AddWithValue("@IsActive", 1);
                if (cskFlatOrder.Checked == true)
                    cmd.Parameters.AddWithValue("@CustomOrdering", txtFlatOrder.Text.Trim());
                else
                    cmd.Parameters.AddWithValue("@CustomOrdering", txtDptWise.Text.Trim());

                cmd.Parameters.AddWithValue("@TIN", txtTIN.Text.Trim());
                cmd.Parameters.AddWithValue("@PreSalaryType", rblSalaryType.SelectedValue);
                cmd.Parameters.AddWithValue("@SalaryType", rblSalaryType.SelectedValue);

                cmd.Parameters.AddWithValue("@PreEmpDutyType", rblDutyType.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpDutyType", rblDutyType.SelectedValue);
                cmd.Parameters.AddWithValue("@AuthorizedPerson", ckbAuthorized.Checked);
                cmd.Parameters.AddWithValue("@WeekendType", rblWeekendType.SelectedValue);

                int result = (int)cmd.ExecuteScalar();

                if (result > 0)
                {
                    CRUD.Execute("Insert into  Personnel_EmpPersonnal (EmpId,Sex) values ('"+ EmpId + "','"+ ViewState["__Gender__"] .ToString()+ "') ");

                }
                else ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UnableSave();", true);

                return true;

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
                return false;
            }
        }
        private void SaveUserAccount(string EmpId, string CompanyId)
        {
            try
            {


                string[] Name = txtName.Text.Split(' ');
                string FirstName = "", LastName = "";
                if (Name.Length == 1) FirstName = Name[0];
                else if (Name.Length == 2) { FirstName = Name[0]; LastName = Name[1]; }
                else if (Name.Length > 2) { for (int i = 0; i < Name.Length; i++) { if (Name.Length != i + 1) FirstName += Name[i]; else LastName = Name[i]; } }
                StringBuilder password = new StringBuilder();
                for (int i = 1; i <= 2; i++)
                {
                    char capitalLeter = GenerateChar(txtNickName.Text.ToUpper());
                    InsertAtRandomPositons(password, capitalLeter);
                }
                for (int i = 1; i <= 2; i++)
                {
                    char smallLetter = GenerateChar(txtNickName.Text.ToLower());
                    InsertAtRandomPositons(password, smallLetter);
                }
                char digit = GenerateChar(EmpId);
                InsertAtRandomPositons(password, digit);
                string pass = password.ToString();
                string username = "";
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("Select NickName From Personnel_EmployeeInfo where CompanyId='" + CompanyId + "' and NickName='" + txtNickName.Text + "'", dt);
                if (dt.Rows.Count == 1)
                    username = txtNickName.Text;
                else
                    username = txtNickName.Text + (dt.Rows.Count - 1);
                ViewState["__Username__"] = username;
                ViewState["__Password__"] = pass;
                string[] getColumns = { "FirstName", "LastName", "UserName", "UserPassword", "Email", "CreatedOn", "CreatedBy", "Status", "CoockieInfo", "CompanyId", "EmpId" };
                string[] getValues = { FirstName, LastName, ComplexLetters.getTangledLetters(username), ComplexLetters.getTangledLetters(pass), "", DateTime.Now.ToString("yyyy-MM-dd"), "1", "1", "", CompanyId, EmpId };
                if (SQLOperation.forSaveValue("UserAccount", getColumns, getValues, sqlDB.connection) == true)
                {

                }
            }
            catch { }
        }
        private static void InsertAtRandomPositons(StringBuilder password, char character)
        {
            int randomPosition = rnd.Next(password.Length + 1);
            password.Insert(randomPosition, character);
        }
        private static char GenerateChar(string availableChars)
        {
            int randomIndex = rnd.Next(availableChars.Length);
            char randomChar = availableChars[randomIndex];
            return randomChar;
        }
        private void saveImg()
        {
            try
            {
                string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);

                System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                int width = 100;
                int height = 100;
                using (System.Drawing.Image thumbnail = image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        thumbnail.Save(Server.MapPath("/EmployeeImages/Images/" + txtEmpCardNo.Text.Trim() + filename), System.Drawing.Imaging.ImageFormat.Png);
                    }
                }


                // //Get Filename from fileupload control
                // string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
                // //Save images into Images folder
                // FileUpload1.SaveAs(Server.MapPath("/EmployeeImages/Images/" + txtEmpCardNo.Text.Trim() + filename));
                //// FileUpload1.SaveAs(Server.MapPath("/EmployeeImages/Images/" + txtEmpCardNo.Text.Trim() + filename));


            }
            catch { }
        }
        public bool ThumbnailCallback()
        {
            return false;
        }
        private void SaveSignature()
        {
            try
            {
                //Get Filename from fileupload control
                string filename = Path.GetFileName(FileUpload2.PostedFile.FileName);
                //Save images into Images folder
                FileUpload2.SaveAs(Server.MapPath("/EmployeeImages/Signature/" + txtEmpCardNo.Text.Trim() + filename));
                // FileUpload1.SaveAs(Server.MapPath("/EmployeeImages/Images/" + txtEmpCardNo.Text.Trim() + filename));
            }
            catch { }
        }
        private void SaveAttachFile()
        {
            try
            {
                

                if (fileUploadDoc.HasFile)
                {
                    DeleteAttachFile(txtRegistrationId.Text);
                    string filename = Path.GetFileName(fileUploadDoc.PostedFile.FileName);
                    filename = txtRegistrationId.Text.Trim() + "." + filename.Substring(filename.Length - 3);                   
                    fileUploadDoc.SaveAs(Server.MapPath("/EmployeeImages/AttachFile/" + filename));                    
                }

            }
            catch { }
        }
        private void DeleteAttachFile(string filename)
        {
            try
            {
                //Get Filename from fileupload control    
                string[] extension = { ".jpeg", ".jpg",".png",".pdf" };
                string path = Server.MapPath("/EmployeeImages/AttachFile/" + filename );
                foreach (string i in extension)
                {
                    FileInfo file = new FileInfo(path + i);
                    if (file.Exists)
                    {
                        file.Delete();                        
                    }
                }
                
            }
            catch { }
        }
        private Boolean UpdateEmployeeInfo(string EmpId)
        {
            try
            {
                DataTable dt = new DataTable();              
                    sqlDB.fillDataTable("Select max(SN) as SN from Personnel_EmpCurrentStatus where EmpId='" + ddlEmpCardNo.SelectedValue + "'", dt);                
                System.Data.SqlTypes.SqlDateTime getDate;
                getDate = SqlDateTime.Null;
                SqlCommand cmd = new SqlCommand("UpdateEmployeeInfo", sqlDB.connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SN", dt.Rows[0]["SN"].ToString());
                cmd.Parameters.AddWithValue("@EmpId", ddlEmpCardNo.SelectedValue);
                //if (EmpId == null)
                //{
                //    cmd.Parameters.AddWithValue("@EmpId", ddlEmpCardNo.SelectedValue);
                //}
                //else
                //{
                //    cmd.Parameters.AddWithValue("@EmpId", EmpId);
                //}
                //  if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin")) cmd.Parameters.AddWithValue("@CompanyId", ddlBranch.SelectedValue);               
                cmd.Parameters.AddWithValue("@CompanyId", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpTypeId", rblEmpType.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpName", txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@NickName", txtNickName.Text.Trim());
                cmd.Parameters.AddWithValue("@EmpNameBn", txtNameBangla.Text.Trim());
                if (chkAlternativeEmpCard.Checked)
                {
                    if (ViewState["__CardNoType__"].ToString().Equals("True"))
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + ViewState["__DptCode__"].ToString() + txtAlternativeCard.Text);
                        //cmd.Parameters.AddWithValue("@EmpProximityNo", txtRegistrationId.Text.Trim());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + ViewState["__FlatCode__"].ToString() + txtAlternativeCard.Text);
                        //cmd.Parameters.AddWithValue("@EmpProximityNo", txtRegistrationId.Text.Trim());
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EmpCardNo", txtEmpCardNo.Text.Trim());
                    string cardNo = new String(txtEmpCardNo.Text.Where(Char.IsNumber).ToArray());
                    //int ProximityNo = int.Parse(cardNo.Substring(DateTime.Now.Year.ToString().Length, cardNo.Length - DateTime.Now.Year.ToString().Length));
                    //cmd.Parameters.AddWithValue("@EmpProximityNo", txtRegistrationId.Text.Trim());
                }
                cmd.Parameters.AddWithValue("@EmpProximityNo", txtRegistrationId.Text.Trim());
                cmd.Parameters.AddWithValue("@PunchType", rblPunchType.SelectedValue);
                cmd.Parameters.AddWithValue("@RealProximityNo", txtProximityNo.Text.Trim());
                cmd.Parameters.AddWithValue("@EmpStatus", ddlEmpStatus.SelectedValue);
              
                
                cmd.Parameters.AddWithValue("@SftId", ddlShift.SelectedValue);
                
               
                cmd.Parameters.AddWithValue("@EmpJoiningDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
                
             
                cmd.Parameters.AddWithValue("@EarnedLeave", txtEarnedLeave.Text);
                if (txtElStart.Text.Length == 0)
                {
                    cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", getDate);
                }
                else cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", convertDateTime.getCertainCulture(txtElStart.Text.Trim()));
              



                if (FileUpload1.HasFile == true)
                {
                    cmd.Parameters.AddWithValue("@EmpPicture", txtEmpCardNo.Text.Trim() + FileUpload1.FileName);
                    if (imageName != "")
                    {
                        System.IO.File.Delete(Request.PhysicalApplicationPath + "/EmployeeImages/Images/" + imageName);
                    }
                    string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);

                    System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                    int width = 100;
                    int height = 100;
                    using (System.Drawing.Image thumbnail = image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            thumbnail.Save(Server.MapPath("/EmployeeImages/Images/" + txtEmpCardNo.Text.Trim() + filename), System.Drawing.Imaging.ImageFormat.Png);
                        }
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EmpPicture", "");
                }
                if (FileUpload2.HasFile == true)
                {
                    cmd.Parameters.AddWithValue("@SignatureImage", txtEmpCardNo.Text.Trim() + FileUpload2.FileName);
                    if (SignatureImage != "")
                    {
                        System.IO.File.Delete(Request.PhysicalApplicationPath + "/EmployeeImages/Signature/" + SignatureImage);
                    }
                    string filename = Path.GetFileName(FileUpload2.PostedFile.FileName);
                    FileUpload2.SaveAs(Server.MapPath("/EmployeeImages/Signature/" + txtEmpCardNo.Text + filename));    //Save images into Images folder
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SignatureImage", "");
                }
             
                cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Text);

                cmd.Parameters.AddWithValue("@ExpireDate", convertDateTime.getCertainCulture(txtExpireDate.Text.Trim()));

                cmd.Parameters.AddWithValue("@PreCompanyId", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@PreEmpTypeId", rblEmpType.SelectedValue);


                //cmd.Parameters.AddWithValue("@PreSalaryType", "Scale");
                //cmd.Parameters.AddWithValue("@SalaryType", "Scale");
               
                //cmd.Parameters.AddWithValue("@PreIncrementAmount", 0);
                //cmd.Parameters.AddWithValue("@IncrementAmount", 0);

                if (ViewState["__CardNoType__"].ToString().Equals("True"))
                {

                    cmd.Parameters.AddWithValue("@PreDptId", ViewState["__DptId__"].ToString());
                    cmd.Parameters.AddWithValue("@DptId", ViewState["__DptId__"].ToString());
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PreDptId", ddlDepartment.SelectedValue);
                    cmd.Parameters.AddWithValue("@DptId", ddlDepartment.SelectedValue);
                }

                cmd.Parameters.AddWithValue("@PreDsgId", ddlDesingnation.SelectedValue);
                cmd.Parameters.AddWithValue("@DsgId", ddlDesingnation.SelectedValue);
                cmd.Parameters.AddWithValue("@PreGId", ddlGrouping.SelectedValue);
                cmd.Parameters.AddWithValue("@GId", ddlGrouping.SelectedValue);
                cmd.Parameters.AddWithValue("@PreEmpStatus", ddlEmpStatus.SelectedValue);



                // cmd.Parameters.AddWithValue("@DateofUpdate", convertDateTime.getCertainCulture(DateTime.Now.ToString("d-M-yyyy"))); this line is commented,03-11-2019.for last increment date issue
                cmd.Parameters.AddWithValue("@TypeOfChange", "s");
                cmd.Parameters.AddWithValue("@EffectiveMonth", "");
                cmd.Parameters.AddWithValue("@OrderRefNo", "");
                cmd.Parameters.AddWithValue("@OrderRefDate", getDate);
                cmd.Parameters.AddWithValue("@Remarks", "");
                cmd.Parameters.AddWithValue("@ActiveSalary", 1);

                cmd.Parameters.AddWithValue("@EarnLeaveDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
                cmd.Parameters.AddWithValue("@IsActive", 1);

                if (cskFlatOrder.Checked == true)
                    cmd.Parameters.AddWithValue("@CustomOrdering", txtFlatOrder.Text.Trim());
                else
                    cmd.Parameters.AddWithValue("@CustomOrdering",txtDptWise.Text.Trim());

                cmd.Parameters.AddWithValue("@TIN", txtTIN.Text.Trim());
                cmd.Parameters.AddWithValue("@PreSalaryType", rblSalaryType.SelectedValue);
                cmd.Parameters.AddWithValue("@SalaryType", rblSalaryType.SelectedValue);

                cmd.Parameters.AddWithValue("@PreEmpDutyType", ViewState["__PreDutyType__"].ToString());
                
                cmd.Parameters.AddWithValue("@EmpDutyType", rblDutyType.SelectedValue);
                cmd.Parameters.AddWithValue("@AuthorizedPerson", ckbAuthorized.Checked);
                cmd.Parameters.AddWithValue("@WeekendType", rblWeekendType.SelectedValue);

                int result = (int)cmd.ExecuteScalar();


                if (result > 0)
                {
                    SaveAttachFile();
                    if (ckbProximityChange.Checked)
                    {
                        if (!ViewState["__OldRealProximity__"].ToString().Equals(txtProximityNo.Text.Trim()))
                            SaveProximityChageLog();
                    }
                    //AllClear();
                    //    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UpdateSuccess();", true);  
                    if (ViewState["__SfiftId__"].ToString() != ddlShift.SelectedValue)
                    {
                        if (EmpId == null)
                        {
                            EmpId = ddlEmpCardNo.SelectedValue;
                        }
                        DataTable dtStID = new DataTable();
                        sqlDB.fillDataTable("SELECT Distinct STId FROM ShiftTransferInfoDetails Where EmpId='" + EmpId + "'", dtStID);
                        if (dtStID.Rows.Count == 1)
                        {
                            SqlCommand cmdDel = new SqlCommand("DELETE FROM ShiftTransferInfoDetails WHERE EmpId='" + EmpId + "'", sqlDB.connection);
                            cmdDel.ExecuteNonQuery();
                            //  saveShiftTransferDetails(EmpId);
                        }
                    }
                }
                //else ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UnableUpdate();", true);
                return true;

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
                return false;
            }
        }
        private void UpdateSpecificEmployee(string EmpId)
        {
            try
            {

                System.Data.SqlTypes.SqlDateTime getDate;
                getDate = SqlDateTime.Null;
                if (FileUpload1.HasFile == true && FileUpload2.HasFile == false)
                {
                    SqlCommand cmd = new SqlCommand("update Personnel_EmployeeInfo  Set EmpId=@EmpId, EmpTypeId=@EmpTypeId, EmpName=@EmpName,NickName=@NickName, EmpNameBn=@EmpNameBn, EmpCardNo=@EmpCardNo, EmpProximityNo=@EmpProximityNo, EmpStartLunch=@EmpStartLunch, EmpEndLunch=@EmpEndLunch, EmpAccountNo=@EmpAccountNo, SftId=@SftId, EmpShiftStartDate=@EmpShiftStartDate, EmpJoiningDate=@EmpJoiningDate, GrdName=@GrdName, EmpJoinigSalary=@EmpJoinigSalary, PfMember=@PfMember, PfDate=@PfDate, PFAmount=@PFAmount, EarnedLeave=@EarnedLeave, EarnedLeaveEffectedFrom=@EarnedLeaveEffectedFrom, BonusType=@BonusType, EmpPicture=@EmpPicture, SalaryCount=@SalaryCount, BankName=@BankName,Type=@Type,WagesType=@WagesType,ExpireDate=@ExpireDate where EmpId=@EmpId ", sqlDB.connection);
                    cmd.Parameters.AddWithValue("@EmpId", EmpId);
                    cmd.Parameters.AddWithValue("@EmpTypeId", rblEmpType.SelectedValue);
                    cmd.Parameters.AddWithValue("@EmpName", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@NickName", txtNickName.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpNameBn", txtNameBangla.Text.Trim());
                    if (chkAlternativeEmpCard.Checked)
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + txtAlternativeCard.Text);
                        cmd.Parameters.AddWithValue("@EmpProximityNo", int.Parse(txtAlternativeCard.Text));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", txtEmpCardNo.Text.Trim());
                        string cardNo = new String(txtEmpCardNo.Text.Where(Char.IsNumber).ToArray());
                        int ProximityNo = int.Parse(cardNo.Substring(DateTime.Now.Year.ToString().Length, cardNo.Length - DateTime.Now.Year.ToString().Length));
                        cmd.Parameters.AddWithValue("@EmpProximityNo", ProximityNo);
                    }
                  
                    cmd.Parameters.AddWithValue("@SftId", ddlShift.SelectedValue);
                   
                    cmd.Parameters.AddWithValue("@EmpJoiningDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
                   
                    cmd.Parameters.AddWithValue("@EarnedLeave", txtEarnedLeave.Text);
                    if (txtElStart.Text.Length == 0)
                    {
                        cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", getDate);
                    }
                    else cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", convertDateTime.getCertainCulture(txtElStart.Text.Trim()));
                   
                    cmd.Parameters.AddWithValue("@EmpPicture", txtEmpCardNo.Text + FileUpload1.FileName);
                    
                    cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Text);

                    cmd.Parameters.AddWithValue("@ExpireDate", convertDateTime.getCertainCulture(txtExpireDate.Text.Trim()));

                    int result = (int)cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                       
                        if (imageName != "")
                        {
                            System.IO.File.Delete(Request.PhysicalApplicationPath + "/EmployeeImages/Images/" + imageName);
                        }
                        string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);

                        System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                        int width = 100;
                        int height = 100;
                        using (System.Drawing.Image thumbnail = image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                thumbnail.Save(Server.MapPath("/EmployeeImages/Images/" + txtEmpCardNo.Text.Trim() + filename), System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                }
                else if (FileUpload1.HasFile == false && FileUpload2.HasFile == false)
                {
                    SqlCommand cmd = new SqlCommand("update Personnel_EmployeeInfo  Set EmpId=@EmpId,EmpTypeId=@EmpTypeId, EmpName=@EmpName,NickName=@NickName, EmpNameBn=@EmpNameBn, EmpCardNo=@EmpCardNo, EmpProximityNo=@EmpProximityNo, EmpStatus=@EmpStatus, EmpStartLunch=@EmpStartLunch, EmpEndLunch=@EmpEndLunch, EmpAccountNo=@EmpAccountNo, SftId=@SftId, EmpShiftStartDate=@EmpShiftStartDate, EmpJoiningDate=@EmpJoiningDate, GrdName=@GrdName, EmpJoinigSalary=@EmpJoinigSalary, PfMember=@PfMember, PfDate=@PfDate, PFAmount=@PFAmount, EarnedLeave=@EarnedLeave, EarnedLeaveEffectedFrom=@EarnedLeaveEffectedFrom, BonusType=@BonusType, SalaryCount=@SalaryCount, BankName=@BankName,Type=@Type,WagesType=@WagesType,ExpireDate=@ExpireDate where EmpId=@EmpId ", sqlDB.connection);
                    cmd.Parameters.AddWithValue("@EmpId", EmpId);
                    cmd.Parameters.AddWithValue("@EmpTypeId", rblEmpType.SelectedValue);
                    cmd.Parameters.AddWithValue("@EmpName", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@NickName", txtNickName.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpNameBn", txtNameBangla.Text.Trim());
                    if (chkAlternativeEmpCard.Checked)
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + txtAlternativeCard.Text);
                        cmd.Parameters.AddWithValue("@EmpProximityNo", int.Parse(txtAlternativeCard.Text));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", txtEmpCardNo.Text.Trim());
                        string cardNo = new String(txtEmpCardNo.Text.Where(Char.IsNumber).ToArray());
                        int ProximityNo = int.Parse(cardNo.Substring(DateTime.Now.Year.ToString().Length, cardNo.Length - DateTime.Now.Year.ToString().Length));
                        cmd.Parameters.AddWithValue("@EmpProximityNo", ProximityNo);
                    }
                    

                   
                    cmd.Parameters.AddWithValue("@SftId", ddlShift.SelectedValue);
                    
                    cmd.Parameters.AddWithValue("@EmpJoiningDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
                    


                  
                    cmd.Parameters.AddWithValue("@EarnedLeave", txtEarnedLeave.Text);
                    if (txtElStart.Text.Length == 0)
                    {
                        cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", getDate);
                    }
                    else cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", convertDateTime.getCertainCulture(txtElStart.Text.Trim()));
                    
                    cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Text);

                    cmd.Parameters.AddWithValue("@ExpireDate", convertDateTime.getCertainCulture(txtExpireDate.Text.Trim()));

                    cmd.ExecuteNonQuery();
                    
                }
                else if (FileUpload1.HasFile == false && FileUpload2.HasFile == true)
                {
                    SqlCommand cmd = new SqlCommand(" update Personnel_EmployeeInfo  Set EmpId=@EmpId,EmpTypeId=@EmpTypeId, EmpName=@EmpName,NickName=@NickName, EmpNameBn=@EmpNameBn, EmpCardNo=@EmpCardNo, EmpProximityNo=@EmpProximityNo, EmpStatus=@EmpStatus, EmpStartLunch=@EmpStartLunch, EmpEndLunch=@EmpEndLunch, EmpAccountNo=@EmpAccountNo, SftId=@SftId, EmpShiftStartDate=@EmpShiftStartDate, EmpJoiningDate=@EmpJoiningDate, GrdName=@GrdName, EmpJoinigSalary=@EmpJoinigSalary, PfMember=@PfMember, PfDate=@PfDate, PFAmount=@PFAmount, EarnedLeave=@EarnedLeave, EarnedLeaveEffectedFrom=@EarnedLeaveEffectedFrom, BonusType=@BonusType,SignatureImage=@SignatureImage, SalaryCount=@SalaryCount, BankName=@BankName,Type=@Type,WagesType=@WagesType,ExpireDate=@ExpireDate where EmpId=@EmpId ", sqlDB.connection);
                    cmd.Parameters.AddWithValue("@EmpId", EmpId);
                    cmd.Parameters.AddWithValue("@EmpTypeId", rblEmpType.SelectedValue);
                    cmd.Parameters.AddWithValue("@EmpName", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@NickName", txtNickName.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpNameBn", txtNameBangla.Text.Trim());
                    if (chkAlternativeEmpCard.Checked)
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + txtAlternativeCard.Text);
                        cmd.Parameters.AddWithValue("@EmpProximityNo", int.Parse(txtAlternativeCard.Text));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", txtEmpCardNo.Text.Trim());
                        string cardNo = new String(txtEmpCardNo.Text.Where(Char.IsNumber).ToArray());
                        int ProximityNo = int.Parse(cardNo.Substring(DateTime.Now.Year.ToString().Length, cardNo.Length - DateTime.Now.Year.ToString().Length));
                        cmd.Parameters.AddWithValue("@EmpProximityNo", ProximityNo);
                    }
                   
                    
                    cmd.Parameters.AddWithValue("@SftId", ddlShift.SelectedValue);
                   
                   
                    cmd.Parameters.AddWithValue("@EmpJoiningDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
                   
                    cmd.Parameters.AddWithValue("@EarnedLeave", txtEarnedLeave.Text);
                    if (txtElStart.Text.Length == 0)
                    {
                        cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", getDate);
                    }
                    else cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", convertDateTime.getCertainCulture(txtElStart.Text.Trim()));
                    
                    cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Text);

                    cmd.Parameters.AddWithValue("@ExpireDate", convertDateTime.getCertainCulture(txtExpireDate.Text.Trim()));

                    int result = (int)cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        
                        if (SignatureImage != "")
                        {
                            System.IO.File.Delete(Request.PhysicalApplicationPath + "/EmployeeImages/Signature/" + SignatureImage);
                        }
                        string filename = Path.GetFileName(FileUpload2.PostedFile.FileName);
                        FileUpload2.SaveAs(Server.MapPath("/EmployeeImages/Signature/" + txtEmpCardNo.Text + filename));    //Save images into Images folder
                    }
                }
                else if (FileUpload1.HasFile == true && FileUpload2.HasFile == true)
                {
                    SqlCommand cmd = new SqlCommand("update Personnel_EmployeeInfo  Set EmpId=@EmpId,EmpTypeId=@EmpTypeId, EmpName=@EmpName,NickName=@NickName, EmpNameBn=@EmpNameBn, EmpCardNo=@EmpCardNo, EmpProximityNo=@EmpProximityNo, EmpStatus=@EmpStatus, EmpStartLunch=@EmpStartLunch, EmpEndLunch=@EmpEndLunch, EmpAccountNo=@EmpAccountNo, SftId=@SftId, EmpShiftStartDate=@EmpShiftStartDate, EmpJoiningDate=@EmpJoiningDate, GrdName=@GrdName, EmpJoinigSalary=@EmpJoinigSalary, PfMember=@PfMember, PfDate=@PfDate, PFAmount=@PFAmount, EarnedLeave=@EarnedLeave, EarnedLeaveEffectedFrom=@EarnedLeaveEffectedFrom, BonusType=@BonusType, EmpPicture=@EmpPicture, SignatureImage=@SignatureImage, SalaryCount=@SalaryCount, BankName=@BankName,Type=@Type,WagesType=@WagesType,ExpireDate=@ExpireDate where EmpId=@EmpId ", sqlDB.connection);
                    cmd.Parameters.AddWithValue("@EmpId", EmpId);
                    cmd.Parameters.AddWithValue("@EmpTypeId", rblEmpType.SelectedValue);
                    cmd.Parameters.AddWithValue("@EmpName", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@NickName", txtNickName.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmpNameBn", txtNameBangla.Text.Trim());
                    if (chkAlternativeEmpCard.Checked)
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", ViewState["__ShortName__"].ToString() + DateTime.Now.Year + txtAlternativeCard.Text);
                        cmd.Parameters.AddWithValue("@EmpProximityNo", int.Parse(txtAlternativeCard.Text));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EmpCardNo", txtEmpCardNo.Text.Trim());
                        string cardNo = new String(txtEmpCardNo.Text.Where(Char.IsNumber).ToArray());
                        int ProximityNo = int.Parse(cardNo.Substring(DateTime.Now.Year.ToString().Length, cardNo.Length - DateTime.Now.Year.ToString().Length));
                        cmd.Parameters.AddWithValue("@EmpProximityNo", ProximityNo);
                    }
                   
                    cmd.Parameters.AddWithValue("@SftId", ddlShift.SelectedValue);
                    
                    cmd.Parameters.AddWithValue("@EmpJoiningDate", convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()));
                    
                    cmd.Parameters.AddWithValue("@EarnedLeave", txtEarnedLeave.Text);
                    if (txtElStart.Text.Length == 0)
                    {
                        cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", getDate);
                    }
                    else cmd.Parameters.AddWithValue("@EarnedLeaveEffectedFrom", convertDateTime.getCertainCulture(txtElStart.Text.Trim()));
                    
                    cmd.Parameters.AddWithValue("@Type", ddlType.SelectedItem.Text);

                    cmd.Parameters.AddWithValue("@ExpireDate", convertDateTime.getCertainCulture(txtExpireDate.Text.Trim()));

                    int result = (int)cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                       

                     
                        if (imageName != "")
                        {
                            System.IO.File.Delete(Request.PhysicalApplicationPath + "/EmployeeImages/Images/" + imageName);
                        }
                        string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);

                        System.Drawing.Image image = System.Drawing.Image.FromStream(FileUpload1.PostedFile.InputStream);
                        int width = 100;
                        int height = 100;
                        using (System.Drawing.Image thumbnail = image.GetThumbnailImage(width, height, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                thumbnail.Save(Server.MapPath("/EmployeeImages/Images/" + txtEmpCardNo.Text.Trim() + filename), System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                        if (SignatureImage != "")
                        {
                            System.IO.File.Delete(Request.PhysicalApplicationPath + "/EmployeeImages/Signature/" + SignatureImage);
                        }
                        string filename2 = Path.GetFileName(FileUpload2.PostedFile.FileName);
                        FileUpload2.SaveAs(Server.MapPath("/EmployeeImages/Signature/" + txtEmpCardNo.Text + filename2));    //Save images into Images folder
                    }
                }
            }
            catch { }
        }
        private void AllClear()
        {
            try
            {
                if (ViewState["__CardNoType__"].ToString().Equals("False"))
                {
                    AutoGenerateEmpId();
                    ddlDepartment.Enabled = true;
                    ddlDepartment.SelectedIndex = 0;
                    ddlDesingnation.Items.Clear();
                }
                if (ViewState["__CardNoType__"].ToString().Equals("True"))
                { 
                    AutoGenerateEmpIdByDepartment();
                    DepartmentCustomOrdering();
                    ddlDesingnation.SelectedIndex = 0;
                }
                FlatCustomOrdering();
                // txtCardNo.Text = "";
               // ddlBranch.Enabled = true;
                Session["_EditStatus_"] = "Save";
                btnSave.Text = "Save";
                ViewState["__CheckEmpCardNo__"] = "False";
                hdfpresentsalryKeypress.Value = "0";
                //hdfUserType.Value = "0";
                txtTIN.Text = "";
                txtExpireDate.Text = "";
                txtName.Text = "";
                txtNickName.Text = "";
                txtNameBangla.Text = "";
                txtProximityNo.Text = "";
                hdfBasic.Value = "0";
                hdfConveyance.Value = "0";
                hdfhouserent.Value = "0";
                hdfMedical.Value = "0";
                //txtDesingnationBangla.Text = "";
                //txtEmpCardNo.Text = "";
                txtAlternativeCard.Text = "";
              
                txtJoiningDate.Text = "";
                // ddlGrade.SelectedIndex = 0;
                //txtJoiningSalary.Text = "0";
                //txtPresentSalary.Text = "0";
                //txtMedical.Text = "0";
                //txtBasic.Text = "0";
                //txtFoodAllowance.Text = "0";
                //txtHouseRent.Text = "0";
                //txtConveyanceAllow.Text = "0";
                //txtHolidayAllow.Text = "0";
                //txtTiffinAllowance.Text = "0";
                //txtNightAllowance.Text = "0";
                //txtAttenBonus.Text = "0";
                //chkPFMember.Checked = false;
                //txtPFDate.Text = "";
                //txtEarnedLeave.Text = "0";
                //txtElStart.Text = "";
                //txtBankName.Text = "";
                imgProfile.ImageUrl = "/images/profileImages/noProfileImage.jpg";
                imgSignature.ImageUrl = "/images/profileImages/Signature.jpg";
                HiddenField1.Value = "";
                ddlGrouping.SelectedValue = "0";
                // lblEmployeeMode.Text = "Add Mode";
                if (ViewState["__WriteAction__"].ToString().Equals("0"))
                {

                    btnSave.CssClass = "";
                    btnSave.Enabled = false;
                }
                else
                {
                    btnSave.CssClass = "css_btn Ptbut";
                    btnSave.Enabled = true;
                }
                hdfsaveupdatestatus.Value = "Save";
                
                //rblEmpType.SelectedValue = "2";
               
                //chkAlternativeEmpCard.Checked = false;
                PropertyInfo isreadonly =
  typeof(System.Collections.Specialized.NameValueCollection).GetProperty(
  "IsReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);
                // make collection editable
                isreadonly.SetValue(this.Request.QueryString, false, null);
                // remove
                this.Request.QueryString.Remove("EmpId");
                this.Request.QueryString.Remove("Edit");

                ddlEmpCardNo.SelectedIndex = 0;
                ddlEmpStatus.SelectedIndex = 0;
                // Request.QueryString.Remove("EmpId");

                // Request.QueryString["EmpId"] =null;
                //------------For Parsonal------------------
                dsBloodGroup.SelectedValue = "0";
                dsDateOfBirth.Text = "";
                dsFatherName.Text = "";
                dsMotherName.Text = "";
                dsPlaceOfBirth.Text = "";
                dsHeight.Text = "";
                dsWeight.Text = "";
                dsSex.SelectedIndex = 0;
                dsBloodGroup.SelectedIndex = 0;
                dsMaritialStatus.SelectedIndex = 0;
                dsReligion.SelectedIndex = 0;
                dsNationIDCardNo.Text = ""; dsNoOfExperience.Text = ""; ddlLastEdQualification.SelectedValue = "0";
                txtMobileNo.Text = "";

                //---------------------------------------

            }
            catch { }
        }
        public void ddlDesingnation_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("Select DsgNameBn From HRD_Designation where DsgId=" + ddlDesingnation.SelectedValue + "", dt);
                //txtDesingnationBangla.Text = dt.Rows[0]["DsgNameBn"].ToString();
            }
            catch { }
        }


        private void SaveProximityChageLog() 
        {
            try {
            
                 string FromDate = "";
                 string ToDate = txtProximityChangeDate.Text;                
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select CONVERT(VARCHAR(10),dateadd(DD, 1, ToDate), 105)    as ToDate from Personnel_EmpProximityChange_Log where EmpId='" + ddlEmpCardNo.SelectedValue + "'  order by SL desc", dt);
                if (dt.Rows.Count > 0)
                {
                    FromDate = dt.Rows[0]["ToDate"].ToString();
                }
                else
                    FromDate = txtJoiningDate.Text.Trim();
               
                SqlCommand cmd = new SqlCommand("Insert into  Personnel_EmpProximityChange_Log (EmpId, EmpProximityNo, FromDate,ToDate)  values (@EmpId, @EmpProximityNo, @FromDate, @ToDate) ", sqlDB.connection);

                cmd.Parameters.AddWithValue("@EmpId",ddlEmpCardNo.SelectedValue);
                cmd.Parameters.AddWithValue("@EmpProximityNo", ViewState["__OldRealProximity__"].ToString());
                cmd.Parameters.AddWithValue("@FromDate", convertDateTime.getCertainCulture(FromDate));
                cmd.Parameters.AddWithValue("@ToDate", convertDateTime.getCertainCulture(ToDate));
                cmd.ExecuteNonQuery();
            }
            catch { }
        }
    

      
        protected void ddlEmpCardNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                if (ddlEmpCardNo.SelectedValue != "0")
                {
                    if (ViewState["__UpdateAction__"].ToString().Equals("0"))
                    {

                        btnSave.CssClass = "";
                        btnSave.Enabled = false;
                    }
                    else
                    {
                        btnSave.CssClass = "css_btn Ptbut";
                        btnSave.Enabled = true;
                    }
                    string[] EmpCardNowithName = ddlEmpCardNo.SelectedItem.Text.ToString().Split(' ');

                    txtEmpCardNo.Text = EmpCardNowithName[0];
                    Session["_EmpId_"] = ddlEmpCardNo.SelectedValue;
                    //lblEmployeeMode.Text = "Edit Mode";
                    Session["_EditStatus_"] = "Update";

                    LoadEmployeeCardNowiseData(ddlEmpCardNo.SelectedValue, "0");
                    LoadEmpPersonalInfo(Session["_EmpId_"].ToString());//load Employee personal

                    // rblusertype.Enabled = false;
                    Session["_EditStatus_"] = "Update";
                    btnSave.Text = "Update";
                    ViewState["__CheckEmpCardNo__"] = "True";
                }
                else { AllClear(); }
            }
            catch { }
        }
        private void LoadEmployeeCardNowiseData(string EmpId, string list)
        {
            try
            {

                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "usertype();", true);
               
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("Select EmpId,MAX(SN) as SN from v_EmployeeDetails  where EmpId='" + EmpId + "' and IsActive=1 Group by EmpId", dt);
                if (dt.Rows.Count == 0)
                {
                    lblMessage.InnerText = "warning->This Card Not Valid";
                    return;
                }
                DataTable dtall = new DataTable();
                sqlDB.fillDataTable("Select * From v_EmployeeDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and SN=" + dt.Rows[0]["SN"].ToString() + "", dtall);
                //if (dtall.Rows[0]["EmpType"].ToString() == "Staff")
                //{
                //    DataTable dtUesr = new DataTable();
                //    sqlDB.fillDataTable("Select UserType From UserAccount where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "'", dtUesr);
                //    if (dtUesr.Rows.Count > 0)
                //    {

                //        string getUserType = ComplexLetters.getEntangledLetters(dtUesr.Rows[0]["UserType"].ToString());
                //        if (getUserType == "User") rblusertype.SelectedIndex = 0;
                //        else if (getUserType == "Viewer") rblusertype.SelectedIndex = 1;
                //        else if (getUserType == "Admin") rblusertype.SelectedIndex = 2;
                //        else if (getUserType == "Super Admin") rblusertype.SelectedIndex = 3;
                //        else rblusertype.SelectedIndex = 4;
                //    }

                //    //rblusertype.SelectedValue = getUserType ;
                //}
                //else fs.Visible = false;
                //string getSftId=dtall.Rows[0]["SftId"].ToString();
                //ddlShift.SelectedValue = getSftId;
                
                ddlBranch.SelectedValue = dtall.Rows[0]["CompanyId"].ToString();
                ddlBranch.Enabled = false;
                rblEmpType.SelectedValue = dtall.Rows[0]["EmpTypeId"].ToString();
                ckbAuthorized.Checked = bool.Parse(dtall.Rows[0]["AuthorizedPerson"].ToString());
                //if (rblEmpType.SelectedValue == "3")
                //{
                //    trtxtExpireDate.Visible = true;
                //    txtExpireDate.Text = Convert.ToDateTime(dtall.Rows[0]["ExpireDate"].ToString()).ToString("d-M-yyyy");
                //}
                //else
                //{
                //    trtxtExpireDate.Visible = false;
                //    txtExpireDate.Text = "";
                //}
                if (list == "1")
                {

                    classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue, ddlBranch.SelectedValue, txtEmpCardNo.Text.Trim());
                    ddlEmpCardNo.SelectedValue = EmpId;
                    ddlEmpCardNo.Enabled = false;
                }  
                txtName.Text = dtall.Rows[0]["EmpName"].ToString();
                txtNickName.Text = dtall.Rows[0]["NickName"].ToString();
                txtNameBangla.Text = dtall.Rows[0]["EmpNameBn"].ToString();

                //classes.commonTask.SearchDepartment(ddlBranch.SelectedValue, ddlDepartment);
                if (ViewState["__CardNoType__"].ToString().Equals("True"))
                {
                    ViewState["__DptId__"] = dtall.Rows[0]["DptId"].ToString();
                    ViewState["__DptCode__"] = dtall.Rows[0]["DptCode"].ToString();
                    ddlDepartment.SelectedValue = dtall.Rows[0]["DptId"].ToString()+"_"+dtall.Rows[0]["DptCode"].ToString();
                    ddlDepartment.Enabled = false;
                   txtDptWise.Text = dtall.Rows[0]["CustomOrdering"].ToString();
                }
                else
                {    
                    ddlDepartment.Enabled = true; 
                    ddlDepartment.SelectedValue = dtall.Rows[0]["DptId"].ToString(); 
                }
                txtRegistrationId.Text = dtall.Rows[0]["EmpProximityNo"].ToString();
                txtTIN.Text = dtall.Rows[0]["TIN"].ToString();
                            
                classes.commonTask.LoadDesignation(dtall.Rows[0]["DptId"].ToString(), ddlDesingnation);
                ddlDesingnation.SelectedValue = dtall.Rows[0]["DsgId"].ToString();
                if (ViewState["__LineORGroupDependency__"].ToString().Equals("True"))
                    classes.commonTask.LoadGrouping(ddlGrouping, ddlBranch.SelectedValue, dtall.Rows[0]["DptId"].ToString());
                else
                    classes.commonTask.LoadGrouping(ddlGrouping, ddlBranch.SelectedValue);
                string GId = (dtall.Rows[0]["GId"].ToString().Equals("")) ? "0" : dtall.Rows[0]["GId"].ToString();
                ddlGrouping.SelectedValue = GId;
                ddlEmpStatus.SelectedValue = dtall.Rows[0]["EmpStatus"].ToString();
          
               
                txtEmpCardNo.Text = dtall.Rows[0]["EmpCardNo"].ToString();
                if (dtall.Rows[0]["PunchType"].ToString().Equals("True"))
                {
                    rblPunchType.SelectedValue = "1";
                    txtProximityNo.Enabled = true;
                }                   
                else
                {
                    rblPunchType.SelectedValue = "0";
                    txtProximityNo.Enabled = false;
                }

                ViewState["__OldRealProximity__"] = dtall.Rows[0]["RealProximityNo"].ToString();
                ckbProximityChange.Enabled = true;
                txtProximityNo.Text = dtall.Rows[0]["RealProximityNo"].ToString();
                //classes.commonTask.LoadShift(ddlShift, ddlBranch.SelectedValue);
                classes.commonTask.LoadInitialShiftByDepartment(ddlShift, ddlBranch.SelectedValue, dtall.Rows[0]["DptId"].ToString());
                ddlShift.SelectedValue = dtall.Rows[0]["SftId"].ToString();
                ViewState["__SfiftId__"] = ddlShift.SelectedValue;
                
                txtJoiningDate.Text = Convert.ToDateTime(dtall.Rows[0]["EmpJoiningDate"].ToString()).ToString("d-M-yyyy");
                txtExpireDate.Text = Convert.ToDateTime(dtall.Rows[0]["ExpireDate"].ToString()).ToString("d-M-yyyy");
               
                ddlType.Text = dtall.Rows[0]["Type"].ToString();

                txtFlatOrder.Text = dtall.Rows[0]["CustomOrdering"].ToString();
                rblSalaryType.SelectedValue= dtall.Rows[0]["SalaryType"].ToString().Trim();

                ViewState["__PreDutyType__"] = dtall.Rows[0]["EmpDutyType"].ToString().Trim();
                if (dtall.Rows[0]["EmpDutyType"].ToString().Equals(""))
                    rblDutyType.SelectedValue = "Regular";
                else
                rblDutyType.SelectedValue = dtall.Rows[0]["EmpDutyType"].ToString().Trim();

                if (dtall.Rows[0]["WeekendType"].ToString().Equals(""))
                    rblWeekendType.SelectedValue = "Regular";
                else
                    rblWeekendType.SelectedValue = dtall.Rows[0]["WeekendType"].ToString().Trim();

                txtEarnedLeave.Text = dtall.Rows[0]["EarnedLeave"].ToString();
                if (dtall.Rows[0]["EarnedLeaveEffectedFrom"].ToString().Length == 0)
                {
                    txtElStart.Text = "";
                }
                else
                {
                    txtElStart.Text = Convert.ToDateTime(dtall.Rows[0]["EarnedLeaveEffectedFrom"].ToString()).ToString("d-M-yyyy");
                }
                
                Session["_EmppictureName_"] = dtall.Rows[0]["EmpPicture"].ToString();
                imageName = dtall.Rows[0]["EmpPicture"].ToString();
                string url = @"/EmployeeImages/Images/" + Path.GetFileName(dtall.Rows[0]["EmpPicture"].ToString());
                imgProfile.ImageUrl = url;
                SignatureImage = dtall.Rows[0]["SignatureImage"].ToString();
                string url2 = @"/EmployeeImages/Signature/" + Path.GetFileName(dtall.Rows[0]["SignatureImage"].ToString());
                imgSignature.ImageUrl = url2;
                txtMobileNo.Text= dtall.Rows[0]["MobileNo"].ToString();
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
            }
        }
        private void deleteImage()
        {
            try
            {
                //Get Filename from fileupload control
                string filename = Path.GetFileName(FileUpload1.PostedFile.FileName);
                //Save images into Images folder
                FileUpload1.SaveAs(Server.MapPath("/EmployeeImages/Images/" + filename));
                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }
                //  cmd = new SqlCommand("Delete From ", dbc.Connection);
            }
            catch { }
        }

        protected void btndivClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "divClose()", true);
            Session["_EmpStatus_"] = "";
        }

        protected void btnEmpAddressclose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "AllHidewithoutEmployeediv()", true);
            Session["_EmpStatus_"] = "";
        }

        protected void btnExperienceClose_Click(object sender, EventArgs e)
        {
            Session["_EmpStatus_"] = "";

            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "AllHidewithoutEmployeediv()", true);

        }

        protected void btnCloseEmpEducation_Click(object sender, EventArgs e)
        {

            Session["_EmpStatus_"] = "";
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "AllHidewithoutEmployeediv()", true);

        }




        protected void btnAllEmployeeClose_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "divAllEmployeeListClose()", true);
        }
        protected void btnNew_Click(object sender, EventArgs e)
        {
          //  AllClear();
            //lblUser.Text = "";
            //lblPass.Text = "";
          //  txtCardNo.Text = "";
            //rblusertype.ClearSelection();
            //rblusertype.Enabled = true;
            //if (rblEmpType.SelectedItem.Text == "Worker") fs.Visible = false;
            //else fs.Visible = true;
            Response.Redirect("/personnel/employee.aspx");

        }
        protected void btnEmployeeAd_Click(object sender, EventArgs e)
        {
            Session["_EmpStatus_"] = "";
            Session["_EmpId_"] = ddlEmpCardNo.SelectedValue;
            /*            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeeAddress.aspx');", true); */ //Open New Tab for Sever side code


            if (ddlEmpCardNo.SelectedIndex < 1)
            {
                lblMessage.InnerText = "warning->Please, Select an Employee First!";
                ddlEmpCardNo.Focus();
                return;
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeeAddress.aspx?EmpId=" + ddlEmpCardNo.SelectedValue + "');", true);
        }

        protected void btnExperience_Click(object sender, EventArgs e)
        {
            if (ddlEmpCardNo.SelectedIndex < 1)
            {
                lblMessage.InnerText = "warning->Please, Select an Employee First!";
                ddlEmpCardNo.Focus();
                return;
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/employee_experience.aspx?EmpId=" + ddlEmpCardNo.SelectedValue + "');", true);
        }

        protected void btnEducation_Click(object sender, EventArgs e)
        {
                       if (ddlEmpCardNo.SelectedIndex < 1)
            {
                lblMessage.InnerText = "warning->Please, Select an Employee First!";
                ddlEmpCardNo.Focus();
                return;
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeeEducation.aspx?EmpId=" + ddlEmpCardNo.SelectedValue + "');", true);
        }

        protected void chkLunchCount_CheckedChanged(object sender, EventArgs e)
        {

        }

        protected void btnFindEmployee_Click(object sender, EventArgs e)
        {
            if (txtCardNo.Text.Trim().Length < 4)
            {
                lblMessage.InnerText = "warning->Please Type Minimum 4 Character of CardNo";
                return;
            }
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("Select Distinct EmpId From Personnel_EmpCurrentStatus where EmpCardNo Like'%" + txtCardNo.Text + "' and EmpTypeId=" + rblEmpType.SelectedValue + "", dt);
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->This Card Not Valid";
                return;
            }
            ViewState["__CheckEmpCardNo__"] = "True";
            LoadEmployeeCardNowiseData(dt.Rows[0]["EmpId"].ToString(), "2");
            LoadEmpPersonalInfo(dt.Rows[0]["EmpId"].ToString());//load Employee personal
            Session["_EditStatus_"] = "Update";
            btnSave.Text = "Update";
        }

        protected void rbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            //if (rblEmpType.SelectedValue == "3")
            //{
            //    trtxtExpireDate.Visible = true;
            //}
            //else
            //{
            //    trtxtExpireDate.Visible = false;
            //}
            if (ViewState["__CheckEmpCardNo__"].ToString() == "False")
            {
                // classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue);
                classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue, ddlBranch.SelectedValue, txtEmpCardNo.Text.Trim());
            }

            //if (ViewState["__UserType__"].ToString().Equals("Super Admin")||ViewState["__UserType__"].ToString().Equals("Master Admin"))
            //classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue, ddlBranch.SelectedValue);
            //else

            // string EmpId = Request.QueryString["EmpId"];
            //    if (EmpId != null)
            //    {
            //        if (rblEmpType.SelectedItem.Text == "Worker") fs.Visible = false;
            //        else fs.Visible = true;
            //        return;
            //    } 
            //    if (rblEmpType.SelectedItem.Text == "Worker")
            //    {
            //        fs.Visible = false; 
            //        AutoGenerateWorkerID();
            //    }
            //    else
            //    {
            //        AutoGenerateWorkerID();
            //        fs.Visible = true;
            //        rblusertype.ClearSelection();
            //    }

        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
               CompanyId=(ddlBranch.SelectedValue.Equals("0000"))?ViewState["__CompanyId__"].ToString():ddlBranch.SelectedValue;
                LoadCompanyInfo();

                if (ViewState["__CardNoType__"].ToString().Equals("True"))
                    loadDepartMentWithCode();
                //classes.commonTask.SearchDepartmentWithCode(CompanyId, ddlDepartment);
                else
                {
                    loadDepartMents();
                    //classes.commonTask.SearchDepartment(CompanyId, ddlDepartment);
                    AutoGenerateEmpId();
                }
                FlatCustomOrdering();
              //  classes.commonTask.LoadShift(ddlShift, CompanyId);
                if (ViewState["__LineORGroupDependency__"].ToString().Equals("False"))
                    classes.commonTask.LoadGrouping(ddlGrouping, CompanyId);

                classes.Employee.LoadEmpCardNo(ddlEmpCardNo, rblEmpType.SelectedValue, CompanyId, txtEmpCardNo.Text.Trim());
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            }
            catch { ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true); }
        }

        protected void chkAlternativeEmpCard_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAlternativeEmpCard.Checked)
            {
                txtAlternativeCard.Enabled = true;
            }
            else
            {
                txtAlternativeCard.Enabled = false;
            }
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
        }



        //------------------------------For Employee Personal--------------------------@Personal@
        private void LoadEmpPersonalInfo(string EmpId)//.........Load Personal Info..........
        {
            try
            {

                DataTable dt;
                sqlDB.fillDataTable("Select Personnel_EmpPersonnal.FatherName, Personnel_EmpPersonnal.MotherName, Personnel_EmpPersonnal.FatherNameBn, " +
                    "Personnel_EmpPersonnal.MotherNameBN,Personnel_EmpPersonnal.RId, Personnel_EmpPersonnal.MaritialStatus,convert(varchar(11)," +
                    "Personnel_EmpPersonnal.DateOfBirth,105) as DateOfBirth, Personnel_EmpPersonnal.PlaceOfBirth, Personnel_EmpPersonnal.Height," +
                    " Personnel_EmpPersonnal.Weight, Personnel_EmpPersonnal.BloodGroup, Personnel_EmpPersonnal.Sex,  Personnel_EmpPersonnal.NoOfExperience," +
                    " Personnel_EmpPersonnal.Nationality, Personnel_EmpPersonnal.NationIDCardNo,Personnel_EmpPersonnal.NumberofChild," +
                    "Personnel_EmpPersonnal.LastEdQualification,Personnel_EmpPersonnal.EmpVisaNo,HRD_Qualification.QName,HRD_Religion.RName from Personnel_EmpPersonnal " +
                    "Left JOIN HRD_Qualification ON Personnel_EmpPersonnal.LastEdQualification=HRD_Qualification.QId LEFT OUTER JOIN HRD_Religion ON " +
                    "Personnel_EmpPersonnal.RId = HRD_Religion.RId  where Personnel_EmpPersonnal.EmpId='" + EmpId + "'", dt = new DataTable());
                if (dt.Rows.Count == 0)
                {
                    ViewState["__IsSave__"] = "Yes";
                    return;
                }
                    
                dsBloodGroup.Text = dt.Rows[0]["BloodGroup"].ToString();
                dsDateOfBirth.Text = dt.Rows[0]["DateOfBirth"].ToString();
                dsFatherName.Text = dt.Rows[0]["FatherName"].ToString();
                //dsFatherNameBn.Text = dt.Rows[0]["FatherNameBn"].ToString();
                dsHeight.Text = dt.Rows[0]["Height"].ToString();
                ddlLastEdQualification.SelectedValue = dt.Rows[0]["LastEdQualification"].ToString();
                dsMaritialStatus.Text = dt.Rows[0]["MaritialStatus"].ToString();
                dsMotherName.Text = dt.Rows[0]["MotherName"].ToString();
                //dsMotherNameBN.Text = dt.Rows[0]["MotherNameBN"].ToString();
                dsNationality.Text = dt.Rows[0]["Nationality"].ToString();
                dsNationIDCardNo.Text = dt.Rows[0]["NationIDCardNo"].ToString();
                dsNoOfExperience.Text = dt.Rows[0]["NoOfExperience"].ToString();
                dsPlaceOfBirth.Text = dt.Rows[0]["PlaceOfBirth"].ToString();
                txtEmpVisaNo.Text = dt.Rows[0]["EmpVisaNo"].ToString();
                dsSex.Text = dt.Rows[0]["Sex"].ToString();
                dsWeight.Text = dt.Rows[0]["Weight"].ToString();
                // btnSavePersonal.Text = "Update";
                //  dsReligion.SelectedItem.Text = dt.Rows[0]["RName"].ToString();
                dsReligion.SelectedValue = dt.Rows[0]["RId"].ToString();
                // txtNumberofchild.Text = dt.Rows[0]["NumberofChild"].ToString();
                chkAlternativeEmpCard.Checked = false;
                chkAlternativeEmpCard.Enabled = false;
            }
            catch { }
        }
        private void saveEmpAddress()
        {
            try
            {
             SqlCommand cmd = new SqlCommand("Insert into Personnel_EmpAddress (EmpId,MobileNo) values ('"+ ViewState["__EmpId__"].ToString() + "','"+txtMobileNo.Text.Trim()+"')", sqlDB.connection);
                cmd.ExecuteNonQuery();
                Session["_EmpId_"] = ViewState["__EmpId__"].ToString();
            }
            catch (Exception ex)
            {
               
            }
        }
        private void updateEmpAddress(string EmpId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Update Personnel_EmpAddress  set MobileNo='" + txtMobileNo.Text.Trim()+"' where EmpId='"+ EmpId + "' ", sqlDB.connection);
             int result= cmd.ExecuteNonQuery();
                if (result==0)
                {
                    SqlCommand cmd1 = new SqlCommand("Insert into Personnel_EmpAddress (EmpId,MobileNo) values ('" + EmpId + "','" + txtMobileNo.Text.Trim() + "')", sqlDB.connection);
                    cmd1.ExecuteNonQuery();
                }


            }
            catch (Exception ex)
            {

            }
        }
        private Boolean saveEmpPersonnal()   //............For Save.........
        {
            try
            {
                System.Data.SqlTypes.SqlDateTime getDate;
                getDate = SqlDateTime.Null;
                //DataTable dtEmp;
                //sqlDB.fillDataTable("SELECT EmpId FROM v_HRD_Shift", dtEmp = new DataTable());
                string EmpId = ViewState["__EmpId__"].ToString();
                SqlCommand cmd = new SqlCommand("Insert into  Personnel_EmpPersonnal (EmpId, FatherName, MotherName, MaritialStatus, DateOfBirth,Age, PlaceOfBirth, Height, Weight, BloodGroup, Sex, RId, LastEdQualification, NoOfExperience, Nationality, NationIDCardNo,EmpVisaNo)  values (@EmpId, @FatherName, @MotherName, @MaritialStatus, @DateOfBirth,@Age, @PlaceOfBirth, @Height, @Weight, @BloodGroup, @Sex, @RId, @LastEdQualification, @NoOfExperience, @Nationality, @NationIDCardNo,@EmpVisaNo) ", sqlDB.connection);

                cmd.Parameters.AddWithValue("@EmpId", ViewState["__EmpId__"].ToString());
                cmd.Parameters.AddWithValue("@FatherName", dsFatherName.Text.Trim());
                cmd.Parameters.AddWithValue("@MotherName", dsMotherName.Text.Trim());
                //cmd.Parameters.AddWithValue("@FatherNameBn", dsFatherNameBn.Text.Trim());
                // cmd.Parameters.AddWithValue("@MotherNameBN", dsMotherNameBN.Text.Trim());
                cmd.Parameters.AddWithValue("@MaritialStatus", dsMaritialStatus.Text.Trim());
                if (dsDateOfBirth.Text.Length == 0)
                {
                    cmd.Parameters.AddWithValue("@DateOfBirth", getDate);
                    cmd.Parameters.AddWithValue("@Age", "");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DateOfBirth", convertDateTime.getCertainCulture(dsDateOfBirth.Text.Trim()));
                    DateTime dof = convertDateTime.getCertainCulture(dsDateOfBirth.Text.Trim());
                    int Year = dof.Year;
                    int Month = dof.Month;
                    int date = dof.Day;
                    TimeSpan TS = DateTime.Now - new DateTime(Year, Month, date);
                    double Years = TS.TotalDays / 365.25;
                    int Age = Convert.ToInt32(Years);
                    cmd.Parameters.AddWithValue("@Age", Age);
                }
                cmd.Parameters.AddWithValue("@PlaceOfBirth", dsPlaceOfBirth.Text.Trim());
                cmd.Parameters.AddWithValue("@Height", dsHeight.Text.Trim());
                cmd.Parameters.AddWithValue("@Weight", dsWeight.Text);
                cmd.Parameters.AddWithValue("@BloodGroup", dsBloodGroup.Text.Trim());
                cmd.Parameters.AddWithValue("@Sex", dsSex.Text.Trim());
                cmd.Parameters.AddWithValue("@RId", dsReligion.SelectedValue);
                cmd.Parameters.AddWithValue("@LastEdQualification", ddlLastEdQualification.SelectedValue);
                cmd.Parameters.AddWithValue("@NoOfExperience", dsNoOfExperience.Text.Trim());
                cmd.Parameters.AddWithValue("@Nationality", dsNationality.Text.Trim());
                cmd.Parameters.AddWithValue("@NationIDCardNo", dsNationIDCardNo.Text.Trim());
                cmd.Parameters.AddWithValue("@EmpVisaNo", txtEmpVisaNo.Text.Trim());
                // cmd.Parameters.AddWithValue("@NumberofChild", txtNumberofchild.Text);

                int result = (int)cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    SqlCommand cmd2 = new SqlCommand("Insert Into HRD_ManpowerStatus(EmpId,Male,Female) values(@EmpId,@Male,@Female)", sqlDB.connection);
                    cmd2.Parameters.AddWithValue("@EmpId", Session["_EmpId_"].ToString());
                    if (dsSex.Text.Trim() == "Male")
                    {
                        cmd2.Parameters.AddWithValue("@Male", 1);
                        cmd2.Parameters.AddWithValue("@Female", 0);
                    }
                    else
                    {
                        cmd2.Parameters.AddWithValue("@Male", 0);
                        cmd2.Parameters.AddWithValue("@Female", 1);
                    }
                    cmd2.ExecuteNonQuery();
                    //lblMessage.InnerText = "success->Successfully saved";
                }
                else lblMessage.InnerText = "error->Unable to save";

                return true;

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
                return false;
            }
        }
        private Boolean SaveValidation()
        {
            try
            {

                DataTable dt = new DataTable();
                sqlDB.fillDataTable("SELECT  STId,FORMAT(TFromDate,'dd-MM-yyyy') as TFromDate,FORMAT(TToDate,'dd-MM-yyyy') as TToDate FROM ShiftTransferInfo WHERE STId=(SELECT MAX(STId) as STId FROM ShiftTransferInfo WHERE SftId='" + ddlShift.SelectedValue + "')", dt);
                if (dt.Rows.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "MsgShift();", true);
                    return false;
                }
                if (convertDateTime.getCertainCulture(dt.Rows[0]["TToDate"].ToString()) < convertDateTime.getCertainCulture(txtJoiningDate.Text.Trim()))
                {
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "MsgShiftDate();", true);
                    return false;
                }
                ViewState["__STId__"] = dt.Rows[0]["STId"].ToString();
                ViewState["__TFromDate__"] = dt.Rows[0]["TFromDate"].ToString();
                ViewState["__TToDate__"] = dt.Rows[0]["TToDate"].ToString();

                return true;
            }
            catch { return false; }
        }
        private Boolean SaveCardValidation()
        {
            try
            {

                DataTable dt = new DataTable();
                //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Master Admin"))
                //{
                if (chkAlternativeEmpCard.Checked)
                {
                    int StartIndex = (ViewState["__CardNoType__"].ToString().Equals("True")) ? ViewState["__DptCode__"].ToString().Length : ViewState["__FlatCode__"].ToString().Length;
                    string CODE = (ViewState["__CardNoType__"].ToString().Equals("True")) ? ViewState["__DptCode__"].ToString() : ViewState["__FlatCode__"].ToString();
                    StartIndex = 8 + StartIndex;
                    sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where SubString(EmpCardNo,8,15)='" + CODE+txtAlternativeCard.Text + "'  and CompanyId='" + ddlBranch.SelectedValue + "'", dt);
                }
                else sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where EmpCardNo='" + txtEmpCardNo.Text + "'  and CompanyId='" + ddlBranch.SelectedValue + "'", dt);
                //}
                //else
                //{
                //    if (chkAlternativeEmpCard.Checked)
                //    {
                //        sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where SubString(EmpCardNo,8,15)='" + txtAlternativeCard.Text + "' and EmpTypeId=" + rblEmpType.SelectedValue + " and CompanyId='" + ViewState["__CompanyId__"].ToString() + "' ", dt);
                //    }
                //    sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where EmpCardNo='" + txtEmpCardNo.Text + "' and EmpTypeId=" + rblEmpType.SelectedValue + " and CompanyId='" + ViewState["__CompanyId__"].ToString() + "' ", dt);
                //}
                if (dt.Rows.Count > 0)
                {
                    //lblMessage.InnerText = "warning->This Employee Card is used by another person";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "CardValidate();", true);
                    return false;
                }
                if (txtProximityNo.Text.Trim() != "") 
                {
                    dt = new DataTable();
                    sqlDB.fillDataTable("Select EmpId From Personnel_EmployeeInfo where RealProximityNo='"+txtProximityNo.Text+ "'  and CompanyId='" + ddlBranch.SelectedValue + "'", dt);
                    if (dt.Rows.Count > 0)
                    {                       
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ProximityNoValidate();", true);
                        return false;
                    }
                }

                return true;
            }
            catch { return false; }
        }
        private Boolean UpdateCardValidation()
        {
            try
            {

                DataTable dt = new DataTable();
                if (chkAlternativeEmpCard.Checked)
                {
                    int StartIndex = (ViewState["__CardNoType__"].ToString().Equals("True")) ? ViewState["__DptCode__"].ToString().Length : ViewState["__FlatCode__"].ToString().Length;
                    string CODE = (ViewState["__CardNoType__"].ToString().Equals("True")) ? ViewState["__DptCode__"].ToString() : ViewState["__FlatCode__"].ToString();
                    StartIndex = 8 + StartIndex;
                    sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where SubString(EmpCardNo,8,15)='" + CODE + txtAlternativeCard.Text + "' and EmpId !='" + ddlEmpCardNo.SelectedValue + "'  and CompanyId='" + ddlBranch.SelectedValue + "'", dt);
                }
               

                else sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where EmpCardNo='" + txtEmpCardNo.Text + "' and EmpId !='" + ddlEmpCardNo.SelectedValue + "'  and CompanyId='" + ddlBranch.SelectedValue + "'", dt);

                string query = "Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where EmpCardNo='" + txtEmpCardNo.Text + "' and EmpId !='" + ddlEmpCardNo.SelectedValue + "'  and CompanyId='" + ddlBranch.SelectedValue + "'";
                //if (chkAlternativeEmpCard.Checked)
                //{
                //    sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where EmpCardNo='" + txtAlternativeCard.Text + "' and EmpId !='" + ddlEmpCardNo.SelectedValue + "' and CompanyId='" + ddlBranch.SelectedValue + "'", dt);
                //}
                //else sqlDB.fillDataTable("Select SubString(EmpCardNo,8,15) as EmpCardNo From Personnel_EmployeeInfo where EmpCardNo='" + txtEmpCardNo.Text + "' and EmpId !='" + ddlEmpCardNo.SelectedValue + "'  and CompanyId='" + ddlBranch.SelectedValue + "'", dt);

                if (dt.Rows.Count > 0)
                {
                    //lblMessage.InnerText = "warning->This Employee Card is used by another person";
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "CardValidate();", true);
                    return false;
                }
                if (rblPunchType.SelectedValue=="1")
                {
                    dt = new DataTable();
                    sqlDB.fillDataTable("Select EmpId From Personnel_EmployeeInfo where RealProximityNo='" + txtProximityNo.Text + "' and EmpId !='" + ddlEmpCardNo.SelectedValue + "'  and CompanyId='" + ddlBranch.SelectedValue + "'", dt);
                    if (dt.Rows.Count > 0)
                    {
                     //   lblMessage.InnerText = "warning->This Proximity No is used by another person";
                      ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "ProximityNoValidate();", true);
                        return false;
                    }
                }
                return true;
            }
            catch { return false; }
        }
        private void saveShiftTransferDetails(string EmpId)
        {
            try
            {
                string[] FromDates = txtJoiningDate.Text.Split('-');
                string[] ToDates = ViewState["__TToDate__"].ToString().Split('-');
                DateTime FromDate = new DateTime(int.Parse(FromDates[2]), int.Parse(FromDates[1]), int.Parse(FromDates[0]));
                DateTime ToDate = new DateTime(int.Parse(ToDates[2]), int.Parse(ToDates[1]), int.Parse(ToDates[0]));
                while (FromDate <= ToDate)
                {
                    FromDates = FromDate.ToString().Split('/');

                    string[] getColumns = { "STId", "SDate", "EmpId", "DptId", "DsgId", "EmpTypeId", "CompanyId" };
                    string[] getValues = { ViewState["__STId__"].ToString(),convertDateTime.getCertainCulture(FromDates[1]+"-"+FromDates[0]+"-"+FromDates[2].Substring(0,4)).ToString(),EmpId,
                                         ddlDepartment.SelectedValue,ddlDesingnation.SelectedValue,rblEmpType.SelectedValue, ddlBranch.SelectedValue};
                    SQLOperation.forSaveValue("ShiftTransferInfoDetails", getColumns, getValues, sqlDB.connection);
                    FromDate = FromDate.AddDays(1);

                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }

        private Boolean updateEmpPersonnal(string EmpId) //............For Update Personal........
        {
            try
            {
                if (ViewState["__IsSave__"].ToString().Equals("Yes"))
                {
                    ViewState["__EmpId__"] = EmpId;
                    if (saveEmpPersonnal())
                        return true;
                    else return false;
                }                    
                System.Data.SqlTypes.SqlDateTime getDate;
                getDate = SqlDateTime.Null;
                if (EmpId == null)
                {
                    EmpId = ddlEmpCardNo.SelectedValue;
                }
                SqlCommand cmd = new SqlCommand(" update Personnel_EmpPersonnal  Set FatherName=@FatherName, MotherName=@MotherName,  MaritialStatus=@MaritialStatus, DateOfBirth=@DateOfBirth,Age=@Age, PlaceOfBirth=@PlaceOfBirth, Height=@Height, Weight=@Weight, BloodGroup=@BloodGroup, Sex=@Sex, RId=@RId, LastEdQualification=@LastEdQualification, NoOfExperience=@NoOfExperience, Nationality=@Nationality, NationIDCardNo=@NationIDCardNo,EmpVisaNo=@EmpVisaNo where EmpId=@EmpId ", sqlDB.connection);
                cmd.Parameters.AddWithValue("@EmpId", EmpId);
                cmd.Parameters.AddWithValue("@FatherName", dsFatherName.Text.Trim());
                cmd.Parameters.AddWithValue("@MotherName", dsMotherName.Text.Trim());
                //cmd.Parameters.AddWithValue("@FatherNameBn", dsFatherNameBn.Text.Trim());
                // cmd.Parameters.AddWithValue("@MotherNameBN", dsMotherNameBN.Text.Trim());
                cmd.Parameters.AddWithValue("@MaritialStatus", dsMaritialStatus.Text.Trim());
                if (dsDateOfBirth.Text.Length == 0)
                {
                    cmd.Parameters.AddWithValue("@DateOfBirth", getDate);
                    cmd.Parameters.AddWithValue("@Age", "");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DateOfBirth", convertDateTime.getCertainCulture(dsDateOfBirth.Text.Trim()));
                    DateTime dof = convertDateTime.getCertainCulture(dsDateOfBirth.Text.Trim());
                    int Year = dof.Year;
                    int Month = dof.Month;
                    int date = dof.Day;
                    TimeSpan TS = DateTime.Now - new DateTime(Year, Month, date);
                    double Years = TS.TotalDays / 365.25;
                    int Age = Convert.ToInt32(Years);
                    cmd.Parameters.AddWithValue("@Age", Age);
                }
                cmd.Parameters.AddWithValue("@PlaceOfBirth", dsPlaceOfBirth.Text.Trim());
                cmd.Parameters.AddWithValue("@Height", dsHeight.Text.Trim());
                if (dsWeight.Text.Length == 0)
                {
                    cmd.Parameters.AddWithValue("@Weight", 0);
                }
                else cmd.Parameters.AddWithValue("@Weight", dsWeight.Text);
                cmd.Parameters.AddWithValue("@BloodGroup", dsBloodGroup.Text.Trim());
                cmd.Parameters.AddWithValue("@Sex", dsSex.Text.Trim());
                cmd.Parameters.AddWithValue("@RId", dsReligion.SelectedValue);
                cmd.Parameters.AddWithValue("@LastEdQualification", ddlLastEdQualification.SelectedValue);
                if (dsNoOfExperience.Text.Length == 0)
                {
                    cmd.Parameters.AddWithValue("@NoOfExperience", 0);
                }
                else cmd.Parameters.AddWithValue("@NoOfExperience", dsNoOfExperience.Text.Trim());
                cmd.Parameters.AddWithValue("@Nationality", dsNationality.Text.Trim());
                cmd.Parameters.AddWithValue("@NationIDCardNo", dsNationIDCardNo.Text.Trim());
                cmd.Parameters.AddWithValue("@EmpVisaNo", txtEmpVisaNo.Text.Trim());
                // cmd.Parameters.AddWithValue("@NumberofChild", txtNumberofchild.Text.Trim());

                int result = (int)cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    SqlCommand cmd2 = new SqlCommand("Update HRD_ManpowerStatus set Male=@Male,Female=@Female where EmpId='" + EmpId + "'", sqlDB.connection);

                    if (dsSex.Text.Trim() == "Male")
                    {
                        cmd2.Parameters.AddWithValue("@Male", 1);
                        cmd2.Parameters.AddWithValue("@Female", 0);
                    }
                    else
                    {
                        cmd2.Parameters.AddWithValue("@Male", 0);
                        cmd2.Parameters.AddWithValue("@Female", 1);
                    }
                    cmd2.ExecuteNonQuery();
                }

                return true;

            }
            catch (Exception ex)
            {
                lblMessage.InnerText = "error->" + ex.Message;
                return false;
            }
        }
        //----------------------------------------------------------------End Employee Personal------------------------------------------------------------
        protected void btnPersonalInfo_Click(object sender, EventArgs e)
        {
            Session["_EmpStatus_"] = "";
            Session["_EmpId_"] = ddlEmpCardNo.SelectedValue;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTab('/personnel/EmployeePersonal.aspx');", true);  //Open New Tab for Sever side code

        }

        protected void cskFlatOrder_CheckedChanged(object sender, EventArgs e)
        {
            if (cskFlatOrder.Checked == true)
                cskDptWise.Checked = false;
            else cskDptWise.Checked = true;
        }

        protected void cskDptWise_CheckedChanged(object sender, EventArgs e)
        {
            if (cskDptWise.Checked == true)
                cskFlatOrder.Checked = false;
            else cskFlatOrder.Checked = true; 
        }

        private void FlatCustomOrdering() 
        {
             DataTable dt = new DataTable();
            CompanyId=(ddlBranch.SelectedValue.Equals("0000"))?"":ddlBranch.SelectedValue;
            sqlDB.fillDataTable("select max(CustomOrdering) as MaxOrderNo from Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "'", dt);
            if (!dt.Rows[0]["MaxOrderNo"].ToString().Equals(""))
                    {
                        txtFlatOrder.Text = (int.Parse(dt.Rows[0]["MaxOrderNo"].ToString()) + 1).ToString();
                    }
                    else { txtFlatOrder.Text = "1"; }
        }

        private void DepartmentCustomOrdering()
        {
            DataTable dt = new DataTable();
            CompanyId = (ddlBranch.SelectedValue.Equals("0000")) ? "" : ddlBranch.SelectedValue;
            sqlDB.fillDataTable("select max(CustomOrdering) as MaxOrderNo from Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' and DptId='" + ViewState["__DptId__"].ToString()+ "'", dt);
            if (!dt.Rows[0]["MaxOrderNo"].ToString().Equals(""))
            {
             txtDptWise.Text = (int.Parse(dt.Rows[0]["MaxOrderNo"].ToString()) + 1).ToString();
            }
            else { txtDptWise.Text = "1"; }
        }

        protected void rblPunchType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rblPunchType.SelectedValue == "0")
            {
                txtProximityNo.Text = "";
                txtProximityNo.Enabled = false;
            }
            else 
            {               
                txtProximityNo.Enabled = true;
                txtProximityNo.Focus();
            }
        }

        protected void ckbProximityChange_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbProximityChange.Checked)
            {
                trProximityChangeDate.Visible = true;
                txtProximityChangeDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
            }
                
            else
                trProximityChangeDate.Visible = false;
           
        }

        //---- Data Import--------
        public string fileUpload(FileUpload FileUploader, string CompanyId)
        {
            try
            {
                string filename = "Employees.xlsx";
                if (FileUploader.HasFile == true)
                {                   
                    File.Delete(HttpContext.Current.Server.MapPath("~/AccessFile/" + CompanyId + "") + filename);
                    FileUploader.SaveAs(HttpContext.Current.Server.MapPath("~/AccessFile/" + CompanyId + "") + filename);
                }
                return filename;
            }
            catch (Exception ex)
            {
                return "";
            }


        }

        public void ReadXL(string filename,string companyId)
        {
            try
            {
                //string getFilePaht = @"AccessFile\\" + companyId + "" + filename;
                string getFilePaht = HttpContext.Current.Server.MapPath("~/AccessFile/" + companyId + "") + filename;
                WorkBook workbook = WorkBook.Load(getFilePaht);
                WorkSheet sheet = workbook.WorkSheets.First();
                foreach (var row in sheet.Rows)
                {
                    string RegID = row.Columns[0].ToString();
                    if (RegID.Trim().Length > 0)
                    {
                        try { int a = int.Parse(RegID); } catch (Exception ex) { continue; }
                    }                    
                    string Name = row.Columns[1].ToString();
                    string Department = row.Columns[2].ToString();
                    string Group = Department;
                    string Designation = row.Columns[3].ToString();
                    string EmpType = row.Columns[4].ToString();
                    string JoiningDate = row.Columns[5].ToString();
                    string Gender = row.Columns[6].ToString();
                    AutoGenerateEmpId();
                    if(RegID=="")
                        ViewState["__RegID__"] = ViewState["__txtRegistrationId__"].ToString();
                    else
                        ViewState["__RegID__"] = RegID;
                    ViewState["__Name__"] = Name;

                    ViewState["__DptID__"] = getDptID(Department);
                    ViewState["__DsgID__"] = getDsgID(ViewState["__DptID__"].ToString(), Designation);
                    ViewState["__GID__"] = getGID(ViewState["__DptID__"].ToString(), Group);
                    ViewState["__EmpTypeID__"] = (EmpType.ToLower()=="worker")?"1":"2";
                    ViewState["__Gender__"] = Gender;
                    if (JoiningDate == "")
                        ViewState["__JoiningDate__"] = DateTime.Now.ToString("yyyy-MM-dd");
                    else
                        ViewState["__JoiningDate__"] = commonTask.ddMMyyyyTo_yyyyMMdd(JoiningDate);


                    ImportEmployeeInfo();


                }
            }
            catch (Exception ex)
            {
              
            }


        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if(fuEmployeesData.HasFile)
                ReadXL(fileUpload(fuEmployeesData, ddlBranch.SelectedValue), ddlBranch.SelectedValue);
        }

        private string getDptID(string DptName)
        {

            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select DptId from HRD_Department where DptName='"+DptName+"'");
            if (dt == null || dt.Rows.Count == 0)
            {
                CRUD.Execute("insert into HRD_Department values('" + classes.commonTask.LoadSL("Select Max(Sl) as SL from HRD_Department", "Department") + "','" + ddlBranch.SelectedValue + "','" + DptName + "',N'','',1,0,0) ");
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select DptId from HRD_Department where DptName='" + DptName + "'");
            }            
            return dt.Rows[0]["DptId"].ToString();
        }

        private string getDsgID(string DptId,string DsgName)
        {

            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select DsgId from HRD_Designation where DptId='"+ DptId + "' and  DsgName='"+ DsgName + "'");
            if (dt == null || dt.Rows.Count == 0)
            {
                CRUD.Execute("insert into HRD_Designation(DsgId, DptId, DsgName, DsgStatus, Ordering) values('" + classes.commonTask.LoadSL("Select Max(SL) as SL From HRD_Designation", "Designation") + "', '" + DptId + "','" + DsgName + "',1,0) ");
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select DsgId from HRD_Designation where DptId='" + DptId + "' and  DsgName='" + DsgName + "'");
            }
            return dt.Rows[0]["DsgId"].ToString();
        }
        private string getGID(string DptId, string GName)
        {

            dt = new DataTable();
            dt = CRUD.ExecuteReturnDataTable("select GID from HRD_Group where DptId='" + DptId + "' and  GName='" + GName + "'");
            if (dt == null || dt.Rows.Count == 0)
            {
                CRUD.Execute("insert into HRD_Group(DptId,CompanyId, GName, IsActive) values('" + DptId + "','" + ddlBranch.SelectedValue + "' ,'" + GName + "',1)");
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select GID from HRD_Group where DptId='" + DptId + "' and  GName='" + GName + "'");
            }
            return dt.Rows[0]["GID"].ToString();
        }


        private void loadDepartMents()
        {
            if (Session["__dataAceesLevel__"].ToString() == "4")
            {
                classes.commonTask.SearchDepartment(ddlBranch.SelectedValue, ddlDepartment);
               
            }
            else if (Session["__dataAceesLevel__"].ToString() == "3")
            {
                
                classes.commonTask.SearchDepartment(ddlBranch.SelectedValue, ddlDepartment);
            }
            else if (Session["__dataAceesLevel__"].ToString() == "2")
            {
                classes.commonTask.SearchDepartment(ddlBranch.SelectedValue, ddlDepartment);
               
            }

        }

        private void  loadDepartMentWithCode()
        {
            if (Session["__dataAceesLevel__"].ToString() == "4")
            {
                classes.commonTask.SearchDepartmentWithCode(ddlBranch.SelectedValue, ddlDepartment);

            }
            else if (Session["__dataAceesLevel__"].ToString() == "3")
            {

                classes.commonTask.SearchDepartmentWithCode(ddlBranch.SelectedValue, ddlDepartment);
            }
            else if (Session["__dataAceesLevel__"].ToString() == "2")
            {
                classes.commonTask.SearchDepartmentWithCode(ddlBranch.SelectedValue, ddlDepartment);

            }
        }
        //---- End Data Import----)
    }
}