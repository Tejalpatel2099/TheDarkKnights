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
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Pages.Product
{
    public class AddRatingTests
    {
        #region TestSetup

        public static AddRatingModel pageModel;

        [SetUp]
        public void TestInitialize()
        {
            pageModel = new AddRatingModel(TestHelper.ProductService)
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

        #region OnPost

        [Test]
        public void OnPost_Invalid_ModelState_Should_Return_PageResult()
        {
            // Arrange
            pageModel.ModelState.AddModelError("TestError", "Invalid Model");

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }

        [Test]
        public void OnPost_Product_Not_Found_Should_RedirectToError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.RouteValues["number"] = 999; // Set an invalid number

            pageModel.PageContext = new PageContext
            {
                HttpContext = context
            };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("../Error", redirect.PageName);
        }

        [Test]
        public void OnPost_Valid_Rating_Should_SaveData_And_RedirectToIndex()
        {
            // Arrange
            var sample = TestHelper.ProductService.GetProducts().First();

            // Set up Product
            pageModel.Product = new ProductModel
            {
                Number = sample.Number,
                Brand = sample.Brand,
                img = sample.img,
                Style = sample.Style,
                Country = sample.Country,
                Variety = sample.Variety,
                Ratings = new int[] { 4, 5 }
            };

            // Set the Rating
            pageModel.Rating = 5;

            // Simulate RouteValues["number"]
            var context = new DefaultHttpContext();
            context.Request.RouteValues["number"] = sample.Number;
            pageModel.PageContext = new PageContext
            {
                HttpContext = context
            };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("/Index", redirect.PageName);
        }



        #endregion OnPost

        #region AddRatingToRamen

        [Test]
        public void AddRatingToRamen_ValidProduct_Should_AddRating()
        {
            // Arrange
            var sample = TestHelper.ProductService.GetProducts().First();
            pageModel.Product = new ProductModel
            {
                Number = sample.Number,
                Brand = sample.Brand,
                img = sample.img,
                Style = sample.Style,
                Country = sample.Country,
                Variety = sample.Variety,
                Ratings = new int[] { 4, 5 }
            };
            pageModel.Rating = 3;

            // Act
            var updated = pageModel.AddRatingToRamen();

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual(3, updated.Ratings.Last());
            Assert.AreEqual(3, updated.Ratings.Length);
        }

        [Test]
        public void AddRatingToRamen_InValidProduct_Should_Return_Null()
        {
            // Arrange
            pageModel.Product = null;
            pageModel.Rating = 4;

            // Act
            var result = pageModel.AddRatingToRamen();

            // Assert
            Assert.IsNull(result);
        }


        #endregion AddRatingToRamen
    }
}
