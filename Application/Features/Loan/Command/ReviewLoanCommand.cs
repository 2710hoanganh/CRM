using MediatR;
using Domain.Models.Common;
using Application.Repositories;
using Application.Repositories.Base;
using Domain.Constants.AppEnum;

namespace Application.Features.Loan.Command
{
    public class ReviewLoanCommand : IRequest<Response<bool>>
    {
        public required int Id { get; set; }
        public required string FeedBack { get; set; }
        public required LoanStatus Status { get; set; }

        public class ReviewLoanCommandHandler : IRequestHandler<ReviewLoanCommand, Response<bool>>
        {
            private readonly ILoanRepository _loanRepository;
            private readonly IUnitOfWork _unitOfWork;
            public ReviewLoanCommandHandler(ILoanRepository loanRepository, IUnitOfWork unitOfWork)
            {
                _loanRepository = loanRepository;
                _unitOfWork = unitOfWork;
            }
            public async Task<Response<bool>> Handle(ReviewLoanCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    await _loanRepository.ExcuteUpdate(x => x.Id == request.Id, x =>
                    {
                        x.Status = (int)request.Status;
                        x.FeedBack = request.FeedBack;
                    }, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return new Response<bool>(ResponseResult.SUCCESS, "Loan reviewed successfully", true, null);
                }
                catch (Exception ex)
                {
                    return new Response<bool>(ResponseResult.ERROR, ex.Message, false, null);
                }
            }
        }
    }
}