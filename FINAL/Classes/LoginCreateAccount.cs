using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Web;
using Microsoft.Data.SqlClient;

namespace FINAL.Classes
{
    public class LoginCreateAccount
    {
        public static String createSuccessful(String forename, String surname, String email, String password, String confirmpassword, String addressline1, String addressline2, String postcode, int phonenumber)
        {
            if (String.IsNullOrEmpty(forename) || String.IsNullOrEmpty(surname) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(addressline1) || String.IsNullOrEmpty(addressline2) || String.IsNullOrEmpty(postcode) || String.IsNullOrEmpty(phonenumber.ToString()))
            {
                return "You left an empty field";
            }
            if (UserFunctions.findExistingRecord("Email", email) == "True")
            {
                return "This email already exists";
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                return "Your email address is missing \"@\" or a \".\"";
            }
            if (password != confirmpassword)
            {
                return "Confirm password don't match";
            }
            if (password.Length > 20 || password.Length < 8)
            {
                return "Your password must be between 8 and 20 characters long";
            }
            if (postcode.Length != 5)
            {
                return "The post code is invalid";
            }
            if (phonenumber.ToString().Length != 7)
            {
                return "The phone number is invalid";
            }

            String hashedpassword = UserFunctions.hashSingleValue(password);

            try
            {
                DBFunctions.sendQuery("INSERT INTO Users (Forename, Surname, Email, Password, AddressLine1, AddressLine2, Postcode, PhoneNumber) VALUES ('" + forename + "', '" + surname + "', '" + email + "', '" + hashedpassword + "', '" + addressline1 + "', '" + addressline2 + "', '" + postcode + "', '" + phonenumber + "')");
                return "Successfuly create account";
            }
            catch(Exception e) { return e.Message; }

        }

        public static String loginSuccessful(String email, String password)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return "You left an empty field";
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                return "Please check you email. It is missing a \"@\" or \".\"";
            }
            if (password.Length > 20 || password.Length < 8)
            {
                return "Your password must be between 8 and 20 characters long";
            }

            String hashedpassword = UserFunctions.hashSingleValue(password);

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "Select * FROM Users";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Email"].ToString() == email && reader["Password"].ToString() == hashedpassword)
                {
                    // validate session
                    conn.Close();
                    return "Successful login";
                }
            }
            conn.Close();
            return "Error while trying to login, plase try again or shop as guest";
        }

        public static String resetemail(String email)
        {
            if (UserFunctions.findExistingRecord("Email", email) == "True")
            {
                String newpassword = "";
                string Char = "PSKENDJFCMVHGUQAZKXIERTaksmxnvitebvkub1234569781012asdfgs";
                Random rand = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int r = rand.Next(Char.Length);
                    newpassword += Char.ToCharArray()[r];
                }

                try
                {
                    DBFunctions.SendEmail(email, "Password Reset", "Hi, your new password is:" + newpassword);
                    DBFunctions.sendQuery("UPDATE Users SET Password ='" + UserFunctions.hashSingleValue(newpassword) + "' WHERE Email ='" + email + "';");
                }
                catch { }
            }
            else
            {

            }

            return "";
        }
    }
}
