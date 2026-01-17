using AutoMapper;
using Domain.Entities;
using Domain.Models.Common;
using Domain.Models.DTO.Loan;

namespace Infrastructure.Extensions.Mapping
{
    public class LoanMapping : Profile
    {
        public LoanMapping()
        {
            CreateMap<Loan, Response<GetLoanInfoResponse>>().ReverseMap();
        }
    }
}