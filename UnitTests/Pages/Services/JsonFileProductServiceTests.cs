using System.Linq;
using NUnit.Framework;
using RamenRatings.WebSite.Models;

namespace UnitTests.Pages.Services
{
    /// <summary>
    /// Unit tests for JsonFileProductService's AddRating method
    /// </summary>
    public class JsonFileProductServiceTests
    {
        #region TestSetup

        [SetUp]
        public void TestInitialize()
        {
            // Initialize or reset any mock/stub data if needed in the future
        }

        #endregion TestSetup

        #region AddRating

        /// <summary>
        /// POST a valid rating on a product with existing ratings
        /// Should return true
        /// </summary>
        [Test]
        public void AddRating_Valid_ProductId_With_Existing_Ratings_Should_Return_True()
        {
            // Act
            var result = TestHelper.ProductService.AddRating(8, 5);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// POST a valid rating on a product with null ratings
        /// Should return true
        /// </summary>
        [Test]
        public void AddRating_Valid_ProductId_With_Null_Ratings_Should_Return_True()
        {
            // Act
            var result = TestHelper.ProductService.AddRating(1234, 5);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// POST a valid rating on a product
        /// Should return true
        /// </summary>
        [Test]
        public void AddRating_Valid_ProductId_Should_Return_True()
        {
            // Act
            var result = TestHelper.ProductService.AddRating(30, 4);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// POST a rating on a product ID not present in the data
        /// Should return false
        /// </summary>
        [Test]
        public void AddRating_Invalid_ProductId_Should_Return_False()
        {
            // Act
            var result = TestHelper.ProductService.AddRating(9999, 5); // ID 9999 should not exist

            // Assert
            Assert.IsFalse(result);
        }

        #endregion AddRating
    }
}
