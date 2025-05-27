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
            "Zimbabwe"
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
            var products = ProductService.GetProducts();

            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            Product = products.FirstOrDefault(p => p.Number == number);

            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }

            return Page();
        }

        /// <summary>
        /// Handles POST requests to update the product.
        /// </summary>
        /// <returns>A redirect to the products page on success, or the same page on failure.</returns>
        public IActionResult OnPost()
        {
            var products = ProductService.GetProducts();

            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updatedProduct = UpdateData();

            if (!ValidateData(updatedProduct, isOtherBrand, isOtherStyle))
            {
                return Page();
            }

            SaveData(updatedProduct);

            return RedirectToPage("/Product/ProductsPage");
        }

        /// <summary>
        /// Updates the product with the new input values.
        /// </summary>
        /// <returns>The updated <see cref="ProductModel"/>.</returns>
        public ProductModel UpdateData()
        {
            var original = ProductService.GetProducts().FirstOrDefault(p => p.Number == Product.Number);

            string brand = Product.Brand == "Other" ? NewBrand : Product.Brand;
            string style = Product.Style == "Other" ? NewStyle : Product.Style;

            isOtherBrand = Product.Brand == "Other";
            isOtherStyle = Product.Style == "Other";

            string variety = string.IsNullOrEmpty(Product.Variety) ? original.Variety : Product.Variety;
            string country = string.IsNullOrEmpty(Product.Country) ? original.Country : Product.Country;
            string vegetarian = string.IsNullOrEmpty(Product.Vegetarian) ? original.Vegetarian : Product.Vegetarian;

            string jsonImageName = original.img;
            if (Image != null)
            {
                var fileExtension = Path.GetExtension(Image.FileName);
                string imageFileName = $"{original.Number}{fileExtension}";
                jsonImageName = "/images/" + imageFileName;
                string imagePath = "wwwroot" + jsonImageName;

                using var fileStream = new FileStream(imagePath, FileMode.Create);
                Image.CopyTo(fileStream);
            }

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
            if (ExistingBrands.Contains(product.Brand) && isOtherBrand)
            {
                BrandError = "Brand already exists";
                return false;
            }

            if (product.Brand.Length > 20)
            {
                BrandError = "Character Limit is 20";
                return false;
            }

            if (ExistingStyles.Contains(product.Style) && isOtherStyle)
            {
                StyleError = "Style already exists";
                return false;
            }

            if (product.Style.Length > 20)
            {
                StyleError = "Character Limit is 20";
                return false;
            }

            if (product.Variety.Length > 20)
            {
                VarietyError = "Character Limit is 20";
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
            var products = ProductService.GetProducts().ToList();
            var index = products.FindIndex(p => p.Number == updateProduct.Number);

            if (index != -1)
            {
                products[index] = updateProduct;
            }

            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
