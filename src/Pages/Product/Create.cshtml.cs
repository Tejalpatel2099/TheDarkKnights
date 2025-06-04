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
