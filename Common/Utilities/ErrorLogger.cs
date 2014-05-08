using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Research.DataOnboarding.Utilities
{
    public static class ErrorLogger
    {
        public static void LogException(Exception ex)
        {
            string connectionSring = string.Empty;
            if (ConfigurationManager.ConnectionStrings["DataOnboardingConnection"] != null)
            {
                connectionSring = ConfigurationManager.ConnectionStrings["DataOnboardingConnection"].ToString();
            }
            using (SqlConnection sqlCon = new SqlConnection(connectionSring))
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlCon;
                sqlCon.Open();
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = string.Concat("Insert into ErrorLogs values('", ex.Message, "','", ex.InnerException, "','", ex.StackTrace.ToString(),"')");
                sqlCmd.ExecuteNonQuery();
                
            }
        }

        public static void Log(string message)
        {
            string connectionSring = string.Empty;
            if (ConfigurationManager.ConnectionStrings["DataOnboardingConnection"] != null)
            {
                connectionSring = ConfigurationManager.ConnectionStrings["DataOnboardingConnection"].ToString();
            }
            using (SqlConnection sqlCon = new SqlConnection(connectionSring))
            {
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlCon;
                sqlCon.Open();
                sqlCmd.CommandType = System.Data.CommandType.Text;
                sqlCmd.CommandText = string.Concat("Insert into ErrorLogs (ErrorMessage) values('", message, "')");
                sqlCmd.ExecuteNonQuery();
            }
        }

    }
}
