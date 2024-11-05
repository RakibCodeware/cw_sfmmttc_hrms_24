using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;

namespace SigmaERP.attendance
{
    public partial class daily_movement : System.Web.UI.Page
    {
        DataTable dt;
        DataTable dtSetPrivilege ;
        string CompanyId = "";
        string query = "";


        //view=265;
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                int[] pagePermission = { 264 };
                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);
                classes.commonTask.LoadEmpTypeWithAll(rblEmpType);
                txtDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
                setPrivilege();               
              if (!classes.commonTask.HasBranch()) 
                  ddlCompany.Enabled = false;
              ddlCompany.SelectedValue = ViewState["__CompanyId__"].ToString();
              Session["__MinDigits__"] = "6";
              
            }
        }

        private void setPrivilege()
        {
           try
            {
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                //------------load privilege setting inof from db------
                //------------load privilege setting inof from db------
                classes.commonTask.LoadBranch(ddlCompany, ViewState["__CompanyId__"].ToString());
               // string[] AccessPermission = new string[0];
               // AccessPermission = checkUserPrivilege.checkUserPrivilegeForReport(ViewState["__CompanyId__"].ToString(), getUserId, ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()), "daily_movement.aspx", ddlCompany, WarningMessage, tblGenerateType, btnPreview);
                //ViewState["__ReadAction__"] = AccessPermission[0];
                classes.commonTask.LoadShiftNameByCompany(ViewState["__CompanyId__"].ToString(), ddlShift);
                classes.commonTask.LoadDepartment(ViewState["__CompanyId__"].ToString(), lstAll);
                //-----------------------------------------------------
               

              

             
            }
            catch { }
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (cbOutTimeMissingReport.Checked == true)
                GenerateOutTimeMissingReport();
            else
            {
                if (rblLanguage.SelectedValue == "EN")
                    GenerateReportInEnglish();
                else
                    GenerateReportInBangla();
            }
          
        }

        private void GenerateOutTimeMissingReport()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            if (classes.commonTask.IsWeekendORHoliday(y + "-" + m + "-" + d))
            {
                if (rblAttStatus.SelectedValue == "P")

                    AttStatus = " and InHour<>'00' ";
                else if (rblAttStatus.SelectedValue == "A")
                    AttStatus = " and InHour='00'  ";
                else if (rblAttStatus.SelectedValue == "Lv")
                    AttStatus = " and AttStatus='Lv'  ";
                else
                    AttStatus = "";
            }

            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();


            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";



            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            if (txtCardNo.Text.Trim().Length == 0) sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,10,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,ATTStatus,CompanyId,DptId,SftId,GId,GName From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and CompanyId " + CompanyList + "  ANd InHour<>'00' and OutHour='00'  AND DptId " + DepartmentList + " " + EmpTypeID + " " + AttStatus + " " + ShiftName + "  order by convert(int,DptCode),convert(int,GId), convert(int,SftId),CustomOrdering ", dt = new DataTable());
            else
            {
                bool hasEmpCard = AccessControl.hasEmpcardPermission(txtCardNo.Text.Trim(), CompanyList);
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                if (!hasEmpCard)
                {
                    lblMessage.InnerText = "warning-> You have no permission on this Employee";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,10,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,ATTStatus,CompanyId,DptId,SftId,GId,GName From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and InHour<>'00' and OutHour='00' And ActiveSalary='True' and IsActive=1 and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and CompanyId " + CompanyList + " " + AttStatus + " ", dt = new DataTable());
            }

            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyMovement__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovement-" + txtDate.Text + "-" + rblPrintType.SelectedValue + "- Daily Out Time Missing Report');", true);  //Open New Tab for Sever side code

        }
        private void GenerateReportInEnglish() 
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string[] dmy = txtDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];
            string AttStatus = "";

            //if (classes.commonTask.IsWeekendORHoliday(y + "-" + m + "-" + d))
            //{
            //    if (rblAttStatus.SelectedValue == "not_payable")
            //    {
            //        lblMessage.InnerText = "warning->This criteria ('Not Payalbe') is not applicable for weekend/holiday!";
            //        return;
            //    }
            //    else if (rblAttStatus.SelectedValue == "P")
            //        AttStatus = " and InHour<>'00' ";
            //    else if (rblAttStatus.SelectedValue == "A")
            //        AttStatus = " and InHour='00'  ";
            //    else if (rblAttStatus.SelectedValue == "Lv")
            //        AttStatus = " and AttStatus='Lv'  ";
            //    else
            //        AttStatus = "";
            //}
            //else
            //{
                if (rblAttStatus.SelectedValue == "WH")
                {
                    AttStatus = " and AttStatus  in('H','W')";
                }
                else if (rblAttStatus.SelectedValue == "not_payable")
                    AttStatus = " and AttStatus not in('Lv','H','W') and PaybleDays=0";
                else
                    AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            //}
          
            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();
          

            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";

            

            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            if (txtCardNo.Text.Trim().Length == 0)
                query = "Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,10,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,CompanyId,DptId,SftId,GId,GName From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and CompanyId " + CompanyList + "   AND DptId " + DepartmentList + " " + EmpTypeID + " " + AttStatus + " " + ShiftName + "  order by convert(int,DptCode),convert(int,GId), convert(int,SftId),CustomOrdering ";
                
            else
            {
                if (txtCardNo.Text.Trim().Length <int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                query = "Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,10,15)+' ('+EmpProximityNo+')' as EmpCardNo,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,SftName,Address,case when ODID >0 then ATTStatus+'(OD)' else ATTStatus end as ATTStatus,CompanyId,DptId,SftId,GId,GName From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and CompanyId " + CompanyList + " " + AttStatus + " ";
                
            }
            sqlDB.fillDataTable(query, dt = new DataTable());
            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyMovement__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovement-" + txtDate.Text + "-" + rblPrintType.SelectedValue + "- Daily Attendance Report');", true);  //Open New Tab for Sever side code
        
        }
        private void GenerateReportInBangla()
        {
            if (lstSelected.Items.Count == 0 && txtCardNo.Text.Trim().Length == 0)
            {
                lblMessage.InnerText = "warning-> Please Select Any Department!";
                lstSelected.Focus();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            string ShiftName = (ddlShift.SelectedValue == "0") ? "" : " and SftName='" + ddlShift.SelectedValue + "' ";
            string AttStatus = (rblAttStatus.SelectedValue == "All") ? "" : " and AttStatus='" + rblAttStatus.SelectedValue + "' ";
            string EmpTypeID = (rblEmpType.SelectedValue == "All") ? "" : " and EmpTypeId=" + rblEmpType.SelectedValue + " ";
            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue.ToString();
            string[] dmy = txtDate.Text.Split('-');
            string d = dmy[0]; string m = dmy[1]; string y = dmy[2];

            string CompanyList = "";
            string ShiftList = "";
            string DepartmentList = "";

            if (!Page.IsValid)   // If Java script are desible then 
            {
                lblMessage.InnerText = "erroe->Please Select From Date And To Date"; return;
            }


            CompanyList = "in ('" + CompanyId + "')";

            //if (ddlShiftList.SelectedItem.ToString().Equals("All"))
            //ShiftList = classes.commonTask.getShiftList(ddlShiftList);          
            //else
            //ShiftList = "in ('" + ddlShiftList.SelectedValue.ToString() + "')";

            DepartmentList = classes.commonTask.getDepartmentList(lstSelected);
            if (txtCardNo.Text.Trim().Length == 0) sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,10,15) as EmpCardNo,EmpNameBn EmpName,DsgNameBn DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyNameBangla CompanyName,DptNameBn DptName,SftNameBangla SftName,AddressBangla Address,ATTStatus,CompanyId,DptId,SftId,GId,GName From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and CompanyId " + CompanyList + " " + ShiftName + "  AND DptId " + DepartmentList + " " + EmpTypeID + " " + AttStatus + " order by convert(int,DptCode),convert(int,GId), convert(int,SftId),CustomOrdering ", dt = new DataTable());
            else
            {
                if (txtCardNo.Text.Trim().Length < int.Parse(Session["__MinDigits__"].ToString()))
                {
                    lblMessage.InnerText = "warning-> Please Type Valid Card Number!(Minimum " + Session["__MinDigits__"].ToString() + " Digits)";
                    txtCardNo.Focus();
                    ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                    return;
                }
                sqlDB.fillDataTable("Select Format(ATTDate,'dd-MM-yyyy') as ATTDate,SubString(EmpCardNo,10,15) as EmpCardNo,EmpNameBn EmpName,DsgNameBn DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyNameBangla CompanyName,DptNameBn DptName,SftNameBangla SftName,AddressBangla Address,ATTStatus,CompanyId,DptId,SftId,GId,GName From v_tblAttendanceRecord where ATTDate='" + y + "-" + m + "-" + d + "' and ActiveSalary='True' and IsActive=1 and EmpCardNo Like'%" + txtCardNo.Text.Trim() + "' and CompanyId " + CompanyList + " " + AttStatus + " ", dt = new DataTable());
            }

            if (dt.Rows.Count == 0)
            {
                lblMessage.InnerText = "warning->No Attendance Available";
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
                return;
            }
            Session["__DailyMovementBangla__"] = dt;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "goToNewTabandWindow('/All Report/Report.aspx?for=DailyMovementBangla-" + txtDate.Text + "-" + rblPrintType.SelectedValue + "');", true);  //Open New Tab for Sever side code

        }
        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstAll, lstSelected);
        }

        protected void btnAddAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstAll, lstSelected);
        }

        protected void btnRemoveItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveItem(lstSelected, lstAll);
        }

        protected void btnRemoveAllItem_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);
            classes.commonTask.AddRemoveAll(lstSelected, lstAll);
        }

        private void addAllTextInShift()
        {
        //    if (ddlShiftList.Items.Count > 2)
        //        ddlShiftList.Items.Insert(1, new ListItem("All", "00"));
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "load();", true);

            CompanyId = (ddlCompany.SelectedValue == "0000") ? ViewState["__CompanyId__"].ToString() : ddlCompany.SelectedValue;
            classes.commonTask.LoadShiftNameByCompany(CompanyId, ddlShift);
            classes.commonTask.LoadDepartment(CompanyId, lstAll);
            lstSelected.Items.Clear();
            //classes.commonTask.LoadShift(ddlShiftList, CompanyId , ViewState["__UserType__"].ToString()); 
           // classes.commonTask.LoadShiftByNumber(ddlShiftList, CompanyId, rblShiftNumber.SelectedValue);
           // addAllTextInShift();
            
        }

       

   

    }
}