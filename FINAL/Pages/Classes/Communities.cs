
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace FINAL.Pages.Classes
{

    public static class Communities
    {

        public static String getPosts(String SessionID, String user, int CommunityID, int cycle) //int requests
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityPosts ORDER BY PostID desc;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";

            int start = (cycle * 15) - 15;
            int i = 0;
            while (reader.Read())
            {

                if (int.Parse(reader["CommunityID"].ToString()) == CommunityID || CommunityID == 0)
                {
                    String baseString = "";
                    if (getPostType(int.Parse(reader["PostID"].ToString())) == 0)
                    {
                        baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/TEXT POSTS.html");
                    }
                    else if (getPostType(int.Parse(reader["PostID"].ToString())) == 1)
                    {
                        baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/IMAGE POSTS.html");
                    }
                    else
                    {
                        baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/VIDEO POSTS.html");
                    }

                    if (ContentModeration.canSeePost(user, int.Parse(reader["PostID"].ToString())) ||
                        (user == null && communityPrivate(CommunityID) == false && communityHidden(CommunityID)))
                    {
                        String video = "";
                        try
                        {
                            String[] videoArray = reader["VideoURL"].ToString().Split('=');
                            video = videoArray[1];
                        }
                        catch { }

                        if (i >= start && i < (cycle * 15))
                        {
                            String temp = baseString.Replace("{TITLE}", reader["Title"].ToString())
                                .Replace("{IMAGEURL}", reader["Image"].ToString()).Replace("{COMMUNITYNAME}",
                                DBFunctions.getCommunityInfo(int.Parse(reader["CommunityID"].ToString()), "CommunityName"))
                                .Replace("{COMMUNITY}", reader["CommunityID"].ToString()).Replace("{ID}", reader["PostID"].ToString())
                                .Replace("{TEXT}", reader["Text"].ToString()).Replace("{CARD}", Communities.getFlairName(int.Parse(reader["FlairID"].ToString())))
                                .Replace("{COMMENTS}", Comments.getComments(int.Parse(reader["PostID"].ToString()))).Replace("{LINK}", video)
                                .Replace("{VIDEOURL}", reader["VideoUrl"].ToString()).Replace("{LIKES}", getPostLikes(int.Parse(reader["PostID"].ToString())).ToString());

                            if (Communities.isAdmin(reader["Username"].ToString()))
                            {
                                temp = temp.Replace("{VERIFIED}", " <i class=\"fas fa-shield-alt\"></i>");
                                temp = temp.Replace("{USERCOLOR}", "var(--admin);");
                            }
                            else if (Communities.isVerified(reader["Username"].ToString()))
                            {
                                temp = temp.Replace("{VERIFIED}", " <i class=\"fas fa-shield-alt\"></i>");
                            }

                            temp = temp.Replace("{USERNAME}", reader["Username"].ToString()).Replace("{VERIFIED}", "").Replace("{USERCOLOR}", "");

                            String options = "";

                            if (UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())))
                            {
                                options += "<p onclick=\"deleteCommunityPost(" + reader["PostID"].ToString() + ", " + reader["CommunityID"].ToString() + ")\">DELETE POST</p>";
                            }

                            if (!UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())) && isAdmin(user))
                            {
                                options += "<p onclick=\"banUser('" + reader["Username"] + "')\">BAN USER</p>";
                            }

                            if (!UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())) && (isAdmin(user)
                                || getCommunityMemberPermissionLevel(user, int.Parse(reader["CommunityID"].ToString())) > 0))
                            {
                                options += "<p onclick=\"communityBanUser(" + reader["CommunityID"].ToString() + ", '" + reader["Username"] + "')\">COMMUNITY BAN USER</p>";
                                options += "<p onclick=\"deleteCommunityPost(" + reader["PostID"].ToString() + ", " + reader["CommunityID"].ToString() + ")\">DELETE POST</p>";
                            }
                            else
                            {
                                options += "<p onclick=\"reportPost(" + reader["PostID"].ToString() + "); postAdminToggle('.post-id-" + reader["PostID"].ToString() + "')\">REPORT POST</p>";
                            }

                            temp = temp.Replace("{OPTIONS}", options);

                            if (SessionID != null)
                            {
                                if (UserInteractions.likedPost(DBFunctions.getUsernameFromSessionID(SessionID), int.Parse(reader["PostID"].ToString())) == true)
                                {
                                    temp = temp.Replace("{LIKECLASS}", "active");
                                }
                                else
                                {
                                    temp = temp.Replace("{LIKECLASS}", "");
                                }
                            }
                            else
                            {
                                if (UserInteractions.likedPost(user, int.Parse(reader["PostID"].ToString())) == true)
                                {
                                    temp = temp.Replace("{LIKECLASS}", "active");
                                }
                                else
                                {
                                    temp = temp.Replace("{LIKECLASS}", "");

                                }

                            }
                            html += temp;
                        }
                        i++;
                    }
                }

            }
            conn.Close();
            return html;
        }

        public static Boolean bannedFromCommunity(String username, int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityBans;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static String getSingularPost(int PostID, String user) //int requests
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityPosts";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {

                if (reader["PostID"].ToString() == PostID.ToString())
                {
                    String baseString = "";
                    if (getPostType(int.Parse(reader["PostID"].ToString())) == 0)
                    {
                        baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/TEXT POSTS.html");
                    }
                    else if (getPostType(int.Parse(reader["PostID"].ToString())) == 1)
                    {
                        baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/IMAGE POSTS.html");
                    }
                    else
                    {
                        baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/VIDEO POSTS.html");
                    }


                    String video = "";
                    try
                    {
                        String[] videoArray = reader["VideoURL"].ToString().Split('=');
                        video = videoArray[1];
                    }
                    catch { }

                    String temp = baseString.Replace("{TITLE}", reader["Title"].ToString())
                        .Replace("{IMAGEURL}", reader["Image"].ToString()).Replace("{COMMUNITYNAME}",
                        DBFunctions.getCommunityInfo(int.Parse(reader["CommunityID"].ToString()), "CommunityName"))
                        .Replace("{COMMUNITY}", reader["CommunityID"].ToString()).Replace("{ID}", reader["PostID"].ToString())
                        .Replace("{TEXT}", reader["Text"].ToString()).Replace("{CARD}", Communities.getFlairName(int.Parse(reader["FlairID"].ToString())))
                        .Replace("{COMMENTS}", Comments.getComments(int.Parse(reader["PostID"].ToString()))).Replace("{LINK}", video)
                        .Replace("{VIDEOURL}", reader["VideoUrl"].ToString()).Replace("{LIKES}", getPostLikes(int.Parse(reader["PostID"].ToString())).ToString());

                    if (Communities.isAdmin(reader["Username"].ToString()))
                    {
                        temp = temp.Replace("{VERIFIED}", " <i class=\"fas fa-shield-alt\"></i>");
                        temp = temp.Replace("{USERCOLOR}", "var(--admin);");
                    }
                    else if (Communities.isVerified(reader["Username"].ToString()))
                    {
                        temp = temp.Replace("{VERIFIED}", " <i class=\"fas fa-shield-alt\"></i>");
                    }

                    temp = temp.Replace("{USERNAME}", reader["Username"].ToString()).Replace("{VERIFIED}", "").Replace("{USERCOLOR}", "");

                    String options = "";

                    if (UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())))
                    {
                        options += "<p onclick=\"deleteCommunityPost(" + reader["PostID"].ToString() + ", " + reader["CommunityID"].ToString() + ")\">DELETE POST</p>";
                    }

                    if (!UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())) && isAdmin(user))
                    {
                        options += "<p onclick=\"banUser('" + reader["Username"] + "')\">BAN USER</p>";
                    }

                    if (!UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())) && (isAdmin(user)
                        || getCommunityMemberPermissionLevel(user, int.Parse(reader["CommunityID"].ToString())) > 0))
                    {
                        options += "<p onclick=\"communityBanUser(" + reader["CommunityID"].ToString() + ", '" + reader["Username"] + "')\">COMMUNITY BAN USER</p>";
                        options += "<p onclick=\"deleteCommunityPost(" + reader["PostID"].ToString() + ", " + reader["CommunityID"].ToString() + ")\">DELETE POST</p>";
                    }
                    else
                    {
                        options += "<p onclick=\"reportPost(" + reader["PostID"].ToString() + "); postAdminToggle('.post-id-" + reader["PostID"].ToString() + "')\">REPORT POST</p>";
                    }

                    temp = temp.Replace("{OPTIONS}", options);

                    if (UserInteractions.likedPost(user, int.Parse(reader["PostID"].ToString())) == true)
                    {
                        temp = temp.Replace("{LIKECLASS}", "active");
                    }
                    else
                    {
                        temp = temp.Replace("{LIKECLASS}", "");
                    }

                    html += temp;
                }

            }
            conn.Close();
            return html;
        }

        public static String getJoinRequests(int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityRequests;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    html += "<div style=\"cursor:pointer;\" id=\"user-request-" + reader["Username"].ToString() + "\" onclick=\"acceptUser('" + reader["Username"].ToString()
                        + "', " + reader["CommunityID"].ToString() + ");\"><p style=\"float:left;\">" + reader["Username"].ToString() + "</p> <i class=\"fas fa-user-check fa-2x\" " +
                        "style=\"float:right;\"></i></div><br><br>";
                }
            }

            //html = "<center><h2 style=\"color:var(--text);\">JOIN REQUESTS</h2></center><br>" + html;
            conn.Close();
            return html;

        }

        public static Boolean hasRequested(String username, int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityRequests;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static Boolean communityPrivate(int CommunityID)
        {
            if (DBFunctions.getCommunityInfo(CommunityID, "Private") == "True")
            {
                return true;
            }
            return false;
        }

        public static Boolean communityHidden(int CommunityID)
        {
            if (DBFunctions.getCommunityInfo(CommunityID, "Hidden") == "True")
            {
                return true;
            }
            return false;
        }

        public static String getPostOwner(int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT PostID, Username FROM CommunityPosts;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["PostID"].ToString() == PostID.ToString())
                {
                    String result = reader["Username"].ToString();
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return null;
        }

        public static int getPostLikes(int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT PostID, Username FROM PostLikes;";
            SqlDataReader reader = query.ExecuteReader();

            int id = 0;
            while (reader.Read())
            {
                if (reader["PostID"].ToString() == PostID.ToString())
                {
                    id++;
                }
            }

            conn.Close();
            return id;
        }

        public static List<String> getCommunityMembers(int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFollowers;";
            SqlDataReader reader = query.ExecuteReader();

            List<String> users = new List<String>();
            while (reader.Read())
            {
                if (reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    users.Add(reader["Username"].ToString());
                }
            }

            conn.Close();
            return users;
        }

        public static String getUsersPosts(String user, String owner, int cycle) //int requests
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT TOP 25 * FROM CommunityPosts ORDER BY PostID desc;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            int start = (cycle * 15) - 15;
            int i = 0;
            while (reader.Read())
            {
                String baseString = "";
                if (getPostType(int.Parse(reader["PostID"].ToString())) == 0)
                {
                    baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/TEXT POSTS.html");
                }
                else if (getPostType(int.Parse(reader["PostID"].ToString())) == 1)
                {
                    baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/IMAGE POSTS.html");
                }
                else
                {
                    baseString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/VIDEO POSTS.html");
                }

                if (ContentModeration.canSeePost(user, int.Parse(reader["PostID"].ToString())))
                {
                    if (reader["Username"].ToString() == owner)
                    {

                        String video = "";
                        try
                        {
                            String[] videoArray = reader["VideoURL"].ToString().Split('=');
                            video = videoArray[1];
                        }
                        catch { }

                        if (i >= start && i < (cycle * 15))
                        {
                            String temp = baseString.Replace("{TITLE}", reader["Title"].ToString())
                            .Replace("{IMAGEURL}", reader["Image"].ToString()).Replace("{COMMUNITYNAME}",
                            DBFunctions.getCommunityInfo(int.Parse(reader["CommunityID"].ToString()), "CommunityName"))
                            .Replace("{COMMUNITY}", reader["CommunityID"].ToString()).Replace("{ID}", reader["PostID"].ToString())
                            .Replace("{TEXT}", reader["Text"].ToString()).Replace("{CARD}", Communities.getFlairName(int.Parse(reader["FlairID"].ToString())))
                            .Replace("{COMMENTS}", Comments.getComments(int.Parse(reader["PostID"].ToString()))).Replace("{LINK}", video)
                            .Replace("{VIDEOURL}", reader["VideoUrl"].ToString()).Replace("{LIKES}", getPostLikes(int.Parse(reader["PostID"].ToString())).ToString());

                            if (Communities.isAdmin(reader["Username"].ToString()))
                            {
                                temp = temp.Replace("{VERIFIED}", " <i class=\"fas fa-shield-alt\"></i>");
                                temp = temp.Replace("{USERCOLOR}", "var(--admin);");
                            }
                            else if (Communities.isVerified(reader["Username"].ToString()))
                            {
                                temp = temp.Replace("{VERIFIED}", " <i class=\"fas fa-shield-alt\"></i>");
                            }

                            temp = temp.Replace("{USERNAME}", reader["Username"].ToString()).Replace("{VERIFIED}", "").Replace("{USERCOLOR}", "");

                            String options = "";

                            if (UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())))
                            {
                                options += "<p onclick=\"deleteCommunityPost(" + reader["PostID"].ToString() + ", " + reader["CommunityID"].ToString() + ")\">DELETE POST</p>";
                            }

                            if (!UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())) && isAdmin(user))
                            {
                                options += "<p onclick=\"banUser('" + reader["Username"] + "')\">BAN USER</p>";
                            }

                            if (!UserPosts.ownsPost(user, int.Parse(reader["PostID"].ToString())) && (isAdmin(user)
                                || getCommunityMemberPermissionLevel(user, int.Parse(reader["CommunityID"].ToString())) > 0))
                            {
                                options += "<p onclick=\"communityBanUser(" + reader["CommunityID"].ToString() + ", '" + reader["Username"] + "')\">COMMUNITY BAN USER</p>";
                                options += "<p onclick=\"deleteCommunityPost(" + reader["PostID"].ToString() + ", " + reader["CommunityID"].ToString() + ")\">DELETE POST</p>";
                            }
                            else
                            {
                                options += "<p onclick=\"reportPost(" + reader["PostID"].ToString() + "); postAdminToggle('.post-id-" + reader["PostID"].ToString() + "')\">REPORT POST</p>";
                            }

                            temp = temp.Replace("{OPTIONS}", options);

                            if (UserInteractions.likedPost(user, int.Parse(reader["PostID"].ToString())) == true)
                            {
                                temp = temp.Replace("{LIKECLASS}", "active");
                            }
                            else
                            {
                                temp = temp.Replace("{LIKECLASS}", "");
                            }

                            html += temp;
                        }
                        i++;
                    }
                }

            }
            conn.Close();
            return html;
        }

        public static int getPostType(int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityPosts";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["PostID"].ToString() == PostID.ToString())
                {
                    if (reader["Text"].ToString() != null && reader["Text"].ToString() != "")
                    {
                        conn.Close();
                        return 0;
                    }
                    else if (reader["Image"].ToString() != null && reader["Image"].ToString() != "")
                    {
                        conn.Close();
                        return 1;
                    }
                    else
                    {
                        conn.Close();
                        return 2;
                    }
                }
            }
            conn.Close();
            return 0;
        }

        public static int getFlairIDFromName(String flairName)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFlairs;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (flairName != null && (reader["FlairName"].ToString().ToLower() == flairName.ToLower()))
                {
                    int result = int.Parse(reader["FlairID"].ToString());
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return 0;
        }

        public static String getFlairName(int flairID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFlairs;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["FlairID"].ToString() == flairID.ToString())
                {
                    String result = reader["FlairName"].ToString();
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return null;
        }

        public static int getCommunityIDFromName(String communityName)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (communityName != null && (reader["CommunityName"].ToString().ToLower() == communityName.ToLower()))
                {
                    int result = int.Parse(reader["CommunityID"].ToString());
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return 0;
        }

        public static String getCommunityInviteCode(int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    String result = reader["InviteCode"].ToString();
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return null;
        }

        public static String getUserCommunitiesNames(String username)
        {
            String html = "<option selected value='0'>MY PROFILE</option>";
            try
            {
                List<String> communities = getUsersCommunitiesIDs(username);

                foreach (String ID in communities)
                {
                    String community = DBFunctions.getCommunityInfo(int.Parse(ID), "CommunityName");
                    html += "<option value='" + community + "'>" + community + "</option>";
                }
            }
            catch
            {

            }

            return html;
        }

        public static String getCommunitiesFlairs()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFlairs;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {

                html += "<option value='" + reader["FlairName"].ToString() + "'>" + reader["FlairName"].ToString().ToUpper() + "</option>";
            }
            conn.Close();
            return html;
        }

        public static List<String> getUsersCommunitiesIDs(String username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFollowers;";
            SqlDataReader reader = query.ExecuteReader();

            List<String> users = new List<string>();
            while (reader.Read())
            {
                if (reader["Username"].ToString() == username)
                {
                    users.Add(reader["CommunityID"].ToString());
                }
            }
            conn.Close();
            return users;
        }

        public static String listNewCommunities()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT TOP 5 * FROM Communities ORDER BY CommunityID DESC;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                html += File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/NEWCOMMUNITIES.html")
                    .Replace("{IMAGEURL}", reader["AvatarUrl"].ToString()).Replace("{COMMUNITYNAME}", reader["CommunityName"].ToString())
                    .Replace("{COMMUNITYID}", reader["CommunityID"].ToString()).Replace("{TYPE}", "Community");
            }

            conn.Close();
            return html;
        }

        public static String listNewUsers()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT TOP 5 * FROM Users ORDER BY UserID DESC;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                html += File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/NEWCOMMUNITIES.html")
                    .Replace("{IMAGEURL}", DBFunctions.getProfilePicture(reader["Username"].ToString())).Replace("{COMMUNITYNAME}", reader["Username"].ToString())
                    .Replace("{COMMUNITYID}", reader["Username"].ToString()).Replace("{TYPE}", "Profile");
            }

            conn.Close();
            return html;
        }

        public static Boolean isAdmin(String username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT Username, Admin FROM Users;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["Admin"].ToString() == "True")
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static Boolean isVerified(String username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT Username, Verified FROM Users;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["Verified"].ToString() == "True")
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static String getCommunityBio(int CommunityID)
        {
            return DBFunctions.getCommunityInfo(CommunityID, "CommunityBio");
        }

        public static int getCommunityMemberPermissionLevel(String username, int CommunityID)
        {
            if (isAdmin(username))
            {
                return 2;
            }
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityModerators";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == username && reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    int result = int.Parse(reader["PermissionLevel"].ToString());
                    conn.Close();
                    return result;
                }
            }

            conn.Close();
            return 0;
        }

        public static int getMostRecentCommunityID()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT CommunityID FROM Communities";
            SqlDataReader reader = query.ExecuteReader();

            int cid = 0;

            while (reader.Read())
            {
                cid = int.Parse(reader["CommunityID"].ToString());
            }

            return cid;
        }

        public static String getRecommendedCommunities(String username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities";
            SqlDataReader reader = query.ExecuteReader();

            List<String> communities = new List<String>();
            while (reader.Read())
            {
                if (!UserInteractions.subscribed(username, int.Parse(reader["CommunityID"].ToString()))
                    && !Communities.communityHidden(int.Parse(reader["CommunityID"].ToString())))
                {
                    String bio = reader["CommunityBio"].ToString();
                    if (bio.Length > 30)
                    {
                        bio = bio.Substring(0, 30) + "...";
                    }

                    String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/RECOMMENDEDCOMMUNITIES.html")
                        .Replace("{ID}", reader["CommunityID"].ToString()).Replace("{IMAGE}", reader["AvatarUrl"].ToString())
                        .Replace("{BIO}", bio).Replace("{COMMUNITY}", reader["CommunityName"].ToString().ToUpper());

                    communities.Add(baseString);
                }
            }

            Random rand = new Random();
            String[] randomisedCommunities = communities.OrderBy(x => rand.Next()).ToArray();
            String html = "";
            int i = 0;
            foreach (String comm in randomisedCommunities)
            {
                if (i < 9)
                {
                    html += comm;
                }
                i++;
            }
            return html;
        }


        public static String getAllCommunities()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();

            SqlCommand query_count = conn.CreateCommand();
            query_count.CommandText = "SELECT Count(*) FROM CommunityModerators";
            Int32 count = (Int32)query_count.ExecuteScalar();
            conn.Close();
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();


            //Console.WriteLine("community count was =" + count);

            String communityList = "";
            int i = 0;
            while (reader.Read())
            {
                String name = reader["CommunityName"].ToString().ToUpper();
                communityList += name;

                i++;

                if (i != count)
                {
                    communityList += ',';
                }


            }
            conn.Close();
            return communityList;
        }

        public static String getRightBarCommunities(String username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            int i = 0;
            while (reader.Read())
            {
                String name = reader["CommunityName"].ToString().ToUpper();

                if (name.Length > 20)
                {
                    name = name.Substring(0, 20) + "...";
                }

                int CommunityID = int.Parse(reader["CommunityID"].ToString());
                String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/RIGHTBAR/COMMUNITIES.html");
                if ((UserInteractions.subscribed(username, CommunityID)) || ((username == null || username == "Steve" || username == "" || username == "null") &&
                    (!Communities.communityPrivate(CommunityID) && !Communities.communityHidden(CommunityID))))
                {
                    baseString = baseString.Replace("{ID}", CommunityID.ToString())
                        .Replace("{IMAGE}", reader["AvatarUrl"].ToString()).Replace("{COMMUNITY}", name)
                        .Replace("{ONCLICK}", "window.location.href = '/Community?id=" + CommunityID + "';");
                    html += baseString;
                    i++;
                }
            }

            i++;
            for (int x = i; x < 14; x++)
            {
                String baseString = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/RIGHTBAR/COMMUNITIES.html");
                baseString = baseString.Replace("{ID}", x.ToString() + "ex").Replace("{IMAGE}", "/images/communityplaceholder.png")
                    .Replace("{COMMUNITY}", "EXPLORE COMMUNITIES")
                    .Replace("{ONCLICK}", "menuActive(7);");
                html += baseString;
            }

            conn.Close();
            return html;
        }
    }
}
