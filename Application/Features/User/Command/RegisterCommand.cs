using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.User;
using Application.Repositories.Base;
using Application.Repositories;
using Domain.Constants.AppEnum;
namespace Application.Features.User.Command
{
    public class RegisterCommand : IRequest<Response<RegisterModelResponse>>
    {
        public required RegisterModelRequest Request { get; set; }

        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<RegisterModelResponse>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IUserRepository _userRepository;
            private readonly IAutoMapper _autoMapper;
            private readonly IHashPassword _hashPassword;
            public RegisterCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IAutoMapper autoMapper, IHashPassword hashPassword)
            {
                _unitOfWork = unitOfWork;
                _userRepository = userRepository;
                _autoMapper = autoMapper;
                _hashPassword = hashPassword;
            }
            public async Task<Response<RegisterModelResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var isExist = await _userRepository.Find(x => x.Email == request.Request.Email, null, false, cancellationToken);
                    if (isExist == true)
                    {
                        return new Response<RegisterModelResponse>(ResponseResult.ERROR, "Email already exists", null, null);
                    }

                    var user = new Domain.Entities.User
                    {
                        FullName = $"{request.Request.FirstName} {request.Request.LastName}",
                        Email = request.Request.Email,
                        FirstName = request.Request.FirstName,
                        LastName = request.Request.LastName,
                        PasswordHash = await _hashPassword.HashPassword(request.Request.Password),
                        Role = (int)Domain.Constants.AppEnum.Role.User,
                    };

                    var result = await _userRepository.Add(user, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return new Response<RegisterModelResponse>(ResponseResult.SUCCESS, "User registered successfully", _autoMapper.Map<RegisterModelResponse>(result), null);   
                }
                catch (Exception ex)
                {
                    return new Response<RegisterModelResponse>(ResponseResult.ERROR, ex.Message, null, null);
                }
            }
        }
    }
}