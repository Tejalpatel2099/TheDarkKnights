using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Pages
{
    /// <summary>
    /// The FilteredModel class displays and filters a list of ramen products
    /// based on style, brand, and rating range selected by the user. It loads all filter options
    /// from the ramen.json and applies the filters 
    /// </summary>
    public class FilteredModel : PageModel
    {
        // Reference to the service that loads product data from the JSON
        private readonly JsonFileProductService ProductService;

        // Service to fetch product data
        public FilteredModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // The list of products to display on the page after filtering
        public IEnumerable<ProductModel> Products { get; set; }

        // All available brands, used to populate the brand filter dropdown
        public List<string> AllBrands { get; set; }
        // All available ramen styles, used to populate the style filter dropdown
        public List<string> AllStyles { get; set; }

        // The style chosen by the user to filter products
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedStyles { get; set; }

        // The brand chosen the the user to filter products
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedBrands { get; set; }

        // The selected sort option from the dropdown
        [BindProperty(SupportsGet = true)]
        public string SortOption { get; set; }

        // Handles GET requests for the page. Loads filter options and applies selected filters
        public void OnGet()
        {
            // Loads all products from data service
            var products = ProductService.GetProducts();

            // Creates the Brand list for the filter
            AllBrands = products.Select(p => p.Brand).Where(b => !string.IsNullOrEmpty(b)).Distinct().OrderBy(b => b).ToList();
            // Creates the style list for the filter
            AllStyles = products.Select(p => p.Style).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList();

            // Apply the Brand filter if they are selected
            if (SelectedBrands?.Any() == true)
                products = products.Where(p => SelectedBrands.Contains(p.Brand));

            // Apply the Style filter if they are selected
            if (SelectedStyles?.Any() == true)
                products = products.Where(p => SelectedStyles.Contains(p.Style));

            // Sets the default rating range
            double min = 1.0;
            double max = 5.0;

            // Parse the rating range from query
            if (double.TryParse(Request.Query["MinRating"], out var parsedMin))
                min = parsedMin;

            if (double.TryParse(Request.Query["MaxRating"], out var parsedMax))
                max = parsedMax;

            // Apply filter for rating range
            products = products.Where(p =>
            {
                if (p.Ratings == null || p.Ratings.Length == 0)
                    return false;

                var avg = p.Ratings.Average();
                return avg >= min && avg <= max;
            });

            // Apply filter for rating range
            Products = products.ToList();

            // Apply sorting based on selected option
            Products = SortOption switch
            {
                "BrandAsc" => Products.OrderBy(p => p.Brand).ToList(),
                "BrandDesc" => Products.OrderByDescending(p => p.Brand).ToList(),
                "RatingAsc" => Products.OrderBy(p => p.Ratings.Average()).ToList(),
                "RatingDesc" => Products.OrderByDescending(p => p.Ratings.Average()).ToList(),
                "RatingNumHigh" => Products.OrderByDescending(p => p.Ratings.Length).ToList(),
                "RatingNumLow" => Products.OrderBy(p => p.Ratings.Length).ToList(),
                _ => Products
            };
        }
    }
}
