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
        //WebHostEnviornment namespace taken in constructor
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        //Gets path of the JSON file ramen.json that will be used for product data
        private string JsonFileName
        {
            get { return Path.Combine(WebHostEnvironment.WebRootPath, "data", "ramen.json"); }
        }
        //Returns all of the products in the JSON file
        public IEnumerable<ProductModel> GetProducts()
        {
            using(var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<ProductModel[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
        }
        //With the given productId adds a rating
        public void AddRating(int productId, int rating)
        {
            var products = GetProducts();
            //Checks if ratings array exists
            if(products.First(x => x.Number == productId).Ratings == null)
            {
                //Creates a new rating array consisting of the rating
                products.First(x => x.Number == productId).Ratings = new int[] { rating };
            }
            else
            {
                //Adds rating to already existing array
                var ratings = products.First(x => x.Number == productId).Ratings.ToList();
                ratings.Add(rating);
                products.First(x => x.Number == productId).Ratings = ratings.ToArray();
            }
            //Saves the new ratings in the JSON file
            using(var outputStream = File.OpenWrite(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<ProductModel>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }), 
                    products
                );
            }
        }
    }
}