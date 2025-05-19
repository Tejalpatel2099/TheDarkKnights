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

        //Brand is not validated will send an error
        public string BrandError { get; set; }

        //Style is not validated will send an error
        public string StyleError { get; set; }

        //Variety is not validated will send an error
        public string VarietyError { get; set; }

        // If brand selected in other is already used
        public bool isOtherBrand { get; set; }

        // If style selected in other is already used
        public bool isOtherStyle { get; set; }

        // List of existing brands for the dropdown
        public List<string> ExistingBrands { get; set; }

        // List of existing styles for the dropdown
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

            // If the Product is null, show an error message and redirect to the error page
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

            // if the model state is not valid, return the page with the error message
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            // updating the product 
            var updatedProduct = UpdateData();

            // If there are validation errors occuring, return the page
            if (ValidateData(updatedProduct, isOtherBrand, isOtherStyle) == false)
            {
                return Page();
            }

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
                isOtherBrand = true;
            }

            // Update if a new value was provided. Ensure null or empty is not entered
            if (style == "Other" && (string.IsNullOrEmpty(Product.Style) == false)) // account for Other field in style
            {
                style = NewStyle;
                isOtherStyle = true;
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
                var fileExtension = Path.GetExtension(Image.FileName); // gets file name of image
                string imageFileName = $"{original.Number}{fileExtension}"; // puts together number and file name
                jsonImageName = "/images/" + imageFileName; // adds the images folder to the front of the file name
                string imagePath = "wwwroot" + jsonImageName; // adds the root folder to the front of the file path

                // Copies the image file path to the image folder
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
        /// Validates the input of the form fields and sets the errors
        /// </summary>
        public bool ValidateData(ProductModel product, bool isOtherBrand, bool isOtherStyle)
        {
            // Checks that the input in Other Brand exists
            if (ExistingBrands.Contains(product.Brand) && isOtherBrand)
            {
                BrandError = "Brand already exists";
                return false;
            }

            // Checks that the input in Brand exceeds character limit
            if (product.Brand.Length > 20)
            {
                BrandError = "Character Limit is 20";
                return false;
            }

            // Checks that the input in Other style exists 
            if (ExistingStyles.Contains(product.Style) && isOtherStyle)
            {
                StyleError = "Style already exists";
                return false;
            }

            // Checks that the input in Style exceeds character limit
            if (product.Style.Length > 20)
            {
                StyleError = "Character Limit is 20";
                return false;
            }

            // Checks that the input in Variety exceeds character limit
            if (product.Variety.Length > 20)
            {
                VarietyError = "Character Limit is 20";
                return false;
            }

            // else, return that there are no validation errors
            return true;
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
