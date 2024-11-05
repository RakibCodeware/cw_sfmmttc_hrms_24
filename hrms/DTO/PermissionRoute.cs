using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SigmaERP.hrms.DTO
{
    public class PermissionRoute
    {
        public int UserPermId { get; set; }
        public int ModuleID { get; set; }
        public string PermissionName { get; set; }
        public string Url { get; set; } = "";
        public string PhysicalLocation { get; set; } = "";
    }
}