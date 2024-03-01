using Ecomm.Data;
using Ecomm.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly MyDBContext _context;

        public CategoryController(MyDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var listOfCategories = _context.Categories.ToList();
            return Ok(listOfCategories);
        }

        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryID == id);
            if (category != null)
            {
                return Ok(category);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult CreateNew(CategoryModel categoryModel)
        {
            var categoryData = new CategoryData
            {
                CategoryName = categoryModel.CategoryName,
                Description = categoryModel.Description  // Assign the value of the Description property
            };

            _context.Categories.Add(categoryData);
            _context.SaveChanges();

            return Ok(categoryData);
        }


        [HttpPut("{id}")]
        public IActionResult UpdateCategoryByID(int id, CategoryModel categoryModel)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryID == id);
            if (category != null)
            {
                category.CategoryName = categoryModel.CategoryName;
                _context.SaveChanges();

                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveCategoryByID(int id)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryID == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();

                return Ok(category);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
