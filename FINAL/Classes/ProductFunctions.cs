using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
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
            String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCT.html");
            String price = "", quantity = "", imagePath = "", description = "", name = "";
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
                    description = reader["Description"].ToString();
                    name = reader["Name"].ToString();
                    imagePath = reader["ImagePath"].ToString();
                    id = int.Parse(reader["ProductID"].ToString());
                }
            }

            conn.Close();
            baseString = baseString.Replace("{PRICE}", price)
                .Replace("{NAME}", name).Replace("{DESCRIPTION}", description)
                .Replace("{QUANTITY}", quantity).Replace("{IMAGE}", imagePath)
                .Replace("{ID}", id.ToString());

            return baseString;
        }


        public static int getProductQuantity(int productID)
        {
            return int.Parse(getProductDetails(productID, "Quantity"));
        }

        public static Boolean productArchived(int productID)
        {
            if (getProductStatus(productID) == 2)
            {
                return true;
            }

            return false;
        }

        public static Boolean productActive(int productID)
        {
            if (getProductStatus(productID) == 1)
            {
                return true;
            }

            return false;
        }

        public static Boolean productInactive(int productID)
        {
            if (getProductStatus(productID) == 0)
            {
                return true;
            }

            return false;
        }

        public static int getProductStatus(int productID)
        {
            return int.Parse(getProductDetails(productID, "Status"));
        }

    }
}
