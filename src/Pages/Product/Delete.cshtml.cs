using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RamenRatings.WebSite.Pages.Product
{
    /// <summary>
    // Razor Page Model for handling deletion of a product (ramen entry)
    /// <summary>
    public class DeleteModel : PageModel
    {
        // Path to the JSON file that stores product data
        public const string JsonFileName = "wwwroot/data/ramen.json";

        // Constructor injecting the product service
        public DeleteModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // Service to interact with product data (load/save)
        public JsonFileProductService ProductService { get; }

        // Binds the product data between the form and the model
        [BindProperty]
        public ProductModel Product { get; set; }

        /// <summary>
        /// Retrieves the product by ID and displays the delete confirmation page
        /// </summary>
        // Handles GET request to display the delete confirmation page
        public IActionResult OnGet(int number)
        {
            // Retrieve the product by its Number (ID)
            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));

            // If the product does not exist, redirect to error page with a message
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }

            // Render the delete confirmation page
            return Page();
        }

        /// <summary>
        /// Deletes the product and redirects to index if model state is valid
        /// </summary>
        // Handles POST request to confirm and perform the delete operation
        public IActionResult OnPost()
        {

            // Delete the product using its unique Number (ID)
            DeleteData(Product.Number);

            // After deletion, redirect back to products page
            return RedirectToPage("/Index");
        }

        /// <summary>
        /// Deletes the product by ID and returns the deleted product object
        /// </summary>
        // Deletes the product by its ID and returns the deleted product
        public ProductModel DeleteData(int id)
        {
            // Get the full list of products
            var dataSet = ProductService.GetProducts();

            // Find the specific product to delete
            var data = dataSet.FirstOrDefault(m => m.Number.Equals(id));

            // Create a new dataset without the deleted product
            var newDataSet = ProductService.GetProducts().Where(m => m.Number != id);

            // Save the updated dataset back to the JSON file
            SaveData(newDataSet);

            // Return the deleted product
            return data;
        }

        /// <summary>
        /// Saves the updated dataset to the JSON file with proper formatting
        /// </summary>
        // Serializes the dataset and writes it to the JSON file
        public void SaveData(IEnumerable<ProductModel> dataSet)
        {
            // Convert the dataset to a formatted JSON string
            var json = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });

            // Write the JSON string to the file, overwriting the old data
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
