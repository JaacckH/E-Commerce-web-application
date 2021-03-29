using Microsoft.AspNetCore.SignalR;
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
            String[] perams = arg.Split(',');
            Console.WriteLine(arg + "!");
            String sessionID = perams[0].ToString();
            String productID = perams[1].ToString();
            int quantity = int.Parse(perams[2].ToString());

            String userID = UserFunctions.getUserID(sessionID);
            if (Basket.containsItem(userID, productID))
            {
                int newQuantity = Basket.getItemQuantity(userID, productID) + 1;
                DBFunctions.sendQuery("UPDATE Basket SET Quantity='" + newQuantity + "' WHERE UserID='" + userID + "' AND ProductID='" + productID + "';");
            }
            else
            {
                DBFunctions.sendQuery("INSERT INTO Basket (UserID, ProductID, Quantity) VALUES('" + userID + "', '" + productID + "', '" + quantity + "');");
            }

            await sendContent(Context.ConnectionId, Basket.getNumOfItems(sessionID).ToString(), "basket-counter");
        }

        public async Task removeFromBasket(String sessionID, String productID)
        {
            String userID = UserFunctions.getUserID(sessionID);
            DBFunctions.sendQuery("DELETE FROM Basket WHERE UserID='" + userID + "' AND ProductID='" + productID + "';");
            await Clients.Client(Context.ConnectionId).SendAsync("removeContainer", productID);
            await sendContent(Context.ConnectionId, Basket.getNumOfItems(sessionID).ToString(), "basket-counter");
        }

        public void sendAlert(String connectionID, String message)
        {
            Clients.Client(connectionID).SendAsync("sendAlert", message);
        }

        public async Task sendContent(String connectionID, String content, String container)
        {
            await Clients.Client(connectionID).SendAsync("ContentDelivery", content, container);
        }

        public async Task sendMessage(String sessionID, String message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                message = message.Replace("'", "''");
                DBFunctions.sendQuery("INSERT INTO ContactMessages ('UserID', 'Message') VALUES('"
                    + UserFunctions.getUserID(sessionID) + "', '" + message + "')");
            }
        }

    }
}
