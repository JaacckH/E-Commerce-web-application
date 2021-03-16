using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.IO;
using FINAL.Pages.Classes;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Group_Project.Models
{
    public class CommunitySettings : PageModel
    {
        String connectionString = DBFunctions.connectionString;
        public String communityName;
        public String communityBio;
        public String subButtonText;
        public int communityID;
        public String back;
        public void onLoad()
        {

            String logger = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);

            if (HttpContext.Request.Cookies["SessionID"] == null || HttpContext.Request.Cookies["SessionID"] == "" || logger == null || logger == "")
            {
                HttpContext.Response.Cookies.Delete("SessionID");
                Response.Redirect("Login");
                return;
            }

            try
            {
                String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
                String id = IDarray[0];

                if (id == null || id == "")
                {
                    Redirect("/");
                    return;
                }

                communityName = DBFunctions.getCommunityInfo(int.Parse(id), "CommunityName");
                communityBio = DBFunctions.getCommunityInfo(int.Parse(id), "CommunityBio");
                back = DBFunctions.getCommunityInfo(int.Parse(id), "AvatarUrl");
                communityID = int.Parse(id);
                HttpContext.Response.Cookies.Append("Community", communityID + "");
                String viewer = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);

                if (Communities.getCommunityMemberPermissionLevel(viewer, int.Parse(id)) < 1)
                {
                    Response.Redirect("Index");
                    return;
                }

                if (UserInteractions.subscribed(viewer, int.Parse(id)))
                {
                    subButtonText = "SUBSCRIBED";
                }
            }
            catch
            {

            }
        }

        public ActionResult uploadImage()
        {

            try
            {
                String SessionID = HttpContext.Request.Cookies["SessionID"];
                String username = DBFunctions.getUsernameFromSessionID(SessionID);
                String img = Request.Form["cpp"].ToString();
                String community = Request.Form["comid"].ToString();
                if (img.Length > 300 || img.Contains("data"))
                {
                    if (Communities.getCommunityMemberPermissionLevel(username, int.Parse(community)) > 0)
                    {
                        DBFunctions.SendQuery("UPDATE Communities SET AvatarUrl='" + img + "' WHERE CommunityID='" + community + "';");
                        HttpContext.Response.Cookies.Delete("Community");
                    }
                }
                Response.Redirect("/CommunitySettings?id=" + community);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public String getHeader()
        {
            String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
            String id = IDarray[0];
            int community = int.Parse(id);
            String avatar = DBFunctions.getCommunityInfo(community, "AvatarURL");
            String name = DBFunctions.getCommunityInfo(community, "CommunityName");
            String bio = DBFunctions.getCommunityInfo(community, "CommunityBio");

            return System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/COMMUNITY HEADER.html")
                .Replace("{IMAGE}", avatar).Replace("{COMMUNITY}", name).Replace("{BIO}", bio);
        }
    }
}
