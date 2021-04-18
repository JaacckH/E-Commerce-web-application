using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace FINAL.Pages.Admin
{
    public class ProductsModel : PageModel
    {
        public void OnGet()
        {
        }

        public String getProducts()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Products ORDER BY ID DESC";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            List<String> users = new List<String>();
            while (reader.Read())
            {
                String stock = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTSTATUS/2.html");
                if (Stock.productInStock(reader["ProductID"].ToString()))
                {
                    stock = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/PRODUCTSTATUS/1.html");
                }

                String baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/SETTINGS/EDITPRODUCT.html")
                    .Replace("{ID}", reader["ProductID"].ToString()).Replace("{NAME}", reader["Name"].ToString())
                    .Replace("{DESCRIPTION}", reader["Description"].ToString()).Replace("{CATEGORY}", reader["Category"].ToString())
                    .Replace("{PRICE}", Utility.formatPrice(reader["Price"].ToString())).Replace("{SALEPRICE}", Utility.formatPrice(reader["WasPrice"].ToString()))
                    .Replace("{IMAGE}", reader["ImagePath"].ToString()).Replace("{DESCRIPTION}", reader["Description"].ToString())
                    .Replace("{MATERIALS}", reader["Materials"].ToString())
                    .Replace("{STOCK}", stock)

                    .Replace("{SIZE10}", getQuantityOfSize(reader["ProductID"].ToString(), "10").ToString())
                    .Replace("{SIZE12}", getQuantityOfSize(reader["ProductID"].ToString(), "12").ToString())
                    .Replace("{SIZE14}", getQuantityOfSize(reader["ProductID"].ToString(), "14").ToString())
                    .Replace("{SIZE16}", getQuantityOfSize(reader["ProductID"].ToString(), "16").ToString())
                    .Replace("{SIZE18}", getQuantityOfSize(reader["ProductID"].ToString(), "18").ToString())
                    .Replace("{SIZE20}", getQuantityOfSize(reader["ProductID"].ToString(), "20").ToString())
                    .Replace("{SIZES}", getSizesHTML(reader["ProductID"].ToString()));


                html += baseString;
            }

            conn.Close();
            return html;
        }

        public String getSizesHTML(String productID)
        {
            String sizes = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/SETTINGS/SIZESELECT.html");
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Stock";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            int i = 1;
            List<String> users = new List<String>();
            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID)
                {
                    html += sizes.Replace("{ID}", i.ToString()).Replace("{SIZE}", reader["SizeID"].ToString())
                        .Replace("{QUANTITY}", reader["Quantity"].ToString()).Replace("selected" + reader["SizeID"].ToString(), "selected")
                        .Replace("{PID}", reader["ProductID"].ToString());
                    i++;
                }
            }

            html = html.Replace("{SCRIPT}", "sizeColAmount = " + i + ";");

            conn.Close();
            return html;
        }

        public int getQuantityOfSize(String productID, String size)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Stock";
            SqlDataReader reader = query.ExecuteReader();

            int quantity = 0;
            List<String> users = new List<String>();
            while (reader.Read())
            {
                if (reader["ProductID"].ToString() == productID && reader["SizeID"].ToString() == size)
                {
                    quantity = int.Parse(reader["Quantity"].ToString());
                }
            }

            conn.Close();
            return quantity;
        }
    }
}
