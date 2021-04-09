using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Messages
    {
        public static String getMessageHTML(String message, Boolean sent)
        {
            String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/SENT.html");
            if (sent == false)
            {
                baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/RECEIVED.html");
            }
            return baseString.Replace("{MESSAGE}", message);
        }

        public static String getUserMessages(String userID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Messages";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["UserID"].ToString() == userID)
                {
                    html += getMessageHTML(reader["Message"].ToString(), true);
                }
                else if (reader["Recipient"].ToString() == userID)
                {
                    html += getMessageHTML(reader["Message"].ToString(), false);
                }
            }

            conn.Close();
            return html;
        }
    }
}
