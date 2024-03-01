using System.ComponentModel.DataAnnotations;

namespace Ecomm.Models
{
    public class CategoryModel
    {
        [Required]
        [MaxLength(50)]
        public string? CategoryName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }
        public int CategoryID { get; internal set; }
    }
}

