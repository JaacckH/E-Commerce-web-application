using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class Notifications
    {
        public static String formatNotification(String user, String message, String trigger, Boolean seen)
        {
            String newMessage = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/NotificationMessages/SEEN.html");

            if (seen == false)
            {
                newMessage = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/Pages/NotificationMessages/UNSEEN.html");
            }

            newMessage = newMessage.Replace("{USER}", trigger.ToUpper() + " ").Replace("{MESSAGE}", message);
            return newMessage;
        }

        public static String getNotifications(String user)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT TOP 10 * FROM Notifications ORDER BY ID DESC;";
            SqlDataReader reader = query.ExecuteReader();

            String html = "";
            while (reader.Read())
            {
                if (user == reader["Username"].ToString())
                {
                    String message = reader["Message"].ToString();
                    if (reader["Sender"].ToString().Length + message.Length > 30)
                    {
                        message = message.Substring(0, 30 - reader["Sender"].ToString().Length) + "...";
                    }

                    Boolean seen = true;
                    if (reader["Seen"].ToString() != "True")
                    {
                        seen = false;
                    }

                    html += Notifications.formatNotification(reader["Username"].ToString(), message, reader["Sender"].ToString(), seen);
                }
            }

            if(html == "")
            {
                html= "<h4 style=\"margin: 0px; margin-left: 10px; margin-top:4px; font-size:14pt; color:#c2c2c2;\">NOTHING TO SEE HERE</h4>";
            }

            conn.Close();
            return html;
        }

        public static void clearNotifications(String username)
        {
            DBFunctions.SendQuery("UPDATE Notifications SET Seen='True' WHERE Username='" + username + "';");
        }

        public static String notificationClass(String Username)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT TOP 10 * FROM Notifications ORDER BY ID DESC;";
            SqlDataReader reader = query.ExecuteReader();

            while (reader.Read())
            {
                if(reader["Username"].ToString() == Username && reader["Seen"].ToString() != "True")
                {
                    String result = "NOTIFICATION";
                    conn.Close();
                    return result;
                }
            }

            return "";
        }
    }
}
