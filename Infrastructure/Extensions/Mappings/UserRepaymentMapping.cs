using AutoMapper;
using Domain.Entities;
using Domain.Models.DTO.UserRepayment;

namespace Infrastructure.Extensions.Mapping
{
    public class UserRepaymentMapping : Profile
    {
        public UserRepaymentMapping()
        {
            CreateMap<UserRepayment, UserRepaymentDateResponse>().
            ForMember(dest => dest.RepaymentDate, opt => opt.MapFrom(src => src.RepaymentDate)).
            ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)).
            ReverseMap();
        }
    }
}