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
            String orderReference = UserFunctions.generateSessionID();
            if (UserFunctions.userLoggedIn(UserFunctions.getSessionID(userID)) == false)
            {
                userID = "GUEST";
            }

            //do calculation for promo code

            foreach (int item in stockItems)
            {
                int promoprice = ProductFunctions.getProductPrice(item); //do calculation using promocode here
                DBFunctions.sendQuery("INSERT INTO OrderedProducts (OrderID, ProductID, PurchasePrice) " +
                    "VALUES('" + orderReference + "', '" + item + "', '" + promoprice + "');");
            }

            DBFunctions.sendQuery("DELETE FROM Basket WHERE UserID='" + userID + "';");
            DBFunctions.sendQuery("INSERT INTO Orders (OrderID, UserID, Name, AddressLine1, Postcode, PhoneNumber, Price, CardNumber, CV2, Expiry) " +
                "VALUES('"+ orderReference + "', '" + userID + "', '" + name + "', '" + addressLine1 + "', '" + postcode + "', '" 
                + phoneNumber + "', '" + price + "', '" + cardNum + "', '" + cv2 + "', '" + expiry + "');");

        }
    }
}
