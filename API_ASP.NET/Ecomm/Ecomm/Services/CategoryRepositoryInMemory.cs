using Ecomm.Models;
using Ecomm.ViewModels;

namespace Ecomm.Services
{
    public class CategoryRepositoryInMemory : ICategoryRepository
    {
        static List<CategoryViewModel> categories = new List<CategoryViewModel>
        {
            new CategoryViewModel{CategoryID = 5, CategoryName = "A"},
            new CategoryViewModel{CategoryID = 6, CategoryName = "B"},
            new CategoryViewModel{CategoryID = 7, CategoryName = "C"},
            new CategoryViewModel{CategoryID = 8, CategoryName = "D"},
            new CategoryViewModel{CategoryID = 9, CategoryName = "E"},
        };
        public CategoryViewModel Add(CategoryModel categoryModel)
        {
            var _category = new CategoryViewModel
            {
                CategoryID = categories.Max(c => c.CategoryID) + 1,
                CategoryName = categoryModel.CategoryName
            };
            categories.Add(_category);
            return _category;
        }

        public void Delete(int id)
        {
            var _category = categories.SingleOrDefault(c => c.CategoryID == id);
            categories.Remove(_category);
        }

        public List<CategoryViewModel> GetAll()
        {
            return categories;
        }

        public CategoryViewModel GetById(int id)
        {
            return categories.SingleOrDefault(c => c.CategoryID == id);
        }

        public void Update(CategoryViewModel category)
        {
            var _category = categories.SingleOrDefault(c => c.CategoryID == category.CategoryID);
            if(_category != null)
            {
                _category.CategoryName = category.CategoryName;
            }
        }
    }
}
