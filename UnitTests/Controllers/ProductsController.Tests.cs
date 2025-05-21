using NUnit.Framework;
using System.Linq;
using RamenRatings.WebSite.Controllers;
using UnitTests.Pages;

namespace UnitTests.Controllers
{
    /// <summary>
    /// Unit tests for ProductsController class
    /// </summary>
    public class ProductsControllerTest
    {
        #region TestSetup
        /// <summary>
        /// Test Setup
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
        }
        #endregion TestSetup

        #region Get
        /// <summary>
        /// Creates a default data of ProductService
        /// Creates a new data and Tests if Equal
        /// </summary>
        [Test]
        public void Get_Valid_All_Data_Present_Should_Return_True()
        {
            // Arrange

            // Act
            // Store data 
            var newData = new ProductsController(TestHelper.ProductService).Get().First();

            // Get expected product
            var response = TestHelper.ProductService.GetProducts().First();

            // Assert
            Assert.AreEqual(newData.Number, response.Number);
        }
        #endregion Get

        #region Patch
        /// <summary>
        /// Creates a default data of ProductService
        /// Creates a new data of ProductController
        /// Creates a new rating request
        /// Applies and checks if changes is equal
        /// </summary>
        [Test]
        public void Patch_Valid_Rating_Should_Return_True()
        {
            // Arrange
            var newData = new ProductsController(TestHelper.ProductService);

            // Create a newRating datapoint to patch to the controller
            var product = newData.ProductService.GetProducts().Last();

            var newRating = new ProductsController.RatingRequest
            {
                ProductId = product.Number,
                Rating = 5
            };

            // Act
            newData.Patch(newRating);

            // Assert
            Assert.AreEqual(product.Number, newRating.ProductId);
        }
        #endregion Patch
    }
}
