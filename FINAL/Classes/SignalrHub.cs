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
                else {
                    Console.WriteLine("Phone number is empty");
                    // create error alert here
                }       
            }
            catch (Exception e){ Console.WriteLine(e.Message); }// create error message
        }
        
        public async Task sendEmail(String email)
        {
            String response = LoginCreateAccount.resetemail(email);
            // send success notification
        }

        public void loginUser(String email, String password)
        {
            try {
                String response = LoginCreateAccount.loginSuccessful(email, password);

                if (response.Contains("Successful"))
                {
                    Console.WriteLine("good login");
                    // redirect user
                }
                else
                {
                    // send error
                    Console.WriteLine("No login");
                }
                
            } catch (Exception e){ Console.WriteLine(e.Message); }

        }
    }
}
