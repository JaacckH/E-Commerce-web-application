using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Statistics
    {
        public static String getTotalRevenue()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Orders;";
            SqlDataReader reader = query.ExecuteReader();

            int price = 0;
            while (reader.Read())
            {
                price += int.Parse(reader["Price"].ToString());
            }

            conn.Close();
            return Utility.formatPrice(price.ToString());
        }

        public static String getNumOfOrders()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Orders;";
            SqlDataReader reader = query.ExecuteReader();

            int orders = 0;
            while (reader.Read())
            {
                orders++;
            }

            conn.Close();
            return orders.ToString();
        }

        public static String getNumOfUnitsSold()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM OrderedProducts;";
            SqlDataReader reader = query.ExecuteReader();

            int orders = 0;
            while (reader.Read())
            {
                orders++;
            }

            conn.Close();
            return orders.ToString();
        }
    }
}
