
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public class Comments
    {
        public static void addComment(String username, String comment, int PostID)
        {
            DBFunctions.SendQuery("INSERT INTO Comments (Username, Comment, PostID) VALUES('" + username + "', '" + comment + "', '" + PostID + "');");
        }
        public static String getComments(int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Comments;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["PostID"].ToString() == PostID.ToString())
                {
                    html += "<p><b><a href=\"/Profile?id=" + reader["Username"].ToString() + "\">" + reader["Username"].ToString() + "</a>:</b> " + reader["Comment"].ToString() + "</p>";
                }
            }

            conn.Close();
            return html;
        }
    }
}
