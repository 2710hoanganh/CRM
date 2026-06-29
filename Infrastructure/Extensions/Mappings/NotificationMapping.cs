using AutoMapper;
using Domain.Entities;
using Domain.Models.DTO.Notification;
namespace Infrastructure.Extensions.Mappings
{
    public class NotificationMapping : Profile
    {
        public NotificationMapping() => CreateMap<Notification, NotficationList>().ReverseMap();
    }
}