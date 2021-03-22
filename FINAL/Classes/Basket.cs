using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Basket
    {
        public static int getNumOfItems(String sessionID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                if (reader["UserID"].ToString() == UserFunctions.getUserID(sessionID))
                {
                    i++;
                }
            }

            conn.Close();
            return i;
        }

        public static int getItemQuantity(String userID, int productID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["UserID"].ToString() == userID && reader["ProductID"].ToString() == productID.ToString())
                {
                    int result = int.Parse(reader["Quantity"].ToString());
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return 0;
        }

        public static String getProductHtml(int productID)
        {
            String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTBASKET.html");
            String price = "", quantity = "", imagePath = "", name = "";
            int id = 0;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Products";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID.ToString())
                {
                    price = reader["Price"].ToString();
                    quantity = reader["Quantity"].ToString();
                    name = reader["Name"].ToString();
                    imagePath = reader["ImagePath"].ToString();
                    id = int.Parse(reader["ProductID"].ToString());
                }
            }

            conn.Close();
            baseString = baseString.Replace("{PRICE}", price)
                .Replace("{QUANTITY}", quantity).Replace("{IMAGE}", imagePath)
                .Replace("{ID}", id.ToString());

            return baseString;
        }

        public static Boolean containsItem(String userID, int itemID)
        {
            if (getItemQuantity(userID, itemID) == 0)
            {
                return false;
            }
            return true;
        }

        public static String getBasketHtml(String SessionID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if(reader["UserID"].ToString() == UserFunctions.getUserID(SessionID))
                {
                    html += getProductHtml(int.Parse(reader["ProductID"].ToString()));
                }
            }

            return html;
        }
    }
}
