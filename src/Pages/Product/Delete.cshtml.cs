using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Pages.Product
{
    public class DeleteModel : PageModel
    {
        public const string JsonFileName = "wwwroot/data/ramen.json"; // path to ramen json

        // new product property
        [BindProperty]
        public ProductModel NewProduct { get; set; }

        // image property
        [BindProperty]
        public IFormFile Image { get; set; }

        // new brand 
        [BindProperty]
        public string NewBrand { get; set; }

        // new style
        [BindProperty]
        public string NewStyle { get; set; }

        // new rating
        [BindProperty]
        public int Rating { get; set; }

        // List of existing brands and styles for the dropdown
        public List<string> ExistingBrands { get; set; }
        public List<string> ExistingStyles { get; set; }

        public Model(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }

        public ProductModel Product;

        public ProductModel DeleteData(string id)
        {
            var dataSet = ProductService.GetProducts();
            var data = dataSet.FirstOrDefault(m => m.Id.Equals(id));

            var newDataSet = ProductService.GetProducts().Where((m => m.Id.Equals(id) == false);

            SaveData(newDataSet);

            return data;

        }

        public IActionResult OnPost(int number)
        {
            if (!ModelState.IsValid)
            {
                return Page();

            }
            ProductService.DeleteData(Product.Id);

            return RedirectToPage("./Index");

        }

            // Get all of the products
 




            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }

            return Page();
        }
    }
}
