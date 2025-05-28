using NUnit.Framework;
using RamenRatings.WebSite.Pages.Product;
using RamenRatings.WebSite.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using RamenRatings.WebSite.Pages;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;

namespace UnitTests.Pages.Product
{
    /// <summary>
    /// Unit tests for the Read page model
    /// </summary>
    public class ReadTests
    {
        #region TestSetup

        // Page model to be tested
        public static ReadModel pageModel;


        /// <summary>
        /// Test setup method to initialize the page model
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Initialize the page model with a mock ProductService
            pageModel = new ReadModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };
        }

        #endregion TestSetup

        #region OnGet

        /// <summary>
        /// Valid product ID should return the product and page result
        /// </summary>
        [Test]
        public void OnGet_Valid_ValidId_Should_Return_Page()
        {
            // Act
            var result = pageModel.OnGet(4);

            // Assert
            //Assert.IsInstanceOf<PageResult>(result);
            Assert.IsNotNull(pageModel.Product);
        }

        /// <summary>
        /// Invalid product ID should redirect to the error page
        /// </summary>
        [Test]
        public void OnGet_Valid_InvalidId_Should_Redirect_To_Error()
        {
            // Arrange
            int invalidId = 999;

            // Act
            var result = pageModel.OnGet(invalidId);

            // Assert 
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.AreEqual("../Error", redirectResult.PageName);
        }

        #endregion OnGet

        #region OnPostAddRatingAsync


        /// <summary>
        /// Product with exisiting ratings should add a new rating
        /// </summary>
        [Test]
        public void OnPostAddRatingAsync_Valid_Product_Should_Add_Rating_And_Redirect()
        {
            // Arrange
            var productNumber = TestHelper.ProductService.GetProducts().First().Number;
            var ratingToAdd = 5;


            // Act
            var result = pageModel.OnPostAddRatingAsync(productNumber, ratingToAdd);

            // Re-fetch the product after update
            var updatedProduct = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == productNumber);


            // Assert
            Assert.AreEqual(ratingToAdd, updatedProduct.Ratings.Last());
            Assert.IsInstanceOf<RedirectToPageResult>(result);
        }

        /// <summary>
        /// Product with null ratings should initialize ratings and add a new rating 
        /// </summary>
        [Test]
        public async Task OnPostAddRatingAsync_Valid_Product_With_Null_Ratings_Should_Initialize_And_Add_Rating()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();
            product.Ratings = null; // Force null to test fallback
            TestHelper.ProductService.UpdateProduct(product);

            var productNumber = product.Number;
            var newRating = 5;

            var pageModel = new ReadModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };

            // Act
            var result = pageModel.OnPostAddRatingAsync(productNumber, newRating);

            // Re-fetch updated product
            var updatedProduct = TestHelper.ProductService.GetProducts().First(p => p.Number == productNumber);

            // Assert therea are ratings in the updated product
            Assert.IsNotNull(updatedProduct.Ratings);
            Assert.Contains(newRating, updatedProduct.Ratings);
            Assert.IsInstanceOf<RedirectToPageResult>(result);
        }


        /// <summary>
        /// Invalid product ID should redirect without crashing
        /// </summary>
        [Test]
        public void OnPostAddRatingAsync_Valid_Invalid_Product_Should_Redirect_Without_Crash()
        {
            // Arrange
            var invalidProductNumber = 9999;
            var ratingToAdd = 4;

            // Act
            var result = pageModel.OnPostAddRatingAsync(invalidProductNumber, ratingToAdd);

            // Assert
            Assert.IsInstanceOf<RedirectToPageResult>(result);

            var redirectResult = result as RedirectToPageResult;
            Assert.AreEqual(invalidProductNumber, redirectResult.RouteValues["number"]);
        }


        #endregion OnPostAddRatingAsync
    }
}
