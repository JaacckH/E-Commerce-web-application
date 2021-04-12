using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace FINAL.Pages
{
    public class ShopModel : PageModel
    {
        public String getProductShowcase()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT TOP 20 * FROM Products";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                html += ProductFunctions.getSubProductHtml(reader["ProductID"].ToString());
            }

            conn.Close();
            return html;
        }
    }
}
