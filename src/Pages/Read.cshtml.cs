using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Services;
using RamenRatings.WebSite.Models;


namespace RamenRatings.WebSite.Pages
{
    public class ReadModel : PageModel
    {
        public ReadModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }

        public ProductModel Product;
        public IActionResult OnGet(string number)
        {
            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }

            return Page();
        }
    }
}
