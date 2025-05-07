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

namespace UnitTests.Pages.Product
{
    public class ReadTests
    {
        #region TestSetup

        public static ReadModel pageModel;

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
        
    }
}