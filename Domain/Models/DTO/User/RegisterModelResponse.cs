namespace Domain.Models.DTO.User
{
    public class RegisterModelResponse
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
    }
}