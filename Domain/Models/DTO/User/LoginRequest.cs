using System.ComponentModel.DataAnnotations;

namespace Domain.Models.DTO.User
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
        public required string Password { get; set; }
    }
}