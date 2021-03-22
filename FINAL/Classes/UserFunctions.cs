using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class UserFunctions
    {
        // Function to return an item of user's data
        public static String getUserDetails(String userID, String detail)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Users";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["UserID"].ToString() == userID.ToString())
                {
                    String result = reader[detail].ToString();
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return null;

        }

        //function to generate a random sessionID so it can be assigned to a user
        public static String generateSessionID()
        {
            String characters = "ABCDEFGHIJKLMONPQRSTUVWQYZ0123456789abcdefghijklmnopqrstuvwxyz";
            String sid = "";
            Random rand = new Random();

            for (int i = 0; i < 12; i++)
            {
                sid += characters[rand.Next(characters.Length)];
            }

            return sid;
        }

        //checks for existing users
        public static Boolean emailIsRegistered(String email)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Users";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Email"].ToString() == email)
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;

        }

        // Hash a single passed value, mostly used for passwords
        public static String hashSingleValue(String RawVal)
        {
            String HashedResult;
            using (var md5Hash = MD5.Create())
            {
                var RawBytes = Encoding.UTF8.GetBytes(RawVal);
                var hashBytes = md5Hash.ComputeHash(RawBytes);
                HashedResult = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }

            return HashedResult.ToString();
        }

        public static Boolean userLoggedIn(String SessionID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Users";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["SessionID"].ToString() == SessionID)
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static String getUserID(String SessionID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT UserID,SessionID FROM Users UNION SELECT UserID,SessionID FROM Guests";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["SessionID"].ToString() == SessionID)
                {
                    String result = reader["UserID"].ToString();
                    conn.Close();
                    return result;
                }
            }

            return null;
        }


    }
}
