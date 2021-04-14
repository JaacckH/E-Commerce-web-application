using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public class SignalrHub : Hub
    {
        public async Task createUserAccount(String forename, String surname, String email, String password, String confirmpassword, String addressline1, String addressline2, String postcode, String phonenumber)
        {
            String response = LoginCreateAccount.createSuccessful(forename, surname, email, password, confirmpassword, addressline1, addressline2, postcode, int.Parse(phonenumber));
            if (response == "DONE")
            {
                loginUser(email, password);
            }
            else
            {
                Console.WriteLine(response);
                //send error to user instead of writing to console
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
                sendAlert(Context.ConnectionId, LoginCreateAccount.getLoginError(email, password));
                Console.WriteLine("No login");
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
                int newQuantity = Basket.getItemQuantity(userID, stockID) + 1;
                DBFunctions.sendQuery("UPDATE Basket SET Quantity='" + newQuantity + "' WHERE UserID='" + userID + "' AND StockID='" + stockID + "';");
            }
            else
            {
                DBFunctions.sendQuery("INSERT INTO Basket (UserID, StockID, Quantity) VALUES('" + userID + "', '" + stockID + "', '" + quantity + "');");
            }

            await Clients.Client(Context.ConnectionId).SendAsync("updateBasket", Basket.getNumOfItems(sessionID).ToString());
        }

        public async Task removeFromBasket(String sessionID, String productID)
        {
            String userID = UserFunctions.getUserID(sessionID);
            DBFunctions.sendQuery("DELETE FROM Basket WHERE UserID='" + userID + "' AND StockID='" + productID + "';");
            await Clients.Client(Context.ConnectionId).SendAsync("removeContainer", productID);
            await sendContent(Context.ConnectionId, Basket.getNumOfItems(sessionID).ToString(), "basket-counter");
        }

        public void sendAlert(String connectionID, String message)
        {
            Clients.Client(connectionID).SendAsync("sendAlert", message);
        }

        public void sendSuccessAlert(String connectionID, String message)
        {
            Clients.Client(connectionID).SendAsync("sendSuccessAlert", message);
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
                DBFunctions.sendQuery("INSERT INTO Messages (UserID, Message, Active, Date) VALUES('"
                    + userID + "', '" + message + "', '1', '" + DateTime.Now.ToString() + "')");
                await appendContent(Context.ConnectionId, Messages.getMessageHTML(message, true), "messages");
                await updateAdminMessages(userID, message);
            }
        }

        public async Task adminSendMessage(String sessionID, String recipient, String message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                String userID = UserFunctions.getUserID(sessionID);
                message = message.Replace("'", "''");
                DBFunctions.sendQuery("INSERT INTO Messages (UserID, Message, Recipient, Active) VALUES('"
                    + userID + "', '" + message + "', '" + recipient + "', '1')");
                await appendContent(Context.ConnectionId, Messages.getMessageHTML(message, true), "messages-" + recipient);
                await appendContent(UserFunctions.getConnectionID(recipient), Messages.getMessageHTML(message, false), "messages");
                //await updateAdminMessages(userID, message);
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

        public void saveShopTabChanges(String sessionID, String vatString)
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
    }
}
