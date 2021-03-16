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
    public class Profile : PageModel
    {
        public string connectionString = DBFunctions.connectionString;

        public string Username, Email, DisplayName, UserType, Gender, Password, DOB, ColourMode, BIO,
            followButton, followAction, btnClass, messageText, messageAction, ProfilePic, ReportText, ReportAction,
            ignoreButton, ignoreAction, Displayname, nameColor, FavCommunity;
        public string favComms = "No Fave communities";
        public void onLoad()
        {

            String logger = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);
            if (HttpContext.Request.Cookies["SessionID"] == null || HttpContext.Request.Cookies["SessionID"] == "" || logger == null || logger == "")
            {
                HttpContext.Response.Cookies.Delete("SessionID");
                Response.Redirect("Login");
                return;
            }

            String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
            String id = IDarray[0];

            if (id == "" || id == null)
            {
                id = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"].ToString());
                Response.Redirect("/Profile?id=" + id);
                return;
            }

            if (DBFunctions.getUserData(id, "Username") == "null")
            {
                Response.Redirect("/");
                return;
            }

            Username = id;
            BIO = DBFunctions.getUserData(Username, "Bio");
            DisplayName = Username;
            ProfilePic = DBFunctions.getProfilePicture(id);
            btnClass = "";
            nameColor = "var(--background-2);";
            messageAction = "messageUser('" + Username + "');";
            messageText = "MESSAGE";
            String viewer;
            viewer = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"].ToString());
            followAction = "toggleFollow('" + Username + "');";
            if (DBFunctions.getUserData(Username, "FavGames") != "null")
            {
                favComms = DBFunctions.getUserData(Username, "FavGames");
            }
           
            
                if (Communities.isAdmin(Username))
            {
                DisplayName = DisplayName + " <i class=\"fas fa-shield-alt\"></i>";
                nameColor = "var(--admin);";
            }
            else if (Communities.isVerified(Username))
            {
                DisplayName = DisplayName + " <i class=\"fas fa-shield-alt\"></i>";
            }



            if (id != null && id != "")
            {
                if (viewer == id)
                {
                    ignoreButton = "SHOP";
                    ignoreAction = "menuActive(4)";
                    ReportAction = "redirectLogout();";
                    ReportText = "LOGOUT";
                    messageAction = "menuActive(5); LoadProfile();";
                    messageText = "SETTINGS";
                    followButton = "EDIT";
                    followAction = "menuActive(5); LoadProfile();";
                }
                else
                {
                    ignoreButton = "IGNORE";
                    ignoreAction = "ignoreUser();";
                    ReportText = "REPORT";
                    ReportAction = "reportUser('" + Username + "');";
                    followButton = "FOLLOW";
                    if (UserInteractions.isFollowing(viewer, id))
                    {
                        followButton = "FOLLOWING";
                        btnClass = "SUBSCRIBED";
                    }
                }
            }

            // establish connecion to the database
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();

            // specifying what we want to return
            query.CommandText = "SELECT * FROM Users;";

            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                // storing data in strings
                try
                {
                    if (reader["Username"].ToString() == Username)
                    {
                        Email = reader["Email"].ToString();
                        UserType = reader["UserType"].ToString();
                        Gender = reader["Gender"].ToString();
                        DOB = reader["DOB"].ToString();
                        ColourMode = reader["ColourMode"].ToString();


                    }
                }
                catch (Exception) { }

            }
        }

        public String getPosts()
        {
            return Communities.getUsersPosts(DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]), Username, 1);
        }


    }
}
