using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using FINAL.Pages.Classes;
using System.Text;

namespace Group_Project.Models
{
    public class CreateAccount : PageModel
    {
        public string HashedPassword, HashedRecovery, Recovery;
        public string Message { get; set; }
        static String connectionString = DBFunctions.connectionString;
        public List<string> Communities { get; set; }
        public List<string> CommunityID { get; set; }
        public List<string> CommunityPP { get; set; }

        public void onLoad()
        {
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["SessionID"]))
            {
                Response.Redirect("Index");
            }
            else
            {
                SqlConnection conn = new SqlConnection();
                conn.ConnectionString = connectionString;
                conn.Open();
                SqlCommand query = conn.CreateCommand();
                query.CommandText = "SELECT * FROM Communities;";
                SqlDataReader reader = query.ExecuteReader();

                Communities = new List<string>();
                CommunityID = new List<string>();
                CommunityPP = new List<string>();

                while (reader.Read())
                {
                    if (reader["Private"].ToString() != "True" && reader["Hidden"].ToString() != "True")
                    {
                        String Com = reader["CommunityName"].ToString();
                        String ComID = reader["CommunityID"].ToString();
                        String Compp = reader["AvatarUrl"].ToString();
                        Communities.Add(Com);
                        CommunityID.Add(ComID);
                        CommunityPP.Add(Compp);
                    }
                }
                reader.Close();
            }
        }

        public IActionResult createAccount()
        {
            string username = "";
            try
            {
                for (int i = 0; i < Communities.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Request.Form["Community-Check-" + CommunityID[i]]))
                    {
                        DBFunctions.SendQuery("INSERT INTO CommunityFollowers (Username, CommunityID) VALUES ('" + username + "', '" + CommunityID[i] + "')");
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
