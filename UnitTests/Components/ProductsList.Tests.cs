using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RamenRatings.WebSite.Components;
using RamenRatings.WebSite.Services;
using System.Linq;
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

        /// <summary>
        /// Returns or filter the default product(s) if the search string is not given
        /// </summary>
        [Test]
        public void ProductList_Valid_Without_Any_Search_String_Shows_All_Products()
        {
            // Arrange
            var allProducts = TestHelper.ProductService.GetProducts();

            // Act
            var cut = ctx.RenderComponent<ProductList>();

            // Assert
            var productTitles = cut.FindAll(".card .card-body .card-title");
            Assert.AreEqual(allProducts.Count(), productTitles.Count);
        }
    }
}
