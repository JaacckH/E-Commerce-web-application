﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Classes
{
    public static class Basket
    {
        public static int getNumOfItems(String sessionID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                if (reader["UserID"].ToString() == UserFunctions.getUserID(sessionID))
                {
                    i++;
                }
            }

            conn.Close();
            return i;
        }

        public static int getItemQuantity(String userID, int productID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["UserID"].ToString() == userID && reader["ProductID"].ToString() == productID.ToString())
                {
                    int result = int.Parse(reader["Quantity"].ToString());
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return 0;
        }

        public static String getProductHtml(int productID, int quantity)
        {
            String baseString = File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTBASKET.html");
            String price = "", imagePath = "", name = "";

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Products";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID.ToString())
                {
                    price = reader["Price"].ToString();
                    name = reader["Name"].ToString();
                    imagePath = reader["ImagePath"].ToString();
                }
            }

            conn.Close();
            baseString = baseString.Replace("{PRICE}", price).Replace("{IMAGE}", imagePath)
                .Replace("{ID}", productID.ToString()).Replace("{QUANTITY}", quantity.ToString())
                .Replace("{NAME}", name);

            return baseString;
        }

        public static Boolean containsItem(String userID, int itemID)
        {
            if (getItemQuantity(userID, itemID) == 0)
            {
                return false;
            }
            return true;
        }

        public static String getBasketHtml(String SessionID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["UserID"].ToString() == UserFunctions.getUserID(SessionID))
                {
                    html += getProductHtml(int.Parse(reader["ProductID"].ToString()),
                        getItemQuantity(reader["UserID"].ToString(), int.Parse(reader["ProductID"].ToString())));
                }
            }

            return html;
        }

        public static int getTotalPrice(String userID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            int price = 0;
            while (reader.Read())
            {
                if(reader["UserID"].ToString() == userID)
                {
                    int quantity = int.Parse(reader["Quantity"].ToString());
                    price += quantity * ProductFunctions.getProductPrice(int.Parse(reader["ProductID"].ToString()));
                }
            }

            conn.Close();
            return price;
        }

        public static List<int> getProductIDs(String userID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Basket";
            SqlDataReader reader = query.ExecuteReader();

            List<int> products = new List<int>();
            while (reader.Read())
            {
                if (reader["UserID"].ToString() == userID)
                {
                    int quantity = int.Parse(reader["Quantity"].ToString());

                    for (int i = 0; i < quantity; i++)
                    {
                        products.Add(int.Parse(reader["ProductID"].ToString()));
                    }
                }
            }

            conn.Close();
            return products;
        }
    }
}