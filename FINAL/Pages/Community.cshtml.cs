using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.IO;
using FINAL.Pages.Classes;

namespace Group_Project.Models
{
    public class Community : PageModel
    {
        String connectionString = DBFunctions.connectionString;
        public String communityName;
        public String communityBio;
        public String subButtonText;
        public String subButtonAction;
        public int communityID;
        public String aboutText = "ABOUT";
        public String aboutAction;
        public String back;
        public String reportButton = "REPORT";
        public String reportAction = "";
        public String postText = "POST";
        public String postAction = "selectPost(1); menuActive(3);";
        public String enabled = "";
        public int members = 0;
        public void onLoad()
        {
            HttpContext.Response.Cookies.Delete("Com");
            String logger = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);

            if (HttpContext.Request.Cookies["SessionID"] == null || HttpContext.Request.Cookies["SessionID"] == "" || logger == null || logger == "")
            {
                HttpContext.Response.Cookies.Delete("SessionID");
                Response.Redirect("Login");
                return;
            }

            subButtonText = "SUBSCRIBE";

            try
            {
                String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
                String id = IDarray[0];
                members = getMemberAmount(int.Parse(id));
                reportAction = "reportCommunity(" + id + ");";
                subButtonAction = "subscribe(" + id + ");";
                communityName = DBFunctions.getCommunityInfo(int.Parse(id), "CommunityName");
                communityBio = DBFunctions.getCommunityInfo(int.Parse(id), "CommunityBio");
                back = DBFunctions.getCommunityInfo(int.Parse(id), "AvatarUrl");
                communityID = int.Parse(id);
                String viewer = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);

                if (!UserInteractions.subscribed(viewer, int.Parse(id)) && Communities.communityHidden(int.Parse(id)))
                {
                    Response.Redirect("/");
                    return;
                }

                if (Communities.getCommunityMemberPermissionLevel(viewer, int.Parse(id)) > 0)
                {
                    reportButton = "SETTINGS";
                    reportAction = "window.location.href = '/CommunitySettings?id=" + id + "';";

                    if (Communities.communityPrivate(int.Parse(id)) == true)
                    {
                        aboutText = "REQUESTS";
                        // aboutAction = "document.getElementById('requests-box').style.visibility = 'visible';";
                    }
                }

                if (Communities.bannedFromCommunity(viewer, int.Parse(id)))
                    {
                    enabled = "disabled";
                }

                if (Communities.communityHidden(int.Parse(id)))
                {
                    postText = "INVITE";
                    postAction = "inviteUsers(" + id + ")";
                }

                if (UserInteractions.subscribed(viewer, int.Parse(id)))
                {
                    subButtonText = "SUBSCRIBED";
                }
                else if (Communities.communityPrivate(int.Parse(id)) == true)
                {
                    subButtonText = "REQUEST";

                    if (Communities.hasRequested(viewer, int.Parse(id)))
                    {
                        subButtonText = "REQUESTED";
                    }
                    else
                    {
                        subButtonAction = "requestJoin(" + id + ")";
                    }
                }

                if (Communities.communityPrivate(communityID) && UserInteractions.subscribed(viewer, communityID))
                {
                    subButtonAction = "leaveCommunity(" + communityID + ");";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public String getPosts()
        {
            String displayTo = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);

            String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
            String id = IDarray[0];

            if(id == null || id == "")
            {
                return null;
            }

            if (Communities.bannedFromCommunity(displayTo, int.Parse(id)))
            {
                return "<center><h2 style=\"margin-top:75px; color:#c2c2c2;\">YOU ARE BANNED FROM THIS COMMUNITY</h2>";
            }

            communityName = DBFunctions.getCommunityInfo(int.Parse(id), "CommunityName");
            communityBio = DBFunctions.getCommunityInfo(int.Parse(id), "CommunityBio");

            if (!ContentModeration.canSeeCommunity(displayTo, int.Parse(id)))
            {
                return null;
            }

            if (id != null && id != "")
            {
                if (Communities.communityPrivate(int.Parse(id)) == false)
                {
                    return Communities.getPosts(HttpContext.Request.Cookies["SessionID"], null, int.Parse(id), 1);
                }
                return Communities.getPosts(null, displayTo, int.Parse(id), 1);
            }

            Response.Redirect("/");
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

        public static int getMemberAmount(int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFollowers;";
            SqlDataReader reader = query.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                if(reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    i++;
                }
            }

            conn.Close();
            return i;
        }
    }
}
