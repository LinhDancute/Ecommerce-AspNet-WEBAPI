namespace Ecomm.Models
{
    public class ProductVM
    {
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int CategoryID { get; set; } // Add the CategoryID property
    }

    public class Product : ProductVM
    {
        public Guid ProductID { get; set; }
    }

    public class ProductModel
    {
        public Guid ProductID { get; set;}
        public string ProductName { get; set; }
        public double Price { get; set; }
        public string CategoryName { get; set; }
    }
}
