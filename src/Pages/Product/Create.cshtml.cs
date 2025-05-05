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
            ExistingBrands = products
            .Select(p => p.Brand)  // Select the brand from each product
            .Distinct()  // Remove duplicates
            .ToList();  // Convert to list 

            // Create the list of styles for the form field drop down
            ExistingStyles = products
                .Select(p => p.Style)  // Select the style from each product
                .Distinct()  // Remove duplicates
                .ToList();  // Convert to list
        }

        public IActionResult OnPost()
        {

            if (!ModelState.IsValid)
            {
                return Page(); // Return to the page if the form data is invalid
            }

            // Create a new product 
            var newProduct = CreateData();

            // Save the updated products list
            SaveData(newProduct);

            return RedirectToPage("/Product/ProductsPage");
        }

        // Create new product and add to the dataset
        public ProductModel CreateData()
        {
            // Generate the next product number. Find the highest existing product number and add 1
            var products = ProductService.GetProducts();
            int newNumber = products.Max(p => p.Number) + 1;

            // Determine the brand to use, existing or new
            string brand;
            if (string.IsNullOrEmpty(NewProduct.Brand))
            {
                brand = NewBrand;  // Use the new brand if the selected brand is empty
            }
            else
            {
                brand = NewProduct.Brand;  // Use the selected brand if available
            }

            // Determine the style to use, existing or new
            string style;
            if (string.IsNullOrEmpty(NewProduct.Style))
            {
                style = NewStyle;  // Use the new style if the selected style is empty
            }
            else
            {
                style = NewProduct.Style;  // Use the selected style if available
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
                Variety = NewProduct.Variety,
                Country = NewProduct.Country,
                img = jsonImageName,  // Store the image json name 
                Ratings = new int[] { Rating }  // Initialize ratings with given rating
            };

            return newProduct;
        }

        // Save the updated dataset back to the JSON file
        public void SaveData(ProductModel newProduct)
        {
            // get the products into a list format
            var products = ProductService.GetProducts().ToList();
            //add the new product to the products list
            products.Add(newProduct);

            // create the json in a json string
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });

            // write the json text into the json
            System.IO.File.WriteAllText(JsonFileName, json);
           
        }
    }
}
