using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Data.SqlClient;

namespace FINAL.Pages.Classes
{
    public static class AdminPannel
    {
        public static String GetUserRows(String SessionID)
        {

            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (!Communities.isAdmin(username))
            {
                return "PERMISSION DENIED";
            }

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Users;";
            SqlDataReader reader = query.ExecuteReader();

            string html = "";
            while (reader.Read())
            {

                if (reader["Username"].ToString() != username)
                {
                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/ADMINUSERSROW.html");
                    String Verified = "", Muted = "", Banned = "";
                    if (reader["Verified"].ToString() == "True")
                    {
                        Verified = "checked";
                    }
                    if (reader["Banned"].ToString() == "True")
                    {
                        Banned = "checked";
                    }
                    if (reader["Muted"].ToString() == "True")
                    {
                        Muted = "checked";
                    }


                    String tempString = baseString.Replace("{ID}", reader["UserID"].ToString()).Replace("{DISPLAYNAME}", reader["DisplayName"].ToString())
                        .Replace("{USERNAME}", reader["Username"].ToString()).Replace("{SHARDS}", reader["Shards"].ToString()).Replace("{VERIFIED}", Verified)
                        .Replace("{BANNED}", Banned).Replace("{MUTED}", Muted);

                    html += tempString;
                }
            }
            conn.Close();
            return html;
        }

        public static String getReports(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (!Communities.isAdmin(username))
            {
                return "PERMISSION DENIED";
            }

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Reports ORDER BY ReportID DESC;";
            SqlDataReader reader = query.ExecuteReader();
            string html = "";

            while (reader.Read())
            {
                String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/ADMINREPORTROW.html");
                baseString = baseString.Replace("{ID}", reader["ReportID"].ToString()).Replace("{USERNAME}", reader["Username"].ToString())
                    .Replace("{URL}", reader["link"].ToString());
                html += baseString;
            }

            conn.Close();
            return html;
        }

        public static String getAppeals(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (!Communities.isAdmin(username))
            {
                return "PERMISSION DENIED";
            }

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM UserAppeals;";
            SqlDataReader reader = query.ExecuteReader();
            string html = "";

            while (reader.Read())
            {
                String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/ADMINAPPEALROW.html");
                baseString = baseString.Replace("{ID}", reader["AppealID"].ToString()).Replace("{USERNAME}", reader["Username"].ToString())
                    .Replace("{APPEAL}", reader["Reason"].ToString());
                html += baseString;
            }

            conn.Close();
            return html;
        }

        public static String GetCommunitiesRows(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (!Communities.isAdmin(username))
            {
                return "PERMISSION DENIED";
            }

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();

            string html = "";

            while (reader.Read())
            {
                String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/ADMINCOMMUNITIESROW.html");
                String NoOfUsers = DBFunctions.ChecCkNumberOfCommunityFollowers(reader["CommunityID"].ToString());
                String Hidden = "", Private = "";
                if (reader["Hidden"].ToString() == "True")
                {
                    Hidden = "<i class=\"fas fa-eye-slash\"></i>";
                }
                else
                {
                    Hidden = "<i class=\"fas fa-eye\"></i>";
                }

                if (reader["Private"].ToString() == "True")
                {
                    Private = "<i class=\"fas fa-user-shield\"></i>";
                }
                else
                {
                    Private = "<i class=\"fas fa-user\"></i>";
                }


                String tempString = baseString.Replace("{ID}", reader["CommunityID"].ToString()).Replace("{COMMUNITYNAME}", reader["CommunityName"].ToString())
                    .Replace("{MEMBERS}", NoOfUsers).Replace("{HIDDEN}", Hidden).Replace("{PRIVATE}", Private);
                //.Replace("", reader[""].ToString())
                html += tempString;
            }
            conn.Close();
            return html;
        }

        public static void UpdateUserVerify(String SessionID, String UserUsername)
        {
            String Username = DBFunctions.getUsernameFromSessionID(SessionID);

            if (Communities.isAdmin(Username))
            {
                if (DBFunctions.CheckUserStatus(UserUsername, "Verified") == "True")
                {
                    DBFunctions.SendQuery("UPDATE Users SET Verified = 'False' WHERE Username = '" + UserUsername + "'");
                }
                else
                {
                    DBFunctions.SendQuery("UPDATE Users SET Verified = 'True' WHERE Username = '" + UserUsername + "'");
                }

            }
        }

        public static void UpdateUserMute(String SessionID, String UserUsername)
        {
            String Username = DBFunctions.getUsernameFromSessionID(SessionID);

            if (Communities.isAdmin(Username))
            {
                if (DBFunctions.CheckUserStatus(UserUsername, "Muted") == "True")
                {
                    DBFunctions.SendQuery("UPDATE Users SET Muted = 'False' WHERE Username = '" + UserUsername + "'");
                }
                else
                {
                    DBFunctions.SendQuery("UPDATE Users SET Muted = 'True' WHERE Username = '" + UserUsername + "'");
                }
            }
        }
        public static void UpdateUserBan(String SessionID, String UserUsername)
        {
            String Username = DBFunctions.getUsernameFromSessionID(SessionID);

            if (Communities.isAdmin(Username))
            {
                if (DBFunctions.CheckUserStatus(UserUsername, "Banned") == "True")
                {
                    DBFunctions.SendQuery("UPDATE Users SET Banned = 'False' WHERE Username = '" + UserUsername + "'");
                }
                else
                {
                    DBFunctions.SendQuery("UPDATE Users SET Banned = 'True' WHERE Username = '" + UserUsername + "'");
                }
            }
        }

        public static void DeleteCommunity(String SessionID, String CommunityID)
        {
            String Username = DBFunctions.getUsernameFromSessionID(SessionID);

            if (Communities.isAdmin(Username))
            {
                DBFunctions.SendQuery("DELETE FROM Communities WHERE CommunityID = '" + CommunityID + "'");
            }
        }
    }
}
