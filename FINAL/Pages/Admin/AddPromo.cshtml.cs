using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FINAL.Pages.Admin
{
    public class AddPromoModel : PageModel
    {
        public IActionResult addPromo()
        {
            try
            {
                String code = Request.Form["input-name"];
                String percentage = Request.Form["input-percentage"];
                String[] start = Request.Form["input-start"].ToString().Split(' ');
                String duration = Request.Form["input-duration"];

                if (!String.IsNullOrEmpty(code) && !String.IsNullOrEmpty(percentage)
                    && !String.IsNullOrEmpty(start[0]) && !String.IsNullOrEmpty(duration)
                    && !Promotions.codeExists(code))
                {
                    Console.WriteLine();
                    int startDate = DateTime.Parse(start[0], new CultureInfo("en-US", true)).DayOfYear; // DateTime.Parse(start[0].ToString(), "MM/DD/YYYY");
                    int endDate = startDate + int.Parse(duration);

                    DBFunctions.sendQuery("INSERT INTO Promotions (PromoCode, Percentage, StartDate, EndDate) " +
                        "VALUES('" + code + "', '" + percentage + "', '" + startDate + "', '" + endDate + "');");

                    Response.Redirect("/Admin/Promotions");
                }
            }

            catch(Exception ex) { Console.WriteLine(ex.Message); }
            return null;
        }

    }
}
