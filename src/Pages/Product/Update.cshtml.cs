using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RamenRatings.WebSite.Pages.Product
{
    /// <summary>
    /// Update Model for Updating the Ramen product
    /// </summary>
    public class UpdateModel : PageModel
    {
        /// <summary>
        /// Path to the JSON data file
        /// </summary>
        public const string JsonFileName = "wwwroot/data/ramen.json";

        /// <summary>
        /// Gets or sets the uploaded image file.
        /// </summary>
        [BindProperty]
        public IFormFile Image { get; set; }

        /// <summary>
        /// Gets or sets the new brand input if "Other" is selected.
        /// </summary>
        [BindProperty]
        public string NewBrand { get; set; }

        /// <summary>
        /// Gets or sets the new style input if "Other" is selected.
        /// </summary>
        [BindProperty]
        public string NewStyle { get; set; }

        /// <summary>
        /// Gets or sets the error message for the brand field.
        /// </summary>
        public string BrandError { get; set; }

        /// <summary>
        /// Gets or sets the error message for the style field.
        /// </summary>
        public string StyleError { get; set; }

        /// <summary>
        /// Gets or sets the error message for the variety field.
        /// </summary>
        public string VarietyError { get; set; }

        /// <summary>
        /// Gets or sets the error message for a duplicate/existing product being entered.
        /// </summary>
        public string DuplicateError { get; set; }

        /// <summary>
        /// Indicates whether "Other" was selected for Brand.
        /// </summary>
        public bool isOtherBrand { get; set; }

        /// <summary>
        /// Indicates whether "Other" was selected for Style.
        /// </summary>
        public bool isOtherStyle { get; set; }

        /// <summary>
        /// List of existing brand options.
        /// </summary>
        public List<string> ExistingBrands { get; set; }

        /// <summary>
        /// List of existing style options.
        /// </summary>
        public List<string> ExistingStyles { get; set; }

        /// <summary>
        /// List of vegetarian options.
        /// </summary>
        public List<string> VegetarianOptions { get; set; } = new List<string> { "Veg", "Not Veg" };

        /// <summary>
        /// List of country options.
        /// </summary>
        public readonly List<string> Countries = new()
        {
            // [List truncated for brevity]
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua and Barbuda", "Argentina", "Armenia", "Australia",
    "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize",
    "Benin", "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso",
    "Burundi", "Cabo Verde", "Cambodia", "Cameroon", "Canada", "Central African Republic", "Chad", "Chile", "China",
    "Colombia", "Comoros", "Congo (Congo-Brazzaville)", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czech Republic",
    "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt",
    "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini", "Ethiopia", "Fiji", "Finland", "France",
    "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guinea-Bissau",
    "Guyana", "Haiti", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel",
    "Italy", "Ivory Coast", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan",
    "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg",
    "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius",
    "Mexico", "Micronesia", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar",
    "Namibia", "Nauru", "Nepal", "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea",
    "North Macedonia", "Norway", "Oman", "Pakistan", "Palau", "Palestine", "Panama", "Papua New Guinea", "Paraguay",
    "Peru", "Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russia", "Rwanda", "Saint Kitts and Nevis",
    "Saint Lucia", "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia",
    "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands",
    "Somalia", "South Africa", "South Korea", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden",
    "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga",
    "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates",
    "United Kingdom", "United States", "Uruguay", "Uzbekistan", "Vanuatu", "Vatican City", "Venezuela", "Vietnam",
    "Yemen", "Zambia", "Zimbabwe"
        };

        /// <summary>
        /// Service for accessing and manipulating product data.
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// Gets or sets the product being updated.
        /// </summary>
        [BindProperty]
        public ProductModel Product { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateModel"/> class.
        /// </summary>
        /// <param name="productService">The product service instance.</param>
        public UpdateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        /// <summary>
        /// Handles GET requests to fetch and display the product for updating.
        /// </summary>
        /// <param name="number">The unique identifier of the product.</param>
        /// <returns>The page result or a redirect if the product is not found.</returns>
        public IActionResult OnGet(int number)
        {
            // Get all of the products in the product service
            var products = ProductService.GetProducts();

            // Set the list of existing brands and styles without duplicates
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            // Get the product number 
            Product = products.FirstOrDefault(p => p.Number == number);

            // If the product number is null, return an error page
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }

            // return to the page
            return Page();
        }

        /// <summary>
        /// Handles POST requests to update the product.
        /// </summary>
        /// <returns>A redirect to the products page on success, or the same page on failure.</returns>
        public IActionResult OnPost()
        {
            // Get all of the product in the product service
            var products = ProductService.GetProducts();

            // Set the list of existing brands and styles without duplicates
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            // If the model state is valid, return the page
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Set the updated product with the updated data
            var updatedProduct = UpdateData();

            // If validation fails, return the page
            if (!ValidateData(updatedProduct, isOtherBrand, isOtherStyle))
            {
                return Page();
            }

            // Save the updated product to the data
            SaveData(updatedProduct);

            // Return to the products page
            return RedirectToPage("/Product/ProductsPage");
        }

        /// <summary>
        /// Updates the product with the new input values.
        /// </summary>
        /// <returns>The updated <see cref="ProductModel"/>.</returns>
        public ProductModel UpdateData()
        {
            // Get the original product from the data source using its unique number
            var original = ProductService.GetProducts().FirstOrDefault(p => p.Number == Product.Number);

            // Determine the final brand value; use new brand if "Other" was selected
            string brand = Product.Brand == "Other" ? NewBrand : Product.Brand;

            // Determine the final style value; use new style if "Other" was selected
            string style = Product.Style == "Other" ? NewStyle : Product.Style;

            // Flags for use in validation for brand and style
            isOtherBrand = Product.Brand == "Other";
            isOtherStyle = Product.Style == "Other";

            // Preserve existing values for variety, country, vegetarian if new input fields are empty
            string variety = string.IsNullOrEmpty(Product.Variety) ? original.Variety : Product.Variety;
            string country = string.IsNullOrEmpty(Product.Country) ? original.Country : Product.Country;
            string vegetarian = string.IsNullOrEmpty(Product.Vegetarian) ? original.Vegetarian : Product.Vegetarian;

            // Start with the original image filename unless a new one is uploaded
            string jsonImageName = original.img;

            if (Image != null)
            {
                // Extract file extension and create a new filename
                var fileExtension = Path.GetExtension(Image.FileName);

                // Combine fileExtension with the product number
                string imageFileName = $"{original.Number}{fileExtension}";

                // Add the image folder to imageFileName
                jsonImageName = "/images/" + imageFileName;

                // Combine with the root folder
                string imagePath = "wwwroot" + jsonImageName;

                // Create file stream and copy the image to the filestream
                using var fileStream = new FileStream(imagePath, FileMode.Create);
                Image.CopyTo(fileStream);
            }

            // Return the product model with the attributes (new and/or existing)
            return new ProductModel
            {
                Number = original.Number,
                Brand = brand,
                Style = style,
                Variety = variety,
                Country = country,
                Vegetarian = vegetarian,
                img = jsonImageName,
                Ratings = original.Ratings
            };
        }

        /// <summary>
        /// Validates the updated product data.
        /// </summary>
        /// <param name="product">The product to validate.</param>
        /// <param name="isOtherBrand">Whether "Other" was selected for Brand.</param>
        /// <param name="isOtherStyle">Whether "Other" was selected for Style.</param>
        /// <returns><c>true</c> if the data is valid; otherwise, <c>false</c>.</returns>
        public bool ValidateData(ProductModel product, bool isOtherBrand, bool isOtherStyle)
        {
            // Check if the brand entered or not
            if (string.IsNullOrWhiteSpace(product.Brand))
            {
                // Set brand error message and return false
                BrandError = "Brand is required.";
                return false;
            }
            // Check if the brand already exists and the user has selected "Other"
            if (ExistingBrands.Contains(product.Brand) && isOtherBrand)
            {
                // Set brand error message and return false
                BrandError = "Brand already exists"; 
                return false;
            }

            // Enforce a character limit for the brand name
            if (product.Brand.Length > 20)
            {
                // Set character limit error message
                BrandError = "Character Limit is 20"; 
                return false;
            }
            // Check if the brand entered or not
            if (string.IsNullOrWhiteSpace(product.Style))
            {
                // Set style error message and return false
                StyleError = "Style is required.";
                return false;
            }
            // Check if the style already exists and the user has selected "Other"
            if (ExistingStyles.Contains(product.Style) && isOtherStyle)
            {
                // Set style error message
                StyleError = "Style already exists"; 
                return false;
            }

            // Enforce a character limit for the style name
            if (product.Style.Length > 20)
            {
                // Set style character limit 
                StyleError = "Character Limit is 20";
                return false;
            }
            // Check if the variety entered or not
            if (string.IsNullOrWhiteSpace(product.Variety))
            {
                // Set variety error message and return false
                VarietyError = "Variety is required.";
                return false;
            }
            // Enforce a character limit for the variety name
            if (product.Variety.Length > 35)
            {
                // Set variety character limit
                VarietyError = "Character Limit is 35";
                return false;
            }
            

            // Check if a product with the same brand, variety, country, style,
            // and pack already exists (excluding the current one)
            var duplicateExists = ProductService.GetProducts().Any(p =>
            p.Number != product.Number &&
            p.Brand.Equals(product.Brand, StringComparison.OrdinalIgnoreCase) &&
            p.Variety.Equals(product.Variety, StringComparison.OrdinalIgnoreCase) &&
            p.Country.Equals(product.Country, StringComparison.OrdinalIgnoreCase) &&
            p.Vegetarian.Equals(product.Vegetarian, StringComparison.OrdinalIgnoreCase) &&
            p.Style.Equals(product.Style, StringComparison.OrdinalIgnoreCase));

            // If a duplicate product exists, show error that the product already exists
            if (duplicateExists)
            {
                // Sets duplicate error message
                DuplicateError = "This product already exists. Modify one or more fields to make it unique.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves the updated product to the JSON data file by replacing the original entry in the product list.
        /// </summary>
        /// <param name="updateProduct">The updated <see cref="ProductModel"/> to save.</param>
        public void SaveData(ProductModel updateProduct)
        {
            // Get all of the product in the product service to a list
            var products = ProductService.GetProducts().ToList();

            // Set the index of a product number being updated
            var index = products.FindIndex(p => p.Number == updateProduct.Number);

            // If index is a valid number, then update the product
            if (index >= 0)
            {
                products[index] = updateProduct;
            }

            // Serialize the JSON with the change
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });

            // Write the text to the JSON
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
