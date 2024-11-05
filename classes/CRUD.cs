using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using adviitRuntimeScripting;

namespace SigmaERP.classes
{
    public static  class CRUD
    {
        private static string sqlCmd = "";
        static  SqlCommand cmd;
        static DataTable dt;
         
        
        public static int ExecuteReturnID(string sqlCmd,SqlConnection con)
        {
            try
            {
                cmd = new SqlCommand(sqlCmd,con);
                 int result=   int.Parse(cmd.ExecuteScalar().ToString());
                return result;
            }
            catch { return 0; }
        }
        public static int ExecuteReturnID(string sqlCmd)
        {
            try
            {

                cmd = new SqlCommand(sqlCmd, sqlDB.connection);
                int result = int.Parse(cmd.ExecuteScalar().ToString());
                return result;
            }
            catch { return 0; }
        }

        public static bool Execute(string sqlCmd, SqlConnection con)
        {
            try
            {

                cmd = new SqlCommand(sqlCmd, con);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public static bool Execute(string sqlCmd)
        {
            try
            {               
                cmd = new SqlCommand(sqlCmd, sqlDB.connection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public static DataTable ExecuteReturnDataTable(string sqlCmd)
        {
            try
            {
                sqlDB.connectionString = Glory.getConnectionString();
                sqlDB.connectDB();
                dt = new DataTable();
               // cmd = new SqlCommand(sqlCmd, con);
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd, sqlDB.connection);
            //   da.SelectCommand.CommandTimeout =300;  // seconds
               da.SelectCommand.CommandTimeout =0;  // seconds              
                da.Fill(dt);                
                return dt;
            }
            catch (Exception ex) { return null; }
        }
        public static DataTable ExecuteReturnDataTable(string sqlCmd,SqlConnection con )
        {
            try
            {
               
                dt = new DataTable();
                // cmd = new SqlCommand(sqlCmd, con);
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd, con);
                //   da.SelectCommand.CommandTimeout =300;  // seconds
                da.SelectCommand.CommandTimeout = 0;  // seconds              
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex) { return null; }
        }
    }
}