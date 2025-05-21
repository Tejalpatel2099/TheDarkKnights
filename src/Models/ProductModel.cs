using System.Text.Json;
using System.Text.Json.Serialization;

namespace RamenRatings.WebSite.Models
{
    /// <summary>
    /// Represents a ramen product model used on the website.
    /// Contains identifying details
    /// </summary>
    public class ProductModel
    {
        /// Get and set Number identifier for the product 

        public int Number { get; set; }

        /// <summary>
        /// Get and set the Brand for the product
        /// </summary>
        public string Brand { get; set; }
        
        /// <summary>
        /// Get and set the image for the product
        /// </summary>
        [JsonPropertyName("img")]
        public string img { get; set; }

        /// <summary>
        /// Get and set the style for the product
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Get and set the Country for the product
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Get and set the Variety for the product
        /// </summary>
        public string Variety { get; set; }

        /// <summary>
        /// Get and set the Vegeterian content for the product
        /// </summary>
        public string Vegetarian{ get; set; }

        /// <summary>
        /// Get and set the array of ratings for the product
        /// </summary>
        public int[] Ratings { get; set; }
        
        /// <summary>
        /// Get and set feedback for the product
        /// </summary>
        public string[] Feedback { get; set; }

        /// <summary>
        /// Serialize the ProductModel from the json
        /// </summary>
        /// <returns></returns>
        public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);

 
    }
}