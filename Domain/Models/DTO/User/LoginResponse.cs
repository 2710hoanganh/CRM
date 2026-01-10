namespace Domain.Models.DTO.User
{
    public class LoginResponse
    {
        public required string AccessToken { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public required string TokenType { get; set; }
        public required int ExpiresIn { get; set; }
        public required UserInfo UserInfo { get; set; }
    }
}