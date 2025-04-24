using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContosoCrafts.WebSite.Models
{
    public class ProductModel
    {
        public string Number { get; set; }
        public string Brand { get; set; }
        
        [JsonPropertyName("img")]
        public string img { get; set; }
        public string Style { get; set; }
        public string Country { get; set; }
        public string Variety { get; set; }
        public int[] Ratings { get; set; }

        public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);

 
    }
}