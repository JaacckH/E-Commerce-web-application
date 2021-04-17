using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Users
    {
        public static String getUsersHTML()
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Users ORDER BY Admin DESC";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {

                String accountType = File.ReadAllText(Environment.CurrentDirectory + "/HTML/ADMIN/USERS/STATUS/1.html");
                if (UserFunctions.isAdmin(reader["UserID"].ToString()))
                {
                    accountType = File.ReadAllText(Environment.CurrentDirectory + "/HTML/ADMIN/USERS/STATUS/2.html");
                }

                String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/ADMIN/USERS/USERS.html")
                    .Replace("{ID}", reader["UserID"].ToString()).Replace("{ACCOUNTTYPE}", accountType)
                    .Replace("{NAME}", reader["Forename"].ToString() + " " + reader["Surname"].ToString())
                    .Replace("{ADDRESS}", reader["AddressLine1"].ToString());
                html += baseString;
            }

            conn.Close();
            return html;

        }
    }
}
