using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Group_Project.Models
{
    public class ProductModel : PageModel
    {
        public String getProduct()
        {
            String[] IDarray = HttpContext.Request.Query["id"].ToString().Split('>');
            String id = IDarray[0];

            if (string.IsNullOrEmpty(id))
            {
                Response.Redirect("/");
                return null;
            }

            return ProductFunctions.getMainProductHtml(id);
        }
    }
}
