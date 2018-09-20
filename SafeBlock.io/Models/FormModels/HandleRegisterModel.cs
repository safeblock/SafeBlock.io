using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SafeBlock.io.Models
{
    public class HandleRegisterModel
    {
        [Required(ErrorMessage = "The email address is required.")]
        [EmailAddress(ErrorMessage =  "This is not a valid email address.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "The password is required.")]
        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 7, ErrorMessage = "The length requirements is not satisfied.")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "The verification is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The verification password is incorrect.")]
        public string VerifyPassword { get; set; }
    }
}