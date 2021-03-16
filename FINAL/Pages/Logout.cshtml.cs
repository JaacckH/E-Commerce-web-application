using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Pages.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Group_Project.Models
{
    public class Logout : PageModel
    {
        public void logout()
        {
            String username = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);
            DBFunctions.SendQuery("DELETE FROM Notifications WHERE Username='" + username + "' AND Seen='True'");
            HttpContext.Response.Cookies.Delete("SessionID");
            Response.Redirect("/");
        }
    }
}
