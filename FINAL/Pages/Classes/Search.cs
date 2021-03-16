
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class Search
    {
        public static String getUsers(String input, String user)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Users;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["Username"].ToString().ToLower().Contains(input.ToLower()))
                {
                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/SEARCHRESULTSUSERS.html")
                        .Replace("{ID}", reader["Username"].ToString()).Replace("{USERNAME}", reader["Username"].ToString())
                        .Replace("{IMAGE}", DBFunctions.getProfilePicture(reader["Username"].ToString())).Replace("{BIO}", "");

                    html += baseString;
                }
            }
            conn.Close();
            return html;
        }

        public static String getCommunities(String input, String user)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["CommunityName"].ToString().ToLower().Contains(input.ToLower()))
                {
                    if (ContentModeration.canSeeCommunity(user, int.Parse(reader["CommunityID"].ToString())))
                    {
                        String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/SEARCHRESULTSCOMMUNITY.html")
                             .Replace("{ID}", reader["CommunityID"].ToString()).Replace("{COMMUNITY}", reader["CommunityName"].ToString())
                             .Replace("{IMAGE}", reader["AvatarUrl"].ToString()).Replace("{BIO}", reader["CommunityBio"].ToString());

                        html += baseString;
                    }
                }
            }
            conn.Close();
            return html;
        }
    }
}
