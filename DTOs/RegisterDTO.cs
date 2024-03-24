using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace projectUsers.DTOs
{
    public class RegisterDTO
    {
       // [Required]
        public string Name { get; set; }
       // [Required]
        public string Username { get; set; }
      //  [Required]
        //[RegularExpression(@"^.*(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*\(\)_\-+=]).*$", ErrorMessage = "Invalid Password")]
    //    [StringLength(50, MinimumLength = 9, ErrorMessage = "Minimum Length is 9 !!")]
        public string Password { get; set; }

        //[Compare("Password")]
        //[NotMapped]
        //public string? ConfirmPassword { get; set; }
      //  [Required]
        [EmailAddress]
    //    [Length(10, 50)]
        public string Email { get; set; }
        //[Compare("Email")]
        //[NotMapped]
        //public string? ConfirmEmail { get; set; }
     //   [Required]
    //    [Range(15, 100)]//, ErrorMessage = "Age should be more than 15 Years and less than 100 ")]
        public int Age { get; set; }

     //   [Required]
        public string Role { get; set; }


        //[JsonIgnore]
        //public string PasswordConfirmed { get; set; }
        //public string Address { get; set; }
        //public int Phone { get; set; }
        //public string firstName { get; set; }
        //public string lastName { get; set; }


    }
}
