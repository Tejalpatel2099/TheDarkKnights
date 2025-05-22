using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;
using System.Collections.Generic;
using System.Linq;

namespace RamenRatings.WebSite.Pages.Product
{
    /// <summary>
    /// Page model for comparing a ramen product with another ramen product,
    /// displaying ramen options in a dropdown and displaying information for each ramen on one page.
    /// </summary>
    public class CompareModel : PageModel
    {
        // Service used to retrieve ramen product data
        private JsonFileProductService ProductService;

        // Error message to display if the user selects the same ramen product for comparison
        public string ErrorMessage { get; set; } = "";

        // Constructor that initializes the product service 
        public CompareModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        // Creates list of ramen products
        public List<ProductModel> Products { get; set; }

        // First selected ramen product number for onpost
        [BindProperty]
        public int Selected1 { get; set; }

        // Second selected ramen product number for onpost
        [BindProperty]
        public int Selected2 { get; set; }

        // Product information for the first ramen product
        public ProductModel Ramen1 { get; set; }

        // Product information for second ramen product
        public ProductModel Ramen2 { get; set; }


        // handles the get request when the page is being accessed
        public void OnGet()
        {
            Products = ProductService.GetProducts().ToList();
        }

        // handles the post request when the page is finished being accessed
        public void OnPost()
        {
            Products = ProductService.GetProducts().ToList();
            // Check if the selected ramen products are the same
            if (Selected1 == Selected2)
            {
                ErrorMessage = "Please select two different ramens.";
                return;
            }
            Ramen1 = Products.FirstOrDefault(p => p.Number == Selected1);
            Ramen2 = Products.FirstOrDefault(p => p.Number == Selected2);
        }
    }
}
