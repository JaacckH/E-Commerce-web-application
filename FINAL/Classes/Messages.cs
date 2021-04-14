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

        public static String getAdminMessageHTML(String message, Boolean sent)
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

        public static String getAdminMessages(String userID, String target)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Messages ORDER BY ID DESC";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["UserID"].ToString() == target || reader["Recipient"].ToString() == target)
                {
                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/ADMIN.html");
                    baseString = baseString.Replace("{DATE}", reader["Date"].ToString())
                        .Replace("{SENDER}", reader["UserID"].ToString()).Replace("{MESSAGE}", reader["Message"].ToString());
                    html += baseString;
                }
            }

            conn.Close();
            return html;
        }

        public static List<String> adminGetUsers()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Messages";
            SqlDataReader reader = query.ExecuteReader();

            List<String> users = new List<String>();
            while (reader.Read())
            {
                if (!arrayContains(users, reader["UserID"].ToString()))
                {
                    users.Add(reader["UserID"].ToString());
                }
            }

            conn.Close();
            return users;
        }

        public static Boolean arrayContains(List<String> list, String user)
        {
            foreach (String us in list)
            {
                if (us == user)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
