using SigmaERP.classes;
using SigmaERP.hrms.repo.repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SigmaERP.hrms.repo.implementations
{
    public class Zk : IDevice
    {
        DataTable IDevice.GetPunch(string ProcessingID, string CompanyID, string CardNo, DateTime ShiftPunchCountStartTime, DateTime ShiftPunchCountEndTime)
        {
            try
            {

                // default att2000 [Old zk]
                DataTable dt = new DataTable();
                return CRUD.ExecuteReturnDataTable("select  distinct u.BADGENUMBER as CardNo,format(c.CHECKTIME,'yyyy-MM-dd HH:mm:ss') as PunchTime from cw_att_zk.dbo.CHECKINOUT c inner join cw_att_zk.dbo.USERINFO u on c.USERID=u.USERID where c.CHECKTIME>='" + ShiftPunchCountStartTime.ToString("yyyy-MM-dd HH:mm:ss") + "' and c.CHECKTIME<='" + ShiftPunchCountEndTime.ToString("yyyy-MM-dd HH:mm:ss") + "'  AND u.BADGENUMBER='" + CardNo + "' order by format(c.CHECKTIME,'yyyy-MM-dd HH:mm:ss')");
            }
            catch (Exception ex) { return null; }
        }
    }
}