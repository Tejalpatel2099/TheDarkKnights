using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Reflection.Metadata;

namespace RamenRatings.WebSite.Pages.Product
{
    /// <summary>
    /// handles functions for products page where the user can browse through products
    /// </summary>
    public class ProductsPageModel : PageModel
    {
        // Logger instance for tracking page events or issues
        private ILogger<ProductsPageModel> logger;

        // logger for products page
        public ProductsPageModel(ILogger<IndexModel> logger, JsonFileProductService productService)
        {
            this.logger = (ILogger<ProductsPageModel>)logger;
            ProductService = productService; //sets the passed productservice
        }

        // Constructs the Products page model with a logger and product service
        public JsonFileProductService ProductService { get; }

        // List of all products to be displayed on the Products page
        public IEnumerable<ProductModel> Products { get; private set; }

        // handles get requests when products page is accessed
        public void OnGet()
        {
            Products = ProductService.GetProducts(); //gets the products and assigns to Products
        }
    }
}
