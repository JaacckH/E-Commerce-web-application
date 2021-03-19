using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public class UserFunctions
    {
        // Function to return one record from the database using users ID
        public static String getUserDetails(int userID, String detail)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "Select * FROM Users";
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

        public static String findExistingRecord(String column, String Record)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "Select * FROM Users";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader[column].ToString() == Record)
                {
                    conn.Close();
                    return "True";
                }
            }

            conn.Close();
            return "Fanse";

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


    }
}
