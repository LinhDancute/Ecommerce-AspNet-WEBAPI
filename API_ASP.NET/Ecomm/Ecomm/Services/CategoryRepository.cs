// CategoryRepository.cs
using Ecomm.Data;
using Ecomm.Models;
using Ecomm.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecomm.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyDBContext _context;

        public CategoryRepository(MyDBContext context)
        {
            _context = context;
        }

        public List<CategoryViewModel> GetAll()
        {
            var categories = _context.Categories.Select(c => new CategoryViewModel
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
            });
            return categories.ToList();
            /*return _context.Categories
                .Select(c => new CategoryViewModel
                {
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName
                })
                .ToList();*/
        }

        public CategoryViewModel GetById(int id)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryID == id);
            if (category != null) {
                return new CategoryViewModel { 
                    CategoryID = category.CategoryID,
                    CategoryName = category.CategoryName,
                };
            }
            /*CategoryData category = _context.Categories
                .FirstOrDefault(c => c.CategoryID == id);

            if (category != null)
            {
                return new CategoryViewModel
                {
                    CategoryID = category.CategoryID,
                    CategoryName = category.CategoryName
                };
            }*/

            return null;
        }

        public CategoryViewModel Add(CategoryModel categoryModel)
        {
            var _category = new CategoryData
            {
                CategoryName = categoryModel.CategoryName,
            };

            _context.Add(_category);
            _context.SaveChanges();
            return new CategoryViewModel
            {
                CategoryID= _category.CategoryID,
                CategoryName= _category.CategoryName,
            };

            /*CategoryData category = new CategoryData
            {
                CategoryName = categoryModel.CategoryName
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            return new CategoryViewModel
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName
            };*/
        }

        public void Update(CategoryViewModel categoryModel)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryID == categoryModel.CategoryID);
            category.CategoryName = categoryModel.CategoryName;
            _context.SaveChanges();
            /*
            CategoryData category = _context.Categories
                .FirstOrDefault(c => c.CategoryID == categoryModel.CategoryID);

            if (category != null)
            {
                category.CategoryName = categoryModel.CategoryName;
                _context.SaveChanges();
            }
            */
        }

        public void Delete(int id)
        {
            var category = _context.Categories.SingleOrDefault(c => c.CategoryID == id);
            if (category != null)
            {
                _context.Remove(category);
                _context.SaveChanges();
            }
            /*
            CategoryData category = _context.Categories
                .FirstOrDefault(c => c.CategoryID == id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }*/
        }
    }
}
