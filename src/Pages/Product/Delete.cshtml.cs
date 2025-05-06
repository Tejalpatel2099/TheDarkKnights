using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Pages.Product
{
    public class DeleteModel : PageModel
    {
        public const string JsonFileName = "wwwroot/data/ramen.json"; // path to ramen json

        public DeleteModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }

        [BindProperty]
        public ProductModel Product { get; set; }

        public IActionResult OnGet(int number)
        {
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
            DeleteData(Product.Number);

            return RedirectToPage("/Index");

        }

        // using the number id of the product, deletes the item from the json file
        public ProductModel DeleteData(int id)
        {
            var dataSet = ProductService.GetProducts();
            var data = dataSet.FirstOrDefault(m => m.Number.Equals(id));

            var newDataSet = ProductService.GetProducts().Where(m => m.Number.Equals(id) == false);

            SaveData(newDataSet);

            return data;

        }

        // Save the updated dataset back to the JSON file with the deleted changes
        public void SaveData(IEnumerable<ProductModel> dataSet)
        {
            // create the json in a json string
            var json = JsonSerializer.Serialize(dataSet, new JsonSerializerOptions { WriteIndented = true });

            // write the json text into the json
            System.IO.File.WriteAllText(JsonFileName, json);

        }
    }
}
