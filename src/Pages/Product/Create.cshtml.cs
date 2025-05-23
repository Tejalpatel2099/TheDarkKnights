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
        public const string JsonFileName = "wwwroot/data/ramen.json";

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

        public List<string> ExistingBrands { get; set; }
        public List<string> ExistingStyles { get; set; }

        public List<string> VegetarianOptions { get; set; } = new List<string> { "Veg", "Not Veg" };

        public readonly List<string> Countries = new List<string>
        {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Anguilla", "Antigua & Barbuda", "Argentina", "Armenia",
            // (truncated for brevity, include full list as in your original file)
            "Yemen", "Zambia", "Zimbabwe"
        };

        public CreateModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public void OnGet()
        {
            var products = ProductService.GetProducts();
            ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            ExistingStyles = products.Select(p => p.Style).Distinct().ToList();
            VegetarianOptions = new List<string> { "Veg", "Not Veg" };
        }

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

            var newProduct = new ProductModel
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

            return newProduct;
        }

        public void SaveData(IEnumerable<ProductModel> dataSet)
        {
            var json = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(JsonFileName, json);
        }
    }
}
