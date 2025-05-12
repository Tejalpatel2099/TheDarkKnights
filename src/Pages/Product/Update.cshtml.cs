using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Pages.Product
{
    /// <summary>
    /// Update Model for Updating the Ramen product
    /// </summary>
    public class UpdateModel : PageModel
    {
        // path to json file
        public const string JsonFileName = "wwwroot/data/ramen.json"; 

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

        /// <summary>
        /// Constructor for the UpdateModel
        /// </summary>
        /// <param name="productService"></param>
        public UpdateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // Service to get the product data
        public JsonFileProductService ProductService { get; }

        // / Product model to bind the form data
        [BindProperty]
        public ProductModel Product { get; set; }

        /// <summary>
        /// OnGet method to get the product data for the given number
        /// </summary>
        /// <param name="number"> Unique number of the product </param>
        /// <returns> return the page or redirect to error if number not found </returns>
        public IActionResult OnGet(int number)
        {
            // Get all of the products
            var products = ProductService.GetProducts();

            // Create the list of products for the form field drop down
            ExistingBrands = products
            .Select(p => p.Brand)  // Select the brand from each product
            .Distinct()  // Remove duplicates
            .ToList();  // Convert to list 

            // Create the list of styles for the form field drop down
            ExistingStyles = products
                .Select(p => p.Style)  // Select the style from each product
                .Distinct()  // Remove duplicates
                .ToList();  // Convert to list

            // Get the product for the given number
            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }

            return Page();
        }
        /// <summary>
        /// Handle the post request to update the product
        /// also saves the updated product to the json file
        /// </summary>
        public IActionResult OnPost()
        {
            // if the model state is not valid, return the page with the error message
            if (ModelState.IsValid == false)
            {
                return Page();
            }
            // updating the product 
            var updatedProduct = UpdateData();

            // save the updated product to the json file
            SaveData(updatedProduct);

            return RedirectToPage("/Product/ProductsPage");
        }

        /// <summary>
        /// Updates the product data 
        /// </summary>
        public ProductModel UpdateData()
        {
            // get the original values
            var original = ProductService.GetProducts().FirstOrDefault(p => p.Number == Product.Number);

            // Set the values
            string brand = Product.Brand;
            string style = Product.Style;
            string variety = original.Variety;
            string country = original.Country;
            string jsonImageName = original.img;

            // Update if a new value was provided. Ensure null or empty is not entered
            if (brand == "Other" && (string.IsNullOrEmpty(Product.Brand) == false)) // account for Other field in brand
            {
                brand = NewBrand;
            }
            // Update if a new value was provided. Ensure null or empty is not entered
            if (style == "Other" && (string.IsNullOrEmpty(Product.Style) == false)) // account for Other field in style
            {
                style = NewStyle;
            }
            // Update Variety if a new value was provided. Ensure null or empty is not entered
            if (string.IsNullOrEmpty(Product.Variety) == false)
            {
                variety = Product.Variety;
            }

            // Update Country if a new value was provided. Ensure null or empty is not entered
            if (string.IsNullOrEmpty(Product.Country) == false)
            {
                country = Product.Country;
            }
            // If the image is not null, save the image and update the jsonImageName
            if ((Image == null) == false)
            {
                var fileExtension = Path.GetExtension(Image.FileName);
                string imageFileName = $"{original.Number}{fileExtension}";
                jsonImageName = "/images/" + imageFileName;
                string imagePath = "wwwroot" + jsonImageName;

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    Image.CopyTo(fileStream);
                }
            }
            // Return the updated product model
            return new ProductModel
            {
                Number = original.Number,
                Brand = brand,
                Style = style,
                Variety = variety,
                Country = country,
                img = jsonImageName,
                Ratings = original.Ratings
            };
        }
        /// <summary>
        /// Saves the updated product to the json file
        /// </summary>
        public void SaveData(ProductModel updateProduct)
        {
            // Get all of the products
            var products = ProductService.GetProducts().ToList();

            // Find the index of the product to update
            var index = products.FindIndex(p => p.Number == updateProduct.Number);

            // If the product is found, update it
            if ((index == -1) == false)
            {
                products[index] = updateProduct;
            }
            //Serialize the updated product list to JSON
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }

    }
}
