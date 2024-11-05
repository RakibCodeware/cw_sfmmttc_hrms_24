using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace SigmaERP.classes
{
    public static class Roster
    {
         static string query="";
        
        public static int saveShiftTransfer(string fDate,string tDate,string shiftId,string dptID,string companyID,string groupID)
        {
            try
            {
                query = @"
INSERT INTO [dbo].[ShiftTransferInfo]
           ([SftId]
           ,[TFromDate]
           ,[TToDate]          
           ,[EntryDate]
           ,[DptId]
           ,[CompanyId]
           ,[GId])
     VALUES
           (" + shiftId + ",'" + fDate + "','" + tDate + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + dptID + "','" + companyID + "','" + groupID + "');" +
                      "SELECT SCOPE_IDENTITY()";
                return CRUD.ExecuteReturnID(query);               

            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static void saveShiftTransferDetails(string fDate,string tDate, string STId, string EmpId,string DptId, string CompanyId, string GId, string FId, string Notes)
        {
            try
            {
                string[] GetDsgID_EmpTypeId = DsgId_EmpTypeId(EmpId).Split('|');
                DateTime FromDate = DateTime.Parse(fDate);
                DateTime ToDate = DateTime.Parse(tDate);                
                while (FromDate <= ToDate)
                {
                    try
                    {
                        //--> delete the existing data----
                        CRUD.Execute("delete from ShiftTransferInfoDetails where EmpId='" + EmpId + "' AND SDate='" + FromDate.ToString("yyyy-MM-dd") + "' AND STId=" + STId + "");
                        //--< delete the existing data-----
                        query = @"INSERT INTO [dbo].[ShiftTransferInfoDetails]
                               ([STId]
                               ,[SDate]
                               ,[EmpId]
                               ,[DptId]
                               ,[DsgId]
                               ,[EmpTypeId]
                               ,[CompanyId]
                               ,[FId]
                               ,[IsWeekend]
                               ,[Notes]
                               ,[GId])
                                VALUES("+STId+",'"+ FromDate.ToString("yyyy-MM-dd") + "','"+ EmpId + "','"+ DptId + "','"+ GetDsgID_EmpTypeId[0] + "','"+ GetDsgID_EmpTypeId[1] + "','"+ CompanyId + "','"+ FId + "','"+ CheckIsOffDay(EmpId, FromDate.ToString("yyyy-MM-dd")).ToString() + "','"+ Notes + "','"+ GId + "')";
                        CRUD.Execute(query);
                    }
                    catch (Exception ex) { InsertToMissingLog(STId, EmpId, FromDate, ex.Message); }
                    FromDate = FromDate.AddDays(1);

                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }

        }
        private static void InsertToMissingLog(string STID, string EmpID, DateTime Date, string Error)
        {
            try
            {
                query = @"INSERT INTO [dbo].[ShiftTransferDetailsMissingLog]
           ([EmpID]           
           ,[Date]
           ,[InsertTime]
           ,[Error]
           ,[STID])
     VALUES
           ('" + EmpID + "','" + Date.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + Error.Replace("'", "") + "'," + STID + ")";
                CRUD.Execute(query);

            }
            catch (Exception ex) { }
        }
        private static bool CheckIsOffDay(string EmpId, string HWDate)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select SL From tblHolydayWeekentEmployeeWiseDetails where EmpId='" + EmpId + "' AND HWDate='" + HWDate + "'");
                if (dt.Rows.Count > 0) return true;
                else return false;
            }
            catch { return false; }
        }
        private static  string DsgId_EmpTypeId(string EmpId)
        {
            try
            {
                DataTable dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select DsgId,EmpTypeId from Personnel_EmpCurrentStatus where EmpId='" + EmpId + "' and IsActive=1");
              
                return dt.Rows[0]["DsgId"].ToString() + '|' + dt.Rows[0]["EmpTypeId"].ToString();
            }
            catch { return null; }
        }

    }    
}