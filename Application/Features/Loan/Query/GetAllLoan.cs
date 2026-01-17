using MediatR;
using Domain.Models.Common;
using Domain.Models.DTO.Loan;
using Application.Repositories.Base;
using Application.Repositories;
using Domain.Constants.AppEnum;

namespace Application.Features.Loan.Query
{
    public class GetAllLoanQuery : BasePaginationQuery, IRequest<Paged<List<ListLoanResponse>>>
    {
        public class GetAllLoanQueryHandler : IRequestHandler<GetAllLoanQuery, Paged<List<ListLoanResponse>>>
        {
            private readonly ILoanRepository _loanRepository;
            private readonly IAutoMapper _autoMapper;
            public GetAllLoanQueryHandler(ILoanRepository loanRepository, IAutoMapper autoMapper)
            {
                _loanRepository = loanRepository;
                _autoMapper = autoMapper;
            }
            public async Task<Paged<List<ListLoanResponse>>> Handle(GetAllLoanQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    // Sử dụng method GetPaginationWithUser - includes được xử lý trong Persistence layer
                    var paged = await _loanRepository.GetPaginationWithUser(
                        filter: null,
                        orderBy: null,
                        selector: x => _autoMapper.Map<ListLoanResponse>(x),
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize,
                        cancellationToken: cancellationToken);

                    var loans = paged.Data.ToList();
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