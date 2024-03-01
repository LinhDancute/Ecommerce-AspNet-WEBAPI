namespace Ecomm.Data
{
    public enum OrderStatus
    {
        New = 0, Payment = 1, Complete = 2, Cancel = -1
    }
    public class OrderData
    {
        public Guid OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public OrderStatus Status { get; set; }

        //relationship

        public ICollection<OrderDetailsData> OrderDetail { get; set; }

        public OrderData()
        {
            OrderDetail = new List<OrderDetailsData>();
        }

    }
}
