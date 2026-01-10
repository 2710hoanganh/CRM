using MediatR;  
using Domain.Models.Common;
using Domain.Models.DTO.User;
using Domain.Constants.AppEnum;
using Application.Repositories;
using Application.Repositories.Base;

namespace Application.Features.User.Query
{
    public class GetUserInfoQuery : IRequest<Response<UserInfo>>
    {
        public required GetInfoRequest Request { get; set; }
        public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Response<UserInfo>>
        {

            private readonly IUserRepository _userRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IAutoMapper _autoMapper;
            public GetUserInfoQueryHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, IAutoMapper autoMapper)
            {
                _userRepository = userRepository;
                _unitOfWork = unitOfWork;
                _autoMapper = autoMapper;
            }
            public async Task<Response<UserInfo>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userRepository.GetOne(x => x.Id == (long)request.Request.Id, null, null, cancellationToken);
                    if (user == null)
                    {
                        return new Response<UserInfo>(ResponseResult.ERROR, "Get user info failed", null, null);
                    }
                    return new Response<UserInfo>(ResponseResult.SUCCESS, "Get user info successful", _autoMapper.Map<UserInfo>(user), null);
                }
                catch (Exception ex)
                {
                    return new Response<UserInfo>(ResponseResult.ERROR, ex.Message, null, null);
                }
            }
        }
    }
}