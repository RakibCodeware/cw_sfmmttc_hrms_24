using adviitRuntimeScripting;
using ComplexScriptingSystem;
using SigmaERP.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SigmaERP.hrd
{
    public partial class Signatures : System.Web.UI.Page
    {        
        string sqlcmd = "";
        DataTable dt;
        protected void Page_Load(object sender, EventArgs e)
        {
            sqlDB.connectionString = Glory.getConnectionString();
            sqlDB.connectDB();
            lblMessage.InnerText = "";
            if (!IsPostBack)
            {
                ViewState["__preRIndex__"] = "No";
                commonTask.LoadSheets(ddlSheet);
                loadDepartment();                
            }

        }
       
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {


                if (ddlSheet.SelectedValue== "0")
                {
                    lblMessage.InnerText = "warning->Please Select Sheet Name";
                    return;
                }
                if (txtSequenceNo.Text.Trim()== "")
                {
                    lblMessage.InnerText = "warning->Please Enter Sequence No";
                    return;
                }

                if (dlStatus.Text == "-Select-")
                {
                    lblMessage.InnerText = "warning->Please Select Status";
                    return;
                }
                if (btnSave.Text == "Update")
                {
                    if (UpdateDepartment() == true)
                    {
                        loadDepartment();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "UpdateSuccess()", true);
                        allClear();
                    }
                }
                else
                {
                    if (SaveDepartment() == true)
                    {
                        allClear();
                        loadDepartment();
                        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "call me", "SaveSuccess()", true);
                    }
                }
            }
            catch
            {

            }
        }
        
        private void allClear()
        {

            ddlSheet.Enabled = true;
            txtSignature.Text = "";
            dlStatus.Text = "-Select-";
            hdnbtnStage.Value = "";
            hdnUpdate.Value = "";
            txtSequenceNo.Text = "";          
            btnSave.Text = "Save";
        }
        private Boolean SaveDepartment()
        {
            try
            {
                int st = dlStatus.Text.Equals("Active") ? 1 : 0;
                return CRUD.Execute(@"INSERT INTO [dbo].[HRD_SignaturesOfSheets]
           ([Signature]
           ,[Sheet]
           ,[Ordering]
           ,[IsActive])
           VALUES
           ('"+txtSignature.Text.Trim()+"','"+ddlSheet.SelectedValue+"',"+txtSequenceNo.Text.Trim()+","+st+")");
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = ex.Message;
                return false;
            }
        }
        private Boolean UpdateDepartment()
        {
            try
            {
                int st= dlStatus.Text.Equals("Active") ?1:0;          
                return CRUD.Execute("Update HRD_SignaturesOfSheets set Signature='"+txtSignature.Text.Trim()+"',Ordering='"+txtSequenceNo.Text.Trim()+"',IsActive="+ st + " where  SL=" + ViewState["__getSL__"].ToString());
            }
            catch (Exception ex)
            {
                lblMessage.InnerText = ex.Message;
                return false;
            }
        }

        private void loadDepartment()
        {
            //try
            //{
                string condition = ddlSheet.SelectedValue=="0"?"": "where ss.Sheet = '"+ddlSheet.SelectedValue+"'";
                sqlcmd = "select ss.SL,ss.Signature,ss.Sheet,ss.IsActive,s.SheetTitle,ss.Ordering from HRD_SignaturesOfSheets ss left join HRD_Sheets s on ss.Sheet=s.sheet " + condition+" order by ss.Sheet,ss.Ordering";
                dt = new DataTable();
                sqlDB.fillDataTable(sqlcmd, dt);
                divDepartmentList.DataSource = dt;
                divDepartmentList.DataBind();                
            //}
            //catch(Exception ex) { }
        }

        public static string DecodeFromUtf8(string utf8String)
        {
            // read the string as UTF-8 bytes.
            byte[] encodedBytes = Encoding.UTF8.GetBytes(utf8String);

            // convert them into unicode bytes.
            byte[] unicodeBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, encodedBytes);

            // builds the converted string.
            return Encoding.Unicode.GetString(encodedBytes);
        }

        protected void divDepartmentList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {

                if (e.CommandName.Equals("Alter"))
                {
                    string a = ViewState["__preRIndex__"].ToString();
                    if (!ViewState["__preRIndex__"].ToString().Equals("No")) divDepartmentList.Rows[int.Parse(ViewState["__preRIndex__"].ToString())].BackColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    divDepartmentList.Rows[rIndex].BackColor = System.Drawing.Color.Yellow;
                    ViewState["__preRIndex__"] = rIndex;
                    setValueToControl(divDepartmentList.DataKeys[rIndex].Values[0].ToString());
                    btnSave.Text = "Update";
                    if (ViewState["__UpdateAction__"].Equals("0"))
                    {
                        btnSave.Enabled = false;
                        btnSave.CssClass = "";
                    }
                    else
                    {
                        btnSave.Enabled = true;
                        btnSave.CssClass = "Rbutton";
                    }                    
                }
                else if (e.CommandName.Equals("deleterow"))
                {
                    int rIndex = Convert.ToInt32(e.CommandArgument.ToString());

                    SQLOperation.forDeleteRecordByIdentifier("HRD_SignaturesOfSheets", "SL", divDepartmentList.DataKeys[rIndex].Values[0].ToString(), sqlDB.connection);                        
                        allClear();
                        lblMessage.InnerText = "success->Successfully Department Deleted";
                        divDepartmentList.Rows[rIndex].Visible = false;
                   
                }
            }
            catch { }
        }
       
        private void setValueToControl(string getSL)
        {
            try
            {
                dt = new DataTable();
                dt = CRUD.ExecuteReturnDataTable("select SL,Sheet,Signature,Ordering,IsActive from HRD_SignaturesOfSheets where SL="+ getSL);
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewState["__getSL__"] = getSL;
                    ddlSheet.SelectedValue = dt.Rows[0]["Sheet"].ToString();
                    txtSignature.Text = dt.Rows[0]["Signature"].ToString();
                    txtSequenceNo.Text = dt.Rows[0]["Ordering"].ToString();
                    if (dt.Rows[0]["IsActive"].ToString().Equals("True"))
                        dlStatus.SelectedValue = "Active";
                    else
                        dlStatus.SelectedValue = "InActive";
                }

            }
            catch { }
        }   
      

        protected void btnNew_Click(object sender, EventArgs e)
        {
            allClear();
            loadDepartment();
        }

        protected void divDepartmentList_RowDataBound(object sender, GridViewRowEventArgs e)
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
            //if (ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Admin") || ComplexLetters.getEntangledLetters(ViewState["__UserType__"].ToString()).Equals("Viewer"))
            //{
            //    try
            //    {
            //        if (ViewState["__DeletAction__"].ToString().Equals("0"))
            //        {
            //            LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
            //            lnkDelete.Enabled = false;
            //            lnkDelete.OnClientClick = "return false";
            //            lnkDelete.ForeColor = Color.Silver;
            //        }

            //    }
            //    catch { }
            //    try
            //    {
            //        if (ViewState["__UpdateAction__"].ToString().Equals("0"))
            //        {
            //            LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkAlter");
            //            lnkDelete.Enabled = false;
            //            lnkDelete.ForeColor = Color.Silver;
            //        }

            //    }
            //    catch { }
            //}
        }       
        
    }
}