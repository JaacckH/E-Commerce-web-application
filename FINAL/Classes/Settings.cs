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

        public static String getEmailTableData(int id, String column)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM EmailTemplates";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ID"].ToString() == id.ToString())
                {
                    String result = reader[column].ToString();
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return null;
        }

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
            DBFunctions.sendQuery("UPDATE Settings SET Value='" + value + "' WHERE Setting='" + setting + "';"); 
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

        public static void updateEmailTemplate(int id, String subject, String heading, String body)
        {
            DBFunctions.sendQuery("UPDATE EmailTemplates SET Subject='" + subject +
                "', Heading='" + heading + "', Body='" + body + "' WHERE ID='" + id + "';");
        }

        public static String getEmailSubject(int id)
        {
            return getEmailTableData(id, "Subject");
        }

        public static String getEmailHeading(int id)
        {
            return getEmailTableData(id, "Heading");
        }

        public static String getEmailBody(int id)
        {
            return getEmailTableData(id, "Body");
        }

        public static String getCategoryHTML(int id)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Categories";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ID"].ToString() == id.ToString())
                {
                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/SETTINGS/CATEGORY.html");
                    baseString = baseString.Replace("{ID}", reader["ID"].ToString()).Replace("{CATEGORY}", reader["Category"].ToString());
                    conn.Close();
                    return baseString;
                }
            }

            conn.Close();
            return null;
        }

        public static String getCategories()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Categories";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                int id = int.Parse(reader["ID"].ToString());
                html += getCategoryHTML(id);
            }

            conn.Close();
            return html;
        }

        public static int getNumOfCategories()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT ID FROM Categories";
            SqlDataReader reader = query.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                i = int.Parse(reader["ID"].ToString());
            }

            conn.Close();
            return i;
        }
    }
}
