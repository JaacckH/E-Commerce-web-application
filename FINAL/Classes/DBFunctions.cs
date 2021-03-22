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
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = input;
            query.ExecuteReader();
            conn.Close();
            Console.WriteLine(input);
        }
    }
}
