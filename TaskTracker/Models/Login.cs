using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker.Models
{
    public class Login
    {
        [Required(ErrorMessage ="Enter email Id")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter your password")]        
        public string Password { get; set; }
    }
}