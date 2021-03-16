using System;
using System.Security.Cryptography;
using System.Text;
using FINAL.Pages.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Group_Project.Models
{
    public class Login : PageModel
    {
        public void onPageLoad()
        {
            string cookie = Request.Cookies["SessionID"];
            if (cookie != null && cookie != "")
            {
                Response.Redirect("Logout");
            }
        }
    }
}
