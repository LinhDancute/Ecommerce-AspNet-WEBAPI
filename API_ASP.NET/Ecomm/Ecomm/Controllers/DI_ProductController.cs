using Ecomm.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DI_ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public DI_ProductController(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetAllProducts(string search, double? from, double? to, string sortBy, int page = 1)
        {
            try
            {
                var result = _productRepository.GetAll(search, from, to, sortBy, page);
                return Ok(result);
            }
            catch
            {
                return BadRequest("We can't get the product");
            }
        }
            
    }
}
