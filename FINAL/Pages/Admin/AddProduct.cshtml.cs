using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FINAL.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace Group_Project.Models
{
    public class AddProductModel : PageModel
    {
        [BindProperty]
        public IFormFile ImgFile { get; set; }

        public readonly IWebHostEnvironment _env;

        public AddProductModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public Int32 uploadID;
        public IActionResult OnGet()
        {
            // user session check
            return Page();
        }

        public IActionResult OnPost()
        {
            //create the file path to upload the image to
            const String Path2 = "ProductImages";
            String name = HttpContext.Request.Form["input-name"];
            String description = HttpContext.Request.Form["input-description"];
            String price = HttpContext.Request.Form["input-price"];
            String tags = HttpContext.Request.Form["input-tags"];
            String material = HttpContext.Request.Form["material"];
            String category = "Womens";

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(description) || String.IsNullOrEmpty(price) || String.IsNullOrEmpty(material))
            {
                TempData["add-product-error"] = "Enter All Fields";
                return null;
            }

            String id = UserFunctions.generateSessionID();
            String path = id + ".png";

            for (int i = 0; i < 20; i++)
            {
                try
                {
                    String size = HttpContext.Request.Form["sizecol-" + i + "-size"];
                    String quantity = HttpContext.Request.Form["input-quantity-" + i];

                    if (!String.IsNullOrEmpty(size))
                    {
                        DBFunctions.sendQuery("INSERT INTO Stock (ProductID, SizeID, Quantity) VALUES('" + id + "','" + size + "','" + quantity + "');");
                    }
                }
                catch { }
            }

            DBFunctions.sendQuery("INSERT INTO Products (ProductID, Name, Description, Price, ImagePath, Tags, Materials, Category) " +
                "VALUES('" + id + "', '" + name + "', '" + description + "', '" + price + "', '../ProductImages/" + path + "', '" + tags + "', '" + material + "', '" + category + "')");


            var FileToUpload = Path.Combine(_env.WebRootPath, Path2, path);
            using (var Fstream = new FileStream(FileToUpload, FileMode.Create))
            {
                ImgFile.CopyTo(Fstream);
            }

            TempData["add-product-error"] = "Product Added Successfully";
            Response.Redirect("/Admin/Products");
            return null;
        }



    }
}
