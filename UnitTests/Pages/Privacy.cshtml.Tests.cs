using RamenRatings.WebSite.Pages;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

namespace UnitTests.Pages
{
    public class PrivacyTests
    {
        #region TestSetup
        public static PrivacyModel pageModel;
        private readonly ILogger<PrivacyModel> _logger;

        [SetUp]
        public void TestInitialize()
        {
            pageModel = new PrivacyModel(_logger)
            {
            };
        }

        #endregion TestSetup
        
        #region OnGet
        [Test]
        public void OnGet_Valid_Should_Return_Valid_Page()
        {
            // Arrange

            // Act
            pageModel.OnGet();

            // Assert
            Assert.AreEqual(true, pageModel.ModelState.IsValid);
        }
        #endregion OnGet
    }
}