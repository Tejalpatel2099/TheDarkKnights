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
    public class CreateModel : PageModel
    {
        public const string JsonFileName = "wwwroot/data/ramen.json"; // path to ramen json

        public JsonFileProductService ProductService { get; }

        [Required(ErrorMessage = "Variety is required.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Variety must be alphanumeric.")]
        public string Variety { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [BindProperty]
        public ProductModel NewProduct { get; set; }

        [BindProperty]
        public IFormFile Image { get; set; }

        [BindProperty]
        public string NewBrand { get; set; }

        [BindProperty]
        public string NewStyle { get; set; }

        [BindProperty]
        public int Rating { get; set; }

        //If brand selected in other is already used
        public bool isOtherBrand { get; set; }

        //If sytle selected in other is already used
        public bool isOtherStyle { get; set; }

        //Brand is not validated will send an error
        public string BrandError { get; set; }

        //Style is not validated will send an error
        public string StyleError { get; set; }

        public List<string> ExistingBrands { get; set; }
        public List<string> ExistingStyles { get; set; }

        public List<string> Countries { get; set; } = new List<string> { "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua &amp; Barbuda", "Argentina", "Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bermuda", "Bhutan", "Bolivia", "Bosnia &amp; Herzegovina", "Botswana", "Brazil", "British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Cape Verde", "Cayman Islands", "Chad", "Chile", "China", "Colombia", "Congo", "Cook Islands", "Costa Rica", "Cote D Ivoire", "Croatia", "Cruise Ship", "Cuba", "Cyprus", "Czech Republic", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Estonia", "Ethiopia", "Falkland Islands", "Faroe Islands", "Fiji", "Finland", "France", "French Polynesia", "French West Indies", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea", "Guinea Bissau", "Guyana", "Haiti", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jersey", "Jordan", "Kazakhstan", "Kenya", "Kuwait", "Kyrgyz Republic", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Mauritania", "Mauritius", "Mexico", "Moldova", "Monaco", "Mongolia", "Montenegro", "Montserrat", "Morocco", "Mozambique", "Namibia", "Nepal", "Netherlands", "Netherlands Antilles", "New Caledonia", "New Zealand", "Nicaragua", "Niger", "Nigeria", "Norway", "Oman", "Pakistan", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Puerto Rico", "Qatar", "Reunion", "Romania", "Russia", "Rwanda", "Saint Pierre &amp; Miquelon", "Samoa", "San Marino", "Satellite", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "South Africa", "South Korea", "Spain", "Sri Lanka", "St Kitts &amp; Nevis", "St Lucia", "St Vincent", "St. Lucia", "Sudan", "Suriname", "Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tanzania", "Thailand", "Timor L'Este", "Togo", "Tonga", "Trinidad &amp; Tobago", "Tunisia", "Turkey", "Turkmenistan", "Turks &amp; Caicos", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam", "Virgin Islands (US)", "Yemen", "Zambia", "Zimbabwe" };


        public CreateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // handles the get request when the page is being accessed
        public void OnGet()
        {
            var products = ProductService.GetProducts();

            // creates the dropdown for Brands and Styles in the form field
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
        }

        // handles the post request when the page is finished being accessed
        public IActionResult OnPost()
        {
            var products = ProductService.GetProducts();
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            var newProduct = CreateData();
            if (ValidateData(newProduct, isOtherBrand, isOtherStyle) == false)
            {
                return Page();
            }
            var updatedDataSet = products.Append(newProduct); //add the new product to the end of the dataset
            SaveData(updatedDataSet);

            return RedirectToPage("/Product/Read", new { number = newProduct.Number }); //redirects to the new product's read page
        }

        public ProductModel CreateData()
        {
            var products = ProductService.GetProducts();
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
                isOtherBrand = true;
            }

            if (style == "Other" && (string.IsNullOrEmpty(NewProduct.Style) == false))
            {
                style = NewStyle;
                isOtherStyle = true;
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
        //Checks for character limit and if "other" brand and style are distinct
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
            return true;
        }

        // saves the data into the json
        public void SaveData(IEnumerable<ProductModel> dataSet)
        {
            var json = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
