using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Pages.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Group_Project.Models
{
    public class Post : PageModel
    {
        public String getPost()
        {
            String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
            String id = IDarray[0];

            if(id == null || id == "")
            {
                Response.Redirect("/");
            }

            if (ContentModeration.postDeleted(int.Parse(id)))
            {
                Response.Redirect("/");
                return "";
            }

            String username = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);

            if (FINAL.Pages.Classes.CommunitySettings.bannedFromCommunity(username, ContentModeration.getCommunityFromPost(int.Parse(id))) == "checked")
            {
                Response.Redirect("/");
                return "";
            }

            return Communities.getSingularPost(int.Parse(id), username);

        }



    }
}
