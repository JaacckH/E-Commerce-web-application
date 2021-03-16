using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace FINAL.Pages.Classes
{
    public static class DBFunctions
    {
        public static String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=\"" + Environment.CurrentDirectory + "\\DATA\\PROJECT DATABASE.MDF\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static void SendQuery(String yourQuery)
        {
            Console.WriteLine("Query: " + yourQuery);
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = yourQuery;
            query.ExecuteReader();
            conn.Close();
        }

        public static String getUserColorMode(String username)
        {
            if (getUserData(username, "ColourMode") == "True")
            {
                return "lightMode";
            }
            return "darkMode";
        }

        public static String getConnectionID(String user)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ConnectionID;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == user)
                {
                    String result = reader["ConnectionID"].ToString();
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return "null";
        }

        public static void setProfilePicture(String username, int ProductID)
        {
            SendQuery("UPDATE Users SET ProfilePicture='" + ProductID + "' WHERE Username='" + username + "';");
        }

        public static String getProfilePicture(String username)
        {
            return "/ProfileImages/" + getUserData(username, "ProfilePicture") + ".png";
        }

        public static String hashSingleValue(String RawVal)
        {
            String HashedResult;
            using (var md5Hash = MD5.Create())
            {
                var RawBytes = Encoding.UTF8.GetBytes(RawVal);
                var hashBytes = md5Hash.ComputeHash(RawBytes);
                HashedResult = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }

            return HashedResult.ToString();
        }

        public static String CheckUserStatus(String username, String columnName) // check any binary status of the user profile (muted/banned/verified)
        {
            if (string.IsNullOrEmpty(username))
            {
                return "0";
            }
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Users WHERE Username = '" + username + "';";
                SqlDataReader reader = query.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[columnName].ToString() == "True")
                    {
                        conn.Close();
                        return "True";
                    }
                }
                conn.Close();

            }
            catch { }
            return "null";
        }

        public static String ChecCkNumberOfCommunityFollowers(String CommunityID)
        {
            if (string.IsNullOrEmpty(CommunityID))
            {
                return "null";
            }
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM CommunityFollowers;";
                SqlDataReader reader = query.ExecuteReader();
                int numberOfUsers = 0;
                while (reader.Read())
                {
                    if (reader["CommunityID"].ToString() == CommunityID.ToString())
                    {
                        numberOfUsers++;
                    }
                }
                conn.Close();
                return numberOfUsers.ToString();
            }
            catch { }
            return "error";
        }

        public static String getUserData(String username, String columnName)
        {
            if(username == "" || username == null)
            {
                return "0";
            }
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT " + columnName + ", Username FROM Users;";
                SqlDataReader reader = query.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["Username"].ToString() == username.ToString())
                    {
                        String result = reader[columnName].ToString();
                        conn.Close();
                        return result;
                    }
                }
                conn.Close();
            }
            catch
            {

            }
            return "null";

        }

        public static String getTableData(String table, String Username, String columnName) //return one record
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT " + columnName + ", Username FROM " + table + ";";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == Username.ToString())
                {
                    String result = reader[columnName].ToString();
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return "null";
        }

        public static String getSessionIDFromUsername(String username)
        {
            return getUserData(username, "SessionID");
        }

        public static String getUsernameFromSessionID(String SessionID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT SessionID, Username FROM Users;";
            SqlDataReader reader = query.ExecuteReader(); ;

            while (reader.Read())
            {
                if (reader["SessionID"].ToString() == SessionID)
                {
                    String result = reader["Username"].ToString();
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return null;
        }

        public static String returnMessages(String user, String recipient)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Messages;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";

            while (reader.Read())
            {
                if ((reader["Sender"].ToString() == user && reader["Recipient"].ToString() == recipient) ||
                    (reader["Sender"].ToString() == recipient && reader["Recipient"].ToString() == user))
                {

                    if (reader["Sender"].ToString() == user)
                    {
                        html += returnFormedMessage(0, reader["Message"].ToString());
                    }
                    else
                    {
                        html += returnFormedMessage(1, reader["Message"].ToString());
                    }
                }
            }
            conn.Close();
            return html;
        }

        public static String returnMostRecentMessage(String user1, String user2)
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Messages ORDER BY Id DESC;";
                SqlDataReader reader = query.ExecuteReader();

                while (reader.Read())
                {
                    if ((reader["Sender"].ToString() == user1 && reader["Recipient"].ToString() == user2) ||
                        (reader["Sender"].ToString() == user2 && reader["Recipient"].ToString() == user1))
                    {
                        String message = reader["Sender"].ToString() + ": " + reader["Message"].ToString();
                        if (message.Length > 18)
                        {
                            message = message.Substring(0, 18) + "...";
                        }
                        conn.Close();
                        return message;
                    }
                }
                conn.Close();
            }
            catch
            {

            }
            return "CLICK TO CHAT";
        }

        public static String returnFormedMessage(int type, String message)
        {
            String html = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/MESSAGES.html").Replace("{MESSAGE}", message);
            if (type == 0)
            {
                html = html.Replace("{STATUS}", "sent");
            }
            else
            {
                html = html.Replace("{STATUS}", "received");
            }
            return html;
        }

        public static String getCommunityInfo(int CommunityID, String Column)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    String result = reader[Column].ToString();
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return null;
        }
    }
}
