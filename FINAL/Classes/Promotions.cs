using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Promotions
    {
        public static String listPromotions()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Promotions ORDER BY Id DESC";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/ADMIN/PROMOTIONS.html")
                    .Replace("{ID}", reader["PromoCode"].ToString()).Replace("{CODE}", reader["PromoCode"].ToString())
                    .Replace("{PERCENTAGE}", reader["Percentage"].ToString()).Replace("{START}", Utility.getDateFromDay(int.Parse(reader["StartDate"].ToString())))
                    .Replace("{END}", Utility.getDateFromDay(int.Parse(reader["EndDate"].ToString())))
                    .Replace("{USAGE}", getUsageAmount(reader["PromoCode"].ToString()).ToString())
                    .Replace("{STATUS}", getPromoStatusHTML(reader["PromoCode"].ToString()));

                html += baseString;
            }

            conn.Close();
            return html;
        }

        public static int getUsageAmount(String promocode)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT PromoCode FROM Orders WHERE PromoCode='" + promocode + "';";
            SqlDataReader reader = query.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                i++;
            }

            conn.Close();
            return i;
        }

        public static Boolean codeExists(String promoCode)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT PromoCode FROM Promotions WHERE PromoCode='" + promoCode + "';";
            SqlDataReader reader = query.ExecuteReader();

            Boolean exists = false;
            while (reader.Read())
            {
                exists = true;
            }

            conn.Close();
            return exists;
        }

        public static int getPercentage(String promoCode)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT PromoCode, Percentage FROM Promotions WHERE PromoCode='" + promoCode + "';";
            SqlDataReader reader = query.ExecuteReader();

            int percentage = 0;
            while (reader.Read())
            {
                percentage = int.Parse(reader["Percentage"].ToString());
            }

            conn.Close();
            return percentage;
        }


        public static String getPromoStatusHTML(String promoCode)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Promotions WHERE PromoCode='" + promoCode + "';";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                html = File.ReadAllText(Environment.CurrentDirectory +
                    "/HTML/ADMIN/PROMOTIONSTATUS/" + getPromoStatus(promoCode) + ".html");
            }

            conn.Close();
            return html;
        }

        public static int getPromoStatus(String promoCode)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Promotions WHERE PromoCode='" + promoCode + "';";
            SqlDataReader reader = query.ExecuteReader();

            int status = 0;
            while (reader.Read())
            {
                int today = DateTime.Now.DayOfYear;
                if (int.Parse(reader["EndDate"].ToString()) < today)
                {
                    status = 3;
                }
                else if (int.Parse(reader["StartDate"].ToString()) > today)
                {
                    status = 2;
                }
                else
                {
                    status = 1;
                }
            }

            conn.Close();
            return status;
        }
    }
}
