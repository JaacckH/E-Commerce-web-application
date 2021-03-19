using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class ProductFunctions
    {
        // return single product record
        public static String getProductDetails(int productID, String detail)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "Select * FROM Products";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID.ToString())
                {
                    String result = reader[detail].ToString();
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return null;

        }

        // return raw html template of a single product
        public static String getProductHtml(int productID)
        {
            String baseString = "";
            String price = "", quantity = "", imagePath = "";
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "Select * FROM Products";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID.ToString())
                {
                    price = reader["Price"].ToString();
                    quantity = reader["Quantity"].ToString();
                }
            }

            conn.Close();
            baseString = baseString.Replace("{PRICE}", price)
                .Replace("{QUANTITY}", quantity).Replace("{IMAGE}", imagePath);

            return baseString;
        }


        public static int getProductQuantity(int productID)
        {
            return int.Parse(getProductDetails(productID, "Quantity"));
        }

        public static int getProductStatus(int productID)
        {
            return int.Parse(getProductDetails(productID, "Status"));
        }

    }
}
