using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RamenRatings.WebSite.Pages.Product
{
    // handles functions for products page where the user can browse through products
    public class ProductsPageModel : PageModel
    {
        private ILogger<ProductsPageModel> logger;

        // logger for products page
        public ProductsPageModel(ILogger<IndexModel> logger, JsonFileProductService productService)
        {
            this.logger = (ILogger<ProductsPageModel>)logger;
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }
        public IEnumerable<ProductModel> Products { get; private set; }

        // handles get requests when products page is accessed
        public void OnGet()
        {
            Products = ProductService.GetProducts();
        }
    }
}
