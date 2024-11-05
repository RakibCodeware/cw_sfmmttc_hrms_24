using adviitRuntimeScripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SigmaERP.classes
{
    public static class PF_ProfitDistribution
    {
        public static int? ProfitDistributePerDay(string CompanyId,string Date, string FdrId,string IsManual,string UserId)
        {
            try
            {
                //------------------------------- Get FDR Information ----------------------------------------------------
               
                DataTable dtFDR = new DataTable();
                sqlDB.fillDataTable(" select  (InterestAmount/Period) as NetInterest,FromDate,ToDate  from PF_FDR  where  FdrID=" + FdrId + "", dtFDR);
                if (DateTime.Parse(Date) < DateTime.Parse(dtFDR.Rows[0]["FromDate"].ToString()) || DateTime.Parse(Date) > DateTime.Parse(dtFDR.Rows[0]["ToDate"].ToString()))
                {
                    return 0;
                }
                string MaturedDate = Date;
                string ProfitMonth = Date;

                //-------------------- Get PF Members --------------------------------------------------------------------
                DataTable dtPfMember = new DataTable();

                sqlDB.fillDataTable("with prall as("+
                    "select SL,EmpID,Month,EmpContribution,EmprContribution,'SGHRM' as ProductDB from SGHRM.dbo.PF_PFRecord "+
                    "where EmpID in(select distinct EmpId from v_PF_MemberListAll where ProductDB='SGHRM' and PFCompanyId='" + CompanyId + "' ) and  Month<'" + MaturedDate + "'"+
                    " union all   select SL,EmpID,Month,EmpContribution,EmprContribution,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_PFRecord "+
                    "where EmpID in(select distinct EmpId from v_PF_MemberListAll where ProductDB='SGFHRM' and PFCompanyId='" + CompanyId + "' ) and  Month<'" + MaturedDate + "'), "+
                    "pr as(select ProductDB, EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)) * (DATEDIFF(DAY,Month,'" + MaturedDate + "')/30*30) Amount from prall "+
                    "group by ProductDB,Empid,Month) "+
                    "select ProductDB,EmpID,sum(Amount) Amount from pr group by ProductDB,Empid", dtPfMember);

                    // comment date: 22-01-2018
                //sqlDB.fillDataTable("with prall as(" +
                //    "select SL,EmpID,Month,EmpContribution,EmprContribution,'SGHRM' as ProductDB from SGHRM.dbo.PF_PFRecord where EmpID in(select distinct EmpId from v_PF_MemberListAll where ProductDB='SGHRM' and PFCompanyId='" + CompanyId + "' ) union all   select SL,EmpID,Month,EmpContribution,EmprContribution,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_PFRecord where EmpID in(select distinct EmpId from v_PF_MemberListAll where ProductDB='SGFHRM' and PFCompanyId='" + CompanyId + "' ))" +
                //    ",ppall as(select *,'SGHRM' as ProductDB from SGHRM.dbo.PF_Profit where EmpID in(select distinct EmpId from v_PF_MemberListAll where ProductDB='SGHRM' and PFCompanyId='" + CompanyId + "' ) union all   select *,'SGFHRM' as ProductDB from SGFHRM.dbo.PF_Profit where EmpID in(select distinct EmpId from v_PF_MemberListAll where ProductDB='SGFHRM' and PFCompanyId='" + CompanyId + "' ) )" +
                //    ",pp as(select ProductDB,EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from ppall where Month<'" + MaturedDate + "' and ProductDB+EmpID in(select EmpId from v_PF_MemberListAll where PFCompanyId='" + CompanyId + "'))" +
                //    ",pr as (select  ProductDB,EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from prall where Month<'" + MaturedDate + "' and (ProductDB+EmpId) in(select ProductDB+EmpId from v_PF_MemberListAll where PFCompanyId='" + CompanyId + "'))" +
                //    ",pr1 as (select * from pr union select * from  pp)" +
                //    ",pr2 as(select ProductDB, EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','" + MaturedDate + "')/30*30) Amount from pr1 group by ProductDB,Empid,Month) " +
                //    "select ProductDB,EmpID,sum(Amount) Amount from pr2 group by ProductDB,Empid", dtPfMember);

                //sqlDB.fillDataTable("  with pr as (select EmpID,EmpContribution,EmprContribution,0 as profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_PFRecord where Month<'" + MaturedDate +
                //   "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1))," +
                //   " pp as (select EmpID,0 as EmpContribution,0 as EmprContribution, profit,CONVERT(VARCHAR(7), Month, 126) Month from PF_Profit where Month<'" + MaturedDate +
                //   "' and EmpID in(select EmpId from Personnel_EmpCurrentStatus where IsActive=1 and PfMember=1)), " +
                //   " pr1 as (select * from pr union select * from  pp)," +
                //   " pr2 as(select EmpiD,Month,(sum(EmpContribution)+sum(EmprContribution)+sum(profit) ) * (DATEDIFF(DAY,Month+'-01','" + MaturedDate + "')/30*30) Amount from pr1 group by Empid,Month) " +
                //   " select EmpID,sum(Amount) Amount from pr2 group by Empid", dtPfMember);

                //sqlDB.fillDataTable(" with a as ( "+
                //    " select pcs.EmpID,(pr.EmpContribution+pr.EmprContribution+sum( isnull(pp.Profit,0))) * (DATEDIFF(DAY,pr.Month,'"+MaturedDate+"')/30*30) TK "+
                //    " from Personnel_EmpCurrentStatus pcs inner join PF_PFRecord pr on pcs.EmpId=pr.EmpID and pcs.IsActive=1 and PfMember=1 left join PF_Profit pp on pr.EmpID=pp.EmpId and pr.Month=pp.Month "+
                //    " where pr.Month<'" + MaturedDate + "' and pcs.CompanyId='"+ddlCompanyName.SelectedValue+"' " +
                //    " group by pcs.EmpID, pr.Month,pr.EmpContribution,pr.EmprContribution ) "+
                //    " select a.EmpID, sum(a.TK) Amount from a  group by a.EmpID", dtPfMember);
                //--------------------------------------------------------------------------------------------------------

                float TotalAmount = float.Parse(dtPfMember.Compute("sum(Amount)", "").ToString());
                float TotalProfit = float.Parse(dtFDR.Rows[0]["NetInterest"].ToString());
                float Profit = 0;
                int count = 0;
                //-------------------------- Delete Existing Record for this FDR------------------------------------------
                SqlCommand cmdDel = new SqlCommand("delete SGHRM.dbo.PF_Profit where month='" + Date + "' and FdrID='" + FdrId + "'", sqlDB.connection);
                SqlCommand cmdDel1 = new SqlCommand("delete SGFHRM.dbo.PF_Profit where month='" + Date + "' and FdrID='" + FdrId + "'", sqlDB.connection);
                cmdDel.ExecuteNonQuery();
                cmdDel1.ExecuteNonQuery();
                //---------------------------------------------------------------------------------------------------------
                //--------------------Delete  Distribution Log-------------------------
                SqlCommand cmdDelLog = new SqlCommand("delete  PF_ProfitDistribution_log where Fdrid="+FdrId+" and DistributionDate='"+Date+"'", sqlDB.connection);
                cmdDelLog.ExecuteNonQuery();
                //-------------------------------------------------------------------------
                for (int i = 0; i < dtPfMember.Rows.Count; i++)
                {
                    Profit = TotalProfit / TotalAmount * float.Parse(dtPfMember.Rows[i]["Amount"].ToString());
                    SqlCommand cmd = new SqlCommand("insert into " + dtPfMember.Rows[i]["ProductDB"].ToString() + ".dbo.PF_Profit values('" + FdrId + "','" + dtPfMember.Rows[i]["EmpId"].ToString() + "','" +
                    ProfitMonth + "','" + Profit + "') ", sqlDB.connection);
                    if (int.Parse(cmd.ExecuteNonQuery().ToString()) == 1)
                        count++;
                }
                //----------------------Save to Distribution log--------------------
                SqlCommand cmddis = new SqlCommand("insert into PF_ProfitDistribution_log values('"+FdrId+"','"+Date+"','"+DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")+"','"+IsManual+"','"+UserId+"')",sqlDB.connection);
                cmddis.ExecuteNonQuery();
                //--------------------------------------------------------------------

                if (DateTime.Parse(Date) == DateTime.Parse(dtFDR.Rows[0]["ToDate"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("update PF_FDR set IsDistributed=1 where FdrID=" + dtFDR.Rows[0]["FdrID"].ToString() + "");
                    cmd.ExecuteNonQuery();
                }
                return count;

            }
            catch { return null; }
        }
    }
}