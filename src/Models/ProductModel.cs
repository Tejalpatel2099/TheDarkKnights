using System.Text.Json;
using System.Text.Json.Serialization;

namespace RamenRatings.WebSite.Models
{
    public class ProductModel
    {
        public int Number { get; set; }
        public string Brand { get; set; }
        
        [JsonPropertyName("img")]
        public string img { get; set; }
        public string Style { get; set; }
        public string Country { get; set; }
        public string Variety { get; set; }
        public int[] Ratings { get; set; }
        
        public string[] Feedback { get; set; }

        public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);

 
    }
}