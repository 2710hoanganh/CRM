namespace Domain.Models.DTO.User
{
    public class RegisterModelResponse
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
    }
}