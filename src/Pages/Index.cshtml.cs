using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Pages
{
    /// <summary> 
    /// The IndexModel class handles the logic for the Index Razor Page,
    /// including loading and filtering the ramen products based on search criteria.
    /// </summary> 
    public class IndexModel : PageModel
    {
        // Logger is read only; for debugging and dianostics
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Constructor that injects the logger and product service.
        /// </summary>
        public IndexModel(ILogger<IndexModel> logger,
            JsonFileProductService productService)
        {
            _logger = logger;
            ProductService = productService;
        }

        /// <summary>
        /// Service to access and filter ramen product data from the JSON file
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// The list of ramen products to be displayed on the Index page
        /// </summary>
        public IEnumerable<ProductModel> Products { get; private set; }

        /// <summary>
        /// Store the total number of brands 
        /// </summary>
        public int TotalBrands { get; private set; }

        /// <summary>
        /// Store the total number of products 
        /// </summary>
        public int TotalProducts { get; private set; }

        /// <summary>
        /// Store the total number of ratings 
        /// </summary>
        public int TotalRatings { get; private set; }

        /// <summary>
        /// Store the highest rated ramen
        /// </summary>
        public string HighestRatedRamen { get; private set; }

        /// <summary>
        /// Store the current search term to show in the search input
        /// </summary>
        public string CurrentFilter { get; private set; }

        /// <summary>
        /// Accept the search string parameter from the query string
        /// </summary>
        public void OnGet(string SearchString)
        {
            // Set the current filter 
            CurrentFilter = SearchString;

            // Set the products
            var products = ProductService.GetProducts();

            // Filter by products based on the search string
            if (!string.IsNullOrEmpty(SearchString))
            {
                // Filter products by variety (case insensitive)
                products = products.Where(p =>
                    p.Variety.Contains(SearchString, System.StringComparison.OrdinalIgnoreCase));
            }

            Products = products;

            // Set the TotalBrand value based on product count of brands
            TotalBrands = Products.Select(p => p.Brand).Distinct().Count();

            // Sets the TotalProducts value based on number in JSON
            TotalProducts = Products.Count();

            // Sums the total number of ratings that the products have
            TotalRatings = Products.Sum(p => p.Ratings.Length);

            // Find the maximum average rating among all products with ratings
            double maxAverageRating = Products
                .Max(p => p.Ratings.Average());

            // Get all products with that maximum average rating
            var topRatedProducts = Products
                .Where(p => Math.Abs(p.Ratings.Average() - maxAverageRating) < 0.0001)
                .ToList();


            // Store the highest rated product variety in highest rated ramen 
            HighestRatedRamen = string.Join(", ", topRatedProducts.Select(p => p.Variety));

        }
    }
}
