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

        public static void setSetting(String setting, String value)
        {
            DBFunctions.sendQuery("UPDATE Settings SET Value='" + value + "' WHERE Setting='" + setting + "';"); ;
        }

        public static String getAdminEmail()
        {
            return getSetting("AdminEmail");
        }

        public static String getAddressLine1()
        {
            return getSetting("AddressLine1");
        }

        public static String getAddressLine2()
        {
            return getSetting("AddressLine2");
        }

        public static String getZipCode()
        {
            return getSetting("ZipCode");
        }

        public static void updateMassSettings(String email, String addressLine1, String addressLine2, String zipCode)
        {
            setSetting("AdminEmail", email);
            setSetting("AddressLine1", addressLine1);
            setSetting("AddressLine2", addressLine2);
            setSetting("ZipCode", zipCode);
        }
    }
}
