using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using RamenRatings.WebSite.Models;
using RamenRatings.WebSite.Services;

namespace RamenRatings.WebSite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        public ProductsController(JsonFileProductService productService)
        {
            ProductService = productService;
        }

        public JsonFileProductService ProductService { get; }

        [HttpGet]
        public IEnumerable<ProductModel> Get()
        {
            return ProductService.GetProducts();
        }

        [HttpPatch]
        public ActionResult Patch([FromBody] RatingRequest request)
        {
            ProductService.AddRating(request.ProductId, request.Rating);
            
            return Ok();
        }

        public class RatingRequest
        {
            public int ProductId { get; set; }
            public int Rating { get; set; }
        }
    }
}