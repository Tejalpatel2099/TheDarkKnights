using NUnit.Framework;
using System.Linq;

using RamenRatings.WebSite.Components;
using UnitTests.Pages;
using Castle.Components.DictionaryAdapter.Xml;
using Bunit;
using Bunit.Rendering;
using AngleSharp.Media.Dom;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics.Metrics;
using RamenRatings.WebSite.Services;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests.Components
{
    public class ProductsGridTests
    {
        #region TestSetup
        /// <summary>
        /// Test Setup
        /// </summary>
        [SetUp]
        public void Setup()
        {
        }
        #endregion TestSetup

        #region Get
        /// <summary>
        /// Creates a default data of ProductService
        /// Creates a new data and Tests if Equal
        /// </summary>
        [Test]
        public void Get_Valid_All_Data_Present_Should_Return_True()
        {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);
            var products = TestHelper.ProductService.GetProducts();
            var cut = ctx.RenderComponent<ProductGrid>();
            var rows = cut.FindAll("tbody");
            Assert.AreEqual(1, rows.Count);



        }
        #endregion Get

        #region Patch
        #endregion Patch
    }
}
