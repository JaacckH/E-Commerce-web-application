using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FINAL.Pages.Classes
{
    public static class FileUpload
    {

        public static void uploadCommunityPP(String SessionID, String path, int CommunityID)
        {
            String user = DBFunctions.getUsernameFromSessionID(SessionID);
            if (Communities.getCommunityMemberPermissionLevel(user, CommunityID) > 0)
            {
                DBFunctions.SendQuery("UPDATE Communities SET AvatarUrl='" + path + "' WHERE CommunityID='" + CommunityID + "';");
            }
        }

    }
}