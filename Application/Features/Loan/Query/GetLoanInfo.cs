using MediatR;
using Domain.Models.Common;
using Application.Repositories;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.Loan;
using Application.Repositories.Base;

namespace Application.Features.Loan.Query
{
    public class GetLoanInfoQuery : BaseFields, IRequest<Response<GetLoanInfoResponse>>
    {
        public class GetLoanInfoQueryHandler : IRequestHandler<GetLoanInfoQuery, Response<GetLoanInfoResponse>>
        {
            private readonly ILoanRepository _loanRepository;
            private readonly IAutoMapper _autoMapper;
            public GetLoanInfoQueryHandler(ILoanRepository loanRepository, IAutoMapper autoMapper)
            {
                _loanRepository = loanRepository;
                _autoMapper = autoMapper;
            }
            public async Task<Response<GetLoanInfoResponse>> Handle(GetLoanInfoQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var loan = await _loanRepository.GetOne(x => x.Id == request.Id, null, null, cancellationToken);
                    if (loan == null)
                    {
                        return new Response<GetLoanInfoResponse>(ResponseResult.ERROR, "Loan not found", null, null);
                    }
                    return new Response<GetLoanInfoResponse>(ResponseResult.SUCCESS, "Loan found", _autoMapper.Map<GetLoanInfoResponse>(loan), null);
                }
                catch (Exception ex)
                {
                    return new Response<GetLoanInfoResponse>(ResponseResult.ERROR, ex.Message, null, null);
                }
            }
        }
    }
}