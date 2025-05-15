using RamenRatings.WebSite.Pages;
using NUnit.Framework;

namespace UnitTests.Pages
{
    public class AboutTests
    {
        #region TestSetup
        public static AboutModel pageModel;
        
        [SetUp]
        public void TestInitialize()
        {
            pageModel = new AboutModel()
            {
            };
        }

        #endregion TestSetup
        
        #region OnGet
        [Test]
        public void OnGet_Valid_Should_Return_Products()
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