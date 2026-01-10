using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.User;
using Domain.Constants.AppEnum;
using Application.Repositories;
using Application.Repositories.Base;
using Microsoft.Extensions.Options;
namespace Application.Features.User.Query
{
    public class LoginQuery : IRequest<Response<LoginResponse>>
    {
        public required LoginRequest Request { get; set; }

        public class LoginQueryHandler : IRequestHandler<LoginQuery, Response<LoginResponse>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IHashPassword _hashPassword;
            private readonly IAutoMapper _autoMapper;
            private readonly ITokenService _tokenService;
            private readonly IOptions<JWT> _jwtSettings;
            public LoginQueryHandler(IUserRepository userRepository, IHashPassword hashPassword, IAutoMapper autoMapper, ITokenService tokenService, IOptions<JWT> jwtSettings, IUnitOfWork unitOfWork)
            {
                _userRepository = userRepository;
                _hashPassword = hashPassword;
                _autoMapper = autoMapper;
                _tokenService = tokenService;
                _jwtSettings = jwtSettings;
                _unitOfWork = unitOfWork;
            }
            public async Task<Response<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    string accessToken = string.Empty;
                    string refreshToken = string.Empty;
                    var user = await _userRepository.GetOne(x => x.Email == request.Request.Email, null, null, cancellationToken);

                    if (user == null)
                    {
                        return new Response<LoginResponse>(ResponseResult.ERROR, "Email or password is incorrect", null, null);
                    }

                    if (await _hashPassword.VerifyPassword(request.Request.Password, user.PasswordHash) == false)
                    {
                        return new Response<LoginResponse>(ResponseResult.ERROR, "Invalid password", null, null);
                    }

                    // current flow refresh token is not expired
                    accessToken = await _tokenService.GenerateAccessToken(user);
                    refreshToken = await _tokenService.GenerateRefreshToken(user);

                    var hashedRefreshToken = await _hashPassword.HashPassword(refreshToken);
                    await _userRepository.ExcuteUpdate(x => x.Id == user.Id, x =>
                    {
                        x.RefreshTokenHash = hashedRefreshToken;
                    }, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return new Response<LoginResponse>(ResponseResult.SUCCESS, "Login successful", new LoginResponse
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenType = "Bearer",
                        ExpiresIn = _jwtSettings.Value.ExpiresIn,
                        UserInfo = _autoMapper.Map<UserInfo>(user)
                    }, null);
                }
                catch (Exception ex)
                {
                    return new Response<LoginResponse>(ResponseResult.ERROR, ex.Message, null, null);
                }
            }
        }
    }
}