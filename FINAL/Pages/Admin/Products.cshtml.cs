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
            query.CommandText = "SELECT * FROM Products ORDER BY Status DESC";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            List<String> users = new List<String>();
            while (reader.Read())
            {
                String baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/HTML/SETTINGS/EDITPRODUCT.html")
                    .Replace("{ID}", reader["ProductID"].ToString()).Replace("{NAME}", reader["Name"].ToString())
                    .Replace("{DESCRIPTION}", reader["Description"].ToString()).Replace("{CATEGORY}", reader["Category"].ToString())
                    .Replace("{PRICE}", Utility.formatPrice(reader["Price"].ToString())).Replace("{SALEPRICE}", Utility.formatPrice(reader["Price"].ToString()))
                    .Replace("{IMAGE}", reader["ImagePath"].ToString()).Replace("{DESCRIPTION}", reader["Description"].ToString());
                html += baseString;
            }

            conn.Close();
            return html;
        }



    }
}
