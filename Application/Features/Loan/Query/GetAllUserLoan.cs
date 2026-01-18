using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.Loan;
using Application.Repositories;
using Application.Repositories.Base;
using Domain.Constants.AppEnum;

namespace Application.Features.Loan.Query
{
    public class GetAllUserLoanQuery : BasePaginationQuery, IRequest<Paged<List<ListLoanResponse>>>
    {
        public class GetAllUserLoanQueryHandler : IRequestHandler<GetAllUserLoanQuery, Paged<List<ListLoanResponse>>>
        {
            private readonly ILoanRepository _loanRepository;
            private readonly IAutoMapper _autoMapper;
            public GetAllUserLoanQueryHandler(ILoanRepository loanRepository, IAutoMapper autoMapper)
            {
                _loanRepository = loanRepository;
                _autoMapper = autoMapper;
            }
            public async Task<Paged<List<ListLoanResponse>>> Handle(GetAllUserLoanQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var paged = await _loanRepository.GetPaginationWithUser(x => x.UserId == request.Id, null, x => _autoMapper.Map<ListLoanResponse>(x), request.PageNumber, request.PageSize, cancellationToken);
                    var loans = paged.Data.Select(x => _autoMapper.Map<ListLoanResponse>(x)).ToList();
                    return new Paged<List<ListLoanResponse>>(loans, request.PageNumber, request.PageSize, paged.TotalRecords, paged.Message);
                }
                catch (Exception)
                {
                    return new Paged<List<ListLoanResponse>>(new List<ListLoanResponse>(), request.PageNumber, request.PageSize, 0, ResponseResult.ERROR.ToString());
                }
            }
        }
    }
}