using NUnit.Framework;
using RamenRatings.WebSite.Pages.Product;
using RamenRatings.WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace UnitTests.Pages.Product
{
    public class DeleteTests
    {
        #region TestSetup

        public static DeleteModel pageModel;

        [SetUp]
        public void TestInitialize()
        {
            pageModel = new DeleteModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };
        }

        #endregion TestSetup

        #region OnGet

        [Test]
        public void OnGet_ValidId_Should_Return_Page()
        {
            // Arrange
            var validProduct = TestHelper.ProductService.GetProducts().First();

            // Act
            var result = pageModel.OnGet(validProduct.Number);

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsNotNull(pageModel.Product);
            Assert.AreEqual(validProduct.Number, pageModel.Product.Number);
        }

        [Test]
        public void OnGet_InvalidId_Should_RedirectToError()
        {
            // Arrange
            int invalidId = -999;

            // Act
            var result = pageModel.OnGet(invalidId);

            // Assert
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("../Error", redirect.PageName);
        }

        #endregion OnGet

        #region OnPost

        [Test]
        public void OnPost_Valid_Should_Delete_And_Redirect()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();
            pageModel.Product = product;

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("/Index", redirect.PageName);

            // Confirm deletion
            var deleted = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == product.Number);
            Assert.IsNull(deleted);

            // Restore for test safety
            TestHelper.ProductService.AddProduct(product);
        }

        [Test]
        public void OnPost_InvalidModel_Should_Return_Page()
        {
            // Arrange
            pageModel.ModelState.AddModelError("Product", "Required");

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }

        #endregion OnPost

        #region DeleteData (Legacy support if still used)

        [Test]
        public void DeleteData_Should_Remove_Product_IfExists()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();
            var originalCount = TestHelper.ProductService.GetProducts().Count();

            // Act
            var deleted = pageModel.DeleteData(product.Number);

            // Assert
            Assert.AreEqual(product.Number, deleted.Number);
            var check = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == product.Number);
            Assert.IsNull(check);

            // Restore
            TestHelper.ProductService.AddProduct(product);
            Assert.AreEqual(originalCount, TestHelper.ProductService.GetProducts().Count());
        }

        [Test]
        public void DeleteData_InvalidId_Should_Return_Null()
        {
            // Arrange
            int invalidId = -123;

            // Act
            var result = pageModel.DeleteData(invalidId);

            // Assert
            Assert.IsNull(result);
        }

        #endregion DeleteData

        #region SaveData (if used)

        [Test]
        public void SaveData_Should_Persist_Deletion()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();
            var products = TestHelper.ProductService.GetProducts().Where(p => p.Number != product.Number);

            // Act
            pageModel.SaveData(products);

            // Assert
            var deleted = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == product.Number);
            Assert.IsNull(deleted);

            // Restore
            TestHelper.ProductService.AddProduct(product);
        }

        #endregion SaveData
    }
}
