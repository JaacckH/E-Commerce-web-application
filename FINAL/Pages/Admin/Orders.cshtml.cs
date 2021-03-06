using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.IO;

namespace FINAL.Pages.Admin
{
    public class OrdersModel : PageModel
    {
        public String getOrders()
        {

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Orders ORDER BY ID Desc";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                try
                {
                    String promoCode = reader["PromoCode"].ToString() + " (-" + Promotions.getPercentage(reader["PromoCode"].ToString()) + "%)";
                    if (String.IsNullOrEmpty(promoCode))
                    {
                        promoCode = "N/A";
                    }

                    String loyaltyPoints = reader["LoyaltyPoints"].ToString() + " (-Rs " + ((int.Parse(reader["LoyaltyPoints"].ToString())) * 50) + ")";
                    if (String.IsNullOrEmpty(loyaltyPoints))
                    {
                        loyaltyPoints = "0";
                    }

                    String baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/ORDERS/ORDER.html");
                    String status = System.IO.File.ReadAllText(Environment.CurrentDirectory
                                + "/HTML/ORDERS/STATUS/" + Orders.getOrderStatus(reader["OrderID"].ToString()) + ".html");
                    baseString = baseString.Replace("{ID}", reader["OrderID"].ToString())
                        .Replace("{DATE}", Utility.getDateFromDay(int.Parse(reader["DateTime"].ToString())))
                        .Replace("{PRICE}", Utility.formatPrice(reader["Price"].ToString())).Replace("{STATUS}", status)
                        .Replace("{NAME}", reader["Name"].ToString())
                        .Replace("{USERID}", Orders.getWhoOrdered(reader["OrderID"].ToString()))
                        .Replace("{PROMOCODE}", promoCode)
                        .Replace("{LOYALTYPOINT}", loyaltyPoints);

                    String address = reader["Name"] + "<br/>" + reader["AddressLine1"] + "<br/>" + reader["Postcode"];
                    baseString = baseString.Replace("{ADDRESS}", address).Replace("{PRODUCTS}", getOrderProducts(reader["OrderID"].ToString()));

                    html += baseString;
                }
                catch
                {

                }
            }

            conn.Close();
            return html;

            return null;

        }

        public String getOrderProducts(String orderID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM OrderedProducts";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["OrderID"].ToString() == orderID)
                {

                    SqlConnection conn2 = new SqlConnection();
                    conn2.ConnectionString = DBFunctions.connectionString;
                    conn2.Open();
                    SqlCommand query2 = conn2.CreateCommand();
                    query2.CommandText = "SELECT * FROM Products";
                    SqlDataReader reader2 = query2.ExecuteReader();

                    while (reader2.Read())
                    {
                        if (reader2["ProductID"].ToString() == reader["ProductID"].ToString())
                        {
                            String baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/ORDERS/ORDER_PRODUCTS.html");
                            baseString = baseString.Replace("{PRODUCTNAME}", reader2["Name"].ToString()).Replace("{QUANTITY}", reader["Quantity"].ToString())
                                .Replace("{SIZE}", reader["Size"].ToString())
                                .Replace("{ID}", "#" + reader["ProductID"].ToString())
                                .Replace("{PRICE}", Utility.formatPrice(reader["PurchasePrice"].ToString()));
                            html += baseString;
                        }
                    }

                    conn2.Close();
                }
            }

            conn.Close();
            return html;
        }
    }
}
