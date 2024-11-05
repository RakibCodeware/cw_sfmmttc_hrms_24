using SigmaERP.hrms.repo.repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace SigmaERP.hrms.repo.services
{
    public class PunchService
    {
        private readonly IDevice _device;

        public PunchService(IDevice device)
        {
            _device = device;
        }

        public DataTable GetPunchData(string ProcessingID, string CompanyID, string CardNo, DateTime ShiftPunchCountStartTime, DateTime ShiftPunchCountEndTime)
        {
            return _device.GetPunch(ProcessingID, CompanyID, CardNo, ShiftPunchCountStartTime, ShiftPunchCountEndTime);
        }
    }

}