using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Handles displaying a ramen product and submitting a new user rating for it.
    /// </summary>
    public class AddRatingModel : PageModel
    {
        // The path to the JSON file where ramen product data is stored.
        public const string JsonFileName = "wwwroot/data/ramen.json";

        // The rating submitted by the user via form.
        [BindProperty]
        public int Rating { get; set; }

        [BindProperty]
        public string Feedback { get; set; }

        // Service for loading and manipulating product data
        public JsonFileProductService ProductService { get; }

        // The product currently being displayed or updated
        public ProductModel Product;

        // Constructor for injection of the product service
        public AddRatingModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // Handles GET requests to display the selected product before adding rating
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

        // Handles POST requests to add a new rating to the product
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // convert number of product to int
            int number = Convert.ToInt32(Request.RouteValues["number"]);

            Product = ProductService.GetProducts().FirstOrDefault(p => p.Number == number);

            if (Product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToPage("../Error");
            }
            // variable to store updated ramen with new rating to be added
            var updatedProduct = AddRatingToRamen();

            if (updatedProduct != null)
            {
                SaveData(updatedProduct);
            }

            return RedirectToPage("/Index");
        }

        // Adds the submitted rating to the current product and returns an updated product 
        public ProductModel AddRatingToRamen()
        {
            // stores the original product
            var original = Product;

            if ((original == null) == false)
            {
                var updatedRatings = original.Ratings.ToList(); // convert the Ratings to a list
                updatedRatings.Add(Rating); // add the new rating

                // return the updated product with new rating
                var updatedProduct = new ProductModel
                {
                    Number = original.Number,
                    Brand = original.Brand,
                    img = original.img,
                    Style = original.Style,
                    Country = original.Country,
                    Variety = original.Variety,
                    Ratings = updatedRatings.ToArray()
                };

                if (string.IsNullOrWhiteSpace(Feedback) == false)
                {
                    List<string> updatedFeedback;

                    if ((original.Feedback == null) == false)
                    {
                        updatedFeedback = original.Feedback.ToList();
                    }
                    else
                    {
                        updatedFeedback = new List<string>();
                    }
                    updatedFeedback.Add(Feedback);
                    updatedProduct.Feedback = updatedFeedback.ToArray();
                }

                return updatedProduct;
            }
            else
            {
                return null;
            }
        }

        // Replaces the original product in the product list and saves all data back to the JSON 
        public void SaveData(ProductModel updateProduct)
        {
            var products = ProductService.GetProducts().ToList(); // get the products in a list

            var index = products.FindIndex(p => p.Number == updateProduct.Number); // get the number of the product

            if ((index == -1) == false)
            {
                products[index] = updateProduct;
            }
            // get the json to store and write the new json information back to it with the new product
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
