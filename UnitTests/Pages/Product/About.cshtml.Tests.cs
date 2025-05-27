using RamenRatings.WebSite.Pages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;

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
        #endregion OnGet
    }
}