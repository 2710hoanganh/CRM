using Domain.Entities.Base;
namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshTokenHash { get; set; }
        public int Role { get; set; }
        public int IdentifyNumber { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public int Gender { get; set; }
        public int Status { get; set; }

        public ICollection<UserReference> UserReferences { get; set; } = new List<UserReference>();
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}