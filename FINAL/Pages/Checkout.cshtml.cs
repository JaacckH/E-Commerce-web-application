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
        public String name, addressline1, postcode, phonenumber, promocode, cardnum, expiry, cv2;

        public void onLoad()
        {
            if (UserFunctions.userLoggedIn(HttpContext.Request.Cookies["SessionID"]))
            {
                String userID = UserFunctions.getUserID(HttpContext.Request.Cookies["SessionID"]);
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = DBFunctions.connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Users";
                SqlDataReader reader = query.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["UserID"].ToString() == userID)
                    {
                        name = reader["Forename"] + " " + reader["Surname"];
                        addressline1 = reader["AddressLine1"].ToString();
                        postcode = reader["Postcode"].ToString();
                        phonenumber = reader["PhoneNumber"].ToString();

                    }
                }
            }
        }

        public IActionResult checkout()
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = HttpContext.Request.Form["name"];
                    addressline1 = HttpContext.Request.Form["addressline1"];
                    postcode = HttpContext.Request.Form["postcode"];
                    phonenumber = HttpContext.Request.Form["phonenumber"];
                    promocode = HttpContext.Request.Form["promocode"];
                    cardnum = HttpContext.Request.Form["cardnumber"];
                    expiry = HttpContext.Request.Form["expiry"];
                    cv2 = HttpContext.Request.Form["cv2"];
                }

                String userID = UserFunctions.getUserID(HttpContext.Request.Cookies["SessionID"]);
                int price = Basket.getTotalPrice(userID);

                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(addressline1) &&
                    !String.IsNullOrEmpty(postcode) && !String.IsNullOrEmpty(phonenumber))
                {

                    if (Payment.isSuccessful(price, cardnum, cv2, expiry))
                    {
                        Orders.processOrder(Basket.getProductIDs(userID), userID, promocode, price, cardnum, cv2, expiry, name, addressline1, postcode, phonenumber);
                    }
                    else
                    {
                        TempData["checkoutError"] = "Payment failed";
                    }

                }
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
                    html += getProductHtml(int.Parse(reader["ProductID"].ToString()),
                        Basket.getItemQuantity(reader["UserID"].ToString(), int.Parse(reader["ProductID"].ToString())));
                }
            }

            return html;
        }

        public static String getProductHtml(int productID, int quantity)
        {
            String baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTCHECKOUT.html");
            String price = "", imagePath = "", name = "";

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
                    name = reader["Name"].ToString();
                    imagePath = reader["ImagePath"].ToString();
                }
            }

            conn.Close();
            baseString = baseString.Replace("{PRICE}", price).Replace("{IMAGE}", imagePath)
                .Replace("{ID}", productID.ToString()).Replace("{QUANTITY}", quantity.ToString())
                .Replace("{NAME}", name);

            return baseString;
        }
    }
}
