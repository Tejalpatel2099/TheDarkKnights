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
    public class UpdateModel : PageModel
    {
        public const string JsonFileName = "wwwroot/data/ramen.json"; // path to ramen json

        // new product property
        [BindProperty]
        public ProductModel ExistingProduct { get; set; }

        // image property
        [BindProperty]
        public IFormFile Image { get; set; }

        // new brand 
        //[BindProperty]
        //public string NewBrand { get; set; }

        //// new style
        //[BindProperty]
        //public string NewStyle { get; set; }

        // new rating
        [BindProperty]
        public int Rating { get; set; }

        // List of existing brands and styles for the dropdown
        public List<string> ExistingBrands { get; set; }
        public List<string> ExistingStyles { get; set; }

        public UpdateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }

        public ProductModel Product;

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

            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }

            ExistingProduct = new ProductModel
            {
                Number = Product.Number,
                Brand = Product.Brand,
                Style = Product.Style,
                Variety = Product.Variety,
                Country = Product.Country,
                img = Product.img,
                Ratings = Product.Ratings
            };

            return Page();
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updatedProduct = UpdateData();

            SaveData(updatedProduct);

            return RedirectToPage("/Product/ProductsPage");
        }

        public ProductModel UpdateData()
        {
            string brand = ExistingProduct.Brand;
            string style = ExistingProduct.Style;

         
            string jsonImageName = ExistingProduct.img;

            if (Image != null)
            {
                var fileExtension = Path.GetExtension(Image.FileName);
                string imageFileName = $"{ExistingProduct.Number}{fileExtension}";
                jsonImageName = "/images/" + imageFileName;
                string imagePath = "wwwroot" + jsonImageName;

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    Image.CopyTo(fileStream);
                }
            }

            Console.WriteLine("Image Name"+jsonImageName);

            return new ProductModel
            {
                Number = ExistingProduct.Number,
                Brand = brand,
                Style = style,
                Variety = ExistingProduct.Variety,
                Country = ExistingProduct.Country,
                img = jsonImageName,
                Ratings = ExistingProduct.Ratings
            };
        }

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
