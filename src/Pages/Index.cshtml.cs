using System.Collections.Generic;
using System.Linq;
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
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger,
            JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }

        public IEnumerable<ProductModel> Products { get; private set; }

        // Store the current search term to show in the search input
        public string CurrentFilter { get; private set; }

        // Accept the search string parameter from the query string
        public void OnGet(string SearchString)
        {
            CurrentFilter = SearchString;

            var products = ProductService.GetProducts();

            if (!string.IsNullOrEmpty(SearchString))
            {
                // Filter products by variety (case insensitive)
                products = products.Where(p =>
                    !string.IsNullOrEmpty(p.Variety) &&
                    p.Variety.Contains(SearchString, System.StringComparison.OrdinalIgnoreCase));
            }

            Products = products;
        }
    }
}
