using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Pages
{
    /// <summary> 
    /// Lucian Petriuc
    /// Veda Ting
    /// Hema Sri
    /// Tejal Patel
    /// </summary> 
    public class IndexModel : PageModel
    {
        //cretes built in logging interface
        private readonly ILogger<IndexModel> _logger;

        //constructor for index model
        public IndexModel(ILogger<IndexModel> logger,
            JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }
        //Product Service
        public JsonFileProductService ProductService { get; }

        //Products 
        public IEnumerable<ProductModel> Products { get; private set; }

        public void OnGet()
        {
            // Get all of the products
            Products = ProductService.GetProducts();
        }
    }
}