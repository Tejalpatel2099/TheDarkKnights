using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RamenRatings.WebSite.Services;
using RamenRatings.WebSite.Models;


namespace RamenRatings.WebSite.Pages
{
    public class ReadModel : PageModel
    {

        //read model constructor
        public ReadModel(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        //Product service
        public JsonFileProductService ProductService { get; }

        //Product that will be used for page
        public ProductModel Product;

        // runs upon the page loading
        public IActionResult OnGet(int number)
        {
            //takes the index from last page and gets product using this
            Product = ProductService.GetProducts().FirstOrDefault(m => m.Number.Equals(number));
            //on case product isn't found return error
            if (Product == null)
            {
                TempData["ErrorMessage"] = "Something went wrong while fetching the product please retry";
                return RedirectToPage("../Error");
            }
            // Return to the page if the data is invalid
            return Page();
        }
    }
}
