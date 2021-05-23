using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string password { get; set; }
    }
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string password { get; set; }
    }

    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
    }
}