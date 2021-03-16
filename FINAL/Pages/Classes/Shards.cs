using FINAL.Pages.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class Shards
    {
        public static int getShardsAmount(String username)
        {
            try
            {
                if (username != null)
                {
                    return int.Parse(DBFunctions.getUserData(username.ToString(), "Shards"));
                }
            }
            catch { }
            return 0;
        }

        public static void giveShards(String username, int amount)
        {
            int newAmount = amount + getShardsAmount(username);
            DBFunctions.SendQuery("UPDATE Users SET Shards='" + newAmount + "' WHERE Username='" + username + "';");
        }

        public static void removeShards(String username, int amount)
        {
            int newAmount = getShardsAmount(username) - amount;
            DBFunctions.SendQuery("UPDATE Users SET Shards='" + newAmount + "' WHERE Username='" + username + "';");
        }
    }
}
