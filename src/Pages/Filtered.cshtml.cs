using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Pages
{
    public class FilteredModel : PageModel
    {
        private readonly JsonFileProductService ProductService;

        public FilteredModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public IEnumerable<ProductModel> Products { get; set; }

        public List<string> AllBrands { get; set; }
        public List<string> AllStyles { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedStyle { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<string> SelectedBrands { get; set; }

        public void OnGet()
        {
            var products = ProductService.GetProducts();

            AllBrands = products.Select(p => p.Brand).Where(b => (string.IsNullOrEmpty(b)) == false).Distinct().OrderBy(b => b).ToList();
            AllStyles = products.Select(p => p.Style).Where(s => (string.IsNullOrEmpty(s)) == false).Distinct().OrderBy(s => s).ToList();

            if (SelectedBrands?.Any() == true)
                products = products.Where(p => SelectedBrands.Contains(p.Brand));

            if (string.IsNullOrEmpty(SelectedStyle) == false)
                products = products.Where(p => p.Style == SelectedStyle);

            Products = products;
        }
    }
}
