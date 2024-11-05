using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrms
{
    public partial class HRMS : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                try
                {
                    //string url = "attendance/month-setup";


                    HttpCookie getCookies = Request.Cookies["userInfo"];
                    if (getCookies == null || getCookies.Value == "")
                    {
                        // Response.Redirect("~/ControlPanel/Login.aspx");
                        // Response.Redirect("~/hrms/UI/auth/login.aspx");                        
                        Response.RedirectToRoute(Routing.LoginRouteName);
                    }
                    else
                    {

                        ////UserName.InnerText = getCookies["__getFirstName__"].ToString();
                        UserName.InnerText = Session["__GetUserFullName__"].ToString();
                        ViewState["__getUserId__"] = getCookies["__getUserId__"].ToString();
                        Session["__GetUID__"] = ViewState["__getUserId__"].ToString();
                        UserEmail.InnerText = Session["__UserEmailText__"].ToString();
                        //UserType.InnerText = "  " + ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString());
                        //  GSName.InnerText = getCookies["__CompanyName__"].ToString();

                        Session["__GetCompanyId__"] = ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                        Session["__isLvAuthority__"] = getCookies["__isLvAuthority__"].ToString();
                        Session["__LvOnlyDpt__"] = getCookies["__LvOnlyDpt__"].ToString();
                        Session["__LvEmpType__"] = getCookies["__LvEmpType__"].ToString();
                        Session["__IsCompliance__"] = getCookies["__IsCompliance__"].ToString();
                        Session["__UserNameText__"] = getCookies["__UserNameText__"].ToString();

                        if (Session["__IsCompliance__"].ToString().Equals("True"))
                        {
                            try
                            {
                                string[] path = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
                                string page = path[path.Length - 1].ToString();
                                string[] pages = { "Notification.aspx", "default.aspx", "payroll_default.aspx", "salary_index.aspx", "leave_default.aspx", "attendance_default.aspx", "employee_index.aspx", "personnel_defult.aspx", "payroll_generation1.aspx", "salary_sheet_Report.aspx", "holyday1.aspx", "monthly_in_out_report.aspx", "monthly_setup1.aspx", "pay_slip.aspx", "separation_generation_rss.aspx", "ManpowerStatement.aspx", "earnleave_payment_generationc.aspx", "earnleave_payment_sheetc.aspx", "earnleave_generationc.aspx", "separationc.aspx", "seperation_sheetc.aspx", "promotionc.aspx", "promotion_sheetc.aspx", "increment_sheetc.aspx", "salary_incrementc.aspx", "AutoIncrementPanel.aspx", "payroll_entry_panelc.aspx", "bonus_index.aspx", "bonus_setupc.aspx", "bonus_month_setupc.aspx", "bonus_generationc.aspx", "bonus_sheet_Reportc.aspx", "job_card.aspx", "Signatures.aspx" };
                                if (!pages.Contains(page))
                                    Response.Redirect("~/default.aspx");

                            }
                            catch { Response.Redirect("~/default.aspx"); }
                        }
                        else if (Session["__UserNameText__"].ToString() == "common")
                        {
                            try
                            {
                                string[] path = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
                                string page = path[path.Length - 1].ToString();
                                string[] pages = { "default.aspx", "attendance_default.aspx", "out_duty_app.aspx", "out_duty_list.aspx", "leave_default.aspx", "aplication.aspx" };
                                if (!pages.Contains(page))
                                    Response.Redirect("/default.aspx");

                            }
                            catch { Response.Redirect("~/default.aspx"); }
                        }
                        else
                        {

                            string[] path = HttpContext.Current.Request.Url.AbsolutePath.Split('/');
                            string page = path[path.Length - 1].ToString();
                            if (page == "Signatures.aspx")
                                Response.Redirect("/default.aspx");

                            if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()) != "Master Admin")
                            {
                                string[] pages = { "pf_index.aspx", "pf_settings.aspx", "pfentrypanel.aspx", "pf_ManualEntry.aspx", "pf_FDR.aspx", "pf_interestentry.aspx", "pf_YearlyExpense.aspx", "pf_interest_distribution.aspx", "pf_report.aspx", "pf_withdraw.aspx",
                                    "advance_index.aspx", "advance_entry.aspx", "advance_entry_final.aspx", "advance_monthly_installment_setup.aspx","advance_info.aspx", "advance_list.aspx"};
                                if (pages.Contains(page))
                                    Response.Redirect("/default.aspx");
                            }

                        }



                    }


                }
                catch (Exception ex)
                {
                    //Response.Redirect("~/ControlPanel/Login.aspx");
                    // Response.Redirect("~/hrms/UI/auth/login.aspx");
                    Response.RedirectToRoute(Routing.LoginRouteName);
                }
            }
        }

        //        public bool IsRouteExists(string url)
        //        {
        ///*            url = url.().ToLower(); */ // Normalize the input URL
        //            foreach (Route route in RouteTable.Routes)
        //            {
        //                var routeUrl = route.Url?.Trim().ToLower();  // Normalize each route URL
        //                if (!string.IsNullOrEmpty(routeUrl) && routeUrl.Equals(url))  // Use Equals instead of Contains
        //                {
        //                    return true;
        //                }
        //            }
        //            return false;
        //        }
        public bool IsRouteExists(string url)
        {
            foreach (Route route in RouteTable.Routes)
            {
                var routeUrl = route.Url?.ToLower();
                if (!string.IsNullOrEmpty(routeUrl) && routeUrl.Contains(url.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();
                HttpCookie setCookies = new HttpCookie("userInfo");
                setCookies.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(setCookies);
                FormsAuthentication.SignOut();
                // Response.Redirect("~/ControlPanel/Login.aspx",false);
                Response.Redirect("/hrms/login", false);

            }
            catch (Exception ex) { }
        }
    }
}