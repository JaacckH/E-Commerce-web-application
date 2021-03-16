using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class LoginCreateAccount
    {
        public static String loginSuccessful(String username, String password)
        {

            String HashedPassword = DBFunctions.hashSingleValue(password);
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT Username, Password FROM Users;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["Password"].ToString() == HashedPassword)
                {
                    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    string id = "";
                    Random rand = new Random();
                    for (int i = 0; i < 16; i++)
                    {
                        int r = rand.Next(chars.Length);
                        id += chars.ToCharArray()[r];
                    }
                    DBFunctions.SendQuery("UPDATE Users SET SessionID='" + id + "' WHERE Username='" + username + "';");
                    conn.Close();
                    return id;
                }
            }
            return null;
        }
        public static String ResetSeccessful(String Recovery, String NewPass, String RepPass)
        {
            try
            {
                if (string.IsNullOrEmpty(Recovery) || string.IsNullOrEmpty(NewPass) || string.IsNullOrEmpty(RepPass))
                {
                    return "YOU LEFT AN EMPTY FIELD";
                }
                String HashedRecovery = DBFunctions.hashSingleValue(Recovery).ToString();

                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = DBFunctions.connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Users;";
                SqlDataReader reader = query.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["RecoveryCode"].ToString() == HashedRecovery)
                    {
                        String HashedPassword = DBFunctions.hashSingleValue(NewPass).ToString();
                        DBFunctions.SendQuery("UPDATE Users SET Password ='" + HashedPassword + "' WHERE RecoveryCode ='" + HashedRecovery + "';");
                        conn.Close();
                        return "Done";
                    }
                }

                conn.Close();
                return "INCORRECT RECOVERY CODE";

            }
            catch { }

            return null;
        }

        public static String CerateSuccessful(String username, String Displayname, String Password, String Email, String ConfirmPassword)
        {
            try
            {
                if (username == "" || Email == "" || Password == "" || Displayname == "")
                {
                    return "YOU LEFT AN EMPTY FIELD";
                }
                if (DBFunctions.getUserData(username, "Username") != "null")
                {
                    return "THIS USERNAME ALREADY EXISTS";
                }
                if (Password != ConfirmPassword)
                {
                    return "PASSWORDS DON'T MATCH";
                }
                if (ContentCheck.validString(username, true, 20) == false || username.Contains(" "))
                {
                    return "YOUR USERNAME IS INVALID";
                }
                if (Displayname.Length > 20)
                {
                    return "YOUR DISPLAYNAME CANNOT EXCEED 20 CHARACTERS";
                }
                if (Password.Length > 20 || Password.Length < 8)
                {
                    return "YOUR PASSWORD MUST BE BETWEEN 8 AND 20 CHARACTERS LONG";
                }
                if(!Email.Contains("@") || !Email.Contains("."))
                {
                    return "YOUR EMAIL ADDRESS IS INVALID";
                }


                String HashedPassword = DBFunctions.hashSingleValue(Password);

                String Recovery = "";
                string Char = "ABCDEFGHIJKLMNOPQRSTUVQXYZabcdefghijklmnopqrstuvwxyz0123456789";
                Random rand = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int r = rand.Next(Char.Length);
                    Recovery += Char.ToCharArray()[r];
                }

                String HashedRecovery = DBFunctions.hashSingleValue(Recovery);


                if (DBFunctions.getUserData(username, "Email") == "null")
                {

                    DBFunctions.SendQuery("INSERT INTO Users (Username, Password, DisplayName, Email, Shards, ProfilePicture, RecoveryCode) VALUES ('" + username + "', '" + HashedPassword + "', '" + Displayname + "', '" + Email + "', '300', '3', '" + HashedRecovery + "')");
                    
                    return "YOUR RECOVERY CODE IS: " + Recovery;
                }
                else
                {
                    return "USERNAME ALREADY EXISTS";
                }
            }
            catch
            {

            }
            return "ERROR OCCURED";
        }
    }
}
