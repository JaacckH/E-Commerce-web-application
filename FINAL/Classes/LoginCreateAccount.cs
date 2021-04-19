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
        public static String createSuccessful(String sessionID, String forename, String surname, String email, String password, String confirmpassword, String addressline1, String phonenumber)
        {
            if (String.IsNullOrEmpty(forename) || String.IsNullOrEmpty(surname) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password) || String.IsNullOrEmpty(addressline1))
            {
                return "You left an empty field";
            }
            if (UserFunctions.emailIsRegistered(email))
            {
                return "An account with this email already exists";
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                return "Please enter a valid email address";
            }
            if (password != confirmpassword)
            {
                return "The passwords you entered do not match";
            }
            if (password.Length > 20 || password.Length < 8)
            {
                return "Your password must be between 8 and 20 characters long";
            }
            if (phonenumber.ToString().Length > 15)
            {
                return "Please enter a valid phone number";
            }

            String userID = UserFunctions.getUserID(sessionID);
            String hashedpassword = UserFunctions.hashSingleValue(password);

            DBFunctions.sendQuery("INSERT INTO Users (UserID, Forename, Surname, Email, Password, AddressLine1, PhoneNumber, DateCreated, Points) " +
                "VALUES ('" + userID + "', '" + forename + "', '" + surname + "', '" + email + "', '" + hashedpassword + "', '" + addressline1 + "', '" + phonenumber + "', '" + DateTime.Now.DayOfYear + "', '0')");
            return "DONE";
        }

        public static Boolean loginSuccessful(String email, String password)
        {
            try
            {
                String hashedpassword = UserFunctions.hashSingleValue(password);
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = DBFunctions.connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Users";
                SqlDataReader reader = query.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["Email"].ToString() == email && reader["Password"].ToString() == hashedpassword)
                    {
                        conn.Close();
                        return true;
                    }
                }
                conn.Close();
            }
            catch { }
            return false;
        }

        public static String getLoginError(String email, String password)
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
            return "Invalid Username or Password";
        }
    }
}
