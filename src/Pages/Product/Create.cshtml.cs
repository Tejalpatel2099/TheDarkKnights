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

namespace RamenRatings.WebSite.Pages.Product
{
    public class CreateModel : PageModel
    {
        public const string JsonFileName = "wwwroot/data/ramen.json"; // path to ramen json

        public JsonFileProductService ProductService { get; }

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

        public List<string> ExistingBrands { get; set; }
        public List<string> ExistingStyles { get; set; }

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newProduct = CreateData();

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
            if (brand == "Other" && !string.IsNullOrEmpty(NewProduct.Brand))
            {
                brand = NewBrand;
            }

            if (style == "Other" && !string.IsNullOrEmpty(NewProduct.Style))
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
            if (!Directory.Exists(directory))
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

            var updatedDataSet = products.Append(newProduct); //add the new product to the end of the dataset
            SaveData(updatedDataSet);
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
