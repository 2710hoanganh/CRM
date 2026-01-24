using MediatR;
using Domain.Models.Common;
using Application.Repositories.Base;
using Domain.Constants.AppEnum;
using Domain.Models.DTO.UserRepayment;
using Application.Repositories;

namespace Application.Features.Loan.Query
{
    public class GetLoanRepaymentDateQuery : BaseFields, IRequest<Response<UserListRepayment>>
    {
        public class GetLoanRepaymentDateQueryHandler : IRequestHandler<GetLoanRepaymentDateQuery, Response<UserListRepayment>>
        {
            private readonly IUserRepaymentRepository _userRepaymentRepository;
            private readonly ILoanRepository _loanRepository;
            private readonly IAutoMapper _autoMapper;
            public GetLoanRepaymentDateQueryHandler(IUserRepaymentRepository userRepaymentRepository, ILoanRepository loanRepository, IAutoMapper autoMapper)
            {
                _userRepaymentRepository = userRepaymentRepository;
                _loanRepository = loanRepository;
                _autoMapper = autoMapper;
            }
            public async Task<Response<UserListRepayment>> Handle(GetLoanRepaymentDateQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var loan = await _loanRepository.GetOne(x => x.Id == request.Id, null, null, cancellationToken);
                    if (loan == null)
                    {
                        return new Response<UserListRepayment>(ResponseResult.NOT_FOUND, "Loan not found", null, null);
                    }
                    var userRepayments = _userRepaymentRepository.Get(
                        x => x.LoanId == request.Id,
                        orderBy: x => x.OrderByDescending(y => y.RepaymentDate),
                        null, 0, 0).ToList();

                    return new Response<UserListRepayment>(ResponseResult.SUCCESS, "Loan repayment dates retrieved successfully", new UserListRepayment
                    {
                        Amount = loan.Amount,
                        RepaymentDates = _autoMapper.Map<List<UserRepaymentDateResponse>>(userRepayments),
                    }, null);
                }
                catch (Exception ex)
                {
                    return new Response<UserListRepayment>(ResponseResult.ERROR, ex.Message, null, null);
                }
            }
        }
    }
}