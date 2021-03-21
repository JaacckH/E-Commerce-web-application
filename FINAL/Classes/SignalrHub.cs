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
            try
            {
                if (!String.IsNullOrEmpty(phonenumber))
                {
                    String response = LoginCreateAccount.createSuccessful(forename, surname, email, password, confirmpassword, addressline1, addressline2, postcode, Int32.Parse(phonenumber));
                    if (response.Contains("Successfuly"))
                    {
                        Console.WriteLine("Account Created");
                        // login the user
                        // create success alert here
                    }
                    else
                    {
                        Console.WriteLine(response);
                        // create error alert here
                    }
                }
                else
                {
                    Console.WriteLine("Phone number is empty");
                    // create error alert here
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }// create error message
        }

        public void sendEmail(String email) //will need await when we sent confirmation or erro
        {
            EmailManagement.sendPasswordReset(email);

            //error message
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

        public void sendAlert(String connectionID, String message)
        {
            Clients.Client(connectionID).SendAsync("sendAlert", message);
        }
    }
}
