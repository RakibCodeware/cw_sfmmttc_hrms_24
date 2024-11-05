using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.hrms.BLL;

namespace SigmaERP.classes
{
    public class Employee
    {
      public static DataTable dt;
        
        public static void LoadEmployeeType(DropDownList dl)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select EmpTypeId,EmpType From HRD_EmployeeType", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpType";
                dl.DataValueField = "EmpTypeId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoWithName(DropDownList dl,string EmpType)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where EmpTypeId=" + EmpType + " and EmpStatus in ('1','8') Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
      
        public static void LoadEmpCardNoWithNameByCompany(DropDownList dl, string CompanyId, string EmpType,string txt)
        {
            try
            {
                string condition = AccessControl.loadEmpCardNumber(CompanyId);
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where " + condition + " and EmpTypeId in(" + EmpType + ") and EmpStatus in ('1','8')  Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(txt, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoWithNameByCompanyForPf(DropDownList dl, string CompanyId)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "'  and EmpStatus in ('1','8') and PfMember=1  Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("All", "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoWithNameByCompanyRShift(DropDownList dl, string CompanyId)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' AND EmpStatus in ('1','8') Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void LoadEmpCardNoWithNameByCompanyRShift(DropDownList dl, string CompanyId,string ShiftId)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where CompanyId='" +CompanyId+ "' AND SftId in ("+ShiftId+") AND EmpStatus in ('1','8') Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void LoadEmpCardNoWithNameByCompanyRShiftForSeperationEmp(DropDownList dl, string CompanyId,string SeperationMonthYear)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpSeparation where CompanyId='" + CompanyId + "'  AND EmpStatus not in ('1','8') AND YearMonth='"+SeperationMonthYear+"' Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void LoadEmpCardNo(DropDownList dl, string EmpType, string CompanyId)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where EmpTypeId=" + EmpType + " and EmpStatus in ('1','8') and CompanyId='"+CompanyId+"' Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNo(DropDownList dl, string EmpType)// For_DeltaAtt
        {
            try
            {
                dt = new DataTable();
               sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where EmpTypeId=" + EmpType + " and EmpStatus in ('1','8')  Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNo(DropDownList dl, string EmpType, string CompanyId, string NewCardNo)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where EmpTypeId=" + EmpType + " and EmpStatus in ('1','8') and CompanyId='" + CompanyId + "' Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(NewCardNo, "0"));
            }
            catch { }
        }

        public static void LoadEmpCardNoForPayroll(DropDownList dl, string CompanyId)// For payroll Entry
        {
            try
            {
                string condition = AccessControl.loadEmpCardNumber(CompanyId);
                dt = new DataTable();
                sqlDB.fillDataTable("Select  SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where IsActive=1 and " + condition + " and EmpStatus in ('1','8')  order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "SN";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("---Select One---", "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoForPayroll_EmpID(DropDownList dl, string CompanyId)// For payroll Entry
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where IsActive=1 and CompanyId=" + CompanyId + " and EmpStatus in ('1','8')  order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoForPayroll_EmpID(DropDownList dl, string CompanyId,string EmpId)// For payroll Entry
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where IsActive=1 and CompanyId=" + CompanyId + " and EmpId='"+EmpId+"'  order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoForPayroll_Compliance(DropDownList dl, string CompanyId)// For payroll Entry
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select  SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus1 where IsActive=1 and CompanyId=" + CompanyId + " and EmpStatus in ('1','8')  order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "SN";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoForPayrollCompliance(DropDownList dl, string CompanyId)// For payroll Entry
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus1 where CompanyId=" + CompanyId + " and EmpStatus in ('1','8')  Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "SN";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoForPayrollwithEmpId(DropDownList dl, string CompanyId)// For payroll Entry
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select EmpId, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo From Personnel_EmployeeInfo where CompanyId=" + CompanyId + " and EmpStatus in ('1','8')   order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void LoadEmpCardNoWithName(DropDownList dl, string EmpType, string InstallmentType)
        {
            try
            {
                dt = new DataTable();
                if (InstallmentType.Equals("1st Installment")) sqlDB.fillDataTable("Select Distinct (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Leave_LeaveApplication where ShortName='M/L' AND IsProcessessed='1' AND EmpTypeId=" + EmpType + " AND (FirstInstallmentSignature='false' OR FirstInstallmentSignature is null)  order by EmpCardNo ", dt);
                else sqlDB.fillDataTable("Select Distinct (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Leave_LeaveApplication where ShortName='M/L' AND IsProcessessed='1' AND EmpTypeId=" + EmpType + " AND FirstInstallmentSignature='true' AND (SecondInstallmentSignature='false' OR SecondInstallmentSignature is null)  order by EmpCardNo ", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void LoadEmpCardNoWithNameAll(DropDownList dl, string EmpType)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,EmpId From v_Personnel_EmpCurrentStatus where EmpTypeId=" + EmpType + "  Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

       

        public static void loadEmpPunismentType(DropDownList dl)
        {
            try
            {
                SQLOperation.selectBySetCommandInDatatable("select PtId,PtName from HRD_PunishmentType ",dt=new DataTable (),sqlDB.connection);
                dl.DataSource = dt;
                dl.DataValueField = "PtId";
                dl.DataTextField = "PtName";
                dl.DataBind();
              
            }
            catch { }
        }
        public static void LoadResignedEmpCardNo(DropDownList dl,string EmpType)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, EmpId,EmpCardNo From Personnel_EmpCurrentStatus where EmpTypeId="+EmpType+" and EmpStatus in(4,7) Group by EmpId,EmpCardNo Order by EmpCardNo ", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpStatus(DropDownList dl)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select EmpStatus,EmpStatusName From HRD_EmpStatus order by EmpStatus", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpStatusName";
                dl.DataValueField = "EmpStatus";
                dl.DataBind();
               
            }
            catch { }
        }
        public static void LoadEmpCardIncPro(DropDownList dl, string TypeOfChange, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadEmpCardNumber(CompanyId);
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),SubString(EmpCardNo,8,16))+' '+EmpName) as EmpCardNo,EmpId From v_Promotion_Increment where TypeOfChange='" + TypeOfChange + "' and "+ condition + "  Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                if (dt.Rows.Count > 1)
                {
                    dl.Items.Insert(0, new ListItem("All", "0"));
                }
            }
            catch { }
        }
        public static void LoadEmpCardIncProCompliance(DropDownList dl, string TypeOfChange, string CompanyId)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Max(SN) as SN, (Convert(nvarchar(50),SubString(EmpCardNo,8,16))+' '+EmpName) as EmpCardNo,EmpId From v_Promotion_Increment1 where TypeOfChange='" + TypeOfChange + "' and CompanyId='" + CompanyId + "'  Group by EmpCardNo,EmpId,EmpName order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                if (dt.Rows.Count > 1)
                {
                    dl.Items.Insert(0, new ListItem("All", "0"));
                }
            }
            catch { }
        }
        public static void LoadEmpCardNo_ForSeperation(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadEmpCardNumber(CompanyId);

                dt = new DataTable();
                sqlDB.fillDataTable("Select  (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,(EmpId+'|'+(Convert(nvarchar(50),EmpCardNo))+'|'+convert(varchar(2),EmpTypeId)) as EmpId From v_Personnel_EmpCurrentStatus where EmpStatus in ('1','8') and "+ condition + " and EmpId not in (select EmpId from Personnel_EmpSeparation where  CompanyId='"+ CompanyId + "' and IsActive=0) and IsActive=1 order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNo_ForSeperationCompliance(DropDownList dl, string CompanyId)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select  (Convert(nvarchar(50),EmpCardNo)+' '+EmpName) as EmpCardNo,(EmpId+'|'+(Convert(nvarchar(50),EmpCardNo))+'|'+convert(varchar(2),EmpTypeId)) as EmpId From v_Personnel_EmpCurrentStatus1 where EmpStatus in ('1','8') and CompanyId='" + CompanyId + "' and EmpId not in (select EmpId from Personnel_EmpSeparation1 where  CompanyId='" + CompanyId + "' and IsActive=0) and IsActive=1 order by EmpCardNo", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static string getEmpIdByCardNo(string CompanyId,string EmpCardNo)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("select EmpId from Personnel_EmployeeInfo where CompanyId='"+ CompanyId + "' and EmpCardNo like'%"+ EmpCardNo.Trim()+ "'", dt);
                return dt.Rows[0]["EmpId"].ToString();
               
            }
            catch { return ""; }
        }
        //public static void getValidEmpCardNo(string )
    }
}