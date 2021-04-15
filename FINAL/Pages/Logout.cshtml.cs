using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group_Project.Models
{
    public class Logout : PageModel
    {
        public void logout()
        {
            HttpContext.Response.Cookies.Delete("SessionID");
            Response.Redirect("Index");
        }
    }
}
