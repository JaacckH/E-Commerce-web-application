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

                email = HttpContext.Request.Form["email"];
                UserPassword = HttpContext.Request.Form["UserPassword"];
                UserConfirmPassword = HttpContext.Request.Form["UserConfirmPassword"];

                forename = HttpContext.Request.Form["name"];
                surname = HttpContext.Request.Form["surname"];

                name = forename + "" + surname;
                addressline1 = HttpContext.Request.Form["addressline1"];
                postcode = HttpContext.Request.Form["postcode"];
                phonenumber = int.Parse(HttpContext.Request.Form["phonenumber"]);
                promocode = HttpContext.Request.Form["promoCode"];
                sessionID = HttpContext.Request.Cookies["SessionID"];
                cardnum = HttpContext.Request.Form["cardnumber1"] + HttpContext.Request.Form["cardnumber2"] +
                    HttpContext.Request.Form["cardnumber3"] + HttpContext.Request.Form["cardnumber4"];

                expiry = HttpContext.Request.Form["expiry1"] + "/" + HttpContext.Request.Form["expiry2"];
                cv2 = HttpContext.Request.Form["cv2"];
                


                String userID = UserFunctions.getUserID(HttpContext.Request.Cookies["SessionID"]);
                int price = Basket.getTotalPrice(userID, promocode);

                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(addressline1) &&
                    !String.IsNullOrEmpty(postcode) && phonenumber != null)
                {

                    if (Payment.isSuccessful(price, cardnum, cv2, expiry))
                    {
                        if (!String.IsNullOrEmpty(UserPassword))
                        {
                            if (UserPassword != UserConfirmPassword)
                            {
                                TempData["checkoutError"] = "Passwords don't match";
                            }
                            else if(UserFunctions.getUserDetails(UserFunctions.getUserID(HttpContext.Request.Cookies["SessionID"]), email) == null)
                            {

                               
                                String response = LoginCreateAccount.createSuccessful(sessionID, forename, surname, email, UserPassword, UserConfirmPassword, addressline1, phonenumber);

                                if (response == "DONE")
                                {
                                    // account has been created
                                    Orders.processOrder(Basket.getStockIDs(userID), userID, promocode, price, cardnum, cv2, expiry, name, addressline1, postcode, phonenumber.ToString());
                                    Response.Redirect("/CheckoutSuccess");
                                    return null;

                                }
                                else
                                {
                                    TempData["checkoutError"] = response;
                                }
                            }

                        }
                        else
                        {
                            // no password entered place order as guest
                            Orders.processOrder(Basket.getStockIDs(userID), userID, promocode, price, cardnum, cv2, expiry, name, addressline1, postcode, phonenumber.ToString());
                            Response.Redirect("/CheckoutSuccess");
                            return null;
                        }

                       
                    }
                    else
                    {
                        TempData["checkoutError"] = "Payment failed";
                    }

                }
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
