using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace FINAL.Pages.Classes
{
    public class UserInteractions
    {

        public static String getUsernameFromSessionID(String SessionID)
        {
            return DBFunctions.getUsernameFromSessionID(SessionID);
        }

        public static Boolean isFollowing(String user1, String user2)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Following;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["User1"].ToString() == user1 && reader["User2"].ToString() == user2)
                {
                    conn.Close();
                    return true;
                }
            }
            conn.Close();
            return false;
        }

        public static Boolean profilePrivate(String username)
        {
            if (DBFunctions.getUserData(username, "Private") == "1")
            {
                return true;
            }
            return false;
        }

        public static void followToggle(String user1, String user2)
        {
            if (isFollowing(user1, user2))
            {
                DBFunctions.SendQuery("DELETE FROM Following WHERE User1='" + user1 + "' AND User2='" + user2 + "';");
            }
            else
            {
                DBFunctions.SendQuery("INSERT INTO Following (User1, User2) VALUES('" + user1 + "', '" + user2 + "');");
            }
        }

        public static Boolean friends(String user1, String user2)
        {
            if (isFollowing(user1, user2) && isFollowing(user2, user1))
            {
                return true;
            }
            return false;
        }

        public static Boolean likedPost(String user, int PostID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM PostLikes;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == user && reader["PostID"].ToString() == PostID.ToString())
                {
                    conn.Close();
                    return true;
                }
            }
            conn.Close();
            return false;
        }

        public static Boolean subscribed(String user, int CommunityID)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM CommunityFollowers;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if (reader["Username"].ToString() == user && reader["CommunityID"].ToString() == CommunityID.ToString())
                {
                    conn.Close();
                    return true;
                }
            }
            conn.Close();
            return false;
        }

        public static void ToggleSubscribe(String user, int CommunityID)
        {
            if (subscribed(user, CommunityID) == true)
            {
                DBFunctions.SendQuery("DELETE FROM CommunityFollowers WHERE Username='" + user + "' AND CommunityID='" + CommunityID + "';");
            }
            else
            {
                DBFunctions.SendQuery("INSERT INTO CommunityFollowers (Username, CommunityID) VALUES('" + user + "', '" + CommunityID + "');");
            }
        }

        public static Boolean postAllowed()
        {
            return true;
        }



    }
}
