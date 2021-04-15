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
            query.CommandText = "SELECT * FROM Messages ORDER BY MessageID DESC";
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

        public static String getAdminIdleMessages(String userID, String target)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Messages ORDER BY MessageID DESC";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if ((reader["UserID"].ToString() == target || reader["Recipient"].ToString() == target) && reader["Available"].ToString() == "False")
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

        public static void setStatus(String userID, int status)
        {
            DBFunctions.sendQuery("UPDATE Messages SET Status='" + status + "' WHERE UserID='" + userID + "';");
        }

        public static List<String> adminGetUsers()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Messages ORDER BY MessageID DESC";
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

        public static String getMessagesAsAdmin(String userID, String recipient)
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
                if (reader["Recipient"].ToString() == recipient)
                {
                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/SENT.html");
                    html += baseString.Replace("{MESSAGE}", reader["Message"].ToString());
                }
                else if (reader["UserID"].ToString() == recipient)
                {
                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/RECEIVED.html");
                    html += baseString.Replace("{MESSAGE}", reader["Message"].ToString());
                }
            }

            conn.Close();
            return html;
        }

        public static String getChatbox(String userID, String recipient)
        {
            if (UserFunctions.isAdmin(userID))
            {
                return File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/ADMINBOX.html").Replace("{MESSAGES}",
                   getMessagesAsAdmin(userID, recipient)).Replace("{RECIPIENT}", recipient);
            }
            return File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/USERBOX.html").Replace("{MESSAGES}",
                getUserMessages(userID));
        }

        public static String getStatusHTML(String userID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Messages";
            SqlDataReader reader = query.ExecuteReader();

            String status = "";
            while (reader.Read())
            {
                if (reader["UserID"].ToString() == userID)
                {
                    status = reader["Status"].ToString();
                }
            }

            conn.Close();
            return File.ReadAllText(Environment.CurrentDirectory + "/HTML/CHATBOX/STATUS/" + status + ".html");
        }
    }
}
