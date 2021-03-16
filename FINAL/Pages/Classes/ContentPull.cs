using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class ContentPull
    {
        static String connectionString = DBFunctions.connectionString;

        public static String getRecents(String username, Boolean search)
        {
            if (search == false)
            {
                username = DBFunctions.getUsernameFromSessionID(username);
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Messages ORDER BY Id desc;";
                SqlDataReader reader = query.ExecuteReader();

                List<String> users = new List<string>();
                List<String> recips = new List<string>();
                String html = "<center><h4 style=\"color:color:var(--text); margin:20px;\">RECENTS</h4></center>";
                while (reader.Read())
                {
                    if (reader["Sender"].ToString() == username || reader["Recipient"].ToString() == username)
                    {
                        String other = reader["Recipient"].ToString();
                        String recip = reader["Sender"].ToString();
                        if (reader["Sender"].ToString() != username)
                        {
                            other = username;
                            recip = reader["Recipient"].ToString();
                        }

                        if (userInArray(other, users) == false && other != username)
                        {
                            users.Add(other);
                            recips.Add(recip);
                            html += File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/CONTACTS.html")
                                .Replace("{USERNAME}", other).Replace("{MESSAGE}", DBFunctions.returnMostRecentMessage(other, recip))
                                .Replace("{IMAGE}", DBFunctions.getProfilePicture(other));
                        }
                    }
                }

                for (int i = 0; i < users.Count; i++)
                {

                }
                conn.Close();
                return html;
            }
            else
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Users;";
                SqlDataReader reader = query.ExecuteReader();

                String html = "<center><h4 style=\"color:var(--text); margin:20px;\">SEARCHED FOR: '" + username + "'</h4></center>";
                while (reader.Read())
                {
                    if (reader["Username"].ToString().ToLower().Contains(username.ToLower()))
                    {
                        String user = reader["Username"].ToString();
                        html += File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/CONTACTS.html").Replace("{USERNAME}", user)
                            .Replace("{MESSAGE}", DBFunctions.returnMostRecentMessage(user, null)).Replace("{IMAGE}", DBFunctions.getProfilePicture(user));
                    }
                }
                conn.Close();
                return html;
            }

        }

        public static Boolean userInArray(String username, List<String> list)
        {
            foreach (String user in list)
            {
                if (user == username)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
