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
    /// Page model for creating a new ramen product, including form validation,
    /// file upload, and adding data to JSON file.
    /// </summary>
    public class CreateModel : PageModel
    {
        // Path to the JSON that stores product data
        public const string JsonFileName = "wwwroot/data/ramen.json"; // path to ramen json

        // Service used to retrieve ramen product data
        public JsonFileProductService ProductService { get; }

        // The variety name entered by the user (required and alphanumeric)
        [Required(ErrorMessage = "Variety is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Variety must be alphanumeric.")]
        public string Variety { get; set; }

        // The country selected for the ramen product (required)
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        // The full product model being created
        [BindProperty]
        public ProductModel NewProduct { get; set; }

        // The uploaded image file for the new ramen product
        [BindProperty]
        public IFormFile Image { get; set; }

        // Custom brand entered by user when "Other" is selected.
        [BindProperty]
        public string NewBrand { get; set; }

        // Custom style entered by user when "Other" is selected.
        [BindProperty]
        public string NewStyle { get; set; }

        // Initial rating given to the new ramen product
        [BindProperty]
        public int Rating { get; set; }


        // List of all existing brands for dropdown display and validation
        public List<string> ExistingBrands { get; set; }
        // List of all existing styles for dropdown display and validation
        public List<string> ExistingStyles { get; set; }

        // List of countries available for selection.
        public readonly List<string> Countries = new List<string> { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua &amp; Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia &amp; Herzegovina", "Botswana", "Brazil", "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Cape Verde", "Cayman Islands", "Chad", "Chile", "China", "Colombia", "Congo", "Cook Islands", "Costa Rica", "Cote D Ivoire", "Croatia", "Cruise Ship", "Cuba", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Fiji", "Finland", "France", "French Polynesia", "French West Indies", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea Bissau", "Guyana", "Haiti", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kuwait", "Kyrgyz Republic", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Mauritania", "Mauritius", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Norway", "Oman", "Pakistan", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russia", "Rwanda", "Saint Pierre &amp; Miquelon", "Samoa", "San Marino", "Satellite", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sri Lanka", "St Kitts &amp; Nevis", "St Lucia", "St Vincent", "St. Lucia", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor L'Este", "Togo", "Tonga", "Trinidad &amp; Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks &amp; Caicos", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe" };

        // Constructor that initializes the product service 
        public CreateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // handles the get request when the page is being accessed
        public void OnGet()
        {
            var products = ProductService.GetProducts(); // gets the products

            // creates the dropdown for Brands and Styles in the form field
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
        }

        // handles the post request when the page is finished being accessed
        public IActionResult OnPost()
        {
            var products = ProductService.GetProducts(); // gets the products
            // stores existing styles and brands
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList(); 
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            // create the new product information
            var newProduct = CreateData();
            var updatedDataSet = products.Append(newProduct); //add the new product to the end of the dataset
            SaveData(updatedDataSet);

            return RedirectToPage("/Product/Read", new { number = newProduct.Number }); //redirects to the new product's read page
        }

        // Builds a new product from form input, saves the uploaded image, and returns the model
        public ProductModel CreateData()
        {
            var products = ProductService.GetProducts(); // get the products
            int newNumber = products.Max(p => p.Number) + 1; //record the new product's numbre

            // record product values entered in form fields 
            string brand = NewProduct.Brand;
            string style = NewProduct.Style;
            string variety = NewProduct.Variety;
            string country = NewProduct.Country;

            // If "Other" was selected for Brand or Style, ensure the value gets recorded
            if (brand == "Other" && (string.IsNullOrEmpty(NewProduct.Brand) == false))
            {
                brand = NewBrand;
            }

            if (style == "Other" && (string.IsNullOrEmpty(NewProduct.Style) == false))
            {
                style = NewStyle;
            }

            // figures out what to name the new product image and where to put it
            var fileExtension = Path.GetExtension(Image.FileName);
            string imageFileName = $"{newNumber}{fileExtension}";
            string jsonImageName = "/images/" + imageFileName;
            string imagePath = "wwwroot" + jsonImageName;

            // Ensure the image directory exists
            var directory = Path.GetDirectoryName(imagePath);
            if (Directory.Exists(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            // Save the uploaded image
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                Image.CopyTo(fileStream);
            }

            // create the new product and return it
            var newProduct = new ProductModel
            {
                Number = newNumber,
                Brand = brand,
                Style = style,
                Variety = variety,
                Country = country,
                img = jsonImageName,
                Ratings = new int[] { Rating }
            };

            return newProduct;
        }


        // saves the data into the json
        public void SaveData(IEnumerable<ProductModel> dataSet)
        {
            var json = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
