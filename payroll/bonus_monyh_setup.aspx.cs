using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using SigmaERP.hrms.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.payroll
{
    public partial class bonud_monyh_setup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            ViewState["__ReadAction__"] = "0";
            ViewState["__WriteAction__"] = "0";
            ViewState["__DeletAction__"] = "0";
            int[] pagePermission = { 398, 399, 400 };
            if (!IsPostBack)
            {

                int[] userPagePermition = AccessControl.hasPermission(pagePermission);
                if (!userPagePermition.Any())
                    Response.Redirect(Routing.defualtUrl);
                    setPrivilege(userPagePermition);
               // classes.commonTask.loadEmpTypeInRadioButtonList(rbEmpTypeList);
                loadSlabType();
                if (!classes.commonTask.HasBranch())
                    ddlComapnyList.Enabled = false;
                ddlComapnyList.SelectedValue = ViewState["__CompanyId__"].ToString();
            }
        }

        DataTable dtSetPrivilege;
        private void setPrivilege(int[]permission)
        {
            try
            {
                
                
                HttpCookie getCookies = Request.Cookies["userInfo"];
                string getUserId = getCookies["__getUserId__"].ToString();
                ViewState["__UserType__"] = getCookies["__getUserType__"].ToString();
                ViewState["__CompanyId__"] = getCookies["__CompanyId__"].ToString();

                //if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Super Admin") || ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Master Admin"))
                //{
                //    classes.commonTask.LoadBranch(ddlComapnyList);
                //    return;
                //}
                //else
                //{
                    ddlComapnyList.Enabled = false;
                    classes.commonTask.LoadBranch(ddlComapnyList, ViewState["__CompanyId__"].ToString());

                if (permission.Contains(398))
                    ViewState["__ReadAction__"] = "1";
                if (permission.Contains(399))
                    ViewState["__WriteAction__"] = "1";
                if (permission.Contains(400))
                    ViewState["__DeletAction__"] = "1";

                checkInitialPermission();
                //if (ComplexLetters.getEntangledLetters(getCookies["__getUserType__"].ToString()).Equals("Admin"))
                //{
                //    btnDelete.Enabled = false;
                //    //dlSelectBonusYearAndType.Enabled = false;
                //}
                //else
                //{
                //    btnSet.Enabled = false;
                //    btnDelete.Enabled = false;
                //}
                //dtSetPrivilege = new DataTable();
                //sqlDB.fillDataTable("select * from UserPrivilege where PageName='bonus_monyh_setup.aspx' and UserId=" + getCookies["__getUserId__"].ToString() + "",dtSetPrivilege);
                //if (dtSetPrivilege.Rows.Count > 0)
                //{
                //    if (bool.Parse(dtSetPrivilege.Rows[0]["ReadAction"].ToString()).Equals(true))
                //    {
                //        gvBonusMonthList.Visible = true;
                //        dlSelectBonusYearAndType.Enabled = true;
                //    }
                //    else gvBonusMonthList.Visible = false;
                //    if (bool.Parse(dtSetPrivilege.Rows[0]["WriteAction"].ToString()).Equals(true))
                //    {
                //        gvBonusMonthList.Visible = true;
                //        btnSet.Enabled = true;
                //    }
                //    else btnSet.Enabled = false;

                //    if (bool.Parse(dtSetPrivilege.Rows[0]["DeleteAction"].ToString()).Equals(true))
                //    {

                //        btnDelete.Enabled = true;
                //    }
                //}

                //}

            }
            catch { }

        }

       

        private void loadSlabType()
        {

            DataTable dt = new DataTable();
            string CompanyId=(ddlComapnyList.SelectedValue.ToString().Equals("0000"))?ViewState["__CompanyId__"].ToString():ddlComapnyList.SelectedValue.ToString();
            sqlDB.fillDataTable("select * from v_Payroll_BonusSetup_DistinctRecord where CompanyId='" + CompanyId + "' order by Year desc", dt);
            dlSelectBonusYearAndType.DataTextField = "BonusType";
            dlSelectBonusYearAndType.DataValueField = "BId";
            dlSelectBonusYearAndType.DataSource = dt;
            dlSelectBonusYearAndType.DataBind();
            dlSelectBonusYearAndType.Items.Insert(0, new ListItem(" ", "0"));

        
        
        }

        protected void dlSelectBonusYearAndType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

             //   divPagelist.Visible = false;
                lblMessage.InnerText = "";
                if (dlSelectBonusYearAndType.SelectedValue.ToString() == "" || dlSelectBonusYearAndType.SelectedValue.ToString() == "0")
                {
                    gvBonusMonthList.DataSource = null;
                    gvBonusMonthList.DataBind();
                    tblGenreateType.Visible = false;
                    lblStatus.Text = " "; 
                    return;
                }
                lblStatus.Text = "Unsetuped Bonus Month Info";
                tblGenreateType.Visible = true;
                DataTable dt = new DataTable();
                dt.Columns.Add("SL", typeof(int));
                dt.Columns.Add("SlabType",typeof(string));
                dt.Columns.Add("EquivalentMonth", typeof(string));
                dt.Columns.Add("Chosen",typeof(bool));
                dt.Columns.Add("Percentage",typeof(string));
                if (chekHasRecord() == true) return;   // verify already setuped ?
                else
                {
                   // divPagelist.Visible = true;
                    string getMonth = "";
                    
                    for (byte b =12; b > 0; b--)
                    {
                       

                        bool chosen = false; int percentage = 0;
                        if (b == 12)
                        {
                            getMonth = "Dec-" + DateTime.Now.Year; chosen = true; percentage = 100;
                        }
                        else if (b == 11) getMonth = "Nov-" + DateTime.Now.Year;
                        else if (b == 10) getMonth = "Oct-" + DateTime.Now.Year;
                        else if (b == 9)
                        {
                            getMonth = "Sep-" + DateTime.Now.Year; chosen = true; percentage = 75;
                        }
                        else if (b == 8) getMonth = "Aug-" + DateTime.Now.Year;
                        else if (b == 7) getMonth = "July-" + DateTime.Now.Year;
                        else if (b == 6)
                        {
                            getMonth = "Jun-" + DateTime.Now.Year; chosen = true; percentage = 50;
                        }
                        else if (b == 5) getMonth = "May-" + DateTime.Now.Year;
                        else if (b == 4) getMonth = "Apr-" + DateTime.Now.Year;
                        else if (b == 3)
                        {
                            getMonth = "Mar-" + DateTime.Now.Year; chosen = true; percentage = 25;
                        }
                        else if (b == 2) getMonth = "Feb-" + DateTime.Now.Year;
                        else if (b == 1) getMonth = "Jan-" + DateTime.Now.Year;

                        dt.Rows.Add(b, b.ToString() + " Months", getMonth, chosen,percentage);
                    
                    }
                    gvBonusMonthList.DataSource = dt;
                    gvBonusMonthList.DataBind();
                    lblStatus.Text = "Unsetuped Bonus Month Info";
                
                }
            }
            catch { }
        }

        private bool chekHasRecord()
        {
            try
            {
                DataTable dt = new DataTable();
                sqlDB.fillDataTable("select * from Payroll_BonusMonthSetup where BId=" + dlSelectBonusYearAndType.SelectedItem.Value.ToString() + "", dt=new DataTable ());
                if (dt.Rows.Count > 0)
                {
                   // divPagelist.Visible = true;
                    gvBonusMonthList.DataSource = dt;
                    gvBonusMonthList.DataBind();
                    lblStatus.Text = "Setuped Bonus Month Info";
                    if (dt.Rows[0]["GenerateOn"].ToString() == "Basic Salary") rblGenerateType.SelectedIndex = 1;
                    else rblGenerateType.SelectedIndex = 0;
                    return true;
                }
                else return false;
            }
            catch { return false; }
        
        }

        protected void btnSet_Click(object sender, EventArgs e)
        {
            try
            {
                bool status = false;
                if (dlSelectBonusYearAndType.SelectedValue.ToString() == "0") return;
                SQLOperation.forDeleteRecordByIdentifier("Payroll_BonusMonthSetup", "BId", dlSelectBonusYearAndType.SelectedValue.ToString(), sqlDB.connection);
                for (byte b = 0; b < gvBonusMonthList.Rows.Count; b++)
                {
                    CheckBox chk = new CheckBox();
                    chk = (CheckBox)gvBonusMonthList.Rows[b].Cells[2].FindControl("chkChosen");
                    TextBox txtPercentage = (TextBox)gvBonusMonthList.Rows[b].Cells[3].FindControl("txtPercentage");

                    bool chosen = (chk.Checked) ? true : false;
                    string[] getColumns = {"BId","SlabType", "EquivalentMonth", "Chosen", "Percentage", "BonusType", "GenerateOn" };
                    string[] getValues = { dlSelectBonusYearAndType.SelectedValue.ToString(),gvBonusMonthList.Rows[b].Cells[0].Text, gvBonusMonthList.Rows[b].Cells[1].Text, chosen.ToString(), txtPercentage.Text,dlSelectBonusYearAndType.SelectedItem.ToString(),rblGenerateType.SelectedItem.ToString() };
                    if (SQLOperation.forSaveValue("Payroll_BonusMonthSetup", getColumns, getValues,sqlDB.connection) == true)
                    {
                        status = true;
                    }
                }

                if (status) lblMessage.InnerText = "success->Successfully Bonus Month Setup";
            }
            catch (Exception ex)
            {
              
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            lblMessage.InnerText = "";
            SQLOperation.forDeleteRecordByIdentifier("Payroll_BonusMonthSetup", "BonusType",dlSelectBonusYearAndType.SelectedItem.ToString(),sqlDB.connection);
            gvBonusMonthList.DataSource = null;
            gvBonusMonthList.DataBind();
        }

        protected void ddlComapnyList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblMessage.InnerText = "";
                loadSlabType();
                gvBonusMonthList.DataSource = null;
                gvBonusMonthList.DataBind();
            }
            catch { }
        }

        protected void tc1_ActiveTabChanged(object sender, EventArgs e)
        {
           if (gvSetupedList.Rows.Count==0) loadSetupedInfo();
        }

        private void loadSetupedInfo()
        {
            try
            {
                DataTable dt=new DataTable ();
                sqlDB.fillDataTable("select distinct BonusType,Format(setupedDate,'dd-MMM-yyyy') setupedDate,convert(int,Year) as Year from v_Payroll_BonusMonthSetup order by convert(int,Year)", dt);
                gvSetupedList.DataSource = dt;
                gvSetupedList.DataBind();

            }
            catch { }
        }

        protected void gvSetupedList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onmouseover"] = "javascript:SetMouseOver(this)";
                    e.Row.Attributes["onmouseout"] = "javascript:SetMouseOut(this)";
                }
            }
            catch { }
        }

        private void checkInitialPermission()
        {
            if (ViewState["__WriteAction__"].ToString().Equals("0"))
            {
                btnSet.Enabled = false;
                btnSet.CssClass = "";

            }
            else
            {
                btnSet.Enabled = true;
                btnSet.CssClass = "Pbutton";
            }
            if (ViewState["__DeletAction__"].ToString().Equals("0"))
            {
                btnDelete.Enabled = false;
                btnDelete.CssClass = "";
            }
            else
            {
                btnDelete.Enabled = true;
                btnDelete.CssClass = "Pbutton";
            }
        }




    }
}