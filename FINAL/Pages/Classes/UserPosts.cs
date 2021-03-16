
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class UserPosts
    {
        public static Boolean ownsPost(String Username, int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityPosts;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if(reader["Username"].ToString() == Username && reader["PostID"].ToString() == PostID.ToString())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
