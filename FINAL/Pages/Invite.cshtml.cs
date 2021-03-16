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
    public class Invite : PageModel
    {
        public void onLoad()
        {
            String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
            String code = IDarray[0];

            String username = DBFunctions.getUsernameFromSessionID(HttpContext.Request.Cookies["SessionID"]);

            if(code == null || username == null || code == "" || username == "")
            {
                Response.Redirect("/");
                return;
            }

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = DBFunctions.connectionString;
            conn.Open();
            SqlCommand query = conn.CreateCommand();
            query.CommandText = "SELECT * FROM Communities;";
            SqlDataReader reader = query.ExecuteReader();

            int CommunityID = 0;
            while (reader.Read())
            {
                if(reader["InviteCode"].ToString() == code)
                {
                    CommunityID = int.Parse(reader["CommunityID"].ToString());
                }
            }
            conn.Close();

            if (UserInteractions.subscribed(username, CommunityID))
            {
                Response.Redirect("/Community?id=" + CommunityID);
                return;
            }

            DBFunctions.SendQuery("INSERT INTO CommunityFollowers (Username, CommunityID) VALUES('" + username + "', '" + CommunityID + "');");
            Response.Redirect("/Community?id=" + CommunityID);
        }
    }
}
