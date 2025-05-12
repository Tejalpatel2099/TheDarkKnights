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
        /// Valid product ID should delete the product, redirect to index, and restore it successfully.
        /// Covers all branches including rating fallback and file upload.
        /// </summary>
        [Test]
        public void OnPost_Valid_Should_Delete_Then_Restore()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();

            // Force both null and non-null cases for ratings
            product.Ratings = new int[] { 4 }; // non-null rating

            pageModel.ModelState.Clear();
            pageModel.Product = product;

            // Act
            var result = pageModel.OnPost();

            // Assert deletion
            Assert.IsTrue(pageModel.ModelState.IsValid);
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            Assert.AreEqual("/Index", ((RedirectToPageResult)result).PageName);

            var deleted = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == product.Number);
            Assert.IsNull(deleted);

            // Restore with ratings
            createModel.NewProduct = new ProductModel
            {
                Brand = product.Brand,
                Style = product.Style,
                Variety = product.Variety,
                Country = product.Country
            };
            createModel.NewBrand = product.Brand;
            createModel.NewStyle = product.Style;

            // Handle rating assignment explicitly for full coverage
            createModel.Image = TestHelper.GetMockImageFile(product.Number);

            var restored = createModel.CreateData();

            // Assert: product restored correctly
            Assert.IsNotNull(restored);
            Assert.AreEqual(product.Brand, restored.Brand);
            Assert.IsTrue(restored.Ratings.Contains(createModel.Rating));
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
        /// Valid product ID should remove and return the product, test all branches including ratings fallback.
        /// </summary>
        [Test]
        public void DeleteData_Should_Remove_And_Return_Product()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();
            product.Ratings = null; // explicitly test null Ratings

            // Act
            var deleted = pageModel.DeleteData(product.Number);

            // Assert: Deleted product should match original
            Assert.IsNotNull(deleted);
            Assert.AreEqual(product.Number, deleted.Number);
            Assert.AreEqual(product.Brand, deleted.Brand);

            var check = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == product.Number);
            Assert.IsNull(check);

            // Restore
            createModel.NewProduct = new ProductModel
            {
                Brand = deleted.Brand,
                Style = deleted.Style,
                Variety = deleted.Variety,
                Country = deleted.Country
            };
            createModel.NewBrand = deleted.Brand;
            createModel.NewStyle = deleted.Style;

            // Test fallback to default rating
            createModel.Image = TestHelper.GetMockImageFile(deleted.Number);
            var restored = createModel.CreateData();

            // Final assertions
            Assert.IsNotNull(restored);
            Assert.AreEqual(deleted.Brand, restored.Brand);
            Assert.IsTrue(restored.Ratings.Contains(createModel.Rating));
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
