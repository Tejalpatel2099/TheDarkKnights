using RamenRatings.WebSite.Pages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Pages
{
    public class AboutTests
    {
        #region TestSetup
        public static AboutModel pageModel;
        
        [SetUp]

        public void TestInitialize()
        {
            var MockLoggerDirect = Mock.Of<ILogger<AboutModel>>();
            pageModel = new AboutModel(MockLoggerDirect)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
            };
        }

        #endregion TestSetup
        
        #region OnGet
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