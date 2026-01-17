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

            CreateMap<Loan, ListLoanResponse>().
            ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName)).
            ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id)).
            ReverseMap();
        }
    }
}