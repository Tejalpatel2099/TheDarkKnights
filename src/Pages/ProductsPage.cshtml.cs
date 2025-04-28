using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RamenRatings.WebSite.Pages
{
    public class ProductsPageModel : PageModel
    {
        private readonly ILogger<ProductsPageModel> _logger;

        public ProductsPageModel(ILogger<ProductsPageModel> logger,
            JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }
        public JsonFileProductService ProductService { get; }
        public IEnumerable<ProductModel> Products { get; private set; }

        public void OnGet()
        {
            Products = ProductService.GetProducts();
        }
    }
}
