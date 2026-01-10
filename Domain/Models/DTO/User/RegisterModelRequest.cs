using System.ComponentModel.DataAnnotations;
namespace Domain.Models.DTO.User
{
    public class RegisterModelRequest
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")]
        public required string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password and ConfirmPassword must match")]
        public required string ConfirmPassword { get; set; }
    }
}