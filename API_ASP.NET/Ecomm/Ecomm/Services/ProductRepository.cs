using Ecomm.Data;
using Ecomm.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecomm.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyDBContext _context;
        public static int pageSize { get; set; } = 5;

        public ProductRepository(MyDBContext context) {
            _context = context;
        }
        public List<ProductModel> GetAll(string search, double? from, double? to, string sortBy, int page = 1)
        {
            var allProducts = _context.Products.Include(p => p.Category).AsQueryable();

            #region Filtering
            if (!string.IsNullOrEmpty(search))
            {
                allProducts = allProducts.Where(p => p.ProductName.Contains(search));
            }

            if (from.HasValue)
            {
                allProducts = allProducts.Where(p => p.Price >= from);
            }

            if (to.HasValue)
            {
                allProducts = allProducts.Where(p => p.Price <= from);
            }
            #endregion

            #region Sorting
            //Default sort by Name (ProductName)
            allProducts = allProducts.OrderBy(p => p.ProductName);

            if(!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "ProductName_Desc": allProducts = allProducts.OrderByDescending(p => p.ProductName);
                        break;
                    case "Price_Asc": allProducts = allProducts.OrderBy(p => p.Price);
                        break;
                    case "Price_Desc": allProducts = allProducts.OrderByDescending(p => p.Price);
                        break;
                }
            }
            #endregion

            /*
            #region Paging
            allProducts = allProducts.Skip((page-1)*pageSize).Take(pageSize);
            #endregion
            var result = allProducts.Select(p => new ProductModel
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price,
                CategoryName = p.Category.CategoryName
            });

            return result.ToList();
            */

            var result = PaginatedList<ProductData>.Create(allProducts, page, pageSize);
            return result.Select(p => new ProductModel
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Price = p.Price,
                CategoryName = p.Category?.CategoryName

            }).ToList();
           
        }
    }
}
