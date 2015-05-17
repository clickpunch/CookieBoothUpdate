using System;
using Microsoft.Framework.ConfigurationModel;
using System.Data.SqlClient;
using System.Data;

namespace CookieBoothUpdate
{
    public sealed class SqlHelper
    {
        //Since this class provides only static methods, make the default constructor private to prevent 
        //instances from being created with "new SqlHelper()".
        private SqlHelper()
        {

        }

        private static string connectionString;
        public static string GetConnectionString()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                var config = new Configuration()
                      .AddJsonFile("config.json")
                      .AddEnvironmentVariables();
                connectionString = config.Get("Data:DefaultConnection:ConnectionString");
            }

            return connectionString;
        }

        // ExecuteNonQuery is basically used for operations where there is nothing returned from the SQL Query or Stored Procedure. Preferred use will be for INSERT, UPDATE and DELETE Operations.
        // This one is for dynamic code based Query
        public static int ExecuteNonQuery(SqlConnection conn, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            using (conn)
            {
                PrepareCommand(cmd, conn, null, CommandType.Text, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        // ExecuteNonQuery is basically used for operations where there is nothing returned from the SQL Query or Stored Procedure. Preferred use will be for INSERT, UPDATE and DELETE Operations.
        // This one is for CommandType.StoredProcedure
        public static int ExecuteNonQuery(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            using (conn)
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        // ExecuteReader is strictly used for fetching records from the SQL Query or Stored Procedure i.e.SELECT Operation.
        // Once the ExecuteReader method is executed it returns an object belonging to IDataReader Interface.Since we are dealing with SQL Server, I have used SqlDataReader.Until the DataReader Read method is returning true, it means that it is fetching record.
        // Hence a while loop is executed and records are fetched one by one.
        // ExecuteReader can also be used to bind a GridView control, but do it only if the GridView does not need Paging to be implemented. This is necessary since if you set AllowPaging to True and bind GridView using DataReader then you will land into an Exception as DataReader fetches records in ForwardOnly Mode.
        public static SqlDataReader ExecuteReader(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return rdr;
        }

        // ExecuteScalar is a handy function when you want to just need one Cell value i.e.one column and one row.
        // What happens when I use ExecuteScalar for SELECT statement with multiple columns and multiple rows?
        // This is a great question and the answer is yes you can use it but as its behavior it will return the very first cell i.e.first row and first column.
        // Can we use ExecuteScalar for INSERT, UPDATE and DELETE Statements?
        // Yes you can. But since INSERT, UPDATE and DELETE Statements return no value you will not get any value returned from the Query as well as you will not get the Rows Affected like you get in ExecuteNonQuery.
        public static object ExecuteScalar(SqlConnection conn, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            SqlCommand cmd = conn.CreateCommand();
            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] commandParameters)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(cmd, commandParameters);
            }
        }
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }
    }


    /* After this working with ADO.NET inside ASP.NET vNext is pretty straightforward:
    public class LogRepository
    {
        public void Add(Log log)
        {

            var parameters = new[]
                    {
                        new SqlParameter("@JobId", log.JobId),
                        new SqlParameter("@ErrorInfo", log.ErrorInfo),
                        new SqlParameter("@DateTimeProcessedUTC", log.DateTimeProcessedUTC)
                    };

            using (var conn = new SqlConnection(SqlHelper.GetConnectionString()))
            {
                SqlHelper.ExecuteNonQuery(
                   conn,
                   CommandType.Text,
                   @" INSERT dbo.Log ([JobId],[ErrorInfo],[DateTimeProcessedUTC])
                            SELECT @JobId, @ErrorInfo, @DateTimeProcessedUTC"
                     ,
                   parameters);

            }

        }
    }
    */

}
