using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SigmaERP.hrms.Data
{
    public static class CRUD
    {
        static DataTable dt;
        // private static string connectionString = "Server=DESKTOP-1EFD3E5;Database=RSSHRM;User Id=sa;Password=123;";
        private static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["local"].ConnectionString;

        // Method to get a SqlConnection object
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Method to execute a query and return a SqlDataReader
        public static SqlDataReader ExecuteQuery(string query)
        {
            SqlConnection connection = GetConnection();
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing query: " + ex.Message);
                return null;
            }
        }
        public static DataTable ExecuteReturnDataTable(string sqlCmd)
        {
            try
            {
                SqlConnection connection = GetConnection();
                connection.Open();
                dt = new DataTable();
                // cmd = new SqlCommand(sqlCmd, con);
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd, connection);
                //   da.SelectCommand.CommandTimeout =300;  // seconds
                da.SelectCommand.CommandTimeout = 0;  // seconds              
                da.Fill(dt);
                CloseConnection(connection);
                return dt;
            }
            catch (Exception ex) { return null; }
        }

        // Method to close the connection
        public static void CloseConnection(SqlConnection connection)
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}