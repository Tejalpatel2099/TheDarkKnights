using RamenRatings.WebSite.Pages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace UnitTests.Pages
{
    /// <summary>
    /// Unit tests for the About Razor PageModel
    /// </summary>
    public class AboutTests
    {
        #region TestSetup

        // pageModel used for testing
        public static AboutModel pageModel;

        /// <summary>
        /// Initializes the test by creating an instance of the AboutModel 
        /// with a mock logger and the real product service
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            var MockLoggerDirect = Mock.Of<ILogger<AboutModel>>();
            var productService = TestHelper.ProductService;


            pageModel = new AboutModel(MockLoggerDirect, productService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
            };
        }

        #endregion TestSetup

        #region OnGet
        /// <summary>
        /// Verifies that  OnGet initializes the page without model errors
        /// </summary>
        [Test]
        public void OnGet_Valid_Should_Return_Valid_Page()
        {
            // Act
            pageModel.OnGet();

            // Reset

            // Assert
            Assert.AreEqual(true, pageModel.ModelState.IsValid);
        }

        /// <summary>
        /// Verifies that OnGet populates Chart Data 
        /// containing Rating, Brand, Country, and Vegetarian keys.
        /// </summary>
        [Test]
        public void OnGet_Valid_Should_Populate_Chart_Data()
        {
            // Act
            pageModel.OnGet();

            // Assert
            Assert.IsTrue(pageModel.ModelState.IsValid);
            Assert.IsTrue(pageModel.ViewData.ContainsKey("ChartData")); // Ensure page model has ChartData

            var chartDataJson = pageModel.ViewData["ChartData"] as string; 
            Assert.IsNotNull(chartDataJson);// Ensure Json is not null through ChartData

            using var doc = JsonDocument.Parse(chartDataJson);
            var root = doc.RootElement;

            // Checks that the root JSON contains the Rating, Brand, Country, Vegetarian properties
            Assert.IsTrue(root.TryGetProperty("Rating", out _));
            Assert.IsTrue(root.TryGetProperty("Brand", out _));
            Assert.IsTrue(root.TryGetProperty("Country", out _));
            Assert.IsTrue(root.TryGetProperty("Vegetarian", out _));
        }
        #endregion OnGet
    }
}