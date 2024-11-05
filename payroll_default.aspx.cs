using ComplexScriptingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP
{
    public partial class payroll_nested1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            try
            {
                if (!IsPostBack)
                {
                    HttpCookie getCookies = Request.Cookies["userInfo"];
                    if (getCookies == null || getCookies.Value == "")
                    {
                        Response.Redirect("~/ControlPanel/Login.aspx");

                    }
                    if (getCookies["__IsCompliance__"].ToString().Equals("True"))
                    {
                        divAdvance.Visible = false;
                        //divBonus.Visible = false;
                        divPF.Visible = false;
                        divVat.Visible = false;
                        divSignatureEntry.Visible = true;
                    }
                    else
                    {
                        //if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()) != "Master Admin")
                        //{
                        //    divAdvance.Visible = false;
                        //    divPF.Visible = false;
                        //}
                            divSignatureEntry.Visible = false;
                    }
                        
                  
                }




            }
            catch (Exception ex)
            {
                Response.Redirect("~/ControlPanel/Login.aspx");
            }
        }
    }
}