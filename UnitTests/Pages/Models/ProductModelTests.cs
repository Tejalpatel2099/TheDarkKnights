using NUnit.Framework;
using RamenRatings.WebSite.Models;

namespace RamenRatings.Tests.Models
{
    public class ProductModelTests
    {
        /// <summary>
        /// Tests the ToString method of ProductModel to ensure it returns a valid JSON representation of the product data.
        /// </summary>
        [Test]
        public void ToString_Valid_Product_Data_Should_Return_Json_Representation()
        {
            // Arrange: Create a ProductModel instance with test data
            var product = new ProductModel
            {
                Number = 42,
                Brand = "Nissin",
                img = "image.png",
                Style = "Bowl",
                Country = "Japan",
                Variety = "Chicken",
                Vegetarian = "No",
                Ratings = new[] { 3, 4, 5 },
                Feedback = new[] { "Good", "Tasty" }
            };

            // Act: Call ToString() which should return a JSON-formatted string
            var json = product.ToString();

            // Assert: Verify the JSON string is not null
            Assert.IsNotNull(json);

            // Assert: Verify each property is correctly represented in the JSON output
            Assert.IsTrue(json.Contains("\"Number\":42"));
            Assert.IsTrue(json.Contains("\"Brand\":\"Nissin\""));
            Assert.IsTrue(json.Contains("\"img\":\"image.png\""));
            Assert.IsTrue(json.Contains("\"Style\":\"Bowl\""));
            Assert.IsTrue(json.Contains("\"Country\":\"Japan\""));
            Assert.IsTrue(json.Contains("\"Variety\":\"Chicken\""));
            Assert.IsTrue(json.Contains("\"Vegetarian\":\"No\""));
            Assert.IsTrue(json.Contains("\"Ratings\":[3,4,5]"));
            Assert.IsTrue(json.Contains("\"Feedback\":[\"Good\",\"Tasty\"]"));
        }
    }
}
