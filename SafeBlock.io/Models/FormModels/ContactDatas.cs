using System;
using System.ComponentModel.DataAnnotations;

namespace SafeBlock.io.Models
{
    public class ContactDatas
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
