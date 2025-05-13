using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using static System.Net.Mime.MediaTypeNames;

namespace RamenRatings.WebSite.Pages.Product
{
    public class AddRatingModel : PageModel
    {
        public const string JsonFileName = "wwwroot/data/ramen.json";

        [BindProperty]
        public int Rating { get; set; }
        public JsonFileProductService ProductService { get; }

        public ProductModel Product;
        public AddRatingModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }
        public IActionResult OnGet(int number)
        {
            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }
            return Page();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int number = Convert.ToInt32(Request.RouteValues["number"]);

            Product = ProductService.GetProducts().FirstOrDefault(p => p.Number == number);

            if (Product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToPage("../Error");
            }

            var updatedProduct = AddRatingToRamen();

            if (updatedProduct != null)
            {
                SaveData(updatedProduct);
            }

            return RedirectToPage("/Index");
        }

        public ProductModel AddRatingToRamen()
        {
            var original = Product;

            if ((original == null) == false)
            {
                var updatedRatings = original.Ratings.ToList();
                updatedRatings.Add(Rating);

                return new ProductModel
                {
                    Number = original.Number,
                    Brand = original.Brand,
                    img = original.img,
                    Style = original.Style,
                    Country = original.Country,
                    Variety = original.Variety,
                    Ratings = updatedRatings.ToArray()
                };
            }
            else
            {
                return null;
            }

           
        }

        public void SaveData(ProductModel updateProduct)
        {
            var products = ProductService.GetProducts().ToList();

            var index = products.FindIndex(p => p.Number == updateProduct.Number);

            if ((index == -1) == false)
            {
                products[index] = updateProduct;
            }
            
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
