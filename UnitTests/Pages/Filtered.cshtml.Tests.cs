using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Pages;
using RamenRatings.WebSite.Services;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace UnitTests.Pages
{
    /// <summary>
    /// Unit tests for the Filtered Page
    /// </summary>
    public class FilteredTests
    {
        #region TestSetup

        public static FilteredModel pageModel;
        private readonly ILogger<FilteredModel> _logger;

        /// <summary>
        /// Test setup initializes the FilteredModel with mock product service
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            pageModel = new FilteredModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext
            };
        }

        #endregion TestSetup

        #region OnGet

        /// <summary>
        /// Should load all brands and styles when no filters are applied
        /// </summary>
        [Test]
        public void OnGet_Valid_No_Filters_Should_Load__Brands_And_Styles()
        {
            // Arrange

            // Act
            pageModel.OnGet();

            // Assert - Brands and styles should be greater than 0
            Assert.IsTrue(pageModel.AllBrands.Count > 0);
            Assert.IsTrue(pageModel.AllStyles.Count > 0);
            Assert.IsTrue(pageModel.Products.Any());
        }

        /// <summary>
        /// Should return products filtered by brand
        /// </summary>
        [Test]
        public void OnGet_Valid_Filter_By_Brand_Should_Return_Expected()
        {
            // Arrange
            // Establish the first brand
            var brand = TestHelper.ProductService.GetProducts().First().Brand;
            pageModel.SelectedBrands = new List<string> { brand };

            // Act
            pageModel.OnGet();

            // Assert
            Assert.IsTrue(pageModel.Products.All(p => p.Brand == brand));
        }

        /// <summary>
        /// Should return products filtered by style
        /// </summary>
        [Test]
        public void OnGet_Valid_Filter_By_Style_Should_Return_Expected()
        {
            // Arrange
            // Establish first style
            var style = TestHelper.ProductService.GetProducts().First().Style;
            pageModel.SelectedStyles = new List<string> { style };

            // Act
            pageModel.OnGet();

            // Assert
            Assert.IsTrue(pageModel.Products.All(p => p.Style == style));
        }

        /// <summary>
        /// Should return products in specified rating range
        /// </summary>
        [Test]
        public void OnGet_Valid_Filter_By_Rating_Range_Should_Return_Expected()
        {
            var products = TestHelper.ProductService.GetProducts();

            // Assign specific ratings to test filtering by average
            foreach (var product in products)
            {
                product.Ratings = new[] { 3, 4, 5 }; // average = 4
                TestHelper.ProductService.UpdateProduct(product);
            }

            // query parameters to trigger filter logic
            TestHelper.HttpContextDefault.Request.QueryString = new QueryString("?MinRating=3&MaxRating=5");

            // Act
            pageModel.OnGet();

            // Assert - products should have products
            var firstProduct = pageModel.Products.First();
            var average = firstProduct.Ratings.Average();
            Assert.IsTrue(pageModel.Products.Any());


        }

        /// <summary>
        /// Should return products sorted by brand ascending
        /// </summary>
        [Test]
        public void OnGet_Valid_Sort_Brand_Ascending_Is_Correct()
        {
            // Arrange
            pageModel.SortOption = "BrandAsc";

            // Act
            pageModel.OnGet();

            // Assert 
            // Ensure the ordered list and sorting function are the same
            var sorted = pageModel.Products.OrderBy(p => p.Brand).Select(p => p.Brand);
            CollectionAssert.AreEqual(sorted, pageModel.Products.Select(p => p.Brand));
        }

        /// <summary>
        /// Should return products sorted by brand descending
        /// </summary>
        [Test]
        public void OnGet_Valid_Sort_By_Brand_Desc_Should_Sort_Correctly()
        {
            // Arrange
            pageModel.SortOption = "BrandDesc";

            // Act
            pageModel.OnGet();

            // Assert
            var expected = pageModel.Products.OrderByDescending(p => p.Brand).Select(p => p.Brand).ToList();
            CollectionAssert.AreEqual(expected, pageModel.Products.Select(p => p.Brand).ToList());
        }

        /// <summary>
        /// Should return products sorted by rating ascending
        /// </summary>
        [Test]
        public void OnGet_Valid_Sort_By_Rating_Asc_Should_Sort_Correctly()
        {
            // Arrange
            pageModel.SortOption = "RatingAsc";

            // Act
            pageModel.OnGet();

            // Assert
            var expected = pageModel.Products.OrderBy(p => p.Ratings.Average()).Select(p => p.Ratings.Average()).ToList();
            var actual = pageModel.Products.Select(p => p.Ratings.Average()).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Should return products sorted by rating descending
        /// </summary>
        [Test]
        public void OnGet_Valid_Sort_By_Rating_Desc_Should_Sort_Correctly()
        {
            // Arrange
            pageModel.SortOption = "RatingDesc";

            // Act
            pageModel.OnGet();

            // Assert
            var expected = pageModel.Products.OrderByDescending(p => p.Ratings.Average()).Select(p => p.Ratings.Average()).ToList();
            var actual = pageModel.Products.Select(p => p.Ratings.Average()).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Should return products sorted by number of ratings (high to low)
        /// </summary>
        [Test]
        public void OnGet_Valid_Sort_By_Rating_Num_High_Should_Sort_Correctly()
        {
            // Arrange
            pageModel.SortOption = "RatingNumHigh";

            // Act
            pageModel.OnGet();

            // Assert
            var expected = pageModel.Products.OrderByDescending(p => p.Ratings.Length).Select(p => p.Ratings.Length).ToList();
            var actual = pageModel.Products.Select(p => p.Ratings.Length).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Should return products sorted by number of ratings (low to high)
        /// </summary>
        [Test]
        public void OnGet_Valid_Sort_By_Low_Rating_Should_Sort_Correctly()
        {
            // Arrange
            pageModel.SortOption = "RatingNumLow";

            // Act
            pageModel.OnGet();

            // Assert
            var expected = pageModel.Products.OrderBy(p => p.Ratings.Length).Select(p => p.Ratings.Length).ToList();
            var actual = pageModel.Products.Select(p => p.Ratings.Length).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }


        #endregion OnGet
    }
}