using NUnit.Framework;
using RamenRatings.WebSite.Pages.Product;
using RamenRatings.WebSite.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace UnitTests.Pages.Product
{
    /// <summary>
    /// Unit tests for the Update page model
    /// </summary>
    public class UpdateTests
    {
        #region TestSetup

        public static UpdateModel pageModel;

        /// <summary>
        /// Sets up the Tests with ProductService
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Initialize the page model with a mock ProductService
            pageModel = new UpdateModel(TestHelper.ProductService)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
                Url = TestHelper.UrlHelper
            };
        }

        #endregion TestSetup

        #region OnGet

        /// <summary>
        /// Valid product ID should return the product and page result
        /// </summary>
        [Test]
        public void OnGet_Valid_Id_Should_Return_Page()
        {
            // Arrange

            // Act
            var result = pageModel.OnGet(4);

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
            Assert.IsNotNull(pageModel.Product);


        }

        /// <summary>
        /// Checks that the VegetarianOptions have Veg and Not Veg options and they exist
        /// </summary>
        [Test]
        public void OnGet_Valid_Id_Should_Contain_Vegetarain()
        {
            // Arrange

            // Act
            var result = pageModel.OnGet(4);

            // Assert
            // verify VegetarianOptions are initialized properly
            var options = pageModel.VegetarianOptions;
            Assert.IsNotNull(options);
            Assert.Contains("Veg", options);
            Assert.Contains("Not Veg", options);
            Assert.AreEqual(2, options.Count);
        }

        [Test]
        public void OnGet_Valid_Id_Can_Set_Vegetarian()
        {
            // Act
            var result = pageModel.OnGet(4);

            // Assert original expectations
            Assert.IsNotNull(pageModel.Product);

            // --- Cover the setter ---
            var newOptions = new List<string> { "Yes", "No" };
            pageModel.VegetarianOptions = newOptions;

            // Confirm it was set correctly
            Assert.AreEqual(newOptions, pageModel.VegetarianOptions);
        }


        /// <summary>
        /// Invalid product ID should redirect to the error page
        /// </summary>
        [Test]
        public void OnGet_Valid_InvalidId_Should_Redirect_To_Error()
        {
            // Arrange
            int invalidId = 999;

            // Act
            var result = pageModel.OnGet(invalidId);

            // Assert
            Assert.IsInstanceOf<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.AreEqual("../Error", redirectResult.PageName);
        }

        #endregion OnGet

        #region OnPost

        /// <summary>
        /// Valid model should update the product and return PageResult
        /// </summary>
        [Test]
        public void OnPost_Valid_Model_Should_Redirect_To_Products_Page()
        {
            // Arrange
            // Arrange
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            var pageModel = new UpdateModel(TestHelper.ProductService)
            {
                Product = new ProductModel
                {
                    Number = original.Number,
                    Brand = "Other",
                    Style = "Other",
                    Variety = "Spicy",
                    Country = "Japan"
                },
                NewBrand = "NotNoodle",
                NewStyle = "NotSnack",
            };

            // Act
            var result = pageModel.OnPost();


            // get the redirected page
            var redirect = result as RedirectToPageResult;

            // Confirm that the page is redirected to a Read page
            Assert.AreEqual("/Product/ProductsPage", redirect.PageName);


        }

        /// <summary>
        /// Invalid model state should return the page without saving
        /// </summary>
        [Test]
        public void OnPost_Valid_Invalid_Model_Should_Return_Page()
        {
            // Arrange
            pageModel.ModelState.AddModelError("Brand", "Required");

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }

        /// <summary>
        /// Tests the validation for brand and style
        /// </summary>
        [Test]
        public void OnPost_Valid_Validation_Failure()
        {
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            var pageModel = new UpdateModel(TestHelper.ProductService)
            {
                Product = new ProductModel
                {
                    Number = original.Number,
                    Brand = "Other",
                    Style = "Other",
                    Variety = "Spicy",
                    Country = "Japan"
                },
                NewBrand = "Noodle",
                NewStyle = "Snack",
            };

            // Act
            var result = pageModel.OnPost();



            // Assert
            Assert.IsInstanceOf<PageResult>(result);
        }
        #endregion OnPost

        #region UpdateData

        /// <summary>
        /// UpdateData should apply new style and brand if provided
        /// </summary>
        [Test]
        public void UpdateData_Valid_Should_Update_Fields_Correctly()
        {
            // Arrange
            var original = TestHelper.ProductService.GetProducts().First();
            pageModel.Product = new ProductModel
            {
                Number = original.Number,
                Brand = "Other",
                Style = "Other",
                Variety = "Spicy",
                Country = "Japan",
                Vegetarian = "Veg"
            };
            pageModel.NewBrand = "New Brand";
            pageModel.NewStyle = "Dry";

            // Add a test image
            var fileName = "test.jpg";
            var ms = new MemoryStream(new byte[] { 1, 2, 3 });
            var file = new FormFile(ms, 0, ms.Length, "Image", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            pageModel.Image = file;

            // Act
            var updated = pageModel.UpdateData();

            // Assert
            Assert.IsNotNull(updated);
            Assert.AreEqual("New Brand", updated.Brand);
            Assert.AreEqual("Dry", updated.Style);
            Assert.AreEqual("Spicy", updated.Variety);
            Assert.AreEqual("Japan", updated.Country);
            Assert.AreEqual("Veg", updated.Vegetarian);
        }

        /// <summary>
        /// Tests UpdateData with products in the event that there could be null fields entered
        /// </summary>
        [Test]
        public void UpdateData_Valid_Default_Values_Should_Use_Original_For_Empty_Fields()
        {
            // Arrange - based on first product
            var original = TestHelper.ProductService.GetProducts().First();

            // initialize the product
            pageModel.Product = new ProductModel
            {
                Number = original.Number,
                Brand = original.Brand,
                Style = original.Style,
                Variety = null,
                Country = null,
                Vegetarian = original.Vegetarian
            };

            // Act
            var updated = pageModel.UpdateData();

            // Assert
            Assert.AreEqual(original.Brand, updated.Brand);
            Assert.AreEqual(original.Style, updated.Style);
            Assert.AreEqual(original.Variety, updated.Variety);
            Assert.AreEqual(original.Country, updated.Country);
            Assert.AreEqual(original.Vegetarian, updated.Vegetarian);
        }


        #endregion UpdateData

        #region SaveData

        /// <summary>
        /// SaveData should persist changes to JSON file
        /// </summary>
        [Test]
        public void SaveData_Valid_Should_Update_Json_File()
        {
            // Arrange
            var product = TestHelper.ProductService.GetProducts().First();
            product.Brand = "Test Brand";

            // Act
            pageModel.SaveData(product);

            // Reload and verify
            var reloaded = TestHelper.ProductService.GetProducts().First(p => p.Number == product.Number);
            Assert.AreEqual("Test Brand", reloaded.Brand);
        }

        #endregion SaveData

        #region ValidateData

        /// <summary>
        /// Validate Data should return false if data is valid
        /// </summary>
        [Test]
        public void ValidateData_Valid_All_Data_Is_Valid()
        {
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            var product = new ProductModel
            {
                Number = original.Number,
                Brand = "New Brand",
                Style = "Dry",
                Variety = "Spicy",
                Country = "Japan"
            };
            pageModel.ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            pageModel.ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            var validated = pageModel.ValidateData(product, true, true);

            Assert.IsTrue(validated);
            Assert.IsNull(pageModel.BrandError);
            Assert.IsNull(pageModel.StyleError);
            Assert.IsNull(pageModel.VarietyError);

        }

        /// <summary>
        /// ValidateData checks that the entry for Other Brand is not valid 
        /// </summary>
        [Test]
        public void ValidateData_Valid_Brand_Other_Not_Valid()
        {
            //Create product that is checked
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            var product = new ProductModel
            {
                Number = original.Number,
                Brand = "Mama",
                Style = "Dry",
                Variety = "Spicy",
                Country = "Japan"
            };
            pageModel.ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            pageModel.ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            //Validate
            var validated = pageModel.ValidateData(product, true, true);

            //Assert
            Assert.IsFalse(validated);
            Assert.AreEqual(pageModel.BrandError, "Brand already exists");
            Assert.IsNull(pageModel.StyleError);
            Assert.IsNull(pageModel.VarietyError);

        }

        /// <summary>
        /// Validate data checks that the character Brand error returns
        /// </summary>
        [Test]
        public void ValidateData_Valid_Brand_Characters_Not_Valid()
        {
            //Create Product that is checked
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            var product = new ProductModel
            {
                Number = original.Number,
                Brand = new string('B', 21),
                Style = "Dry",
                Variety = "Spicy",
                Country = "Japan"
            };
            pageModel.ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            pageModel.ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            //Validate
            var validated = pageModel.ValidateData(product, true, true);

            //Assert
            Assert.IsFalse(validated);
            Assert.AreEqual(pageModel.BrandError, "Character Limit is 20");
            Assert.IsNull(pageModel.StyleError);
            Assert.IsNull(pageModel.VarietyError);

        }

        /// <summary>
        /// ValidateData checks that the Other Style entered is not valid
        /// </summary>
        [Test]
        public void ValidateData_Valid_Style_Other_Not_Valid()
        {
            //Create product that is checked
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            var product = new ProductModel
            {
                Number = original.Number,
                Brand = "New Brand",
                Style = "Bowl",
                Variety = "Spicy",
                Country = "Japan"
            };
            pageModel.ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            pageModel.ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            //Validate
            var validated = pageModel.ValidateData(product, true, true);

            //Assert
            Assert.IsFalse(validated);
            Assert.AreEqual(pageModel.StyleError, "Style already exists");
            Assert.IsNull(pageModel.BrandError);
            Assert.IsNull(pageModel.VarietyError);

        }

        /// <summary>
        /// ValidateData checks that the style characters are not valid
        /// </summary>
        [Test]
        public void ValidateData_Valid_Style_Characters_Not_Valid()
        {
            //Takes original product
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            //creates new product
            var product = new ProductModel
            {
                Number = original.Number,
                Brand = original.Brand,
                Style = new string('S', 21),
                Variety = "Spicy",
                Country = "Japan"
            };
            pageModel.ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            pageModel.ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            //Checks if boolean validated
            var validated = pageModel.ValidateData(product, false, true);

            //Assert
            Assert.IsFalse(validated);
            Assert.AreEqual(pageModel.StyleError, "Character Limit is 20");
            Assert.IsNull(pageModel.BrandError);
            Assert.IsNull(pageModel.VarietyError);

        }

        /// <summary>
        /// ValidateData checks that the style variety is not a valid style
        /// </summary>
        [Test]
        public void ValidateData_Valid_Style_Variety_Not_Valid()
        {
            //Create product that is checked
            var products = TestHelper.ProductService.GetProducts();
            var original = products.First();
            var product = new ProductModel
            {
                Number = original.Number,
                Brand = original.Brand,
                Style = original.Style,
                Variety = new string('V', 21),
                Country = "Japan"
            };
            pageModel.ExistingBrands = products.Select(p => p.Brand).Distinct().ToList();
            pageModel.ExistingStyles = products.Select(p => p.Style).Distinct().ToList();

            //Validate
            var validated = pageModel.ValidateData(product, false, false);

            //Assert
            Assert.IsFalse(validated);
            Assert.AreEqual("Character Limit is 20", pageModel.VarietyError);


        }
        #endregion ValidateData

    }
}
