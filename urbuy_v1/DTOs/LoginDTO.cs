using System.ComponentModel.DataAnnotations;

namespace urbuy_v1.DTOs
{
    public class LoginDTO
    {
        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }
    }
}
