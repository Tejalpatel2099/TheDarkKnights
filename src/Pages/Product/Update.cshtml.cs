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

        public UpdateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }

        [BindProperty]
        public ProductModel Product { get; set; }

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
            // get the original values
            var original = ProductService.GetProducts().FirstOrDefault(p => p.Number == Product.Number);

            // Set the values
            string brand = Product.Brand;
            string style = Product.Style;
            string variety = original.Variety;
            string country = original.Country;
            string jsonImageName = original.img;

            // Update if a new value was provided. Ensure null or empty is not entered
            if (brand == "Other" && !string.IsNullOrEmpty(Product.Brand)) // account for Other field in brand
            {
                brand = NewBrand;
            }

            if (style == "Other" && !string.IsNullOrEmpty(Product.Style)) // account for Other field in style
            {
                style = NewStyle;
            }

            if (!string.IsNullOrEmpty(Product.Variety))
            {
                variety = Product.Variety;
            }

            if (!string.IsNullOrEmpty(Product.Country))
            {
                country = Product.Country;
            }

            if (Image != null)
            {
                var fileExtension = Path.GetExtension(Image.FileName);
                string imageFileName = $"{original.Number}{fileExtension}";
                jsonImageName = "/images/" + imageFileName;
                string imagePath = "wwwroot" + jsonImageName;

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    Image.CopyTo(fileStream);
                }
            }

            return new ProductModel
            {
                Number = original.Number,
                Brand = brand,
                Style = style,
                Variety = variety,
                Country = country,
                img = jsonImageName,
                Ratings = original.Ratings
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
