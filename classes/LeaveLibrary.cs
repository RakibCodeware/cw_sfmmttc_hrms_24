using adviitRuntimeScripting;
using ComplexScriptingSystem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SigmaERP.classes
{
    public class LeaveLibrary
    {
        public static bool viewLeaveApplication(string LaCode)
        {
            try {
                string getSQLCMD;
                DataTable dt = new DataTable();
                DataTable dtApprovedRejectedDate = new DataTable();
                getSQLCMD = " SELECT LACode,EmpId, format(FromDate,'dd-MM-yyyy') as FromDate, format(ToDate,'dd-MM-yyyy') as ToDate, TotalDays,"
                    + "  Remarks, LeaveName, DsgName, DptName, CompanyName,format(EmpJoiningDate,'dd-MM-yyyy') as EmpJoiningDate, SUBSTRING(EmpCardNo,10,6) as EmpCardNo, "
                    + " EmpName, Address, LvAddress, LvContact, CompanyId, DptId, format(ApplyDate,'dd-MM-yyyy') as ApplyDate "
                    + " FROM"
                    + " dbo.v_Leave_LeaveApplication"
                    + " where LACode=" + LaCode + "";
                sqlDB.fillDataTable(getSQLCMD, dt);
                if (dt.Rows.Count == 0)
                {
                    return false;
                 //   lblMessage.InnerText = "warning->No data found."; return;
                }
                string EmpID = dt.Rows[0]["EmpId"].ToString();
                string[] FDate = dt.Rows[0]["FromDate"].ToString().Split('-');
                HttpContext.Current.Session["__Language__"] = "English";
                HttpContext.Current.Session["__LeaveApplication__"] = dt;
                getSQLCMD = "with pEL as (select EmpID, ReserveEeanLeaveDays from Earnleave_Reserved where ReserveFor = '" + FDate[2] + "-01-01' and EmpId = '" + EmpID + "'),bEL as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + FDate[2] + "-01-01' and GenerateDate<= '" + FDate[2] + "-12-31' and EmpId = '" + EmpID + "' group by EmpID), bEL1 as (select ei.EmpId,ISNULL(pEL.ReserveEeanLeaveDays, 0) + ISNULL(bEL.EarnLeaveDays, 0) as bEL from Personnel_EmployeeInfo ei left join pEL on ei.EmpId = pEL.EmpId left join bEL on ei.EmpId = bEL.EmpID where ei.EmpID = '"+ EmpID + "'), lvd as ( select CompanyId, Leaveid, ShortName,count(ShortName) as Amount,case when Sex='Male' then 'm/l'else '' end Sex " +
                     " from v_Leave_LeaveApplicationDetails where EmpId='" + EmpID + "' and IsApproved=1 and LeaveDate>='" + FDate[2] + "-01-01' and LeaveDate<'" + FDate[2] + "-" + FDate[1] + "-" + FDate[0] + "'" +
                     " group by CompanyId,Leaveid, ShortName,Sex),pcs as (select case when Sex='Male' then 'm/l'else '' end Sex,CompanyId from v_EmployeeDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsActive=1 ) ,lc as ( select LeaveId,LeaveName,ShortName,case when ShortName='a/l' then bEL else  LeaveDays end as LeaveDays,LeaveNature,IsDeductionAllowed,CompanyId  from tblLeaveConfig cross join bEL1 where CompanyId=(select CompanyId from pcs)) ," +
                     " la as(select  LeaveId,TotalDays from Leave_LeaveApplication where LACode=" + LaCode + ")" +
                     " select lc.ShortName,ISNULL(lvd.Amount,0) as Amount,lc.LeaveDays,lc.CompanyId,lc.LeaveName,( lc.LeaveDays-ISNULL(lvd.Amount,0) )as Remaining,TotalDays Applied from  lc left join lvd on lc.LeaveId=lvd.LeaveId or lc.ShortName=lvd.ShortName and lc.CompanyId=lvd.CompanyId  left join la on lc.LeaveId=la.LeaveId " +
                     " where  lc.ShortName not in('sr/l',(select   Sex from pcs))";
                sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                HttpContext.Current.Session["__LeaveCurrentStatus__"] = dt;
                return true;               
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool viewLeaveApplication_Rejected(string LaCode)
        {
            try
            {
                string getSQLCMD;
                DataTable dt = new DataTable();
                DataTable dtApprovedRejectedDate = new DataTable();
                getSQLCMD = " SELECT LACode,EmpId, format(FromDate,'dd-MM-yyyy') as FromDate, format(ToDate,'dd-MM-yyyy') as ToDate, TotalDays,"
                    + "  Remarks, LeaveName, DsgName, DptName, CompanyName,format(EmpJoiningDate,'dd-MM-yyyy') as EmpJoiningDate, SUBSTRING(EmpCardNo,10,6) as EmpCardNo, "
                    + " EmpName, Address, LvAddress, LvContact, CompanyId, DptId, format(ApplyDate,'dd-MM-yyyy') as ApplyDate "
                    + " FROM"
                    + " dbo.v_Leave_LeaveApplication_Log"
                    + " where LACode=" + LaCode + "";
                sqlDB.fillDataTable(getSQLCMD, dt);
                if (dt==null || dt.Rows.Count == 0)
                {
                    return false;
                }
                string EmpID = dt.Rows[0]["EmpId"].ToString();
                string[] FDate = dt.Rows[0]["FromDate"].ToString().Split('-');
                HttpContext.Current.Session["__Language__"] = "English";
                HttpContext.Current.Session["__LeaveApplication__"] = dt;
                getSQLCMD = "with pEL as (select EmpID, ReserveEeanLeaveDays from Earnleave_Reserved where ReserveFor = '" + FDate[2] + "-01-01' and EmpId = '" + EmpID + "'),bEL as (select EmpID, isnull(sum(EarnLeaveDays), 0) as EarnLeaveDays from Earnleave_BalanceDetailsLog where GenerateDate >= '" + FDate[2] + "-01-01' and GenerateDate<= '" + FDate[2] + "-12-31' and EmpId = '" + EmpID + "' group by EmpID), bEL1 as (select ei.EmpId,ISNULL(pEL.ReserveEeanLeaveDays, 0) + ISNULL(bEL.EarnLeaveDays, 0) as bEL from Personnel_EmployeeInfo ei left join pEL on ei.EmpId = pEL.EmpId left join bEL on ei.EmpId = bEL.EmpID where ei.EmpID = '" + EmpID + "'), lvd as ( select CompanyId, Leaveid, ShortName,count(ShortName) as Amount,case when Sex='Male' then 'm/l'else '' end Sex " +
                     " from v_Leave_LeaveApplicationDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsApproved=1 and LeaveDate>='" + FDate[2] + "-01-01' and LeaveDate<'" + FDate[2] + "-" + FDate[1] + "-" + FDate[0] + "'" +
                     " group by CompanyId,Leaveid, ShortName,Sex),pcs as (select case when Sex='Male' then 'm/l'else '' end Sex,CompanyId from v_EmployeeDetails where EmpId='" + dt.Rows[0]["EmpId"].ToString() + "' and IsActive=1 ) ,lc as (select LeaveId,LeaveName,ShortName,case when ShortName='a/l' then bEL else  LeaveDays end as LeaveDays,LeaveNature,IsDeductionAllowed,CompanyId  from tblLeaveConfig cross join bEL1 where CompanyId=(select CompanyId from pcs)) ," +
                     " la as(select  LeaveId,TotalDays from Leave_LeaveApplication_Log where LACode=" + LaCode + ")" +
                     " select lc.ShortName,ISNULL(lvd.Amount,0) as Amount,lc.LeaveDays,lc.CompanyId,lc.LeaveName,( lc.LeaveDays-ISNULL(lvd.Amount,0) )as Remaining,TotalDays Applied from  lc left join lvd on lc.LeaveId=lvd.LeaveId or lc.ShortName=lvd.ShortName and lc.CompanyId=lvd.CompanyId  left join la on lc.LeaveId=la.LeaveId " +
                     " where  lc.ShortName not in('sr/l',(select   Sex from pcs))";
                sqlDB.fillDataTable(getSQLCMD, dt = new DataTable());
                HttpContext.Current.Session["__LeaveCurrentStatus__"] = dt;
                return true;

            }
            catch (Exception ex){ return false; }
        }
        public static string GetLeaveFormSerialNo()
        {
            try
            {
                DataTable dt=new DataTable();
                sqlDB.fillDataTable("select ShortName from HRD_CompanyInfo",dt=new DataTable ());
                string setLFSL=dt.Rows[0]["ShortName"].ToString()+"-";
                dt = new DataTable();
                SQLOperation.selectBySetCommandInDatatable("select Max(convert(int,RIGHT(LeaveFormSLNo,4))) as LeaveFormSLNo from Leave_LeaveApplication "+
                    " where LeaveFormSLNo like '%"+DateTime.Now.Year+"%'",dt,sqlDB.connection);
                if (dt.Rows[0]["LeaveFormSLNo"].ToString().Trim().Length == 0) return setLFSL += DateTime.Now.Year + "-0001";

                int getLFSL = Convert.ToInt32(dt.Rows[0]["LeaveFormSLNo"].ToString()) + 1;
                if (getLFSL.ToString().Length == 1) return  setLFSL += DateTime.Now.Year + "-000"+getLFSL;
                else if (getLFSL.ToString().Length == 2) return  setLFSL += DateTime.Now.Year + "-00"+getLFSL;
                else if (getLFSL.ToString().Length == 3) return  setLFSL += DateTime.Now.Year + "-0" + getLFSL;
                else if (getLFSL.ToString().Length == 4) return setLFSL += DateTime.Now.Year + "-" + getLFSL;

                return null;
            }
            catch { return null; }
        }

        public static void LeaveCount(string AttDate,string LACode)
        {
            try
            {
                SqlCommand cmd; DataTable dt = new DataTable();
                // find Todate of this leave
                sqlDB.fillDataTable("select FORMAT(ToDate,'yyyy-MM-dd') as ToDate,LeaveId,LeaveName,LACode from v_Leave_LeaveApplication where LACode=" + LACode + "", dt);

                // if Todate is equal of current select days then below code is execute
                if (dt.Rows.Count>0)
                if (AttDate.Equals(dt.Rows[0]["ToDate"].ToString()))
                {
                    cmd = new System.Data.SqlClient.SqlCommand("Update Leave_LeaveApplication set IsProcessessed='0' where LACode= " + dt.Rows[0]["LACode"].ToString() + "", sqlDB.connection);
                    cmd.ExecuteNonQuery();

                }

                // for changed used status for leave 
                cmd = new System.Data.SqlClient.SqlCommand("Update Leave_LeaveApplicationDetails set used='1' where LeaveDate='" + AttDate + "' AND LACode=" + dt.Rows[0]["LACode"].ToString() + "", sqlDB.connection);
                cmd.ExecuteNonQuery();               
            }
            catch { }

        }
    }
}