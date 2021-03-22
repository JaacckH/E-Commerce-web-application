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
            String name = HttpContext.Request.Form["productname"];
            String description = HttpContext.Request.Form["productdescription"];
            String price = HttpContext.Request.Form["productprice"];
            String quantity = HttpContext.Request.Form["productquantity"];

            if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(description) || String.IsNullOrEmpty(price) || String.IsNullOrEmpty(quantity))
            {
                TempData["add-product-error"] = "Enter All Fields";
                return null;
            }

            String path = (ProductFunctions.getNewestID() + 1) + ".png";

            DBFunctions.sendQuery("INSERT INTO Products (Name, Description, Price, Quantity, ImagePath) " +
                "VALUES('" + name + "', '" + description + "', '" + price + "', '" + quantity + "', '../ProductImages/" + path + "')");

            var FileToUpload = Path.Combine(_env.WebRootPath, Path2, path);
            using (var Fstream = new FileStream(FileToUpload, FileMode.Create))
            {
                ImgFile.CopyTo(Fstream);
            }

            TempData["add-product-error"] = "Product Added Successfully";
            return null;
        }
    }
}
