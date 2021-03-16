using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class CommunitySettings
    {
        public static String getCommunityUsersTable(String SessionID, int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFollowers;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["CommunityID"].ToString() == CommunityID.ToString() && 
                    reader["Username"].ToString() != DBFunctions.getUsernameFromSessionID(SessionID))
                {
                    String username = reader["Username"].ToString();
                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/COLUMNS/COMMUNITYSETTINGS.html");
                    baseString = baseString.Replace("{ID}", reader["ID"].ToString()).Replace("{MODERATOR}", isCommunityModerator(username, CommunityID))
                        .Replace("{MUTED}", mutedFromCommunity(username, CommunityID)).Replace("{BANNED}", bannedFromCommunity(username, CommunityID))
                        .Replace("{USERNAME}", username).Replace("{COMMUNITYID}", reader["CommunityID"].ToString());
                    html += baseString;
                }
            }

            conn.Close();
            return html;
        }

        public static String isCommunityModerator(String username, int CommunityID)
        {
            if (Communities.getCommunityMemberPermissionLevel(username, CommunityID) > 0)
            {
                return "checked";
            }
            return "";
        }

        public static String bannedFromCommunity(String username, int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityBans;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    conn.Close();
                    return "checked";
                }
            }

            conn.Close();
            return "";
        }

        public static String mutedFromCommunity(String username, int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityMutes;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    conn.Close();
                    return "checked";
                }
            }

            conn.Close();
            return "";
        }

        public static void communityBanUser(String SessionID, int CommunityID, String user)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.getCommunityMemberPermissionLevel(username, CommunityID) > 0)
            {
                if (CommunitySettings.bannedFromCommunity(user, CommunityID) == "checked")
                {
                    DBFunctions.SendQuery("DELETE FROM CommunityBans WHERE Username='" + user + "' AND CommunityID='" + CommunityID + "';");
                }
                else
                {
                    //DBFunctions.SendQuery("DELETE FROM CommunityFollowers WHERE Username='" + user + "' AND CommunityID='" + CommunityID + "';");
                    DBFunctions.SendQuery("DELETE FROM CommunityModerators WHERE Username='" + user + "' AND CommunityID='" + CommunityID + "';");
                    DBFunctions.SendQuery("INSERT INTO CommunityBans (Username, CommunityID, BannedReason) VALUES('" + user + "', '" + CommunityID + "', '" + "something bad probably..." + "')");
                }
            }
        }

        public static void communityMuteUser(String SessionID, int CommunityID, String user)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.getCommunityMemberPermissionLevel(username, CommunityID) > 0)
            {
                if (CommunitySettings.mutedFromCommunity(user, CommunityID) == "checked")
                {
                    DBFunctions.SendQuery("DELETE FROM CommunityMutes WHERE Username='" + user + "' AND CommunityID='" + CommunityID + "';");
                }
                else
                {
                    DBFunctions.SendQuery("INSERT INTO CommunityMutes (Username, CommunityID) VALUES('" + user + "', '" + CommunityID + "');");
                }
            }
        }

    }
}
