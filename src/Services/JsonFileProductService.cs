using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using RamenRatings.WebSite.Models;
using Microsoft.AspNetCore.Hosting;

namespace RamenRatings.WebSite.Services
{
    public class JsonFileProductService
    {
        // WebHostEnvironment injected via constructor
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // Gets path of the JSON file ramen.json that will be used for product data
        private string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "ramen.json"); }
        }

        /// <summary>
        /// Returns all of the products in the JSON file.
        /// </summary>
        public IEnumerable<ProductModel> GetProducts()
        {
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<ProductModel[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }

        /// <summary>
        /// With the given productId adds a rating to that product.
        /// </summary>
        public bool AddRating(int productId, int rating)
        {
            // Set products with all products
            var products = GetProducts();

            // If the productId does not match the product, return false
            if (products.All(product => (product.Number == productId) == false))
            {
                return false;
            }

            // Set target as the first product that matches the given product ID
            var target = products.First(x => x.Number == productId);

            // List of ratings
            List<int> ratings;

            // Convert ratings to a list
            ratings = target.Ratings.ToList();
            
            // Add the new rating to the list
            ratings.Add(rating);
            target.Ratings = ratings.ToArray();

            // Save updated list
            SaveProducts(products);
            return true;
        }

        /// <summary>
        /// Updates the given product in the list and saves it.
        /// </summary>
        public void UpdateProduct(ProductModel updatedProduct)
        {
            var products = GetProducts().ToList();
            var index = products.FindIndex(p => p.Number == updatedProduct.Number);

            if ((index == -1) == false)
            {
                products[index] = updatedProduct;

                SaveProducts(products);
            }
        }

        /// <summary>
        /// Saves the product list to the JSON file.
        /// </summary>
        private void SaveProducts(IEnumerable<ProductModel> products)
        {
            using (var outputStream = File.Create(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<ProductModel>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        Indented = true
                    }),
                    products
                );
            }
        }
    }
}
