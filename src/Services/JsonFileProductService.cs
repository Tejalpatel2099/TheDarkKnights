using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using RamenRatings.WebSite.Models;

namespace RamenRatings.WebSite.Services
{
    /// <summary>
    /// Service for reading and writing product data to JSON file.
    /// </summary>
    public class JsonFileProductService
    {
        /// <summary>
        /// Constructor injects the web host environment to get access to the wwwroot path.
        /// </summary>
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        /// <summary>
        /// Full path to the JSON file.
        /// </summary>
        private string JsonFileName =>
            Path.Combine(WebHostEnvironment.WebRootPath, "data", "ramen.json");

        /// <summary>
        /// Returns all of the products in the JSON file.
        /// </summary>
        public IEnumerable<ProductModel> GetProducts()
        {
            using var jsonFileReader = File.OpenText(JsonFileName);

            return JsonSerializer.Deserialize<ProductModel[]>(
                jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        /// <summary>
        /// Adds a rating to the product with the specified ID.
        /// Returns false if product is not found.
        /// </summary>
        public bool AddRating(int productId, int rating)
        {
            var products = GetProducts();

            if (products.All(product => product.Number != productId))
            {
                return false;
            }

            var target = products.First(x => x.Number == productId);

            var ratings = target.Ratings?.ToList() ?? new List<int>();

            ratings.Add(rating);
            target.Ratings = ratings.ToArray();

            SaveProducts(products);

            return true;
        }

        /// <summary>
        /// Updates the given product in the JSON file.
        /// </summary>
        public void UpdateProduct(ProductModel updatedProduct)
        {
            var products = GetProducts().ToList();
            var index = products.FindIndex(p => p.Number == updatedProduct.Number);

            if (index != -1)
            {
                products[index] = updatedProduct;
                SaveProducts(products);
            }
        }

        /// <summary>
        /// Writes the given product list to the JSON file.
        /// </summary>
        private void SaveProducts(IEnumerable<ProductModel> products)
        {
            using var outputStream = File.Create(JsonFileName);

            JsonSerializer.Serialize(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    Indented = true
                }),
                products
            );
        }
    }
}
