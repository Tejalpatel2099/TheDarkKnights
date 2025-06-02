using NUnit.Framework;
using RamenRatings.WebSite.Models;

namespace RamenRatings.Tests.Models
{
    public class ProductModelTests
    {
        [Test]
        public void ToString_Valid_Product_Should_Return_Json_Representation()
        {
            // Arrange
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

            // Act
            var json = product.ToString();

            // Assert
            Assert.IsNotNull(json);
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
