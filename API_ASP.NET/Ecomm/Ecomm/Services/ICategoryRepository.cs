// ICategoryRepository.cs
using Ecomm.Models;
using Ecomm.ViewModels;
using System.Collections.Generic;

namespace Ecomm.Services
{
    public interface ICategoryRepository
    {
        List<CategoryViewModel> GetAll();
        CategoryViewModel GetById(int id);
        CategoryViewModel Add(CategoryModel categoryModel);
        void Update(CategoryViewModel category);
        void Delete(int id);
    }
}
