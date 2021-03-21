using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FINAL.Pages
{
    public class LoginModel : PageModel
    {
        public void onLoad()
        {
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["SessionID"]))
            {
                if(UserFunctions.userLoggedIn(HttpContext.Request.Cookies["SessionID"]) == true)
                {
                    Response.Redirect("/");
                    return;
                }
            }
        }
    }
}
