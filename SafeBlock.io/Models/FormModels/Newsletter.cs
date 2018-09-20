using System.ComponentModel.DataAnnotations;

namespace SafeBlock.io.Models
{
    public class Newsletter
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}