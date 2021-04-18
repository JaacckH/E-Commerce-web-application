using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Stock
    {
        public static String getStockDetail(int stockID, String detail)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Stock";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["StockID"].ToString() == stockID.ToString())
                {
                    String result = reader[detail].ToString();
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return null;
        }

        public static Boolean productInStock(String productID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Stock WHERE ProductID='" + productID + "';";
            SqlDataReader reader = query.ExecuteReader();

            Boolean stock = false;
            while (reader.Read())
            {
                if (int.Parse(reader["Quantity"].ToString()) > 0)
                {
                    stock = true;
                }
            }

            conn.Close();
            return stock;
        }

        public static String getStockIDsFromProductID(String productID, String size)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Stock WHERE ProductID='" + productID + "';";
            SqlDataReader reader = query.ExecuteReader();

            String id = "";
            while (reader.Read())
            {
                if (reader["SizeID"].ToString() == size)
                {
                    id = reader["StockID"].ToString();
                }
            }

            conn.Close();
            return id;
        }
    }
}
