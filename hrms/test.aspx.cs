using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrms
{
    public partial class test : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var modules = GetModules(); // Your method to fetch modules
                var menu = BuildMenu(modules); // Build MenuItem list
                var menuHtml = BuildMenuHtml(menu); // Convert to HTML

                ltMenu.Text = menuHtml; // Set the HTML to Literal
            }
        }
        public class Module
        {
            public int ModuleID { get; set; }
            public int ParentId { get; set; }
            public string Name { get; set; }
            public bool IsPermission { get; set; }
            public List<Module> Children { get; set; } = new List<Module>();
            public string Url { get; set; } // Add URL property here
        }


        public class Permission
        {
            public int PermissionId { get; set; }
            public string Name { get; set; }
            public bool IsPermission { get; set; }
            public int ModuleID { get; set; }
        }

        public class MenuItem
        {
            public string Name { get; set; }
            public List<MenuItem> Children { get; set; } = new List<MenuItem>();
            public string Url { get; set; }
        }

        public List<Module> GetModules()
        {
            return new List<Module>
    {
        new Module
        {
            ModuleID = 191,
            ParentId = 0,
            Name = "Settings",
            IsPermission = false,
            Url = "/Settings.aspx", // Set the URL for the Settings module
            Children = new List<Module>
            {
                new Module
                {
                    ModuleID = 192,
                    ParentId = 191,
                    Name = "Attendance",
                    IsPermission = false,
                    Url = "attendance/month-setup", // Set the URL for the Department module
                    Children = new List<Module>
                    {
                        new Module
                        {
                            ModuleID = 193,
                            ParentId = 192,
                            Name = "View",
                            IsPermission = false,
                            Url = "/ViewDepartment.aspx" // Set the URL for View in Department
                        }
                    }
                },
                new Module
                {
                    ModuleID = 194,
                    ParentId = 191,
                    Name = "Designation",
                    IsPermission = false,
                    Url = "/Designation.aspx", // Set the URL for the Designation module
                    Children = new List<Module>
                    {
                        new Module
                        {
                            ModuleID = 195,
                            ParentId = 194,
                            Name = "View",
                            IsPermission = true,
                            Url = "/ViewDesignation.aspx" // Set the URL for View in Designation
                        }
                    }
                }
            }
        },
                new Module
        {
            ModuleID = 191,
            ParentId = 0,
            Name = "Payroll",
            IsPermission = false,
            Url = "/Settings.aspx", // Set the URL for the Settings module
            Children = new List<Module>
            {
                new Module
                {
                    ModuleID = 192,
                    ParentId = 191,
                    Name = "Attendance",
                    IsPermission = false,
                    Url = "attendance/month-setup", // Set the URL for the Department module
                    Children = new List<Module>
                    {
                        new Module
                        {
                            ModuleID = 193,
                            ParentId = 192,
                            Name = "View",
                            IsPermission = false,
                            Url = "/ViewDepartment.aspx" // Set the URL for View in Department
                        }
                    }
                },
                new Module
                {
                    ModuleID = 194,
                    ParentId = 191,
                    Name = "Designation",
                    IsPermission = false,
                    Url = "/Designation.aspx", // Set the URL for the Designation module
                    Children = new List<Module>
                    {
                        new Module
                        {
                            ModuleID = 195,
                            ParentId = 194,
                            Name = "View",
                            IsPermission = true,
                            Url = "/ViewDesignation.aspx" // Set the URL for View in Designation
                        }
                    }
                }
            }
        }
    };

    }


        public string BuildMenuHtml(List<MenuItem> menuItems)
        {
            var html = new StringBuilder();

            html.Append("<ul>");

            foreach (var menuItem in menuItems)
            {
                // Use the URL if it is set; otherwise, just display the name
                html.Append($"<li><a href='{menuItem.Url}'>{menuItem.Name}</a>");

                if (menuItem.Children != null && menuItem.Children.Count > 0)
                {
                    html.Append(BuildMenuHtml(menuItem.Children));
                }

                html.Append("</li>");
            }

            html.Append("</ul>");

            return html.ToString();
        }



        public List<MenuItem> BuildMenu(List<Module> modules)
        {
            var menu = new List<MenuItem>();

            foreach (var module in modules)
            {
                if (!module.IsPermission)
                {
                    var menuItem = new MenuItem { Name = module.Name };

                    if (module.Children != null && module.Children.Count > 0)
                    {
                        menuItem.Children = BuildMenu(module.Children);
                    }

                    menu.Add(menuItem);
                }
            }

            return menu;
        }
    }
}