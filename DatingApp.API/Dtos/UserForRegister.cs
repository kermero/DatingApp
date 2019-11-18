using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegister
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(8,MinimumLength = 3, ErrorMessage = "You must specify password between 4 and 8 characters")]
        public string Password { get; set; }
    }
}