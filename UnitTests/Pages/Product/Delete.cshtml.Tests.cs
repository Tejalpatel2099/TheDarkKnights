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
		public static DeleteModel pageModel;
		public static CreateModel createModel;

		[SetUp]
		public void TestInitialize()
		{
			pageModel = new DeleteModel(TestHelper.ProductService)
			{
				PageContext = TestHelper.PageContext,
				TempData = TestHelper.TempData,
				Url = TestHelper.UrlHelper
			};

			createModel = new CreateModel(TestHelper.ProductService)
			{
				PageContext = TestHelper.PageContext,
				TempData = TestHelper.TempData,
				Url = TestHelper.UrlHelper
			};
		}

		#region OnGet

		[Test]
		public void OnGet_ValidId_Should_Return_Page()
		{
			var validProduct = TestHelper.ProductService.GetProducts().First();

			var result = pageModel.OnGet(validProduct.Number);

			Assert.IsInstanceOf<PageResult>(result);
			Assert.IsNotNull(pageModel.Product);
			Assert.AreEqual(validProduct.Number, pageModel.Product.Number);
		}

		[Test]
		public void OnGet_InvalidId_Should_RedirectToError()
		{
			var result = pageModel.OnGet(-999);

			Assert.IsInstanceOf<RedirectToPageResult>(result);
			var redirect = result as RedirectToPageResult;
			Assert.AreEqual("../Error", redirect.PageName);
		}

		#endregion

		#region OnPost

		[Test]
		public void OnPost_Valid_Should_Delete_Then_Restore()
		{
			// Arrange
			var product = TestHelper.ProductService.GetProducts().First();
			pageModel.ModelState.Clear(); // Ensure clean ModelState
			pageModel.Product = product;

			// Act: Delete
			var result = pageModel.OnPost();

			// Debug: Print any model errors (for troubleshooting)
			if (!pageModel.ModelState.IsValid)
			{
				var errors = string.Join("; ", pageModel.ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage));
				Console.WriteLine($"ModelState Errors: {errors}");
			}

			// Assert: Ensure ModelState is valid and redirect occurred
			Assert.IsTrue(pageModel.ModelState.IsValid, "ModelState should be valid.");
			Assert.IsNotNull(pageModel.Product, "Product should not be null.");
			Assert.IsInstanceOf<RedirectToPageResult>(result);
			Assert.AreEqual("/Index", ((RedirectToPageResult)result).PageName);

			// Assert: Product was deleted
			var deleted = TestHelper.ProductService.GetProducts().FirstOrDefault(p => p.Number == product.Number);
			Assert.IsNull(deleted, "Product should have been deleted.");

			// Restore: Create new product using CreateModel
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

			// Assert: New product was added
			Assert.IsNotNull(restored);
			Assert.AreEqual(product.Brand, restored.Brand);
		}

		[Test]
		public void OnPost_InvalidModel_Should_Return_Page()
		{
			pageModel.ModelState.AddModelError("Product", "Required");

			var result = pageModel.OnPost();

			Assert.IsInstanceOf<PageResult>(result);
		}

		#endregion

		#region DeleteData

		[Test]
		public void DeleteData_Should_Remove_And_Return_Product()
		{
			var product = TestHelper.ProductService.GetProducts().First();
			var deleted = pageModel.DeleteData(product.Number);

			Assert.IsNotNull(deleted);
			Assert.AreEqual(product.Number, deleted.Number);

			// Restore
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
			Assert.IsNotNull(restored);
		}

		[Test]
		public void DeleteData_InvalidId_Should_Return_Null()
		{
			var result = pageModel.DeleteData(-123);

			Assert.IsNull(result);
		}

		#endregion
	}
}
