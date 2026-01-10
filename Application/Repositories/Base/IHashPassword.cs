namespace Application.Repositories.Base
{
    public interface IHashPassword
    {
        Task<string> HashPassword(string password);
        Task<bool> VerifyPassword(string password, string hashedPassword);
    }
}