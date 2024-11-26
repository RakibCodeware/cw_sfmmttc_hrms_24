using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using ComplexScriptingSystem;
using SigmaERP.hrms.BLL;

namespace SigmaERP.classes
{
    public class commonTask
    {
        public static DataTable dt;
        public static SqlDataAdapter da;
        public static string sqlCmd = "";

        public static string ddMMyyyyTo_yyyyMMdd(string date)
        {
            try
            {
                string[] d = date.Split('-');
                date = d[2] + "-" + d[1] + "-" + d[0];
                return date;
            }
            catch { return ""; }
        }       
        public static DataTable getDepartment(string SL)
        {
            try
            {
                sqlCmd = "SELECT SL,CompanyId,DptName, DptNameBn,DptCode, DptStatus,DptId,convert(varchar(10),CourseStartDate,120) as CourseStartDate,convert(varchar(10),CourseEndDate,120) as CourseEndDate,isnull(StipendAmount,0) as StipendAmount FROM HRD_Department where SL=" + SL;
                dt = new DataTable();
                sqlDB.fillDataTable(sqlCmd, dt);
                return dt;
            }
            catch { return null; }
        }
        public static DataTable getDesignation(string SL)
        {
            try
            {
                sqlCmd = "SELECT SL,CompanyId,CompanyName,DptId,DsgId, DptName,DsgName,DsgNameBn, DsgShortName, DsgStatus,Ordering FROM v_HRD_Designation where SL=" + SL;
                dt = new DataTable();
                sqlDB.fillDataTable(sqlCmd, dt);
                return dt;
            }
            catch { return null; }
        }
        public static bool checkJoiningDate(string Empcardno, int DurationDays)
        {
            try {
                da = new SqlDataAdapter("select DATEDIFF(DAY, EmpJoiningDate,  GETDATE()) as Days from Personnel_EmployeeInfo where Empcardno='" + Empcardno + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                if (int.Parse(dt.Rows[0]["Days"].ToString()) > DurationDays)
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }

        public static bool checkJoiningDateForEL(string Empcardno, string FromDate, string UserID)
        {
            try
            {
                if (UserID.Equals("1008") || UserID.Equals("7080"))//  master admin
                    return true;
                da = new SqlDataAdapter("select convert(varchar(10), EmpJoiningDate,120) as EmpJoiningDate from Personnel_EmployeeInfo where Empcardno='" + Empcardno + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                DateTime EmpJoiningDate = DateTime.Parse(dt.Rows[0]["EmpJoiningDate"].ToString());
                if (EmpJoiningDate.AddYears(1) <= DateTime.Parse(FromDate))
                    return true;
                else
                    return false;
            }
            catch { return false; }
        }
        public static bool IsGarments()
        {
            da = new SqlDataAdapter("select IsGarments from Payroll_Office_IsGarments", sqlDB.connection);
            da.Fill(dt = new DataTable());
            if (bool.Parse(dt.Rows[0]["IsGarments"].ToString()))
                return true;
            else
                return false;

        }
        public static void LoadEmpType(RadioButtonList rbl)
        {
            da = new SqlDataAdapter("SELECT EmpType, EmpTypeId FROM HRD_EmployeeType", sqlDB.connection);
            da.Fill(dt = new DataTable());
            for (byte i = 0; i < dt.Rows.Count; i++)
                rbl.Items.Add(new ListItem(dt.Rows[i]["EmpType"].ToString(), dt.Rows[i]["EmpTypeId"].ToString()));
            rbl.Items[0].Selected = true;
        }
        public static void LoadEmpTypeWithAll(RadioButtonList rbl)
        {
            dt = new DataTable();
            dt = SigmaERP.hrms.Data.CRUD.ExecuteReturnDataTable("SELECT EmpType, EmpTypeId FROM HRD_EmployeeType");
            if (dt.Rows.Count > 1)
                rbl.Items.Add(new ListItem("All", "All"));
            for (byte i = 0; i < dt.Rows.Count; i++)
                rbl.Items.Add(new ListItem(dt.Rows[i]["EmpType"].ToString(), dt.Rows[i]["EmpTypeId"].ToString()));
            rbl.Items[0].Selected = true;
        }
        public static void LoadEmpTypeWithAll(RadioButtonList rbl, string a)
        {
            da = new SqlDataAdapter("SELECT EmpType, EmpTypeId FROM HRD_EmployeeType", sqlDB.connection);
            da.Fill(dt = new DataTable());
            if (dt.Rows.Count > 1)
                rbl.Items.Add(new ListItem("All", "0"));
            for (byte i = 0; i < dt.Rows.Count; i++)
                rbl.Items.Add(new ListItem(dt.Rows[i]["EmpType"].ToString(), dt.Rows[i]["EmpTypeId"].ToString()));
            rbl.Items[0].Selected = true;
        }
        public static void LoadShiftNameByCompany(string CompnayId, DropDownList ddl)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompnayId);
                dt = new DataTable();

                sqlDB.fillDataTable("SELECT Distinct SftName from HRD_Shift where " + condition + " and IsActive=1", dt);
                ddl.DataValueField = "SftName";
                ddl.DataTextField = "SftName";
                ddl.DataSource = dt;
                ddl.DataBind();
                if (ddl.Items.Count > 1)
                    ddl.Items.Insert(0, new ListItem("All", "0"));
            }
            catch { }
        }
        public static void LoadEarnleaveList(string CompnayId, DropDownList ddl)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable(@"select distinct convert(varchar(10),StartDate,120)+'/'+convert(varchar(10),EndDate,120) as Value,format(EndDate,'MMM-yyyy')+' [ '+ convert(varchar(10),StartDate,120)+' to '+convert(varchar(10),EndDate,120)+' ] ' as Text ,EndDate,year(EndDate) ,month(EndDate) from Payroll_EarnLeavePaymentSheet where CompanyId='" + CompnayId
                    + "' order by year(EndDate) desc, month(EndDate) desc, EndDate desc", dt);
                ddl.DataValueField = "Value";
                ddl.DataTextField = "Text";
                ddl.DataSource = dt;
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static bool GroupORLineDependency()
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("select * from HRD_SubDepartmentInfo", dt);
            if (dt.Rows[0]["HasSubDepartment"].ToString().Equals("False"))
                return false;
            else return true;
        }
        public static bool HasBranch()
        {
             dt = new DataTable();
            //sqlDB.fillDataTable("select * from HRD_BranchType", dt);
            dt = SigmaERP.hrms.Data.CRUD.ExecuteReturnDataTable("select * from HRD_BranchType");
            if (dt.Rows[0]["HasBranch"].ToString().Equals("False"))
                return false;
            else return true;

          


        }
        public static void addAllTextInShift(DropDownList dl)
        {
            if (dl.Items.Count > 2)
                dl.Items.Insert(1, new ListItem("All", "00"));
        }
        public static void LoadDepartment(string CompnayId, ListBox lst)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompnayId);
                dt = new DataTable();
                //sqlDB.fillDataTable("SELECT DptId, DptName FROM HRD_Department where CompanyId=" + CompnayId + "", dt);
                dt = SigmaERP.hrms.Data.CRUD.ExecuteReturnDataTable("SELECT DptId, DptName FROM HRD_Department where " + condition + "");
                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }

        public static void LoadDepartmentDDL(DropDownList downList,string CompnayId)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompnayId);
                DataTable dt = CRUD.ExecuteReturnDataTable("SELECT DptId, DptName FROM HRD_Department where  " + condition + "");
                downList.DataValueField = "DptId";
                downList.DataTextField = "DptName";
                downList.DataSource = dt;
                downList.DataBind();
                downList.Items.Insert(0, new ListItem("---Select One---", "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoByEmpType(DropDownList dl, string CompanyId, string EmpTypeID)
        {
            try
            {
                string condition = AccessControl.loadEmpCardNumber(CompanyId);
                EmpTypeID = (EmpTypeID == "All") ? "" : "EmpTypeId=" + EmpTypeID + " and";
                dt = new DataTable();
                sqlDB.fillDataTable("Select MAX(SN) as SN,EmpId,EmpCardNo+' [ '+EmpName+' ]' as EmpCardNo From v_EmployeeProfile where " + EmpTypeID + "  EmpStatus in ('1','8') and IsActive='1' and " + condition + " Group by EmpId,EmpCardNo,EmpName,DptCode,CustomOrdering order by DptCode, CustomOrdering", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "SN";
                dl.DataBind();
            }
            catch { }
        }

        public static void LoadEmpCardNoByShift(DropDownList dl, string CompanyId, string SftID)
        {
            try
            {

                dt = new DataTable();
                sqlDB.fillDataTable("Select EmpCardNo,EmpId,EmpCardNo+' [ '+EmpName+' ]' as EmpCardNoAndName From v_Personnel_EmpCurrentStatus where   EmpStatus in ('1','8') and IsActive='1' and CompanyId='" + CompanyId + "' and SftId=" + SftID + " Group by EmpId,EmpCardNo,EmpName,DptCode,CustomOrdering order by DptCode, CustomOrdering", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNoAndName";
                dl.DataValueField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void LoadBranch(DropDownList dl)
        {
            try
            {
                dt = new DataTable();
                dt = SigmaERP.hrms.Data.CRUD.ExecuteReturnDataTable("SELECT CompanyId, CompanyName FROM HRD_CompanyInfo");
                dl.DataValueField = "CompanyId";
                dl.DataTextField = "CompanyName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0000"));  //0000 for developer account type
            }
            catch { }
        }
        public static void LoadAdvanceStatus(DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("select StatusID, Status from HRD_AdvanceStatus where IsActive = 1", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "StatusID";
                dl.DataTextField = "Status";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("All", "00"));  //0000 for developer account type
            }
            catch { }
        }

        public static void LoadBranch(DropDownList dl, string CompanyId)
        {
            try {

                //da = new SqlDataAdapter("SELECT CompanyId, CompanyName FROM HRD_CompanyInfo where CompanyId='" + CompanyId + "'", sqlDB.connection);
                dt = new DataTable();
                dt = SigmaERP.hrms.Data.CRUD.ExecuteReturnDataTable("SELECT CompanyId, CompanyName FROM HRD_CompanyInfo where CompanyId='" + CompanyId + "'");

                //da.Fill(dt = new DataTable());
                dl.DataValueField = "CompanyId";
                dl.DataTextField = "CompanyName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0000"));  //0000 for developer account type
            }
            catch { }
        }

        public static void loadDivision(DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("SELECT DId, DName FROM HRD_Division", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DId";
                dl.DataTextField = "DName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEducationalQui(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select Distinct QId,QName From HRD_Qualification order by QName ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "QName";
                dl.DataValueField = "QId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void loadDivision(DropDownList dl, string CompanyId)
        {
            try
            {
                da = new SqlDataAdapter("SELECT DId, DName FROM HRD_Division where CompanyId='" + CompanyId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DId";
                dl.DataTextField = "DName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void loadDivision(DropDownList dl, string CompanyId, string UserType)
        {
            try
            {
                if (UserType.Equals("Super Admin")) da = new SqlDataAdapter("SELECT DId, DName FROM HRD_Division", sqlDB.connection);
                else da = new SqlDataAdapter("SELECT DId, DName FROM HRD_Division where CompanyId='" + CompanyId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DId";
                dl.DataTextField = "DName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadGrouping(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select GId,GName From HRD_Group where  IsActive='True'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "GId";
                dl.DataTextField = "GName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadSheets(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("select Sheet,SheetTitle from HRD_Sheets where IsActive=1", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "Sheet";
                dl.DataTextField = "SheetTitle";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("--Select--", "0"));
            }
            catch { }
        }

        public static void LoadGrouping(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                sqlDB.fillDataTable("Select GId,GName From HRD_Group where " + condition + " and IsActive='True'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "GId";
                dl.DataTextField = "GName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadGrouping(DropDownList dl, string CompanyId, string DptId)
        {
            try
            {
                sqlDB.fillDataTable("Select GId,GName From HRD_Group where CompanyId='" + CompanyId + "' and DptId='" + DptId + "' and IsActive='True'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "GId";
                dl.DataTextField = "GName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void loadDepartment(DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("SELECT DptId, DptName FROM HRD_Department", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
            }
            catch { }
        }

        public static void loadDepartmentListByCompany(DropDownList dl, string CompanyId)
        {
            try
            {
                string condtion = AccessControl.loadDepartmetCondition(CompanyId);
                da = new SqlDataAdapter("SELECT DptId, DptName FROM HRD_Department where " + condtion + "", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void loadCourseListByCompany(DropDownList dl, string CompanyId)
        {
            try
            {
                string condtion = AccessControl.loadDepartmetCondition(CompanyId);
                da = new SqlDataAdapter(@"SELECT DptId,  DptName + 
       ISNULL(' (' + CONVERT(VARCHAR(10), CourseStartDate, 120) + ' to ' + CONVERT(VARCHAR(10), CourseEndDate, 120) + ')', '') AS CourseName FROM HRD_Department where " + condtion + "", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "CourseName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void loadDepartmentListByCompanyWithAll(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                da = new SqlDataAdapter("SELECT DptId, DptName FROM HRD_Department where " + condition + "", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("All", "0"));
            }
            catch { }
        }
        public static void loadLeaveNameByCompany(DropDownList dl, string CompanyId)
        {
            try
            {
                da = new SqlDataAdapter("SELECT LeaveId, LeaveName+' '+shortName as LeaveName  from tblLeaveConfig where ShortName <>'sr/l' and CompanyId='" + CompanyId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "LeaveId";
                dl.DataTextField = "LeaveName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadDepartmentListByCompanyAndShift(DropDownList dl, string CompanyId, string ShiftId)
        {
            try
            {
                da = new SqlDataAdapter("SELECT Distinct DptId, DptName FROM v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' AND SftId=" + ShiftId + "", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadMonthId(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("select distinct (convert (varchar(7),Month)+'-'+Year) as MonthId from v_MonthlySalarySheet", dt = new DataTable());
                dl.DataValueField = "MonthId";
                dl.DataTextField = "MonthId";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(" ", " "));
            }
            catch { }
        }
        public static void LoadMonthName(string CompanyId, DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select MonthID,MonthName From tblMonthSetup where CompanyId='" + CompanyId + "' order by FromDate Desc ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "MonthName";
                dl.DataValueField = "MonthID";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void loadEmpTypeInRadioButtonList(RadioButtonList rbl)
        {
            sqlDB.fillDataTable("select EmpType,EmpTypeId from HRD_EmployeeType", dt = new DataTable());
            rbl.DataValueField = "EmpTypeId";
            rbl.DataTextField = "EmpType";
            rbl.DataSource = dt;
            rbl.DataBind();

        }


        public static void loadLeaveType(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("select LeaveNameID,LeaveName from Leave_LeaveType where IsActive=1 order by [Order] ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "LeaveNameID";
                dl.DataTextField = "LeaveName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadLeaveTypes(DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("Select LeaveId, LeaveName as LeaveName from tblLeaveConfig", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "LeaveId";
                dl.DataTextField = "LeaveName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(" ", "50"));
            }
            catch { }
        }

        public static void loadSeparationType(DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("Select EmpStatus, EmpStatusName from HRD_EmpStatus where EmpStatus in ('3','4','5','6','7')", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "EmpStatus";
                dl.DataTextField = "EmpStatusName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(" ", "50"));
            }
            catch { }
        }

        public static void loadEmpTypeInRadioButtonList(RadioButtonList rbl, string hasMnu)
        {
            sqlDB.fillDataTable("select EmpType,EmpTypeId from HRD_EmployeeType", dt = new DataTable());
            rbl.DataValueField = "EmpTypeId";
            rbl.DataTextField = "EmpType";
            rbl.DataSource = dt;
            rbl.DataBind();
            rbl.Items.Insert(0, new ListItem("Indivdual", "50"));
        }

        public static void SearchDepartment(string companyId, DropDownList dl)
        {
            try
            {
                string conditon = AccessControl.loadDepartmetCondition(companyId);
 ;
                da = new SqlDataAdapter("SELECT DptId, DptName FROM HRD_Department where "+ conditon + "", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void SearchDepartmentWithCode(string companyId, DropDownList dl)
        {
            try
            {
                string conditon = AccessControl.loadDepartmetCondition(companyId);
                da = new SqlDataAdapter("SELECT DptId+'_'+DptCode as DptId,DptCode, DptName FROM HRD_Department " + conditon + " order by convert(int,DptCode)", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void SearchDepartmentWithCode(string companyId, string DptId, DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("SELECT DptId+'_'+DptCode as DptId,DptCode, DptName FROM HRD_Department where CompanyId=" + companyId + " and DptId='" + DptId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
            }
            catch { }
        }
        public static void LoadLine(string DptId, DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select LnId,LnCode From HRD_Line where LnStatus='True' and DptId='" + DptId + "'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "LnId";
                dl.DataTextField = "LnCode";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadFloor(string DptId, DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select FId,FCode From HRD_Floor where FStatus='True' and DptId='" + DptId + "'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "FId";
                dl.DataTextField = "FCode";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadGroup(string DptId, DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select GrpId,GrpName From HRD_GroupInfo where GrpStatus='True' and DptId='" + DptId + "'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "GrpId";
                dl.DataTextField = "GrpName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadDesignation(string DptId, DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select DsgId,DsgName From HRD_Designation where DsgStatus='True' and DptId='" + DptId + "'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "DsgId";
                dl.DataTextField = "DsgName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void SearchDesignation(string DptId, DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("SELECT DsgId, DsgName FROM HRD_Designation where DptId='" + DptId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DsgId";
                dl.DataTextField = "DsgName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void SearchLine(string DptId, DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("SELECT LnId, LnCode FROM HRD_Line where DptId='" + DptId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "LnId";
                dl.DataTextField = "LnCode";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void SearchFloore(string DptId, DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("SELECT FId, FCode FROM HRD_Floor where DptId='" + DptId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "FId";
                dl.DataTextField = "FCode";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void SearchGroup(string DptId, DropDownList dl)
        {
            try
            {
                da = new SqlDataAdapter("SELECT GrpId, GrpName FROM HRD_GroupInfo where DptId='" + DptId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "GrpId";
                dl.DataTextField = "GrpName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }


        public static void LoadShift(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadShiftByNumber(DropDownList dl, string CompanyId, string Number)
        {
            try
            {
                Number = (Number == "00") ? "" : " Top(" + Number + ") ";
                sqlDB.fillDataTable("Select " + Number + " SftId,SftName From HRD_Shift where companyId='" + CompanyId + "' AND IsActive='true' and IsInitial=0 order by SftId  desc", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadShift(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where " + condition + " AND IsActive='true'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadShiftOnlyIndependent(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where companyId='" + CompanyId + "' AND IsActive='true' and DptID='0'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void LoadInitialShift(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                sqlDB.fillDataTable("select SftId,SftName from HRD_Shift where IsActive=1 and IsInitial=1 and " + condition + "", dt = new DataTable());
                string condggition="select SftId,SftName from HRD_Shift where IsActive = 1 and IsInitial = 1 and " + condition + "";
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadInitialShiftByDepartment(DropDownList dl, string CompanyId, string DptID)
        {
            try
            {
                sqlDB.fillDataTable("select SftId,SftName from HRD_Shift where IsActive=1  and CompanyId='" + CompanyId + "' and DptId In('" + DptID + "','0')", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadShiftByDepartment(DropDownList dl, string CompanyId, string DptID)
        {
            try
            {
                sqlDB.fillDataTable("select SftId,SftName from HRD_Shift where IsActive=1  and CompanyId='" + CompanyId + "' and DptId in( '" + DptID + "','0')", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadShift(DropDownList dl, string CompanyId, int t)
        {
            try
            {
                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where companyId='" + CompanyId + "' AND IsActive='true'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                if (dl.Items.Count > 1)
                {
                    dl.Items.Insert(0, new ListItem("All", "0"));
                }
            }
            catch { }
        }
        public static void LoadShift(DropDownList dl, string CompanyId, string UserType)
        {
            try
            {
                if (UserType.Equals("Super Admin") || UserType.Equals("Master Admin")) sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where IsActive='true'", dt = new DataTable());
                else sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where companyId='" + CompanyId + "' AND IsActive='true'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadGrade(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select GrdName,GrdName From HRD_Grade where GrdStatus='True'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "GrdName";
                dl.DataTextField = "GrdName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void loadEmpCardNo(DropDownList dl, string empId)
        {
            try
            {
                dl.Items.Clear();
                SQLOperation.selectBySetCommandInDatatable("select distinct EmpCardNo from Personnel_EmpCurrentStatus where EmpStatus in('1','8') AND EmpTypeId=" + empId + "", dt = new DataTable(), sqlDB.connection);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(dl.Items.Count, new ListItem("Card No", "Card No"));
                dl.SelectedIndex = dl.Items.Count - 1;

            }
            catch { }
        }

        public static void loadPFMemberList(DropDownList dl, string CompanyId)
        {
            try
            {
                dl.Items.Clear();
                SQLOperation.selectBySetCommandInDatatable("select EmpID,  EmpCardNo+' '+ EmpName as EmpCardNo  from v_Personnel_EmpCurrentStatus where isActive=1 and PfMember=1  and CompanyID='" + CompanyId + "'", dt = new DataTable(), sqlDB.connection);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpID";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", ""));


            }
            catch { }
        }
        public static void loadPFMemberListForWithdraw(DropDownList dl, string CompanyId)
        {
            try
            {

                string condition = AccessControl.loadEmpCardNumber(CompanyId);
                dl.Items.Clear();
                SQLOperation.selectBySetCommandInDatatable("select EmpID+'/'+convert(varchar(10),PfDate,120)  EmpID,  EmpCardNo+' '+ EmpName as EmpCardNo  from v_Personnel_EmpCurrentStatus where isActive=1 and PfMember=1  and " + condition + "", dt = new DataTable(), sqlDB.connection);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpID";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", ""));
            }
            catch { }
        }
        public static void loadEmpCardNo(DropDownList dl)
        {
            try
            {
                SQLOperation.selectBySetCommandInDatatable("select Distinct EmpCardNo from Personnel_EmployeeInfo where EmpStatus not in('Dismissed')", dt = new DataTable(), sqlDB.connection);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(dl.Items.Count, new ListItem("Card No", "Card No"));
                dl.SelectedIndex = dl.Items.Count - 1;

            }
            catch { }
        }
        public static void loadMaternityEmpCardNo(DropDownList dl, string TypeId)
        {
            try
            {
                SQLOperation.selectBySetCommandInDatatable("select  EmpCardNo from v_Leave_LeaveApplication where ShortName='m/l' and EmpTypeId=" + TypeId + " group by EmpCardNo", dt = new DataTable(), sqlDB.connection);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("Card No", "0"));


            }
            catch { }
        }

        public static void LoadRligion(DropDownList dl)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select RId,RName From HRD_Religion where RId<>1 order by RId ", dt);
                dl.DataSource = dt;
                dl.DataTextField = "RName";
                dl.DataValueField = "RId";
                dl.DataBind();
                // dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }


        public static void LoadRligionForBonus(DropDownList dl)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select RId,RName From HRD_Religion  order by RId ", dt);
                dl.DataSource = dt;
                dl.DataTextField = "RName";
                dl.DataValueField = "RId";
                dl.DataBind();
                // dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void loadEmpTye(DropDownList dl)
        {
            try
            {
                dt = new DataTable();

                sqlDB.fillDataTable("select EmpTypeId,EmpType from  HRD_EmployeeType ", dt);
                dl.DataSource = dt;

                dl.DataValueField = "EmpTypeId";
                dl.DataTextField = "EmpType";
                dl.DataBind();
                dl.Items.Insert(dl.Items.Count, new ListItem("Select Type", "Select Type"));
                dl.SelectedIndex = dl.Items.Count - 1;

            }
            catch { }
        }
        public static void loadEmpTye(RadioButtonList rl)
        {
            try
            {
                dt = new DataTable();

                sqlDB.fillDataTable("select EmpTypeId,EmpType from  HRD_EmployeeType ", dt);
                rl.DataSource = dt;

                rl.DataValueField = "EmpTypeId";
                rl.DataTextField = "EmpType";
                rl.DataBind();
                // dl.Items.Insert(dl.Items.Count, new ListItem("Select Type", "Select Type"));
                // dl.SelectedIndex = dl.Items.Count - 1;

            }
            catch { }
        }
        public static void LoadEmpCardNoAppoinmentLetter(string EmpTypeId, string DId, string DptId, DropDownList dl)
        {
            try
            {
                dt = new DataTable();
                sqlDB.fillDataTable("Select Distinct EmpCardNo,EmpId from Personnel_EmpCurrentStatus where EmpTypeId=" + EmpTypeId + " and DId=" + DId + " and DptId=" + DptId + " and IsActive=1", dt);
                dl.DataSource = dt;

                dl.DataValueField = "EmpId";
                dl.DataTextField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadEmpCardNoByDepartment(DropDownList dl, string CompanyId, string DptId)
        {
            try
            {

                dt = new DataTable();
                sqlDB.fillDataTable("Select EmpCardNo,EmpId,EmpCardNo+' [ '+EmpName+' ]' as EmpCardNoAndName From v_Personnel_EmpCurrentStatus where   EmpStatus in ('1','8') and IsActive='1' and CompanyId='" + CompanyId + "' and DptId=" + DptId + " Group by EmpId,EmpCardNo,EmpName,DptCode,CustomOrdering order by DptCode, CustomOrdering", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNoAndName";
                dl.DataValueField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadEmpCardNoByCompany(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadEmpCardNumber(CompanyId);
                dt = new DataTable();
                sqlDB.fillDataTable("Select EmpCardNo,EmpId,EmpCardNo+' [ '+EmpName+' ]' as EmpCardNoAndName From v_Personnel_EmpCurrentStatus where   EmpStatus in ('1','8') and IsActive='1' and " + condition + " Group by EmpId,EmpCardNo,EmpName,DptCode,CustomOrdering order by DptCode, CustomOrdering", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNoAndName";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void loadEmpCardNoByCompanyAndEmpType(DropDownList dl, string CompanyId, string empType)
        {
            try
            {

                dt = new DataTable();
                sqlDB.fillDataTable("Select EmpCardNo,EmpId,EmpCardNo+' [ '+EmpName+' ]' as EmpCardNoAndName From v_Personnel_EmpCurrentStatus where   EmpStatus in ('1','8') and IsActive='1' and CompanyId='" + CompanyId + "' and EmpTypeId in(" + empType + ") Group by EmpId,EmpCardNo,EmpName,DptCode,CustomOrdering order by DptCode, CustomOrdering", dt);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNoAndName";
                dl.DataValueField = "EmpId";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadEmpCardNo(DropDownList dl, string empId, string CompanyId)
        {
            try
            {
                dl.Items.Clear();
                SQLOperation.selectBySetCommandInDatatable("select distinct EmpCardNo from Personnel_EmpCurrentStatus where EmpStatus in('1','8') AND EmpTypeId=" + empId + " and CompanyId='" + CompanyId + "'", dt = new DataTable(), sqlDB.connection);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(dl.Items.Count, new ListItem("Card No", "Card No"));
                dl.SelectedIndex = dl.Items.Count - 1;

            }
            catch { }
        }
        public static void loadEmpCardNoCompliance(DropDownList dl, string empId, string CompanyId)
        {
            try
            {
                dl.Items.Clear();
                SQLOperation.selectBySetCommandInDatatable("select distinct EmpCardNo from Personnel_EmpCurrentStatus1 where EmpStatus in('1','8') AND EmpTypeId=" + empId + " and CompanyId='" + CompanyId + "'", dt = new DataTable(), sqlDB.connection);
                dl.DataSource = dt;
                dl.DataTextField = "EmpCardNo";
                dl.DataValueField = "EmpCardNo";
                dl.DataBind();
                dl.Items.Insert(dl.Items.Count, new ListItem("Card No", "Card No"));
                dl.SelectedIndex = dl.Items.Count - 1;

            }
            catch { }
        }
        public static string LoadSL(string cmd, string HRDName)
        {
            try
            {
                string SL = "";
                DataTable dt = new DataTable();
                DataTable dtHRD = new DataTable();
                sqlDB.fillDataTable(cmd, dt);
                if (dt.Rows[0]["SL"].ToString() == "")
                {
                    SL = "0001";
                }
                else
                {
                    string ID = "";
                    if (HRDName == "Division")
                    {
                        //sqlDB.fillDataTable("Select DId From HRD_Division where SL=" + dt.Rows[0]["SL"].ToString() + "", dtHRD = new DataTable());
                        sqlDB.fillDataTable("Select max(convert(int, DId)) as DId From HRD_Division", dtHRD = new DataTable());
                        ID = int.Parse(dtHRD.Rows[0]["DId"].ToString()).ToString();
                    }
                    else if (HRDName == "Department")
                    {
                        // sqlDB.fillDataTable("Select DptId From HRD_Department where SL=" + dt.Rows[0]["SL"].ToString() + "", dtHRD = new DataTable());
                        sqlDB.fillDataTable("Select max(convert(int, DptId)) as DptId From HRD_Department", dtHRD = new DataTable());
                        ID = int.Parse(dtHRD.Rows[0]["DptId"].ToString()).ToString();
                    }
                    else if (HRDName == "Designation")
                    {
                        //   sqlDB.fillDataTable("Select DsgId From HRD_Designation where SL=" + dt.Rows[0]["SL"].ToString() + "", dtHRD = new DataTable());
                        sqlDB.fillDataTable("Select max(convert(int, DsgId)) as  DsgId From HRD_Designation", dtHRD = new DataTable());
                        ID = int.Parse(dtHRD.Rows[0]["DsgId"].ToString()).ToString();
                    }
                    else if (HRDName == "Grade")
                    {
                        // sqlDB.fillDataTable("Select GrdId From HRD_Grade where SL=" + dt.Rows[0]["SL"].ToString() + "", dtHRD = new DataTable());
                        sqlDB.fillDataTable("Select max(convert(int, GrdId)) as GrdId From HRD_Grade", dtHRD = new DataTable());
                        ID = int.Parse(dtHRD.Rows[0]["GrdId"].ToString()).ToString();
                    }
                    else if (HRDName == "Floor")
                    {
                        // sqlDB.fillDataTable("Select FId From HRD_Floor where SL=" + dt.Rows[0]["SL"].ToString() + "", dtHRD = new DataTable());
                        sqlDB.fillDataTable("Select max(convert(int, FId)) as FId From HRD_Floor", dtHRD = new DataTable());
                        ID = int.Parse(dtHRD.Rows[0]["FId"].ToString()).ToString();
                    }
                    else if (HRDName == "Line")
                    {
                        // sqlDB.fillDataTable("Select LnId From HRD_Line where SL=" + dt.Rows[0]["SL"].ToString() + "", dtHRD = new DataTable());
                        sqlDB.fillDataTable("Select max(convert(int, LnId)) as LnId From HRD_Line", dtHRD = new DataTable());
                        ID = int.Parse(dtHRD.Rows[0]["LnId"].ToString()).ToString();
                    }
                    if (ID.Length == 1) SL = "000" + (int.Parse(ID) + 1);
                    else if (ID.Length == 2) SL = "00" + (int.Parse(ID) + 1);
                    else if (ID.Length == 3) SL = "0" + (int.Parse(ID) + 1);
                    else if (ID.Length == 4) SL = (int.Parse(ID) + 1).ToString();

                }
                return SL;
            }
            catch { return "0001"; }
        }
        public static void LoadDistrict(DropDownList dl)
        {
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("Select * From HRD_District order by DstName", dt);
            dl.DataValueField = "DstId";
            dl.DataTextField = "DstName";
            dl.DataSource = dt;
            dl.DataBind();

            dl.Items.Insert(0, new ListItem("Select", "0"));

        }
        //public static void LoadThana(DropDownList dl) 
        //{
        //    sqlDB.loadDropDownList("Select * From HRDThanaInfo", dl);
        //    dl.DataValueField = "DstId";
        //    dl.DataTextField = "DstName";
        //    dl.DataSource = dt;
        //    dl.DataBind();
        //}
        public static void LoadMonthName(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select MonthID,MonthName From tblMonthSetup order by MonthName Desc ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "MonthName";
                dl.DataValueField = "MonthID";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        // ----------------For Seperation--------------------
        public static void LoadMonthForSeperation(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("Select distinct EFMonth,format(EffectiveDate,'MMM-yyy')as EFMonthName , SUBSTRING(EFMonth,0,3), SUBSTRING(EFMonth,4,4) From v_SeparationSheet where CompanyId='" + CompanyId + "' order by  SUBSTRING(EFMonth,4,4) Desc, SUBSTRING(EFMonth,0,3) desc ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "EFMonthName";
                dl.DataValueField = "EFMonth";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadMonthForSeperationCompliance(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("Select distinct EFMonth,format(EffectiveDate,'MMM-yyy')as EFMonthName , SUBSTRING(EFMonth,0,3), SUBSTRING(EFMonth,4,4) From v_SeparationSheet1 where CompanyId='" + CompanyId + "' order by  SUBSTRING(EFMonth,4,4) Desc, SUBSTRING(EFMonth,0,3) desc ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "EFMonthName";
                dl.DataValueField = "EFMonth";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadMonthForSeperation(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("Select distinct EFMonth,format(EffectiveDate,'MMM-yyy')as EFMonthName From v_SeparationSheet order by EFMonth Desc ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "EFMonthName";
                dl.DataValueField = "EFMonth";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        //-------------------------------------------------------------------------
        //-------------------------------For Promotion-----------------------------------------
        public static void LoadMonthForPromotion(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlCmd = "select distinct FORMAT(CONVERT(datetime,convert(varchar(2),SUBSTRING(EffectiveMonth,1,2))+'/'+convert(varchar(2),01)+'/'+convert(varchar(4),SUBSTRING(EffectiveMonth,4,10))),'MMM-yyyy')" +
                           " as MonthName,EffectiveMonth,substring(EffectiveMonth,4,4) ,substring(EffectiveMonth,0,3)  from v_Promotion_Increment where TypeOfChange='p' and CompanyId='" + CompanyId + "' order by substring(EffectiveMonth,4,4) desc,substring(EffectiveMonth,0,3) desc";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "MonthName";
                dl.DataValueField = "EffectiveMonth";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadMonthForPromotionCompliance(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlCmd = "select distinct FORMAT(CONVERT(datetime,convert(varchar(2),SUBSTRING(EffectiveMonth,1,2))+'/'+convert(varchar(2),01)+'/'+convert(varchar(4),SUBSTRING(EffectiveMonth,4,10))),'MMM-yyyy')" +
                           " as MonthName,EffectiveMonth,substring(EffectiveMonth,4,4) ,substring(EffectiveMonth,0,3)  from v_Promotion_Increment1 where TypeOfChange='p' and CompanyId='" + CompanyId + "' order by substring(EffectiveMonth,4,4) desc,substring(EffectiveMonth,0,3) desc";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "MonthName";
                dl.DataValueField = "EffectiveMonth";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        //-----------------------------------------------------------------
        //-------------------------------For Increament-----------------------------------------
        public static void LoadMonthForIncreament(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlCmd = "select distinct FORMAT(CONVERT(datetime,convert(varchar(2),SUBSTRING(EffectiveMonth,1,2))+'/'+convert(varchar(2),01)+'/'+convert(varchar(4),SUBSTRING(EffectiveMonth,4,10))),'MMM-yyyy')" +
                         " as MonthName,EffectiveMonth,substring(EffectiveMonth,4,4) ,substring(EffectiveMonth,0,3)  from v_Promotion_Increment where TypeOfChange='i' and CompanyId='" + CompanyId + "' order by substring(EffectiveMonth,4,4) desc,substring(EffectiveMonth,0,3) desc";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "MonthName";
                dl.DataValueField = "EffectiveMonth";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void LoadMonthForIncreamentCompliance(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlCmd = "select distinct FORMAT(CONVERT(datetime,convert(varchar(2),SUBSTRING(EffectiveMonth,1,2))+'/'+convert(varchar(2),01)+'/'+convert(varchar(4),SUBSTRING(EffectiveMonth,4,10))),'MMM-yyyy')" +
                         " as MonthName,EffectiveMonth,substring(EffectiveMonth,4,4) ,substring(EffectiveMonth,0,3)  from v_Promotion_Increment1 where TypeOfChange='i' and CompanyId='" + CompanyId + "' order by substring(EffectiveMonth,4,4) desc,substring(EffectiveMonth,0,3) desc";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                dl.DataSource = dt;
                dl.DataTextField = "MonthName";
                dl.DataValueField = "EffectiveMonth";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }


        //-----------------------------------------------------------------
        public static string getCompaniesList(DropDownList ddlCompanyList)
        {
            try
            {
                string CompanyList = "";
                for (byte b = 0; b < ddlCompanyList.Items.Count; b++)
                {

                    if (b == 0 && b == ddlCompanyList.Items.Count - 1)
                    {
                        CompanyList = "in('" + ddlCompanyList.Items[b].Value + "')";
                    }
                    else if (b == 0 && b != ddlCompanyList.Items.Count - 1)
                    {
                        CompanyList += "in ('" + ddlCompanyList.Items[b].Value + "'";
                    }
                    else if (b != 0 && b == ddlCompanyList.Items.Count - 1)
                    {
                        CompanyList += ",'" + ddlCompanyList.Items[b].Value + "')";
                    }
                    else CompanyList += ",'" + ddlCompanyList.Items[b].Value + "'";
                }
                return CompanyList;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string getDivisionsList(DropDownList ddlDivisionList)
        {
            try
            {
                string DivisionList = "";
                for (byte b = 0; b < ddlDivisionList.Items.Count; b++)
                {

                    if (b == 0 && b == ddlDivisionList.Items.Count - 1)
                    {
                        DivisionList = "in('" + ddlDivisionList.Items[b].Value + "')";
                    }
                    else if (b == 0 && b != ddlDivisionList.Items.Count - 1)
                    {
                        DivisionList += "in ('" + ddlDivisionList.Items[b].Value + "'";
                    }
                    else if (b != 0 && b == ddlDivisionList.Items.Count - 1)
                    {
                        DivisionList += ",'" + ddlDivisionList.Items[b].Value + "')";
                    }
                    else DivisionList += ",'" + ddlDivisionList.Items[b].Value + "'";
                }
                return DivisionList;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string getShiftList(DropDownList ddlShiftList)
        {
            try
            {
                string ShiftList = "";
                for (byte b = 0; b < ddlShiftList.Items.Count; b++)
                {

                    if (b == 0 && b == ddlShiftList.Items.Count - 1)
                    {
                        ShiftList = "in(" + ddlShiftList.Items[b].Value + ")";
                    }
                    else if (b == 0 && b != ddlShiftList.Items.Count - 1)
                    {
                        ShiftList += "in (" + ddlShiftList.Items[b].Value + "";
                    }
                    else if (b != 0 && b == ddlShiftList.Items.Count - 1)
                    {
                        ShiftList += "," + ddlShiftList.Items[b].Value + ")";
                    }
                    else ShiftList += "," + ddlShiftList.Items[b].Value + "";
                }
                return ShiftList;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string getDepartmentList(ListBox lstSelectedDepartment)
        {
            try
            {

                string DepartmentList = "";
                for (int b = 0; b < lstSelectedDepartment.Items.Count; b++)
                {

                    if (b == 0 && b == lstSelectedDepartment.Items.Count - 1)
                    {
                        DepartmentList = "in('" + lstSelectedDepartment.Items[b].Value + "')";
                    }
                    else if (b == 0 && b != lstSelectedDepartment.Items.Count - 1)
                    {
                        DepartmentList += "in ('" + lstSelectedDepartment.Items[b].Value + "'";
                    }
                    else if (b != 0 && b == lstSelectedDepartment.Items.Count - 1)
                    {
                        DepartmentList += ",'" + lstSelectedDepartment.Items[b].Value + "')";
                    }
                    else DepartmentList += ",'" + lstSelectedDepartment.Items[b].Value + "'";
                }
                return DepartmentList;
            }
            catch (Exception ex)
            {
                return " ";
            }
        }

        public static string getDepartmentList()
        {
            try
            {
                da = new SqlDataAdapter("SELECT DptId, DptName FROM HRD_Department", sqlDB.connection);
                da.Fill(dt = new DataTable());

                string DepartmentList = "";
                for (byte b = 0; b < dt.Rows.Count; b++)
                {

                    if (b == 0 && b == dt.Rows.Count - 1)
                    {
                        DepartmentList = "in('" + dt.Rows[b]["DptId"].ToString() + "')";
                    }
                    else if (b == 0 && b != dt.Rows.Count - 1)
                    {
                        DepartmentList += "in ('" + dt.Rows[b]["DptId"].ToString() + "'";
                    }
                    else if (b != 0 && b == dt.Rows.Count - 1)
                    {
                        DepartmentList += ",'" + dt.Rows[b]["DptId"].ToString() + "')";
                    }
                    else DepartmentList += ",'" + dt.Rows[b]["DptId"].ToString() + "'";
                }
                return DepartmentList;
            }
            catch (Exception ex)
            {
                return " ";
            }
        }

        public static void LoadDepartmentByDivision(string divisionId, ListBox lst)
        {
            try
            {
                dt = new DataTable();

                sqlDB.fillDataTable("SELECT DptId, DptName FROM HRD_Department where DId=" + divisionId + "", dt);

                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }

        public static void LoadDepartmentByCompanyAndShiftInListBox(string CompanyId, string ShiftId, ListBox lst) // v_Personnel_EmpCurrentStatus replaced By v_tblAttendanceRecord
        {
            try
            {
                dt = new DataTable();


                sqlDB.fillDataTable("SELECT Distinct DptId, DptName FROM v_tblAttendanceRecord where CompanyId='" + CompanyId + "' AND SftId " + ShiftId + "", dt = new DataTable());

                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }


        public static void LoadDepartmentByCompanyInListBox(string CompanyId, ListBox lst)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                dt = new DataTable();


                sqlDB.fillDataTable("SELECT  DptId, DptName FROM HRD_Department where " + condition + " And DptStatus=1 ", dt = new DataTable());
                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }

        public static void LoadDepartmentByCompanyInListBox1(string CompanyId, ListBox lst)
        {
            try
            {
                dt = new DataTable();


                sqlDB.fillDataTable("SELECT Distinct DptId, DptName FROM v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' ", dt = new DataTable());

                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }
        public static void LoadDepartmentByCompanyInListBoxType(ListBox lst, string TypeId, string CompanyId)
        {
            try
            {
                dt = new DataTable();


                sqlDB.fillDataTable("SELECT Distinct DptId, DptName FROM v_EmployeeDetails where CompanyId='" + CompanyId + "' and EmpTypeId=" + TypeId + "", dt = new DataTable());

                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }
        public static void LoadDepartmentByCompanyInListBoxType(ListBox lst, string TypeId, string CompanyId, string MonthName)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                dt = new DataTable();

                TypeId = (TypeId == "All") ? "" : " and EmpTypeId=" + TypeId + "";
                sqlDB.fillDataTable("SELECT Distinct DptId, DptName FROM v_SeparationSheet where " + condition + " " + TypeId + "  and EFMonth='" + MonthName + "' and IsActive=1", dt = new DataTable());

                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }
        public static void LoadDepartmentByCompanyInListBoxTypeCompliance(ListBox lst, string TypeId, string CompanyId, string MonthName)
        {
            try
            {
                dt = new DataTable();

                TypeId = (TypeId == "All") ? "" : " and EmpTypeId=" + TypeId + "";
                sqlDB.fillDataTable("SELECT Distinct DptId, DptName FROM v_SeparationSheet1 where CompanyId='" + CompanyId + "' " + TypeId + "  and EFMonth='" + MonthName + "' and IsActive=1", dt = new DataTable());

                lst.DataValueField = "DptId";
                lst.DataTextField = "DptName";
                lst.DataSource = dt;
                lst.DataBind();
            }
            catch { }
        }

        public static void loadBankNameCompanyWise(string companyId,DropDownList ddlbankList)
        {
           
            DataTable dt = new DataTable();
            sqlDB.fillDataTable("select  CAST(BankId AS VARCHAR) + '_' + BankShortName AS BankId, BankName + '(' + BankShortName + ')' AS BankName from Hrd_BankInfo where IsActive=1 and  BankShortName is not null and CompanyId='0001'", dt);
            ddlbankList.DataSource = dt;
            ddlbankList.DataValueField = "BankId";
            ddlbankList.DataTextField = "BankName";
            ddlbankList.DataBind();
            ddlbankList.Items.Insert(0, new ListItem("ALL", "0"));
           
        }
        public static void AddRemoveItem(ListBox aSource, ListBox aTarget)
        {

            ListItemCollection licCollection;

            try
            {

                licCollection = new ListItemCollection();
                for (int intCount = 0; intCount < aSource.Items.Count; intCount++)
                {
                    if (aSource.Items[intCount].Selected == true)
                        licCollection.Add(aSource.Items[intCount]);
                }

                for (int intCount = 0; intCount < licCollection.Count; intCount++)
                {
                    aSource.Items.Remove(licCollection[intCount]);
                    aTarget.Items.Add(licCollection[intCount]);
                }


            }
            catch (Exception expException)
            {
                // Response.Write(expException.Message);
            }
            finally
            {
                licCollection = null;
            }

        }

        public static void AddRemoveAll(ListBox aSource, ListBox aTarget)
        {

            try
            {

                foreach (ListItem item in aSource.Items)
                {
                    aTarget.Items.Add(item);
                }
                aSource.Items.Clear();

            }
            catch (Exception expException)
            {
                // Response.Write(expException.Message);
            }

        }

        public static DataTable AttSummaryDetails(string sid, string cid, string did, string title, string d, string m, string y, string Attstatus, string fd, string fm, string fy)
        {
            try
            {


                dt = new DataTable();
                if (title == "Department" || title == "Shift")
                {
                    sqlDB.fillDataTable("Select format(ATTDate,'dd-MM-yyyy') as ATTDate,right(EmpCardNo,len( EmpCardNo)-4) as EmpCardNo,ATTStatus,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,DptCode,GId , GName ,Address From v_tblAttendanceRecord where CompanyId='" + cid + "'  and DptId='" + did + "' AND EmpStatus in (1,8) and ATTDate between '" + fy + "-" + fm + "-" + fd + "' and '" + y + "-" + m + "-" + d + "' order by GId, CustomOrdering ", dt);
                    //else sqlDB.fillDataTable("Select format(ATTDate,'dd-MM-yyyy') as ATTDate,right(EmpCardNo,len( EmpCardNo)-4) as EmpCardNo,ATTStatus,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,DptCode,GId , GName ,Address From v_tblAttendanceRecord where CompanyId='" + cid + "'  and DptId='" + did + "' AND EmpStatus in (1,8) and ATTDate between '" + fy + "-" + fm + "-" + fd + "' and '" + y + "-" + m + "-" + d + "' order by GroupOrdering,GId, CustomOrdering ", dt);
                }
                else if (Attstatus == "Total")
                {
                    sqlDB.fillDataTable("Select format(ATTDate,'dd-MM-yyyy') as ATTDate,right(EmpCardNo,len( EmpCardNo)-4) as EmpCardNo,ATTStatus,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,DptCode,GId , GName ,Address From v_tblAttendanceRecord where CompanyId='" + cid + "'  and DptId='" + did + "' AND EmpStatus in (1,8) and ATTDate between '" + fy + "-" + fm + "-" + fd + "' and '" + y + "-" + m + "-" + d + "' order by GId, CustomOrdering ", dt);
                    //else sqlDB.fillDataTable("Select format(ATTDate,'dd-MM-yyyy') as ATTDate,right(EmpCardNo,len( EmpCardNo)-4) as EmpCardNo,ATTStatus,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,DptCode,GId , GName ,Address From v_tblAttendanceRecord where CompanyId='" + cid + "'  and DptId='" + did + "'  AND EmpStatus in (1,8) and ATTDate between '" + fy + "-" + fm + "-" + fd + "' and '" + y + "-" + m + "-" + d + "' order by GroupOrdering,GId, CustomOrdering ", dt);
                }
                else
                {
                    sqlDB.fillDataTable("Select format(ATTDate,'dd-MM-yyyy') as ATTDate,right(EmpCardNo,len( EmpCardNo)-4) as EmpCardNo,ATTStatus,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,DptCode,GId , GName ,Address From v_tblAttendanceRecord where CompanyId='" + cid + "'  and DptId='" + did + "' AND EmpStatus in (1,8) and ATTStatus='" + Attstatus + "' and ATTDate between '" + fy + "-" + fm + "-" + fd + "' and '" + y + "-" + m + "-" + d + "' order by GId, CustomOrdering ", dt);
                    //else sqlDB.fillDataTable("Select format(ATTDate,'dd-MM-yyyy') as ATTDate,right(EmpCardNo,len( EmpCardNo)-4) as EmpCardNo,ATTStatus,EmpName,DsgName,InHour,InMin,OutHour,OutMin,InSec,OutSec,CompanyName,DptName,DptCode,GId , GName ,Address From v_tblAttendanceRecord where CompanyId='" + cid + "'  and DptId='" + did + "' AND EmpStatus in (1,8) and ATTStatus='" + Attstatus + "' and ATTDate between '" + fy + "-" + fm + "-" + fd + "' and '" + y + "-" + m + "-" + d + "' order by GroupOrdering,GId, CustomOrdering ", dt);
                }
                return dt;
            }
            catch { return dt; }
        }


        public static void loadMonthIdByCompany(DropDownList ddlMonthList, string CompanyId)
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable(" select Format(FromDate,'MMM-yyyy') as YearMonth,Format(FromDate,'MM-yyyy') as MonthYear from tblMonthSetup where CompanyId='" + CompanyId + "' order by Year(FromDate) desc ,Month(FromDate) desc", dt);
                ddlMonthList.DataSource = dt;
                ddlMonthList.DataValueField = "MonthYear";
                ddlMonthList.DataTextField = "YearMonth";
                ddlMonthList.DataBind();
                ddlMonthList.Items.Insert(0, new ListItem(" ", "0"));
            }
            catch { }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------
        public static void LoadShiftByDepartmentForOnloyInitilizedShift(DropDownList ddl, string DptId)
        {
            try
            {
                sqlDB.fillDataTable("select distinct SftId,SftName from v_Personnel_EmpCurrentStatus where DptId='" + DptId + "'" +
                     "order by SftName", dt = new DataTable());
                ddl.DataTextField = "SftName";
                ddl.DataValueField = "SftId";
                ddl.DataSource = dt;
                ddl.DataBind();

                ddl.Items.Insert(0, new ListItem(" ", "0"));
            }
            catch { }
        }
        public static void LoadShiftForSMOperation(DropDownList dl, string CompanyId, string dptId)
        {
            try
            {
                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where IsInitial='false' and CompanyId='" + CompanyId + "' AND DptId='" + dptId + "' order by sftName ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }

        public static void LoadShiftForSMOperation(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where IsInitial='false' and " + condition + " order by sftName ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void loadDepartmentListByCompanyAndGroup(DropDownList dl, string CompanyId, string GroupId)
        {
            try
            {
                da = new SqlDataAdapter("select Distinct DptId,DptName from v_Personnel_EmpCurrentStatus where CompanyId=" + CompanyId + " AND  GId=" + GroupId + " order by DptName ", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));

                int i = 0;
                foreach (ListItem item in dl.Items)
                {
                    if (i % 2 == 0) item.Attributes.Add("style", "color:green");
                    else item.Attributes.Add("style", "color:red");

                    i++;
                }
            }
            catch { }
        }
        public static void LoadGroupByDepartmentInListBox_ForRoster(ListBox lb, string DptId, string CompanyId)// Load Group By Department for Roster Report
        {
            try
            {
                sqlDB.fillDataTable("select distinct GId,GName from v_ShiftTransferInfoDetails  where DptId='" + DptId + "' and CompanyId='" + CompanyId + "' ", dt = new DataTable());
                lb.DataTextField = "GName";
                lb.DataValueField = "GId";
                lb.DataSource = dt;
                lb.DataBind();
            }
            catch { }
        }
        public static void loadGroupByDepartment_Company(DropDownList dl, string CompanyId, string departmentId)
        {
            try
            {
                da = new SqlDataAdapter("select distinct GId,GName From v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' and DptId='" + departmentId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "GId";
                dl.DataTextField = "GName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("All", "0"));
            }
            catch { }
        }
        public static void loadGroupByDepartment(DropDownList dl, string departmentId)
        {
            try
            {
                da = new SqlDataAdapter("select GId,GName from HRD_Group where DptId ='" + departmentId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "GId";
                dl.DataTextField = "GName";
                dl.DataSource = dt;
                dl.DataBind();
                if (dl.Items.Count > 1)
                    dl.Items.Insert(0, new ListItem("All", "0"));
            }
            catch { }
        }
        public static void LoadShiftByDepartmentWithAll(DropDownList ddl, string DptId)
        {
            try
            {
                sqlDB.fillDataTable("select distinct SftId,SftName from v_ShiftTransferInfoDetails where DptId='" + DptId + "' ", dt = new DataTable());
                ddl.DataTextField = "SftName";
                ddl.DataValueField = "SftId";
                ddl.DataSource = dt;
                ddl.DataBind();
                if (dt.Rows.Count > 1)
                    ddl.Items.Insert(0, new ListItem("All", "00"));
                else ddl.Items.Insert(0, new ListItem("", "0"));


                int i = 0;
                foreach (ListItem item in ddl.Items)
                {
                    if (i % 2 == 0) item.Attributes.Add("style", "color:green");
                    else item.Attributes.Add("style", "color:red");

                    i++;
                }
            }
            catch { }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------
        public static void LoadShiftWithoutInitialByDepartment(DropDownList dl, string CompanyId, string DptId)
        {
            try
            {

                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where companyId='" + CompanyId + "' and DptId='" + DptId + "' and IsActive='true' and IsInitial=0", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem(string.Empty, "0"));
            }
            catch { }
        }
        public static void loadDepartmentListByCompany_ForShrink(DropDownList dl, string CompanyId)
        {
            try
            {
                string condition = AccessControl.loadDepartmetCondition(CompanyId);
                da = new SqlDataAdapter("SELECT DptId, DptName FROM HRD_Department where " + condition + "", sqlDB.connection);
                
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("All", "0"));
            }
            catch { }
        }
        public static string loadAttMachineName(string CompanyId)
        {
            try
            {
                da = new SqlDataAdapter("SELECT AttMachineName FROM HRD_CompanyInfo  where CompanyId='" + CompanyId + "'", sqlDB.connection);
                da.Fill(dt = new DataTable());
                if (dt.Rows.Count > 0)
                    return dt.Rows[0]["AttMachineName"].ToString();
                else
                    return "";
            }
            catch { return ""; }
        }

        //------------------------For Bangla Month Name-------------------------------
        public static string GenerateBanglaMonthYMD(string Date)
        {
            string[] DatePart = Date.Split('-');
            int MonthId = int.Parse(DatePart[1].ToString());
            switch (MonthId)
            {
                case 1:
                    Date = "Rvbyqvix";
                    break;
                case 2:
                    Date = "†d«eª“qvix";
                    break;
                case 3:
                    Date = "gvP©";
                    break;
                case 4:
                    Date = "GwcÖj";
                    break;
                case 5:
                    Date = "†g";
                    break;
                case 6:
                    Date = "Ryb";
                    break;
                case 7:
                    Date = "RyjvB";
                    break;
                case 8:
                    Date = "AvM÷";
                    break;
                case 9:
                    Date = "†m‡Þ¤^i";
                    break;
                case 10:
                    Date = "A‡±vei";
                    break;
                case 11:
                    Date = "b‡f¤^i";
                    break;
                case 12:
                    Date = "wW‡m¤^i";
                    break;
            }
            return DatePart[2] + " " + Date + " " + DatePart[0];
        }
        public static string GenerateBanglaMonthDMY(string Date)
        {
            string[] DatePart = Date.Split('-');
            int MonthId = int.Parse(DatePart[1].ToString());
            switch (MonthId)
            {
                case 1:
                    Date = "Rvbyqvix";
                    break;
                case 2:
                    Date = "†d«eª“qvix";
                    break;
                case 3:
                    Date = "gvP©";
                    break;
                case 4:
                    Date = "GwcÖj";
                    break;
                case 5:
                    Date = "†g";
                    break;
                case 6:
                    Date = "Ryb";
                    break;
                case 7:
                    Date = "RyjvB";
                    break;
                case 8:
                    Date = "AvM÷";
                    break;
                case 9:
                    Date = "†m‡Þ¤^i";
                    break;
                case 10:
                    Date = "A‡±vei";
                    break;
                case 11:
                    Date = "b‡f¤^i";
                    break;
                case 12:
                    Date = "wW‡m¤^i";
                    break;
            }
            return DatePart[0] + " " + Date + " " + DatePart[2];
        }
        public static string GenerateBanglaMonthNameMY(string MonthID)
        {
            string[] DatePart = MonthID.Split('-');
            int MonthId = int.Parse(DatePart[0].ToString());
            switch (MonthId)
            {
                case 1:
                    MonthID = "Rvbyqvix";
                    break;
                case 2:
                    MonthID = "†deª“qvwi"; //"†d«eª“qvix";
                    break;
                case 3:
                    MonthID = "gvP©";
                    break;
                case 4:
                    MonthID = "GwcÖj";
                    break;
                case 5:
                    MonthID = "†g";
                    break;
                case 6:
                    MonthID = "Ryb";
                    break;
                case 7:
                    MonthID = "RyjvB";
                    break;
                case 8:
                    MonthID = "AvM÷";
                    break;
                case 9:
                    MonthID = "†m‡Þ¤^i";
                    break;
                case 10:
                    MonthID = "A‡±vei";
                    break;
                case 11:
                    MonthID = "b‡f¤^i";
                    break;
                case 12:
                    MonthID = "wW‡m¤^i";
                    break;
            }
            return MonthID + "-" + DatePart[1];
        }
        public static void LoadAssignedShiftList_BySearchTToDate(string DepartmentId, string GroupId, string CompanyId, DropDownList ddl, string TToDate)
        {
            try
            {
                sqlCmd = " select sti.STId,hs.SftName+' | '+CONVERT(varchar, Format(sti.TFromDate,'MMM-dd-yyyy')) +' '+CONVERT(varchar,Format(sti.TToDate,'MMM-dd-yyyy')) as ShiftTitle from  " +
                    " HRD_Shift as hs inner join ShiftTransferInfo as sti on hs.SftId=sti.SftId  AND sti.DptId='" + DepartmentId + "' AND sti.GId='" + GroupId + "' AND sti.CompanyId='" + CompanyId + "' AND TToDate >= '" + TToDate + "'";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());
                ddl.DataTextField = "ShiftTitle";
                ddl.DataValueField = "STId";
                ddl.DataSource = dt;
                ddl.DataBind();
                ddl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void LoadShiftForSMOperation_WithAll(DropDownList dl, string CompanyId, string dptId)
        {
            try
            {
                sqlDB.fillDataTable("Select SftId,SftName From HRD_Shift where IsInitial='false' and CompanyId='" + CompanyId + "' AND DptId='" + dptId + "' order by sftName ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "SftId";
                dl.DataTextField = "SftName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("All", "0"));
            }
            catch { }
        }

        public static bool IsWeekendORHoliday(string Date)
        {
            try
            {
                sqlDB.fillDataTable(";with  WH as(select WeekendDate as WHDate from Attendance_WeekendInfo union select HDate as WHDate from tblHolydayWork ) " +
                    "select WhDate from WH where WHDate='" + Date + "' ", dt = new DataTable());
                if (dt.Rows.Count > 0)
                    return true;
                else return false;
            }
            catch { return false; }
        }

        public static void loadTaxYears(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select convert(varchar(10),TaxId)+'|'+TaxYears+'|'+ convert(varchar(10),OrderNo)+'|'+Convert(varchar,FromMonth)+'|'+Convert(varchar,ToMonth)  as TaxId,TaxYears +' [ '+format(FromMonth,'MM-yyyy')+ ' To ' +format(ToMonth,'MM-yyyy')+' ]'+' [ '+Type+' ]' as TaxYears from v_VatTax_Years where CompanyId='" + CompanyId + "' order by year(SUBSTRING(TaxYears,6,4)) desc, OrderNo desc ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "TaxId";
                dl.DataTextField = "TaxYears";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void loadTaxYearsUnpaid(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select convert(varchar(10),TaxId)+'|'+TaxYears+'|'+ convert(varchar(10),OrderNo)+'|'+Convert(varchar,FromMonth)+'|'+Convert(varchar,ToMonth)  as TaxId,TaxYears +' [ '+format(FromMonth,'MM-yyyy')+ ' To ' +format(ToMonth,'MM-yyyy')+' ]'+' [ '+Type+' ]' as TaxYears from v_VatTax_Years where CompanyId='" + CompanyId + "' And IsPaid=0 order by year(SUBSTRING(TaxYears,6,4)) desc, OrderNo desc ", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "TaxId";
                dl.DataTextField = "TaxYears";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadTaxYearsOnlyYears(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select distinct TaxYears from v_VatTax_Years where CompanyId='" + CompanyId + "'", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "TaxYears";
                dl.DataTextField = "TaxYears";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadGenerateTaxType(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select * from VatTax_GenerateType where CompanyId='" + CompanyId + "' order by Id desc", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "ID";
                dl.DataTextField = "Type";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("Select Type", "00"));
            }
            catch { }
        }

        public static void loadFDRList(DropDownList dl, string CompanyId, string InvestmentType)
        {
            try
            {
                sqlDB.fillDataTable("select FdrID,FdrNo from PF_FDR where CompanyId='" + CompanyId + "' and Type=" + InvestmentType + " order by FdrID desc", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "FdrID";
                dl.DataTextField = "FdrNo";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static void loadFDRListForProfitDistribution(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select PF_FDR.FdrID,FdrNo from PF_FDR_Interest inner join PF_FDR on PF_FDR_Interest.FdrID=PF_FDR.FdrID where CompanyId='" + CompanyId + "' order by ToDate", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "FdrID";
                dl.DataTextField = "FdrNo";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void loadPFExpenseYear(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select convert(varchar,Date)+'/'+convert(varchar,Expense) Date, Year(Date) Year from PF_YearlyExpense where CompanyId='" + CompanyId + "' order by Date desc", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "Date";
                dl.DataTextField = "Year";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static void loadPFInvestmentType(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("select ID,InvestmentType from PF_InvestmentType", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "ID";
                dl.DataTextField = "InvestmentType";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }

        public static string returnBanglaMonth(string month)
        {
            switch (month)
            {
                case "01":
                case "Jan":
                    return "Rvbyqvwi";
                case "02":
                case "Feb":
                    return "‡deªæqvwi";
                case "03":
                case "Mar":
                    return "gvP";
                case "04":
                case "Apr":
                    return "GwcÖj";
                case "05":
                case "May":
                    return "†g";
                case "06":
                case "Jun":
                    return "Ryb";
                case "07":
                case "Jul":
                    return "RyjvB";
                case "08":
                case "Aug":
                    return "AvM÷";
                case "09":
                case "Sep":
                    return "†m‡Þ¤^i";
                case "10":
                case "Oct":
                    return "A†±vei";
                case "11":
                case "Nov":
                    return "b‡f¤^i";
                case "12":
                case "Dec":
                    return "wW‡m¤^i";

                default:
                    return "";
            }
        }
        public static DataTable getAttendanceBonus(string CompanyID)
        {
            da = new SqlDataAdapter("select EmpTypeID,AttBonus from HRD_AttendanceBonus where CompanyId='" + CompanyID + "'", sqlDB.connection);
            da.Fill(dt = new DataTable());
            return dt;
        }

        public static bool notLateForShortleave(string date, string EmpID)
        {
            try
            {
                string[] d = date.Split('-');
                da = new SqlDataAdapter("select LvDate from Leave_ShortLeave where LvDate = '" + d[2] + "-" + d[1] + "-" + d[0] + "'  and FromTime <= (select DATEADD(MINUTE, SftAcceptableLate, SftStartTime) from Personnel_EmployeeInfo as pe inner join HRD_Shift as s on pe.SftId = s.SftId where EmpId = '" + EmpID + "') and EmpId = '" + EmpID + "'  and LvStatus = 1", sqlDB.connection);
                da.Fill(dt = new DataTable());
                if (dt.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch { return false; }


        }
        public static bool isShortleave(string date, string EmpID)
        {
            try
            {

                da = new SqlDataAdapter("select LvDate from Leave_ShortLeave where LvDate = '" + date + "' and EmpId = '" + EmpID + "'  and LvStatus = 1", sqlDB.connection);
                da.Fill(dt = new DataTable());
                if (dt.Rows.Count > 0)
                    return true;
                else
                    return false;
            }
            catch { return false; }


        }
        public static bool IsODAuthority(string CompanyID)
        {
            try
            {
                sqlDB.fillDataTable("select IsODAuthority from HRD_CompanyInfo where CompanyId='" + CompanyID + "'", dt = new DataTable());
                return bool.Parse(dt.Rows[0]["IsODAuthority"].ToString());

            }
            catch { return true; }
        }
        public static void loadDepartmentByAdminForOutDuty(DropDownList dl, string CompanyId, string AuthorityID)
        {
            try
            {
                da = new SqlDataAdapter("select Distinct DptId,DptName from  v_Personnel_EmpCurrentStatus where CompanyId='" + CompanyId + "' and IsActive=1 and ( EmpId in(select distinct EmpID from tblOutDutyAuthorityAccessControl where AuthorityID = " + AuthorityID + ") or EmpId in(select EmpId from UserAccount where UserId=" + AuthorityID + ") )", sqlDB.connection);
                da.Fill(dt = new DataTable());
                dl.DataValueField = "DptId";
                dl.DataTextField = "DptName";
                dl.DataSource = dt;
                dl.DataBind();
                if (dl.Items.Count > 1)
                    dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static string getODAuthorityAction(string AuthorityID, string EmpID)
        {
            try
            {
                sqlDB.fillDataTable("select  AuthorityAction from tblOutDutyAuthorityAccessControl where AuthorityID=" + AuthorityID + " and EmpID='" + EmpID + "'", dt = new DataTable());
                return dt.Rows[0]["AuthorityAction"].ToString();

            }
            catch { return ""; }
        }
        public static string getLvAuthorityAction(string AuthorityID, string EmpID)
        {
            try
            {
                sqlDB.fillDataTable("select  AuthorityAction from tblLeaveAuthorityAccessControl where AuthorityID=" + AuthorityID + " and EmpID='" + EmpID + "'", dt = new DataTable());
                return dt.Rows[0]["AuthorityAction"].ToString();

            }
            catch { return ""; }
        }
        public static bool IsLeaveAuthority(string CompanyID)
        {
            try
            {
                sqlDB.fillDataTable("select IsLeaveAuthority from HRD_CompanyInfo where CompanyId='" + CompanyID + "'", dt = new DataTable());
                return bool.Parse(dt.Rows[0]["IsLeaveAuthority"].ToString());
            }
            catch { return true; }
        }
        public static void loadPFCompany(DropDownList dl)
        {
            try
            {
                sqlDB.fillDataTable("select PFCompanyId,PFCompanyName from PF_CompanyInfo", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "PFCompanyId";
                dl.DataTextField = "PFCompanyName";
                dl.DataBind();

            }
            catch { }
        }
        public static DataTable returnPFMemberInfo(string EmpCardNo)
        {
            try
            {
                sqlDB.fillDataTable("select ProductDB,EmpId from v_PF_MemberListAll where EmpProximityNo='" + EmpCardNo + "' or SUBSTRING(EmpCardNo,8,6)='" + EmpCardNo + "'", dt = new DataTable());
                return dt;

            }
            catch { return null; }
        }
        public static DataTable returnPFMemberInfoByEmpID(string EmpId)
        {
            try
            {
                sqlDB.fillDataTable("select ProductDB,EmpId from v_PF_MemberListAll where EmpID='" + EmpId + "'", dt = new DataTable());
                return dt;

            }
            catch { return null; }
        }
        public static void loadPFInvestment(DropDownList dl, string CompanyId)
        {
            try
            {
                sqlDB.fillDataTable("select FdrID,FdrNo from PF_FDR where CompanyID='" + CompanyId + "' and IsDistributed=0 order by year(ToDate),month(ToDate),day(ToDate)", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "FdrID";
                dl.DataTextField = "FdrNo";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("", "0"));
            }
            catch { }
        }
        public static DataTable getOutDutyApplication(string ODID)
        {
            try {
                string getSQLCMD;
                getSQLCMD = " SELECT SL,EmpId, CompanyName, EmpName, substring(EmpCardNo,8,10) as EmpCardNo, DsgName,convert(varchar(10), Date,105) as Date, Remark, Place, DptName, AssignedBy," +
                    " Address,CONVERT(varchar(15),CAST(InTime AS TIME),100) InTime, CONVERT(varchar(15),CAST(OutTime AS TIME),100) OutTime,ClientName,Purpose,format(AppiedDate,'dd-MM-yyyy hh:mm:ss tt') as  AppiedDate FROM   v_tblOutdutyDetails where SL=" + ODID;
                sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                return dt;
            }
            catch (Exception ex) { return null; }
        }

        public static string getEmpIDByEmpCardNo(string EmpCardNo)
        {
            try {
                sqlCmd = "select EmpId from Personnel_EmployeeInfo where EmpCardNo like '%" + EmpCardNo + "'";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());// check valid employee 
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["EmpId"].ToString();
                }
                else
                {
                    
                    return "";
                }
            }
            catch (Exception ex) { return ""; }
        }
        public static bool IsRosterWeekend(string WDate, string EmpId)
        {
            try {
                sqlCmd = "select * from tblEmpWiseWeekendinfo where Date='"+ WDate + "' and  EmpID='"+ EmpId + "'";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());// check valid employee 
                if (dt != null && dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    
                    return false;
                }
            }
            catch (Exception ex) { return false; }
        }
        public static string GeneralDayInfo(string CompanyId, string Date)
        {
            try
            {
                sqlCmd = "select EmpTypeId from tblGeneralDays where CompanyId='"+ CompanyId + "' and GeneralDay='"+ Date + "'";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());// check valid employee 
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["EmpTypeId"].ToString();
                }
                else
                {

                    return "";
                }
            }
            catch (Exception ex) { return ""; }
        }
        public static string getLastWorkingDay(string EmpId, string Date)
        {
            try
            {
                sqlCmd = "select convert(varchar(10), Max(ATTDate),120) as LastWorkingDay from tblAttendanceRecord where EmpId='"+ EmpId + "'  and  ATTDate <'"+ Date + "' and (PaybleDays=1 or ATTStatus='Lv')";
                sqlDB.fillDataTable(sqlCmd, dt = new DataTable());// check valid employee 
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt.Rows[0]["LastWorkingDay"].ToString();
                }
                else
                {

                    return "";
                }
            }
            catch (Exception ex) { return ""; }
        }
        public static bool isWH(string date)
        {
            try
            {
                sqlDB.fillDataTable("select WeekendDate from Attendance_WeekendInfo where WeekendDate ='" + date + "'", dt = new DataTable());
                if (dt.Rows.Count > 0)
                    return true;
                else
                {
                    sqlDB.fillDataTable("select HDate from tblHolydayWork where HDate ='" + date + "'", dt = new DataTable());
                    if (dt.Rows.Count > 0)
                        return true;
                    else
                        return false;
                }

            }
            catch (Exception ex) { return false; }

        }

        public static void loadReportName(DropDownList dl, int [] permissionID)
        {
            string rpermissionId = string.Join(",", permissionID);
            try
            {
                sqlDB.fillDataTable("select userPermId,PermissionName from userpermission where userPermId in("+ rpermissionId + ")", dt = new DataTable());
                dl.DataSource = dt;
                dl.DataValueField = "userPermId";
                dl.DataTextField = "PermissionName";
                dl.DataBind();
                dl.Items.Insert(0, new ListItem("---Select---", "0"));
            }
            catch { }
        }

    }
}