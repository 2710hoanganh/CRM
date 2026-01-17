using AutoMapper;
using Domain.Entities;
using Domain.Models.DTO.UserReference;

namespace Infrastructure.Extensions.Mapping
{
    public class UserReferenceMapping : Profile
    {
        public UserReferenceMapping()
        {
            CreateMap<UserReference, GetUserReferenceResponse>().ReverseMap();
        }
    }
}