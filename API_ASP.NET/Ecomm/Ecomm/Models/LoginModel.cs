using System.ComponentModel.DataAnnotations;

namespace Ecomm.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength (250)]
        public string Password { get; set; }
    }
}
