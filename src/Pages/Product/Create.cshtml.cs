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

        // product service
        public JsonFileProductService ProductService { get; }

        // new product property
        [BindProperty]
        public ProductModel NewProduct { get; set; }

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

        // create model
        public CreateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // runs upon the page loading
        public void OnGet()
        {
            // Get all of the products
            var products = ProductService.GetProducts();

            // Create the list of products for the form field drop down
            // Select the brand from each product
            // Remove duplicates
            // Convert to list 
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();

            // Create the list of styles for the form field drop down
            // Select the style from each product
            // Remove duplicates
            // Convert to list
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
            
        }

        // runs the page after the create page is submitted
        public IActionResult OnPost()
        {

            if (!ModelState.IsValid)
            {
                return Page(); // Return to the page if the form data is invalid
            }

            // Create a new product 
            var newProduct = CreateData();

            // redirect to the new product's page
            return RedirectToPage("/Product/Read", new { number = newProduct.Number });
        }

        // Create new product and add to the dataset
        public ProductModel CreateData()
        {
            // Generate the next product number. Find the highest existing product number and add 1
            var products = ProductService.GetProducts();
            int newNumber = products.Max(p => p.Number) + 1;

            // Set the values
            string brand = NewProduct.Brand;
            string style = NewProduct.Style;
            string variety = NewProduct.Variety;
            string country = NewProduct.Country;

            // Determine the brand to use, existing or new. If existing brand is empty, set to new
            if (brand == "Other" && !string.IsNullOrEmpty(NewProduct.Brand))
            {
                brand = NewBrand;  // Use the new brand if the selected brand is empty
            }

            // Determine the style to use, existing or new. If existing style is empty, set to new
            if (style == "Other" && !string.IsNullOrEmpty(NewProduct.Style))
            {
                style = NewStyle;  // Use the new style if the selected style is empty
            }
           
            // Handle the image file upload 
            var fileExtension = Path.GetExtension(Image.FileName); // Get file extension 
            string imageFileName = $"{newNumber}{fileExtension}";  // Create image file name with the new product number
            string jsonImageName = "/images" + "/" + imageFileName;
            string imagePath = "wwwroot" + jsonImageName; // Save it in the images folder

            // Save the uploaded image 
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                Image.CopyTo(fileStream);
            }

            // Create the new product
            var newProduct = new ProductModel
            {
                Number = newNumber,
                Brand = brand,
                Style = style,
                Variety = variety,
                Country = country,
                img = jsonImageName,  // Store the image json name 
                Ratings = new int[] { Rating }  // Initialize ratings with given rating
            };

            // Append the product and save the set
            var updatedDataSet = products.Append(newProduct);
            SaveData(updatedDataSet);
            return newProduct;
        }

        // Save the updated dataset back to the JSON file
        public void SaveData(IEnumerable<ProductModel> dataSet)
        {
            // create the json in a json string
            var json = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });

            // write the json text into the json
            System.IO.File.WriteAllText(JsonFileName, json);
           
        }
    }
}
