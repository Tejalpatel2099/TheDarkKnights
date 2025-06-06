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
    /// Unit testing for Compare Tests
    /// </summary>
    public class CompareTests
    {
        // Set up the tests
        #region TestSetup
        // Declare the model of the Compare page to be used in unit tests
        public static CompareModel pageModel;

        [SetUp]
        /// <summary>
        /// Initializes mock Compare page model for testing.
        /// </summary>
        public void TestInitialize()
        {
            // Initialize the page model with a mock ProductService
            pageModel = new CompareModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };
        }

        #endregion TestSetup

        /// <summary>
        /// Checking whether OnGet functions correctly. 
        /// </summary>
        #region OnGet

        /// Checks that ramen products are populated
        [Test]
        public void OnGet_Valid_Should_Populate_Products()
        {
            // Act - get the page
            pageModel.OnGet();

            // Assert - make sure the page returns products
            Assert.IsNotNull(pageModel.Products);
            Assert.IsTrue(pageModel.Products.Any());
        }

        #endregion OnGet

        /// <summary>
        /// Checking whether OnPost functions correctly. 
        /// </summary>
        #region OnPost Tests

        /// Checks that the selections for comparing two ramens are valid 
        [Test]
        public void OnPost_Valid_Selection_Should_Assign_Ramen1_And_Ramen2()
        {
            // Arrange
            // Create the list of products
            var products = TestHelper.ProductService.GetProducts().ToList();

            // Assign first as the first index
            var first = products[0];

            // If there are more than 2 products, assign the second as the 2nd index on the list
            var second = products[0];
            if (products.Count > 1)
            {
                second = products[1];
            }

            // Assign the selections
            pageModel.Selected1 = first.Number;
            pageModel.Selected2 = second.Number;

            // Act
            pageModel.OnPost();

            // Assert
            // Make sure the 2 ramen choices are assigned 
            Assert.IsNotNull(pageModel.Ramen1);
            Assert.IsNotNull(pageModel.Ramen2);

            // Check that the products match up with what was assigned
            Assert.AreEqual(first.Number, pageModel.Ramen1.Number);
            Assert.AreEqual(second.Number, pageModel.Ramen2.Number);
        }

        /// Checks that the selections for comparing two same ramens are invalid 
        [Test]
        public void OnPost_Valid_Same_Selection_Should_Not_Assign_Ramen1_And_Ramen2()
        {
            // Arrange
            // Create the list of products
            var products = TestHelper.ProductService.GetProducts().ToList();

            // Assign first as the first index of both ramens
            var first = products[0];
            var second = products[0];
            

            // Assign the selections
            pageModel.Selected1 = first.Number;
            pageModel.Selected2 = second.Number;

            // Act
            pageModel.OnPost();

            // Assert
            // Make sure the 2 ramen selected are null in the model
            Assert.IsNull(pageModel.Ramen1);
            Assert.IsNull(pageModel.Ramen2);
            Assert.AreEqual("Please select two different ramens.", pageModel.ErrorMessage);
        }
        #endregion OnPost Tests
    }
}
