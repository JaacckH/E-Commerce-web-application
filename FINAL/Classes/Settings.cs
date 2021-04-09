using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FINAL.Classes
{
    public static class Settings
    {
        public static String getSetting(String Setting)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Settings";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Setting"].ToString() == Setting)
                {
                    String result = reader["Value"].ToString();
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return null;
        }

        public static String getAdminEmail()
        {
            return File.ReadAllText(Environment.CurrentDirectory + "/Information/AdminEmail.txt");
        }

        public static String getAddressLine1()
        {
            return File.ReadAllText(Environment.CurrentDirectory + "/Information/Address/AddressLine1.txt");
        }

        public static String getAddressLine2()
        {
            return File.ReadAllText(Environment.CurrentDirectory + "/Information/Address/AddressLine2.txt");
        }

        public static String getZipCode()
        {
            return File.ReadAllText(Environment.CurrentDirectory + "/Information/Address/ZipCode.txt");
        }

        public static void updateMassSettings(String email, String addressLine1, String addressLine2, String zipCode)
        {
            File.WriteAllText(Environment.CurrentDirectory + "/Information/AdminEmail.txt", email);
            File.WriteAllText(Environment.CurrentDirectory + "/Information/Address/AddressLine1.txt", addressLine1);
            File.WriteAllText(Environment.CurrentDirectory + "/Information/Address/AddressLine2.txt", addressLine2);
            File.WriteAllText(Environment.CurrentDirectory + "/Information/Address/ZipCode.txt", zipCode);
        }
    }
}
