using NUnit.Framework;
using RamenRatings.WebSite.Pages.Product;
using RamenRatings.WebSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System;

namespace UnitTests.Pages.Product
{
    public class DeleteTests
    {
        // Shared instances of page models for reuse across test methods
        public static DeleteModel pageModel;
        public static CreateModel createModel;

        // Set up before each test
        [SetUp]
        public void TestInitialize()
        {
            // Initialize the DeleteModel with necessary test helpers and context
            pageModel = new DeleteModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };

            // Initialize the CreateModel for restoring deleted test data
            createModel = new CreateModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };
        }

        #region OnGet

        // Test that OnGet returns a PageResult and correctly loads a valid product
        [Test]
        public void OnGet_ValidId_Should_Return_Page()
        {
            var validProduct = TestHelper.ProductService.GetProducts().First();

            var result = pageModel.OnGet(validProduct.Number);

            Assert.IsInstanceOf<PageResult>(result); // should return a page
            Assert.IsNotNull(pageModel.Product); // product should be found
            Assert.AreEqual(validProduct.Number, pageModel.Product.Number); // correct product loaded
        }

        // Test that OnGet redirects to error page when given an invalid ID
        [Test]
        public void OnGet_InvalidId_Should_RedirectToError()
        {
            var result = pageModel.OnGet(-999); // invalid product ID

            Assert.IsInstanceOf<RedirectToPageResult>(result); // should redirect
            var redirect = result as RedirectToPageResult;
            Assert.AreEqual("../Error", redirect.PageName); // should redirect to error page
        }

        #endregion

        #region OnPost

        // Test that a valid post deletes the product and redirects, then restores the product
        [Test]
        public void OnPost_Valid_Should_Delete_Then_Restore()
        {
            // Arrange: get a valid product and prepare the page model
            var product = TestHelper.ProductService.GetProducts().First();
            pageModel.ModelState.Clear(); // ensure ModelState is valid
            pageModel.Product = product;

            // Act: attempt to delete the product
            var result = pageModel.OnPost();

            // Optional debug output for troubleshooting
            if (!pageModel.ModelState.IsValid)
            {
                var errors = string.Join("; ", pageModel.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                Console.WriteLine($"ModelState Errors: {errors}");
            }

            // Assert: verify deletion and redirection
            Assert.IsTrue(pageModel.ModelState.IsValid, "ModelState should be valid.");
            Assert.IsNotNull(pageModel.Product, "Product should not be null.");
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            Assert.AreEqual("/Index", ((RedirectToPageResult)result).PageName);

            // Assert: verify product is actually deleted
            var deleted = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == product.Number);
            Assert.IsNull(deleted, "Product should have been deleted.");

            // Restore the deleted product using CreateModel
            createModel.NewProduct = new ProductModel
            {
                Brand = product.Brand,
                Style = product.Style,
                Variety = product.Variety,
                Country = product.Country
            };
            createModel.NewBrand = product.Brand;
            createModel.NewStyle = product.Style;
            createModel.Rating = product.Ratings?.FirstOrDefault() ?? 3;
            createModel.Image = TestHelper.GetMockImageFile(product.Number);

            var restored = createModel.CreateData();

            // Assert: verify product restoration
            Assert.IsNotNull(restored);
            Assert.AreEqual(product.Brand, restored.Brand);
        }

        // Test that invalid model state results in returning the same page
        [Test]
        public void OnPost_InvalidModel_Should_Return_Page()
        {
            // Simulate model error
            pageModel.ModelState.AddModelError("Product", "Required");

            var result = pageModel.OnPost();

            Assert.IsInstanceOf<PageResult>(result); // should stay on the same page
        }

        #endregion

        #region DeleteData

        // Test that DeleteData successfully removes and returns a product
        [Test]
        public void DeleteData_Should_Remove_And_Return_Product()
        {
            var product = TestHelper.ProductService.GetProducts().First();

            // Act: delete the product
            var deleted = pageModel.DeleteData(product.Number);

            // Assert: deleted product matches
            Assert.IsNotNull(deleted);
            Assert.AreEqual(product.Number, deleted.Number);

            // Restore the product
            createModel.NewProduct = new ProductModel
            {
                Brand = product.Brand,
                Style = product.Style,
                Variety = product.Variety,
                Country = product.Country
            };
            createModel.NewBrand = product.Brand;
            createModel.NewStyle = product.Style;
            createModel.Rating = product.Ratings?.FirstOrDefault() ?? 3;
            createModel.Image = TestHelper.GetMockImageFile(product.Number);

            var restored = createModel.CreateData();
            Assert.IsNotNull(restored); // ensure it was restored
        }

        // Test that DeleteData returns null when given an invalid ID
        [Test]
        public void DeleteData_InvalidId_Should_Return_Null()
        {
            var result = pageModel.DeleteData(-123); // invalid ID

            Assert.IsNull(result); // nothing should be deleted
        }

        #endregion
    }
}
