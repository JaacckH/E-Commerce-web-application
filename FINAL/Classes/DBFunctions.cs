using Microsoft.Data.SqlClient;
using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class DBFunctions
    {
        public static String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=\"" + Environment.CurrentDirectory + "\\DATA\\OUIOUI.MDF\";Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static void sendQuery(String input)
        {
            try
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = DBFunctions.connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = input;
                query.ExecuteReader();
                conn.Close();
                Console.WriteLine(input);
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public static Boolean valueExists(String table, String column, String value)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT " + column + " FROM " + table;
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader[column].ToString() == value)
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static int getNumOfRecords(String table)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM " + table;
            SqlDataReader reader = query.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                i++;
            }

            conn.Close();
            return i;
        }
    }
}
