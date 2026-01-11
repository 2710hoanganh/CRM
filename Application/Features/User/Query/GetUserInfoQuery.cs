using MediatR;  
using Domain.Models.Common;
using Domain.Models.DTO.User;
using Domain.Constants.AppEnum;
using Application.Repositories;
using Application.Repositories.Base;

namespace Application.Features.User.Query
{
    public class GetUserInfoQuery : BaseFields, IRequest<Response<UserInfo>>
    {
        public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, Response<UserInfo>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IAutoMapper _autoMapper;
            public GetUserInfoQueryHandler(IUserRepository userRepository, IAutoMapper autoMapper)
            {
                _userRepository = userRepository;
                _autoMapper = autoMapper;
            }
            public async Task<Response<UserInfo>> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userRepository.GetOne(x => x.Id == request.Id, null, null, cancellationToken);
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