
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public class ContentModeration
    {
        public static Boolean canSeePost(String username, int PostID)
        {
            int community = getCommunityFromPost(PostID);

            if(postDeleted(PostID) == true)
            {
                return false;
            }

            if(username == null && Communities.communityHidden(community))
            {
                return false;
            }

            if (CommunitySettings.bannedFromCommunity(username, getCommunityFromPost(PostID)) == "checked")
            {
                return false;
            }

            if (community == 0)
            {
                if((Communities.getPostOwner(PostID) == username) || UserInteractions.isFollowing(username, Communities.getPostOwner(PostID)))
                {
                    return true;
                }
            }
            else
            {
                if (username == null || username == "")
                {
                    if (communityPrivate(community) == false)
                    {
                        return true;
                    }
                }

                if (userInCommunity(username, community) == true)
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean postDeleted(int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM DeletedPosts;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if(reader["PostID"].ToString() == PostID.ToString())
                {
                    conn.Close();
                    return true;
                }
            }

            conn.Close();
            return false;
        }

        public static Boolean canSeeCommunity(String username, int CommunityID)
        {
            if(CommunitySettings.bannedFromCommunity(username, CommunityID) == "checked")
            {
                return false;
            }

            if(userInCommunity(username, CommunityID) == false && Communities.communityHidden(CommunityID))
            {
                return false;
            }

            if (userInCommunity(username, CommunityID) == true || communityPrivate(CommunityID) == false)
            {
                return true;
            }
            return false;
        }

        public static Boolean userInCommunity(String username, int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFollowers;";
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

        public static int getCommunityFromPost(int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT CommunityID, PostID FROM CommunityPosts;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["PostID"].ToString() == PostID.ToString())
                {
                    int result = int.Parse(reader["CommunityID"].ToString());
                    conn.Close();
                    return result;
                }
            }
            conn.Close();
            return 0;
        }

    }
}
