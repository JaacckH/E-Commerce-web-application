using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public class SignalrHub : Hub
    {
        public async Task createUserAccount(Boolean returnToCheckout, String sessionID, String forename, String surname, String email, String password, String confirmpassword, String addressline1, String phonenumber)
        {
            try
            {
                String response = LoginCreateAccount.createSuccessful(sessionID, forename, surname, email, password, confirmpassword, addressline1, phonenumber);
                if (response == "DONE")
                {
                    if (returnToCheckout == true)
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("setCheckoutCookie");
                    }
                    loginUser(email, password);
                }
                else
                {
                    sendAlert(Context.ConnectionId, response);
                }
            }
            catch
            {
                sendAlert(Context.ConnectionId, "You left an empty field");
            }
        }

        public void setConnectionID(String sessionID)
        {
            if (UserFunctions.userLoggedIn(sessionID))
            {
                DBFunctions.sendQuery("UPDATE Users SET ConnectionID='" + Context.ConnectionId + "' WHERE SessionID='" + sessionID + "';");
            }
            else
            {
                DBFunctions.sendQuery("UPDATE Guests SET ConnectionID='" + Context.ConnectionId + "' WHERE SessionID='" + sessionID + "';");
            }
        }

        public void sendEmail(String email) //will need await when we sent confirmation or error to user
        {
            EmailManagement.sendPasswordReset(email);
        }

        public void loginUser(String email, String password)
        {
            if (LoginCreateAccount.loginSuccessful(email, password) == true)
            {
                String sessionID = UserFunctions.generateSessionID();
                DBFunctions.sendQuery("UPDATE Users SET SessionID='" + sessionID + "' WHERE Email='" + email + "';");
                Clients.Client(Context.ConnectionId).SendAsync("setSessionID", sessionID);
                Clients.Client(Context.ConnectionId).SendAsync("Redirect", "/");
            }
            else
            {
                Console.WriteLine("Alert Sent");
                sendAlert(Context.ConnectionId, LoginCreateAccount.getLoginError(email, password));
            }
        }

        public async Task addToBasket(String arg)
        {

            String[] args = arg.Split(',');
            String sessionID = args[0];
            int stockID = int.Parse(args[1]);
            int quantity = int.Parse(args[2]);

            String userID = UserFunctions.getUserID(sessionID);
            if (Basket.containsItem(userID, stockID))
            {
                //int newQuantity = Basket.getItemQuantity(userID, stockID) + 1;
                //DBFunctions.sendQuery("UPDATE Basket SET Quantity='" + newQuantity + "' WHERE UserID='" + userID + "' AND StockID='" + stockID + "';");
                sendAlert(Context.ConnectionId, "This item is already in your Basket. <u onclick=\"removeFromBasket('" + stockID + "');\">Would you like to Remove it?</u>");
            }
            else
            {
                DBFunctions.sendQuery("INSERT INTO Basket (UserID, StockID, Quantity) VALUES('" + userID + "', '" + stockID + "', '" + quantity + "');");
                sendSuccessAlert(Context.ConnectionId, "You Successfully added a Product to your Basket. <u onclick=\"window.location.href='/Basket';\">Click to see your Basket.</u>");
            }

            await Clients.Client(Context.ConnectionId).SendAsync("updateBasket", Basket.getNumOfItems(sessionID).ToString());
        }

        public async Task removeFromBasket(String sessionID, String stockID)
        {
            String userID = UserFunctions.getUserID(sessionID);
            DBFunctions.sendQuery("DELETE FROM Basket WHERE UserID='" + userID + "' AND StockID='" + stockID + "';");
            await sendContent(Context.ConnectionId, Basket.getNumOfItems(sessionID).ToString(), "basket-counter");
            await Clients.Client(Context.ConnectionId).SendAsync("removeContainer", stockID);
        }

        public void sendAlert(String connectionID, String message)
        {
            Clients.Client(connectionID).SendAsync("ShowError", message);
        }

        public void sendSuccessAlert(String connectionID, String message)
        {
            Clients.Client(connectionID).SendAsync("ShowSuccess", message);
        }

        public void sendAcknowledgeAlert(String connectionID, String message)
        {
            Clients.Client(connectionID).SendAsync("ShowAcknowledge", message);
        }

        public async Task sendContent(String connectionID, String content, String container)
        {
            await Clients.Client(connectionID).SendAsync("ContentDelivery", content, container);
        }

        public async Task appendContent(String connectionID, String content, String container)
        {
            await Clients.Client(connectionID).SendAsync("AppendDelivery", content, container);
        }

        public async Task sendMessage(String sessionID, String message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                String userID = UserFunctions.getUserID(sessionID);
                message = message.Replace("'", "''");
                DBFunctions.sendQuery("INSERT INTO Messages (UserID, Message, Active, Date, Status) VALUES('"
                    + userID + "', '" + message + "', '1', '" + DateTime.Now.ToString() + "', '3')");
                await appendContent(Context.ConnectionId, Messages.getMessageHTML(message, true), "messages");
                await updateAdminMessages(userID, message);
            }
        }

        public async Task adminSendMessage(String sessionID, String recipient, String message)
        {
            if (!String.IsNullOrEmpty(message) && UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                String userID = UserFunctions.getUserID(sessionID);
                message = message.Replace("'", "''");
                DBFunctions.sendQuery("INSERT INTO Messages (UserID, Message, Recipient, Active, Status) VALUES('"
                    + userID + "', '" + message + "', '" + recipient + "', '1', '2')");
                Messages.setStatus(recipient, 2);
                await appendContent(Context.ConnectionId, Messages.getMessageHTML(message, true), "messages-" + recipient);
                await appendContent(UserFunctions.getConnectionID(recipient), Messages.getMessageHTML(message, false), "messages");
            }
        }

        public async Task updateAdminMessages(String sender, String message)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT UserID, Admin, ConnectionID FROM Users";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Admin"].ToString() == "True")
                {
                    Console.WriteLine(reader["ConnectionID"].ToString());
                    await appendContent(reader["ConnectionID"].ToString(), Messages.getMessageHTML(message, false), "messages-" + sender);
                }
            }

            conn.Close();
        }

        public void updateSettings(String sessionID, String email, String addressLine1, String addressLine2, String zipCode)
        {
            if (!String.IsNullOrEmpty(sessionID) && !String.IsNullOrEmpty(email) &&
                !String.IsNullOrEmpty(addressLine1) && !String.IsNullOrEmpty(addressLine2) &&
                !String.IsNullOrEmpty(zipCode) && UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                Settings.updateMassSettings(email, addressLine1, addressLine2, zipCode);
                sendSuccessAlert(Context.ConnectionId, "Successfully Updated Fields");
            }
            else
            {
                sendAlert(Context.ConnectionId, "Please enter all fields");
            }
        }

        public void saveShopTabChanges(String sessionID, String vatString, String tagsString)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                int vat = 0;
                try
                {
                    vat = int.Parse(vatString);
                }
                catch { sendAlert(Context.ConnectionId, "VAT Must be an Integer value."); return; }

                Settings.setSetting("VAT", vat.ToString());
                Settings.setSetting("Tags", tagsString.ToString());
                sendSuccessAlert(Context.ConnectionId, "Settings Updated");
            }
        }

        public async Task addCategory(String sessionID, String category)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)) &&
                !DBFunctions.valueExists("Categories", "Category", category))
            {
                DBFunctions.sendQuery("INSERT INTO Categories (Category) VALUES('" + category + "');");
                await appendContent(Context.ConnectionId, Settings.getCategoryHTML(Settings.getNumOfCategories()), "category-select");
                sendSuccessAlert(Context.ConnectionId, "Category Added");
            }
        }

        public void deleteCategory(String sessionID, int categoryID)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                DBFunctions.sendQuery("DELETE FROM Categories WHERE ID='" + categoryID + "';");
                sendSuccessAlert(Context.ConnectionId, "Category Deleted");
            }
        }

        public void updateParcel(String sessionID, String price, int type)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                try
                {
                    if (type == 1)
                    {
                        Settings.setSetting("SmallParcelPrice", price.ToString());
                    }
                    else
                    {
                        Settings.setSetting("LargeParcelPrice", price.ToString());
                    }
                    sendSuccessAlert(Context.ConnectionId, "Parcel Settings Changed");
                }
                catch
                {
                    sendAlert(Context.ConnectionId, "Something went wrong, check the info you entered.");
                }
            }
        }

        public void UpdateEmailTemplate(String sessionID, int id, String subject, String heading, String body)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)) && !String.IsNullOrEmpty(subject)
                && !String.IsNullOrEmpty(heading) && !String.IsNullOrEmpty(body))
            {
                Settings.updateEmailTemplate(id, subject, heading, body);
                sendSuccessAlert(Context.ConnectionId, "Email Template Updated");
            }
        }

        public async Task updateQuantity(String stockID)
        {
            int maxQuantity = int.Parse(Stock.getStockDetail(int.Parse(stockID), "Quantity"));
            await Clients.Client(Context.ConnectionId).SendAsync("setMaxQuantity", maxQuantity);
        }

        public async Task openChat(String SessionID, String recipient)
        {
            String userID = UserFunctions.getUserID(SessionID);
            if (UserFunctions.isAdmin(userID) && recipient == "null")
            {
                sendAlert(Context.ConnectionId, "Null Recipient");
                return;
            }
            await sendContent(Context.ConnectionId, Messages.getChatbox(userID, recipient), "chatbox-placeholder");
        }

        public void markAsSettled(String sessionID, String userID)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                Messages.setStatus(userID, 1);
            }
        }

        public void confirmOrderEmail(String sessionID, String email)
        {
            String UserID = UserFunctions.getUserID(sessionID);
            String OrderId = Orders.lastUserOrderID(UserID);

            EmailManagement.SendEmail(email, "Order " + OrderId + " Confirmation", "Thanks for choosing Oui Oui fashion! Your order has been placed! order ID: " + OrderId + ".");
        }

        public async Task addPromoCode(String sessionID, String promoCode)
        {
            String userID = UserFunctions.getUserID(sessionID);
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)) &&
                Promotions.codeExists(promoCode) && Promotions.getPromoStatus(promoCode) == 1)
            {
                int prePrice = Basket.getTotalPrice(userID, null);
                int price = Basket.getTotalPrice(userID, promoCode);
                await sendContent(Context.ConnectionId, "Promotions: (CODE: " + promoCode + ")", "promotion-display");
                await sendContent(Context.ConnectionId, "Rs -" + Utility.formatPrice((prePrice - price).ToString()) + " (-" + Promotions.getPercentage(promoCode) + "%)", "promotion-amount");
                await sendContent(Context.ConnectionId, "Rs " + Utility.formatPrice(price.ToString()), "checkout-total");
                sendSuccessAlert(Context.ConnectionId, "Promotion Code Applied");
            }
            else
            {
                sendAlert(Context.ConnectionId, "Invalid Promotion Code");
            }
        }

        public async Task updateOrderStatus(String sessionID, String orderID, String status)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)) &&
                int.Parse(status) != Orders.getOrderStatus(orderID))
            {
                Orders.setOrderStatus(orderID, int.Parse(status));
                await UpdateOrderStatusHtml(orderID, int.Parse(status));
                sendSuccessAlert(Context.ConnectionId, "Order Status Updated");
            }
        }

        public async Task UpdateOrderStatusHtml(String orderID, int status)
        {
            String statusHtml = System.IO.File.ReadAllText(Environment.CurrentDirectory
            + "/HTML/ORDERS/STATUS/" + status + ".html");
            await Clients.Client(Context.ConnectionId).SendAsync("UpdateOrderStatus", orderID, statusHtml);
        }

        public async Task promoteUser(String sessionID, String userID)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                if (UserFunctions.getUserID(sessionID) == userID)
                {
                    sendAlert(Context.ConnectionId, "You may not demote yourself");
                    return;
                }

                String accountType = File.ReadAllText(Environment.CurrentDirectory + "/HTML/ADMIN/USERS/STATUS/2.html");
                int admin = 1;
                if (UserFunctions.isAdmin(userID))
                {
                    accountType = File.ReadAllText(Environment.CurrentDirectory + "/HTML/ADMIN/USERS/STATUS/1.html");
                    admin = 0;
                }

                DBFunctions.sendQuery("UPDATE Users SET Admin='" + admin + "' WHERE UserID='" + userID + "';");
                await sendContent(Context.ConnectionId, accountType, "account-type1-" + userID);
                await sendContent(Context.ConnectionId, accountType, "account-type2-" + userID);
                sendSuccessAlert(Context.ConnectionId, "User permissions were changed");

            }
        }

        public async Task deletePromoCode(String sessionID, String promoCode)
        {
            if (Promotions.codeExists(promoCode) && UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                DBFunctions.sendQuery("DELETE FROM Promotions WHERE PromoCode='" + promoCode + "';");
                await Clients.Client(Context.ConnectionId).SendAsync("deleteContainer", "data-row-" + promoCode);
                sendSuccessAlert(Context.ConnectionId, "Promotion Deleted");
            }
        }

        public async Task redirect(String connectionID, String path)
        {
            await Clients.Client(connectionID).SendAsync("Redirect", path);
        }

        public async Task checkout(String sessionID, String email, String forename, String surname,
            String addressline1, String phonenumber, String promocode, String cardnum, String expiry,
            String cv2, String userPassword, String userConfirmPassword)
        {

            String userID = UserFunctions.getUserID(sessionID);
            int price = Basket.getTotalPrice(userID, promocode);
            String name = forename + " " + surname;

            if (!String.IsNullOrEmpty(addressline1) && phonenumber != null)
            {

                if (Payment.isSuccessful(price, cardnum, cv2, expiry))
                {
                    if (!String.IsNullOrEmpty(userPassword))
                    {
                        if (userPassword != userConfirmPassword)
                        {
                            Console.WriteLine(userPassword + "," + userConfirmPassword);
                            sendAlert(Context.ConnectionId, "Your passwords don't match");
                            return;
                        }
                        else if (UserFunctions.getUserDetails(UserFunctions.getUserID(sessionID), email) == null)
                        {
                            String response = LoginCreateAccount.createSuccessful(sessionID, forename, surname, email, userPassword, userConfirmPassword, addressline1, phonenumber);
                            if (response == "DONE")
                            {
                                // account has been created
                                Orders.processOrder(Basket.getStockIDs(userID), userID, promocode, price, cardnum, cv2, expiry, name, addressline1, phonenumber.ToString());
                                await redirect(Context.ConnectionId, "/CheckoutSuccess");
                                return;
                            }
                            else
                            {
                                sendAlert(Context.ConnectionId, response);
                            }
                        }
                    }
                    else
                    {
                        Orders.processOrder(Basket.getStockIDs(userID), userID, promocode, price, cardnum, cv2, expiry, name, addressline1, phonenumber.ToString());
                        await redirect(Context.ConnectionId, "/CheckoutSuccess");
                        return;
                    }


                }
                else
                {
                    sendAlert(Context.ConnectionId, "Payment Failed");
                }

            }
        }

        public void updateProduct(String sessionID, String productID, String name, String description, String price,
            String wasprice, String category, String tags, String material, String sizeArray, String quantityArray)
        {
            if (UserFunctions.isAdmin(UserFunctions.getUserID(sessionID)))
            {
                price = price.Replace(",", "");
                wasprice = wasprice.Replace(",", "");

                if (String.IsNullOrEmpty(material))
                {
                    material = ProductFunctions.getProductDetails(productID, "Materials");
                }

                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(description) && !String.IsNullOrEmpty(price) &&
                    !String.IsNullOrEmpty(category) && !String.IsNullOrEmpty(sizeArray) && !String.IsNullOrEmpty(quantityArray)
                    && !String.IsNullOrEmpty(material))
                {
                    String[] sizes = sizeArray.Split(",");
                    String[] quantities = quantityArray.Split(",");

                    if (sizes.Length != quantities.Length)
                    {
                        sendAlert(Context.ConnectionId, "Please check your product size fields");
                        return;
                    }

                    int i = 0;
                    foreach (String product in sizes)
                    {
                        String size = sizes[i];
                        String quantity = quantities[i];

                        if (quantity == "0" || String.IsNullOrEmpty(quantity))
                        {
                            DBFunctions.sendQuery("DELETE FROM Stock WHERE ProductID='" + productID + "' AND SizeID='" + size + "';");
                        }
                        else
                        {
                            DBFunctions.sendQuery("DELETE FROM Stock WHERE ProductID='" + productID + "' AND SizeID='" + size + "';");
                            DBFunctions.sendQuery("INSERT INTO Stock (ProductID, SizeID, Quantity) VALUES('" + productID + "', '" + size + "', '" + quantity + "')");
                        }
                        i++;
                    }

                    DBFunctions.sendQuery("UPDATE Products SET Name='" + name + "', Description='" + description + "', " +
                        "Price='" + price + "', WasPrice='" + wasprice + "', Category='" + category + "', Tags='" + tags + "', " +
                        "Materials='" + material + "' WHERE ProductID='" + productID + "';");

                    sendSuccessAlert(Context.ConnectionId, "Product Information Updated");

                }
                else
                {
                    sendAlert(Context.ConnectionId, "Please fill in all the fields");
                    return;
                }


            }
        }

    }
}
