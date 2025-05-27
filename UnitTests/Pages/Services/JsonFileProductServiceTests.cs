
using System.Linq;
using RamenRatings.WebSite.Models;
using NUnit.Framework;


namespace UnitTests.Pages.Services
{
    public class JsonFileProductServiceTests
    {
        #region TestSetup

        [SetUp]
        public void TestInitialize()
        {
        }

        #endregion TestSetup

        #region AddRating
        /// <summary>
        /// REST Get Products data
        /// POST a valid rating
        /// Test that the data that was added was added correctly
        /// </summary>
        [Test]
        public void AddRating_Valid_ProductId_Return_true()
        {
            // Arrange


            // Act
            // Store the result of the AddRating method (which is being tested)
            bool validAdd = TestHelper.ProductService.AddRating(30, 4);

            // Assert
            Assert.AreEqual(true, validAdd);

        }

        /// <summary>
        /// REST POST data that doesn't fit the constraints defined in function
        /// Test if it Adds
        /// Returns False because it wont add
        /// </summary>
        [Test]
        public void AddRating_Invalid_Product_ID_Not_Present_Should_Return_False()
        {
            // Arrange

            // Act
            // Store the result of the AddRating method (which is being tested)
            var result = TestHelper.ProductService.AddRating(123, 5);

            // Assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// REST Get Products data
        /// POST a new valid rating on existing rated product
        /// Test that the data that was added was added correctly
        /// </summary>
        [Test]
        public void AddRating_Valid_ProductId_Adding_Rating_On_Existing_Product_Rating_Should_Return_true()
        {
            // Arrange


            // Act
            // Store the result of the AddRating method (which is being tested)
            bool validAdd = TestHelper.ProductService.AddRating(8, 5);

            // Assert
            Assert.AreEqual(true, validAdd);
        }

        /// <summary>
        /// POST a new valid rating on existing  product with null ratings
        /// Test that the data that was added was added correctly
        /// </summary>
        [Test]
        public void AddRating_Valid_ProductId_Adding_Rating_On_Existing_Product_Nul_Rating_Should_Return_true()
        {
            // Arrange
            // Act
            // Store the result of the AddRating method (which is being tested)
            bool validAdd = TestHelper.ProductService.AddRating(29, 5);

            // Assert

            Assert.AreEqual(true, validAdd);
        }

    }
    #endregion AddRating
}
