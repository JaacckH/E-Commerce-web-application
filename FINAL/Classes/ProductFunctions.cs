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
        public static String getProductDetails(String productID, String detail)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Products";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID)
                {
                    String result = reader[detail].ToString();
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return null;
        }

        public static String getMainProductHtml(String productID)
        {
            String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTMAIN.html");
            String price = "", imagePath = "", description = "", name = "", id = "", sizeHtml = "", maxQuantity = "", category = "", wasPrice = "";

            SqlConnection connSize = new SqlConnection();
            connSize.ConnectionString = DBFunctions.connectionString;
            connSize.Open();
            SqlCommand querySize = connSize.CreateCommand();
            querySize.CommandText = "SELECT * FROM Stock";
            SqlDataReader readerSize = querySize.ExecuteReader();

            while (readerSize.Read())
            {
                if (readerSize["ProductID"].ToString() == productID
                    && readerSize["Available"].ToString() != "False"
                    && int.Parse(readerSize["Quantity"].ToString()) > 0)
                {
                    maxQuantity = readerSize["Quantity"].ToString();
                    sizeHtml = "<option value=\"" + readerSize["StockID"] + "\">" + readerSize["SizeID"] + "</option>" + sizeHtml;
                }
            }

            connSize.Close();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Products";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID)
                {
                    price = reader["Price"].ToString();
                    description = reader["Description"].ToString();
                    name = reader["Name"].ToString();
                    imagePath = reader["ImagePath"].ToString();
                    id = reader["ProductID"].ToString();
                    category = reader["Category"].ToString();
                    wasPrice = reader["WasPrice"].ToString();
                }
            }

            conn.Close();
            baseString = baseString.Replace("{PRICE}", Utility.formatPrice(price))
                .Replace("{NAME}", name).Replace("{DESCRIPTION}", description)
                .Replace("{IMAGE}", imagePath)
                .Replace("{ID}", id.ToString())
                .Replace("{SIZES}", sizeHtml)
                .Replace("{MAXQUANTITY}", maxQuantity)
                .Replace("{PRICE}", price)
                .Replace("{CATEGORY}", category);

            if (!String.IsNullOrEmpty(wasPrice) && int.Parse(price) < int.Parse(wasPrice))
            {
                baseString = baseString.Replace("{WASPRICE}", "<p style=\"font-size:12pt; color:red;\"><span>Was: Rs " + Utility.formatPrice(wasPrice) + "</span></p>");
            }

            baseString = baseString.Replace("{WASPRICE}", "");

            return baseString;
        }

        // return raw html template of a single product
        public static String getSubProductHtml(String productID)
        {
            String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTSUB.html");
            String price = "", imagePath = "", description = "", name = "", id = "", tags = "", sizes = "";

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
                    description = reader["Description"].ToString();
                    name = reader["Name"].ToString();
                    imagePath = reader["ImagePath"].ToString();
                    id = reader["ProductID"].ToString();
                    tags = reader["Tags"].ToString();
                }
            }

            conn.Close();


            SqlConnection connSize = new SqlConnection();
            connSize.ConnectionString = DBFunctions.connectionString;
            connSize.Open();
            SqlCommand querySize = connSize.CreateCommand();
            querySize.CommandText = "SELECT * FROM Stock";
            SqlDataReader readerSize = querySize.ExecuteReader();

            while (readerSize.Read())
            {
                if (readerSize["ProductID"].ToString() == productID
                    && readerSize["Available"].ToString() != "False"
                    && int.Parse(readerSize["Quantity"].ToString()) > 0)
                {
                    sizes = readerSize["SizeID"].ToString() + "," + sizes;

                }
            }

            connSize.Close();

            baseString = baseString.Replace("{PRICE}", Utility.formatPrice(price))
                .Replace("{NAME}", name).Replace("{DESCRIPTION}", description)
                .Replace("{IMAGE}", imagePath)
                .Replace("{TAGS}", name + "," + sizes + "," + tags)
                .Replace("{ID}", id.ToString());

            return baseString;
        }

        public static int getProductQuantity(String stockID)
        {
            return int.Parse(getProductDetails(stockID, "Quantity"));
        }

        public static Boolean productArchived(String productID)
        {
            if (getProductStatus(productID) == 2)
            {
                return true;
            }
            return false;
        }

        public static Boolean productActive(String productID)
        {
            if (getProductStatus(productID) == 1)
            {
                return true;
            }
            return false;
        }

        public static Boolean productInactive(String productID)
        {
            if (getProductStatus(productID) == 0)
            {
                return true;
            }
            return false;
        }

        public static int getProductStatus(String productID)
        {
            return int.Parse(getProductDetails(productID, "Status"));
        }

        public static int getProductPrice(int stockID)
        {
            return int.Parse(getProductDetails(Stock.getStockDetail(stockID, "ProductID"), "Price"));
        }

    }
}
