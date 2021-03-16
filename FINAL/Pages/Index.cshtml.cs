using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Pages.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group_Project.Models
{
    public class IndexModel : PageModel
    {
        public void onLoad()
        {
            HttpContext.Response.Cookies.Delete("Com");
            if (HttpContext.Request.Cookies["SessionID"] == null || HttpContext.Request.Cookies["SessionID"] == "")
            {
                Response.Redirect("Login");
            }
        }



    }
}
