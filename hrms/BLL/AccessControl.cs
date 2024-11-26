using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace SigmaERP.hrms.BLL
{
    public static class AccessControl
    {
        
        public static int[] hasPermission(int[] PagePermissions)
        {
            if (HttpContext.Current.Session["__ActualPermission__"] == null)
            {
                HttpContext.Current.Response.Redirect("hrms/login");
            }


            int[] verifiedPermission;
            int[] userPermissionArray = HttpContext.Current.Session["__ActualPermission__"].ToString().Split(',').Select(int.Parse).ToArray();
            verifiedPermission = PagePermissions.Intersect(userPermissionArray).ToArray();

            return verifiedPermission;
        }

        public static void checkPermission(int userId)
        {
            string query = "select ur.Permissions,u.AdditionalPermissions,u.RemovedPermissions from userRoles ur inner join users u on ur.userroleId=u.userroleId where u.UserId="+userId;
            DataTable dt = CRUD.ExecuteReturnDataTable(query);
            DataRow dataRow = dt.Rows[0];
            string permissions = dataRow["Permissions"].ToString();
            string AdditionalPermissions = dataRow["AdditionalPermissions"].ToString();
            string RemovedPermissions = dataRow["RemovedPermissions"].ToString();
            string actualPerm = actualPermission(permissions, AdditionalPermissions, RemovedPermissions);
            HttpContext.Current.Session["__ActualPermission__"] = actualPerm;
        }

        private static string actualPermission(string permissions, string AdditionalPermissions, string RemovedPermissions)
        {
            List<int> addPermissionList = new List<int>();
            List<int> removePermissionList = new List<int>();


            List<int> permissionList = permissions.Trim('[', ']').Split(',').Select(int.Parse).ToList();
            if (!string.IsNullOrEmpty(AdditionalPermissions))
                addPermissionList = AdditionalPermissions.Trim('[', ']').Split(',').Select(int.Parse).ToList();
            if (!string.IsNullOrEmpty(RemovedPermissions))
                removePermissionList = RemovedPermissions.Trim('[', ']').Split(',').Select(int.Parse).ToList();
            foreach (int perm in addPermissionList)
            {
                if (!permissionList.Contains(perm))
                {
                    permissionList.Add(perm);
                }
            }
            foreach (int perm in removePermissionList)
            {
                permissionList.Remove(perm);
            }

            string actualPermission = string.Join(",", permissionList);
            return actualPermission;
        }



    

        public static void  getDataAccessPermission(string userID)
        {
            string query = "select DataAccessPermission,ur.DataAccessLevel  from userroles ur left join users u on ur.UserRoleID=u.UserRoleID where u.userId='"+ userID + "'";
            DataTable dt=CRUD.ExecuteReturnDataTable(query);
            string dataAcceslevel = dt.Rows[0]["DataAccessLevel"].ToString();
            string permissions = dt.Rows[0]["DataAccessPermission"].ToString();

            permissions = permissions.Trim('[', ']');
            string[] permissionsArray = permissions
                .Split(',')
                .Select(p => p.Trim().Trim('"')) 
                .ToArray();
            HttpContext.Current.Session["__dataAccesPemission__"] = string.Join(",", permissionsArray.Select(p => $"'{p}'"));
            HttpContext.Current.Session["__dataAceesLevel__"]=dataAcceslevel;


        }


    
        public static string getDataAccessCondition(string companyId,string ddlDepartment)
        {


            string condition = " CompanyId='" + companyId + "'";
            //for all
            if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "3")
            {
                if (ddlDepartment == "0")
                {
                    condition += "";
                }
                else
                {
                    condition += "and DptId in(" + ddlDepartment + ") ";
                }

                   
            }  

            //custom
            else if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "4")
            {
                if (ddlDepartment == "0")
                {
                    if (HttpContext.Current.Session["__isGuestUser__"].ToString() == "True")
                    {
                        condition += "and DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ")";
                    }
                    else
                    {
                        condition += "and DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ") or  EmpId='" + HttpContext.Current.Session["__empId__"].ToString() + "' ";
                    }
                }
                else
                {
                    condition += "and DptId in(" + ddlDepartment + ") ";
                }

               
            }
            //for own department 
            else if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "2") 
            {
                condition += "and DptId='" + HttpContext.Current.Session["__dptId__"] + "'";
            }
            //only me
            else
            {
                condition += "and EmpId='" + HttpContext.Current.Session["__empId__"].ToString() + "'";
            }

            return condition;
        }

        //private static string HandleCustomAccess(string ddlDepartment)
        //{
        //    if (ddlDepartment == "0")
        //    {
        //        if (HttpContext.Current.Session["__isGuestUser__"].ToString() == "True")
        //        {
        //            return " and DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ")";
        //        }
        //        else
        //        {
        //            return  " and DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ") or  EmpId='" + HttpContext.Current.Session["__empId__"].ToString() + "' ";
        //        }
        //    }
        //    else
        //    {
        //        return "and DptId in(" + ddlDepartment + ") ";
        //    }
        //}
        public static string  loadEmpCardNumber(string CompanyId)
        {
            string conditon = " CompanyId = '" + CompanyId + "'";
            if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "3")
            {
                conditon += "";  //for all department 
            }
           else if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "4")
            {
                conditon += "and DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ") or EmpId='" + HttpContext.Current.Session["__empId__"].ToString() + "'";  //for custom and own department
            }

            else if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "2")
            {
                conditon += " and DptId in(" + HttpContext.Current.Session["__dptId__"].ToString() + ") ";
            }
            return conditon;
        }


        public static string  loadDepartmetCondition(string CompanyId)
        {
            string conditon = " CompanyId = '" + CompanyId + "'";
            if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "3")
            {
                conditon += "";  //for all department 
            }
            else if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "4")
            {
                conditon += " and DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ") ";  //for custom 
            }
            else if(HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "2") // own department
            {
                conditon += " and DptId in(" + HttpContext.Current.Session["__dptId__"].ToString() + ") ";
            }
            return conditon;
        }

        public static string hasOwnEmpIdWithOtherDepartment()
        {
            // Fetch the session value dynamically when this method is called
            string hasOtherDeptAccessButNotOwn =" or  EmpId='" + HttpContext.Current.Session["__GetEmpId__"].ToString() + "'";
            return hasOtherDeptAccessButNotOwn;

    }


        public static bool hasEmpcardPermission(string empCard,string companyList)
        {
            string conditon = " CompanyId in( '" + companyList + "')";
            if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "3")
            {
                conditon += "";  //for all department 
            }
            else if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "4")
            {
                conditon += "and DptId in(" + HttpContext.Current.Session["__dataAccesPemission__"].ToString() + ") or EmpId='" + HttpContext.Current.Session["__empId__"].ToString() + "'";  //for custom
            }

            else if (HttpContext.Current.Session["__dataAceesLevel__"].ToString() == "2")   //for own department
            {
                conditon += " and DptId in(" + HttpContext.Current.Session["__dptId__"].ToString() + ") ";
            }
            string query = "select DptId,EmpcardNO from  v_Personnel_EmpCurrentStatus where EmpcardNO like '%"+ empCard + "' and "+ conditon + "";
            DataTable dt = CRUD.ExecuteReturnDataTable(query);
            if (dt.Rows.Count > 0)
            {
                return false;
            }
            return true;
        }

}
}