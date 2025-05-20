using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace RamenRatings.WebSite.Pages
{
    public class AboutModel : PageModel
    {

        // Logger variable
        private readonly ILogger<AboutModel> _logger;

        // Constructor that accepts logger as input
        public AboutModel(ILogger<AboutModel> logger)
        {
            _logger = logger;
        }
        // Handle get when the page is accessed
        public void OnGet()
        {

        }
    }
}
