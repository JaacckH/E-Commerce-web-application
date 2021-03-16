using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class Store
    {

        public static String listProductsForSale(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ProfileImages ORDER BY Tier DESC;";
            SqlDataReader reader = query.ExecuteReader();
            String html = getEquippedHTML(username).Replace("{OC}", "1");

            while (reader.Read())
            {
                int ProductID = int.Parse(reader["ProductID"].ToString());
                if (!ppOwned(username, ProductID) && getPrice(ProductID) != 0)
                {
                    html += getProductHTML(ProductID, false);
                }
            }

            conn.Close();
            return html;
        }

        public static String listOwnedProducts(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ProfileImages;";
            SqlDataReader reader = query.ExecuteReader();
            String html = getEquippedHTML(username).Replace("{OC}", "2");

            while (reader.Read())
            {
                int ProductID = int.Parse(reader["ProductID"].ToString());
                if (ppOwned(username, ProductID) || getPrice(ProductID) == 0)
                {
                    html += getProductHTML(ProductID, true);
                }
            }

            conn.Close();
            return html;
        }

        public static String getProductHTML(int ProductID, Boolean owned)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ProfileImages;";
            SqlDataReader reader = query.ExecuteReader();
            String html = "";

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == ProductID.ToString())
                {
                    html = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/STOREITEM.html");
                    if (owned == true)
                    {
                        html = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/OWNEDITEM.html");
                    }
                    html = html.Replace("{PRICE}", getPrice(ProductID).ToString()).Replace("{IMAGEURL}", "/ProfileImages/" + reader["ProductID"].ToString() + ".png")
                        .Replace("{ID}", reader["ProductID"].ToString()).Replace("{OC}", "1");
                }
            }
            conn.Close();
            return html;
        }

        public static String getEquippedHTML(String Username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ProfileImages;";
            SqlDataReader reader = query.ExecuteReader();
            String html = "";

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == getEquipped(Username).ToString())
                {
                    html = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/EQUIPPED.html");
                    html = html.Replace("{PRICE}", getPrice(getEquipped(Username)).ToString()).Replace("{IMAGEURL}", DBFunctions.getProfilePicture(Username))
                        .Replace("{ID}", getEquipped(Username).ToString());
                }
            }
            conn.Close();
            return html;
        }

        public static int getEquipped(String Username)
        {
            return int.Parse(DBFunctions.getUserData(Username, "ProfilePicture"));
        }

        public static Boolean ppOwned(String username, int ProductID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ProfilePicOwnership;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == ProductID.ToString() && getPrice(int.Parse(reader["ProductID"].ToString())) == 0)
                {
                    conn.Close();
                    return true;
                }
                if (reader["ProductID"].ToString() == ProductID.ToString() && reader["Username"].ToString() == username)
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static int getPrice(int ProductID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM ProfileImages;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == ProductID.ToString())
                {
                    int price = int.Parse(reader["Tier"].ToString()) * 100;
                    conn.Close();
                    return price;
                }
            }
            conn.Close();
            return 0;
        }
    }
}
