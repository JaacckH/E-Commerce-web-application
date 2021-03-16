using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class Banners
    {
        public static String getBannerColor(String username)
        {
            int ProfilePicture = int.Parse(DBFunctions.getUserData(username, "ProfilePicture"));
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ProfileImages;";
            SqlDataReader reader = query.ExecuteReader();

            int tier = 0;
            while (reader.Read())
            {
                if(reader["ProductID"].ToString() == ProfilePicture.ToString())
                {
                    tier = int.Parse(reader["Tier"].ToString());
                }
            }
            conn.Close();

            String color = "";
            if(tier == 0)
            {
                color = "#969dbc";
            }
            else if (tier == 1)
            {
                color = "#6b718e";
            }
            else if (tier == 2)
            {
                color = "#4bc5fe";
            }
            else if (tier == 3)
            {
                color = "#45a89c";
            }
            else if (tier == 4)
            {
                color = "#ffe3d9";
            }
            else if (tier == 5)
            {
                color = "#fc726f";
            }
            else if (tier == 6)
            {
                color = "#f6d056";
            }

            return color;
        }
    }
}
