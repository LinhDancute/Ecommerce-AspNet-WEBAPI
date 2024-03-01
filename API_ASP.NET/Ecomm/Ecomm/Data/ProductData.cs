using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ecomm.Data
{
    [Table("Product")]
    public class ProductData
    {
        [Key]
        public Guid ProductID { get; set; }
        public int CategoryID { get; set; }

        [Required(ErrorMessage ="Name cannot empty")]
        public string ProductName { get; set; }

        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        
        //relationship
        public ICollection<OrderDetailsData> OrderDetail { get; set; }
        [JsonIgnore]
        public CategoryData Category { get; set; }

        public ProductData()
        {
            OrderDetail = new HashSet<OrderDetailsData>();
        }
    }
}
