using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
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
        public string SelectedStyle { get; set; }

        // The brand chosen the the user to filter products
        [BindProperty(SupportsGet = true)]
        public List<string> SelectedBrands { get; set; }

        // Handles GET requests for the page. Loads filter options and applies selected filters
        public void OnGet()
        {
            // Loads all products from data service
            var products = ProductService.GetProducts();

            // Creates the Brand list for the filter
            AllBrands = products.Select(p => p.Brand).Where(b => (string.IsNullOrEmpty(b)) == false).Distinct().OrderBy(b => b).ToList();
            // Creates the style list for the filter
            AllStyles = products.Select(p => p.Style).Where(s => (string.IsNullOrEmpty(s)) == false).Distinct().OrderBy(s => s).ToList();

            // Apply the Brand filter if they are selected
            if (SelectedBrands?.Any() == true)
                products = products.Where(p => SelectedBrands.Contains(p.Brand));

            // Apply the Style filter if they are selected
            if (string.IsNullOrEmpty(SelectedStyle) == false)
                products = products.Where(p => p.Style == SelectedStyle);

            // Set the products
            Products = products;

            // Sets the default rating range
            double min = 1.0;
            double max = 5.0;

            // Parse the rating range if needed
            if (double.TryParse(Request.Query["MinRating"], out var parsedMin))
            {
                min = parsedMin;
            }

            if (double.TryParse(Request.Query["MaxRating"], out var parsedMax))
            {
                max = parsedMax;
            }

            // Apply filter for rating range
            Products = products.Where(p => (p.Ratings?.FirstOrDefault() ?? 0) >= min && (p.Ratings?.FirstOrDefault() ?? 0) <= max).ToList();

        }
    }
}
