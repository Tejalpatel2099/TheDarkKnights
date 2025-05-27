using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RamenRatings.WebSite.Components;  // Adjust if your ProductList is in a different namespace
using RamenRatings.WebSite.Services;
using System.Linq;
using UnitTests.Pages;

namespace UnitTests.Components
{
    public class ProductListTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            // Register the JsonFileProductService from your test helper or a mock
            ctx.Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
        }

        [TearDown]
        public void TearDown()
        {
            ctx.Dispose();
        }

        [Test]
        public void ProductList_With_SearchString_Filters_Products()
        {
            // Arrange
            var partialSearch = "Chicken";  // Example search term

            // Act
            var cut = ctx.RenderComponent<ProductList>(
                parameters => parameters.Add(p => p.SearchString, partialSearch));

            // Assert
            var productTitles = cut.FindAll(".card-body > .card-title");
            Assert.IsNotEmpty(productTitles);
            Assert.IsTrue(productTitles.All(title => title.TextContent.Contains(partialSearch, System.StringComparison.OrdinalIgnoreCase)));
        }

        [Test]
        public void ProductList_Without_SearchString_Shows_All_Products()
        {
            // Arrange
            var allProducts = TestHelper.ProductService.GetProducts();

            // Act
            var cut = ctx.RenderComponent<ProductList>();

            // Assert
            var productTitles = cut.FindAll(".card-body > .card-title");
            Assert.AreEqual(allProducts.Count(), productTitles.Count);
        }

        // Optional: Additional tests for selecting a product or submitting a rating could be added here
    }
}