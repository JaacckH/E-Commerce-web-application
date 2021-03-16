using System;
using System.Security.Cryptography;
using System.Text;
using FINAL.Pages.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Group_Project.Models
{
    public class BANNED : PageModel
    {
        public ActionResult processAppeal()
        {
            try
            {
                String reason = Request.Form["reason"];
                if (reason != null && reason != "")
                {
                    String SessionID = HttpContext.Request.Cookies["SessionID"];
                    String username = DBFunctions.getUsernameFromSessionID(SessionID);
                    DBFunctions.SendQuery("INSERT INTO UserAppeals (Username, Reason) VALUES('" + username + "', '" + reason + "')");
                }
            }
            catch
            {

            }
            return null;
        }

        public Boolean hasAppealed()
        {
            String SessionID = HttpContext.Request.Cookies["SessionID"];
            String username = DBFunctions.getUsernameFromSessionID(SessionID);

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT Username FROM UserAppeals;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username)
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }
    }
}
