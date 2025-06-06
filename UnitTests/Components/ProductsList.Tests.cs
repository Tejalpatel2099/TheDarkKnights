using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RamenRatings.WebSite.Components;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using System;
using System.Linq;
using System.Reflection;
using UnitTests.Pages;

namespace UnitTests.Components
{
    /// <summary>
    /// Tests for Product Lists component
    /// </summary>
    public class ProductListTests
    {
        private Bunit.TestContext ctx;

        /// <summary>
        /// Setting up the context for the test
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            // Register the JsonFileProductService from your test helper or a mock
            ctx.Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
        }

        /// <summary>
        /// Clean up the resources after disposing the context
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();
        }

        #region ProductList
        /// <summary>
        /// Returns or filter the product(s) if the search string is given
        /// </summary>
        [Test]
        public void ProductList_Valid_Search_String_Filters_Products()
        {
            // Arrange
            var partialSearch = "Chicken";  // Example search term

            // Act
            var cut = ctx.RenderComponent<ProductList>(
                parameters => parameters.Add(p => p.SearchString, partialSearch));

            // Assert
            var productTitles = cut.FindAll(".card .card-body .card-title");
            Assert.IsNotEmpty(productTitles);
            Assert.IsTrue(productTitles.All(title => title.TextContent.Contains(partialSearch, System.StringComparison.OrdinalIgnoreCase)));
        }

        #endregion ProductList

        #region SelectProduct
        /// <summary>
        /// Verifies SelectProduct updates the internal selectedProduct field.
        /// </summary>
        [Test]
        public void SelectProduct_Valid_Should_Update_SelectedProduct()
        {
            // Arrange
            var cut = ctx.RenderComponent<ProductList>();
            var instance = cut.Instance;
            var firstProduct = TestHelper.ProductService.GetProducts().First();

            // Act
            var selectMethod = instance.GetType().GetMethod("SelectProduct", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            selectMethod.Invoke(instance, new object[] { firstProduct.Number });

            // Assert
            var selectedProductField = instance.GetType().GetField("selectedProduct", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var selectedProductValue = selectedProductField.GetValue(instance) as ProductModel;

            Assert.IsNotNull(selectedProductValue);
            Assert.AreEqual(firstProduct.Number, selectedProductValue.Number);
        }

        #endregion SelectProduct

        #region GetCurrentRating
        /// <summary>
        /// Verifies GetCurrentRating correctly calculates average and vote count.
        /// </summary>
        [Test]
        public void GetCurrentRating_Valid_Should_Set_Correct_Values()
        {
            // Arrange
            var cut = ctx.RenderComponent<ProductList>();
            var instance = cut.Instance;
            var product = TestHelper.ProductService.GetProducts().First(x => x.Ratings != null && x.Ratings.Length > 0);

            // Act
            var selectMethod = instance.GetType().GetMethod("SelectProduct", BindingFlags.NonPublic | BindingFlags.Instance);
            selectMethod.Invoke(instance, new object[] { product.Number });

            // Assert
            var ratingField = instance.GetType().GetField("currentRating", BindingFlags.NonPublic | BindingFlags.Instance);
            var voteCountField = instance.GetType().GetField("voteCount", BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(product.Ratings.Sum() / product.Ratings.Length, (int)ratingField.GetValue(instance));
            Assert.AreEqual(product.Ratings.Length, (int)voteCountField.GetValue(instance));
        }

        #endregion GetCurrentRating

        #region SubmitRating
        /// <summary>
        /// Ensures SubmitRating adds a rating and refreshes the selected product.
        /// </summary>
        [Test]
        public void SubmitRating_Valid_Rating_Updates_SelectedProduct()
        {
            // Arrange
            var cut = ctx.RenderComponent<ProductList>();
            var instance = cut.Instance;
            var product = TestHelper.ProductService.GetProducts().First(p => p.Ratings != null && p.Ratings.Length > 0);

            //Act
            // Use reflection to call SelectProduct and set state
            var selectMethod = instance.GetType().GetMethod("SelectProduct", BindingFlags.NonPublic | BindingFlags.Instance);
            selectMethod.Invoke(instance, new object[] { product.Number });

            // Use reflection to call SubmitRating with a valid rating
            var submitMethod = instance.GetType().GetMethod("SubmitRating", BindingFlags.NonPublic | BindingFlags.Instance);
            submitMethod.Invoke(instance, new object[] { 4 });

            // Assert selectedProduct still matches expected
            var selectedProductField = instance.GetType().GetField("selectedProduct", BindingFlags.NonPublic | BindingFlags.Instance);
            var selectedProductValue = selectedProductField.GetValue(instance) as ProductModel;

            Assert.IsNotNull(selectedProductValue);
            Assert.AreEqual(product.Number, selectedProductValue.Number);
        }

        #endregion SubmitRating

        #region OnParametersSet

        /// <summary>
        /// Returns or filter the default product(s) if the search string is not given
        /// </summary>
        [Test]
        public void OnParametersSet_Valid_Without_Any_Search_String_Shows_All_Products()
        {
            // Arrange
            var allProducts = TestHelper.ProductService.GetProducts();

            // Act
            var cut = ctx.RenderComponent<ProductList>();

            // Assert
            var productTitles = cut.FindAll(".card .card-body .card-title");
            Assert.AreEqual(allProducts.Count(), productTitles.Count);
        }


        /// <summary>
        /// Tests that the ProductList component correctly filters products
        /// when a valid SearchString is provided that matches the Variety field.
        /// </summary>
        [Test]
        public void OnParametersSet_Valid_Match_Variety_Should_Render_Filtered_Product()
        {
            // Arrange
            var searchTerm = "Chicken"; // Must exist in one of the product varieties
            var cut = ctx.RenderComponent<ProductList>(
                parameters => parameters.Add(p => p.SearchString, searchTerm));

            // Act
            var titles = cut.FindAll(".card-title");

            // Assert
            Assert.IsNotEmpty(titles, "Expected filtered results to be displayed");
            Assert.IsTrue(titles.All(t =>
                t.TextContent.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)),
                "Each title should contain the search term");
        }

        #endregion OnParametersSet

        #region BuildRenderTree
        /// <summary>
        /// Test rendering stars for the first real product from JSON.
        /// </summary>
        [Test]
        public void StarRating_From_Real_First_Product_Should_Render_Correct_Stars()
        {
            // Arrange: Load the actual first product
            var realProduct = TestHelper.ProductService.GetProducts().FirstOrDefault();

            // Set star values for average, expected, half, and empty
            double avg = realProduct.Ratings.Average();
            int expectedFull = (int)System.Math.Floor(avg);
            bool hasHalf = (avg - expectedFull) >= 0.5;
            int expectedHalf = hasHalf ? 1 : 0;
            int expectedEmpty = 5 - expectedFull - expectedHalf;

            // Act
            var cut = ctx.RenderComponent<ProductList>();
            var productCard = cut.FindAll(".card").First();

            var fullStars = productCard.GetElementsByClassName("fas fa-star").Length;
            var halfStars = productCard.GetElementsByClassName("fas fa-star-half-alt").Length;
            var emptyStars = productCard.GetElementsByClassName("far fa-star").Length;

            // Assert
            Assert.AreEqual(expectedFull, fullStars);
            Assert.AreEqual(expectedHalf, halfStars);
            Assert.AreEqual(expectedEmpty, emptyStars);
        }
        #endregion BuildRenderTree
    }
}
