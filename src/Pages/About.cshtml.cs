using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RamenRatings.WebSite.Services;
using System.Linq;
using System.Text.Json;

namespace RamenRatings.WebSite.Pages
{
    public class AboutModel : PageModel
    {

        // Logger variable
        private readonly ILogger<AboutModel> _logger;

        // Reference to the service that loads product data from the JSON
        private readonly JsonFileProductService ProductService;

        // Constructor that accepts logger and ProductService as input
        public AboutModel(ILogger<AboutModel> logger, JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }

        // Handle get when the page is accessed - this will view counts of Ratings
        public void OnGet()
        {
            var products = ProductService.GetProducts();

            // Rating distribution (1 to 5)
            var ratingCounts = Enumerable.Range(1, 5)
                .Select(r => new { label = $"{r} Stars", count = products.Count(p => (p.Ratings?.FirstOrDefault() ?? 0) == r) })
                .ToList();

            // Brand distribution
            var brandCounts = products
                .GroupBy(p => p.Brand)
                .Select(g => new { label = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count) 
                .Take(20) // Limit to 20 brands
                .ToList();

            // Country distribution
            var countryCounts = products
                .GroupBy(p => p.Country)
                .Select(g => new { label = g.Key, count = g.Count() })
                .OrderByDescending(x => x.count)
                .ToList();

            // Vegetarian vs Non-Vegetarian
            var vegCounts = products
                .GroupBy(p => p.Vegetarian) // Group by Veg, No Veg
                .Select(g => new { label = g.Key, count = g.Count() })
                .ToList();

            ViewData["ChartData"] = JsonSerializer.Serialize(new
            {
                Rating = ratingCounts,
                Brand = brandCounts,
                Country = countryCounts,
                Vegetarian = vegCounts
            });
        }
    }
}
