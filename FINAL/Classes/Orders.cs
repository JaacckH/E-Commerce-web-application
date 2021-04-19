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
            String expiry, String name, String addressLine1, String phoneNumber)
        {
            GC.Collect();

            if (Promotions.codeExists(promoCode))
            {
                int percent = Promotions.getPercentage(promoCode);
                price = (1 - (percent / 100)) * price;
            }
            else
            {
                promoCode = null;
            }

            String orderReference = UserFunctions.generateSessionID();

            foreach (int item in stockItems)
            {
                int promoprice = ProductFunctions.getProductPrice(item); //do calculation using promocode here
                DBFunctions.sendQuery("INSERT INTO OrderedProducts (OrderID, ProductID, PurchasePrice, Quantity, Size) " +
                    "VALUES('" + orderReference + "', '" + Stock.getStockDetail(item, "ProductID") + "', '" + promoprice + "', '"
                    + Basket.getItemQuantity(userID, item) + "', '" + Stock.getStockDetail(item, "SizeID") + "');");
            }

            DBFunctions.sendQuery("DELETE FROM Basket WHERE UserID='" + userID + "';");

            if (UserFunctions.userLoggedIn(UserFunctions.getSessionID(userID)) == false)
            {
                userID += " (GUEST)";
            }

            double loyaltyPoints = int.Parse(UserFunctions.getUserDetails(userID, "Points"));
            loyaltyPoints += Math.Floor(price / 500.0);
            DBFunctions.sendQuery("UPDATE Users SET Points='" + loyaltyPoints + "' WHERE UserID='" + userID + "';");


            DBFunctions.sendQuery("INSERT INTO Orders (OrderID, UserID, Name, AddressLine1, Postcode, PhoneNumber, Price, CardNumber, CV2, Expiry, DateTime, PromoCode, Status) " +
                "VALUES('" + orderReference + "', '" + userID + "', '" + name + "', '" + addressLine1 + "', 'N/A', '"
                + phoneNumber + "', '" + price + "', '" + cardNum + "', '" + cv2 + "', '" + expiry + "', '" + DateTime.Now.DayOfYear + "', '" + promoCode + "', '1');");

        }

        public static String lastUserOrderID(String UserID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Orders WHERE UserID = '" + UserID + "';";
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
            return null;
        }

        public static int getNumOfOrdersForDay(int day)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Orders WHERE DateTime='" + day + "';";
            SqlDataReader reader = query.ExecuteReader();

            int amount = 0;
            while (reader.Read())
            {
                amount++;
            }

            conn.Close();
            return amount;
        }

        public static int getOrderStatus(String orderID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT OrderID, Status FROM Orders WHERE OrderID='" + orderID + "';";
            SqlDataReader reader = query.ExecuteReader();

            int status = 0;
            while (reader.Read())
            {
                status = int.Parse(reader["Status"].ToString());
            }

            conn.Close();
            return status;
        }

        public static void setOrderStatus(String orderID, int status)
        {
            DBFunctions.sendQuery("UPDATE Orders SET Status='" + status + "' WHERE OrderID='" + orderID + "';");
        }

        public static String getWhoOrdered(String orderID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT UserID, OrderID FROM Orders WHERE OrderID='" + orderID + "';";
            SqlDataReader reader = query.ExecuteReader();

            String userID = "";
            while (reader.Read())
            {
                userID = reader["UserID"].ToString();
            }

            conn.Close();
            return userID;
        }

    }
}
