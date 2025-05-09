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

namespace UnitTests.Pages.Product
{
    public class UpdateTests
    {
        #region TestSetup

        public static UpdateModel pageModel;

        [SetUp]
        public void TestInitialize()
        {
            // Initialize the page model with a mock ProductService
            pageModel = new UpdateModel(TestHelper.ProductService)
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
        public void OnGet_ValidId_Should_Return_Page()
        {
            // Arrange

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
        public void OnGet_InvalidId_Should_RedirectToError()
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

        #region OnPost

        /// <summary>
        /// Valid model should update the product and return PageResult
        /// </summary>
        [Test]
        public void OnPost_ValidModel_Should_RedirectToProductsPage()
        {
            // Arrange
            var sample = TestHelper.ProductService.GetProducts().First();
            pageModel.Product = new ProductModel
            {
                Number = sample.Number,
                Brand = sample.Brand,
                Style = sample.Style,
                Variety = "Updated Variety",
                Country = "Updated Country"
            };

            // Act
            var result = pageModel.OnPost() as PageResult;

            //Assert
            Assert.IsInstanceOf<Microsoft.AspNetCore.Mvc.RazorPages.PageResult>(result);
        }

        /// <summary>
        /// Invalid model state should return the page without saving
        /// </summary>
        [Test]
        public void OnPost_InvalidModel_Should_Return_Page()
        {
            // Arrange
            pageModel.ModelState.AddModelError("Brand", "Required");

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }

        #endregion OnPost

        #region UpdateData

        /// <summary>
        /// UpdateData should apply new style and brand if provided
        /// </summary>
        [Test]
        public void UpdateData_Should_Update_Fields_Correctly()
        {
            // Arrange
            var original = TestHelper.ProductService.GetProducts().First();
            pageModel.Product = new ProductModel
            {
                Number = original.Number,
                Brand = "Other",
                Style = "Other",
                Variety = "Spicy",
                Country = "Japan"
            };
            pageModel.NewBrand = "New Brand";
            pageModel.NewStyle = "Dry";

            // Act
            var updated = pageModel.UpdateData();

            // Assert
            Assert.AreEqual("New Brand", updated.Brand);
            Assert.AreEqual("Dry", updated.Style);
            Assert.AreEqual("Spicy", updated.Variety);
            Assert.AreEqual("Japan", updated.Country);
        }

        #endregion UpdateData

        #region SaveData

        /// <summary>
        /// SaveData should persist changes to JSON file
        /// </summary>
        [Test]
        public void SaveData_Should_Update_Json_File()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();
            product.Brand = "Test Brand";

            // Act
            pageModel.SaveData(product);

            // Reload and verify
            var reloaded = TestHelper.ProductService.GetProducts().First(p => p.Number == product.Number);
            Assert.AreEqual("Test Brand", reloaded.Brand);
        }

        #endregion SaveData
    }
}
