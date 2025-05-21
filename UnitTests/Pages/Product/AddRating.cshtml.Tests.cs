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
    /// <summary>
    /// Unit testing for Add Ratings Model
    /// </summary>
    public class AddRatingTests
    {
        #region TestSetup
        // Declare the model of the Add Rating page to be used in unit tests
        public static AddRatingModel pageModel;


        [SetUp]
        /// <summary>
        /// Initializes mock Add Rating model for testing.
        /// </summary>
        public void TestInitialize()
        {
            // Initialize the page model with a mock ProductService
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
        /// <summary>
        /// Valid product ID should return the product and page result
        /// </summary>
        public void OnGet_Valid_Id_Should_Return_Page()
        {
            // Arrange

            // Act
            var result = pageModel.OnGet(4);

            // Assert
            //Assert.IsInstanceOf<PageResult>(result);
            Assert.IsNotNull(pageModel.Product);
        }

        [Test]
        /// <summary>
        /// InValid product ID should return the error page
        /// </summary>
        public void OnGet_Valid_Invalid_Id_Should_Redirect_To_Error()
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
        /// <summary>
        /// Invalid ModelState should return the page result
        /// </summary>
        public void OnPost_Valid_Invalid_ModelState_Should_Return_Page_Result()
        {
            // Arrange
            pageModel.ModelState.AddModelError("TestError", "Invalid Model");

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }

        [Test]
        /// <summary>
        /// Invalid product ID should redirect to the error page
        /// </summary>
        public void OnPost_Valid_Product_Not_Found_Should_Redirect_To_Error()
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
        /// <summary>
        /// Valid rating should save data and redirect to the index page
        /// </summary>
        public void OnPost_Valid_Rating_Should_SaveData_And_Redirect_To_Index()
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
        /// <summary>
        /// Valid product without feedback should add the rating
        /// </summary>
        public void AddRatingToRamen_Valid_Product_Without_Feedback_Should_Add_Rating()
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
                Ratings = new int[] { 4, 5 },
                Feedback = null
            };
            pageModel.Rating = 3;
            pageModel.Feedback = null;

            // Act
            var updated = pageModel.AddRatingToRamen();

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual(3, updated.Ratings.Last());
            Assert.AreEqual(3, updated.Ratings.Length);
        }

        [Test]
        /// <summary>
        /// Valid product with feedback should add the rating
        /// </summary>
        public void AddRatingToRamen_Valid_Feedback_Should_Add_Feedback()
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
                Ratings = new int[] { 5 },
                Feedback = new string[] { "Tasty!" }
            };

            pageModel.Rating = 4;
            pageModel.Feedback = "Loved the spice!";

            // Act
            var updated = pageModel.AddRatingToRamen();

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual(2, updated.Feedback.Length);
            Assert.AreEqual("Loved the spice!", updated.Feedback.Last());
        }

        [Test]
        /// <summary>
        /// Valid product with null feedback should add the rating
        /// </summary>
        public void AddRatingToRamen_Valid_Product_With_No_Feedback_Should_Add_Feedback()
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
                Ratings = new int[] { 5 },
                Feedback = null
            };

            pageModel.Rating = 4;
            pageModel.Feedback = "Loved the spice!";

            // Act
            var updated = pageModel.AddRatingToRamen();

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual(1, updated.Feedback.Length);
            Assert.AreEqual("Loved the spice!", updated.Feedback.Last());
        }


        [Test]
        /// <summary>
        /// Invalid product should return null
        /// </summary>
        public void AddRatingToRamen_Valid_InValidProduct_Should_Return_Null()
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
