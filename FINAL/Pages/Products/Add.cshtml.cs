using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using FINAL.Models;

namespace FINAL.Pages.Products
{
    public class AddModel : PageModel
    {
        [BindProperty]
        public IFormFile ImgFile { get; set; }

        public readonly IWebHostEnvironment _env;

        public AddModel(IWebHostEnvironment env)
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
            const string Path2 = "ProductImages";
            var FileToUpload = Path.Combine(_env.WebRootPath, Path2, ImgFile.FileName);

            // upload the image on the filestream
            using (var Fstream = new FileStream(FileToUpload, FileMode.Create))
            {
                ImgFile.CopyTo(Fstream);
            }


            // redirects to the image page for the image uploaded passing through the image ID
            return RedirectToPage("/Products/Index", new { imgid = uploadID });
        }

    }
}
