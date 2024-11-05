using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SigmaERP.hrms.DTO
{
    public class RouteDTO
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public string Url { get; set; }
        public string PhysicalLocation { get; set; }
    }
}