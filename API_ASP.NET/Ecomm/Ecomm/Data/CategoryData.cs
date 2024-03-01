using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Ecomm.Data
{
    [Table("Category")]
    public class CategoryData
    {
        [Key]
        public int CategoryID { get; set; }
        [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }


        //relationship
        public ICollection<ProductData> Product{ get; set; }




    }
}
