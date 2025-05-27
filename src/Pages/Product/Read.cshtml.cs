using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Services;
using RamenRatings.WebSite.Models;
using System.Collections.Generic;

namespace RamenRatings.WebSite.Pages
{
    /// <summary>
    /// Page model for reading an existing ramen product based on product information it the JSON file
    /// </summary>
    public class ReadModel : PageModel
    {
        /// <summary>
        /// Constructor to initialize the product service
        /// </summary>
        public ReadModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        /// <summary>
        /// Product service to access data
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// Product that will be used for the page
        /// </summary>
        public ProductModel Product;

        /// <summary>
        /// Runs when the page loads with GET request
        /// </summary>
        /// <param name="number">Product ID</param>
        /// <returns>Page result or redirect if product not found</returns>
        public IActionResult OnGet(int number)
        {
            // Retrieve product by ID
            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));

            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product. Please retry.";
                return RedirectToPage("../Error");
            }

            return Page();
        }

        /// <summary>
        /// Handles POST request to add a new rating to the product
        /// </summary>
        /// <param name="ProductNumber">Product ID</param>
        /// <param name="Rating">New rating value</param>
        /// <returns>Redirects back to the same read page</returns>
        public IActionResult OnPostAddRatingAsync(int ProductNumber, int Rating)
        {
            // Get the product to update
            var product = ProductService.GetProducts().FirstOrDefault(p => p.Number == ProductNumber);

            if ((product == null) == false)
            {
                // Append new rating to existing ratings array
                List<int> ratings;

                if ((product.Ratings == null) == false)
                {
                    ratings = product.Ratings.ToList();
                }
                else
                {
                    ratings = new List<int>();
                }

                ratings.Add(Rating);
                product.Ratings = ratings.ToArray();

                // Save updated product
                ProductService.UpdateProduct(product);
            }

            // Redirect back to the same product read page
            return RedirectToPage(new { number = ProductNumber });
        }
    }
}
