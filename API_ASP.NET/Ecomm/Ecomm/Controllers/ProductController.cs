using Ecomm.Data;
using Ecomm.Models;
using Ecomm.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MyDBContext _context;

        public ProductController(MyDBContext context)
        {
            _context = context;
        }

        

        
        /*
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.Include(c => c.Product).ToList();

            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Products = c.Product.Select(p => new ProductViewModel
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Price = p.Price
                }).ToList()
            }).ToList();

            return Ok(categoryViewModels);
        }
        */

        [HttpGet]
        public IActionResult GetAll()
        {
            var ListofProducts = _context.Products.ToList();
            return Ok(ListofProducts);
        }

        [HttpGet("{id}")]
        public IActionResult GetByID(string id)
        {
            var product = _context.Products.SingleOrDefault(p => p.ProductID == Guid.Parse(id));
            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult CreateNew(ProductVM productModel)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryID == productModel.CategoryID);
            
            var product = new ProductData
            {
                ProductID = Guid.NewGuid(),
                ProductName = productModel.ProductName,
                Price = productModel.Price,
                CategoryID = productModel.CategoryID
            };

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProductByID(string id, Product productModel)
        {
            var product = _context.Products.SingleOrDefault(p => p.ProductID == Guid.Parse(id));
            if (product != null)
            {
                product.ProductName = productModel.ProductName;
                product.Price = productModel.Price;
                product.CategoryID = productModel.CategoryID;
                _context.SaveChanges();

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveProductByID(string id)
        {
            var product = _context.Products.SingleOrDefault(p => p.ProductID == Guid.Parse(id));
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();

                return Ok(product);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
