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
        public String name, addressline1, postcode, phonenumber, promocode, cardnum, expiry, cv2, email;

        public IActionResult checkout()
        {
            try
            {
                name = HttpContext.Request.Form["name"] + " " + HttpContext.Request.Form["surname"];
                addressline1 = HttpContext.Request.Form["addressline1"];
                postcode = HttpContext.Request.Form["postcode"];
                phonenumber = HttpContext.Request.Form["phonenumber"];
                promocode = HttpContext.Request.Form["promoCode"];

                cardnum = HttpContext.Request.Form["cardnumber1"] + HttpContext.Request.Form["cardnumber2"] +
                    HttpContext.Request.Form["cardnumber3"] + HttpContext.Request.Form["cardnumber4"];

                expiry = HttpContext.Request.Form["expiry1"] + "/" + HttpContext.Request.Form["expiry2"];
                cv2 = HttpContext.Request.Form["cv2"];
                email = HttpContext.Request.Form["email"];


                String userID = UserFunctions.getUserID(HttpContext.Request.Cookies["SessionID"]);
                int price = Basket.getTotalPrice(userID);

                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(addressline1) &&
                    !String.IsNullOrEmpty(postcode) && !String.IsNullOrEmpty(phonenumber))
                {

                    if (Payment.isSuccessful(price, cardnum, cv2, expiry))
                    {
                        Orders.processOrder(Basket.getStockIDs(userID), userID, promocode, price, cardnum, cv2, expiry, name, addressline1, postcode, phonenumber);
                        Response.Redirect("/CheckoutSuccess");
                        return null;
                    }
                    else
                    {
                        TempData["checkoutError"] = "Payment failed";
                    }

                }
                //Response.Redirect("/");
            }
            catch { }
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
            baseString = baseString.Replace("{PRICE}", price).Replace("{IMAGE}", imagePath)
                .Replace("{ID}", productID.ToString()).Replace("{QUANTITY}", quantity.ToString())
                .Replace("{NAME}", name).Replace("{SIZE}", Stock.getStockDetail(StockID, "SizeID"));

            return baseString;
        }
    }
}
