using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FINAL.Pages
{
    public class UserSettingsModel : PageModel
    {
        public String FirstName, Surname, Email, AddressLine1, AddressLine2, Postcode, PhoneNumber, CardNumber, Expiry;
        public void OnGet()
        {
            String UserID = Classes.UserFunctions.getUserID(HttpContext.Request.Cookies["SessionID"]);
             FirstName = Classes.UserFunctions.getUserDetails(UserID, "Forename");
             Surname = Classes.UserFunctions.getUserDetails(UserID, "Surname");
             Email = Classes.UserFunctions.getUserDetails(UserID, "Email");
             AddressLine1 = Classes.UserFunctions.getUserDetails(UserID, "AddressLine1");
             AddressLine2 = Classes.UserFunctions.getUserDetails(UserID, "AddressLine2");
             Postcode = Classes.UserFunctions.getUserDetails(UserID, "Postcode");
             PhoneNumber = Classes.UserFunctions.getUserDetails(UserID, "PhoneNumber");

             CardNumber = Classes.UserFunctions.getUserDetails(UserID, "CardNumber");
             Expiry = Classes.UserFunctions.getUserDetails(UserID, "Expiry");



        }


    }
}
