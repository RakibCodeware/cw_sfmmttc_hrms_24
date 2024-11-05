using SigmaERP.classes;
using SigmaERP.hrms.repo.repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SigmaERP.hrms.repo.implementations
{
    public class ZkBiotime : IDevice
    {
        DataTable IDevice.GetPunch(string ProcessingID, string CompanyID, string CardNo, DateTime ShiftPunchCountStartTime, DateTime ShiftPunchCountEndTime)
        {
            try
            {

                // ZkBiotime
                DataTable dt = new DataTable();
                return CRUD.ExecuteReturnDataTable("select  emp_code as CardNo,FORMAT(punch_time,'yyyy-MM-dd HH:mm:ss') as PunchTime from zkbiotime.dbo.iclock_transaction where punch_time >='" + ShiftPunchCountStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and punch_time <='" + ShiftPunchCountEndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'  and emp_code='" + CardNo + "'  order by punch_time FORMAT(punch_time,'yyyy-MM-dd HH:mm:ss')");
            }
            catch (Exception ex) { return null; }
        }
    }
}