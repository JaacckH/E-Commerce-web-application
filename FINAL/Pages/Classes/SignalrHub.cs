using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace FINAL.Pages.Classes
{
    public class SignalrHub : Hub
    {
        public async Task SendMessage(String username, String recipient, String message)
        {

            String user = DBFunctions.getUsernameFromSessionID(username);

            if (user == recipient)
            {
                await showError(Context.ConnectionId, "SOMEONE'S DESPERATE FOR FRIENDS...");
                return;
            }

            Console.WriteLine(user + ", " + recipient + ", " + message);
            DBFunctions.SendQuery("INSERT INTO Messages (Sender, Recipient, Message) VALUES('" + user + "', '" + recipient + "', '" + message + "');");
            await Clients.Client(DBFunctions.getConnectionID(recipient)).SendAsync("ReceiveMessage", user, recipient, DBFunctions.returnFormedMessage(1, message));
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", user, recipient, DBFunctions.returnFormedMessage(0, message));
            await sendContent(Context.ConnectionId, ContentPull.getRecents(username, false), "RECENT");
            await sendContent(DBFunctions.getConnectionID(recipient), ContentPull.getRecents(DBFunctions.getSessionIDFromUsername(recipient), false), "RECENT");
        }

        public void saveConnectionID(String user)
        {
            user = DBFunctions.getUsernameFromSessionID(user);
            DBFunctions.SendQuery("DELETE FROM ConnectionID WHERE Username='" + user + "';");
            DBFunctions.SendQuery("INSERT INTO ConnectionID (Username, ConnectionID) VALUES ('" + user + "', '" + Context.ConnectionId + "');");
            GC.Collect();
        }

        public async Task getUserList(String user, Boolean search)
        {
            await sendContent(Context.ConnectionId, ContentPull.getRecents(user, search), "RECENT");
        }

        public async Task searchGetUsers(String input, String SessionID)
        {
            String user = DBFunctions.getUsernameFromSessionID(SessionID);
            await sendContent(Context.ConnectionId, Search.getUsers(input, user), "searchUsers");
            await sendContent(Context.ConnectionId, Search.getCommunities(input, user), "searchCommunities");
        }

        public async Task getMessages(String recipient, String user)
        {
            user = DBFunctions.getUsernameFromSessionID(user);
            String html = DBFunctions.returnMessages(user, recipient);
            await sendContent(Context.ConnectionId, html, "messagesList");
        }

        public async Task followUser(String SessionID, String user)
        {
            if (SessionID != null && SessionID != "")
            {
                UserInteractions.followToggle(DBFunctions.getUsernameFromSessionID(SessionID), user);
                if (UserInteractions.isFollowing(DBFunctions.getUsernameFromSessionID(SessionID), user))
                {
                    await sendContent(Context.ConnectionId, "FOLLOWING", "followButton");
                    Shards.giveShards(user, 3);
                    await showAcknowledge(Context.ConnectionId, "YOU FOLLOWED " + user.ToUpper());
                }
                else
                {
                    await sendContent(Context.ConnectionId, "FOLLOW", "followButton");
                }
            }
        }

        

        public async Task subscribe(String SessionID, int CommunityID)
        {
            if (SessionID != null && SessionID != "")
            {
                if (Communities.bannedFromCommunity(DBFunctions.getUsernameFromSessionID(SessionID), CommunityID))
                {
                    await showError(Context.ConnectionId, "NICE TRY FELLA");
                    return;
                }

                if (!Communities.communityPrivate(CommunityID))
                {
                    UserInteractions.ToggleSubscribe(DBFunctions.getUsernameFromSessionID(SessionID), CommunityID);
                    if (UserInteractions.subscribed(DBFunctions.getUsernameFromSessionID(SessionID), CommunityID))
                    {
                        await sendContent(Context.ConnectionId, "SUBSCRIBED", "sub-btn");
                        Shards.giveShards(DBFunctions.getUsernameFromSessionID(SessionID), 1);
                        await showAcknowledge(Context.ConnectionId, "YOU SUBSCRIBED TO " + DBFunctions.getCommunityInfo(CommunityID, "CommunityName").ToUpper());
                    }
                    else
                    {
                        Shards.removeShards(DBFunctions.getUsernameFromSessionID(SessionID), 1);
                        await sendContent(Context.ConnectionId, "SUBSCRIBE", "sub-btn");
                    }
                }
            }
        }

        public async Task requestJoin(String SessionID, int CommunityID)
        {
            if (SessionID != null && SessionID != "")
            {
                String user = DBFunctions.getUsernameFromSessionID(SessionID);
                if (!Communities.hasRequested(user, CommunityID))
                {
                    DBFunctions.SendQuery("INSERT INTO CommunityRequests (Username, CommunityID) VALUES('" + user + "', '" + CommunityID + "')");
                    await sendContent(Context.ConnectionId, "REQUESTED", "sub-btn");
                    await showAcknowledge(Context.ConnectionId, "JOIN REQUEST SENT");
                }
            }
        }

        public async Task likePost(String SessionID, int id)
        {
            if (SessionID != null && SessionID != "")
            {
                String username = DBFunctions.getUsernameFromSessionID(SessionID);
                await toggleLike(username, id);
            }
            else
            {
                await Clients.Client(Context.ConnectionId).SendAsync("redirectLogin");
            }
        }

        public async Task toggleLike(String user, int id)
        {
            if (UserInteractions.likedPost(user, id) == true)
            {
                DBFunctions.SendQuery("DELETE FROM PostLikes WHERE Username='" + user + "' AND PostID='" + id + "';");
                Shards.removeShards(Communities.getPostOwner(id), 1);
                Shards.removeShards(user, 1);
            }
            else
            {
                Shards.giveShards(user, 1);
                Shards.giveShards(Communities.getPostOwner(id), 1);
                await sendNotification(Communities.getPostOwner(id), user, "LIKED YOUR POST");
                DBFunctions.SendQuery("INSERT INTO PostLikes (Username, PostID) VALUES('" + user + "', '" + id + "');");
            }
            await Clients.All.SendAsync("ContentDelivery", Communities.getPostLikes(id).ToString(), "post-likes-" + id);
        }


        public async Task selectPost(String SessionID, int type)
        {

            String user = DBFunctions.getUsernameFromSessionID(SessionID);

            if (DBFunctions.getUserData(user, "Muted") == "True")
            {
                String muted = "<center><h1 style=\"color:#c2c2c2; margin-top:150px; margin-bottom:500px;\">YOU ARE MUTED AND CANNOT POST</h1></center>";
                await sendContent(Context.ConnectionId, muted, "nav-youtube-post");
                await sendContent(Context.ConnectionId, muted, "nav-text-post");
                await sendContent(Context.ConnectionId, muted, "nav-image-post");
                return;
            }
            String text = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/POSTSCODE/TEXT.html")
                .Replace("{COMMUNITIES}", Communities.getUserCommunitiesNames(user)).Replace("{FLAIRS}", Communities.getCommunitiesFlairs());

            //  String image = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/POSTSCODE/IMAGE.html")
            //      .Replace("{COMMUNITIES}", Communities.getUserCommunitiesNames(user)).Replace("{FLAIRS}", Communities.getCommunitiesFlairs());

            String video = File.ReadAllText(Environment.CurrentDirectory + "/Pages/HtmlTemplates/POSTSCODE/VIDEO.html")
                    .Replace("{COMMUNITIES}", Communities.getUserCommunitiesNames(user)).Replace("{FLAIRS}", Communities.getCommunitiesFlairs());

            await sendContent(Context.ConnectionId, video, "nav-youtube-post");
            await sendContent(Context.ConnectionId, text, "nav-text-post");
            //await sendContent(Context.ConnectionId, image, "nav-image-post");

        }
        public async Task addTextPost(String SessionID, String title, String text, String community, String flair)
        {
            try
            {

                if (title != null && title != "" && text != null && text != "" && UserInteractions.postAllowed() == true
                    && community != null && community != "" && flair != null && flair != "null")
                {

                    if (title.Length > 30)
                    {
                        await showError(Context.ConnectionId, "THE MAX TITLE LENGTH IS 30 CHARACTERS");
                        return;
                    }

                    if (text.Length > 300)
                    {
                        await showError(Context.ConnectionId, "THE MAX POST LENGTH IS 300 CHARACTERS");
                        return;
                    }

                    title = title.Replace("'", "''");
                    text = text.Replace("'", "''");
                    String user = DBFunctions.getUsernameFromSessionID(SessionID);
                    int CommunityID = Communities.getCommunityIDFromName(community);
                    int flairID = Communities.getFlairIDFromName(flair);
                    String date = DateTime.Now.ToString();

                    if (CommunitySettings.mutedFromCommunity(user, CommunityID) == "checked")
                    {
                        await showError(Context.ConnectionId, "YOU ARE MUTED IN THIS COMMUNITY");
                        return;
                    }

                    if (CommunitySettings.bannedFromCommunity(user, CommunityID) == "checked")
                    {
                        await showError(Context.ConnectionId, "YOU ARE BANNED IN THIS COMMUNITY");
                        return;
                    }

                    DBFunctions.SendQuery("INSERT INTO CommunityPosts (Username, CommunityID, Date, FlairID, Title, Text) " +
                        "VALUES('" + user + "', '" + CommunityID + "', '" + date + "', '" + flairID + "', '" + title + "', '" + text + "')");
                    await sendContent(Context.ConnectionId, "<a href=\"/\"><p>SUCCESS See Post ></p></a>", "create-post");
                    await redirect("/");
                    Shards.giveShards(user, 3);
                }
                else
                {
                    await showError(Context.ConnectionId, "PLEASE ENTER ALL FIELDS CORRECTLY.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message Error: " + ex.Message);
            }
        }

        public async Task addImagePost(String SessionID, String title, String path, String community, String flair)
        {
            try
            {
                if (title != null && title != "" && path != null && path != "" && UserInteractions.postAllowed() == true
                    && community != null && community != "" && flair != null && flair != "null")
                {
                    if (title.Length > 30)
                    {
                        await showError(Context.ConnectionId, "THE MAX TITLE LENGTH IS 30 CHARACTERS");
                        return;
                    }

                    title = title.Replace("'", "''");
                    String user = DBFunctions.getUsernameFromSessionID(SessionID);
                    int CommunityID = Communities.getCommunityIDFromName(community);
                    int flairID = Communities.getFlairIDFromName(flair);
                    String date = DateTime.Now.ToString();

                    if (CommunitySettings.mutedFromCommunity(user, CommunityID) == "checked")
                    {
                        await showError(Context.ConnectionId, "YOU ARE MUTED IN THIS COMMUNITY");
                        return;
                    }

                    if (CommunitySettings.bannedFromCommunity(user, CommunityID) == "checked")
                    {
                        await showError(Context.ConnectionId, "YOU ARE BANNED IN THIS COMMUNITY");
                    }

                    DBFunctions.SendQuery("INSERT INTO CommunityPosts (Username, CommunityID, Date, FlairID, Title, Image) " +
                        "VALUES('" + user + "', '" + CommunityID + "', '" + date + "', '" + flairID + "', '" + title + "', '" + path + "')");
                    await sendContent(Context.ConnectionId, "<a href=\"/\"><p>SUCCESS See Post ></p></a>", "create-post");
                    await redirect("/");
                    Shards.giveShards(user, 3);
                }
                else
                {
                    await showError(Context.ConnectionId, "PLEASE ENTER ALL FIELDS CORRECTLY.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message Error: " + ex.Message);
            }
        }

        public async Task addVideoPost(String SessionID, String title, String path, String community, String flair)
        {
            try
            {
                if (title != null && title != "" && path != null && path != "" && UserInteractions.postAllowed() == true
                    && community != null && community != "" && flair != null && flair != "null")
                {

                    if (title.Length > 30)
                    {
                        await showError(Context.ConnectionId, "THE MAX TITLE LENGTH IS 30 CHARACTERS");
                        return;
                    }

                    String user = DBFunctions.getUsernameFromSessionID(SessionID);
                    int CommunityID = Communities.getCommunityIDFromName(community);

                    if (CommunitySettings.mutedFromCommunity(user, CommunityID) == "checked")
                    {
                        await showError(Context.ConnectionId, "YOU ARE MUTED IN THIS COMMUNITY");
                        return;
                    }

                    if (CommunitySettings.bannedFromCommunity(user, CommunityID) == "checked")
                    {
                        await showError(Context.ConnectionId, "YOU ARE BANNED IN THIS COMMUNITY");
                        return;
                    }

                    title = title.Replace("'", "''");

                    int flairID = Communities.getFlairIDFromName(flair);
                    String date = DateTime.Now.ToString();

                    DBFunctions.SendQuery("INSERT INTO CommunityPosts (Username, CommunityID, Date, FlairID, Title, VideoUrl) " +
                        "VALUES('" + user + "', '" + CommunityID + "', '" + date + "', '" + flairID + "', '" + title + "', '" + path + "')");
                    await sendContent(Context.ConnectionId, "<a href=\"/\"><p>SUCCESS See Post ></p></a>", "create-post");
                    await redirect("/");
                    Shards.giveShards(user, 3);
                }
                else
                {
                    await showError(Context.ConnectionId, "PLEASE ENTER ALL FIELDS CORRECTLY");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message Error: " + ex.Message);
            }
        }

        public async Task addComment(String SessionID, String comment, int PostID)
        {

            if (DBFunctions.getUserData(DBFunctions.getUsernameFromSessionID(SessionID), "Muted") == "True")
            {
                await showError(Context.ConnectionId, "YOU ARE MUTED AND CANNOT COMMENT ON POSTS");
                return;
            }
            if (comment != null && comment != "")
            {
                String user = DBFunctions.getUsernameFromSessionID(SessionID);
                Comments.addComment(user, comment, PostID);
                Shards.giveShards(user, 1);
                await sendNotification(Communities.getPostOwner(PostID), user, "COMMENTED ON YOUR POST");
                await Clients.All.SendAsync("ContentDelivery", Comments.getComments(PostID), "comment-list-" + PostID);
                await showAcknowledge(Context.ConnectionId, "COMMENT ADDED");
            }
        }
        public async Task UpdateContent(String userConnectionID, String content, String div)
        {
            await Clients.Client(userConnectionID).SendAsync("UpdateContent", content, div);
            GC.Collect();
        }
        public async Task sendContent(String userConnectionID, String content, String div)
        {
            await Clients.Client(userConnectionID).SendAsync("ContentDelivery", content, div);
            GC.Collect();
           
        }
        public async Task sendValue(String userConnectionID, String content, String div, Boolean type)
        {
                await Clients.Client(userConnectionID).SendAsync("InputDelivery", content, div, type);
                GC.Collect();
        }



        public async Task communityPromoteUser(String SessionID, int CommunityID, String user)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.getCommunityMemberPermissionLevel(username, CommunityID) > 0)
            {
                if (Communities.getCommunityMemberPermissionLevel(user, CommunityID) > 0)
                {
                    DBFunctions.SendQuery("DELETE FROM CommunityModerators WHERE Username='" + user + "' AND CommunityID='" + CommunityID + "';");
                    await showAcknowledge(Context.ConnectionId, user.ToUpper() + " WAS DEMOTED");
                }
                else
                {
                    DBFunctions.SendQuery("INSERT INTO CommunityModerators (Username, CommunityID, PermissionLevel) VALUES('" + user + "', '" + CommunityID + "', '1');");
                    await showAcknowledge(Context.ConnectionId, user.ToUpper() + " WAS PROMOTED TO MODERATOR");
                }
            }
        }

        public async Task deletePost(String SessionID, int PostID, int CommunityID)
        {
            String user = DBFunctions.getUsernameFromSessionID(SessionID);

            if (UserPosts.ownsPost(user, PostID) || Communities.isAdmin(user)
                || Communities.getCommunityMemberPermissionLevel(user, CommunityID) > 0)
            {
                DBFunctions.SendQuery("INSERT INTO DeletedPosts (PostID) VALUES('" + PostID + "');");
                await Clients.Client(Context.ConnectionId).SendAsync("ContentDelivery", "", "post-container-" + PostID);
                await showSuccess(Context.ConnectionId, "POST DELETED");
            }
        }

        public async Task createCommunity(String SessionID, String name, String bio, String type)
        {
            String user = DBFunctions.getUsernameFromSessionID(SessionID);
            if (name != null && name != "" && bio != null && bio != "" && ContentCheck.validString(name, true, 20) && ContentCheck.validString(bio, false, 200))
            {

                try
                {
                    if (Communities.getCommunityIDFromName(name) > 0)
                    {
                        await showError(Context.ConnectionId, "A COMMUNITY WITH THIS NAME ALREADY EXISTS");
                        return;
                    }
                }
                catch
                {
                    await showError(Context.ConnectionId, "A COMMUNITY WITH THIS NAME ALREADY EXISTS");
                    return;
                }

                String hidden = "";
                if (type == "3")
                {
                    hidden = "True";
                }

                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string id = "";
                Random rand = new Random();
                for (int i = 0; i < 8; i++)
                {
                    int r = rand.Next(chars.Length);
                    id += chars.ToCharArray()[r];
                }

                bio = bio.Replace("'", "''");
                Console.WriteLine(2 + name + ", " + bio);
                DBFunctions.SendQuery("INSERT INTO Communities (CommunityName, CommunityBio, AvatarUrl, BannerUrl, Private, Hidden, InviteCode)" +
                    " VALUES('" + name + "','" + bio + "','" + "/images/tempback.png"
                    + "','" + "Banner" + "', 'False', 'False', '" + id + "');");

   //             DBFunctions.SendQuery("INSERT INTO Communities (CommunityName, CommunityBio, AvatarUrl, BannerUrl, Private, Hidden, InviteCode)" +
   // " VALUES('" + name + "','" + bio + "','" + "/images/tempback.png"
   // + "','" + "Banner" + "', 'True', '" + hidden + "', '" + id + "');");

                DBFunctions.SendQuery("INSERT INTO CommunityFollowers (Username, CommunityID) VALUES('"
                    + user + "', '" + Communities.getMostRecentCommunityID() + "');");

                DBFunctions.SendQuery("INSERT INTO CommunityModerators (Username, CommunityID, PermissionLevel) VALUES('"
                    + user + "', '" + Communities.getMostRecentCommunityID() + "', '2');");

                await redirect("/CommunitySettings?id=" + Communities.getMostRecentCommunityID());
            }
            else
            {
                await showError(Context.ConnectionId, "PLEASE ENTER ALL FIELDS CORRECTLY.");
            }
        }

        public async Task redirect(String path)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("redirect", path);
        }

        public async Task communityBanUser(String SessionID, int CommunityID, String user)
        {
            CommunitySettings.communityBanUser(SessionID, CommunityID, user);

            if (CommunitySettings.bannedFromCommunity(user, CommunityID) == "checked")
            {
                await sendNotification(user, DBFunctions.getCommunityInfo(CommunityID, "CommunityName"), "BANNED YOU FROM THIER COMMUNITY");
                await showAcknowledge(Context.ConnectionId, user.ToUpper() + " WAS BANNED");
            }
            else
            {
                await sendNotification(user, DBFunctions.getCommunityInfo(CommunityID, "CommunityName"), "PARDONED YOU FROM THIER COMMUNITY");
                await showAcknowledge(Context.ConnectionId, user.ToUpper() + " WAS PARDONED");
            }
        }

        public async Task communityMuteUser(String SessionID, int CommunityID, String user)
        {
            CommunitySettings.communityMuteUser(SessionID, CommunityID, user);

            if (CommunitySettings.mutedFromCommunity(user, CommunityID) == "Checked")
            {
                await showAcknowledge(Context.ConnectionId, user.ToUpper() + " WAS MUTED");
            }
            else
            {
                await showAcknowledge(Context.ConnectionId, user.ToUpper() + " WAS UNMUTED");
            }
        }

        public async Task sendNotification(String Username, String userWhoTriggered, String Message)
        {
            if (userWhoTriggered != Username)
            {
                String ConnectionID = DBFunctions.getConnectionID(Username);
                String newMessage = Notifications.formatNotification(Username, Message, userWhoTriggered, false);
                DBFunctions.SendQuery("INSERT INTO Notifications (Username, Message, Sender, Seen) VALUES('" + Username + "', '" + Message + "', '" + userWhoTriggered + "', 'False');");
                await Clients.Client(ConnectionID).SendAsync("SendNotification", newMessage);
            }
        }


        // Profile Settings
        public async Task sendContentSettings(String userConnectionID, String content, String ID)
        {
            await Clients.Client(userConnectionID).SendAsync("ValueDelivery", content, ID);
            GC.Collect();
        }

        public async Task ProfileInfo(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            String DisplayName = DBFunctions.getUserData(username, "DisplayName");
            String Email = DBFunctions.getUserData(username, "Email");
            String Gender = DBFunctions.getUserData(username, "Gender");
            String AccountType = DBFunctions.getUserData(username, "Admin");
            String Bio = DBFunctions.getUserData(username, "Bio");
            String FavCommunity = DBFunctions.getUserData(username, "FavGames");
            String genderValue = "0";

            if (string.IsNullOrEmpty(AccountType))
            {
                AccountType = "STANDARD";
            }
            else
            {
                AccountType = "ADMIN";
            }

            if (string.IsNullOrEmpty(Gender))
            {
                Gender = "Please select gender";
            }

            if(Gender == "PRIVATE")
            {
                genderValue = "0";
            }
            if (Gender == "MALE")
            {
                genderValue = "1";
            }
            if (Gender == "FEMALE")
            {
                genderValue = "2";
            }
            if (Gender == "OTHER")
            {
                genderValue = "3";
            }

            await sendContent(Context.ConnectionId, username, "settings-text-1");
            await sendContent(Context.ConnectionId, DisplayName, "settings-text-2");
            await sendContent(Context.ConnectionId, Email, "settings-text-4");
            // await sendContent(Context.ConnectionId, genderValue, "genderSelect");
            await sendContent(Context.ConnectionId, AccountType, "AccountType");
            await sendContent(Context.ConnectionId, Bio, "settings-text-0");
            // true = input field, flase = droupdown
            await sendValue(Context.ConnectionId, FavCommunity, "fav-community", true);
            await sendValue(Context.ConnectionId, genderValue, "genderSelect", false);




        }

        public async Task CreateCommunityList(String SessionID)
        {
            String communityList = Communities.getAllCommunities();
            await Clients.Client(Context.ConnectionId).SendAsync("CommunityDelivery", communityList);
            //GC.Collect();
        }

        public async Task UpdateProfile(String SessionID, String Email, String ColourMode, String Bio, String FavCommunity, String Gender)
        {
            try { 
           
            if (!string.IsNullOrEmpty(Email))
            {
                DBFunctions.SendQuery("UPDATE Users SET Email = '" + Email + "' WHERE SessionID = '" + SessionID + "'");
            }
            if (!string.IsNullOrEmpty(Bio))
            {
                DBFunctions.SendQuery("UPDATE Users SET Bio = '" + Bio + "' WHERE SessionID = '" + SessionID + "'");
            }
            if (!string.IsNullOrEmpty(FavCommunity))
            {
                DBFunctions.SendQuery("UPDATE Users SET FavGames = '" + FavCommunity + "' WHERE SessionID = '" + SessionID + "'");
            }
            if (!string.IsNullOrEmpty(Gender))
            {
                DBFunctions.SendQuery("UPDATE Users SET Gender = '" + Gender + "' WHERE SessionID = '" + SessionID + "'");
            }

                DBFunctions.SendQuery("UPDATE Users SET ColourMode = '" + ColourMode + "' WHERE SessionID = '" + SessionID + "'");

            await showSuccess(Context.ConnectionId, "PROFILE UPDATED");
            }
            catch { await showError(Context.ConnectionId, "FAIL UPDATING THE PROFILE"); }

        }

        // Profile Settings

        // Admin Panel

        public void UpdateUserVerification(String SessionID, String UserUsername)
        {
            AdminPannel.UpdateUserVerify(SessionID, UserUsername);
        }

        public void UpdateUserMute(String SessionID, String UserUsername)
        {
            AdminPannel.UpdateUserMute(SessionID, UserUsername);
        }

        public void UpdateUserBan(String SessionID, String UserUsername)
        {
            AdminPannel.UpdateUserBan(SessionID, UserUsername);
        }

        // Admin Panel
        public async Task ProcessPurchase(String SessionID, int ProductID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (!Store.ppOwned(username, ProductID) && Store.getPrice(ProductID) > 0)
            {
                if (Shards.getShardsAmount(username) >= Store.getPrice(ProductID))
                {
                    DBFunctions.SendQuery("INSERT INTO ProfilePicOwnership (ProductID, Username) VALUES('" + ProductID + "', '" + username + "');");
                    Shards.removeShards(username, Store.getPrice(ProductID));
                    await Clients.Client(Context.ConnectionId).SendAsync("RemoveContainer", "store-item-" + ProductID);
                    await Clients.Client(Context.ConnectionId).SendAsync("RemoveContainer", "equipped-2");
                    await Clients.Client(Context.ConnectionId).SendAsync("AppendFront", Store.getProductHTML(ProductID, true), "store-owned");
                    await Clients.Client(Context.ConnectionId).SendAsync("AppendFront", Store.getEquippedHTML(username).Replace("{OC}", "2"), "store-owned");
                    DBFunctions.setProfilePicture(username, ProductID);
                    await Clients.Client(Context.ConnectionId).SendAsync("updateProfilePicture", DBFunctions.getProfilePicture(username));
                    await showSuccess(Context.ConnectionId, "AVATAR PURCHASED & EQUIPPED");
                }
                else
                {
                    await showError(Context.ConnectionId, "INSUFFICIENT SHARDS");
                }
            }
            else
            {
                DBFunctions.setProfilePicture(username, ProductID);
                await showSuccess(Context.ConnectionId, "PROFILE PICTURE EQUIPED");
                await Clients.Client(Context.ConnectionId).SendAsync("updateProfilePicture", DBFunctions.getProfilePicture(username));
            }
        }

        public async Task updateShardCounters(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            await sendContent(Context.ConnectionId, Shards.getShardsAmount(username).ToString(), "shard-counter-1");
            await sendContent(Context.ConnectionId, Shards.getShardsAmount(username).ToString(), "shard-counter-2");
        }

        public async Task showError(String ConnectionID, String message)
        {
            await Clients.Client(ConnectionID).SendAsync("ShowError", message);
        }

        public async Task showSuccess(String ConnectionID, String message)
        {
            await Clients.Client(ConnectionID).SendAsync("ShowSuccess", message);
        }

        public async Task showAcknowledge(String ConnectionID, String message)
        {
            await Clients.Client(ConnectionID).SendAsync("ShowAcknowledge", message);
        }

        public async Task acceptUser(String SessionID, String user, int CommunityID)
        {
            String executor = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.getCommunityMemberPermissionLevel(executor, CommunityID) > 0)
            {
                if (Communities.hasRequested(user, CommunityID))
                {
                    DBFunctions.SendQuery("DELETE FROM CommunityRequests WHERE Username='" + user + "';");
                    DBFunctions.SendQuery("INSERT INTO CommunityFollowers (Username, CommunityID) VALUES('" + user + "', '" + CommunityID + "')");
                    await sendNotification(user, DBFunctions.getCommunityInfo(CommunityID, "CommunityName").ToUpper(), "ACCEPTED YOUR JOIN REQUEST");
                }
            }
        }

        public void leaveCommunity(String SessionID, int CommunityID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            DBFunctions.SendQuery("DELETE FROM CommunityFollowers WHERE Username='" + username + "' AND CommunityID='" + CommunityID + "';");
        }

        public async Task clearNotifications(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            await sendContent(Context.ConnectionId, Notifications.getNotifications(username), "notifications");
            Notifications.clearNotifications(username);
        }

        public async Task copyInvite(String SessionID, int CommunityID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.communityHidden(CommunityID) && UserInteractions.subscribed(username, CommunityID))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("copyInvite", Communities.getCommunityInviteCode(CommunityID));
                await showSuccess(Context.ConnectionId, "INVITATION LINK COPIED");
            }
        }

        public async Task updateCommunity(String SessionID, int CommunityID, String visible, String bio)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.getCommunityMemberPermissionLevel(username, CommunityID) > 0)
            {
                bio = bio.Replace("'", "''");

                if (bio.Length > 200)
                {
                    bio = bio.Substring(0, 200);
                }

                if (bio == null || bio == "")
                {
                    await showError(Context.ConnectionId, "PLEASE ENTER A BIO");
                    return;
                }

                String visibility = "False";
                if (visible == "1")
                {
                    visibility = "True";
                }

                DBFunctions.SendQuery("UPDATE Communities SET CommunityBio='" + bio + "', Hidden='" + visibility + "' WHERE CommunityID='" + CommunityID + "';");
                await showSuccess(Context.ConnectionId, "YOUR CHANGES HAVE BEEN SAVED");
            }
        }

        public void deleteCommunity(String SessionID, int CommunityID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.getCommunityMemberPermissionLevel(username, CommunityID) > 0)
            {
                DBFunctions.SendQuery("DELETE FROM Communities WHERE CommunityID='" + CommunityID + "';");
                DBFunctions.SendQuery("DELETE FROM CommunityModerators WHERE CommunityID='" + CommunityID + "';");
                DBFunctions.SendQuery("DELETE FROM CommunityPosts WHERE CommunityID='" + CommunityID + "';");
                DBFunctions.SendQuery("DELETE FROM CommunityMutes WHERE CommunityID='" + CommunityID + "';");
                DBFunctions.SendQuery("DELETE FROM CommunityBans WHERE CommunityID='" + CommunityID + "';");
                DBFunctions.SendQuery("DELETE FROM CommunityRequests WHERE CommunityID='" + CommunityID + "';");
                DBFunctions.SendQuery("DELETE FROM CommunityFollowers WHERE CommunityID='" + CommunityID + "';");
            }
        }

        public async Task reportCommunity(String SessionID, int ID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            String link = "/Community?id=" + ID;
            DBFunctions.SendQuery("INSERT INTO Reports (Link, Username) VALUES('" + link + "', '" + username + "');");
            await showSuccess(Context.ConnectionId, "THANK YOU FOR YOUR REPORT");
        }

        public async Task reportUser(String SessionID, String ID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            String link = "/Profile?id=" + ID;
            DBFunctions.SendQuery("INSERT INTO Reports (Link, Username) VALUES('" + link + "', '" + username + "');");
            await showSuccess(Context.ConnectionId, "THANK YOU FOR YOUR REPORT");
        }

        public async Task reportPost(String SessionID, int ID)
        {
            String username = "GUEST";
            if (SessionID != null && SessionID == "")
            {
                username = DBFunctions.getUsernameFromSessionID(SessionID);
            }
            String link = "/Post?id=" + ID;
            DBFunctions.SendQuery("INSERT INTO Reports (Link, Username) VALUES('" + link + "', '" + username + "');");
            await showSuccess(Context.ConnectionId, "THANK YOU FOR YOUR REPORT");
        }

        public async Task deleteReport(String SessionID, int ReportID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.isAdmin(username))
            {
                DBFunctions.SendQuery("DELETE FROM Reports WHERE ReportID='" + ReportID + "';");
                await showAcknowledge(Context.ConnectionId, "REPORT MARKED AS RESOLVED");
                await sendContent(Context.ConnectionId, "", "reports-row-" + ReportID);
            }

        }

        public async Task deleteAppeal(String SessionID, int ReportID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.isAdmin(username))
            {
                DBFunctions.SendQuery("DELETE FROM UserAppeals WHERE AppealID='" + ReportID + "';");
                await showAcknowledge(Context.ConnectionId, "THE APPEAL WAS DENIED");
                await sendContent(Context.ConnectionId, "", "appeals-row-" + ReportID);
            }

        }

        public async Task unbanUser(String SessionID, String user, int RowID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.isAdmin(username))
            {
                DBFunctions.SendQuery("UPDATE Users SET Banned='False' WHERE Username='" + user + "';");
                DBFunctions.SendQuery("DELETE FROM UserAppeals WHERE AppealID='" + RowID + "';");
                await showAcknowledge(Context.ConnectionId, "THE USER WAS UNBANNED");
                await sendContent(Context.ConnectionId, "", "appeals-row-" + RowID);
            }

        }

        public async Task loginUser(String username, String password, Boolean redirect)
        {

            String SessionID = LoginCreateAccount.loginSuccessful(username, password);
            if (SessionID != null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("setSessionID", SessionID, redirect);
                return;
            }
            await showError(Context.ConnectionId, "USERNAME OR PASSWORD IS INCORRECT");
        }

        public async Task ResetUser(String RecoveryCode, String NewPass, String PassRepeat)
        {
            String error = LoginCreateAccount.ResetSeccessful(RecoveryCode, NewPass, PassRepeat);

            if (error == "Done")
            {
                await showSuccess(Context.ConnectionId, "PASSWORD CHANGED");
                return;
            }
            await showError(Context.ConnectionId, error);
        }

        public async Task Createuseraccount(String username, String Displayname, String Password, String Email, String ConfirmPassword, bool AgeCheck)
        {
            if (AgeCheck == true)
            { 
                 String error = LoginCreateAccount.CerateSuccessful(username, Displayname, Password, Email, ConfirmPassword);

                 if (error.Contains("RECOVERY"))
                 {
                     await loginUser(username, Password, false);
                     await Clients.Client(Context.ConnectionId).SendAsync("ProgressCreateAccount", error);
                     return;
                 }
                 await showError(Context.ConnectionId, error);
            }
            else
            {
                await showError(Context.ConnectionId, "PLEASE ACCEPT TERMS AND CONDITIONS");
            }
        }
        public async Task ignoreUser()
        {
            await showError(Context.ConnectionId, "HOLD TIGHT, THIS FEATURE ISN'T QUITE READY YET");
        }

        public async Task sharePost(int id)
        {
            await Clients.Client(Context.ConnectionId).SendAsync("copyPost", id);
            await showSuccess(Context.ConnectionId, "POST LINK COPIED");
        }

        public async Task toggleTheme(String SessionID)
        {
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            if(DBFunctions.getUserColorMode(username) == "lightMode")
            {
                DBFunctions.SendQuery("UPDATE Users SET ColourMode='False' WHERE Username='" + username + "';");
            }
            else
            {
                DBFunctions.SendQuery("UPDATE Users SET ColourMode='True' WHERE Username='" + username + "';");
            }
            await showSuccess(Context.ConnectionId, "COLOUR MODE CHANGED");
        }

        public async void requestPosts(String SessionID, int CommunityID, int cycle)
        {
            //null username community cycle
            String username = DBFunctions.getUsernameFromSessionID(SessionID);
            String html = Communities.getPosts(null, username, 0, cycle);
            await UpdateContent(Context.ConnectionId, html, "activity-feed-container");
        }
    }
}

