namespace Domain.Models.DTO.User
{
    public class UserInfo
    {
        public required ulong Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string FullName { get; set; }
        public required int Role { get; set; }
    }
}