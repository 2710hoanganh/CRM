using Domain.Entities;
namespace Application.Repositories.Base
{
    public interface ITokenService
    {
        Task<string> GenerateAccessToken(User user);
        Task<string> GenerateRefreshToken(User user);
    }
}