using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public class SignalrHub : Hub
    {
        public void test(String text)
        {
            Console.WriteLine(text);
        }

        public async Task createUserAccount(String forename, String surname, String email, String password, String confirmpassword, String addressline1, String addressline2, String postcode, String phonenumber)
        {
            try
            {
                string response = LoginCreateAccount.createSuccessful(forename, surname, email, password, confirmpassword, addressline1, addressline2, postcode, Int32.Parse(phonenumber));
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
            catch { }// create error message
        }
        
        public async Task sendEmail(String email)
        {
            String response = LoginCreateAccount.resetemail(email);
           
        }
    }
}
