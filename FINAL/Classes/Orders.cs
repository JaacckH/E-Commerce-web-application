using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Orders
    {
        //userID, price, cardnum, cv2, expiry, name, addressline1, postcode, phonenumber
        public static void processOrder(List<int> stockItems, String userID, String promoCode, int price, String cardNum, String cv2,
            String expiry, String name, String addressLine1, String postcode, String phoneNumber)
        {

            GC.Collect();
            String orderReference = UserFunctions.generateSessionID();

            Console.WriteLine("ran0");
            foreach (int item in stockItems)
            {

                Console.WriteLine("ran1");
                int promoprice = ProductFunctions.getProductPrice(item); //do calculation using promocode here
                DBFunctions.sendQuery("INSERT INTO OrderedProducts (OrderID, ProductID, PurchasePrice, Quantity, Size) " +
                    "VALUES('" + orderReference + "', '" + Stock.getStockDetail(item, "ProductID") + "', '" + promoprice + "', '"
                    + Basket.getItemQuantity(userID, item) + "', '" + Stock.getStockDetail(item, "SizeID") + "');");
                Console.WriteLine("ran2");
            }

            DBFunctions.sendQuery("DELETE FROM Basket WHERE UserID='" + userID + "';");

            if (UserFunctions.userLoggedIn(UserFunctions.getSessionID(userID)) == false)
            {
                userID += " (GUEST)";
            }

            DBFunctions.sendQuery("INSERT INTO Orders (OrderID, UserID, Name, AddressLine1, Postcode, PhoneNumber, Price, CardNumber, CV2, Expiry, DateTime) " +
                "VALUES('" + orderReference + "', '" + userID + "', '" + name + "', '" + addressLine1 + "', '" + postcode + "', '"
                + phoneNumber + "', '" + price + "', '" + cardNum + "', '" + cv2 + "', '" + expiry + "', '" + DateTime.Now.DayOfYear + "');");

        }

        public static string lastUserOrderID(String UserID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Orders WHERE UserID = '" + UserID +"';";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (!String.IsNullOrEmpty(reader["OrderID"].ToString()))
                {
                    String id = reader["OrderID"].ToString();
                    conn.Close();
                    return id;
                }
            }

            conn.Close();
            return "";
        }

    }
}
