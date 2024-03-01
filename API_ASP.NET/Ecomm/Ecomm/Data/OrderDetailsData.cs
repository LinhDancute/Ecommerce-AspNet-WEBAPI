using System.ComponentModel.DataAnnotations.Schema;

namespace Ecomm.Data
{
    public class OrderDetailsData
    {
        public Guid OrderID { get; set; }
        public Guid ProductID { get; set; }
        
        public double Price { get; set; }
        public int Quantity { get; set; }
        public byte Discount { get; set; }

        //relationship
        public required OrderData Order { get; set; }
        public required ProductData Product { get; set; }


    }
}
