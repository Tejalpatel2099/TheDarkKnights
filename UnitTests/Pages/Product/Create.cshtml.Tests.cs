using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Pages;
using RamenRatings.WebSite.Pages.Product;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Pages.Product
{
    /// <summary>
    /// Unit testing for Create Tests
    /// </summary>
    public class CreateTests
    {

        #region TestSetup
        // Declare the model of the Update page to be used in unit tests
        public static CreateModel pageModel;

        [SetUp]
        /// <summary>
        /// Initializes mock Update page model for testing.
        /// </summary>
        public void TestInitialize()
        {
            // Initialize the page model with a mock ProductService
            pageModel = new CreateModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };
        }

        #endregion TestSetup

        /// <summary>
        /// Checking whether OnGet functions correctly. 
        /// Checks for the dropdowns that are created and products are populated
        /// </summary>
        #region OnGet
        [Test]
        public void OnGet_Valid_Should_Populate_Brand_And_Style()
        {
            // Act
            pageModel.OnGet();

            // Assert 
            // Check that the dropdown options for Brands and Styles are populated with values
            Assert.IsNotNull(pageModel.ExistingBrands);
            Assert.IsNotNull(pageModel.ExistingStyles);
        }

        [Test]
        public void OnGet_Valid_Products_Should_Not_Return_Null()
        {
            // Act
            // get the products that are in the json
            var products = TestHelper.ProductService.GetProducts();

            // Assert
            // confirms there are products populated
            Assert.IsNotNull(products);
        }
        #endregion OnGet

        /// <summary>
        /// Checking whether a new product is created by the user
        /// </summary>
        #region CreateData
        [Test]
        public void CreateData_Valid_Should_Create_New_Product_With_Incremented_Number()
        {
            // Arrange
            // create the newly added Noodle Salt product
            var pageModel = new CreateModel(TestHelper.ProductService)
            {
                NewProduct = new ProductModel
                {
                    Brand = "Other",
                    Style = "Other",
                    Variety = "Salt",
                    Country = "USA"
                },
                NewBrand = "Noodle",
                NewStyle = "Snack",
                Rating = 5
            };

            // Add a test image
            var fileName = "test.jpg";
            var ms = new MemoryStream(new byte[] { 1, 2, 3 });
            var file = new FormFile(ms, 0, ms.Length, "Image", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            pageModel.Image = file;

            // Calculate the number of products before and now
            var productsBefore = TestHelper.ProductService.GetProducts().ToList();
            var expectedNumber = productsBefore.Max(p => p.Number) + 1;

            // Act
            // CreateData should add the new product
            var result = pageModel.CreateData();

            // Assert
            // Check that all attributes of the new product are what was expected
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedNumber, result.Number);
            Assert.AreEqual("Noodle", result.Brand);
            Assert.AreEqual("Snack", result.Style);
            Assert.AreEqual("Salt", result.Variety);
            Assert.AreEqual("USA", result.Country);
            Assert.AreEqual("/images/" + expectedNumber + ".jpg", result.img);
            Assert.AreEqual(5, result.Ratings[0]);
        }
        #endregion CreateData


        /// <summary>
        /// Checking whether a OnPost functions as expected where creating a new product will
        /// redirect to that product's read page
        /// </summary>
        #region OnPost

        [Test]
        public void OnPost_ValidModel_ShouldRedirectToReadPage()
        {
            // Arrange
            var pageModel = new CreateModel(TestHelper.ProductService)
            {
                NewProduct = new ProductModel
                {
                    Brand = "Other",
                    Style = "Other",
                    Variety = "Salt",
                    Country = "USA"
                },
                NewBrand = "Noodle",
                NewStyle = "Snack",
                Rating = 5
            };

            // Add a test image
            var ms = new MemoryStream(new byte[] { 1, 2, 3 });
            var file = new FormFile(ms, 0, ms.Length, "Image", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            pageModel.Image = file;

            // Act
            // invoke OnPost
            var result = pageModel.OnPost();

            // get the redirected page
            var redirect = result as RedirectToPageResult;

            // get the number from the redirected read page
            var readNumber = redirect.RouteValues["number"];

            // get the product number of the last product
            var latestProduct = TestHelper.ProductService.GetProducts().Max(p => p.Number);

            // Assert
            // Confirm that the page is redirected to a Read page
            Assert.AreEqual("/Product/Read", redirect.PageName);
            // Confirm that the read page number and the last product have the same number
            Assert.AreEqual(latestProduct, readNumber);

        }

        #endregion OnPost
    }
}
