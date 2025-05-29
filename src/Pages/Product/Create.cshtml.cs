using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RamenRatings.WebSite.Pages.Product
{
    /// <summary>
    /// PageModel for creating a new ramen product.
    /// </summary>
    public class CreateModel : PageModel
    {
        /// <summary>
        /// Path to the JSON file where product data is stored.
        /// </summary>
        public const string JsonFileName = "wwwroot/data/ramen.json";

        /// <summary>
        /// Service to manage product data.
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// Variety of the ramen product. Alphanumeric validation enforced.
        /// </summary>
        [Required(ErrorMessage = "Variety is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Variety must be alphanumeric.")]
        public string Variety { get; set; }

        /// <summary>
        /// Country of origin for the ramen product.
        /// </summary>
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        /// <summary>
        /// The new product model bound to the form.
        /// </summary>
        [BindProperty]
        public ProductModel NewProduct { get; set; }

        /// <summary>
        /// The uploaded image file.
        /// </summary>
        [BindProperty]
        public IFormFile Image { get; set; }

        /// <summary>
        /// Custom brand input if "Other" is selected.
        /// </summary>
        [BindProperty]
        public string NewBrand { get; set; }

        /// <summary>
        /// Custom style input if "Other" is selected.
        /// </summary>
        [BindProperty]
        public string NewStyle { get; set; }

        /// <summary>
        /// Rating for the new product.
        /// </summary>
        [BindProperty]
        public int Rating { get; set; }

        /// <summary>
        /// List of existing brands to populate the dropdown.
        /// </summary>
        public List<string> ExistingBrands { get; set; }

        /// <summary>
        /// List of existing styles to populate the dropdown.
        /// </summary>
        public List<string> ExistingStyles { get; set; }

        /// <summary>
        /// List of vegetarian options.
        /// </summary>
        public List<string> VegetarianOptions { get; set; } = new List<string> { "Veg", "Not Veg" };

        /// <summary>
        /// Static list of countries for country dropdown.
        /// </summary>
        public readonly List<string> Countries = new List<string>
        {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua & Barbuda", "Argentina", "Armenia",
            // (Truncated for brevity)
            "Yemen", "Zambia", "Zimbabwe"
        };

        /// <summary>
        /// Constructor that injects the product service.
        /// </summary>
        /// <param name="productService">Service to access product data.</param>
        public CreateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        /// <summary>
        /// Handles GET requests. Populates form dropdowns.
        /// </summary>
        public void OnGet()
        {
            var products = ProductService.GetProducts();
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
            VegetarianOptions = new List<string> { "Veg", "Not Veg" };
        }

        /// <summary>
        /// Handles POST requests. Validates form and saves new product.
        /// </summary>
        /// <returns>Redirects to read page on success; redisplays form on failure.</returns>
        public IActionResult OnPost()
        {
            var products = ProductService.GetProducts();
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
            VegetarianOptions = new List<string> { "Veg", "Not Veg" };

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newProduct = CreateData();
            var updatedDataSet = products.Append(newProduct);
            SaveData(updatedDataSet);

            return RedirectToPage("/Product/Read", new { number = newProduct.Number });
        }

        /// <summary>
        /// Creates a new ProductModel based on form data and uploaded image.
        /// </summary>
        /// <returns>The newly created ProductModel instance.</returns>
        public ProductModel CreateData()
        {
            var products = ProductService.GetProducts();
            int newNumber = products.Max(p => p.Number) + 1;

            string brand = NewProduct.Brand;
            string style = NewProduct.Style;
            string variety = NewProduct.Variety;
            string country = NewProduct.Country;
            string vegetarian = NewProduct.Vegetarian;

            if (brand == "Other" && !string.IsNullOrEmpty(NewBrand))
            {
                brand = NewBrand;
            }

            if (style == "Other" && !string.IsNullOrEmpty(NewStyle))
            {
                style = NewStyle;
            }

            var fileExtension = Path.GetExtension(Image.FileName);
            string imageFileName = $"{newNumber}{fileExtension}";
            string jsonImageName = "/images/" + imageFileName;
            string imagePath = "wwwroot" + jsonImageName;

            var directory = Path.GetDirectoryName(imagePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                Image.CopyTo(fileStream);
            }

            return new ProductModel
            {
                Number = newNumber,
                Brand = brand,
                Style = style,
                Variety = variety,
                Country = country,
                Vegetarian = vegetarian,
                img = jsonImageName,
                Ratings = new int[] { Rating }
            };
        }

        /// <summary>
        /// Saves the updated dataset to the JSON file.
        /// </summary>
        /// <param name="dataSet">The complete set of product data to save.</param>
        public void SaveData(IEnumerable<ProductModel> dataSet)
        {
            var json = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
