using RamenRatings.WebSite.Pages;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace UnitTests.Pages.Product
{
    /// <summary>
    /// Unit testing for Index Tests
    /// </summary>
    public class IndexTests
    {
        // Database MiddleTier
        #region TestSetup
        public static IndexModel pageModel;
        private readonly ILogger<IndexModel> _logger;
        /// <summary>
        /// Initialize of Test
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            pageModel = new IndexModel(_logger, TestHelper.ProductService)
            {
            };
        }

        #endregion TestSetup
        /// <summary>
        /// Checking whether product user want is there in result or not.
        /// </summary>
        #region OnGet
        [Test]
        public void OnGet_Valid_Should_Return_Products()
        {
            // Arrange

            // Act
            pageModel.OnGet("chicken");

            // Access CurrentFilter getter to cover it
            var filter = pageModel.CurrentFilter;

            // Assert 
            // Ensure model is valid
            Assert.AreEqual(true, pageModel.ModelState.IsValid);

            // Are there any in existence?
            Assert.AreEqual(true, pageModel.Products.ToList().Any());

            // Check the filter has chicken
            Assert.AreEqual("chicken", filter);
        }

        /// <summary>
        /// Tests that nothing entered in search should still return products with the filter applied
        /// </summary>
        [Test]
        public void OnGet_Valid_No_Search_Should_Return_All_Products()
        {
            // Act
            pageModel.OnGet(null);//get null

            // establish the filter
            var filter = pageModel.CurrentFilter;

            // Assert
            Assert.IsTrue(pageModel.Products.Any());
            Assert.IsNull(filter);
        }

        #endregion OnGet
    }
}