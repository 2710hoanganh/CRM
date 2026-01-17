using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.UserReference;
using Application.Repositories;
using Application.Repositories.Base;
using Domain.Constants.AppEnum;
namespace Application.Features.UserReference.Query
{
    public class GetUserReferenceQuery : BasePaginationQuery, IRequest<Paged<List<GetUserReferenceResponse>>>
    {
        public class GetUserReferenceHandler : IRequestHandler<GetUserReferenceQuery, Paged<List<GetUserReferenceResponse>>>
        {
            private readonly IUserReferenceRepository _userReferenceRepository;
            private readonly IAutoMapper _autoMapper;
            public GetUserReferenceHandler(IUserReferenceRepository userReferenceRepository, IAutoMapper autoMapper)
            {
                _userReferenceRepository = userReferenceRepository;
                _autoMapper = autoMapper;
            }

            public async Task<Paged<List<GetUserReferenceResponse>>> Handle(GetUserReferenceQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var paged = await _userReferenceRepository.GetPagination(x => x.UserId == request.Id,
                                                                                    null,
                                                                                    null,
                                                                                    request.PageNumber,
                                                                                    request.PageSize,
                                                                                    cancellationToken);

                    var userReferences = paged.Data.Select(x => _autoMapper.Map<GetUserReferenceResponse>(x)).ToList();
                    return new Paged<List<GetUserReferenceResponse>>(userReferences, request.PageNumber, request.PageSize, paged.TotalRecords, paged.Message);
                }
                catch (Exception)
                {
                    return new Paged<List<GetUserReferenceResponse>>(new List<GetUserReferenceResponse>(), request.PageNumber, request.PageSize, 0, ResponseResult.ERROR.ToString());
                }
            }
        }
    }
}