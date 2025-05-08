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

        /// <summary>
        /// Set up before each test
        /// </summary>
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

        /// <summary>
        /// Valid product ID should return the Delete page and load the correct product
        /// </summary>
        [Test]
        public void OnGet_ValidId_Should_Return_Page()
        {
            var validProduct = TestHelper.ProductService.GetProducts().First();

            var result = pageModel.OnGet(validProduct.Number);

            Assert.IsInstanceOf<PageResult>(result); // should return a page
            Assert.IsNotNull(pageModel.Product); // product should be found
            Assert.AreEqual(validProduct.Number, pageModel.Product.Number); // correct product loaded
        }

        /// <summary>
        /// Invalid product ID should redirect to the error page
        /// </summary>
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

        /// <summary>
        /// Valid product should be deleted and then restored after test verification
        /// </summary>
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

        /// <summary>
        /// Invalid model state should result in returning the same page
        /// </summary>
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

        /// <summary>
        /// DeleteData should remove and return the deleted product
        /// </summary>
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

        /// <summary>
        /// DeleteData should return null for an invalid product ID
        /// </summary>
        [Test]
        public void DeleteData_InvalidId_Should_Return_Null()
        {
            var result = pageModel.DeleteData(-123); // invalid ID

            Assert.IsNull(result); // nothing should be deleted
        }

        #endregion
    }
}
