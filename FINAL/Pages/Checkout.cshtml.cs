using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Group_Project.Models
{
    public class CheckoutModel : PageModel
    {
        public String name, addressline1, postcode, promocode, cardnum, expiry, cv2, email, UserPassword, UserConfirmPassword, forename, surname, sessionID;
        public int phonenumber;
        public IActionResult checkout()
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                TempData["checkoutError"] = e.Message;
            }
            return null;
        }

        public String getCheckoutItemsHtml(String SessionID)
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
                if (reader["UserID"].ToString() == UserFunctions.getUserID(SessionID))
                {
                    html += getProductHtml(int.Parse(reader["StockID"].ToString()),
                        Basket.getItemQuantity(reader["UserID"].ToString(), int.Parse(reader["StockID"].ToString())));
                }
            }

            return html;
        }

        public static String getProductHtml(int StockID, int quantity)
        {
            String baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTCHECKOUT.html");
            String price = "", imagePath = "", name = "";

            String productID = Stock.getStockDetail(StockID, "ProductID");

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
                    name = reader["Name"].ToString();
                    imagePath = reader["ImagePath"].ToString();
                }
            }

            conn.Close();
            baseString = baseString.Replace("{PRICE}", Utility.formatPrice(price)).Replace("{IMAGE}", imagePath)
                .Replace("{ID}", productID.ToString()).Replace("{QUANTITY}", quantity.ToString())
                .Replace("{NAME}", name).Replace("{SIZE}", Stock.getStockDetail(StockID, "SizeID"));

            return baseString;
        }
    }
}
